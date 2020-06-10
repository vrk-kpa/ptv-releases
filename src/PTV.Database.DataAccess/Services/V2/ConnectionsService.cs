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
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common.Connections;

namespace PTV.Database.DataAccess.Services.V2
{
    [Framework.RegisterService(typeof(IConnectionsService), RegisterType.Transient)]
    [Framework.RegisterService(typeof(IConnectionsServiceInternal), RegisterType.Transient)]
    internal partial class ConnectionsService : ServiceBase, IConnectionsService, IConnectionsServiceInternal
    {
        private readonly IContextManager contextManager;
        private ILanguageOrderCache languageOrderCache;
        private ITypesCache typesCache;
        private IServiceUtilities utilities;
        private ITranslationServiceInternal translationService;
        private IAddressService addressService;
        private readonly IUrlService urlService;

        public ConnectionsService(
            IContextManager contextManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IVersioningManager versioningManager,
            ICacheManager cacheManager,
            IServiceUtilities utilities,
            ITranslationServiceInternal translationService,
            IAddressService addressService,
            IUrlService urlService
        ) :
            base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker,
                versioningManager)
        {
            this.contextManager = contextManager;
            this.languageOrderCache = cacheManager.LanguageOrderCache;
            typesCache = cacheManager.TypesCache;
            this.utilities = utilities;
            this.translationService = translationService;
            this.addressService = addressService;
            this.urlService = urlService;
        }

