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
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS O
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.DirectRaw;
using PTV.Database.DataAccess.Extensions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Database.Model.Views;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;


namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ISearchService), RegisterType.Transient)]
    [RegisterService(typeof(ISearchServiceInternal), RegisterType.Transient)]
    internal class SearchService : ServiceBase, ISearchServiceInternal
    {

        private readonly IContextManager contextManager;
        private readonly IDatabaseRawContext rawContext;
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly ILanguageOrderCache languageOrderCache;
        private readonly ICommonServiceInternal commonService;
        private readonly IPahaTokenAccessor pahaTokenAccessor;
        private readonly ITargetGroupDataCache targetGroupDataCache;
        private readonly IUserOrganizationService userOrganizationService;
        private readonly IExpirationService expirationService;

        private readonly List<Guid> nonArchivedStatusIds;
        private readonly List<Guid> archivedStatusIds;

        public SearchService(
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IContextManager contextManager,
            ITypesCache typesCache,
            ILanguageCache languageCache,
            ILanguageOrderCache languageOrderCache,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            ICommonServiceInternal commonService,
            IDatabaseRawContext rawContext,
            IPahaTokenAccessor pahaTokenAccessor,
            ITargetGroupDataCache targetGroupDataCache,
            IVersioningManager versioningManager,
            IUserOrganizationService userOrganizationService,
            IExpirationService expirationService
            )
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker, versioningManager)
        {
            this.contextManager = contextManager;
            this.typesCache = typesCache;
            this.languageCache = languageCache;
            this.commonService = commonService;
            this.rawContext = rawContext;
            this.languageOrderCache = languageOrderCache;
            this.pahaTokenAccessor = pahaTokenAccessor;
            this.targetGroupDataCache = targetGroupDataCache;
            this.userOrganizationService = userOrganizationService;
            this.expirationService = expirationService;
            
            this.nonArchivedStatusIds = new List<Guid>
            {
                typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString()),
                typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString()),
                typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString())
            };
            this.archivedStatusIds = new List<Guid>
            {
                typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString()),
                typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString())
            };
        }

        public IVmSearchBase SearchEntities(IVmEntitySearch vmEntitySearch)
        {
            var vmEntitySearchByStatus = GetVmEntitySearchByStatus(vmEntitySearch);
            var returnData = Search(vmEntitySearchByStatus);

            contextManager.ExecuteReader(unitOfWork =>
            {
                FillSearchedData(returnData.SearchResult, vmEntitySearchByStatus.Expiration, unitOfWork);
            });


            FillEnumEntities(returnData, () =>
            {
                var usedOrgIds = returnData.SearchResult
                    .SelectMany(org => org.Organizations)
                    .Union(returnData.SearchResult.SelectMany(org => org.Producers))
                    .Union(returnData.SearchResult.Where(ent => ent.OrganizationId.HasValue).Select(ent => ent.OrganizationId.Value))
                    .Distinct()
                    .ToList();

                return GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizations(usedOrgIds));
            });
            return returnData;
        }

        public VmSearchResult<IVmEntityListItem> SearchEntities(IVmEntitySearch vmEntitySearch, IUnitOfWork unitOfWork)
        {
            vmEntitySearch.SearchType = vmEntitySearch.SearchType ?? SearchTypeEnum.Other;
            var vmEntitySearchByStatus = GetVmEntitySearchByStatus(vmEntitySearch);
            var returnData = Search(vmEntitySearchByStatus);
            FillSearchedData(returnData.SearchResult, vmEntitySearchByStatus.Expiration, unitOfWork);

            FillEnumEntities(returnData, () =>
            {
                var usedOrgIds = returnData.SearchResult
                    .SelectMany(org => org.Organizations)
                    .Union(returnData.SearchResult.SelectMany(org => org.Producers))
                    .Union(returnData.SearchResult.Where(ent => ent.OrganizationId.HasValue).Select(ent => ent.OrganizationId.Value))
                    .Distinct()
                    .ToList();

                return GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizations(usedOrgIds));
            });
            return returnData;
        }

        private VmEntitySearchByStatus GetVmEntitySearchByStatus(IVmEntitySearch vmEntitySearch)
        {
            if (!vmEntitySearch.UseOnlySelectedStatuses)
            {
                commonService.ExtendPublishingStatusesByEquivalents(vmEntitySearch.SelectedPublishingStatuses);
            }

            var vmEntitySearchByStatus = new VmEntitySearchByStatus(vmEntitySearch, nonArchivedStatusIds, archivedStatusIds);
            return vmEntitySearchByStatus;
        }

        private VmSearchResult<IVmEntityListItem> Search(IVmEntitySearchByStatus vmEntitySearch)
        {
            vmEntitySearch.ContentTypeEnum = vmEntitySearch.ContentTypes.Any() ? vmEntitySearch.ContentTypes.Aggregate((i, j) => i | j) : SearchEntityTypeEnum.All;
            VmSearchResult<IVmEntityListItem> returnData = new VmSearchResult<IVmEntityListItem> {SearchResult = new List<IVmEntityListItem>()};

            if (InvalidSearchCriteria(vmEntitySearch))
            {
                return returnData;
            }

            FilterSearchCriteria(vmEntitySearch);
            var isChannelSearchInvoked = false;
            var isServiceSearchInvoked = false;
            var tempSql = string.Empty;
            foreach (var contentType in vmEntitySearch.ContentTypes)
            {
                switch (contentType)
                {
                    case SearchEntityTypeEnum.Service:
                    case SearchEntityTypeEnum.ServicePermit:
                    case SearchEntityTypeEnum.ServiceProfessional:
                    case SearchEntityTypeEnum.ServiceService:
                        if (!isServiceSearchInvoked && IsSearchTypeUsed(vmEntitySearch, contentType))
                        {
                            isServiceSearchInvoked = true;
                            tempSql = AddSqlToUnion(tempSql, vmEntitySearch, GetSearchServiceSql<ServiceVersioned>,
                                GetSearchServiceSql<VLastArchivedService>);
                        }

                        break;
                    case SearchEntityTypeEnum.Channel:
                    case SearchEntityTypeEnum.EChannel:
                    case SearchEntityTypeEnum.Phone:
                    case SearchEntityTypeEnum.PrintableForm:
                    case SearchEntityTypeEnum.ServiceLocation:
                    case SearchEntityTypeEnum.WebPage:
                        if (!isChannelSearchInvoked && IsSearchTypeUsed(vmEntitySearch, contentType))
                        {
                            isChannelSearchInvoked = true;
                            tempSql = AddSqlToUnion(tempSql, vmEntitySearch,
                                GetSearchChannelSql<ServiceChannelVersioned>,
                                GetSearchChannelSql<VLastArchivedServiceChannel>);
                        }

                        break;
                    case SearchEntityTypeEnum.GeneralDescription:
                        if (IsSearchTypeUsed(vmEntitySearch, contentType))
                        {
                            tempSql = AddSqlToUnion(tempSql, vmEntitySearch,
                                GetSearchGeneralDescriptionSql<StatutoryServiceGeneralDescriptionVersioned>,
                                GetSearchGeneralDescriptionSql<VLastArchivedGeneralDescription>);
                        }
                        break;
                    case SearchEntityTypeEnum.Organization:
                        if (IsSearchTypeUsed(vmEntitySearch, contentType))
                        {
                            tempSql = AddSqlToUnion(tempSql, vmEntitySearch,
                                GetSearchOrganizationSql<OrganizationVersioned>,
                                GetSearchOrganizationSql<VLastArchivedOrganization>);
                        }
                        break;
                    case SearchEntityTypeEnum.ServiceCollection:
                        if (IsSearchTypeUsed(vmEntitySearch, contentType))
                        {
                            tempSql = AddSqlToUnion(tempSql, vmEntitySearch,
                                GetSearchServiceCollectionSql<ServiceCollectionVersioned>,
                                GetSearchServiceCollectionSql<VLastArchivedServiceCollection>);
                        }
                        break;
                }
            }

            if (string.IsNullOrEmpty(tempSql))
            {
                return returnData;
            }

            var pageSize = vmEntitySearch.MaxPageCount > 0
                ? vmEntitySearch.MaxPageCount
                : CoreConstants.MaximumNumberOfAllItems;
            var totalCountSql = GetSqlCountEntitySelect(tempSql);
            var searchSql = GetSqlAllEntityPageSelect(
                GetSqlAllEntityOrderSelect(
                    tempSql,
                    vmEntitySearch.SortData,
                    new VmSortParam {Column = "Modified", SortDirection = SortDirectionEnum.Desc}),
                page: vmEntitySearch.PageNumber, pageSize: pageSize );

            var (totalCount,result) = ExecuteSearch(searchSql, totalCountSql, vmEntitySearch);

            var safePageNumber = vmEntitySearch.PageNumber.PositiveOrZero();
            var moreAvailable = totalCount.MoreResultsAvailable(safePageNumber);
            returnData.Count = totalCount;
            returnData.PageNumber = ++safePageNumber;
            returnData.MoreAvailable = moreAvailable;
            returnData.SearchResult = result.InclusiveToList();

            return returnData;
        }

        private string AddSqlToUnion(
            string sqlUnion, 
            IVmEntitySearchByStatus vmEntitySearch,
            Func<IVmEntitySearchByStatus, string> entityQuery,
            Func<IVmEntitySearchByStatus, string> viewQuery)
        {
            if (vmEntitySearch.SelectedPublishingStatuses.Any())
            {
                sqlUnion = AddSqlToUnion(sqlUnion, entityQuery(vmEntitySearch));
            }
        
            if (vmEntitySearch.SelectedArchivedStatuses.Any())
            {
                sqlUnion = AddSqlToUnion(sqlUnion, viewQuery(vmEntitySearch));
            }
        
            return sqlUnion;
        }

        private void FillSearchedData(IReadOnlyList<IVmEntityListItem> result, DateTime expiration, IUnitOfWork unitOfWork)
        {
            FillExpiration(result, expiration, unitOfWork);
            FillServicesInformation(result, unitOfWork);
            FillOrganizationsInformation(result, unitOfWork);
            FillChannelsInformation(result, unitOfWork);
            FillServiceCollectionsInformation(result, unitOfWork);
            FillGeneralDescriptionsInformation(result, unitOfWork);
        }

        private void FillExpiration(IReadOnlyList<IVmEntityListItem> result, DateTime expiration, IUnitOfWork unitOfWork)
        {
            if (expiration != default(DateTime))
            {
                var lifeTime = expiration.ToEpochTime();
                var dateUtcNow = DateTime.UtcNow.ToEpochTime();
                foreach (var x in result)
                {
                    if (x.ExpireOn != null) continue;
                    var modified = expirationService.GetNonEvaModifiedDate(x.Modified.FromEpochTime(), x.LastOperationType,
                        x.VersioningId, unitOfWork);
                    x.ExpireOn = dateUtcNow + (modified.ToEpochTime() - lifeTime);
                }
            }
        }

        private bool InvalidSearchCriteria(IVmEntitySearch vmEntitySearch)
        {
            switch (vmEntitySearch.SearchType)
            {
                case SearchTypeEnum.Id:
                    return !vmEntitySearch.Id.IsAssigned();
                case SearchTypeEnum.Phone:
                    return (vmEntitySearch.IsLocalNumber != true && !vmEntitySearch.PhoneDialCode.IsAssigned());
                case SearchTypeEnum.Address:
                    return !vmEntitySearch.AddressStreetId.IsAssigned() && !vmEntitySearch.PostalCodeId.IsAssigned();
                case null:
                    return true;
                default:
                    return false;
            }
        }

        private (int,IList<VmEntityListItem>) ExecuteSearch(string sqlSearch, string sqlCount, IVmEntitySearchByStatus vmEntitySearch)
        {
            return rawContext.ExecuteReaderAsync(async db => (await db.SelectOneAsync<int>(sqlCount, vmEntitySearch), await db.SelectListAsync<VmEntityListItem>(sqlSearch, vmEntitySearch))).Result;
        }

        private (int,IList<T>) ExecuteSearch<T>(string sqlSearch, string sqlCount, IVmEntitySearch vmEntitySearch)
        {
            return rawContext.ExecuteReaderAsync(async db => (await db.SelectOneAsync<int>(sqlCount, vmEntitySearch), await db.SelectListAsync<T>(sqlSearch, vmEntitySearch))).Result;
        }

        private int ExecuteSearchCount(string sqlCount, IVmEntitySearch vmEntitySearch)
        {
            return rawContext.ExecuteReaderAsync(async db => (await db.SelectOneAsync<int>(sqlCount, vmEntitySearch))).Result;
        }

        private List<Guid> GetServiceTypeCriteria(IVmEntitySearch vmEntitySearch)
        {
            var result =  new List<Guid>();

            if (vmEntitySearch.ContentTypes.Contains(SearchEntityTypeEnum.ServiceService))
            {
                result.Add(typesCache.Get<ServiceType>(ServiceTypeEnum.Service.ToString()));
            }
            if (vmEntitySearch.ContentTypes.Contains(SearchEntityTypeEnum.ServicePermit))
            {
                result.Add(typesCache.Get<ServiceType>(ServiceTypeEnum.PermissionAndObligation.ToString()));
            }
            if (vmEntitySearch.ContentTypes.Contains(SearchEntityTypeEnum.ServiceProfessional))
            {
                result.Add(typesCache.Get<ServiceType>(ServiceTypeEnum.ProfessionalQualifications.ToString()));
            }

            return result;
        }

        private bool ApplySearchByName(IVmEntitySearch entitySearch)
        {
            return entitySearch.SearchType == SearchTypeEnum.Name && !string.IsNullOrEmpty(entitySearch.Name);
        }

        private bool ApplySearchById(IVmEntitySearch entitySearch)
        {
            return entitySearch.SearchType == SearchTypeEnum.Id && entitySearch.Id.IsAssigned();
        }

        private bool ApplySearchByPhone(IVmEntitySearch entitySearch)
        {
            return entitySearch.SearchType == SearchTypeEnum.Phone
                  // && !string.IsNullOrEmpty(entitySearch.PhoneNumber)
                   && (entitySearch.PhoneDialCode.IsAssigned() || entitySearch.IsLocalNumber == true);
        }

        private bool ApplySearchByAddress(IVmEntitySearch entitySearch)
        {
            return entitySearch.SearchType == SearchTypeEnum.Address &&
                   (entitySearch.AddressStreetId.IsAssigned() || entitySearch.PostalCodeId.IsAssigned());
        }

        private bool ApplySearchByEmail(IVmEntitySearch entitySearch)
        {
            return entitySearch.SearchType == SearchTypeEnum.Email && !string.IsNullOrEmpty(entitySearch.Email);
        }

        private void FilterSearchCriteria(IVmEntitySearch vmEntitySearch)
        {
            if (vmEntitySearch.ContentTypes == null || !vmEntitySearch.ContentTypes.Any())
            {
                vmEntitySearch.ContentTypes = Enum.GetValues(typeof(SearchEntityTypeEnum)).Cast<SearchEntityTypeEnum>().ToList();
            }

            vmEntitySearch.ServiceServiceType = GetServiceTypeCriteria(vmEntitySearch);
            vmEntitySearch.Name = vmEntitySearch.Name != null ? Regex.Replace(vmEntitySearch.Name.Trim(), @"\s+", " ").ToLower() : vmEntitySearch.Name;

            var isGdSelected = vmEntitySearch.ContentTypes.All(x =>  new List<SearchEntityTypeEnum>
            {
                SearchEntityTypeEnum.GeneralDescription
            }.Contains(x));

            // subFilters are enabled for either services, or GD, but not both
            var isSubFiltersEnabled = isGdSelected || vmEntitySearch.ContentTypes.All(x => new List<SearchEntityTypeEnum>
            {
                SearchEntityTypeEnum.Service,
                SearchEntityTypeEnum.ServicePermit,
                SearchEntityTypeEnum.ServiceProfessional,
                SearchEntityTypeEnum.ServiceService
            }.Contains(x));

            if (!isSubFiltersEnabled)
            {
                vmEntitySearch.IndustrialClasses = new List<Guid>();
                vmEntitySearch.LifeEvents = new List<Guid>();
                vmEntitySearch.OntologyTerms = new List<Guid>();
                vmEntitySearch.ServiceClasses = new List<Guid>();
                vmEntitySearch.TargetGroups = new List<Guid>();
                vmEntitySearch.ServiceTypes = new List<Guid>();
                vmEntitySearch.GeneralDescriptionTypes = new List<Guid>();
                vmEntitySearch.ServiceTypes = new List<Guid>();
            }

            if (!isGdSelected)
            {
                vmEntitySearch.GeneralDescriptionTypes = new List<Guid>();
                vmEntitySearch.ServiceTypes = new List<Guid>();
            }

            if (isSubFiltersEnabled && vmEntitySearch.TargetGroups != null)
            {
                var selectedTg = vmEntitySearch.TargetGroups.Any()
                    ? targetGroupDataCache.All().Where(x => vmEntitySearch.TargetGroups.Contains(x.Id)).Select(x=>x.Code.ToUpper())
                    : targetGroupDataCache.All().Select(x=>x.Code.ToUpper());
                if (!selectedTg.Contains("KR1"))
                {
                    vmEntitySearch.LifeEvents = new List<Guid>();
                }
                if (!selectedTg.Contains("KR2"))
                {
                    vmEntitySearch.IndustrialClasses = new List<Guid>();
                }
            }
            
            vmEntitySearch.UserName = pahaTokenAccessor.UserName.ToLower();
            if (vmEntitySearch.LanguageIds == null || !vmEntitySearch.LanguageIds.Any())
            {
                vmEntitySearch.LanguageIds = vmEntitySearch.Languages.Select(code => languageCache.Get(code)).ToList();
            }
            
            if (!string.IsNullOrEmpty(vmEntitySearch.Language) && !vmEntitySearch.LanguageId.HasValue)
            {
                if (vmEntitySearch.Languages.Contains(vmEntitySearch.Language) && vmEntitySearch.SortData.Any(x=>x.Column == SqlConstants.Name))
                {
                    vmEntitySearch.LanguageId = languageCache.Get(vmEntitySearch.Language);
                }
            }
        }

        #region Services SQL
        private string GetSearchServiceSql<TTable>(IVmEntitySearchByStatus vmEntitySearch)
        {
            #region SearchByFilterParam

            var tableName = typeof(TTable).Name;
            var resultSelect = GetSqlServiceSelect<TTable>(vmEntitySearch);

            if (vmEntitySearch.EntityIds != null && vmEntitySearch.EntityIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<TTable>("EntityIds", "UnificRootId"));

            } else if (vmEntitySearch.EntityVersionIds != null && vmEntitySearch.EntityVersionIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<TTable>("EntityVersionIds", "Id"));

            }
            else
            {
                //My content
                if (vmEntitySearch.MyContent)
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlMyContent());
                }

                //Service classes
                if (vmEntitySearch.ServiceClasses != null && vmEntitySearch.ServiceClasses.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlServiceFintoClause<ServiceClass, TTable>("ServiceClasses"));
                }
                //Life events
                if (vmEntitySearch.LifeEvents != null && vmEntitySearch.LifeEvents.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlServiceFintoClause<LifeEvent, TTable>("LifeEvents"));
                }

                //Industrial classes
                if (vmEntitySearch.IndustrialClasses != null && vmEntitySearch.IndustrialClasses.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlServiceFintoClause<IndustrialClass, TTable>("IndustrialClasses"));
                }

                //Ontology terms
                if (vmEntitySearch.OntologyTerms != null && vmEntitySearch.OntologyTerms.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlServiceFintoClause<OntologyTerm, TTable>("OntologyTerms"));
                }

                //Target groups
                if (vmEntitySearch.TargetGroups != null && vmEntitySearch.TargetGroups.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlServiceFintoClause<TargetGroup, TTable>("TargetGroups", true));
                }

                // Area Type
                if (vmEntitySearch.AreaInformationTypes != null && vmEntitySearch.AreaInformationTypes.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<TTable>("AreaInformationTypes", "AreaInformationTypeId"));
                }

                if (vmEntitySearch.OrganizationIds != null && vmEntitySearch.OrganizationIds.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlServiceOrganization<TTable>("OrganizationIds"));
                }

                if (ApplySearchById(vmEntitySearch))
                {
                    var rootId = vmEntitySearch.Id;
                    if (rootId.IsAssigned())
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlName($"{tableName}.UnificRootId", " = '" + rootId + "'"));
                    }
                }

                if (ApplySearchByName(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchName<TTable>());
                }
                if (ApplySearchByEmail(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchEmail<TTable>());
                }

                if (vmEntitySearch.LanguageIds != null && vmEntitySearch.LanguageIds.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<ServiceLanguageAvailability>("LanguageIds", "LanguageId"));

                }

                if (vmEntitySearch.ServiceServiceType.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlServiceType<TTable>("ServiceServiceType"));
                }
            }

            resultSelect = AddPublishingStatusFilters<TTable>(vmEntitySearch, resultSelect);

            var additionalGroupBy =
                GetSqlName($"{tableName}.Expiration", ", ") + 
                GetSqlName($"{tableName}.OrganizationId", ", ") +
                GetSqlName($"{tableName}.OriginalId", ", ") +
                GetSqlName(vmEntitySearch.UseLocalizedSubType
                ? SqlConstants.LocalizedSubEntityType + "\",\"" + SqlConstants.SubEntityType
                : SqlConstants.SubEntityType);

            var additionalPreferredGroupBy = vmEntitySearch.UsePreferredPublishingStatus
                ? GetSqlName(SqlConstants.ServiceUnificID, ",") + GetSqlName(SqlConstants.PreferredPublishinStatus)
                : string.Empty;

            resultSelect = resultSelect + GetSqlEntityOrder<TTable>(additionalGroupBy, additionalPreferredGroupBy);

            #endregion SearchByFilterParam

            return resultSelect;
        }

        private string AddPublishingStatusFilters<TTable>(IVmEntitySearchByStatus vmEntitySearch, string resultSelect)
        {
            var isArchivedView = typeof(TTable).GetInterfaces().Any(i => i == typeof(ILastArchivedView));
            if (!isArchivedView && vmEntitySearch.SelectedPublishingStatuses.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<TTable>("SelectedPublishingStatuses", "PublishingStatusId"));
            }

            if (isArchivedView && vmEntitySearch.SelectedArchivedStatuses.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<TTable>("SelectedArchivedStatuses", "PublishingStatusId"));
            }

            return resultSelect;
        }

        private string GetSqlServiceSelect<TTable>(IVmEntitySearch vmEntitySearch)
        {
            var tableName = typeof(TTable).Name;
            var distinct = vmEntitySearch.UsePreferredPublishingStatus
                ? " SELECT DISTINCT ON ("+SqlConstants.ServiceUnificID+") " + GetSqlSelectNameAs($"{tableName}.UnificRootId", SqlConstants.ServiceUnificID)
                : SqlStatement.SelectDistinct;
            return distinct +
                   GetSqlEntityColumns<TTable, ServiceName>() +
                   GetSqlSelectName($"{tableName}.OrganizationId") +
                   GetSqlSelectName($"{tableName}.OriginalId") +
                   GetSqlSelectEpochTimeNameAs($"{tableName}.Expiration", "ExpireOn") +
                   GetSqlSelectStringValueAs(SqlConstants.Service, SqlConstants.EntityType) +
                   GetSqlSelectServiceSubType(tableName) +
                   (vmEntitySearch.UseLocalizedSubType ? GetSqlSelectServiceSubTypeLocalized(languageCache.Get(vmEntitySearch.Language)) : string.Empty) +
                   GetSqlSelectEntityNameType("ServiceName", SqlConstants.NameType, false) +
                   GetPreferredPublishingStatus<TTable>(vmEntitySearch) +
                   GetOrganizationSorting<TTable>(vmEntitySearch) +
                   SqlStatement.From + GetSqlName(typeof(TTable).Name, " ") +
                   GetSqlInnerJoin("ServiceName", $"{tableName}.Id", "ServiceName.ServiceVersionedId") +
                   GetSqlInnerJoin("ServiceLanguageAvailability", $"{tableName}.Id", "ServiceLanguageAvailability.ServiceVersionedId") +
                   GetSqlLanguageOrderNumberInnerJoin("Language", "ServiceName.LocalizationId", "Language.Id", vmEntitySearch.LanguageId)+
                   GetSqlLeftJoin("ServiceType", $"{tableName}.TypeId", "ServiceType.Id") +
                   GetSqlInnerJoin("Versioning", $"{tableName}.VersioningId", "Versioning.Id");
        }

        private string GetPreferredPublishingStatus<TEntity>(IVmEntitySearch vmEntitySearch)
        {
            if (!vmEntitySearch.UsePreferredPublishingStatus) return string.Empty;
            var entity = typeof(TEntity).Name;
            return ", (CASE  WHEN " + GetSqlName(entity + ".PublishingStatusId") +
                   " = '" + PublishingStatusCache.Get(PublishingStatus.Published) + "' THEN 0 " +
                   " WHEN " + GetSqlName(entity + ".PublishingStatusId") +
                   " = '" + PublishingStatusCache.Get(PublishingStatus.Draft) + "' THEN 1 " +
                   " ELSE 2 END) AS "+ GetSqlName(SqlConstants.PreferredPublishinStatus);
        }

        private string GetOrganizationSorting<TEntity>(IVmEntitySearch vmEntitySearch)
        {
            if (!vmEntitySearch.UseOrganizationSorting) return string.Empty;
            CultureInfo ci = CultureInfo.GetCultureInfo(vmEntitySearch.Language);
            StringComparer comp = StringComparer.Create(ci, true);
            var entity = typeof(TEntity).Name;
            var userOrgsAll = userOrganizationService.GetAllUserOrganizations(new List<PublishingStatus>
                {PublishingStatus.Draft, PublishingStatus.Published, PublishingStatus.Modified});
            var userOrgs = userOrgsAll.ToDictionary(k => k.Id,
                    v => (v.Translation.Texts.ContainsKey(vmEntitySearch.Language)
                        ? v.Translation.Texts[vmEntitySearch.Language]
                        : v.Translation.Texts.FirstOrDefault().Value ?? v.Name))
                .OrderByDescending(x=>x.Value, comp);
            var i = 0;
            var result = ", (CASE ";
            userOrgs.ForEach(org =>
                {
                    result += string.Format(" WHEN {0} = '{1}' THEN {2} ", GetSqlName(entity + ".OrganizationId"), org.Key ,i);
                    i++;
                });
            result += " ELSE "+i+" END) AS "+ GetSqlName(SqlConstants.Organization);
            return result;
        }

        private string GetSqlSelectServiceSubType(string tableName)
        {
            return "(case when " + GetSqlName($"{tableName}.TypeId") + " is not null " +
                " then " + GetSqlName("ServiceType.Code") +
                " else ( " + SqlStatement.SelectDistinct + "gdsertype.\"Code\"" +
                SqlStatement.From + GetSqlName("ServiceType", " as gdsertype") +
                GetSqlInnerJoin("StatutoryServiceGeneralDescriptionVersioned",
                    "StatutoryServiceGeneralDescriptionVersioned.UnificRootId",
                    "StatutoryServiceGeneralDescriptionId") +
                "where gdsertype.\"Id\" = " + GetSqlName("StatutoryServiceGeneralDescriptionVersioned.TypeId") +
                " And " + GetSqlName("PublishingStatusId"," = '" + PublishingStatusCache.Get(PublishingStatus.Published) + "'") +
                ")ENd)AS " + GetSqlName(SqlConstants.SubEntityType, ",");
        }

        private string GetSqlSelectServiceSubTypeLocalized(Guid languageId)
        {
            return "(case when " + GetSqlName("ServiceVersioned.TypeId") + " is not null " +
                   " then " +
                   " ( " + SqlStatement.SelectDistinct + "serType.\"Name\"" +
                   SqlStatement.From + GetSqlName("ServiceTypeName", " as serType") +
                   " Where serType.\"TypeId\" = " + GetSqlName("ServiceVersioned.TypeId") +
                   " And serType.\"LocalizationId\" = '" +languageId +"')" +
                   " else ( " + SqlStatement.SelectDistinct + "gdsertype.\"Name\"" +
                   SqlStatement.From + GetSqlName("ServiceTypeName", " as gdsertype") +
                   GetSqlInnerJoin("StatutoryServiceGeneralDescriptionVersioned",
                       "StatutoryServiceGeneralDescriptionVersioned.UnificRootId",
                       "StatutoryServiceGeneralDescriptionId") +
                   " Where gdsertype.\"TypeId\" = " + GetSqlName("StatutoryServiceGeneralDescriptionVersioned.TypeId") +
                   " And gdsertype.\"LocalizationId\" = '" +languageId +"'"+
                   " And " + GetSqlName("PublishingStatusId"," = '" + PublishingStatusCache.Get(PublishingStatus.Published) + "'") +
                   ")ENd)AS " + GetSqlName(SqlConstants.LocalizedSubEntityType, ",");
        }

        private string GetSqlServiceType<TTable>(string collectionName)
        {
            var result = "( " + GetSqlInClause<TTable>(collectionName, "TypeId") +
                         " OR " + GetSqlName("StatutoryServiceGeneralDescriptionId") + " IN (" +
                         " Select " + GetSqlName("UnificRootId") + " from " +
                         GetSqlName("StatutoryServiceGeneralDescriptionVersioned") +
                         " Where " + GetSqlName("PublishingStatusId",
                             " = '" + PublishingStatusCache.Get(PublishingStatus.Published) + "'") +
                         " AND " + GetSqlInClause(collectionName, "TypeId") + "))";
            return result;
        }
        private string GetSqlServiceFintoClause<TFinto, TTable>(string collectionName, bool isTargetGroup = false)
        {
            var entityName = typeof(TTable).Name;
            var fintoName = typeof(TFinto).Name;
            var result = "(EXISTS (SELECT 1 FROM " + GetSqlName("Service" + fintoName) + " WHERE " +
                         GetSqlName(entityName + ".Id", "=") +
                         GetSqlName("Service" + fintoName + ".ServiceVersionedId") +
                         (isTargetGroup ? "AND  " + GetSqlName("Override", "= 'false'") : string.Empty ) +
                         " AND " + GetSqlInClause(collectionName, fintoName + "Id") + ")" +
                         " OR " + GetSqlName("StatutoryServiceGeneralDescriptionId") +
                         " IN (SELECT " + GetSqlName("UnificRootId") +
                         " FROM " + GetSqlName("StatutoryServiceGeneralDescriptionVersioned") +
                         " Inner Join " + GetSqlName("StatutoryService" + fintoName, "ON") +
                         GetSqlName("StatutoryServiceGeneralDescriptionVersioned.Id", "=") +
                         GetSqlName("StatutoryService" + fintoName + ".StatutoryServiceGeneralDescriptionVersionedId") +
                         " WHERE " + GetSqlInClause(collectionName, fintoName + "Id") +
                         " AND " + GetSqlName("StatutoryServiceGeneralDescriptionVersioned.PublishingStatusId",
                             " = '" + PublishingStatusCache.Get(PublishingStatus.Published) + "'") +
                         "))";
            return result;
        }

        private string GetSqlServiceOrganization<TTable>(string collectionName)
        {
            var tableName = typeof(TTable).Name;
            var result = "(" + GetSqlInClause<TTable>(collectionName, "OrganizationId") +
                         " OR EXISTS(SELECT 1 FROM " + GetSqlName("OrganizationService" ) +
                         " WHERE " +  GetSqlName($"{tableName}.Id","=") +  GetSqlName("OrganizationService.ServiceVersionedId") +
                         " AND " + GetSqlInClause(collectionName, "OrganizationId") + ")" +
                         " OR EXISTS(SELECT 1 FROM " + GetSqlName("ServiceProducer" ) +
                         GetSqlInnerJoin("ServiceProducerOrganization", "ServiceProducerOrganization.ServiceProducerId", "ServiceProducer.Id") +
                         " WHERE " +  GetSqlName($"{tableName}.Id","=") +  GetSqlName("ServiceProducer.ServiceVersionedId") +
                         " AND " + GetSqlInClause(collectionName, "OrganizationId") + ")" +
                         ")";
            return result;
        }

        private void FillServicesInformation(IReadOnlyList<IVmEntityListItem> search, IUnitOfWork unitOfWork)
        {
            var vmEntityListItems = search.Where(x => x.EntityType == SearchEntityTypeEnum.Service).ToList();
            var entitiesIds = vmEntityListItems.Select(x => x.Id).ToList();
            var unificRootIds = vmEntityListItems.Select(x => x.UnificRootId).ToList();
            if (entitiesIds.Any())
            {
                var organizationServicesRep = unitOfWork.CreateRepository<IOrganizationServiceRepository>();
                var organizationServices = organizationServicesRep.All().Where(i => entitiesIds.Contains(i.ServiceVersionedId)).ToList().GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.Select(j => j.OrganizationId).ToList());

                var serviceProducerOrgRep = unitOfWork.CreateRepository<IServiceProducerOrganizationRepository>();
                var spo = serviceProducerOrgRep.All().Where(i => entitiesIds.Contains(i.ServiceProducer.ServiceVersionedId)).Select(i => new { i.ServiceProducer.ServiceVersionedId, i.OrganizationId}).ToList().GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.Select(j => j.OrganizationId).ToList());

                var names = GetServicesNames(entitiesIds, unitOfWork);
                var languages = GetServicesLanguageAvailability(entitiesIds, unitOfWork);
                var idsWithOrders = GetListOfExistingTranslationOrders<ServiceTranslationOrder>(unificRootIds, unitOfWork);
                var astiConnections = GetServicesHaveAsti(unificRootIds, unitOfWork);
                vmEntityListItems.ForEach(x=>
                {
                    x.Name = names.TryGetOrDefault(x.Id, new Dictionary<string, string>());
                    x.LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            languages.TryGetOrDefault(x.Id, new List<ServiceLanguageAvailability>()));
                    if (x.OrganizationId != null)
                        x.Organizations = new List<Guid> {x.OrganizationId.Value}
                            .Union(organizationServices.TryGetOrDefault(x.Id, new List<Guid>())).ToList();
                    x.Producers = spo.TryGetOrDefault(x.Id, new List<Guid>());
                    x.IsCopyTagVisible = IsCopyTagVisible(x);
                    x.HasTranslationOrder = idsWithOrders.Contains(x.UnificRootId);
                    x.HasAstiConnection = astiConnections.TryGetOrDefault(x.UnificRootId);
                });
            }
        }

        private Dictionary<Guid, bool> GetServicesHaveAsti(List<Guid> unificRootIds, IUnitOfWork unitOfWork)
        {
            var connectionRepo = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            return connectionRepo.All()
                .Where(x => unificRootIds.Contains(x.ServiceId))
                .ToList()
                .GroupBy(x => x.ServiceId)
                .ToDictionary(x => x.Key, x => x.Any(y => y.IsASTIConnection));
        }

        private bool IsCopyTagVisible(IVmEntityListItem x)
        {
            return x.OriginalId.HasValue && x.VersionMajor == 0 && x.VersionMinor <= 1;
        }

        private HashSet<Guid> GetListOfExistingTranslationOrders<TEntity>(List<Guid> ids, IUnitOfWork unitOfWork)
            where TEntity : IEntityTranslationOrder
        {
            var arrivedState = typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.Arrived.ToString());
            var rep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            return rep.All()
                .WithUnificRootId(ids)
                .Where(x =>
                    x.TranslationOrder.TranslationOrderStates.Any(tos =>
                        tos.Last && tos.TranslationStateId != arrivedState))
                .SelectUnificRootId()
                .ToHashSet();
        }

        private Dictionary<Guid, List<ServiceLanguageAvailability>> GetServicesLanguageAvailability(List<Guid> entitiesIds, IUnitOfWork unitOfWork)
        {
            var langAvailabilitiesRep = unitOfWork.CreateRepository<IServiceLanguageAvailabilityRepository>();
            return langAvailabilitiesRep.All().Where(x => entitiesIds.Contains(x.ServiceVersionedId)).ToList()
                .GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());

        }
        private Dictionary<Guid, Dictionary<string, string>> GetServicesNames(List<Guid> serviceIds, IUnitOfWork unitOfWork)
        {
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var serviceNameRep = unitOfWork.CreateRepository<IServiceNameRepository>();
            
            return serviceNameRep.All()
                .Where(x => serviceIds.Contains(x.ServiceVersionedId) && (x.TypeId == nameTypeId))
                .OrderBy(i => i.Localization.OrderNumber)
                .Select(i => new {i.ServiceVersionedId, i.Name, i.LocalizationId})
                .ToList()
                .GroupBy(i => i.ServiceVersionedId)
                .ToDictionary(i => i.Key,
                    i => i.ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));
        }

        #endregion

        #region Organization SQL
        private string  GetSearchOrganizationSql<TTable>(IVmEntitySearchByStatus vmEntitySearch)
        {
            #region SearchByFilterParam

            var tableName = typeof(TTable).Name;
            var resultSelect = GetSqlOrganizationSelect<TTable>(vmEntitySearch);

            if (vmEntitySearch.EntityIds != null && vmEntitySearch.EntityIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<TTable>("EntityIds", "UnificRootId"));

            }
            else if (vmEntitySearch.EntityVersionIds != null && vmEntitySearch.EntityVersionIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<TTable>("EntityVersionIds", "Id"));

            }
            else
            {
                //My content
                if (vmEntitySearch.MyContent)
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlMyContent());
                }
                if (vmEntitySearch.OrganizationIds != null && vmEntitySearch.OrganizationIds.Any())
                {
                    var allChildren = GetOrganizationsFlatten(commonService.GetOrganizationNamesTree(vmEntitySearch.OrganizationIds));
                    vmEntitySearch.SubOrganizationIds = allChildren.Select(i => i.Id).ToList();
                    resultSelect = AddSqlWhere(resultSelect, GetSqlOrganizations<TTable>());
                }

                if (ApplySearchById(vmEntitySearch))
                {
                    var rootId = vmEntitySearch.Id;
                    if (rootId.IsAssigned())
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlName($"{tableName}.UnificRootId", " = '"+rootId+"'"));
                    }
                }

                if (ApplySearchByName(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchName<TTable>());
                }
                if (ApplySearchByEmail(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchEmail<TTable>());
                }
                if (ApplySearchByAddress(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlSearchAddress<TTable, OrganizationAddress>("OrganizationVersioned",
                            vmEntitySearch.AddressStreetId.IsAssigned(),
                            !string.IsNullOrEmpty(vmEntitySearch.StreetNumber),
                            vmEntitySearch.PostalCodeId.IsAssigned()));
                }

                if (ApplySearchByPhone(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchPhone<TTable,OrganizationPhone>("OrganizationVersioned", vmEntitySearch.IsLocalNumber, vmEntitySearch.PhoneDialCode, vmEntitySearch.PhoneNumber));
                }

                if (vmEntitySearch.LanguageIds != null && vmEntitySearch.LanguageIds.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlInClause<OrganizationLanguageAvailability>("LanguageIds", "LanguageId"));
                }
            }

            resultSelect = AddPublishingStatusFilters<TTable>(vmEntitySearch, resultSelect);

            resultSelect = resultSelect + GetSqlEntityOrder<TTable>(GetSqlName($"{tableName}.ParentId"));

            #endregion SearchByFilterParam

            return resultSelect;
        }
        private string GetSqlOrganizationSelect<TTable>(IVmEntitySearchByStatus vmEntitySearch)
        {
            var tableName = typeof(TTable).Name;
            return SqlStatement.SelectDistinct +
                   GetSqlEntityColumns<TTable, OrganizationName>() +
                   GetSqlSelectNameAs($"{tableName}.ParentId",GetSqlName("OrganizationId")) +
                   GetSqlSelectValueAs("NULL::uuid", "OriginalId") +
                   GetSqlSelectValueAs("NULL", "ExpireOn") +
                   GetSqlSelectStringValueAs(SqlConstants.Organization, SqlConstants.EntityType) +
                   GetSqlSelectStringValueAs(SqlConstants.Organization, SqlConstants.SubEntityType) +
                   GetSqlSelectEntityNameType("OrganizationName", "OrganizationDisplayNameType.DisplayNameTypeId", SqlConstants.NameType, false) +
                   SqlStatement.From + GetSqlName(tableName) +
                   GetSqlInnerJoin("OrganizationName", $"{tableName}.Id", "OrganizationName.OrganizationVersionedId") +
                   GetSqlLanguageOrderNumberInnerJoin("Language", "OrganizationName.LocalizationId", "Language.Id", vmEntitySearch.LanguageId) +
                   GetSqlInnerJoin("OrganizationLanguageAvailability", $"{tableName}.Id", "OrganizationLanguageAvailability.OrganizationVersionedId") +
                   GetSqlInnerJoin("Versioning", $"{tableName}.VersioningId", "Versioning.Id") +
                   GetSqlLeftJoin("OrganizationDisplayNameType",
                       "OrganizationName.OrganizationVersionedId", "OrganizationDisplayNameType.OrganizationVersionedId",
                       "OrganizationName.LocalizationId", "OrganizationDisplayNameType.LocalizationId");
        }
        private string GetSqlOrganizations<TTable>()
        {
            var result = "( " + GetSqlInClause<TTable>("OrganizationIds", "ParentId") +
                         " OR " + GetSqlInClause<TTable>("SubOrganizationIds", "UnificRootId") +
                         " OR " + GetSqlInClause<TTable>("OrganizationIds", "UnificRootId") + ") ";
            return result;
        }

        private void FillOrganizationsInformation(IReadOnlyList<IVmEntityListItem> search, IUnitOfWork unitOfWork)
        {
            var vmEntityListItems = search.Where(x => x.EntityType == SearchEntityTypeEnum.Organization).ToList();
            var entitiesIds = vmEntityListItems.Select(x => x.Id).ToList();
            var unificRootIds = vmEntityListItems.Select(x => x.UnificRootId).ToList();
            if (entitiesIds.Any())
            {
                var names = GetOrganizationsNames(entitiesIds, unitOfWork);
                var languages = GetOrganizationsLanguageAvailability(entitiesIds, unitOfWork);
                var astiConnections = GetOrganizationsHaveAsti(unificRootIds, unitOfWork);
                vmEntityListItems.ForEach(x=>
                {
                    x.Name = names.TryGetOrDefault(x.Id, new Dictionary<string, string>());
                    x.LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            languages.TryGetOrDefault(x.Id, new List<OrganizationLanguageAvailability>()));
                    x.HasAstiConnection = astiConnections.TryGetOrDefault(x.UnificRootId);
                });
            }
        }

        private Dictionary<Guid, bool> GetOrganizationsHaveAsti(List<Guid> organizationRootIds, IUnitOfWork unitOfWork)
        {
            var serviceRepo = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var channelRepo = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var connectionRepo = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();

            var serviceOrgIds = serviceRepo.All()
                .Where(x => organizationRootIds.Contains(x.OrganizationId))
                .Select(x => new {x.OrganizationId, x.UnificRootId})
                .Distinct()
                .ToList();
            var channelOrgIds = channelRepo.All()
                .Where(x => organizationRootIds.Contains(x.OrganizationId))
                .Select(x => new {x.OrganizationId, x.UnificRootId})
                .Distinct()
                .ToList();

            var serviceIds = serviceOrgIds.Select(x => x.UnificRootId).ToHashSet();
            var channelIds = channelOrgIds.Select(x => x.UnificRootId).ToHashSet();
            var astiConnections = connectionRepo.All()
                .Where(x => x.IsASTIConnection &&
                            (serviceIds.Contains(x.ServiceId) || channelIds.Contains(x.ServiceChannelId)))
                .Select(x => new {x.ServiceId, x.ServiceChannelId})
                .Distinct()
                .ToList();

            var result = new Dictionary<Guid, bool>();
            foreach (var rootId in organizationRootIds.Distinct())
            {
                var serviceSubset = serviceOrgIds.Where(x => x.OrganizationId == rootId).Select(x => x.UnificRootId)
                    .ToHashSet();
                var channelSubset = channelOrgIds.Where(x => x.OrganizationId == rootId).Select(x => x.UnificRootId)
                    .ToHashSet();
                var containsAsti = astiConnections.Any(x =>
                    serviceSubset.Contains(x.ServiceId) || channelSubset.Contains(x.ServiceChannelId));
                result.Add(rootId, containsAsti);
            }

            return result;
        }

        private Dictionary<Guid, List<OrganizationLanguageAvailability>> GetOrganizationsLanguageAvailability(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork)
        {
            var langAvailabilitiesRep = unitOfWork.CreateRepository<IOrganizationLanguageAvailabilityRepository>();
            return langAvailabilitiesRep.All().Where(x => entitiesIds.Contains(x.OrganizationVersionedId)).ToList()
                .GroupBy(i => i.OrganizationVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());

        }
        private Dictionary<Guid, Dictionary<string, string>> GetOrganizationsNames(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork)
        {
            var nameRep = unitOfWork.CreateRepository<IOrganizationNameRepository>();

            return nameRep.All()
                .Where
                (
                    i => entitiesIds.Contains(i.OrganizationVersionedId) &&
                    (
                        i.OrganizationVersioned.OrganizationDisplayNameTypes.Any(m => m.DisplayNameTypeId == i.TypeId && m.LocalizationId == i.LocalizationId)
                        ||
                        i.OrganizationVersioned.OrganizationDisplayNameTypes.All(k => k.LocalizationId != i.LocalizationId)
                    )
                )
                .ToList()
                .GroupBy(i => i.OrganizationVersionedId)
                .ToDictionary
                (
                    i => i.Key,
                    i => i
                        .GroupBy(j => j.LocalizationId)
                        .OrderBy(j => languageOrderCache.Get(j.Key))
                        .ToDictionary(
                            x => languageCache.GetByValue(x.Key),
                            x => x.First().Name)
                );
        }

        #endregion

        #region Channel SQL
        private string GetSearchChannelSql<TTable>(IVmEntitySearchByStatus vmEntitySearch)
        {
            var tableName = typeof(TTable).Name;
            var channelTypes = Enum.GetNames(typeof(ServiceChannelTypeEnum));
            vmEntitySearch.ChannelTypeIds = new List<Guid>();
            if (vmEntitySearch.ContentTypes != null)
            {
                if (vmEntitySearch.ContentTypes.Contains(SearchEntityTypeEnum.Channel))
                {
                    channelTypes.ForEach(type => vmEntitySearch.ChannelTypeIds.Add(typesCache.Get<ServiceChannelType>(type)));
                }
                else
                {
                    vmEntitySearch.ContentTypes.ForEach(type =>
                    {
                        if (channelTypes.Contains(type.ToString()))
                        {
                            vmEntitySearch.ChannelTypeIds.Add(typesCache.Get<ServiceChannelType>(type.ToString()));
                        }
                    });
                }
            }

            #region SearchByFilterParam

            var resultSelect = GetSqlChannelSelect<TTable>(vmEntitySearch);

            if (vmEntitySearch.EntityIds != null && vmEntitySearch.EntityIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<TTable>("EntityIds", "UnificRootId"));

            } else if (vmEntitySearch.EntityVersionIds != null && vmEntitySearch.EntityVersionIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<TTable>("EntityVersionIds", "Id"));

            }
            else
            {
                //My content
                if (vmEntitySearch.MyContent)
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlMyContent());
                }
                if (vmEntitySearch.ChannelTypeIds != null && vmEntitySearch.ChannelTypeIds.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlInClause<ServiceChannelType>("ChannelTypeIds", "Id"));
                }

                if (vmEntitySearch.OrganizationIds != null && vmEntitySearch.OrganizationIds.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlInClause<TTable>("OrganizationIds", "OrganizationId"));
                }

                if (ApplySearchById(vmEntitySearch))
                {
                    var rootId = vmEntitySearch.Id;
                    if (rootId.HasValue)
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlName($"{tableName}.UnificRootId", " = '"+rootId+"'"));
                    }
                }

                if (ApplySearchByName(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchName<TTable>());
                }
                if (ApplySearchByEmail(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchEmail<TTable>());
                }
                if (ApplySearchByAddress(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlSearchAddress<TTable, ServiceChannelAddress>("ServiceChannelVersioned",
                            vmEntitySearch.AddressStreetId.IsAssigned(),
                            vmEntitySearch.AddressStreetNumberId.IsAssigned() ||
                            !string.IsNullOrEmpty(vmEntitySearch.StreetNumber),
                            vmEntitySearch.PostalCodeId.IsAssigned()));
                }

                if (ApplySearchByPhone(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchPhone<TTable, ServiceChannelPhone>("ServiceChannelVersioned", vmEntitySearch.IsLocalNumber, vmEntitySearch.PhoneDialCode, vmEntitySearch.PhoneNumber));
                }

                if (vmEntitySearch.LanguageIds != null && vmEntitySearch.LanguageIds.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<ServiceChannelLanguageAvailability>("LanguageIds", "LanguageId"));
                }

                // Area Type
                if (vmEntitySearch.AreaInformationTypes != null && vmEntitySearch.AreaInformationTypes.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<TTable>("AreaInformationTypes", "AreaInformationTypeId"));
                }
            }

            resultSelect = AddPublishingStatusFilters<TTable>(vmEntitySearch, resultSelect);

            var additionalGroupBy =
                GetSqlName($"{tableName}.Expiration", ", ") +
                GetSqlName($"{tableName}.OrganizationId", ", ") +
                GetSqlName($"{tableName}.OriginalId", ", ") +
                GetSqlName(SqlConstants.SubEntityType);

            resultSelect = resultSelect + GetSqlEntityOrder<TTable>(additionalGroupBy);

            #endregion SearchByFilterParam

            return resultSelect;
        }
        private string GetSqlChannelSelect<TTable>(IVmEntitySearchByStatus vmEntitySearch)
        {
            var tableName = typeof(TTable).Name;
            return SqlStatement.SelectDistinct +
                   GetSqlEntityColumns<TTable, ServiceChannelName>() +
                   GetSqlSelectName($"{tableName}.OrganizationId") +
                   GetSqlSelectName($"{tableName}.OriginalId") +
                   GetSqlSelectEpochTimeNameAs($"{tableName}.Expiration", "ExpireOn") +
                   GetSqlSelectStringValueAs(SqlConstants.Channel, SqlConstants.EntityType) +
                   GetSqlSelectNameAs("ServiceChannelType.Code", GetSqlName(SqlConstants.SubEntityType)) +
                   GetSqlSelectEntityNameType("ServiceChannelName", "ServiceChannelDisplayNameType.DisplayNameTypeId", SqlConstants.NameType, false) +
                   SqlStatement.From + GetSqlName(tableName) +
                   GetSqlInnerJoin("ServiceChannelName", $"{tableName}.Id", "ServiceChannelName.ServiceChannelVersionedId") +
                   GetSqlInnerJoin("ServiceChannelLanguageAvailability", $"{tableName}.Id", "ServiceChannelLanguageAvailability.ServiceChannelVersionedId") +
                   GetSqlLanguageOrderNumberInnerJoin("Language", "ServiceChannelName.LocalizationId", "Language.Id", vmEntitySearch.LanguageId) +
                   GetSqlInnerJoin("ServiceChannelType", $"{tableName}.TypeId", "ServiceChannelType.Id") +
                   GetSqlInnerJoin("Versioning", $"{tableName}.VersioningId", "Versioning.Id") +
                   GetSqlLeftJoin("ServiceChannelDisplayNameType",
                        "ServiceChannelName.ServiceChannelVersionedId", "ServiceChannelDisplayNameType.ServiceChannelVersionedId",
                        "ServiceChannelName.LocalizationId", "ServiceChannelDisplayNameType.LocalizationId");
        }

        private void FillChannelsInformation(IReadOnlyList<IVmEntityListItem> search, IUnitOfWork unitOfWork)
        {
            var vmEntityListItems = search.Where(x => x.EntityType == SearchEntityTypeEnum.Channel).ToList();
            var entitiesIds = vmEntityListItems.Select(x => x.Id).ToList();
            var unificRootIds = vmEntityListItems.Select(x => x.UnificRootId).ToList();
            if (entitiesIds.Any())
            {
                var names = GetChannelsNames(entitiesIds, unitOfWork);
                var languages = GetChannelsLanguageAvailability(entitiesIds, unitOfWork);
                var idsWithOrders = GetListOfExistingTranslationOrders<ServiceChannelTranslationOrder>(unificRootIds, unitOfWork);
                var astiConnections = GetChannelsHaveAsti(unificRootIds, unitOfWork);
                vmEntityListItems.ForEach(x=>
                {
                    x.Name = names.TryGetOrDefault(x.Id, new Dictionary<string, string>());
                    x.LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            languages.TryGetOrDefault(x.Id, new List<ServiceChannelLanguageAvailability>()));
                    x.IsCopyTagVisible = IsCopyTagVisible(x);
                    x.HasTranslationOrder = idsWithOrders.Contains(x.UnificRootId);
                    x.HasAstiConnection = astiConnections.TryGetOrDefault(x.UnificRootId);
                });
            }
        }

        private Dictionary<Guid, bool> GetChannelsHaveAsti(List<Guid> unificRootIds, IUnitOfWork unitOfWork)
        {
            var connectionRepo = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            return connectionRepo.All()
                .Where(x => unificRootIds.Contains(x.ServiceChannelId))
                .ToList()
                .GroupBy(x => x.ServiceChannelId)
                .ToDictionary(x => x.Key, x => x.Any(y => y.IsASTIConnection));
        }

        private Dictionary<Guid, List<ServiceChannelLanguageAvailability>> GetChannelsLanguageAvailability(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork)
        {
            var langAvailabilitiesRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
            return langAvailabilitiesRep.All().Where(x => entitiesIds.Contains(x.ServiceChannelVersionedId)).ToList()
                .GroupBy(i => i.ServiceChannelVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());

        }
        private Dictionary<Guid, Dictionary<string, string>> GetChannelsNames(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork)
        {
            var nameRep = unitOfWork.CreateRepository<IServiceChannelNameRepository>();

            return nameRep.All()
                .Where
                (
                    i => entitiesIds.Contains(i.ServiceChannelVersionedId) &&
                    (
                        i.ServiceChannelVersioned.DisplayNameTypes.Any(m => m.DisplayNameTypeId == i.TypeId && m.LocalizationId == i.LocalizationId) ||
                        i.ServiceChannelVersioned.DisplayNameTypes.All(k => k.LocalizationId != i.LocalizationId)
                    )
                )
                .ToList()
                .GroupBy(i => i.ServiceChannelVersionedId)
                .ToDictionary
                (
                    i => i.Key,
                    i => i
                        .GroupBy(j => j.LocalizationId)
                        .OrderBy(j => languageOrderCache.Get(j.Key))
                        .ToDictionary(x => languageCache.GetByValue(x.Key), x => x.First().Name)
                );

        }

        #endregion

        #region Service Collections SQL
        private string GetSearchServiceCollectionSql<TTable>(IVmEntitySearchByStatus vmEntitySearch)
        {
            #region SearchByFilterParam

            var tableName = typeof(TTable).Name;
            var resultSelect = GetSqlServiceCollectionSelect<TTable>(vmEntitySearch);

            if (vmEntitySearch.EntityIds != null && vmEntitySearch.EntityIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<TTable>("EntityIds", "UnificRootId"));

            } else if (vmEntitySearch.EntityVersionIds != null && vmEntitySearch.EntityVersionIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<TTable>("EntityVersionIds", "Id"));

            }
            else
            {
                //My content
                if (vmEntitySearch.MyContent)
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlMyContent());
                }

                if (vmEntitySearch.OrganizationIds != null && vmEntitySearch.OrganizationIds.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlInClause<TTable>("OrganizationIds", "OrganizationId"));
                }

                if (ApplySearchById(vmEntitySearch))
                {
                    var rootId = vmEntitySearch.Id;
                    if (rootId.HasValue)
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlName($"{tableName}.UnificRootId", " = '" + rootId + "'"));
                    }
                }
                if (ApplySearchByName(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchName<TTable>());
                }
                if (ApplySearchByEmail(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchEmail<TTable>());
                }

                if (vmEntitySearch.LanguageIds != null && vmEntitySearch.LanguageIds.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<ServiceCollectionLanguageAvailability>("LanguageIds", "LanguageId"));
                }
            }

            resultSelect = AddPublishingStatusFilters<TTable>(vmEntitySearch, resultSelect);

            var additionalGroupBy =
                GetSqlName($"{tableName}.OrganizationId", ", ") +
                GetSqlName($"{tableName}.OriginalId");

            resultSelect = resultSelect + GetSqlEntityOrder<TTable>(additionalGroupBy);

            #endregion SearchByFilterParam

            return resultSelect;
        }
        private string GetSqlServiceCollectionSelect<TTable>(IVmEntitySearchByStatus vmEntitySearch)
        {
            var tableName = typeof(TTable).Name;
            return SqlStatement.SelectDistinct +
                   GetSqlEntityColumns<TTable, ServiceCollectionName>() +
                   GetSqlSelectName($"{tableName}.OrganizationId") +
                   GetSqlSelectName($"{tableName}.OriginalId") +
                   GetSqlSelectValueAs("NULL", "ExpireOn") +
                   GetSqlSelectStringValueAs(SqlConstants.ServiceCollection, SqlConstants.EntityType) +
                   GetSqlSelectStringValueAs(SqlConstants.ServiceCollection, SqlConstants.SubEntityType) +
                   GetSqlSelectValueAs("0", SqlConstants.NameType, false) +
                   SqlStatement.From + GetSqlName(tableName) +
                   GetSqlInnerJoin("ServiceCollectionName", $"{tableName}.Id",
                       "ServiceCollectionName.ServiceCollectionVersionedId") +
                   GetSqlInnerJoin("ServiceCollectionLanguageAvailability", $"{tableName}.Id", "ServiceCollectionLanguageAvailability.ServiceCollectionVersionedId") +
                   GetSqlLanguageOrderNumberInnerJoin("Language", "ServiceCollectionName.LocalizationId", "Language.Id", vmEntitySearch.LanguageId) +
                   GetSqlInnerJoin("Versioning", $"{tableName}.VersioningId", "Versioning.Id");
        }

        private void FillServiceCollectionsInformation(IReadOnlyList<IVmEntityListItem> search, IUnitOfWork unitOfWork)
        {
            var vmEntityListItems = search.Where(x => x.EntityType == SearchEntityTypeEnum.ServiceCollection).ToList();
            var entitiesIds = vmEntityListItems.Select(x => x.Id).ToList();
            if (entitiesIds.Any())
            {
                var names = GetServiceCollectionsNames(entitiesIds, unitOfWork);
                var languages = GetServiceCollectionsLanguageAvailability(entitiesIds, unitOfWork);
                vmEntityListItems.ForEach(x=>
                {
                    x.Name = names.TryGetOrDefault(x.Id, new Dictionary<string, string>());
                    x.LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            languages.TryGetOrDefault(x.Id, new List<ServiceCollectionLanguageAvailability>()));
                    x.IsCopyTagVisible = IsCopyTagVisible(x);
                });
            }
        }
        private Dictionary<Guid, List<ServiceCollectionLanguageAvailability>> GetServiceCollectionsLanguageAvailability(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork)
        {
            var langAvailabilitiesRep = unitOfWork.CreateRepository<IServiceCollectionLanguageAvailabilityRepository>();
            return langAvailabilitiesRep.All().Where(x => entitiesIds.Contains(x.ServiceCollectionVersionedId)).ToList()
                .GroupBy(i => i.ServiceCollectionVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());

        }
        private Dictionary<Guid, Dictionary<string, string>> GetServiceCollectionsNames(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork)
        {
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var nameRep = unitOfWork.CreateRepository<IServiceCollectionNameRepository>();

            return nameRep.All()
                .Where(x => entitiesIds.Contains(x.ServiceCollectionVersionedId) && (x.TypeId == nameTypeId))
                .OrderBy(i => i.Localization.OrderNumber)
                .Select(i => new {i.ServiceCollectionVersionedId, i.Name, i.LocalizationId}).ToList()
                .GroupBy(i => i.ServiceCollectionVersionedId)
                .ToDictionary(i => i.Key,
                    i => i.ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));
        }

        #endregion

        #region General description SQL
        private string GetSearchGeneralDescriptionSql<TTable>(IVmEntitySearchByStatus vmEntitySearch)
        {
            #region SearchByFilterParam

            var resultSelect = GetSqlGeneralDescriptionSelect<TTable>(vmEntitySearch);

            if (vmEntitySearch.EntityIds != null && vmEntitySearch.EntityIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<TTable>("EntityIds", "UnificRootId"));

            } else if (vmEntitySearch.EntityVersionIds != null && vmEntitySearch.EntityVersionIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<TTable>("EntityVersionIds", "Id"));

            }
            else
            {
                //My content
                if (vmEntitySearch.MyContent)
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlMyContent());
                }

                //Service classes
                if (vmEntitySearch.ServiceClasses != null && vmEntitySearch.ServiceClasses.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlGeneralDescriptionFintoClause<ServiceClass, TTable>("ServiceClasses"));
                }
                //Life events
                if (vmEntitySearch.LifeEvents != null && vmEntitySearch.LifeEvents.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlGeneralDescriptionFintoClause<LifeEvent, TTable>("LifeEvents"));
                }

                //Industrial classes
                if (vmEntitySearch.IndustrialClasses != null && vmEntitySearch.IndustrialClasses.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlGeneralDescriptionFintoClause<IndustrialClass, TTable>("IndustrialClasses"));
                }

                //Ontology terms
                if (vmEntitySearch.OntologyTerms != null && vmEntitySearch.OntologyTerms.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlGeneralDescriptionFintoClause<OntologyTerm, TTable>("OntologyTerms"));
                }

                //Target groups
                if (vmEntitySearch.TargetGroups != null && vmEntitySearch.TargetGroups.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlGeneralDescriptionFintoClause<TargetGroup, TTable>("TargetGroups"));
                }

                if (ApplySearchById(vmEntitySearch))
                {
                    var rootId = vmEntitySearch.Id;
                    if (rootId.HasValue)
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlName($"{typeof(TTable).Name}.UnificRootId", " = '" + rootId + "'"));
                    }
                }

                if (ApplySearchByName(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchName<TTable>());
                }

                if (ApplySearchByEmail(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchEmail<TTable>());
                }

                if (vmEntitySearch.LanguageIds != null && vmEntitySearch.LanguageIds.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<GeneralDescriptionLanguageAvailability>("LanguageIds", "LanguageId"));
                }

                if (vmEntitySearch.ServiceTypes != null && vmEntitySearch.ServiceTypes.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<TTable>("ServiceTypes", "TypeId"));
                }
                if (vmEntitySearch.GeneralDescriptionTypes != null && vmEntitySearch.GeneralDescriptionTypes.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<TTable>("GeneralDescriptionTypes", "GeneralDescriptionTypeId"));
                }
            }

            resultSelect = AddPublishingStatusFilters<TTable>(vmEntitySearch, resultSelect);

            resultSelect = resultSelect + GetSqlEntityOrder<TTable>(GetSqlName(SqlConstants.SubEntityType));

            #endregion SearchByFilterParam

            return resultSelect;
        }
        private string GetSqlGeneralDescriptionSelect<TTable>(IVmEntitySearchByStatus vmEntitySearch)
        {
            var tableName = typeof(TTable).Name;
            return SqlStatement.SelectDistinct +
                   GetSqlEntityColumns<TTable, StatutoryServiceName>() +
                   GetSqlSelectValueAs("NULL::uuid", "OrganizationId") +
                   GetSqlSelectValueAs("NULL::uuid", "OriginalId") +
                   GetSqlSelectValueAs("NULL", "ExpireOn") +
                   GetSqlSelectStringValueAs(SqlConstants.GeneralDescription, SqlConstants.EntityType) +
                   GetSqlSelectNameAs("ServiceType.Code", GetSqlName(SqlConstants.SubEntityType)) +
                   GetSqlSelectValueAs("0", SqlConstants.NameType, false) +
                   SqlStatement.From + GetSqlName(tableName) +
                   GetSqlInnerJoin("StatutoryServiceName", $"{tableName}.Id", "StatutoryServiceName.StatutoryServiceGeneralDescriptionVersionedId") +
                   GetSqlInnerJoin("GeneralDescriptionLanguageAvailability", $"{tableName}.Id", "GeneralDescriptionLanguageAvailability.StatutoryServiceGeneralDescriptionVersionedId") +
                   GetSqlLanguageOrderNumberInnerJoin("Language", "StatutoryServiceName.LocalizationId", "Language.Id", vmEntitySearch.LanguageId) +
                   GetSqlInnerJoin("ServiceType", $"{tableName}.TypeId", "ServiceType.Id") +
                   GetSqlInnerJoin("Versioning", $"{tableName}.VersioningId", "Versioning.Id");
        }
        private string GetSqlGeneralDescriptionFintoClause<TFinto, TTable>(string collectionName)
        {
            var entityName = typeof(TTable).Name;
            var fintoName = typeof(TFinto).Name;
            var result = " EXISTS (SELECT 1 FROM " + GetSqlName("StatutoryService" + fintoName) + " WHERE " +
                         GetSqlName(entityName + ".Id", "=") +
                         GetSqlName("StatutoryService" + fintoName + ".StatutoryServiceGeneralDescriptionVersionedId") +
                         " AND " + GetSqlInClause(collectionName, fintoName + "Id") + ")";
            return result;
        }

        private void FillGeneralDescriptionsInformation(IReadOnlyList<IVmEntityListItem> search, IUnitOfWork unitOfWork)
        {
            var vmEntityListItems = search.Where(x => x.EntityType == SearchEntityTypeEnum.GeneralDescription).ToList();
            var entitiesIds = vmEntityListItems.Select(x => x.Id).ToList();
            var unificRootIds = vmEntityListItems.Select(x => x.UnificRootId).ToList();
            if (entitiesIds.Any())
            {
                var names = GetGeneralDescriptionsNames(entitiesIds, unitOfWork);
                var languages = GetGeneralDescriptionsLanguageAvailability(entitiesIds, unitOfWork);
                var idsWithOrders = GetListOfExistingTranslationOrders<GeneralDescriptionTranslationOrder>(unificRootIds, unitOfWork);
//                var astiConnections = GetGeneralDescriptionsHaveAsti(unificRootIds, unitOfWork);
                vmEntityListItems.ForEach(x=>
                {
                    x.Name = names.TryGetOrDefault(x.Id, new Dictionary<string, string>());
                    x.LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            languages.TryGetOrDefault(x.Id, new List<GeneralDescriptionLanguageAvailability>()));
                    x.HasTranslationOrder = idsWithOrders.Contains(x.UnificRootId);
//                    x.HasAstiConnection = astiConnections.TryGetOrDefault(x.UnificRootId);
                });
            }
        }

