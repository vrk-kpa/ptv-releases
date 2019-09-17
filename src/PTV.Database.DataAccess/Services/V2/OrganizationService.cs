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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using IOrganizationService = PTV.Database.DataAccess.Interfaces.Services.V2.IOrganizationService;
using IOrganizationServiceInternal = PTV.Database.DataAccess.Interfaces.Services.V2.IOrganizationServiceInternal;
using PTV.Domain.Model.Models.V2.Organization;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services.V2
{
    [Framework.RegisterService(typeof(IOrganizationService), RegisterType.Transient)]
    [Framework.RegisterService(typeof(IOrganizationServiceInternal), RegisterType.Transient)]
    internal class OrganizationService :
        EntityServiceBase<OrganizationVersioned, Organization, OrganizationLanguageAvailability>,
        IOrganizationService
    {
        private IContextManager contextManager;
        private ITypesCache typesCache;
        private IUserOrganizationService userOrganizationService;
        private IPahaTokenProcessor tokenProcessor;
        private ICommonServiceInternal commonService;
        private IServiceUtilities utilities;
        private IOrganizationServiceInternal organizationServiceInternal;
        private IAddressService addressService;

        public OrganizationService(
            IContextManager contextManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            ICommonServiceInternal commonService,
            ICacheManager cacheManager,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IVersioningManager versionManager,
            IServiceUtilities utilities,
            IValidationManager validationManager,
            IUserOrganizationService userOrganizationService,
            IPahaTokenProcessor tokenProcessor,
            IOrganizationServiceInternal organizationServiceInternal,
            IAddressService addressService
        ) :
            base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker,
                contextManager, utilities, commonService, validationManager, versionManager)
        {
            this.contextManager = contextManager;
            this.typesCache = cacheManager.TypesCache;
            this.userOrganizationService = userOrganizationService;
            this.tokenProcessor = tokenProcessor;
            this.commonService = commonService;
            this.utilities = utilities;
            this.organizationServiceInternal = organizationServiceInternal;
            this.addressService = addressService;
        }

        public Guid GetSahaIdForPtvOrgRootId(IUnitOfWork unitOfWork, Guid ptvRootId)
        {
            var sahaId = unitOfWork.CreateRepository<ISahaOrganizationInformationRepository>().All().FirstOrDefault(i => i.OrganizationId == ptvRootId)?.SahaId;
            return sahaId.IsAssigned() ? sahaId.Value : ptvRootId;
        }

        public Dictionary<Guid, List<Guid>> GetSahaIdsForPtvOrgRootIds(List<Guid> ptvRootIds)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var sahaIdsRaw = unitOfWork.CreateRepository<ISahaOrganizationInformationRepository>().All().Where(i => ptvRootIds.Contains(i.OrganizationId)).ToList();
                var grouped = sahaIdsRaw.GroupBy(i => i.OrganizationId);
                var sahaIds = grouped.ToDictionary(i => i.Key, i => i.Select(j => j.SahaId).ToList());
                ptvRootIds.Except(sahaIds.Keys).ForEach(i => sahaIds.Add(i, new List<Guid>() { i }));
                return sahaIds;
            });
        }


        public void CreateNonExistingSahaOrganization(PahaOrganizationDto organization)
        {
            if (organization == null || !organization.Id.IsAssigned() ||
                (string.IsNullOrEmpty(organization.Name) && string.IsNullOrEmpty(organization.Name))) return;
            bool userOrgExists = contextManager.ExecuteReader(unitOfWork =>
            {
                return unitOfWork.CreateRepository<IOrganizationVersionedRepository>().All().Any(i =>
                    i.UnificRootId == organization.Id ||
                    i.UnificRoot.SahaOrganizationInformations.Any(j => j.SahaId == organization.Id));
            });
            if (!userOrgExists)
            {
                var newOrgId = contextManager.ExecuteWriter(unitOfWork =>
                {
                    var id = TranslationManagerToEntity
                        .Translate<PahaOrganizationDto, OrganizationVersioned>(organization, unitOfWork).UnificRootId;
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                    return id;
                });
                if (newOrgId.IsAssigned())
                {
                    contextManager.ExecuteWriter(unitOfWork =>
                    {
                        var sahaOrgInfoRep = unitOfWork.CreateRepository<ISahaOrganizationInformationRepository>();
                        if (!sahaOrgInfoRep.All().Any(i => i.OrganizationId == newOrgId && i.SahaId == organization.Id))
                        {
                            sahaOrgInfoRep.Add(new SahaOrganizationInformation()
                            {
                                SahaId = organization.Id, SahaParentId = organization.Id, OrganizationId = newOrgId,
                                Name = organization.Name
                            });
                        }

                        unitOfWork.Save(SaveMode.AllowAnonymous);
                    });
                }
            }
        }

        public VmOrganizationHeader GetOrganizationHeader(Guid? organizationId)
        {
            return contextManager.ExecuteReader(unitOfWork => GetOrganizationHeader(organizationId, unitOfWork));
        }

        public VmOrganizationHeader GetOrganizationHeader(Guid? organizationId, IUnitOfWork unitOfWork)
        {
            var result = new VmOrganizationHeader();
            OrganizationVersioned entity;
            result = GetModel<OrganizationVersioned, VmOrganizationHeader>(entity = GetEntity<OrganizationVersioned>(
                organizationId, unitOfWork,
                q => q
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x => x.OrganizationNames)
                    .Include(x => x.Versioning)
            ), unitOfWork);
            result.PreviousInfo = organizationId.HasValue
                ? Utilities.GetEntityEditableInfo<OrganizationVersioned, Organization, OrganizationLanguageAvailability>(organizationId.Value, unitOfWork)
                : null;
            result.MissingLanguages = organizationServiceInternal.GetOrganizationMissingLanguages(organizationId, unitOfWork);
            return result;
        }

        public VmOrganizationHeader RemoveOrganization(Guid organizationId)
        {
            return ExecuteRemove(organizationId, GetOrganizationHeader);
        }

        public VmOrganizationHeader DeleteOrganization(Guid organizationId)
        {
            return ExecuteDelete(organizationId, GetOrganizationHeader,
                unitOfWork => organizationServiceInternal.CascadeDeleteOrganization(unitOfWork, organizationId, HistoryAction.Delete));
//            VmOrganizationHeader result = null;
//            contextManager.ExecuteWriter(unitOfWork =>
//            {
//                var entity = CascadeDeleteOrganization(unitOfWork, organizationId);
//                unitOfWork.Save();
//                result = GetOrganizationHeader(organizationId, unitOfWork);
//            });
//            UnLockOrganization(result.Id.Value);
//            return result;
        }

        public VmArchiveResult CheckDeleteOrganization(Guid organizationId)
        {
            return contextManager.ExecuteWriter(unitOfWork =>
                organizationServiceInternal.CheckOrganizationContentForDelete(unitOfWork, organizationId));
        }

        public VmOrganizationOutput GetOrganization(VmOrganizationBasic model)
        {
            return ExecuteGet(model, (unitOfWork, vm) => GetOrganization(unitOfWork, model));
        }

        private VmOrganizationOutput GetOrganization(IUnitOfWork unitOfWork, VmOrganizationBasic model)
        {
            VmOrganizationOutput result = null;
            result = GetModel<OrganizationVersioned, VmOrganizationOutput>(GetEntity<OrganizationVersioned>(model.Id,
                unitOfWork,
                q => q.Include(x => x.OrganizationNames)
                    .Include(x => x.OrganizationDescriptions)
                    .Include(x => x.PublishingStatus)
                    .Include(x => x.Business)
                    .Include(x => x.Municipality)
                    .ThenInclude(x => x.MunicipalityNames)
                    .Include(x => x.OrganizationEmails)
                    .ThenInclude(x => x.Email)
                    .Include(x => x.OrganizationPhones)
                    .ThenInclude(x => x.Phone)
                    .ThenInclude(x => x.ExtraTypes)
                    .Include(x => x.OrganizationPhones)
                    .ThenInclude(x => x.Phone)
                    .ThenInclude(x => x.PrefixNumber)
                    .ThenInclude(x => x.Country)
                    .ThenInclude(x => x.CountryNames)
                    .Include(x => x.OrganizationWebAddress)
                    .ThenInclude(x => x.WebPage)
                    .Include(x => x.OrganizationAddresses)
                    .ThenInclude(x => x.Address)
                    .ThenInclude(x => x.AddressPostOfficeBoxes)
                    .ThenInclude(x => x.PostOfficeBoxNames)
                    .Include(x => x.OrganizationAddresses)
                    .ThenInclude(x => x.Address)
                    .ThenInclude(x => x.AddressForeigns)
                    .ThenInclude(x => x.ForeignTextNames)
                    .Include(x => x.OrganizationAddresses)
                    .ThenInclude(x => x.Address)
                    .ThenInclude(x => x.AddressPostOfficeBoxes)
                    .ThenInclude(x => x.PostalCode)
                    .ThenInclude(x => x.PostalCodeNames)
                    .Include(x => x.OrganizationAddresses)
                    .ThenInclude(x => x.Address)
                    .ThenInclude(x => x.AddressAdditionalInformations)
                    .Include(x => x.OrganizationAddresses)
                    .ThenInclude(x => x.Address)
                    .ThenInclude(x => x.Coordinates)
                    .Include(x => x.OrganizationAddresses)
                    .ThenInclude(x => x.Address)
                    .ThenInclude(x => x.ExtraTypes)
                    .Include(x => x.OrganizationAddresses)
                    .ThenInclude(x => x.Address)
                    .ThenInclude(x => x.Country)
                    .ThenInclude(x => x.CountryNames)
                    .Include(x => x.OrganizationAddresses)
                    .ThenInclude(x => x.Address)
                    .ThenInclude(x => x.ClsAddressPoints)
                    .ThenInclude(x => x.Municipality)
                    .Include(x => x.OrganizationAddresses)
                    .ThenInclude(x => x.Address)
                    .ThenInclude(x => x.ClsAddressPoints)
                    .ThenInclude(x => x.AddressStreet)
                    .ThenInclude(x => x.StreetNames)
                    .Include(x => x.OrganizationAddresses)
                    .ThenInclude(x => x.Address)
                    .ThenInclude(x => x.ClsAddressPoints)
                    .ThenInclude(x => x.AddressStreet)
                    .ThenInclude(x => x.StreetNumbers)
                    .ThenInclude(x => x.PostalCode)
                    .ThenInclude(x => x.PostalCodeNames)
                    .Include(x => x.OrganizationAddresses)
                    .ThenInclude(x => x.Address)
                    .ThenInclude(x => x.ClsAddressPoints)
                    .ThenInclude(x => x.AddressStreetNumber)
                    .Include(x => x.OrganizationAddresses)
                    .ThenInclude(x => x.Address)
                    .ThenInclude(x => x.ClsAddressPoints)
                    .ThenInclude(x => x.PostalCode)
                    .ThenInclude(x => x.PostalCodeNames)
                    .Include(x => x.OrganizationAddresses)
                    .ThenInclude(x => x.Address)
                    .ThenInclude(x => x.AddressOthers)
                    .ThenInclude(x => x.PostalCode)
                    .ThenInclude(x => x.PostalCodeNames)
                    .Include(x => x.OrganizationAreas)
                    .ThenInclude(x => x.Area)
                    .Include(x => x.OrganizationEInvoicings)
                    .ThenInclude(x => x.EInvoicingAdditionalInformations)
                    .Include(x => x.OrganizationAreaMunicipalities)
                    .Include(x => x.OrganizationDisplayNameTypes)
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x => x.Versioning)
            ), unitOfWork);

            result.PreviousInfo = result.Id.HasValue
                ? Utilities.GetEntityEditableInfo<OrganizationVersioned, Organization, OrganizationLanguageAvailability>(result.Id.Value, unitOfWork)
                : null;

            if (result.ParentId.HasValue)
            {
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations",
                        CommonService.GetOrganizations(new List<Guid> {result.ParentId.Value})));
            }

            if (result.ResponsibleOrganizationRegionId.HasValue)
            {
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", CommonService.GetOrganizations(new List<Guid> {result.ResponsibleOrganizationRegionId.Value})));
            }

            result.MissingLanguages = organizationServiceInternal.GetOrganizationMissingLanguages(model.Id, unitOfWork);
            return result;
        }

        public VmOrganizationOutput SaveOrganization(VmOrganizationInput model)
        {
            Dictionary<(Guid, string), Guid> newStreets = new Dictionary<(Guid, string), Guid>();
            return ExecuteSave
            (
                model,
                unitOfWork => SaveOrganization(unitOfWork, model),
                (unitOfWork, entity) => GetOrganization(unitOfWork, new VmOrganizationBasic() {Id = entity.Id}),
                new List<Action<IUnitOfWorkWritable>>
                {
                    unitOfWork => addressService.AddNewStreetAddresses(unitOfWork, model.VisitingAddresses.Concat(model.PostalAddresses), newStreets),
                    unitOfWork => addressService.AddNewStreetAddressNumbers(unitOfWork, model.VisitingAddresses.Concat(model.PostalAddresses), newStreets)
                }
            );
        }

        private OrganizationVersioned SaveOrganization(IUnitOfWorkWritable unitOfWork, VmOrganizationInput model)
        {
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();

            if (!commonService.IsOidUniqueForOrganization(model.Oid, model.Id, unitOfWork))
            {
                throw new PtvArgumentException("", model.Oid);
            }

            utilities.CheckIdFormat(model.Oid);

            if (typesCache.Get<OrganizationType>(OrganizationTypeEnum.TT1.ToString()) == model.OrganizationType ||
                typesCache.Get<OrganizationType>(OrganizationTypeEnum.TT2.ToString()) == model.OrganizationType)
            {
                if (organizationRep.All().Single(x => x.Id == model.Id).TypeId != model.OrganizationType)
                {
                    throw new PtvServiceArgumentException("Organization type is not allowed!",
                        new List<string> {typesCache.GetByValue<OrganizationType>(model.OrganizationType.Value)});
                }

            }
/* SOTE has been disabled (SFIPTV-1177)
            // SFIPTV-689: Creating of Sote organizations should not be allowed via UI
            if (commonService.OrganizationIsSote(model.OrganizationType))
            {
                if (!model.Id.IsAssigned())
                {
                    throw new PtvServiceArgumentException("Organization type is not allowed!",
                        new List<string> {typesCache.GetByValue<OrganizationType>(model.OrganizationType.Value)});
                }

                if (tokenProcessor.UserRole != UserRoleEnum.Eeva)
                {
                    var orgToUpdate = organizationRep.All().SingleOrDefault(x => x.Id == model.Id);
                    if (orgToUpdate == null || orgToUpdate.ResponsibleOrganizationRegionId != model.ResponsibleOrganizationRegionId)
                    {
                        throw new PtvServiceArgumentException("You do not have rights to edit organization of SOTE type!",
                            new List<string> {typesCache.GetByValue<OrganizationType>(model.OrganizationType.Value)});
                    }
                }
            }
*/            

            var entity = TranslationManagerToEntity.Translate<VmOrganizationInput, OrganizationVersioned>(model, unitOfWork);

            commonService.CreateHistoryMetaData<OrganizationVersioned, OrganizationLanguageAvailability>(entity);

            unitOfWork.Save();

            return entity;
        }

        public VmEntityHeaderBase PublishOrganization(IVmLocalizedEntityModel model)
        {
            if (!model.Id.IsAssigned()) return null;
            Guid? organizationId = model.Id;
            var affected = ContextManager.ExecuteWriter(unitOfWork =>
            {
                //Validate mandatory values
                var validationMessages =
                    ValidationManager.CheckEntity<OrganizationVersioned>(organizationId.Value, unitOfWork, model);
                if (validationMessages.Any())
                {
                    throw new PtvValidationException(validationMessages, null);
                }

                //Publishing
                return CommonService.PublishAndScheduleEntity<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWork,
                    model);
            });
            return ContextManager.ExecuteReader(unitOfWork => GetOrganizationHeader(affected.Id, unitOfWork));
        }

        public VmEntityHeaderBase ScheduleOrganization(IVmLocalizedEntityModel model)
        {
            return ExecuteScheduleEntity(model, (unitOfWork, result) => GetOrganizationHeader(result.Id, unitOfWork));
        }

        public IVmEntityBase LockOrganization(Guid id, bool isLockDisAllowedForArchived = false)
        {
            return Utilities.LockEntityVersioned<OrganizationVersioned, Organization>(id, isLockDisAllowedForArchived);
        }

        public IVmEntityBase UnLockOrganization(Guid id)
        {
            return Utilities.UnLockEntityVersioned<OrganizationVersioned, Organization>(id);
        }

        public VmOrganizationHeader WithdrawOrganization(Guid organizationId)
        {
            return ExecuteWithdraw(organizationId, GetOrganizationHeader, () =>
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    if (GetOrganizationConnectedEntities(unitOfWork, organizationId).AnyConnected)
                    {
                        throw new WithdrawConnectedExistsException();
                    }
                });
            });
