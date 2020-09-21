/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Enums;
using Microsoft.Extensions.Logging;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework.Exceptions;
using PTV.Domain.Model.Models.OpenApi.V11;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof (IServiceAndChannelService), RegisterType.Transient)]
    internal class ServiceAndChannelService : ServiceBase, IServiceAndChannelService
    {
        private readonly IContextManager contextManager;
        private ILogger logger;
        private IServiceUtilities utilities;
        private IServiceService serviceService;
        private IChannelService channelService;
        private ITypesCache typesCache;
        private IAddressService addressService;
        private IPostalCodeCache postalCodeCache;
        private readonly IUrlService urlService;

        public ServiceAndChannelService(IContextManager contextManager,
                                       ITranslationEntity translationEntToVm,
                                       ITranslationViewModel translationVmtoEnt,
                                       ILogger<ServiceAndChannelService> logger,
                                       IServiceUtilities utilities,
                                       IServiceService serviceService,
                                       IChannelService channelService,
                                       IPublishingStatusCache publishingStatusCache,
                                       IUserOrganizationChecker userOrganizationChecker,
                                       IVersioningManager versioningManager,
                                       ITypesCache typesCache,
                                       IAddressService addressService,
                                       IPostalCodeCache postalCodeCache,
                                       IUrlService urlService
            )
            : base(translationEntToVm, translationVmtoEnt, publishingStatusCache, userOrganizationChecker, versioningManager)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.serviceService = serviceService;
            this.channelService = channelService;
            this.typesCache = typesCache;
            this.addressService = addressService;
            this.postalCodeCache = postalCodeCache;
            this.urlService = urlService;
        }

        #region Open api

        public IList<string> SaveServicesAndChannels(List<V11VmOpenApiServiceServiceChannelAstiInBase> serviceAndChannelRelations)
        {
            var list = new List<string>();
            if (serviceAndChannelRelations != null)
            {
                ProcessNewAddresses(serviceAndChannelRelations
                    .Where(x=>x?.ContactDetails?.Addresses!=null)
                    .SelectMany(relation => relation.ContactDetails.Addresses
                        .Where(adr=>adr.StreetAddress!=null)
                        .Select(x=>x.StreetAddress)));
                ProcessNewUrls(serviceAndChannelRelations.Where(x => x?.ContactDetails?.WebPages != null)
                    .SelectMany(x => x.ContactDetails.WebPages).Where(x => x != null).Select(x => x.Url));

                foreach (var service in serviceAndChannelRelations)
                {
                    try
                    {
                        list.Add(SaveServiceServiceChannel(service));
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                        list.Add(ex.Message);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Updates channel connections for a defined service. This is used for both ASTI and regular connections
        /// Regular users can update data within ASTI connections but they cannot remove any ASTI connections.
        /// ASTI users can remove and update ASTI connections - regular connections are not removed.
        /// More rules in https://confluence.csc.fi/display/PAL/ASTI-project and https://jira.csc.fi/browse/PTV-2065.
        /// </summary>
        /// <param name="relations">The connection model</param>
        /// <param name="openApiVersion">The open api version to be returned.</param>
        /// <param name="isASTI"></param>
        /// <returns>Updated service with connection information</returns>
        public IVmOpenApiServiceVersionBase SaveServiceConnections(V11VmOpenApiServiceAndChannelRelationAstiInBase relations, int openApiVersion, bool isASTI)
        {
            if (relations == null) return null;
            var rootID = relations.ServiceId.Value;
            
            ProcessConnectionsCommonLanguage(relations);
            var channelRelationsInputList = new List<V11VmOpenApiServiceServiceChannelAstiInBase>(relations.ChannelRelations);

            if (relations.DeleteAllChannelRelations && relations.ChannelRelations?.Count == 0)
            {
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var service = TranslationManagerToEntity.Translate<V11VmOpenApiServiceAndChannelRelationAstiInBase, Service>(relations, unitOfWork);
                    var updatedConnectionIds = channelRelationsInputList.Count > 0 ? channelRelationsInputList.Select(r => r.ChannelGuid).ToList() : new List<Guid>();
                    RemoveAstiConnections(isASTI, service, updatedConnectionIds, unitOfWork);
                    AddModifiedPropagationChain(unitOfWork);
                    unitOfWork.Save(SaveMode.Normal, PreSaveAction.DoNotCheckRoles);
                });
            }
            else
            {
                foreach (var channelRelation in channelRelationsInputList)
                {
                    relations.ChannelRelations = new List<V11VmOpenApiServiceServiceChannelAstiInBase>{channelRelation};
                    Service service;
                    var updatedConnections = new List<ServiceServiceChannel>();
                    try
                    {
                        contextManager.ExecuteWriter(unitOfWork =>
                        {
                            // Check address municipalities (PTV-4103)
                            relations.ChannelRelations?.ForEach(r => r.ContactDetails?.Addresses.ForEach(a => CheckAddress(unitOfWork, a)));

                            service = TranslationManagerToEntity.Translate<V11VmOpenApiServiceAndChannelRelationAstiInBase, Service>(relations, unitOfWork);
                            // We need to manually remove right connections from collection.
                            // If connections are ASTI connections regular connections should stay as they are.
                            // If connections are regular connections ASTI connections should stay as they are - the data can be updated though (except extraTypes).
                            var updatedConnectionIds = channelRelationsInputList.Count > 0 ? channelRelationsInputList.Select(r => r.ChannelGuid).ToList() : new List<Guid>();
                            updatedConnections = service.ServiceServiceChannels.Where(c => updatedConnectionIds.Contains(c.ServiceChannelId)).ToList();
                            RemoveAstiConnections(isASTI, service, updatedConnectionIds, unitOfWork);

                            // Remove archiving date on connected entities if system create ASTI connection
                            if (isASTI && relations.ChannelRelations != null)
                            {
                                RemoveArchiveDateOfEntities(unitOfWork, relations.ServiceId.Value, relations.ChannelRelations.Select(x => x.ServiceChannelId));
                            }

                            AddModifiedPropagationChain(unitOfWork);

                            // Asti users can update any items
                            unitOfWork.Save(SaveMode.Normal, isASTI ? PreSaveAction.DoNotCheckRoles : PreSaveAction.Standard);
                        });
                    }
                    catch(Exception ex)
                    {
                        var errorMsg = $"Error occured while updating relations for a service with id {rootID}. {ex.Message}";
                        logger.LogError(errorMsg + " " + ex.StackTrace);
                        throw new Exception(errorMsg);
                    }

                    UpdateConnectionAddresses(updatedConnections);
                }
            }

            return serviceService.GetServiceById(rootID, openApiVersion, VersionStatusEnum.Latest);
        }

        private void ProcessConnectionsCommonLanguage(V11VmOpenApiServiceAndChannelRelationAstiInBase relations)
        {
            if (relations.ServiceId != null)
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var removedConnections = utilities.ProcessConnectionCommonLanguage(DomainEnum.Services,
                        new Dictionary<Guid, IEnumerable<Guid>>
                            {{relations.ServiceId.Value, relations.ChannelRelations.Select(x => x.ChannelGuid)}},
                        unitOfWork);
                    if (removedConnections.Any())
                    {
                        var removedIds = removedConnections.Select(x => x.Item2);
                        relations.ChannelRelations = relations.ChannelRelations.Where(x => !removedIds.Contains(x.ChannelGuid)).ToList();
                    }
                });
            }
        }

        private void RemoveAstiConnections(bool isASTI, Service service, List<Guid> updatedConnectionIds, IUnitOfWorkWritable unitOfWork)
        {
            var connections = service.ServiceServiceChannels.Where(c => c.IsASTIConnection == isASTI).ToList();
            var connectionsToRemove = connections.Where(a => !updatedConnectionIds.Contains(a.ServiceChannelId)).ToList();
            RemoveConnections(unitOfWork, connectionsToRemove);
        }

        private void UpdateConnectionAddresses(List<ServiceServiceChannel> updatedConnections)
        {
            var streetId = typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString());
            foreach (var connection in updatedConnections)
            {
                if (connection.ServiceServiceChannelAddresses.HasData())
                {
                    var addresses = connection.ServiceServiceChannelAddresses
                        .Where(a => a.Address.TypeId == streetId)
                        .Select(x => x.AddressId).ToList();
                    addressService.UpdateAddress(addresses);
                }
            }
        }

        public IList<string> SaveServicesAndChannelsBySource(List<V11VmOpenApiServiceAndChannelRelationBySource> serviceAndChannelRelations)
        {
            var list = new List<string>();
            var userId = utilities.GetRelationIdForExternalSource();

            ProcessNewAddresses(serviceAndChannelRelations
                .Where(x=>x.ContactDetails?.Addresses != null)
                .SelectMany(r=>r.ContactDetails.Addresses
                    .Where(x=>x.StreetAddress != null)
                    .Select(x=>x.StreetAddress)));
            ProcessNewUrls(serviceAndChannelRelations.Where(x => x?.ContactDetails?.WebPages != null)
                .SelectMany(x => x.ContactDetails.WebPages).Where(x => x != null).Select(x => x.Url));

            foreach (var relation in serviceAndChannelRelations)
            {
                Guid? serviceId = null;
                Guid? serviceChannelId = null;

                try
                {
                    contextManager.ExecuteReader(unitOfWork =>
                    {
                        try
                        {
                            serviceId = GetPTVId<Service>(relation.ServiceSourceId, userId, unitOfWork);
                            if (!serviceId.IsAssigned()) { list.Add(string.Format(CoreMessages.OpenApi.EntityNotFound, "Service", relation.ServiceSourceId)); }
                            serviceChannelId = GetPTVId<ServiceChannel>(relation.ServiceChannelSourceId, userId, unitOfWork);
                            if (!serviceChannelId.IsAssigned())
                            {
                                list.Add(string.Format(CoreMessages.OpenApi.EntityNotFound, "ServiceChannel", relation.ServiceChannelSourceId));
                            }
                        }
                        catch(Exception ex)
                        {
                            list.Add(ex.Message);
                        }
                    });
                    if (serviceId.IsAssigned() && serviceChannelId.IsAssigned())
                    {
                        list.Add(SaveServiceServiceChannel(new V11VmOpenApiServiceServiceChannelAstiInBase
                        {
                            ServiceGuid = serviceId.Value,
                            ChannelGuid = serviceChannelId.Value,
                            Description = relation.Description,
                            ServiceChargeType = relation.ServiceChargeType,
                            ContactDetails = relation.ContactDetails?.ConvertToInBaseModel(),
                            ServiceHours = relation.ServiceHours
                        }));
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    list.Add(ex.Message);
                }
            }
            return list;
        }

        /// <summary>
        /// Updates the channel connections for a defined service. External source identifiers are used.
        /// </summary>
        /// <param name="serviceSourceId">The external source identifier of the service.</param>
        /// <param name="relationsBySource">The connection model</param>
        /// <param name="openApiVersion">The open api version to be returned.</param>
        /// <param name="isASTI"></param>
        /// <returns>Updated service with connection information</returns>
        public IVmOpenApiServiceVersionBase SaveServiceConnectionsBySource(string serviceSourceId, V11VmOpenApiServiceAndChannelRelationBySourceAsti relationsBySource, int openApiVersion, bool isASTI)
        {
            if (relationsBySource == null) return null;

            var errors = new List<string>();
            var userId = utilities.GetRelationIdForExternalSource();
            var relations = new V11VmOpenApiServiceAndChannelRelationAstiInBase
            {
                DeleteAllChannelRelations = relationsBySource.DeleteAllChannelRelations,
            };

            contextManager.ExecuteReader(unitOfWork =>
            {
                try
                {
                    relations.ServiceId = GetPTVId<Service>(serviceSourceId, userId, unitOfWork);

                    // Check the current version (publishing status) of service. PTV-3933
                    PublishingStatus? currentPublishingStatus = relations.ServiceId.HasValue ? serviceService.GetLatestVersionPublishingStatus(relations.ServiceId.Value) : null;
                    if (!currentPublishingStatus.HasValue)
                    {
                        throw new Exception(string.Format(CoreMessages.OpenApi.EntityNotFound, "Service", serviceSourceId));
                    }
                    else if (currentPublishingStatus.Value != PublishingStatus.Draft && currentPublishingStatus.Value != PublishingStatus.Published)
                    {
                        throw new Exception($"Publishing status for service '{serviceSourceId}' is {currentPublishingStatus}. You cannot update service!");
                    }

                    foreach (var channelRelation in relationsBySource.ChannelRelations)
                    {
                        try
                        {
                            var channelId = GetPTVId<ServiceChannel>(channelRelation.ServiceChannelSourceId, userId, unitOfWork);
                            if (channelId.IsAssigned())
                            {
                                CheckChannelData(unitOfWork, channelId, channelRelation.IsASTIConnection, channelRelation.ServiceHours, channelRelation.ContactDetails);

                                channelRelation.ExtraTypes.ForEach(e => { e.ChannelGuid = channelId; e.ServiceGuid = relations.ServiceId.Value; });
                                relations.ChannelRelations.Add(new V11VmOpenApiServiceServiceChannelAstiInBase
                                {
                                    ChannelGuid = channelId,
                                    ServiceGuid = relations.ServiceId.Value,
                                    Description = channelRelation.Description,
                                    ServiceChargeType = channelRelation.ServiceChargeType,
                                    ExtraTypes = channelRelation.ExtraTypes,
                                    ServiceHours = channelRelation.ServiceHours,
                                    ContactDetails = channelRelation.ContactDetails?.ConvertToInBaseModel(),
                                    IsASTIConnection = channelRelation.IsASTIConnection,
                                    DeleteServiceChargeType = channelRelation.DeleteServiceChargeType,
                                    DeleteAllDescriptions = channelRelation.DeleteAllDescriptions,
                                    DeleteAllServiceHours = channelRelation.DeleteAllServiceHours,
                                    DeleteAllExtraTypes = channelRelation.DeleteAllExtraTypes,
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            errors.Add(ex.Message);
                        }
                    }
                }
                catch (ExternalSourceNotFoundException nex)
                {
                    throw nex;
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                }
            });
            if (errors.Count > 0)
            {
                throw new Exception(String.Join(", ", errors));
            }

            return SaveServiceConnections(relations, openApiVersion, isASTI);
        }

        /// <summary>
        /// Updates service connections related to defined service channel. This is a method only for ASTI so ASTI rules are added.
        /// See ASTI rules from page https://confluence.csc.fi/display/PAL/ASTI-project and https://jira.csc.fi/browse/PTV-2065.
        /// </summary>
        /// <param name="request">The connection model</param>
        /// <param name="openApiVersion">The open api version to be returned.</param>
        /// <returns>Updated service channel with connection information</returns>
        public IVmOpenApiServiceChannel SaveServiceChannelConnections(V11VmOpenApiChannelServicesIn relations, int openApiVersion)
        {
            if (relations == null) return null;
            var rootID = relations.ChannelId.Value;
            var serviceRelationsInputList = new List<V11VmOpenApiServiceChannelServiceInBase>(relations.ServiceRelations);

            if (relations.DeleteAllServiceRelations && relations.ServiceRelations?.Count == 0)
            {
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var serviceChannel = TranslationManagerToEntity.Translate<V11VmOpenApiChannelServicesIn, ServiceChannel>(relations, unitOfWork);
                    var updatedConnections = serviceRelationsInputList.Count > 0 ? serviceRelationsInputList.Select(r => r.ServiceGuid).ToList() : new List<Guid>();
                    RemoveAstiConnections(serviceChannel, updatedConnections, unitOfWork);
                    AddModifiedPropagationChain(unitOfWork);
                    unitOfWork.Save(SaveMode.Normal, PreSaveAction.DoNotCheckRoles);
                });
            }
            else
            {
                foreach (var serviceRelation in serviceRelationsInputList)
                {
                    relations.ServiceRelations = new List<V11VmOpenApiServiceChannelServiceInBase> {serviceRelation};
                    var connectionsToSave = new List<ServiceServiceChannel>();
                    try
                    {
                        ProcessNewAddresses(relations.ServiceRelations?
                            .Where(x => x.ContactDetails?.Addresses != null)
                            .SelectMany(x => x.ContactDetails.Addresses)
                            .Where(x => x.StreetAddress != null)
                            .Select(x => x.StreetAddress));
                        ProcessNewUrls(relations.ServiceRelations?.Where(x => x.ContactDetails?.WebPages != null)
                            .SelectMany(x => x.ContactDetails.WebPages).Where(x => x != null).Select(x => x.Url));

                        contextManager.ExecuteWriter(unitOfWork =>
                        {
                            // Check address municipalities (PTV-4103)
                            relations.ServiceRelations?.ForEach(r => r.ContactDetails?.Addresses.ForEach(a => CheckAddress(unitOfWork, a)));
                            var serviceChannel = TranslationManagerToEntity.Translate<V11VmOpenApiChannelServicesIn, ServiceChannel>(relations, unitOfWork);
                            // We need to manually remove ASTI connections from collection. All other connections should stay as they are.
                            var updatedConnections = serviceRelationsInputList.Count > 0 ? serviceRelationsInputList.Select(r => r.ServiceGuid).ToList()
                                : new List<Guid>();
                            connectionsToSave = serviceChannel.ServiceServiceChannels.Where(c => updatedConnections.Contains(c.ServiceId)).ToList();
                            RemoveAstiConnections(serviceChannel, updatedConnections, unitOfWork);
                            AddModifiedPropagationChain(unitOfWork);
                            unitOfWork.Save(SaveMode.Normal, PreSaveAction.DoNotCheckRoles);
                        });
                    }
                    catch (Exception ex)
                    {
                        var errorMsg =
                            $"Error occured while updating relations for a service channel with id {rootID}. {ex.Message}";
                        logger.LogError(errorMsg + " " + ex.StackTrace);
                        throw new Exception(errorMsg);
                    }

                    UpdateConnectionAddresses(connectionsToSave);
                }
            }

            return channelService.GetServiceChannelById(rootID, openApiVersion, VersionStatusEnum.Latest);
        }

        private void RemoveAstiConnections(ServiceChannel serviceChannel, List<Guid> updatedConnections, IUnitOfWorkWritable unitOfWork)
        {
            var astiConnections = serviceChannel.ServiceServiceChannels.Where(c => c.IsASTIConnection == true).ToList();
            var connectionsToRemove = astiConnections.Where(a => !updatedConnections.Contains(a.ServiceId)).ToList();
            RemoveConnections(unitOfWork, connectionsToRemove);
        }

        private void ProcessNewUrls(IEnumerable<string> urls)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                urlService.AddNewUrls(unitOfWork, urls);
                unitOfWork.Save();
            });
        }

        private void CheckChannelData(IUnitOfWork unitOfWork, Guid channelId, bool isAstiConnection, IList<V11VmOpenApiServiceHour> serviceHours, V9VmOpenApiContactDetailsInBase contactDetails)
        {
            var channel = channelService.GetServiceChannelByIdSimple(channelId);
            if (channel == null)
            {
                throw new Exception(string.Format(CoreMessages.OpenApi.EntityNotFound, "ServiceChannel", channelId));
            }
            // Is the channel visible for all?
            // Asti can always update connections despite of channel visibility.
            // Eeva can always update connections despite of channel visibility.
            if (utilities.UserHighestRole() != UserRoleEnum.Eeva && !isAstiConnection)
            {
                var security = ((VmOpenApiServiceChannel)channel).Security;
                if (!channel.IsVisibleForAll && security == null && !(security != null && security.IsOwnOrganization))
                {
                    throw new Exception(string.Format(CoreMessages.OpenApi.ChannelNotVisibleForAll, channel.Id));
                }
            }

            var strServiceLocation = ServiceChannelTypeEnum.ServiceLocation.ToString();

            // ASTI connections can only be added for service location channels! PTV-3539
            if (isAstiConnection && channel.ServiceChannelType != strServiceLocation)
            {
                throw new Exception(string.Format(CoreMessages.OpenApi.MustBeServiceLocationChannel, channel.Id, channel.ServiceChannelType));
            }

            // Only service location channel can have service hours or contact details! PTV-2475
            if (channel.ServiceChannelType != strServiceLocation && (serviceHours?.Count > 0 || contactDetails != null))
            {
                throw new Exception(string.Format(CoreMessages.OpenApi.AdditionalInfoForConnection, channel.Id, channel.ServiceChannelType));
            }

            // Set address sub type Abroad as Foreign (PTV-3914)
            // Check address municipality (PTV-4103)
            contactDetails?.Addresses?.ForEach(a =>
            {
                if (a.SubType == AddressConsts.ABROAD)
                {
                    a.SubType = AddressTypeEnum.Foreign.ToString();
                }
                CheckAddress(unitOfWork, a);
            });
        }

        private void RemoveConnections(IUnitOfWorkWritable unitOfWork, List<ServiceServiceChannel> connectionsToRemove)
        {
            var connectionRepo = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var descriptionRepo = unitOfWork.CreateRepository<IServiceServiceChannelDescriptionRepository>();
            connectionsToRemove.ForEach(c =>
            {
                // have to get the descriptions from db to be able to delete them
                var descriptions = descriptionRepo.All().Where(r => r.ServiceChannelId == c.ServiceChannelId && r.ServiceId == c.ServiceId).ToList();
                descriptions.ForEach(d => descriptionRepo.Remove(d));
                connectionRepo.Remove(c);
            });
        }

        private string SaveServiceServiceChannel(V11VmOpenApiServiceServiceChannelAstiInBase serviceServiceChannel)
        {
            if (!serviceServiceChannel.ChannelGuid.IsAssigned())
            {
                serviceServiceChannel.ChannelGuid = serviceServiceChannel.ServiceChannelId.ParseToGuidWithExeption();
                serviceServiceChannel.ServiceGuid = serviceServiceChannel.ServiceId.ParseToGuidWithExeption();
            }

            PublishingStatus? currentPublishingStatus = serviceService.GetLatestVersionPublishingStatus(serviceServiceChannel.ServiceGuid);
            if (!currentPublishingStatus.HasValue)
            {
                throw new Exception(string.Format(CoreMessages.OpenApi.EntityNotFound, "Service", serviceServiceChannel.ServiceGuid));
            }
            else if (currentPublishingStatus.Value != PublishingStatus.Draft && currentPublishingStatus.Value != PublishingStatus.Published)
            {
                throw new Exception($"Publishing status for service '{serviceServiceChannel.ServiceGuid}' is {currentPublishingStatus}. You cannot update service!");
            }

            PublishingStatus? channelPublishingStatus = channelService.GetLatestVersionPublishingStatus(serviceServiceChannel.ChannelGuid);
            if (!channelPublishingStatus.HasValue)
            {
                return string.Format(CoreMessages.OpenApi.EntityNotFound, "Service channel", serviceServiceChannel.ChannelGuid);
            }

            string msg = null;
            var streetId = typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString());
            ServiceServiceChannel result = null;
           
            //  ProcessNewAddresses(serviceServiceChannel?.ContactDetails?.Addresses.Select(x=>x?.StreetAddress));
            try
            {
                if (CheckConnection(serviceServiceChannel))
                {
                    contextManager.ExecuteWriter(unitOfWork =>
                    {
                        result = TranslationManagerToEntity
                            .Translate<V11VmOpenApiServiceServiceChannelAstiInBase, ServiceServiceChannel>(
                                serviceServiceChannel, unitOfWork);
                        AddModifiedPropagationChain(unitOfWork);
                        unitOfWork.Save();

                        msg = string.Format(CoreMessages.OpenApi.ServiceServiceChannelAdded,
                            serviceServiceChannel.ChannelGuid, serviceServiceChannel.ServiceGuid);
                    });

                    if (result?.ServiceServiceChannelAddresses?.HasData() ?? false)
                    {
                        var addressIds = result.ServiceServiceChannelAddresses
                            .Where(a => a.Address.TypeId == streetId)
                            .Select(a => a.AddressId)
                            .ToList();
                        addressService.UpdateAddress(addressIds);
                    }
                }
                else
                {
                    msg = string.Format(CoreMessages.OpenApi.ServiceServiceChannelIgnored,
                        serviceServiceChannel.ChannelGuid, serviceServiceChannel.ServiceGuid);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        private bool CheckConnection(V11VmOpenApiServiceServiceChannelAstiInBase serviceServiceChannel)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                //Check channel data
                CheckChannelData(unitOfWork, serviceServiceChannel.ChannelGuid,
                    serviceServiceChannel.IsASTIConnection, serviceServiceChannel.ServiceHours,
                    serviceServiceChannel.ContactDetails);
                //Check common language
                var serviceLanguages = utilities.GetEntityLanguages<ServiceVersioned, ServiceLanguageAvailability>(
                    new List<Guid> {serviceServiceChannel.ServiceGuid},
                    unitOfWork);
                var channelLanguages =
                    utilities.GetEntityLanguages<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                        new List<Guid> {serviceServiceChannel.ChannelGuid},
                        unitOfWork);
                return serviceLanguages.Any() && channelLanguages.Any() &&
                       serviceLanguages[serviceServiceChannel.ServiceGuid].Intersect(channelLanguages[serviceServiceChannel.ChannelGuid]).Any();
            });
        }

        private void AddModifiedPropagationChain(IUnitOfWorkWritable unitOfWork)
        {
            unitOfWork.AddModifiedPropagationChain<ServiceServiceChannelDescription, ServiceServiceChannel>(i => i.AddPath(j => j.ServiceServiceChannel).Final());
            unitOfWork.AddModifiedPropagationChain<ServiceServiceChannelDigitalAuthorization, ServiceServiceChannel>(i => i.AddPath(j => j.ServiceServiceChannel).Final());
            unitOfWork.AddModifiedPropagationChain<ServiceServiceChannelExtraType, ServiceServiceChannel>(i => i.AddPath(j => j.ServiceServiceChannel).Final());
            unitOfWork.AddModifiedPropagationChain<ServiceHoursAdditionalInformation, ServiceServiceChannel>(i => i.AddPath(j => j.ServiceHours).AddPath(j => j.ServiceServiceChannelServiceHours).AddPath(j => j.ServiceServiceChannel).Final());
            unitOfWork.AddModifiedPropagationChain<ServiceHours, ServiceServiceChannel>(i => i.AddPath(j => j.ServiceServiceChannelServiceHours).AddPath(j => j.ServiceServiceChannel).Final());
            unitOfWork.AddModifiedPropagationChain<DailyOpeningTime, ServiceServiceChannel>(i => i.AddPath(j => j.OpeningHour).AddPath(j => j.ServiceServiceChannelServiceHours).AddPath(j => j.ServiceServiceChannel).Final());
            unitOfWork.AddModifiedPropagationChain<AddressForeignTextName, ServiceServiceChannel>(i => i.AddPath(j => j.AddressForeign).AddPath(j => j.Address).AddPath(j => j.ServiceServiceChannelAddresses).AddPath(j => j.ServiceServiceChannel).Final());
            unitOfWork.AddModifiedPropagationChain<PostOfficeBoxName, ServiceServiceChannel>(i => i.AddPath(j => j.AddressPostOfficeBox).AddPath(j => j.Address).AddPath(j => j.ServiceServiceChannelAddresses).AddPath(j => j.ServiceServiceChannel).Final());
            unitOfWork.AddModifiedPropagationChain<AddressPostOfficeBox, ServiceServiceChannel>(i => i.AddPath(j => j.Address).AddPath(j => j.ServiceServiceChannelAddresses).AddPath(j => j.ServiceServiceChannel).Final());
            unitOfWork.AddModifiedPropagationChain<AddressAdditionalInformation, ServiceServiceChannel>(i => i.AddPath(j => j.Address).AddPath(j => j.ServiceServiceChannelAddresses).AddPath(j => j.ServiceServiceChannel).Final());
            unitOfWork.AddModifiedPropagationChain<Address, ServiceServiceChannel>(i => i.AddPath(j => j.ServiceServiceChannelAddresses).AddPath(j => j.ServiceServiceChannel).Final());
            unitOfWork.AddModifiedPropagationChain<Phone, ServiceServiceChannel>(i => i.AddPath(j => j.ServiceServiceChannelPhones).AddPath(j => j.ServiceServiceChannel).Final());
            unitOfWork.AddModifiedPropagationChain<WebPage, ServiceServiceChannel>(i => i.AddPath(j => j.ServiceServiceChannelWebPages).AddPath(j => j.ServiceServiceChannel).Final());
            unitOfWork.AddModifiedPropagationChain<Email, ServiceServiceChannel>(i => i.AddPath(j => j.ServiceServiceChannelEmails).AddPath(j => j.ServiceServiceChannel).Final());
        }

        private void RemoveArchiveDateOfEntities(IUnitOfWorkWritable unitOfWork, Guid serviceId, IEnumerable<string> channelIds)
        {
            foreach (var channelId in channelIds.Where(x=>!string.IsNullOrEmpty(x)))
            {
                var channel = VersioningManager.GetLastPublishedVersion<ServiceChannelVersioned>(unitOfWork, Guid.Parse(channelId));
                unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>().All()
                    .Where(l => l.ServiceChannelVersionedId == channel.EntityId)
                    .ForEach(l => l.ArchiveAt = null);
            }
            var service = VersioningManager.GetLastPublishedVersion<ServiceVersioned>(unitOfWork, serviceId);
            if (service != null)
            {
                unitOfWork.CreateRepository<IServiceLanguageAvailabilityRepository>().All()
                    .Where(l => l.ServiceVersionedId == service.EntityId)
                    .ForEach(l => l.ArchiveAt = null);
            }

        }

        private void ProcessNewAddresses(IEnumerable<VmOpenApiAddressStreetIn> model)
        {
            if(model.IsNullOrEmpty()) return;
            Dictionary<(Guid, string), Guid> newStreets = new Dictionary<(Guid, string), Guid>();

            var addresses = model.Select(x => new VmAddressSimple
            {
                StreetType = AddressTypeEnum.Street.ToString(),
                StreetName = x.Street.ToDictionary(k=>k.Language, v=>v.Value.Trim().FirstCharToUpper()),
                PostalCode = new VmPostalCode
                {
                    Id = postalCodeCache.GuidByCode(x.PostalCode),
                    Code = x.PostalCode,
                    MunicipalityId = postalCodeCache.MunicipalityIdForCode(x.PostalCode)
                },
                Municipality = postalCodeCache.MunicipalityIdForCode(x.PostalCode),
                StreetNumber = x.StreetNumber,
                Coordinates = new List<VmCoordinate>()
            });
            contextManager.ExecuteWriter(unitOfWork =>
            {
                addressService.AddNewStreetAddresses(unitOfWork, addresses, newStreets);
                unitOfWork.Save();
            });
            contextManager.ExecuteWriter(unitOfWork =>
            {
                addressService.AddNewStreetAddressNumbers(unitOfWork,addresses, newStreets);
                unitOfWork.Save();
            });
        }

        #endregion
    }
}
