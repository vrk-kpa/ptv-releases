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
using System.Collections;
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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Services.Validation;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.DataAccess.Repositories;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework.Extensions;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Database.Model.Models.Base;
using PTV.Database.Model.ServiceDataHolders;
using PTV.Domain.Logic;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.Security;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using ILogger = NLog.ILogger;
using PTV.Database.DataAccess.Utils.OpenApi;
using PTV.Domain.Model.Models.V2;
using PTV.Domain.Model.Models.V2.Mass;
using ServiceCollection = PTV.Database.Model.Models.ServiceCollection;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ICommonService), RegisterType.Transient)]
    [RegisterService(typeof(ICommonServiceInternal), RegisterType.Transient)]
    internal class CommonService : ServiceBase, ICommonService, ICommonServiceInternal
    {
        private static readonly List<string> SelectedPublishingStatuses = new List<string>() { PublishingStatus.Draft.ToString(), PublishingStatus.Published.ToString() };

        private readonly IContextManager contextManager;
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;
        private readonly IDataServiceFetcher dataServiceFetcher;
        private readonly IServiceUtilities utilities;
        private readonly IValidationManager validationManager;
        private readonly ILanguageOrderCache languageOrderCache;
        private readonly ApplicationConfiguration configuration;
        private IOrganizationTreeDataCache organizationTreeDataCache;
        private IRestrictionFilterManager restrictionFilterManager;
        private IPahaTokenProcessor pahaTokenProcessor;
        private ILogger<CommonService> logger;
        private IIndustrialClassCacheInternal industrialClassCache;

        public CommonService(
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IContextManager contextManager,
            ITypesCache typesCache,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IDataServiceFetcher dataServiceFetcher,
            IServiceUtilities utilities,
            IVersioningManager versioningManager,
            IValidationManager validationManager,
            ApplicationConfiguration configuration,
            IOrganizationTreeDataCache organizationTreeDataCache,
            IRestrictionFilterManager restrictionFilterManager,
            IPahaTokenProcessor pahaTokenProcessor,
            ILanguageCache languageCache,
            ILogger<CommonService> logger,
            ILanguageOrderCache languageOrderCache,
            IIndustrialClassCacheInternal industrialClassCache)
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker, versioningManager)
        {
            this.contextManager = contextManager;
            this.typesCache = typesCache;
            this.dataServiceFetcher = dataServiceFetcher;
            this.utilities = utilities;
            this.configuration = configuration;
            this.organizationTreeDataCache = organizationTreeDataCache;
            this.restrictionFilterManager = restrictionFilterManager;
            this.pahaTokenProcessor = pahaTokenProcessor;
            this.languageCache = languageCache;
            this.logger = logger;
            this.validationManager = validationManager;
            this.languageOrderCache = languageOrderCache;
            this.industrialClassCache = industrialClassCache;
        }

        public IVmGetFrontPageSearch GetFrontPageSearch()
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var publishingStatuses = GetPublishingStatuses();
                var result = new VmGetFrontPageSearch
                {
                    SelectedPublishingStatuses = publishingStatuses.Where(x => SelectedPublishingStatuses.Contains(x.Code)).Select(x => x.Id).ToList()
                };

                return result;
            });
        }
        public IVmDictionaryItemsData<IEnumerable<IVmBase>> GetOrganizationEnum()
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var result = new VmEnumBase();
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", GetOrganizationNames()));
                return result.EnumCollection;
            });
        }
        public IVmDictionaryItemsData<IEnumerable<IVmBase>> GetEnumTypes(VmUserInfoBase userInfo)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceClassesRep = unitOfWork.CreateRepository<IServiceClassRepository>();
                var targetGroupRep = unitOfWork.CreateRepository<ITargetGroupRepository>();
                var serviceClasses = TranslationManagerToVm.TranslateAll<ServiceClass, VmListItem>(GetIncludesForFinto<ServiceClass, ServiceClassName>(unitOfWork, serviceClassesRep.All().Where(x => x.IsValid).OrderBy(x => x.Label)));
                var organizationTypeRep = unitOfWork.CreateRepository<IOrganizationTypeRepository>();
                var orgTypes = CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<OrganizationType, OrganizationTypeName>(unitOfWork, organizationTypeRep.All())), x => x.Name).ToList();
                orgTypes.ForEach(x => x.IsDisabled = x.Children.Any());
                
                // filter organizations types (SFIPTV-689)
                var organizationTypesChildren = orgTypes.SelectMany(ot => ot.Children).OfType<VmExpandedVmTreeItem>().ToList();
                restrictionFilterManager.SetAccessForGuidTypes<OrganizationType>(Guid.Empty, organizationTypesChildren);
                
                var targetGroups = CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<TargetGroup, TargetGroupName>(unitOfWork, targetGroupRep.All().Where(x => x.IsValid).OrderBy(x => x.Label))), x => x.Code);

                var digitalAuthorizationRep = unitOfWork.CreateRepository<IDigitalAuthorizationRepository>();
//                var digitalAuthorization =
//                    TranslationManagerToVm
//                        .TranslateAll<DigitalAuthorization, VmListItem>(
//                            GetIncludesForFinto<DigitalAuthorization, DigitalAuthorizationName>(unitOfWork, digitalAuthorizationRep.All())
//                        );
                var lifeEventRep = unitOfWork.CreateRepository<ILifeEventRepository>();
//                var keyWordRep = unitOfWork.CreateRepository<IKeywordRepository>();
                var result = new VmEnumBase();
                var publishingStatuses = GetPublishingStatuses();
                
                var industrialClasses = TranslationManagerToVm.TranslateAll<IFintoItem, VmTreeItem>(industrialClassCache.GetAllForLastLevel());

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("AreaInformationTypes", GetAreaInformationTypes()),
                    () => GetEnumEntityCollectionModel("AreaTypes", GetAreaTypes()),
                    () => GetEnumEntityCollectionModel("AstiTypes", GetExtraSubTypes()),
                    () => GetEnumEntityCollectionModel("BusinessRegions",
                        GetAreas(unitOfWork, AreaTypeEnum.BusinessRegions)),
