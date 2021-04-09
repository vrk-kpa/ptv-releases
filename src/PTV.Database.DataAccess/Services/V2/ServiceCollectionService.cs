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
using System.Linq;
using IServiceCollectionService = PTV.Database.DataAccess.Interfaces.Services.V2.IServiceCollectionService;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.ServiceCollection;
using PTV.Framework;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework.Interfaces;
using ServiceCollectionServiceModel = PTV.Database.Model.Models.ServiceCollectionService;

namespace PTV.Database.DataAccess.Services.V2
{
    [Framework.RegisterService(typeof(IServiceCollectionService), RegisterType.Transient)]
    [Framework.RegisterService(typeof(IServiceCollectionServiceInternal), RegisterType.Transient)]
    internal class ServiceCollectionService : 
        EntityServiceBase<ServiceCollectionVersioned, 
            ServiceCollection, 
            ServiceCollectionLanguageAvailability>, 
        IServiceCollectionServiceInternal
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly ICommonServiceInternal commonService;

        private readonly IServiceUtilities utilities;
        private readonly ILanguageOrderCache languageOrderCache;
        private readonly ISearchServiceInternal searchService;

        public ServiceCollectionService(
            IContextManager contextManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IServiceUtilities utilities,
            ICommonServiceInternal commonService,
            ICacheManager cacheManager,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IValidationManager validationManager,
            IVersioningManager versioningManager,
            ISearchServiceInternal searchService) :
            base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker, contextManager, utilities, commonService, validationManager, versioningManager)
        {
            this.typesCache = cacheManager.TypesCache;
            this.languageCache = cacheManager.LanguageCache;
            this.utilities = utilities;
            this.languageOrderCache = cacheManager.LanguageOrderCache;
            this.commonService = commonService;
            this.searchService = searchService;
        }

        public VmServiceCollectionHeader GetServiceCollectionHeader(Guid? serviceCollectionId)
        {
            return ContextManager.ExecuteReader(unitOfWork => GetServiceCollectionHeader(serviceCollectionId, unitOfWork));
        }

        public VmServiceCollectionHeader GetServiceCollectionHeader(Guid? serviceCollectionId, IUnitOfWork unitOfWork)
        {
            ServiceCollectionVersioned entity;
            var result = GetModel<ServiceCollectionVersioned, VmServiceCollectionHeader>(entity = GetEntity<ServiceCollectionVersioned>(serviceCollectionId, unitOfWork,
                q => q
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x => x.ServiceCollectionNames)
                    .Include(x => x.Versioning)
            ), unitOfWork);
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var connectedServicesRepo = unitOfWork.CreateRepository<IServiceCollectionServiceRepository>();
            var connectedChannelsRepo = unitOfWork.CreateRepository<IServiceCollectionServiceChannelRepository>();
            result.NumberOfServices = connectedServicesRepo.All()
                .Count(x => x.ServiceCollectionId == entity.UnificRootId &&
                            x.Service.Versions.Any(o =>
                                o.PublishingStatusId == draftStatusId ||
                                o.PublishingStatusId == publishingStatusId
                            ));
            result.NumberOfChannels = connectedChannelsRepo.All()
                .Count(x => x.ServiceCollectionId == entity.UnificRootId &&
                            x.ServiceChannel.Versions.Any(o =>
                                o.PublishingStatusId == draftStatusId ||
                                o.PublishingStatusId == publishingStatusId
                            ));

            result.PreviousInfo = serviceCollectionId.HasValue 
                ? Utilities.GetEntityEditableInfo<ServiceCollectionVersioned, ServiceCollection, ServiceCollectionLanguageAvailability>(serviceCollectionId.Value, unitOfWork) 
                : null;
            return result;
        }



        public VmServiceCollectionOutput GetServiceCollection(VmServiceCollectionBasic model)
        {
            return ExecuteGet(model, (unitOfWork, vm) => GetServiceCollection(unitOfWork, model));
        }

