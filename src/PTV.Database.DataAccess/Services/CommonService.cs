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
using ServiceCollection = Microsoft.Extensions.DependencyInjection.ServiceCollection;

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
        private readonly ServiceUtilities utilities;
        private readonly IVersioningManager versioningManager;
        private readonly IValidationManager validationManager;
        private readonly ApplicationConfiguration configuration;
        private IOrganizationTreeDataCache organizationTreeDataCache;
        private IRestrictionFilterManager restrictionFilterManager;
        private IPahaTokenProcessor pahaTokenProcessor;
        private ILogger<CommonService> logger;

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
            IValidationManager validationManager,
            ApplicationConfiguration configuration,
            IOrganizationTreeDataCache organizationTreeDataCache,
            IRestrictionFilterManager restrictionFilterManager,
            IPahaTokenProcessor pahaTokenProcessor,
            ILanguageCache languageCache,
            ILogger<CommonService> logger)
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
            this.typesCache = typesCache;
            this.dataServiceFetcher = dataServiceFetcher;
            this.utilities = utilities;
            this.versioningManager = versioningManager;
            this.configuration = configuration;
            this.organizationTreeDataCache = organizationTreeDataCache;
            this.restrictionFilterManager = restrictionFilterManager;
            this.pahaTokenProcessor = pahaTokenProcessor;
            this.languageCache = languageCache;
            this.logger = logger;
            this.validationManager = validationManager;
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
                var orgTypes = CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<OrganizationType, OrganizationTypeName>(unitOfWork, organizationTypeRep.All())), x => x.Name);
                orgTypes.ForEach(x => x.IsDisabled = x.Children.Any());
                var targetGroups = CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<TargetGroup, TargetGroupName>(unitOfWork, targetGroupRep.All().Where(x => x.IsValid).OrderBy(x => x.Label))), x => x.Code);

                var digitalAuthorizationRep = unitOfWork.CreateRepository<IDigitalAuthorizationRepository>();
//                var digitalAuthorization =
//                    TranslationManagerToVm
//                        .TranslateAll<DigitalAuthorization, VmListItem>(
//                            GetIncludesForFinto<DigitalAuthorization, DigitalAuthorizationName>(unitOfWork, digitalAuthorizationRep.All())
//                        );
                var lifeEventRep = unitOfWork.CreateRepository<ILifeEventRepository>();
//                var keyWordRep = unitOfWork.CreateRepository<IKeywordRepository>();
                var industrialClassesRep = unitOfWork.CreateRepository<IIndustrialClassRepository>();
                var result = new VmEnumBase();
                var publishingStatuses = GetPublishingStatuses();
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("AreaInformationTypes", GetAreaInformationTypes()),
                    () => GetEnumEntityCollectionModel("AreaTypes", GetAreaTypes()),
                    () => GetEnumEntityCollectionModel("AstiTypes", GetExtraSubTypes()),
                    () => GetEnumEntityCollectionModel("BusinessRegions", GetAreas(unitOfWork, AreaTypeEnum.BusinessRegions)),
