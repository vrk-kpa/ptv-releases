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
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Views;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Database.DataAccess.Interfaces.Caches;
using IOrganizationServiceInternal = PTV.Database.DataAccess.Interfaces.Services.V2.IOrganizationServiceInternal;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Notifications;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ITasksService), RegisterType.Transient)]
    [RegisterService(typeof(ITasksServiceInternal), RegisterType.Transient)]
    internal class TasksService : ServiceBase, ITasksService, ITasksServiceInternal
    {
        private readonly IContextManager contextManager;
        private readonly IServiceUtilities serviceUtilities;
        private readonly IPublishingStatusCache publishingStatusCache;
        private readonly IServiceService serviceService;
        private readonly IChannelService channelService;
        private readonly ITypesCache typesCache;
        private IPahaTokenProcessor pahaTokenProcessor;
        private readonly ILogger logger;
        private ICommonServiceInternal commonService;
        private IOrganizationServiceInternal organizationService;
        private ISearchServiceInternal searchService;

        public TasksService(ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IContextManager contextManager,
            IServiceService serviceService,
            IChannelService channelService,            
            ICommonServiceInternal commonService,
            IOrganizationServiceInternal organizationService,
            IServiceUtilities serviceUtilities,
            IVersioningManager versioningManager,
            ILogger<TasksService> logger,
            IPahaTokenProcessor pahaTokenProcessor,
            ISearchServiceInternal searchService,     
            ICacheManager cacheManager) :
                base(translationManagerToVm,
                    translationManagerToEntity,
                    publishingStatusCache,
                    userOrganizationChecker,
                    versioningManager)
        {
            this.contextManager = contextManager;
            this.serviceUtilities = serviceUtilities;
            this.publishingStatusCache = publishingStatusCache;
            this.serviceService = serviceService;
            this.channelService = channelService;            
            this.commonService = commonService;
            this.logger = logger;
            this.typesCache = cacheManager.TypesCache;
            this.pahaTokenProcessor = pahaTokenProcessor;
            this.organizationService = organizationService;
            this.searchService = searchService;
        }

        public IVmListItemsData<VmTasksBase> GetTasksNumbers()
        {
                var result = new VmListItemsData<VmTasksBase>();         
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var tasksIds = GetTasksIds(unitOfWork);
                    
                    result.Add(new VmTasksBase() { Id = TasksIdsEnum.OutdatedDraftServices, Count = tasksIds.OutdatedDraftServicesIds.Count()});
                    result.Add(new VmTasksBase() { Id = TasksIdsEnum.OutdatedDraftChannels, Count = tasksIds.OutdatedDraftChannelsIds.Count()});
                    result.Add(new VmTasksBase() { Id = TasksIdsEnum.OutdatedPublishedServices, Count = tasksIds.OutdatedPublishedServicesIds.Count()});
                    result.Add(new VmTasksBase() { Id = TasksIdsEnum.OutdatedPublishedChannels, Count = tasksIds.OutdatedPublishedChannelsIds.Count()});
                    result.Add(new VmTasksBase() { Id = TasksIdsEnum.ServicesWithoutChannels, Count = tasksIds.ServicesWithoutChannelsIds.Count()});
                    result.Add(new VmTasksBase() { Id = TasksIdsEnum.ChannelsWithoutServices, Count = tasksIds.ChannelsWithoutServicesIds.Count()});
                    result.Add(new VmTasksBase() { Id = TasksIdsEnum.MissingLanguageOrganizations, Count = tasksIds.MissingLanguageOrganizationsIds.Count()});
                    result.Add(GetTranslationArrivedCount(unitOfWork));
            });

            return result;
        }
        
        private VmTasksBase GetTranslationArrivedCount(IUnitOfWork unitOfWork)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingTranslationOrderRepository>();
            var userOrganizationId = serviceUtilities.GetUserMainOrganization();
            var count = trackingRepository
                .All()
                .Where(x => x.OrganizationId == userOrganizationId)
                .Count(x => x.Created >= DateTime.UtcNow.AddMonths(-1));
            return new VmTasksBase()
            {
                Id = TasksIdsEnum.TranslationArrived,
                Count = count
            };
        }
        
        private const int MAX_RESULTS = 10;
        
        private VmTasks GetTranslationArrived(VmTasksSearch model, IUnitOfWork unitOfWork)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingTranslationOrderRepository>();
            var translationOrderRepository = unitOfWork.CreateRepository<ITranslationOrderRepository>();
            var pageNumber = model.PageNumber.PositiveOrZero();
            var userOrganizationId = serviceUtilities.GetUserMainOrganization();
            var resultTemp = trackingRepository.All()
                .Where(x => x.OrganizationId == userOrganizationId)
                .Where(x => x.Created >= DateTime.UtcNow.AddMonths(-1))
                .OrderByDescending(x => x.Created).ThenBy(x=>x.Id);

            var notifications = resultTemp
                .ApplyPaging(pageNumber, MAX_RESULTS);

            var translationIds = notifications.SearchResult.Select(x => x.TranslationOrderId);

            var translationOrders = translationOrderRepository
                .All()
                .Include(x => x.ServiceChannelTranslationOrders)
                .Include(x => x.ServiceTranslationOrders)
                .Where(x => translationIds.Contains(x.Id))
                .Where(x => x.ServiceTranslationOrders.Any() || x.ServiceChannelTranslationOrders.Any());
            
            var serviceTranslationOrders = translationOrders
                .Where(x => x.ServiceTranslationOrders.Any())
                .ToDictionary(key => key.Id, value => value.ServiceTranslationOrders.First().ServiceId);
            
            var serviceChannelTranslationOrders = translationOrders
                .Where(x => x.ServiceChannelTranslationOrders.Any())
                .ToDictionary(key => key.Id, value => value.ServiceChannelTranslationOrders.First().ServiceChannelId);
            
            var versionedServices = commonService.GetNotificationEntity<ServiceVersioned, ServiceLanguageAvailability>(serviceTranslationOrders.Values, unitOfWork, q => q.Include(a => a.ServiceNames));
            var versionedServiceNames = commonService.GetEntityNames(versionedServices);
            var versionedServiceLanguages = commonService.GetLanguageAvailabilites<ServiceVersioned,ServiceLanguageAvailability>(versionedServices);

            var versionedServiceChannels = commonService.GetNotificationEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannelTranslationOrders.Values, unitOfWork, q => q.Include(a => a.ServiceChannelNames));
            var versionedServiceChannelNames = commonService.GetEntityNames(versionedServiceChannels);               
            var versionedServiceChannelLanguages = commonService.GetLanguageAvailabilites<ServiceChannelVersioned,ServiceChannelLanguageAvailability>(versionedServiceChannels);
            
            return new VmTasks()
            {
                Entities = new VmListItemsData<VmNotification>(
                    notifications.SearchResult
                        .Select(notification =>
                        {
                            var isChannel =
                                serviceChannelTranslationOrders.ContainsKey(notification.TranslationOrderId);
                            return new VmNotification()
                            {
                                Id = notification.Id,
                                Name = isChannel
                                    ? versionedServiceChannelNames[serviceChannelTranslationOrders[notification.TranslationOrderId]]
                                    : versionedServiceNames[serviceTranslationOrders[notification.TranslationOrderId]],
                                LanguagesAvailabilities = isChannel 
                                        ? versionedServiceChannelLanguages[
                                        serviceChannelTranslationOrders[notification.TranslationOrderId]]
                                        : versionedServiceLanguages[
                                        serviceTranslationOrders[notification.TranslationOrderId]],
                                OperationType = notification.OperationType,
                                CreatedBy = notification.CreatedBy,
                                Created = notification.Created.ToEpochTime(),
                                MainEntityType = isChannel
                                        ? EntityTypeEnum.Channel
                                        : EntityTypeEnum.Service,
                                SubEntityType = isChannel
                                        ? commonService.GetChannelSubType(
                                            serviceChannelTranslationOrders[notification.TranslationOrderId],
                                            versionedServiceChannels)
                                        : EntityTypeEnum.Service.ToString(),
                                VersionedId = isChannel
                                        ? versionedServiceChannels.First(x=>x.UnificRootId == serviceChannelTranslationOrders[notification.TranslationOrderId]).Id
                                        : versionedServices.First(x=>x.UnificRootId == serviceTranslationOrders[notification.TranslationOrderId]).Id,
                                PublishingStatusId = isChannel
                                    ? versionedServiceChannels.First(x=>x.UnificRootId == serviceChannelTranslationOrders[notification.TranslationOrderId]).PublishingStatusId
                                    : versionedServices.First(x=>x.UnificRootId == serviceTranslationOrders[notification.TranslationOrderId]).PublishingStatusId,
                            };
                        })
                    ),
                Count = resultTemp.Count(),
                MoreAvailable = notifications.MoreAvailable,
                PageNumber = ++pageNumber,
                Id = TasksIdsEnum.TranslationArrived
            };
        }
        
        class TasksIds {
            public IEnumerable<Guid> OutdatedDraftServicesIds { get; set; }
            public IEnumerable<Guid> OutdatedDraftChannelsIds { get; set; }
            public IEnumerable<Guid> OutdatedPublishedServicesIds { get; set; }
            public IEnumerable<Guid> OutdatedPublishedChannelsIds { get; set; }
            public IEnumerable<Guid> ServicesWithoutChannelsIds { get; set; }
            public IEnumerable<Guid> ChannelsWithoutServicesIds { get; set; }
            public IEnumerable<Guid> MissingLanguageOrganizationsIds { get; set; }
        }

        private TasksIds GetTasksIds(IUnitOfWork unitOfWork) {
            var result = new TasksIds();

            result.OutdatedDraftServicesIds = GetEntityIds<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, PublishingStatus.Draft);

            result.OutdatedDraftChannelsIds = GetEntityIds<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, PublishingStatus.Draft);
            
            result.OutdatedPublishedServicesIds = GetEntityIds<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, PublishingStatus.Published);
            
            result.OutdatedPublishedChannelsIds = GetEntityIds<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, PublishingStatus.Published);

            result.ServicesWithoutChannelsIds = GetOrphansIds<ServiceVersioned>(unitOfWork);
            result.ChannelsWithoutServicesIds = GetOrphansIds<ServiceChannelVersioned>(unitOfWork);
            result.MissingLanguageOrganizationsIds = GetMissingLanguageOrganizationIds(unitOfWork).Keys.ToList();
            return result;
        }

        private int GetTasksIdsCount(IUnitOfWork unitOfWork, TasksIdsEnum taskType)
        {
            switch (taskType)
            {
                case TasksIdsEnum.OutdatedDraftServices:
                    return GetEntityIds<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, PublishingStatus.Draft).Count();
                case TasksIdsEnum.OutdatedPublishedServices:
                    return GetEntityIds<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, PublishingStatus.Published).Count();
                case TasksIdsEnum.OutdatedDraftChannels:
                    return GetEntityIds<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, PublishingStatus.Draft).Count();
                case TasksIdsEnum.OutdatedPublishedChannels:
                    return GetEntityIds<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, PublishingStatus.Published).Count();
                case TasksIdsEnum.ServicesWithoutChannels:
                    return GetOrphansIds<ServiceVersioned>(unitOfWork).Count();
                case TasksIdsEnum.ChannelsWithoutServices:
                    return GetOrphansIds<ServiceChannelVersioned>(unitOfWork).Count();
                case TasksIdsEnum.MissingLanguageOrganizations:
                    return GetMissingLanguageOrganizationIds(unitOfWork).Count();
                default:
                    throw new ArgumentOutOfRangeException(nameof(taskType), taskType, null);
            }
        }

        private class Period
        {
            public DateTime Modified { get; set; }
            public Guid PeriodId { get; set; }
        }

        private class Tasks
        {
            public Tasks()
            {
                Entities = new List<VmTaskEntity>();
            }
            
            public IEnumerable<VmTaskEntity> Entities { get; set; }
            public bool IsPostponeButtonVisible { get; set; }
        }

        public DateTime? GetExpirationTime<TEntity>(IUnitOfWork unitOfWork, Guid entityId, DateTime? utcNow = null)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, new()
        {
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entity = repository.All().SingleOrDefault(e => e.Id == entityId);
            return entity == null 
                ? null 
                : base.GetExpirationTime(unitOfWork, entity, utcNow);
        }

        public Dictionary<Guid, VmExpirationOfEntity> GetExpirationInformation<TEntity, TLanguageAvailability>(IUnitOfWork unitOfWork, Guid unificRootId, Guid publishingStatusId)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            var dateTimeUtcNow = DateTime.UtcNow;
            var normalizedStatusId = base.NormalizePublishingStatusId<TEntity>(unitOfWork, unificRootId, publishingStatusId);
            if (!normalizedStatusId.HasValue) return null;

            var lifeTime = base.GetLifeTime<TEntity>(unitOfWork, normalizedStatusId.Value);
            if (!lifeTime.HasValue) return null;
            
            var expiration = GetEntityAllEntitiesWithGroup<TEntity, TLanguageAvailability>(unitOfWork, normalizedStatusId.Value, new List<Guid>() {unificRootId}, 0);
            
            var result = new Dictionary<Guid, VmExpirationOfEntity>();
            
            if ((expiration == null || !expiration.Entities.Any()) && unificRootId.IsAssigned()) //prepare setting of expireOn 
            {
                var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
                expiration = expiration ?? new Tasks();
                expiration.Entities = repository.All().Where(x => x.UnificRootId == unificRootId && x.PublishingStatusId == normalizedStatusId.Value)
                    .Select(x => new VmTaskEntity() {UnificRootId = x.UnificRootId, Modified = x.Modified}).ToList();
            }

            expiration?.Entities.ForEach(e =>
                {
                    result.Add(e.UnificRootId, new VmExpirationOfEntity()
                    {
                        ExpireOn = base.CalculateExpirationTime(e.Modified, lifeTime.Value, dateTimeUtcNow),
                        // Warning should be visible only for services which do not have publishing scheduled in the future
                        IsWarningVisible = expiration.IsPostponeButtonVisible && !e.HasScheduledLanguageAvailability
                    });
                });

            return result;
        }
        
        private Tasks GetEntityAllEntitiesWithGroup<TEntity, TLanguageAvailability>(IUnitOfWork unitOfWork, Guid publishingStatusId, IEnumerable<Guid> definedEntities = null, int? skip = null)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            if (!skip.HasValue) skip = 1;
            var userOrganizations = serviceUtilities.GetAllUserOrganizations();
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entityTypeName = typeof(TEntity).Name.Replace("Versioned", "");
            var configurationRepository = unitOfWork.CreateRepository<IVTasksConfigurationRepository>();
            var publishedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            // get all expiration times of given type and publishing status, skip first one, becuase it is time used for archivation (top time, that all entities should be already archived)
            var tasksConfiguration = configurationRepository.All().Where(x => x.Entity == entityTypeName && x.PublishingStatusId == publishingStatusId)
                .OrderBy(x => x.Interval).Select(x => new Period() { PeriodId = x.Id, Modified = x.Interval}).Skip(skip.Value).ToList();

            if (!tasksConfiguration.Any()) return null;

            var firstWarningTime = tasksConfiguration.Last().Modified;
            var lastWarningTime = tasksConfiguration.First().Modified;
            var resultTemp = definedEntities != null ? repository.All().Where(x => definedEntities.Contains(x.UnificRootId)) : repository.All();

            var selectedPublishingStatuses = new List<Guid> { publishingStatusId };
            commonService.ExtendPublishingStatusesByEquivalents(selectedPublishingStatuses);
            resultTemp = resultTemp.WherePublishingStatusIn(selectedPublishingStatuses);
            if (publishingStatusId == draftStatusId)
            {
                resultTemp = resultTemp
                    .Where(x =>
                        !repository.All()
                            .Any(y => y.UnificRootId == x.UnificRootId && y.PublishingStatusId == publishedStatusId));
            }

            var entities = resultTemp.Where(x => userOrganizations.Contains(x.OrganizationId)
                                                 && x.Modified < firstWarningTime)
                .Select(x => new VmTaskEntity()
                {
                    UnificRootId = x.UnificRootId, Modified = x.Modified,
                    HasScheduledLanguageAvailability = x.LanguageAvailabilities.Any(la => la.PublishAt > DateTime.UtcNow)
                }).ToList();

            var result = new Tasks();
            
            entities.ForEach(entity =>
            {
                foreach (var config in tasksConfiguration)
                {
                    if (entity.Modified >= config.Modified) continue;
                    entity.GroupId = config.PeriodId;
                    entity.LowerThanLast = entity.Modified < lastWarningTime;
                    break;
                }            
            });

            var filterRepository = unitOfWork.CreateRepository<ITasksFilterRepository>();
            var userGuid = pahaTokenProcessor.UserName.GetGuid();
            var userFilters = filterRepository.All().Where(x => x.UserId == userGuid).ToList();

            result.Entities = entities
                .Where(x => !userFilters.Any(y => y.EntityId == x.UnificRootId && y.TypeId == x.GroupId))
                .DistinctBy(x => x.UnificRootId);
            
            // another skip, because the button should not be shown for last period before archivation
            result.IsPostponeButtonVisible = result.Entities.Any(x => tasksConfiguration.Select(y => y.PeriodId).Skip(skip.Value).Contains(x.GroupId));
            
            return result;
        }
        
        private IEnumerable<Guid> GetEntityIds<TEntity, TLanguageAvailability>(IUnitOfWork unitOfWork, Guid publishingStatusId)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            return GetEntityAllEntitiesWithGroup<TEntity, TLanguageAvailability>(unitOfWork, publishingStatusId).Entities.Select(x => x.UnificRootId);
        }
        
        private IEnumerable<Guid> GetEntityIds<TEntity, TLanguageAvailability>(IUnitOfWork unitOfWork, PublishingStatus publishingStatus)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            var publishingStatusId = publishingStatusCache.Get(publishingStatus);

            return GetEntityIds<TEntity, TLanguageAvailability>(unitOfWork, publishingStatusId);
        }
        
        private IEnumerable<Guid> GetOrphansIds<TEntity>(IUnitOfWork unitOfWork)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, new()
        {
            var publishingStatusIds = new List<Guid>()
            {
                publishingStatusCache.Get(PublishingStatus.Published),
                publishingStatusCache.Get(PublishingStatus.Draft),
                publishingStatusCache.Get(PublishingStatus.Modified)
            };
            
            var userOrganizations = serviceUtilities.GetAllUserOrganizations();
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entities = repository.All().Where(x => userOrganizations.Contains(x.OrganizationId) && publishingStatusIds.Contains(x.PublishingStatusId));
            var entityIds = entities.Select(x => x.UnificRootId).Distinct();
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            IQueryable<Guid> connectedEntitiesIds;


            if (typeof(TEntity) == typeof(ServiceVersioned))
            {
                connectedEntitiesIds = serviceChannelRep.All().Select(x => x.ServiceId);
            } else if (typeof(TEntity) == typeof(ServiceChannelVersioned))
            {
                connectedEntitiesIds = serviceChannelRep.All().Select(x => x.ServiceChannelId);
            }
            else
            {
                return new List<Guid>();
            }
            
            return entityIds.Except(connectedEntitiesIds).ToList();
        }
        
        private DateTime GetDateForType(IUnitOfWork unitOfWork, DateTime currentTime, AppEnvironmentDataTypeEnum type)
        {
            var typeId = typesCache.Get<AppEnvironmentDataType>(type.ToString());

            return GetDateForType(unitOfWork, currentTime, typeId);
        }

        private DateTime GetDateForType(IUnitOfWork unitOfWork, DateTime currentTime, Guid typeId)
        {
            var appEnvironmentData = unitOfWork.CreateRepository<IAppEnvironmentDataRepository>();
            var expirationDate = appEnvironmentData.All().FirstOrDefault(x => x.TypeId == typeId);
            if (expirationDate == null) return DateTime.MinValue;

            return int.TryParse(expirationDate.FreeText, out var weeks) ? currentTime.AddDays(-(weeks * 7)) : DateTime.MinValue;
        }

        private void PostoponeEntities<TEntity, TLanguageAvailability>(Guid publishingStatusId)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            contextManager.ExecuteWriter(unitOfWork =>
                {
                    TranslationManagerToEntity.TranslateAll<VmTaskEntity, TasksFilter>(
                        GetEntityAllEntitiesWithGroup<TEntity, TLanguageAvailability>(unitOfWork, publishingStatusId).Entities.Where(x=>!x.LowerThanLast), unitOfWork);
                    unitOfWork.Save();
                });
        }

        public IVmSearchBase PostoponeEntities(VmPostponeTasks model)
        {
            switch (model.TaskType)
            {
                case TasksIdsEnum.OutdatedDraftServices:
                    PostoponeEntities<ServiceVersioned, ServiceLanguageAvailability>(publishingStatusCache.Get(PublishingStatus.Draft));
                    break;
                case TasksIdsEnum.OutdatedDraftChannels:
                    PostoponeEntities<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(publishingStatusCache.Get(PublishingStatus.Draft));
                    break;
                case TasksIdsEnum.OutdatedPublishedChannels:
                    PostoponeEntities<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(publishingStatusCache.Get(PublishingStatus.Published));
                    break;
                case TasksIdsEnum.OutdatedPublishedServices:
                    PostoponeEntities<ServiceVersioned, ServiceLanguageAvailability>(publishingStatusCache.Get(PublishingStatus.Published));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return GetTasksEntities(new VmTasksSearch() { TaskType = model.TaskType, GetCount = true});
        }
        
        public IVmSearchBase GetTasksEntities(VmTasksSearch model)
        {
            VmTasks result = null;
            Tasks tasks = null;
            DateTime expiration = DateTime.MinValue;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var configurationRepository = unitOfWork.CreateRepository<IVTasksConfigurationRepository>();
                IEnumerable<Guid> entityIds;
                Guid publishingStatusId = Guid.Empty;
                switch (model.TaskType)
                {
                    case TasksIdsEnum.OutdatedDraftServices:
                    case TasksIdsEnum.OutdatedDraftChannels:
                        publishingStatusId = publishingStatusCache.Get(PublishingStatus.Draft);
                        expiration = configurationRepository.All().Where(x =>
                                x.Entity == "ServiceChannel" && x.PublishingStatusId == publishingStatusId)
                            .OrderBy(x => x.Interval).First().Interval;
                        break;
                    case TasksIdsEnum.OutdatedPublishedChannels:
                    case TasksIdsEnum.OutdatedPublishedServices:
                        publishingStatusId = publishingStatusCache.Get(PublishingStatus.Published);
                        expiration = configurationRepository.All().Where(x =>
                                x.Entity == "Service" && x.PublishingStatusId == publishingStatusId)
                            .OrderBy(x => x.Interval).First().Interval;
                        break;
                    case TasksIdsEnum.ServicesWithoutChannels:
                    case TasksIdsEnum.TranslationArrived:
                    case TasksIdsEnum.ChannelsWithoutServices:
                    case TasksIdsEnum.MissingLanguageOrganizations:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                switch (model.TaskType)
                {
                    case TasksIdsEnum.OutdatedDraftChannels:
                    case TasksIdsEnum.OutdatedPublishedChannels:
                    case TasksIdsEnum.ChannelsWithoutServices:
                        List<Guid> chPubStatuses = null;
                        if (model.TaskType == TasksIdsEnum.ChannelsWithoutServices)
                        {
                            entityIds = GetOrphansIds<ServiceChannelVersioned>(unitOfWork);
                            chPubStatuses = new List<Guid>()
                            {
                                publishingStatusCache.Get(PublishingStatus.Published),
                                publishingStatusCache.Get(PublishingStatus.Draft),
                                publishingStatusCache.Get(PublishingStatus.Modified)
                            };
                        }
                        else
                        {
                            tasks = GetEntityAllEntitiesWithGroup<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork,
                                publishingStatusId);

                            entityIds = tasks.Entities.Select(x => x.UnificRootId);
                            chPubStatuses = new List<Guid>() {publishingStatusId};
                        }

                        var channels = channelService.SearchChannels(unitOfWork, new VmChannelSearchParams()
                        {
                            SortData = model.SortData,
                            SelectedPublishingStatuses = chPubStatuses,
                            MaxPageCount = model.MaxPageCount,
                            PageNumber = model.PageNumber,
                            Skip = model.Skip,
                            EntityIds = entityIds,
                            Expiration = expiration
                        }) as VmSearchResult<IVmChannelListItem>;
                        result = new VmTasks(channels);
                        if (channels != null)
                        {
                            result.Entities = channels.SearchResult.GroupBy(x => x.UnificRootId).Select(x =>
                                x.FirstOrDefault(y =>
                                    y.PublishingStatusId == publishingStatusCache.Get(PublishingStatus.Published)) ?? x.First());
                            result.Count = model.GetCount ? GetTasksIdsCount(unitOfWork, model.TaskType) : -1;
                            result.IsPostponeButtonVisible = tasks?.IsPostponeButtonVisible ?? false;
                        }
                        
                        break;
                    
                    case TasksIdsEnum.OutdatedDraftServices:
                    case TasksIdsEnum.OutdatedPublishedServices:
                    case TasksIdsEnum.ServicesWithoutChannels:
                        List<Guid> sPubStatuses = null;
                        if (model.TaskType == TasksIdsEnum.ServicesWithoutChannels)
                        {
                            entityIds = GetOrphansIds<ServiceVersioned>(unitOfWork);
                            sPubStatuses = new List<Guid>()
                            {
                                publishingStatusCache.Get(PublishingStatus.Published),
                                publishingStatusCache.Get(PublishingStatus.Draft),
                                publishingStatusCache.Get(PublishingStatus.Modified)
                            };
                        }
                        else
                        {
                            tasks = GetEntityAllEntitiesWithGroup<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, publishingStatusId);
                            entityIds = tasks.Entities.Select(x => x.UnificRootId);
                            sPubStatuses = new List<Guid>() {publishingStatusId};
                        }

                        var services = serviceService.SearchServices(unitOfWork, new VmServiceSearch()
                        {
                            SortData = model.SortData,
                            MaxPageCount = model.MaxPageCount,
                            PageNumber = model.PageNumber,
                            Skip = model.Skip,
                            SelectedPublishingStatuses = sPubStatuses,
                            EntityIds = entityIds,
                            Expiration = expiration
                        }) as VmSearchResult<IVmServiceListItem>;
                        
                        result = new VmTasks(services);
                        if (services != null)
                        {
                            result.Entities = services.SearchResult.GroupBy(x => x.UnificRootId).Select(x =>
                                x.FirstOrDefault(y =>
                                    y.PublishingStatusId == publishingStatusCache.Get(PublishingStatus.Published)) ?? x.First());
                            result.Count = model.GetCount ? GetTasksIdsCount(unitOfWork, model.TaskType) : -1;
                            result.IsPostponeButtonVisible = tasks?.IsPostponeButtonVisible ?? false;
                        }
                        break;
                    case TasksIdsEnum.TranslationArrived:
                        result = GetTranslationArrived(model, unitOfWork);
                        break;
                    case TasksIdsEnum.MissingLanguageOrganizations:
                        var missingLanguages = GetMissingLanguageOrganizationIds(unitOfWork);
                        entityIds = missingLanguages.Keys.ToList();
                        sPubStatuses = new List<Guid>()
                        {
                            publishingStatusCache.Get(PublishingStatus.Published),
                            publishingStatusCache.Get(PublishingStatus.Draft)
                        };
                        var organizations = searchService.SearchOrganizations(new VmEntitySearch()
                        {
                            SortData = model.SortData,
                            MaxPageCount = model.MaxPageCount,
                            PageNumber = model.PageNumber,
                            Skip = model.Skip,
                            EntityVersionIds = entityIds,
                            SelectedPublishingStatuses = sPubStatuses,
                            ContentTypes = new List<SearchEntityTypeEnum>(){SearchEntityTypeEnum.Organization}
                        }, unitOfWork) as VmSearchResult<IVmEntityListItem>;
                        result = new VmTasks(organizations);
                        if (organizations != null)
                        {
                            result.Entities = organizations.SearchResult;
                            result.Entities.OfType<VmEntityListItem>().ForEach(org => org.MissingLanguages = missingLanguages[org.Id]);
                            result.Count = model.GetCount ? GetTasksIdsCount(unitOfWork, model.TaskType) : -1;                            
                            result.IsPostponeButtonVisible = false;
                        }
                        break;
                    default:
                        result = null;
                        break;
                }
            });
            if (result == null) return result;
            result.Id = model.TaskType;

            return result;
        }
        
        public void ArchiveEntityByExpirationDate()
        {
            var statusesToArchive = new List<PublishingStatus>
            {
                PublishingStatus.Draft,
                PublishingStatus.Published
            };
            
            commonService.ExtendPublishingStatusesByEquivalents(statusesToArchive);

            contextManager.ExecuteWriter(unitOfWork =>
            {
                try
                {
                    ArchiveByExpirationDate<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, statusesToArchive);
                    ArchiveByExpirationDate<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, statusesToArchive);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                }
                catch (Exception ex)
                {
                    var errorMsg = string.Format($"Archiving of entities by expiration date thrown exception: {ex.Message}");
                    logger.LogError(errorMsg + " " + ex.StackTrace);
                }
            });
        }

        private void ArchiveByExpirationDate<TEntity, TLanguageAvailability>(IUnitOfWorkWritable unitOfWork, List<PublishingStatus> publishingStatuses)
            where TEntity : class, IAuditing, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IOrganizationInfo, new()
            where TLanguageAvailability : class, ILanguageAvailability
        {
            var entitiesToArchive = new List<TEntity>();
            foreach (var status in publishingStatuses)
            {
                entitiesToArchive.AddRange(GetEntityIdsByExpirationDate<TEntity, TLanguageAvailability>(unitOfWork, status));
            }

            //Console.WriteLine($"Entity: '{typeof(TEntity).Name}' have {entitiesToArchive.Count} items after expiration date.");
            logger.LogInformation($"Entity: '{typeof(TEntity).Name}' have {entitiesToArchive.Count} items after expiration date.");
           
            //Archiving 
            ArchiveEntities<TEntity, TLanguageAvailability>(unitOfWork, entitiesToArchive);            
        }

        private void ArchiveEntities<TEntity, TLanguageAvailability>(IUnitOfWorkWritable unitOfWork, List<TEntity> entitiesToArchive)
            where TEntity : class, IAuditing, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IOrganizationInfo, new()
            where TLanguageAvailability :class, ILanguageAvailability
        {
            var publishingStatusDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            foreach (var entity in entitiesToArchive)
            {
                entity.PublishingStatus = null;
                entity.PublishingStatusId = publishingStatusDeleted;
            }
            //add version history
            var uniqEntities = entitiesToArchive.DistinctBy(x=>x.UnificRootId).ToList();
            foreach (var entity in uniqEntities)
            {
                VersioningManager.AddMinorVersion(unitOfWork, entity);
                commonService.AddHistoryMetaData<TEntity, TLanguageAvailability>(entity, HistoryAction.Delete);
            }
            //Track archive entities
            TrackArchiveEntities(unitOfWork, uniqEntities);
        }

        private void TrackArchiveEntities<TEntity>(IUnitOfWorkWritable unitOfWork, List<TEntity> entitiesToArchive)
            where TEntity : class, IAuditing, IEntityIdentifier, IVersionedVolume, IOrganizationInfo, new()
        {
            var repository = unitOfWork.CreateRepository<ITrackingEntityVersionedRepository>();
            entitiesToArchive.ForEach(entity =>
            {
                repository.All().Where(x => x.UnificRootId == entity.UnificRootId && x.OperationType == EntityState.Deleted.ToString())
                    .ForEach(r => repository.Remove(r));
                var entityTrack = new TrackingEntityVersioned()
                {
                    Id = Guid.NewGuid(),
                    UnificRootId = entity.UnificRootId,
                    EntityType = entity is ServiceVersioned ? EntityTypeEnum.Service.ToString() : EntityTypeEnum.Channel.ToString(),
                    OrganizationId = entity.OrganizationId,
                    OperationType = EntityState.Deleted.ToString()
                };
                repository.Add(entityTrack);
            });
        }

        private IEnumerable<TEntity> GetEntityIdsByExpirationDate<TEntity, TLanguageAvailability>(IUnitOfWorkWritable unitOfWork, PublishingStatus publishingStatus)
            where TEntity : class, IAuditing, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            var publishingStatusId = publishingStatusCache.Get(publishingStatus);
            var expiration = GetExpiration<TEntity>(unitOfWork, publishingStatusId);
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var expirationDate = (expiration > DateTime.MinValue)
                ? expiration
                : DateTime.MinValue;

            var allEntities = repository.All().Include(x => x.LanguageAvailabilities);
            
            if (publishingStatus == PublishingStatus.Modified)
            {
                // return only restored expired entities
                var statusPublishedId = publishingStatusCache.Get(PublishingStatus.Published.ToString());
                expiration = GetExpiration<TEntity>(unitOfWork, publishingStatusCache.Get(PublishingStatus.Draft.ToString()));
                expirationDate = (expiration > DateTime.MinValue)
                    ? expiration
                    : DateTime.MinValue;
                
                logger.LogInformation($"Entity: '{typeof(TEntity).Name}' publishingStatus: {publishingStatus} with expiration date: {expirationDate} ({expiration})");

                return allEntities
                    .Where(x => x.PublishingStatusId == publishingStatusId && x.Modified < expirationDate)
                    // Entities with scheduled publishing date in the future should not be archived
                    .Where(x => !x.LanguageAvailabilities.Any(la => la.PublishAt > DateTime.UtcNow))
                    .Where(x => !repository.All().Any(y=>y.UnificRootId == x.UnificRootId && y.PublishingStatusId == statusPublishedId))
                    .ToList();
            }

            //Console.WriteLine($"Entity: '{typeof(TEntity).Name}' publishingStatus: {publishingStatus} with expiration date: {expirationDate} ({expirationWeekNumber}weeks)");
            logger.LogInformation($"Entity: '{typeof(TEntity).Name}' publishingStatus: {publishingStatus} with expiration date: {expirationDate} ({expiration})");

            var result = allEntities
                .Where(x => x.PublishingStatusId == publishingStatusId && x.Modified < expirationDate)
                // Entities with scheduled publishing date in the future should not be archived
                .Where(x => !x.LanguageAvailabilities.Any(la => la.PublishAt > DateTime.UtcNow))
                .ToList();

            if (publishingStatus == PublishingStatus.Published && result.Any())
            {
                // add also modified versions to result
                var modifiedStatusId = publishingStatusCache.Get(PublishingStatus.Modified.ToString());
                var unificRootIds = result.Select(x => x.UnificRootId);
                var modifiedEntities = allEntities
                    .Where(x => unificRootIds.Contains(x.UnificRootId) && x.PublishingStatusId == modifiedStatusId);
                result.AddRange(modifiedEntities);
                
                logger.LogInformation($"Entity: '{typeof(TEntity).Name}' publishingStatus: {PublishingStatus.Modified.ToString()} with expiration date: {expirationDate} ({expiration})");
            }

            return result;
        }

        private DateTime GetExpiration<TEntity>(IUnitOfWork unitOfWork, Guid publishingStatusId)
            where TEntity : class, IAuditing, IEntityIdentifier, IVersionedVolume, new()
        {
            var taskConfigurations = GetTaskConfigurations<TEntity>(unitOfWork, publishingStatusId);
            return taskConfigurations.Where(x => x.Code.Contains("ExpirationTime")).Select(y => y.Interval).FirstOrDefault();
        }

        private IEnumerable<VTasksConfiguration> GetTaskConfigurations<TEntity>(IUnitOfWork unitOfWork, Guid publishingStatusId)
            where TEntity : class, IAuditing, IEntityIdentifier, IVersionedVolume, new()
        {
            var configurationRepository = unitOfWork.CreateRepository<IVTasksConfigurationRepository>();
            var entityTypeName = typeof(TEntity).Name.Replace("Versioned", "");

            var tasksConfiguration = configurationRepository.All()
                .Where(x => x.Entity == entityTypeName && x.PublishingStatusId == publishingStatusId)
                .ToList();

            return tasksConfiguration;
        }

        private Dictionary<Guid, List<Guid>> GetMissingLanguageOrganizationIds(IUnitOfWork unitOfWork)
        {
            var result = new Dictionary<Guid, List<Guid>>();
            var userOrgId = serviceUtilities.GetAllUserOrganizations();
            userOrgId.ForEach(orgId =>
            {
                var versionId = VersioningManager.GetLastVersion<OrganizationVersioned>(unitOfWork, orgId)?.EntityId;
                var missingIds = organizationService.GetOrganizationMissingLanguages(versionId, unitOfWork).ToList();
                if (missingIds.Any() && versionId.HasValue)
                {
                    result.Add(versionId.Value, missingIds);
                }
            });
            
            return result;
        }

        #region Open api

        public IVmOpenApiGuidPageVersionBase<VmOpenApiExpiringTask> GetTasks(TasksIdsEnum taskId, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) return new VmOpenApiExpiringTasks();

            Guid? statusId = null;
            var statusIds = new List<Guid>();
            Tasks tasks = null;
            DateTime? expiration = DateTime.MinValue;
            List<Guid> entityIds = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var configurationRepository = unitOfWork.CreateRepository<IVTasksConfigurationRepository>();

                switch (taskId)
                {
                    case TasksIdsEnum.OutdatedDraftServices:
                        statusId = publishingStatusCache.Get(PublishingStatus.Draft);
                        statusIds.Add(statusId.Value);
                        statusIds.Add(publishingStatusCache.Get(PublishingStatus.Modified));
                        expiration = configurationRepository.All().Where(x =>
                            x.Entity == "Service" && x.PublishingStatusId == statusId)
                        .OrderBy(x => x.Interval).FirstOrDefault()?.Interval;
                        tasks = GetEntityAllEntitiesWithGroup<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, statusId.Value);
                        break;
                    case TasksIdsEnum.OutdatedPublishedServices:
                        statusId = publishingStatusCache.Get(PublishingStatus.Published);
                        statusIds.Add(statusId.Value);
                        expiration = configurationRepository.All().Where(x =>
                            x.Entity == "Service" && x.PublishingStatusId == statusId)
                        .OrderBy(x => x.Interval).FirstOrDefault()?.Interval;
                        tasks = GetEntityAllEntitiesWithGroup<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, statusId.Value);
                        break;
                    case TasksIdsEnum.OutdatedDraftChannels:
                        statusId = publishingStatusCache.Get(PublishingStatus.Draft);
                        statusIds.Add(statusId.Value);
                        statusIds.Add(publishingStatusCache.Get(PublishingStatus.Modified));
                        expiration = configurationRepository.All().Where(x =>
                            x.Entity == "ServiceChannel" && x.PublishingStatusId == statusId)
                        .OrderBy(x => x.Interval).FirstOrDefault()?.Interval;
                        tasks = GetEntityAllEntitiesWithGroup<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, statusId.Value);
                        break;
                    case TasksIdsEnum.OutdatedPublishedChannels:
                        statusId = publishingStatusCache.Get(PublishingStatus.Published);
                        statusIds.Add(statusId.Value);
                        expiration = configurationRepository.All().Where(x =>
                            x.Entity == "ServiceChannel" && x.PublishingStatusId == statusId)
                        .OrderBy(x => x.Interval).FirstOrDefault()?.Interval;
                        tasks = GetEntityAllEntitiesWithGroup<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, statusId.Value);
                        break;
                    default:
                        break;
                }  
            });

            if (taskId == TasksIdsEnum.OutdatedDraftServices || taskId == TasksIdsEnum.OutdatedPublishedServices)
            {
                entityIds = tasks?.Entities.Select(x => x.UnificRootId).ToList();
                return serviceService.GetTaskServices(pageNumber, pageSize, entityIds, expiration.HasValue ? expiration.Value : DateTime.MinValue, statusIds);
            }

            if (taskId == TasksIdsEnum.OutdatedDraftChannels || taskId == TasksIdsEnum.OutdatedPublishedChannels)
            {
                entityIds = tasks?.Entities.Select(x => x.UnificRootId).ToList();
                return channelService.GetTaskServiceChannels(pageNumber, pageSize, entityIds, expiration.HasValue ? expiration.Value : DateTime.MinValue, statusIds);
            }
            return null;            
        }

        public IVmOpenApiGuidPageVersionBase<VmOpenApiTask> GetOrphanItemsTasks(TasksIdsEnum taskId, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) return new VmOpenApiTasks();

            var statusIds = new List<Guid>();
            List<Guid> entityIds = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var configurationRepository = unitOfWork.CreateRepository<IVTasksConfigurationRepository>();

                switch (taskId)
                {                    
                    case TasksIdsEnum.ServicesWithoutChannels:
                        entityIds = GetOrphansIds<ServiceVersioned>(unitOfWork).ToList();
                        statusIds.Add(publishingStatusCache.Get(PublishingStatus.Published));
                        statusIds.Add(publishingStatusCache.Get(PublishingStatus.Draft));
                        statusIds.Add(publishingStatusCache.Get(PublishingStatus.Modified));
                        break;
                    case TasksIdsEnum.ChannelsWithoutServices:
                        entityIds = GetOrphansIds<ServiceChannelVersioned>(unitOfWork).ToList();
                        statusIds.Add(publishingStatusCache.Get(PublishingStatus.Published));
                        statusIds.Add(publishingStatusCache.Get(PublishingStatus.Draft));
                        statusIds.Add(publishingStatusCache.Get(PublishingStatus.Modified));
                        break;
                    default:
                        break;
                }
            });

            if (taskId == TasksIdsEnum.ServicesWithoutChannels)
            {
                return serviceService.GetTaskServices(pageNumber, pageSize, entityIds, statusIds);
            }

            if (taskId == TasksIdsEnum.ChannelsWithoutServices)
            {
                return channelService.GetTaskServiceChannels(pageNumber, pageSize, entityIds, statusIds);
            }

            return null;
        }

        #endregion
    }
}
