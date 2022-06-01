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

        public IVmOpenApiServiceCollectionVersionBase GetServiceCollectionById(Guid id, int openApiVersion, bool getOnlyPublished = true, bool showHeader = false)
        {
            IVmOpenApiServiceCollectionVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetServiceCollectionById(unitOfWork, id, openApiVersion, getOnlyPublished, showHeader);
            });

            return result;
        }

        private IVmOpenApiServiceCollectionVersionBase GetServiceCollectionById(IUnitOfWork unitOfWork, Guid id, int openApiVersion, bool getOnlyPublished, bool showHeader)
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
                    var result = GetServiceCollectionWithDetails(unitOfWork, entityId.Value, openApiVersion, showHeader, getOnlyPublished);
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while getting a service collection with id {id}");
                throw;
            }
            return null;
        }

        private IEnumerable<VmOpenApiServiceCollectionChannel> GetDummyChannels(IUnitOfWork unitOfWork)
        {
            var channelRepo = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var channels = channelRepo.All()
                .Include(x => x.Type)
                .Include(x => x.ServiceChannelDescriptions)
                .Include(x => x.ServiceChannelNames)
                .OrderBy(x => x.Modified)
                .Take(10)
                .ToList()
                .Select(x => new VmOpenApiServiceCollectionChannel
                {
                    Description = new List<VmOpenApiLocalizedListItem>
                    {
                        new VmOpenApiLocalizedListItem
                        {
                            Language = "fi",
                            Type = "Description",
                            Value = x.ServiceChannelDescriptions.FirstOrDefault()?.Description
                        }
                    },
                    Id = x.UnificRootId,
                    Name = new List<VmOpenApiLocalizedListItem>
                    {
                        new VmOpenApiLocalizedListItem
                        {
                            Language = "fi",
                            Type = "Name",
                            Value = x.ServiceChannelNames.FirstOrDefault()?.Name
                        }
                    },
                    ServiceChannelType = x.Type.Code
                });

            return channels;
        }

        public IVmOpenApiServiceCollectionVersionBase GetServiceCollectionBySource(string sourceId, int openApiVersion, bool getOnlyPublished = true, string userName = null, bool showHeader = false)
        {
            IVmOpenApiServiceCollectionVersionBase result = new VmOpenApiServiceCollectionVersionBase();
            var userId = userName ?? utilities.GetRelationIdForExternalSource();
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var rootId = GetPTVId<ServiceCollection>(sourceId, userId, unitOfWork);
                    result = GetServiceCollectionById(unitOfWork, rootId, openApiVersion, getOnlyPublished, showHeader);

                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while getting service collection by source id {sourceId}.");
                throw;
            }
            return result;
        }

        public IVmOpenApiServiceCollectionBase AddServiceCollection(IVmOpenApiServiceCollectionInVersionBase vm, int openApiVersion, string userName = null)
        {
            var serviceCollection = new ServiceCollectionVersioned();
            var saveMode = SaveMode.Normal;
            var userId = userName ?? utilities.GetRelationIdForExternalSource(false);

            // Using separate ExecuteWriter calls because TranslationManagerToEntity does not
            // support creating a new entity and saving child collection during
            // single unit of work.

            contextManager.ExecuteWriter(unitOfWork =>
            {
                ThrowIfExternalSourceExists<ServiceCollection>(vm.SourceId, userId, unitOfWork);

                serviceCollection = TranslationManagerToEntity.Translate<IVmOpenApiServiceCollectionInVersionBase, ServiceCollectionVersioned>(vm, unitOfWork);
                var repository = unitOfWork.CreateRepository<IServiceCollectionVersionedRepository>();
                repository.Add(serviceCollection);

                // Create the mapping between external source id and PTV id
                if (!string.IsNullOrEmpty(vm.SourceId))
                {
                    SetExternalSource(serviceCollection.UnificRoot, vm.SourceId, userId, unitOfWork);
                }

                commonService.CreateHistoryMetaData<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(serviceCollection);

                unitOfWork.Save(saveMode, userName: userName);
            });

            contextManager.ExecuteWriter(unitOfWork =>
            {
                var index = 0;
                var serviceList = new List<VmOpenApiServiceCollectionService>();
                vm.ServiceCollectionServices.ForEach(serviceId => serviceList.Add(new VmOpenApiServiceCollectionService
                {
                    Id = serviceId,
                    OwnerReferenceId = serviceCollection.UnificRootId,
                    OrderNumber = ++index
                }));

                index = 0;
                var channelList = new List<VmOpenApiServiceCollectionChannel>();
                vm.ServiceCollectionChannels.ForEach(channelId => channelList.Add(new VmOpenApiServiceCollectionChannel
                {
                    Id = channelId,
                    OwnerReferenceId = serviceCollection.UnificRootId,
                    OrderNumber = ++index
                }));

                var serviceRepo = unitOfWork.CreateRepository<IServiceCollectionServiceRepository>();
                serviceList.ForEach(item =>
                {
                    var entity = TranslationManagerToEntity.Translate<VmOpenApiServiceCollectionService, Model.Models.ServiceCollectionService>(item, unitOfWork);
                    serviceRepo.Add(entity);
                    unitOfWork.Save(saveMode, userName: userName);
                });

                var channelRepo = unitOfWork.CreateRepository<IServiceCollectionServiceChannelRepository>();
                channelList.ForEach(item =>
                {
                    var entity = TranslationManagerToEntity.Translate<VmOpenApiServiceCollectionChannel, Model.Models.ServiceCollectionServiceChannel>(item, unitOfWork);
                    channelRepo.Add(entity);
                    unitOfWork.Save(saveMode, userName: userName);
                });
            });

            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                commonService.PublishAllAvailableLanguageVersions<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(serviceCollection.Id, i => i.ServiceCollectionVersionedId == serviceCollection.Id);
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

                    if (vm.DeleteAllChannels || vm.ServiceCollectionChannels?.Count > 0)
                    {
                        var list = new List<VmOpenApiServiceCollectionChannel>();
                        var serviceCollChannelRep = unitOfWork.CreateRepository<IServiceCollectionServiceChannelRepository>();
                        var index = !vm.DeleteAllChannels 
                            ? serviceCollChannelRep.All().Count(x => x.ServiceCollectionId == serviceCollection.UnificRootId) 
                            : 0;
                        vm.ServiceCollectionChannels.ForEach(channelId => list.Add(new VmOpenApiServiceCollectionChannel
                        {
                            Id = channelId,
                            OwnerReferenceId = vm.Id,
                            OrderNumber = ++index
                        }));
                        var resultb = TranslationManagerToEntity.TranslateAll<VmOpenApiServiceCollectionChannel, Model.Models.ServiceCollectionServiceChannel>(list, unitOfWork);
                        if (vm.DeleteAllChannels)
                        {
                            serviceCollection.UnificRoot.ServiceCollectionServiceChannels = resultb.ToList();
                        }
                        else
                        {
                            resultb.ForEach(item => serviceCollection.UnificRoot.ServiceCollectionServiceChannels.Add(item));
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
                commonService.PublishAllAvailableLanguageVersions<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(serviceCollection.Id, i => i.ServiceCollectionVersionedId == serviceCollection.Id);
            }

            return GetServiceCollectionWithDetails(serviceCollection.Id, openApiVersion, false);
        }

        private IVmOpenApiServiceCollectionVersionBase GetServiceCollectionWithDetails(IUnitOfWork unitOfWork, Guid versionId, int openApiVersion, bool showHeader, bool getOnlyPublished = true)
        {
            return GetServiceCollectionsWithDetails(unitOfWork, new List<Guid> { versionId }, openApiVersion, showHeader, getOnlyPublished).FirstOrDefault();
        }

        private IVmOpenApiServiceCollectionVersionBase GetServiceCollectionWithDetails(Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiServiceCollectionVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetServiceCollectionsWithDetails(unitOfWork, new List<Guid> { versionId }, openApiVersion,  getOnlyPublished).FirstOrDefault();
            });
            return result;
        }
        
        private void FillServiceCollectionNames(List<ServiceCollectionVersioned> serviceCollections, IUnitOfWork unitOfWork)
        {
            var repository = unitOfWork.CreateRepository<IServiceCollectionNameRepository>();
            var serviceCollectionIds = serviceCollections.Select(x => x.Id).Distinct().ToList();

            var all = repository.All()
                .Where(x => serviceCollectionIds.Contains(x.ServiceCollectionVersionedId))
                .ToList();

            foreach (var serviceCollection in serviceCollections)
            {
                var my = all.Where(x => x.ServiceCollectionVersionedId == serviceCollection.Id).ToList();
                if (my.Any())
                {
                    serviceCollection.ServiceCollectionNames = my;
                }
            }
        }

        private void FillServiceCollectionDescriptions(List<ServiceCollectionVersioned> serviceCollections, IUnitOfWork unitOfWork)
        {
            var repository = unitOfWork.CreateRepository<IServiceCollectionDescriptionRepository>();
            var serviceCollectionIds = serviceCollections.Select(x => x.Id).Distinct().ToList();

            var all = repository.All()
                .Where(x => serviceCollectionIds.Contains(x.ServiceCollectionVersionedId))
                .ToList();

            foreach (var serviceCollection in serviceCollections)
            {
                var my = all.Where(x => x.ServiceCollectionVersionedId == serviceCollection.Id).ToList();
                if (my.Any())
                {
                    serviceCollection.ServiceCollectionDescriptions = my;
                }
            }
        }        

        private IList<IVmOpenApiServiceCollectionVersionBase> GetServiceCollectionsWithDetails(IUnitOfWork unitOfWork, List<Guid> versionIdList, int openApiVersion, bool showHeader, bool getOnlyPublished = true)
        {
            if (versionIdList.Count == 0) return new List<IVmOpenApiServiceCollectionVersionBase>();

            var serviceCollectionRep = unitOfWork.CreateRepository<IServiceCollectionVersionedRepository>();
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var channelRepo = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();

            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var resultTemp = unitOfWork.ApplyIncludes(
                serviceCollectionRep.All().Where(s => versionIdList.Contains(s.Id)), q =>
                        q.Include(i => i.UnificRoot).ThenInclude(i => i.ServiceCollectionServices)
                        .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceCollectionServiceChannels)
                        .Include(i => i.LanguageAvailabilities));
            
            // Filter out items that do not have language versions published!
            var temp = getOnlyPublished 
                ? resultTemp.Where(c => c.LanguageAvailabilities.Any(l => l.StatusId == publishedId)).ToList() 
                : resultTemp.ToList();
            
            var serviceCollections = temp.OrderByDescending(x => x.Modified).ToList();
            
            // Fetch names and descriptions separately because EF generates additional
            // order by columns that slow down the query. See:
            // https://stackoverflow.com/questions/25504754/prevent-entity-framework-adding-order-by-when-using-include
            // With EF5 we could perhaps use AsSplitQuery
            FillServiceCollectionNames(serviceCollections, unitOfWork);
            FillServiceCollectionDescriptions(serviceCollections, unitOfWork);
            
            var serviceIds = serviceCollections
                .SelectMany(i => i.UnificRoot.ServiceCollectionServices)
                .Select(i => i.ServiceId)
                .ToList();
            var services = serviceRep.All()
                .Where(s => serviceIds.Contains(s.UnificRootId) && s.PublishingStatusId == publishedId)
                .Include(h => h.ServiceNames)
                .Include(h => h.ServiceDescriptions)
                .Include(h => h.LanguageAvailabilities)
                .Include(h => h.UnificRoot)
                .ToList();
            
            var channelIds = serviceCollections
                .SelectMany(i => i.UnificRoot.ServiceCollectionServiceChannels)
                .Select(i => i.ServiceChannelId)
                .ToList();
            var channels = channelRepo.All()
                .Where(s => channelIds.Contains(s.UnificRootId) && s.PublishingStatusId == publishedId)
                .Include(x => x.Type)
                .Include(h => h.ServiceChannelNames)
                .Include(h => h.ServiceChannelDescriptions)
                .Include(h => h.LanguageAvailabilities)
                .Include(h => h.UnificRoot)
                .ToList();

            serviceCollections.SelectMany(i => i.UnificRoot.ServiceCollectionServices)
                .ForEach(srvCol => { srvCol.Service = services.FirstOrDefault(i => i.UnificRootId == srvCol.ServiceId)?.UnificRoot; });
            serviceCollections.SelectMany(i => i.UnificRoot.ServiceCollectionServiceChannels)
                .ForEach(srvCol => { srvCol.ServiceChannel = channels.FirstOrDefault(i => i.UnificRootId == srvCol.ServiceChannelId)?.UnificRoot; });

            // Find only published services for service collections