        /// <summary>
        /// Collection of Publishing status types which are allowed when selecting connected services
        /// or service channels.
        /// </summary>
        private List<Guid> AllowedPublishingStatusTypes => new List<Guid>
        {
            typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString()),
            typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString()),
            typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString())
        };

        public VmConnectionsPageOutput SaveRelations(VmConnectionsPageInput model)
        {
            ProcessNewAddressesAndUrls(CreateConnectionInputModel(model));
            switch (model.MainEntityType)
            {
                case DomainEnum.Services:
                    ProcessNewServiceConnections(CreateConnectionInputModel(model).ToList());
                    break;
                case DomainEnum.Channels:
                    ProcessNewChannelConnections(CreateConnectionInputModel(model).ToList());
                    break;
                default:
                    break;
            }
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SaveRelations(unitOfWork, model);
                unitOfWork.Save();
            });
            return GetRelations(model);
        }

        private IEnumerable<VmConnectionsInput> CreateConnectionInputModel(VmConnectionsPageInput model)
        {
            return model.Connections.Select(connection =>
            {
                return new VmConnectionsInput
                {
                    Id = connection.MainEntity.Id,
                    UnificRootId = connection.MainEntity.UnificRootId,
                    SelectedConnections = connection.Childs,
                    UseOrder = true
                };
            });
        }

        private void SaveRelations(IUnitOfWorkWritable unitOfWork, VmConnectionsPageInput model)
        {
            var connections = CreateConnectionInputModel(model);
            switch (model.MainEntityType)
            {
                case DomainEnum.Services:
                    TranslationManagerToEntity.TranslateAll<VmConnectionsInput, Service>(connections, unitOfWork);
                    break;
                case DomainEnum.Channels:
                    TranslationManagerToEntity.TranslateAll<VmConnectionsInput, ServiceChannel>(connections, unitOfWork);
                    break;
                default:
                    break;
            }
            AddModifiedPropagationChain(unitOfWork);
        }

        private VmConnectionsPageOutput GetRelations(VmConnectionsPageInput model)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                return GetRelations(unitOfWork, model);
            });
        }

        private VmConnectionsPageOutput GetRelations(IUnitOfWork unitOfWork, VmConnectionsPageInput model)
        {
            var result = new VmConnectionsPageOutput();
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            if (model.MainEntityType == DomainEnum.Channels)
            {
                var serviceChannelIds = model.Connections.Select(i => i.MainEntity.Id).ToList();
                var channelLangAvailabilitiesRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
                var channelLangAvailabilities = channelLangAvailabilitiesRep.All().Where(x => serviceChannelIds.Contains(x.ServiceChannelVersionedId)).ToList()
                    .GroupBy(i => i.ServiceChannelVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());

                result.Channels = new List<VmChannelConnectionsOutput>();

                var serviceChannelsUnificRootIds = model.Connections.Select(i => i.MainEntity.UnificRootId).ToList();
                var serviceChannels = GetAllServiceChannelRelations(unitOfWork, serviceChannelsUnificRootIds);

                foreach (var connection in model.Connections)
                {
                   var connectionResult = new VmChannelConnectionsOutput
                    {
                        Id = connection.MainEntity.Id,
                        Connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, VmChannelConnectionOutput>(
                            serviceChannels.TryGetOrDefault(connection.MainEntity.UnificRootId, new List<ServiceServiceChannel>())
                                .OrderBy(x => x.ServiceOrderNumber))
                            .ToList(),
                        LanguagesAvailabilities = TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            channelLangAvailabilities.TryGetOrDefault(connection.MainEntity.Id, new List<ServiceChannelLanguageAvailability>())),
                        ChannelTypeId = connection.MainEntity.ChannelTypeId,
                        ChannelType = connection.MainEntity.ChannelTypeId.HasValue ? typesCache.GetByValue<ServiceChannelType>(connection.MainEntity.ChannelTypeId.Value) : string.Empty,
                    };
                    result.Channels.Add(connectionResult);
                }
            }
            if (model.MainEntityType == DomainEnum.Services)
            {
                result.Services = new List<VmServiceConnectionsOutput>();

                var serviceIds = model.Connections.Select(i => i.MainEntity.Id).ToList();
                var serviceLangAvailabilitiesRep = unitOfWork.CreateRepository<IServiceLanguageAvailabilityRepository>();
                var serviceLangAvailabilities = serviceLangAvailabilitiesRep.All().Where(x => serviceIds.Contains(x.ServiceVersionedId)).ToList()
                    .GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());

                var serviceUnificRootIds = model.Connections.Select(i => i.MainEntity.UnificRootId).ToList();

                var serviceChannels = GetAllServiceRelations(unitOfWork, serviceUnificRootIds);

                foreach (var connection in model.Connections)
                {
                    var connectionResult = new VmServiceConnectionsOutput
                    {
                        Id = connection.MainEntity.Id,
                        Connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, VmConnectionOutput>(
                            serviceChannels.TryGetOrDefault(connection.MainEntity.UnificRootId, new List<ServiceServiceChannel>()))
                            .OrderBy(x => x.ChannelOrderNumber)
                            .ThenBy(x => x.ConnectionOrderNumber)
                            .ToList(),
                        LanguagesAvailabilities = TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            serviceLangAvailabilities.TryGetOrDefault(connection.MainEntity.Id, new List<ServiceLanguageAvailability>()))
                    };
                    result.Services.Add(connectionResult);
                }
            }
            return result;
        }

        public Dictionary<Guid, List<ServiceServiceChannel>> GetAllServiceRelations(IUnitOfWork unitOfWork, List<Guid> unificRootIds)
        {
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var draftStatusId =typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId =typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var serviceRepo = unitOfWork.CreateRepository<IServiceVersionedRepository>();

            var serviceConnections = serviceRepo.All()
                .Include(x => x.UnificRoot)
                .ThenInclude(x => x.ServiceServiceChannels)
                .Where(x => unificRootIds.Contains(x.UnificRootId) && AllowedPublishingStatusTypes.Contains(x.PublishingStatusId))
                .SelectMany(x => x.UnificRoot.ServiceServiceChannels)
                .Distinct()
                .ToList()
                .GroupBy(i => i.ServiceId)
                .ToDictionary(i => i.Key, i => i.ToList());

            IncludeServiceCommonDetails(unitOfWork, serviceConnections);
            IncludeServiceContactDetails(unitOfWork, serviceConnections);
            IncludeServiceAddresses(unitOfWork, serviceConnections);

            var servicesChannelUnificIds = serviceConnections.Values.SelectMany(x => x.Select(y => y.ServiceChannelId)).Distinct();
            var channels = serviceChannelRep.All()
                .Include(x => x.ServiceChannelNames)
                .Include(j => j.LanguageAvailabilities)
                .Include(j => j.DisplayNameTypes)
                .Include(j => j.Type)
                .Where(x => servicesChannelUnificIds.Contains(x.UnificRootId) && AllowedPublishingStatusTypes.Contains(x.PublishingStatusId))
                .ToList()
                .GroupBy(x => x.UnificRootId).Select(x =>
                    x.OrderBy(y =>
                        y.PublishingStatusId == publishingStatusId ? 0 :
                        y.PublishingStatusId == draftStatusId ? 1 :
                        y.PublishingStatusId == modifiedStatusId ? 2 : 3).FirstOrDefault())
                .ToDictionary(x => x.UnificRootId, y => y);

            var result = new Dictionary<Guid, List<ServiceServiceChannel>>();

            foreach (var connections in serviceConnections)
            {
                var subResult = new List<ServiceServiceChannel>();
                foreach (var connection in connections.Value)
                {
                    var channelVersion = channels.TryGetOrDefault(connection.ServiceChannelId);
                    if (channelVersion == null)
                        continue;

                    connection.ServiceChannel = new ServiceChannel {Versions = new List<ServiceChannelVersioned> {channelVersion}};
                    subResult.Add(connection);
                }

                if (subResult.Any())
                {
                    result.Add(connections.Key, subResult);
                }
            }

            return result;
        }
        public Dictionary<Guid, List<ServiceServiceChannel>> GetAllServiceChannelRelations(IUnitOfWork unitOfWork, List<Guid> unificRootIds)
        {
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var draftStatusId =typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId =typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var channelRepo = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();

            var channelConnections = channelRepo.All()
                .Include(x => x.UnificRoot)
                .ThenInclude(x => x.ServiceServiceChannels)
                .Where(x => unificRootIds.Contains(x.UnificRootId) &&
                            AllowedPublishingStatusTypes.Contains(x.PublishingStatusId))
                .SelectMany(x => x.UnificRoot.ServiceServiceChannels)
                .Distinct()
                .ToList()
                .GroupBy(x => x.ServiceChannelId)
                .ToDictionary(x => x.Key, x => x.ToList());

            IncludeChannelCommonDetails(unitOfWork, channelConnections);
            IncludeChannelContactDetails(unitOfWork, channelConnections);
            IncludeChannelAddresses(unitOfWork, channelConnections);

            var servicesUnificIds = channelConnections.Values.SelectMany(x => x.Select(y => y.ServiceId)).Distinct().ToList();
            var services = serviceRep.All()
                .Include(x => x.ServiceNames)
                .Include(j => j.LanguageAvailabilities)
                .Include(j => j.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions)
                .Where(x => servicesUnificIds.Contains(x.UnificRootId) && AllowedPublishingStatusTypes.Contains(x.PublishingStatusId))
                .ToList()
                .GroupBy(x => x.UnificRootId).Select(x =>
                    x.OrderBy(y =>
                        y.PublishingStatusId == publishingStatusId ? 0 :
                        y.PublishingStatusId == draftStatusId ? 1 :
                        y.PublishingStatusId == modifiedStatusId ? 2 : 3).FirstOrDefault())
                .ToDictionary(x => x.UnificRootId, y => y);

            var result = new Dictionary<Guid, List<ServiceServiceChannel>>();

            foreach (var connections in channelConnections)
            {
                var subResult = new List<ServiceServiceChannel>();
                foreach (var connection in connections.Value)
                {
                    var serviceVersion = services.TryGetOrDefault(connection.ServiceId);
                    if (serviceVersion == null)
                        continue;

                    connection.Service = new Service {Versions = new List<ServiceVersioned> {serviceVersion}};
                    subResult.Add(connection);
                }

                if (subResult.Any())
                {
                    result.Add(connections.Key, subResult);
                }
            }

            return result;
        }

        public VmConnectionsOutput SaveServiceRelations(VmConnectionsInput model)
        {
            ProcessNewServiceConnections(new List<VmConnectionsInput> {model});
            return SaveRelations<ServiceVersioned, Service>(model);
        }

        public VmConnectionsOutput SaveServiceChannelRelations(VmConnectionsInput model)
        {
            ProcessNewChannelConnections(new List<VmConnectionsInput> {model});
            return SaveRelations<ServiceChannelVersioned, ServiceChannel>(model);
        }

        private VmConnectionsOutput SaveRelations<TEntity, TBaseEntity>(VmConnectionsInput model) where TEntity : class, IVersionedVolume
            where TBaseEntity : class, IVersionedRoot
        {
            ProcessNewAddressesAndUrls(new List<VmConnectionsInput> {model});
            contextManager.ExecuteWriter(unitOfWork =>
            {
                //utilities.CheckIsEntityConnectable<TEntity>(model.Id, unitOfWork);
                SaveRelations<TEntity, TBaseEntity>(unitOfWork, model);
                unitOfWork.Save();
            });
            return GetRelations<TEntity>(model);
        }

        private void SetUnificRootIds<TEntity>(List<VmConnectionsInput> connections) where TEntity : class, IVersionedVolume
        {
            contextManager.ExecuteReader(unitOfWork =>
            {
                foreach (var mainEntity in connections.Where(x=>!x.UnificRootId.IsAssigned()))
                {
                    var unificRootId = VersioningManager
                        .GetUnificRootId<TEntity>(unitOfWork, mainEntity.Id);
                    if (unificRootId.HasValue)
                    {
                        mainEntity.UnificRootId = unificRootId.Value;
                    }
                }

            });
        }

        private void ProcessNewServiceConnections(List<VmConnectionsInput> connections)
        {
            SetUnificRootIds<ServiceVersioned>(connections);
            foreach (var parent in connections.Where(x=>x.UnificRootId.IsAssigned()))
            {
                foreach (var child in parent.SelectedConnections)
                {
                    contextManager.ExecuteWriter(unitOfWork =>
                    {
                        utilities.CheckIsEntityConnectable<ServiceVersioned>(parent.Id, unitOfWork);
                        var model = new VmConnectionInput
                        {
                            MainEntityId = parent.UnificRootId,
                            ConnectedEntityId = child.ConnectedEntityId
                        };
                        TranslationManagerToEntity.Translate<VmConnectionInput, ServiceServiceChannel>(model, unitOfWork);
                        unitOfWork.Save();
                    });
                }
            }
        }

        private void ProcessNewChannelConnections(List<VmConnectionsInput> connections)
        {
            SetUnificRootIds<ServiceChannelVersioned>(connections);
            foreach (var parent in connections.Where(x=>x.UnificRootId.IsAssigned()))
            {
                foreach (var child in parent.SelectedConnections)
                {
                    contextManager.ExecuteWriter(unitOfWork =>
                    {
                        utilities.CheckIsEntityConnectable<ServiceChannelVersioned>(parent.Id, unitOfWork);
                        var model = new VmConnectionInput
                        {
                            MainEntityId = child.ConnectedEntityId,
                            ConnectedEntityId =  parent.UnificRootId
                        };
                        TranslationManagerToEntity.Translate<VmConnectionInput, ServiceServiceChannel>(model, unitOfWork);
                        unitOfWork.Save();
                    });
                }
            }
        }

        private void SaveRelations<TEntity, TBaseEntity>(IUnitOfWorkWritable unitOfWork, VmConnectionsInput model) where TEntity : class, IVersionedVolume
            where TBaseEntity : class, IVersionedRoot
        {
            var unificRootId = VersioningManager.GetUnificRootId<TEntity>(unitOfWork, model.Id);
            if (unificRootId.HasValue)
            {
                model.UnificRootId = unificRootId.Value;
                TranslationManagerToEntity.Translate<VmConnectionsInput, TBaseEntity>(model, unitOfWork);
                AddModifiedPropagationChain(unitOfWork);
            }
        }

        private void ProcessNewAddressesAndUrls(IEnumerable<VmConnectionsInput> model)
        {
            Dictionary<(Guid, string), Guid> newStreets = new Dictionary<(Guid, string), Guid>();
            var addresses = model.SelectMany(x => x.SelectedConnections)
                .Where(x => x.ContactDetails != null && x.ContactDetails.PostalAddresses.Any())
                .SelectMany(x => x.ContactDetails.PostalAddresses).ToList();

            var urls = model.SelectMany(x => x.SelectedConnections)
                .Where(x => x.ContactDetails?.WebPages?.Any() == true).SelectMany(x =>
                    x.ContactDetails.WebPages.SelectMany(y => y.Value.Select(z => z.UrlAddress)))
                .ToList();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                urlService.AddNewUrls(unitOfWork, urls);
                addressService.AddNewStreetAddresses(unitOfWork, addresses, newStreets);
                unitOfWork.Save();
            });
            contextManager.ExecuteWriter(unitOfWork =>
            {
                addressService.AddNewStreetAddressNumbers(unitOfWork, addresses, newStreets);
                unitOfWork.Save();
            });
        }

        public VmConnectionsOutput GetChannelRelations(VmConnectionsInput model)
        {
            return GetRelations<ServiceChannelVersioned>(model);
        }

        private VmConnectionsOutput GetRelations<TEntity>(VmConnectionsInput model) where TEntity : class, IVersionedVolume
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                VmConnectionsOutput result = null;
                if (typeof(TEntity) == typeof(ServiceVersioned))
                {
                    result = GetServiceRelations(unitOfWork, model);
                } else if (typeof(TEntity) == typeof(ServiceChannelVersioned))
                {
                    result = GetChannelRelations(unitOfWork, model);
                }

                return result;
            });
        }

        private VmServiceConnectionsOutput GetServiceRelations(IUnitOfWork unitOfWork, VmConnectionsInput model)
        {
            var serviceLangAvailabilitiesRep = unitOfWork.CreateRepository<IServiceLanguageAvailabilityRepository>();
            var unificRootId = VersioningManager.GetUnificRootId<ServiceVersioned>(unitOfWork, model.Id);
            if (unificRootId.HasValue)
            {
                var relations = GetAllServiceRelations(unitOfWork, new List<Guid> { unificRootId.Value });
                var result = new VmServiceConnectionsOutput
                {
                    Connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, VmConnectionOutput>(
                        relations.TryGetOrDefault(unificRootId.Value, new List<ServiceServiceChannel>()))
                        .OrderBy(x => x.ChannelOrderNumber)
                        .ThenBy(x=>x.ConnectionOrderNumber)
                        .ToList(),
                    Id = model.Id,
                    LanguagesAvailabilities = TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                        serviceLangAvailabilitiesRep.All()
                            .Where(x => model.Id == x.ServiceVersionedId)
                            .ToList()
                            .OrderBy(x => languageOrderCache.Get(x.LanguageId))
                            .ToList())
                };
                result.NumberOfConnections = result.Connections.Count;
                return result;
            }
            return null;
        }

        private VmChannelConnectionsTranslationOutput GetChannelRelations(IUnitOfWork unitOfWork, VmConnectionsInput model)
        {
            var channelLangAvailabilitiesRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
            var unificRootId = VersioningManager.GetUnificRootId<ServiceChannelVersioned>(unitOfWork, model.Id);
            if (unificRootId.HasValue)
            {
                var relations = GetAllServiceChannelRelations(unitOfWork, new List<Guid> { unificRootId.Value });
                var result = new VmChannelConnectionsTranslationOutput
                {
                    Connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, VmChannelConnectionOutput>(
                           relations.TryGetOrDefault(unificRootId.Value, new List<ServiceServiceChannel>()))
                        .OrderBy(x => x.ServiceOrderNumber)
                        .ToList(),
                    LanguagesAvailabilities = TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                        channelLangAvailabilitiesRep.All()
                            .Where(x => model.Id == x.ServiceChannelVersionedId)
                            .ToList()
                            .OrderBy(x => languageOrderCache.Get(x.LanguageId))
                            .ToList()),
                    Id = model.Id
                };
                result.TranslationAvailability = translationService.GetChannelTranslationAvailabilities(unitOfWork, result.Id, unificRootId.Value, result.Connections);
                result.NumberOfConnections = result.Connections.Count;
                return result;
            }
            return null;
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
    }
}
