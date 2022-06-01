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
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Logic;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Localization;
using PTV.Domain.Model.Models.Security;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IEnumService), RegisterType.Transient)]
    internal class EnumService : ServiceBase, IEnumService
    {
        private readonly IContextManager contextManager;
        private readonly IServiceClassCacheInternal serviceClassCache;
        private readonly ITargetGroupCacheInternal targetGroupCache;
        private readonly IDigitalAuthorizationCacheInternal digitalAuthorizationCache;
        private readonly ILifeEventCacheInternal lifeEventCache;
        private readonly IDataServiceFetcher dataServiceFetcher;
        private readonly ILanguageCache languageCache;
        private readonly IPahaTokenAccessor pahaTokenAccessor;
        private readonly IResolveManager resolveManager;
        private readonly IIndustrialClassCacheInternal industrialClassCache;
        private readonly ITreeTools treeTools;
        private readonly IAreaCache areaCache;
        private readonly IDialCodeCache dialCodeCache;
        private readonly IMunicipalityCache municipalityCache;
        private readonly ITranslationCompanyCache translationCompanyCache;
        private readonly IHolidayDateCache holidayDateCache;
        private readonly IServiceNumberCache serviceNumberCache;
        private readonly ILanguageOrderCache languageOrderCache;

        public EnumService(
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IVersioningManager versioningManager,
            IContextManager contextManager,
            IServiceClassCacheInternal serviceClassCache,
            ITargetGroupCacheInternal targetGroupCache,
            IDigitalAuthorizationCacheInternal digitalAuthorizationCache,
            ILifeEventCacheInternal lifeEventCache,
            IDataServiceFetcher dataServiceFetcher,
            ILanguageCache languageCache,
            ILanguageOrderCache languageOrderCache,
            IPahaTokenAccessor pahaTokenAccessor,
            IResolveManager resolveManager,
            IIndustrialClassCacheInternal industrialClassCache,
            ITreeTools treeTools,
            IAreaCache areaCache,
            IDialCodeCache dialCodeCache,
            IMunicipalityCache municipalityCache,
            ITranslationCompanyCache translationCompanyCache,
            IHolidayDateCache holidayDateCache,
            IServiceNumberCache serviceNumberCache
        )
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker,
                versioningManager)
        {
            this.contextManager = contextManager;
            this.serviceClassCache = serviceClassCache;
            this.targetGroupCache = targetGroupCache;
            this.digitalAuthorizationCache = digitalAuthorizationCache;
            this.lifeEventCache = lifeEventCache;
            this.dataServiceFetcher = dataServiceFetcher;
            this.languageCache = languageCache;
            this.languageOrderCache = languageOrderCache;
            this.pahaTokenAccessor = pahaTokenAccessor;
            this.resolveManager = resolveManager;
            this.industrialClassCache = industrialClassCache;
            this.treeTools = treeTools;
            this.areaCache = areaCache;
            this.dialCodeCache = dialCodeCache;
            this.municipalityCache = municipalityCache;
            this.translationCompanyCache = translationCompanyCache;
            this.holidayDateCache = holidayDateCache;
            this.serviceNumberCache = serviceNumberCache;
        }

        public IVmDictionaryItemsData<IEnumerable<IVmBase>> GetEnumTypes(VmUserInfoBase userInfo)
        {
            var serviceClasses = TranslationManagerToVm.TranslateAll<ServiceClass, VmListItem>(serviceClassCache.GetAllValid()
                .OrderBy(x => x.Label));
            var topTargetGroups = CreateTree<VmExpandedVmTreeItem>(targetGroupCache.GetTopLevel(), x => x.Code);
            var topLifeEvents = CreateTree<VmTreeItem>(lifeEventCache.GetTopLevel(), x => x.Name);
            var topServiceClasses = CreateTree<VmTreeItem>(serviceClassCache.GetTopLevel(), x => x.Name);
            var topDigitalAuthorizations =
                CreateTree<VmExpandedVmTreeItem>(treeTools.LoadFintoTree(digitalAuthorizationCache.GetTree()), //for this case select all digital authorization
                    x => x.Name);
            
            // Industrial classes
            var validIndustrialClasses = industrialClassCache.GetAllValid();
            var industrialClassesTree = BuildIndustrialClassesTree(validIndustrialClasses);
            var industrialClasses = treeTools.ReplaceLevelByLevel(industrialClassesTree, 2, 4);
            
            var targetGroups = CreateTree<VmExpandedVmTreeItem>(targetGroupCache.GetTree(), x => x.Code);
            var businessRegions = TranslationManagerToVm.TranslateAll<Area, VmListItemReferences>(areaCache.GetBusinessRegions()).OrderBy(x => x.Name).ToList();
            var provinces = TranslationManagerToVm.TranslateAll<Area, VmListItemReferences>(areaCache.GetProvinces()).OrderBy(x => x.Name).ToList();
            var hospitalRegions = TranslationManagerToVm.TranslateAll<Area, VmListItemReferences>(areaCache.GetHospitalRegions()).OrderBy(x => x.Name).ToList();
            var dialCodes = TranslationManagerToVm.TranslateAll<DialCode, VmDialCode>(dialCodeCache.GetDialCodes());
            
            var municipalities = TranslationManagerToVm
                .TranslateAll<Municipality, VmListItem>(municipalityCache.GetAll().Where(x => x.IsValid))
                .OrderBy(x => x.Name).ToList();
            var translationCompanies = TranslationManagerToVm.TranslateAll<TranslationCompany, VmTranslationCompany>(translationCompanyCache.GetAll());
            var holidayDates = TranslationManagerToVm
                .TranslateAll<HolidayDate, VmHolidayDateListItem>(holidayDateCache.GetAll().OrderBy(x => x.Date).ToList());
            var serviceNumbers = TranslationManagerToVm.TranslateAll<ServiceNumber, VmServiceNumber>(serviceNumberCache.GetAll().ToList());

            var orgTypes = new List<VmExpandedVmTreeItem>();
            var organizationAreaInformations = new List<IVmAreaInformation>();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var organizationTypeRep = unitOfWork.CreateRepository<IOrganizationTypeRepository>();
                orgTypes = CreateTree<VmExpandedVmTreeItem>(
                    treeTools.LoadFintoTree(
                        GetIncludesForFinto<OrganizationType, OrganizationTypeName>(unitOfWork,
                            organizationTypeRep.All().Where(x=>x.IsValid)).ToList()), x => x.Name).ToList();
                orgTypes.ForEach(x => x.IsDisabled = x.Children.Any());

                // filter organizations types (SFIPTV-689)
                var organizationTypesChildren =
                    orgTypes.SelectMany(ot => ot.Children).OfType<VmExpandedVmTreeItem>().ToList();

                using (var scope = resolveManager.CreateScope())
                {
                    var restrictionFilterManager = scope.ServiceProvider.GetService<IRestrictionFilterManager>();
                    restrictionFilterManager.SetAccessForGuidTypes<OrganizationType>(Guid.Empty, organizationTypesChildren);
                }

                organizationAreaInformations = GetDefaultAreaInformationList(userInfo.UserOrganization, unitOfWork).ToList();
            });

            var result = new VmEnumBase();
            FillEnumEntities(result,
                () => GetEnumEntityCollectionModel("AreaInformationTypes", dataServiceFetcher.FetchType<AreaInformationType>()),
                () => GetEnumEntityCollectionModel("AreaTypes", dataServiceFetcher.FetchType<AreaType>()),
                () => GetEnumEntityCollectionModel("AstiTypes", dataServiceFetcher.FetchType<ExtraSubType>()),
                () => GetEnumEntityCollectionModel("BusinessRegions", businessRegions),
                () => GetEnumEntityCollectionModel("ChargeTypes", dataServiceFetcher.FetchType<ServiceChargeType>()),
                () => GetEnumEntityCollectionModel("ServiceChannelConnectionTypes", dataServiceFetcher.FetchType<ServiceChannelConnectionType>()),
                () => GetEnumEntityCollectionModel("DialCodes", dialCodes),
                () => GetEnumEntityCollectionModel("HospitalRegions", hospitalRegions),
                () => GetEnumEntityCollectionModel("Languages", GetLanguages()),
                () => GetEnumEntityCollectionModel("TranslationLanguages", GetTranslationLanguages()),
                () => GetEnumEntityCollectionModel("TranslationOrderLanguages", GetTranslationOrderLanguages()),
                () => GetEnumEntityCollectionModel("Municipalities", municipalities),
                () => GetEnumEntityCollectionModel("ServiceClasses", serviceClasses),
                () => GetEnumEntityCollectionModel("Provinces", provinces),
                () => GetEnumEntityCollectionModel("PublishingStatuses", GetPublishingStatuses()),
                () => GetEnumEntityCollectionModel("ServiceTypes", dataServiceFetcher.FetchType<ServiceType>()),
                () => GetEnumEntityCollectionModel("GeneralDescriptionTypes", GetGeneralDescriptionTypes()),
                () => GetEnumEntityCollectionModel("TopTargetGroups", topTargetGroups),
                () => GetEnumEntityCollectionModel("TopLifeEvents", topLifeEvents),
                () => GetEnumEntityCollectionModel("TopServiceClasses", topServiceClasses),
                () => GetEnumEntityCollectionModel("TopDigitalAuthorizations", topDigitalAuthorizations),
                () => GetEnumEntityCollectionModel("IndustrialClasses", industrialClasses),
                () => GetEnumEntityCollectionModel("PhoneNumberTypes", dataServiceFetcher.FetchType<PhoneNumberType>()),
                () => GetEnumEntityCollectionModel("FundingTypes", dataServiceFetcher.FetchType<ServiceFundingType>()),
                () => GetEnumEntityCollectionModel("ChannelTypes", dataServiceFetcher.FetchType<ServiceChannelType>()),
                () => GetEnumEntityCollectionModel("ProvisionTypes", dataServiceFetcher.FetchType<ProvisionType>()),
                () => GetEnumEntityCollectionModel("TargetGroups", targetGroups),
                () => GetEnumEntityCollectionModel("OrganizationTypes", orgTypes),
                () => GetEnumEntityCollectionModel("PrintableFormUrlTypes", dataServiceFetcher.FetchType<PrintableFormChannelUrlType>()),
                () => GetEnumEntityCollectionModel("OrganizationAreaInformations", organizationAreaInformations),
                () => GetEnumEntityCollectionModel("TranslationStateTypes", dataServiceFetcher.FetchType<TranslationStateType>()),
                () => GetEnumEntityCollectionModel("TranslationCompanies", translationCompanies),
                () => GetEnumEntityCollectionModel("ExtraTypes", dataServiceFetcher.FetchType<ExtraType>()),
                () => GetEnumEntityCollectionModel("ServerConstants", GetServiceConstants()),
                () => GetEnumEntityCollectionModel("AccessibilityClassificationLevelTypes", dataServiceFetcher.FetchType<AccessibilityClassificationLevelType>()),
                () => GetEnumEntityCollectionModel("WcagLevelTypes", dataServiceFetcher.FetchType<WcagLevelType>()),
                () => GetEnumEntityCollectionModel("Holidays", dataServiceFetcher.FetchType<Holiday>().OrderBy(x=>x.OrderNumber)),
                () => GetEnumEntityCollectionModel("HolidayDates", holidayDates),
                () => GetEnumEntityCollectionModel("ServiceNumbers", serviceNumbers),
                () => GetEnumEntityCollectionModel("JobStatusTypes", dataServiceFetcher.FetchType<JobStatusType>()),
                () => GetEnumEntityCollectionModel("JobExecutionTypes", dataServiceFetcher.FetchType<JobExecutionType>()),
                () => GetEnumEntityCollectionModel("VoucherTypes", dataServiceFetcher.FetchType<VoucherType>())
                );

            return result.EnumCollection;
        }

        #region Workaround methods to improve performance (Translators are too slow)
        private IEnumerable<VmExpandedVmTreeItem> BuildIndustrialClassesTree(List<IndustrialClass> entities)
        {
            var models = new List<VmExpandedVmTreeItem>();
            var parentIds = entities.Where(x => x.ParentId == null).Select(x => x.Id).ToList();
            var children = new Dictionary<Guid, List<IVmTreeItem>>();

            foreach (var entity in entities)
            {
                var model = MapToModel(entity);
                models.Add(model);

                if (entity.ParentId == null)
                {
                    continue;
                }

                AddToChildren(children, entity.ParentId.Value, model);
            }

            foreach (var model in models)
            {
                var relatedChildren = children.TryGetOrDefault(model.Id);
                model.Children = relatedChildren;
                model.IsLeaf = relatedChildren.IsNullOrEmpty();
            }

            return models.Where(x => parentIds.Contains(x.Id));
        }

        private static void AddToChildren(Dictionary<Guid, List<IVmTreeItem>> children, Guid parentId, VmExpandedVmTreeItem model)
        {
            if (!children.ContainsKey(parentId) || children[parentId] == null)
            {
                children.Add(parentId, new List<IVmTreeItem>());
            }

            children[parentId].Add(model);
        }

        private VmExpandedVmTreeItem MapToModel(IndustrialClass entity)
        {
            return new VmExpandedVmTreeItem
            {
                Children = new List<IVmTreeItem>(),
                Code = entity.Code,
                Id = entity.Id,
                Invalid = !entity.IsValid,
                Name = entity.Label,
                Description = entity.Descriptions
                    .Select(x => (Name: languageCache.GetByValue(x.LocalizationId), x.Description))
                    .ToDictionary(x => x.Name, x => x.Description),
                Translation = new VmTranslationItem
                {
                    Id = entity.Id,
                    Texts = entity.Names.GroupBy(x => x.LocalizationId)
                        .ToDictionary(n => languageCache.GetByValue(n.Key),
                            n => n.Select(x => x.Name).FirstOrDefault()),
                    DefaultText = entity.Names.OrderBy(x => languageOrderCache.Get(x.LocalizationId))
                        .FirstOrDefault()
                        ?.Name
                },
                AreChildrenLoaded = true
            };
        }

        #endregion

        public VmListItemsData<VmEnumType> GetLanguages()
        {
            return dataServiceFetcher.FetchType<Language>();
        }

        public IReadOnlyList<VmListItem> GetTranslationLanguages()
        {
            var tCodes = languageCache.AllowedLanguageCodes;
            return GetLanguages().Where(x => tCodes.Contains(x.Code)).ToList();
        }

        public IReadOnlyList<VmListItem> GetTranslationOrderLanguages()
        {
            var tCodes = languageCache.TranslationOrderLanguageCodes;
            return GetLanguages().Where(x => tCodes.Contains(x.Code)).ToList();
        }

        public IVmListItemsData<IVmRestrictableType> GetPublishingStatuses()
        {
            return new VmListItemsData<IVmRestrictableType>(dataServiceFetcher.FetchType<PublishingStatusType>()
                .Select(i => new VmPublishingStatus(i)
                    {
                        Type = i.Code.Parse<PublishingStatus>()
                    }));
        }

        public VmListItemsData<VmEnumType> GetGeneralDescriptionTypes()
        {
            var gdTypes = dataServiceFetcher.FetchType<GeneralDescriptionType>();
            if (pahaTokenAccessor.UserRole != UserRoleEnum.Eeva)
            {
                using (var scope = resolveManager.CreateScope())
                {
                    var restrictionFilterManager = scope.ServiceProvider.GetService<IRestrictionFilterManager>();
                    restrictionFilterManager.SetAccessForGuidTypes<GeneralDescriptionType>(pahaTokenAccessor.ActiveOrganizationId, gdTypes);
                }
            }

            return gdTypes;
        }

        public IReadOnlyList<VmParser> GetServiceConstants()
        {
            return new List<VmParser>
            {
                new VmParser
                {
                    Code = nameof(DomainConstants.OidParser),
                    Pattern = DomainConstants.OidParser
                },
                new VmParser()
                {
                    Code = "DefaultDialCodeId",
                    Pattern = dialCodeCache.GetDefaultDialCode().Id.ToString()
                }
            };
        }

        private IEnumerable<IVmAreaInformation> GetDefaultAreaInformationList(Guid? organizationId,
            IUnitOfWork unitOfWork)
        {
            if (!organizationId.IsAssigned())
            {
                return new List<IVmAreaInformation>();
            }

            return new List<IVmAreaInformation>
            {
                GetAreaInformationForOrganization(organizationId.Value, unitOfWork)
            };
        }

        private IVmAreaInformation GetAreaInformationForOrganization(Guid organizationId, IUnitOfWork unitOfWork)
        {
            var result = new VmOrganizationAreaInformation();
            var orgId = VersioningManager.GetVersionId<OrganizationVersioned>(unitOfWork, organizationId, null, false);
            if (orgId.HasValue)
            {
                var organization = GetEntity<OrganizationVersioned>(orgId, unitOfWork,
                    q => q
                        .Include(x => x.OrganizationAreaMunicipalities)
                        .Include(x => x.OrganizationAreas).ThenInclude(x => x.Area)
                );

                result =
                    TranslationManagerToVm
                        .Translate<OrganizationVersioned, VmOrganizationAreaInformation>(organization);
            }

            return result;
        }
    }
}
