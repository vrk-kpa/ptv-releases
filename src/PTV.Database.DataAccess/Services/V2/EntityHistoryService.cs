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

using System;
using System.Collections.Generic;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Framework;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Database.DataAccess.Utils;
using PTV.Domain.Model.Models.V2.Organization;

namespace PTV.Database.DataAccess.Services.V2
{
    [Framework.RegisterService(typeof(IEntityHistoryService), RegisterType.Transient)]
    internal class EntityHistoryService : ServiceBase, IEntityHistoryService
    {
        private readonly IContextManager contextManager;
        private readonly ILanguageOrderCache languageOrderCache;
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly IOrganizationTreeDataCacheWithEntity organizationCache;

        private const int MAX_RESULTS = 10;

        public EntityHistoryService(
            IContextManager contextManager,
            IVersioningManager versioningManager,
            ICacheManager cacheManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IOrganizationTreeDataCache organizationCache
        ) : base(
            translationManagerToVm,
            translationManagerToEntity,
            publishingStatusCache,
            userOrganizationChecker,
            versioningManager
        ) {
            this.contextManager = contextManager;
            languageOrderCache = cacheManager.LanguageOrderCache;
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
            this.organizationCache = organizationCache as IOrganizationTreeDataCacheWithEntity;
        }

        // Channel //
        public IVmSearchBase GetChannelConnectionHistory(IVmHistorySearch search)
        {
            return contextManager.ExecuteReader(unitOfWork => GetChannelConnectionHistory(search, unitOfWork));
        }
        private IVmSearchBase GetChannelConnectionHistory(IVmHistorySearch search, IUnitOfWork unitOfWork)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingServiceServiceChannelRepository>();
            var serviceVersionedRepository = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var pageNumber = search.PageNumber.PositiveOrZero();

            var resultTemp = trackingRepository.All()
                .Where(x => x.OperationType == EntityState.Added.ToString() || x.OperationType == EntityState.Deleted.ToString())
                .Where(x => x.ChannelId == search.Id)
                .OrderByDescending(x => x.Created);

            var operations = resultTemp
                .ApplyPaging(pageNumber, MAX_RESULTS);

            var serviceIds = operations.SearchResult
                .Select(operation => VersioningManager
                    .GetLastVersion<ServiceVersioned>(unitOfWork, operation.ServiceId)
                    .EntityId
                )
                .Distinct()
                .ToList();

            var versionedServices = serviceVersionedRepository.All()
                .Include(x => x.ServiceNames)
                .Include(x => x.LanguageAvailabilities)
                .Where(x => serviceIds.Contains(x.Id))
                .ToList();

