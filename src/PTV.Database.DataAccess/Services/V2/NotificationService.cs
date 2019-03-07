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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Logic;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Attributes;

namespace PTV.Database.DataAccess.Services.V2
{
    [RegisterService(typeof(INotificationService), RegisterType.Transient)]
    internal class NotificationService : ServiceBase, INotificationService
    {
        private readonly IContextManager contextManager;
        private readonly ILogger logger;
        private readonly IServiceUtilities utilities;
        private readonly ITypesCache typesCache;
        private readonly ILanguageOrderCache languageOrderCache;
        private readonly ILanguageCache languageCache;
        private readonly IPahaTokenProcessor pahaTokenProcessor;
        private ICommonServiceInternal commonService;
        private ISearchServiceInternal searchService;
        
        private const int MAX_RESULTS = 100;
        
        public NotificationService(
            IContextManager contextManager,
            IVersioningManager versioningManager,
            ILogger<NotificationService> logger,
            IServiceUtilities utilities,
            ICacheManager cacheManager,
            ICommonServiceInternal commonService,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IPahaTokenProcessor pahaTokenProcessor,
            ISearchServiceInternal searchService
        ): base (
                translationManagerToVm,
                translationManagerToEntity,
                publishingStatusCache,
                userOrganizationChecker,
                versioningManager
        ) {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.typesCache = cacheManager.TypesCache;
            this.languageOrderCache = cacheManager.LanguageOrderCache;
            this.languageCache = cacheManager.LanguageCache;
            this.pahaTokenProcessor = pahaTokenProcessor;
            this.commonService = commonService;
            this.searchService = searchService;
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
                GetConnectionChangesCount()
            });
        }
        
        // Service channel in common use
        private VmNotificationsBase GetChannelInCommonUseCount()
        {
            return contextManager.ExecuteReader(GetChannelInCommonUseCount);
        }
        private VmNotificationsBase GetChannelInCommonUseCount(IUnitOfWork unitOfWork)
        {
            var count = GetChannelInCommonUse(unitOfWork).Count();
            return new VmNotificationsBase()
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
            var pageNumber = search.PageNumber.PositiveOrZero();
            var resultTemp = GetChannelInCommonUse(unitOfWork)
                .OrderByDescending(x => x.Created)
                .ThenBy(x => x.Id);
           
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());

            var serNameRep = unitOfWork.CreateRepository<IServiceNameRepository>().All();
            var notifications = resultTemp.Select(notification => new
                {
                    Id = notification.Id,
                    ServiceId = notification.ServiceId,
                    Name = serNameRep.OrderBy(y => 
                        y.ServiceVersioned.PublishingStatusId == publishingStatusId ? 0 : 
                        y.ServiceVersioned.PublishingStatusId == draftStatusId ? 1 : 
                        y.ServiceVersioned.PublishingStatusId == modifiedStatusId ? 2 : 3)
                    .ThenBy(y => 
                            y.Localization.Code == DomainConstants.DefaultLanguage ? 0 : 1)
                    .FirstOrDefault(x=>x.ServiceVersioned.UnificRootId == notification.ServiceId).Name,
                    CreatedBy = notification.CreatedBy,
                    Created = notification.Created
                })
                .ApplySorting(search.SortData)
                .ApplyPaging(pageNumber, MAX_RESULTS);
            
            var versionedServices = commonService.GetNotificationEntity<ServiceVersioned, ServiceLanguageAvailability>(notifications.SearchResult.Select(x => x.ServiceId), unitOfWork, q => q.Include(a => a.ServiceNames));            
            var versionedServiceNames = commonService.GetEntityNames(versionedServices);
            var versionedServicesLanguages = commonService.GetLanguageAvailabilites<ServiceVersioned, ServiceLanguageAvailability>(versionedServices);
           
            return new VmNotifications
            {
                Notifications = new VmListItemsData<VmNotification>(
                    notifications.SearchResult
                        .Select(notification => new VmNotification()
                        {
                            Id = notification.Id,
                            Name = versionedServiceNames[notification.ServiceId],
                            LanguagesAvailabilities = versionedServicesLanguages[notification.ServiceId],
                            CreatedBy = notification.CreatedBy,
                            Created = notification.Created.ToEpochTime(),
                            MainEntityType = EntityTypeEnum.Service,
                            SubEntityType = EntityTypeEnum.Service.ToString(),   
                            VersionedId = versionedServices.First(x=>x.UnificRootId == notification.ServiceId).Id,
                            PublishingStatusId = versionedServices.First(x=>x.UnificRootId == notification.ServiceId).PublishingStatusId
                        })
                    ),
                Count = resultTemp.Count(),
                MoreAvailable = notifications.MoreAvailable,
                PageNumber = ++pageNumber,
                Id = Notification.ServiceChannelInCommonUse
            };
        }
        private IQueryable<NotificationServiceServiceChannel> GetChannelInCommonUse(IUnitOfWork unitOfWork)
        {
            var userOrganizationId = utilities.GetUserMainOrganization();      
            var userId = pahaTokenProcessor.UserName.GetGuid();
            
            var notificationServiceRepository = unitOfWork.CreateRepository<INotificationServiceServiceChannelRepository>();
            return notificationServiceRepository
                .All()
                .Include(n => n.Filters)
                .Where(x => x.OrganizationId == userOrganizationId)
                .Where(x => x.Created >= DateTime.UtcNow.AddMonths(-1))
                .Where(x => x.Filters.All(f => f.UserId != userId));
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
            return new VmNotificationsBase()
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

            var versionedChannels = commonService.GetNotificationEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(resultTemp
                .Where(notification => notification.EntityType == EntityTypeEnum.Channel.ToString())
                .Select(x => x.UnificRootId), unitOfWork, q => q.Include(a => a.ServiceChannelNames));            
            var versionedChannelNames = commonService.GetEntityNames(versionedChannels);
            
            var versionedServices = commonService.GetNotificationEntity<ServiceVersioned, ServiceLanguageAvailability>(resultTemp
                .Where(notification => notification.EntityType == EntityTypeEnum.Service.ToString())
                .Select(x => x.UnificRootId), unitOfWork, q => q.Include(a => a.ServiceNames));
            var versionedServiceNames = commonService.GetEntityNames(versionedServices);

            var chSortedNames = versionedChannelNames.ToDictionary(x => x.Key,
                y => y.Value.ContainsKey(DomainConstants.DefaultLanguage) ? y.Value[DomainConstants.DefaultLanguage] : y.Value.Any() ? y.Value.First().Value : String.Empty);
            var sSortedNames = versionedServiceNames.ToDictionary(x => x.Key,
                y => y.Value.ContainsKey(DomainConstants.DefaultLanguage) ? y.Value[DomainConstants.DefaultLanguage] : y.Value.Any() ? y.Value.First().Value : String.Empty);
            var names = chSortedNames.Merge(sSortedNames);
            
            var notifications = resultTemp.Select(notification => new
                {
                    Id = notification.Id,
                    UnificRootId = notification.UnificRootId,
                    EntityType = notification.EntityType,
                    OperationType = notification.OperationType,
                    Name = names[notification.UnificRootId],
                    CreatedBy = notification.CreatedBy,
                    Created = notification.Created
                })
                .ApplySorting(search.SortData)
                .ApplyPaging(pageNumber, MAX_RESULTS);
            
            
            var versionedChannelLanguages =
                commonService.GetLanguageAvailabilites<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(versionedChannels);

            
            var versionedServiceLanguages =
                commonService.GetLanguageAvailabilites<ServiceVersioned, ServiceLanguageAvailability>(versionedServices);

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
                                ? commonService.GetChannelSubType(notification.UnificRootId, versionedChannels)
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
            return new VmNotificationsBase()
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
            
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var genNameRep = unitOfWork.CreateRepository<IStatutoryServiceNameRepository>().All();
            var notifications = resultTemp.Select(notification => new
                {
                    Id = notification.Id,
                    OperationType = notification.OperationType,
                    GenerealDescriptionId = notification.GenerealDescriptionId,
                    Name = genNameRep.OrderBy(y => 
                            y.StatutoryServiceGeneralDescriptionVersioned.PublishingStatusId == publishingStatusId ? 0 : 
                            y.StatutoryServiceGeneralDescriptionVersioned.PublishingStatusId == draftStatusId ? 1 : 
                            y.StatutoryServiceGeneralDescriptionVersioned.PublishingStatusId == modifiedStatusId ? 2 : 3)
                        .ThenBy(y => 
                            y.Localization.Code == DomainConstants.DefaultLanguage ? 0 : 1)
                        .FirstOrDefault(x=>x.StatutoryServiceGeneralDescriptionVersioned.UnificRootId == notification.GenerealDescriptionId).Name,
                    CreatedBy = notification.CreatedBy,
                    Created = notification.Created
                })
                .ApplySorting(search.SortData)
                .ApplyPaging(pageNumber, MAX_RESULTS);

            var versionedGds = commonService.GetNotificationEntity<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(notifications.SearchResult.Select(x => x.GenerealDescriptionId),
                unitOfWork, q => q.Include(a => a.Names),
                VersioningManager.GetLastPublishedVersion<StatutoryServiceGeneralDescriptionVersioned>);
            
            var gdNames = commonService.GetEntityNames(versionedGds);
            var gdLanguages =
                commonService.GetLanguageAvailabilites<StatutoryServiceGeneralDescriptionVersioned,
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
        
        private VmNotificationsBase GetConnectionChangesCount()
        {
            return new VmNotificationsBase()
            {
                Count = searchService.SearchConnectionsCount(new VmEntitySearch
                {
                    OrganizationIds = utilities.GetUserOrganizations().ToList()
                }),
                Id = Notification.ConnectionChanges
            };
        }
        
        public IVmSearchBase GetConnectionChanges(IVmNotificationSearch search)
        {
            return contextManager.ExecuteReader(unitOfWork => GetConnectionChanges(search, unitOfWork));
        }
        
        private IVmSearchBase GetConnectionChanges(IVmNotificationSearch search, IUnitOfWork unitOfWork)
        {
            var resultTemp = (VmSearchResult<IVmNotificationConnectionListItem>)searchService.SearchConnections(new VmEntitySearch
            {
                OrganizationIds = utilities.GetUserOrganizations().ToList(),
                PageNumber = search.PageNumber,
                Skip = search.Skip,
                SortData = search.SortData,
                MaxPageCount = search.MaxPageCount
            });

            var versionedServices = commonService.GetNotificationEntity<ServiceVersioned, ServiceLanguageAvailability>(resultTemp.SearchResult
                .Select(x => x.EntityType == EntityTypeEnum.Service ? x.EntityId : x.ConnectedId), unitOfWork, q => q.Include(a => a.ServiceNames));
            
            var servicesNames = commonService.GetEntityNames(versionedServices);
            var serviceLanguages = commonService.GetLanguageAvailabilites<ServiceVersioned,
                    ServiceLanguageAvailability>(versionedServices);
            
            var versionedChannels = commonService.GetNotificationEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(resultTemp.SearchResult
                .Select(x => x.EntityType == EntityTypeEnum.Service ? x.ConnectedId : x.EntityId), unitOfWork, q => q.Include(a => a.ServiceChannelNames));
            
            var channelsNames = commonService.GetEntityNames(versionedChannels);
            var channelsLanguages = commonService.GetLanguageAvailabilites<ServiceChannelVersioned,
                ServiceChannelLanguageAvailability>(versionedChannels);
            
            return new VmNotifications()
            {
                Notifications = new VmListItemsData<VmNotification>(
                    resultTemp.SearchResult
                        .Select(notification => new VmNotification()
                        {
                            Id = notification.Id,
                            Name = notification.EntityType == EntityTypeEnum.Service ? servicesNames[notification.EntityId] : channelsNames[notification.EntityId],
                            LanguagesAvailabilities = notification.EntityType == EntityTypeEnum.Service ? serviceLanguages[notification.EntityId] : channelsLanguages[notification.EntityId],
                            OperationType = notification.OperationType,
                            CreatedBy = notification.CreatedBy,
                            Created = notification.Created,
                            MainEntityType = notification.EntityType == EntityTypeEnum.Service ? EntityTypeEnum.Service : EntityTypeEnum.Channel,
                            SubEntityType = notification.EntityType == EntityTypeEnum.Service ? EntityTypeEnum.Service.ToString() : typesCache.GetByValue<ServiceChannelType>(versionedChannels.First(x=>x.UnificRootId==notification.EntityId).TypeId),
                            VersionedId = notification.EntityType == EntityTypeEnum.Service ? versionedServices.First(x=>x.UnificRootId==notification.EntityId).Id : versionedChannels.First(x=>x.UnificRootId==notification.EntityId).Id ,
                            PublishingStatusId = notification.EntityType == EntityTypeEnum.Service ? versionedServices.First(x=>x.UnificRootId==notification.EntityId).PublishingStatusId : versionedChannels.First(x=>x.UnificRootId==notification.EntityId).PublishingStatusId,
                            ConnectedName = notification.EntityType == EntityTypeEnum.Channel ? servicesNames[notification.ConnectedId] : channelsNames[notification.ConnectedId],
                            ConnectedMainEntityType = notification.EntityType == EntityTypeEnum.Channel ? EntityTypeEnum.Service : EntityTypeEnum.Channel,
                            ConnectedSubEntityType = notification.EntityType == EntityTypeEnum.Channel ? EntityTypeEnum.Service.ToString() : typesCache.GetByValue<ServiceChannelType>(versionedChannels.First(x=>x.UnificRootId==notification.ConnectedId).TypeId),
                            ConnectedVersionedId = notification.EntityType == EntityTypeEnum.Channel ? versionedServices.First(x=>x.UnificRootId==notification.ConnectedId).Id : versionedChannels.First(x=>x.UnificRootId==notification.ConnectedId).Id 
                        })
                    ),
                Count = resultTemp.Count,
                MoreAvailable = resultTemp.MoreAvailable,
                PageNumber = resultTemp.PageNumber,
                Id = Notification.ConnectionChanges
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
            count += RemoveNotifications<TrackingGeneralDescriptionVersioned>(unitOfWork);
            count += RemoveNotifications<TrackingTranslationOrder>(unitOfWork);
            count += RemoveNotifications<NotificationServiceServiceChannel>(unitOfWork);
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
    }
}
