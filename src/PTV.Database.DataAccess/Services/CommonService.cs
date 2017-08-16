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
using Microsoft.AspNetCore.Mvc.Rendering;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework.Extensions;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ICommonService), RegisterType.Transient)]
    [RegisterService(typeof(ICommonServiceInternal), RegisterType.Transient)]
    internal class CommonService : ServiceBase, ICommonService, ICommonServiceInternal
    {
        private static readonly List<string> TranslationLanguageCodes = new List<string>() { LanguageCode.fi.ToString(), LanguageCode.sv.ToString(), LanguageCode.en.ToString() };
        private static readonly List<string> SelectedPublishingStatuses = new List<string>() { PublishingStatus.Draft.ToString(), PublishingStatus.Published.ToString() };

        private readonly IContextManager contextManager;
        private readonly ITypesCache typesCache;
        private readonly IDataServiceFetcher dataServiceFetcher;
        private readonly ServiceUtilities utilities;
        private readonly IVersioningManager versioningManager;
        private readonly ApplicationConfiguration configuration;
        private IHttpContextAccessor httpContextAccessor;

        public CommonService(
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IContextManager contextManager,
            ITypesCache typesCache,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IDataServiceFetcher dataServiceFetcher,
            ServiceUtilities utilities,
            IVersioningManager versioningManager,
            ApplicationConfiguration configuration,
            IHttpContextAccessor httpContextAccessor
            )
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
            this.typesCache = typesCache;
            this.dataServiceFetcher = dataServiceFetcher;
            this.utilities = utilities;
            this.versioningManager = versioningManager;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        public IVmGetFrontPageSearch GetFrontPageSearch()
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceClassesRep = unitOfWork.CreateRepository<IServiceClassRepository>();
                var targetGroupRep = unitOfWork.CreateRepository<ITargetGroupRepository>();
                var serviceClasses = TranslationManagerToVm.TranslateAll<ServiceClass, VmListItem>(GetIncludesForFinto<ServiceClass, ServiceClassName>(unitOfWork,serviceClassesRep.All().OrderBy(x => x.Label)));
                var targetGroups = CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<TargetGroup, TargetGroupName>(unitOfWork, targetGroupRep.All().OrderBy(x => x.Label))), x => x.Code);
                var result = new VmGetFrontPageSearch
                {
                    OrganizationId = utilities.GetUserMainOrganization()
                };
                var publishingStatuses = GetPublishingStatuses();
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ServiceClasses", serviceClasses),
                    () => GetEnumEntityCollectionModel("PublishingStatuses", publishingStatuses),
                    () => GetEnumEntityCollectionModel("ServiceTypes", GetServiceTypes()),
                    () => GetEnumEntityCollectionModel("ChargeTypes", GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("PhoneNumberTypes", GetPhoneTypes()),
                    () => GetEnumEntityCollectionModel("ChannelTypes", GetServiceChannelTypes()),
                    () => GetEnumEntityCollectionModel("TargetGroups", targetGroups));
                result.SelectedPublishingStatuses = publishingStatuses.Where(x => SelectedPublishingStatuses.Contains(x.Code)).Select(x => x.Id).ToList();
                return result;
            });
        }

        public IVmBase GetTypedData(IEnumerable<string> dataTypes)
        {
            return new VmListItemsData<IVmBase>(dataServiceFetcher.Fetch(dataTypes));
        }

        public VmListItemsData<VmListItem> GetPhoneChargeTypes()
        {
            return dataServiceFetcher.FetchType<ServiceChargeType>();
        }

        public VmListItemsData<VmListItem> GetWebPageTypes()
        {
            return dataServiceFetcher.FetchType<WebPageType>();
        }

        public VmListItemsData<VmListItem> GetServiceTypes()
        {
            return dataServiceFetcher.FetchType<ServiceType>();
        }

        public VmListItemsData<VmListItem> GetProvisionTypes()
        {
            return dataServiceFetcher.FetchType<ProvisionType>();
        }

        public VmListItemsData<VmListItem> GetPrintableFormUrlTypes()
        {
            return dataServiceFetcher.FetchType<PrintableFormChannelUrlType>();
        }

        public VmListItemsData<VmListItem> GetPhoneTypes()
        {
            return dataServiceFetcher.FetchType<PhoneNumberType>();
        }

        public VmListItemsData<VmListItem> GetServiceHourTypes()
        {
            return dataServiceFetcher.FetchType<ServiceHourType>();
        }

        public VmListItemsData<VmListItem> GetPublishingStatuses()
        {
            return new VmListItemsData<VmListItem>(dataServiceFetcher.FetchType<PublishingStatusType>().Select(i => new VmPublishingStatus()
            {
                Id = i.Id,
                Name = i.Name,
                Code = i.Code,
                OrderNumber = i.OrderNumber,
                Type = i.Code.Parse<PublishingStatus>()
            }));
        }

        public VmListItemsData<VmListItem> GetCoordinateTypes()
        {
            return dataServiceFetcher.FetchType<CoordinateType>();
        }

        public VmListItemsData<VmListItem> GetAreaInformationTypes()
        {
            return dataServiceFetcher.FetchType<AreaInformationType>();
        }

        public VmListItemsData<VmListItem> GetAreaTypes()
        {
            return dataServiceFetcher.FetchType<AreaType>();
        }

        public IReadOnlyList<VmListItem> GetOrganizationNames(IUnitOfWork unitOfWork, string searchText = null, bool takeAll = true)
        {
			// get enum values to local variables before using them inside LINQ queries as otherwise the provider does the queries in-memory and not in DB
            // as it cannot translate the enum values to db queries and this way we get better performance as the queries are evaluated in DB
            var organizationRepository = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());

            var resultTemp = organizationRepository.All().Where(x => x.PublishingStatusId == psPublished);

            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.ToLower();
                resultTemp = resultTemp.Where(x => x.OrganizationNames.Any(n => n.Name.ToLower().Contains(searchText) && n.TypeId == x.OrganizationDisplayNameTypes.FirstOrDefault(type => type.LocalizationId == n.LocalizationId).DisplayNameTypeId));
            }

            if (!takeAll)
            {
                resultTemp = resultTemp.Take(CoreConstants.MaximumNumberOfAllItems);
            }
            resultTemp = unitOfWork.ApplyIncludes(resultTemp, q => q.Include(i => i.OrganizationNames)
                                                                        .ThenInclude(i => i.Localization)
                                                                     .Include(i => i.OrganizationDisplayNameTypes));
            return TranslationManagerToVm.TranslateAll<OrganizationVersioned, VmListItem>(resultTemp);
        }

        public IReadOnlyList<VmListItem> GetUserAvailableOrganizationNamesForOrganization(IUnitOfWork unitOfWork, Guid? organizationId = null, Guid? parentId = null)
        {
            var organizationRepository = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var resultTemp = organizationRepository.All().Where(x => x.PublishingStatusId == psPublished);
            var userHighestRole = utilities.UserHighestRole();
            if (userHighestRole != UserRoleEnum.Eeva)
            {
                var userOrganizationIds = new List<Guid>();
                var structure = utilities.GetUserCompleteOrgStructure(unitOfWork);
                if (organizationId.IsAssigned())
                {                   
                    var orgRole = structure.SelectMany(i => i).FirstOrDefault(x => x.OrganizationId == organizationId)?.Role;
                    if (orgRole == UserRoleEnum.Pete)
                    {
                        userOrganizationIds = structure.SelectMany(i => i).Where(x => x.Role == orgRole).Select(i => i.OrganizationId).ToList();
                    }
                    else if(parentId.IsAssigned())
                    {
                        userOrganizationIds.Add(parentId.Value);
                    }
                }
                else
                {
                    if (userHighestRole == UserRoleEnum.Pete)
                    {
                        userOrganizationIds = structure.SelectMany(i => i).Where(x => x.Role == UserRoleEnum.Pete).Select(i => i.OrganizationId).ToList();
                    }
                }                
                resultTemp = resultTemp.Where(x => userOrganizationIds.Contains(x.UnificRootId));
            }

            resultTemp = unitOfWork.ApplyIncludes(resultTemp, q => q.Include(i => i.OrganizationNames)
               .ThenInclude(i => i.Localization)
               .Include(i => i.OrganizationDisplayNameTypes));
            return TranslationManagerToVm.TranslateAll<OrganizationVersioned, VmListItem>(resultTemp);
        }

        public IReadOnlyList<VmListItem> GetUserAvailableOrganizationNames(IUnitOfWork unitOfWork, string searchText = null, bool takeAll = true)
        {
            var organizationRepository = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var resultTemp = organizationRepository.All().Where(x => x.PublishingStatusId == psPublished);

            if (utilities.UserHighestRole() != UserRoleEnum.Eeva)
            {
                var userOrganizationIds = utilities.GetAllUserOrganizations(unitOfWork);
                resultTemp = resultTemp.Where(x => userOrganizationIds.Contains(x.UnificRootId));
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.ToLower();
                resultTemp = resultTemp.Where(x => x.OrganizationNames.Any(n => n.Name.ToLower().Contains(searchText) && n.TypeId == x.OrganizationDisplayNameTypes.FirstOrDefault(type => type.LocalizationId == n.LocalizationId).DisplayNameTypeId));
            }

            if (!takeAll)
            {
                resultTemp = resultTemp.Take(CoreConstants.MaximumNumberOfAllItems);
            }
            resultTemp = unitOfWork.ApplyIncludes(resultTemp, q => q.Include(i => i.OrganizationNames)
                .ThenInclude(i => i.Localization)
                .Include(i => i.OrganizationDisplayNameTypes));
            return TranslationManagerToVm.TranslateAll<OrganizationVersioned, VmListItem>(resultTemp);
        }

        public IReadOnlyList<VmLaw> GetLaws(IUnitOfWork unitOfWork, List<Guid> takeIds)
        {
            var lawRep = unitOfWork.CreateRepository<ILawRepository>();
            var resultTemp = lawRep.All()
                .Where(x => takeIds.Contains(x.Id))
                .Include(x => x.Names)
                .Include(x => x.WebPages).ThenInclude(w => w.WebPage);
            return TranslationManagerToVm.TranslateAll<Law, VmLaw>(resultTemp);
        }

        public List<VmTreeItem> GetOrganizations(IUnitOfWork unitOfWork)
        {
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var psPublished = PublishingStatusCache.Get(PublishingStatus.Published);
            var organizations = unitOfWork.ApplyIncludes(organizationRep.All().Where(x => x.PublishingStatusId == psPublished), query => query.Include(organization => organization.OrganizationNames));
            return CreateTree<VmTreeItem>(LoadOrganizationTree(organizations, 1));
        }

        public VmListItemsData<VmListItem> GetLanguages()
        {
            return dataServiceFetcher.FetchType<Language>();
        }

        public IReadOnlyList<VmListItem> GetTranslationLanguages()
        {
            return GetLanguages().Where(x => TranslationLanguageCodes.Contains(x.Code)).ToList();
        }

        public VmListItemsData<VmListItem> GetServiceChannelTypes()
        {
            return dataServiceFetcher.FetchType<ServiceChannelType>();
        }

        public IReadOnlyList<VmListItem> GetMunicipalities(IUnitOfWork unitOfWork)
        {
            var municipalityRep = unitOfWork.CreateRepository<IMunicipalityRepository>();
            var municipalities = unitOfWork.ApplyIncludes(municipalityRep.All(), i => i.Include(j => j.MunicipalityNames));
            return TranslationManagerToVm.TranslateAll<Municipality, VmListItem>(municipalities).OrderBy(x => x.Name).ToList();
        }

        public IReadOnlyList<VmListItemReferences> GetAreas(IUnitOfWork unitOfWork, AreaTypeEnum type)
        {
            var areaRep = unitOfWork.CreateRepository<IAreaRepository>();
            var areas = unitOfWork.ApplyIncludes(areaRep.All().Where(x=>x.AreaTypeId == typesCache.Get<AreaType>(type.ToString()) && x.IsValid), i => i.Include(j => j.AreaNames).Include(j=>j.AreaMunicipalities));
            return TranslationManagerToVm.TranslateAll<Area, VmListItemReferences>(areas).OrderBy(x => x.Name).ToList();
        }

        public IReadOnlyList<VmDialCode> GetDefaultDialCode(IUnitOfWork unitOfWork)
        {
            var defaultCountryCode = configuration.GetDefaultCountryCode();
            var dialCodeRep = unitOfWork.CreateRepository<IDialCodeRepository>();
            var defaultDialCodes = unitOfWork.ApplyIncludes(dialCodeRep.All().Where(x => x.Country.Code == defaultCountryCode.ToUpper()), i => i.Include(j => j.Country).ThenInclude(j => j.CountryNames));
            return TranslationManagerToVm.TranslateAll<DialCode, VmDialCode>(defaultDialCodes);
        }

        public IReadOnlyList<VmListItem> GetOrganizationNamesWithoutSetOfOrganizations(IUnitOfWork unitOfWork, IList<Guid?> organizationSet)
        {
            var psDraft = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var organizationNameRep = unitOfWork.CreateRepository<IOrganizationNameRepository>();
            return
                TranslationManagerToVm.TranslateAll<OrganizationName, VmListItem>(
                    organizationNameRep.All().Where(x => !organizationSet.Contains(x.OrganizationVersionedId))
                    .Where(x => x.OrganizationVersioned.PublishingStatusId == psDraft || x.OrganizationVersioned.PublishingStatusId == psPublished)
                    .Where(x => x.TypeId == x.OrganizationVersioned.OrganizationDisplayNameTypes.FirstOrDefault(type => type.LocalizationId == x.LocalizationId).DisplayNameTypeId))
                    .OrderBy(x => x.Name, StringComparer.CurrentCulture).ToList();
        }

        
        public Guid GetDraftStatusId()
        {
            return typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
        }

        public string GetLocalization(Guid? languageId)
        {
            if (languageId.HasValue)
            {
                return typesCache.GetByValue<Language>(languageId.Value);
            }
            return LanguageCode.fi.ToString();
        }

        public Guid GetLocalizationId(string langCode)
        {
            return typesCache.Get<Language>((!string.IsNullOrEmpty(langCode) ? langCode : LanguageCode.fi.ToString()));
        }

        public PublishingResult PublishEntity<TEntity, TLanguageAvail>(VmPublishingModel model) where TEntity  : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new() where TLanguageAvail : class, ILanguageAvailability
        {
            return contextManager.ExecuteWriter(unitOfWork => PublishEntity<TEntity, TLanguageAvail>(unitOfWork, model));
        }

        public PublishingResult PublishEntity<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, VmPublishingModel model)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            if (model.LanguagesAvailabilities.Select(i => i.StatusId).All(i => i != psPublished))
            {
                throw new PublishLanguageException();
            }
            var serviceRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entity = serviceRep.All().Single(x => x.Id == model.Id);
            var targetStatus = (entity.PublishingStatusId == psOldPublished || entity.PublishingStatusId == psDeleted)
                ? PublishingStatus.Modified
                : PublishingStatus.Published;
            var affected = versioningManager.PublishVersion(unitOfWork, entity, targetStatus);
            versioningManager.ChangeStatusOfLanguageVersion<TEntity, TLanguageAvail>(unitOfWork, entity, model.LanguagesAvailabilities);
            unitOfWork.Save();
            var processedEntityResults = affected.First(i => i.Id == model.Id);
            return new PublishingResult
            {
                AffectedEntities = affected,
                Id = processedEntityResults.Id,
                PublishingStatusOld = processedEntityResults.PublishingStatusOld,
                PublishingStatusNew = processedEntityResults.PublishingStatusNew,
                Version = new VmVersion()
                {
                    Major = entity.Versioning.VersionMajor,
                    Minor = entity.Versioning.VersionMinor
                }
            };

        }

        private VmPublishingResultModel ChangeEntityToModified<TEntity>(Guid entityVersionedId, Func<IUnitOfWork, TEntity,bool> additionalCheckAction = null) where TEntity : class, IEntityIdentifier, IVersionedVolume
        {
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var serviceRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
                var entity = serviceRep.All().Include(j => j.Versioning).Single(x => x.Id == entityVersionedId);
                if (additionalCheckAction != null)
                {
                    if (!additionalCheckAction(unitOfWork, entity))
                    {
                        return new VmPublishingResultModel()
                        {
                            Id = entityVersionedId,
                            PublishingStatusId = entity.PublishingStatusId,
                            Version = new VmVersion() {Major = entity.Versioning.VersionMajor, Minor = entity.Versioning.VersionMinor}
                        };
                    }
                }
                var affected = versioningManager.ChangeToModified(unitOfWork, entity, new List<PublishingStatus>(){ PublishingStatus.Deleted, PublishingStatus.OldPublished, PublishingStatus.Published});
                unitOfWork.Save();
                var result = new VmPublishingResultModel()
                {
                    Id = entityVersionedId,
                    PublishingStatusId = affected.FirstOrDefault(i => i.Id == entityVersionedId)?.PublishingStatusNew,
                    Version = new VmVersion() {Major = entity.Versioning.VersionMajor, Minor = entity.Versioning.VersionMinor}
                };
                return result;
            });
        }

        public VmPublishingResultModel WithdrawEntity<TEntity>(Guid entityVersionedId) where TEntity : class, IEntityIdentifier, IVersionedVolume
        {
            try
            {
                return ChangeEntityToModified<TEntity>(entityVersionedId);
            }
            catch (InvalidOperationException)
            {
                throw new WithdrawModifiedExistsException();
            }
        }

        public VmPublishingResultModel WithdrawEntityByRootId<TEntity>(Guid rootId) where TEntity : class, IEntityIdentifier, IVersionedVolume, IPublishingStatus
        {
            TEntity entity = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                // Get right version id
                var entityId = versioningManager.GetVersionId<TEntity>(unitOfWork, rootId);
                var repo = unitOfWork.GetSet<TEntity>();
                entity = repo.Where(e => e.Id == entityId).FirstOrDefault();
            });

            if (entity == null)
            {
                return null;
            }

            if (entity.PublishingStatusId == typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString()))
            {
                throw new ModifiedExistsException("Latest version of item is already in Modified state. No actions needed!", null);
            }

            return WithdrawEntity<TEntity>(entity.Id);
            
        }

        public VmPublishingResultModel RestoreEntity<TEntity>(Guid entityVersionedId, Func<IUnitOfWork, TEntity, bool> additionalCheckAction = null) where TEntity : class, IEntityIdentifier, IVersionedVolume
        {
            try
            {
                return ChangeEntityToModified<TEntity>(entityVersionedId, additionalCheckAction);
            }
            catch (InvalidOperationException)
            {
                throw new RestoreModifiedExistsException();
            }
        }

        public VmEnvironmentInstructionsOut SaveEnvironmentInstructions(VmEnvironmentInstructionsIn model)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var instructionType = typesCache.Get<AppEnvironmentDataType>(AppEnvironmentDataTypeEnum.EnvironmentInstruction.ToString());
                var appDataRepository = unitOfWork.CreateRepository<IAppEnvironmentDataRepository>();
                var lastVersion = appDataRepository.All().Where(x => x.TypeId == instructionType).OrderByDescending(x => x.Version).FirstOrDefault()?.Version ?? 0;
                model.Version = ++lastVersion;
                appDataRepository.Add(TranslationManagerToEntity.Translate<VmEnvironmentInstructionsIn, AppEnvironmentData>(model, unitOfWork));
                unitOfWork.Save();
            });
            return GetEnvironmentInstructions();
        }

        public VmEnvironmentInstructionsOut GetEnvironmentInstructions()
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var instructionType = typesCache.Get<AppEnvironmentDataType>(AppEnvironmentDataTypeEnum.EnvironmentInstruction.ToString());
                var appDataRepository = unitOfWork.CreateRepository<IAppEnvironmentDataRepository>();
                // take last two newest changes
                var instructions = appDataRepository.All().Where(x => x.TypeId == instructionType).OrderByDescending(x => x.Version).Take(2);
                return new VmEnvironmentInstructionsOut()
                {
                    Instructions = TranslationManagerToVm.TranslateAll<AppEnvironmentData, VmEnvironmentInstruction>(instructions)
                };
            });
        }

        private void AddCoStatus(IList<Guid> statuses, Guid statusOne, Guid statusTwo)
        {
            if (statuses.Contains(statusOne) && !statuses.Contains(statusTwo)) statuses.Add(statusTwo);
            if (statuses.Contains(statusTwo) && !statuses.Contains(statusOne)) statuses.Add(statusOne);
        }

        public void ExtendPublishingStatusesByEquivalents(IList<Guid> statuses)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var psDraft = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var psModified = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            AddCoStatus(statuses, psDeleted, psOldPublished);
            AddCoStatus(statuses, psDraft, psModified);
        }

        public PublishingResult PublishAllAvailableLanguageVersions<TEntity, TLanguageAvail>(Guid Id, Expression<Func<TLanguageAvail, bool>> getSelectedIdFunc) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new() where TLanguageAvail : class, ILanguageAvailability
        {
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var vmLanguages = new List<VmLanguageAvailabilityInfo>();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var repository = unitOfWork.CreateRepository<IRepository<TLanguageAvail>>();
                var languages = repository.All().Where(getSelectedIdFunc).Select(i => i.LanguageId).ToList();
                languages.ForEach(l => vmLanguages.Add(new VmLanguageAvailabilityInfo() { LanguageId = l, StatusId = psPublished }));
            });

            return PublishEntity<TEntity, TLanguageAvail>(new VmPublishingModel
            {
                Id = Id,
                LanguagesAvailabilities = vmLanguages
            });
        }

        public bool OrganizationExists(Guid id, PublishingStatus? requiredStatus = null)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                if (requiredStatus.HasValue)
                {
                    var statusId = typesCache.Get<PublishingStatusType>(requiredStatus.ToString());
                    return organizationRep.All().Any(o => o.UnificRootId.Equals(id) && o.PublishingStatusId == statusId);
                }

                return organizationRep.All().Any(o => o.UnificRootId.Equals(id));
            });
        }

        public IList<PublishingAffectedResult> RestoreArchivedEntity<TEntity>(IUnitOfWorkWritable unitOfWork, Guid versionId) where TEntity : class, IEntityIdentifier, IVersionedVolume, new()
        {
            var rep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entity = rep.All().Single(x => x.Id == versionId);

            return versioningManager.PublishVersion(unitOfWork, entity, PublishingStatus.Modified);
        }
        public bool IsDescriptionEnumType(Guid typeId, string type)
        {
            return typesCache.Compare<DescriptionType>(typeId, type);
        }

        public Guid GetDescriptionTypeId(string code)
        {
            return typesCache.Get<DescriptionType>(code);
        }

        public VmListItemsData<VmListItem> GetServiceChannelConnectionTypes()
        {
            return dataServiceFetcher.FetchType<ServiceChannelConnectionType>();
        }

        public VmListItemsData<VmListItem> GetServiceFundingTypes()
        {
            return dataServiceFetcher.FetchType<ServiceFundingType>();
        }

        public List<Guid> GetUserOrganizations(Guid userOrganization)
        {
            var list = new List<Guid>();
            contextManager.ExecuteReader(unitOfWork =>
            {
                list = GetOrganizationRootIdsFlatten(unitOfWork, userOrganization);
            });

            return list;
        }

        public IVmOpenApiGuidPageVersionBase GetServcesAndChannelsByOrganization(Guid organizationId, DateTime? date, int pageNumber, int pageSize)
        {
            var vm = new VmOpenApiEntityGuidPage(pageNumber, pageSize);
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            contextManager.ExecuteReader(unitOfWork =>
            {
                // Get services related to given organization
                // Main responsible organizations
                var serviceIds = unitOfWork.CreateRepository<IServiceVersionedRepository>().All()
                    .Where(s => s.OrganizationId == organizationId).Select(o => o.Id).ToList();
                // Other responsible organizations
                serviceIds.AddRange(unitOfWork.CreateRepository<IOrganizationServiceRepository>().All()
                    .Where(o => o.OrganizationId == organizationId).Select(o => o.ServiceVersionedId).ToList());
                // Producers
                serviceIds.AddRange(unitOfWork.CreateRepository<IServiceProducerOrganizationRepository>().All()
                    .Where(o => o.OrganizationId == organizationId).Select(o => o.ServiceProducer.ServiceVersionedId).ToList());

                // Get services
                var servicesQuery = unitOfWork.CreateRepository<IServiceVersionedRepository>().All()
                    .Where(s => serviceIds.Contains(s.Id) && s.PublishingStatusId == publishedId && s.LanguageAvailabilities.Any(l => l.StatusId == publishedId));

                // Get channels related to given organization
                var channelQuery = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>().All()
                    .Where(c => c.OrganizationId.Equals(organizationId) && c.PublishingStatusId == publishedId && c.LanguageAvailabilities.Any(l => l.StatusId == publishedId));
                if (date.HasValue)
                {
                    servicesQuery = servicesQuery.Where(service => service.Modified > date.Value);
                    channelQuery = channelQuery.Where(serviceChannel => serviceChannel.Modified > date.Value);
                }

                var entities = new List<EntityBase>();
                entities.AddRange(servicesQuery.ToList());
                entities.AddRange(channelQuery.ToList());
                SetItemPage(vm, entities.AsQueryable());
            });

            return vm;
        }
    }
}