//                    () => GetEnumEntityCollectionModel("DigitalAuthorizations", digitalAuthorization),
                    () => GetEnumEntityCollectionModel("ChargeTypes", GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("ServiceChannelConnectionTypes",
                        GetServiceChannelConnectionTypes()),
                    () => GetEnumEntityCollectionModel("DialCodes", GetDefaultDialCode(unitOfWork)),
                    () => GetEnumEntityCollectionModel("HospitalRegions",
                        GetAreas(unitOfWork, AreaTypeEnum.HospitalRegions)),
                    () => GetEnumEntityCollectionModel("Languages", GetLanguages()),
                    () => GetEnumEntityCollectionModel("TranslationLanguages", GetTranslationLanguages()),
                    () => GetEnumEntityCollectionModel("TranslationOrderLanguages", GetTranslationOrderLanguages()),
                    //() => GetEnumEntityCollectionModel("Keywords", TranslationManagerToVm.TranslateAll<Keyword, VmKeywordItem>(keyWordRep.All().OrderBy(x => x.Name))),
                    () => GetEnumEntityCollectionModel("Municipalities", GetMunicipalities(unitOfWork, false)),
//                    () => GetEnumEntityCollectionModel("Organizations", GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ServiceClasses", serviceClasses),
                    () => GetEnumEntityCollectionModel("Provinces", GetAreas(unitOfWork, AreaTypeEnum.Province)),
                    () => GetEnumEntityCollectionModel("PublishingStatuses", publishingStatuses),
                    () => GetEnumEntityCollectionModel("ServiceTypes", GetServiceTypes()),
                    () => GetEnumEntityCollectionModel("GeneralDescriptionTypes", GetGeneralDescriptionTypes()),
                    // todo: use cached data for TG
                    () => GetEnumEntityCollectionModel("TopTargetGroups",
                        CreateTree<VmExpandedVmTreeItem>(
                            LoadFintoTree(
                                GetIncludesForFinto<TargetGroup, TargetGroupName>(unitOfWork,
                                    targetGroupRep.All().OrderBy(x => x.Label)), 1), x => x.Code)),
                    () => GetEnumEntityCollectionModel("TopLifeEvents",
                        CreateTree<VmTreeItem>(
                            LoadFintoTree(GetIncludesForFinto<LifeEvent, LifeEventName>(unitOfWork, lifeEventRep.All()),
                                1), x => x.Name)),
                    () => GetEnumEntityCollectionModel("TopServiceClasses",
                        CreateTree<VmTreeItem>(
                            LoadFintoTree(
                                GetIncludesForFinto<ServiceClass, ServiceClassName>(unitOfWork,
                                    serviceClassesRep.All().Include(x => x.Descriptions)), 1), x => x.Name)),
                    () => GetEnumEntityCollectionModel("TopDigitalAuthorizations",
                        CreateTree<VmExpandedVmTreeItem>(
                            LoadFintoTree(
                                GetIncludesForFinto<DigitalAuthorization, DigitalAuthorizationName>(unitOfWork,
                                    digitalAuthorizationRep.All())), x => x.Name)),
                    () => GetEnumEntityCollectionModel("IndustrialClasses", industrialClasses),
                    () => GetEnumEntityCollectionModel("PhoneNumberTypes", GetPhoneTypes()),
                    () => GetEnumEntityCollectionModel("FundingTypes", GetServiceFundingTypes()),
                    () => GetEnumEntityCollectionModel("ChannelTypes", GetServiceChannelTypes()),
                    () => GetEnumEntityCollectionModel("ProvisionTypes", GetProvisionTypes()),
                    () => GetEnumEntityCollectionModel("TargetGroups", targetGroups),
                    () => GetEnumEntityCollectionModel("OrganizationTypes", orgTypes),
                    () => GetEnumEntityCollectionModel("PrintableFormUrlTypes", GetPrintableFormUrlTypes()),
                    () => GetEnumEntityCollectionModel("OrganizationAreaInformations",
                        GetDefaultAreaInformationList(userInfo.UserOrganization, unitOfWork)),
                    () => GetEnumEntityCollectionModel("TranslationStateTypes", GetTranslationStateTypes()),
                    () => GetEnumEntityCollectionModel("TranslationCompanies", GetTranslationCompanies(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ExtraTypes", GetExtraTypes()),
                    () => GetEnumEntityCollectionModel("ServerConstants", GetServiceConstants()),
                    () => GetEnumEntityCollectionModel("AccessibilityClassificationLevelTypes", GetAccessibilityClassificationLevelTypes()),
                    () => GetEnumEntityCollectionModel("WcagLevelTypes", GetWcagLevelTypes()));

            return result.EnumCollection;
            });
        }

        public IVmDictionaryItemsData<IEnumerable<IVmBase>> GetEnumTypesForLogin()
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var result = new VmEnumBase();
                FillEnumEntities(result,
//                    () => GetEnumEntityCollectionModel("Organizations", GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("UserAccessRightsGroups", GetUserAccessRightsGroups(unitOfWork))
                );

                return result.EnumCollection;
            });
        }

        public IVmBase GetTypedData(IEnumerable<string> dataTypes)
        {
            return new VmListItemsData<IVmBase>(dataServiceFetcher.Fetch(dataTypes));
        }

        public VmListItemsData<VmEnumType> GetExtraSubTypes()
        {
            return dataServiceFetcher.FetchType<ExtraSubType>();
        }
        
        public VmListItemsData<VmEnumType> GetExtraTypes()
        {
            return dataServiceFetcher.FetchType<ExtraType>();
        }

        public VmListItemsData<VmEnumType> GetPhoneChargeTypes()
        {
            return dataServiceFetcher.FetchType<ServiceChargeType>();
        }
 
        public VmListItemsData<VmEnumType> GetWebPageTypes()
        {
            return dataServiceFetcher.FetchType<WebPageType>();
        }
        
        public VmListItemsData<VmEnumType> GetGeneralDescriptionTypes()
        {
            var gdTypes = dataServiceFetcher.FetchType<GeneralDescriptionType>();
            if (pahaTokenProcessor.UserRole != UserRoleEnum.Eeva)
            {
                restrictionFilterManager.SetAccessForGuidTypes<GeneralDescriptionType>(pahaTokenProcessor.ActiveOrganization, gdTypes);
            }
            return gdTypes;
        }
        
        public VmListItemsData<VmEnumType> GetServiceTypes()
        {
            return dataServiceFetcher.FetchType<ServiceType>();
        }

        public VmListItemsData<VmEnumType> GetProvisionTypes()
        {
            return dataServiceFetcher.FetchType<ProvisionType>();
        }

        public VmListItemsData<VmEnumType> GetPrintableFormUrlTypes()
        {
            return dataServiceFetcher.FetchType<PrintableFormChannelUrlType>();
        }

        public VmListItemsData<VmEnumType> GetPhoneTypes()
        {
            return dataServiceFetcher.FetchType<PhoneNumberType>();
        }

        public VmListItemsData<VmEnumType> GetServiceHourTypes()
        {
            return dataServiceFetcher.FetchType<ServiceHourType>();
        }

        public VmListItemsData<VmEnumType> GetTranslationStateTypes()
        {
            return dataServiceFetcher.FetchType<TranslationStateType>();
        }

        public VmListItemsData<VmEnumType> GetPublishingStatuses()
        {
            return new VmListItemsData<VmEnumType>(dataServiceFetcher.FetchType<PublishingStatusType>().Select(i => new VmPublishingStatus(i)
            {
                Type = i.Code.Parse<PublishingStatus>()
            }));
        }

        public VmListItemsData<VmEnumType> GetCoordinateTypes()
        {
            return dataServiceFetcher.FetchType<CoordinateType>();
        }

        public VmListItemsData<VmEnumType> GetAreaInformationTypes()
        {
            return dataServiceFetcher.FetchType<AreaInformationType>();
        }

        public VmListItemsData<VmEnumType> GetAreaTypes()
        {
            return dataServiceFetcher.FetchType<AreaType>();
        }

        public IReadOnlyList<VmListItem> GetOrganizationNames(string searchText = null, bool takeAll = true, List<PublishingStatus> allowedPublishingStatuses = null)
        {
            // get enum values to local variables before using them inside LINQ queries as otherwise the provider does the queries in-memory and not in DB
            // as it cannot translate the enum values to db queries and this way we get better performance as the queries are evaluated in DB
            var resultTemp = ((IOrganizationTreeDataCacheWithEntity)organizationTreeDataCache).GetFlatDataEntities().Values.AsEnumerable();
          
            if (allowedPublishingStatuses != null && allowedPublishingStatuses.Any())
            {
                var translationStatusIds = allowedPublishingStatuses.Select(publishingStatus => typesCache.Get<PublishingStatusType>(publishingStatus.ToString())).ToList();
                resultTemp = resultTemp.Where(x => translationStatusIds.Contains(x.PublishingStatusId));
            }
            
            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.ToLower();
                resultTemp = resultTemp.Where(x => x.OrganizationNames.Any(n => !string.IsNullOrEmpty(n.Name) && n.Name.ToLower().Contains(searchText) && n.TypeId == x.OrganizationDisplayNameTypes.FirstOrDefault(type => type.LocalizationId == n.LocalizationId)?.DisplayNameTypeId));
            }
            if (!takeAll)
            {
                resultTemp = resultTemp.Take(CoreConstants.MaximumNumberOfAllItems);
            }
            return TranslationManagerToVm.TranslateAll<OrganizationVersioned, VmListItemWithStatus>(resultTemp);
        }

        public List<OrganizationTreeItem> GetOrganizationNamesTree(string searchText)
        {
            // get enum values to local variables before using them inside LINQ queries as otherwise the provider does the queries in-memory and not in DB
            // as it cannot translate the enum values to db queries and this way we get better performance as the queries are evaluated in DB
            var resultTemp = organizationTreeDataCache.GetData().Values.AsEnumerable();

            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.ToLower();
                resultTemp = resultTemp.Where(x => x.Organization.OrganizationNames.Any(n => !string.IsNullOrEmpty(n.Name) && n.Name.ToLower().Contains(searchText) && n.TypeId == x.Organization.OrganizationDisplayNameTypes.FirstOrDefault(type => type.LocalizationId == n.LocalizationId)?.DisplayNameTypeId));
            }

            return resultTemp.ToList();
        }
        
        public List<OrganizationTreeItem> GetOrganizationNamesTree(ICollection<Guid> ids = null)
        {
            var allOrgs = organizationTreeDataCache.GetData();
            if (ids == null)
            {
                return allOrgs.Values.Where(org => org.Parent == null).ToList();
            }
            return ids.Distinct().Select(x => allOrgs.TryGet(x)).Where(x => x != null).ToList();
        }
        
        public IReadOnlyList<VmListItem> GetOrganizations(IEnumerable<Guid> ids)
        {
            // get enum values to local variables before using them inside LINQ queries as otherwise the provider does the queries in-memory and not in DB
            // as it cannot translate the enum values to db queries and this way we get better performance as the queries are evaluated in DB
            var allOrgs = ((IOrganizationTreeDataCacheWithEntity)organizationTreeDataCache).GetFlatDataEntities();
            return TranslationManagerToVm.TranslateAll<OrganizationVersioned, VmListItemWithStatus>(ids.Distinct().Select(x => allOrgs.TryGet(x)).Where(x => x != null));
        }

        public IReadOnlyList<VmListItem> GetUserAccessRightsGroups(IUnitOfWork unitOfWork)
        {
            var uARGR = unitOfWork.CreateRepository<IUserAccessRightsGroupRepository>();

            var resultTemp = uARGR.All();

            resultTemp = unitOfWork.ApplyIncludes(resultTemp, q => q.Include(i => i.UserAccessRightsGroupNames));

            return TranslationManagerToVm.TranslateAll<UserAccessRightsGroup, VmListItem>(resultTemp);
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
        

        public IReadOnlyList<string> GetTranslationLanguageList()
        {
            return languageCache.AllowedLanguageCodes;
        }


        public VmListItemsData<VmEnumType> GetServiceChannelTypes()
        {
            return dataServiceFetcher.FetchType<ServiceChannelType>();
        }
        
        public IReadOnlyList<VmParser> GetServiceConstants()
        {
            return new List<VmParser>
            {
                new VmParser
                {
                    Code = nameof(DomainConstants.OidParser),
                    Pattern = DomainConstants.OidParser
                }
            };
        }

        public IReadOnlyList<VmListItem> GetMunicipalities(IUnitOfWork unitOfWork, bool onlyValid = true)
        {
            var municipalityRep = unitOfWork.CreateRepository<IMunicipalityRepository>();
            var qry = municipalityRep.All();
            if (onlyValid) qry = qry.Where(m => m.IsValid);
            var municipalities = unitOfWork.ApplyIncludes(qry, i => i.Include(j => j.MunicipalityNames));
            return TranslationManagerToVm.TranslateAll<Municipality, VmListItem>(municipalities).OrderBy(x => x.Name).ToList();
        }

        public IReadOnlyList<VmListItemReferences> GetAreas(IUnitOfWork unitOfWork, AreaTypeEnum type)
        {
            var areaRep = unitOfWork.CreateRepository<IAreaRepository>();
            var areas = unitOfWork.ApplyIncludes(areaRep.All().Where(x=>x.AreaTypeId == typesCache.Get<AreaType>(type.ToString())), i => i.Include(j => j.AreaNames).Include(j=>j.AreaMunicipalities));
            return TranslationManagerToVm.TranslateAll<Area, VmListItemReferences>(areas).OrderBy(x => x.Name).ToList();
        }

        public IList<VmOpenApiCodeListItem> GetAreaCodeList(AreaTypeEnum? type = null)
        {
            IReadOnlyList<VmOpenApiCodeListItem> result = new List<VmOpenApiCodeListItem>();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var areaRep = unitOfWork.CreateRepository<IAreaRepository>();
                var query = areaRep.All();

                if (type.HasValue)
                {
                    var typeId = typesCache.Get<AreaType>(type.Value.ToString());
                    query = query.Where(area => area.AreaTypeId == typeId);
                }

                var resultTemp = unitOfWork.ApplyIncludes(query, i => i.Include(j => j.AreaNames));
                resultTemp = resultTemp.OrderBy(x => x.Code);

                result = TranslationManagerToVm.TranslateAll<Area, VmOpenApiCodeListItem>(resultTemp);
            });

            return new List<VmOpenApiCodeListItem>(result);
        }

        public IReadOnlyList<VmTranslationCompany> GetTranslationCompanies(IUnitOfWork unitOfWork)
        {
            var companiesRep = unitOfWork.CreateRepository<ITranslationCompanyRepository>();
            return TranslationManagerToVm.TranslateAll<TranslationCompany, VmTranslationCompany>(companiesRep.All());
        }
        
        public VmListItemsData<VmEnumType> GetAccessibilityClassificationLevelTypes()
        {
            return dataServiceFetcher.FetchType<AccessibilityClassificationLevelType>();
        }
        
        public VmListItemsData<VmEnumType> GetWcagLevelTypes()
        {
            return dataServiceFetcher.FetchType<WcagLevelType>();
        }

        public IReadOnlyList<VmDialCode> GetDefaultDialCode(IUnitOfWork unitOfWork)
        {
            var defaultCountryCode = configuration.GetDefaultCountryCode();
            var dialCodeRep = unitOfWork.CreateRepository<IDialCodeRepository>();
            var defaultDialCodes = unitOfWork.ApplyIncludes(dialCodeRep.All().Where(x => x.Country.Code == defaultCountryCode.ToUpper()), i => i.Include(j => j.Country).ThenInclude(j => j.CountryNames));
            return TranslationManagerToVm.TranslateAll<DialCode, VmDialCode>(defaultDialCodes);
        }
        
        public Guid? GetDefaultDialCodeId(IUnitOfWork unitOfWork)
        {
            var defaultCountryCode = configuration.GetDefaultCountryCode();
            var dialCodeRep = unitOfWork.CreateRepository<IDialCodeRepository>();
            return dialCodeRep.All().Where(x => x.Country.Code.ToUpper() == defaultCountryCode.ToUpper()).Select(i => i.Id).FirstOrDefault();
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
            return DomainConstants.DefaultLanguage;
        }

        public Guid GetLocalizationId(string langCode)
        {
            return typesCache.Get<Language>((!string.IsNullOrEmpty(langCode) ? langCode : DomainConstants.DefaultLanguage));
        }

        public PublishingResult PublishEntity<TEntity, TLanguageAvail>(IVmLocalizedEntityModel model) 
            where TEntity  : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new() 
            where TLanguageAvail : class, ILanguageAvailability
        {
            return contextManager.ExecuteWriter(unitOfWork => PublishEntity<TEntity, TLanguageAvail>(unitOfWork, model));
        }

        public PublishingResult SchedulePublishArchiveEntity<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork,
            IVmLocalizedEntityModel model, bool updateHistory = true)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            var userName = unitOfWork.GetUserNameForAuditing();
            var currentDateTime = DateTime.UtcNow;
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var massModels = model.LanguagesAvailabilities
                .Where(language => language.StatusId == psPublished)
                .Select(language => new VmMassLanguageAvailabilityModel
            {
                Id = model.Id,
                LanguageId = language.LanguageId,
                Reviewed = currentDateTime,
                ReviewedBy = userName,
                Archived = currentDateTime,
                ArchivedBy = userName,
                ValidTo = language.ValidTo?.FromEpochTime(),
                ValidFrom = language.ValidFrom?.FromEpochTime(),
                PublishAction = PublishActionTypeEnum.SchedulePublishArchive,
                AllowSaveNull = true
            });
            var languageAvail = TranslationManagerToEntity.TranslateAll<VmMassLanguageAvailabilityModel, TLanguageAvail>(massModels, unitOfWork).ToList();
            if (updateHistory)
            {
                UpdateHistoryMetaData<TEntity, TLanguageAvail>(model.Id, languageAvail, unitOfWork);
            }
            unitOfWork.Save(preSaveAction: PreSaveAction.DoNotSetAudits);
            return new PublishingResult{Id = model.Id};
        }

        public PublishingResult PublishAndScheduleEntity<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork,
            IVmLocalizedEntityModel model)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            //Schedule entity -- not update history of previous version
            SchedulePublishArchiveEntity<TEntity, TLanguageAvail>(unitOfWork, model, false);
            //remove language only for scheduling
            model.LanguagesAvailabilities = model.LanguagesAvailabilities.Where(x => !x.ValidFrom.HasValue).ToList();
            return PublishEntity<TEntity, TLanguageAvail>(unitOfWork, model);
        }

        public PublishingResult PublishEntity<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IVmLocalizedEntityModel model, HistoryAction publishAction = HistoryAction.Publish)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            //Publish entity              
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var psModified = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            if (model.LanguagesAvailabilities.Select(i => i.StatusId).All(i => i != psPublished))
            {
                throw new PublishLanguageException();
            }
            var serviceRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entity = serviceRep.All().Single(x => x.Id == model.Id);
            var newPublished = entity;
            if (entity.PublishingStatusId == psPublished || entity.PublishingStatusId == psOldPublished || entity.PublishingStatusId == psDeleted)
            {
                newPublished = VersioningManager.CreateEntityVersion(unitOfWork, entity, VersioningMode.Standard, PublishingStatus.Published);
            }
            var affected = VersioningManager.PublishVersion(unitOfWork, newPublished);