//                    () => GetEnumEntityCollectionModel("DigitalAuthorizations", digitalAuthorization),
                    () => GetEnumEntityCollectionModel("ChargeTypes", GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("ServiceChannelConnectionTypes", GetServiceChannelConnectionTypes()),
                    () => GetEnumEntityCollectionModel("DialCodes", GetDefaultDialCode(unitOfWork)),
                    () => GetEnumEntityCollectionModel("HospitalRegions", GetAreas(unitOfWork, AreaTypeEnum.HospitalRegions)),
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
                    () => GetEnumEntityCollectionModel("TopTargetGroups", CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<TargetGroup, TargetGroupName>(unitOfWork, targetGroupRep.All().OrderBy(x => x.Label)), 1), x => x.Code)),
                    () => GetEnumEntityCollectionModel("TopLifeEvents", CreateTree<VmTreeItem>(LoadFintoTree(GetIncludesForFinto<LifeEvent, LifeEventName>(unitOfWork, lifeEventRep.All()), 1), x => x.Name)),
                    () => GetEnumEntityCollectionModel("TopServiceClasses", CreateTree<VmTreeItem>(LoadFintoTree(GetIncludesForFinto<ServiceClass, ServiceClassName>(unitOfWork, serviceClassesRep.All().Include(x => x.Descriptions)), 1), x => x.Name)),
                    () => GetEnumEntityCollectionModel("TopDigitalAuthorizations", CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<DigitalAuthorization, DigitalAuthorizationName>(unitOfWork, digitalAuthorizationRep.All())), x => x.Name)),
                    () => GetEnumEntityCollectionModel("IndustrialClasses", TranslationManagerToVm.TranslateAll<IFintoItem, VmTreeItem>(GetIncludesForFinto<IndustrialClass, IndustrialClassName>(unitOfWork, industrialClassesRep.All().Where(x => x.Code == "5" && x.IsValid).OrderBy(x => x.Label))).ToList()),
                    () => GetEnumEntityCollectionModel("PhoneNumberTypes", GetPhoneTypes()),
                    () => GetEnumEntityCollectionModel("FundingTypes", GetServiceFundingTypes()),
                    () => GetEnumEntityCollectionModel("ChannelTypes", GetServiceChannelTypes()),
                    () => GetEnumEntityCollectionModel("ProvisionTypes", GetProvisionTypes()),
                    () => GetEnumEntityCollectionModel("TargetGroups", targetGroups),
                    () => GetEnumEntityCollectionModel("OrganizationTypes", orgTypes),
                    () => GetEnumEntityCollectionModel("PrintableFormUrlTypes", GetPrintableFormUrlTypes()),
                    () => GetEnumEntityCollectionModel("OrganizationAreaInformations", GetDefaultAreaInformationList(userInfo.UserOrganization, unitOfWork)),
                    () => GetEnumEntityCollectionModel("TranslationStateTypes", GetTranslationStateTypes()),
                    () => GetEnumEntityCollectionModel("TranslationCompanies", GetTranslationCompanies(unitOfWork)));

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

        public IReadOnlyList<VmListItem> GetOrganizationNames(string searchText = null, bool takeAll = true)
        {
            // get enum values to local variables before using them inside LINQ queries as otherwise the provider does the queries in-memory and not in DB
            // as it cannot translate the enum values to db queries and this way we get better performance as the queries are evaluated in DB
            var resultTemp = ((IOrganizationTreeDataCacheWithEntity)organizationTreeDataCache).GetFlatDataEntities().Values.AsEnumerable();

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
            IVmLocalizedEntityModel model)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            var userName = unitOfWork.GetUserNameForAuditing();
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var massModels = model.LanguagesAvailabilities
                .Where(language => language.StatusId == psPublished)
                .Select(language => new VmMassLanguageAvailabilityModel
            {
                Id = model.Id,
                LanguageId = language.LanguageId,
                Reviewed = DateTime.UtcNow,
                ReviewedBy = userName,
                ValidTo = language.ValidTo?.FromEpochTime(),
                ValidFrom = language.ValidFrom?.FromEpochTime(),
                AllowSaveNull = true
            });
            TranslationManagerToEntity.TranslateAll<VmMassLanguageAvailabilityModel, TLanguageAvail>(massModels, unitOfWork);
            unitOfWork.Save(preSaveAction: PreSaveAction.DoNotSetAudits);
            return new PublishingResult{Id = model.Id};
        }       

        public PublishingResult PublishEntity<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IVmLocalizedEntityModel model, bool saveAutomatically = true)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
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
                newPublished = versioningManager.CreateEntityVersion(unitOfWork, entity, VersioningMode.Standard, PublishingStatus.Published);
            }
            var affected = versioningManager.PublishVersion(unitOfWork, newPublished);
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
                });
            }

            versioningManager.ChangeStatusOfLanguageVersion<TEntity, TLanguageAvail>(unitOfWork, newPublished, model.LanguagesAvailabilities);
            AddHistoryMetaData<TEntity, TLanguageAvail>(newPublished, HistoryAction.Publish);
           
            if (saveAutomatically)
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
        
        public PublishingResult ExecutePublishEntity<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IVmLocalizedEntityModel model)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            Guid? entityId = model.Id;

            //Validate mandatory values
            var validationMessages =  validationManager.CheckEntity<TEntity>(entityId.Value, unitOfWork, model);
            if (validationMessages.Any())
            {
                throw new PtvValidationException(validationMessages, null);
            }

            //Publishing
            return PublishEntity<TEntity, TLanguageAvail>(unitOfWork, model, saveAutomatically: false);
        }

        public IList<PublishingResult> ExecutePublishEntities<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IReadOnlyList<IVmLocalizedEntityModel> modelList)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            
            var result = new List<PublishingResult>();
            
            foreach (var model in modelList)
            {
                try
                {
                    Guid? entityId = model.Id;
                    //Validate mandatory values
                    var validationMessages =  validationManager.CheckEntity<TEntity>(entityId.Value, unitOfWork, model);
                    
                    if (validationMessages.Any())
                    {
                        result.Add(new PublishingResultWithValidationMessages()
                        {
                            Id = model.Id,
                            ValidationMessages = validationMessages
                        });
                        model.LanguagesAvailabilities = model.LanguagesAvailabilities
                            .Where(x => !validationMessages.ContainsKey(x.LanguageId)).ToList();
                    }
                    if (model.LanguagesAvailabilities.Count > 0)
                    {
                        result.Add(PublishEntity<TEntity, TLanguageAvail>(unitOfWork, model, saveAutomatically: false));
                    }
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
                    versioningManager.ChangeToModified(unitOfWork, entity, new List<PublishingStatus>() { PublishingStatus.Deleted, PublishingStatus.OldPublished, PublishingStatus.Published });
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
                var last = versioningManager.GetLastModifiedVersion<TEntity>(unitOfWork, entity.UnificRootId);
                if (last != null && last.EntityId != entity.Id)
                {
                    throw new PublishLanguageException();
                }

                if (lastPublishedLanguageAction != null && (
                   (newStatus == PublishingStatus.Deleted && entity.LanguageAvailabilities.Where(x => x.LanguageId != model.LanguageId).All(x => x.StatusId == psDeleted)) ||
                   (entity.PublishingStatusId == psPublished && entity.LanguageAvailabilities.Where(x => x.LanguageId != model.LanguageId).All(x => x.StatusId != psPublished))
                ))
                {
                    lastPublishedLanguageAction(unitOfWork, entity);
                }

                if (entity.PublishingStatusId == psDeleted || entity.PublishingStatusId == psOldPublished || entity.PublishingStatusId == psPublished)
                {
                    entity = versioningManager.CreateEntityVersion(unitOfWork, entity, VersioningMode.Standard,
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

                versioningManager.ChangeStatusOfLanguageVersion<TEntity, TLanguageAvail>(unitOfWork, entity, newLanguages);
                AddHistoryMetaData<TEntity, TLanguageAvail>(entity);
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


        public void ExecuteArchiveEntityLanguageVersions<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IReadOnlyList<IVmLocalizedEntityModel> modelList)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            //TODO ADD result messages
            foreach (var model in modelList)
            {
                try
                {
                    ArchiveLanguageVersionWithVersioning<TEntity, TLanguageAvail>(unitOfWork, model);
                }
                catch (Exception)
                {
                    logger.LogError($"MASS TOOL - Archive failed for {model.Id} - {string.Join(";", model.LanguagesAvailabilities.Select(x => $"(L:{x.LanguageId}, S:{x.StatusId}."))}.");
                }
            }
        }
        
        
        public void ExecuteArchiveEntities<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IReadOnlyList<Guid> entityIds)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            //TODO ADD result messages
            var result = new List<TEntity>();
     
            foreach (var entityId in entityIds)
            {
                result.Add(ChangeEntityVersionedToDeleted<TEntity, TLanguageAvail>(unitOfWork, entityId));
            }
        }

        public TEntity ArchiveLanguageVersionWithVersioning<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IVmLocalizedEntityModel model)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
                var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
                var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                var entityRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
                var entity = entityRep.All().Include(x => x.LanguageAvailabilities).Single(x => x.Id == model.Id);

            entity.LanguageAvailabilities.ForEach(x =>
            {
                x.PublishAt = null;
                x.ArchiveAt = null;
            });
            
            if (IsWholeEntityToArchiving<TEntity, TLanguageAvail>(entity, model))
            {
                return ChangeEntityVersionedToDeleted<TEntity, TLanguageAvail>(unitOfWork, entity.Id);
            }

            //New entity version
            var newEntityVersion = versioningManager.CreateEntityVersion(unitOfWork, entity, VersioningMode.Standard,
            entity.PublishingStatusId == psPublished ? PublishingStatus.Published : (PublishingStatus?)null); //Modified

            //Set status
            var newLocalizedModel = model;
            newLocalizedModel.LanguagesAvailabilities.ForEach(x =>
                {
                    x.StatusId = psDeleted;
                }
            );
      
            versioningManager.ChangeStatusOfLanguageVersion<TEntity, TLanguageAvail>(unitOfWork, newEntityVersion, newLocalizedModel.LanguagesAvailabilities);
        
            //Versioning
            AddHistoryMetaData<TEntity, TLanguageAvail>(newEntityVersion);
            return newEntityVersion;
        }

        private VmPublishingResultModel ChangeEntityToModified<TEntity, TLanguageAvail>(Guid entityVersionedId, Func<IUnitOfWork, TEntity,bool> additionalCheckAction = null) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IValidity, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var serviceRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
                var entity = serviceRep.All().Include(j => j.Versioning).Single(x => x.Id == entityVersionedId);
                var historyAction =
                    typesCache.GetByValue<PublishingStatusType>(entity.PublishingStatusId) ==
                    PublishingStatus.Deleted.ToString()
                        ? HistoryAction.Restore
                        : HistoryAction.Withdraw; 
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
                versioningManager.ChangeStatusOfLanguageVersion<TEntity, TLanguageAvail>(unitOfWork, entity, typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString()));
                var affected = versioningManager.ChangeToModified(unitOfWork, entity, new List<PublishingStatus>(){ PublishingStatus.Deleted, PublishingStatus.OldPublished, PublishingStatus.Published});
                AddHistoryMetaData<TEntity, TLanguageAvail>(entity, historyAction);
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
            versioningManager.AddMinorVersion(unitOfWork, entity);
            AddHistoryMetaData<TEntity, TLanguageAvail>(entity, HistoryAction.Delete);
            unitOfWork.Save();
            
            return entity;
        }
        
        public TEntity ChangeEntityVersionedToDeleted<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, Guid entityId) //with one entityVersion 
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
                ChangeEntitiesToDeleted<TEntity, TLanguageAvail>(unitOfWork, new List<TEntity>() {entity});
                versioningManager.AddMinorVersion(unitOfWork, entity);
                AddHistoryMetaData<TEntity, TLanguageAvail>(entity, HistoryAction.Delete);
            }
            return entity;
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
              
                versioningManager.ChangeStatusOfLanguageVersion<TEntity, TLanguageAvail>(unitOfWork, e, newLanguageAvailabilities);
            });
        }

        public VmPublishingResultModel WithdrawEntity<TEntity, TLanguageAvail>(Guid entityVersionedId) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IValidity, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            try
            {
                return ChangeEntityToModified<TEntity, TLanguageAvail>(entityVersionedId);
            }
            catch (InvalidOperationException)
            {
                throw new WithdrawModifiedExistsException();
            }
        }

        private bool CheckModifiedExistsForRoot<TEntity>(IUnitOfWork unitOfWork, Guid rootId) where TEntity : class, IEntityIdentifier, IVersionedVolume, IValidity
        {
            return versioningManager.GetVersionId<TEntity>(unitOfWork, rootId, PublishingStatus.Modified).IsAssigned();
        }
        
        public bool CheckModifiedExists<TEntity>(Guid versionedId) where TEntity : class, IEntityIdentifier, IVersionedVolume, IValidity
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var rootId = versioningManager.GetUnificRootId<TEntity>(unitOfWork, versionedId);
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
                var entityId = versioningManager.GetVersionId<TEntity>(unitOfWork, rootId);
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
            try
            {
                return ChangeEntityToModified<TEntity, TLanguageAvail>(entityVersionedId, additionalCheckAction);
            }
            catch (InvalidOperationException)
            {
                throw new RestoreModifiedExistsException();
            }
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

            var result = versioningManager.PublishVersion(unitOfWork, entity, PublishingStatus.Modified);
            AddHistoryMetaData<TEntity,TLanguageAvail>(entity, HistoryAction.Restore, true);
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

        public IVmOpenApiGuidPageVersionBase GetServicesAndChannelsByOrganization(Guid organizationId, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null)
        {
            var vm = new VmOpenApiEntityGuidPage(pageNumber, pageSize);
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceQuery = unitOfWork.CreateRepository<IServiceVersionedRepository>().All()
                    .Where(s => s.PublishingStatusId == publishedId && s.LanguageAvailabilities.Any(l => l.StatusId == publishedId) &&
                    (s.OrganizationId == organizationId || // Main responsible organization
                    s.OrganizationServices.Any(o => o.OrganizationId == organizationId) || // Other responsible organizations
                    s.ServiceProducers.Any(sp => sp.Organizations.Any(o => o.OrganizationId == organizationId)) // Producers
                    ))
                    .Select(i => new VmOpenApiEntityItem { Id = i.UnificRootId, Created = i.Created, Modified = i.Modified, Type = typeof(Service).Name });
                var channelQuery = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>().All()
                    .Where(c => c.OrganizationId.Equals(organizationId) && c.PublishingStatusId == publishedId && c.LanguageAvailabilities.Any(l => l.StatusId == publishedId))
                    .Select(i => new VmOpenApiEntityItem { Id = i.UnificRootId, Created = i.Created, Modified = i.Modified, Type = typeof(ServiceChannel).Name });

                if (date.HasValue)
                {
                    serviceQuery = serviceQuery.Where(service => service.Modified > date.Value);
                    channelQuery = channelQuery.Where(serviceChannel => serviceChannel.Modified > date.Value);
                }
                if (dateBefore.HasValue)
                {
                    serviceQuery = serviceQuery.Where(service => service.Modified < dateBefore.Value);
                    channelQuery = channelQuery.Where(serviceChannel => serviceChannel.Modified < dateBefore.Value);
                }
                var itemList = serviceQuery.Union(channelQuery);

                vm.SetPageCount(itemList.Count());
                if (vm.IsValidPageNumber())
                {
                    // Get the items for one page
                    vm.ItemList = itemList.OrderBy(o => o.Created).Skip(vm.GetSkipSize()).Take(vm.GetTakeSize()).ToList();
                }
            });

            return vm;
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
            var orgId = versioningManager.GetVersionId<OrganizationVersioned>(unitOfWork, organizationId, null, false);
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

        public void AddHistoryMetaData<TEntity, TLanguageAvail>(TEntity entity, HistoryAction action = HistoryAction.Save, bool setByEntity = false) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            if (entity.Versioning != null)
            {
                entity.Versioning.Meta = JsonConvert.SerializeObject(new VmHistoryMetaData
                {
                    EntityStatusId = entity.PublishingStatusId,
                    HistoryAction = action,
                    LanguagesMetaData = entity.LanguageAvailabilities.Select(x => new VmHistoryMetaDataLanguage
                    {
                        EntityStatusId = setByEntity ? entity.PublishingStatusId : x.StatusId,
                        LanguageId = x.LanguageId
                    }).ToList()
                });
            }
        }

        public void CopyEntity<TEntityVersioned, TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, Guid entityVersionedId, Guid? organizationId)
            where TEntityVersioned : class, IEntityIdentifier, IVersionedVolume<TEntity>, IMultilanguagedEntity<TLanguageAvail>, IOriginalEntity, IOrganizationInfo, INameReferences, new()
            where TEntity : IVersionedRoot, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            var unificRootId = versioningManager.GetUnificRootId<TEntityVersioned>(unitOfWork, entityVersionedId);

            if (unificRootId.HasValue)
            {
                //GetLastVersion 
                var lastEntityVersion = versioningManager.GetLastPublishedDraftVersion<TEntityVersioned>(unitOfWork, unificRootId.Value);
                if (lastEntityVersion != null)
                {
                    var entityRep = unitOfWork.CreateRepository<IRepository<TEntityVersioned>>();
                    var lastPublishedDraftEntity = entityRep.All().Single(x => x.Id == lastEntityVersion.EntityId);

                    var copiedEntity =
                        versioningManager.CreateCopyVersion<TEntityVersioned, TEntity, TLanguageAvail>(unitOfWork,
                            lastPublishedDraftEntity, PublishingStatus.Draft);
                    
                    FinalizeCopyEntity<TEntityVersioned, TLanguageAvail>(copiedEntity, lastPublishedDraftEntity.Id, organizationId);
                   
                }
            }
        }

        public void FinalizeCopyEntity<TEntityVersioned, TLanguageAvail>(TEntityVersioned newEntity, Guid originEntityId, Guid? organizationId)
            where TEntityVersioned : class, IEntityIdentifier, IVersionedVolume, IOriginalEntity, IOrganizationInfo, INameReferences, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            //set organization id
            if (organizationId.HasValue)
            {
                newEntity.OrganizationId = organizationId.Value;
            }
                
            //set previous origin entity
            newEntity.OriginalId = originEntityId;
                
            //set additional to copy name text
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            newEntity.Names.Where(x => x.TypeId == nameTypeId).ForEach(x => x.Name = $"[*] {x.Name}");
            
            AddHistoryMetaData<TEntityVersioned, TLanguageAvail>(newEntity, HistoryAction.Copy);
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
    }
}