//

        private VmServiceCollectionOutput GetServiceCollection(IUnitOfWork unitOfWork, VmServiceCollectionBasic model)
        {
            var entity = GetEntity<ServiceCollectionVersioned>(model.Id, unitOfWork,
                q => q.Include(x => x.ServiceCollectionNames)
                    .Include(x => x.ServiceCollectionDescriptions)
                    .Include(x => x.Organization)
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x => x.Versioning)
                    .Include(x => x.UnificRoot).ThenInclude(j => j.ServiceCollectionServices)
            );
            var result = GetModel<ServiceCollectionVersioned, VmServiceCollectionOutput>(entity, unitOfWork);

            result.Services = GetAllServiceCollectionServices(unitOfWork, entity.UnificRootId);
            result.Channels = GetAllServiceCollectionChannels(unitOfWork, entity.UnificRootId);
            result.PreviousInfo = result.Id.HasValue ? Utilities.GetEntityEditableInfo<ServiceCollectionVersioned, ServiceCollection, ServiceCollectionLanguageAvailability>(result.Id.Value, unitOfWork) : null;
            result.NumberOfServices = result.Services.Count;
            result.NumberOfChannels = result.Channels.Count;
            FillEnumEntities(result, () =>
            {
                var ids = new List<Guid> {entity.OrganizationId};
                return GetEnumEntityCollectionModel("Organizations", CommonService.GetOrganizations(ids));
            });

            return result;
        }

        private Dictionary<Guid, VmListItemWithStatus> GetConnectionOrganizations(IUnitOfWork unitOfWork,
            params Guid[] organizationIds)
        {
            var publishedStatusId = PublishingStatusCache.Get(PublishingStatus.Published);
            var mainNameType = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var organizationRepo = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var organizationNames = organizationRepo.All()
                .Include(x => x.OrganizationNames)
                .Where(x => x.PublishingStatusId == publishedStatusId && organizationIds.Contains(x.UnificRootId))
                .SelectMany(x => x.OrganizationNames
                    .Where(y => y.TypeId == mainNameType)
                    .Select(y => new
                    {
                        x.UnificRootId,
                        x.TypeId,
                        VersionId = x.Id,
                        y.Name,
                        y.LocalizationId
                    }))
                .ToList()
                .GroupBy(x => x.UnificRootId)
                .ToDictionary(x => x.Key, x =>
                {
                    var names = x.ToDictionary(y => languageCache.GetByValue(y.LocalizationId), y => y.Name);
                    var defaultName = names.TryGetOrDefault("fi", names.Values.FirstOrDefault());
                    var (typeId, versionId) = x.Select(y => (y.TypeId, y.VersionId)).FirstOrDefault();
                    return new VmListItemWithStatus
                    {
                        Id = x.Key,
                        Name = defaultName,
                        PublishingStatusId = publishedStatusId,
                        Translation = new VmTranslationItem
                        {
                            DefaultText = defaultName,
                            Id = x.Key,
                            Texts = names,
                        },
                        TypeId = typeId,
                        VersionedId = versionId
                    };
                });

            return organizationNames;
        }

        private List<VmServiceCollectionConnection> GetAllServiceCollectionServices(IUnitOfWork unitOfWork, Guid serviceCollectionUnificRootId)
        {
            var serviceCollectionServiceRep = unitOfWork.CreateRepository<IServiceCollectionServiceRepository>();
            var serviceCollections = serviceCollectionServiceRep.All()
                .Where(x => x.ServiceCollectionId == serviceCollectionUnificRootId)
                .OrderBy(x=>x.OrderNumber)
                .ToList();
            
            var serviceVersionedRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var serviceVersionedIds = serviceCollections.Select(x =>
                VersioningManager.GetLastPublishedDraftVersion<ServiceVersioned>(unitOfWork, x.ServiceId)?.EntityId ??
                VersioningManager.GetLastModifiedVersion<ServiceVersioned>(unitOfWork, x.ServiceId)?.EntityId)
                .Where(x=>x.HasValue)
                .ToList();

            var services = serviceVersionedRep.All()
                .Include(x=>x.ServiceNames)
                .Include(x=>x.LanguageAvailabilities)
                .Where(x => serviceVersionedIds.Contains(x.Id))
                .ToList();
            
            var organizationUnificRootIds = services.Select(x => x.OrganizationId).Distinct().ToArray();
            var organizationNames = GetConnectionOrganizations(unitOfWork, organizationUnificRootIds);
            var serviceUnificRootIds = services.Select(x => x.UnificRootId);

            return serviceCollections.Where(x=>serviceUnificRootIds.Contains(x.ServiceId)).Select(connection =>
            {
                var service = services.Single(x => x.UnificRootId == connection.ServiceId);
                var typeId = !service.StatutoryServiceGeneralDescriptionId.HasValue
                    ? service.TypeId.Value
                    : GetGeneralDesriptionServiceType(service.StatutoryServiceGeneralDescriptionId.Value, unitOfWork);
                var names = organizationNames.TryGetOrDefault(service.OrganizationId);
                
                return new VmServiceCollectionConnection
                {
                    Id = service.Id,
                    UnificRootId = service.UnificRootId,
                    Name = service.ServiceNames
                        .Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()))
                        .ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name),
                    SubEntityType = typesCache.GetByValue<ServiceType>(typeId.Value).ConvertToEnum(SearchEntityTypeEnum.ServiceService),
                    EntityType = SearchEntityTypeEnum.Service,
                    LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            service.LanguageAvailabilities.OrderBy(x => languageOrderCache.Get(x.LanguageId))),
                    Organization = names,
                    OrganizationId = names.Id,
                    Modified = connection.Modified,
                    ModifiedBy = connection.ModifiedBy,
                    ConnectionId = connection.ServiceId.ToString() + connection.ServiceCollectionId.ToString()
                };
            }).ToList();
        }
        
        private List<VmServiceCollectionConnection> GetAllServiceCollectionChannels(IUnitOfWork unitOfWork, Guid serviceCollectionUnificRootId)
        {
            var serviceCollectionChannelRep = unitOfWork.CreateRepository<IServiceCollectionServiceChannelRepository>();
            var serviceCollections = serviceCollectionChannelRep.All()
                .Where(x => x.ServiceCollectionId == serviceCollectionUnificRootId)
                .OrderBy(x=>x.OrderNumber)
                .ToList();
            
            var channelVersionedRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var channelVersionedIds = serviceCollections.Select(x =>
                VersioningManager.GetLastPublishedDraftVersion<ServiceChannelVersioned>(unitOfWork, x.ServiceChannelId)?.EntityId ??
                VersioningManager.GetLastModifiedVersion<ServiceChannelVersioned>(unitOfWork, x.ServiceChannelId)?.EntityId)
                .Where(x=>x.HasValue)
                .ToList();

            var channels = channelVersionedRep.All()
                .Include(x=>x.ServiceChannelNames)
                .Include(x=>x.LanguageAvailabilities)
                .Where(x => channelVersionedIds.Contains(x.Id))
                .ToList();
            
            var organizationUnificRootIds = channels.Select(x => x.OrganizationId).Distinct().ToArray();
            var organizationNames = GetConnectionOrganizations(unitOfWork, organizationUnificRootIds);
            var channelUnificRootIds = channels.Select(x => x.UnificRootId);

            return serviceCollections.Where(x=>channelUnificRootIds.Contains(x.ServiceChannelId)).Select(connection =>
            {
                var channel = channels.Single(x => x.UnificRootId == connection.ServiceChannelId);
                var names = organizationNames.TryGetOrDefault(channel.OrganizationId);
                
                return new VmServiceCollectionConnection
                {
                    Id = channel.Id,
                    UnificRootId = channel.UnificRootId,
                    Name = channel.ServiceChannelNames
                        .Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()))
                        .ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name),
                    SubEntityType = typesCache.GetByValue<ServiceChannelType>(channel.TypeId).ConvertToEnum(SearchEntityTypeEnum.ServiceLocation),
                    EntityType = SearchEntityTypeEnum.Channel,
                    LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            channel.LanguageAvailabilities.OrderBy(x => languageOrderCache.Get(x.LanguageId))),
                    Organization = names,
                    OrganizationId = names.Id,
                    Modified = connection.Modified,
                    ModifiedBy = connection.ModifiedBy,
                    ConnectionId = connection.ServiceChannelId.ToString() + connection.ServiceCollectionId.ToString()
                };
            }).ToList();
        }

        public List<VmServiceCollectionServiceOutput> GetAllServiceRelations(IUnitOfWork unitOfWork, Guid serviceUnificRootId)
        {
            var serviceCollectionServiceRep = unitOfWork.CreateRepository<IServiceCollectionServiceRepository>();
            var serviceCollections = serviceCollectionServiceRep.All().Where(x => x.ServiceId == serviceUnificRootId).OrderBy(x=>x.OrderNumber)
                .ToList();
            var serviceCollectionVersionedRep = unitOfWork.CreateRepository<IServiceCollectionVersionedRepository>();
            var serviceCollectionVersionedIds = serviceCollections.Select(x =>
                VersioningManager.GetLastPublishedDraftVersion<ServiceCollectionVersioned>(unitOfWork, x.ServiceCollectionId)?.EntityId ??
                VersioningManager.GetLastModifiedVersion<ServiceCollectionVersioned>(unitOfWork, x.ServiceCollectionId)?.EntityId).Where(x=>x.HasValue).ToList();

            var connectedCollections = serviceCollectionVersionedRep.All()
                .Include(x=>x.ServiceCollectionNames)
                .Include(x=>x.LanguageAvailabilities)
                .Where(x => serviceCollectionVersionedIds.Contains(x.Id)).ToList();

            var collectionUnificIds = connectedCollections.Select(x => x.UnificRootId);
            return serviceCollections.Where(x=>collectionUnificIds.Contains(x.ServiceCollectionId)).Select(connection =>
            {
                var serviceCollection = connectedCollections.Single(x => x.UnificRootId == connection.ServiceCollectionId);
                return new VmServiceCollectionServiceOutput
                {
                    ConnectionId = connection.ServiceId.ToString() + connection.ServiceCollectionId.ToString(),
                    Id = serviceCollection.Id,
                    UnificRootId = serviceCollection.UnificRootId,
                    Name = serviceCollection.ServiceCollectionNames.Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())).ToDictionary(x => languageCache.GetByValue(x.LocalizationId),
                        x => x.Name),
                    LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            serviceCollection.LanguageAvailabilities.OrderBy(x => languageOrderCache.Get(x.LanguageId))),
                    OrganizationId = serviceCollection.OrganizationId,
                    Modified = connection.Modified.ToEpochTime(),
                    ModifiedBy = connection.ModifiedBy
                };
            }).ToList();
        }
        
        public List<VmServiceCollectionConnection> GetAllChannelRelations(IUnitOfWork unitOfWork, Guid channelUnificRootId, Guid organizationRootId)
        {
            var serviceCollectionChannelRep = unitOfWork.CreateRepository<IServiceCollectionServiceChannelRepository>();
            var serviceCollections = serviceCollectionChannelRep.All()
                .Where(x => x.ServiceChannelId == channelUnificRootId)
                .OrderBy(x=>x.OrderNumber)
                .ToList();
            var serviceCollectionVersionedRep = unitOfWork.CreateRepository<IServiceCollectionVersionedRepository>();
            var serviceCollectionVersionedIds = serviceCollections.Select(x =>
                VersioningManager.GetLastPublishedDraftVersion<ServiceCollectionVersioned>(unitOfWork, x.ServiceCollectionId)?.EntityId ??
                VersioningManager.GetLastModifiedVersion<ServiceCollectionVersioned>(unitOfWork, x.ServiceCollectionId)?.EntityId)
                .Where(x=>x.HasValue)
                .ToList();

            var connectedCollections = serviceCollectionVersionedRep.All()
                .Include(x=>x.ServiceCollectionNames)
                .Include(x=>x.LanguageAvailabilities)
                .Where(x => serviceCollectionVersionedIds.Contains(x.Id))
                .ToList();

            var connectionOrganization = GetConnectionOrganizations(unitOfWork, organizationRootId)
                .FirstOrDefault().Value;

            var collectionUnificIds = connectedCollections.Select(x => x.UnificRootId);
            return serviceCollections.Where(x=>collectionUnificIds.Contains(x.ServiceCollectionId)).Select(connection =>
            {
                var serviceCollection = connectedCollections.Single(x => x.UnificRootId == connection.ServiceCollectionId);
                return new VmServiceCollectionConnection
                {
                    ConnectionId = connection.ServiceChannelId.ToString() + connection.ServiceCollectionId.ToString(),
                    Id = serviceCollection.Id,
                    UnificRootId = serviceCollection.UnificRootId,
                    Name = serviceCollection.ServiceCollectionNames
                        .Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()))
                        .ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name),
                    LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            serviceCollection.LanguageAvailabilities.OrderBy(x => languageOrderCache.Get(x.LanguageId))),
                    Organization = connectionOrganization,
                    OrganizationId = connectionOrganization.Id,
                    Modified = connection.Modified,
                    ModifiedBy = connection.ModifiedBy
                };
            }).ToList();
        }

        private Guid? GetGeneralDesriptionServiceType(Guid id, IUnitOfWork unitOfWork)
        {
            var rep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            var versions = rep.All().Where(x => x.UnificRootId == id);
            return VersioningManager.ApplyPublishingStatusFilterFallback(versions)?.TypeId;
        }

        public VmServiceCollectionOutput SaveServiceCollection(VmServiceCollectionBase model)
        {
            return ExecuteSave
            (
                model,
                unitOfWork => SaveServiceCollection(unitOfWork, model),
                (unitOfWork, entity) => GetServiceCollection(unitOfWork, new VmServiceCollectionBasic {Id = entity.Id})
            );
        }

        private ServiceCollectionVersioned SaveServiceCollection(IUnitOfWorkWritable unitOfWork, VmServiceCollectionBase model)
        {
            var entity = TranslationManagerToEntity.Translate<VmServiceCollectionBase, ServiceCollectionVersioned>(model, unitOfWork);
            commonService.CreateHistoryMetaData<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(entity, model);
            return entity;
        }

        public VmEntityHeaderBase PublishServiceCollection(IVmLocalizedEntityModel model)
        {
            return model.Id.IsAssigned() ? ContextManager.ExecuteWriter(unitOfWork => PublishServiceCollection(unitOfWork, model)) : null;
        }

        private VmEntityHeaderBase PublishServiceCollection(IUnitOfWorkWritable unitOfWork, IVmLocalizedEntityModel model)
        {
            Guid? serviceCollectionId = model.Id;

            //Validate mandatory values
            var validationMessages = ValidationManager.CheckEntity<ServiceCollectionVersioned>(serviceCollectionId.Value, unitOfWork, model);
            if (validationMessages.Any())
            {
                throw new PtvValidationException(validationMessages, null);
            }

            //Publishing
            var affected = CommonService.PublishAndScheduleEntity<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(unitOfWork, model);

            return GetServiceCollectionHeader(affected.Id, unitOfWork);
        }

        public VmEntityHeaderBase ScheduleServiceCollection(IVmLocalizedEntityModel model)
        {
            return ExecuteScheduleEntity(model, (unitOfWork, result) => GetServiceCollectionHeader(result.Id, unitOfWork));
        }

        public VmServiceCollectionHeader RemoveServiceCollection(Guid serviceCollectionId)
        {
            return ExecuteRemove(serviceCollectionId, GetServiceCollectionHeader);
        }

        public VmServiceCollectionHeader DeleteServiceCollection(Guid serviceCollectionId)
        {
            return ExecuteDelete(serviceCollectionId, GetServiceCollectionHeader);
//            VmServiceCollectionHeader result = null;
//            ContextManager.ExecuteWriter(unitOfWork =>
//            {
//                var deletedServiceCollection = DeleteServiceCollection(unitOfWork, serviceCollectionId);
//                unitOfWork.Save();
//                result = GetServiceCollectionHeader(deletedServiceCollection.Id, unitOfWork);
//
//            });
//            UnLockServiceCollection(result.Id.Value);
//            return result;
        }