//        private Dictionary<Guid, bool> GetGeneralDescriptionsHaveAsti(List<Guid> unificRootIds, IUnitOfWork unitOfWork)
//        {
//            var serviceRepo = unitOfWork.CreateRepository<IServiceVersionedRepository>();
//            return serviceRepo.All()
//                .Include(sv => sv.StatutoryServiceGeneralDescription)
//                .Include(sv => sv.UnificRoot).ThenInclude(s => s.ServiceServiceChannels)
//                .Where(sv =>
//                    sv.StatutoryServiceGeneralDescriptionId.HasValue &&
//                    unificRootIds.Contains(sv.StatutoryServiceGeneralDescriptionId.Value))
//                .GroupBy(sv => sv.StatutoryServiceGeneralDescriptionId.Value)
//                .ToDictionary(g => g.Key,
//                    g => g.Any(sv => sv.UnificRoot.ServiceServiceChannels.Any(con => con.IsASTIConnection)));
//        }

        private Dictionary<Guid, List<GeneralDescriptionLanguageAvailability>> GetGeneralDescriptionsLanguageAvailability(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork)
        {
            var langAvailabilitiesRep = unitOfWork.CreateRepository<IGeneralDescriptionLanguageAvailabilityRepository>();
            return langAvailabilitiesRep.All().Where(x => entitiesIds.Contains(x.StatutoryServiceGeneralDescriptionVersionedId)).ToList()
                .GroupBy(i => i.StatutoryServiceGeneralDescriptionVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());

        }
        private Dictionary<Guid, Dictionary<string, string>> GetGeneralDescriptionsNames(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork)
        {
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var nameRep = unitOfWork.CreateRepository<IStatutoryServiceNameRepository>();

            return nameRep.All()
                .Where(x => entitiesIds.Contains(x.StatutoryServiceGeneralDescriptionVersionedId) && (x.TypeId == nameTypeId))
                .OrderBy(i => i.Localization.OrderNumber)
                .Select(i => new {i.StatutoryServiceGeneralDescriptionVersionedId, i.Name, i.LocalizationId}).ToList()
                .GroupBy(i => i.StatutoryServiceGeneralDescriptionVersionedId)
                .ToDictionary(i => i.Key,
                    i => i.ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));
        }

        #endregion

        #region SQL creation

        private string GetSqlEntityOrder<TEntity>(string additionGroupBy = "", string additionalOrderBy = "")
        {
            var entityName = typeof(TEntity).Name;
            var addGroupBy = !string.IsNullOrEmpty(additionGroupBy) ? ", "+ additionGroupBy : "";
            var addOrderBy = !string.IsNullOrEmpty(additionalOrderBy) ? additionalOrderBy + ", ": "";
            return
                SqlStatement.GroupBy +
                GetSqlSelectName(SqlConstants.TempName) +
                GetSqlSelectName(SqlConstants.NameType) +
                GetSqlSelectName($"{entityName}.Id") +
                GetSqlSelectName($"{entityName}.LastOperationType") +
                GetSqlSelectName($"{entityName}.Modified") +
                GetSqlSelectName($"{entityName}.ModifiedBy") +
                GetSqlSelectName($"{entityName}.PublishingStatusId") +
                GetSqlSelectName($"{entityName}.UnificRootId") +
                GetSqlSelectName("Versioning.VersionMajor") +
                GetSqlSelectName("Versioning.VersionMinor") +
                GetSqlName("Versioning.Id") +
                addGroupBy +
                SqlStatement.OrderBy +
                addOrderBy +
                GetSqlName(entityName+".Id", "," + SqlConstants.Min + "," + SqlConstants.NameType);
        }
        /// <summary>
        /// create string from name of DB entity for postgress DB
        /// </summary>
        /// <param name="name">name of table or column</param>
        /// <param name="postFix">some addition string</param>
        /// <returns>formated SQL for postrgess DB</returns>
        /// <example>
        /// name = "tableName.columnName"
        /// postFix = "AS name"
        /// result -> "tableName"."columnName" AS name
        /// </example>
        private string GetSqlName(string name, string postFix = "")
        {
            return "\"" + string.Join("\".\"", name.Split('.')) + "\" " + postFix;
        }

        /// <summary>
        /// create string from name of DB entity for postgress DB ended by comma
        /// </summary>
        /// <param name="name">name of table or column</param>
        /// <returns>formated SQL for postrgess DB</returns>
        /// <example>
        /// name = "tableName.columnName"
        /// result -> "tableName"."columnName",
        /// </example>
        private string GetSqlSelectName(string name)
        {
            return GetSqlName(name, ", ");
        }

        /// <summary>
        /// create string from name of DB entity for postgress DB ended by AS name
        /// </summary>
        /// <param name="name">name of table or column</param>
        /// <param name="asName"> AS name</param>
        /// <returns>formated SQL for postrgess DB</returns>
        /// <example>
        /// name = "tableName.columnName"
        /// asName = "name"
        /// result -> "tableName"."columnName" AS name,
        /// </example>
        private string GetSqlSelectNameAs(string name, string asName, bool useComma = true)
        {
            return GetSqlName(name, " AS "+asName + (useComma ? ", " : " "));
        }

        /// <summary>
        /// create string from value ended by AS name
        /// </summary>
        /// <param name="value">default value in column</param>
        /// <param name="asName"> AS name</param>
        /// <returns>formated SQL for postrgess DB</returns>
        /// <example>
        /// value = "defValue"
        /// asName = "name"
        /// result -> 'defValue' AS name,
        /// </example>
        private string GetSqlSelectStringValueAs(string value, string asName)
        {
            return "'" + value + "' AS " + GetSqlSelectName(asName);
        }

        /// <summary>
        /// create string from value ended by AS name
        /// </summary>
        /// <param name="value">default value in column</param>
        /// <param name="asName"> AS name</param>
        /// <returns>formated SQL for postrgess DB</returns>
        /// <example>
        /// value = "defValue"
        /// asName = "name"
        /// result -> defValue AS name,
        /// </example>
        private string GetSqlSelectValueAs(string value, string asName, bool withComma = true)
        {
            return " " +value + " AS " + (withComma ? GetSqlSelectName(asName) : GetSqlName(asName));
        }

        /// <summary>
        /// create string from name of DB entity for postgress DB ended by AS name
        /// </summary>
        /// <param name="name">name of column</param>
        /// <param name="asName"> AS name</param>
        /// <returns>formated SQL for postrgess DB</returns>
        /// <example>
        /// name = "columnName"
        /// asName = "name"
        /// result -> TRIM("columnName") AS name,
        /// </example>
        private string GetSqlSelectTrimNameAs(string name, string asName = SqlConstants.TempName)
        {
            return " TRIM(" + GetSqlName(name, ") AS " + asName + ", ");
        }

        /// <summary>
        /// create string from name of DB entity for postgress DB ended by AS name
        /// </summary>
        /// <param name="name">name of column</param>
        /// <param name="asName"> AS name</param>
        /// <returns>formated SQL for postrgess DB</returns>
        /// <example>
        /// name = "columnName"
        /// asName = "name"
        /// result -> MIN("columnName") AS name,
        /// </example>
        private string GetSqlSelectMinNameAs(string name, string asName = SqlConstants.Min)
        {
            return " MIN(" + GetSqlName(name) + ") AS "+asName+", ";
        }

        /// <summary>
        /// create string from name of DB entity for postgress DB ended by AS name
        /// </summary>
        /// <param name="name">name of column</param>
        /// <param name="asName"> AS name</param>
        /// <returns>formated SQL for postrgess DB</returns>
        /// <example>
        /// name = "columnName"
        /// asName = "epochTime"
        /// result -> extract(epoch from "columnName") * 1000 AS epochTime,
        /// </example>
        private string GetSqlSelectEpochTimeNameAs(string name, string asName)
        {
            return
                " extract(epoch from " + GetSqlName(name, ") * 1000 AS ") + GetSqlSelectName(asName);
        }

        /// <summary>
        /// create string from name of DB entity name table for postgress DB ended by AS name
        /// </summary>
        /// <param name="tableName">name of entity name table</param>
        /// <param name="asName"> AS name</param>
        /// <returns>formated SQL for postrgess DB</returns>
        /// <example>
        /// name = "tableName"
        /// asName = "nameType"
        /// result ->(case when "tableName"."TypeId" ='NameTypeEnum.Name' then 0 else 1 END) AS nameType,
        /// </example>
        private string GetSqlSelectEntityNameType(string tableName, string asName, bool withComma = true)
        {
            return
                "(case when " + GetSqlName(tableName+".TypeId", "='") +
                typesCache.Get<NameType>(NameTypeEnum.Name.ToString()) + "' then 0 else 1 END) AS "+asName+ (withComma ?", ":string.Empty);

        }

        /// <summary>
        /// create string from name of DB entity name table for postgress DB ended by AS name
        /// </summary>
        /// <param name="tableName">name of entity name table</param>
        /// <param name="typeIdColumn">name of typeId column</param>
        /// <param name="asName"> AS name</param>
        /// <returns>formated SQL for postrgess DB</returns>
        /// <example>
        /// name = "tableName"
        /// typeIdColumn = "typeIdColumn"
        /// asName = "nameType"
        /// result ->(case when "tableName"."TypeId" = "typeIdColumn" then 0 else 1 END) AS nameType,
        /// </example>
        private string GetSqlSelectEntityNameType(string tableName, string typeIdColumn ,string asName, bool withComma = true)
        {
            return
                "(case when " +
                    GetSqlName(tableName+".TypeId", "=") + GetSqlName(typeIdColumn) +
                " then 0 else 1 END) AS "+asName+ (withComma ?", ":string.Empty);

        }



        /// <summary>
        /// Create IN clause for potggress DB
        /// </summary>
        /// <param name="collectionName">Name of parameter collection</param>
        /// <param name="column">Column name</param>
        /// <param name="equal"> IN or NOT IN </param>
        /// <typeparam name="TEntity">Table entity</typeparam>
        /// <returns>In clause</returns>
        /// <example>
        /// TEntity = Service
        /// collectionName = "filteredIds"
        /// column = "Id"
        /// result -> "Service"."Id" = ANY(@collectionName)
        /// </example>
        private string GetSqlInClause<TEntity>(string collectionName, string column, bool equal = true)
        {
            var entityName = typeof(TEntity).Name;
            return GetSqlInClause(collectionName, entityName + "." + column, equal);
        }
        /// <summary>
        /// Create IN clause for postgres DB
        /// </summary>
        /// <param name="collectionName">Name of parameter collection</param>
        /// <param name="column">Column name</param>
        /// <param name="equal"> IN or NOT IN </param>
        /// <returns>In clause</returns>
        /// <example>
        /// collectionName = "filteredIds"
        /// column = "Id"
        /// result -> "Id" = ANY(@collectionName)
        /// </example>
        private string GetSqlInClause(string collectionName, string column, bool equal = true)
        {
            return GetSqlName(column) + (equal?"=":"<>") +
                   " ANY(@"+ collectionName+") ";
        }
        /// <summary>
        /// Create inner join SQL string
        /// </summary>
        /// <param name="toTable">Table name for join</param>
        /// <param name="fromId">From id</param>
        /// <param name="toId">To Id</param>
        /// <returns>Inner join string</returns>
        private string GetSqlInnerJoin(string toTable, string fromId, string toId)
        {
            return " Inner Join " + GetSqlName(toTable) +
                   " ON " + GetSqlName(fromId) +
                   " = " + GetSqlName(toId) + " ";
        }

        private string GetSqlLeftJoin(string toTable, string fromId, string toId)
        {
            return " Left Join " + GetSqlName(toTable) +
                   " ON " + GetSqlName(fromId) +
                   " = " + GetSqlName(toId) + " ";
        }
        
        private string GetSqlLeftJoin(string toTable, string fromId1, string toId1, string fromId2, string toId2)
        {
            return " Left Join " + GetSqlName(toTable) +
                   " ON " + GetSqlName(fromId1) +
                   " = " + GetSqlName(toId1) + " AND "
                   + GetSqlName(fromId2) +
                   " = " + GetSqlName(toId2);
        }
        
        private string GetSqlInnerJoin(string toTable, string fromId1, string toId1, string fromId2, string toId2)
        {
            return " Inner Join " + GetSqlName(toTable) +
                   " ON " + GetSqlName(fromId1) +
                   " = " + GetSqlName(toId1) + " AND "
                   + GetSqlName(fromId2) +
                   " = " + GetSqlName(toId2);
        }

        private string GetSqlLanguageOrderNumberInnerJoin(string toTable, string fromId, string toId, Guid? languageId)
        {
            if (!languageId.HasValue) return GetSqlInnerJoin(toTable, fromId, toId);
            return " Inner Join " +
            "( select \"Id\", CASE WHEN \"Id\" = '"+languageId.Value+"' THEN 0 ELSE \"OrderNumber\" END \"OrderNumber\" from \"Language\" ) "
               + GetSqlName(toTable) +
               " ON " + GetSqlName(fromId) +
               " = " + GetSqlName(toId) + " ";
        }


        private string GetSqlSearchName<TEntity>()
        {
            var entityName = typeof(TEntity).Name;
            var result = "( lower(" + GetSqlName("Name", ") Like CONCAT('%',@Name,'%')") +
                         " OR lower(" +
                         GetSqlName(entityName + ".CreatedBy", ") Like CONCAT('%',@Name,'%')") +
                         " OR lower(" +
                         GetSqlName(entityName + ".ModifiedBy", ") Like CONCAT('%',@Name,'%')") + ")";
            return result;
        }

        private string GetSqlSearchFullName<TEntity, TNameTable>()
        {
            var entityName = typeof(TEntity).Name;
            var tableName = typeof(TNameTable).Name;
            var result = "( lower(" + GetSqlName(tableName +".Name", ") Like CONCAT('%',@Name,'%')") +
                         " OR lower(" +
                         GetSqlName(entityName + ".CreatedBy", ") Like CONCAT('%',@Name,'%')") +
                         " OR lower(" +
                         GetSqlName(entityName + ".ModifiedBy", ") Like CONCAT('%',@Name,'%')") + ")";
            return result;
        }
        
        private string GetSqlSearchEmail<TEntity>()
        {
            var entityName = typeof(TEntity).Name;
            var result = "(lower(" +
                         GetSqlName(entityName + ".CreatedBy", ") Like CONCAT('%',@Email,'%')") +
                         " OR lower(" +
                         GetSqlName(entityName + ".ModifiedBy", ") Like CONCAT('%',@Email,'%')") + ")";
            return result;
        }

        private string GetSqlSearchAddress<TEntity, TAddrRel>(string baseTableName, bool byStreet, bool byStreetNumber, bool byPostalCode)
        {
            if (!byStreet && !byStreetNumber && !byPostalCode) return String.Empty;
            var entityName = typeof(TEntity).Name;
            var addrRelationName = typeof(TAddrRel).Name;
            var result = "(EXISTS (SELECT 1 FROM " + GetSqlName(addrRelationName) + " AS m" +
                         " INNER JOIN \"Address\" AS \"m.Address\" ON m.\"AddressId\" = \"m.Address\".\"Id\"" +
                         " WHERE EXISTS (SELECT 1 FROM \"ClsAddressPoint\" AS n" +
                         " WHERE ";
            if (byStreet)
            {
                result += "(n.\"AddressStreetId\" = @AddressStreetId)";
            }
            if (byStreetNumber)
            {
                if (byStreet) result += " AND ";
                result += "((n.\"StreetNumber\" = @StreetNumber) OR (n.\"StreetNumber\" ILIKE CONCAT(@StreetNumber, ' %')))";
            }
            if (byPostalCode)
            {
                if (byStreet || byStreetNumber) result += " AND ";
                result += "(n.\"PostalCodeId\" = @PostalCodeId)";
            }

            result += " AND (\"m.Address\".\"Id\" = n.\"AddressId\")) AND ("+GetSqlName(entityName)+".\"Id\" = m."+GetSqlName(baseTableName+"Id")+")))";

            return result;
        }

        private Guid? GetDefaultDialCodeId()
        {
            return contextManager.ExecuteReader(unitOfWork => commonService.GetDefaultDialCodeId(unitOfWork) );
        }

        private string GetSqlSearchPhone<TEntity, TPhoneRel>(string baseTableName, bool? isLocalNumber, Guid? prefixNumber, string phoneNumber)
        {
            var defaultDialCodeId = GetDefaultDialCodeId();
            isLocalNumber = (isLocalNumber.HasValue && isLocalNumber.Value) ||
                            (prefixNumber.HasValue && prefixNumber.Value == defaultDialCodeId);
            var entityName = typeof(TEntity).Name;
            var phoneRelationName = typeof(TPhoneRel).Name;
            var result = "EXISTS (SELECT 1 FROM " + GetSqlName(phoneRelationName) + " AS m INNER JOIN \"Phone\" AS \"m.Phone\" ON m.\"PhoneId\" = \"m.Phone\".\"Id\"" +
                (isLocalNumber == true
                ? " WHERE ((\"m.Phone\".\"PrefixNumberId\" IS NULL OR \"m.Phone\".\"PrefixNumberId\" = '" + defaultDialCodeId + "')"
                : " WHERE ((\"m.Phone\".\"PrefixNumberId\" = @PhoneDialCode) ") +
                (!string.IsNullOrEmpty(phoneNumber)
                ?  "AND (STRPOS(\"m.Phone\".\"Number\", @PhoneNumber) > 0)) "
                :  " )" )
                + "AND ("+GetSqlName(entityName)+".\"Id\" = m."+GetSqlName(baseTableName+"Id")+"))";
            return result;
        }

        private string GetSqlMyContent()
        {
            return " EXISTS (SELECT 1 FROM " + GetSqlName("Versioning", " AS ") + GetSqlName("Ver") + " WHERE " +
                   GetSqlName("Ver.UnificRootId", " = ") + GetSqlName("Versioning.UnificRootId") +
                   " AND (lower(" + GetSqlName("Ver.ModifiedBy") + ") = @UserName OR lower(" +
                   GetSqlName("Ver.CreatedBy") + ") = @UserName  ))";

        }

        private string AddSqlWhere(string input, string clause)
        {
            return input.Contains("WHERE") ? input + " AND " + clause : input + " WHERE " + clause;
        }

        private string GetSqlEntityColumns<TEntity, TEntityName>()
        {
            var entity = typeof(TEntity).Name;
            var entityName = typeof(TEntityName).Name;
            return GetSqlSelectName(entity + ".Id") +
                   GetSqlSelectTrimNameAs(entityName + ".Name") +
                   GetSqlSelectMinNameAs("Language.OrderNumber") +
                   GetSqlSelectEpochTimeNameAs(entity + ".Modified", "Modified") +
                   GetSqlSelectName(entity + ".ModifiedBy") +
                   GetSqlSelectName(entity + ".PublishingStatusId") +
                   GetSqlSelectName(entity + ".UnificRootId") +
                   GetSqlSelectName("Versioning.VersionMajor") +
                   GetSqlSelectName("Versioning.VersionMinor") +
                   GetSqlName("Versioning.Id") + " AS \"VersioningId\", " +
                   GetSqlSelectName(entity + ".LastOperationType");
        }

        private string GetSqlSorting(List<VmSortParam> sortParams, VmSortParam defaultSortParam = null, bool useVersionSorting = true)
        {
            StringBuilder orderBuilder = new StringBuilder();

            var versionSortingParams = new List<VmSortParam>
            {
                new VmSortParam { Column = "UnificRootId", SortDirection = SortDirectionEnum.Asc, Order = 997},
                new VmSortParam { Column = "VersionMajor", SortDirection = SortDirectionEnum.Desc, Order = 998},
                new VmSortParam { Column = "VersionMinor", SortDirection = SortDirectionEnum.Desc, Order = 999},
            };

            if (!sortParams.Any() && defaultSortParam == null)
            {
                return String.Empty;
            }

            sortParams = sortParams.Any() ? sortParams : new List<VmSortParam> { defaultSortParam };

            var entityTypeSort = sortParams.FirstOrDefault(x => x.Column == "entityType");
            var subEntityTypeSort = sortParams.FirstOrDefault(x => x.Column == "subEntityType");
            if (entityTypeSort != null && subEntityTypeSort == null)
            {
                sortParams.Add(new VmSortParam
                {
                    Column = "subEntityType",
                    SortDirection = entityTypeSort.SortDirection
                });
            }

            var sort = useVersionSorting ? sortParams.Union(versionSortingParams) : sortParams;
            sort.ForEach(param =>
            {
                orderBuilder.Append(GetSqlName(param.Column[0].ToString().ToUpper() + param.Column.Substring(1), " "))
                    .Append(param.SortDirection)
                    .Append(",");
            });

            var orderString = orderBuilder
                .Replace("Name", SqlConstants.TempName)
                .ToString();
            return orderString.Remove(orderString.LastIndexOf(','), 1);
        }

        private string GetSqlAllEntityOrderSelect(string sqlQuery, List<VmSortParam> sortParams, VmSortParam defaultSortParam = null)
        {
            return SqlStatement.Select +
               GetSqlSelectName("Id") +
               GetSqlSelectName(SqlConstants.TempName) +
               GetSqlSelectName("OrganizationId") +
               GetSqlSelectName("OriginalId") +
               GetSqlSelectName(SqlConstants.EntityType) +
               GetSqlSelectName("Modified") +
               GetSqlSelectName("ModifiedBy") +
               GetSqlSelectName(SqlConstants.SubEntityType) +
               GetSqlSelectName("PublishingStatusId") +
               GetSqlSelectName("UnificRootId") +
               GetSqlSelectName("VersionMajor") +
               GetSqlSelectName("VersionMinor") +
               GetSqlSelectName("VersioningId") +
               GetSqlSelectName("LastOperationType") +
               GetSqlName("ExpireOn") +
            SqlStatement.From + "(" + sqlQuery + ") orderedresult " +
            SqlStatement.OrderBy + GetSqlSorting(sortParams, defaultSortParam);
        }
        private string GetSqlAllEntityPageSelect(string sqlQuery, int pageSize = CoreConstants.MaximumNumberOfAllItems, int page = 1)
        {
            return SqlStatement.Select +
                   GetSqlSelectName("Id") +
                   GetSqlSelectName("OrganizationId") +
                   GetSqlSelectName("OriginalId") +
                   GetSqlSelectName(SqlConstants.EntityType) +
                   GetSqlSelectName("Modified") +
                   GetSqlSelectName("ModifiedBy") +
                   GetSqlSelectName(SqlConstants.SubEntityType) +
                   GetSqlSelectName("PublishingStatusId") +
                   GetSqlSelectName("UnificRootId") +
                   GetSqlSelectName("VersionMajor") +
                   GetSqlSelectName("VersionMinor") +
                   GetSqlSelectName("VersioningId") +
                   GetSqlSelectName("LastOperationType") +
                   GetSqlName("ExpireOn") +
                   SqlStatement.From + "(" + sqlQuery + ") pageresult " +
                   SqlStatement.Limit + pageSize +
                   SqlStatement.Offset + (pageSize * page);
        }
        private string GetSqlCountEntitySelect(string sqlQuery)
        {
            return "Select COUNT(*) From (" + sqlQuery + ") result";
        }
        private string AddSqlToUnion(string sqlUnion, string sqlQuery)
        {
            return sqlUnion.Length > 0 ? sqlUnion + " UNION(" + sqlQuery + ")" : "(" + sqlQuery + ")";
        }

        #endregion SQL creation

        #region Search type

        private bool IsSearchTypeUsed(IVmEntitySearch search, SearchEntityTypeEnum contentType)
        {
            var searchSetup = new Dictionary<SearchEntityTypeEnum, List<SearchTypeEnum>>
            {
                {SearchEntityTypeEnum.Channel, new List<SearchTypeEnum> { SearchTypeEnum.Other }} ,
                {SearchEntityTypeEnum.EChannel, new List<SearchTypeEnum> { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email }} ,
                {SearchEntityTypeEnum.WebPage, new List<SearchTypeEnum> { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email }} ,
                {SearchEntityTypeEnum.PrintableForm, new List<SearchTypeEnum> { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email }} ,
                {SearchEntityTypeEnum.Phone, new List<SearchTypeEnum> { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email, SearchTypeEnum.Phone }} ,
                {SearchEntityTypeEnum.ServiceLocation, new List<SearchTypeEnum> { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email, SearchTypeEnum.Address }} ,

                {SearchEntityTypeEnum.Service, new List<SearchTypeEnum> { SearchTypeEnum.Other }} ,
                {SearchEntityTypeEnum.ServiceService, new List<SearchTypeEnum> { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email }} ,
                {SearchEntityTypeEnum.ServicePermit, new List<SearchTypeEnum> { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email }} ,
                {SearchEntityTypeEnum.ServiceProfessional, new List<SearchTypeEnum> { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email }} ,

                {SearchEntityTypeEnum.Organization, new List<SearchTypeEnum> { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email, SearchTypeEnum.Address, SearchTypeEnum.Phone, SearchTypeEnum.Other }} ,
                {SearchEntityTypeEnum.ServiceCollection, new List<SearchTypeEnum> { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email }} ,
                {SearchEntityTypeEnum.GeneralDescription, new List<SearchTypeEnum> { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email }} ,
            };
            return searchSetup[contentType].Contains(search.SearchType ?? SearchTypeEnum.Name);
        }

        #endregion Search type

        #region Notification Connections SQL

        private string GetSqlConnectionsOrderSelect(string sqlQuery, List<VmSortParam> sortParams, VmSortParam defaultSortParam = null)
        {
            var sb = new StringBuilder();
            return sb
                .Append(SqlStatement.Select)
                .Append(GetSqlSelectName("Id"))
                .Append(GetSqlSelectName("entityid"))
                .Append(GetSqlSelectName(SqlConstants.TempName))
                .Append(GetSqlSelectName("connectedid"))
                .Append(GetSqlSelectName("Created"))
                .Append(GetSqlSelectName("CreatedBy"))
                .Append(GetSqlSelectName("OperationType"))
                .Append(GetSqlName(SqlConstants.EntityType))
                .Append(SqlStatement.From + "(" + sqlQuery + ") orderedresult ")
                .Append(SqlStatement.OrderBy + GetSqlSorting(sortParams, defaultSortParam, false))
                .ToString();
        }
        private string GetSqlConnectionsPageSelect(string sqlQuery, int pageSize = CoreConstants.MaximumNumberOfAllItems, int page = 1)
        {
            var sb = new StringBuilder();
            return sb
               .Append(SqlStatement.Select)
               .Append(GetSqlSelectName("Id"))
               .Append(GetSqlSelectName("entityid"))
               .Append(GetSqlSelectName("connectedid"))
               .Append(GetSqlSelectName("Created"))
               .Append(GetSqlSelectName("CreatedBy"))
               .Append(GetSqlSelectName("OperationType"))
               .Append(GetSqlName(SqlConstants.EntityType))
               .Append(SqlStatement.From + "(" + sqlQuery + ") pageresult ")
               .Append(SqlStatement.Limit + pageSize)
               .Append(SqlStatement.Offset + (pageSize * page))
               .ToString();
        }

        private string GetSqlNotificationChannelSelect()
        {
            return SqlStatement.SelectDistinct +
                   GetSqlSelectName("TrackingServiceServiceChannel.Id") +
                   GetSqlSelectNameAs("ChannelId", "entityid") +
                   GetSqlSelectStringValueAs(SqlConstants.Channel, SqlConstants.EntityType) +
                   GetSqlSelectNameAs("ServiceId", "connectedid") +
                   GetSqlSelectTrimNameAs("ServiceChannelName.Name") +
                   GetSqlSelectEntityNameType("ServiceChannelName",SqlConstants.NameType) +
                   GetSqlSelectMinNameAs("Language.OrderNumber") +
                   GetSqlSelectEpochTimeNameAs("TrackingServiceServiceChannel.Created", "Created") +
                   GetSqlSelectName("TrackingServiceServiceChannel.CreatedBy") +
                   GetSqlName("TrackingServiceServiceChannel.OperationType") +
                   SqlStatement.From + GetSqlName("TrackingServiceServiceChannel") +
                   GetSqlInnerJoin("ServiceChannelVersioned", "TrackingServiceServiceChannel.ChannelId",
                       "ServiceChannelVersioned.UnificRootId") +
                   GetSqlInnerJoin("ServiceChannelName", "ServiceChannelVersioned.Id",
                       "ServiceChannelName.ServiceChannelVersionedId") +
                   GetSqlInnerJoin("Language", "ServiceChannelName.LocalizationId", "Language.Id") +
                   GetSqlInnerJoin("ServiceVersioned", "TrackingServiceServiceChannel.ServiceId",
                       "ServiceVersioned.UnificRootId");
        }

        private string GetSqlNotificationServiceSelect()
        {
            return SqlStatement.SelectDistinct +
                   GetSqlSelectName("TrackingServiceServiceChannel.Id") +
                   GetSqlSelectNameAs("ServiceId", "entityid") +
                   GetSqlSelectStringValueAs(SqlConstants.Service, SqlConstants.EntityType) +
                   GetSqlSelectNameAs("ChannelId", "connectedid") +
                   GetSqlSelectTrimNameAs("ServiceName.Name") +
                   GetSqlSelectEntityNameType("ServiceName",SqlConstants.NameType) +
                   GetSqlSelectMinNameAs("Language.OrderNumber") +
                   GetSqlSelectEpochTimeNameAs("TrackingServiceServiceChannel.Created", "Created") +
                   GetSqlSelectName("TrackingServiceServiceChannel.CreatedBy") +
                   GetSqlName("TrackingServiceServiceChannel.OperationType") +
                   SqlStatement.From + GetSqlName("TrackingServiceServiceChannel") +
                   GetSqlInnerJoin("ServiceVersioned", "TrackingServiceServiceChannel.ServiceId",
                       "ServiceVersioned.UnificRootId") +
                   GetSqlInnerJoin("ServiceName", "ServiceVersioned.Id",
                       "ServiceName.ServiceVersionedId") +
                   GetSqlInnerJoin("Language", "ServiceName.LocalizationId", "Language.Id") +
                   GetSqlInnerJoin("ServiceChannelVersioned", "TrackingServiceServiceChannel.ChannelId",
                       "ServiceChannelVersioned.UnificRootId");
        }

        private string AddSqlNotificationOrder()
        {
            var sb = new StringBuilder();
            return
                sb
                .Append(SqlStatement.GroupBy)
                .Append(GetSqlSelectName("TrackingServiceServiceChannel.Id"))
                .Append(GetSqlSelectName(SqlConstants.TempName))
                .Append(GetSqlSelectName(SqlConstants.NameType))
                .Append(GetSqlSelectName("OrderNumber"))
                .Append(GetSqlSelectName("entityid"))
                .Append(GetSqlName("connectedid"))
                .Append(SqlStatement.OrderBy)
                .Append(GetSqlSelectName("TrackingServiceServiceChannel.Id"))
                .Append(GetSqlSelectName("OrderNumber"))
                .Append(GetSqlName(SqlConstants.NameType))
                .ToString();
        }

        private string GetSearchNotificationServiceSql()
        {
            var resultSelect = GetSqlNotificationServiceSelect();
            resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<ServiceVersioned>("OrganizationIds", "OrganizationId"));
            resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<ServiceChannelVersioned>("OrganizationIds", "OrganizationId", false));
            resultSelect = AddSqlWhere(resultSelect, GetSqlName("ServiceVersioned.PublishingStatusId", " <> '"+ typesCache.Get<PublishingStatusType>(PublishingStatus.Removed.ToString()) +"' "));
            resultSelect = AddSqlWhere(resultSelect, GetSqlName("ServiceChannelVersioned.PublishingStatusId", " <> '"+ typesCache.Get<PublishingStatusType>(PublishingStatus.Removed.ToString()) +"' "));
            resultSelect = AddSqlWhere(resultSelect, GetSqlName("TrackingServiceServiceChannel.Created", " > timezone('utc', now() - interval '1 month' )"));
            return resultSelect + AddSqlNotificationOrder();
        }

        private string GetSearchNotificationChannelSql()
        {
            var resultSelect = GetSqlNotificationChannelSelect();
            resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<ServiceChannelVersioned>("OrganizationIds", "OrganizationId"));
            resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<ServiceVersioned>("OrganizationIds", "OrganizationId", false));
            resultSelect = AddSqlWhere(resultSelect, GetSqlName("ServiceVersioned.PublishingStatusId", " <> '"+ typesCache.Get<PublishingStatusType>(PublishingStatus.Removed.ToString()) +"' "));
            resultSelect = AddSqlWhere(resultSelect, GetSqlName("ServiceChannelVersioned.PublishingStatusId", " <> '"+ typesCache.Get<PublishingStatusType>(PublishingStatus.Removed.ToString()) +"' "));
            resultSelect = AddSqlWhere(resultSelect, GetSqlName("TrackingServiceServiceChannel.Created", " > timezone('utc', now() - interval '1 month' )"));
            return resultSelect + AddSqlNotificationOrder();
        }

        public int SearchConnectionsCount(IVmEntitySearch vmEntitySearch)
        {
            var tempSql = string.Empty;
            tempSql = AddSqlToUnion(tempSql, GetSearchNotificationServiceSql());
            tempSql = AddSqlToUnion(tempSql, GetSearchNotificationChannelSql());

            var totalCountSql = GetSqlCountEntitySelect(tempSql);
            return ExecuteSearchCount(totalCountSql, vmEntitySearch);
        }

        public IVmSearchBase SearchConnections(IVmEntitySearch vmEntitySearch)
        {
            VmSearchResult<IVmNotificationConnectionListItem> returnData = new VmSearchResult<IVmNotificationConnectionListItem> {SearchResult = new List<IVmNotificationConnectionListItem>()};

            var tempSql = string.Empty;
            tempSql = AddSqlToUnion(tempSql, GetSearchNotificationServiceSql());
            tempSql = AddSqlToUnion(tempSql, GetSearchNotificationChannelSql());

            var totalCountSql = GetSqlCountEntitySelect(tempSql);

            var searchSql = GetSqlConnectionsPageSelect(
                GetSqlConnectionsOrderSelect(
                    tempSql,
                    vmEntitySearch.SortData,
                    new VmSortParam {Column = "Created", SortDirection = SortDirectionEnum.Desc}),
                page: vmEntitySearch.PageNumber);

            var (totalCount,result) = ExecuteSearch<VmNotificationConnectionListItem>(searchSql, totalCountSql, vmEntitySearch);

            var safePageNumber = vmEntitySearch.PageNumber.PositiveOrZero();
            var moreAvailable = totalCount.MoreResultsAvailable(safePageNumber);
            returnData.Count = totalCount;
            returnData.PageNumber = ++safePageNumber;
            returnData.MoreAvailable = moreAvailable;
            returnData.SearchResult = result.InclusiveToList();
            return returnData;
        }

        #endregion Notification Connections SQL
        
        #region Service Generaldescription SQL

        public IVmSearchBase SearchServiceGeneralDescriptionEntities(IVmEntitySearch vmEntitySearch, IUnitOfWork unitOfWork)
        {
            var returnData = SearchServiceGeneralDescriptions(vmEntitySearch);
            var entitiesIds = returnData.SearchResult.Select(x => x.Id).ToList();
            if (entitiesIds.Any())
            {
                var names = GetGeneralDescriptionsNames(entitiesIds, unitOfWork);
                var languages = GetGeneralDescriptionsLanguageAvailability(entitiesIds, unitOfWork);
                returnData.SearchResult.ForEach(x=>
                {
                    x.Name = names.TryGetOrDefault(x.Id, new Dictionary<string, string>());
                    x.LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            languages.TryGetOrDefault(x.Id, new List<GeneralDescriptionLanguageAvailability>()));
                });
            }
            return returnData;
        }

        private VmSearchResult<VmServiceGeneralDescriptionListItem> SearchServiceGeneralDescriptions(IVmEntitySearch vmEntitySearch)
        {
            VmSearchResult<VmServiceGeneralDescriptionListItem> returnData = new VmSearchResult<VmServiceGeneralDescriptionListItem> {SearchResult = new List<VmServiceGeneralDescriptionListItem>()};
            if (InvalidSearchCriteria(vmEntitySearch))
            {
                return returnData;
            }

            var tempSql = String.Empty; 
            tempSql = AddSqlToUnion(tempSql, GetSearchServiceGeneralDescriptionSql(vmEntitySearch));
            
            if (string.IsNullOrEmpty(tempSql))
            {
                return returnData;
            }
            
            var totalCountSql = GetSqlCountEntitySelect(tempSql);
            
            var searchSql = GetSqlServiceGeneralDescriptionPageSelect(
                GetSqlServiceGeneralDescriptionOrderSelect(
                    tempSql,
                    vmEntitySearch.SortData,
                    new VmSortParam {Column = "Modified", SortDirection = SortDirectionEnum.Desc}),
                page: vmEntitySearch.PageNumber);
            
            var (totalCount,result) = ExecuteSearch<VmServiceGeneralDescriptionListItem>(searchSql, totalCountSql, vmEntitySearch);

            var safePageNumber = vmEntitySearch.PageNumber.PositiveOrZero();
            var moreAvailable = totalCount.MoreResultsAvailable(safePageNumber);
            returnData.Count = totalCount;
            returnData.PageNumber = ++safePageNumber;
            returnData.MoreAvailable = moreAvailable;
            returnData.SearchResult = result.InclusiveToList();

            return returnData;
        }
        
        private string GetSearchServiceGeneralDescriptionSql(IVmEntitySearch vmEntitySearch)
        {
            var uiLanguageId = string.IsNullOrEmpty(vmEntitySearch.Language)
                ? languageCache.Get("fi")
                : languageCache.Get(vmEntitySearch.Language);

            #region SearchByFilterParam

            var resultSelect = GetSqlServiceGeneralDescriptionSelect();
            
            if (vmEntitySearch.Id.HasValue)
            {
                resultSelect = AddSqlWhere(resultSelect, GetSqlName("StatutoryServiceGeneralDescriptionVersioned.UnificRootId", " = '" + vmEntitySearch.Id.Value + "'"));
            }
            else if (ApplySearchByName(vmEntitySearch))
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlSearchFullName<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceName>());
            }

            if (vmEntitySearch.ServiceTypes != null && vmEntitySearch.ServiceTypes.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<StatutoryServiceGeneralDescriptionVersioned>("ServiceTypes", "TypeId"));
            }

            if (vmEntitySearch.GeneralDescriptionTypes != null && vmEntitySearch.GeneralDescriptionTypes.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<StatutoryServiceGeneralDescriptionVersioned>("GeneralDescriptionTypes",
                        "GeneralDescriptionTypeId"));
            }

            if (vmEntitySearch.SelectedPublishingStatuses.Count != 0)
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<StatutoryServiceGeneralDescriptionVersioned>("SelectedPublishingStatuses",
                        "PublishingStatusId"));
            }

            if (vmEntitySearch.RestrictedTypes.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<StatutoryServiceGeneralDescriptionVersioned>("RestrictedTypes",
                        "GeneralDescriptionTypeId", false));
            }

            resultSelect = AddSqlWhere(resultSelect,
                GetSqlName("ServiceTypeName.LocalizationId", " = '" + uiLanguageId + "'"));
            resultSelect = AddSqlWhere(resultSelect,
                GetSqlName("GeneralDescriptionTypeName.LocalizationId", " = '" + uiLanguageId + "'"));
            

            resultSelect = resultSelect + AddSqlServiceGeneralDescriptionOrder();

            #endregion SearchByFilterParam

            return resultSelect;
        }
        
        private string GetSqlServiceGeneralDescriptionSelect()
        {
            return SqlStatement.SelectDistinct +
                   GetSqlSelectName("StatutoryServiceGeneralDescriptionVersioned.Id") +
                   GetSqlSelectStringValueAs(SqlConstants.GeneralDescription, SqlConstants.EntityType) +
                   GetSqlSelectTrimNameAs("StatutoryServiceName.Name") +
                   GetSqlSelectMinNameAs("Language.OrderNumber") +
                   GetSqlSelectEpochTimeNameAs("StatutoryServiceGeneralDescriptionVersioned.Modified", "Modified") +
                   GetSqlSelectName("StatutoryServiceGeneralDescriptionVersioned.UnificRootId") +
                   GetSqlSelectNameAs("ServiceTypeName.Name", GetSqlName(SqlConstants.SubEntityType)) +
                   GetSqlSelectNameAs("StatutoryServiceGeneralDescriptionVersioned.TypeId", GetSqlName("ServiceTypeId")) +
                   GetSqlSelectName("StatutoryServiceGeneralDescriptionVersioned.GeneralDescriptionTypeId") +
                   GetSqlSelectNameAs("GeneralDescriptionTypeName.Name", GetSqlName(SqlConstants.GenDesType), false) +
                   SqlStatement.From + GetSqlName("StatutoryServiceGeneralDescriptionVersioned") +
                   GetSqlInnerJoin("StatutoryServiceName", "StatutoryServiceGeneralDescriptionVersioned.Id", "StatutoryServiceName.StatutoryServiceGeneralDescriptionVersionedId") +
                   GetSqlInnerJoin("GeneralDescriptionLanguageAvailability", "StatutoryServiceGeneralDescriptionVersioned.Id", "GeneralDescriptionLanguageAvailability.StatutoryServiceGeneralDescriptionVersionedId") +
                   GetSqlInnerJoin("Language", "StatutoryServiceName.LocalizationId", "Language.Id") +
                   GetSqlInnerJoin("ServiceTypeName", "StatutoryServiceGeneralDescriptionVersioned.TypeId", "ServiceTypeName.TypeId") +
                   GetSqlInnerJoin("GeneralDescriptionTypeName", "StatutoryServiceGeneralDescriptionVersioned.GeneralDescriptionTypeId", "GeneralDescriptionTypeName.TypeId");
        }
        
        private string GetSqlServiceGeneralDescriptionOrderSelect(string sqlQuery, List<VmSortParam> sortParams, VmSortParam defaultSortParam = null)
        {
            var sb = new StringBuilder();
            return sb
                .Append(SqlStatement.Select)
                .Append(GetSqlSelectName("Id"))
                .Append(GetSqlSelectName("UnificRootId"))
                .Append(GetSqlSelectName("ServiceTypeId"))
                .Append(GetSqlSelectName("GeneralDescriptionTypeId"))
                .Append(GetSqlSelectName(SqlConstants.TempName))
                .Append(GetSqlSelectName(SqlConstants.GenDesType))
                .Append(GetSqlSelectName(SqlConstants.SubEntityType))
                .Append(GetSqlName(SqlConstants.EntityType))
                .Append(SqlStatement.From + "(" + sqlQuery + ") orderedresult ")
                .Append(SqlStatement.OrderBy + GetSqlSorting(sortParams, defaultSortParam, false))
                .ToString();
        }
        private string GetSqlServiceGeneralDescriptionPageSelect(string sqlQuery, int pageSize = CoreConstants.MaximumNumberOfAllItems, int page = 1)
        {
            var sb = new StringBuilder();
            return sb
               .Append(SqlStatement.Select)
               .Append(GetSqlSelectName("Id"))
               .Append(GetSqlSelectName("UnificRootId"))
               .Append(GetSqlSelectName("ServiceTypeId"))
               .Append(GetSqlName("GeneralDescriptionTypeId"))
               .Append(SqlStatement.From + "(" + sqlQuery + ") pageresult ")
               .Append(SqlStatement.Limit + pageSize)
               .Append(SqlStatement.Offset + (pageSize * page))
               .ToString();
        }
        
        private string AddSqlServiceGeneralDescriptionOrder()
        {
            var sb = new StringBuilder();
            return
                sb
                    .Append(SqlStatement.GroupBy)
                    .Append(GetSqlSelectName("StatutoryServiceGeneralDescriptionVersioned.Id"))
                    .Append(GetSqlSelectName(SqlConstants.TempName))
                    .Append(GetSqlSelectName("Language.OrderNumber"))
                    .Append(GetSqlSelectName(SqlConstants.SubEntityType))
                    .Append(GetSqlSelectName("ServiceTypeId"))
                    .Append(GetSqlSelectName("GeneralDescriptionTypeId"))
                    .Append(GetSqlSelectName(SqlConstants.EntityType))
                    .Append(GetSqlName(SqlConstants.GenDesType))
                    .Append(SqlStatement.OrderBy)
                    .Append(GetSqlSelectName("StatutoryServiceGeneralDescriptionVersioned.Id"))
                    .Append(GetSqlName("Language.OrderNumber"))
                    .ToString();
        }
        #endregion

        #region constants

        private static class SqlStatement
        {
            public const string Select = " SELECT ";
            public const string From = " FROM ";
            public const string GroupBy = " GROUP BY ";
            public const string OrderBy = " ORDER BY ";
            public const string Limit = " LIMIT ";
            public const string Offset = " OFFSET ";
            public const string SelectDistinct = " SELECT DISTINCT ON (1) ";
        }

        public static class SqlConstants
        {
            public const string GeneralDescription = "GeneralDescription";
            public const string ServiceCollection = "ServiceCollection";
            public const string Organization = "Organization";
            public const string Service = "Service";
            public const string Channel = "Channel";
            public const string EntityType = "EntityType";
            public const string SubEntityType = "SubEntityType";
            public const string GenDesType = "GeneralDescriptionType";
            public const string LocalizedSubEntityType = "LocalizedSubEntityType";
            public const string NameType = "nametype";
            public const string TempName = "tempname";
            public const string Min = "min";
            public const string ServiceUnificID = "suid";
            public const string PreferredPublishinStatus = "pps";
            public const string Name = "name";
        }

        #endregion constants
    }
}