            return new VmSearchResult<VmConnectionOperation>
            {
                SearchResult = new VmListItemsData<VmConnectionOperation>(
                    operations.SearchResult
                        .Select(operation =>
                        {
                            var versionedService = versionedServices
                                .FirstOrDefault(x => operation.ChannelId == search.Id && operation.ServiceId == x.UnificRootId);
                            return new VmConnectionOperation
                            {
                                Id = operation.Id,
                                Name = versionedService
                                    .ServiceNames
                                    .Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()))
                                    .ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name),
                                LanguagesAvailabilities = TranslationManagerToVm
                                    .TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                                        versionedService
                                            .LanguageAvailabilities
                                            .OrderBy(x => languageOrderCache.Get(x.LanguageId))
                                    ),
                                OperationType = operation.OperationType,
                                CreatedBy = operation.CreatedBy,
                                Created = operation.Created.ToEpochTime(),
                                PublishingStatusId = versionedService.PublishingStatusId,
                                EntityType = EntityTypeEnum.Service,
                                SubEntityType = versionedService.TypeId,
                                EntityId = versionedService.Id,
                            };
                        })
                    ),
                Count = resultTemp.Count(),
                MoreAvailable = operations.MoreAvailable,
                PageNumber = ++pageNumber
            };
        }

        public IVmSearchBase GetChannelEntityHistory(IVmHistorySearch search)
        {
            return contextManager.ExecuteReader(unitOfWork => GetChannelEntityHistory(search, unitOfWork));
        }
        private IVmSearchBase GetChannelEntityHistory(IVmHistorySearch search, IUnitOfWork unitOfWork)
        {
            var pageNumber = search.PageNumber.PositiveOrZero();
            var resultTemp = GetEntityHistoryQuery(search, unitOfWork);
            var versionings = resultTemp
                    .OrderByDescending(x => x.Created).ThenByDescending(x=>x.VersionMajor).ThenByDescending(x=>x.VersionMinor)
                    .ApplyPaging(pageNumber, MAX_RESULTS);
            if (search.Id != null) PolyfillChannels(versionings, search.Id.Value, unitOfWork);

            var rowCount = resultTemp.Count();
            return GetEntityHistoryResult(rowCount, pageNumber, versionings);
        }
       
        private Dictionary<(int, int), ServiceChannelVersioned> GetChannelVersions(Guid versionId, IUnitOfWork unitOfWork)
        {
            var versioningsRepository = unitOfWork.CreateRepository<IVersioningRepository>();
            var entityUnificRootId = versioningsRepository
                .All().Where(x => x.Id == versionId && x.Ignored == false && x.Channels.Any())
                .Include(x => x.Channels)
                .First()
                .Channels
                .First()
                .UnificRootId;
            var serviceChannelRepository = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            return serviceChannelRepository.All()
                .Where(x => x.UnificRootId == entityUnificRootId)
                .Include(x => x.ServiceChannelNames)
                .Include(x => x.LanguageAvailabilities)
                .Include(x => x.Versioning)
                .OrderByDescending(x=>x.Versioning.VersionMajor)
                .ThenByDescending(x=>x.Versioning.VersionMinor)
                .ToDictionary(k => (k.Versioning.VersionMajor, k.Versioning.VersionMinor), v => v);

        }
        private void PolyfillChannels(VmSearchResult<Versioning> versionings, Guid versionId, IUnitOfWork unitOfWork)
        {
            var allVersions = GetChannelVersions(versionId, unitOfWork);
            foreach (var versioning in versionings.SearchResult.Where(x => x.Channels.IsNullOrEmpty()))
            {
                var polyfillChannel = allVersions.TryGetOrDefault((versioning.VersionMajor, versioning.VersionMinor)) ??
                                      allVersions.TryGetOrDefault((versioning.VersionMajor, 0));

                if (polyfillChannel == null)
                {
                    var prevVersion = allVersions.FirstOrDefault(x => x.Key.Item1 <= versioning.VersionMajor);
                    polyfillChannel = prevVersion.Value ?? allVersions.LastOrDefault(x => x.Key.Item1 > versioning.VersionMajor).Value;
                }

                if (polyfillChannel != null)
                {
                    versioning.Channels.Add(polyfillChannel);
                }
            }
        }

        // Service //
        public IVmSearchBase GetServiceConnectionHistory(IVmHistorySearch search)
        {
            return contextManager.ExecuteReader(unitOfWork => GetServiceConnectionHistory(search, unitOfWork));
        }
        private IVmSearchBase GetServiceConnectionHistory(IVmHistorySearch search, IUnitOfWork unitOfWork)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingServiceServiceChannelRepository>();
            var channelVersionedRepository = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var pageNumber = search.PageNumber.PositiveOrZero();

            var resultTemp = trackingRepository.All()
                .Where(x => x.OperationType == EntityState.Added.ToString() || x.OperationType == EntityState.Deleted.ToString())
                .Where(x => x.ServiceId == search.Id)
                .OrderByDescending(x => x.Created).ThenBy(x=>x.Id);

            var operations = resultTemp
                .ApplyPaging(pageNumber, MAX_RESULTS);

            var channelIds = operations.SearchResult
                .Select(operation => VersioningManager
                    .GetLastVersion<ServiceChannelVersioned>(unitOfWork, operation.ChannelId)?
                    .EntityId
                )
                .Distinct()
                .ToList();

            var versionedChannels = channelVersionedRepository.All()
                .Include(x => x.ServiceChannelNames)
                .Include(x => x.LanguageAvailabilities)
                .Include(x => x.DisplayNameTypes)
                .Where(x => channelIds.Contains(x.Id))
                .ToList();

            var versionedIds = versionedChannels.Select(x => x.UnificRootId);

            return new VmSearchResult<VmConnectionOperation>
            {
                SearchResult = new VmListItemsData<VmConnectionOperation>(
                    operations.SearchResult
                        .Where(x=>versionedIds.Contains(x.ChannelId)) // some channels can be permanently removed
                        .Select(operation =>
                        {
                            var versionedChannel = versionedChannels
                                .FirstOrDefault(x => x.UnificRootId == operation.ChannelId && operation.ServiceId == search.Id);
                            return new VmConnectionOperation
                            {
                                Id = operation.Id,
                                Name = versionedChannel
                                    .ServiceChannelNames
                                    .Where(j => versionedChannel.DisplayNameTypes.Any(dt=> dt.DisplayNameTypeId == j.TypeId && dt.LocalizationId == j.LocalizationId) ||
                                                versionedChannel.DisplayNameTypes.All(dt=> dt.LocalizationId != j.LocalizationId))
                                    .ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name),
                                LanguagesAvailabilities = TranslationManagerToVm
                                    .TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                                        versionedChannel
                                            .LanguageAvailabilities
                                            .OrderBy(x => languageOrderCache.Get(x.LanguageId))
                                    ),
                                OperationType = operation.OperationType,
                                CreatedBy = operation.CreatedBy,
                                Created = operation.Created.ToEpochTime(),
                                PublishingStatusId = versionedChannel.PublishingStatusId,
                                EntityType = EntityTypeEnum.Channel,
                                SubEntityType = versionedChannel.TypeId,
                                EntityId = versionedChannel.Id,
                            };
                        })
                    ),
                Count = resultTemp.Count(),
                MoreAvailable = operations.MoreAvailable,
                PageNumber = ++pageNumber
            };
        }

        public IVmSearchBase GetServiceEntityHistory(IVmHistorySearch search)
        {
            return contextManager.ExecuteReader(unitOfWork => GetServiceEntityHistory(search, unitOfWork));
        }

        private Dictionary<(int, int), ServiceVersioned> GetServiceVersions(Guid versionId, IUnitOfWork unitOfWork)
        {
            var versioningsRepository = unitOfWork.CreateRepository<IVersioningRepository>();
            var entityUnificRootId = versioningsRepository
                .All().Where(x => x.Id == versionId && x.Ignored == false && x.Services.Any())
                .Include(x => x.Services)
                .First()
                .Services
                .First()
                .UnificRootId;
            var serviceRepository = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            return serviceRepository.All()
                .Where(x => x.UnificRootId == entityUnificRootId)
                .Include(x => x.ServiceNames)
                .Include(x => x.LanguageAvailabilities)
                .Include(x => x.Versioning)
                .OrderByDescending(x=>x.Versioning.VersionMajor)
                .ThenByDescending(x=>x.Versioning.VersionMinor)
                .ToDictionary(k => (k.Versioning.VersionMajor, k.Versioning.VersionMinor), v => v);

        }

        private IVmSearchBase GetServiceEntityHistory(IVmHistorySearch search, IUnitOfWork unitOfWork)
        {
            var pageNumber = search.PageNumber.PositiveOrZero();
            var resultTemp = GetEntityHistoryQuery(search, unitOfWork);
            var versionings = resultTemp
                .OrderByDescending(x => x.Created).ThenByDescending(x=>x.VersionMajor).ThenByDescending(x=>x.VersionMinor)
                .ApplyPaging(pageNumber, MAX_RESULTS);
            if (search.Id != null) PolyfillServices(versionings, search.Id.Value, unitOfWork);

            var rowCount = resultTemp.Count();
            return GetEntityHistoryResult(rowCount, pageNumber, versionings);
        }

        private void PolyfillServices(VmSearchResult<Versioning> versionings, Guid versionId, IUnitOfWork unitOfWork)
        {
            var allVersions = GetServiceVersions(versionId, unitOfWork);
            foreach (var versioning in versionings.SearchResult.Where(x => x.Services.IsNullOrEmpty()))
            {
                var polyfillService = allVersions.TryGetOrDefault((versioning.VersionMajor, versioning.VersionMinor)) ??
                                      allVersions.TryGetOrDefault((versioning.VersionMajor, 0));

                if (polyfillService == null)
                {
                    var prevVersion = allVersions.FirstOrDefault(x => x.Key.Item1 <= versioning.VersionMajor);
                    polyfillService = prevVersion.Value ?? allVersions.LastOrDefault(x => x.Key.Item1 > versioning.VersionMajor).Value;
                }

                if (polyfillService != null)
                {
                    versioning.Services.Add(polyfillService);
                }
            }
        }

        // Organization //
        public IVmSearchBase GetOrganizationEntityHistory(IVmHistorySearch search)
        {
            return contextManager.ExecuteReader(unitOfWork => GetOrganizationEntityHistory(search, unitOfWork));
        }
        private IVmSearchBase GetOrganizationEntityHistory(IVmHistorySearch search, IUnitOfWork unitOfWork)
        {
            var pageNumber = search.PageNumber.PositiveOrZero();
            var resultTemp = GetEntityHistoryQuery(search, unitOfWork);
            var psRemoved = typesCache.Get<PublishingStatusType>(PublishingStatus.Removed.ToString());
            var versionings = resultTemp.Where(i => i.Services.All(j => j.PublishingStatusId != psRemoved))
                .OrderByDescending(x => x.Created).ThenByDescending(x=>x.VersionMajor).ThenByDescending(x=>x.VersionMinor)
                .ApplyPaging(pageNumber, MAX_RESULTS);
            if (search.Id != null) PolyfillOrganizations(versionings, search.Id.Value, unitOfWork);

            var rowCount = resultTemp.Count();
            return GetEntityHistoryResult(rowCount, pageNumber, versionings);
        }

        private Dictionary<(int, int), OrganizationVersioned> GetOrganizationVersions(Guid versionId, IUnitOfWork unitOfWork)
        {
            var versioningsRepository = unitOfWork.CreateRepository<IVersioningRepository>();
            var entityUnificRootId = versioningsRepository
                .All().Where(x => x.Id == versionId && x.Ignored == false && x.Organizations.Any())
                .Include(x => x.Organizations)
                .First()
                .Organizations
                .First()
                .UnificRootId;
            var organizationRepository = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            return organizationRepository.All()
                .Where(x => x.UnificRootId == entityUnificRootId)
                .Include(x => x.OrganizationNames)
                .Include(x => x.LanguageAvailabilities)
                .Include(x => x.Versioning)
                .OrderByDescending(x=>x.Versioning.VersionMajor)
                .ThenByDescending(x=>x.Versioning.VersionMinor)
                .ToDictionary(k => (k.Versioning.VersionMajor, k.Versioning.VersionMinor), v => v);

        }
        private void PolyfillOrganizations(VmSearchResult<Versioning> versionings, Guid versionId, IUnitOfWork unitOfWork)
        {
            var allVersions = GetOrganizationVersions(versionId, unitOfWork);
            foreach (var versioning in versionings.SearchResult.Where(x => x.Organizations.IsNullOrEmpty()))
            {
                var polyfillOrganization = allVersions.TryGetOrDefault((versioning.VersionMajor, versioning.VersionMinor)) ??
                                           allVersions.TryGetOrDefault((versioning.VersionMajor, 0));

                if (polyfillOrganization == null)
                {
                    var prevVersion = allVersions.FirstOrDefault(x => x.Key.Item1 <= versioning.VersionMajor);
                    polyfillOrganization = prevVersion.Value ?? allVersions.LastOrDefault(x => x.Key.Item1 > versioning.VersionMajor).Value;
                }

                if (polyfillOrganization != null)
                {
                    versioning.Organizations.Add(polyfillOrganization);
                }
            }
        }

        // ServiceCollection //
        public IVmSearchBase GetServiceCollectionConnectionHistory(IVmHistorySearch search)
        {
            return contextManager.ExecuteReader(unitOfWork => GetServiceCollectionConnectionHistory(search, unitOfWork));
        }
        private IVmSearchBase GetServiceCollectionConnectionHistory(IVmHistorySearch search, IUnitOfWork unitOfWork)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingServiceCollectionServiceRepository>();
            var connectedEntitiesVersionedRepository = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var pageNumber = search.PageNumber.PositiveOrZero();

            var resultTemp = trackingRepository.All()
                .Where(x => x.ServiceCollectionId == search.Id)
                .OrderByDescending(x => x.Created);

            var operations = resultTemp
                .ApplyPaging(pageNumber, MAX_RESULTS);

            var connectedEntitesIds = operations.SearchResult
                .Select(operation => VersioningManager
                    .GetLastVersion<ServiceVersioned>(unitOfWork, operation.ServiceId)
                    .EntityId
                )
                .Distinct()
                .ToList();

            var versionedConnectedEntities = connectedEntitiesVersionedRepository.All()
                .Include(x => x.ServiceNames)
                .Include(x => x.LanguageAvailabilities)
                .Where(x => connectedEntitesIds.Contains(x.Id))
                .ToList();

            return new VmSearchResult<VmConnectionOperation>
            {
                SearchResult = new VmListItemsData<VmConnectionOperation>(
                    operations.SearchResult
                        .Select(operation =>
                        {
                            var versionedConnectedEntity = versionedConnectedEntities
                                .FirstOrDefault(x => operation.ServiceCollectionId == search.Id && operation.ServiceId == x.UnificRootId);
                            return new VmConnectionOperation
                            {
                                Id = operation.Id,
                                Name = versionedConnectedEntity
                                    .ServiceNames
                                    .Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()))
                                    .ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name),
                                LanguagesAvailabilities = TranslationManagerToVm
                                    .TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                                        versionedConnectedEntity
                                            .LanguageAvailabilities
                                            .OrderBy(x => languageOrderCache.Get(x.LanguageId))
                                    ),
                                OperationType = operation.OperationType,
                                CreatedBy = operation.CreatedBy,
                                Created = operation.Created.ToEpochTime(),
                                PublishingStatusId = versionedConnectedEntity.PublishingStatusId,
                                EntityType = EntityTypeEnum.Service,
                                SubEntityType = versionedConnectedEntity.TypeId,
                                EntityId = versionedConnectedEntity.Id,
                            };
                        })
                    ),
                Count = resultTemp.Count(),
                MoreAvailable = operations.MoreAvailable,
                PageNumber = ++pageNumber
            };
        }

        public IVmSearchBase GetServiceCollectionEntityHistory(IVmHistorySearch search)
        {
            return contextManager.ExecuteReader(unitOfWork => GetServiceCollectionEntityHistory(search, unitOfWork));
        }
        private IVmSearchBase GetServiceCollectionEntityHistory(IVmHistorySearch search, IUnitOfWork unitOfWork)
        {
            var pageNumber = search.PageNumber.PositiveOrZero();
            var resultTemp = GetEntityHistoryQuery(search, unitOfWork);
            var versionings = resultTemp
                .OrderByDescending(x => x.Created).ThenByDescending(x=>x.VersionMajor).ThenByDescending(x=>x.VersionMinor)
                .ApplyPaging(pageNumber, MAX_RESULTS);
            if (search.Id != null) PolyfillServiceCollections(versionings, search.Id.Value, unitOfWork);

            var rowCount = resultTemp.Count();
            return GetEntityHistoryResult(rowCount, pageNumber, versionings);
        }
        
        private Dictionary<(int, int), ServiceCollectionVersioned> GetServiceCollectionVersions(Guid versionId, IUnitOfWork unitOfWork)
        {
            var versioningsRepository = unitOfWork.CreateRepository<IVersioningRepository>();
            var entityUnificRootId = versioningsRepository
                .All().Where(x => x.Id == versionId && x.Ignored == false && x.ServiceCollections.Any())
                .Include(x => x.ServiceCollections)
                .First()
                .ServiceCollections
                .First()
                .UnificRootId;
            var serviceCollectionRepository = unitOfWork.CreateRepository<IServiceCollectionVersionedRepository>();
            return serviceCollectionRepository.All()
                .Where(x => x.UnificRootId == entityUnificRootId)
                .Include(x => x.ServiceCollectionNames)
                .Include(x => x.LanguageAvailabilities)
                .Include(x => x.Versioning)
                .OrderByDescending(x=>x.Versioning.VersionMajor)
                .ThenByDescending(x=>x.Versioning.VersionMinor)
                .ToDictionary(k => (k.Versioning.VersionMajor, k.Versioning.VersionMinor), v => v);

        }
        
        private void PolyfillServiceCollections(VmSearchResult<Versioning> versionings, Guid versionId, IUnitOfWork unitOfWork)
        {
            var allVersions = GetServiceCollectionVersions(versionId, unitOfWork);
            foreach (var versioning in versionings.SearchResult.Where(x => x.ServiceCollections.IsNullOrEmpty()))
            {
                var polyfillServiceCollection = allVersions.TryGetOrDefault((versioning.VersionMajor, versioning.VersionMinor)) ??
                                      allVersions.TryGetOrDefault((versioning.VersionMajor, 0));

                if (polyfillServiceCollection == null)
                {
                    var prevVersion = allVersions.FirstOrDefault(x => x.Key.Item1 <= versioning.VersionMajor);
                    polyfillServiceCollection = prevVersion.Value ?? allVersions.LastOrDefault(x => x.Key.Item1 > versioning.VersionMajor).Value;
                }

                if (polyfillServiceCollection != null)
                {
                    versioning.ServiceCollections.Add(polyfillServiceCollection);
                }
            }
        }

        // General description
        public IVmSearchBase GetGeneralDescriptionEntityHistory(IVmHistorySearch search)
        {
            return contextManager.ExecuteReader(unitOfWork => GetGeneralDescriptionEntityHistory(search, unitOfWork));
        }
        private IVmSearchBase GetGeneralDescriptionEntityHistory(IVmHistorySearch search, IUnitOfWork unitOfWork)
        {
            var pageNumber = search.PageNumber.PositiveOrZero();
            var resultTemp = GetEntityHistoryQuery(search, unitOfWork);
            var versionings = resultTemp
                .OrderByDescending(x => x.Created).ThenByDescending(x=>x.VersionMajor).ThenByDescending(x=>x.VersionMinor)
                .ApplyPaging(pageNumber, MAX_RESULTS);
            if (search.Id != null) PolyfillGeneralDescriptions(versionings, search.Id.Value, unitOfWork);

            var rowCount = resultTemp.Count();
            return GetEntityHistoryResult(rowCount, pageNumber, versionings);
        }

        private Dictionary<(int, int), StatutoryServiceGeneralDescriptionVersioned> GetGeneralDescriptionVersions(Guid versionId, IUnitOfWork unitOfWork)
        {
            var versioningsRepository = unitOfWork.CreateRepository<IVersioningRepository>();
            var entityUnificRootId = versioningsRepository
                .All().Where(x => x.Id == versionId && x.Ignored == false && x.GeneralDescriptions.Any())
                .Include(x => x.GeneralDescriptions)
                .First()
                .GeneralDescriptions
                .First()
                .UnificRootId;
            var generalDescriptionRepository = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            return generalDescriptionRepository.All()
                .Where(x => x.UnificRootId == entityUnificRootId)
                .Include(x => x.Names)
                .Include(x => x.LanguageAvailabilities)
                .Include(x => x.Versioning)
                .OrderByDescending(x=>x.Versioning.VersionMajor)
                .ThenByDescending(x=>x.Versioning.VersionMinor)
                .ToDictionary(k => (k.Versioning.VersionMajor, k.Versioning.VersionMinor), v => v);

        }
        private void PolyfillGeneralDescriptions(VmSearchResult<Versioning> versionings, Guid versionId, IUnitOfWork unitOfWork)
        {
            var allVersions = GetGeneralDescriptionVersions(versionId, unitOfWork);
            foreach (var versioning in versionings.SearchResult.Where(x => x.GeneralDescriptions.IsNullOrEmpty()))
            {
                var polyfillGeneralDescription = allVersions.TryGetOrDefault((versioning.VersionMajor, versioning.VersionMinor)) ??
                                                allVersions.TryGetOrDefault((versioning.VersionMajor, 0));

                if (polyfillGeneralDescription == null)
                {
                    var prevVersion = allVersions.FirstOrDefault(x => x.Key.Item1 <= versioning.VersionMajor);
                    polyfillGeneralDescription = prevVersion.Value ?? allVersions.LastOrDefault(x => x.Key.Item1 > versioning.VersionMajor).Value;
                }

                if (polyfillGeneralDescription != null)
                {
                    versioning.GeneralDescriptions.Add(polyfillGeneralDescription);
                }
            }
        }

        public IVmSearchBase GetGeneralDescriptionConnectionHistory(IVmHistorySearch search)
        {
            return contextManager.ExecuteReader(unitOfWork => GetGeneralDescriptionConnectionHistory(search, unitOfWork));
        }
        private IVmSearchBase GetGeneralDescriptionConnectionHistory(IVmHistorySearch search, IUnitOfWork unitOfWork)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingGeneralDescriptionServiceChannelRepository>();
            var connectedEntitiesVersionedRepository = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var pageNumber = search.PageNumber.PositiveOrZero();

            var resultTemp = trackingRepository.All()
                .Where(x => x.GeneralDescriptionId == search.Id)
                .OrderByDescending(x => x.Created);

            var operations = resultTemp
                .ApplyPaging(pageNumber, MAX_RESULTS);

            var connectedEntitesIds = operations.SearchResult
                .Select(operation => VersioningManager
                    .GetLastVersion<ServiceChannelVersioned>(unitOfWork, operation.ChannelId)
                    .EntityId
                )
                .Distinct()
                .ToList();

            var versionedConnectedEntities = connectedEntitiesVersionedRepository.All()
                .Include(x => x.ServiceChannelNames)
                .Include(x => x.LanguageAvailabilities)
                .Include(x => x.DisplayNameTypes)
                .Where(x => connectedEntitesIds.Contains(x.Id))
                .ToList();

            return new VmSearchResult<VmConnectionOperation>
            {
                SearchResult = new VmListItemsData<VmConnectionOperation>(
                    operations.SearchResult
                        .Select(operation =>
                        {
                            var versionedConnectedEntity = versionedConnectedEntities
                                .FirstOrDefault(x => operation.GeneralDescriptionId == search.Id && operation.ChannelId == x.UnificRootId);
                            return new VmConnectionOperation
                            {
                                Id = operation.Id,
                                Name = versionedConnectedEntity
                                    .ServiceChannelNames
                                    .Where(j => versionedConnectedEntity.DisplayNameTypes.Any(dt=> dt.DisplayNameTypeId == j.TypeId && dt.LocalizationId == j.LocalizationId) ||
                                                versionedConnectedEntity.DisplayNameTypes.All(dt=> dt.LocalizationId != j.LocalizationId))
                                    .ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name),
                                LanguagesAvailabilities = TranslationManagerToVm
                                    .TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                                        versionedConnectedEntity
                                            .LanguageAvailabilities
                                            .OrderBy(x => languageOrderCache.Get(x.LanguageId))
                                    ),
                                OperationType = operation.OperationType,
                                CreatedBy = operation.CreatedBy,
                                Created = operation.Created.ToEpochTime(),
                                PublishingStatusId = versionedConnectedEntity.PublishingStatusId,
                                EntityType = EntityTypeEnum.Channel,
                                SubEntityType = versionedConnectedEntity.TypeId,
                                EntityId = versionedConnectedEntity.Id,
                            };
                        })
                    ),
                Count = resultTemp.Count(),
                MoreAvailable = operations.MoreAvailable,
                PageNumber = ++pageNumber
            };
        }

        // Entity history
        private IQueryable<Versioning> GetEntityHistoryQuery(IVmHistorySearch search, IUnitOfWork unitOfWork)
        {
            var versioningsRepository = unitOfWork.CreateRepository<IVersioningRepository>();
            var versioningsByVersionedId = versioningsRepository.All().Where(e => e.Id == search.Id && !e.Ignored).Select(e => e.UnificRootId);
            var mainsIgnored = versioningsRepository.All().Where(x => versioningsByVersionedId.Contains(x.UnificRootId) && x.Ignored && x.VersionMinor == 0).Select(j => j.VersionMajor);
            return versioningsRepository.All().Where(x => versioningsByVersionedId.Contains(x.UnificRootId) && !x.Ignored && !mainsIgnored.Contains(x.VersionMajor));

        }
        private IVmSearchBase GetEntityHistoryResult(int rowCount, int pageNumber, VmSearchResult<Versioning> tempResult)
        {
            var searchResult = new VmListItemsData<VmEntityOperation>(
                TranslationManagerToVm.TranslateAll<Versioning, VmEntityOperation>(tempResult.SearchResult)).Where(i => i != null).ToList();

            var templateOrganizationIds = searchResult
                .Where(o => o.TemplateOrganizationId.IsAssigned())
                .Select(o => o.TemplateOrganizationId)
                .Distinct()
                .ToHashSet();

            if (templateOrganizationIds.Any())
            {
                var organizationDictionary = organizationCache.GetFlatDataEntities()
                    .Where(o => templateOrganizationIds.Contains(o.Key))
                    .ToDictionary(o => o.Key, o => o.Value);

                foreach (var operation in searchResult.Where(i => i.TemplateOrganizationId.IsAssigned()))
                {
                    if (organizationDictionary.TryGetValue(operation.TemplateOrganizationId.Value,
                        out var organizationVersioned))
                    {
                        operation.TemplateOrganization = TranslationManagerToVm.Translate<OrganizationVersioned, VmOrganizationHeader>(organizationVersioned);
                    }
                }
            }

            var result = new VmSearchResult<VmEntityOperation>
            {
                SearchResult = searchResult,
                Count = rowCount,
                MoreAvailable = tempResult.MoreAvailable,
                PageNumber = ++pageNumber
            };
            return result;
        }
    }
}
