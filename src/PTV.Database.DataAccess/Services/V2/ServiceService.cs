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
using IServiceService = PTV.Database.DataAccess.Interfaces.Services.V2.IServiceService;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
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
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Framework;
using PTV.Domain.Model.Models.V2.Connections;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Framework.Exceptions.DataAccess;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services.V2
{
    [RegisterService(typeof(IServiceService), RegisterType.Transient)]
    internal class ServiceService : EntityServiceBase<ServiceVersioned, Service, ServiceLanguageAvailability>,
        IServiceService
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly IGeneralDescriptionService generalDescriptionService;
        private readonly IConnectionsServiceInternal connectionsService;
        private readonly IServiceCollectionServiceInternal serviceCollectionService;
        private readonly ICommonServiceInternal commonService;
        private readonly ITranslationService translationService;
        private readonly XliffParser xliffParser;
        private readonly IServiceUtilities utilities;
        private readonly IPahaTokenAccessor pahaTokenAccessor;
        private readonly IUserOrganizationService userOrganizationService;
        private readonly IServiceServiceInternal serviceInternal;
        private readonly ISearchServiceInternal searchService;
        private readonly IUrlService urlService;
        private readonly IExpirationService expirationService;

        public ServiceService(
            IContextManager contextManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IServiceUtilities utilities,
            ICommonServiceInternal commonService,
            IGeneralDescriptionService generalDescriptionService,
            IConnectionsServiceInternal connectionsService,
            IServiceCollectionServiceInternal serviceCollectionService,
            ITranslationService translationService,
            ICacheManager cacheManager,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IValidationManager validationManager,
            IVersioningManager versioningManager,
            XliffParser xliffParser,
            IPahaTokenAccessor pahaTokenAccessor,
            IUserOrganizationService userOrganizationService,
            IServiceServiceInternal serviceInternal,
            ISearchServiceInternal searchService,
            IUrlService urlService,
            IExpirationService expirationService) :
            base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker,
                contextManager, utilities, commonService, validationManager, versioningManager)
        {
            this.generalDescriptionService = generalDescriptionService;
            this.typesCache = cacheManager.TypesCache;
            this.languageCache = cacheManager.LanguageCache;
            this.translationService = translationService;
            this.xliffParser = xliffParser;
            this.utilities = utilities;
            this.connectionsService = connectionsService;
            this.serviceCollectionService = serviceCollectionService;
            this.commonService = commonService;
            this.pahaTokenAccessor = pahaTokenAccessor;
            this.userOrganizationService = userOrganizationService;
            this.serviceInternal = serviceInternal;
            this.searchService = searchService;
            this.urlService = urlService;
            this.expirationService = expirationService;
        }

        public Guid? SaveServiceSimple(VmServiceInput model)
        {
            return ExecuteSaveSimple
            (
                model,
                unitOfWork => SaveService(unitOfWork, model)
            );
        }

        public VmServiceHeader GetServiceHeader(Guid? serviceId)
        {
            return ContextManager.ExecuteReader(unitOfWork => GetServiceHeader(serviceId, unitOfWork));
        }

        private VmServiceHeader GetServiceHeader(Guid? serviceId, IUnitOfWork unitOfWork)
        {
            ServiceVersioned entity;
            var result = GetModel<ServiceVersioned, VmServiceHeader>(entity = GetEntity<ServiceVersioned>(serviceId,
                unitOfWork,
                q => q
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x => x.ServiceNames)
                    .Include(x => x.Versioning)
            ), unitOfWork);
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var connRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            result.NumberOfConnections = connRep.All()
                .Count(x => x.ServiceId == entity.UnificRootId &&
                            x.ServiceChannel.Versions.Any(o =>
                                o.PublishingStatusId == draftStatusId ||
                                o.PublishingStatusId == publishingStatusId
                            ));

            result.PreviousInfo = serviceId.HasValue
                ? Utilities.GetEntityEditableInfo<ServiceVersioned, Service, ServiceLanguageAvailability>(
                    serviceId.Value, unitOfWork)
                : null;
            if (serviceId.HasValue)
            {
                var unificRootId = VersioningManager.GetUnificRootId<ServiceVersioned>(unitOfWork, serviceId.Value);
                if (unificRootId.HasValue)
                {
                    result.TranslationAvailability =
                        translationService.GetServiceTranslationAvailabilities(unitOfWork, serviceId.Value,
                            unificRootId.Value);
                }

                UpdateExpiration(result, unitOfWork, entity);
            }

            return result;
        }

        public VmKeywordSearchOutput KeywordSearch(VmKeywordSearch model)
        {
            return ContextManager.ExecuteReader(unitOfWork => KeywordSearch(model, unitOfWork));
        }

        public VmKeywordSearchOutput KeywordSearch(VmKeywordSearch model, IUnitOfWork unitOfWork)
        {
            var localizationId = languageCache.Get(model.LocalizationCode);
            var keywordRep = unitOfWork.CreateRepository<IKeywordRepository>();
            var name = model.Name.ToLower();
            var keywords = keywordRep.All()
                .Where(i => i.Name.ToLower().Contains(name) && i.LocalizationId == localizationId).OrderBy(i => i.Name)
                .Take(CoreConstants.MaximumNumberOfAllItems + 1).ToList();
            return new VmKeywordSearchOutput
            {
                Keywords = TranslationManagerToVm.TranslateAll<Keyword, VmKeywordItem>(
                    keywords.Take(CoreConstants.MaximumNumberOfAllItems)),
                MoreAvailable = keywords.Count > CoreConstants.MaximumNumberOfAllItems
            };
        }

        public VmServiceOutput GetService(VmServiceBasic model)
        {
            return ExecuteGet(model, (unitOfWork, _) => GetService(model, unitOfWork));
        }

        private VmServiceOutput GetService(VmServiceBasic model, IUnitOfWork unitOfWork)
        {
            var entity = GetEntity<ServiceVersioned>(model.Id,
                unitOfWork,
                q => q.Include(x => x.ServiceNames)
                    .Include(x => x.ServiceDescriptions)
                    .Include(x => x.OrganizationServices)
                    .Include(x => x.ServiceRequirements)
                    .Include(x => x.AreaMunicipalities)
                    .Include(x => x.Areas).ThenInclude(x => x.Area)
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x => x.Versioning)
                    .Include(x => x.ServiceLanguages)
                    .Include(x => x.ServiceWebPages).ThenInclude(x => x.WebPage)
                    .Include(x => x.ServiceWebPages).ThenInclude(x => x.Localization)
            );
            if (model.IncludeConnections)
            {
                IncludeConnections(unitOfWork, entity);
            }
            IncludeClassification(unitOfWork, entity);
            IncludeLegislation(unitOfWork, entity);

            var result = GetModel<ServiceVersioned, VmServiceOutput>(entity, unitOfWork);

            if (model.IncludeConnections)
            {
                AddConnectionsInfo(unitOfWork, entity, result);
            }
            
            result.PreviousInfo = result.Id.HasValue
                ? Utilities.GetEntityEditableInfo<ServiceVersioned, Service, ServiceLanguageAvailability>(
                    result.Id.Value, unitOfWork)
                : null;
            result.TranslationAvailability =
                translationService.GetServiceTranslationAvailabilities(unitOfWork, result.Id.Value,
                    result.UnificRootId);

            if (result.GeneralDescriptionId != null)
            {
                result.GeneralDescriptionOutput =
                    generalDescriptionService.GetGeneralDescription(unitOfWork, new VmGeneralDescriptionGet
                    {
                        Id = result.GeneralDescriptionId,
                        OnlyPublished = true
                    });

                result.GeneralDescriptionOutput.Id = result.GeneralDescriptionOutput.UnificRootId;
            }

            FillEnumEntities(result, 
                () => {
                    var ids = new List<Guid>(result.ResponsibleOrganizations);
                    ids.AddRange(
                        entity.ServiceProducers.SelectMany(p => p.Organizations.Select(org => org.OrganizationId)));
                    ids.Add(entity.OrganizationId);
                    ids.AddRange(result.Connections?.Where(c => c.OrganizationId.HasValue)?
                        .Select(c => c.OrganizationId.Value) ?? Array.Empty<Guid>());
                    return GetEnumEntityCollectionModel("Organizations", CommonService.GetOrganizations(ids));
                },
                () => GetEnumEntityCollectionModel("OrganizationAreaInformations", new List<IVmAreaInformation>
                {
                    commonService.GetAreaInformationForOrganization(new VmGetAreaInformation()
                        {OrganizationId = result.Organization.Value})
                    
                }
                ));

            UpdateExpiration(result, unitOfWork, entity);
            return result;
        }

        private void AddConnectionsInfo(IUnitOfWork unitOfWork, ServiceVersioned entity, VmServiceOutput result)
        {
            var organizationIds = userOrganizationService.GetAllUserOrganizationIds();
            var relations = connectionsService.GetAllServiceRelations(unitOfWork, new List<Guid> {entity.UnificRootId});
            result.Connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, VmConnectionOutput>(
                    relations.TryGetOrDefault(entity.UnificRootId, new List<ServiceServiceChannel>()))
                .OrderBy(c => c.ChannelOrderNumber)
                .ThenBy(c => c.ConnectionOrderNumber)
                .ToList();
            result.NumberOfConnections = result.Connections.Count;
            result.ServiceCollections =
                serviceCollectionService.GetAllServiceRelations(unitOfWork, entity.UnificRootId);
            // Display disconnected channel notifications only to relevant users
            result.DisconnectedConnections = organizationIds.Contains(entity.OrganizationId)
                ? GetDisconnectedChannels(unitOfWork, result.UnificRootId)
                : new List<VmDisconnectedChannel>();
        }

        private void IncludeConnections(IUnitOfWork unitOfWork, ServiceVersioned entity)
        {
            if (unitOfWork == null || entity == null)
            {
                return;
            }

            var serviceRepo = unitOfWork.CreateRepository<IServiceRepository>();
            var unificRoot = serviceRepo.All()
                .Include(j => j.ServiceServiceChannels)
                .FirstOrDefault(j => j.Id == entity.UnificRootId);

            entity.UnificRoot = unificRoot;
        }

        private void IncludeLegislation(IUnitOfWork unitOfWork, ServiceVersioned entity)
        {
            if (unitOfWork == null || entity == null)
            {
                return;
            }

            var serviceLawRepo = unitOfWork.CreateRepository<IServiceLawRepository>();
            var serviceLaws = serviceLawRepo.All()
                .Include(j => j.Law).ThenInclude(k => k.Names)
                .Include(j => j.Law).ThenInclude(k => k.WebPages).ThenInclude(l => l.Localization)
                .Include(j => j.Law).ThenInclude(k => k.WebPages).ThenInclude(l => l.WebPage)
                .Where(j => j.ServiceVersionedId == entity.Id)
                .ToList();

            entity.ServiceLaws = serviceLaws;
        }

        private void IncludeClassification(IUnitOfWork unitOfWork, ServiceVersioned entity)
        {
            if (unitOfWork == null || entity == null)
            {
                return;
            }

            var keywordRepo = unitOfWork.CreateRepository<IServiceKeywordRepository>();
            var serviceClassRepo = unitOfWork.CreateRepository<IServiceServiceClassRepository>();
            var ontologyRepo = unitOfWork.CreateRepository<IServiceOntologyTermRepository>();
            var lifeEventRepo = unitOfWork.CreateRepository<IServiceLifeEventRepository>();
            var industrialClassRepo = unitOfWork.CreateRepository<IServiceIndustrialClassRepository>();
            var targetGroupRepo = unitOfWork.CreateRepository<IServiceTargetGroupRepository>();
            var serviceProducersRepo = unitOfWork.CreateRepository<IServiceProducerRepository>();

            var keywords = keywordRepo.All()
                .Include(x => x.Keyword)
                .Where(x => x.ServiceVersionedId == entity.Id && !string.IsNullOrEmpty(x.Keyword.Name) )
                .ToList();
            var serviceClasses = serviceClassRepo.All()
                .Include(x => x.ServiceClass).ThenInclude(x => x.Names)
                .Where(x => x.ServiceVersionedId == entity.Id)
                .ToList();
            var ontologies = ontologyRepo.All()
                .Include(x => x.OntologyTerm).ThenInclude(x => x.Names)
                .Where(x => x.ServiceVersionedId == entity.Id)
                .ToList();
            var lifeEvents = lifeEventRepo.All()
                .Include(x => x.LifeEvent).ThenInclude(x => x.Names)
                .Where(x => x.ServiceVersionedId == entity.Id)
                .ToList();
            var industrialClasses = industrialClassRepo.All()
                .Include(x => x.IndustrialClass).ThenInclude(x => x.Names)
                .Where(x => x.ServiceVersionedId == entity.Id)
                .ToList();
            var targetGroups = targetGroupRepo.All()
                .Where(x => x.ServiceVersionedId == entity.Id)
                .ToList();
            var serviceProducers = serviceProducersRepo.All()
                .Include(x => x.Organizations)
                .Include(x => x.AdditionalInformations)
                .Where(x => x.ServiceVersionedId == entity.Id)
                .ToList();
            entity.ServiceKeywords = keywords;
            entity.ServiceServiceClasses = serviceClasses;
            entity.ServiceOntologyTerms = ontologies;
            entity.ServiceLifeEvents = lifeEvents;
            entity.ServiceIndustrialClasses = industrialClasses;
            entity.ServiceTargetGroups = targetGroups;
            entity.ServiceProducers = serviceProducers;
        }

        private void UpdateExpiration(VmServiceHeader result, IUnitOfWork unitOfWork, ServiceVersioned service)
        {
            var expireOn = expirationService.GetExpirationDate(unitOfWork, service);
            if (expireOn == null) return;
            result.ExpireOn = expireOn.Value.ToEpochTime();
            result.IsNotUpdatedWarningVisible = expireOn < DateTime.UtcNow;
        }

        public VmServiceOutput SaveService(VmServiceInput model)
        {
            return ExecuteSave
            (
                model,
                unitOfWork => SaveService(unitOfWork, model),
                (unitOfWork, entity) => GetService(new VmServiceBasic {Id = entity.Id, IncludeConnections = true}, unitOfWork),
                new List<Action<IUnitOfWorkWritable>>
                {
                    unitOfWork => urlService.AddNewUrls(unitOfWork,
                        model.Laws.SelectMany(x => x.WebPage.Select(y => y.Value.UrlAddress))
                            .Concat(model.ServiceVouchers.SelectMany(x => x.Value.Select(y => y.UrlAddress))))
                }
            );
        }

        private ServiceVersioned SaveService(IUnitOfWorkWritable unitOfWork, VmServiceInput model)
        {
            CheckDuplicityOfServiceNames(model);

            if (model.GeneralDescriptionId.HasValue)
            {
                var psPublishedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                var gdRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var gd = unitOfWork.ApplyIncludes
                (
                    gdRep.All()
                        .Where(x => x.UnificRootId == model.GeneralDescriptionId)
                        .OrderBy(x => x.PublishingStatus.PriorityFallback),
                    i => i
                        .Include(x => x.PublishingStatus)
                        .Include(x => x.TargetGroups)
                        .Include(x => x.ServiceClasses)
                        .Include(x => x.OntologyTerms)
                        .Include(x => x.Names)
                        .Include(x => x.Keywords).ThenInclude(a => a.Keyword).ThenInclude(b => b.Localization)
                        .Include(x => x.LanguageAvailabilities)
                ).FirstOrDefault();

                if (gd != null)
                {
                    // check user rights for general descriptions
                    var restrictedTypes = commonService.GetRestrictedDescriptionTypes();
                    if (restrictedTypes.Contains(gd.GeneralDescriptionTypeId))
                        throw new PtvAppException("User does not have rights for the general description!");

                    model.GeneralDescriptionServiceTypeId = gd.TypeId;
                    model.GeneralDescriptionChargeTypeId = gd.ChargeTypeId;

                    var publishedLanguagesIds = gd.LanguageAvailabilities.Where(x => x.StatusId == psPublishedId)
                        .Select(x => x.LanguageId).ToList();
                    gd.Names = gd.Names.Where(x => publishedLanguagesIds.Contains(x.LocalizationId)).ToList();
                    model.GeneralDescriptionTargetGroups = gd.TargetGroups.Select(x => x.TargetGroupId).ToList();
                    model.GeneralDescriptionServiceClasses = gd.ServiceClasses.Select(x => x.ServiceClassId).ToList();
                    model.GeneralDescriptionOntologyTerms = gd.OntologyTerms.Select(x => x.OntologyTermId).ToList();
                    model.GeneralDescriptionKeywords = TranslationManagerToVm.TranslateAll<StatutoryServiceGeneralDescriptionKeyword, VmKeywordItem>(gd.Keywords).ToList();

                    var gdHeader = TranslationManagerToVm.Translate<IBaseInformation, VmEntityHeaderBase>(gd);
                    model.GeneralDescriptionName = gdHeader?.Name;
                }
            }

            //confirm delivered translations
            if (model.Id.IsAssigned())
            {
                // We need to load the actual existing keywords so that we can filter by the text/language
                var ids = model.Keywords.SelectMany(pair =>
                {
                    return pair.Value.Select(kw =>
                        new VmKeywordItem {Id = kw, OwnerReferenceId = model.Id});
                }).Select(x => x.Id).Distinct().ToList();
                var repo = unitOfWork.CreateRepository<IKeywordRepository>();
                var translatedKeywords = repo.All().Where(x => ids.Contains(x.Id));
                model.ExistingKeywords = translatedKeywords.Select(x => new VmKeywordItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    LocalizationId = x.LocalizationId,
                    LocalizationCode =  languageCache.GetByValue(x.LocalizationId),
                    OwnerReferenceId = model.Id
                }).Where(x => !string.IsNullOrEmpty(x.Name)).ToList();

                foreach (var language in model.LanguagesAvailabilities)
                {
                    translationService.ConfirmServiceDeliveredTranslation(unitOfWork, model.Id.Value,
                        language.LanguageId);
                }
            }

            var entity = TranslationManagerToEntity.Translate<VmServiceInput, ServiceVersioned>(model, unitOfWork);
            expirationService.SetExpirationDate(unitOfWork, entity);
            commonService.CreateHistoryMetaData<ServiceVersioned, ServiceLanguageAvailability>(entity, model);
            return entity;
        }

        private void CheckDuplicityOfServiceNames(VmServiceInput model)
        {
            foreach (var name in model.Name)
            {
                if (model.Organization.HasValue &&
                    commonService.CheckExistsServiceNameWithinOrganization(name.Value, model.Organization.Value,
                        model.UnificRootId))
                {
                    throw new DuplicityCheckException("", name.Value);
                }
            }
        }

        public VmEntityHeaderBase PublishService(IVmLocalizedEntityModel model)
        {
            if (!model.Id.IsAssigned()) return null;
            Guid? serviceId = model.Id;
            var result = ContextManager.ExecuteWriter(unitOfWork =>
            {
                //Validate mandatory values
                var validationMessages =
                    ValidationManager.CheckEntity<ServiceVersioned>(serviceId.Value, unitOfWork, model);
                if (validationMessages.Any())
                {
                    throw new PtvValidationException(validationMessages, null);
                }

                //Publishing
                var affected =
                    CommonService.PublishAndScheduleEntity<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork,
                        model);

                //update delivered translation
                var publishedStatusId = PublishingStatusCache.Get(PublishingStatus.Published);
                if (affected.Id.IsAssigned())
                {
                    foreach (var language in model.LanguagesAvailabilities.Where(x => x.StatusId == publishedStatusId))
                    {
                        var ids = new List<Guid> {affected.Id};
                        expirationService.SetExpirationDateForPublishing<ServiceVersioned>(unitOfWork, ids, utilities.UserHighestRole() == UserRoleEnum.Eeva);
                        translationService.ConfirmServiceDeliveredTranslation(unitOfWork, affected.Id,
                            language.LanguageId, allowRemoveTrackingOrders: true);
                    }
                }

                return affected;
            });
            return ContextManager.ExecuteReader(unitOfWork => GetServiceHeader(result.Id, unitOfWork));
        }

        public VmEntityHeaderBase ScheduleService(IVmLocalizedEntityModel model)
        {
            ContextManager.ExecuteReader(unitOfWork =>
            {
                var serviceRepo = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var service = serviceRepo.All().SingleOrDefault(sv => sv.Id == model.Id);
                var expirationDate = expirationService.GetExpirationDate(unitOfWork, service);
                if(model.LanguagesAvailabilities.Any(la => la.ValidFrom.FromEpochTime() > expirationDate))
                    throw new PtvAppException("Publishing date cannot be scheduled after automatic archiving date.", "Service.ScheduleException.LateDate");
            });
            return ExecuteScheduleEntity(model, (unitOfWork, result) => GetServiceHeader(result.Id, unitOfWork));
        }

        public VmServiceHeader DeleteService(Guid serviceId)
        {
            return ExecuteDelete(serviceId, GetServiceHeader,
                unitOfWork => serviceInternal.OnDeletingService(unitOfWork, serviceId));
        }

        public VmServiceHeader RemoveService(Guid serviceId)
        {
            //commonService.CheckArchiveAstiContract<ServiceVersioned>(serviceId);
            return ExecuteRemove(serviceId, GetServiceHeader);
        }

        public IVmEntityBase LockService(Guid id, bool isLockDisAllowedForArchived = false)
        {
            return Utilities.LockEntityVersioned<ServiceVersioned, Service>(id, isLockDisAllowedForArchived);
        }

        public IVmEntityBase UnLockService(Guid id)
        {
            return Utilities.UnLockEntityVersioned<ServiceVersioned, Service>(id);
        }

        public VmServiceHeader WithdrawService(Guid serviceId)
        {
            var result = CommonService.WithdrawEntity<ServiceVersioned, ServiceLanguageAvailability>(serviceId);
            SetServiceExpirationDate(result.Id);
            return ContextManager.ExecuteReader(unitOfWork => GetServiceHeader(result.Id, unitOfWork));
        }

        public VmServiceHeader RestoreService(Guid serviceId)
        {
            var result = CommonService.RestoreEntity<ServiceVersioned, ServiceLanguageAvailability>(serviceId);
            SetServiceExpirationDate(result.Id);
            return ContextManager.ExecuteReader(unitOfWork => GetServiceHeader(result.Id, unitOfWork));
        }

        public VmServiceHeader ArchiveLanguage(VmEntityBasic model)
        {
            return ExecuteArchiveLanguage(model, GetServiceHeader);
//            var entity = CommonService.ArchiveLanguage<ServiceVersioned, ServiceLanguageAvailability>(model);
//            UnLockEntity(entity.Id);
//            return GetServiceHeader(entity.Id);
        }

        public VmServiceHeader RestoreLanguage(VmEntityBasic model)
        {
            var result = CommonService.RestoreLanguage<ServiceVersioned, ServiceLanguageAvailability>(model);
            SetServiceExpirationDate(result.Id);
            return ContextManager.ExecuteReader(unitOfWork => GetServiceHeader(result.Id, unitOfWork));
        }

        public VmServiceHeader WithdrawLanguage(VmEntityBasic model)
        {
            var result = CommonService.WithdrawLanguage<ServiceVersioned, ServiceLanguageAvailability>(model);
            SetServiceExpirationDate(result.Id);
            return ContextManager.ExecuteReader(unitOfWork => GetServiceHeader(result.Id, unitOfWork));
        }
        
        private void SetServiceExpirationDate(Guid? entityId)
        {
            ContextManager.ExecuteWriter(unitOfWork =>
            {
                var serviceVersionedRepo = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var entity = serviceVersionedRepo.All().FirstOrDefault(x => x.Id == entityId);
                if (entity != null)
                {
                    expirationService.SetExpirationDate(unitOfWork, entity);
                    unitOfWork.Save();
                }
            });
        }

        public VmServiceHeader GetValidatedEntity(VmEntityBasic model)
        {
            DateTime? expireOn = null;
            var result = ExecuteValidate
            (
                () => Utilities.LockEntityVersioned<ServiceVersioned, Service>(model.Id.Value, true),
                (unitOfWork) =>
                {
                    expireOn = expirationService.GetExpirationDate<ServiceVersioned>(unitOfWork, model.Id.Value);
                    return GetServiceHeader(model.Id, unitOfWork);
                }
            );

            // ExpireOn is not filled automatically when GetValidatedEntity is called
            result.ExpireOn = expireOn.ToEpochTime() ?? 0;
            return result;
        }

        public Dictionary<string, List<ValidationMessage>> GetValidatedEntitySimple(Guid id)
        {
            var result = ExecuteValidate
            (
                () => Utilities.LockEntityVersioned<ServiceVersioned, Service>(id, true),
                unitOfWork => GetServiceHeader(id, unitOfWork)
            );

            return result.LanguagesAvailabilities.ToDictionary(x => languageCache.GetByValue(x.LanguageId),
                x => x.ValidatedFields);
        }

        public Guid? ArchiveLanguageSimple(VmEntityBasic model)
        {
            return ExecuteArchiveLanguage(model, (id, _) => id);
        }

        public Guid? DeleteServiceSimple(Guid serviceId)
        {
            return ExecuteDelete(serviceId, (id, _) => id,
                unitOfWork => serviceInternal.OnDeletingService(unitOfWork, serviceId));
        }

        public Guid? WithdrawLanguageSimple(VmServiceBasic model)
        {
            var result = CommonService.WithdrawLanguage<ServiceVersioned, ServiceLanguageAvailability>(model);
            return result?.Id;
        }

        public Guid? WithdrawServiceSimple(Guid id)
        {
            var result = CommonService.WithdrawEntity<ServiceVersioned, ServiceLanguageAvailability>(id);
            return result?.Id;
        }

        public Guid? RestoreLanguageSimple(VmServiceBasic model)
        {
            var result = CommonService.RestoreLanguage<ServiceVersioned, ServiceLanguageAvailability>(model);
            return result?.Id;
        }

        public Guid? RestoreServiceSimple(Guid id)
        {
            var result = CommonService.RestoreEntity<ServiceVersioned, ServiceLanguageAvailability>(id);
            return result?.Id;
        }

        public Guid? RemoveServiceSimple(Guid id)
        {
            return ExecuteRemove(id, (resultId, _) => resultId);
        }

        public VmConnectableServiceSearchResult GetConnectableService(VmConnectableServiceSearch search)
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
            var notCommonId =
                typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.NotCommon.ToString());

            #region SearchByFilterParam

            VmEntitySearch vmSearch = new VmEntitySearch();
            vmSearch.ContentTypes = new List<SearchEntityTypeEnum>
            {
                SearchEntityTypeEnum.ServiceService, SearchEntityTypeEnum.ServicePermit,
                SearchEntityTypeEnum.ServiceProfessional
            };
            vmSearch.SearchType = SearchTypeEnum.Name;
            var languagesIds = FillConnectableLanguagesIds(search, notCommonId, vmSearch);

            if (search.OrganizationId.HasValue)
            {
                vmSearch.OrganizationId = search.OrganizationId.Value;
            }
            else if (utilities.UserHighestRole() != UserRoleEnum.Eeva)
            {
                var userOrgs = utilities.GetAllUserOrganizations();
                if (userOrgs.Any())
                {
                    vmSearch.OrganizationIds = userOrgs.ToList();
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

            if (search.ServiceTypeId.HasValue)
            {
                var serviceType = typesCache.GetByValue<ServiceType>(search.ServiceTypeId.Value);
                switch (serviceType)
                {
                    case "Service":
                        vmSearch.ContentTypes = new List<SearchEntityTypeEnum>
                            {SearchEntityTypeEnum.ServiceService};
                        break;
                    case "PermissionAndObligation":
                        vmSearch.ContentTypes = new List<SearchEntityTypeEnum>
                            {SearchEntityTypeEnum.ServicePermit};
                        break;
                    case "ProfessionalQualifications":
                        vmSearch.ContentTypes = new List<SearchEntityTypeEnum>
                            {SearchEntityTypeEnum.ServiceProfessional};
                        break;
                }
            }

            vmSearch.LanguageIds = languagesIds;
            vmSearch.SelectedPublishingStatuses = new List<Guid> {publishedStatusId, draftStatusId};
            vmSearch.UseOnlySelectedStatuses = true;

            #endregion SearchByFilterParam

            search.SortData.ForEach(column =>
            {
                if (column.Column.ToLower() == SearchService.SqlConstants.SubEntityType.ToLower())
                {
                    column.Column = SearchService.SqlConstants.LocalizedSubEntityType;
                    vmSearch.UseLocalizedSubType = true;
                }
            });
            vmSearch.Language = search.Language;
            vmSearch.SortData = search.SortData;
            vmSearch.PageNumber = search.PageNumber.PositiveOrZero();

            var resultEnt = searchService.SearchEntities(vmSearch) as VmSearchResult<IVmEntityListItem>;

            var result = resultEnt?.SearchResult.Select(i => new VmConnectableService
                {
                    Id = i.Id,
                    UnificRootId = i.UnificRootId,
                    Name = i.Name,
                    ServiceType = typesCache.Get<ServiceType>(i.SubEntityType.ToString()),
                    LanguagesAvailabilities = i.LanguagesAvailabilities,
                    OrganizationId = i.OrganizationId,
                    Modified = i.Modified,
                    ModifiedBy = i.ModifiedBy
                })
                .ToList();
            var returnData = new VmConnectableServiceSearchResult
            {
                SearchResult = result,
                MoreAvailable = resultEnt?.MoreAvailable ?? false,
                Count = resultEnt?.Count ?? 0,
                PageNumber = resultEnt?.PageNumber ?? 0,
                EnumCollection = resultEnt?.EnumCollection
            };
            return returnData;
        }

        private List<Guid> FillConnectableLanguagesIds(VmConnectableServiceSearch search,
            Guid notCommonId, VmEntitySearch vmSearch)
        {
            return ContextManager.ExecuteReader(unitOfWork =>
            {
                var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                var channel = channelRep.All().FirstOrDefault(x => x.Id == search.Id);
                if (channel?.ConnectionTypeId == notCommonId)
                {
                    vmSearch.OrganizationId = channel.OrganizationId;
                }

                var languageAvailabilityRep =
                    unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
                var languagesIds = languageAvailabilityRep.All()
                    .Where(x => x.ServiceChannelVersionedId == search.Id)
                    .Select(x => x.LanguageId).ToList();
                return languagesIds;
            });
        }

        private VmEntitySearch CreateOrganizationConnectionEntitySearch(VmConnectionsServiceSearch search)
        {
            var uiLanguage = string.IsNullOrEmpty(search.Language) ? "fi" : search.Language;
            var publishedStatusId = PublishingStatusCache.Get(PublishingStatus.Published);
            var draftStatusId = PublishingStatusCache.Get(PublishingStatus.Draft);

            #region SearchByFilterParam

            var vmSearch = new VmEntitySearch
            {
                ContentTypes = new List<SearchEntityTypeEnum>
                {
                    SearchEntityTypeEnum.ServiceService,
                    SearchEntityTypeEnum.ServicePermit,
                    SearchEntityTypeEnum.ServiceProfessional
                },
                SearchType = SearchTypeEnum.Name,
                MaxPageCount = search.MaxPageCount > 0 ? search.MaxPageCount : 1
            };


            var userOrgs = utilities.GetAllUserOrganizations();
            if (userOrgs.Any())
            {
                vmSearch.OrganizationIds = userOrgs.ToList();
            }

            if (!vmSearch.OrganizationIds.Any() && search.OrganizationId.HasValue)
            {
                vmSearch.OrganizationId = search.OrganizationId;
            }

            vmSearch.SelectedPublishingStatuses = new List<Guid>{publishedStatusId, draftStatusId};
            commonService.ExtendPublishingStatusesByEquivalents(vmSearch.SelectedPublishingStatuses);
            vmSearch.UseOnlySelectedStatuses = true;
            vmSearch.PreferredPublishingStatus = PreferredPublishingStatusEnum.LastPublished;
            vmSearch.UseOrganizationSorting = true;

            #endregion SearchByFilterParam

            search.SortData.ForEach(column =>
            {
                if (column.Column.ToLower() == SearchService.SqlConstants.SubEntityType.ToLower())
                {
                    column.Column = SearchService.SqlConstants.LocalizedSubEntityType;
                    vmSearch.UseLocalizedSubType = true;
                }
            });
            vmSearch.Language = uiLanguage;
            vmSearch.SortData = search.SortData;
            vmSearch.PageNumber = search.PageNumber.PositiveOrZero();
            return vmSearch;
        }

        public VmConnectionsServiceSearchResult GetOrganizationConnections(VmConnectionsServiceSearch search)
        {
            if (search.SortData == null || !search.SortData.Any())
            {
                search.SortData = new List<VmSortParam>
                {
                    new VmSortParam {Column = "Modified", Order = 2, SortDirection = SortDirectionEnum.Desc},
                    new VmSortParam {Column = "Id", Order = 3, SortDirection = SortDirectionEnum.Desc}
                };
            }

            var vmSearch = CreateOrganizationConnectionEntitySearch(search);
            return ContextManager.ExecuteReader(unitOfWork => SearchServicesWithConnections(vmSearch, unitOfWork));
        }

        private VmEntitySearch CreateConnectionEntitySearch(VmConnectionsServiceSearch search)
        {
            var uiLanguage = string.IsNullOrEmpty(search.Language) ? "fi" : search.Language;
            var vmSearch = new VmEntitySearch
            {
                ContentTypes = new List<SearchEntityTypeEnum>
                {
                    SearchEntityTypeEnum.ServiceService,
                    SearchEntityTypeEnum.ServicePermit,
                    SearchEntityTypeEnum.ServiceProfessional
                },
                SearchType = SearchTypeEnum.Name,
                MaxPageCount = search.MaxPageCount > 0 ? search.MaxPageCount : 100
            };


            if (!string.IsNullOrEmpty(search.Fulltext))
            {
                var rootId = GetRootIdFromString(search.Fulltext);
                if (!rootId.HasValue)
                {
                    var searchText = search.Fulltext.ToLower();
                    vmSearch.Name = searchText;
                }
                else
                {
                    vmSearch.EntityIds.Add(rootId.Value);
                    vmSearch.SearchType = SearchTypeEnum.Id;
                }
            }

            if (search.OrganizationId.HasValue)
            {
                vmSearch.OrganizationId = search.OrganizationId;
            }
            else if (utilities.UserHighestRole() != UserRoleEnum.Eeva)
            {
                var userOrgs = utilities.GetAllUserOrganizations();
                if (userOrgs.Any())
                {
                    vmSearch.OrganizationIds = userOrgs.ToList();
                }
            }


            if (search.ServiceTypeId.HasValue)
            {
                vmSearch.ServiceServiceType.Add(search.ServiceTypeId.Value);
                var contentType = SearchEntityTypeEnum.Service;
                var serviceType =
                    Enum.Parse<ServiceTypeEnum>(typesCache.GetByValue<ServiceType>(search.ServiceTypeId.Value));
                switch (serviceType)
                {
                    case ServiceTypeEnum.Service : contentType = SearchEntityTypeEnum.ServiceService;
                         break;
                    case ServiceTypeEnum.ProfessionalQualifications : contentType = SearchEntityTypeEnum.ServiceProfessional;
                        break;
                    case ServiceTypeEnum.PermissionAndObligation : contentType = SearchEntityTypeEnum.ServicePermit;
                        break;
                }
                vmSearch.ContentTypes = new List<SearchEntityTypeEnum>{contentType};
            }

            if (search.Languages != null && search.Languages.Any())
            {
                vmSearch.Languages = search.Languages;
            }

            if (search.ServiceClasses != null && search.ServiceClasses.Any())
            {
                vmSearch.ServiceClasses = search.ServiceClasses;
            }

            if (search.OntologyTerms != null && search.OntologyTerms.Any())
            {
                vmSearch.OntologyTerms = search.OntologyTerms;
            }

            if (search.TargetGroups != null && search.TargetGroups.Any())
            {
                vmSearch.TargetGroups = search.TargetGroups;
            }

            if (search.AreaInformationTypes != null && search.AreaInformationTypes.Any())
            {
                vmSearch.AreaInformationTypes = search.AreaInformationTypes;
            }

            if (search.LifeEvents != null && search.LifeEvents.Any())
            {
                vmSearch.LifeEvents = search.LifeEvents;
            }

            if (search.IndustrialClasses != null && search.IndustrialClasses.Any())
            {
                vmSearch.IndustrialClasses = search.IndustrialClasses;
            }

            vmSearch.SelectedPublishingStatuses = search.SelectedPublishingStatuses;
            commonService.ExtendPublishingStatusesByEquivalents(vmSearch.SelectedPublishingStatuses);
            vmSearch.UseOnlySelectedStatuses = true;
            vmSearch.PreferredPublishingStatus = PreferredPublishingStatusEnum.LastPublished;
            vmSearch.Language = uiLanguage;
            vmSearch.SortData = search.SortData;
            vmSearch.PageNumber = search.PageNumber.PositiveOrZero();
            return vmSearch;
        }

        public VmConnectionsServiceSearchResult GetConnectionsService(VmConnectionsServiceSearch search)
        {
            if (search.SortData == null || !search.SortData.Any())
            {
                search.SortData = new List<VmSortParam>
                {
                    new VmSortParam {Column = "Modified", Order = 2, SortDirection = SortDirectionEnum.Desc},
                    new VmSortParam {Column = "Id", Order = 3, SortDirection = SortDirectionEnum.Desc}
                };
            }
            var vmSearch = CreateConnectionEntitySearch(search);
            return ContextManager.ExecuteReader(unitOfWork => SearchServicesWithConnections(vmSearch, unitOfWork));
        }

        private VmConnectionsServiceSearchResult SearchServicesWithConnections(VmEntitySearch vmSearch, IUnitOfWork unitOfWork)
        {
            var resultEnt = searchService.SearchEntities(vmSearch) as VmSearchResult<IVmEntityListItem>;

            var relations = connectionsService.GetAllServiceRelations(unitOfWork, resultEnt.SearchResult.Select(i => i.UnificRootId).Distinct().ToList());

            var result = resultEnt.SearchResult.Select(i =>
            {
                return new VmServiceConnectionsServiceItem
                {
                    Id = i.Id,
                    UnificRootId = i.UnificRootId,
                    Name = i.Name,
                    ServiceType = typesCache.Get<ServiceType>(i.SubEntityType.ToString()),
                    OrganizationId = i.OrganizationId,
                    LanguagesAvailabilities = i.LanguagesAvailabilities,
                    Connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, VmConnectionOutput>(
                            relations.TryGetOrDefault(i.UnificRootId, new List<ServiceServiceChannel>()))
                        .OrderBy(x => x.ChannelOrderNumber)
                        .ThenBy(x => x.ConnectionOrderNumber)
                        .ToList(),
                    SuggestedChannelIds = GetSuggestedChannels(unitOfWork, i.Id)
                };
            }).ToList();

            result.ForEach(r => r.NumberOfConnections = r.Connections.Count);

            var returnData = new VmConnectionsServiceSearchResult
            {
                SearchResult = result,
                MoreAvailable = resultEnt?.MoreAvailable ?? false,
                Count = resultEnt?.Count ?? 0,
                PageNumber = resultEnt?.PageNumber ?? 0,
                EnumCollection = resultEnt?.EnumCollection
            };
            return returnData;
        }

        public List<Guid> GetSuggestedChannels(Guid serviceId)
        {
            return ContextManager.ExecuteReader(unitOfWork => GetSuggestedChannels(unitOfWork, serviceId));
        }

        public List<Guid> GetSuggestedChannels(IUnitOfWork unitOfWork, Guid serviceId)
        {
            return unitOfWork.CreateRepository<IServiceVersionedRepository>().All()
                .Where(x => x.Id == serviceId)
                .SelectMany(x => x
                    .StatutoryServiceGeneralDescription
                    .StatutoryServiceGeneralDescriptionServiceChannels
                    .Select(y => y.ServiceChannelId)
                )
                .ToList();
        }

        public IVmEntityBase IsConnectable(Guid id)
        {
            ContextManager.ExecuteWriter(unitOfWork =>
            {
                utilities.CheckIsEntityConnectable<ServiceVersioned>(id, unitOfWork);
            });
            return null;
        }

        public VmServiceTranslationResult GenerateXliff(VmServiceTranslation model)
        {
            ContextManager.ExecuteReader(unitOfWork =>
            {
                var sv = GetEntity<ServiceVersioned>(model.Id, unitOfWork, i => i
                    .Include(s => s.ServiceNames)
                    .Include(s => s.ServiceDescriptions)
                    .Include(s => s.ServiceKeywords).ThenInclude(s => s.Keyword)
                    .Include(s => s.ServiceWebPages).ThenInclude(s => s.WebPage)
                    .Include(s => s.ServiceWebPages).ThenInclude(s => s.Localization)
                    .Include(x => x.ServiceRequirements)
                    .Include(x => x.ServiceLaws).ThenInclude(j => j.Law).ThenInclude(k => k.Names)
                    .Include(x => x.ServiceLaws).ThenInclude(j => j.Law).ThenInclude(k => k.WebPages)
                    .ThenInclude(l => l.Localization)
                    .Include(x => x.ServiceLaws).ThenInclude(j => j.Law).ThenInclude(k => k.WebPages)
                    .ThenInclude(l => l.WebPage)
                    .Include(x => x.ServiceWebPages).ThenInclude(x => x.WebPage)
                    .Include(x => x.ServiceWebPages).ThenInclude(x => x.Localization)
                    .Include(x => x.ServiceProducers).ThenInclude(x => x.Organizations)
                    .Include(x => x.ServiceProducers).ThenInclude(x => x.AdditionalInformations));

                return new VmServiceTranslationResult
                {
                    Data = xliffParser.GenerateServiceXliff(sv, model.SourceLocalizationId, model.TargetLocalizationId)
                };
            });

            return null;
        }

        public VmTranslationOrderStateSaveOutputs SendServiceEntityToTranslation(VmTranslationOrderInput model)
        {
            Guid entityId = Guid.Empty;
            ContextManager.ExecuteWriter(unitOfWork =>
            {
                translationService.CheckServiceOrderUpdate(model, unitOfWork);
                entityId = translationService.SaveServiceTranslationOrder(unitOfWork, model);
                unitOfWork.Save();
            });
            SetServiceExpirationDate(entityId);
            return GetServiceTranslationSaveData(entityId, model.SourceLanguage);
        }

        private VmTranslationOrderStateSaveOutputs GetServiceTranslationSaveData(Guid entityId, Guid languageId)
        {
            return ContextManager.ExecuteReader(unitOfWork => new VmTranslationOrderStateSaveOutputs
            {
                Id = entityId,
                Services = new List<VmServiceOutput>
                {
                    GetService(new VmServiceBasic {Id = entityId, IncludeConnections = true}, unitOfWork)
                },
                Translations = new List<VmTranslationOrderStateOutputs>
                {
                    translationService.GetServiceTranslationOrderStates(unitOfWork, entityId, languageId)
                }
            });
        }

        public VmTranslationOrderStateOutputs GetServiceTranslationData(VmTranslationDataInput model)
        {
            return ContextManager.ExecuteReader(unitOfWork =>
            {
                return translationService.GetServiceTranslationOrderStates(unitOfWork, model.EntityId,
                    model.SourceLanguage);
            });
        }

        private List<VmDisconnectedChannel> GetDisconnectedChannels(IUnitOfWork unitOfWork, Guid unificRootId)
        {
            var userId = pahaTokenAccessor.UserName.GetGuid();

            var notificationServiceServiceChannelRepo =
                unitOfWork.CreateRepository<INotificationServiceServiceChannelRepository>();
            var notificationServiceServiceChannels = notificationServiceServiceChannelRepo
                .All()
                .Include(n => n.Filters)
                .Where(n => n.ServiceId == unificRootId && !n.Filters.Where(f => f.UserId == userId)
                                .Select(f => f.NotificationServiceServiceChannelId).Contains(n.Id))
                .ToList()
                .GroupBy(n => new {ServiceChannelId = n.ChannelId, FilterId = n.Id})
                .ToDictionary( // Dictionary <FilterId - ServiceChannelVersionedId>
                    k => k.Key.FilterId,
                    v => VersioningManager.GetLastVersion<ServiceChannelVersioned>(unitOfWork, v.Key.ServiceChannelId)
                        .EntityId);

            // no notification for the service
            if (notificationServiceServiceChannels.IsNullOrEmpty()) return new List<VmDisconnectedChannel>();

            var serviceChannels = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>()
                .All()
                .Where(x => notificationServiceServiceChannels.Values.Contains(x.Id))
                .Include(ch => ch.ServiceChannelNames)
                .Include(ch => ch.DisplayNameTypes)
                .ToList();

            return notificationServiceServiceChannels.Select(ch =>
            {
                var serviceChannel = serviceChannels.First(x => x.Id == ch.Value);
                return new VmDisconnectedChannel
                {
                    ChannelUnificRootId = serviceChannel.UnificRootId,
                    ChannelVersionedId = ch.Value,
                    NotificationId = ch.Key,
                    ChannelType = typesCache.GetByValue<ServiceChannelType>(serviceChannel.TypeId),
                    Name = serviceChannel.ServiceChannelNames
                        .Where(j => serviceChannel.DisplayNameTypes.Any(dt =>
                                        dt.DisplayNameTypeId == j.TypeId && dt.LocalizationId == j.LocalizationId) ||
                                    serviceChannel.DisplayNameTypes.All(dt => dt.LocalizationId != j.LocalizationId))
                        .ToDictionary(k => languageCache.GetByValue(k.LocalizationId),
                            v => v.Name)
                };
            }).ToList();
        }

        /// <summary>
        /// Disable notifications for disconnected channels for current user
        /// </summary>
        /// <param name="model">get model</param>
        /// <returns></returns>
        public IVmEntityBase DisableDisconnectedChannelNotifications(VmGuidList model)
        {
            return ContextManager.ExecuteWriter(
                unitOfWork => DisableDisconnectedChannelNotifications(unitOfWork, model));
        }

        private IVmEntityBase DisableDisconnectedChannelNotifications(IUnitOfWorkWritable unitOfWork, IVmGuidList model)
        {
            if (model == null) return null;
            if (model.Data.IsNullOrEmpty()) return null;

            var userId = pahaTokenAccessor.UserName.GetGuid();

            var notificationFilterRep =
                unitOfWork.CreateRepository<INotificationServiceServiceChannelFilterRepository>();
            foreach (var notificationId in model.Data)
            {
                notificationFilterRep.Add(new NotificationServiceServiceChannelFilter
                {
                    NotificationServiceServiceChannelId = notificationId,
                    UserId = userId
                });
            }

            unitOfWork.Save();
            return null;
        }
    }

    [RegisterService(typeof(IServiceServiceInternal), RegisterType.Transient)]
    internal class ServiceServiceInternal : IServiceServiceInternal
    {
        private readonly IVersioningManager versioningManager;
        private readonly ICommonServiceInternal commonService;

        public ServiceServiceInternal(ICommonServiceInternal commonService, IVersioningManager versioningManager)
        {
            this.commonService = commonService;
            this.versioningManager = versioningManager;
        }

        public void OnDeletingService(IUnitOfWorkWritable unitOfWork, Guid entityId)
        {
            commonService.CheckArchiveAstiContract<ServiceVersioned>(unitOfWork, entityId);
            DeleteServiceConnections(unitOfWork, entityId);
        }

        private void DeleteServiceConnections(IUnitOfWork unitOfWork, Guid serviceVersionedId)
        {
            // Note: connection data between service and channel is not removed because
            // when use returns entity from archive and adds the same connection, she wants
            // to see the data that was there prior to archiving

            var serviceCollecctionServiceRep = unitOfWork.CreateRepository<IServiceCollectionServiceRepository>();
            var unificRootId = versioningManager.GetUnificRootId<ServiceVersioned>(unitOfWork, serviceVersionedId);
            serviceCollecctionServiceRep.All().Where(x => x.ServiceId == unificRootId)
                .ForEach(item => serviceCollecctionServiceRep.Remove(item));
        }
    }
}
