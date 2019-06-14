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
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Logic;
using PTV.Framework.Extensions;


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
        private readonly IPahaTokenProcessor pahaTokenProcessor;
        private readonly ITargetGroupDataCache targetGroupDataCache;
        private readonly ApplicationConfiguration applicationConfiguration;

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
            IPahaTokenProcessor pahaTokenProcessor,
            ITargetGroupDataCache targetGroupDataCache,
            IVersioningManager versioningManager,
            ApplicationConfiguration applicationConfiguration
            )
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker, versioningManager)
        {
            this.contextManager = contextManager;
            this.typesCache = typesCache;
            this.languageCache = languageCache;
            this.commonService = commonService;            
            this.rawContext = rawContext;
            this.languageOrderCache = languageOrderCache;
            this.pahaTokenProcessor = pahaTokenProcessor;
            this.targetGroupDataCache = targetGroupDataCache;
            this.applicationConfiguration = applicationConfiguration;
        }
        
        public IVmSearchBase SearchEntities(IVmEntitySearch vmEntitySearch)
        {
            var returnData = Search(vmEntitySearch);

            contextManager.ExecuteReader(unitOfWork =>
            {
                FillSearchedData(returnData.SearchResult, unitOfWork);
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
            var returnData = Search(vmEntitySearch);
            FillSearchedData(returnData.SearchResult, unitOfWork);

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
        
        private VmSearchResult<IVmEntityListItem> Search(IVmEntitySearch vmEntitySearch)
        {
            vmEntitySearch.ContentTypeEnum = vmEntitySearch.ContentTypes.Any() ? vmEntitySearch.ContentTypes.Aggregate((i, j) => i | j) : SearchEntityTypeEnum.All;
            VmSearchResult<IVmEntityListItem> returnData = new VmSearchResult<IVmEntityListItem>() {SearchResult = new List<IVmEntityListItem>()};

            if (InvalidSearchCriteria(vmEntitySearch))
            {
                return returnData;
            }
            
            FilterSearchCriteria(vmEntitySearch);
            FilterSortingCriteria(vmEntitySearch);
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
                            tempSql = AddSqlToUnion(tempSql, GetSearchServiceSql(vmEntitySearch));
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
                            tempSql = AddSqlToUnion(tempSql, GetSearchChannelSql(vmEntitySearch));
                        }

                        break;
                    case SearchEntityTypeEnum.GeneralDescription:
                        if (IsSearchTypeUsed(vmEntitySearch, contentType))
                        {
                            tempSql = AddSqlToUnion(tempSql, GetSearchGeneralDescriptionSql(vmEntitySearch));
                        }
                        break;
                    case SearchEntityTypeEnum.Organization:
                        if (IsSearchTypeUsed(vmEntitySearch, contentType))
                        {
                            tempSql = AddSqlToUnion(tempSql, GetSearchOrganizationSql(vmEntitySearch));
                        }
                        break;
                    case SearchEntityTypeEnum.ServiceCollection:
                        if (IsSearchTypeUsed(vmEntitySearch, contentType))
                        {
                            tempSql = AddSqlToUnion(tempSql, GetSearchServiceCollectionSql(vmEntitySearch));
                        }
                        break;
                }
            }

            if (string.IsNullOrEmpty(tempSql))
            {
                return returnData;
            }

            var totalCountSql = GetSqlCountEntitySelect(tempSql);
            var searchSql = GetSqlAllEntityPageSelect(
                GetSqlAllEntityOrderSelect(
                    tempSql,
                    vmEntitySearch.SortData,
                    new VmSortParam() {Column = "Modified", SortDirection = SortDirectionEnum.Desc}),
                page: vmEntitySearch.PageNumber);

            var (totalCount,result) = ExecuteSearch(searchSql, totalCountSql, vmEntitySearch);
            
            FillExpiration(result, vmEntitySearch.Expiration);
            
            
            var safePageNumber = vmEntitySearch.PageNumber.PositiveOrZero();
            var moreAvailable = totalCount.MoreResultsAvailable(safePageNumber);
            returnData.Count = totalCount;
            returnData.PageNumber = ++safePageNumber;
            returnData.MoreAvailable = moreAvailable;
            returnData.SearchResult = result.InclusiveToList();

            return returnData;
        }

        private void FillSearchedData(IReadOnlyList<IVmEntityListItem> result, IUnitOfWork unitOfWork)
        {
            FillServicesInformation(result, unitOfWork);
            FillOrganizationsInformation(result, unitOfWork);
            FillChannelsInformation(result, unitOfWork);
            FillServiceCollectionsInformation(result, unitOfWork);
            FillGeneralDescriptionsInformation(result, unitOfWork);
        } 
        
        private void FillExpiration(IList<VmEntityListItem> search, DateTime expiration)
        {
            if (expiration != default(DateTime))
            {
                var lifeTime = expiration.ToEpochTime();
                var dateUtcNow = DateTime.UtcNow.ToEpochTime();
                search.ForEach(x =>
                {
                    x.ExpireOn = dateUtcNow + (x.Modified - lifeTime);
                });    
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

        private (int,IList<VmEntityListItem>) ExecuteSearch(string sqlSearch, string sqlCount, IVmEntitySearch vmEntitySearch)
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
            commonService.ExtendPublishingStatusesByEquivalents(vmEntitySearch.SelectedPublishingStatuses);
            vmEntitySearch.UserName = pahaTokenProcessor.UserName.ToLower();
            if (vmEntitySearch.LanguageIds == null || !vmEntitySearch.LanguageIds.Any())
            {
                vmEntitySearch.LanguageIds = vmEntitySearch.Languages.Select(code => languageCache.Get(code)).ToList();
            }
        }

        private void FilterSortingCriteria(IVmEntitySearch vmEntitySearch)
        {
            if (vmEntitySearch.SortData.Any(x => x.Column == "entityType"))
            {
                var subChannelTypes = Enum.GetNames(typeof(ServiceChannelTypeEnum)).ToList();
                var subServiceTypes = new List<string>
                {
                    SearchEntityTypeEnum.ServicePermit.ToString(),
                    SearchEntityTypeEnum.ServiceProfessional.ToString(),
                    SearchEntityTypeEnum.ServiceService.ToString()
                };
                if (vmEntitySearch.ContentTypes.Select(x => subChannelTypes.Contains(x.ToString())
                                ? SearchEntityTypeEnum.Channel.ToString()
                                : x.ToString()).Distinct().Count() == 1 || 
                    vmEntitySearch.ContentTypes.Select(x => subServiceTypes.Contains(x.ToString())
                                ? SearchEntityTypeEnum.Service.ToString()
                                : x.ToString()).Distinct().Count() == 1)
                {
                    vmEntitySearch.SortData = vmEntitySearch.SortData.Where(x=>x.Column != "entityType").ToList();
                }               
            }
        }

        #region Services SQL
        private string GetSearchServiceSql(IVmEntitySearch vmEntitySearch)
        {
            #region SearchByFilterParam

            var resultSelect = GetSqlServiceSelect();
            
            if (vmEntitySearch.EntityIds != null && vmEntitySearch.EntityIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<ServiceVersioned>("EntityIds", "UnificRootId")); 
                               
            } else if (vmEntitySearch.EntityVersionIds != null && vmEntitySearch.EntityVersionIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<ServiceVersioned>("EntityVersionIds", "Id")); 
                               
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
                        GetSqlServiceFintoClause<ServiceClass>("ServiceClasses"));
                }
                //Life events
                if (vmEntitySearch.LifeEvents != null && vmEntitySearch.LifeEvents.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlServiceFintoClause<LifeEvent>("LifeEvents"));
                }
                
                //Industrial classes
                if (vmEntitySearch.IndustrialClasses != null && vmEntitySearch.IndustrialClasses.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlServiceFintoClause<IndustrialClass>("IndustrialClasses"));
                }
                
                //Ontology terms
                if (vmEntitySearch.OntologyTerms != null && vmEntitySearch.OntologyTerms.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlServiceFintoClause<OntologyTerm>("OntologyTerms"));
                }
                
                //Target groups
                if (vmEntitySearch.TargetGroups != null && vmEntitySearch.TargetGroups.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlServiceFintoClause<TargetGroup>("TargetGroups", true));
                }

                if (vmEntitySearch.OrganizationIds != null && vmEntitySearch.OrganizationIds.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlServiceOrganization("OrganizationIds")); 
                }

                if (ApplySearchById(vmEntitySearch))
                {
                    var rootId = vmEntitySearch.Id;
                    if (rootId.IsAssigned())
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlName("ServiceVersioned.UnificRootId", " = '" + rootId + "'"));
                    }
                }

                if (ApplySearchByName(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchName<ServiceVersioned>());
                }
                if (ApplySearchByEmail(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchEmail<ServiceVersioned>());
                }

                if (vmEntitySearch.LanguageIds != null && vmEntitySearch.LanguageIds.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlInClause<ServiceName>("LanguageIds", "LocalizationId"));
                }

                if (vmEntitySearch.ServiceServiceType.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlServiceType("ServiceServiceType"));      
                }                
            }
            
            if (vmEntitySearch.SelectedPublishingStatuses.Count != 0)
            {
               resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<ServiceVersioned>("SelectedPublishingStatuses", "PublishingStatusId"));
            }

            resultSelect = resultSelect + GetSqlEntityOrder<ServiceVersioned>();
            
            #endregion SearchByFilterParam

            return resultSelect;
        }
        private string GetSqlServiceSelect()
        {
            return SqlStatement.SelectDistinct +
                   GetSqlEntityColumns<ServiceVersioned, ServiceName>() +
                   GetSqlSelectName("ServiceVersioned.OrganizationId") +
                   GetSqlSelectName("ServiceVersioned.OriginalId") +
                   GetSqlSelectStringValueAs(SqlConstants.Service, SqlConstants.EntityType) +
                   GetSqlSelectStringValueAs(SqlConstants.Service, SqlConstants.SubEntityType) +
                   GetSqlSelectEntityNameType("ServiceName", SqlConstants.NameType, false) +
                   SqlStatement.From + GetSqlName("ServiceVersioned", " ") +
                   GetSqlInnerJoin("ServiceName", "ServiceVersioned.Id", "ServiceName.ServiceVersionedId") +
                   GetSqlInnerJoin("Language", "ServiceName.LocalizationId", "Language.Id")+
                   GetSqlInnerJoin("Versioning", "ServiceVersioned.VersioningId", "Versioning.Id");
        }
        private string GetSqlServiceType(string collectionName)
        {
            var result = "( " + GetSqlInClause<ServiceVersioned>(collectionName, "TypeId") +
                         " OR " + GetSqlName("StatutoryServiceGeneralDescriptionId") + " IN (" +
                         " Select " + GetSqlName("UnificRootId") + " from " +
                         GetSqlName("StatutoryServiceGeneralDescriptionVersioned") +
                         " Where " + GetSqlName("PublishingStatusId",
                             " = '" + PublishingStatusCache.Get(PublishingStatus.Published) + "'") +
                         " AND " + GetSqlInClause(collectionName, "TypeId") + "))"; 
            return result;
        }
        private string GetSqlServiceFintoClause<TFinto>(string collectionName, bool isTargetGroup = false)
        {
            var entityName = "ServiceVersioned";
            var fintoName = typeof(TFinto).Name;
            var result = "(EXISTS (SELECT 1 FROM " + GetSqlName("Service" + fintoName) + " WHERE " +
                         GetSqlName(entityName + ".Id", "=") +
                         GetSqlName("Service" + fintoName + "." + entityName + "Id") +
                         (isTargetGroup ? "AND  " + GetSqlName("Override", "= 'false'") : string.Empty ) +
                         " AND " + GetSqlInClause(collectionName, fintoName + "Id") + ")" +
                         " OR " + GetSqlName("StatutoryServiceGeneralDescriptionId") +
                         " IN (SELECT " + GetSqlName("UnificRootId") +
                         " FROM " + GetSqlName("StatutoryServiceGeneralDescriptionVersioned") +
                         " Inner Join " + GetSqlName("StatutoryService" + fintoName, "ON") +
                         GetSqlName("StatutoryServiceGeneralDescriptionVersioned.Id", "=") +
                         GetSqlName("StatutoryService" + fintoName + ".StatutoryServiceGeneralDescriptionVersionedId") +
                         " WHERE " + GetSqlInClause(collectionName, fintoName + "Id") +
                         "))";
            return result;
        }

        private string GetSqlServiceOrganization(string collectionName)
        {
            var result = "(" + GetSqlInClause<ServiceVersioned>(collectionName, "OrganizationId") +
                         " OR EXISTS(SELECT 1 FROM " + GetSqlName("OrganizationService" ) +
                         " WHERE " +  GetSqlName("ServiceVersioned.Id","=") +  GetSqlName("OrganizationService.ServiceVersionedId") +
                         " AND " + GetSqlInClause(collectionName, "OrganizationId") + ")" +
                         " OR EXISTS(SELECT 1 FROM " + GetSqlName("ServiceProducer" ) +
                         GetSqlInnerJoin("ServiceProducerOrganization", "ServiceProducerOrganization.ServiceProducerId", "ServiceProducer.Id") +
                         " WHERE " +  GetSqlName("ServiceVersioned.Id","=") +  GetSqlName("ServiceProducer.ServiceVersionedId") +
                         " AND " + GetSqlInClause(collectionName, "OrganizationId") + ")" +
                         ")";
            return result;
        }

        private void FillServicesInformation(IReadOnlyList<IVmEntityListItem> search, IUnitOfWork unitOfWork)
        {
            var vmEntityListItems = search.Where(x => x.EntityType == SearchEntityTypeEnum.Service).ToList();
            var entitiesIds = vmEntityListItems.Select(x => x.Id).ToList();
            if (entitiesIds.Any())
            {
                var organizationServicesRep = unitOfWork.CreateRepository<IOrganizationServiceRepository>();
                var organizationServices = organizationServicesRep.All().Where(i => entitiesIds.Contains(i.ServiceVersionedId)).ToList().GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.Select(j => j.OrganizationId).ToList());
                
                var serviceProducerOrgRep = unitOfWork.CreateRepository<IServiceProducerOrganizationRepository>();
                var spo = serviceProducerOrgRep.All().Where(i => entitiesIds.Contains(i.ServiceProducer.ServiceVersionedId)).Select(i => new { i.ServiceProducer.ServiceVersionedId, i.OrganizationId}).ToList().GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.Select(j => j.OrganizationId).ToList());
                
                var names = GetServicesNames(entitiesIds, unitOfWork);
                var languages = GetServicesLanguageAvailability(entitiesIds, unitOfWork);
                var idsWithOrders = GetListOfExistingTranslationOrders<ServiceTranslationOrder>(vmEntityListItems.Select(x => x.UnificRootId).ToList(), unitOfWork);
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
                });
            }            
        }

        private bool IsCopyTagVisible(IVmEntityListItem x)
        {
            return x.OriginalId.HasValue && x.VersionMajor == 0 && x.VersionMinor <= 1;
        }

        private HashSet<Guid> GetListOfExistingTranslationOrders<TEntity>(List<Guid> ids, IUnitOfWork unitOfWork) where TEntity : IEntityTranslationOrder
        {
            var arrivedState = typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.Arrived.ToString());
            var rep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            return rep.All()
                .Where(x =>
                    ids.Contains(x.UnificRootId) &&
                    x.TranslationOrder.TranslationOrderStates.Any(tos => tos.Last && tos.TranslationStateId != arrivedState))
                .Select(x => x.UnificRootId)
                .ToHashSet();
        }

        private Dictionary<Guid, List<ServiceLanguageAvailability>> GetServicesLanguageAvailability(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork)
        {
            var langAvailabilitiesRep = unitOfWork.CreateRepository<IServiceLanguageAvailabilityRepository>();
            return langAvailabilitiesRep.All().Where(x => entitiesIds.Contains(x.ServiceVersionedId)).ToList()
                .GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());

        }
        private Dictionary<Guid, Dictionary<string, string>> GetServicesNames(ICollection<Guid> serviceIds, IUnitOfWork unitOfWork)
        {
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var serviceNameRep = unitOfWork.CreateRepository<IServiceNameRepository>();

            return serviceNameRep.All()
                .Where(x => serviceIds.Contains(x.ServiceVersionedId) && (x.TypeId == nameTypeId))
                .OrderBy(i => i.Localization.OrderNumber)
                .Select(i => new {i.ServiceVersionedId, i.Name, i.LocalizationId}).ToList()
                .GroupBy(i => i.ServiceVersionedId)
                .ToDictionary(i => i.Key,
                    i => i.ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));
        }
        
        #endregion
        
        #region Organization SQL
        private string  GetSearchOrganizationSql(IVmEntitySearch vmEntitySearch)
        {
            #region SearchByFilterParam

            var resultSelect = GetSqlOrganizationSelect();
            
            if (vmEntitySearch.EntityIds != null && vmEntitySearch.EntityIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<OrganizationVersioned>("EntityIds", "UnificRootId")); 
                               
            } 
            else if (vmEntitySearch.EntityVersionIds != null && vmEntitySearch.EntityVersionIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<OrganizationVersioned>("EntityVersionIds", "Id")); 
                               
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
                    resultSelect = AddSqlWhere(resultSelect, GetSqlOrganizations()); 
                }

                if (ApplySearchById(vmEntitySearch))
                {
                    var rootId = vmEntitySearch.Id;
                    if (rootId.IsAssigned())
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlName("OrganizationVersioned.UnificRootId", " = '"+rootId+"'"));
                    }
                }
                
                if (ApplySearchByName(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchName<OrganizationVersioned>());
                }
                if (ApplySearchByEmail(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchEmail<OrganizationVersioned>());
                }
                if (ApplySearchByAddress(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchAddress<OrganizationVersioned, OrganizationAddress>(vmEntitySearch.AddressStreetId.IsAssigned(), !string.IsNullOrEmpty(vmEntitySearch.StreetNumber), vmEntitySearch.PostalCodeId.IsAssigned()));
                }
                
                if (ApplySearchByPhone(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchPhone<OrganizationVersioned,OrganizationPhone>(vmEntitySearch.IsLocalNumber, vmEntitySearch.PhoneDialCode, vmEntitySearch.PhoneNumber));
                }

                if (vmEntitySearch.LanguageIds != null && vmEntitySearch.LanguageIds.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlInClause<OrganizationName>("LanguageIds", "LocalizationId"));
                }
            }
            
            if (vmEntitySearch.SelectedPublishingStatuses.Count != 0)
            {
               resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<OrganizationVersioned>("SelectedPublishingStatuses", "PublishingStatusId"));
            }

            resultSelect = resultSelect + GetSqlEntityOrder<OrganizationVersioned>();
            
            #endregion SearchByFilterParam

            return resultSelect;
        }
        private string GetSqlOrganizationSelect()
        {
            return SqlStatement.SelectDistinct +
                   GetSqlEntityColumns<OrganizationVersioned, OrganizationName>() +
                   GetSqlSelectNameAs("OrganizationVersioned.ParentId",GetSqlName("OrganizationId")) +
                   GetSqlSelectValueAs("NULL::uuid", "OriginalId") +
                   GetSqlSelectStringValueAs(SqlConstants.Organization, SqlConstants.EntityType) +
                   GetSqlSelectStringValueAs(SqlConstants.Organization, SqlConstants.SubEntityType) +
                   GetSqlSelectEntityNameType("OrganizationName", "OrganizationDisplayNameType.DisplayNameTypeId", SqlConstants.NameType, false) +
                   SqlStatement.From + GetSqlName("OrganizationVersioned") +
                   GetSqlInnerJoin("OrganizationName", "OrganizationVersioned.Id", "OrganizationName.OrganizationVersionedId") +
                   GetSqlInnerJoin("Language", "OrganizationName.LocalizationId", "Language.Id") +
                   GetSqlInnerJoin("Versioning", "OrganizationVersioned.VersioningId", "Versioning.Id") +
                   GetSqlInnerJoin("OrganizationDisplayNameType",
                       "OrganizationName.OrganizationVersionedId", "OrganizationDisplayNameType.OrganizationVersionedId",
                       "OrganizationName.LocalizationId", "OrganizationDisplayNameType.LocalizationId");
        }
        private string GetSqlOrganizations()
        {
            var result = "( " + GetSqlInClause<OrganizationVersioned>("OrganizationIds", "ParentId") +
                         " OR " + GetSqlInClause<OrganizationVersioned>("SubOrganizationIds", "UnificRootId") +
                         " OR " + GetSqlInClause<OrganizationVersioned>("OrganizationIds", "UnificRootId") + ") ";
            return result;
        }

        private void FillOrganizationsInformation(IReadOnlyList<IVmEntityListItem> search, IUnitOfWork unitOfWork)
        {
            var vmEntityListItems = search.Where(x => x.EntityType == SearchEntityTypeEnum.Organization).ToList();
            var entitiesIds = vmEntityListItems.Select(x => x.Id).ToList();
            if (entitiesIds.Any())
            {
                var names = GetOrganizationsNames(entitiesIds, unitOfWork);
                var languages = GetOrganizationsLanguageAvailability(entitiesIds, unitOfWork);
                vmEntityListItems.ForEach(x=>
                {
                    x.Name = names.TryGetOrDefault(x.Id, new Dictionary<string, string>());
                    x.LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            languages.TryGetOrDefault(x.Id, new List<OrganizationLanguageAvailability>()));
                });
            }            
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
        private string GetSearchChannelSql(IVmEntitySearch vmEntitySearch)
        {
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

            var resultSelect = GetSqlChannelSelect();
            
            if (vmEntitySearch.EntityIds != null && vmEntitySearch.EntityIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<ServiceChannelVersioned>("EntityIds", "UnificRootId")); 
                               
            } else if (vmEntitySearch.EntityVersionIds != null && vmEntitySearch.EntityVersionIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<ServiceChannelVersioned>("EntityVersionIds", "Id")); 
                               
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
                        GetSqlInClause<ServiceChannelVersioned>("OrganizationIds", "OrganizationId")); 
                }

                if (ApplySearchById(vmEntitySearch))
                {
                    var rootId = vmEntitySearch.Id;
                    if (rootId.HasValue)
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlName("ServiceChannelVersioned.UnificRootId", " = '"+rootId+"'"));
                    }
                }

                if (ApplySearchByName(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchName<ServiceChannelVersioned>());
                }
                if (ApplySearchByEmail(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchEmail<ServiceChannelVersioned>());
                }
                if (ApplySearchByAddress(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchAddress<ServiceChannelVersioned, ServiceChannelAddress>(vmEntitySearch.AddressStreetId.IsAssigned(), vmEntitySearch.AddressStreetNumberId.IsAssigned() || !string.IsNullOrEmpty(vmEntitySearch.StreetNumber), vmEntitySearch.PostalCodeId.IsAssigned()));
                }
                
                if (ApplySearchByPhone(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchPhone<ServiceChannelVersioned, ServiceChannelPhone>(vmEntitySearch.IsLocalNumber, vmEntitySearch.PhoneDialCode, vmEntitySearch.PhoneNumber));
                }

                if (vmEntitySearch.LanguageIds != null && vmEntitySearch.LanguageIds.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlInClause<ServiceChannelName>("LanguageIds", "LocalizationId"));
                }
            }
            
            if (vmEntitySearch.SelectedPublishingStatuses.Count != 0)
            {
               resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<ServiceChannelVersioned>("SelectedPublishingStatuses", "PublishingStatusId"));
            }
            
            resultSelect = resultSelect + GetSqlEntityOrder<ServiceChannelVersioned>(SqlConstants.SubEntityType);
            
            #endregion SearchByFilterParam

            return resultSelect;
        }
        private string GetSqlChannelSelect()
        {
            return SqlStatement.SelectDistinct +
                   GetSqlEntityColumns<ServiceChannelVersioned, ServiceChannelName>() +
                   GetSqlSelectName("ServiceChannelVersioned.OrganizationId") +
                   GetSqlSelectName("ServiceChannelVersioned.OriginalId") +
                   GetSqlSelectStringValueAs(SqlConstants.Channel, SqlConstants.EntityType) +
                   GetSqlSelectNameAs("ServiceChannelType.Code", GetSqlName(SqlConstants.SubEntityType)) +  
                   GetSqlSelectValueAs("0", SqlConstants.NameType, false) +
                   SqlStatement.From + GetSqlName("ServiceChannelVersioned") +
                   GetSqlInnerJoin("ServiceChannelName", "ServiceChannelVersioned.Id", "ServiceChannelName.ServiceChannelVersionedId") +
                   GetSqlInnerJoin("Language", "ServiceChannelName.LocalizationId", "Language.Id") +
                   GetSqlInnerJoin("ServiceChannelType", "ServiceChannelVersioned.TypeId", "ServiceChannelType.Id") +
                   GetSqlInnerJoin("Versioning", "ServiceChannelVersioned.VersioningId", "Versioning.Id");
        }
        
        private void FillChannelsInformation(IReadOnlyList<IVmEntityListItem> search, IUnitOfWork unitOfWork)
        {
            var vmEntityListItems = search.Where(x => x.EntityType == SearchEntityTypeEnum.Channel).ToList();
            var entitiesIds = vmEntityListItems.Select(x => x.Id).ToList();
            if (entitiesIds.Any())
            {
                var names = GetChannelsNames(entitiesIds, unitOfWork);
                var languages = GetChannelsLanguageAvailability(entitiesIds, unitOfWork);
                var idsWithOrders = GetListOfExistingTranslationOrders<ServiceChannelTranslationOrder>(vmEntityListItems.Select(x => x.UnificRootId).ToList(), unitOfWork); 
                vmEntityListItems.ForEach(x=>
                {
                    x.Name = names.TryGetOrDefault(x.Id, new Dictionary<string, string>());
                    x.LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            languages.TryGetOrDefault(x.Id, new List<ServiceChannelLanguageAvailability>()));
                    x.IsCopyTagVisible = IsCopyTagVisible(x);
                    x.HasTranslationOrder = idsWithOrders.Contains(x.UnificRootId);
                });
            }            
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
        private string GetSearchServiceCollectionSql(IVmEntitySearch vmEntitySearch)
        {
            #region SearchByFilterParam

            var resultSelect = GetSqlServiceCollectionSelect();
            
            if (vmEntitySearch.EntityIds != null && vmEntitySearch.EntityIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<ServiceCollectionVersioned>("EntityIds", "UnificRootId")); 
                               
            } else if (vmEntitySearch.EntityVersionIds != null && vmEntitySearch.EntityVersionIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<ServiceCollectionVersioned>("EntityVersionIds", "Id")); 
                               
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
                        GetSqlInClause<ServiceCollectionVersioned>("OrganizationIds", "OrganizationId")); 
                }
                
                if (ApplySearchById(vmEntitySearch))
                {
                    var rootId = vmEntitySearch.Id;
                    if (rootId.HasValue)
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlName("ServiceCollectionVersioned.UnificRootId", " = '" + rootId + "'"));
                    }
                }
                if (ApplySearchByName(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchName<ServiceCollectionVersioned>());
                }
                if (ApplySearchByEmail(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchEmail<ServiceCollectionVersioned>());
                }

                if (vmEntitySearch.LanguageIds != null && vmEntitySearch.LanguageIds.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlInClause<ServiceCollectionName>("LanguageIds", "LocalizationId"));
                }
            }
            
            if (vmEntitySearch.SelectedPublishingStatuses.Count != 0)
            {
               resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<ServiceCollectionVersioned>("SelectedPublishingStatuses", "PublishingStatusId"));
            }

            resultSelect = resultSelect + GetSqlEntityOrder<ServiceCollectionVersioned>();
            
            #endregion SearchByFilterParam

            return resultSelect;
        }
        private string GetSqlServiceCollectionSelect()
        {
            return SqlStatement.SelectDistinct +
                   GetSqlEntityColumns<ServiceCollectionVersioned, ServiceCollectionName>() +
                   GetSqlSelectName("ServiceCollectionVersioned.OrganizationId") +
                   GetSqlSelectName("ServiceCollectionVersioned.OriginalId") +
                   GetSqlSelectStringValueAs(SqlConstants.ServiceCollection, SqlConstants.EntityType) +
                   GetSqlSelectStringValueAs(SqlConstants.ServiceCollection, SqlConstants.SubEntityType) +
                   GetSqlSelectValueAs("0", SqlConstants.NameType, false) +
                   SqlStatement.From + GetSqlName("ServiceCollectionVersioned") +
                   GetSqlInnerJoin("ServiceCollectionName", "ServiceCollectionVersioned.Id",
                       "ServiceCollectionName.ServiceCollectionVersionedId") +
                   GetSqlInnerJoin("Language", "ServiceCollectionName.LocalizationId", "Language.Id") +
                   GetSqlInnerJoin("Versioning", "ServiceCollectionVersioned.VersioningId", "Versioning.Id");    
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
        private string GetSearchGeneralDescriptionSql(IVmEntitySearch vmEntitySearch)
        {
            #region SearchByFilterParam

            var resultSelect = GetSqlGeneralDescriptionSelect();
            
            if (vmEntitySearch.EntityIds != null && vmEntitySearch.EntityIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<StatutoryServiceGeneralDescriptionVersioned>("EntityIds", "UnificRootId")); 
                               
            } else if (vmEntitySearch.EntityVersionIds != null && vmEntitySearch.EntityVersionIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<StatutoryServiceGeneralDescriptionVersioned>("EntityVersionIds", "Id")); 
                               
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
                        GetSqlGeneralDescriptionFintoClause<ServiceClass>("ServiceClasses"));
                }
                //Life events
                if (vmEntitySearch.LifeEvents != null && vmEntitySearch.LifeEvents.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlGeneralDescriptionFintoClause<LifeEvent>("LifeEvents"));
                }
                
                //Industrial classes
                if (vmEntitySearch.IndustrialClasses != null && vmEntitySearch.IndustrialClasses.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlGeneralDescriptionFintoClause<IndustrialClass>("IndustrialClasses"));
                }
                
                //Ontology terms
                if (vmEntitySearch.OntologyTerms != null && vmEntitySearch.OntologyTerms.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlGeneralDescriptionFintoClause<OntologyTerm>("OntologyTerms"));
                }
                
                //Target groups
                if (vmEntitySearch.TargetGroups != null && vmEntitySearch.TargetGroups.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlGeneralDescriptionFintoClause<TargetGroup>("TargetGroups"));
                }
                
                if (ApplySearchById(vmEntitySearch))
                {
                    var rootId = vmEntitySearch.Id;
                    if (rootId.HasValue)
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlName("StatutoryServiceGeneralDescriptionVersioned.UnificRootId", " = '" + rootId + "'"));
                    }
                }
                
                if (ApplySearchByName(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchName<StatutoryServiceGeneralDescriptionVersioned>());
                }
                
                if (ApplySearchByEmail(vmEntitySearch))
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlSearchEmail<StatutoryServiceGeneralDescriptionVersioned>());
                }

                if (vmEntitySearch.LanguageIds != null && vmEntitySearch.LanguageIds.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect,
                        GetSqlInClause<StatutoryServiceName>("LanguageIds", "LocalizationId"));
                }

                if (vmEntitySearch.ServiceTypes != null && vmEntitySearch.ServiceTypes.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<StatutoryServiceGeneralDescriptionVersioned>("ServiceTypes", "TypeId"));      
                }  
                if (vmEntitySearch.GeneralDescriptionTypes != null && vmEntitySearch.GeneralDescriptionTypes.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<StatutoryServiceGeneralDescriptionVersioned>("GeneralDescriptionTypes", "GeneralDescriptionTypeId"));           
                }
            }
            
            if (vmEntitySearch.SelectedPublishingStatuses.Count != 0)
            {
               resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<StatutoryServiceGeneralDescriptionVersioned>("SelectedPublishingStatuses", "PublishingStatusId"));
            }

            resultSelect = resultSelect + GetSqlEntityOrder<StatutoryServiceGeneralDescriptionVersioned>();
            
            #endregion SearchByFilterParam

            return resultSelect;
        }
        private string GetSqlGeneralDescriptionSelect()
        {
            return SqlStatement.SelectDistinct +
                   GetSqlEntityColumns<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceName>() +
                   GetSqlSelectValueAs("NULL::uuid", "OrganizationId") +
                   GetSqlSelectValueAs("NULL::uuid", "OriginalId") +
                   GetSqlSelectStringValueAs(SqlConstants.GeneralDescription, SqlConstants.EntityType) +
                   GetSqlSelectStringValueAs(SqlConstants.GeneralDescription, SqlConstants.SubEntityType) +
                   GetSqlSelectValueAs("0", SqlConstants.NameType, false) +
                   SqlStatement.From + GetSqlName("StatutoryServiceGeneralDescriptionVersioned") +
                   GetSqlInnerJoin("StatutoryServiceName", "StatutoryServiceGeneralDescriptionVersioned.Id", "StatutoryServiceName.StatutoryServiceGeneralDescriptionVersionedId") +
                   GetSqlInnerJoin("Language", "StatutoryServiceName.LocalizationId", "Language.Id") +
                   GetSqlInnerJoin("Versioning", "StatutoryServiceGeneralDescriptionVersioned.VersioningId", "Versioning.Id");   
        }
        private string GetSqlGeneralDescriptionFintoClause<TFinto>(string collectionName)
        {
            var entityName = "StatutoryServiceGeneralDescriptionVersioned";
            var fintoName = typeof(TFinto).Name;
            var result = " EXISTS (SELECT 1 FROM " + GetSqlName("StatutoryService" + fintoName) + " WHERE " +
                         GetSqlName(entityName + ".Id", "=") +
                         GetSqlName("StatutoryService" + fintoName + "." + entityName + "Id") +
                         " AND " + GetSqlInClause(collectionName, fintoName + "Id") + ")";
            return result;
        }         
        
        private void FillGeneralDescriptionsInformation(IReadOnlyList<IVmEntityListItem> search, IUnitOfWork unitOfWork)
        {
            var vmEntityListItems = search.Where(x => x.EntityType == SearchEntityTypeEnum.GeneralDescription).ToList();
            var entitiesIds = vmEntityListItems.Select(x => x.Id).ToList();
            if (entitiesIds.Any())
            {
                var names = GetGeneralDescriptionsNames(entitiesIds, unitOfWork);
                var languages = GetGeneralDescriptionsLanguageAvailability(entitiesIds, unitOfWork);
                var idsWithOrders = GetListOfExistingTranslationOrders<GeneralDescriptionTranslationOrder>(vmEntityListItems.Select(x => x.UnificRootId).ToList(), unitOfWork);
                vmEntityListItems.ForEach(x=>
                {
                    x.Name = names.TryGetOrDefault(x.Id, new Dictionary<string, string>());
                    x.LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            languages.TryGetOrDefault(x.Id, new List<GeneralDescriptionLanguageAvailability>()));
                    x.HasTranslationOrder = idsWithOrders.Contains(x.UnificRootId);
                });
            }            
        }
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
      
        private string GetSqlEntityOrder<TEntity>(string additionGroupBy = "")
        {
            var entityName = typeof(TEntity).Name;
            var addGroupBy = !string.IsNullOrEmpty(additionGroupBy) ? ", "+ GetSqlName(additionGroupBy) : "";
            return
                SqlStatement.GroupBy + 
                GetSqlSelectName(SqlConstants.TempName) + 
                GetSqlSelectName(SqlConstants.NameType) +
                GetSqlSelectName(entityName+".Id") + 
                GetSqlSelectName("Versioning.VersionMajor") +
                GetSqlName("Versioning.VersionMinor") + 
                addGroupBy +
                SqlStatement.OrderBy + 
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
        private string GetSqlSelectNameAs(string name, string asName)
        {
            return GetSqlName(name, " AS "+asName+", ");
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
        private string GetSqlInnerJoin(string toTable, string fromId1, string toId1, string fromId2, string toId2)
        {
            return " Inner Join " + GetSqlName(toTable) +
                   " ON " + GetSqlName(fromId1) +
                   " = " + GetSqlName(toId1) + " AND " 
                   + GetSqlName(fromId2) +
                   " = " + GetSqlName(toId2);
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
        
        private string GetSqlSearchEmail<TEntity>()
        {
            var entityName = typeof(TEntity).Name;
            var result = "(lower(" +
                         GetSqlName(entityName + ".CreatedBy", ") Like CONCAT('%',@Email,'%')") +
                         " OR lower(" +
                         GetSqlName(entityName + ".ModifiedBy", ") Like CONCAT('%',@Email,'%')") + ")";
            return result;
        }
        
        private string GetSqlSearchAddress<TEntity, TAddrRel>(bool byStreet, bool byStreetNumber, bool byPostalCode)
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
        
            result += " AND (\"m.Address\".\"Id\" = n.\"AddressId\")) AND ("+GetSqlName(entityName)+".\"Id\" = m."+GetSqlName(entityName+"Id")+")))";
            
            return result;
        }

        private Guid? GetDefaultDialCodeId()
        {
            return contextManager.ExecuteReader(unitOfWork => commonService.GetDefaultDialCodeId(unitOfWork) );
        }
        
        private string GetSqlSearchPhone<TEntity, TPhoneRel>(bool? isLocalNumber, Guid? prefixNumber, string phoneNumber)
        {
            var defaultDialCodeId = GetDefaultDialCodeId();
            isLocalNumber = (isLocalNumber.HasValue && isLocalNumber.Value) || 
                            (prefixNumber.HasValue && prefixNumber.Value == defaultDialCodeId);
            var entityName = typeof(TEntity).Name;
            var phoneRelationName = typeof(TPhoneRel).Name;
            var result = "EXISTS (SELECT 1 FROM " + GetSqlName(phoneRelationName) + " AS m INNER JOIN \"Phone\" AS \"m.Phone\" ON m.\"PhoneId\" = \"m.Phone\".\"Id\"" +
                (isLocalNumber == true
                ? $" WHERE ((\"m.Phone\".\"PrefixNumberId\" IS NULL OR \"m.Phone\".\"PrefixNumberId\" = '" + defaultDialCodeId + "')" 
                : " WHERE ((\"m.Phone\".\"PrefixNumberId\" = @PhoneDialCode) ") +
                (!string.IsNullOrEmpty(phoneNumber)
                ?  "AND (STRPOS(\"m.Phone\".\"Number\", @PhoneNumber) > 0)) " 
                :  " )" )
                + "AND ("+GetSqlName(entityName)+".\"Id\" = m."+GetSqlName(entityName+"Id")+"))";
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
                   GetSqlSelectName("Versioning.VersionMinor");
        }

        private string GetSqlSorting(List<VmSortParam> sortParams, VmSortParam defaultSortParam = null, bool useVersionSorting = true)
        {
            StringBuilder orderBuilder = new StringBuilder();            
        
            var versionSortingParams = new List<VmSortParam>() {
                new VmSortParam() { Column = "UnificRootId", SortDirection = SortDirectionEnum.Asc, Order = 997},
                new VmSortParam() { Column = "VersionMajor", SortDirection = SortDirectionEnum.Desc, Order = 998},
                new VmSortParam() { Column = "VersionMinor", SortDirection = SortDirectionEnum.Desc, Order = 999},
            };

            if (!sortParams.Any() && defaultSortParam == null)
            {
                return String.Empty;
            }

            sortParams = sortParams.Any() ? sortParams : new List<VmSortParam>() { defaultSortParam };

            if (sortParams.Any(x => x.Column == "entityType"))
            {
                sortParams.Add(new VmSortParam { Column = SqlConstants.SubEntityType, SortDirection = SortDirectionEnum.Asc});
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
               GetSqlName("VersionMinor") +
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
                   GetSqlName("VersionMinor") +
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
            var searchSetup = new Dictionary<SearchEntityTypeEnum, List<SearchTypeEnum>>()
            {
                {SearchEntityTypeEnum.Channel, new List<SearchTypeEnum>() { SearchTypeEnum.Other }} ,
                {SearchEntityTypeEnum.EChannel, new List<SearchTypeEnum>() { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email }} ,
                {SearchEntityTypeEnum.WebPage, new List<SearchTypeEnum>() { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email }} ,
                {SearchEntityTypeEnum.PrintableForm, new List<SearchTypeEnum>() { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email }} ,
                {SearchEntityTypeEnum.Phone, new List<SearchTypeEnum>() { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email, SearchTypeEnum.Phone }} ,
                {SearchEntityTypeEnum.ServiceLocation, new List<SearchTypeEnum>() { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email, SearchTypeEnum.Address }} ,
                
                {SearchEntityTypeEnum.Service, new List<SearchTypeEnum>() { SearchTypeEnum.Other }} ,
                {SearchEntityTypeEnum.ServiceService, new List<SearchTypeEnum>() { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email }} ,
                {SearchEntityTypeEnum.ServicePermit, new List<SearchTypeEnum>() { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email }} ,
                {SearchEntityTypeEnum.ServiceProfessional, new List<SearchTypeEnum>() { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email }} ,
                
                {SearchEntityTypeEnum.Organization, new List<SearchTypeEnum>() { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email, SearchTypeEnum.Address, SearchTypeEnum.Phone, SearchTypeEnum.Other }} ,
                {SearchEntityTypeEnum.ServiceCollection, new List<SearchTypeEnum>() { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email }} ,
                {SearchEntityTypeEnum.GeneralDescription, new List<SearchTypeEnum>() { SearchTypeEnum.Id, SearchTypeEnum.Name, SearchTypeEnum.Email }} ,
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
            resultSelect = AddSqlWhere(resultSelect, GetSqlName("TrackingServiceServiceChannel.Created", " > timezone('utc', now() - interval '1 month' )"));
            return resultSelect + AddSqlNotificationOrder();
        }
        
        private string GetSearchNotificationChannelSql()
        {
            var resultSelect = GetSqlNotificationChannelSelect();
            resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<ServiceChannelVersioned>("OrganizationIds", "OrganizationId"));
            resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<ServiceVersioned>("OrganizationIds", "OrganizationId", false));
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
            VmSearchResult<IVmNotificationConnectionListItem> returnData = new VmSearchResult<IVmNotificationConnectionListItem>() {SearchResult = new List<IVmNotificationConnectionListItem>()};

            var tempSql = string.Empty;
            tempSql = AddSqlToUnion(tempSql, GetSearchNotificationServiceSql());
            tempSql = AddSqlToUnion(tempSql, GetSearchNotificationChannelSql());
            
            var totalCountSql = GetSqlCountEntitySelect(tempSql);
           
            var searchSql = GetSqlConnectionsPageSelect(
                GetSqlConnectionsOrderSelect(
                    tempSql,
                    vmEntitySearch.SortData,
                    new VmSortParam() {Column = "Created", SortDirection = SortDirectionEnum.Desc}),
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

        private static class SqlConstants
        {
            public const string GeneralDescription = "GeneralDescription";
            public const string ServiceCollection = "ServiceCollection";
            public const string Organization = "Organization";
            public const string Service = "Service";
            public const string Channel = "Channel";
            public const string EntityType = "EntityType";
            public const string SubEntityType = "SubEntityType";
            public const string NameType = "nametype";
            public const string TempName = "tempname";
            public const string Min = "min";
        }

        #endregion constants
    }
}