//          
//            var result = CommonService.WithdrawEntity<OrganizationVersioned, OrganizationLanguageAvailability>(organizationId);
//            return GetOrganizationHeader(result.Id);
        }

        private VmArchiveResult GetOrganizationConnectedEntities(IUnitOfWork unitOfWork, Guid? organizationId)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var organization = organizationRep.All().SingleOrDefault(x => x.Id == organizationId);
            var organizations = organizationRep.All().Where(x => x.Id == organizationId);
            return new VmArchiveResult
            {
                Id = organization.Id,
                PublishingStatusId = organization.PublishingStatusId,
                ChannelsConnected = organizations.Any(i =>
                    i.UnificRoot.OrganizationServiceChannelsVersioned.Any(j =>
                        j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished)),
                SubOrganizationsConnected = organizations.Any(i =>
                    i.UnificRoot.Children.Any(j =>
                        j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished)),
                ServicesConnected = organizations.Any(i => i.UnificRoot.OrganizationServices.Any(j =>
                                        j.ServiceVersioned.PublishingStatusId != psDeleted &&
                                        j.ServiceVersioned.PublishingStatusId != psOldPublished))
                                    || organizations.Any(i => i.UnificRoot.ServiceProducerOrganizations.Any(j =>
                                        j.ServiceProducer.ServiceVersioned.PublishingStatusId != psDeleted &&
                                        j.ServiceProducer.ServiceVersioned.PublishingStatusId != psOldPublished))
                                    || organizations.Any(i => i.UnificRoot.OrganizationServicesVersioned.Any(j =>
                                        j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished))
            };
        }

        public VmOrganizationHeader RestoreOrganization(Guid organizationId)
        {
            return ExecuteRestore(organizationId, GetOrganizationHeader);
//            // note: cyclic dependency checking has been moved to OrganizationRoleChecker
//            var result = CommonService.RestoreEntity<OrganizationVersioned, OrganizationLanguageAvailability>(organizationId);
//            return GetOrganizationHeader(result.Id);
        }

        public VmOrganizationHeader ArchiveLanguage(VmEntityBasic model)
        {
            return ExecuteArchiveLanguage(model, GetOrganizationHeader);
//            var entity = CommonService.ArchiveLanguage<OrganizationVersioned, OrganizationLanguageAvailability>(model);
//            UnLockEntity(entity.Id);
//            return GetOrganizationHeader(entity.Id);
        }

        public VmOrganizationHeader RestoreLanguage(VmEntityBasic model)
        {
            return ExecuteRestoreLanguage(model, GetOrganizationHeader);
//            var entity = CommonService.RestoreLanguage<OrganizationVersioned, OrganizationLanguageAvailability>(model);
//            UnLockEntity(entity.Id);
//            return GetOrganizationHeader(entity.Id);
        }

        public VmOrganizationHeader WithdrawLanguage(VmEntityBasic model)
        {
            return ExecuteWithdrawLanguage(model, GetOrganizationHeader);
//            var entity = CommonService.WithdrawLanguage<OrganizationVersioned, OrganizationLanguageAvailability>(model);
//            UnLockEntity(entity.Id);
//            return GetOrganizationHeader(entity.Id);
        }

        public VmOrganizationHeader GetValidatedEntity(VmEntityBasic model)
        {
            return ExecuteValidate
            (
                () => Utilities.LockEntityVersioned<OrganizationVersioned, Organization>(model.Id.Value, true),
                (unitOfWork) => GetOrganizationHeader(model.Id, unitOfWork)
            );
        }

        public IVmListItemsData<IVmListItem> GetOrganizationList(VmOrganizationListSearch search)
        {
            var alowedPublishingStatuses = search.SearchOnlyDraftAndPublished
                ? new List<PublishingStatus>() {PublishingStatus.Draft, PublishingStatus.Published, PublishingStatus.Modified}
                : new List<PublishingStatus>();

            if (search.SearchAll || tokenProcessor.UserRole == UserRoleEnum.Eeva)
            {
                return new VmListItemsData<IVmListItem>(CommonService.GetOrganizationNames(search.SearchValue, false, alowedPublishingStatuses));
            }

            var organizations = userOrganizationService.GetAllUserOrganizations(alowedPublishingStatuses);
            string searchText = search.SearchValue.ToLower();
            return new VmListItemsData<IVmListItem>(organizations.Where
                (
                    x => x.Name.ToLower().Contains(searchText) || x.Translation.Texts.Any(n =>
                             !string.IsNullOrEmpty(n.Value) && n.Value.ToLower().Contains(searchText))
                )
            );
        }

        public List<Guid> GetMainRootOrganizationsIds(List<PublishingStatus> publishingStatuses = null)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var query = unitOfWork.CreateRepository<IOrganizationVersionedRepository>().All().Where(i => i.ParentId == null);
                if (!publishingStatuses.IsNullOrEmpty())
                {
                    var psIds = publishingStatuses.Select(i => typesCache.Get<PublishingStatusType>(i.ToString())).ToList();
                    query = query.Where(i => psIds.Contains(i.PublishingStatusId));
                }

                return query.Select(i => i.UnificRootId).Distinct().ToList();
            });
        }
    }


    [RegisterService(typeof(IOrganizationServiceInternal), RegisterType.Transient)]
    internal class OrganizationServiceInternal : IOrganizationServiceInternal
    {
        private IServiceUtilities utilities;
        private IVersioningManager versioningManager;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private ICommonServiceInternal commonService;

        public OrganizationServiceInternal(IServiceUtilities utilities, IVersioningManager versioningManager, ICacheManager cacheManager, ICommonServiceInternal commonService)
        {
            this.utilities = utilities;
            this.versioningManager = versioningManager;
            this.commonService = commonService;
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public IEnumerable<Guid> GetOrganizationMissingLanguages(Guid? organizationId, IUnitOfWork unitOfWork)
        {
            var result = new List<Guid>();
            var organizationRootId =
                versioningManager.GetUnificRootId<OrganizationVersioned>(unitOfWork, organizationId);
            var userOrgIds = utilities.GetAllUserOrganizations();
            if (utilities.UserHighestRole() != UserRoleEnum.Eeva && organizationRootId != null &&
                !userOrgIds.Contains(organizationRootId.Value))
            {
                return result;
            }

            var orgLangAvailRep = unitOfWork.CreateRepository<IOrganizationLanguageAvailabilityRepository>();
            var serLangAvailRep = unitOfWork.CreateRepository<IServiceLanguageAvailabilityRepository>();
            var chanLangVersionRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
            var publishedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var statuses = new List<Guid>
            {
                publishedStatusId,
                draftStatusId,
                modifiedStatusId
            };
            var organizationLanguages = orgLangAvailRep.All().Where(x =>
                x.OrganizationVersionedId == organizationId && statuses.Contains(x.StatusId)).Select(x => x.LanguageId);

            //find missing languages from services
            languageCache.AllowedLanguageIds
                .Where(id => !organizationLanguages.Contains(id))
                .ForEach(missingId =>
                    {
                        if (serLangAvailRep.All()
                            .Where(lang => lang.LanguageId == missingId &&
                                           lang.ServiceVersioned.OrganizationId == organizationRootId &&
                                           statuses.Contains(lang.ServiceVersioned.PublishingStatusId))
                            .GroupBy(x => x.ServiceVersioned.UnificRootId)
                            .ToDictionary(v => v.Key,
                                z => z.OrderBy(y =>
                                    y.StatusId == publishedStatusId ? 0 :
                                    y.StatusId == draftStatusId ? 1 :
                                    y.StatusId == modifiedStatusId ? 2 : 3).First())
                            .Any(lang =>
                                statuses.Contains(lang.Value.StatusId)))
                        {
                            result.Add(missingId);
                        }
                    }
                );
            //find missing languages from channels
            languageCache.AllowedLanguageIds
                .Where(id => !organizationLanguages.Contains(id))
                .Where(id => !result.Contains(id))
                .ForEach(missingId =>
                    {
                        if (chanLangVersionRep.All()
                            .Where(lang => lang.LanguageId == missingId &&
                                           lang.ServiceChannelVersioned.OrganizationId == organizationRootId &&
                                           statuses.Contains(lang.ServiceChannelVersioned.PublishingStatusId))
                            .GroupBy(x => x.ServiceChannelVersioned.UnificRootId)
                            .ToDictionary(v => v.Key,
                                z => z.OrderBy(y =>
                                    y.StatusId == publishedStatusId ? 0 :
                                    y.StatusId == draftStatusId ? 1 :
                                    y.StatusId == modifiedStatusId ? 2 : 3).First())
                            .Any(lang =>
                                statuses.Contains(lang.Value.StatusId)))
                        {
                            result.Add(missingId);
                        }
                    }
                );
            return result;
        }

        public void CascadeDeleteOrganization(IUnitOfWorkWritable unitOfWork, Guid id, HistoryAction action)
        {
            CascadeDeleteOrganization(unitOfWork, id, false, action);
        }
        
        public VmArchiveResult CheckOrganizationContentForDelete(IUnitOfWorkWritable unitOfWork, Guid id)
        {
            return CascadeDeleteOrganization(unitOfWork, id, true, HistoryAction.Delete);
        }
        
        private VmArchiveResult CascadeDeleteOrganization(IUnitOfWorkWritable unitOfWork, Guid? id, bool checkDelete, HistoryAction action)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();

            /// TODO : check if organization is in use
//            var organizationUserMapRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
//            var userMaps = organizationUserMapRep.All();
//            var organizationIsUsed =
//                organizationRep.All()
//                    .Where(x => x.Id == id)
//                    .Any(
//                        i => userMaps.Any(k => k.OrganizationId == i.UnificRootId));
//            if (organizationIsUsed)
//            {
//                throw new OrganizationNotDeleteInUserUseException();
//            }

            var organization = organizationRep.All().SingleOrDefault(x => x.Id == id);

            if (checkDelete)
            {
                var organizations = organizationRep.All().Where(x => x.Id == id);
                var result = new VmArchiveResult
                {
                    Id = organization.Id,
                    PublishingStatusId = organization.PublishingStatusId,
                    ChannelsConnected = organizations.Any(i =>
                        i.UnificRoot.OrganizationServiceChannelsVersioned.Any(j =>
                            j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished)),
                    SubOrganizationsConnected = organizations.Any(i =>
                        i.UnificRoot.Children.Any(j =>
                            j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished)),
                    ServicesConnected = organizations.Any(i => i.UnificRoot.OrganizationServices.Any(j =>
                                            j.ServiceVersioned.PublishingStatusId != psDeleted &&
                                            j.ServiceVersioned.PublishingStatusId != psOldPublished))
                                        || organizations.Any(i => i.UnificRoot.ServiceProducerOrganizations.Any(j =>
                                            j.ServiceProducer.ServiceVersioned.PublishingStatusId != psDeleted &&
                                            j.ServiceProducer.ServiceVersioned.PublishingStatusId != psOldPublished))
                                        || organizations.Any(i => i.UnificRoot.OrganizationServicesVersioned.Any(j =>
                                            j.PublishingStatusId != psDeleted &&
                                            j.PublishingStatusId != psOldPublished))
                };
                if (result.AnyConnected)
                {
                    return result;
                }
            }
            else
            {
//                CommonService.ChangeEntityToDeleted<OrganizationVersioned>(unitOfWork, organization.Id);
                ArchiveConnectedChannels(unitOfWork, organization.UnificRootId);
                ArchiveConnectedServices(unitOfWork, organization.UnificRootId);
                ArchiveConnectedServiceCollections(unitOfWork, organization.UnificRootId);
                ArchiveSubOrganizations(unitOfWork, organization.UnificRootId);
            }

            return new VmArchiveResult {Id = organization.Id, PublishingStatusId = organization.PublishingStatusId};
        }
        
        private void ArchiveSubOrganizations(IUnitOfWorkWritable unitOfWork, Guid organizationId)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var subOrgs = organizationRep.All()
                .Where(x => x.PublishingStatusId != psDeleted && x.PublishingStatusId != psOldPublished)
                .Where(x => x.ParentId == organizationId).ToList();

            foreach (var subOrg in subOrgs)
            {
                commonService.ChangeEntityToDeleted<OrganizationVersioned, OrganizationLanguageAvailability>(
                    unitOfWork, subOrg.Id, HistoryAction.ArchivedViaOrganization, true);
                ArchiveConnectedChannels(unitOfWork, subOrg.UnificRootId);
                ArchiveConnectedServices(unitOfWork, subOrg.UnificRootId);
                ArchiveConnectedServiceCollections(unitOfWork, subOrg.UnificRootId);
                ArchiveSubOrganizations(unitOfWork, subOrg.UnificRootId);
            }
        }
        
        private void ArchiveConnectedServiceCollections(IUnitOfWorkWritable unitOfWork, Guid organizationId)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var serviceCollectionRep = unitOfWork.CreateRepository<IServiceCollectionVersionedRepository>();
            serviceCollectionRep.All().Where(x => x.PublishingStatusId != psDeleted && x.PublishingStatusId != psOldPublished)
                .Where(x => x.OrganizationId == organizationId)
                .ForEach(x =>
                {
                    commonService.ChangeEntityToDeleted<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(
                        unitOfWork, x.Id, HistoryAction.ArchivedViaOrganization, true);
                });
        }

        private void ArchiveConnectedChannels(IUnitOfWorkWritable unitOfWork, Guid organizationId)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            channelRep.All().Where(x => x.PublishingStatusId != psDeleted && x.PublishingStatusId != psOldPublished)
                .Where(x => x.OrganizationId == organizationId)
                .ForEach(x =>
                {
                    commonService.ChangeEntityToDeleted<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                        unitOfWork, x.Id, HistoryAction.ArchivedViaOrganization, true);
                });
        }

        private void ArchiveConnectedServices(IUnitOfWorkWritable unitOfWork, Guid organizationId)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var services = serviceRep.All()
                .Where(x => x.PublishingStatusId != psDeleted && x.PublishingStatusId != psOldPublished)
                .Where(x => x.OrganizationServices.Any(o => o.OrganizationId == organizationId) ||
                            x.ServiceProducers.SelectMany(sp => sp.Organizations)
                                .Any(o => o.OrganizationId == organizationId) ||
                            x.OrganizationId == organizationId)
                .Include(x => x.OrganizationServices).ThenInclude(x => x.Organization).ThenInclude(x => x.Versions)
                .Include(x => x.ServiceProducers).ThenInclude(x => x.Organizations).ThenInclude(x => x.Organization)
                .ThenInclude(x => x.Versions)
                .ToList();

            foreach (var service in services)
            {
                var restOrganizations = service.OrganizationServices.Where(x =>
                    x.OrganizationId != organizationId && x.Organization.Versions.Any(y =>
                        y.PublishingStatusId != psDeleted && y.PublishingStatusId != psOldPublished)).ToList();
                service.OrganizationServices = restOrganizations.ToList();

                var producersToDelete =
                    HandleServiceProducers(service.ServiceProducers, organizationId, psDeleted, psOldPublished);
                if (producersToDelete.Any())
                {
                    producersToDelete.ForEach(p => service.ServiceProducers.Remove(p));

                    var orderNumber = 1;
                    service.ServiceProducers.OrderBy(p => p.OrderNumber).ForEach(p => p.OrderNumber = orderNumber++);
                }

                if (service.OrganizationId == organizationId)
                {
                    commonService.ChangeEntityToDeleted<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork,
                        service.Id, HistoryAction.ArchivedViaOrganization, true);
                }
            }
        }
        
        private List<ServiceProducer> HandleServiceProducers(ICollection<ServiceProducer> serviceProducers,
            Guid organizationId, Guid psDeleted, Guid psOldPublished)
        {
            var producersToDelete = new List<ServiceProducer>();
            if (serviceProducers == null) return producersToDelete;

            foreach (var producer in serviceProducers)
            {
                if (producer.ProvisionTypeId ==
                    typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString()))
                {
                    var producerOrganizations = producer.Organizations.Where(spo =>
                        spo.OrganizationId != organizationId && spo.Organization.Versions.Any(ov =>
                            ov.PublishingStatusId != psDeleted && ov.PublishingStatusId != psOldPublished)).ToList();
                    if (producerOrganizations.Any())
                    {
                        producer.Organizations = producerOrganizations;
                    }
                    else
                    {
                        producersToDelete.Add(producer);
                    }
                }

                if (producer.ProvisionTypeId ==
                    typesCache.Get<ProvisionType>(ProvisionTypeEnum.PurchaseServices.ToString()))
                {
                    var spo = producer.Organizations.FirstOrDefault();
                    if (spo == null)
                    {
                        producersToDelete.Add(producer);
                    }
                    else
                    {
                        if (spo.OrganizationId == organizationId && !spo.Organization.Versions.Any(ov =>
                                ov.PublishingStatusId != psDeleted && ov.PublishingStatusId != psOldPublished))
                        {
                            producersToDelete.Add(producer);
                        }
                    }
                }

                if (producer.ProvisionTypeId == typesCache.Get<ProvisionType>(ProvisionTypeEnum.Other.ToString()))
                {
                    var spo = producer.Organizations.FirstOrDefault();
                    if (spo != null && spo.OrganizationId == organizationId && !spo.Organization.Versions.Any(ov =>
                            ov.PublishingStatusId != psDeleted && ov.PublishingStatusId != psOldPublished))
                    {
                        producersToDelete.Add(producer);
                    }
                }
            }

            return producersToDelete;
        }
    }
}