//            var affected = new List<PublishingAffectedResult>
//            {
//                new PublishingAffectedResult()
//                {
//                    Id = newPublished.Id,
//                    PublishingStatusOld = entity.PublishingStatusId,
//                    PublishingStatusNew = newPublished.PublishingStatusId
//                }
//            };

            if (newPublished.Id == entity.Id)
            {
                entity.PublishingStatus = null;
                entity.PublishingStatusId = psPublished;

                var affectedIds = affected.Select(y => y.Id).ToList();
                var affectedEntities = unitOfWork.ApplyIncludes(serviceRep.All().Where(x => affectedIds.Contains(x.Id)), q => q.Include(i => i.LanguageAvailabilities)).ToList();
                
                affectedEntities.Where(x => x.PublishingStatusId == psOldPublished || x.PublishingStatusId == psDeleted).ForEach(ae =>
                {
                    ae.LanguageAvailabilities.ForEach(y =>
                    {
                        y.PublishAt = null;
                        y.ArchiveAt = null;
                        y.LastFailedPublishAt = null;
                    }); 
                });
            }
            else
            {
                affected.Add(new PublishingAffectedResult()
                {
                    Id = entity.Id,
                    PublishingStatusOld = entity.PublishingStatusId,
                    PublishingStatusNew = psOldPublished
                });
                
                entity.PublishingStatus = null;
                entity.PublishingStatusId = psOldPublished;
                entity.LanguageAvailabilities.ForEach(x =>
                {
                    x.PublishAt = null;
                    x.ArchiveAt = null;
                    x.LastFailedPublishAt = null;
                });
            }

            VersioningManager.ChangeStatusOfLanguageVersion<TEntity, TLanguageAvail>(unitOfWork, newPublished, model.LanguagesAvailabilities);
            AddHistoryMetaData<TEntity, TLanguageAvail>(newPublished, action: publishAction);
           
            if (publishAction == HistoryAction.Publish)
            {
                unitOfWork.Save();
            }

            var processedEntityResults = affected.First(i => i.PublishingStatusNew == psPublished);
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
        
        public IList<PublishingResult> ExecutePublishEntities<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IReadOnlyList<IVmLocalizedEntityModel> modelList, bool allowAnonymous = false)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            
            var result = new List<PublishingResult>();

            var historyAction = allowAnonymous ? HistoryAction.MassPublish : HistoryAction.Publish;
            foreach (var model in modelList)
            {
                try
                {
                    Guid? entityId = model.Id;
                    //Validate mandatory values
                    var validationMessages =  validationManager.CheckEntity<TEntity>(entityId.Value, unitOfWork, model);
                    
                    if (validationMessages.Any())
                    {
                        var languageErrorIds = model.LanguagesAvailabilities.Where
                        (
                            la => la.StatusId == PublishingStatusCache.Get(PublishingStatus.Published) &&
                            validationMessages.ContainsKey(la.LanguageId)
                        ).Select((la => la.LanguageId)).ToList();
                        
                        if (languageErrorIds.Count > 0)
                        {
                            SetFailedPublishDate<TEntity, TLanguageAvail>(unitOfWork, model.Id,
                                languageErrorIds, allowAnonymous);
                            result.Add(new PublishingResult
                            {
                                Id = model.Id                           
                            });
                            continue;
                        }
                    }
                    result.Add(PublishEntity<TEntity, TLanguageAvail>(unitOfWork, model, historyAction));
                }
                catch (Exception e)
                {
                    logger.LogError(
                        $"Publishing failed for {model.Id} - {string.Join(";", model.LanguagesAvailabilities.Select(x => $"(L:{x.LanguageId}, S:{x.StatusId}."))}.");
                    logger.LogError($"{e.GetType()} message:{e.Message} details:{CoreExtensions.ExtractAllInnerExceptions(e)}");
                }

            }

            return result;
        }

        /// <summary>
        /// Sets the last failed publish date to the set publish date and then resets publish date for each specified
        /// language availability.
        /// </summary>
        /// <param name="unitOfWork">A writeable unit of work.</param>
        /// <param name="entityId">ID of the entity with failed language availabilities.</param>
        /// <param name="languageIds">IDs of languages that failed to publish for the given entity.</param>
        /// <param name="allowAnonymous">Allow anonymous saving to database.</param>
        /// <typeparam name="TEntity">Type of the versioned entity.</typeparam>
        /// <typeparam name="TLanguageAvail">Type of the entities language availabilities.</typeparam>
        private void SetFailedPublishDate<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, Guid entityId, List<Guid> languageIds, bool allowAnonymous)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            var entityRepo = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entity = entityRepo.All()
                .Include(x => x.LanguageAvailabilities)
                .Single(x => x.Id == entityId);
            var failedLanguageAvailabilities = entity.LanguageAvailabilities
                .Where(la => languageIds.Contains(la.LanguageId));

            foreach (var failedLanguageAvailability in failedLanguageAvailabilities)
            {
                failedLanguageAvailability.LastFailedPublishAt = failedLanguageAvailability.PublishAt;
                failedLanguageAvailability.PublishAt = null;
            }

            if (allowAnonymous)
            {
                unitOfWork.Save(SaveMode.AllowAnonymous);
            }
            else
            {
                unitOfWork.Save();
            }
        }
        public TEntity ArchiveLanguage<TEntity, TLanguageAvail>(VmEntityBasic model)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            return ArchiveWithdrawOrRestoreLanguage<TEntity, TLanguageAvail>(model, PublishingStatus.Deleted, (unitOfWork, entity) => throw new ArchiveLanguageException());
        }

        public TEntity WithdrawLanguage<TEntity, TLanguageAvail>(VmEntityBasic model)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            return ArchiveWithdrawOrRestoreLanguage<TEntity, TLanguageAvail>(model, PublishingStatus.Draft,
                (unitOfWork, entity) =>
                {
                    VersioningManager.ChangeToModified(unitOfWork, entity, new List<PublishingStatus>() { PublishingStatus.Deleted, PublishingStatus.OldPublished, PublishingStatus.Published });
                });
        }

        public TEntity RestoreLanguage<TEntity, TLanguageAvail>(VmEntityBasic model)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            return ArchiveWithdrawOrRestoreLanguage<TEntity, TLanguageAvail>(model, PublishingStatus.Draft,
                (unitOfWork, entity) => throw new RestoreLanguageException());
        }

        private TEntity ArchiveWithdrawOrRestoreLanguage<TEntity, TLanguageAvail>(VmEntityBasic model, PublishingStatus newStatus, Action<IUnitOfWork, TEntity> lastPublishedLanguageAction = null)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
                var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
                var psNewStatus = typesCache.Get<PublishingStatusType>(newStatus.ToString());
                var serviceRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
                var entity = serviceRep.All().Include(x => x.LanguageAvailabilities).Single(x => x.Id == model.Id);
                if (model.LanguageId == null) return entity;
                var last = VersioningManager.GetLastModifiedVersion<TEntity>(unitOfWork, entity.UnificRootId);
                if (last != null && last.EntityId != entity.Id)
                {
                    throw new PublishLanguageException();
                }

                if (lastPublishedLanguageAction != null && (
                   (newStatus == PublishingStatus.Deleted && entity.LanguageAvailabilities.Where(x => x.LanguageId != model.LanguageId).All(x => x.StatusId == psDeleted)) ||
                   (newStatus != PublishingStatus.Deleted && entity.PublishingStatusId == psPublished && entity.LanguageAvailabilities.Where(x => x.LanguageId != model.LanguageId).All(x => x.StatusId != psPublished))
                ))
                {
                    lastPublishedLanguageAction(unitOfWork, entity);
                }

                if (entity.PublishingStatusId == psDeleted || entity.PublishingStatusId == psOldPublished || entity.PublishingStatusId == psPublished)
                {
                    entity = VersioningManager.CreateEntityVersion(unitOfWork, entity, VersioningMode.Standard,
                        entity.PublishingStatusId == psPublished ? PublishingStatus.Published : PublishingStatus.Modified);
                }

                var newLanguages = new List<VmLanguageAvailabilityInfo>
                {
                    new VmLanguageAvailabilityInfo()
                    {
                        LanguageId = model.LanguageId.Value,
                        StatusId = psNewStatus
                    }
                };

                VersioningManager.ChangeStatusOfLanguageVersion<TEntity, TLanguageAvail>(unitOfWork, entity, newLanguages);
                AddHistoryMetaData<TEntity, TLanguageAvail>(entity, forceUpdate:true);
                unitOfWork.Save(parentEntity: entity);
                return entity;
            });
        }

        private bool IsWholeEntityToArchiving<TEntity, TLanguageAvail>(TEntity entity, IVmLocalizedEntityModel model)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());

            return entity.LanguageAvailabilities.Where(x => !model.LanguagesAvailabilities.Select(y => y.LanguageId).Contains(x.LanguageId)).All(z => z.StatusId == psDeleted)
                   || (entity.LanguageAvailabilities.All(x => model.LanguagesAvailabilities.Select(y => y.LanguageId).Contains(x.LanguageId))
                   && entity.LanguageAvailabilities.Count == model.LanguagesAvailabilities.Count);
        }


        public void ExecuteArchiveEntityLanguageVersions<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IReadOnlyList<IVmLocalizedEntityModel> modelList, Action<IUnitOfWorkWritable, Guid, TEntity> onAdditionalAction = null)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            foreach (var model in modelList)
            {
                try
                {
                    ArchiveLanguageVersionWithVersioning<TEntity, TLanguageAvail>(unitOfWork, model, onAdditionalAction);
                }
                catch (Exception)
                {
                    logger.LogError($"MASS TOOL - Archive failed for {model.Id} - {string.Join(";", model.LanguagesAvailabilities.Select(x => $"(L:{x.LanguageId}, S:{x.StatusId}."))}.");
                }
            }
        }
        
        
        public void ExecuteArchiveEntities<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IReadOnlyList<Guid> entityIds, Action<IUnitOfWorkWritable, Guid, TEntity> onAdditionalAction = null)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            foreach (var entityId in entityIds)
            {
                try
                {
                    ChangeEntityVersionedToDeleted<TEntity, TLanguageAvail>(unitOfWork, entityId, onAdditionalAction);
                }
                catch (Exception e)
                {
                    logger.LogError($"MASS TOOL - Archive failed for {entityId} with exception: {e}");
                }
            }
        }

        public TEntity ArchiveLanguageVersionWithVersioning<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IVmLocalizedEntityModel model, Action<IUnitOfWorkWritable, Guid, TEntity> onAdditionalAction = null)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
                var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
                var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                var entityRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
                var entity = entityRep.All().Include(x => x.LanguageAvailabilities).Single(x => x.Id == model.Id);

            if (IsWholeEntityToArchiving<TEntity, TLanguageAvail>(entity, model))
            {
                return ChangeEntityVersionedToDeleted<TEntity, TLanguageAvail>(unitOfWork, entity.Id, onAdditionalAction);
            }

            //New entity version
            var newEntityVersion = VersioningManager.CreateEntityVersion(unitOfWork, entity, VersioningMode.Standard,
            entity.PublishingStatusId == psPublished ? PublishingStatus.Published : (PublishingStatus?)null); //Modified

            //Set status
            var newLocalizedModel = model;
            newLocalizedModel.LanguagesAvailabilities.ForEach(x =>
                {
                    x.StatusId = psDeleted;
                }
            );
      
            VersioningManager.ChangeStatusOfLanguageVersion<TEntity, TLanguageAvail>(unitOfWork, newEntityVersion, newLocalizedModel.LanguagesAvailabilities);
        
            //Versioning
            AddHistoryMetaData<TEntity, TLanguageAvail>(newEntityVersion);
            return newEntityVersion;
        }

        private VmPublishingResultModel ChangeEntityToModified<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, Guid entityVersionedId, HistoryAction historyAction, Func<IUnitOfWork, TEntity,bool> additionalCheckAction = null)
             where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IValidity, new()
             where TLanguageAvail : class, ILanguageAvailability
        {

            var serviceRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entity = serviceRep.All().Include(j => j.Versioning).Single(x => x.Id == entityVersionedId);

            if (CheckModifiedExistsForRoot<TEntity>(unitOfWork, entity.UnificRootId))
            {
                throw new InvalidOperationException();
            }
            
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
            VersioningManager.ChangeStatusOfLanguageVersion<TEntity, TLanguageAvail>(unitOfWork, entity, typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString()));
            var affected = VersioningManager.ChangeToModified(unitOfWork, entity, new List<PublishingStatus>(){ PublishingStatus.Deleted, PublishingStatus.OldPublished, PublishingStatus.Published});
            AddHistoryMetaData<TEntity, TLanguageAvail>(entity, action: historyAction);

            if (historyAction == HistoryAction.Restore || historyAction == HistoryAction.Withdraw)
            {
                unitOfWork.Save();
            }

            var result = new VmPublishingResultModel()
            {
                Id = entityVersionedId,
                PublishingStatusId = affected.FirstOrDefault(i => i.Id == entityVersionedId)?.PublishingStatusNew,
                Version = new VmVersion() {Major = entity.Versioning.VersionMajor, Minor = entity.Versioning.VersionMinor}
            };
            
            return result;
        }

        public TEntity ChangeEntityToDeleted<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, Guid entityId)  //with DraftModifiedPublished version
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            var serviceRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entity = serviceRep.All()
                .Include(x=>x.LanguageAvailabilities)
                .Single(x => x.Id == entityId);
            
            var statesToDelete = new List<Guid>
            {
                typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString()),
                typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString()),
                typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString())
            };
            
            var allEntities = serviceRep.All().Where(x => x.UnificRootId == entity.UnificRootId && statesToDelete.Contains(x.PublishingStatusId));
            
            ChangeEntitiesToDeleted<TEntity, TLanguageAvail>(unitOfWork, allEntities);
            VersioningManager.AddMinorVersion(unitOfWork, entity);
            AddHistoryMetaData<TEntity, TLanguageAvail>(entity, action: HistoryAction.Delete);
            unitOfWork.Save();
            
            return entity;
        }
        
        public TEntity ChangeEntityVersionedToDeleted<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork,
            Guid entityId, Action<IUnitOfWorkWritable, Guid, TEntity> onAdditionalAction = null) //with one entityVersion 
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            var serviceRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entity = serviceRep.All()
                .Include(x=>x.LanguageAvailabilities)
                .Single(x => x.Id == entityId);
            
            var statesToDelete = new List<Guid>
            {
                typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString()),
                typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString()),
                typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString())
            };
            if (statesToDelete.Contains(entity.PublishingStatusId))
            {
                onAdditionalAction?.Invoke(unitOfWork, entityId, entity);
                ChangeEntitiesToDeleted<TEntity, TLanguageAvail>(unitOfWork, new List<TEntity>() {entity});
                VersioningManager.AddMinorVersion(unitOfWork, entity);
                AddHistoryMetaData<TEntity, TLanguageAvail>(entity, action: HistoryAction.Delete);
            }
            return entity;
        }

        public TEntity RemoveEntity<TEntity, TLanguageAvail>(Guid entityId) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            return contextManager.ExecuteWriter(unitOfWork => ChangeEntityToRemoved<TEntity, TLanguageAvail>(unitOfWork, entityId));
        }
        
        public TEntity ChangeEntityToRemoved<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, Guid entityId)  //with DraftModifiedPublished version
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            var serviceRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entity = serviceRep.All()
                .Include(x=>x.LanguageAvailabilities)
                .Single(x => x.Id == entityId);
            
            var modifiedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var publishedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            if (entity.PublishingStatusId == modifiedId)
            {
                TEntity publishedEntity = serviceRep.All().FirstOrDefault(j => j.UnificRootId == entity.UnificRootId && j.PublishingStatusId == publishedId);
                if (publishedEntity != null)
                {
                    ChangeEntitiesToRemoved<TEntity, TLanguageAvail>(unitOfWork, new List<TEntity>() {entity});
                    return publishedEntity;
                }
            }
            unitOfWork.Save();
            return entity;
        }
        
        private void ChangeEntitiesToRemoved<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IEnumerable<TEntity> archivingEntities) 
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            var publishingStatusRemoved = typesCache.Get<PublishingStatusType>(PublishingStatus.Removed.ToString());
            var versioningRep = unitOfWork.CreateRepository<IVersioningRepository>();
            archivingEntities.ForEach(e =>
            {    
                e.PublishingStatus = null;
                e.PublishingStatusId = publishingStatusRemoved;
                versioningRep.All().FirstOrDefault(j => j.Id == e.VersioningId).SafeCall(m => m.Ignored = true);
                if (e.LanguageAvailabilities.IsNullOrEmpty()) unitOfWork.LoadCollection(e, i => i.LanguageAvailabilities);
                e.LanguageAvailabilities.ForEach(i => i.StatusId = publishingStatusRemoved);
            });
        }

        
        private void ChangeEntitiesToDeleted<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IEnumerable<TEntity> archivingEntities) //Core
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            var publishingStatusDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());

            archivingEntities.ForEach(e =>
            {    
                e.PublishingStatus = null;
                e.PublishingStatusId = publishingStatusDeleted;
                
                var newLanguageAvailabilities = new List<VmLanguageAvailabilityInfo>();
                newLanguageAvailabilities.AddRange(e.LanguageAvailabilities.Select(x =>
                    new VmLanguageAvailabilityInfo() {LanguageId = x.LanguageId, StatusId = publishingStatusDeleted}).ToList());
              
                VersioningManager.ChangeStatusOfLanguageVersion<TEntity, TLanguageAvail>(unitOfWork, e, newLanguageAvailabilities);
            });
        }

        public VmPublishingResultModel WithdrawEntity<TEntity, TLanguageAvail>(Guid entityVersionedId) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IValidity, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                try
                {
                    return ChangeEntityToModified<TEntity, TLanguageAvail>(unitOfWork, entityVersionedId, HistoryAction.Withdraw);
                }
                catch (InvalidOperationException)
                {
                    throw new WithdrawModifiedExistsException();
                }
            });
        }

        private bool CheckModifiedExistsForRoot<TEntity>(IUnitOfWork unitOfWork, Guid rootId) where TEntity : class, IEntityIdentifier, IVersionedVolume, IValidity
        {
            return VersioningManager.GetVersionId<TEntity>(unitOfWork, rootId, PublishingStatus.Modified).IsAssigned();
        }
        
        public bool CheckModifiedExists<TEntity>(Guid versionedId) where TEntity : class, IEntityIdentifier, IVersionedVolume, IValidity
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var rootId = VersioningManager.GetUnificRootId<TEntity>(unitOfWork, versionedId);
                return rootId.IsAssigned() && CheckModifiedExistsForRoot<TEntity>(unitOfWork, rootId.Value);
            });
        }

        public VmPublishingResultModel WithdrawEntityByRootId<TEntity, TLanguageAvail>(Guid rootId) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IValidity, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            TEntity entity = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                // Get right version id
                var entityId = VersioningManager.GetVersionId<TEntity>(unitOfWork, rootId);
                var repo = unitOfWork.GetSet<TEntity>();
                entity = repo.FirstOrDefault(e => e.Id == entityId);
            });

            if (entity == null)
            {
                return null;
            }

            if (entity.PublishingStatusId == typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString()))
            {
                throw new ModifiedExistsException("Latest version of item is already in Modified state. No actions needed!", null);
            }

            return WithdrawEntity<TEntity, TLanguageAvail>(entity.Id);

        }

        public VmPublishingResultModel RestoreEntity<TEntity, TLanguageAvail>(Guid entityVersionedId, Func<IUnitOfWork, TEntity, bool> additionalCheckAction = null) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IValidity, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                try
                {
                    return ChangeEntityToModified<TEntity, TLanguageAvail>(unitOfWork, entityVersionedId, HistoryAction.Restore, additionalCheckAction);

                }
                catch (InvalidOperationException)
                {
                    throw new RestoreModifiedExistsException();
                }
            });
        }

        public IVmListItemsData<VmEnvironmentInstruction> SaveEnvironmentInstructions(VmEnvironmentInstructionsIn model)
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

        public IVmListItemsData<VmEnvironmentInstruction> GetEnvironmentInstructions()
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var instructionType = typesCache.Get<AppEnvironmentDataType>(AppEnvironmentDataTypeEnum.EnvironmentInstruction.ToString());
                var appDataRepository = unitOfWork.CreateRepository<IAppEnvironmentDataRepository>();
                // take last two newest changes
                var instructions = appDataRepository.All().Where(x => x.TypeId == instructionType).OrderByDescending(x => x.Version).Take(2);
                return new VmListItemsData<VmEnvironmentInstruction>(TranslationManagerToVm.TranslateAll<AppEnvironmentData, VmEnvironmentInstruction>(instructions));
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
        
        private void AddCoStatus(IList<PublishingStatus> statuses, PublishingStatus statusOne, PublishingStatus statusTwo)
        {
            if (statuses.Contains(statusOne) && !statuses.Contains(statusTwo)) statuses.Add(statusTwo);
            if (statuses.Contains(statusTwo) && !statuses.Contains(statusOne)) statuses.Add(statusOne);
        }

        public void ExtendPublishingStatusesByEquivalents(IList<PublishingStatus> statuses)
        {
            AddCoStatus(statuses, PublishingStatus.Deleted, PublishingStatus.OldPublished);
            AddCoStatus(statuses, PublishingStatus.Draft, PublishingStatus.Modified);
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

        public IList<PublishingAffectedResult> RestoreArchivedEntity<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, Guid versionId) 
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            var rep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entity = rep.All().Include(x=>x.LanguageAvailabilities).Single(x => x.Id == versionId);

            var result = VersioningManager.PublishVersion(unitOfWork, entity, PublishingStatus.Modified);
            AddHistoryMetaData<TEntity,TLanguageAvail>(entity, action: HistoryAction.Restore, setByEntity: true);
            return result;
        }
        public bool IsDescriptionEnumType(Guid typeId, string type)
        {
            return typesCache.Compare<DescriptionType>(typeId, type);
        }

        public Guid GetDescriptionTypeId(string code)
        {
            return typesCache.Get<DescriptionType>(code);
        }

        public VmListItemsData<VmEnumType> GetServiceChannelConnectionTypes()
        {
            return dataServiceFetcher.FetchType<ServiceChannelConnectionType>();
        }

        public VmListItemsData<VmEnumType> GetServiceFundingTypes()
        {
            return dataServiceFetcher.FetchType<ServiceFundingType>();
        }

        public IVmOpenApiGuidPageVersionBase<VmOpenApiEntityItem> GetServicesAndChannelsByOrganization(Guid organizationId, bool getSpecialTypes, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null)
        {
            var handler = new ServicesAndChannelsByOrganizationPageHandler(organizationId, getSpecialTypes, PublishingStatusCache, typesCache, date, dateBefore, pageNumber, pageSize);
            if (handler.PageNumber <= 0) return handler.GetModel();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var totalCount = handler.Search(unitOfWork);
            });

            return handler.GetModel();
        }

        public VmEntityHeaderBase GetValidatedHeader(VmEntityHeaderBase header, Dictionary<Guid, List<ValidationMessage>> validationMessages)
        {
            header.LanguagesAvailabilities.ForEach(x =>
            {
                List<ValidationMessage> messages;
                if (!validationMessages.TryGetValue(x.LanguageId, out messages)) return;
                x.CanBePublished = !messages.Any();
                x.ValidatedFields = messages;
            });
            return header;
        }

        /// <summary>
        /// Get area information for service from organization
        /// </summary>
        /// <param name="model">IVmGetAreaInformation</param>
        /// <returns>IVmAreaInformation</returns>
        public IVmAreaInformation GetAreaInformationForOrganization(IVmGetAreaInformation model)
        {
            return contextManager.ExecuteReader(unitOfWork => GetAreaInformationForOrganization(model.OrganizationId, unitOfWork));
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

                result = TranslationManagerToVm.Translate<OrganizationVersioned, VmOrganizationAreaInformation>(organization);
            }
            return result;
        }

        private IEnumerable<IVmAreaInformation> GetDefaultAreaInformationList(Guid? organizationId, IUnitOfWork unitOfWork)
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

        public void AddHistoryMetaData<TEntity, TLanguageAvail>(
            TEntity entity,
            ICopyTemplate copyTemplate = null,
            VmTranslationOrderEntityTargetLanguages translationOrderDetails = null,
            HistoryAction? action = null,
            bool setByEntity = false,
            bool forceUpdate = false)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            if (entity.Versioning != null && (entity.Versioning.Meta == null || forceUpdate))
            {
                var defaultAction = (copyTemplate != null
                                     && copyTemplate.TemplateId.IsAssigned()
                                     && copyTemplate.TemplateOrganizationId.IsAssigned())
                    ? HistoryAction.Copy
                    : HistoryAction.Save;
                
                entity.Versioning.Meta = JsonConvert.SerializeObject(new VmHistoryMetaData
                {
                    EntityStatusId = entity.PublishingStatusId,
                    HistoryAction = action ?? defaultAction,
                    TemplateId = copyTemplate?.TemplateId,
                    TemplateOrganizationId = copyTemplate?.TemplateOrganizationId,
                    SourceLanguageId = translationOrderDetails?.SourceLanguage,
                    TargetLanguageIds = translationOrderDetails?.TargetLanguages,
                    LanguagesMetaData = entity.LanguageAvailabilities.Select(x => new VmHistoryMetaDataLanguage
                    {
                        EntityStatusId = setByEntity ? entity.PublishingStatusId : x.StatusId,
                        LanguageId = x.LanguageId,
                        Reviewed = x.Modified <= x.Reviewed ? x.Reviewed : null,
                        ReviewedBy = x.Modified <= x.Reviewed ? x.ReviewedBy : null,
                        PublishedAt = x.Modified <= x.Reviewed ? x.PublishAt : null,
                        Archived = x.SetForArchived,
                        ArchivedBy = x.SetForArchivedBy,
                        ArchivedAt = x.ArchiveAt
                    }).ToList()
                });
            }
        }
        
        public void UpdateHistoryMetaData<TEntity, TLanguageAvail>(IEnumerable<Guid> entityIds, List<TLanguageAvail> languages, IUnitOfWorkWritable unitOfWork) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailabilityBase
        {
            var entities = unitOfWork.CreateRepository<IRepository<TEntity>>()
                .All()
                .Where(x => entityIds.Contains(x.Id))
                .Include(x => x.Versioning);
 
            entities.ForEach(entity => UpdateHistoryMetaData(entity, languages.Where(x=>x.Id == entity.Id).ToList(), unitOfWork));
        }
        private void UpdateHistoryMetaData<TEntity, TLanguageAvail>(TEntity entity, List<TLanguageAvail> languages, IUnitOfWorkWritable unitOfWork) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            if (entity.Versioning?.Meta != null)
            {
                var metaData = JsonConvert.DeserializeObject<VmHistoryMetaData>(entity.Versioning.Meta);
                var updatedLanguagesIds = languages.Select(x => x.LanguageId);
                metaData.LanguagesMetaData
                    .Where(x=>updatedLanguagesIds.Contains(x.LanguageId))
                    .ForEach(metaLang =>
                    {
                        var newLangAvail = languages.First(lang => lang.LanguageId == metaLang.LanguageId);
                        metaLang.Reviewed = newLangAvail.Reviewed;
                        metaLang.ReviewedBy = newLangAvail.ReviewedBy;
                        metaLang.PublishedAt = newLangAvail.PublishAt;
                        metaLang.Archived = newLangAvail.SetForArchived;
                        metaLang.ArchivedBy = newLangAvail.SetForArchivedBy;
                        metaLang.ArchivedAt = newLangAvail.ArchiveAt;
                    });
                
                entity.Versioning.Meta = JsonConvert.SerializeObject(metaData);
            }
        }
        
        private void UpdateHistoryMetaData<TEntity, TLanguageAvail>(Guid entityId, List<TLanguageAvail> languages, IUnitOfWorkWritable unitOfWork) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            var entity = unitOfWork.CreateRepository<IRepository<TEntity>>()
                .All()
                .Where(x => x.Id == entityId)
                .Include(x => x.Versioning)
                .First();
            
            UpdateHistoryMetaData(entity, languages, unitOfWork);
        }
        
        

        public void CopyEntity<TEntityVersioned, TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, Guid entityVersionedId, Guid? organizationId)
            where TEntityVersioned : class, IEntityIdentifier, IVersionedVolume<TEntity>, IMultilanguagedEntity<TLanguageAvail>, IOriginalEntity, IOrganizationInfo, INameReferences, new()
            where TEntity : IVersionedRoot, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            var unificRootId = VersioningManager.GetUnificRootId<TEntityVersioned>(unitOfWork, entityVersionedId);

            if (unificRootId.HasValue)
            {
                //GetLastVersion 
                var lastEntityVersion = VersioningManager.GetLastPublishedDraftVersion<TEntityVersioned>(unitOfWork, unificRootId.Value);
                if (lastEntityVersion != null)
                {
                    var entityRep = unitOfWork.CreateRepository<IRepository<TEntityVersioned>>();
                    var lastPublishedDraftEntity = entityRep.All().Single(x => x.Id == lastEntityVersion.EntityId);

                    var copiedEntity =
                        VersioningManager.CreateCopyVersion<TEntityVersioned, TEntity, TLanguageAvail>(unitOfWork,
                            lastPublishedDraftEntity, PublishingStatus.Draft);

                    var copiedModel = new VmCopyTemplate
                    {
                        TemplateId = lastPublishedDraftEntity.Id,
                        TemplateOrganizationId = lastPublishedDraftEntity.OrganizationId
                    };
                    
                    FinalizeCopyEntity<TEntityVersioned, TLanguageAvail>(copiedEntity, copiedModel, organizationId);
                   
                }
            }
        }

        public void FinalizeCopyEntity<TEntityVersioned, TLanguageAvail>(TEntityVersioned newEntity, ICopyTemplate copiedModel, Guid? organizationId)
            where TEntityVersioned : class, IEntityIdentifier, IVersionedVolume, IOriginalEntity, IOrganizationInfo, INameReferences, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            //set organization id
            if (organizationId.HasValue)
            {
                newEntity.OrganizationId = organizationId.Value;
            }
                
            //set previous origin entity
            newEntity.OriginalId = copiedModel.TemplateId;

            AddHistoryMetaData<TEntityVersioned, TLanguageAvail>(newEntity, copiedModel, action: HistoryAction.Copy);
        }
        
        public void ExecuteCopyEntities<TEntityVersioned, TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IReadOnlyList<Guid> entityIds, Guid? organizationId = null)
            where TEntityVersioned : class, IEntityIdentifier, IVersionedVolume<TEntity>, IMultilanguagedEntity<TLanguageAvail>, IOriginalEntity, IOrganizationInfo, INameReferences, new()
            where TEntity : IVersionedRoot, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            foreach (var entityId in entityIds)
            {
                CopyEntity<TEntityVersioned, TEntity, TLanguageAvail>(unitOfWork, entityId, organizationId);
            }
        }
        
        public void ExecuteRestoreEntities<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IReadOnlyList<Guid> entityIds, Action<IUnitOfWorkWritable, Guid, TEntity> onAdditionalAction = null)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IValidity, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            foreach (var entityId in entityIds)
            {
                try
                {
                    ChangeEntityToModified<TEntity, TLanguageAvail>(unitOfWork, entityId, HistoryAction.MassRestore);
                }
                catch (Exception e)
                {
                    logger.LogError($"MASS TOOL - Restore failed for {entityId} with exception: {e}");
                }
            }
        }
        
        public List<TEntity> GetNotificationEntity<TEntity, TLanguage>(IEnumerable<Guid> unificRootIds, IUnitOfWork unitOfWork, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeChain, Func<IUnitOfWork, Guid, VersionInfo> customSelect = null)
            where TEntity : class, IVersionedVolume, IMultilanguagedEntity<TLanguage>
            where TLanguage : LanguageAvailability 
        {
            var entityRepository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            
            if (customSelect == null)
            {
                customSelect = VersioningManager.GetLastVersion<TEntity>;
            }
            
            var ids = unificRootIds
                .Select(unificRootId => customSelect(unitOfWork, unificRootId)
                    ?.EntityId
                ).Where(x => x != null)
                .Distinct()
                .ToList();

            var query = entityRepository.All()
                .Include(x => x.LanguageAvailabilities)
                .Where(x => ids.Contains(x.Id));
            return unitOfWork.ApplyIncludes(query, includeChain)            
                .ToList();
        }
        
        public string GetChannelSubType(Guid entityId,
            List<ServiceChannelVersioned> allEntities)
        {
            var entity = allEntities.FirstOrDefault(x => x.UnificRootId == entityId);
            if (entity != null)
            {
                return typesCache.GetByValue<ServiceChannelType>(entity.TypeId);
            }
            return EntityTypeEnum.Channel.ToString();
        }
        
        public Dictionary<Guid,Dictionary<string,string>> GetEntityNames<TEntity>(List<TEntity> versionedGd) where TEntity : class, IVersionedVolume, INameReferences
        {
            return versionedGd
                .ToDictionary(key => key.UnificRootId, value => value.Names
                    .Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()))
                    .ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));
        }
        
        public Dictionary<Guid,IReadOnlyList<VmLanguageAvailabilityInfo>> GetLanguageAvailabilites<TEntity, TLanguage>(List<TEntity> entities) where TEntity : class, IVersionedVolume, IMultilanguagedEntity<TLanguage>
            where TLanguage : LanguageAvailability 
        {
            return entities
                .ToDictionary(key => key.UnificRootId, value => TranslationManagerToVm
                    .TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                        value.LanguageAvailabilities
                            .OrderBy(x => languageOrderCache.Get(x.LanguageId))
                    ));
        }
        
        public void RemoveNotCommonConnections(IEnumerable<Guid> versionedIds, IUnitOfWorkWritable unitOfWork, bool checkChannelStatus = true)
        {
            var notCommonType = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.NotCommon.ToString());
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            
            var connectionRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();

            var channels = serviceChannelRep.All()
                .Where(x => versionedIds.Contains(x.Id) &&
                            (!checkChannelStatus || x.ConnectionTypeId == notCommonType));
            var channelUnificRootIds = channels.Select(x => x.UnificRootId);
            
            var connections = connectionRep.All().Where(x => channelUnificRootIds.Contains(x.ServiceChannelId));
            
            var serviceUnificRootIds = connections.Select(x => x.ServiceId);
            var services = serviceRep.All().Where(x => serviceUnificRootIds.Contains(x.UnificRootId))
                .GroupBy(x => x.UnificRootId).Select(x =>
                    x.OrderBy(y =>
                        y.PublishingStatusId == publishingStatusId ? 0 :
                        y.PublishingStatusId == draftStatusId ? 1 :
                        y.PublishingStatusId == modifiedStatusId ? 2 : 3).FirstOrDefault());
  
            connections.ForEach(connection =>
            {
                var channel = channels.FirstOrDefault(x => x.UnificRootId == connection.ServiceChannelId);
                var service = services.FirstOrDefault(x => x.UnificRootId == connection.ServiceId);
                if (service != null && channel != null && service.OrganizationId != channel.OrganizationId)
                {
                    connectionRep.Remove(connection);
                }
            });               
        }
        
        public List<Guid> GetServiceChannelRelationIds(IUnitOfWork unitOfWork, Guid unificRootId)
        {
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var rootIds = serviceChannelRep.All()
                .Where(x => x.ServiceChannelId == unificRootId)
                .Select(x => x.ServiceId).ToList();
            return rootIds.Select(x => VersioningManager.GetLastPublishedModifiedDraftVersion<ServiceVersioned>(unitOfWork, x).EntityId).ToList();           
        }
        
        public void DeleteServiceChannelConnections(IUnitOfWork unitOfWork, Guid serviceChannelVersionedId)
        {
            var serviceServiceChannelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var gdServiceChannelRep = unitOfWork.CreateRepository<IGeneralDescriptionServiceChannelRepository>();
            var unificRootId = VersioningManager.GetUnificRootId<ServiceChannelVersioned>(unitOfWork, serviceChannelVersionedId);
            serviceServiceChannelRep.All().Where(x => x.ServiceChannelId == unificRootId).ForEach(item => serviceServiceChannelRep.Remove(item));
            gdServiceChannelRep.All().Where(x => x.ServiceChannelId == unificRootId).ForEach(item => gdServiceChannelRep.Remove(item));
        }

        public bool IsOidUniqueForOrganization(string oid, Guid? organizationId = null, IUnitOfWork unitOfWork = null)
        {
            return unitOfWork != null ?
                    IsOidUniqueForOrganization(oid, unitOfWork, organizationId) :
                    contextManager.ExecuteReader(uow => IsOidUniqueForOrganization(oid, uow, organizationId));
        }


        // oid duplicity is not allowed for any organization
        private bool IsOidUniqueForOrganization(string oid, IUnitOfWork unitOfWork, Guid? organizationId = null)
        {
            // if oid is not filled, duplicity should not be checked
            if (string.IsNullOrEmpty(oid)) return true;
            
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            return organizationId.HasValue
                // if organization already exist, check for duplicity
                ? !organizationRep.All().Any(x => x.UnificRootId != VersioningManager.GetUnificRootId<OrganizationVersioned>(unitOfWork, organizationId) && x.Oid == oid)
                // creation phase, id is not exist, check if there are any organizations with oid
                : !organizationRep.All().Any(x => x.Oid == oid);
        }

        private List<Guid> GetSoteTypes()
        {
            return new List<Guid>
            {
                typesCache.Get<OrganizationType>(OrganizationTypeEnum.SotePrivate.ToString()),
                typesCache.Get<OrganizationType>(OrganizationTypeEnum.SotePublic.ToString())
            };
        }

        /// <summary>
        /// Check, is organization is sote type
        /// </summary>
        /// <param name="organizationTypeId"></param>
        /// <returns></returns>
        public bool OrganizationIsSote(Guid? organizationTypeId)
        {
            return organizationTypeId.HasValue && GetSoteTypes().Contains(organizationTypeId.Value);
        }

        /// <summary>
        /// Check, is organization is sote type
        /// </summary>
        /// <param name="organizationType"></param>
        /// <returns></returns>
        public bool OrganizationIsSote(string organizationType)
        {
            if (string.IsNullOrEmpty(organizationType)) return false; // SFIPTV-953
            return OrganizationIsSote(typesCache.Get<OrganizationType>(organizationType));
        }

        /// <summary>
        /// Creates GeneralDescriptionFocTypeOrganizationSoteRelation filters for organization list 
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="unificRootIds"></param>
        public void HandleOrganizationSoteFocFilter(IUnitOfWorkWritable unitOfWork, List<Guid> unificRootIds)
        {
            const string FilterName = "GeneralDescriptionFocTypeOrganizationSoteRelation";
            
            // get restriction filter id
            var restrictionFilterIds = GetRestrictionFilter(FilterName);
            if (restrictionFilterIds.IsNullOrEmpty()) return;
            
            var organizationFilterRep = unitOfWork.CreateRepository<IOrganizationFilterRepository>();
            var existingFilters = organizationFilterRep
                .All()
                .Select(f => new KeyValuePair<Guid, Guid>(f.OrganizationId, f.FilterId))
                .ToList();

            unificRootIds.ForEach(organizationId =>
            {
                restrictionFilterIds.ForEach(filterId =>
                {
                    // try to get filter
                    if (existingFilters.Contains(new KeyValuePair<Guid, Guid>(organizationId, filterId))) return;
                    
                    // add new filter
                    organizationFilterRep.Add(new OrganizationFilter
                    {
                        OrganizationId = organizationId,
                        FilterId = filterId
                    });
                });
            });
        }

        private List<Guid> GetRestrictionFilter(string filterName)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var restrictionFilterRep = unitOfWork.CreateRepository<IRestrictionFilterRepository>();
                var restrictionFilters = restrictionFilterRep.All().Where(rf => rf.FilterName == filterName);
                return restrictionFilters.IsNullOrEmpty() ? new List<Guid>() : restrictionFilters.Select(f => f.Id).ToList();
            });
        }

        /// <summary>
        /// Get restricted general descriptions 
        /// </summary>
        /// <returns></returns>
        public List<Guid> GetRestrictedDescriptionTypes()
        {
            var gdTypes = typesCache.GetCacheData<GeneralDescriptionType>();
            return pahaTokenProcessor.UserRole == UserRoleEnum.Eeva 
                ? new List<Guid>() 
                : restrictionFilterManager.SetAccessForGuidTypes<GeneralDescriptionType>(pahaTokenProcessor.ActiveOrganization, gdTypes)
                    .Where(i => i.Access == EVmRestrictionFilterType.Denied)
                    .Select(i => i.Id).ToList();
        }
           
        public IVmListItemsData<IVmLastModifiedInfo> GetEntityLastModifiedInfos(VmMassDataModel<VmRestoringModel> model)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var result = new List<IVmLastModifiedInfo>();

                foreach (var service in model.Services)
                {
                    result.AddExceptNull(utilities.GetEntityLastModifiedInfo<ServiceVersioned, Service>(service.Id, unitOfWork));
                }
                
                foreach (var channel in model.Channels)
                {
                    result.AddExceptNull(utilities.GetEntityLastModifiedInfo<ServiceChannelVersioned, ServiceChannel>(channel.Id, unitOfWork));
                }
                
                foreach (var serviceCollection in model.ServiceCollections)
                {
                    result.AddExceptNull(utilities.GetEntityLastModifiedInfo<ServiceCollectionVersioned, ServiceCollection>(serviceCollection.Id, unitOfWork));
                }

                return new VmListItemsData<IVmLastModifiedInfo>(result);
            });
            
        }

        /// <summary>
        /// Throw exception if user try archive entity with ASTI connection
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityVersionedId"></param>
        /// <typeparam name="TEntity"></typeparam>
        public void CheckArchiveAstiContract<TEntity>(IUnitOfWork unitOfWork, Guid entityVersionedId) where TEntity : class, IVersionedVolume
        {
            var unificRootId = VersioningManager.GetUnificRootId<TEntity>(unitOfWork, entityVersionedId);
            var connectionRepo = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var isAstiContract = connectionRepo.All()
                .Any(con => (con.ServiceChannelId == unificRootId || con.ServiceId == unificRootId) &&
                            con.IsASTIConnection);
            if (pahaTokenProcessor.UserRole != UserRoleEnum.Eeva && isAstiContract)
                throw new PtvAppException(
                    "You have no access right to remove ASTI contract service locations and services");
        }
    }
}
