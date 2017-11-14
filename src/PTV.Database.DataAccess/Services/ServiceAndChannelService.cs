/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Enums;
using Microsoft.Extensions.Logging;
using PTV.Database.Model.Models.Base;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models.OpenApi;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Utils;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V7;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof (IServiceAndChannelService), RegisterType.Transient)]
    internal class ServiceAndChannelService : ServiceBase, IServiceAndChannelService
    {
        private readonly IContextManager contextManager;
        private ILogger logger;
        private DataUtils dataUtils;
        private ServiceUtilities utilities;
        private IServiceService serviceService;
        private IChannelService channelService;
        private IVersioningManager versioningManager;
        private readonly ITypesCache typesCache;
        private IUserOrganizationService userOrganizationService;

        public ServiceAndChannelService(IContextManager contextManager,
                                       ITranslationEntity translationEntToVm,
                                       ITranslationViewModel translationVmtoEnt,
                                       ILogger<ServiceAndChannelService> logger,
                                       ServiceUtilities utilities,
                                       DataUtils dataUtils,
                                       IServiceService serviceService,
                                       IChannelService channelService,
                                       IPublishingStatusCache publishingStatusCache,
                                       IVersioningManager versioningManager,
                                       IUserOrganizationChecker userOrganizationChecker,
                                       ICacheManager cacheManager,
                                       IUserOrganizationService userOrganizationService
            )
            : base(translationEntToVm, translationVmtoEnt, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.dataUtils = dataUtils;
            this.serviceService = serviceService;
            this.channelService = channelService;
            this.versioningManager = versioningManager;
            this.userOrganizationService = userOrganizationService;
            typesCache = cacheManager.TypesCache;
        }

        public List<IVmBase> SaveServiceAndChannels(VmRelations relations)
        {
            var result = new List<IVmBase>();

            Dictionary<Guid, Guid> serviceVersion = new Dictionary<Guid, Guid>();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                IQueryable<Guid> supportedChannelIds = null;
                var userOrgs = userOrganizationService.GetUserCompleteOrgStructure(unitOfWork);
                if (userOrganizationService.UserHighestRole(userOrgs) != UserRoleEnum.Eeva)
                {
                    var schvRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                    var userOrgsIds = userOrgs.SelectMany(i => i).Select(i => i.OrganizationId).ToList();
                    var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                    var psCommonForAll = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.CommonForAll.ToString());
                    var channelIds = relations.ServiceAndChannelRelations.SelectMany(r => r.ChannelRelations).Select(ch => ch.ConnectedChannel.Id);
                    var channels = schvRep.All().Where(i => channelIds.Contains(i.Id));

                    var supportedChannels = channels.Where(ch => (userOrgsIds.Contains(ch.OrganizationId) || (ch.PublishingStatusId == psPublished && ch.ConnectionTypeId == psCommonForAll)));

                    supportedChannelIds = supportedChannels.Select(ch => ch.UnificRootId);
                }

                SetTranslatorLanguage(relations);
                relations.ServiceAndChannelRelations = relations.ServiceAndChannelRelations.Distinct(new RelationComparer()).ToList();
                relations.ServiceAndChannelRelations.ForEach(i => i.ChannelRelations = i.ChannelRelations.Distinct(new RelationChannelsComparer()).ToList());
					
				var serviceVersionedRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var serviceVersionedIds = relations.ServiceAndChannelRelations.Select(sv => sv.Id).ToList();
                var unificRootIds = serviceVersionedRep.All().Where(sv => serviceVersionedIds.Contains(sv.Id)).ToDictionary(k => k.Id, v => v.UnificRootId);
                foreach (var item in relations.ServiceAndChannelRelations)
                {
                    if (!item.Id.IsAssigned() || !unificRootIds.ContainsKey(item.Id.Value)) continue;
                    item.UnificRootId = unificRootIds[item.Id.Value];
                    item.ChannelRelations.ForEach(chs => chs.Service = unificRootIds[item.Id.Value]);
                }
				
				
                var relationData = TranslationManagerToEntity.TranslateAll<VmServiceChannelRelation, Service>(relations.ServiceAndChannelRelations, unitOfWork);
                serviceVersion = unitOfWork.TranslationCloneCache.GetFromCachedSet<ServiceVersioned>().ToDictionary(i => i.OriginalEntity.Id, i => i.ClonedEntity.Id);

                foreach (var service in relationData)
                {
                    var supportedServiceServiceChannels = supportedChannelIds == null
						? service.ServiceServiceChannels
                        : service.ServiceServiceChannels.Where(ch => supportedChannelIds.Contains(ch.ServiceChannelId)).ToList();

                    service.ServiceServiceChannels = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, supportedServiceServiceChannels, query => query.ServiceId == service.Id, channel => channel.ServiceChannelId);
                    UpdateDigitalAuthorizationCollection(unitOfWork, service.ServiceServiceChannels);
                }

                unitOfWork.Save();
            });

            contextManager.ExecuteReader(unitOfWork =>
            { 
                var serviceCloneData = new Dictionary<Guid, string>();
                relations.ServiceAndChannelRelations.ForEach(service =>
                {
                    if (serviceVersion.ContainsKey(service.Id.Value))
                    {
                        serviceCloneData.Add(serviceVersion[service.Id.Value], service.ConnectedServiceId);
                    }
                });
                var cloneServiceIds = serviceCloneData.Select(y => y.Key).ToList();

                var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var resultTemp = unitOfWork.ApplyIncludes(serviceRep.All().Where(x => cloneServiceIds.Contains(x.Id)), q =>
                    q.Include(i => i.ServiceNames).ThenInclude(i => i.Type)
                    .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Versions).ThenInclude(i => i.ServiceChannelNames).ThenInclude(i => i.Type)
                    .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Versions).ThenInclude(i => i.Type)
                    .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDescriptions).ThenInclude(i => i.Type)
                    .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDigitalAuthorizations).ThenInclude(i => i.DigitalAuthorization)
                    );

                var cloneServices = TranslationManagerToVm.TranslateAll<ServiceVersioned, VmServiceRelationListItem>(resultTemp).Cast<IVmServiceListItem>().ToList();

                cloneServices.ForEach(service =>
                     {
                         if (serviceCloneData.ContainsKey(service.Id))
                         {
                             result.Add(new VmConnectedService() { UiId = serviceCloneData[service.Id], Service = service });
                         }
                     });
                });
            return result;
        }

        private void UpdateDigitalAuthorizationCollection(IUnitOfWorkWritable unitOfWork, ICollection<ServiceServiceChannel> relations)
        {
            foreach (var relation in relations)
            {
                relation.ServiceServiceChannelDigitalAuthorizations = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, relation.ServiceServiceChannelDigitalAuthorizations, query => query.ServiceId == relation.ServiceId && query.ServiceChannelId == relation.ServiceChannelId, da => da.DigitalAuthorizationId);
            }
        }

        public VmChannelRelation GetRelationDetail(VmGetRelationDetail model)
        {
            VmChannelRelation result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                TranslationManagerToVm.SetLanguage(model.Language);
                var relationRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
                var resultTemp = unitOfWork.ApplyIncludes(relationRep.All().Where(x => (x.ServiceId == model.ServiceId.Value && x.ServiceChannelId == model.ChannelId.Value)), i => i
                    .Include(j => j.ServiceChannel).ThenInclude(j => j.Versions).ThenInclude(j => j.ServiceChannelNames).ThenInclude(j => j.Type)
                    .Include(j => j.ServiceChannel).ThenInclude(j => j.Versions).ThenInclude(j => j.Type)
                    .Include(j => j.ServiceServiceChannelDescriptions).ThenInclude(j => j.Type)
                    .Include(j => j.ServiceServiceChannelDigitalAuthorizations).ThenInclude(j => j.DigitalAuthorization),
                    true).FirstOrDefault();

                if (resultTemp != null)
                {
                    result = TranslationManagerToVm.Translate<ServiceServiceChannel, VmChannelRelation>(resultTemp);
                    result.Id = model.ChannelRelationId;
                }
            });

            return result;
        }

        private List<PublishingAffectedResult> PublishEntities<TEntity>(IUnitOfWorkWritable unitOfWork, List<Guid> entitiesIds) where TEntity : class, IEntityIdentifier, IVersionedVolume, new()
        {
            var entityRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entities = entityRep.All().Where(x => entitiesIds.Contains(x.Id)).ToList();
            return entities.Select(service => versioningManager.PublishVersion(unitOfWork, service)).SelectMany(i => i).ToList();
        }

        public VmPublishServiceAndChannelResult PublishRelations(VmPublishServiceAndChannel model)
        {
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var resultServices = PublishEntities<ServiceVersioned>(unitOfWork, model.Services);
                var resultChannels = PublishEntities<ServiceChannelVersioned>(unitOfWork, model.Channels);
                unitOfWork.Save();
                return new VmPublishServiceAndChannelResult()
                {
                    Channels = resultChannels.Select(i => new VmEntityStatusBase() {Id = i.Id, PublishingStatusId = i.PublishingStatusNew}).ToList<IVmEntityBase>(),
                    Services = resultServices.Select(i => new VmEntityStatusBase() {Id = i.Id, PublishingStatusId = i.PublishingStatusNew}).ToList<IVmEntityBase>()
                };
            });
        }

        #region Open api

        public IList<string> SaveServicesAndChannels(List<V7VmOpenApiServiceServiceChannelAstiInBase> serviceAndChannelRelations)
        {
            var list = new List<string>();
            if (serviceAndChannelRelations != null)
            {
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
        /// <returns>Updated service with connection information</returns>
        public IVmOpenApiServiceVersionBase SaveServiceConnections(V7VmOpenApiServiceAndChannelRelationAstiInBase relations, int openApiVersion)
        {
            if (relations == null) return null;

            Service service;
            var rootID = relations.ServiceId.Value;
            try
            {
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    service = TranslationManagerToEntity.Translate<V7VmOpenApiServiceAndChannelRelationAstiInBase, Service>(relations, unitOfWork);
                    // We need to manually remove right connections from collection. 
                    // If connections are ASTI connections regular connections should stay as they are.
                    // If connections are regular connections ASTI connections should stay as they are - the data can be updated though (except extraTypes). 
                    var updatedConnectionIds = relations.ChannelRelations?.Count > 0 ? relations.ChannelRelations.Select(r => r.ChannelGuid).ToList() : new List<Guid>();
                    var updatedConnections = service.ServiceServiceChannels.Where(c => updatedConnectionIds.Contains(c.ServiceChannelId)).ToList();
                    var connections = service.ServiceServiceChannels.Where(c => c.IsASTIConnection == relations.IsASTI).ToList();
                    var connectionsToRemove = connections.Where(a => !updatedConnectionIds.Contains(a.ServiceChannelId)).ToList();
                    RemoveConnections(unitOfWork, connectionsToRemove);

                    unitOfWork.Save();
                });
            }
            catch(Exception ex)
            {
                var errorMsg = $"Error occured while updating relations for a service with id {rootID}. {ex.Message}";
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }

            return serviceService.GetServiceById(rootID, openApiVersion, VersionStatusEnum.Latest);
        }

        public IList<string> SaveServicesAndChannelsBySource(List<V7VmOpenApiServiceAndChannelRelationBySource> serviceAndChannelRelations)
        {
            var list = new List<string>();
            var userId = utilities.GetRelationIdForExternalSource();
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
                        list.Add(SaveServiceServiceChannel(new V7VmOpenApiServiceServiceChannelAstiInBase
                        {
                            ServiceGuid = serviceId.Value,
                            ChannelGuid = serviceChannelId.Value,
                            Description = relation.Description,
                            ServiceChargeType = relation.ServiceChargeType,
                            ContactDetails = relation.ContactDetails,
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
        /// <returns>Updated service with connection information</returns>
        public IVmOpenApiServiceVersionBase SaveServiceConnectionsBySource(string serviceSourceId, V7VmOpenApiServiceAndChannelRelationBySourceAsti relationsBySource, int openApiVersion)
        {
            if (relationsBySource == null) return null;

            var errors = new List<string>();
            var userId = utilities.GetRelationIdForExternalSource();
            var relations = new V7VmOpenApiServiceAndChannelRelationAstiInBase()
            {
                DeleteAllChannelRelations = relationsBySource.DeleteAllChannelRelations,
                IsASTI = relationsBySource.IsASTI
            };

            contextManager.ExecuteReader(unitOfWork =>
            {
                try
                {
                    relations.ServiceId = GetPTVId<Service>(serviceSourceId, userId, unitOfWork);
                    foreach (var channelRelation in relationsBySource.ChannelRelations)
                    {
                        var channelId = GetPTVId<ServiceChannel>(channelRelation.ServiceChannelSourceId, userId, unitOfWork);
                        if (channelId.IsAssigned())
                        {
                            CheckChannelData(unitOfWork, channelId, channelRelation.IsASTIConnection, channelRelation.ServiceHours, channelRelation.ContactDetails);

                            channelRelation.ExtraTypes.ForEach(e => { e.ChannelGuid = channelId; e.ServiceGuid = relations.ServiceId.Value; });
                            relations.ChannelRelations.Add(new V7VmOpenApiServiceServiceChannelAstiInBase
                            {
                                ChannelGuid = channelId,
                                ServiceGuid = relations.ServiceId.Value,
                                Description = channelRelation.Description,
                                ServiceChargeType = channelRelation.ServiceChargeType,
                                ExtraTypes = channelRelation.ExtraTypes,
                                ServiceHours = channelRelation.ServiceHours,
                                ContactDetails = channelRelation.ContactDetails,
                                IsASTIConnection = channelRelation.IsASTIConnection,
                                DeleteServiceChargeType = channelRelation.DeleteServiceChargeType,
                                DeleteAllDescriptions = channelRelation.DeleteAllDescriptions,
                                DeleteAllServiceHours = channelRelation.DeleteAllServiceHours,
                                DeleteAllExtraTypes = channelRelation.DeleteAllExtraTypes,
                            });
                        }
                    }
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

            return SaveServiceConnections(relations, openApiVersion);
        }

        /// <summary>
        /// Updates service connections related to defined service channel. This is a method only for ASTI so ASTI rules are added.
        /// See ASTI rules from page https://confluence.csc.fi/display/PAL/ASTI-project and https://jira.csc.fi/browse/PTV-2065.
        /// </summary>
        /// <param name="request">The connection model</param>
        /// <param name="openApiVersion">The open api version to be returned.</param>
        /// <returns>Updated service channel with connection information</returns>
        public IVmOpenApiServiceChannel SaveServiceChannelConnections(V7VmOpenApiChannelServicesIn relations, int openApiVersion)
        {
            if (relations == null) return null;

            var rootID = relations.ChannelId.Value;
            try
            {
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var serviceChannel = TranslationManagerToEntity.Translate<V7VmOpenApiChannelServicesIn, ServiceChannel>(relations, unitOfWork);
                    // We need to manually remove ASTI connections from collection. All other connections should stay as they are.
                    var updatedConnections = relations.ServiceRelations?.Count > 0 ? relations.ServiceRelations.Select(r => r.ServiceGuid).ToList() : new List<Guid>();
                    var astiConnections = serviceChannel.ServiceServiceChannels.Where(c => c.IsASTIConnection == true).ToList();
                    var connectionsToRemove = astiConnections.Where(a => !updatedConnections.Contains(a.ServiceId)).ToList();
                    RemoveConnections(unitOfWork, connectionsToRemove);

                    unitOfWork.Save();
                });
            }
            catch (Exception ex)
            {
                var errorMsg = $"Error occured while updating relations for a service channel with id {rootID}. {ex.Message}";
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }

            return channelService.GetServiceChannelById(rootID, openApiVersion, VersionStatusEnum.Latest);
        }

        private void CheckChannelData(IUnitOfWork unitOfWork, Guid channelId, bool isAstiConnection, IList<V4VmOpenApiServiceHour> serviceHours, VmOpenApiContactDetailsInBase contactDetails)
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

            // Only service location channel can have service hours or contact details! PTV-2475
            if (channel.ServiceChannelType != ServiceChannelTypeEnum.ServiceLocation.ToString() &&
                (serviceHours?.Count > 0 || contactDetails != null))
            {
                throw new Exception(string.Format(CoreMessages.OpenApi.AdditionalInfoForConnection, channel.Id, channel.ServiceChannelType));
            }
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

        private string SaveServiceServiceChannel(V7VmOpenApiServiceServiceChannelAstiInBase serviceServiceChannel)
        {
            if (!serviceServiceChannel.ChannelGuid.IsAssigned())
            {
                serviceServiceChannel.ChannelGuid = serviceServiceChannel.ServiceChannelId.ParseToGuidWithExeption();
                serviceServiceChannel.ServiceGuid = serviceServiceChannel.ServiceId.ParseToGuidWithExeption();
            }

            var currentVersion = serviceService.GetServiceByIdSimple(serviceServiceChannel.ServiceGuid);
            if (currentVersion == null || string.IsNullOrEmpty(currentVersion.PublishingStatus))
            {
                return string.Format(CoreMessages.OpenApi.EntityNotFound, "Service", serviceServiceChannel.ServiceGuid);
            }
            else if (currentVersion.PublishingStatus != PublishingStatus.Draft.ToString() && currentVersion.PublishingStatus != PublishingStatus.Published.ToString())
            {
                return $"Publishing status for service '{serviceServiceChannel.ServiceGuid}' is {currentVersion.PublishingStatus}. You cannot update service!";
            }

            var channel = channelService.GetServiceChannelByIdSimple(serviceServiceChannel.ChannelGuid);
            if (channel == null || !channel.Id.IsAssigned())
            {
                return string.Format(CoreMessages.OpenApi.EntityNotFound, "Service channel", serviceServiceChannel.ChannelGuid);
            }

            string msg = null;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                try
                {
                    CheckChannelData(unitOfWork, serviceServiceChannel.ChannelGuid, serviceServiceChannel.IsASTIConnection, serviceServiceChannel.ServiceHours, serviceServiceChannel.ContactDetails);

                    var result = TranslationManagerToEntity.Translate<IVmOpenApiServiceServiceChannelInVersionBase, ServiceServiceChannel>(serviceServiceChannel, unitOfWork);
                    
                    unitOfWork.Save();
                    msg = string.Format(CoreMessages.OpenApi.ServiceServiceChannelAdded, serviceServiceChannel.ChannelGuid, serviceServiceChannel.ServiceGuid);
                }
                catch(Exception ex)
                {
                    msg = ex.Message;
                }                
            });

            return msg;
        }        

        #endregion

        class RelationComparer : IEqualityComparer<VmServiceChannelRelation>
        {
            public bool Equals(VmServiceChannelRelation x, VmServiceChannelRelation y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(VmServiceChannelRelation obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        class RelationChannelsComparer : IEqualityComparer<VmChannelRelation>
        {
            public bool Equals(VmChannelRelation x, VmChannelRelation y)
            {
                return x.ConnectedChannel?.RootId == y.ConnectedChannel?.RootId;
            }

            public int GetHashCode(VmChannelRelation obj)
            {
                return obj.ConnectedChannel?.RootId.GetHashCode() ?? obj.Id.GetHashCode();
            }
        }
    }
}
