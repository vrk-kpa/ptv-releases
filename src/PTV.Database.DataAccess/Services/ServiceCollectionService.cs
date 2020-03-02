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
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Enums;

using Microsoft.Extensions.Logging;
using PTV.Domain.Model.Models.OpenApi;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Framework.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Database.DataAccess.Utils.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V10;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof (IServiceCollectionService), RegisterType.Transient)]
    internal class ServiceCollectionService : ServiceBase, IServiceCollectionService
    {
        private IContextManager contextManager;

        private ILogger logger;
        private IServiceUtilities utilities;
        private ICommonServiceInternal commonService;
        private ITypesCache typesCache;
        private ILanguageOrderCache languageOrderCache;
        private ILanguageCache languageCache;

        public ServiceCollectionService(
            IContextManager contextManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            ILogger<ServiceService> logger,
            IServiceUtilities utilities,
            ICommonServiceInternal commonService,
            ITypesCache typesCache,
            IPublishingStatusCache publishingStatusCache,
            IVersioningManager versioningManager,
            IUserOrganizationChecker userOrganizationChecker,
            ILanguageOrderCache languageOrderCache,
            ILanguageCache languageCache)
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker, versioningManager)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.commonService = commonService;
            this.typesCache = typesCache;
            this.languageOrderCache = languageOrderCache;
            this.languageCache = languageCache;
        }

        public IVmEntityBase DeleteServiceCollection(Guid serviceCollectionId)
        {
            ServiceCollectionVersioned result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = DeleteServiceCollection(unitOfWork, serviceCollectionId);
                unitOfWork.Save();

            });
            return new VmEntityStatusBase { Id = result.Id, PublishingStatusId = result.PublishingStatusId };
        }

        private ServiceCollectionVersioned DeleteServiceCollection(IUnitOfWorkWritable unitOfWork, Guid? serviceCollectionId)
        {
            var publishStatus = TranslationManagerToEntity.Translate<String, PublishingStatusType>(PublishingStatus.Deleted.ToString(), unitOfWork);

            var serviceCollectionRep = unitOfWork.CreateRepository<IServiceCollectionVersionedRepository>();
            var serviceCollection = serviceCollectionRep.All().Single(x => x.Id == serviceCollectionId.Value);
            serviceCollection.PublishingStatus = publishStatus;

            return serviceCollection;
        }

        #region Open Api

        public IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServiceCollections(DateTime? date, int pageNumber, int pageSize, bool archived, DateTime? dateBefore = null)
        {
            var pageHandler = new V3GuidPagingHandler<ServiceCollectionVersioned, ServiceCollection, ServiceCollectionName, ServiceCollectionLanguageAvailability>(
                archived ? EntityStatusExtendedEnum.Archived : EntityStatusExtendedEnum.Published, date, dateBefore, PublishingStatusCache, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, pageHandler);
        }

        public IVmOpenApiModelWithPagingBase<V10VmOpenApiServiceCollectionItem> GetServiceCollectionsByOrganization(IList<Guid> organizationIds, int pageNumber)
        {
            var pageHandler = new ServiceCollectionsByOrganizationPagingHandler(organizationIds, PublishingStatusCache, languageCache, typesCache, TranslationManagerToVm, pageNumber);
            return GetPage(contextManager, pageHandler);
        }

        public IVmOpenApiServiceCollectionVersionBase GetServiceCollectionById(Guid id, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiServiceCollectionVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetServiceCollectionById(unitOfWork, id, openApiVersion, getOnlyPublished);
            });

            return result;
        }

        private IVmOpenApiServiceCollectionVersionBase GetServiceCollectionById(IUnitOfWork unitOfWork, Guid id, int openApiVersion, bool getOnlyPublished)
        {
            try
            {
                // Get the right version id
                Guid? entityId = null;
                if (getOnlyPublished)
                {
                    entityId = VersioningManager.GetVersionId<ServiceCollectionVersioned>(unitOfWork, id, PublishingStatus.Published);
                }
                else
                {
                    entityId = VersioningManager.GetVersionId<ServiceCollectionVersioned>(unitOfWork, id);
                }
                if (entityId.IsAssigned())
                {
                    return GetServiceCollectionWithDetails(unitOfWork, entityId.Value, openApiVersion, getOnlyPublished);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting a service collection with id {0}. {1}", id, ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }
            return null;
        }

        public IVmOpenApiServiceCollectionVersionBase GetServiceCollectionBySource(string sourceId, int openApiVersion, bool getOnlyPublished = true, string userName = null)
        {
            IVmOpenApiServiceCollectionVersionBase result = new VmOpenApiServiceCollectionVersionBase();
            var userId = userName ?? utilities.GetRelationIdForExternalSource();
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var rootId = GetPTVId<ServiceCollection>(sourceId, userId, unitOfWork);
                    result = GetServiceCollectionById(unitOfWork, rootId, openApiVersion, getOnlyPublished);

                });
            }
            catch (Exception ex)
            {
                logger.LogError($"Error occured while getting service collection by source id {sourceId}. {ex.Message}");
                throw ex;
            }
            return result;
        }

        public IVmOpenApiServiceCollectionBase AddServiceCollection(IVmOpenApiServiceCollectionInVersionBase vm, int openApiVersion, string userName = null)
        {
            var serviceCollection = new ServiceCollectionVersioned();
            var saveMode = SaveMode.Normal;
            var userId = userName ?? utilities.GetRelationIdForExternalSource(false);
            var useOtherEndPoint = false;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Check if the external source already exists. Let's not throw the excpetion here to avoid contextManager to catch the exception.
                useOtherEndPoint = ExternalSourceExists<ServiceCollection>(vm.SourceId, userId, unitOfWork);
                if (!useOtherEndPoint)
                {
                    serviceCollection = TranslationManagerToEntity.Translate<IVmOpenApiServiceCollectionInVersionBase, ServiceCollectionVersioned>(vm, unitOfWork);
                    var index = 0;
                    var list = new List<VmOpenApiServiceCollectionService>();
                    vm.ServiceCollectionServices.ForEach(serviceId => list.Add(new VmOpenApiServiceCollectionService
                    {
                        Id = serviceId,
                        OwnerReferenceId = serviceCollection.UnificRootId,
                        OrderNumber = ++index
                    }));
                    serviceCollection.UnificRoot.ServiceCollectionServices = TranslationManagerToEntity.TranslateAll<VmOpenApiServiceCollectionService, Model.Models.ServiceCollectionService>(list, unitOfWork).ToList();

                    var serviceCollectionRep = unitOfWork.CreateRepository<IServiceCollectionVersionedRepository>();
                    serviceCollectionRep.Add(serviceCollection);

                    // Create the mapping between external source id and PTV id
                    if (!string.IsNullOrEmpty(vm.SourceId))
                    {
                        SetExternalSource(serviceCollection.UnificRoot, vm.SourceId, userId, unitOfWork);
                    }
                    commonService.CreateHistoryMetaData<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(serviceCollection);
                    unitOfWork.Save(saveMode, userName: userName);
                }
            });

            if (useOtherEndPoint)
            {
                throw new ExternalSourceExistsException(string.Format(CoreMessages.OpenApi.ExternalSourceExists, vm.SourceId));
            }

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = commonService.PublishAllAvailableLanguageVersions<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(serviceCollection.Id, i => i.ServiceCollectionVersionedId == serviceCollection.Id);
            }

            return GetServiceCollectionWithDetails(serviceCollection.Id, openApiVersion, false);
        }

        public IVmOpenApiServiceCollectionBase SaveServiceCollection(IVmOpenApiServiceCollectionInVersionBase vm, int openApiVersion, string sourceId = null, string userName = null)
        {
            var saveMode = SaveMode.Normal;
            IVmOpenApiServiceCollectionBase result = new VmOpenApiServiceCollectionBase();
            ServiceCollectionVersioned serviceCollection = null;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                if (vm.PublishingStatus == PublishingStatus.Deleted.ToString())
                {
                    serviceCollection = DeleteServiceCollection(unitOfWork, vm.VersionId.Value);
                }
                else
                {
                    // Entity needs to be restored?
                    if (vm.CurrentPublishingStatus == PublishingStatus.Deleted.ToString())
                    {
                        if (vm.PublishingStatus == PublishingStatus.Modified.ToString() || vm.PublishingStatus == PublishingStatus.Published.ToString())
                        {
                            // We need to restore already archived item
                            var publishingResult = commonService.RestoreArchivedEntity<ServiceCollectionVersioned,ServiceCollectionLanguageAvailability>(unitOfWork, vm.VersionId.Value);
                        }
                    }

                    serviceCollection = TranslationManagerToEntity.Translate<IVmOpenApiServiceCollectionInVersionBase, ServiceCollectionVersioned>(vm, unitOfWork);

                    if (vm.DeleteAllServices || vm.ServiceCollectionServices?.Count > 0)
                    {
                        var list = new List<VmOpenApiServiceCollectionService>();
                        var serviceCollServiceRep = unitOfWork.CreateRepository<IServiceCollectionServiceRepository>();
                        var index = !vm.DeleteAllServices ? serviceCollServiceRep
                            .All().Count(x => x.ServiceCollectionId == serviceCollection.UnificRootId) : 0;
                        vm.ServiceCollectionServices.ForEach(serviceId => list.Add(new VmOpenApiServiceCollectionService
                        {
                            Id = serviceId,
                            OwnerReferenceId = vm.Id,
                            OrderNumber = ++index
                        }));
                        var resulta = TranslationManagerToEntity.TranslateAll<VmOpenApiServiceCollectionService, Model.Models.ServiceCollectionService>(list, unitOfWork);
                        if (vm.DeleteAllServices)
                        {
                            serviceCollection.UnificRoot.ServiceCollectionServices = resulta.ToList();
                        }
                        else
                        {
                            resulta.ForEach(item => serviceCollection.UnificRoot.ServiceCollectionServices.Add(item));
                        }
                    }

                    // Update the mapping between external source id and PTV id
                    if (!string.IsNullOrEmpty(vm.SourceId))
                    {
                        UpdateExternalSource<ServiceCollection>(serviceCollection.UnificRootId, vm.SourceId, utilities.GetRelationIdForExternalSource(), unitOfWork);
                    }
                }
                commonService.CreateHistoryMetaData<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(serviceCollection, setByEntity: true);
                unitOfWork.Save(saveMode, PreSaveAction.Standard, serviceCollection, userName);
            });

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = commonService.PublishAllAvailableLanguageVersions<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(serviceCollection.Id, i => i.ServiceCollectionVersionedId == serviceCollection.Id);
            }

            return GetServiceCollectionWithDetails(serviceCollection.Id, openApiVersion, false);
        }

        private IVmOpenApiServiceCollectionVersionBase GetServiceCollectionWithDetails(IUnitOfWork unitOfWork, Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            return GetServiceCollectionsWithDetails(unitOfWork, new List<Guid> { versionId }, openApiVersion, getOnlyPublished).FirstOrDefault();
        }

        private IVmOpenApiServiceCollectionVersionBase GetServiceCollectionWithDetails(Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiServiceCollectionVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetServiceCollectionsWithDetails(unitOfWork, new List<Guid> { versionId }, openApiVersion, getOnlyPublished).FirstOrDefault();
            });
            return result;
        }

        private IList<IVmOpenApiServiceCollectionVersionBase> GetServiceCollectionsWithDetails(IUnitOfWork unitOfWork, List<Guid> versionIdList, int openApiVersion, bool getOnlyPublished = true)
        {
            if (versionIdList.Count == 0) return new List<IVmOpenApiServiceCollectionVersionBase>();

            var serviceCollectionRep = unitOfWork.CreateRepository<IServiceCollectionVersionedRepository>();
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();

            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var resultTemp = unitOfWork.ApplyIncludes(serviceCollectionRep.All().Where(s => versionIdList.Contains(s.Id)), q =>
                q.Include(i => i.ServiceCollectionNames)
                .Include(i => i.ServiceCollectionDescriptions)
                .Include(i => i.UnificRoot).ThenInclude(i=> i.ServiceCollectionServices)
                .Include(i => i.LanguageAvailabilities)
                .OrderByDescending(i => i.Modified));

            // Filter out items that do not have language versions published!
            var serviceCollections = getOnlyPublished ? resultTemp.Where(c => c.LanguageAvailabilities.Any(l => l.StatusId == publishedId)).ToList() : resultTemp.ToList();

            var serviceIds = serviceCollections.SelectMany(i => i.UnificRoot.ServiceCollectionServices).Select(i => i.ServiceId).ToList();
            var services = unitOfWork.ApplyIncludes(serviceRep.All().Where(s => serviceIds.Contains(s.UnificRootId) && s.PublishingStatusId == publishedId), q =>
                q.Include(h => h.ServiceNames).Include(h => h.ServiceDescriptions).Include(h => h.LanguageAvailabilities).Include(h => h.UnificRoot)).ToList();

            serviceCollections.SelectMany(i => i.UnificRoot.ServiceCollectionServices).ForEach(srvCol => { srvCol.Service = services.FirstOrDefault(i => i.UnificRootId == srvCol.ServiceId)?.UnificRoot; });

            // Find only published services for service collections
//            var publishedServices = serviceCollections.SelectMany(i => i.UnificRoot.ServiceCollectionServices).Select(i => i.Service).SelectMany(i => i.Versions)
//                .Where(i => i.PublishingStatusId == publishedId).ToList();
            var publishedServiceRootIds = services.Select(i => i.UnificRootId).Distinct().ToList();

            serviceCollections.ForEach(serviceCollection =>
            {
                // Filter out not published services
                serviceCollection.UnificRoot.ServiceCollectionServices = serviceCollection.UnificRoot.ServiceCollectionServices.Where(c => publishedServiceRootIds.Contains(c.ServiceId)).ToList();

                // Filter out not published language versions
                if (getOnlyPublished)
                {
                    var notPublishedLanguageVersions = serviceCollection.LanguageAvailabilities.Where(l => l.StatusId != publishedId).Select(l => l.LanguageId).ToList();
                    if (notPublishedLanguageVersions.Count > 0)
                    {
                        serviceCollection.ServiceCollectionNames = serviceCollection.ServiceCollectionNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        serviceCollection.ServiceCollectionDescriptions = serviceCollection.ServiceCollectionDescriptions.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    }
                }
            });

//            // Fill with service names (only type: Name)
//            var serviceNames = unitOfWork.CreateRepository<IServiceNameRepository>().All()
//                .Where(i => publishedServiceIds.Contains(i.ServiceVersionedId) && i.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()))
//                .GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.ToList());
//
//            // Fill with service descriptions (only type: Description)
//            var serviceDescriptions = unitOfWork.CreateRepository<IServiceDescriptionRepository>().All()
//                .Where(i => publishedServiceIds.Contains(i.ServiceVersionedId) && i.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()))
//                .GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.ToList());
//
//            // Get language availabilities for published services
//            var serviceLanguageAvailabilities = unitOfWork.CreateRepository<IServiceLanguageAvailabilityRepository>().All()
//                .Where(i => publishedServiceIds.Contains(i.ServiceVersionedId) && i.StatusId == publishedId)
//                .GroupBy(i => i.ServiceVersionedId)
//                .ToDictionary(i => i.Key, i => i.Select(l => l.LanguageId).ToList());

            // Filter out name and description language versions that are not available
            services.ForEach(s =>
            {
                var availableLanguages = s.LanguageAvailabilities.Where(i => i.StatusId == publishedId).Select(i => i.LanguageId).ToList();
                var nameType = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
                var descriptionType = typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString());
                s.ServiceNames = s.ServiceNames.Where(n => n.TypeId == nameType && availableLanguages.Contains(n.LocalizationId)).ToList();
                s.ServiceDescriptions = s.ServiceDescriptions.Where(n => n.TypeId == descriptionType && availableLanguages.Contains(n.LocalizationId)).ToList();
            });

            var result = TranslationManagerToVm.TranslateAll<ServiceCollectionVersioned, VmOpenApiServiceCollectionVersionBase>(serviceCollections);
            if (result.IsNullOrEmpty())
            {
                throw new Exception(CoreMessages.OpenApi.RecordNotFound);
            }

            // Get the right open api view model version
            var userId = utilities.GetRelationIdForExternalSource(false);
            var versionList = new List<IVmOpenApiServiceCollectionVersionBase>();
            result.ForEach(serviceCollection =>
            {
                // Get the sourceId if user is logged in
                if (serviceCollection.Id.IsAssigned() && !string.IsNullOrEmpty(userId))
                {
                    serviceCollection.SourceId = GetSourceId<ServiceCollection>(serviceCollection.Id.Value, userId, unitOfWork);
                }
                versionList.Add(GetEntityByOpenApiVersion(serviceCollection as IVmOpenApiServiceCollectionVersionBase, openApiVersion));
            });

            return versionList;
        }

        #endregion
    }
}
