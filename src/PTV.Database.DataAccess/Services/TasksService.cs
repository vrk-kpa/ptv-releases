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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Views;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Domain.Logic;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ITasksService), RegisterType.Transient)]
    [RegisterService(typeof(ITasksServiceInternal), RegisterType.Transient)]
    internal class TasksService : ServiceBase, ITasksService, ITasksServiceInternal
    {
        private readonly IContextManager contextManager;
        private readonly ApplicationConfiguration configuration;
        private readonly ServiceUtilities serviceUtilities;
        private readonly IPublishingStatusCache publishingStatusCache;
        private readonly IServiceService serviceService;
        private readonly IChannelService channelService;
        private readonly ITypesCache typesCache;
        private IPahaTokenProcessor pahaTokenProcessor;
        private readonly ILogger logger;
        private ICommonServiceInternal commonService;

        public TasksService(ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            ApplicationConfiguration configuration,
            IContextManager contextManager,
            IServiceService serviceService,
            IChannelService channelService,            
            ICommonServiceInternal commonService,
            ServiceUtilities serviceUtilities,
            ILogger<TasksService> logger,
            IPahaTokenProcessor pahaTokenProcessor,
            ICacheManager cacheManager) :
                base(translationManagerToVm,
                    translationManagerToEntity,
                    publishingStatusCache,
                    userOrganizationChecker)
        {
            this.configuration = configuration;
            this.contextManager = contextManager;
            this.serviceUtilities = serviceUtilities;
            this.publishingStatusCache = publishingStatusCache;
            this.serviceService = serviceService;
            this.channelService = channelService;            
            this.commonService = commonService;
            this.logger = logger;
            this.typesCache = cacheManager.TypesCache;
            this.pahaTokenProcessor = pahaTokenProcessor;
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
            });

            return result;
        }

        class TasksIds {
            public IEnumerable<Guid> OutdatedDraftServicesIds { get; set; }
            public IEnumerable<Guid> OutdatedDraftChannelsIds { get; set; }
            public IEnumerable<Guid> OutdatedPublishedServicesIds { get; set; }
            public IEnumerable<Guid> OutdatedPublishedChannelsIds { get; set; }
            public IEnumerable<Guid> ServicesWithoutChannelsIds { get; set; }
            public IEnumerable<Guid> ChannelsWithoutServicesIds { get; set; }
        }

        private TasksIds GetTasksIds(IUnitOfWork unitOfWork) {
            var result = new TasksIds();

            result.OutdatedDraftServicesIds = GetEntityIds<ServiceVersioned>(unitOfWork, PublishingStatus.Draft);

            result.OutdatedDraftChannelsIds = GetEntityIds<ServiceChannelVersioned>(unitOfWork, PublishingStatus.Draft);
            
            result.OutdatedPublishedServicesIds = GetEntityIds<ServiceVersioned>(unitOfWork, PublishingStatus.Published);
            
            result.OutdatedPublishedChannelsIds = GetEntityIds<ServiceChannelVersioned>(unitOfWork, PublishingStatus.Published);

            result.ServicesWithoutChannelsIds = GetOrphansIds<ServiceVersioned>(unitOfWork);
            result.ChannelsWithoutServicesIds = GetOrphansIds<ServiceChannelVersioned>(unitOfWork);
            return result;
        }

        private int GetTasksIdsCount(IUnitOfWork unitOfWork, TasksIdsEnum taskType)
        {
            switch (taskType)
            {
                case TasksIdsEnum.OutdatedDraftServices:
                    return GetEntityIds<ServiceVersioned>(unitOfWork, PublishingStatus.Draft).Count();
                case TasksIdsEnum.OutdatedPublishedServices:
                    return GetEntityIds<ServiceVersioned>(unitOfWork, PublishingStatus.Published).Count();
                case TasksIdsEnum.OutdatedDraftChannels:
                    return GetEntityIds<ServiceChannelVersioned>(unitOfWork, PublishingStatus.Draft).Count();
                case TasksIdsEnum.OutdatedPublishedChannels:
                    return GetEntityIds<ServiceChannelVersioned>(unitOfWork, PublishingStatus.Published).Count();
                case TasksIdsEnum.ServicesWithoutChannels:
                    return GetOrphansIds<ServiceVersioned>(unitOfWork).Count();
                case TasksIdsEnum.ChannelsWithoutServices:
                    return GetOrphansIds<ServiceChannelVersioned>(unitOfWork).Count();
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

        public Dictionary<Guid, VmExpirationOfEntity> GetExpirationInformation<TEntity>(IUnitOfWork unitOfWork, Guid unificRootId, Guid publishingStatusId)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, new()
        {
            var dateTimeUtcNow = DateTime.UtcNow;
            var expirations = GetEntityAllEntitiesWithGroup<TEntity>(unitOfWork, publishingStatusId, new List<Guid>() {unificRootId}, 0);
            
            var entityTypeName = typeof(TEntity).Name.Replace("Versioned", "");
            var configurationRepository = unitOfWork.CreateRepository<IVTasksConfigurationRepository>();
            var expiration = configurationRepository.All().Where(x =>
                    x.Entity == entityTypeName && x.PublishingStatusId == publishingStatusId)
                .OrderBy(x => x.Interval).FirstOrDefault()?.Interval;

            if (!expiration.HasValue) return null;
            
            var result = new Dictionary<Guid, VmExpirationOfEntity>();
            
            var lifeTime = expiration.Value;
            
            if ((expirations == null || !expirations.Entities.Any()) && unificRootId.IsAssigned()) //prepare setting of expireOn 
            {
                var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
                expirations = expirations ?? new Tasks();
                expirations.Entities = repository.All().Where(x => x.UnificRootId == unificRootId && x.PublishingStatusId == publishingStatusId)
                    .Select(x => new VmTaskEntity() {UnificRootId = x.UnificRootId, Modified = x.Modified}).ToList();
            }

            expirations?.Entities.ForEach(e =>
                {
                    result.Add(e.UnificRootId, new VmExpirationOfEntity()
                    {
                        ExpireOn = dateTimeUtcNow.Add(e.Modified - lifeTime),
                        IsWarningVisible = expirations.IsPostponeButtonVisible
                    });
                });

            return result;
        }
        
        private Tasks GetEntityAllEntitiesWithGroup<TEntity>(IUnitOfWork unitOfWork, Guid publishingStatusId, IEnumerable<Guid> definedEntities = null, int? skip = null)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, new()
        {
            if (!skip.HasValue) skip = 1;
            var userOrganizations = serviceUtilities.GetAllUserOrganizations();
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entityTypeName = typeof(TEntity).Name.Replace("Versioned", "");
            var configurationRepository = unitOfWork.CreateRepository<IVTasksConfigurationRepository>();
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
            var entities = resultTemp.Where(x => userOrganizations.Contains(x.OrganizationId) &&
                           x.Modified < firstWarningTime)
                .Select(x => new VmTaskEntity () { UnificRootId = x.UnificRootId, Modified = x.Modified}).ToList();

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
        
        private IEnumerable<Guid> GetEntityIds<TEntity>(IUnitOfWork unitOfWork, Guid publishingStatusId)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, new()
        {
            return GetEntityAllEntitiesWithGroup<TEntity>(unitOfWork, publishingStatusId).Entities.Select(x => x.UnificRootId);
        }
        
        private IEnumerable<Guid> GetEntityIds<TEntity>(IUnitOfWork unitOfWork, PublishingStatus publishingStatus)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, new()
        {
            var publishingStatusId = publishingStatusCache.Get(publishingStatus);

            return GetEntityIds<TEntity>(unitOfWork, publishingStatusId);
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

        private void PostoponeEntities<TEntity>(Guid publishingStatusId)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, new()
        {
            contextManager.ExecuteWriter(unitOfWork =>
                {
                    TranslationManagerToEntity.TranslateAll<VmTaskEntity, TasksFilter>(
                        GetEntityAllEntitiesWithGroup<TEntity>(unitOfWork, publishingStatusId).Entities.Where(x=>!x.LowerThanLast), unitOfWork);
                    unitOfWork.Save();
                });
        }

        public IVmSearchBase PostoponeEntities(VmPostponeTasks model)
        {
            switch (model.TaskType)
            {
                case TasksIdsEnum.OutdatedDraftServices:
                    PostoponeEntities<ServiceVersioned>(publishingStatusCache.Get(PublishingStatus.Draft));
                    break;
                case TasksIdsEnum.OutdatedDraftChannels:
                    PostoponeEntities<ServiceChannelVersioned>(publishingStatusCache.Get(PublishingStatus.Draft));
                    break;
                case TasksIdsEnum.OutdatedPublishedChannels:
                    PostoponeEntities<ServiceChannelVersioned>(publishingStatusCache.Get(PublishingStatus.Published));
                    break;
                case TasksIdsEnum.OutdatedPublishedServices:
                    PostoponeEntities<ServiceVersioned>(publishingStatusCache.Get(PublishingStatus.Published));
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
                        break;
                    case TasksIdsEnum.ChannelsWithoutServices:
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
                            tasks = GetEntityAllEntitiesWithGroup<ServiceChannelVersioned>(unitOfWork,
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
                                    y.PublishingStatusId == publishingStatusCache.Get(PublishingStatus.Published)) ?? x.First()).OrderBy(x=>x.Modified);
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
                            tasks = GetEntityAllEntitiesWithGroup<ServiceVersioned>(unitOfWork, publishingStatusId);
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
                                    y.PublishingStatusId == publishingStatusCache.Get(PublishingStatus.Published)) ?? x.First()).OrderBy(x=>x.Modified);
                            result.Count = model.GetCount ? GetTasksIdsCount(unitOfWork, model.TaskType) : -1;
                            result.IsPostponeButtonVisible = tasks?.IsPostponeButtonVisible ?? false;
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

            contextManager.ExecuteWriter(unitOfWork =>
            {
                try
                {
                    ArchiveByExpirationDate<ServiceVersioned>(unitOfWork, statusesToArchive);
                    ArchiveByExpirationDate<ServiceChannelVersioned>(unitOfWork, statusesToArchive);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                }
                catch (Exception ex)
                {
                    var errorMsg = string.Format($"Archiving of entities by expiration date thrown exception: {ex.Message}");
                    logger.LogError(errorMsg + " " + ex.StackTrace);
                }
            });
        }

        private void ArchiveByExpirationDate<TEntity>(IUnitOfWorkWritable unitOfWork, List<PublishingStatus> publishingStatuses)
            where TEntity : class, IAuditing, IEntityIdentifier, IVersionedVolume, new()
        {
            var entitiesToArchive = new List<TEntity>();
            foreach (var status in publishingStatuses)
            {
                entitiesToArchive.AddRange(GetEntityIdsByExpirationDate<TEntity>(unitOfWork, status));
            }

            //Console.WriteLine($"Entity: '{typeof(TEntity).Name}' have {entitiesToArchive.Count} items after expiration date.");
            logger.LogInformation($"Entity: '{typeof(TEntity).Name}' have {entitiesToArchive.Count} items after expiration date.");
           
            //Archiving 
            ArchiveEntities(entitiesToArchive);
        }

        private void ArchiveEntities<TEntity>(List<TEntity> entitiesToArchive)
            where TEntity : class, IAuditing, IEntityIdentifier, IVersionedVolume, new()
        {
            var publishingStatusDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            foreach (var entity in entitiesToArchive)
            {
                entity.PublishingStatus = null;
                entity.PublishingStatusId = publishingStatusDeleted;
            }
        }

        private IEnumerable<TEntity> GetEntityIdsByExpirationDate<TEntity>(IUnitOfWorkWritable unitOfWork, PublishingStatus publishingStatus)
            where TEntity : class, IAuditing, IEntityIdentifier, IVersionedVolume, new()
        {
            var publishingStatusId = publishingStatusCache.Get(publishingStatus);
            var expiration = GetExpiration<TEntity>(unitOfWork, publishingStatusId);
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var expirationDate = (expiration > DateTime.MinValue)
                ? expiration
                : DateTime.MinValue;

            //Console.WriteLine($"Entity: '{typeof(TEntity).Name}' publishingStatus: {publishingStatus} with expiration date: {expirationDate} ({expirationWeekNumber}weeks)");
            logger.LogInformation($"Entity: '{typeof(TEntity).Name}' publishingStatus: {publishingStatus} with expiration date: {expirationDate} ({expiration})");

            return repository.All()
                .Where(x => x.PublishingStatusId == publishingStatusId && x.Modified < expirationDate)
                .ToList();
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
    }
}