//        private ServiceCollectionVersioned DeleteServiceCollection(IUnitOfWorkWritable unitOfWork, Guid? serviceCollectionId)
//        {
//            return CommonService.ChangeEntityToDeleted<ServiceCollectionVersioned>(unitOfWork, serviceCollectionId.Value);
//        }

        public IVmEntityBase LockServiceCollection(Guid id, bool isLockDisAllowedForArchived = false)
        {
            return Utilities.LockEntityVersioned<ServiceCollectionVersioned, ServiceCollection>(id, isLockDisAllowedForArchived);
        }

        public IVmEntityBase UnLockServiceCollection(Guid id)
        {
            return Utilities.UnLockEntityVersioned<ServiceCollectionVersioned, ServiceCollection>(id);
        }

        public VmServiceCollectionHeader WithdrawServiceCollection(Guid serviceId)
        {
            return ExecuteWithdraw(serviceId, GetServiceCollectionHeader);
//            var result = CommonService.WithdrawEntity<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(serviceId);
//            UnLockServiceCollection(result.Id.Value);
//            return GetServiceCollectionHeader(result.Id);
        }

        public VmServiceCollectionHeader RestoreServiceCollection(Guid serviceCollectionId)
        {
            return ExecuteRestore(serviceCollectionId, GetServiceCollectionHeader);
//            var result = CommonService.RestoreEntity<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(serviceCollectionId);
//            UnLockServiceCollection(result.Id.Value);
//            return GetServiceCollectionHeader(result.Id);
        }

        public VmServiceCollectionHeader ArchiveLanguage(VmEntityBasic model)
        {
            return ExecuteArchiveLanguage(model, GetServiceCollectionHeader);
//            var entity = CommonService.ArchiveLanguage<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(model);
//            UnLockServiceCollection(entity.Id);
//            return GetServiceCollectionHeader(entity.Id);
        }

        public VmServiceCollectionHeader RestoreLanguage(VmEntityBasic model)
        {
            return ExecuteRestoreLanguage(model, GetServiceCollectionHeader);
//            var entity = CommonService.RestoreLanguage<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(model);
//            UnLockServiceCollection(entity.Id);
//            return GetServiceCollectionHeader(entity.Id);
        }

        public VmServiceCollectionHeader WithdrawLanguage(VmEntityBasic model)
        {
            return ExecuteWithdrawLanguage(model, GetServiceCollectionHeader);
//            var entity = CommonService.WithdrawLanguage<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(model);
//            UnLockServiceCollection(entity.Id);
//            return GetServiceCollectionHeader(entity.Id);
        }

        public VmServiceCollectionHeader GetValidatedEntity(VmEntityBasic model)
        {
            return ExecuteValidate
            (
                () => Utilities.LockEntityVersioned<ServiceCollectionVersioned, ServiceCollection>(model.Id.Value, true),
                (unitOfWork) => GetServiceCollectionHeader(model.Id, unitOfWork)
            );
        }

        private void ProcessNewServiceSetConnections(VmServiceCollectionConnectionsInput modelInput)
        {
            ContextManager.ExecuteReader(unitOfWork =>
            {
                var unificRootId = VersioningManager
                    .GetUnificRootId<ServiceCollectionVersioned>(unitOfWork, modelInput.Id);
                if (unificRootId.HasValue)
                {
                    modelInput.UnificRootId = unificRootId.Value;
                }
            });
            if (!modelInput.UnificRootId.IsAssigned()) return;

            var order = 1;
            var serviceInputs = modelInput.SelectedServices.Select(x => new ServiceCollectionServiceModel
            {
                OrderNumber = order++,
                ServiceId = x.UnificRootId,
                ServiceCollectionId = modelInput.UnificRootId
            });
            order = 1;
            var channelInputs = modelInput.SelectedChannels.Select(x => new ServiceCollectionServiceChannel
            {
                OrderNumber = order++,
                ServiceChannelId = x.UnificRootId,
                ServiceCollectionId = modelInput.UnificRootId
            });
            
            ContextManager.ExecuteWriter(unitOfWork =>
            {
                utilities.CheckIsEntityConnectable<ServiceCollectionVersioned>(modelInput.Id, unitOfWork);
                
                // Remove all old connections
                var serviceCollectionServiceRepo = unitOfWork.CreateRepository<IServiceCollectionServiceRepository>();
                var serviceCollectionChannelRepo = unitOfWork.CreateRepository<IServiceCollectionServiceChannelRepository>();
                serviceCollectionServiceRepo.Remove(serviceCollectionServiceRepo.All().Where(x => x.ServiceCollectionId == modelInput.UnificRootId));
                serviceCollectionChannelRepo.Remove(serviceCollectionChannelRepo.All().Where(x => x.ServiceCollectionId == modelInput.UnificRootId));
                unitOfWork.Save();
            });
            
            ContextManager.ExecuteWriter(unitOfWork =>
            {
                // Add old kept and new added
                var serviceCollectionServiceRepo = unitOfWork.CreateRepository<IServiceCollectionServiceRepository>();
                var serviceCollectionChannelRepo = unitOfWork.CreateRepository<IServiceCollectionServiceChannelRepository>();
                foreach (var serviceInput in serviceInputs)
                {
                    serviceCollectionServiceRepo.Add(serviceInput);
                }
                foreach (var channelInput in channelInputs)
                {
                    serviceCollectionChannelRepo.Add(channelInput);
                }
                unitOfWork.Save();
            });
        }

        public VmServiceCollectionConnectionsOutput SaveRelations(VmServiceCollectionConnectionsInput model)
        {
            ProcessNewServiceSetConnections(model);
            // ContextManager.ExecuteWriter(unitOfWork =>
            // {
            //     //utilities.CheckIsEntityConnectable<ServiceCollectionVersioned>(model.Id, unitOfWork);
            //     SaveRelations(unitOfWork, model);
            //     unitOfWork.Save();
            // });
            return GetRelations(model);
        }

        // private void SaveRelations(IUnitOfWorkWritable unitOfWork, VmServiceCollectionConnectionsInput model)
        // {
        //     var unificRootId = VersioningManager.GetUnificRootId<ServiceCollectionVersioned>(unitOfWork, model.Id);
        //     if (unificRootId.HasValue)
        //     {
        //         model.UnificRootId = unificRootId.Value;
        //         TranslationManagerToEntity.Translate<VmServiceCollectionConnectionsInput, ServiceCollection>(model, unitOfWork);
        //     }
        // }

        public IVmEntityBase IsConnectable(Guid id)
        {
            ContextManager.ExecuteWriter(unitOfWork =>
            {
                utilities.CheckIsEntityConnectable<ServiceCollectionVersioned>(id, unitOfWork);
            });
            return null;
        }

        public IVmBase GetConnectableContent(VmConnectableContentSearch search)
        {
            search.Name = search.Name?.Trim();

            if (search.SortData == null || !search.SortData.Any())
            {
                search.SortData = new List<VmSortParam>
                {
                    new VmSortParam {Column = "Modified", Order = 2, SortDirection = SortDirectionEnum.Desc},
                    new VmSortParam {Column = "Id", Order = 3, SortDirection = SortDirectionEnum.Desc}
                };
            }
            
            var publishedStatusId = PublishingStatusCache.Get(PublishingStatus.Published);
            var draftStatusId = PublishingStatusCache.Get(PublishingStatus.Draft);
            var languagesIds = new List<Guid> {languageCache.Get(search.Language)};

            var vmSearch = new VmEntitySearch
            {
                ContentTypes = GetContentTypes(search.ContentType),
                SearchType = SearchTypeEnum.Name
            };
            
            if (search.OrganizationId.HasValue)
            {
                vmSearch.OrganizationId = search.OrganizationId.Value;
            }
            else
            {
                if (utilities.UserHighestRole() == UserRoleEnum.Shirley)
                {
                    var userOrgs = utilities.GetAllUserOrganizations();
                    if (userOrgs.Any())
                    {
                        vmSearch.OrganizationIds = userOrgs.ToList();
                    }
                }
            }

            if (!string.IsNullOrEmpty(search.Name))
            {
                var rootId = GetRootIdFromString(search.Name);
                if (!rootId.HasValue)
                {
                    vmSearch.Name = search.Name;
                }
                else
                {
                    vmSearch.SearchType = SearchTypeEnum.Id;
                    vmSearch.Id = rootId;
                }
            }

            vmSearch.LanguageIds = languagesIds;
            vmSearch.SelectedPublishingStatuses = new List<Guid> {publishedStatusId, draftStatusId};
            vmSearch.UseOnlySelectedStatuses = true;

            vmSearch.Language = search.Language;
            vmSearch.SortData = search.SortData;
            vmSearch.PageNumber = search.PageNumber.PositiveOrZero();

            var resultEnt = searchService.SearchEntities(vmSearch) as VmSearchResult<IVmEntityListItem>;
            var organizationNames = ContextManager.ExecuteReader(unitOfWork =>
            {
                return GetConnectionOrganizations(unitOfWork, resultEnt.SearchResult
                    .Where(x => x.OrganizationId.HasValue)
                    .Select(x => x.OrganizationId.Value)
                    .ToArray());
            });

            var result = resultEnt?.SearchResult.Select(i => new VmConnectableContent
                {
                    Id = i.Id,
                    UnificRootId = i.UnificRootId,
                    Name = i.Name,
                    EntityType = i.EntityType,
                    SubEntityType = i.SubEntityType,
                    LanguagesAvailabilities = i.LanguagesAvailabilities,
                    Organization = organizationNames.TryGetOrDefault(i.OrganizationId ?? Guid.Empty),
                    OrganizationId = i.OrganizationId,
                    Modified = i.Modified.FromEpochTime(),
                    ModifiedBy = i.ModifiedBy
                })
                .ToList();
            var returnData = new VmSearchResult<VmConnectableContent>
            {
                SearchResult = result,
                MoreAvailable = resultEnt?.MoreAvailable ?? false,
                Count = resultEnt?.Count ?? 0,
                PageNumber = resultEnt?.PageNumber ?? 0,
                EnumCollection = resultEnt?.EnumCollection
            };
            return returnData;
        }

        private static List<SearchEntityTypeEnum> GetContentTypes(SearchEntityTypeEnum? input)
        {
            return input switch
            {
                var nullable when nullable == null || nullable == SearchEntityTypeEnum.All => new List<SearchEntityTypeEnum>
                {
                    SearchEntityTypeEnum.ServiceLocation,
                    SearchEntityTypeEnum.EChannel,
                    SearchEntityTypeEnum.WebPage,
                    SearchEntityTypeEnum.PrintableForm,
                    SearchEntityTypeEnum.Phone,
                    SearchEntityTypeEnum.ServiceService,
                    SearchEntityTypeEnum.ServicePermit,
                    SearchEntityTypeEnum.ServiceProfessional
                },
                SearchEntityTypeEnum.Channel => new List<SearchEntityTypeEnum>
                {
                    SearchEntityTypeEnum.ServiceLocation,
                    SearchEntityTypeEnum.EChannel,
                    SearchEntityTypeEnum.WebPage,
                    SearchEntityTypeEnum.PrintableForm,
                    SearchEntityTypeEnum.Phone
                },
                SearchEntityTypeEnum.Service => new List<SearchEntityTypeEnum>
                {
                    SearchEntityTypeEnum.ServiceService,
                    SearchEntityTypeEnum.ServicePermit,
                    SearchEntityTypeEnum.ServiceProfessional
                },
                _ => new List<SearchEntityTypeEnum> {input.Value}
            };
        }

        private VmServiceCollectionConnectionsOutput GetRelations(VmServiceCollectionConnectionsInput model)
        {
            return ContextManager.ExecuteReader(unitOfWork => GetRelations(unitOfWork, model));
        }

        private VmServiceCollectionConnectionsOutput GetRelations(IUnitOfWork unitOfWork, VmServiceCollectionConnectionsInput model)
        {
            var serviceLangAvailabilitiesRep = unitOfWork.CreateRepository<IServiceCollectionLanguageAvailabilityRepository>();
            var unificRootId = VersioningManager.GetUnificRootId<ServiceCollectionVersioned>(unitOfWork, model.Id);
            if (unificRootId.HasValue)
            {
                var languageAvailabilities = serviceLangAvailabilitiesRep.All()
                    .Where(x => model.Id == x.ServiceCollectionVersionedId)
                    .ToList()
                    .OrderBy(x => languageOrderCache.Get(x.LanguageId))
                    .ToList();
                
                var result = new VmServiceCollectionConnectionsOutput
                {
                    Services = GetAllServiceCollectionServices(unitOfWork, unificRootId.Value),
                    Channels = GetAllServiceCollectionChannels(unitOfWork, unificRootId.Value),
                    Id = model.Id,
                    LanguagesAvailabilities = TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(languageAvailabilities)
                };
                return result;
            }
            return null;
        }
    }
}
