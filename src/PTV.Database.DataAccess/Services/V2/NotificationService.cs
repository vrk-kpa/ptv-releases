using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.V2.Notifications;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services.V2
{
    [RegisterService(typeof(INotificationService), RegisterType.Transient)]
    internal class NotificationService : ServiceBase, INotificationService
    {
        private readonly IContextManager contextManager;
        private readonly ILogger logger;
        private readonly ServiceUtilities utilities;
        private readonly IVersioningManager versioningManager;
        private readonly ITypesCache typesCache;
        private readonly ILanguageOrderCache languageOrderCache;
        private readonly ILanguageCache languageCache;
        
        private const int MAX_RESULTS = 10;
        
        public NotificationService(
            IContextManager contextManager,
            IVersioningManager versioningManager,
            ILogger<NotificationService> logger,
            ServiceUtilities utilities,
            ICacheManager cacheManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker
        ): base (
                translationManagerToVm,
                translationManagerToEntity,
                publishingStatusCache,
                userOrganizationChecker            
        ) {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.versioningManager = versioningManager;
            this.typesCache = cacheManager.TypesCache;
            this.languageOrderCache = cacheManager.LanguageOrderCache;
            this.languageCache = cacheManager.LanguageCache;
        }

        public IVmListItemsData<VmNotificationsBase> GetNotificationsNumbers()
        {
            return contextManager.ExecuteReader(unitOfWork => new VmListItemsData<VmNotificationsBase>
            {
                GetChannelInCommonUseCount(unitOfWork),
                GetContentChangesCount(EntityState.Modified,unitOfWork),
                GetContentChangesCount(EntityState.Deleted,unitOfWork),
                GetGeneralDescriptionChangesCount(EntityState.Added,unitOfWork),
                GetGeneralDescriptionChangesCount(EntityState.Modified,unitOfWork),
                GetTranslationArrivedCount(unitOfWork)
            });
        }
        
        // Service channel in common use
        private VmNotificationsBase GetChannelInCommonUseCount()
        {
            return contextManager.ExecuteReader(GetChannelInCommonUseCount);
        }
        private VmNotificationsBase GetChannelInCommonUseCount(IUnitOfWork unitOfWork)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingOrganizationChannelRepository>();
            var userOrganizationId = utilities.GetUserMainOrganization();
            var count = trackingRepository
                .All()
                .Where(x => x.OrganizationId == userOrganizationId)
                .Count(x => x.Created >= DateTime.UtcNow.AddMonths(-1));
            return new VmNotifications()
            {
                Id = Notification.ServiceChannelInCommonUse,
                Count = count
            };
        }

        public IVmSearchBase GetChannelInCommonUse(IVmNotificationSearch search)
        {
            return contextManager.ExecuteReader(unitOfWork => GetChannelInCommonUse(search, unitOfWork));
        }
        private IVmSearchBase GetChannelInCommonUse(IVmNotificationSearch search, IUnitOfWork unitOfWork)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingOrganizationChannelRepository>();
            var pageNumber = search.PageNumber.PositiveOrZero();
            var userOrganizationId = utilities.GetUserMainOrganization();
            var resultTemp = trackingRepository.All()
                .Where(x => x.OrganizationId == userOrganizationId)
                .Where(x => x.Created >= DateTime.UtcNow.AddMonths(-1))
                .OrderByDescending(x => x.Created).ThenBy(x=>x.Id);

            var notifications = resultTemp
                .ApplyPaging(pageNumber, MAX_RESULTS);
                            
            var versionedChannels = GetNotificationEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(notifications.SearchResult.Select(x => x.ServiceChannelId), unitOfWork, q => q.Include(a => a.ServiceChannelNames));            
            var versionedChannelNames = GetEntityNames(versionedChannels);
            var versionedChannelLanguages =
                GetLanguageAvaliabilites<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(versionedChannels);

            return new VmNotifications()
            {
                Notifications = new VmListItemsData<VmNotification>(
                    notifications.SearchResult
                        .Select(notification => new VmNotification()
                        {
                            Id = notification.Id,
                            Name = versionedChannelNames[notification.ServiceChannelId],
                            LanguagesAvailabilities = versionedChannelLanguages[notification.ServiceChannelId],
                            OperationType = notification.OperationType,
                            CreatedBy = notification.CreatedBy,
                            Created = notification.Created.ToEpochTime(),
                            MainEntityType = EntityTypeEnum.Channel,
                            SubEntityType = GetChannelSubType(notification.ServiceChannelId, versionedChannels),   
                            VersionedId = versionedChannels.First(x=>x.UnificRootId == notification.ServiceChannelId).Id,
                            PublishingStatusId = versionedChannels.First(x=>x.UnificRootId == notification.ServiceChannelId).PublishingStatusId
                        })
                    ),
                Count = resultTemp.Count(),
                MoreAvailable = notifications.MoreAvailable,
                PageNumber = ++pageNumber,
                Id = Notification.ServiceChannelInCommonUse
            };
        }
        
        // Suomi.fi content changes/archived
        private VmNotificationsBase GetContentChangesCount(EntityState state)
        {
            return contextManager.ExecuteReader(unitOfWork => GetContentChangesCount(state, unitOfWork));
        }
        private VmNotificationsBase GetContentChangesCount(EntityState state, IUnitOfWork unitOfWork)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingEntityVersionedRepository>();
            var userOrganizationId = utilities.GetUserMainOrganization();
            var count = trackingRepository
                .All()
                .Where(x => x.OrganizationId == userOrganizationId)
                .Where(x => x.OperationType == state.ToString())
                .Count(x => x.Created >= DateTime.UtcNow.AddMonths(-1));
            return new VmNotifications()
            {
                Id = state == EntityState.Modified ? Notification.ContentUpdated : Notification.ContentArchived,
                Count = count
            };
        }
        
        public IVmSearchBase GetContentChanged(IVmNotificationSearch search)
        {
            return contextManager.ExecuteReader(unitOfWork => GetContentChanges(search, unitOfWork, EntityState.Modified));
        }
        public IVmSearchBase GetContentArchived(IVmNotificationSearch search)
        {
            return contextManager.ExecuteReader(unitOfWork => GetContentChanges(search, unitOfWork, EntityState.Deleted));
        }
        private IVmSearchBase GetContentChanges(IVmNotificationSearch search, IUnitOfWork unitOfWork, EntityState state)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingEntityVersionedRepository>();
            var pageNumber = search.PageNumber.PositiveOrZero();
            var userOrganizationId = utilities.GetUserMainOrganization();
            var resultTemp = trackingRepository.All()
                .Where(x => x.OperationType == state.ToString())
                .Where(x => x.OrganizationId == userOrganizationId)
                .Where(x => x.Created >= DateTime.UtcNow.AddMonths(-1))
                .OrderByDescending(x => x.Created).ThenBy(x=>x.Id);

            var notifications = resultTemp
                .ApplyPaging(pageNumber, MAX_RESULTS);
            
            var versionedChannels = GetNotificationEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(notifications.SearchResult
                .Where(notification => notification.EntityType == EntityTypeEnum.Channel.ToString())
                .Select(x => x.UnificRootId), unitOfWork, q => q.Include(a => a.ServiceChannelNames));            
            var versionedChannelNames = GetEntityNames(versionedChannels);
            var versionedChannelLanguages =
                GetLanguageAvaliabilites<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(versionedChannels);

            var versionedServices = GetNotificationEntity<ServiceVersioned, ServiceLanguageAvailability>(notifications.SearchResult
                .Where(notification => notification.EntityType == EntityTypeEnum.Service.ToString())
                .Select(x => x.UnificRootId), unitOfWork, q => q.Include(a => a.ServiceNames));
            var versionedServiceNames = GetEntityNames(versionedServices);
            var versionedServiceLanguages =
                GetLanguageAvaliabilites<ServiceVersioned, ServiceLanguageAvailability>(versionedServices);

            return new VmNotifications()
            {
                Notifications = new VmListItemsData<VmNotification>(
                    notifications.SearchResult
                        .Select(notification => new VmNotification()
                        {
                            Id = notification.Id,
                            VersionedId = notification.EntityType == EntityTypeEnum.Channel.ToString() 
                                ? versionedChannels.First(x=>x.UnificRootId == notification.UnificRootId).Id
                                : versionedServices.First(x=>x.UnificRootId == notification.UnificRootId).Id,
                            Name = notification.EntityType == EntityTypeEnum.Channel.ToString() ?
                                versionedChannelNames[notification.UnificRootId] :
                                versionedServiceNames[notification.UnificRootId]
                            ,
                            LanguagesAvailabilities = notification.EntityType == EntityTypeEnum.Channel.ToString() ?
                                versionedChannelLanguages[notification.UnificRootId] :
                                versionedServiceLanguages[notification.UnificRootId]
                            ,
                            OperationType = notification.OperationType,
                            CreatedBy = notification.CreatedBy,
                            Created = notification.Created.ToEpochTime(),
                            MainEntityType = Enum.Parse<EntityTypeEnum>(notification.EntityType),
                            SubEntityType = notification.EntityType == EntityTypeEnum.Channel.ToString() 
                                ? GetChannelSubType(notification.UnificRootId, versionedChannels)
                                : notification.EntityType,
                            PublishingStatusId =  notification.EntityType == EntityTypeEnum.Channel.ToString() 
                                ? versionedChannels.First(x=>x.UnificRootId == notification.UnificRootId).PublishingStatusId
                                : versionedServices.First(x=>x.UnificRootId == notification.UnificRootId).PublishingStatusId 
                        })
                    ),
                Count = resultTemp.Count(),
                MoreAvailable = notifications.MoreAvailable,
                PageNumber = ++pageNumber,
                Id = state == EntityState.Modified ? Notification.ContentUpdated : Notification.ContentArchived
            };
        }
        
        // Translation arrived
        private VmNotificationsBase GetTranslationArrivedCount()
        {
            return contextManager.ExecuteReader(GetTranslationArrivedCount);
        }
        private VmNotificationsBase GetTranslationArrivedCount(IUnitOfWork unitOfWork)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingTranslationOrderRepository>();
            var userOrganizationId = utilities.GetUserMainOrganization();
            var count = trackingRepository
                .All()
                .Where(x => x.OrganizationId == userOrganizationId)
                .Count(x => x.Created >= DateTime.UtcNow.AddMonths(-1));
            return new VmNotifications()
            {
                Id = Notification.TranslationArrived,
                Count = count
            };
        }
        
        public IVmSearchBase GetTranslationArrived(IVmNotificationSearch search)
        {
            return contextManager.ExecuteReader(unitOfWork => GetTranslationArrived(search, unitOfWork));
        }
        private IVmSearchBase GetTranslationArrived(IVmNotificationSearch search, IUnitOfWork unitOfWork)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingTranslationOrderRepository>();
            var translationOrderRepository = unitOfWork.CreateRepository<ITranslationOrderRepository>();
            var pageNumber = search.PageNumber.PositiveOrZero();
            var userOrganizationId = utilities.GetUserMainOrganization();
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
            
            var versionedServices = GetNotificationEntity<ServiceVersioned, ServiceLanguageAvailability>(serviceTranslationOrders.Values, unitOfWork, q => q.Include(a => a.ServiceNames));
            var versionedServiceNames = GetEntityNames(versionedServices);
            var versionedServiceLanguages = GetLanguageAvaliabilites<ServiceVersioned,ServiceLanguageAvailability>(versionedServices);

            var versionedServiceChannels = GetNotificationEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannelTranslationOrders.Values, unitOfWork, q => q.Include(a => a.ServiceChannelNames));
            var versionedServiceChannelNames = GetEntityNames(versionedServiceChannels);               
            var versionedServiceChannelLanguages = GetLanguageAvaliabilites<ServiceChannelVersioned,ServiceChannelLanguageAvailability>(versionedServiceChannels);
            
            return new VmNotifications()
            {
                Notifications = new VmListItemsData<VmNotification>(
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
                                        ? GetChannelSubType(
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
                Id = Notification.TranslationArrived
            };
        }

        // GD added/changed
        private VmNotificationsBase GetGeneralDescriptionChangesCount(EntityState state)
        {
            return contextManager.ExecuteReader(unitOfWork => GetGeneralDescriptionChangesCount(state, unitOfWork));
        }
        private VmNotificationsBase GetGeneralDescriptionChangesCount(EntityState state, IUnitOfWork unitOfWork)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingGeneralDescriptionVersionedRepository>();
            var count = trackingRepository
                .All()
                .Where(x => x.OperationType == state.ToString())
                .Count(x => x.Created >= DateTime.UtcNow.AddMonths(-1));
            return new VmNotifications()
            {
                Id = state == EntityState.Added ? Notification.GeneralDescriptionCreated : Notification.GeneralDescriptionUpdated,
                Count = count
            };
        }
        
        public IVmSearchBase GetGeneralDescriptionChanged(IVmNotificationSearch search)
        {
            return contextManager.ExecuteReader(unitOfWork => GetGeneralDescriptionChanges(search, unitOfWork, EntityState.Modified));
        }
        public IVmSearchBase GetGeneralDescriptionAdded(IVmNotificationSearch search)
        {
            return contextManager.ExecuteReader(unitOfWork => GetGeneralDescriptionChanges(search, unitOfWork, EntityState.Added));
        }
        private IVmSearchBase GetGeneralDescriptionChanges(IVmNotificationSearch search, IUnitOfWork unitOfWork, EntityState state)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingGeneralDescriptionVersionedRepository>();
            
            var pageNumber = search.PageNumber.PositiveOrZero();
            var resultTemp = trackingRepository.All()
                .Where(x => x.OperationType == state.ToString())
                .Where(x => x.Created >= DateTime.UtcNow.AddMonths(-1))
                .OrderByDescending(x => x.Created).ThenBy(x=>x.Id);

            var notifications = resultTemp
                .ApplyPaging(pageNumber, MAX_RESULTS);
                            
           var versionedGds = GetNotificationEntity<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(notifications.SearchResult.Select(x => x.GenerealDescriptionId),
                unitOfWork, q => q.Include(a => a.Names));
            var gdNames = GetEntityNames(versionedGds);
            var gdLanguages =
                GetLanguageAvaliabilites<StatutoryServiceGeneralDescriptionVersioned,
                    GeneralDescriptionLanguageAvailability>(versionedGds);
            
            return new VmNotifications()
            {
                Notifications = new VmListItemsData<VmNotification>(
                    notifications.SearchResult
                        .Select(notification => new VmNotification()
                        {
                            Id = notification.Id,
                            Name = gdNames[notification.GenerealDescriptionId],
                            LanguagesAvailabilities = gdLanguages[notification.GenerealDescriptionId],
                            OperationType = notification.OperationType,
                            CreatedBy = notification.CreatedBy,
                            Created = notification.Created.ToEpochTime(),
                            MainEntityType = EntityTypeEnum.GeneralDescription,
                            SubEntityType = EntityTypeEnum.GeneralDescription.ToString(),
                            VersionedId = versionedGds.First(x=>x.UnificRootId==notification.GenerealDescriptionId).Id,
                            PublishingStatusId = versionedGds.First(x=>x.UnificRootId==notification.GenerealDescriptionId).PublishingStatusId
                        })
                    ),
                Count = resultTemp.Count(),
                MoreAvailable = notifications.MoreAvailable,
                PageNumber = ++pageNumber,
                Id = state == EntityState.Added ? Notification.GeneralDescriptionCreated : Notification.GeneralDescriptionUpdated
            };
        }

        public void ClearOldNotifications()
        {
            contextManager.ExecuteWriter(ClearOldNotifications);
        }

        private void ClearOldNotifications(IUnitOfWorkWritable unitOfWork)
        {
            var count = 0;

            count += RemoveNotifications<TrackingEntityVersioned>(unitOfWork);
            count += RemoveNotifications<TrackingOrganizationChannel>(unitOfWork);
            count += RemoveNotifications<TrackingGeneralDescriptionVersioned>(unitOfWork);
            count += RemoveNotifications<TrackingTranslationOrder>(unitOfWork);
            unitOfWork.Save(SaveMode.AllowAnonymous);
            Console.WriteLine("Number of removed notifications: {0}", count);
        }

        private int RemoveNotifications<TNotification>(IUnitOfWork unitOfWork) where TNotification : class, IAuditing
        {
            bool Rem(IAuditing x) => x.Created < DateTime.UtcNow.AddMonths(-1);
            var count = 0;
            var repository = unitOfWork.CreateRepository<IRepository<TNotification>>();
            repository.All().AsEnumerable().Where((Func<IAuditing, bool>) Rem).OfType<TNotification>().ForEach(x=>
            {
                repository.Remove(x);
                count++;
            });
            return count;
        }

        private Dictionary<Guid,Dictionary<string,string>> GetEntityNames<TEntity>(List<TEntity> versionedGd) where TEntity : class, IVersionedVolume, INameReferences
        {
            return versionedGd
                .ToDictionary(key => key.UnificRootId, value => value.Names
                    .Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()))
                    .ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));
        }
        
        private Dictionary<Guid,IReadOnlyList<VmLanguageAvailabilityInfo>> GetLanguageAvaliabilites<TEntity, TLanguage>(List<TEntity> entities) where TEntity : class, IVersionedVolume, IMultilanguagedEntity<TLanguage>
            where TLanguage : LanguageAvailability 
        {
            return entities
                .ToDictionary(key => key.UnificRootId, value => TranslationManagerToVm
                    .TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                        value.LanguageAvailabilities
                            .OrderBy(x => languageOrderCache.Get(x.LanguageId))
                    ));
        }
       
        private List<TEntity> GetNotificationEntity<TEntity, TLanguage>(IEnumerable<Guid> unificRootIds, IUnitOfWork unitOfWork, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeChain)
            where TEntity : class, IVersionedVolume, IMultilanguagedEntity<TLanguage>
            where TLanguage : LanguageAvailability 
        {
            var entityRepository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var ids = unificRootIds
                .Select(unificRootId => versioningManager
                    .GetLastVersion<TEntity>(unitOfWork, unificRootId)
                    .EntityId
                )
                .Distinct()
                .ToList();

            var query = entityRepository.All()
                .Include(x => x.LanguageAvailabilities)
                .Where(x => ids.Contains(x.Id));
            return unitOfWork.ApplyIncludes(query, includeChain)            
                .ToList();
        }

        private string GetChannelSubType(Guid entityId,
            List<ServiceChannelVersioned> allEntities)
        {
            var entity = allEntities.FirstOrDefault(x => x.UnificRootId == entityId);
            if (entity != null)
            {
                return typesCache.GetByValue<ServiceChannelType>(entity.TypeId);
            }
            return EntityTypeEnum.Channel.ToString();
        }
    }
}
