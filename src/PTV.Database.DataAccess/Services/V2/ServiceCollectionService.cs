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
using PTV.Database.DataAccess.Utils;
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

namespace PTV.Database.DataAccess.Services.V2
{
    [Framework.RegisterService(typeof(IServiceCollectionService), RegisterType.Transient)]    
    [Framework.RegisterService(typeof(IServiceCollectionServiceInternal), RegisterType.Transient)]
    internal class ServiceCollectionService : EntityServiceBase<ServiceCollectionVersioned, ServiceCollection, ServiceCollectionLanguageAvailability>, IServiceCollectionServiceInternal
    {
        private IContextManager contextManager;
        
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly IConnectionsServiceInternal connectionsService;
        private readonly ICommonServiceInternal commonService;

        private readonly ITranslationService translationService;
        private readonly XliffParser xliffParser;
        private readonly IServiceUtilities utilities;
        private readonly ILanguageOrderCache languageOrderCache;
        private ITasksServiceInternal tasksService;
        
        public ServiceCollectionService(
            IContextManager contextManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IServiceUtilities utilities,
            ICommonServiceInternal commonService,
            IConnectionsServiceInternal connectionsService,
            ITranslationService translationService,
            ICacheManager cacheManager,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IValidationManager validationManager,
            IVersioningManager versioningManager,
            ITasksServiceInternal tasksService,
            XliffParser xliffParser) :
            base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker, contextManager, utilities, commonService, validationManager, versioningManager)
        {
            this.typesCache = cacheManager.TypesCache;
            this.languageCache = cacheManager.LanguageCache;
            this.translationService = translationService;
            this.xliffParser = xliffParser;
            this.utilities = utilities;
            this.connectionsService = connectionsService;
            this.languageOrderCache = cacheManager.LanguageOrderCache;
            this.tasksService = tasksService;
            this.commonService = commonService;
            this.contextManager = contextManager;
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
            var connRep = unitOfWork.CreateRepository<IServiceCollectionServiceRepository>();
            result.NumberOfConnections = connRep.All()
                .Count(x => x.ServiceCollectionId == entity.UnificRootId &&
                            x.Service.Versions.Any(o =>
                                o.PublishingStatusId == draftStatusId ||
                                o.PublishingStatusId == publishingStatusId
                            ));

            result.PreviousInfo = serviceCollectionId.HasValue ? Utilities.GetEntityEditableInfo<ServiceCollectionVersioned, ServiceCollection, ServiceCollectionLanguageAvailability>(serviceCollectionId.Value, unitOfWork) : null;
            return result;
        }

       

        public VmServiceCollectionOutput GetServiceCollection(VmServiceCollectionBasic model)
        {
            return ExecuteGet(model, (unitOfWork, vm) => GetServiceCollection(unitOfWork, model));
        }

//       
        
        private VmServiceCollectionOutput GetServiceCollection(IUnitOfWork unitOfWork, VmServiceCollectionBasic model)
        {
            VmServiceCollectionOutput result = null;
            ServiceCollectionVersioned entity = null;
            result = GetModel<ServiceCollectionVersioned, VmServiceCollectionOutput>(entity = GetEntity<ServiceCollectionVersioned>(model.Id, unitOfWork,
                q => q.Include(x => x.ServiceCollectionNames)
                    .Include(x => x.ServiceCollectionDescriptions)
                    .Include(x => x.Organization)
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x => x.Versioning)
                    .Include(x => x.UnificRoot).ThenInclude(j => j.ServiceCollectionServices)
            ), unitOfWork);
           
