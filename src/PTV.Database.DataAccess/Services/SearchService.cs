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
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.ResponseCaching.Internal;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.DirectRaw;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;


namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ISearchService), RegisterType.Transient)]
    internal class SearchService : ServiceBase, ISearchService
    {
        
        private readonly IContextManager contextManager;
        private readonly IDatabaseRawContext rawContext;
        private readonly ITypesCache typesCache;
        private ILanguageCache languageCache;
        private ILanguageOrderCache languageOrderCache;
        private ICommonServiceInternal commonService;
        private IPahaTokenProcessor pahaTokenProcessor;
        
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
            IPahaTokenProcessor pahaTokenProcessor
            )
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
            this.typesCache = typesCache;
            this.languageCache = languageCache;
            this.commonService = commonService;            
            this.rawContext = rawContext;
            this.languageOrderCache = languageOrderCache;
            this.pahaTokenProcessor = pahaTokenProcessor;
        }
        
        public IVmSearchBase SearchEntities(IVmEntitySearch vmEntitySearch)
        {
            VmSearchResult<IVmEntityListItem> returnData = new VmSearchResult<IVmEntityListItem>() {SearchResult = new List<IVmEntityListItem>()};
            
            contextManager.ExecuteReader(unitOfWork =>
            {
                FilterSearchCtiteria(vmEntitySearch, unitOfWork);
                FilterSotringCriteria(vmEntitySearch);
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
                            if (!isServiceSearchInvoked)
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
                            if (!isChannelSearchInvoked)
                            {
                                isChannelSearchInvoked = true;
                                tempSql = AddSqlToUnion(tempSql, GetSearchChannelSql(vmEntitySearch));
                            }
                            break;
                        case SearchEntityTypeEnum.GeneralDescription:
                            tempSql = AddSqlToUnion(tempSql, GetSearchGeneralDescriptionSql(vmEntitySearch));
                            break;
                        case SearchEntityTypeEnum.Organization:
                            tempSql = AddSqlToUnion(tempSql, GetSearchOrganizationSql(vmEntitySearch));
                            break;
                        case SearchEntityTypeEnum.ServiceCollection:
                           tempSql = AddSqlToUnion(tempSql, GetSearchServiceCollectionSql(vmEntitySearch));
                            break;
                    }
                }
                
                var totalCountSql = GetSqlCountEntitySelect(tempSql);
                var searchSql = GetSqlAllEntityPageSelect(
                    GetSqlAllEntityOrderSelect(
                        tempSql,
                        vmEntitySearch.SortData,
                        new VmSortParam() {Column = "Modified", SortDirection = SortDirectionEnum.Desc}),
                    page: vmEntitySearch.PageNumber);
                
                var totalCount = ExecuteCount(totalCountSql, vmEntitySearch);
                var result = ExecuteSearch(searchSql, vmEntitySearch).ToList();
                
                FillServicesInformation(result, unitOfWork, vmEntitySearch.LanguageIds);
                FillOrganizationsInformation(result, unitOfWork, vmEntitySearch.LanguageIds);
                FillChannelsInformation(result, unitOfWork, vmEntitySearch.LanguageIds);
                FillServiceCollectionsInformation(result, unitOfWork, vmEntitySearch.LanguageIds);
                FillGeneralDescriptionsInformation(result, unitOfWork, vmEntitySearch.LanguageIds);
                
                var safePageNumber = vmEntitySearch.PageNumber.PositiveOrZero();
                var moreAvailable = totalCount.MoreResultsAvailable(safePageNumber);
                returnData.Count = totalCount;
                returnData.PageNumber = ++safePageNumber;
                returnData.MoreAvailable = moreAvailable;
                returnData.SearchResult = result;
                
                return returnData;
            });
            
            FillEnumEntities(returnData, () =>
            { 
                var usedOrgIds = returnData.SearchResult
                    .SelectMany(org => org.Organizations)
                    .Union(returnData.SearchResult.SelectMany(org => org.Producers))
                    .Union(returnData.SearchResult.Where(ent=>ent.OrganizationId.HasValue).Select(ent => ent.OrganizationId.Value))
                    .Distinct()
                    .ToList();

                return GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizations(usedOrgIds));
            });
            return returnData;
        }

        private IList<VmEntityListItem> ExecuteSearch(string sql, IVmEntitySearch vmEntitySearch)
        {
            return rawContext.ExecuteReader(db => db.SelectList<VmEntityListItem>(sql, vmEntitySearch));
        }
        private int ExecuteCount(string sql, IVmEntitySearch vmEntitySearch)
        {
            return rawContext.ExecuteReader(db => db.SelectOne<int>(sql, vmEntitySearch));
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
        

        private void FilterSearchCtiteria(IVmEntitySearch vmEntitySearch, IUnitOfWork unitOfWork)
        {
            if (vmEntitySearch.ContentTypes == null || !vmEntitySearch.ContentTypes.Any())
            {
                vmEntitySearch.ContentTypes = Enum.GetValues(typeof(SearchEntityTypeEnum)).Cast<SearchEntityTypeEnum>().ToList();
            }

            vmEntitySearch.ServiceServiceType = GetServiceTypeCriteria(vmEntitySearch);
            vmEntitySearch.Name = vmEntitySearch.Name != null ? Regex.Replace(vmEntitySearch.Name.Trim(), @"\s+", " ").ToLower() : vmEntitySearch.Name;           
            vmEntitySearch.LanguageIds = vmEntitySearch.Languages?.Select(language => languageCache.Get(language.ToString())).ToList();
            if (vmEntitySearch.LanguageIds != null && !vmEntitySearch.LanguageIds.Any())
            {
                vmEntitySearch.LanguageIds = languageCache.AllowedLanguageIds;
            }

            var isGdSelected = vmEntitySearch.ContentTypes.All(x =>  new List<SearchEntityTypeEnum>
            {
                SearchEntityTypeEnum.GeneralDescription
            }.Contains(x));
            var isSubFiltersEnabled = vmEntitySearch.ContentTypes.All(x => new List<SearchEntityTypeEnum>
            {
                SearchEntityTypeEnum.Service,
                SearchEntityTypeEnum.ServicePermit,
                SearchEntityTypeEnum.ServiceProfessional,
                SearchEntityTypeEnum.ServiceService,
                SearchEntityTypeEnum.GeneralDescription
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
                var tgRep = unitOfWork.CreateRepository<ITargetGroupRepository>();
                var selcectedTg = vmEntitySearch.TargetGroups.Any()
                    ? tgRep.All().Where(x => vmEntitySearch.TargetGroups.Contains(x.Id)).Select(x=>x.Code.ToUpper())
                    : tgRep.All().Select(x=>x.Code.ToUpper());
                if (!selcectedTg.Contains("KR1"))
                {
                    vmEntitySearch.LifeEvents = new List<Guid>();
                }
                if (!selcectedTg.Contains("KR2"))
                {
                    vmEntitySearch.IndustrialClasses = new List<Guid>();
                }
            }
            commonService.ExtendPublishingStatusesByEquivalents(vmEntitySearch.SelectedPublishingStatuses);
            vmEntitySearch.UserName = pahaTokenProcessor.UserName.ToLower();
        }

        private void FilterSotringCriteria(IVmEntitySearch vmEntitySearch)
        {
            if (vmEntitySearch.SortData.Any(x => x.Column == "entityType"))
            {
                var subChannelTypes = Enum.GetNames(typeof(ServiceChannelTypeEnum)).ToList();
                if (vmEntitySearch.ContentTypes.Select(x =>
                            subChannelTypes.Contains(x.ToString())
                                ? SearchEntityTypeEnum.Channel.ToString()
                                : x.ToString())
                        .Distinct().Count() == 1)
                {
                    vmEntitySearch.SortData = vmEntitySearch.SortData.Where(x=>x.Column != "entityType").ToList();
                }               
            }
        }

        #region Services SQL
        private string GetSearchServiceSql(IVmEntitySearch vmEntitySearch)
        {
            #region SearchByFilterParam

            var resultSelect = GetSqlServieSelect();
            
            if (vmEntitySearch.EntityIds != null && vmEntitySearch.EntityIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<ServiceVersioned>("EntityIds", "UnificRootId")); 
                               
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

                
                if (!string.IsNullOrEmpty(vmEntitySearch.Name))
                {
                    var rootId = GetRootIdFromString(vmEntitySearch.Name);
                    if (!rootId.HasValue)
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlSearchName<ServiceVersioned>());
                    }
                    else
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlName("ServiceVersioned.UnificRootId", " = '"+rootId+"'"));                        
                    }
                }
                resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<ServiceName>("LanguageIds", "LocalizationId"));

                if (vmEntitySearch.ServiceServiceType.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlServiceType("ServiceServiceType"));      
                }                
            }
            
            if (vmEntitySearch.SelectedPublishingStatuses != null)
            {
               resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<ServiceVersioned>("SelectedPublishingStatuses", "PublishingStatusId"));
            }

            resultSelect = resultSelect + GetSqlEntityOrder<ServiceVersioned>();
            
            #endregion SearchByFilterParam

            return resultSelect;
        }
        private string GetSqlServieSelect()
        {
            return "Select DISTINCT ON (1) " +
                   GetSqlEntityColumns<ServiceVersioned, ServiceName>() +
                   GetSqlName("ServiceVersioned.OrganizationId", ", ") +
                   "'Service' AS " + GetSqlName("EntityType",", ") +
                   "'Service' AS " + GetSqlName("SubEntityType",", ") +
                   "(case when " + GetSqlName("ServiceName.TypeId", "='") + typesCache.Get<NameType>(NameTypeEnum.Name.ToString()) + "' then 0 else 1 END) AS nametype " +
                   "from " + GetSqlName("ServiceVersioned", " ") +
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

        private void FillServicesInformation(List<VmEntityListItem> search, IUnitOfWork unitOfWork, ICollection<Guid> languagesIds)
        {
            var vmEntityListItems = search.ToList();
            var serviceIds = vmEntityListItems.Where(x => x.EntityType == SearchEntityTypeEnum.Service)
                .Select(x => x.Id).ToList();
            if (serviceIds.Any())
            {
                var organizationServicesRep = unitOfWork.CreateRepository<IOrganizationServiceRepository>();
                var organizationServices = organizationServicesRep.All().Where(i => serviceIds.Contains(i.ServiceVersionedId)).ToList().GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.Select(j => j.OrganizationId).ToList());
                
                var serviceProducerOrgRep = unitOfWork.CreateRepository<IServiceProducerOrganizationRepository>();
                var spo = serviceProducerOrgRep.All().Where(i => serviceIds.Contains(i.ServiceProducer.ServiceVersionedId)).Select(i => new { i.ServiceProducer.ServiceVersionedId, i.OrganizationId}).ToList().GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.Select(j => j.OrganizationId).ToList());
                
                var names = GetServicesNames(serviceIds, unitOfWork, languagesIds);
                var languages = GetServicesLanguageAvailability(serviceIds, unitOfWork);
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
                });
            }            
        }
        private Dictionary<Guid, List<ServiceLanguageAvailability>> GetServicesLanguageAvailability(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork)
        {
            var langAvailabilitiesRep = unitOfWork.CreateRepository<IServiceLanguageAvailabilityRepository>();
            return langAvailabilitiesRep.All().Where(x => entitiesIds.Contains(x.ServiceVersionedId)).ToList()
                .GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());

        }
        private Dictionary<Guid, Dictionary<string, string>> GetServicesNames(ICollection<Guid> serviceIds, IUnitOfWork unitOfWork, ICollection<Guid> languagesIds)
        {
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var serviceNameRep = unitOfWork.CreateRepository<IServiceNameRepository>();

            return serviceNameRep.All()
                .Where(x => serviceIds.Contains(x.ServiceVersionedId) &&
                            ((languagesIds != null && languagesIds.Contains(x.LocalizationId) &&
                              x.TypeId == nameTypeId) || x.TypeId == nameTypeId))
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
                
                if (!string.IsNullOrEmpty(vmEntitySearch.Name))
                {
                    var rootId = GetRootIdFromString(vmEntitySearch.Name);
                    if (!rootId.HasValue)
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlSearchName<OrganizationVersioned>());
                    }
                    else
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlName("OrganizationVersioned.UnificRootId", " = '"+rootId+"'"));                        
                    }
                }
                resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<OrganizationName>("LanguageIds", "LocalizationId"));                               
            }
            
            if (vmEntitySearch.SelectedPublishingStatuses != null)
            {
               resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<OrganizationVersioned>("SelectedPublishingStatuses", "PublishingStatusId"));
            }

            resultSelect = resultSelect + GetSqlEntityOrder<OrganizationVersioned>();
            
            #endregion SearchByFilterParam

            return resultSelect;
        }
        private string GetSqlOrganizationSelect()
        {
            return "Select DISTINCT ON (1) " +
                   GetSqlEntityColumns<OrganizationVersioned, OrganizationName>() +
                   GetSqlName("OrganizationVersioned.ParentId", " AS ") + GetSqlName("OrganizationId", ", ") +
                   "'Organization' AS " + GetSqlName("EntityType", ", ") +
                   "'Organization' AS " + GetSqlName("SubEntityType", ", ") +
                   "(case when " + GetSqlName("OrganizationName.TypeId", "=") +
                   GetSqlName("OrganizationDisplayNameType.DisplayNameTypeId") + " then 0 else 1 END) AS nametype " +
                   " from " + GetSqlName("OrganizationVersioned", " ") +
                   GetSqlInnerJoin("OrganizationName", "OrganizationVersioned.Id",
                       "OrganizationName.OrganizationVersionedId") +
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

        private void FillOrganizationsInformation(List<VmEntityListItem> search, IUnitOfWork unitOfWork, ICollection<Guid> languagesIds)
        {
            var vmEntityListItems = search.Where(x => x.EntityType == SearchEntityTypeEnum.Organization).ToList();
            var entitiesIds = vmEntityListItems.Select(x => x.Id).ToList();
            if (entitiesIds.Any())
            {
                var names = GetOrganizationsNames(entitiesIds, unitOfWork, languagesIds);
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
        private Dictionary<Guid, Dictionary<string, string>> GetOrganizationsNames(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork, ICollection<Guid> languagesIds)
        {
            var aternateNameTypeId = typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString());
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
            if(vmEntitySearch.ContentTypes != null && vmEntitySearch.ContentTypes.Contains(SearchEntityTypeEnum.Channel))
            {
                channelTypes.ForEach(type =>
                    vmEntitySearch.ChannelTypeIds.Add(
                        typesCache.Get<ServiceChannelType>(type)));                    
            }
            else
            {
                vmEntitySearch.ContentTypes.ForEach(type =>
                {
                    if (channelTypes.Contains(type.ToString()))
                        vmEntitySearch.ChannelTypeIds.Add(
                            typesCache.Get<ServiceChannelType>(type.ToString()));
                });
            }
            #region SearchByFilterParam

            var resultSelect = GetSqlChannelSelect();
            
            if (vmEntitySearch.EntityIds != null && vmEntitySearch.EntityIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<ServiceChannelVersioned>("EntityIds", "UnificRootId")); 
                               
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
                
                if (!string.IsNullOrEmpty(vmEntitySearch.Name))
                {
                    var rootId = GetRootIdFromString(vmEntitySearch.Name);
                    if (!rootId.HasValue)
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlSearchName<ServiceChannelVersioned>());
                    }
                    else
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlName("ServiceChannelVersioned.UnificRootId", " = '"+rootId+"'"));                        
                    }
                }
                resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<ServiceChannelName>("LanguageIds", "LocalizationId"));                               
            }
            
            if (vmEntitySearch.SelectedPublishingStatuses != null)
            {
               resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<ServiceChannelVersioned>("SelectedPublishingStatuses", "PublishingStatusId"));
            }

            resultSelect = resultSelect + GetSqlEntityOrder<ServiceChannelVersioned>("ServiceChannelType.Code");
            
            #endregion SearchByFilterParam

            return resultSelect;
        }
        private string GetSqlChannelSelect()
        {
            return "Select DISTINCT ON (1) " +
                   GetSqlEntityColumns<ServiceChannelVersioned, ServiceChannelName>() +
                   GetSqlName("ServiceChannelVersioned.OrganizationId", ", ") +
                   "'Channel' AS " + GetSqlName("EntityType",", ") +                  
                   GetSqlName("ServiceChannelType.Code", " AS ") + GetSqlName("SubEntityType", ", ") +
                   "0 AS "+ GetSqlName("nametype") +
                   " from " + GetSqlName("ServiceChannelVersioned", " ") +
                   GetSqlInnerJoin("ServiceChannelName", "ServiceChannelVersioned.Id",
                       "ServiceChannelName.ServiceChannelVersionedId") +
                   GetSqlInnerJoin("Language", "ServiceChannelName.LocalizationId", "Language.Id") +
                   GetSqlInnerJoin("ServiceChannelType", "ServiceChannelVersioned.TypeId", "ServiceChannelType.Id") +
                   GetSqlInnerJoin("Versioning", "ServiceChannelVersioned.VersioningId", "Versioning.Id");                  
        }
        
        private void FillChannelsInformation(List<VmEntityListItem> search, IUnitOfWork unitOfWork, ICollection<Guid> languagesIds)
        {
            var vmEntityListItems = search.Where(x => x.EntityType == SearchEntityTypeEnum.Channel).ToList();
            var entitiesIds = vmEntityListItems.Select(x => x.Id).ToList();
            if (entitiesIds.Any())
            {
                var names = GetChannelsNames(entitiesIds, unitOfWork, languagesIds);
                var languages = GetChannelsLanguageAvailability(entitiesIds, unitOfWork);
                vmEntityListItems.ForEach(x=>
                {
                    x.Name = names.TryGetOrDefault(x.Id, new Dictionary<string, string>());
                    x.LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            languages.TryGetOrDefault(x.Id, new List<ServiceChannelLanguageAvailability>()));
                });
            }            
        }
        private Dictionary<Guid, List<ServiceChannelLanguageAvailability>> GetChannelsLanguageAvailability(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork)
        {
            var langAvailabilitiesRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
            return langAvailabilitiesRep.All().Where(x => entitiesIds.Contains(x.ServiceChannelVersionedId)).ToList()
                .GroupBy(i => i.ServiceChannelVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());

        }
        private Dictionary<Guid, Dictionary<string, string>> GetChannelsNames(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork, ICollection<Guid> languagesIds)
        {
            var aternateNameTypeId = typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString());
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

            var resultSelect = GetSqlServieCollectionSelect();
            
            if (vmEntitySearch.EntityIds != null && vmEntitySearch.EntityIds.Any())
            {
                resultSelect = AddSqlWhere(resultSelect,
                    GetSqlInClause<ServiceCollectionVersioned>("EntityIds", "UnificRootId")); 
                               
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
                
                if (!string.IsNullOrEmpty(vmEntitySearch.Name))
                {
                    var rootId = GetRootIdFromString(vmEntitySearch.Name);
                    if (!rootId.HasValue)
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlSearchName<ServiceCollectionVersioned>());
                    }
                    else
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlName("ServiceCollectionVersioned.UnificRootId", " = '"+rootId+"'"));                        
                    }
                }
                resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<ServiceCollectionName>("LanguageIds", "LocalizationId"));                               
            }
            
            if (vmEntitySearch.SelectedPublishingStatuses != null)
            {
               resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<ServiceCollectionVersioned>("SelectedPublishingStatuses", "PublishingStatusId"));
            }

            resultSelect = resultSelect + GetSqlEntityOrder<ServiceCollectionVersioned>();
            
            #endregion SearchByFilterParam

            return resultSelect;
        }
        private string GetSqlServieCollectionSelect()
        {
            return "Select DISTINCT ON (1) " +
                   GetSqlEntityColumns<ServiceCollectionVersioned, ServiceCollectionName>() +
                   GetSqlName("ServiceCollectionVersioned.OrganizationId", ", ") +
                   "'ServiceCollection' AS " + GetSqlName("EntityType", ", ") +
                   "'ServiceCollection' AS " + GetSqlName("SubEntityType",", ") +
                   "0 AS "+ GetSqlName("nametype") +
                   "from " + GetSqlName("ServiceCollectionVersioned", " ") +
                   GetSqlInnerJoin("ServiceCollectionName", "ServiceCollectionVersioned.Id",
                       "ServiceCollectionName.ServiceCollectionVersionedId") +
                   GetSqlInnerJoin("Language", "ServiceCollectionName.LocalizationId", "Language.Id") +
                   GetSqlInnerJoin("Versioning", "ServiceCollectionVersioned.VersioningId", "Versioning.Id");     
                ;
        }
       
        private void FillServiceCollectionsInformation(List<VmEntityListItem> search, IUnitOfWork unitOfWork, ICollection<Guid> languagesIds)
        {
            var vmEntityListItems = search.Where(x => x.EntityType == SearchEntityTypeEnum.ServiceCollection).ToList();
            var entitiesIds = vmEntityListItems.Select(x => x.Id).ToList();
            if (entitiesIds.Any())
            {
                var names = GetServiceCollectionsNames(entitiesIds, unitOfWork, languagesIds);
                var languages = GetServiceCollectionsLanguageAvailability(entitiesIds, unitOfWork);
                vmEntityListItems.ForEach(x=>
                {
                    x.Name = names.TryGetOrDefault(x.Id, new Dictionary<string, string>());
                    x.LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            languages.TryGetOrDefault(x.Id, new List<ServiceCollectionLanguageAvailability>()));
                });
            }            
        }
        private Dictionary<Guid, List<ServiceCollectionLanguageAvailability>> GetServiceCollectionsLanguageAvailability(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork)
        {
            var langAvailabilitiesRep = unitOfWork.CreateRepository<IServiceCollectionLanguageAvailabilityRepository>();
            return langAvailabilitiesRep.All().Where(x => entitiesIds.Contains(x.ServiceCollectionVersionedId)).ToList()
                .GroupBy(i => i.ServiceCollectionVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());

        }
        private Dictionary<Guid, Dictionary<string, string>> GetServiceCollectionsNames(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork, ICollection<Guid> languagesIds)
        {
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var nameRep = unitOfWork.CreateRepository<IServiceCollectionNameRepository>();

            return nameRep.All()
                .Where(x => entitiesIds.Contains(x.ServiceCollectionVersionedId) &&
                            ((languagesIds != null && languagesIds.Contains(x.LocalizationId) &&
                              x.TypeId == nameTypeId) || x.TypeId == nameTypeId))
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

                if (!string.IsNullOrEmpty(vmEntitySearch.Name))
                {
                    var rootId = GetRootIdFromString(vmEntitySearch.Name);
                    if (!rootId.HasValue)
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlSearchName<StatutoryServiceGeneralDescriptionVersioned>());
                    }
                    else
                    {
                        resultSelect = AddSqlWhere(resultSelect, GetSqlName("StatutoryServiceGeneralDescriptionVersioned.UnificRootId", " = '"+rootId+"'"));                        
                    }
                }
                resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<StatutoryServiceName>("LanguageIds", "LocalizationId"));

                if (vmEntitySearch.ServiceTypes != null && vmEntitySearch.ServiceTypes.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<StatutoryServiceGeneralDescriptionVersioned>("ServiceTypes", "TypeId"));      
                }  
                if (vmEntitySearch.GeneralDescriptionTypes != null && vmEntitySearch.GeneralDescriptionTypes.Any())
                {
                    resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<StatutoryServiceGeneralDescriptionVersioned>("GeneralDescriptionTypes", "GeneralDescriptionTypeId"));           
                }
            }
            
            if (vmEntitySearch.SelectedPublishingStatuses != null)
            {
               resultSelect = AddSqlWhere(resultSelect, GetSqlInClause<StatutoryServiceGeneralDescriptionVersioned>("SelectedPublishingStatuses", "PublishingStatusId"));
            }

            resultSelect = resultSelect + GetSqlEntityOrder<StatutoryServiceGeneralDescriptionVersioned>();
            
            #endregion SearchByFilterParam

            return resultSelect;
        }
        private string GetSqlGeneralDescriptionSelect()
        {
            return "Select DISTINCT ON (1) " +
                   GetSqlEntityColumns<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceName>() +
                   "NULL::uuid AS "+ GetSqlName("OrganizationId",", ") +
                   "'GeneralDescription' AS " + GetSqlName("EntityType",", ") +
                   "'GeneralDescription' AS " + GetSqlName("SubEntityType",", ") +
                   "0 AS "+ GetSqlName("nametype") +
                   "from " + GetSqlName("StatutoryServiceGeneralDescriptionVersioned", " ") +
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
        
        private void FillGeneralDescriptionsInformation(List<VmEntityListItem> search, IUnitOfWork unitOfWork, ICollection<Guid> languagesIds)
        {
            var vmEntityListItems = search.Where(x => x.EntityType == SearchEntityTypeEnum.GeneralDescription).ToList();
            var entitiesIds = vmEntityListItems.Select(x => x.Id).ToList();
            if (entitiesIds.Any())
            {
                var names = GetGeneralDescriptionsNames(entitiesIds, unitOfWork, languagesIds);
                var languages = GetGeneralDescriptionsLanguageAvailability(entitiesIds, unitOfWork);
                vmEntityListItems.ForEach(x=>
                {
                    x.Name = names.TryGetOrDefault(x.Id, new Dictionary<string, string>());
                    x.LanguagesAvailabilities =
                        TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            languages.TryGetOrDefault(x.Id, new List<GeneralDescriptionLanguageAvailability>()));
                });
            }            
        }
        private Dictionary<Guid, List<GeneralDescriptionLanguageAvailability>> GetGeneralDescriptionsLanguageAvailability(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork)
        {
            var langAvailabilitiesRep = unitOfWork.CreateRepository<IGeneralDescriptionLanguageAvailabilityRepository>();
            return langAvailabilitiesRep.All().Where(x => entitiesIds.Contains(x.StatutoryServiceGeneralDescriptionVersionedId)).ToList()
                .GroupBy(i => i.StatutoryServiceGeneralDescriptionVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());

        }
        private Dictionary<Guid, Dictionary<string, string>> GetGeneralDescriptionsNames(ICollection<Guid> entitiesIds, IUnitOfWork unitOfWork, ICollection<Guid> languagesIds)
        {
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var nameRep = unitOfWork.CreateRepository<IStatutoryServiceNameRepository>();

            return nameRep.All()
                .Where(x => entitiesIds.Contains(x.StatutoryServiceGeneralDescriptionVersionedId) &&
                            ((languagesIds != null && languagesIds.Contains(x.LocalizationId) &&
                              x.TypeId == nameTypeId) || x.TypeId == nameTypeId))
                .OrderBy(i => i.Localization.OrderNumber)
                .Select(i => new {i.StatutoryServiceGeneralDescriptionVersionedId, i.Name, i.LocalizationId}).ToList()
                .GroupBy(i => i.StatutoryServiceGeneralDescriptionVersionedId)
                .ToDictionary(i => i.Key,
                    i => i.ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));
        }
     
        #endregion  
        
        #region SQL creation 

        private const string TEMPNAME = "tempname";
        private string GetSqlEntityOrder<TEntity>(string additionGroupBy = "")
        {
            var entityName = typeof(TEntity).Name;
            var addGroupBy = !string.IsNullOrEmpty(additionGroupBy) ? ", "+ GetSqlName(additionGroupBy) : "";
            return
                " GROUP BY " + 
                GetSqlName(TEMPNAME, ", ") + 
                GetSqlName("nametype", ", ") +
                GetSqlName(entityName+".Id", ", ") + 
                GetSqlName("Versioning.VersionMajor",", ") +
                GetSqlName("Versioning.VersionMinor") + 
                addGroupBy +
                " ORDER BY " + GetSqlName(entityName+".Id", ", min, nametype");
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
        /// Create IN clause for potggress DB
        /// </summary>
        /// <param name="collectionName">Name of parameter collection</param>
        /// <param name="column">Column name</param>
        /// <typeparam name="TEntity">Table entity</typeparam>
        /// <returns>In clause</returns>
        /// <example>
        /// TEntity = Service
        /// collectionName = "filteredIds"
        /// column = "Id"
        /// result -> "Service"."Id" = ANY(@collectionName) 
        /// </example>
        private string GetSqlInClause<TEntity>(string collectionName, string column)
        {
            var entityName = typeof(TEntity).Name;
            return GetSqlInClause(collectionName, entityName + "." + column);               
        }
        /// <summary>
        /// Create IN clause for potggress DB
        /// </summary>
        /// <param name="collectionName">Name of parameter collection</param>
        /// <param name="column">Column name</param>
        /// <returns>In clause</returns>
        /// <example>
        /// collectionName = "filteredIds"
        /// column = "Id"
        /// result -> "Id" = ANY(@collectionName) 
        /// </example>
        private string GetSqlInClause(string collectionName, string column)
        {
            return GetSqlName(column) + 
                   "= ANY(@"+ collectionName+") ";
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
            return GetSqlName(entity + ".Id", ", ") +
                   "TRIM(" +GetSqlName(entityName + ".Name", ") AS " + TEMPNAME + ", ") +
                   "MIN(" + GetSqlName("Language.OrderNumber") + ") AS min, " +
                   "extract(epoch from " + GetSqlName(entity + ".Modified", ") * 1000 AS ") + GetSqlName("Modified",", ") +
                   GetSqlName(entity + ".ModifiedBy", ", ") +
                   GetSqlName(entity + ".PublishingStatusId", ", ") +
                   GetSqlName(entity + ".UnificRootId", ", ") +
                   GetSqlName("Versioning.VersionMajor", ", ") +
                   GetSqlName("Versioning.VersionMinor", ", ");
        }

        private string GetSqlSorting(List<VmSortParam> sortParams, VmSortParam defaultSortParam = null)
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
                sortParams.Add(new VmSortParam { Column = "SubEntityType", SortDirection = SortDirectionEnum.Asc});
            }
            
            var sort = sortParams.Union(versionSortingParams);
            sort.ForEach(param =>
            {
                orderBuilder.Append(GetSqlName(param.Column[0].ToString().ToUpper() + param.Column.Substring(1), " "))
                    .Append(param.SortDirection)
                    .Append(",");
            });

            var orderString = orderBuilder
                .Replace("Name", TEMPNAME)
                .ToString();
            return orderString.Remove(orderString.LastIndexOf(','), 1);;
        }

        private string GetSqlAllEntityOrderSelect(string sqlQuery, List<VmSortParam> sortParams, VmSortParam defaultSortParam = null)
        {
            return "Select " +
               GetSqlName("Id",", ") +
               GetSqlName(TEMPNAME, ", ") +
               GetSqlName("OrganizationId",", ") +
               GetSqlName("EntityType",", ") +
               GetSqlName("Modified",", ") +
               GetSqlName("ModifiedBy",", ") +
               GetSqlName("SubEntityType",", ") +
               GetSqlName("PublishingStatusId",", ") +
               GetSqlName("UnificRootId",", ") +
               GetSqlName("VersionMajor",", ") +
               GetSqlName("VersionMinor") +
            " From (" + sqlQuery + ") orderedresult " +
            " ORDER BY " + GetSqlSorting(sortParams, defaultSortParam);
        }
        private string GetSqlAllEntityPageSelect(string sqlQuery, int pageSize = CoreConstants.MaximumNumberOfAllItems, int page = 1)
        {
            return "Select " +
                   GetSqlName("Id", ", ") +
                   GetSqlName("OrganizationId", ", ") +
                   GetSqlName("EntityType", ", ") +
                   GetSqlName("Modified", ", ") +
                   GetSqlName("ModifiedBy", ", ") +
                   GetSqlName("SubEntityType", ", ") +
                   GetSqlName("PublishingStatusId", ", ") +
                   GetSqlName("UnificRootId") +
                   " From (" + sqlQuery + ") pageresult " +
                   " LIMIT " + pageSize +
                   " OFFSET " + (pageSize * page);
        }
        private string GetSqlCountEntitySelect(string sqlQuery)
        {
            return "Select COUNT(*) From (" + sqlQuery + ") result";
        }
        private string AddSqlToUnion(string sqlUnion, string sqlQuery)
        {
            return sqlUnion.Length > 0 ? sqlUnion + " UNION(" + sqlQuery + ")" : "(" + sqlQuery + ")";
        }

        #endregion
    }
}
