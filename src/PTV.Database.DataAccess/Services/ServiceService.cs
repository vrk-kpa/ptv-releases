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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;

using Microsoft.Extensions.Logging;
using PTV.Domain.Model.Models.OpenApi;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using PTV.Database.DataAccess.Caches;
using PTV.Framework.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.ServiceManager;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Database.DataAccess.Utils.OpenApi;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.OpenApi.V11;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IServiceService), RegisterType.Transient)]
    internal class ServiceService : ServiceBase, IServiceService
    {
        private IContextManager contextManager;

        private ILogger logger;
        private IServiceUtilities utilities;
        private ICommonServiceInternal commonService;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private IGeneralDescriptionService generalDescriptionService;
        private ILanguageOrderCache languageOrderCache;
        private ITargetGroupDataCache targetGroupDataCache;
        private readonly IUrlService urlService;
        private readonly IExpirationService expirationService;

        public ServiceService(
            IContextManager contextManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            ILogger<ServiceService> logger,
            IServiceUtilities utilities,
            ICommonServiceInternal commonService,
            ITypesCache typesCache,
            ILanguageCache languageCache,
            IPublishingStatusCache publishingStatusCache,
            IVersioningManager versioningManager,
            IGeneralDescriptionService generalDescriptionService,
            IUserOrganizationChecker userOrganizationChecker,
            ILanguageOrderCache languageOrderCache,
            ITargetGroupDataCache targetGroupDataCache,
            IUrlService urlService,
            IExpirationService expirationService)
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker,
                versioningManager)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.commonService = commonService;
            this.typesCache = typesCache;
            this.languageCache = languageCache;
            this.generalDescriptionService = generalDescriptionService;
            this.languageOrderCache = languageOrderCache;
            this.targetGroupDataCache = targetGroupDataCache;
            this.urlService = urlService;
            this.expirationService = expirationService;
        }

        private ServiceVersioned DeleteService(IUnitOfWorkWritable unitOfWork, Guid? serviceId)
        {
            return commonService.ChangeEntityToDeleted<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, serviceId.Value, HistoryAction.Delete);
        }

        #region Open Api

        public IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServices(DateTime? date, int pageNumber, int pageSize, EntityStatusExtendedEnum state, DateTime? dateBefore = null)
        {
            var handler = new EntitiesWithConnectionsPagingHandler<ServiceVersioned, Service, ServiceName, ServiceLanguageAvailability>(state, date, dateBefore, PublishingStatusCache, typesCache, pageNumber, pageSize);
            // Filter out service names that are not published if we are getting only publihed items (PTV-3689).
            return GetPage(contextManager, handler);
        }

        public IVmOpenApiModelWithPagingBase<VmOpenApiExpiringTask> GetTaskServices(int pageNumber, int pageSize, List<Guid> entityIds, DateTime lifeTime, List<Guid> publishingStatusIds)
        {
            var handler = new ExpiringTasksPagingHandler<ServiceVersioned, ServiceName, ServiceLanguageAvailability>(entityIds, publishingStatusIds, TranslationManagerToVm, lifeTime, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        public IVmOpenApiModelWithPagingBase<VmOpenApiTask> GetTaskServices(int pageNumber, int pageSize, List<Guid> entityIds, List<Guid> publishingStatusIds)
        {
            var handler = new TasksPagingHandler<ServiceVersioned, ServiceName, ServiceLanguageAvailability>(entityIds, publishingStatusIds, TranslationManagerToVm, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        private IVmOpenApiModelWithPagingBase<IVmOpenApiServiceVersionBase> GetPageWithAllData(IEntitiesWithAllDataPagingHandler<IVmOpenApiServiceVersionBase> pageHandler, bool fillWithAllGdData, int openApiVersion)
        {
            if (pageHandler.PageNumber <= 0) return pageHandler.GetModel();

            IVmOpenApiModelWithPagingBase<IVmOpenApiServiceVersionBase> model = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var totalCount = pageHandler.Search(unitOfWork);
                model = pageHandler.GetModel();
                if (totalCount > 0 && pageHandler.EntityVersionIds?.Count > 0)
                {
                    model.ItemList = GetServicesWithDetails(unitOfWork, pageHandler.EntityVersionIds, openApiVersion, fillWithAllGdData);
                }
            });

            return model;
        }

        public IList<IVmOpenApiServiceVersionBase> GetServices(List<Guid> idList, int openApiVersion, bool fillWithAllGdData)
        {
            if (idList == null || idList.Count == 0)
            {
                return null;
            }

            IList<IVmOpenApiServiceVersionBase> result = new List<IVmOpenApiServiceVersionBase>();
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    IList<Expression<Func<ServiceVersioned, bool>>> filters = new List<Expression<Func<ServiceVersioned, bool>>>
                    {
                        c => idList.Contains(c.UnificRootId)
                    };

                    result = GetServicesWithDetails(unitOfWork, filters, openApiVersion, fillWithAllGdData);
                });
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting services. {0}", ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }

            return result;
        }

        public IVmOpenApiModelWithPagingBase<IVmOpenApiServiceVersionBase> GetServicesByOrganization(IList<Guid> organizationIds, int openApiVersion, int pageNumber, bool fillWithAllGdData)
        {
            var handler = new ServicesWithAllDataByOrganizationPagingHandler(organizationIds, PublishingStatusCache, pageNumber);
            return GetPageWithAllData(handler, fillWithAllGdData, openApiVersion);
        }
        public IVmOpenApiModelWithPagingBase<IVmOpenApiServiceVersionBase> GetServicesWithAllDataByMunicipality(Guid municipalityId, bool includeWholeCountry, int openApiVersion, int pageNumber, bool fillWithAllGdData)
        {
            var handler = new ServicesWithAllDataByMunicipalityPagingHandler(municipalityId, includeWholeCountry, typesCache, PublishingStatusCache, pageNumber);
            return GetPageWithAllData(handler, fillWithAllGdData, openApiVersion);
        }

        public IVmOpenApiModelWithPagingBase<IVmOpenApiServiceVersionBase> GetServicesWithAllDataByArea(Guid areaId, bool includeWholeCountry, int openApiVersion, int pageNumber, bool fillWithAllGdData)
        {
            var handler = new ServicesWithAllDataByAreaPagingHandler(areaId, includeWholeCountry, typesCache, PublishingStatusCache, pageNumber);
            return GetPageWithAllData(handler, fillWithAllGdData, openApiVersion);
        }

        public IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServicesByServiceChannel(Guid channelId, DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null)
        {
            var handler = new ServicesByServiceChannelPagingHandler(channelId, date, dateBefore, PublishingStatusCache, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        /// <summary>
        /// Gets all published services that are related to given service class. Takes also into account services where attached general desription is related to given service class.
        /// </summary>
        /// <param name="serviceClassId"></param>
        /// <param name="date"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServicesByServiceClass(List<Guid> serviceClassIds, DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null)
        {
            var handler = new ServicesByServiceClassPagingHandler(serviceClassIds, date, dateBefore, PublishingStatusCache, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        public IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServicesByMunicipality(Guid municipalityId, bool includeWholeCountry, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null)
        {
            var handler = new ServicesByMunicipalityPagingHandler(municipalityId, includeWholeCountry, date, dateBefore, PublishingStatusCache, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        public IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServicesByArea(Guid areaId, bool includeWholeCountry, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null)
        {
            var handler = new ServicesByAreaPagingHandler(areaId, includeWholeCountry, typesCache, date, dateBefore, PublishingStatusCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        /// <summary>
        /// Gets all published services that are related to given taget group. Takes also into account services where attached general desription is related to given target group.
        /// </summary>
        /// <param name="targetGroupId"></param>
        /// <param name="date"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServicesByTargetGroup(Guid targetGroupId, DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null)
        {
            var handler = new ServicesByTargetGroupPagingHandler(targetGroupId, date, dateBefore, PublishingStatusCache, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        /// <summary>
        /// Gets all published services that are related to given industrail. Takes also into account services where attached general desription is related to given industrial class.
        /// </summary>
        /// <param name="industrialClassId"></param>
        /// <param name="date"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServicesByIndustrialClass(Guid industrialClassId, DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null)
        {
            var handler = new ServicesByIndustrialClassPagingHandler(industrialClassId, date, dateBefore, PublishingStatusCache, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        /// <summary>
        /// Gets all published services that are related to given service type. Takes also into account services where attached general desription is related to given service type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="dateBefore"></param>
        /// <returns></returns>
        public IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServicesByType(string type, DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null)
        {
            var typeId = typesCache.Get<ServiceType>(type);

            if (!typeId.IsAssigned())
            {
                throw new Exception("Type is not valid!");
            }

            var handler = new ServicesByTypePagingHandler(typeId, date, dateBefore, PublishingStatusCache, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        public IVmOpenApiServiceVersionBase GetServiceById(Guid id, int openApiVersion, VersionStatusEnum status, bool fillWithAllGdData = false)
        {
            IVmOpenApiServiceVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork => { result = GetServiceById(unitOfWork, id, openApiVersion, status, fillWithAllGdData); });

            return result;
        }

        /// <summary>
        /// Returns the latest version of a service with minimum data included.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IVmOpenApiServiceVersionBase GetServiceByIdSimple(Guid id, bool getOnlyPublished = true)
        {
            IVmOpenApiServiceVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                try
                {
                    Guid? entityId = null;
                    if (getOnlyPublished)
                    {
                        // Get published version
                        entityId = VersioningManager.GetVersionId<ServiceVersioned>(unitOfWork, id, PublishingStatus.Published);
                    }
                    else
                    {
                        // Get latest version regardless of the publishing status
                        entityId = VersioningManager.GetVersionId<ServiceVersioned>(unitOfWork, id, null, false);
                    }

                    if (entityId.IsAssigned())
                    {
                        result = GetServiceWithSimpleDetails(unitOfWork, entityId.Value);
                    }
                }
                catch (Exception ex)
                {
                    var errorMsg = string.Format("Error occured while getting a service with id {0}. {1}", id, ex.Message);
                    logger.LogError(errorMsg + " " + ex.StackTrace);
                    throw new Exception(errorMsg);
                }
            });

            return result;
        }

        public PublishingStatus? GetLatestVersionPublishingStatus(Guid id)
        {
            PublishingStatus? currentPublishingStatus = null;
            contextManager.ExecuteReader(unitOfWork => { currentPublishingStatus = VersioningManager.GetLatestVersionPublishingStatus<ServiceVersioned>(unitOfWork, id); });

            return currentPublishingStatus;
        }

        private IVmOpenApiServiceVersionBase GetServiceById(IUnitOfWork unitOfWork, Guid id, int openApiVersion, VersionStatusEnum status, bool fillWithAllGdData)
        {
            try
            {
                // Get the right version id
                Guid? entityId = null;
                switch (status)
                {
                    case VersionStatusEnum.Published:
                        entityId = VersioningManager.GetVersionId<ServiceVersioned>(unitOfWork, id, PublishingStatus.Published);
                        break;
                    case VersionStatusEnum.Latest:
                        entityId = VersioningManager.GetVersionId<ServiceVersioned>(unitOfWork, id, null, false);
                        break;
                    case VersionStatusEnum.LatestActive:
                        entityId = VersioningManager.GetVersionId<ServiceVersioned>(unitOfWork, id, null, true);
                        break;
                    default:
                        break;
                }

                if (entityId.IsAssigned())
                {
                    return GetServiceWithDetails(unitOfWork, entityId.Value, openApiVersion, status == VersionStatusEnum.Published ? true : false, fillWithAllGdData);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting a service with id {0}. {1}", id, ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }

            return null;
        }

        public IVmOpenApiServiceVersionBase GetServiceBySource(string sourceId)
        {
            var userId = utilities.GetRelationIdForExternalSource();
            Guid? rootId = null;
            try
            {
                contextManager.ExecuteReader(unitOfWork => { rootId = GetPTVId<Service>(sourceId, userId, unitOfWork); });
            }
            catch (Exception ex)
            {
                logger.LogError($"Error occured while getting services by source id {sourceId}. {ex.Message}");
                throw ex;
            }

            return rootId.HasValue ? GetServiceByIdSimple(rootId.Value, false) : null;
        }

        // SFIPTV-1919
        private void ValidateExpirationTime(IUnitOfWorkWritable unitOfWork, ServiceVersioned service, DateTime? lastChangeDate)
        {
            var expirationTime = expirationService.GetExpirationDate(unitOfWork, service, lastChangeDate: lastChangeDate);
            if (service.LanguageAvailabilities.Any(la => la.PublishAt > expirationTime))
                throw new PtvAppException("Publishing date cannot be scheduled after automatic archiving date.", "Channel.ScheduleException.LateDate");
        }

        public IVmOpenApiServiceBase AddService(IVmOpenApiServiceInVersionBase vm, int openApiVersion, bool attachProposedChannels)
        {
            var service = new ServiceVersioned();
            var saveMode = SaveMode.Normal;
            var userId = utilities.GetRelationIdForExternalSource();
            var useOtherEndPoint = false;
            var isUserEeva = utilities.UserHighestRole() == UserRoleEnum.Eeva;

            ProcessNewUrls(vm);

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Check if the external source already exists. Let's not throw the excpetion here to avoid contextManager to catch the exception.
                useOtherEndPoint = ExternalSourceExists<Service>(vm.SourceId, userId, unitOfWork);
                if (!useOtherEndPoint)
                {
                    CheckVm(vm, unitOfWork, attachProposedChannels, true); // Includes checking general description data!

                    // Set user name which is used within language availabilities and check the publishing status (SFIPTV-190)
                    vm.UserName = unitOfWork.GetUserNameForAuditing();
                    if (vm.ValidFrom.HasValue && vm.ValidFrom > DateTime.UtcNow)
                    {
                        // For timed publishing the version created needs to be set as draft
                        vm.PublishingStatus = PublishingStatus.Draft.ToString();
                    }

                    service = TranslationManagerToEntity.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(vm, unitOfWork);

                    ValidateExpirationTime(unitOfWork, service, DateTime.UtcNow);

                    var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    serviceRep.Add(service);

                    // Create the mapping between external source id and PTV id
                    if (!string.IsNullOrEmpty(vm.SourceId))
                    {
                        SetExternalSource(service.UnificRoot, vm.SourceId, userId, unitOfWork);
                    }

                    expirationService.SetExpirationDate(unitOfWork, service,
                        vm.PublishingStatus == PublishingStatus.Published.ToString()
                            ? ActionTypeEnum.SaveAndPublish
                            : ActionTypeEnum.Save,
                        isUserEeva);
                    commonService.CreateHistoryMetaData<ServiceVersioned, ServiceLanguageAvailability>(service, setByEntity: true);
                    unitOfWork.Save(saveMode);
                }
            });

            if (useOtherEndPoint)
            {
                throw new ExternalSourceExistsException(string.Format(CoreMessages.OpenApi.ExternalSourceExists, vm.SourceId));
            }

            // Add connections for defined service channels (PTV-2317)
            if (vm.ServiceServiceChannels?.Count > 0)
            {
                foreach (var connection in vm.ServiceServiceChannels)
                {
                    contextManager.ExecuteWriter(unitOfWork =>
                    {
                        connection.ServiceGuid = service.UnificRootId;
                        TranslationManagerToEntity
                            .Translate<V11VmOpenApiServiceServiceChannelAstiInBase, ServiceServiceChannel>(connection,
                                unitOfWork);
                        unitOfWork.Save(saveMode);
                    });
                }
            }

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = commonService.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(service.Id, i => i.ServiceVersionedId == service.Id);
                expirationService.SetExpirationDateForPublishing<ServiceVersioned>(contextManager, service.Id, isUserEeva);
            }

            return GetServiceWithDetails(service.Id, openApiVersion, false);
        }

        public IVmOpenApiServiceVersionBase SaveService(IVmOpenApiServiceInVersionBase vm, int openApiVersion, bool attachProposedChannels, string sourceId = null)
        {
            if (vm == null) return null;

            var saveMode = SaveMode.Normal;
            ServiceVersioned service = null;
            var isUserEeva = utilities.UserHighestRole() == UserRoleEnum.Eeva;

            ProcessNewUrls(vm);

            contextManager.ExecuteWriter(unitOfWork =>
            {
                if (vm.VersionId.IsAssigned())
                {
                    CheckVm(vm, unitOfWork, attachProposedChannels, rootId: vm.Id);

                    // Set user name which is used within language availabilities and check the publishing status (SFIPTV-190)
                    vm.UserName = unitOfWork.GetUserNameForAuditing();
                    var currentTime = DateTime.UtcNow;
                    if ((vm.ValidFrom.HasValue && vm.ValidFrom.Value > currentTime) || (vm.ValidTo.HasValue && vm.ValidTo.Value > currentTime))
                    {
                        if (vm.ValidFrom.HasValue)
                        {
                            // For timed publishing the version created needs to be set as modified
                            vm.PublishingStatus = PublishingStatus.Modified.ToString();
                        }

                        // We need to get the available languages to be able update the publishing and archiving dates for different language versions
                        var allAvailableLanguages = unitOfWork.CreateRepository<IRepository<ServiceLanguageAvailability>>().All()
                            .Where(x => x.ServiceVersionedId == vm.VersionId).Select(i => i.LanguageId).Select(x => languageCache.GetByValue(x)).ToHashSet();
                        vm.AvailableLanguages.ForEach(l => allAvailableLanguages.Add(l));
                        vm.AvailableLanguages = allAvailableLanguages.ToList();
                    }

                    if (vm.PublishingStatus == PublishingStatus.Deleted.ToString())
                    {
                        service = DeleteService(unitOfWork, vm.VersionId);
                    }
                    else
                    {
                        // Entity needs to be restored?
                        if (vm.CurrentPublishingStatus == PublishingStatus.Deleted.ToString())
                        {
                            if (vm.PublishingStatus == PublishingStatus.Modified.ToString() || vm.PublishingStatus == PublishingStatus.Published.ToString())
                            {
                                // We need to restore already archived item
                                var publishingResult = commonService.RestoreArchivedEntity<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, vm.VersionId.Value);
                            }
                        }

                        service = TranslationManagerToEntity.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(vm, unitOfWork);

                        ValidateExpirationTime(unitOfWork, service, DateTime.UtcNow);

                        // Add connections for defined service channels (PTV-2315)
                        if (vm.ServiceServiceChannels?.Count > 0)
                        {
                            vm.ServiceServiceChannels.ForEach(s => s.ServiceGuid = service.UnificRootId);
                            var relations = new V11VmOpenApiServiceAndChannelRelationAstiInBase {ChannelRelations = vm.ServiceServiceChannels.ToList(), ServiceId = service.UnificRootId};
                            service.UnificRoot = TranslationManagerToEntity.Translate<V11VmOpenApiServiceAndChannelRelationAstiInBase, Service>(relations, unitOfWork);
                        }

                        // Update the mapping between external source id and PTV id
                        if (!string.IsNullOrEmpty(vm.SourceId))
                        {
                            UpdateExternalSource<Service>(service.UnificRootId, vm.SourceId, utilities.GetRelationIdForExternalSource(), unitOfWork);
                        }
                    }

                    expirationService.SetExpirationDate(unitOfWork, service,
                        vm.CurrentPublishingStatus == PublishingStatus.Published.ToString()
                            ? ActionTypeEnum.SaveAndPublish
                            : ActionTypeEnum.Save,
                        isUserEeva);
                    commonService.CreateHistoryMetaData<ServiceVersioned, ServiceLanguageAvailability>(service, setByEntity: true);
                    unitOfWork.Save(saveMode, PreSaveAction.Standard, service);
                }

            });

            if (service == null) return null;

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = commonService.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(service.Id, i => i.ServiceVersionedId == service.Id);
                expirationService.SetExpirationDateForPublishing<ServiceVersioned>(contextManager, service.Id, isUserEeva);
            }

            return GetServiceWithDetails(service.Id, openApiVersion, false);
        }

        private void ProcessNewUrls(IVmOpenApiServiceInVersionBase vm)
        {
            var newUrls = vm.Legislation?.SelectMany(x => x.WebPages).Select(x => x.Url).ToList() ?? new List<string>();
            newUrls.AddRange(vm.ServiceVouchers?.Select(x => x.Url) ?? new List<string>());
            contextManager.ExecuteWriter(unitOfWork =>
            {
                urlService.AddNewUrls(unitOfWork, newUrls);
                unitOfWork.Save();
            });
        }

        /// <summary>
        /// Returns a list of services that do not exist (within idList).
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public List<Guid> CheckServices(List<Guid> idList)
        {
            List<Guid> existingServices = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
                var query = unitOfWork.CreateRepository<IServiceVersionedRepository>().All().Where(s => idList.Contains(s.UnificRootId) && s.PublishingStatusId == publishedId);

                existingServices = query.Select(s => s.UnificRootId).ToList();
            });

            if (existingServices?.Count > 0) return idList.Where(i => !existingServices.Contains(i)).ToList();

            return idList;
        }


        private IVmOpenApiServiceVersionBase GetServiceWithDetails(IUnitOfWork unitOfWork, Guid versionId, int openApiVersion, bool getOnlyPublished = true, bool fillWithAllGdData = false)
        {
            //return GetServicesWithDetails(unitOfWork, new List<Guid> { versionId }, openApiVersion, getOnlyPublished).FirstOrDefault();
            //// Measure
            //var watch = new Stopwatch(); var totalWatch = new Stopwatch();
            //logger.LogTrace("****************************************");
            //logger.LogTrace($"GetServiceWithDetails starts. Id: {versionId}");
            //watch.Start(); totalWatch.Start();
            //// end measure
            IVmOpenApiServiceVersionBase result = null;
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var query = serviceRep.All().Where(s => s.Id == versionId);
            if (getOnlyPublished)
            {
                query = query.Where(c => c.LanguageAvailabilities.Any(l => l.StatusId == publishedId));
            }

            var service = unitOfWork.ApplyIncludes(query, GetServiceIncludeChain()).FirstOrDefault();

            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Fetch from db: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// end measure

            if (service != null)
            {
                var serviceList = new List<ServiceVersioned> { service };
                IncludeClassification(unitOfWork, serviceList);
                IncludeLegislation(unitOfWork, serviceList);
                IncludeAreas(unitOfWork, serviceList);

                // Find only published organizations for services and map the data
                GetAndMapOrganizationsForServices(serviceList, unitOfWork);

                ////Measure
                //watch.Stop();
                //logger.LogTrace($"*** Get organizations from db: {watch.ElapsedMilliseconds} ms.");
                //watch.Restart();
                //// End measure

                // Filter out not published language versions
                FilterOutNotPublishedLanguageVersions(service, publishedId, getOnlyPublished);

                ////Measure
                //watch.Stop();
                //logger.LogTrace($"*** Filtering: {watch.ElapsedMilliseconds} ms.");
                //watch.Restart();
                //// End measure

                // HACK: don't use translator for service channels
                // this is because the translator kills the performance when there are many items in the collection(s)
                // For example it takes roughly 0.002ms to translate an item and there are 1000 items in the collection. It takes 2 seconds to translate the collection

                result = TranslationManagerToVm.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(service);

                ////Measure
                //watch.Stop();
                //logger.LogTrace($"*** Translation: {watch.ElapsedMilliseconds} ms.");
                //watch.Restart();
                //// End measure
            }

            if (result == null)
            {
                return null;
            }

            // Find only published service collections for a service (which have at least one published language version)
            result.ServiceCollections = GetServiceCollections(new List<Guid> {service.UnificRootId}, unitOfWork);

            // Find only published service channels for a service
            result.ServiceChannels = GetServiceChannels(new List<Guid> {service.UnificRootId}, unitOfWork);

            ////Measure
            //watch.Stop(); totalWatch.Stop();
            //logger.LogTrace($"*** Get connections from db: {watch.ElapsedMilliseconds} ms.");
            //logger.LogTrace($"*** Total: {totalWatch.ElapsedMilliseconds} ms.");
            //logger.LogTrace($"***************************************************.");
            //// End measure

            // Fill with the general description related data
            FillWithGeneralDescriptionData(result, unitOfWork, fillWithAllGdData);

            return GetServiceByOpenApiVersion(unitOfWork, result, openApiVersion);
        }

        private void GetAndMapOrganizationsForServices(IList<ServiceVersioned> services, IUnitOfWork unitOfWork)
        {
            if (services == null || services.Count == 0) return;
            // Find only published organizations for services
            var allOrganizationIds = services.SelectMany(service => service.OrganizationServices).Select(o => o.OrganizationId).Distinct().ToList();
            var serviceProducers = services.SelectMany(service => service.ServiceProducers.SelectMany(s => s.Organizations)).Select(o => o.OrganizationId).Distinct().ToList();
            allOrganizationIds.AddRange(serviceProducers.Except(allOrganizationIds));

            // Let's also include main responsible organization
            var mainOrganizations = services.Select(service => service.OrganizationId).Distinct().ToList();
            allOrganizationIds.AddRange(mainOrganizations.Except(allOrganizationIds));

            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            // Get published organizations and the names
            var organizationVersionedRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var publishedOrganizations = unitOfWork.ApplyIncludes(
                organizationVersionedRep.All().Where(i => allOrganizationIds.Contains(i.UnificRootId) &&
                                                          i.PublishingStatusId == publishedId &&
                                                          i.LanguageAvailabilities.Any(l => l.StatusId == publishedId)), // Filter out organizations with no language versions published
                q => q.Include(i => i.OrganizationNames).Include(i => i.LanguageAvailabilities)).ToList();
            // Loop through organizations and filter out names that are not published (PTV-3689).
            publishedOrganizations.ForEach(org =>
            {
                var publishedLanguages = org.LanguageAvailabilities.Where(i => i.StatusId == publishedId).Select(i => i.LanguageId).ToList();
                org.OrganizationNames = org.OrganizationNames.Where(n => publishedLanguages.Contains(n.LocalizationId)).ToList();
            });
            var publishedOrganizationRootIds = publishedOrganizations.Select(o => o.UnificRootId).ToList();

            // Map organization data for all services. Also filter out organizations that are not published.
            services.ForEach(service =>
            {
                // Filter out not published organizations
                service.OrganizationServices = service.OrganizationServices.Where(i => publishedOrganizationRootIds.Contains(i.OrganizationId)).ToList();
                // Map service organizations
                if (publishedOrganizations?.Count > 0)
                {
                    service.OrganizationServices.ForEach(os =>
                    {
                        var organizationVersioned = publishedOrganizations.FirstOrDefault(po => po.UnificRootId == os.OrganizationId);
                        if (organizationVersioned != null)
                        {
                            os.Organization = new Organization {Id = os.OrganizationId};
                            os.Organization.Versions.Add(organizationVersioned);
                        }

                    });
                }

                // Map service producers
                service.ServiceProducers.Where(sp => sp.Organizations != null).ForEach(producer =>
                {
                    // Filter out not published organizations
                    producer.Organizations = producer.Organizations.Where(o => publishedOrganizationRootIds.Contains(o.OrganizationId)).ToList();
                    // Map organization
                    if (publishedOrganizations.Count > 0)
                    {
                        producer.Organizations.ForEach(org =>
                        {
                            var organizationVersioned = publishedOrganizations.FirstOrDefault(po => po.UnificRootId == org.OrganizationId);
                            if (organizationVersioned != null)
                            {
                                org.Organization = new Organization {Id = org.OrganizationId};
                                org.Organization.Versions.Add(organizationVersioned);
                            }
                        });
                    }
                });
                // Map main organization
                if (publishedOrganizations.Count > 0)
                {

                    var mainOrganization = publishedOrganizations.FirstOrDefault(o => o.UnificRootId == service.OrganizationId);
                    if (mainOrganization != null)
                    {
                        service.Organization.Versions.Add(mainOrganization);
                    }
                    else
                    {
                        service.Organization = null;
                    }
                }
                else
                {
                    service.Organization = null;
                }
            });
        }

        private IVmOpenApiServiceVersionBase GetServiceWithDetails(Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiServiceVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork => { result = GetServiceWithDetails(unitOfWork, versionId, openApiVersion, getOnlyPublished); });

            return result;
        }

//        private IList<IVmOpenApiServiceVersionBase> GetServicesWithDetailsOld(IUnitOfWork unitOfWork, List<Guid> versionIdList, int openApiVersion, bool getOnlyPublished = true)
//        {
//            if (versionIdList.Count == 0) return new List<IVmOpenApiServiceVersionBase>();
//
//            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
//            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
//
//            var resultTemp = unitOfWork.ApplyIncludes(serviceRep.All().Where(s => versionIdList.Contains(s.Id)), q =>
//                q.Include(i => i.ServiceLanguages).ThenInclude(i => i.Language)
//                .Include(i => i.ServiceNames)
//                .Include(i => i.ServiceDescriptions)
//                .Include(i => i.ServiceServiceClasses).ThenInclude(i => i.ServiceClass).ThenInclude(i => i.Names)
//                .Include(i => i.ServiceServiceClasses).ThenInclude(i => i.ServiceClass).ThenInclude(i => i.Descriptions)
//                .Include(i => i.ServiceOntologyTerms).ThenInclude(i => i.OntologyTerm).ThenInclude(i => i.Names)
//                .Include(i => i.ServiceTargetGroups).ThenInclude(i => i.TargetGroup).ThenInclude(i => i.Names)
//                .Include(i => i.ServiceLifeEvents).ThenInclude(i => i.LifeEvent).ThenInclude(i => i.Names)
//                .Include(i => i.ServiceIndustrialClasses).ThenInclude(i => i.IndustrialClass).ThenInclude(i => i.Names)
//                .Include(i => i.ServiceKeywords).ThenInclude(i => i.Keyword)
//                .Include(i => i.ServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.Names)
//                .Include(i => i.ServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.WebPages).ThenInclude(i => i.WebPage)
//                .Include(i => i.ServiceRequirements)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Versions)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDescriptions)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDigitalAuthorizations)
//                    .ThenInclude(i => i.DigitalAuthorization).ThenInclude(i => i.Names)
//                .Include(i => i.OrganizationServices).ThenInclude(i => i.Organization).ThenInclude(i => i.Versions)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelExtraTypes).ThenInclude(i => i.ExtraSubType)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelExtraTypes).ThenInclude(i => i.ServiceServiceChannelExtraTypeDescriptions)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelServiceHours).ThenInclude(i => i.ServiceHours)
//                    .ThenInclude(i => i.DailyOpeningTimes)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelServiceHours).ThenInclude(i => i.ServiceHours)
//                    .ThenInclude(i => i.AdditionalInformations)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelEmails).ThenInclude(i => i.Email)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelPhones).ThenInclude(i => i.Phone).ThenInclude(i => i.PrefixNumber)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelWebPages).ThenInclude(i => i.WebPage)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
//                    .ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
//                    .ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
//                    .ThenInclude(i => i.AddressStreets).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
//                    .ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
//                    .ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
//                    .ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
//                    .ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
//                    .ThenInclude(i => i.AddressAdditionalInformations)
//                .Include(i => i.ServiceProducers).ThenInclude(i => i.AdditionalInformations)
//                .Include(i => i.ServiceProducers).ThenInclude(i => i.Organizations).ThenInclude(i => i.Organization).ThenInclude(i => i.Versions)
//                .Include(i => i.ServiceWebPages).ThenInclude(i => i.WebPage)
//                .Include(i => i.LanguageAvailabilities)
//                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaNames)
//                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
//                .Include(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceCollectionServices).ThenInclude(i => i.ServiceCollection).ThenInclude(i => i.Versions).ThenInclude(i => i.LanguageAvailabilities)
//                .Include(i => i.Organization).ThenInclude(i => i.Versions)
//                .OrderByDescending(i => i.Modified));
//
//            // Filter out items that do not have language versions published!
//            var services = getOnlyPublished ? resultTemp.Where(c => c.LanguageAvailabilities.Any(l => l.StatusId == publishedId)).ToList() : resultTemp.ToList();
//
//            // Find only published service channels for services
//            var allChannels = services.SelectMany(i => i.UnificRoot.ServiceServiceChannels).Select(i => i.ServiceChannel)
//                .SelectMany(i => i.Versions).Where(i => i.PublishingStatusId == publishedId).ToList();
//            var channelsIds = allChannels.Select(i => i.Id).ToList();
//            var publishedServiceChannelRootIds = new List<Guid>();
//            if (channelsIds.Count > 0)
//            {
//                var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
//                publishedServiceChannelRootIds = serviceChannelRep.All().Where(c => channelsIds.Contains(c.Id))
//                    .Where(s => s.LanguageAvailabilities.Any(l => l.StatusId == publishedId)) // Filter out channels with no language versions published
//                    .Select(c => c.UnificRootId).ToList();
//            }
//
//            // Find only published service collections for services (which have at least one published language version)
//            var publishedServiceCollections = services.SelectMany(i => i.UnificRoot.ServiceCollectionServices).SelectMany(i => i.ServiceCollection.Versions)
//                .Where(i => i.PublishingStatusId == publishedId && i.LanguageAvailabilities.Any(l => l.StatusId == publishedId)).ToList();
//            var publishedServiceCollectionIds = publishedServiceCollections.Select(i => i.Id).ToList();
//
//            // Find only published organizations for services
//            var allOrganizations = services.SelectMany(i => i.OrganizationServices).Where(i => i.Organization != null).Select(i => i.Organization)
//                .SelectMany(i => i.Versions).Where(i => i.PublishingStatusId == publishedId).ToList();
//            var serviceProducerOrganizations = services.SelectMany(i => i.ServiceProducers).SelectMany(i => i.Organizations).Where(i => i.Organization != null).Select(i => i.Organization)
//                .SelectMany(i => i.Versions).Where(i => i.PublishingStatusId == publishedId && !allOrganizations.Contains(i)).ToList();
//            allOrganizations.AddRange(serviceProducerOrganizations);
//            // Let's also include main responsible organization
//            allOrganizations.AddRange(services.Select(i => i.Organization).SelectMany(i => i.Versions).Where(i => i.PublishingStatusId == publishedId).ToList());
//            var organizationIds = allOrganizations.Select(i => i.Id).ToList();
//            var publishedOrganizationRootIds = new List<Guid>();
//            if (organizationIds.Count > 0)
//            {
//                var organizationVersionedRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
//                publishedOrganizationRootIds = organizationVersionedRep.All().Where(i => organizationIds.Contains(i.Id))
//                    .Where(s => s.LanguageAvailabilities.Any(l => l.StatusId == publishedId)) // Filter out organizations with no language versions published
//                    .Select(i => i.UnificRootId).ToList();
//            }
//
//            services.ForEach(service =>
//            {
//                // Filter out not published serviceChannels
//                service.UnificRoot.ServiceServiceChannels = service.UnificRoot.ServiceServiceChannels.Where(c => publishedServiceChannelRootIds.Contains(c.ServiceChannelId)).ToList();
//
//                // Filter out not published service collections
//                service.UnificRoot.ServiceCollectionServices = service.UnificRoot.ServiceCollectionServices
//                    .Where(c => c.ServiceCollection.Versions.Any(x => x.PublishingStatusId == publishedId))
//                    .GroupBy(c => c.ServiceCollectionId)
//                    .Select(c => c.FirstOrDefault())
//                    .ToList();
//
//                // Filter out not published organizations
//                service.OrganizationServices = service.OrganizationServices.Where(i => publishedOrganizationRootIds.Contains(i.OrganizationId)).ToList();
//                foreach (var producer in service.ServiceProducers.Where(sp => sp.Organizations != null))
//                {
//                    producer.Organizations = producer.Organizations.Where(o => publishedOrganizationRootIds.Contains(o.OrganizationId)).ToList();
//                }
//
//                // Filter out not published language versions
//                FilterOutNotPublishedLanguageVersions(service, publishedId, getOnlyPublished);
//            });
//
//            // Fill with service channel names
//            var channelNames = unitOfWork.CreateRepository<IServiceChannelNameRepository>().All().Where(i => channelsIds.Contains(i.ServiceChannelVersionedId)).ToList()
//                .GroupBy(i => i.ServiceChannelVersionedId).ToDictionary(i => i.Key, i => i.ToList());
//            allChannels.ForEach(c =>
//            {
//                c.ServiceChannelNames = channelNames.TryGet(c.Id);
//            });
//
//            // Fill with organization names
//            var organizationNames = unitOfWork.CreateRepository<IOrganizationNameRepository>().All().Where(i => organizationIds.Contains(i.OrganizationVersionedId)).ToList()
//                .GroupBy(i => i.OrganizationVersionedId).ToDictionary(i => i.Key, i => i.ToList());
//            allOrganizations.ForEach(o =>
//            {
//                o.OrganizationNames = organizationNames.TryGet(o.Id);
//            });
//
//            // Fill with service collection names
//            var serviceCollectionNames = unitOfWork.CreateRepository<IServiceCollectionNameRepository>().All()
//                .Where(i => publishedServiceCollectionIds.Contains(i.ServiceCollectionVersionedId)).ToList()
//                .GroupBy(i => i.ServiceCollectionVersionedId).ToDictionary(i => i.Key, i => i.ToList());
//            publishedServiceCollections.ForEach(o =>
//            {
//                o.ServiceCollectionNames = serviceCollectionNames.TryGet(o.Id);
//            });
//
//            var result = TranslationManagerToVm.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(services);
//            if (result == null)
//            {
//                throw new Exception(CoreMessages.OpenApi.RecordNotFound);
//            }
//
//            // Get the right open api view model version
//            var versionList = new List<IVmOpenApiServiceVersionBase>();
//            result.ForEach(service =>
//            {
//                versionList.Add(GetServiceByOpenApiVersion(unitOfWork, service, openApiVersion));
//            });
//
//            return versionList;
//        }

        private IList<IVmOpenApiServiceVersionBase> GetServicesWithDetails(IUnitOfWork unitOfWork, IList<Expression<Func<ServiceVersioned, bool>>> filters, int openApiVersion, bool fillWithAllGdData)
        {
            //// Measure
            //var watch = new Stopwatch(); var totalWatch = new Stopwatch();
            //logger.LogTrace("****************************************");
            //logger.LogTrace($"GetServicesWithDetails - a list");
            //watch.Start(); totalWatch.Start();
            //// end measure

            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            // Get only published items -  filter out items that do not have any language versions published.
            filters.Add(e => e.PublishingStatusId == publishedId && e.LanguageAvailabilities.Any(l => l.StatusId == publishedId));

            var query = serviceRep.All();
            filters.ForEach(a => query = query.Where(a));

            var totalCount = query.Count();
            if (totalCount > 100)
            {
                throw new Exception(CoreMessages.OpenApi.TooManyItems);
            }

            if (totalCount == 0)
            {
                return null;
            }

            var services = unitOfWork.ApplyIncludes(query, GetServiceIncludeChain()).ToList();
            IncludeClassification(unitOfWork, services);
            IncludeLegislation(unitOfWork, services);
            IncludeAreas(unitOfWork, services);

            return GetServicesWithDetails(unitOfWork, services, openApiVersion, fillWithAllGdData);
        }

        private IList<IVmOpenApiServiceVersionBase> GetServicesWithDetails(IUnitOfWork unitOfWork, IList<Guid> versionIds, int openApiVersion, bool fillWithAllGdData)
        {
            if (versionIds == null || versionIds.Count == 0)
            {
                return null;
            }

            var query = unitOfWork.CreateRepository<IRepository<ServiceVersioned>>().All()
                .Where(s => versionIds.Contains(s.Id));

            var services = unitOfWork.ApplyIncludes(query, GetServiceIncludeChain()).ToList();
            IncludeClassification(unitOfWork, services);
            IncludeLegislation(unitOfWork, services);
            IncludeAreas(unitOfWork, services);

            return GetServicesWithDetails(unitOfWork, services, openApiVersion, fillWithAllGdData);
        }

        private IList<IVmOpenApiServiceVersionBase> GetServicesWithDetails(IUnitOfWork unitOfWork, IList<ServiceVersioned> services, int openApiVersion, bool fillWithAllGdData, bool getOnlyPublished = true)
        {
            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Fetch from db: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// end measure

            // Find only published organizations for services and map data
            GetAndMapOrganizationsForServices(services, unitOfWork);

            services.ForEach(service =>
            {
                // Filter out not published language versions
                FilterOutNotPublishedLanguageVersions(service, PublishingStatusCache.Get(PublishingStatus.Published), getOnlyPublished);
            });

            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Filtering: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// End measure

            var result = TranslationManagerToVm.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(services);

            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Translation: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// End measure

            if (result == null)
            {
                throw new Exception(CoreMessages.OpenApi.RecordNotFound);
            }

            // Service root ids
            List<Guid> rootIds = result.Where(c => c.Id != null).Select(c => c.Id.Value).Distinct().ToList();

            // Find only published service collections for services (which have at least one published language version)
            var allCollections = GetServiceCollections(rootIds, unitOfWork);
            // Find only published service channels for services
            var allConnections = GetServiceChannels(rootIds, unitOfWork);

            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Get connections from db: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// End measure

            // Get all related general descriptions
            var gdIds = result.Where(r => r.GeneralDescriptionId.IsAssigned()).Select(r => r.GeneralDescriptionId.Value).ToList();
            IList<VmOpenApiGeneralDescriptionVersionBase> gdList = null;
            if (fillWithAllGdData)
            {
                gdList = generalDescriptionService.GetPublishedGeneralDescriptionsWithDetails(gdIds);
            }
            else
            {
                gdList = generalDescriptionService.GetGeneralDescriptionsSimple(gdIds);
            }

            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Get related gds: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// End measure

            var versionList = new List<IVmOpenApiServiceVersionBase>();
            result.ForEach(service =>
            {
                // Map service collections
                if (allCollections?.Count > 0)
                {
                    var collections = allCollections.Where(c => c.OwnerReferenceId == service.Id).ToList();
                    if (collections?.Count > 0)
                    {
                        service.ServiceCollections = collections;
                    }
                }

                // Map connections
                if (allConnections?.Count > 0)
                {
                    var connections = allConnections.Where(s => s.OwnerReferenceId == service.Id).ToList();
                    if (connections?.Count > 0)
                    {
                        service.ServiceChannels = connections;
                    }
                }

                // Fill with the general description related data
                if (service.GeneralDescriptionId.IsAssigned())
                {
                    FillWithGeneralDescriptionData(service, gdList?.FirstOrDefault(gd => gd.Id == service.GeneralDescriptionId), fillWithAllGdData);
                }

                // Get the right open api view model version.
                versionList.Add(GetServiceByOpenApiVersion(unitOfWork, service, openApiVersion));
            });

            ////Measure
            //watch.Stop(); totalWatch.Stop();
            //logger.LogTrace($"*** Channels for services mapping: {watch.ElapsedMilliseconds} ms.");
            //logger.LogTrace($"*** Total: {totalWatch.ElapsedMilliseconds} ms.");
            //logger.LogTrace($"***************************************************.");
            //// End measure

            return versionList;
        }

        private List<VmOpenApiServiceServiceCollection> GetServiceCollections(List<Guid> rootIds, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            // Find only published service collections for a service (which have at least one published language version)
            var serviceCollectionRep = unitOfWork.CreateRepository<IServiceCollectionServiceRepository>();
            var serviceCollectionVersionsRep = unitOfWork.CreateRepository<IServiceCollectionVersionedRepository>();
            var allCollections = serviceCollectionRep.All().Where(c => rootIds.Contains(c.ServiceId) && c.ServiceCollection.Versions.Any(x => x.PublishingStatusId == publishedId) &&
                                                                        c.ServiceCollection.Versions.Any(t => t.LanguageAvailabilities.Any(l => l.StatusId == publishedId))).ToList();

            var collectionIds = allCollections.Select(i => i.ServiceCollectionId).ToList();
            var collectionVersions = unitOfWork.ApplyIncludes(serviceCollectionVersionsRep.All().Where(i => collectionIds.Contains(i.UnificRootId) && i.PublishingStatusId == publishedId), q => q.Include(j => j.ServiceCollectionNames)).ToList();

            return allCollections.Select(collection =>
                new VmOpenApiServiceServiceCollection
                {
                    Id = collection.ServiceCollectionId,
                    Name = TranslationManagerToVm
                        .TranslateAll<ServiceCollectionName, VmOpenApiLanguageItem>(collectionVersions.FirstOrDefault(i => i.UnificRootId == collection.ServiceCollectionId)?.ServiceCollectionNames).InclusiveToList(),
                    OwnerReferenceId = collection.ServiceId
                }).ToList();
        }

        private List<V11VmOpenApiServiceServiceChannel> GetServiceChannels(List<Guid> rootIds, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            // Find only published service channels for services
            var connectionRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var connectionQuery = connectionRep.All().Where(c => rootIds.Contains(c.ServiceId));
            var allConnections = connectionQuery.ToList();

            if (allConnections.Count <= 0)
            {
                return null;
            }

            commonService.IncludeConnectionAddresses(unitOfWork, allConnections);
            commonService.IncludeConnectionCommonDetails(unitOfWork, allConnections);
            commonService.IncludeConnectionContactDetails(unitOfWork, allConnections);

            var serviceList = new List<V11VmOpenApiServiceServiceChannel>();

            // Fill with service channel names
            var channelRootIds = allConnections.Select(s => s.ServiceChannelId).Distinct().ToList();
            var channelRepo = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();

            var channels = channelRepo.All()
                .Include(i => i.UnificRoot)
                .Include(i => i.ServiceChannelNames)
                .Include(i => i.LanguageAvailabilities)
                .Include(i => i.Type)
                .Where(i => channelRootIds.Contains(i.UnificRootId) && i.PublishingStatusId == publishedId)
                .ToList();

            // SFIPTV-1908: Somehow it is possible that two different versions of same entity are published at the same time,
            // so we need to make sure that there exists only one version per entity.
            if (channels.Any(c1 => channels.FirstOrDefault(c2 => c2.UnificRootId == c1.UnificRootId && c2.Id != c1.Id) != null))
            {
                channels = channels.GroupBy(x => x.UnificRootId).ToDictionary(i => i.Key, i => i.ToList())
                    .Select(x => x.Value.OrderByDescending(c => c.Modified).FirstOrDefault()).Where(x => x != null).ToList();
            }
            var channelDict = channels.ToDictionary(i => i.UnificRootId);


            var connectionChannelOrdering = allConnections.Select(connection =>
                {
                    if (channelDict.TryGetValue(connection.ServiceChannelId, out var channel))
                    {
                        return new
                        {
                            ChannelOrderNumber = channel?.Type?.OrderNumber ?? 0,
                            ConnectionOrderNumber = connection.ChannelOrderNumber ?? int.MaxValue,
                            Connection = connection,
                            Channel = channel
                        };
                    }

                    return null;
                })
                .Where(x => x != null)
                .OrderBy(x => x.ChannelOrderNumber)
                .ThenBy(x => x.ConnectionOrderNumber);

            connectionChannelOrdering.ForEach(model =>
            {
                var name = GetNameWithFallback(
                    model.Channel.ServiceChannelNames,
                    model.Channel.LanguageAvailabilities.Where(i => i.StatusId == publishedId).Select(i => i.LanguageId).ToList(),
                    typesCache,
                    languageCache);

                var vmssc = new V11VmOpenApiServiceServiceChannel
                {
                    OwnerReferenceId = model.Connection.ServiceId,
                    ServiceChannel = new VmOpenApiItem {Id = model.Connection.ServiceChannelId, Name = name},
                    Modified = model.Connection.Modified
                };

                // map base connection data
                MapConnection(model.Connection, vmssc, typesCache, languageCache);

                // extra types
                vmssc.ExtraTypes = GetExtraTypes(model.Connection, typesCache, languageCache);

                // contactdetails
                vmssc.ContactDetails = GetContactDetails<V9VmOpenApiContactDetails>(model.Connection, typesCache, languageCache);

                // digitalAuthorizations
                vmssc.DigitalAuthorizations = GetDigitalAuthorizations(model.Connection, languageCache);

                serviceList.Add(vmssc);
            });

            return serviceList;
        }

        private Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>> GetServiceIncludeChain()
        {
            return q =>
                q.Include(i => i.ServiceLanguages).ThenInclude(i => i.Language)
                    .Include(i => i.ServiceNames)
                    .Include(i => i.ServiceDescriptions)
                    .Include(i => i.ServiceRequirements)
                    .Include(i => i.OrganizationServices)
                    .Include(i => i.ServiceWebPages).ThenInclude(i => i.WebPage)
                    .Include(i => i.ServiceWebPages).ThenInclude(i => i.Localization)
                    .Include(i => i.LanguageAvailabilities)
                    .Include(i => i.AreaMunicipalities)
                    .Include(i => i.Organization)
                    .Include(i => i.UnificRoot);
        }

        private void IncludeClassification(IUnitOfWork unitOfWork, IList<ServiceVersioned> entities)
        {
            if (unitOfWork == null || entities == null || entities.Count == 0)
            {
                return;
            }

            var ids = entities.Select(e => e.Id).ToList();

            var keywordRepo = unitOfWork.CreateRepository<IServiceKeywordRepository>();
            var serviceClassRepo = unitOfWork.CreateRepository<IServiceServiceClassRepository>();
            var ontologyRepo = unitOfWork.CreateRepository<IServiceOntologyTermRepository>();
            var lifeEventRepo = unitOfWork.CreateRepository<IServiceLifeEventRepository>();
            var industrialClassRepo = unitOfWork.CreateRepository<IServiceIndustrialClassRepository>();
            var targetGroupRepo = unitOfWork.CreateRepository<IServiceTargetGroupRepository>();
            var serviceProducersRepo = unitOfWork.CreateRepository<IServiceProducerRepository>();

            var keywords = keywordRepo.All()
                .Include(x => x.Keyword)
                .Where(x => ids.Contains(x.ServiceVersionedId))
                .ToList();
            var serviceClasses = serviceClassRepo.All()
                .Include(x => x.ServiceClass).ThenInclude(x => x.Names)
                .Include(x => x.ServiceClass).ThenInclude(x => x.Descriptions)
                .Where(x => ids.Contains(x.ServiceVersionedId))
                .ToList();
            var ontologies = ontologyRepo.All()
                .Include(x => x.OntologyTerm).ThenInclude(x => x.Names)
                .Where(x => ids.Contains(x.ServiceVersionedId))
                .ToList();
            var lifeEvents = lifeEventRepo.All()
                .Include(x => x.LifeEvent).ThenInclude(x => x.Names)
                .Where(x => ids.Contains(x.ServiceVersionedId))
                .ToList();
            var industrialClasses = industrialClassRepo.All()
                .Include(x => x.IndustrialClass).ThenInclude(x => x.Names)
                .Where(x => ids.Contains(x.ServiceVersionedId))
                .ToList();
            var targetGroups = targetGroupRepo.All()
                .Include(x => x.TargetGroup).ThenInclude(x => x.Names)
                .Where(x => ids.Contains(x.ServiceVersionedId))
                .ToList();
            var serviceProducers = serviceProducersRepo.All()
                .Include(x => x.Organizations)
                .Include(x => x.AdditionalInformations)
                .Where(x => ids.Contains(x.ServiceVersionedId))
                .ToList();

            entities.ForEach(entity =>
            {
                entity.ServiceKeywords = keywords.Where(k => k.ServiceVersionedId == entity.Id).ToList();
                entity.ServiceServiceClasses = serviceClasses.Where(k => k.ServiceVersionedId == entity.Id).ToList();
                entity.ServiceOntologyTerms = ontologies.Where(k => k.ServiceVersionedId == entity.Id).ToList();
                entity.ServiceLifeEvents = lifeEvents.Where(k => k.ServiceVersionedId == entity.Id).ToList();
                entity.ServiceIndustrialClasses = industrialClasses.Where(k => k.ServiceVersionedId == entity.Id).ToList();
                entity.ServiceTargetGroups = targetGroups.Where(k => k.ServiceVersionedId == entity.Id).ToList();
                entity.ServiceProducers = serviceProducers.Where(k => k.ServiceVersionedId == entity.Id).ToList();
            });
        }

        private void IncludeLegislation(IUnitOfWork unitOfWork, IList<ServiceVersioned> entities)
        {
            if (unitOfWork == null || entities == null || entities.Count == 0)
            {
                return;
            }

            var ids = entities.Select(e => e.Id).ToList();


            var serviceLawRepo = unitOfWork.CreateRepository<IServiceLawRepository>();
            var serviceLaws = serviceLawRepo.All()
                .Include(j => j.Law).ThenInclude(k => k.Names)
                .Include(j => j.Law).ThenInclude(k => k.WebPages).ThenInclude(l => l.Localization)
                .Include(j => j.Law).ThenInclude(k => k.WebPages).ThenInclude(l => l.WebPage)
                .Where(x => ids.Contains(x.ServiceVersionedId))
                .ToList();

            entities.ForEach(entity => entity.ServiceLaws = serviceLaws.Where(k => k.ServiceVersionedId == entity.Id).ToList());
        }

        private void IncludeAreas(IUnitOfWork unitOfWork, IList<ServiceVersioned> entities)
        {
            if (unitOfWork == null || entities == null || entities.Count == 0)
            {
                return;
            }

            var ids = entities.Select(e => e.Id).ToList();

            var serviceAreasRepo = unitOfWork.CreateRepository<IServiceAreaRepository>();
            var serviceAreas = serviceAreasRepo.All()
                .Include(i => i.Area).ThenInclude(i => i.AreaNames)
                .Include(i => i.Area).ThenInclude(i => i.AreaMunicipalities)
                .Where(i => ids.Contains(i.ServiceVersionedId))
                .ToList();

            // Get related municipalities
            var allMunicipalityIds = serviceAreas.Select(a => a.Area).SelectMany(a => a.AreaMunicipalities).Select(m => m.MunicipalityId).Distinct().ToList();
            var entityMunicipalities = entities.SelectMany(a => a.AreaMunicipalities).Select(m => m.MunicipalityId).Distinct().ToList();
            allMunicipalityIds.AddRange(entityMunicipalities.Except(allMunicipalityIds));

            var municipalityRepo = unitOfWork.CreateRepository<IMunicipalityRepository>();
            var municipalities = municipalityRepo.All()
                .Include(m => m.MunicipalityNames)
                .Where(x => allMunicipalityIds.Contains(x.Id))
                .ToList();

            serviceAreas.ForEach(area => area.Area.AreaMunicipalities.ForEach(a =>
            {
                if (a.Municipality == null)
                {
                    a.Municipality = municipalities.FirstOrDefault(m => m.Id == a.MunicipalityId);
                }
            }));

            entities.ForEach(entity =>
            {
                entity.AreaMunicipalities.ForEach(am => am.Municipality = municipalities.FirstOrDefault(m => m.Id == am.MunicipalityId));
                entity.Areas = serviceAreas.Where(a => a.ServiceVersionedId == entity.Id).ToList();
            });
        }

        private void FilterOutNotPublishedLanguageVersions(ServiceVersioned service, Guid publishedId, bool getOnlyPublished)
        {
            // Filter out not published language versions
            if (getOnlyPublished)
            {
                var notPublishedLanguageVersions = service.LanguageAvailabilities.Where(l => l.StatusId != publishedId).Select(l => l.LanguageId).ToList();
                if (notPublishedLanguageVersions.Count > 0)
                {
                    service.ServiceNames = service.ServiceNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    service.ServiceDescriptions = service.ServiceDescriptions.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    service.ServiceKeywords = service.ServiceKeywords.Where(i => !notPublishedLanguageVersions.Contains(i.Keyword.LocalizationId)).ToList();
                    service.ServiceLaws.ForEach(law =>
                    {
                        law.Law.Names = law.Law.Names.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        law.Law.WebPages = law.Law.WebPages.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    });
                    service.ServiceRequirements = service.ServiceRequirements.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    service.UnificRoot.ServiceServiceChannels.ForEach(channel =>
                    {
                        channel.ServiceServiceChannelDescriptions = channel.ServiceServiceChannelDescriptions.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    });
                    service.ServiceProducers.ForEach(sp => { sp.AdditionalInformations = sp.AdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList(); });
                    service.ServiceWebPages = service.ServiceWebPages.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                }
            }
        }

        private IVmOpenApiServiceVersionBase GetServiceByOpenApiVersion(IUnitOfWork unitOfWork, IVmOpenApiServiceVersionBase baseVersion, int openApiVersion)
        {
            // Get the sourceId if user is logged in
            var userId = utilities.GetRelationIdForExternalSource(false);
            if (!string.IsNullOrEmpty(userId))
            {
                baseVersion.SourceId = GetSourceId<Service>(baseVersion.Id.Value, userId, unitOfWork);
            }

            return GetEntityByOpenApiVersion(baseVersion, openApiVersion);
        }

        private IVmOpenApiServiceVersionBase GetServiceWithSimpleDetails(IUnitOfWork unitOfWork, Guid versionId)
        {
            if (!versionId.IsAssigned()) return null;

            ServiceVersioned entity = null;
            return GetModel<ServiceVersioned, VmOpenApiServiceVersionBase>(entity = GetEntity<ServiceVersioned>(versionId, unitOfWork,
                q => q.Include(x => x.LanguageAvailabilities)
                    .Include(i => i.ServiceTargetGroups).ThenInclude(i => i.TargetGroup)
                    .Include(i => i.ServiceNames)
                    .Include(i => i.ServiceDescriptions)
                    .Include(i => i.Organization)), unitOfWork);
        }

        private void CheckVm(IVmOpenApiServiceInVersionBase vm, IUnitOfWorkWritable unitOfWork, bool attachProposedChannels, bool createOperation = false, Guid? rootId = null)
        {
            CheckKeywords(vm, unitOfWork);
            CheckTargetGroups(vm.TargetGroups);

            // Check general description related data.
            // In PUT method the view model may not include general description even the service has earlier been attached into a general description.
            // Therefore if the request viewmodel does not include general description id let's get the general description related to the service from db.
            // If gd is deleted we should not check the gd related data from old version. PTV-3926
            var generalDescriptionId = vm.GeneralDescriptionId.ParseToGuid();
            if (!generalDescriptionId.IsAssigned() && !vm.DeleteGeneralDescriptionId && rootId.IsAssigned())
            {
                // Let's try to get the statutory general description attached for service from db.
                var service = VersioningManager.GetSpecificVersionByRoot<ServiceVersioned>(unitOfWork, rootId.Value, PublishingStatus.Published)
                              ?? VersioningManager.GetSpecificVersionByRoot<ServiceVersioned>(unitOfWork, rootId.Value, PublishingStatus.Draft);
                if (service != null)
                {
                    generalDescriptionId = service.StatutoryServiceGeneralDescriptionId;
                }
            }

            if (generalDescriptionId.IsAssigned())
            {
                // Get the general description
                var generalDescription = generalDescriptionService.GetGeneralDescriptionVersionBase(generalDescriptionId.Value, 0);
                if (generalDescription != null)
                {
                    // If name is defined within general description and service name is empty let's copy general decription name into service - only when creating the object!
                    if (generalDescription.Names?.Count() > 0 && (vm.ServiceNames == null || vm.ServiceNames?.Count() == 0) && createOperation)
                    {
                        vm.ServiceNames = generalDescription.Names.ToList();
                    }

                    // If service type is defined within general description, service related type is ignored
                    if (!string.IsNullOrEmpty(generalDescription.Type))
                    {
                        vm.Type = null;
                    }

                    // If service charge type is defined within general description, service related service charge type is ignored
                    if (!string.IsNullOrEmpty(generalDescription.ServiceChargeType))
                    {
                        vm.ServiceChargeType = null;
                    }

                    // Check finto items so that service and general description does not include overlapping finto items
                    vm.ServiceClasses = CheckFintoItems(vm.ServiceClasses, generalDescription.ServiceClasses);
                    vm.OntologyTerms = CheckFintoItems(vm.OntologyTerms, generalDescription.OntologyTerms);
                    vm.TargetGroups = CheckFintoItems(vm.TargetGroups, generalDescription.TargetGroups);
                    vm.LifeEvents = CheckFintoItems(vm.LifeEvents, generalDescription.LifeEvents);
                    vm.IndustrialClasses = CheckFintoItems(vm.IndustrialClasses, generalDescription.IndustrialClasses);

                    // attach proposed service channels into service (PTV-2315)
                    if (attachProposedChannels && generalDescription.ServiceChannels?.Count > 0)
                    {
                        if (vm.ServiceServiceChannels == null)
                        {
                            vm.ServiceServiceChannels = new List<V11VmOpenApiServiceServiceChannelAstiInBase>();
                        }

                        generalDescription.ServiceChannels.ForEach(channel =>
                        {
                            // do we already have the item in a channel list?
                            var item = vm.ServiceServiceChannels.Where(i => i.ChannelGuid == channel.ServiceChannel.Id.Value).FirstOrDefault();

                            if (item == null)
                            {
                                vm.ServiceServiceChannels.Add(new V11VmOpenApiServiceServiceChannelAstiInBase
                                {
                                    ChannelGuid = channel.ServiceChannel.Id.Value,
                                    ServiceChargeType = channel.ServiceChargeType,
                                    Description = channel.Description,
                                });
                            }
                            else
                            {
                                item.ServiceChargeType = channel.ServiceChargeType;
                                item.Description = channel.Description;
                            }
                        });
                    }
                }
            }
        }

        private IList<string> CheckFintoItems<TFintoModel>(IList<string> list, IList<TFintoModel> fintoItemList) where TFintoModel : IVmOpenApiFintoItemVersionBase
        {
            return (fintoItemList == null) || (list == null) ? list : list.Except(fintoItemList.Select(i => i.Uri)).ToList();
        }

        private void CheckKeywords(IVmOpenApiServiceInVersionBase vm, IUnitOfWorkWritable unitOfWork)
        {
            if (vm.Keywords == null || vm.Keywords.Count == 0)
            {
                return;
            }

            var rep = unitOfWork.CreateRepository<IKeywordRepository>();
            var keywordsToCheck = vm.Keywords.Select(i => i.Value.ToLower()).ToList();
            var localizations = vm.Keywords.Select(i => languageCache.Get(i.Language)).ToList();
            var keywords = rep.All().Where(i => keywordsToCheck.Contains(i.Name.ToLower()) && localizations.Contains(i.LocalizationId)).ToList();
            vm.Keywords.ForEach(k =>
            {
                var keyWord = keywords.FirstOrDefault(x => x.Name.ToLower() == k.Value.ToLower() && x.LocalizationId == languageCache.Get(k.Language));
                if (keyWord != null)
                {
                    k.Id = keyWord.Id;
                }
            });
        }

        private void CheckTargetGroups(IList<string> targetGroupsUri)
        {
            if (targetGroupsUri.IsNullOrEmpty()) return;
            var parentUris = targetGroupDataCache.All().Where(tg => targetGroupsUri.Contains(tg.Uri)).Where(i => !string.IsNullOrEmpty(i.ParentUri)).Select(i => i.ParentUri).Except(targetGroupsUri).ToList();
            parentUris.ForEach(targetGroupsUri.Add);
        }

        private void FillWithGeneralDescriptionData(IVmOpenApiServiceVersionBase service, IUnitOfWork unitOfWork, bool fillWithAllData = false)
        {
            if (!service.GeneralDescriptionId.IsAssigned())
            {
                return;
            }

            // Get the general description
            IVmOpenApiGeneralDescriptionVersionBase generalDescription = null;
            //
            if (fillWithAllData)
            {
                generalDescription = generalDescriptionService.GetPublishedGeneralDescriptionWithDetails(unitOfWork, service.GeneralDescriptionId.Value);
            }
            else
            {
                generalDescription = generalDescriptionService.GetGeneralDescriptionSimple(unitOfWork, service.GeneralDescriptionId.Value);
            }

            // Fill with GD data
            if (generalDescription != null)
            {
                FillWithGeneralDescriptionData(service, generalDescription, fillWithAllData);
            }
        }

        private void FillWithGeneralDescriptionData(IVmOpenApiServiceVersionBase service, IVmOpenApiGeneralDescriptionVersionBase generalDescription, bool fillWithAllData = false)
        {
            // PTV-1667: Type (Tyyppi), Name (Nimi), Target Groups (Kohderyhmä), ServiceClass (Palveluluokka) and OntologyWords (Asiasanat) are filled from general description.
            // Name is always saved into db (copied from general description), so we do not need to fill it.
            // SFIPTV-786: Fill with all general description data.
            if (generalDescription == null) return;

/* SOTE has been disabled (SFIPTV-1177)
            // SOTE: service-subVersion
            if (generalDescription.IsSoteType)
            {
                service.SubType = generalDescription.GeneralDescriptionType;
            } else
*/
            if (fillWithAllData) // We should always return the general desription type when we are filling all the data from GD (SFIPTV-786)
            {
                service.SubType = generalDescription.GeneralDescriptionType;
            }

            // If service type is defined within general description, service related type is ignored
            if (!string.IsNullOrEmpty(generalDescription.Type) && service.Type.IsNullOrEmpty())
            {
                service.Type = generalDescription.Type;
            }

            // finto items - attach items from general description into service items.
            // Target groups
            if (service.TargetGroups.Count == 0)
            {
                service.TargetGroups = generalDescription.TargetGroups;
            }
            else
            {
                var targetGroups = service.TargetGroups.Where(t => !t.Override).ToList(); // Get the items that are not overridden
                var targetGroupUris = targetGroups.Select(t => t.Uri).ToList();
                var overriddenUris = service.TargetGroups.Where(t => t.Override).Select(t => t.Uri).ToList();
                generalDescription.TargetGroups.ForEach(target =>
                {
                    if (!overriddenUris.Contains(target.Uri)) // If general description target group is not overridden by service target group let's attach it into service target groups.
                    {
                        if (!targetGroupUris.Contains(target.Uri)) targetGroups.Add(target);
                    }
                });
                service.TargetGroups = targetGroups;
            }

            service.ServiceClasses = generalDescription.ServiceClasses.Union(service.ServiceClasses, new FintoItemComparer<V7VmOpenApiFintoItemWithDescription>()).ToList();
            service.OntologyTerms = generalDescription.OntologyTerms.Union(service.OntologyTerms, new FintoItemComparer<V4VmOpenApiFintoItem>()).ToList();

            // Check service charge type.
            // If general description has charge type set, charge type for service has to be null! PTV-2347
            // If we are filling with all the data, let's always get the service charge type from GD if it is set (SFIPTV-786).
            if (!string.IsNullOrEmpty(generalDescription.ServiceChargeType))
            {
                if (fillWithAllData)
                {
                    service.ServiceChargeType = generalDescription.ServiceChargeType;
                }
                else
                {
                    service.ServiceChargeType = null;
                }
            }

            // Fill all the other data from GD (SFIPTV-786)
            if (fillWithAllData)
            {
                // Descriptions
                if (generalDescription.Descriptions?.Count > 0)
                {
                    // Let's mark general description related descriptions with prefix 'GD_'.
                    var prefix = "GD_";
                    var gdDescriptions = generalDescription.Descriptions.Where(d => !d.Value.IsNullOrEmpty()).ToList();
                    if (gdDescriptions?.Count > 0)
                    {
                        if (service.ServiceDescriptions == null)
                        {
                            service.ServiceDescriptions = new List<VmOpenApiLocalizedListItem>();
                        }
                        gdDescriptions.ForEach(i =>
                        {
                            if (!i.Type.StartsWith(prefix))
                            { i.Type = prefix + i.Type; }
                            service.ServiceDescriptions.Add(i);
                        });
                    }
                }

                // Life events
                service.LifeEvents = generalDescription.LifeEvents.Union(service.LifeEvents, new FintoItemComparer<V4VmOpenApiFintoItem>()).ToList();
                // Industrial classes
                service.IndustrialClasses = generalDescription.IndustrialClasses.Union(service.IndustrialClasses, new FintoItemComparer<V4VmOpenApiFintoItem>()).ToList();

                // Requirements
                if (generalDescription.Requirements?.Count > 0)
                {
                    if (service.Requirements == null || service.Requirements?.Count == 0 || service.Requirements.All(x => x.Value == null))
                    {
                        service.Requirements = generalDescription.Requirements;
                    }
                    else
                    {
                        generalDescription.Requirements.ForEach(r =>
                        {
                            var serviceRequirement = service.Requirements.FirstOrDefault(sr => sr.Language == r.Language);

                            if (serviceRequirement != null)
                            {
                                serviceRequirement.Value = string.IsNullOrEmpty(serviceRequirement.Value)
                                    ? r.Value
                                    : r.Value + "\n\n" + serviceRequirement.Value;
                            }
                        });
                    }
                }

                // Legislation
                if (generalDescription.Legislation?.Count > 0)
                {
                    if (service.Legislation == null || service.Legislation.Count == 0)
                    {
                        service.Legislation = generalDescription.Legislation;
                    }
                    else
                    {
                        generalDescription.Legislation.ForEach(l => service.Legislation.Add(l));
                    }
                }
            }
        }

        public class FintoItemComparer<T> : IEqualityComparer<T> where T : VmOpenApiFintoItemVersionBase
        {
            public bool Equals(T x, T y)
            {
                return x.Uri == y.Uri;
            }

            public int GetHashCode(T obj)
            {
                return obj.Id.GetHashCode();
            }
        }


        public List<Guid> CheckServicesAccess(List<Guid> serviceIds)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var result = new List<Guid>();
                if (utilities.UserHighestRole() == UserRoleEnum.Eeva) return result;
                serviceIds.ForEach(id =>
                {
                    var serviceInfo =
                        VersioningManager.GetLastPublishedModifiedDraftVersion<ServiceVersioned>(unitOfWork, id);
                    var service = unitOfWork.CreateRepository<IServiceVersionedRepository>().All()
                        .Single(x => x.Id == serviceInfo.EntityId);
                    if (!GetOrganizationSecurity(service, unitOfWork))
                    {
                        result.Add(id);
                    }
                });
                return result;
            });
        }
        #endregion
    }
}
