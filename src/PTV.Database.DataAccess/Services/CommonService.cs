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
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.Security;

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
        public IVmDictionaryItemsData<IEnumerable<IVmBase>> GetOrganizationEnum()
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {                
                var result = new VmEnumBase();               
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", GetOrganizationNames(unitOfWork)));                   
                return result.EnumCollection;
            });
        }
        public IVmDictionaryItemsData<IEnumerable<IVmBase>> GetEnumTypes(VmUserInfoBase userInfo)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceClassesRep = unitOfWork.CreateRepository<IServiceClassRepository>();
                var targetGroupRep = unitOfWork.CreateRepository<ITargetGroupRepository>();
                var serviceClasses = TranslationManagerToVm.TranslateAll<ServiceClass, VmListItem>(GetIncludesForFinto<ServiceClass, ServiceClassName>(unitOfWork, serviceClassesRep.All().OrderBy(x => x.Label)));
                var organizationTypeRep = unitOfWork.CreateRepository<IOrganizationTypeRepository>();
                var orgTypes = CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<OrganizationType, OrganizationTypeName>(unitOfWork, organizationTypeRep.All())), x => x.Name);
                orgTypes.ForEach(x => x.IsDisabled = x.Children.Any());
                var targetGroups = CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<TargetGroup, TargetGroupName>(unitOfWork, targetGroupRep.All().OrderBy(x => x.Label))), x => x.Code);

                var digitalAuthorizationRep = unitOfWork.CreateRepository<IDigitalAuthorizationRepository>();
                var digitalAuthorization =
                    TranslationManagerToVm
                        .TranslateAll<DigitalAuthorization, VmListItem>(
                            GetIncludesForFinto<DigitalAuthorization, DigitalAuthorizationName>(unitOfWork, digitalAuthorizationRep.All())
                        );
                var lifeEventRep = unitOfWork.CreateRepository<ILifeEventRepository>();
                var keyWordRep = unitOfWork.CreateRepository<IKeywordRepository>();
                var industrialClassesRep = unitOfWork.CreateRepository<IIndustrialClassRepository>();
                var result = new VmEnumBase();
                var publishingStatuses = GetPublishingStatuses();
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("AreaInformationTypes", GetAreaInformationTypes()),
                    () => GetEnumEntityCollectionModel("AreaTypes", GetAreaTypes()),
                    () => GetEnumEntityCollectionModel("AstiTypes", GetExtraSubTypes()),
                    () => GetEnumEntityCollectionModel("BusinessRegions", GetAreas(unitOfWork, AreaTypeEnum.BusinessRegions)),
                    () => GetEnumEntityCollectionModel("DigitalAuthorizations", digitalAuthorization),
                    () => GetEnumEntityCollectionModel("ChargeTypes", GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("ServiceChannelConnectionTypes", GetServiceChannelConnectionTypes()),
                    () => GetEnumEntityCollectionModel("DialCodes", GetDefaultDialCode(unitOfWork)),
                    () => GetEnumEntityCollectionModel("HospitalRegions", GetAreas(unitOfWork, AreaTypeEnum.HospitalRegions)),
                    () => GetEnumEntityCollectionModel("Languages", GetLanguages()),
                    () => GetEnumEntityCollectionModel("TranslationLanguages", GetTranslationLanguages()),
                    //() => GetEnumEntityCollectionModel("Keywords", TranslationManagerToVm.TranslateAll<Keyword, VmKeywordItem>(keyWordRep.All().OrderBy(x => x.Name))),
                    () => GetEnumEntityCollectionModel("Municipalities", GetMunicipalities(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Organizations", GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ServiceClasses", serviceClasses),
                    () => GetEnumEntityCollectionModel("Provinces", GetAreas(unitOfWork, AreaTypeEnum.Province)),
                    () => GetEnumEntityCollectionModel("PublishingStatuses", publishingStatuses),
                    () => GetEnumEntityCollectionModel("ServiceTypes", GetServiceTypes()),
                    () => GetEnumEntityCollectionModel("TopTargetGroups", CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<TargetGroup, TargetGroupName>(unitOfWork, targetGroupRep.All().OrderBy(x => x.Label)), 1), x => x.Code)),
                    () => GetEnumEntityCollectionModel("TopLifeEvents", CreateTree<VmTreeItem>(LoadFintoTree(GetIncludesForFinto<LifeEvent, LifeEventName>(unitOfWork, lifeEventRep.All()), 1), x => x.Name)),
                    () => GetEnumEntityCollectionModel("TopServiceClasses", CreateTree<VmTreeItem>(LoadFintoTree(GetIncludesForFinto<ServiceClass, ServiceClassName>(unitOfWork, serviceClassesRep.All().Include(x => x.Descriptions)), 1), x => x.Name)),
                    () => GetEnumEntityCollectionModel("TopDigitalAuthorizations", CreateTree<VmTreeItem>(LoadFintoTree(GetIncludesForFinto<DigitalAuthorization, DigitalAuthorizationName>(unitOfWork, digitalAuthorizationRep.All()), 1), x => x.Name)),
                    () => GetEnumEntityCollectionModel("IndustrialClasses", TranslationManagerToVm.TranslateAll<IFintoItem, VmTreeItem>(GetIncludesForFinto<IndustrialClass, IndustrialClassName>(unitOfWork, industrialClassesRep.All().Where(x => x.Code == "5").OrderBy(x => x.Label))).ToList()),
                    () => GetEnumEntityCollectionModel("PhoneNumberTypes", GetPhoneTypes()),
                    () => GetEnumEntityCollectionModel("FundingTypes", GetServiceFundingTypes()),
                    () => GetEnumEntityCollectionModel("ChannelTypes", GetServiceChannelTypes()),
                    () => GetEnumEntityCollectionModel("ProvisionTypes", GetProvisionTypes()),
                    () => GetEnumEntityCollectionModel("TargetGroups", targetGroups),
                    () => GetEnumEntityCollectionModel("OrganizationTypes", orgTypes),
                    () => GetEnumEntityCollectionModel("PrintableFormUrlTypes", GetPrintableFormUrlTypes()),
                    () => GetEnumEntityCollectionModel("OrganizationAreaInformations", GetDefaultAreaInformationList(userInfo.UserOrganization, unitOfWork)));

            return result.EnumCollection;
            });
        }

        public IVmBase GetTypedData(IEnumerable<string> dataTypes)
        {
            return new VmListItemsData<IVmBase>(dataServiceFetcher.Fetch(dataTypes));
        }

        public VmListItemsData<VmListItem> GetExtraSubTypes()
        {
            return dataServiceFetcher.FetchType<ExtraSubType>();
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
                                                                     .Include(i => i.OrganizationDisplayNameTypes)
                                                                     .Include(i => i.LanguageAvailabilities));
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

        public List<IVmTreeItem> GetOrganizations(IUnitOfWork unitOfWork)
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
            var tCodes = configuration.GetTranslationLanguages();
            return GetLanguages().Where(x => tCodes.Contains(x.Code)).ToList();
        }

        public VmListItemsData<VmListItem> GetServiceChannelTypes()
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
            var areas = unitOfWork.ApplyIncludes(areaRep.All().Where(x=>x.AreaTypeId == typesCache.Get<AreaType>(type.ToString()) && x.IsValid), i => i.Include(j => j.AreaNames).Include(j=>j.AreaMunicipalities));
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

        public PublishingResult PublishEntity<TEntity, TLanguageAvail>(IVmPublishingModel model) where TEntity  : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new() where TLanguageAvail : class, ILanguageAvailability
        {
            return contextManager.ExecuteWriter(unitOfWork => PublishEntity<TEntity, TLanguageAvail>(unitOfWork, model));
        }

        public PublishingResult PublishEntity<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IVmPublishingModel model)
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
            }

            versioningManager.ChangeStatusOfLanguageVersion<TEntity, TLanguageAvail>(unitOfWork, newPublished, model.LanguagesAvailabilities);
            unitOfWork.Save();
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

                if (lastPublishedLanguageAction != null && entity.PublishingStatusId == psPublished &&
                    entity.LanguageAvailabilities
                        .Where(x => x.LanguageId != model.LanguageId)
                        .All(x => x.StatusId != psPublished))
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
                unitOfWork.Save();
                return entity;
            });
        }

        private VmPublishingResultModel ChangeEntityToModified<TEntity, TLanguageAvail>(Guid entityVersionedId, Func<IUnitOfWork, TEntity,bool> additionalCheckAction = null) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
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
                versioningManager.ChangeStatusOfLanguageVersion<TEntity, TLanguageAvail>(unitOfWork, entity, typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString()));
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

        public TEntity ChangeEntityToDeleted<TEntity>(IUnitOfWorkWritable unitOfWork, Guid entityId) where TEntity : class, IEntityIdentifier, IVersionedVolume
        {
            var publishingStatusDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());

            var statesToDelete = new List<Guid>
            {
                typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString()),
                typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString()),
                typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString())
            };

            var serviceRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entity = serviceRep.All().Single(x => x.Id == entityId);

            var allEntities = serviceRep.All().Where(x => x.UnificRootId == entity.UnificRootId && statesToDelete.Contains(x.PublishingStatusId));
            allEntities.ForEach(e =>
            {
                e.PublishingStatus = null;
                e.PublishingStatusId = publishingStatusDeleted;
            });

            unitOfWork.Save();

            return entity;
        }

        public VmPublishingResultModel WithdrawEntity<TEntity, TLanguageAvail>(Guid entityVersionedId) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
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

        public VmPublishingResultModel WithdrawEntityByRootId<TEntity, TLanguageAvail>(Guid rootId) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
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

            return WithdrawEntity<TEntity, TLanguageAvail>(entity.Id);

        }

        public VmPublishingResultModel RestoreEntity<TEntity, TLanguageAvail>(Guid entityVersionedId, Func<IUnitOfWork, TEntity, bool> additionalCheckAction = null) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
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

        public IVmOpenApiGuidPageVersionBase GetServicesAndChannelsByOrganization(Guid organizationId, DateTime? date, int pageNumber, int pageSize)
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
    }
}
