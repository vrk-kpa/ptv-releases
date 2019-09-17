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
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Database.DataAccess.Utils;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IAdminTasksService), RegisterType.Transient)]
    internal class AdminTasksService : ServiceBase, IAdminTasksService
    {
        private readonly IContextManager contextManager;
        private readonly ITypesCache typesCache;
        private readonly IPublishingStatusCache publishingStatusCache;
        private readonly ILogger logger;
        private readonly ICommonServiceInternal commonService;
        private readonly ITranslationService translationService;

        public AdminTasksService(ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IContextManager contextManager,
            ICommonServiceInternal commonService,
            ITranslationService translationService,
            IVersioningManager versioningManager,
            ILogger<TasksService> logger,
            ICacheManager cacheManager) :
                base(translationManagerToVm,
                    translationManagerToEntity,
                    publishingStatusCache,
                    userOrganizationChecker,
                    versioningManager)
        {
            this.contextManager = contextManager;
            this.publishingStatusCache = publishingStatusCache;
            this.commonService = commonService;
            this.logger = logger;
            this.translationService = translationService;
            this.typesCache = cacheManager.TypesCache;
        }

        private const int MAX_RESULTS = 1000;
        
        public IVmListItemsData<VmAdminTasksBase> GetTasksNumbers()
        {
            var result = new VmListItemsData<VmAdminTasksBase>();
            contextManager.ExecuteReader(unitOfWork =>
            {
                result.Add(GetFailedTranslationOrdersCount(unitOfWork));
            });

            return result;
        }
        
        public IVmSearchBase GetTasksEntities(VmAdminTasksSearch model)
        {
            VmAdminTasks result = null;
            
            contextManager.ExecuteReader(unitOfWork =>
            {
                switch (model.TaskType)
                {
                    case AdminTasksIdsEnum.FailedTranslationOrders:
                        result = GetFailedTranslationOrders(model, unitOfWork);
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
        
        private VmAdminTasks GetFailedTranslationOrdersCount(IUnitOfWork unitOfWork)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var stateTypeFailForInvestigationId = typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.FailForInvestigation.ToString());
            var count = translationOrderStateRep.All()
                .Where(x => x.Last && x.TranslationStateId == stateTypeFailForInvestigationId)
                .Distinct()
                .Count();
            
            return new VmAdminTasks
            {
                Id = AdminTasksIdsEnum.FailedTranslationOrders,
                Count = count
            };
        }
        
        private VmAdminTasks GetFailedTranslationOrders(VmAdminTasksSearch model, IUnitOfWork unitOfWork)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var stateTypeFailForInvestigationId = typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.FailForInvestigation.ToString());
            var publishingStatusDeletedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var pageNumber = model.PageNumber.PositiveOrZero();
            
            var resultTemp = translationOrderStateRep.All()
                .Include(i => i.TranslationOrder)
                    .ThenInclude(i => i.ServiceTranslationOrders)
                .Include(i => i.TranslationOrder)
                    .ThenInclude(i => i.ServiceChannelTranslationOrders)
                .Include(i => i.TranslationOrder)
                    .ThenInclude(i => i.GeneralDescriptionTranslationOrders)
                .Where(x => x.Last && x.TranslationStateId == stateTypeFailForInvestigationId)    
                .OrderByDescending(x => x.SendAt)
                .Distinct();

            var resultWithPaging = resultTemp
                .ApplyPaging(pageNumber, MAX_RESULTS);
            
            var serviceTranslationOrders = resultWithPaging.SearchResult
                .Where(x => x.TranslationOrder.ServiceTranslationOrders.Any())    
                .ToDictionary(x => x.TranslationOrder.Id, y => y.TranslationOrder.ServiceTranslationOrders.First().ServiceId);    
            
            var versionedServices = commonService.GetNotificationEntity<ServiceVersioned, ServiceLanguageAvailability>(serviceTranslationOrders.Values, unitOfWork, q => q.Include(a => a.ServiceNames));
            var versionedServiceNames = commonService.GetEntityNames(versionedServices);
            var versionedServiceLanguages = commonService.GetLanguageAvailabilites<ServiceVersioned,ServiceLanguageAvailability>(versionedServices);

            var serviceChannelTranslationOrders = resultWithPaging.SearchResult
                .Where(x => x.TranslationOrder.ServiceChannelTranslationOrders.Any())    
                .ToDictionary(x => x.TranslationOrder.Id, y => y.TranslationOrder.ServiceChannelTranslationOrders.First().ServiceChannelId); 
            
            var versionedServiceChannels = commonService.GetNotificationEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannelTranslationOrders.Values, unitOfWork, q => q.Include(a => a.ServiceChannelNames));
            var versionedServiceChannelNames = commonService.GetEntityNames(versionedServiceChannels);               
            var versionedServiceChannelLanguages = commonService.GetLanguageAvailabilites<ServiceChannelVersioned,ServiceChannelLanguageAvailability>(versionedServiceChannels);

            var generalDescriptionTranslationOrders = resultWithPaging.SearchResult
                .Where(x => x.TranslationOrder.GeneralDescriptionTranslationOrders.Any())    
                .ToDictionary(x => x.TranslationOrder.Id, y => y.TranslationOrder.GeneralDescriptionTranslationOrders.First().StatutoryServiceGeneralDescriptionId);    
            
            var versionedGeneralDescriptions = commonService.GetNotificationEntity<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(generalDescriptionTranslationOrders.Values, unitOfWork, q => q.Include(a => a.Names));
            var versionedGeneralDescriptionNames = commonService.GetEntityNames(versionedGeneralDescriptions);
            var versionedGeneralDescriptionLanguages = commonService.GetLanguageAvailabilites<StatutoryServiceGeneralDescriptionVersioned,GeneralDescriptionLanguageAvailability>(versionedGeneralDescriptions);
  
            return new VmAdminTasks()
            {
                Entities = new VmListItemsData<VmAdminTranslationItem>(
                    resultWithPaging.SearchResult
                        .Select(translationItem =>
                        {
                            var serviceTranslation = translationItem.TranslationOrder.ServiceTranslationOrders.FirstOrDefault();
                            var channelChannelTranslation =  translationItem.TranslationOrder.ServiceChannelTranslationOrders.FirstOrDefault();
                            var generalDescriptionTranslation = translationItem.TranslationOrder.GeneralDescriptionTranslationOrders.FirstOrDefault();
                            
                            var entityPublishingStatusId = serviceTranslation != null
                                    ? versionedServices.First(x => x.UnificRootId == serviceTranslation.ServiceId).PublishingStatusId
                                    : channelChannelTranslation != null 
                                        ? versionedServiceChannels.First(x => x.UnificRootId == channelChannelTranslation.ServiceChannelId).PublishingStatusId
                                        : generalDescriptionTranslation != null
                                            ?versionedGeneralDescriptions.First(x => x.UnificRootId == generalDescriptionTranslation.StatutoryServiceGeneralDescriptionId).PublishingStatusId
                                            : Guid.Empty;

                            var languagesAvailabilities = serviceTranslation != null
                                ? versionedServiceLanguages[
                                    serviceTranslationOrders[translationItem.TranslationOrderId]]
                                : channelChannelTranslation != null
                                    ? versionedServiceChannelLanguages[
                                        serviceChannelTranslationOrders[translationItem.TranslationOrderId]]
                                    : versionedGeneralDescriptionLanguages[
                                        generalDescriptionTranslationOrders[translationItem.TranslationOrderId]];

                            foreach (var languageAvailablity in languagesAvailabilities)
                            {
                                if (entityPublishingStatusId == publishingStatusDeletedId)
                                {
                                    languageAvailablity.StatusId = publishingStatusDeletedId;
                                }
                            }
                            
                            return new VmAdminTranslationItem()
                            {
                                Id = translationItem.TranslationOrder.Id,
                                SourceLanguage = translationItem.TranslationOrder.SourceLanguageId, 
                                Targetlanguage = translationItem.TranslationOrder.TargetLanguageId,
                                SenderEmail = translationItem.TranslationOrder.SenderEmail,
                                SentAt = translationItem.SendAt.ToEpochTime(),
                                TranslationStateTypeId = translationItem.TranslationStateId,
                                
                                Name = serviceTranslation != null
                                    ? versionedServiceNames[serviceTranslationOrders[translationItem.TranslationOrderId]]
                                    : channelChannelTranslation != null 
                                        ? versionedServiceChannelNames[serviceChannelTranslationOrders[translationItem.TranslationOrderId]]
                                        : versionedGeneralDescriptionNames[generalDescriptionTranslationOrders[translationItem.TranslationOrderId]],
                                    
                                LanguagesAvailabilities = languagesAvailabilities,
                                
                                EntityUnificRootId = serviceTranslation?.ServiceId 
                                                     ?? channelChannelTranslation?.ServiceChannelId 
                                                     ?? generalDescriptionTranslation?.StatutoryServiceGeneralDescriptionId
                                                     ?? Guid.Empty,
                                
                                EntityVersionedId = serviceTranslation != null
                                    ? versionedServices.First(x => x.UnificRootId == serviceTranslation.ServiceId).Id
                                    : channelChannelTranslation != null 
                                        ? versionedServiceChannels.First(x => x.UnificRootId == channelChannelTranslation.ServiceChannelId).Id
                                        : generalDescriptionTranslation != null
                                            ?versionedGeneralDescriptions.First(x => x.UnificRootId == generalDescriptionTranslation.StatutoryServiceGeneralDescriptionId).Id
                                            : Guid.Empty,
                               
                                EntityType =  serviceTranslation != null
                                                ? EntityTypeEnum.Service
                                                : channelChannelTranslation != null 
                                                    ? EntityTypeEnum.Channel
                                                    : EntityTypeEnum.GeneralDescription,
                                
                                SubEntityType = serviceTranslation != null
                                    ? EntityTypeEnum.Service.ToString()
                                    : channelChannelTranslation != null 
                                        ? commonService.GetChannelSubType(serviceChannelTranslationOrders[translationItem.TranslationOrderId], versionedServiceChannels)
                                        : EntityTypeEnum.GeneralDescription.ToString(),
                                
                                ErrorDescription = translationItem.InfoDetails,
                                
                                NestedItems = GetPreviousFailedTranslationOrders(translationItem.TranslationOrderId, unitOfWork)
                            };
                        })
                    ),
                Count = resultTemp.Count(),
                MoreAvailable = resultWithPaging.MoreAvailable,
                PageNumber = ++pageNumber,
                Id = AdminTasksIdsEnum.FailedTranslationOrders
            };
        }

        private List<VmAdminTranslationDetailItem> GetPreviousFailedTranslationOrders(Guid translationOrderId, IUnitOfWork unitOfWork)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var stateTypeFailForInvestigationId = typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.FailForInvestigation.ToString());
            
            var previousTranslationOrderStates = translationOrderStateRep.All()
                .Include(i => i.TranslationOrder) 
                .Where(x => x.Last == false && x.TranslationOrderId == translationOrderId && x.TranslationStateId == stateTypeFailForInvestigationId )    
                .OrderByDescending(x => x.SendAt)
                .Distinct()
                .ToList();
            
            return previousTranslationOrderStates.Select(translationItem => new VmAdminTranslationDetailItem()
            {
                Id = translationItem.Id,
                ParentId = translationItem.TranslationOrderId,
                SentAt = translationItem.SendAt.ToEpochTime(),
                SenderEmail = translationItem.TranslationOrder.SenderEmail,
                TranslationStateTypeId = translationItem.TranslationStateId
            }).ToList();
        }

        public IVmSearchBase FetchFailedTranslationOrders(VmAdminTasksSearch model)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
                var stateTypeFailForInvestigationId = typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.FailForInvestigation.ToString());
                var stateTypeRequestForRepetationId = typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.RequestForRepetition.ToString());

                var failForInvestaigationOrders = translationOrderStateRep.All()
                    .Where(x => x.Last && x.TranslationStateId == stateTypeFailForInvestigationId)
                    .Select(x => x.TranslationOrderId)
                    .Distinct()
                    .ToList();

                foreach (var translationOrderId in failForInvestaigationOrders)
                {
                    translationService.AddTranslationOrderState(unitOfWork, translationOrderId, stateTypeRequestForRepetationId);
                }

                if (failForInvestaigationOrders.Any())
                {
                    unitOfWork.Save();
                }
            });
            
            return GetTasksEntities(model);
        }

        public IVmSearchBase CancelTranslationOrder(VmAdminTasksSearch model)
        {
            if (model.Id == null) return null;
            
            contextManager.ExecuteWriter(unitOfWork =>
            {
                translationService.AddTranslationOrderState(unitOfWork, model.Id.Value, typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.RequestForCancel.ToString()));
                unitOfWork.Save();
            });
            
            return GetTasksEntities(model);
        }
    }
   
}
