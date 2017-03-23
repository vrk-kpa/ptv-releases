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
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PTV.Domain.Model.Models;
using System.Linq.Expressions;
using PTV.Domain.Logic.Channels;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Interfaces;
using PTV.Database.DataAccess.Caches;
using Microsoft.AspNetCore.Http;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof (IOrganizationService), RegisterType.Transient)]
    [DataProviderServiceNameAttribute("Organizations")]
    internal class OrganizationService : ServiceBase, IOrganizationService, IDataProviderService
    {
        private readonly IContextManager contextManager;
        private ILogger logger;
        private DataUtils dataUtils;
        private ServiceUtilities utilities;
        private ICommonServiceInternal commonService;
        private OrganizationLogic organizationLogic;
        private ITypesCache typesCache;
        private IAddressService addressService;
        private ILanguageCache languageCache;
        private IVersioningManager versioningManager;

        public OrganizationService(IContextManager contextManager,
                                   ITranslationEntity translationEntToVm,
                                   ITranslationViewModel translationVmtoEnt,
                                   ILogger<OrganizationService> logger,
                                   OrganizationLogic organizationLogic,
                                   ServiceUtilities utilities,
                                   DataUtils dataUtils,
                                   ICommonServiceInternal commonService,
                                   IHttpContextAccessor ctxAccessor,
                                   IAddressService addressService,
                                   IPublishingStatusCache publishingStatusCache,
                                   ILanguageCache languageCache,
                                   IVersioningManager versioningManager,
                                   IUserOrganizationChecker userOrganizationChecker,
                                   ITypesCache typesCache)
            : base(translationEntToVm, translationVmtoEnt, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.commonService = commonService;
            this.organizationLogic = organizationLogic;
            this.dataUtils = dataUtils;
            this.addressService = addressService;
            this.languageCache = languageCache;
            this.versioningManager = versioningManager;
            this.typesCache = typesCache;
            // ITypesCache cannot be injected in constructor because it uses internal access modifier
            // get the typescache from requestservices (IServiceProvider) basically using Locator pattern here
            //typesCache = ctxAccessor.HttpContext.RequestServices.GetService(typeof(ITypesCache)) as ITypesCache;
        }

        public void TestMethod()
        {
            contextManager.ExecuteReader(unitOfWork =>
            {
                //var rep = unitOfWork.CreateRepository<IOrganizationRepository>();
                //var all = rep.All();
                //var res = translationEntToVm.TranslateAll<IVmOrganization>(all);
            });
        }

        public IVmOpenApiGuidPageVersionBase GetOrganizations(DateTime? date, int pageNumber = 1, int pageSize = 1000)
        {
            return GetOrganizations(new VmOpenApiGuidPage(pageNumber, pageSize), date);
        }
        public IVmOpenApiGuidPageVersionBase V3GetOrganizations(DateTime? date, int pageNumber = 1, int pageSize = 1000)
        {
            return GetOrganizations(new V3VmOpenApiGuidPage(pageNumber, pageSize), date);
        }

        private IVmOpenApiGuidPageVersionBase GetOrganizations(IVmOpenApiGuidPageVersionBase vm, DateTime? date)
        {
            contextManager.ExecuteReader(unitOfWork =>
            {
                var filters = new List<Expression<Func<OrganizationVersioned, bool>>>() { PublishedFilter<OrganizationVersioned>(), ValidityFilter<OrganizationVersioned>() };
                SetItemPage(vm, date, unitOfWork, filters, q => q.Include(i => i.OrganizationNames));
            });

            return vm;
        }

        public IVmOpenApiOrganizationVersionBase GetOrganizationById(Guid id, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiOrganizationVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetOrganizationById(unitOfWork, id, openApiVersion, getOnlyPublished);
            });

            return result;
        }

        private IVmOpenApiOrganizationVersionBase GetOrganizationById(IUnitOfWork unitOfWork, Guid id, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiOrganizationVersionBase result = null;
            try
            {
                // Get the right version id
                Guid? entityId = null;
                if (getOnlyPublished)
                {
                    entityId = versioningManager.GetVersionId<OrganizationVersioned>(unitOfWork, id, PublishingStatus.Published);
                }
                else
                {
                    entityId = versioningManager.GetVersionId<OrganizationVersioned>(unitOfWork, id);
                }
                if (entityId.IsAssigned())
                {
                    result = GetOrganizationWithDetails(unitOfWork, entityId.Value, openApiVersion, getOnlyPublished);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting an organization with id {0}. {1}", id, ex.Message);
                logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
            return result;
        }

        public IList<IVmOpenApiOrganizationVersionBase> GetOrganizationsByBusinessCode(string code, int openApiVersion)
        {
            try
            {
                IList<IVmOpenApiOrganizationVersionBase> results = new List<IVmOpenApiOrganizationVersionBase>();

                //Expression<Func<Organization, bool>> filter = o => o.Business.Code != null && o.Business.Code.Equals(code);
                //return GetOrganizationsWithDetails(filter);

                // Performance fix, replace above code. Locally the above code executed 1300ms and now in ~150ms (excluding the first call "warm up")
                // in training env it took 9 to 10 seconds
                // the query is very slow when using navigation property to filter
                // first get list of organization ids and use those to fetch the information
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var guidList = new List<Guid>();
                    var bidRepo = unitOfWork.CreateRepository<IBusinessRepository>();
                    var businessIds = bidRepo.All().Where(bid => bid.Code != null && bid.Code.Equals(code)).Select(b => b.Id).ToList();

                    if (businessIds.Count > 0)
                    {
                        var orgRepo = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                        // Get organization versions that have the defined business code and which are published.
                        // the contains is evaluated in client because EF doesn't currently handle the nullable type
                        // https://github.com/aspnet/EntityFramework/issues/4114 (closed, we use this same solution .HasValue but still evaluated locally)
                        // https://github.com/aspnet/EntityFramework/issues/4247 relates to the previous and is currently labeled as enhancement
                        guidList = orgRepo.All().Where(o => o.BusinessId.HasValue && businessIds.Contains(o.BusinessId.Value)).Where(PublishedFilter<OrganizationVersioned>()).Where(ValidityFilter<OrganizationVersioned>()).Select(o => o.Id).ToList();
                    }
                    if (guidList.Count > 0)
                    {
                        results = GetOrganizationsWithDetails(unitOfWork, guidList, openApiVersion);
                    }
                });

                return results;
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting an organization with code {0}. {1}", code, ex.Message);
                logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
        }

        public IVmOpenApiOrganizationVersionBase GetOrganizationByOid(string oid, int openApiVersion)
        {
            try
            {
                IVmOpenApiOrganizationVersionBase result = null;

                contextManager.ExecuteReader(unitOfWork =>
                {
                    var orgRepo = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                    var guidList = orgRepo.All().Where(o => o.Oid.Equals(oid)).Where(PublishedFilter<OrganizationVersioned>()).Where(ValidityFilter<OrganizationVersioned>()).Select(o => o.Id).ToList();
                    if (guidList.Count > 0)
                    {
                        result = GetOrganizationWithDetails(unitOfWork, guidList.FirstOrDefault(), openApiVersion);
                    }
                });
                return result;
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting an organization with Oid {0}. {1}", oid, ex.Message);
                logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
        }

        public Guid GetOrganizationIdByOid(string oid)
        {
            var guid = Guid.Empty;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                var org = orgRep.All().FirstOrDefault(o => o.Oid.Equals(oid));
                if (org != null)
                {
                    guid = org.UnificRootId;
                }
            });
            return guid;
        }

        public Guid GetOrganizationIdBySource(string sourceId)
        {
            var guid = Guid.Empty;
            var userId = utilities.GetRelationIdForExternalSource();
            contextManager.ExecuteReader(unitOfWork =>
            {
                guid = GetPTVId<Organization>(sourceId, userId, unitOfWork);
            });
            return guid;
        }

        public IVmOpenApiOrganizationVersionBase GetOrganizationBySource(string sourceId, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiOrganizationVersionBase result = null;
            var userId = utilities.GetRelationIdForExternalSource();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var rootId = GetPTVId<Organization>(sourceId, userId, unitOfWork);
                result = GetOrganizationById(unitOfWork, rootId, openApiVersion, getOnlyPublished);
            });
            return result;
        }

        public IVmOpenApiOrganizationVersionBase AddOrganization(IVmOpenApiOrganizationInVersionBase vm, bool allowAnonymous, int openApiVersion)
        {
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = utilities.GetRelationIdForExternalSource();
            var organization = new OrganizationVersioned();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Check if the external source already exists
                if (ExternalSourceExists<Organization>(vm.SourceId, userId, unitOfWork))
                {
                    throw new Exception(string.Format(CoreMessages.OpenApi.ExternalSourceExists, vm.SourceId));
                }

                organization = TranslationManagerToEntity.Translate<IVmOpenApiOrganizationInVersionBase, OrganizationVersioned>(vm, unitOfWork);

                var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                organizationRep.Add(organization);

                // Create the mapping between external source id and PTV id
                if (!string.IsNullOrEmpty(vm.SourceId))
                {
                    SetExternalSource(organization.UnificRoot, vm.SourceId, userId, unitOfWork);
                }

                unitOfWork.Save(saveMode);
            });

            // Update the map coordinates for addresses
            if (organization?.OrganizationAddresses?.Count > 0)
            {
                var addresses = organization.OrganizationAddresses.Select(x => x.AddressId);
                addressService.UpdateAddress(addresses.ToList());
            }

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = commonService.PublishAllAvailableLanguageVersions<OrganizationVersioned, OrganizationLanguageAvailability>(organization.Id, i => i.OrganizationVersionedId == organization.Id);
            }

            return GetOrganizationWithDetails(organization.Id, openApiVersion, false);
        }

        public bool OrganizationExists(Guid id)
        {
            var result = false;

            contextManager.ExecuteReader(unitOfWork =>
            {
                var organizationRep = unitOfWork.CreateRepository<IOrganizationRepository>();
                if (organizationRep.All().FirstOrDefault(o => o.Id.Equals(id)) != null)
                {
                    result = true;
                }
            });
            return result;
        }

        public IVmListItemsData<IVmListItem> GetOrganizations(string searchText)
        {
            IReadOnlyList<VmListItem> result = new List<VmListItem>();

            contextManager.ExecuteReader(unitOfWork =>
            {
                result = commonService.GetOrganizationNames(unitOfWork, searchText, false);
            });

            return new VmListItemsData<IVmListItem>(result);

        }

        public IVmGetOrganizationSearch GetOrganizationSearch()
        {
            // get enum values to local variables before using them inside LINQ queries as otherwise the provider does the queries in-memory and not in DB
            // as it cannot translate the enum values to db queries and this way we get better performance as the queries are evaluated in DB
            string statusDeletedCode = PublishingStatus.Deleted.ToString();
            string statusOldPublishedCode = PublishingStatus.OldPublished.ToString();

            var result = new VmGetOrganizationSearch();
            contextManager.ExecuteReader(unitOfWork =>
            {
				// PTV-866 requested by customer, end user are confused when searching with preselected organization
                //var userOrganization = utilities.GetUserOrganization(unitOfWork);


				// PTV-866 requested by customer, end user are confused when searching with preselected organization
                //result.OrganizationId = userOrganization?.Id

                var publishingStatuses = commonService.GetPublishingStatuses();

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("PublishingStatuses", publishingStatuses)
                );
                result.SelectedPublishingStatuses = publishingStatuses.Where(x => x.Code != statusDeletedCode && x.Code != statusOldPublishedCode).Select(x => x.Id).ToList();

            });

            return result;
        }

        private IVmOrganizationSearchResult FullTextSearchOrganizations(IVmOrganizationSearch vmOrganizationSearch, bool takeAll = false)
        {
            IReadOnlyList<IVmOrganizationListItem> result = new List<VmOrganizationListItem>();
            var skip = 0;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var organizationNamesRepository = unitOfWork.CreateRepository<IOrganizationNameRepository>();

                var languagesIds = vmOrganizationSearch.Languages.Select(language => languageCache.Get(language.ToString()));
                var organizationNames = organizationNamesRepository.All();
                // Fulltext filter //
                if (!string.IsNullOrEmpty(vmOrganizationSearch.Name))
                {
                    organizationNames = organizationNames
                        .Where(organizationName =>
                            organizationName.Name.ToLower().Contains(vmOrganizationSearch.Name.ToLower()) &&
                            languagesIds.Contains(organizationName.LocalizationId)
                        );
                }
                else
                {
                    organizationNames = organizationNames
                        .Where(organizationName =>
                            languagesIds.Contains(organizationName.LocalizationId)
                        );
                }

                organizationNames = unitOfWork.ApplyIncludes(organizationNames, q =>
                    q.Include(i => i.OrganizationVersioned)
                        .ThenInclude(i => i.OrganizationNames)
                        .ThenInclude(i => i.Type)
                    .Include(i => i.OrganizationVersioned)
                        .ThenInclude(i => i.Versioning)
                    .Include(i => i.OrganizationVersioned)
                        .ThenInclude(j => j.LanguageAvailabilities)
                        .ThenInclude(i => i.Language)
                    .Include(i => i.OrganizationVersioned)
                        .ThenInclude(i => i.Parent)
                        .ThenInclude(i => i.Versions)
                        .ThenInclude(i => i.OrganizationNames)
                        .ThenInclude(i => i.Type)
                    .Include(i => i.OrganizationVersioned)
                        .ThenInclude(i => i.UnificRoot)
                        .ThenInclude(i => i.Children)
                        .ThenInclude(i => i.OrganizationNames)
                        .ThenInclude(i => i.Type)
                );
                // Additional filters //
                if (vmOrganizationSearch.OrganizationId.HasValue)
                {
                    var organizationsRepository = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                    var allChildren = GetOrganizationsFlatten(
                        LoadOrganizationTree(organizationsRepository.All(), int.MaxValue, new List<Guid>() { vmOrganizationSearch.OrganizationId.Value }));
                    var allChildrenIds = allChildren.Select(i => i.Id).ToList();
                    organizationNames = organizationNames
                        .Where(x => allChildrenIds.Contains(x.OrganizationVersioned.Id) || x.OrganizationVersioned.UnificRootId == vmOrganizationSearch.OrganizationId.Value);
                }
                if (vmOrganizationSearch.SelectedPublishingStatuses != null)
                {
                    commonService.ExtendPublishingStatusesDeletedOldPublished(vmOrganizationSearch.SelectedPublishingStatuses);
                    organizationNames = organizationNames
                        .Where(x => vmOrganizationSearch.SelectedPublishingStatuses.Contains(x.OrganizationVersioned.PublishingStatusId));
                }

                var names = organizationNames
                    .Skip(vmOrganizationSearch.Skip)
                    .Take(CoreConstants.MaximumNumberOfAllItems * vmOrganizationSearch.Languages.Count)
                    .ToList();
                var organizations = names
                    .Select(x => x.OrganizationVersioned)
                    .GroupBy(x => x.Id)
                    .Select(x => x.First())
                    .Take(CoreConstants.MaximumNumberOfAllItems)
                    .ToList();

                organizations.ForEach(x => { x.OrganizationNames = x.OrganizationNames.Intersect(names).ToList(); });

                skip = organizations.Aggregate(0, (acc, organization) => organization.OrganizationNames.Count + acc);

                result = TranslationManagerToVm.TranslateAll<OrganizationVersioned, VmOrganizationSearchListItem>(organizations);
            });
            return new VmOrganizationSearchResult() {
                Organizations = result,
                Skip = skip
            };
        }
        private IVmOrganizationSearchResult FilteredSearchOrganizations(IVmOrganizationSearch vmOrganizationSearch, bool takeAll = false)
        {
            IReadOnlyList<IVmOrganizationListItem> result = new List<VmOrganizationListItem>();
            //var count = 0;

            contextManager.ExecuteReader(unitOfWork =>
            {
                var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                var resultTemp = orgRep.All();
                var languageCode = SetTranslatorLanguage(vmOrganizationSearch);
                if (vmOrganizationSearch.OrganizationId.HasValue)
                {
                    //var allChilds = GetAllChildrenFlatten(resultTemp, new List<Guid>() {vmOrganizationSearch.OrganizationId.Value});
                    var allChildren = GetOrganizationsFlatten(LoadOrganizationTree(resultTemp, int.MaxValue, new List<Guid>() { vmOrganizationSearch.OrganizationId.Value }));
                    var allChildrenIds = allChildren.Select(i => i.Id).ToList();
                    resultTemp = resultTemp.Where(x => allChildrenIds.Contains(x.Id) || x.UnificRootId == vmOrganizationSearch.OrganizationId.Value);
                }

                if (!string.IsNullOrEmpty(vmOrganizationSearch.Name))
                {
                    var searchText = vmOrganizationSearch.Name.ToLower();
                    resultTemp = resultTemp.Where(x => x.OrganizationNames.Any(y => y.LocalizationId == languageCache.Get(languageCode.ToString()) && y.TypeId == x.DisplayNameTypeId && y.Name.ToLower().Contains(searchText)));
                }
                else
                {
                    resultTemp =
                        resultTemp.Where(
                            x =>
                                x.OrganizationNames.Any(
                                    y =>
                                        y.LocalizationId == languageCache.Get(languageCode.ToString()) &&
                                        y.TypeId == x.DisplayNameTypeId &&
                                        !string.IsNullOrEmpty(y.Name)));
                }

                if (vmOrganizationSearch.SelectedPublishingStatuses != null)
                {
                    commonService.ExtendPublishingStatusesDeletedOldPublished(vmOrganizationSearch.SelectedPublishingStatuses);
                    resultTemp = resultTemp.WherePublishingStatusIn(vmOrganizationSearch.SelectedPublishingStatuses);
                }

                //count = resultTemp.Count();
                resultTemp = resultTemp.OrderBy(i => i.UnificRootId);

                resultTemp = !takeAll ? vmOrganizationSearch.PageNumber > 0
                    ? resultTemp.Skip(vmOrganizationSearch.PageNumber * CoreConstants.MaximumNumberOfAllItems).Take(CoreConstants.MaximumNumberOfAllItems)
                    : resultTemp.Take(CoreConstants.MaximumNumberOfAllItems) : resultTemp;

                resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                    q.Include(i => i.OrganizationNames).ThenInclude(i => i.Type)
                    .Include(x => x.Versioning)
                    .Include(j => j.LanguageAvailabilities).ThenInclude(i => i.Language)
                    .Include(i => i.Parent).ThenInclude(i => i.Versions).ThenInclude(i => i.OrganizationNames).ThenInclude(i => i.Type)
                    .Include(i => i.UnificRoot).ThenInclude(i => i.Children).ThenInclude(i => i.OrganizationNames).ThenInclude(i => i.Type));
                result = TranslationManagerToVm.TranslateAll<OrganizationVersioned, VmOrganizationListItem>(resultTemp);
            });

            return new VmOrganizationSearchResult() {
                Organizations = result,
                PageNumber = ++vmOrganizationSearch.PageNumber,
               // Count = count
            };
        }
        public IVmOrganizationSearchResult SearchOrganizations(IVmOrganizationSearch vmOrganizationSearch, bool takeAll = false)
        {
            vmOrganizationSearch.Name = vmOrganizationSearch.Name != null ? vmOrganizationSearch.Name.Trim() : vmOrganizationSearch.Name;
            var result = vmOrganizationSearch.Languages.Count > 1
                ? FullTextSearchOrganizations(vmOrganizationSearch, takeAll)
                : FilteredSearchOrganizations(vmOrganizationSearch, takeAll);
            return result;
        }

        public VmEntityNames GetOrganizationNames(VmEntityBase model)
        {
            var result = new VmEntityNames();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                var organization = unitOfWork.ApplyIncludes(organizationRep.All(), q =>
                    q.Include(i => i.OrganizationNames)
                        .Include(i => i.LanguageAvailabilities)).Single(x => x.Id == model.Id.Value);

                result = TranslationManagerToVm.Translate<OrganizationVersioned, VmEntityNames>(organization);

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages())
                );

            });
            return result;
        }

        public IVmOrganizationStep1 GetOrganizationStep1(IVmGetOrganizationStep model)
        {
            var result = new VmOrganizationStep1();
            contextManager.ExecuteReader(unitOfWork =>
            {
                TranslationManagerToVm.SetLanguage(model.Language);
                var organization = GetEntity<OrganizationVersioned>(model.OrganizationId, unitOfWork,
                    q => q
                        .Include(x => x.OrganizationNames)
                        .Include(x => x.OrganizationDescriptions)
                        .Include(x => x.PublishingStatus)
                        .Include(x => x.Business)
                        .Include(x => x.Municipality).ThenInclude(x => x.MunicipalityNames)
                        .Include(x => x.OrganizationEmails).ThenInclude(x => x.Email)
                        .Include(x => x.OrganizationPhones).ThenInclude(x => x.Phone).ThenInclude(x => x.PrefixNumber).ThenInclude(x => x.Country).ThenInclude(x => x.CountryNames)
                        .Include(x => x.OrganizationWebAddress).ThenInclude(x => x.WebPage)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.StreetNames)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.PostalCode).ThenInclude(x => x.PostalCodeNames)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressAdditionalInformations)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.Coordinates)
                        );

                result = GetModel<OrganizationVersioned, VmOrganizationStep1>(organization, unitOfWork);

                var organizationTypeRep = unitOfWork.CreateRepository<IOrganizationTypeRepository>();
                var orgTypes = CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<OrganizationType, OrganizationTypeName>(unitOfWork, organizationTypeRep.All())), x => x.Name);
                orgTypes.ForEach(x => x.IsDisabled = x.Children.Any());

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("OrganizationTypes", orgTypes),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("WebPageTypes", commonService.GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("Municipalities", commonService.GetMunicipalities(unitOfWork))
                );
            });

            return result;
        }

        public IVmOrganizationStep1 SaveOrganizationStep1(VmOrganizationModel model)
        {
            Guid? organizationId = null;
            OrganizationVersioned organizationVersioned = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                organizationLogic.PrefilterViewModel(model.Step1Form);
                var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                var typeNameRep = unitOfWork.CreateRepository<INameTypeRepository>();
                if (!string.IsNullOrEmpty(model.Step1Form.OrganizationId) && organizationRep.All().Any(x => (x.UnificRootId != model.Step1Form.UnificRootId) && (x.Oid == model.Step1Form.OrganizationId)))
                {
                    throw new ArgumentException("", model.Step1Form.OrganizationId);
                }

                if (typesCache.Get<OrganizationType>(OrganizationTypeEnum.TT1.ToString()) == model.Step1Form.OrganizationTypeId ||
                    typesCache.Get<OrganizationType>(OrganizationTypeEnum.TT2.ToString()) == model.Step1Form.OrganizationTypeId)
                {
                    if(organizationRep.All().Single(x => x.Id == model.Id).TypeId != model.Step1Form.OrganizationTypeId)
                    {
                        throw new PtvServiceArgumentException("Organization type is not allowed!", new List<string> { typesCache.GetByValue<OrganizationType>(model.Step1Form.OrganizationTypeId.Value) });
                    }

                }

                var nameCode = model.Step1Form.IsAlternateNameUsedAsDisplayName ? NameTypeEnum.AlternateName : NameTypeEnum.Name;
                model.Step1Form.DisplayNameId = typeNameRep.All().First(x => x.Code == nameCode.ToString()).Id;

                TranslationManagerToEntity.SetLanguage(model.Step1Form.Language);
                organizationVersioned = TranslationManagerToEntity.Translate<VmOrganizationModel, OrganizationVersioned>(model, unitOfWork);

                //Removing emails
                var emailRep = unitOfWork.CreateRepository<IOrganizationEmailRepository>();
                var emailIds = organizationVersioned.OrganizationEmails.Select(x => x.EmailId).ToList();
                var emailsToRemove = emailRep.All().Where(x => x.Email.Localization.Code == model.Step1Form.Language.ToString() && x.OrganizationVersionedId == organizationVersioned.Id && !emailIds.Contains(x.EmailId));
                emailsToRemove.ForEach(x => emailRep.Remove(x));

                //Removing phones
                var phoneRep = unitOfWork.CreateRepository<IOrganizationPhoneRepository>();
                var phoneIds = organizationVersioned.OrganizationPhones.Select(x => x.PhoneId).ToList();
                var phonesToRemove = phoneRep.All().Where(x => x.Phone.Localization.Code == model.Step1Form.Language.ToString() &&  x.OrganizationVersionedId == organizationVersioned.Id && !phoneIds.Contains(x.PhoneId));
                phonesToRemove.ForEach(x => phoneRep.Remove(x));

                //Removing webPages
                var webPageRep = unitOfWork.CreateRepository<IOrganizationWebPageRepository>();
                var wpIds = organizationVersioned.OrganizationWebAddress.Select(x => x.WebPage.Id).ToList();
                var webPagesToRemove = webPageRep.All().Where(x => x.WebPage.Localization.Code == model.Step1Form.Language.ToString() && x.OrganizationVersionedId == organizationVersioned.Id && !wpIds.Contains(x.WebPageId));
                webPagesToRemove.ForEach(x => webPageRep.Remove(x));

                //Removing Address
                var addressRep = unitOfWork.CreateRepository<IOrganizationAddressRepository>();
                var addressIds = organizationVersioned.OrganizationAddresses.Select(x => x.AddressId).ToList();
                var addressesToRemove = addressRep.All().Where(x => x.OrganizationVersionedId == organizationVersioned.Id && !addressIds.Contains(x.AddressId));
                addressesToRemove.ForEach(x => addressRep.Remove(x));

                unitOfWork.Save(parentEntity: organizationVersioned);
                organizationId = organizationVersioned.Id;

            });
            var addresses = organizationVersioned.OrganizationAddresses.Select(x => x.AddressId);
            addressService.UpdateAddress(addresses);
            return GetOrganizationStep1(new VmGetOrganizationStep { OrganizationId = organizationId, Language = model.Step1Form.Language });
        }

        public IVmEntityBase AddApiOrganization(VmOrganizationModel model)
        {
            OrganizationVersioned organizationVersioned = null;
            var result = new VmEnumEntityRootStatusBase();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                organizationVersioned = AddOrganization(unitOfWork, model);
                unitOfWork.Save();
                FillEnumEntities(result,
                  () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork).ToList())
              );
            });

            var addresses = organizationVersioned.OrganizationAddresses.Select(x => x.AddressId);
            addressService.UpdateAddress(addresses);
            result.Id = organizationVersioned.Id;
            result.UnificRootId = organizationVersioned.UnificRootId;
            result.PublishingStatusId = commonService.GetDraftStatusId();
            return result;
        }

        private OrganizationVersioned AddOrganization(IUnitOfWorkWritable unitOfWork, VmOrganizationModel vm)
        {
            var typeNameRep = unitOfWork.CreateRepository<INameTypeRepository>();
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();

            if (!string.IsNullOrEmpty(vm.Step1Form.OrganizationId) && organizationRep.All().Any(x => x.Oid == vm.Step1Form.OrganizationId))
            {
                throw new ArgumentException("", vm.Step1Form.OrganizationId);
            }
            if (typesCache.Get<OrganizationType>(OrganizationTypeEnum.TT1.ToString()) == vm.Step1Form.OrganizationTypeId ||
                typesCache.Get<OrganizationType>(OrganizationTypeEnum.TT2.ToString()) == vm.Step1Form.OrganizationTypeId)
            {
                throw new PtvServiceArgumentException("Organization type is not allowed!", new List<string> { typesCache.GetByValue<OrganizationType>(vm.Step1Form.OrganizationTypeId.Value) });
            }

            vm.PublishingStatusId = commonService.GetDraftStatusId();
            var nameCode = vm.Step1Form.IsAlternateNameUsedAsDisplayName ? NameTypeEnum.AlternateName : NameTypeEnum.Name;
            vm.Step1Form.DisplayNameId = typeNameRep.All().First(x => x.Code == nameCode.ToString()).Id;
            organizationLogic.PrefilterViewModel(vm.Step1Form);
            TranslationManagerToEntity.SetLanguage(vm.Language);
            var organization = TranslationManagerToEntity.Translate<VmOrganizationModel, OrganizationVersioned>(vm, unitOfWork);
            organizationRep.Add(organization);
            return organization;
        }

        private IVmOpenApiOrganizationVersionBase GetOrganizationWithDetails(IUnitOfWork unitOfWork, Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            return GetOrganizationsWithDetails(unitOfWork, new List<Guid> { versionId }, openApiVersion, getOnlyPublished).FirstOrDefault();
        }

        private IVmOpenApiOrganizationVersionBase GetOrganizationWithDetails(Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiOrganizationVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetOrganizationsWithDetails(unitOfWork, new List<Guid> { versionId }, openApiVersion, getOnlyPublished).FirstOrDefault();
            });
            return result;
        }

        private IList<IVmOpenApiOrganizationVersionBase> GetOrganizationsWithDetails(IUnitOfWork unitOfWork, List<Guid> versionIdList, int openApiVersion, bool getOnlyPublished = true)
        {
            if (versionIdList.Count == 0) return new List<IVmOpenApiOrganizationVersionBase>();

            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var resultTemp = unitOfWork.ApplyIncludes(organizationRep.All().Where(o => versionIdList.Contains(o.Id)), q =>
                q.Include(i => i.Business)
                    .Include(i => i.Type)
                    .Include(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                    .Include(i => i.OrganizationEmails).ThenInclude(i => i.Email)
                    .Include(i => i.OrganizationNames)
                    .Include(i => i.UnificRoot).ThenInclude(i => i.OrganizationServices).ThenInclude(i => i.WebPages).ThenInclude(i => i.WebPage)
                    .Include(i => i.UnificRoot).ThenInclude(i => i.OrganizationServices).ThenInclude(i => i.AdditionalInformations)
                    .Include(i => i.UnificRoot).ThenInclude(i => i.OrganizationServices).ThenInclude(i => i.ServiceVersioned).ThenInclude(i => i.LanguageAvailabilities)
                    .Include(i => i.OrganizationDescriptions)
                    .Include(x => x.OrganizationPhones).ThenInclude(x => x.Phone).ThenInclude(i => i.PrefixNumber)
                    .Include(i => i.OrganizationWebAddress).ThenInclude(i => i.WebPage)
                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.StreetNames)
                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
                    .Include(i => i.PublishingStatus)
                    .Include(i => i.LanguageAvailabilities)
                ).ToList();

            // Filter out items that do not have language versions published!
            var organizations = getOnlyPublished ? resultTemp.Where(c => c.LanguageAvailabilities.Any(l => l.StatusId == publishedId)).ToList() : resultTemp.ToList();

            if (getOnlyPublished)
            {
                organizations.ForEach(
                    organization =>
                    {
                        // Filter out not published services
                        organization.UnificRoot.OrganizationServices =
                            organization.UnificRoot.OrganizationServices.Where(i => i.ServiceVersioned.PublishingStatusId == publishedId &&
                            i.ServiceVersioned.LanguageAvailabilities.Any(l => l.StatusId == publishedId)) // filter out items if no published language versions are available
                            .ToList();

                        // Filter out not published language versions
                        var notPublishedLanguageVersions = organization.LanguageAvailabilities.Where(i => i.StatusId != publishedId).Select(i => i.LanguageId).ToList();
                        if (notPublishedLanguageVersions.Count > 0)
                        {
                            organization.OrganizationEmails = organization.OrganizationEmails.Where(i => !notPublishedLanguageVersions.Contains(i.Email.LocalizationId)).ToList();
                            organization.OrganizationNames = organization.OrganizationNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                            organization.UnificRoot.OrganizationServices.ForEach(service =>
                            {
                                service.AdditionalInformations = service.AdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                                service.WebPages = service.WebPages.Where(i => !notPublishedLanguageVersions.Contains(i.WebPage.LocalizationId)).ToList();
                            });
                            organization.OrganizationDescriptions = organization.OrganizationDescriptions.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                            organization.OrganizationPhones = organization.OrganizationPhones.Where(i => !notPublishedLanguageVersions.Contains(i.Phone.LocalizationId)).ToList();
                            organization.OrganizationWebAddress = organization.OrganizationWebAddress.Where(i => !notPublishedLanguageVersions.Contains(i.WebPage.LocalizationId)).ToList();
                            organization.OrganizationAddresses.ForEach(address => {
                                address.Address.StreetNames = address.Address.StreetNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                                address.Address.AddressAdditionalInformations = address.Address.AddressAdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                            });
                        }
                    }
                );
            }

            var result = TranslationManagerToVm.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(organizations).ToList();

            if (result == null)
            {
                throw new Exception(CoreMessages.OpenApi.RecordNotFound);
            }

            // Get the right open api view model version
            IList<IVmOpenApiOrganizationVersionBase> vmList = new List<IVmOpenApiOrganizationVersionBase>();
            result.ForEach(org =>
            {
                vmList.Add(GetEntityByOpenApiVersion(org as IVmOpenApiOrganizationVersionBase, openApiVersion));
            });

            return vmList;
        }

        public IVmEntityBase GetOrganizationStatus(Guid? organizationId)
        {
            VmPublishingStatus result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = GetOrganizationStatus(unitOfWork, organizationId);
            });
            return new VmEntityStatusBase { PublishingStatusId = result.Id };
        }

        private VmPublishingStatus GetOrganizationStatus(IUnitOfWorkWritable unitOfWork, Guid? organizationId)
        {
            var serviceRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var service = serviceRep.All()
                            .Include(x => x.PublishingStatus)
                            .Single(x => x.Id == organizationId.Value);

            return TranslationManagerToVm.Translate<PublishingStatusType, VmPublishingStatus>(service.PublishingStatus);
        }


        public VmPublishingResultModel PublishOrganization(VmPublishingModel model)
        {
            Guid channelId = model.Id;
            var affected = commonService.PublishEntity<OrganizationVersioned, OrganizationLanguageAvailability>(model);
            var result = new VmPublishingResultModel()
            {
                Id = channelId,
                PublishingStatusId = affected.AffectedEntities.First(i => i.Id == channelId).PublishingStatusNew,
                LanguagesAvailabilities = model.LanguagesAvailabilities,
                Version = affected.Version
            };
            FillEnumEntities(result, () => GetEnumEntityCollectionModel("Services", affected.AffectedEntities.Select(i => new VmEntityStatusBase() { Id = i.Id, PublishingStatusId = i.PublishingStatusNew }).ToList<IVmBase>()));
            return result;
        }

        public IVmEntityBase DeleteOrganization(Guid? organizationId)
        {
            OrganizationVersioned result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = DeleteOrganization(unitOfWork, organizationId);
                unitOfWork.Save();
            });
            return new VmEntityStatusBase() { Id = result?.Id, PublishingStatusId = result?.PublishingStatusId };
        }

        private OrganizationVersioned DeleteOrganization(IUnitOfWorkWritable unitOfWork, Guid? id)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var organizationUserMapRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
            var userMaps = organizationUserMapRep.All();
            var organizationIsUsed =
                organizationRep.All()
                    .Where(x => x.Id == id)
                    .Any(
                        i =>
                            i.UnificRoot.OrganizationServices.Any(j => j.ServiceVersioned.PublishingStatusId != psDeleted && j.ServiceVersioned.PublishingStatusId != psOldPublished) ||
                            i.UnificRoot.OrganizationServiceChannelsVersioned.Any(j => j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished)||
                            userMaps.Any(k => k.OrganizationId == i.UnificRootId));
            if (organizationIsUsed)
            {
                throw new OrganizationNotDeleteInUseException();
            }
            var organization = organizationRep.All().SingleOrDefault(x => x.Id == id);
            organization.SafeCall(i => i.PublishingStatusId = psDeleted);
            return organization;
        }

        public IVmOpenApiOrganizationVersionBase SaveOrganization(IVmOpenApiOrganizationInVersionBase vm, bool allowAnonymous, int openApiVersion)
        {
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = utilities.GetRelationIdForExternalSource();
            var organization = new OrganizationVersioned();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Get the root id according to source id (if defined)
                var rootId = vm.Id ?? GetPTVId<Organization>(vm.SourceId, userId, unitOfWork);

                // Get right version id
                vm.Id = versioningManager.GetVersionId<OrganizationVersioned>(unitOfWork, rootId);

                if (vm.PublishingStatus == PublishingStatus.Deleted.ToString())
                {
                    organization = DeleteOrganization(unitOfWork, vm.Id);
                }
                else
                {
                    // Entity needs to be restored?
                    if (vm.CurrentPublishingStatus == PublishingStatus.Deleted.ToString())
                    {
                        if (vm.PublishingStatus == PublishingStatus.Modified.ToString() || vm.PublishingStatus == PublishingStatus.Published.ToString())
                        {
                            // We need to restore already archived item
                            var publishingResult = commonService.RestoreArchivedEntity<OrganizationVersioned>(unitOfWork, vm.Id.Value);
                        }
                    }

                    organization = TranslationManagerToEntity.Translate<IVmOpenApiOrganizationInVersionBase, OrganizationVersioned>(vm, unitOfWork);

                    if (vm.DeleteAllPhones || (vm.PhoneNumbers != null && vm.PhoneNumbers.Count > 0))
                    {
                        // Remove the phones that were not included in vm
                        organization.OrganizationPhones = UpdateOrganizationCollectionWithRemove<OrganizationPhone, Phone>(unitOfWork, organization.Id,
                            organization.OrganizationPhones, e => e.PhoneId);
                    }
                    if (vm.DeleteAllEmails || (vm.EmailAddresses != null && vm.EmailAddresses.Count > 0))
                    {
                        // Remove all emails that were not included in vm
                        organization.OrganizationEmails = UpdateOrganizationCollectionWithRemove<OrganizationEmail, Email>(unitOfWork, organization.Id,
                            organization.OrganizationEmails, e => e.EmailId);
                    }
                    if (vm.DeleteAllWebPages || (vm.WebPages != null && vm.WebPages.Count > 0))
                    {
                        // Remove all web pages that were not included in vm
                        organization.OrganizationWebAddress = UpdateOrganizationCollectionWithRemove<OrganizationWebPage, WebPage>(unitOfWork, organization.Id,
                            organization.OrganizationWebAddress, e => e.WebPageId);
                    }
                    if (vm.DeleteAllAddresses || (vm.Addresses != null && vm.Addresses.Count > 0))
                    {
                        // Remove all adresses that were not included in vm
                        organization.OrganizationAddresses = UpdateOrganizationCollectionWithRemove<OrganizationAddress, Address>(unitOfWork, organization.Id,
                            organization.OrganizationAddresses, e => e.AddressId);
                    }

                    // Update the mapping between external source id and PTV id
                    if (!string.IsNullOrEmpty(vm.SourceId))
                    {
                        UpdateExternalSource<Organization>(organization.UnificRootId, vm.SourceId, userId, unitOfWork);
                    }
                }

                unitOfWork.Save(saveMode, organization);
            });

            // Update the map coordinates for addresses
            if (vm.PublishingStatus != PublishingStatus.Deleted.ToString())
            {
                if (organization?.OrganizationAddresses?.Count > 0)
                {
                    var addresses = organization.OrganizationAddresses.Select(x => x.AddressId);
                    addressService.UpdateAddress(addresses.ToList());
                }
            }

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = commonService.PublishAllAvailableLanguageVersions<OrganizationVersioned, OrganizationLanguageAvailability>(organization.Id, i => i.OrganizationVersionedId == organization.Id);
            }

            return GetOrganizationWithDetails(organization.Id, openApiVersion, false);
        }

        private ICollection<T> UpdateOrganizationCollectionWithRemove<T, TEntity>(IUnitOfWorkWritable unitOfWork, Guid organizationId, ICollection<T> collection, Func<T, Guid> getSelectedIdFunc)
            where T : IOrganization  where TEntity : IEntityIdentifier
        {
            // Remove all organization related entities that were not included in collection
            var updatedEntities = collection.Select(getSelectedIdFunc).ToList();
            var rep = unitOfWork.CreateRepository<IRepository<T>>();
            var currentOrganizationEntities = rep.All().Where(e => e.OrganizationVersionedId == organizationId).Select(getSelectedIdFunc).ToList();
            var entityRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            currentOrganizationEntities.Where(e => !updatedEntities.Contains(e)).ForEach(e => {
                var entity = entityRep.All().FirstOrDefault(r => r.Id == e);
                if (entity != null)
                {
                    entityRep.Remove(entity);
                }
                });

            return collection;
        }

        public PublishingStatus? GetOrganizationStatusByRootId(Guid id)
        {
            PublishingStatus? result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = versioningManager.GetLatestVersionPublishingStatus<OrganizationVersioned>(unitOfWork, id);
            });

            return result;
        }

        public PublishingStatus? GetOrganizationStatusBySourceId(string sourceId)
        {
            PublishingStatus? result = null;
            bool externalSourceExists = false;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var id = GetPTVId<Organization>(sourceId, utilities.GetRelationIdForExternalSource(), unitOfWork);
                if (id != Guid.Empty)
                {
                    externalSourceExists = true;
                    result = versioningManager.GetLatestVersionPublishingStatus<OrganizationVersioned>(unitOfWork, id);
                }
            });
            if (!externalSourceExists) { throw new Exception(string.Format(CoreMessages.OpenApi.ExternalSourceNotExists, sourceId)); }
            return result;
        }

        #region Lock
        public IVmEntityBase LockOrganization(Guid id)
        {
            return utilities.LockEntityVersioned<OrganizationVersioned, Organization>(id);
        }

        public IVmEntityBase UnLockOrganization(Guid id)
        {
            return utilities.UnLockEntityVersioned<OrganizationVersioned, Organization>(id);
        }
        public IVmEntityBase IsOrganizationLocked(Guid id)
        {
            return utilities.CheckIsEntityLocked(id);
        }

        public IReadOnlyList<IVmBase> Get(IUnitOfWork unitOfWork)
        {
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var psPublished = PublishingStatusCache.Get(PublishingStatus.Published);
            var organizations = unitOfWork
                .ApplyIncludes(organizationRep.All().Where(x => x.PublishingStatusId == psPublished), query => query.Include(organization => organization.OrganizationNames));
            return CreateTree<VmTreeItem>(LoadOrganizationTree(organizations));
        }
        #endregion Lock
    }
}