//            var publishedServices = serviceCollections.SelectMany(i => i.UnificRoot.ServiceCollectionServices).Select(i => i.Service).SelectMany(i => i.Versions)
//                .Where(i => i.PublishingStatusId == publishedId).ToList();
            var publishedServiceRootIds = services.Select(i => i.UnificRootId).Distinct().ToList();
            var publishedChannelRootIds = channels.Select(i => i.UnificRootId).Distinct().ToList();

            serviceCollections.ForEach(serviceCollection =>
            {
                // Filter out not published services
                serviceCollection.UnificRoot.ServiceCollectionServices = serviceCollection.UnificRoot.ServiceCollectionServices
                        .Where(c => publishedServiceRootIds.Contains(c.ServiceId))
                        .ToList();
                
                serviceCollection.UnificRoot.ServiceCollectionServiceChannels = serviceCollection.UnificRoot.ServiceCollectionServiceChannels
                    .Where(c => publishedChannelRootIds.Contains(c.ServiceChannelId))
                    .ToList();

                // Filter out not published language versions
                if (getOnlyPublished)
                {
                    var notPublishedLanguageVersions = serviceCollection.LanguageAvailabilities
                        .Where(l => l.StatusId != publishedId)
                        .Select(l => l.LanguageId)
                        .ToList();
                    
                    if (notPublishedLanguageVersions.Count > 0)
                    {
                        serviceCollection.ServiceCollectionNames = serviceCollection.ServiceCollectionNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        serviceCollection.ServiceCollectionDescriptions = serviceCollection.ServiceCollectionDescriptions.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    }
                }
            });

            // Filter out name and description language versions that are not available
            services.ForEach(s =>
            {
                var availableLanguages = s.LanguageAvailabilities.Where(i => i.StatusId == publishedId).Select(i => i.LanguageId).ToList();
                var nameType = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
                var descriptionType = typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString());
                s.ServiceNames = s.ServiceNames.Where(n => n.TypeId == nameType && availableLanguages.Contains(n.LocalizationId)).ToList();
                s.ServiceDescriptions = s.ServiceDescriptions.Where(n => n.TypeId == descriptionType && availableLanguages.Contains(n.LocalizationId)).ToList();
            });
            
            channels.ForEach(c =>
            {
                var availableLanguages = c.LanguageAvailabilities.Where(i => i.StatusId == publishedId).Select(i => i.LanguageId).ToList();
                var nameType = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
                var descriptionType = typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString());
                c.ServiceChannelNames = c.ServiceChannelNames.Where(n => n.TypeId == nameType && availableLanguages.Contains(n.LocalizationId)).ToList();
                c.ServiceChannelDescriptions = c.ServiceChannelDescriptions.Where(n => n.TypeId == descriptionType && availableLanguages.Contains(n.LocalizationId)).ToList();
            });

            TranslationManagerToVm.SetValue(openApiVersion, showHeader);
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
    }
}
