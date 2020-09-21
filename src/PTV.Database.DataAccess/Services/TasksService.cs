﻿/**
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
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
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
using PTV.Domain.Logic;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using IOrganizationServiceInternal = PTV.Database.DataAccess.Interfaces.Services.V2.IOrganizationServiceInternal;
using PTV.Database.DataAccess.Utils;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Notifications;
using PTV.Framework.Interfaces;
using PTV.Framework.Logging;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ITasksService), RegisterType.Transient)]
    internal class TasksService : ServiceBase, ITasksService
    {
        private readonly IContextManager contextManager;
        private readonly IServiceUtilities serviceUtilities;
        private readonly IPublishingStatusCache publishingStatusCache;
        private readonly IServiceService serviceService;
        private readonly IChannelService channelService;
        private readonly ILogger logger;
        private readonly ICommonServiceInternal commonService;
        private readonly IOrganizationServiceInternal organizationService;
        private readonly ISearchServiceInternal searchService;
        private readonly IBrokenLinkService brokenLinkService;
        private readonly IExpirationService expirationService;
        private readonly IExpirationTimeCache expirationTimeCache;
        private readonly ITypesCache typesCache;
        
        private readonly int searchOlderThanNumberOfMonth = 1;
        private readonly int searchArchivedContentOlderThanNumberOfMonth = 6;

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
            ISearchServiceInternal searchService,
            IBrokenLinkService brokenLinkService,
            IExpirationService expirationService,
            IExpirationTimeCache expirationTimeCache, 
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
            this.organizationService = organizationService;
            this.searchService = searchService;
            this.brokenLinkService = brokenLinkService;
            this.expirationService = expirationService;
            this.expirationTimeCache = expirationTimeCache;
            this.typesCache = cacheManager.TypesCache;
        }

        public IVmListItemsData<VmTasksBase> GetTasksNumbers()
        {
            var userOrganizations = serviceUtilities.GetAllUserOrganizations();
            return GetTasksNumbers(userOrganizations);
        }

        public IVmListItemsData<VmTasksBase> GetTasksNumbers(IList<Guid> forOrganizations)
        {
                var result = new VmListItemsData<VmTasksBase>();
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var tasksIds = GetTasksIds(unitOfWork,forOrganizations);

                    result.Add(new VmTasksBase { Id = TasksIdsEnum.OutdatedDraftServices, Count = tasksIds.OutdatedDraftServicesIds.Count()});
                    result.Add(new VmTasksBase { Id = TasksIdsEnum.OutdatedDraftChannels, Count = tasksIds.OutdatedDraftChannelsIds.Count()});
                    result.Add(new VmTasksBase { Id = TasksIdsEnum.OutdatedPublishedServices, Count = tasksIds.OutdatedPublishedServicesIds.Count()});
                    result.Add(new VmTasksBase { Id = TasksIdsEnum.OutdatedPublishedChannels, Count = tasksIds.OutdatedPublishedChannelsIds.Count()});
                    result.Add(GetContentArchivedCount(unitOfWork, forOrganizations));
                    result.Add(new VmTasksBase { Id = TasksIdsEnum.ServicesWithoutChannels, Count = tasksIds.ServicesWithoutChannelsIds.Count()});
                    result.Add(new VmTasksBase { Id = TasksIdsEnum.ChannelsWithoutServices, Count = tasksIds.ChannelsWithoutServicesIds.Count()});
                    result.Add(new VmTasksBase { Id = TasksIdsEnum.MissingLanguageOrganizations, Count = tasksIds.MissingLanguageOrganizationsIds.Count()});
                    result.Add(GetTranslationArrivedCount(unitOfWork,forOrganizations));
                    result.Add(GetTranslationInProgressCount(unitOfWork,forOrganizations));
                    result.Add(GetTimedPublishFailedCount(unitOfWork, forOrganizations));
                    result.Add(new VmTasksBase { Id = TasksIdsEnum.UnstableLinks, Count = brokenLinkService.GetUnstableLinksCount(forOrganizations, false, unitOfWork)});
                    result.Add(new VmTasksBase { Id = TasksIdsEnum.ExceptionLinks, Count = brokenLinkService.GetUnstableLinksCount(forOrganizations, true, unitOfWork)});
                });

            return result;
        }

        private VmTasksBase GetTimedPublishFailedCount(IUnitOfWork unitOfWork, IList<Guid> forOrganizations)
        {
            var serviceLangRepo = unitOfWork.CreateRepository<IServiceLanguageAvailabilityRepository>();
            var serviceCollectionLangRepo =
                unitOfWork.CreateRepository<IServiceCollectionLanguageAvailabilityRepository>();
            var channelLangRepo = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
            var organizationLangRepo = unitOfWork.CreateRepository<IOrganizationLanguageAvailabilityRepository>();
            var generalDescriptionLangRepo = unitOfWork.CreateRepository<IGeneralDescriptionLanguageAvailabilityRepository>();

            var serviceCount = serviceLangRepo.All()
                .Include(x => x.ServiceVersioned)
                .Count(x => x.LastFailedPublishAt.HasValue &&
                            forOrganizations.Contains(x.ServiceVersioned.OrganizationId));
            var serviceCollectionCount = serviceCollectionLangRepo.All()
                .Include(x => x.ServiceCollectionVersioned)
                .Count(x => x.LastFailedPublishAt.HasValue &&
                            forOrganizations.Contains(x.ServiceCollectionVersioned.OrganizationId));
            var channelCount = channelLangRepo.All()
                .Include(x => x.ServiceChannelVersioned)
                .Count(x => x.LastFailedPublishAt.HasValue &&
                            forOrganizations.Contains(x.ServiceChannelVersioned.OrganizationId));
            var organizationCount = organizationLangRepo.All()
                .Include(x => x.OrganizationVersioned)
                .Count(x => x.LastFailedPublishAt.HasValue &&
                            forOrganizations.Contains(x.OrganizationVersioned.UnificRootId));
            var generalDescriptionCount = 0;

            if (serviceUtilities.UserHighestRole() == UserRoleEnum.Eeva)
            {
                generalDescriptionCount = generalDescriptionLangRepo.All()
                    .Include(x => x.StatutoryServiceGeneralDescriptionVersioned)
                    .Count(x => x.LastFailedPublishAt.HasValue);
            }
            return new VmTasksBase
            {
                Id = TasksIdsEnum.TimedPublishFailed,
                Count = serviceCount + serviceCollectionCount + channelCount + organizationCount + generalDescriptionCount
            };
        }

        private VmTasksBase GetTranslationArrivedCount(IUnitOfWork unitOfWork, IList<Guid> forOrganizations)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingTranslationOrderRepository>();
            var count = trackingRepository
                .All()
                .Where(x => forOrganizations.Contains(x.OrganizationId))
                .Count(x => x.Created >= DateTime.UtcNow.AddMonths(-searchOlderThanNumberOfMonth));
            return new VmTasksBase
            {
                Id = TasksIdsEnum.TranslationArrived,
                Count = count
            };

        }
        
        private VmTasksBase GetTranslationInProgressCount(IUnitOfWork unitOfWork, IList<Guid> forOrganizations)
        {
            var arrivedStateTypeId = typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.Arrived.ToString());
            var translationOrderStateRepo = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var count = translationOrderStateRepo.All()
                .Where(x => x.Last 
                            && x.TranslationStateId != arrivedStateTypeId 
                            && forOrganizations.Contains(x.TranslationOrder.OrganizationIdentifier.Value))
                .Count(x => x.SendAt >= DateTime.UtcNow.AddMonths(-searchOlderThanNumberOfMonth));
            
            return new VmTasksBase
            {
                Id = TasksIdsEnum.TranslationInProgress,
                Count = count
            };
        }

        private const int MAX_RESULTS = 10;

        private VmTasks GetBrokenLinks(IUnitOfWork unitOfWork, IList<Guid> forOrganizations, bool isException)
        {
            var brokenLinks = brokenLinkService.GetBrokenLinks(unitOfWork, isException, forOrganizations).ToList();

            return new VmTasks
            {
                Id = TasksIdsEnum.TimedPublishFailed,
                Count = brokenLinks.Count,
                Entities = brokenLinks,
                PageNumber = 1,
                MoreAvailable = false,
                MaxPageCount = 1
            };
        }

        private VmTasks GetTimedPublishFailed(IUnitOfWork unitOfWork, IList<Guid> forOrganizations)
        {
            var services = GetTimedPublishFailedServices(unitOfWork, forOrganizations);
            var serviceCollections = GetTimedPublishFailedServiceCollections(unitOfWork, forOrganizations);
            var channels = GetTimedPublishFailedChannels(unitOfWork, forOrganizations);
            var organizations = GetTimedPublishFailedOrganizations(unitOfWork, forOrganizations);
            var generalDescriptions = new List<StatutoryServiceGeneralDescriptionVersioned>();

            if (serviceUtilities.UserHighestRole() == UserRoleEnum.Eeva)
            {
                generalDescriptions = GetTimedPublishFailedGeneralDescriptions(unitOfWork).ToList();
            }

            var serviceTasks = TranslationManagerToVm.TranslateAll<ServiceVersioned, VmTimedPublishFailedTask>(services);
            var serviceCollectionTasks = TranslationManagerToVm.TranslateAll<ServiceCollectionVersioned, VmTimedPublishFailedTask>(serviceCollections);
            var channelTasks = TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmTimedPublishFailedTask>(channels);
            var organizationTasks = TranslationManagerToVm.TranslateAll<OrganizationVersioned, VmTimedPublishFailedTask>(organizations);
            var generalDescriptionTasks = TranslationManagerToVm.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmTimedPublishFailedTask>(generalDescriptions);

            return new VmTasks
            {
                Id = TasksIdsEnum.TimedPublishFailed,
                Count = serviceTasks.SelectMany(x => x.LanguagesAvailabilities).Count()
                        + serviceCollectionTasks.SelectMany(x => x.LanguagesAvailabilities).Count()
                        + channelTasks.SelectMany(x => x.LanguagesAvailabilities).Count()
                        + organizationTasks.SelectMany(x => x.LanguagesAvailabilities).Count()
                        + generalDescriptionTasks.SelectMany(x => x.LanguagesAvailabilities).Count(),
                Entities = serviceTasks.Concat(serviceCollectionTasks).Concat(channelTasks).Concat(organizationTasks)
                    .Concat(generalDescriptionTasks),
                PageNumber = 1,
                MoreAvailable = false,
                MaxPageCount = 1
            };
        }

        private static List<ServiceVersioned> GetTimedPublishFailedServices(IUnitOfWork unitOfWork, IList<Guid> forOrganizations)
        {
            var serviceLangRepo = unitOfWork.CreateRepository<IServiceLanguageAvailabilityRepository>();
            var serviceRepo = unitOfWork.CreateRepository<IServiceVersionedRepository>();

            var serviceIds = serviceLangRepo.All()
                .Include(x => x.ServiceVersioned)
                .Where(x => x.LastFailedPublishAt.HasValue &&
                            forOrganizations.Contains(x.ServiceVersioned.OrganizationId))
                .Select(x => x.ServiceVersionedId)
                .Distinct()
                .ToList();

            var services = serviceRepo.All()
                .Include(x => x.ServiceNames)
                .Include(x => x.LanguageAvailabilities)
                .Include(x => x.Type)
                .Where(x => serviceIds.Contains(x.Id))
                .ToList();
            return services;
        }

        private static List<ServiceCollectionVersioned> GetTimedPublishFailedServiceCollections(IUnitOfWork unitOfWork, IList<Guid> forOrganizations)
        {
            var serviceCollectionLangRepo = unitOfWork.CreateRepository<IServiceCollectionLanguageAvailabilityRepository>();
            var serviceCollectionRepo = unitOfWork.CreateRepository<IServiceCollectionVersionedRepository>();

            var serviceCollectionIds = serviceCollectionLangRepo.All()
                .Include(x => x.ServiceCollectionVersioned)
                .Where(x => x.LastFailedPublishAt.HasValue &&
                            forOrganizations.Contains(x.ServiceCollectionVersioned.OrganizationId))
                .Select(x => x.ServiceCollectionVersionedId)
                .Distinct()
                .ToList();

            var serviceCollections = serviceCollectionRepo.All()
                .Include(x => x.ServiceCollectionNames)
                .Include(x => x.LanguageAvailabilities)
                .Where(x => serviceCollectionIds.Contains(x.Id))
                .ToList();

            return serviceCollections;
        }

        private static List<ServiceChannelVersioned> GetTimedPublishFailedChannels(IUnitOfWork unitOfWork, IList<Guid> forOrganizations)
        {
            var channelLangRepo = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
            var channelRepo = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();

            var channelIds = channelLangRepo.All()
                .Include(x => x.ServiceChannelVersioned)
                .Where(x => x.LastFailedPublishAt.HasValue &&
                            forOrganizations.Contains(x.ServiceChannelVersioned.OrganizationId))
                .Select(x => x.ServiceChannelVersionedId)
                .Distinct()
                .ToList();

            var channels = channelRepo.All()
                .Include(x => x.ServiceChannelNames)
                .Include(x => x.LanguageAvailabilities)
                .Include(x => x.Type)
                .Where(x => channelIds.Contains(x.Id))
                .ToList();
            return channels;
        }

        private static List<OrganizationVersioned> GetTimedPublishFailedOrganizations(IUnitOfWork unitOfWork, IList<Guid> forOrganizations)
        {
            var organizationLangRepo = unitOfWork.CreateRepository<IOrganizationLanguageAvailabilityRepository>();
            var organizationRepo = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();

            var organizationIds = organizationLangRepo.All()
                .Include(x => x.OrganizationVersioned)
                .Where(x => x.LastFailedPublishAt.HasValue &&
                            forOrganizations.Contains(x.OrganizationVersioned.UnificRootId))
                .Select(x => x.OrganizationVersionedId)
                .Distinct()
                .ToList();

            var organizations = organizationRepo.All()
                .Include(x => x.OrganizationNames)
                .Include(x => x.LanguageAvailabilities)
                .Where(x => organizationIds.Contains(x.Id))
                .ToList();
            return organizations;
        }

        private static List<StatutoryServiceGeneralDescriptionVersioned> GetTimedPublishFailedGeneralDescriptions(IUnitOfWork unitOfWork)
        {
            var generalDescriptionLangRepo = unitOfWork.CreateRepository<IGeneralDescriptionLanguageAvailabilityRepository>();
            var generalDescriptionRepo = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();

            var generalDescriptionIds = generalDescriptionLangRepo.All()
                .Include(x => x.StatutoryServiceGeneralDescriptionVersioned)
                .Where(x => x.LastFailedPublishAt.HasValue)
                .Select(x => x.StatutoryServiceGeneralDescriptionVersionedId)
                .Distinct()
                .ToList();

            var generalDescriptions = generalDescriptionRepo.All()
                .Include(x => x.Names)
                .Include(x => x.Type)
                .Include(x => x.LanguageAvailabilities)
                .Where(x => generalDescriptionIds.Contains(x.Id))
                .ToList();
            return generalDescriptions;
        }

        private VmTasks GetTranslationArrived(VmTasksSearch model, IUnitOfWork unitOfWork, IList<Guid> forOrganizations)
        {
            //NOT used for general description, because it has no organization
            var trackingRepository = unitOfWork.CreateRepository<ITrackingTranslationOrderRepository>();
            var translationOrderRepository = unitOfWork.CreateRepository<ITranslationOrderRepository>();
            var pageNumber = model.PageNumber.PositiveOrZero();

            var resultTemp = trackingRepository.All()
                .Where(x => forOrganizations.Contains(x.OrganizationId))
                .Where(x => x.Created >= DateTime.UtcNow.AddMonths(-searchOlderThanNumberOfMonth))
                .OrderByDescending(x => x.Created).ThenBy(x=>x.Id);

            var notifications = resultTemp
                .ApplyPaging(pageNumber, MAX_RESULTS);

            var translationIds = notifications.SearchResult.Select(x => x.TranslationOrderId);

            var translationOrders = translationOrderRepository
                .All()
                .Include(x => x.ServiceChannelTranslationOrders)
                .Include(x => x.ServiceTranslationOrders)
                .Where(x => translationIds.Contains(x.Id))
                .Where(x => x.ServiceTranslationOrders.Any() ||
                            x.ServiceChannelTranslationOrders.Any());

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

            return new VmTasks
            {
                Entities = new VmListItemsData<VmNotification>(
                    notifications.SearchResult
                        .Select(notification =>
                        {
                            var isChannel = serviceChannelTranslationOrders.ContainsKey(notification.TranslationOrderId);
                            var translationOrder = translationOrders.FirstOrDefault(x => x.Id == notification.TranslationOrderId);
                            
                            return new VmNotification
                            {
                                Id = notification.Id,
                                Name = isChannel
                                    ? versionedServiceChannelNames[serviceChannelTranslationOrders[notification.TranslationOrderId]]
                                    : versionedServiceNames[serviceTranslationOrders[notification.TranslationOrderId]],
                                LanguagesAvailabilities = isChannel
                                        ? versionedServiceChannelLanguages[serviceChannelTranslationOrders[notification.TranslationOrderId]]
                                        : versionedServiceLanguages[serviceTranslationOrders[notification.TranslationOrderId]],
                                OperationType = notification.OperationType,
                                CreatedBy = translationOrder?.CreatedBy ?? string.Empty,
                                Created = translationOrder?.Created.ToEpochTime() ?? DateTime.MinValue.ToEpochTime(),
                                EntityType = isChannel
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
        
        private VmTasks GetTranslationInProgress(VmTasksSearch model, IUnitOfWork unitOfWork, IList<Guid> forOrganizations)
        {
            //NOT used for general description, because it has no organization
            var arrivedStateTypeId = typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.Arrived.ToString());
            var translationOrderStateRepo = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var translationOrderRepository = unitOfWork.CreateRepository<ITranslationOrderRepository>();
            var pageNumber = model.PageNumber.PositiveOrZero();

            var resultTemp = translationOrderStateRepo.All()
                .Where(x => x.Last 
                    && x.TranslationStateId != arrivedStateTypeId 
                    && forOrganizations.Contains(x.TranslationOrder.OrganizationIdentifier.Value)
                    && x.SendAt >= DateTime.UtcNow.AddMonths(-searchOlderThanNumberOfMonth))
                .OrderByDescending(x => x.SendAt).ThenBy(x=>x.Id);

            var notifications = resultTemp
                .ApplyPaging(pageNumber, MAX_RESULTS);

            var translationIds = notifications.SearchResult.Select(x => x.TranslationOrderId);

            var translationOrders = translationOrderRepository
                .All()
                .Include(x => x.ServiceChannelTranslationOrders)
                .Include(x => x.ServiceTranslationOrders)
                .Where(x => translationIds.Contains(x.Id))
                .Where(x => x.ServiceTranslationOrders.Any() ||
                            x.ServiceChannelTranslationOrders.Any());

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

            return new VmTasks
            {
                Entities = new VmListItemsData<VmNotification>(
                    notifications.SearchResult
                        .Select(notification =>
                        {
                            var isChannel = serviceChannelTranslationOrders.ContainsKey(notification.TranslationOrderId);
                            var translationOrder = translationOrders.FirstOrDefault(x => x.Id == notification.TranslationOrderId);

                            return new VmNotification
                            {
                                Id = notification.Id,
                                Name = isChannel
                                    ? versionedServiceChannelNames[serviceChannelTranslationOrders[notification.TranslationOrderId]]
                                    : versionedServiceNames[serviceTranslationOrders[notification.TranslationOrderId]],
                                LanguagesAvailabilities = isChannel
                                    ? versionedServiceChannelLanguages[serviceChannelTranslationOrders[notification.TranslationOrderId]]
                                    : versionedServiceLanguages[serviceTranslationOrders[notification.TranslationOrderId]],
                                OperationType = typesCache.GetByValue<TranslationStateType>(notification.TranslationStateId).ToString(),
                                CreatedBy = translationOrder?.CreatedBy ?? string.Empty,
                                Created = translationOrder?.Created.ToEpochTime() ?? DateTime.MinValue.ToEpochTime(),
                                EntityType = isChannel
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
                                TranslationStateTypeId = notification.TranslationStateId
                            };
                        })
                    ),
                Count = resultTemp.Count(),
                MoreAvailable = notifications.MoreAvailable,
                PageNumber = ++pageNumber,
                Id = TasksIdsEnum.TranslationInProgress
            };
        }
        
        private VmTasksBase GetContentArchivedCount(IUnitOfWork unitOfWork, IList<Guid> forOrganizations)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingEntityVersionedRepository>();

            var count = trackingRepository
                .All()
                .Where(x => forOrganizations.Contains(x.OrganizationId))
                .Where(x => x.OperationType == EntityState.Deleted.ToString())
                .Count(x => x.Created >= DateTime.UtcNow.AddMonths(-searchArchivedContentOlderThanNumberOfMonth));
            
            return new VmTasksBase
            {
                Id = TasksIdsEnum.ContentArchived,
                Count = count
            };
        }

        private VmTasks GetContentArchived(IUnitOfWork unitOfWork, VmTasksSearch search, IList<Guid> forOrganizations)
        {
            var trackingRepository = unitOfWork.CreateRepository<ITrackingEntityVersionedRepository>();
            var pageNumber = search.PageNumber.PositiveOrZero();

            var resultTemp = trackingRepository.All()
                .Where(x => x.OperationType == EntityState.Deleted.ToString())
                .Where(x => forOrganizations.Contains(x.OrganizationId))
                .Where(x => x.Created >= DateTime.UtcNow.AddMonths(-searchArchivedContentOlderThanNumberOfMonth))
                .OrderByDescending(x => x.Created)
                .ThenBy(x=>x.Id);

            var channelRootIds = resultTemp
                .Where(notification => notification.EntityType == EntityTypeEnum.Channel.ToString())
                .Select(x => x.UnificRootId)
                .ToList();

            var serviceRootIds = resultTemp
                .Where(notification => notification.EntityType == EntityTypeEnum.Service.ToString())
                .Select(x => x.UnificRootId)
                .ToList();

            var versionedChannels =
                commonService.GetNotificationEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                    channelRootIds,
                    unitOfWork,
                    q => q.Include(a => a.ServiceChannelNames).Include(x => x.DisplayNameTypes));
            var versionedChannelNames = commonService.GetChannelNames(versionedChannels);

            var versionedServices = commonService.GetNotificationEntity<ServiceVersioned, ServiceLanguageAvailability>(
                serviceRootIds,
                unitOfWork,
                q => q.Include(a => a.ServiceNames));
            var versionedServiceNames = commonService.GetEntityNames(versionedServices);

            var serviceNameRep = unitOfWork.CreateRepository<IServiceNameRepository>().All();
            var channelNameRep = unitOfWork.CreateRepository<IServiceChannelNameRepository>().All();
            var deletedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());

            var notifications = resultTemp.Select(notification => new
                {
                    Id = notification.Id,
                    UnificRootId = notification.UnificRootId,
                    EntityType = notification.EntityType,
                    OperationType = notification.OperationType,
                    Name = serviceNameRep.OrderBy(y =>
                                   y.ServiceVersioned.PublishingStatusId == publishingStatusId ? 0 :
                                   y.ServiceVersioned.PublishingStatusId == draftStatusId ? 1 :
                                   y.ServiceVersioned.PublishingStatusId == modifiedStatusId ? 2 : 
                                   y.ServiceVersioned.PublishingStatusId == deletedStatusId ? 3 : 4)
                               .ThenBy(y => y.Localization.Code == DomainConstants.DefaultLanguage ? 0 : 1)
                               .Where(x=>x.ServiceVersioned.UnificRootId == notification.UnificRootId)
                               .Select(z => z.Name)
                               .FirstOrDefault() ??
                           channelNameRep.OrderBy(y => 
                                   y.ServiceChannelVersioned.PublishingStatusId == publishingStatusId ? 0 :
                                   y.ServiceChannelVersioned.PublishingStatusId == draftStatusId ? 1 :
                                   y.ServiceChannelVersioned.PublishingStatusId == modifiedStatusId ? 2 : 
                                   y.ServiceChannelVersioned.PublishingStatusId == deletedStatusId ? 3 : 4)
                               .ThenBy(y => y.Localization.Code == DomainConstants.DefaultLanguage ? 0 : 1)
                               .Where(x => x.ServiceChannelVersioned.UnificRootId == notification.UnificRootId && 
                                                    (x.ServiceChannelVersioned.DisplayNameTypes.Any(dt=> dt.DisplayNameTypeId == x.TypeId && dt.LocalizationId == x.LocalizationId) ||
                                                     x.ServiceChannelVersioned.DisplayNameTypes.All(dt=> dt.LocalizationId != x.LocalizationId)))
                               .Select(z => z.Name)
                               .FirstOrDefault(),
                    CreatedBy = notification.CreatedBy,
                    Created = notification.Created
                })
                .ApplySorting(search.SortData)
                .ApplyPaging(pageNumber, MAX_RESULTS);


            var versionedChannelLanguages =
                commonService.GetLanguageAvailabilites<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(versionedChannels);


            var versionedServiceLanguages =
                commonService.GetLanguageAvailabilites<ServiceVersioned, ServiceLanguageAvailability>(versionedServices);

            return new VmTasks
            {
                Entities = new VmListItemsData<VmArchivedNotification>(
                    notifications.SearchResult
                        .Select(notification => new VmArchivedNotification
                        {
                            Id = notification.EntityType == EntityTypeEnum.Channel.ToString()
                                ? versionedChannels.First(x=>x.UnificRootId == notification.UnificRootId).Id
                                : versionedServices.First(x=>x.UnificRootId == notification.UnificRootId).Id,
                            UnificRootId = notification.UnificRootId,
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
                            EntityType = Enum.Parse<EntityTypeEnum>(notification.EntityType),
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
                Id = TasksIdsEnum.ContentArchived
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

        private TasksIds GetTasksIds(IUnitOfWork unitOfWork, IList<Guid> forOrganizations) {
            var result = new TasksIds();

            result.OutdatedDraftServicesIds = GetEntityIds<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, PublishingStatus.Draft,forOrganizations);

            result.OutdatedDraftChannelsIds = GetEntityIds<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, PublishingStatus.Draft,forOrganizations);

            result.OutdatedPublishedServicesIds = GetEntityIds<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, PublishingStatus.Published,forOrganizations);

            result.OutdatedPublishedChannelsIds = GetEntityIds<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, PublishingStatus.Published,forOrganizations);

            result.ServicesWithoutChannelsIds = GetOrphansIds<ServiceVersioned>(unitOfWork, forOrganizations);
            result.ChannelsWithoutServicesIds = GetOrphansIds<ServiceChannelVersioned>(unitOfWork, forOrganizations);
            result.MissingLanguageOrganizationsIds = GetMissingLanguageOrganizationIds(unitOfWork,forOrganizations).Keys.ToList();
            return result;
        }

        private int GetTasksIdsCount(IUnitOfWork unitOfWork, TasksIdsEnum taskType, IList<Guid> forOrganizations)
        {
            switch (taskType)
            {
                case TasksIdsEnum.OutdatedDraftServices:
                    return GetEntityIds<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, PublishingStatus.Draft,forOrganizations).Count();
                case TasksIdsEnum.OutdatedPublishedServices:
                    return GetEntityIds<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, PublishingStatus.Published,forOrganizations).Count();
                case TasksIdsEnum.OutdatedDraftChannels:
                    return GetEntityIds<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, PublishingStatus.Draft,forOrganizations).Count();
                case TasksIdsEnum.OutdatedPublishedChannels:
                    return GetEntityIds<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, PublishingStatus.Published,forOrganizations).Count();
                case TasksIdsEnum.ServicesWithoutChannels:
                    return GetOrphansIds<ServiceVersioned>(unitOfWork,forOrganizations).Count();
                case TasksIdsEnum.ChannelsWithoutServices:
                    return GetOrphansIds<ServiceChannelVersioned>(unitOfWork,forOrganizations).Count();
                case TasksIdsEnum.MissingLanguageOrganizations:
                    return GetMissingLanguageOrganizationIds(unitOfWork,forOrganizations).Count();
                default:
                    throw new ArgumentOutOfRangeException(nameof(taskType), taskType, null);
            }
        }

        private class Period
        {
            // SFIPTV-1921
            public DateTime Modified { get; set; }
            public Guid PeriodId { get; set; }
            public DateTime Expires { get; set; }
        }

        private class Tasks
        {
            public Tasks()
            {
                Entities = new List<VmTaskEntity>();
            }

            public IEnumerable<VmTaskEntity> Entities { get; set; }
        }

        private IEnumerable<Guid> GetEntityIds<TEntity, TLanguageAvailability>(IUnitOfWork unitOfWork, Guid publishingStatusId, IList<Guid> forOrganizations)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            return expirationService
                .GetExpirationTasks<TEntity, TLanguageAvailability>(unitOfWork, publishingStatusId, forOrganizations)
                .Select(x => x.UnificRootId);
        }

        private IEnumerable<Guid> GetEntityIds<TEntity, TLanguageAvailability>(IUnitOfWork unitOfWork, PublishingStatus publishingStatus, IList<Guid> forOrganizations)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            var publishingStatusId = publishingStatusCache.Get(publishingStatus);

            return GetEntityIds<TEntity, TLanguageAvailability>(unitOfWork, publishingStatusId, forOrganizations);
        }

        private IEnumerable<Guid> GetOrphansIds<TEntity>(IUnitOfWork unitOfWork, IList<Guid> forOrganizations)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, new()
        {
            var publishingStatusIds = new List<Guid>
            {
                publishingStatusCache.Get(PublishingStatus.Published),
                publishingStatusCache.Get(PublishingStatus.Draft),
                publishingStatusCache.Get(PublishingStatus.Modified)
            };

            //var userOrganizations = serviceUtilities.GetAllUserOrganizations();
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entities = repository.All().Where(x => forOrganizations.Contains(x.OrganizationId) && publishingStatusIds.Contains(x.PublishingStatusId));
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

        public IVmSearchBase GetTasksEntities(VmTasksSearch model)
        {
            var userOrganizations = serviceUtilities.GetAllUserOrganizations();
            model.GetCount = true;
            return GetTasksEntities(model, userOrganizations);
        }

        public IVmSearchBase GetBrokenLinks(VmBrokenLinkContentSearch model)
        {
            VmOrganizationBrokenLink result = null;
            var userOrganizations = serviceUtilities.GetAllUserOrganizations();
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = brokenLinkService.GetBrokenLinks(unitOfWork, model.TaskType == TasksIdsEnum.ExceptionLinks, userOrganizations, model.PageNumber, model.Id).First();
            });
            return result;
        }

        public IVmSearchBase GetTasksEntities(VmTasksSearch model, IList<Guid> forOrganizations)
        {
            VmTasks result = null;
            Tasks tasks = null;
            DateTime expiration = DateTime.MinValue;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var configurationRepository = unitOfWork.CreateRepository<IVTasksConfigurationRepository>();
                IList<Guid> entityIds;
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
                        var tt = configurationRepository.All().Where(x =>
                                x.Entity == "Service" && x.PublishingStatusId == publishingStatusId)
                            .OrderBy(x => x.Interval);
                        expiration = configurationRepository.All().Where(x =>
                                x.Entity == "Service" && x.PublishingStatusId == publishingStatusId)
                            .OrderBy(x => x.Interval).First().Interval;
                        break;
                    case TasksIdsEnum.ServicesWithoutChannels:
                    case TasksIdsEnum.TranslationArrived:
                    case TasksIdsEnum.ChannelsWithoutServices:
                    case TasksIdsEnum.MissingLanguageOrganizations:
                    case TasksIdsEnum.TimedPublishFailed:
                    case TasksIdsEnum.UnstableLinks:
                    case TasksIdsEnum.ExceptionLinks:
                    case TasksIdsEnum.TranslationInProgress:
                    case TasksIdsEnum.ContentArchived:
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
                            entityIds = GetOrphansIds<ServiceChannelVersioned>(unitOfWork,forOrganizations).ToList();
                            chPubStatuses = new List<Guid>
                            {
                                publishingStatusCache.Get(PublishingStatus.Published),
                                publishingStatusCache.Get(PublishingStatus.Draft),
                                publishingStatusCache.Get(PublishingStatus.Modified)
                            };
                            if (!model.SortData.Any())
                            {
                                model.SortData = new List<VmSortParam>
                                    {new VmSortParam {Column = "name", SortDirection = SortDirectionEnum.Asc}};
                            }
                        }
                        else
                        {
                            tasks = new Tasks
                            {
                                Entities = expirationService
                                    .GetExpirationTasks<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                                        unitOfWork, publishingStatusId, forOrganizations)
                            };

                            entityIds = tasks.Entities.Select(x => x.UnificRootId).ToList();
                            chPubStatuses = new List<Guid> {publishingStatusId};
                            if (!model.SortData.Any())
                            {
                                model.SortData = new List<VmSortParam>
                                    {new VmSortParam {Column = "ExpireOn", SortDirection = SortDirectionEnum.Asc}};
                            }
                        }

                        var channels = entityIds.Count != 0 ? searchService.SearchEntities(new VmEntitySearch
                        {
                            SortData = model.SortData,
                            SelectedPublishingStatuses = chPubStatuses,
                            MaxPageCount = model.MaxPageCount,
                            PageNumber = model.PageNumber,
                            Skip = model.Skip,
                            EntityIds = entityIds,
                            Expiration = expiration,
                            ContentTypes = new List<SearchEntityTypeEnum> {SearchEntityTypeEnum.Channel},
                            Language = model.Language,
                            Languages = model.Language.IsNullOrEmpty() ? null : new List<string>{model.Language}
                        }, unitOfWork) : null;
                        result = new VmTasks(channels);
                        if (channels != null)
                        {
                            result.Entities = channels.SearchResult.GroupBy(x => x.UnificRootId).Select(x =>
                                x.FirstOrDefault(y =>
                                    y.PublishingStatusId == publishingStatusCache.Get(PublishingStatus.Published)) ?? x.First());
                            result.Count = model.GetCount ? result.Count : -1;
                        }

                        break;

                    case TasksIdsEnum.OutdatedDraftServices:
                    case TasksIdsEnum.OutdatedPublishedServices:
                    case TasksIdsEnum.ServicesWithoutChannels:
                        List<Guid> sPubStatuses = null;
                        if (model.TaskType == TasksIdsEnum.ServicesWithoutChannels)
                        {
                            entityIds = GetOrphansIds<ServiceVersioned>(unitOfWork,forOrganizations).ToList();
                            sPubStatuses = new List<Guid>
                            {
                                publishingStatusCache.Get(PublishingStatus.Published),
                                publishingStatusCache.Get(PublishingStatus.Draft),
                                publishingStatusCache.Get(PublishingStatus.Modified)
                            };
                            if (!model.SortData.Any())
                            {
                                model.SortData = new List<VmSortParam>
                                    {new VmSortParam {Column = "name", SortDirection = SortDirectionEnum.Asc}};
                            }
                        }
                        else
                        {
                            tasks = new Tasks
                            {
                                Entities = expirationService
                                    .GetExpirationTasks<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork,
                                        publishingStatusId, forOrganizations)
                            };
                            entityIds = tasks.Entities.Select(x => x.UnificRootId).ToList();
                            sPubStatuses = new List<Guid> {publishingStatusId};
                            if (!model.SortData.Any())
                            {
                                model.SortData = new List<VmSortParam>
                                    {new VmSortParam {Column = "ExpireOn", SortDirection = SortDirectionEnum.Asc}};
                            }
                        }

                        var services = entityIds.Count != 0 ? searchService.SearchEntities(new VmEntitySearch
                        {
                            SortData = model.SortData,
                            MaxPageCount = model.MaxPageCount,
                            PageNumber = model.PageNumber,
                            Skip = model.Skip,
                            SelectedPublishingStatuses = sPubStatuses,
                            EntityIds = entityIds,
                            Expiration = expiration,
                            ContentTypes = new List<SearchEntityTypeEnum> {SearchEntityTypeEnum.Service},
                            Language = model.Language,
                            Languages = model.Language.IsNullOrEmpty() ? null : new List<string>{model.Language}
                        }, unitOfWork) : null;

                        result = new VmTasks(services);
                        if (services != null)
                        {
                            result.Entities = services.SearchResult.GroupBy(x => x.UnificRootId).Select(x =>
                                x.FirstOrDefault(y =>
                                    y.PublishingStatusId == publishingStatusCache.Get(PublishingStatus.Published)) ?? x.First());
                            result.Count = model.GetCount ? result.Count : -1;
                        }
                        break;
                    case TasksIdsEnum.TranslationArrived:
                        result = GetTranslationArrived(model, unitOfWork, forOrganizations);
                        break;
                    case TasksIdsEnum.TranslationInProgress:
                        result = GetTranslationInProgress(model, unitOfWork, forOrganizations);
                        break;
                    case TasksIdsEnum.MissingLanguageOrganizations:
                        var missingLanguages = GetMissingLanguageOrganizationIds(unitOfWork,forOrganizations);
                        entityIds = missingLanguages.Keys.ToList();
                        sPubStatuses = new List<Guid>
                        {
                            publishingStatusCache.Get(PublishingStatus.Published),
                            publishingStatusCache.Get(PublishingStatus.Draft),
                            publishingStatusCache.Get(PublishingStatus.Deleted)
                        };
                        var organizations = entityIds.Count != 0 ? searchService.SearchEntities(new VmEntitySearch
                        {
                            SortData = model.SortData,
                            MaxPageCount = model.MaxPageCount,
                            PageNumber = model.PageNumber,
                            Skip = model.Skip,
                            EntityVersionIds = entityIds,
                            SelectedPublishingStatuses = sPubStatuses,
                            ContentTypes = new List<SearchEntityTypeEnum> {SearchEntityTypeEnum.Organization}
                        }, unitOfWork) as VmSearchResult<IVmEntityListItem> : null;
                        result = new VmTasks(organizations);
                        if (organizations != null)
                        {
                            result.Entities = organizations.SearchResult;
                            result.Entities.OfType<VmEntityListItem>().ForEach(org => org.MissingLanguages = missingLanguages[org.Id]);
                            result.Count = model.GetCount ? organizations.SearchResult.Count : -1;
                        }
                        break;
                    case TasksIdsEnum.TimedPublishFailed:
                        result = GetTimedPublishFailed(unitOfWork, forOrganizations);
                        break;
                    case TasksIdsEnum.UnstableLinks:
                        result = GetBrokenLinks(unitOfWork, forOrganizations, false);
                        break;
                    case TasksIdsEnum.ExceptionLinks:
                        result = GetBrokenLinks(unitOfWork, forOrganizations, true);
                        break;
                    case TasksIdsEnum.ContentArchived:
                        result = GetContentArchived(unitOfWork, model, forOrganizations);
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

        public void ArchiveEntityByExpirationDate(VmJobLogEntry logInfo)
        {
            var utcNow = DateTime.UtcNow;
            var statusesToArchive = new List<PublishingStatus>
            {
                PublishingStatus.Draft,
                PublishingStatus.Published
            };

            commonService.ExtendPublishingStatusesByEquivalents(statusesToArchive);

            try
            {
                ArchiveByExpirationDate<ServiceVersioned, ServiceLanguageAvailability>(statusesToArchive, utcNow);
                ArchiveByExpirationDate<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(statusesToArchive, utcNow);
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format($"Archiving of entities by expiration date thrown exception: {ex.Message}");
                logger.LogSchedulerError(logInfo, errorMsg + " " + ex.StackTrace);
            };
        }

        // SFIPTV-1921
        private void ArchiveByExpirationDate<TEntity, TLanguageAvailability>(List<PublishingStatus> publishingStatuses, DateTime utcNow)
            where TEntity : class, IAuditing, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IOrganizationInfo, IExpirable, new()
            where TLanguageAvailability : class, ILanguageAvailability
        {
            var entitiesToArchive = new Dictionary<string, List<TEntity>>();
            foreach (var status in publishingStatuses)
            {
                if (status == PublishingStatus.Modified)
                {
                    var expiredAfterMonthsPublished = GetExpirationMonthCount<TEntity>(PublishingStatus.Published);
                    var expiredAfterMonthsDraft = GetExpirationMonthCount<TEntity>(PublishingStatus.Draft);

                    var entities = contextManager.ExecuteReader(unitOfWork =>
                        expirationService.GetEntityIdsByExpirationDate<TEntity, TLanguageAvailability>(unitOfWork,
                            status, utcNow));

                    var enumerable = entities.ToList();
                    var modifiedWithoutPublish = enumerable.Where(x =>
                    {
                        if (x?.Versioning?.Meta == null)
                            return true;

                        var metaData = JsonConvert.DeserializeObject<VmHistoryMetaData>(x.Versioning.Meta);
                        return metaData.HistoryAction == HistoryAction.Withdraw || metaData.HistoryAction == HistoryAction.Restore || metaData.HistoryAction == HistoryAction.MassRestore;
                    });

                    var modifiedWithPublish = enumerable.Where(x =>
                    {
                        if (x?.Versioning?.Meta == null)
                            return false;

                        var metaData = JsonConvert.DeserializeObject<VmHistoryMetaData>(x.Versioning.Meta);
                        return metaData.HistoryAction != HistoryAction.Withdraw && metaData.HistoryAction != HistoryAction.Restore && metaData.HistoryAction != HistoryAction.MassRestore;
                    });

                    UpdateEntitiesToArchive(entitiesToArchive, modifiedWithPublish, expiredAfterMonthsPublished);
                    UpdateEntitiesToArchive(entitiesToArchive, modifiedWithoutPublish, expiredAfterMonthsDraft);
                }
                else
                {
                    var expiredAfterMonths = GetExpirationMonthCount<TEntity>(status);
                    var entities = contextManager.ExecuteReader(unitOfWork =>
                        expirationService.GetEntityIdsByExpirationDate<TEntity, TLanguageAvailability>(unitOfWork,
                            status, utcNow));
                    UpdateEntitiesToArchive(entitiesToArchive, entities, expiredAfterMonths);

                }

                //Console.WriteLine($"Entity: '{typeof(TEntity).Name}' have {entitiesToArchive.Count} items after expiration date.");
                logger.LogInformation($"Entity: '{typeof(TEntity).Name}' have {entitiesToArchive.Count} items after expiration date.");
            }

            //Archiving
            ArchiveEntities<TEntity, TLanguageAvailability>(entitiesToArchive);
        }

        private void UpdateEntitiesToArchive<TEntity>(Dictionary<string, List<TEntity>> entitiesToArchive, IEnumerable<TEntity> entities, string expiredAfterMonths = "NaN")
        {
            if (entitiesToArchive.TryGetValue(expiredAfterMonths, out var entitiesToArchiveTemp))
            {
                entitiesToArchiveTemp.AddRange(entities);
            }
            else
            {
                entitiesToArchive.Add(expiredAfterMonths,entities.ToList());
            }
        }

        private string GetExpirationMonthCount<TEntity>(PublishingStatus status)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var configurationRepository = unitOfWork.CreateRepository<IVTasksConfigurationRepository>();
                var entityTypeName = typeof(TEntity).Name.Replace("Versioned", "");
                var statusId = publishingStatusCache.Get(status.ToString());

                var months = configurationRepository
                    .All()
                    .FirstOrDefault(x => x.Entity == entityTypeName
                                         && x.PublishingStatusId == statusId
                                         && x.Code.Contains("ExpirationTime"))?.Months;

                return months?.ToString("#.#");
            });
        }

        private void ArchiveEntities<TEntity, TLanguageAvailability>(Dictionary<string, List<TEntity>> entitiesToArchive)
            where TEntity : class, IAuditing, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IOrganizationInfo, new()
            where TLanguageAvailability :class, ILanguageAvailability
        {
            foreach (var (expiredAfterMonths, entities) in entitiesToArchive)
            {
                ArchiveEntitiesByBatch<TEntity, TLanguageAvailability>(entities, expiredAfterMonths, 20);
            }

            //Track archive entities
            var uniqEntities = entitiesToArchive.SelectMany(x => x.Value).DistinctBy(x => x.UnificRootId).ToList();
            TrackArchiveEntities(uniqEntities);
        }

        private void ArchiveEntitiesByBatch<TEntity, TLanguageAvailability>(List<TEntity> entities,
            string expiredAfterMonths, int batchSize)
            where TEntity : class, IAuditing, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, new()
            where TLanguageAvailability : class, ILanguageAvailability
        {
            var index = 0;
            foreach (var batch in entities.Batch(batchSize))
            {
                if (Debugger.IsAttached)
                {
                    Console.WriteLine($"Archiving {typeof(TEntity).Name} {index} from {entities.Count} items.");
                }
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    foreach (var entity in batch)
                    {
                        index++;

                        commonService.ChangeEntityVersionedToDeleted<TEntity, TLanguageAvailability>(
                            unitOfWork, entity.Id, HistoryAction.Expired, true, expiredAfterMonths: expiredAfterMonths);
                    }

                    unitOfWork.Save(SaveMode.AllowAnonymous);
                });
            }
        }

        private void TrackArchiveEntities<TEntity>(List<TEntity> entitiesToArchive)
            where TEntity : class, IVersionedVolume, IOrganizationInfo, new()
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var repository = unitOfWork.CreateRepository<ITrackingEntityVersionedRepository>();
                foreach (var entity in entitiesToArchive)
                {
                    repository.All().Where(x =>
                            x.UnificRootId == entity.UnificRootId && x.OperationType == EntityState.Deleted.ToString())
                        .ForEach(r => repository.Remove(r));

                    var entityTrack = new TrackingEntityVersioned
                    {
                        Id = Guid.NewGuid(),
                        UnificRootId = entity.UnificRootId,
                        EntityType = entity is ServiceVersioned
                            ? EntityTypeEnum.Service.ToString()
                            : EntityTypeEnum.Channel.ToString(),
                        OrganizationId = entity.OrganizationId,
                        OperationType = EntityState.Deleted.ToString()
                    };
                    repository.Add(entityTrack);
                }

                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }

        private Dictionary<Guid, List<Guid>> GetMissingLanguageOrganizationIds(IUnitOfWork unitOfWork, IList<Guid> forOrganizations)
        {
            var result = new Dictionary<Guid, List<Guid>>();
            //var userOrgId = serviceUtilities.GetAllUserOrganizations();
            foreach (var orgId in forOrganizations)
            {
                var versionId = VersioningManager.GetLastVersion<OrganizationVersioned>(unitOfWork, orgId)?.EntityId;
                if (versionId.IsAssigned() && !result.ContainsKey(versionId.Value))
                {
                    var missingIds = organizationService.GetOrganizationMissingLanguages(versionId, unitOfWork).ToList();
                    if (missingIds.Any())
                    {
                        result.Add(versionId.Value, missingIds);
                    }
                }
            }

            return result;
        }

        #region Open api

        public IVmOpenApiModelWithPagingBase<VmOpenApiExpiringTask> GetTasks(TasksIdsEnum taskId, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) return new VmOpenApiExpiringTasks();

            Guid? statusId = null;
            var statusIds = new List<Guid>();
            Tasks tasks = null;
            DateTime? expiration = DateTime.MinValue;
            List<Guid> entityIds = null;
            var forOrganizations = serviceUtilities.GetAllUserOrganizations();
            contextManager.ExecuteReader(unitOfWork =>
            {
                switch (taskId)
                {
                    case TasksIdsEnum.OutdatedDraftServices:
                        statusId = publishingStatusCache.Get(PublishingStatus.Draft);
                        statusIds.Add(statusId.Value);
                        statusIds.Add(publishingStatusCache.Get(PublishingStatus.Modified));
                        expiration = expirationTimeCache.GetExpirationTimes(typeof(Service), statusId.Value)?.Modified;
                        tasks = new Tasks
                        {
                            Entities = expirationService
                                .GetExpirationTasks<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork,
                                    statusId.Value, forOrganizations)
                        };
                        break;
                    case TasksIdsEnum.OutdatedPublishedServices:
                        statusId = publishingStatusCache.Get(PublishingStatus.Published);
                        statusIds.Add(statusId.Value);
                        expiration = expirationTimeCache.GetExpirationTimes(typeof(Service), statusId.Value)?.Modified;
                        tasks = new Tasks
                        {
                            Entities = expirationService
                                .GetExpirationTasks<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork,
                                    statusId.Value, forOrganizations)
                        };
                        break;
                    case TasksIdsEnum.OutdatedDraftChannels:
                        statusId = publishingStatusCache.Get(PublishingStatus.Draft);
                        statusIds.Add(statusId.Value);
                        statusIds.Add(publishingStatusCache.Get(PublishingStatus.Modified));
                        expiration = expirationTimeCache.GetExpirationTimes(typeof(ServiceChannel), statusId.Value)?.Modified;
                        tasks = new Tasks
                        {
                            Entities = expirationService
                                .GetExpirationTasks<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                                    unitOfWork, statusId.Value, forOrganizations)
                        };
                        break;
                    case TasksIdsEnum.OutdatedPublishedChannels:
                        statusId = publishingStatusCache.Get(PublishingStatus.Published);
                        statusIds.Add(statusId.Value);
                        expiration = expirationTimeCache.GetExpirationTimes(typeof(ServiceChannel), statusId.Value)?.Modified;
                        tasks = new Tasks
                        {
                            Entities = expirationService
                                .GetExpirationTasks<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                                    unitOfWork, statusId.Value, forOrganizations)
                        };
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

        public IVmOpenApiModelWithPagingBase<VmOpenApiTask> GetOrphanItemsTasks(TasksIdsEnum taskId, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) return new VmOpenApiTasks();

            var statusIds = new List<Guid>();
            List<Guid> entityIds = null;
            var forOrganizations = serviceUtilities.GetAllUserOrganizations();
            contextManager.ExecuteReader(unitOfWork =>
            {
                switch (taskId)
                {
                    case TasksIdsEnum.ServicesWithoutChannels:
                        entityIds = GetOrphansIds<ServiceVersioned>(unitOfWork,forOrganizations).ToList();
                        statusIds.Add(publishingStatusCache.Get(PublishingStatus.Published));
                        statusIds.Add(publishingStatusCache.Get(PublishingStatus.Draft));
                        statusIds.Add(publishingStatusCache.Get(PublishingStatus.Modified));
                        break;
                    case TasksIdsEnum.ChannelsWithoutServices:
                        entityIds = GetOrphansIds<ServiceChannelVersioned>(unitOfWork,forOrganizations).ToList();
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