            result.Connections = GetAllServiceCollectionRelations(unitOfWork, entity.UnificRootId);
            result.PreviousInfo = result.Id.HasValue ? Utilities.GetEntityEditableInfo<ServiceCollectionVersioned, ServiceCollection, ServiceCollectionLanguageAvailability>(result.Id.Value, unitOfWork) : null;
            result.NumberOfConnections = result.Connections.Count;
            FillEnumEntities(result, () =>
            {
                var ids = new List<Guid> {entity.OrganizationId};
                return GetEnumEntityCollectionModel("Organizations", CommonService.GetOrganizations(ids));
            });
            var userOrganizations = utilities.GetAllUserOrganizations();
            var expirationInformation =
                tasksService.GetExpirationInformation<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, result.UnificRootId, result.PublishingStatus,userOrganizations);

            if (expirationInformation == null || !expirationInformation.ContainsKey(result.UnificRootId)) return result;
            
            result.ExpireOn = expirationInformation[result.UnificRootId].ExpireOn.ToEpochTime();
            result.IsWarningVisible = expirationInformation[result.UnificRootId].IsWarningVisible;

            
            
            return result;
        }

        private List<VmServiceCollectionConnectionOutput> GetAllServiceCollectionRelations(IUnitOfWork unitOfWork, Guid serviceCollectionUnificRootId)
        {
            var serviceCollectionServiceRep = unitOfWork.CreateRepository<IServiceCollectionServiceRepository>();
            var serviceCollections = serviceCollectionServiceRep.All().Where(x => x.ServiceCollectionId == serviceCollectionUnificRootId).OrderBy(x=>x.OrderNumber)
                .ToList();
            var serviceVersionedRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var serviceVersionedIds = serviceCollections.Select(x =>
                VersioningManager.GetLastPublishedDraftVersion<ServiceVersioned>(unitOfWork, x.ServiceId)?.EntityId ??
                VersioningManager.GetLastModifiedVersion<ServiceVersioned>(unitOfWork, x.ServiceId)?.EntityId).Where(x=>x.HasValue).ToList();

            var services = serviceVersionedRep.All()
                .Include(x=>x.ServiceNames)
                .Include(x=>x.LanguageAvailabilities)
                .Where(x => serviceVersionedIds.Contains(x.Id)).ToList();

            var serviceUnificRootIds = services.Select(x => x.UnificRootId);
            
            return serviceCollections.Where(x=>serviceUnificRootIds.Contains(x.ServiceId)).Select(connection =>
            {
                var service = services.Single(x => x.UnificRootId == connection.ServiceId);
                var typeId = !service.StatutoryServiceGeneralDescriptionId.HasValue
                    ? service.TypeId.Value
                    : GetGeneralDesriptionServiceType(service.StatutoryServiceGeneralDescriptionId.Value, unitOfWork);
                return new VmServiceCollectionConnectionOutput
                {
                    ConnectionId = connection.ServiceCollectionId.ToString() + connection.ServiceId.ToString(),
                    Id = service.Id,
                    UnificRootId = service.UnificRootId,
                    Name = service.ServiceNames.Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())).ToDictionary(x => languageCache.GetByValue(x.LocalizationId),
                        x => x.Name),
                    ServiceType = typeId,
                    LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            service.LanguageAvailabilities.OrderBy(x => languageOrderCache.Get(x.LanguageId))),
                    OrganizationId = service.OrganizationId,
                    Modified = connection.Modified.ToEpochTime(),
                    ModifiedBy = connection.ModifiedBy
                };
            }).ToList();
        }
        
        public List<VmServiceCollectionConnectionOutput> GetAllServiceRelations(IUnitOfWork unitOfWork, Guid serviceUnificRootId)
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
                return new VmServiceCollectionConnectionOutput
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
                (unitOfWork, entity) => GetServiceCollection(unitOfWork, new VmServiceCollectionBasic() {Id = entity.Id})
            );
        }

        private ServiceCollectionVersioned SaveServiceCollection(IUnitOfWorkWritable unitOfWork, VmServiceCollectionBase model)
        {
            var entity = TranslationManagerToEntity.Translate<VmServiceCollectionBase, ServiceCollectionVersioned>(model, unitOfWork);
            commonService.AddHistoryMetaData<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(entity, model);
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
        
        public VmServiceCollectionConnectionsOutput SaveRelations(VmConnectionsInput model)
        {
            ContextManager.ExecuteWriter(unitOfWork =>
            {
                utilities.CheckIsEntityConnectable<ServiceCollectionVersioned>(model.Id, unitOfWork);
                SaveRelations(unitOfWork, model);
                unitOfWork.Save();
            });
            return GetRelations(model);
        }

        private void SaveRelations(IUnitOfWorkWritable unitOfWork, VmConnectionsInput model)
        {
            var unificRootId = VersioningManager.GetUnificRootId<ServiceCollectionVersioned>(unitOfWork, model.Id);
            if (unificRootId.HasValue)
            {
                model.UnificRootId = unificRootId.Value;
                TranslationManagerToEntity.Translate<VmConnectionsInput, ServiceCollection>(model, unitOfWork);
            }
        }

        public IVmEntityBase IsConnectable(Guid id)
        {
            ContextManager.ExecuteWriter(unitOfWork =>
            {
                utilities.CheckIsEntityConnectable<ServiceCollectionVersioned>(id, unitOfWork);                
            });
            return null;
        }

        private VmServiceCollectionConnectionsOutput GetRelations(VmConnectionsInput model)
        {
            return ContextManager.ExecuteReader(unitOfWork => GetRelations(unitOfWork, model));
        }
               
        private VmServiceCollectionConnectionsOutput GetRelations(IUnitOfWork unitOfWork, VmConnectionsInput model)
        {
            var serviceLangAvailabilitiesRep = unitOfWork.CreateRepository<IServiceCollectionLanguageAvailabilityRepository>();
            var unificRootId = VersioningManager.GetUnificRootId<ServiceCollectionVersioned>(unitOfWork, model.Id);
            if (unificRootId.HasValue)
            {
                //var relations = GetAllServiceCollectionRelations(unitOfWork, unificRootId.Value );
                var result = new VmServiceCollectionConnectionsOutput()
                {
                    Connections = GetAllServiceCollectionRelations(unitOfWork, unificRootId.Value),
                    Id = model.Id,
                    LanguagesAvailabilities = TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(serviceLangAvailabilitiesRep.All().Where(x => model.Id == x.ServiceCollectionVersionedId).OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList())
                };
                return result;
            }
            return null;
        }            
    }
}
