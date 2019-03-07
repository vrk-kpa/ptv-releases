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
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using System.Text.RegularExpressions;
using System.Diagnostics;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Database.DataAccess.Utils.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V10;
using PTV.Framework.Enums;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof (IOrganizationService), RegisterType.Transient)]
    [DataProviderServiceNameAttribute("Organizations")]
    internal class OrganizationService : ServiceBase, IOrganizationService, IDataProviderService
    {
        private readonly IContextManager contextManager;
        private ILogger logger;
        private DataUtils dataUtils;
        private IServiceUtilities utilities;
        private ICommonServiceInternal commonService;
        private OrganizationLogic organizationLogic;
        private ITypesCache typesCache;
        private IAddressService addressService;
        private ILanguageCache languageCache;
        private ILanguageOrderCache languageOrderCache;
        private IUserOrganizationService userOrganizationService;
        private IPahaTokenProcessor tokenProcessor;

        public OrganizationService(IContextManager contextManager,
                                   ITranslationEntity translationEntToVm,
                                   ITranslationViewModel translationVmtoEnt,
                                   ILogger<OrganizationService> logger,
                                   OrganizationLogic organizationLogic,
                                   IServiceUtilities utilities,
                                   DataUtils dataUtils,
                                   ICommonServiceInternal commonService,
                                   IAddressService addressService,
                                   IPublishingStatusCache publishingStatusCache,
                                   ILanguageCache languageCache,
                                   IVersioningManager versioningManager,
                                   IUserOrganizationChecker userOrganizationChecker,
                                   ITypesCache typesCache,
                                   ILanguageOrderCache languageOrderCache,
                                   IUserOrganizationService userOrganizationService,
                                   IPahaTokenProcessor tokenProcessor)
            : base(translationEntToVm, translationVmtoEnt, publishingStatusCache, userOrganizationChecker, versioningManager)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.commonService = commonService;
            this.organizationLogic = organizationLogic;
            this.dataUtils = dataUtils;
            this.addressService = addressService;
            this.languageCache = languageCache;
            this.typesCache = typesCache;
            this.languageOrderCache = languageOrderCache;
            this.userOrganizationService = userOrganizationService;
            this.tokenProcessor = tokenProcessor;
        }

        public IVmOpenApiGuidPageVersionBase<V8VmOpenApiOrganizationItem> GetOrganizations(DateTime? date, int openApiVersion, EntityStatusEnum status, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null)
        {
            var extendedStatus = (EntityStatusExtendedEnum)(int)status;
            var handler = new OrganizationGuidPageHandler(extendedStatus, date, dateBefore, PublishingStatusCache, pageNumber, pageSize);
            return GetPage(handler, extendedStatus == EntityStatusExtendedEnum.Published);
            }

        private Dictionary<Guid, List<OrganizationLanguageAvailability>> GetLanguageAvailabilities(IUnitOfWork unitOfWork, List<Guid> idList)
        {
            var langAvailabilitiesRep = unitOfWork.CreateRepository<IRepository<OrganizationLanguageAvailability>>();
            return langAvailabilitiesRep.All().Where(x => idList.Contains(x.OrganizationVersionedId)).ToList()
                .GroupBy(i => i.OrganizationVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());
        }

        private Dictionary<Guid, Dictionary<Guid, string>> GetNames(IUnitOfWork unitOfWork, List<Guid> idList, Dictionary<Guid, List<OrganizationLanguageAvailability>> languageAvailabilities = null)
        {
            var nameRep = unitOfWork.CreateRepository<IRepository<OrganizationName>>();
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var names = nameRep.All().Where(x => idList.Contains(x.OrganizationVersionedId) && (x.TypeId == nameTypeId)).OrderBy(i => i.Localization.OrderNumber)
                .Select(i => new { id = i.OrganizationVersionedId, i.Name, i.LocalizationId }).ToList().GroupBy(i => i.id)
                .ToDictionary(i => i.Key, i => i.ToDictionary(x => x.LocalizationId, x => x.Name));

            // Do we need to filter out unpublished service names? (PTV-3689)
            if (languageAvailabilities != null)
            {
                var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
                var publishedNames = new Dictionary<Guid, Dictionary<Guid, string>>();
                names.ForEach(name =>
                {
                    var publishedLanguageIds = languageAvailabilities.TryGetOrDefault(name.Key, new List<OrganizationLanguageAvailability>()).Where(l => l.StatusId == publishedId).Select(l => l.LanguageId).ToList();
                    publishedNames.Add(name.Key, name.Value.Where(n => publishedLanguageIds.Contains(n.Key)).ToDictionary(i => i.Key, i => i.Value));
                });
                return publishedNames;
            }

            return names;
        }

        private IVmOpenApiGuidPageVersionBase<TItemModel> GetPage<TItemModel>(IGuidPageWithNameHandlerBase<OrganizationVersioned, TItemModel> pageHandler, bool filterUnpublishedNames = true)
            where TItemModel : IVmOpenApiItem, new()
        {
            if (pageHandler.PageNumber <= 0) return pageHandler.GetModel();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var totalCount = pageHandler.Search(unitOfWork);
                if (totalCount > 0)
                {
                    pageHandler.Names = GetNames(unitOfWork, pageHandler.EntityIds, filterUnpublishedNames ? GetLanguageAvailabilities(unitOfWork, pageHandler.EntityIds) : null);
                }
            });

            return pageHandler.GetModel();
        }

        public IList<IVmOpenApiOrganizationVersionBase> GetOrganizations(List<Guid> idList, int openApiVersion)
        {
            if (idList == null || idList.Count == 0)
            {
                return null;
            }

            IList<IVmOpenApiOrganizationVersionBase> result = new List<IVmOpenApiOrganizationVersionBase>();
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    result = GetOrganizationsWithDetails(unitOfWork, c => idList.Contains(c.UnificRootId), openApiVersion);
                });
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting organizations. {0}", ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }

            return result;
        }
        public IVmOpenApiGuidPageVersionBase<VmOpenApiOrganizationSaha> GetOrganizationsSaha(DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null)
        {
            var handler = new OrganizationSahaGuidPageHandler(date, dateBefore, PublishingStatusCache, languageCache, typesCache, pageNumber, pageSize);

            if (pageNumber <= 0) return handler.GetModel();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var totalCount = handler.Search(unitOfWork);
            });

            return handler.GetModel();
        }

        public IVmOpenApiGuidPageVersionBase<V8VmOpenApiOrganizationItem> GetOrganizationsByMunicipality(Guid municipalityId, bool includeWholeCountry, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null)
        {
            var handler = new OrganizationByMunicipalityPageHandler(municipalityId, includeWholeCountry, typesCache, date, dateBefore, PublishingStatusCache, pageNumber, pageSize);
            return GetPage(handler);
        }

        public IVmOpenApiGuidPageVersionBase<V8VmOpenApiOrganizationItem> GetOrganizationsByArea(Guid areaId, bool includeWholeCountry, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null)
        {
            var handler = new OrganizationByAreaPageHandler(areaId, includeWholeCountry, typesCache, date, dateBefore, PublishingStatusCache, pageNumber, pageSize);
            return GetPage(handler);
        }

        public IVmOpenApiGuidPageVersionBase<VmOpenApiItem> GetOrganizationsHierarchy(DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null)
        {
            //// Measure
            //var watch = new Stopwatch();
            //logger.LogTrace("****************************************");
            //logger.LogTrace($"GetOrganizations starts.");
            //watch.Start();
            //// end measure
            ///
            var handler = new OrganizationsOrphanPageHandler(date, dateBefore, PublishingStatusCache, pageNumber, pageSize);
            return GetPage(handler);
            
            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Fetch from db: {watch.ElapsedMilliseconds} ms.");
            //// end measure
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

        public IVmOpenApiOrganizationSaha GetOrganizationSahaById(Guid id)
        {
            IVmOpenApiOrganizationSaha result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
                var query = organizationRep.All().Where(i => i.UnificRootId == id && (i.PublishingStatusId == psPublished || i.PublishingStatusId == psDeleted)).OrderByDescending(i => i.Modified);
                var resultTemp = unitOfWork.ApplyIncludes(query, q => q.Include(i => i.OrganizationNames).Include(i => i.UnificRoot).ThenInclude(i => i.SahaOrganizationInformations));
                result = TranslationManagerToVm.Translate<OrganizationVersioned, VmOpenApiOrganizationSaha>(resultTemp.FirstOrDefault(i => i.PublishingStatusId == psPublished) ?? resultTemp.FirstOrDefault(i => i.PublishingStatusId == psDeleted));
                if (result == null)
                {
                    throw new PtvAppException(CoreMessages.OpenApi.RecordNotFound);
                }
            });

            return result;
        }

        public IVmOpenApiOrganizationHierarchy GetOrganizationsHierarchy(Guid id)
        {
            IVmOpenApiOrganizationHierarchy result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                var organization = GetOrganizationWithParents(unitOfWork, organizationRep, id);
                var organizationVersioned = organization?.Versions?.FirstOrDefault();
                if (organizationVersioned != null)
                {
                    // Get all sub organizations
                    GetSubOrganizations(unitOfWork, organizationRep, new List<OrganizationVersioned> { organizationVersioned });

                    result = TranslationManagerToVm.Translate<OrganizationVersioned, VmOpenApiOrganizationHierarchy>(organizationVersioned);                    
                }
            });

            return result;
        }
        
        private List<OrganizationName> GetPublishedNames(OrganizationVersioned organization)
        {
            if (organization == null) return null;

            var publishedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var publishedLanguageIds = organization.LanguageAvailabilities.Where(l => l.StatusId == publishedStatusId).Select(l => l.LanguageId).ToList();
            return organization.OrganizationNames.Where(i => publishedLanguageIds.Contains(i.LocalizationId)).ToList();
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
                    entityId = VersioningManager.GetVersionId<OrganizationVersioned>(unitOfWork, id, PublishingStatus.Published);
                }
                else
                {
                    entityId = VersioningManager.GetVersionId<OrganizationVersioned>(unitOfWork, id, null, false);
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
                        results = GetOrganizationsWithDetails(unitOfWork, o => o.BusinessId.HasValue && businessIds.Contains(o.BusinessId.Value), openApiVersion);
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

        public Guid? GetOrganizationIdByBusinessCode(string businessCode)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                var orgId = orgRep.All().Where(o => o.Business.Code == businessCode).Select(i => i.UnificRootId).FirstOrDefault();
                if (!orgId.IsAssigned())
                {
                    return (Guid?)null;
                }
                return orgId;
            });
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

        public IVmOpenApiOrganizationVersionBase AddOrganization(IVmOpenApiOrganizationInVersionBase vm, int openApiVersion)
        {
            var saveMode = SaveMode.Normal;
            var userId = utilities.GetRelationIdForExternalSource();
            var organization = new OrganizationVersioned();

            try
            {
                contextManager.ExecuteWriter(unitOfWork =>
                {

                    // Check if the external source already exists
                    if (ExternalSourceExists<Organization>(vm.SourceId, userId, unitOfWork))
                    {
                        throw new Exception(string.Format(CoreMessages.OpenApi.ExternalSourceExists, vm.SourceId));
                    }

                    // check sote user rights (SFIPTV-692, Requirements for API (IN), Req 1)
                    CheckSoteUserRights(vm.OrganizationType);

                    // Set user name which is used within language availabilities and check the publishing status (SFIPTV-190)
                    vm.UserName = unitOfWork.GetUserNameForAuditing();
                    if (vm.ValidFrom.HasValue && vm.ValidFrom > DateTime.Now)
                    {
                        // For timed publishing the version created needs to be set as draft
                        vm.PublishingStatus = PublishingStatus.Draft.ToString();
                    }

                    // Check address related municipalities
                    vm.Addresses.ForEach(a => CheckAddress(unitOfWork, a));

                    organization = TranslationManagerToEntity.Translate<IVmOpenApiOrganizationInVersionBase, OrganizationVersioned>(vm, unitOfWork);
                    HandleOrganizationSoteFocFilter(unitOfWork, organization);

                    var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                    organizationRep.Add(organization);

                    // Create the mapping between external source id and PTV id
                    if (!string.IsNullOrEmpty(vm.SourceId))
                    {
                        SetExternalSource(organization.UnificRoot, vm.SourceId, userId, unitOfWork);
                    }

                    commonService.AddHistoryMetaData<OrganizationVersioned, OrganizationLanguageAvailability>(organization, setByEntity:true);
                    unitOfWork.Save(saveMode);
                });

                // Update the map coordinates for addresses
                if (organization?.OrganizationAddresses?.Count > 0)
                {
                    // only for visiting addresses which are of type street
                    var visitingAddressId = typesCache.Get<AddressCharacter>(AddressCharacterEnum.Visiting.ToString());
                    var streetId = typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString());
                    var addresses = organization.OrganizationAddresses.Where(a => a.CharacterId == visitingAddressId && a.Address.TypeId == streetId).Select(x => x.AddressId);
                    addressService.UpdateAddress(addresses.ToList());
                }

                // Publish all language versions
                if (vm.PublishingStatus == PublishingStatus.Published.ToString())
                {
                    var publishingResult = commonService.PublishAllAvailableLanguageVersions<OrganizationVersioned, OrganizationLanguageAvailability>(organization.Id, i => i.OrganizationVersionedId == organization.Id);
                }

                return GetOrganizationWithDetails(organization.Id, openApiVersion, false);
            }
            catch (Exception ex)
            {
                logger.LogError("AddOrganization error: " + ex.Message);
                throw ex;
            }
        }

        private bool IsCyclicDependency(IUnitOfWork unitOfWork, Guid unificRootId, Guid? parentId)
        {
            if (parentId == null) return false;
            if (!unificRootId.IsAssigned() || !parentId.IsAssigned()) return false;
            if (unificRootId == parentId) return true;
            var filteredOutStatuses = new List<Guid>()
            {
                typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString()),
                typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString())
            };
            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var higherOrgs = orgRep.All().Where(i => !filteredOutStatuses.Contains(i.PublishingStatusId)).Where(i => i.UnificRootId == parentId.Value && i.ParentId != null).Select(i => i.ParentId.Value).Distinct().ToList();
            var allTree = higherOrgs.ToList();
            CyclicCheck(unitOfWork, higherOrgs, ref allTree, filteredOutStatuses);
            return allTree.Contains(unificRootId);
        }
        
        private void CyclicCheck(IUnitOfWork unitOfWork, List<Guid> orgs, ref List<Guid> allTree, List<Guid> filteredOutStatuses)
        {
            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var higherOrgs = orgRep.All().Where(i => !filteredOutStatuses.Contains(i.PublishingStatusId)).Where(i => orgs.Contains(i.UnificRootId) && i.ParentId != null).Select(i => i.ParentId.Value).Distinct().ToList();
            var toCheck = higherOrgs.Except(allTree).ToList();
            allTree.AddRange(toCheck);
            if (toCheck.Any())
            {
                CyclicCheck(unitOfWork, toCheck, ref allTree, filteredOutStatuses);
            }
        }

        private void GetSubOrganizations(IUnitOfWork unitOfWork, IOrganizationVersionedRepository orgRep, IList<OrganizationVersioned> organizations)
        {
            if (organizations == null || organizations.Count == 0) return;

            Guid publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            var orgIds = organizations.Select(i => i.UnificRootId).ToList();
            // Find all published sub organizations - also language availabilites has to be added to be able to fetch only published names (PTV-3689)
            var subOrgQuery = orgRep.All().Where(ov => ov.ParentId != null && orgIds.Contains(ov.ParentId.Value) && ov.PublishingStatusId == publishedId &&
                ov.LanguageAvailabilities.Any(ola => ola.StatusId == publishedId));
            var allSubOrganizations = unitOfWork.ApplyIncludes(subOrgQuery, q => q.Include(i => i.UnificRoot).Include(i => i.LanguageAvailabilities)).ToList();

            if (allSubOrganizations == null || allSubOrganizations.Count == 0)
            {
                return;
            }

            // Get all the sub organizations
            GetSubOrganizations(unitOfWork, orgRep, allSubOrganizations);
            
            // Get the names for organizations
            var subOrganizationIds = allSubOrganizations.Select(s => s.Id).ToList();
            var subOrganizationNames = unitOfWork.CreateRepository<IOrganizationNameRepository>().All().Where(i => subOrganizationIds.Contains(i.OrganizationVersionedId)).ToList()
                .GroupBy(i => i.OrganizationVersionedId).ToDictionary(i => i.Key, i => i.ToList());
            // Set the names for sub organizations
            allSubOrganizations.ForEach(s =>
            {
                // Filter out names that have not been published
                var names = subOrganizationNames.TryGet(s.Id);
                var publishedLanguages = s.LanguageAvailabilities.Where(la => la.StatusId == publishedId).Select(la => la.LanguageId).ToList();
                s.OrganizationNames = names.Where(n => publishedLanguages.Contains(n.LocalizationId)).ToList();
            });

            // Map the sub organization to right parent.            
            var subOrganizations = allSubOrganizations?.GroupBy(o => o.ParentId).ToDictionary(i => i.Key, i => i.ToList());
            organizations.ForEach(o => o.UnificRoot.Children = subOrganizations.TryGet(o.UnificRootId));
        }

        /// <summary>
        /// Get the organization with all the parent organizations attached.
        /// </summary>
        private Organization GetOrganizationWithParents(IUnitOfWork unitOfWork, IOrganizationVersionedRepository orgRep, Guid rootId)
        {
            // Get the right version id
            Guid? entityId = VersioningManager.GetVersionId<OrganizationVersioned>(unitOfWork, rootId, PublishingStatus.Published);

            if (!entityId.IsAssigned())
            {
                return null;
            }

            Guid publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            var organization = unitOfWork.ApplyIncludes(orgRep.All().Where(o => o.Id.Equals(entityId.Value)
                && o.PublishingStatusId == publishedId), q =>
                        q.Include(i => i.OrganizationNames)
                        .Include(i => i.LanguageAvailabilities) // Needs to be included to be able to filter out not published names.
                        .Include(i => i.UnificRoot)
                        ).FirstOrDefault();

            if (organization == null)
            {
                return null;
            }

            //Filter out not published names
            organization.OrganizationNames = GetPublishedNames(organization);

            // Find all parent organizations
            if (organization.ParentId.HasValue)
            {
                organization.Parent = GetOrganizationWithParents(unitOfWork, orgRep, organization.ParentId.Value);
            }

            return organization.UnificRoot;
        }

        /// <summary>
        /// Get the root organization id for organization (organizationId)
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        private Guid GetOrganizationRoodId(IUnitOfWork unitOfWork, Guid organizationId)
        {
            Guid publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var parent = orgRep.All().Where(i => i.UnificRootId == organizationId && i.PublishingStatusId == publishedId &&
                i.LanguageAvailabilities.Any(la => la.StatusId == publishedId)).Select(x => new
                {
                    x.UnificRootId,
                    x.ParentId,
                }).FirstOrDefault();
            if (parent == null)
            {
                return organizationId;
            }

            if (!parent.ParentId.HasValue)
            {
                return parent.UnificRootId;
            }

            return GetOrganizationRoodId(unitOfWork, parent.ParentId.Value);
        }

        /// <summary>
        /// Get root organization ids for all the organizations within 'rootOrganizations' dictionary.
        /// The key of the 'rootOrganizations' dictionary indicates the organization for which we are fetching the root id.
        /// The tuple includes the root/parent id and boolean value which indicates if the root is already found (true).
        /// Only handle items where the root id is not yet set.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="rootOrganizations"></param>
        private void GetOrganizationRoodIdsForOrganizations(IUnitOfWork unitOfWork, Dictionary<Guid, Tuple<Guid, bool>> rootOrganizations)
        {
            var notHandeledOrganizations = rootOrganizations.Where(x => x.Value.Item2 == false).ToList();
            if (notHandeledOrganizations == null || notHandeledOrganizations.Count == 0)
            {
                // All the organizations have now root organization set.
                return;
            }

            // Get the organizations from db.
            Guid publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var orgIds = notHandeledOrganizations.Select(x => x.Value.Item1).Distinct().ToList();
            var parents = orgRep.All().Where(i => orgIds.Contains(i.UnificRootId) && i.PublishingStatusId == publishedId &&
                i.LanguageAvailabilities.Any(la => la.StatusId == publishedId)).Select(x => new
                {
                    x.UnificRootId,
                    x.ParentId,
                }).ToList();
            
            notHandeledOrganizations.ForEach(entry =>
            {
                // Get the parent organization that was fetched from db.
                var parent = parents.Where(p => p.UnificRootId == entry.Value.Item1).FirstOrDefault();
                
                if (parent == null)
                {
                    // The root organization is already set correctly.
                    // The parent has probably been archived which is why it could not be found from published organizations.
                    // Only set the boolean value to true to indicate that the root was found.
                    var rootId = entry.Value.Item1;
                    rootOrganizations[entry.Key] = Tuple.Create(rootId, true);
                }
                else if (!parent.ParentId.HasValue)
                {
                    // The parent is the root
                    rootOrganizations[entry.Key] = Tuple.Create(parent.UnificRootId, true);
                }
                else
                {
                    // The parent has a parent defined.
                    // Set the root to point to parent id and leave the boolean value as false to indicate that this item needs further processing/checking.
                    rootOrganizations[entry.Key] = Tuple.Create(parent.ParentId.Value, false);
                }
            });

            GetOrganizationRoodIdsForOrganizations(unitOfWork, rootOrganizations);
        }

        private IVmOpenApiOrganizationVersionBase GetOrganizationWithDetails(IUnitOfWork unitOfWork, Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            //return GetOrganizationsWithDetails(unitOfWork, new List<Guid> { versionId }, openApiVersion, getOnlyPublished).FirstOrDefault();

            //// Measure
            //var watch = new Stopwatch();
            //logger.LogTrace("****************************************");
            //logger.LogTrace($"GetOrganizationWithDetails starts. Id: {versionId}");
            //watch.Start();
            //// end measure

            Guid publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var orgRepo = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var orgQuery = orgRepo.All().Where(ov => ov.Id == versionId);

            if (getOnlyPublished)
            {
                orgQuery = orgQuery.Where(ov => ov.LanguageAvailabilities.Any(ola => ola.StatusId == publishedId));
            }

            var organization = unitOfWork.ApplyIncludes(orgQuery, GetOrganizationIncludeChain()).FirstOrDefault();

            if (organization == null)
            {
                // organization not found
                return null;
            }

            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Fetch from db: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// end measure

            // Find all published sub organizations - also language availabilites has to be added to be able to fetch only published names (PTV-3689)
            var subOrgQuery = orgRepo.All().Where(ov => ov.ParentId == organization.UnificRootId && ov.PublishingStatusId == publishedId &&
                ov.LanguageAvailabilities.Any(ola => ola.StatusId == publishedId));
            var allSubOrganizations = unitOfWork.ApplyIncludes(subOrgQuery, q => q.Include(i => i.LanguageAvailabilities)).ToList();

            // set sub-organization names if there were any
            if (allSubOrganizations?.Count > 0)
            {
                var subOrganizationIds = allSubOrganizations.Select(s => s.Id).ToList();
                var subOrganizationNames = unitOfWork.CreateRepository<IOrganizationNameRepository>().All().Where(i => subOrganizationIds.Contains(i.OrganizationVersionedId)).ToList()
                    .GroupBy(i => i.OrganizationVersionedId).ToDictionary(i => i.Key, i => i.ToList());
                // Set the names for sub organizations
                allSubOrganizations.ForEach(s =>
                {
                    s.OrganizationNames = subOrganizationNames.TryGet(s.Id);
                });
            }

            // sub organizations with names
            var subOrganizations = allSubOrganizations?.GroupBy(o => o.ParentId).ToDictionary(i => i.Key, i => i.ToList());

            // Filter out not published language versions
            FilterOutNotPublishedLanguageVersions(organization, publishedId, getOnlyPublished);

            // Fill with sub organizations
            organization.UnificRoot.Children = subOrganizations.TryGet(organization.UnificRootId);

            // NOTE: Organization services are fetched after translation to improve performance with custom queries
            var tmpResult = TranslationManagerToVm.Translate<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(organization);

            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Sub organizations, filtering & translation: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// end measure

            // Get the sourceId if user is logged in
            var userId = utilities.GetRelationIdForExternalSource(false);
            if (!string.IsNullOrEmpty(userId))
            {
                tmpResult.SourceId = GetSourceId<Organization>(tmpResult.Id.Value, userId, unitOfWork);
            }

            // Get organizations services
            tmpResult.Services = GetOrganizationsServices(new List<Guid> { organization.UnificRootId }, unitOfWork);

            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Organization services: {watch.ElapsedMilliseconds} ms.");
            //// end measure

            // Get root organization identifier (PTV-4000)
            if (tmpResult.ParentOrganizationId.HasValue)
            {
                // Does the parent organization have any parent?
                var publishedParent = organization.Parent.Versions.FirstOrDefault(o => o.PublishingStatusId == publishedId);
                if (publishedParent != null && publishedParent.ParentId.HasValue)
                {
                    tmpResult.OrganizationRootId = GetOrganizationRoodId(unitOfWork, publishedParent.ParentId.Value);
                }
                else
                {
                    // The parent organization does not have any parent so it is the actual root organization
                    tmpResult.OrganizationRootId = publishedParent.UnificRootId;
                }
            }
            else
            {
                // The organization itself is root
                tmpResult.OrganizationRootId = tmpResult.Id;
            }
            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Organization services: {watch.ElapsedMilliseconds} ms.");
            //// end measure

            return GetEntityByOpenApiVersion(tmpResult as IVmOpenApiOrganizationVersionBase, openApiVersion);
        }

        private void FilterOutNotPublishedLanguageVersions(OrganizationVersioned organization, Guid publishedId, bool getOnlyPublished)
        {
            // Filter out not published language versions
            if (getOnlyPublished)
            {
                var notPublishedLanguageVersions = organization.LanguageAvailabilities.Where(i => i.StatusId != publishedId).Select(i => i.LanguageId).ToList();
                if (notPublishedLanguageVersions.Count > 0)
                {
                    organization.OrganizationEmails = organization.OrganizationEmails.Where(i => !notPublishedLanguageVersions.Contains(i.Email.LocalizationId)).ToList();
                    organization.OrganizationNames = organization.OrganizationNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    organization.OrganizationDescriptions = organization.OrganizationDescriptions.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    organization.OrganizationPhones = organization.OrganizationPhones.Where(i => !notPublishedLanguageVersions.Contains(i.Phone.LocalizationId)).ToList();
                    organization.OrganizationWebAddress = organization.OrganizationWebAddress.Where(i => !notPublishedLanguageVersions.Contains(i.WebPage.LocalizationId)).ToList();

                    organization.OrganizationEInvoicings.ForEach(invoicing =>
                    {
                        invoicing.EInvoicingAdditionalInformations = invoicing.EInvoicingAdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    });

                    organization.OrganizationAddresses.ForEach(address =>
                    {
                        address.Address.AddressStreets.ForEach(c =>
                        {
                            c.StreetNames = c.StreetNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        });
                        address.Address.AddressPostOfficeBoxes.ForEach(c =>
                        {
                            c.PostOfficeBoxNames = c.PostOfficeBoxNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        });
                        address.Address.AddressForeigns.ForEach(c =>
                        {
                            c.ForeignTextNames = c.ForeignTextNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        });
                        address.Address.AddressAdditionalInformations = address.Address.AddressAdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    });
                }
            }

            // Filter out not published parent organization
            if (organization.Parent?.Versions?.Count > 0)
            {
                if (!organization.Parent.Versions.Any(o => o.PublishingStatusId == publishedId))
                {
                    organization.ParentId = null;
                }
            }
        }

        private List<V10VmOpenApiOrganizationService> GetOrganizationsServices(List<Guid> organizationUnificRootIds, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            //// Measure
            //var watch = new Stopwatch();
            //logger.LogTrace($"GetOrganizationsServices starts.");
            //watch.Start();
            //// end measure

            Guid publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            // list for organizations services to return
            List<V10VmOpenApiOrganizationService> services = new List<V10VmOpenApiOrganizationService>(200);

            // Get organizations organizationservices (Default RoleType => OtherResponsible)
            var organizationServicesQuery = unitOfWork.CreateRepository<IOrganizationServiceRepository>().All()
            .Where(os => organizationUnificRootIds.Contains(os.OrganizationId)
            && os.ServiceVersioned.PublishingStatusId == publishedId
            && os.ServiceVersioned.LanguageAvailabilities.Any(l => l.StatusId == publishedId));

            var organizationServices = organizationServicesQuery.Select(x => new
            {
                OrganizationId = x.OrganizationId,
                ServiceId = x.ServiceVersioned.UnificRootId,
                ServiceVersionedId = x.ServiceVersioned.Id
            }).ToList();

            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Fetch from db - OrganizationServices: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// end measure

            // Get services where organization is main responsible (Default RoleType => Responsible)
            var responsibleServicesQuery = unitOfWork.CreateRepository<IServiceVersionedRepository>().All()
                .Where(sv => organizationUnificRootIds.Contains(sv.OrganizationId)
                && sv.PublishingStatusId == publishedId);

            // ToDictionary used for performance as we need to lookup entries with ServiceVersionedId
            var responsibleServices = responsibleServicesQuery.Select(x => new
            {
                OrganizationId = x.OrganizationId,
                ServiceId = x.UnificRootId,
                ServiceVersionedId = x.Id
            }).ToDictionary(a => a.ServiceVersionedId, a => a);

            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Fetch from db - Main responsible: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();

            // Get services where organisation is the producer (Default RoleType => Producer)
            var producerServicesQuery = unitOfWork.CreateRepository<IServiceProducerOrganizationRepository>().All()
                .Where(spo => organizationUnificRootIds.Contains(spo.OrganizationId)
                && spo.ServiceProducer.ServiceVersioned.PublishingStatusId == publishedId);

            var producerServices = producerServicesQuery.Select(x => new
            {
                OrganizationId = x.OrganizationId,
                ServiceId = x.ServiceProducer.ServiceVersioned.UnificRootId,
                ServiceVersionedId = x.ServiceProducer.ServiceVersioned.Id,
                ProvisionTypeId = x.ServiceProducer.ProvisionTypeId
            }).ToList();

            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Fetch from db - Producers: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();

            // Next fetch names for services (get all service ids and the use that list to fetch names in all languages)
            // (worst case is around 1500 IDs)
            var allServiceIds = organizationServices.Select(x => x.ServiceVersionedId).ToList();
            allServiceIds.AddWithDistinct(responsibleServices.Keys); // could be duplicates with organizationservices
            allServiceIds.AddRange(producerServices.Select(x => x.ServiceVersionedId));

            // Do we have any services
            if (allServiceIds.Count > 0)
            {
                // We need to take into account published language versions when getting names (PTV-3689).
                // Get published language availabilities
                var serviceLanguageAvailabilities = unitOfWork.CreateRepository<IServiceLanguageAvailabilityRepository>().All()
                    .Where(i => allServiceIds.Contains(i.ServiceVersionedId) && i.StatusId == publishedId)
                    .Select(sla => new VmOpenApiGuid
                    {
                        LocalizationId = sla.LanguageId,
                        OwnerReferenceId = sla.ServiceVersionedId,
                    })
                    .ToList()
                    .GroupBy(i => i.OwnerReferenceId).ToDictionary(i => i.Key, i => i.ToList());

                // Get names
                var serviceNames = unitOfWork.CreateRepository<IServiceNameRepository>().All()
                    .Where(i => allServiceIds.Contains(i.ServiceVersionedId))
                    .Select(sn => new VmName()
                    {
                        LocalizationId = sn.LocalizationId,
                        Name = sn.Name,
                        TypeId = sn.TypeId,
                        OwnerReferenceId = sn.ServiceVersionedId
                    })
                    .ToList() // materialize
                    .GroupBy(i => i.OwnerReferenceId).ToDictionary(i => i.Key, i => i.ToList());

                //// Measure
                //watch.Stop();
                //logger.LogTrace($"*** Fetch from db - service names: {watch.ElapsedMilliseconds} ms.");
                //watch.Restart();

                // Add the services to the list (first organization services then responsible services and last the producer)
                // Order does matter so that we don't need to do many lookups
                //
                // AdditionalInformation and WebPages are always empty lists these are remains from previous implementation (these fields might get removed in the future)

                // loop the organization services and create view model for, also check if the same service exists in responsibleServices
                // if so then change the RoleType and remove the matched service from responsibleServices
                organizationServices.ForEach(x =>
                {
                    var os = new V10VmOpenApiOrganizationService()
                    {
                        AdditionalInformation = new List<VmOpenApiLanguageItem>(),
                        OrganizationId = x.OrganizationId.ToString(),
                        RoleType = CommonConsts.OTHER_RESPONSIBLE,
                        Service = new VmOpenApiItem()
                        {
                            Id = x.ServiceId,
                            Name = GetServiceNameWithFallback(serviceNames.TryGet(x.ServiceVersionedId), serviceLanguageAvailabilities.TryGet(x.ServiceVersionedId))
                        },
                    };

                    // check if the same service is in responsibleServices
                    // responsibleServices is dictionary on purpose as the key lookup is much faster than using Where Linq
                    // as that would need to enumerate the whole collection every time
                    var match = responsibleServices.TryGet(x.ServiceVersionedId);

                    if (match != null)
                    {
                        os.RoleType = CommonConsts.RESPONSIBLE;
                        // remove the responsible service entry so that we don't add the service two times
                        responsibleServices.Remove(x.ServiceVersionedId);
                    }

                    services.Add(os);
                });

                // Services where organization is responsible
                responsibleServices.Values.ForEach(x =>
                {
                    var rs = new V10VmOpenApiOrganizationService()
                    {
                        AdditionalInformation = new List<VmOpenApiLanguageItem>(),
                        OrganizationId = x.OrganizationId.ToString(),
                        RoleType = CommonConsts.RESPONSIBLE,
                        Service = new VmOpenApiItem()
                        {
                            Id = x.ServiceId,
                            Name = GetServiceNameWithFallback(serviceNames.TryGet(x.ServiceVersionedId), serviceLanguageAvailabilities.TryGet(x.ServiceVersionedId))
                        },
                    };

                    services.Add(rs);
                });

                // Services where organization is producer
                producerServices.ForEach(x =>
                {
                    var ps = new V10VmOpenApiOrganizationService()
                    {
                        AdditionalInformation = new List<VmOpenApiLanguageItem>(),
                        OrganizationId = x.OrganizationId.ToString(),
                        ProvisionType = typesCache.GetByValue<ProvisionType>(x.ProvisionTypeId),
                        RoleType = CommonConsts.PRODUCER,
                        Service = new VmOpenApiItem()
                        {
                            Id = x.ServiceId,
                            Name = GetServiceNameWithFallback(serviceNames.TryGet(x.ServiceVersionedId), serviceLanguageAvailabilities.TryGet(x.ServiceVersionedId))
                        },
                    };

                    services.Add(ps);
                });

                //// Measure
                //watch.Stop();
                //logger.LogTrace($"*** Services mapped: {watch.ElapsedMilliseconds} ms.");
            }

            return services;
        }

        /// <summary>
        /// Tries to get the service name in the following order: FI, SV and then EN.
        /// Returns only published service names. (PTV-3689)
        /// </summary>
        /// <param name="serviceNames">List of service names</param>
        /// <param name="publishedLanguages">List of published languages</param>
        /// <returns>service name or null</returns>
        private string GetServiceNameWithFallback(ICollection<VmName> serviceNames, List<VmOpenApiGuid> publishedLanguages)
        {
            if (serviceNames == null || serviceNames.Count == 0)
            {
                return null;
            }

            if (publishedLanguages == null || publishedLanguages.Count == 0)
            {
                return null;
            }

            Guid nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var languageIds = publishedLanguages.Select(i => i.LocalizationId).ToList();

            string sname = null;

            // first try to get finnish name
            var fiStr = "fi";
            if (languageCache.AllowedLanguageCodes.Contains(fiStr))
            {
                var fiGuid = languageCache.Get(fiStr);
                if (languageIds.Contains(fiGuid))
                {
                    sname = GetServiceName(serviceNames, languageCache.Get(fiStr), nameTypeId);
                    // did we find FI name
                    if (!string.IsNullOrWhiteSpace(sname))
                    {
                        return sname;
                    }
                }
            }

            // try to get swedish name
            var svStr = "sv";
            if (languageCache.AllowedLanguageCodes.Contains(fiStr))
            {
                var svGuid = languageCache.Get(svStr);
                if (languageIds.Contains(svGuid))
                {
                    sname = GetServiceName(serviceNames, languageCache.Get(svStr), nameTypeId);
                    // did we find SV name
                    if (!string.IsNullOrWhiteSpace(sname))
                    {
                        return sname;
                    }
                }
            }

            // We have not yet found any name for item so let's take the first allowed language available.
            foreach (var allowedLangugageId in languageCache.AllowedLanguageIds)
            {
                if (!languageIds.Contains(allowedLangugageId)) continue;
                sname = GetServiceName(serviceNames, allowedLangugageId, nameTypeId);
                // did we find FI name
                if (!string.IsNullOrWhiteSpace(sname))
                {
                    return sname;
                }
            }

            return sname;
        }

        /// <summary>
        /// Get service name.
        /// </summary>
        /// <param name="serviceNames">List of service names</param>
        /// <param name="languageId">what language to get</param>
        /// <param name="nameTypeId">what type of name to get</param>
        /// <returns>service name or null</returns>
        private static string GetServiceName(ICollection<VmName> serviceNames, Guid languageId, Guid nameTypeId)
        {
            if (serviceNames == null || serviceNames.Count == 0)
            {
                return null;
            }

            return serviceNames.Where(sn => sn.LocalizationId == languageId && sn.TypeId == nameTypeId).FirstOrDefault()?.Name;
        }

        private IVmOpenApiOrganizationVersionBase GetOrganizationWithDetails(Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiOrganizationVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetOrganizationWithDetails(unitOfWork, versionId , openApiVersion, getOnlyPublished);
            });
            return result;
        }

//        private IList<IVmOpenApiOrganizationVersionBase> GetOrganizationsWithDetailsOld(IUnitOfWork unitOfWork, List<Guid> versionIdList, int openApiVersion, bool getOnlyPublished = true)
//        {
//            if (versionIdList.Count == 0) return new List<IVmOpenApiOrganizationVersionBase>();
//
//            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
//            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
//
//            var resultTemp = unitOfWork.ApplyIncludes(organizationRep.All().Where(o => versionIdList.Contains(o.Id)), q =>
//                q.Include(i => i.Business)
//                    .Include(i => i.Type)
//                    .Include(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
//                    .Include(i => i.OrganizationEmails).ThenInclude(i => i.Email)
//                    .Include(i => i.OrganizationNames)
//                    .Include(i => i.OrganizationDisplayNameTypes)
//                    .Include(i => i.UnificRoot).ThenInclude(i => i.OrganizationServices).ThenInclude(i => i.ServiceVersioned).ThenInclude(i => i.LanguageAvailabilities)
//                    .Include(i => i.UnificRoot).ThenInclude(i => i.OrganizationServicesVersioned)
//                    .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceProducerOrganizations).ThenInclude(i => i.ServiceProducer).ThenInclude(i => i.ServiceVersioned)
//                    .Include(i => i.OrganizationDescriptions)
//                    .Include(x => x.OrganizationPhones).ThenInclude(x => x.Phone).ThenInclude(i => i.PrefixNumber)
//                    .Include(i => i.OrganizationWebAddress).ThenInclude(i => i.WebPage)
//                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(x => x.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
//                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(x => x.AddressStreets).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
//                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(x => x.AddressStreets).ThenInclude(i => i.StreetNames)
//
//                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(x => x.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
//                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(x => x.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
//                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(x => x.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
//
//                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(x => x.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
//
//                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
//                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
//                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
//
//                    .Include(i => i.OrganizationEInvoicings).ThenInclude(i => i.EInvoicingAdditionalInformations)
//
//                    .Include(i => i.LanguageAvailabilities)
//                    .Include(i => i.OrganizationAreas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaNames)
//                    .Include(i => i.OrganizationAreas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
//                    .Include(i => i.OrganizationAreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
//                    .Include(i => i.Parent).ThenInclude(i => i.Versions)
//                ).ToList();
//
//            // Filter out items that do not have language versions published!
//            var organizations = getOnlyPublished ? resultTemp.Where(c => c.LanguageAvailabilities.Any(l => l.StatusId == publishedId)).ToList() : resultTemp.ToList();
//
//            // https://github.com/aspnet/EntityFrameworkCore/issues/7922
//            // converting Guids to nullable so that the follwing Contains is executed on server and not on client
//
//            // Find all sub organizations and names for them
//            //"Linq to entity, contains nullable guids fix"
//            var organizationRootIds = organizations.Select(o => o.UnificRootId).Cast<Guid?>().ToList();
//            var allSubOrganizations = organizationRep.All().Where(o => organizationRootIds.Contains(o.ParentId)
//                && o.PublishingStatusId == publishedId && o.LanguageAvailabilities.Any(l => l.StatusId == publishedId)).ToList();
//
//            var subOrganizationIds = allSubOrganizations.Select(s => s.Id).ToList();
//            var subOrganizationNames = unitOfWork.CreateRepository<IOrganizationNameRepository>().All().Where(i => subOrganizationIds.Contains(i.OrganizationVersionedId)).ToList()
//                .GroupBy(i => i.OrganizationVersionedId).ToDictionary(i => i.Key, i => i.ToList());
//            // Set the names for sub organizations
//            allSubOrganizations.ForEach(s =>
//            {
//                s.OrganizationNames = subOrganizationNames.TryGet(s.Id);
//            });
//
//            // sub organizations with names
//            var subOrganizations = allSubOrganizations.GroupBy(o => o.ParentId).ToDictionary(i => i.Key, i => i.ToList());
//
//            organizations.ForEach(
//                organization =>
//                {
//                    // Filter out not published services
//                    organization.UnificRoot.OrganizationServices =
//                        organization.UnificRoot.OrganizationServices.Where(i => i.ServiceVersioned.PublishingStatusId == publishedId &&
//                        i.ServiceVersioned.LanguageAvailabilities.Any(l => l.StatusId == publishedId)) // filter out items if no published language versions are available
//                        .ToList();
//
//                    // Filter out not published services (main responsible)
//                    organization.UnificRoot.OrganizationServicesVersioned = organization.UnificRoot.OrganizationServicesVersioned.Where(i => i.PublishingStatusId == publishedId).ToList();
//
//                    // Filter out not published services (organization as producer)
//                    organization.UnificRoot.ServiceProducerOrganizations = organization.UnificRoot.ServiceProducerOrganizations.Where(i => i.ServiceProducer.ServiceVersioned.PublishingStatusId == publishedId).ToList();
//
//                    // Filter out not published language versions
//                    if (getOnlyPublished)
//                    {
//                        var notPublishedLanguageVersions = organization.LanguageAvailabilities.Where(i => i.StatusId != publishedId).Select(i => i.LanguageId).ToList();
//                        if (notPublishedLanguageVersions.Count > 0)
//                        {
//                            organization.OrganizationEmails = organization.OrganizationEmails.Where(i => !notPublishedLanguageVersions.Contains(i.Email.LocalizationId)).ToList();
//                            organization.OrganizationNames = organization.OrganizationNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
//                            organization.OrganizationDescriptions = organization.OrganizationDescriptions.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
//                            organization.OrganizationPhones = organization.OrganizationPhones.Where(i => !notPublishedLanguageVersions.Contains(i.Phone.LocalizationId)).ToList();
//                            organization.OrganizationWebAddress = organization.OrganizationWebAddress.Where(i => !notPublishedLanguageVersions.Contains(i.WebPage.LocalizationId)).ToList();
//
//                            organization.OrganizationEInvoicings.ForEach(invoicing =>
//                            {
//                                invoicing.EInvoicingAdditionalInformations = invoicing.EInvoicingAdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
//                            });
//
//                            organization.OrganizationAddresses.ForEach(address =>
//                            {
//                                address.Address.AddressStreets.ForEach(c =>
//                                {
//                                    c.StreetNames = c.StreetNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
//                                });
//                                address.Address.AddressPostOfficeBoxes.ForEach(c =>
//                                {
//                                    c.PostOfficeBoxNames = c.PostOfficeBoxNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
//                                });
//                                address.Address.AddressForeigns.ForEach(c =>
//                                {
//                                    c.ForeignTextNames = c.ForeignTextNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
//                                });
//                                address.Address.AddressAdditionalInformations = address.Address.AddressAdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
//                            });
//                        }
//                    }
//
//                    // Filter out not published parent organization
//                    if (organization.Parent?.Versions?.Count > 0)
//                    {
//                        if (!organization.Parent.Versions.Any(o => o.PublishingStatusId == publishedId))
//                        {
//                            organization.ParentId = null;
//                        }
//                    }
//
//                    // Fill with sub organizations
//                    organization.UnificRoot.Children = subOrganizations.TryGet(organization.UnificRootId);
//
//                }
//            );
//
//            // Fill with service names
//            var allServices = organizations.SelectMany(i => i.UnificRoot.OrganizationServices).Select(i => i.ServiceVersioned).ToList();
//            allServices.AddRange(organizations.SelectMany(i => i.UnificRoot.OrganizationServicesVersioned).ToList());
//            allServices.AddRange(organizations.SelectMany(i => i.UnificRoot.ServiceProducerOrganizations).Select(i => i.ServiceProducer.ServiceVersioned).ToList());
//            var serviceIds = allServices.Select(i => i.Id).ToList();
//            var serviceNames = unitOfWork.CreateRepository<IServiceNameRepository>().All().Where(i => serviceIds.Contains(i.ServiceVersionedId)).ToList()
//                .GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.ToList());
//            allServices.ForEach(service =>
//            {
//                service.ServiceNames = serviceNames.TryGet(service.Id);
//            });
//
//            var result = TranslationManagerToVm.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(organizations).ToList();
//
//            if (result == null)
//            {
//                throw new Exception(CoreMessages.OpenApi.RecordNotFound);
//            }
//
//            // Get the right open api view model version
//            IList<IVmOpenApiOrganizationVersionBase> vmList = new List<IVmOpenApiOrganizationVersionBase>();
//            result.ForEach(org =>
//            {
//                // Get the sourceId if user is logged in
//                var userId = utilities.GetRelationIdForExternalSource(false);
//                if (!string.IsNullOrEmpty(userId))
//                {
//                    org.SourceId = GetSourceId<Organization>(org.Id.Value, userId, unitOfWork);
//                }
//                vmList.Add(GetEntityByOpenApiVersion(org as IVmOpenApiOrganizationVersionBase, openApiVersion));
//            });
//
//            return vmList;
//        }

        private IList<IVmOpenApiOrganizationVersionBase> GetOrganizationsWithDetails(IUnitOfWork unitOfWork, Expression<Func<OrganizationVersioned, bool>> filter, int openApiVersion, bool getOnlyPublished = true)
        {
            //// Measure
            //var watch = new Stopwatch();
            //logger.LogTrace("****************************************");
            //logger.LogTrace($"GetOrganizationsWithDetails starts.");
            //watch.Start();
            //// end measure
            var resultList = new List<VmOpenApiOrganizationVersionBase>();
            // Use dictionary to set the organizations' root id. 
            // The Key is the organization in question. Tuple includes the parent id (or the actual root) 
            // and the boolean value within Tuple indicates if the root has already been found (true).
            var organizationRoots = new Dictionary<Guid, Tuple<Guid, bool>>();
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var query = organizationRep.All().Where(filter);
            if (getOnlyPublished)
            {
                query = query.Where(c => c.PublishingStatusId == publishedId && c.LanguageAvailabilities.Any(l => l.StatusId == publishedId));
            }

            var totalCount = query.Count();
            if (totalCount > 100)
            {
                throw new Exception(CoreMessages.OpenApi.TooManyItems);
            }
            if (totalCount == 0)
            {
                return null;
            }

            var organizations = unitOfWork.ApplyIncludes(query, GetOrganizationIncludeChain()).ToList();
            List<Guid> rootIds = new List<Guid>();

            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Fetch from db: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// end measure

            if (organizations?.Count > 0)
            {
                // Find all published sub organizations
                rootIds = organizations.Select(o => o.UnificRootId).ToList();
                var allSubOrganizations = organizationRep.All().Where(ov => ov.ParentId != null && rootIds.Contains(ov.ParentId.Value) && ov.PublishingStatusId == publishedId && ov.LanguageAvailabilities.Any(ola => ola.StatusId == publishedId)).ToList();

                // set sub-organization names if there were any
                if (allSubOrganizations?.Count > 0)
                {
                    var subOrganizationIds = allSubOrganizations.Select(s => s.Id).ToList();
                    var subOrganizationNames = unitOfWork.CreateRepository<IOrganizationNameRepository>().All().Where(i => subOrganizationIds.Contains(i.OrganizationVersionedId)).ToList()
                        .GroupBy(i => i.OrganizationVersionedId).ToDictionary(i => i.Key, i => i.ToList());
                    // Set the names for sub organizations
                    allSubOrganizations.ForEach(s =>
                    {
                        s.OrganizationNames = subOrganizationNames.TryGet(s.Id);
                    });
                }
                
                organizations.ForEach(organization =>
                {
                    // Filter out not published language versions
                    FilterOutNotPublishedLanguageVersions(organization, publishedId, getOnlyPublished);

                    // Fill with sub organizations
                    // sub organizations with names
                    var subOrganizations = allSubOrganizations.GroupBy(o => o.ParentId.Value).ToDictionary(i => i.Key, i => i.ToList());
                    organization.UnificRoot.Children = subOrganizations.TryGet(organization.UnificRootId);

                    // Set organization root id (PTV-4000).
                    if (organization.ParentId.HasValue)
                    {
                        // Does the parent organization have any parent?
                        var publishedParent = organization.Parent.Versions.FirstOrDefault(o => o.PublishingStatusId == publishedId);
                        if (publishedParent != null && publishedParent.ParentId.HasValue)
                        {
                            organizationRoots.Add(organization.UnificRootId, Tuple.Create(publishedParent.ParentId.Value, false)); // We need to still process these ones later!
                        }
                        else
                        {
                            // The parent organization does not have any parent so this one is the actual root organization
                            organizationRoots.Add(organization.UnificRootId, Tuple.Create(publishedParent.UnificRootId, true));
                        }
                    }
                    else
                    {
                        // The organization itself is root
                        organizationRoots.Add(organization.UnificRootId, Tuple.Create(organization.UnificRootId, true));
                    }
                });

                resultList = TranslationManagerToVm.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(organizations).ToList();
            }

            // Set the root ids for organizations where it has not yet been set
            GetOrganizationRoodIdsForOrganizations(unitOfWork, organizationRoots);

            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Get sub organizations from db: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// End measure

            // organization services
            var allServices = GetOrganizationsServices(rootIds, unitOfWork);

            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Get connections from db: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// End measure

            // Get the right open api view model version and at the same time set the organization services.
            // Set also the organization root ids (PTV-4000).
            IList<IVmOpenApiOrganizationVersionBase> vmList = new List<IVmOpenApiOrganizationVersionBase>();
            resultList.ForEach(org =>
            {
                // Get the sourceId if user is logged in
                var userId = utilities.GetRelationIdForExternalSource(false);
                if (!string.IsNullOrEmpty(userId))
                {
                    org.SourceId = GetSourceId<Organization>(org.Id.Value, userId, unitOfWork);
                }

                // Set organization services
                var services = allServices.Where(s => s.OrganizationId == org.Id.ToString()).ToList();
                if(services.Count > 0)
                {
                    org.Services = services;
                }

                // Set organization root id
                org.OrganizationRootId = organizationRoots.TryGet(org.Id.Value)?.Item1;

                // Get the right open api version
                vmList.Add(GetEntityByOpenApiVersion(org as IVmOpenApiOrganizationVersionBase, openApiVersion));
            });

            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Finalization - mapping & right version: {watch.ElapsedMilliseconds} ms.");
            //// End measure

            return vmList;
        }

        private VmPublishingStatus GetOrganizationStatus(IUnitOfWorkWritable unitOfWork, Guid? organizationId)
        {
            var serviceRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var service = serviceRep.All()
                            .Include(x => x.PublishingStatus)
                            .Single(x => x.Id == organizationId.Value);

            return TranslationManagerToVm.Translate<PublishingStatusType, VmPublishingStatus>(service.PublishingStatus);
        }

        private VmArchiveResult CascadeDeleteOrganization(IUnitOfWorkWritable unitOfWork, Guid? id, bool checkDelete = false)
        {
            if (checkDelete)
            {
                return GetOrganizationConnectedEntities(unitOfWork, id.Value);
            }
            else
            {
                var organization = commonService.ChangeEntityToDeleted<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWork, id.Value);
                ArchiveConnectedChannels(unitOfWork, organization.UnificRootId);
                ArchiveConnectedServices(unitOfWork, organization.UnificRootId);
                ArchiveSubOrganizations(unitOfWork, organization.UnificRootId);
                return new VmArchiveResult { Id = organization.Id, PublishingStatusId = organization.PublishingStatusId };
            }
        }

        private VmArchiveResult GetOrganizationConnectedEntities(IUnitOfWorkWritable unitOfWork, Guid? organizationId)
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
                ChannelsConnected = organizations.Any(i => i.UnificRoot.OrganizationServiceChannelsVersioned.Any(j => j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished)),
                SubOrganizationsConnected = organizations.Any(i => i.UnificRoot.Children.Any(j => j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished)),
                ServicesConnected = organizations.Any(i => i.UnificRoot.OrganizationServices.Any(j => j.ServiceVersioned.PublishingStatusId != psDeleted && j.ServiceVersioned.PublishingStatusId != psOldPublished))
                                    || organizations.Any(i => i.UnificRoot.ServiceProducerOrganizations.Any(j => j.ServiceProducer.ServiceVersioned.PublishingStatusId != psDeleted && j.ServiceProducer.ServiceVersioned.PublishingStatusId != psOldPublished))
                                    || organizations.Any(i => i.UnificRoot.OrganizationServicesVersioned.Any(j => j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished))

            };
        }

        private void ArchiveConnectedChannels(IUnitOfWorkWritable unitOfWork, Guid organizationId)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            channelRep.All().Where(x => x.PublishingStatusId != psDeleted && x.PublishingStatusId != psOldPublished)
                            .Where(x => x.OrganizationId == organizationId)
                            .ForEach(x => x.SafeCall(i => i.PublishingStatusId = psDeleted));
        }

        private void ArchiveConnectedServices(IUnitOfWorkWritable unitOfWork, Guid organizationId)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var services = serviceRep.All().Where(x => x.PublishingStatusId != psDeleted && x.PublishingStatusId != psOldPublished)
                            .Where(x => x.OrganizationServices.Any(o=>o.OrganizationId == organizationId) ||
                                        x.ServiceProducers.SelectMany(sp => sp.Organizations).Any(o => o.OrganizationId == organizationId) ||
                                        x.OrganizationId == organizationId)
                            .Include(x => x.OrganizationServices).ThenInclude(x=>x.Organization).ThenInclude(x=>x.Versions)
                            .Include(x => x.ServiceProducers).ThenInclude(x => x.Organizations).ThenInclude(x => x.Organization).ThenInclude(x => x.Versions)
                            .ToList();

            foreach (var service in services)
            {
                var restOrganizations = service.OrganizationServices.Where(x => x.OrganizationId != organizationId && x.Organization.Versions.Any(y=>y.PublishingStatusId != psDeleted && y.PublishingStatusId != psOldPublished)).ToList();
                service.OrganizationServices = restOrganizations.ToList();

                var producersToDelete = HandleServiceProducers(service.ServiceProducers, organizationId, psDeleted, psOldPublished);
                if (producersToDelete.Any())
                {
                    producersToDelete.ForEach(p => service.ServiceProducers.Remove(p));

                    var orderNumber = 1;
                    service.ServiceProducers.OrderBy(p => p.OrderNumber).ForEach(p => p.OrderNumber = orderNumber++);
                }
                if (service.OrganizationId == organizationId)
                {
                    service.SafeCall(i => i.PublishingStatusId = psDeleted);
                }
            }
        }

        private List<ServiceProducer> HandleServiceProducers(ICollection<ServiceProducer> serviceProducers, Guid organizationId, Guid psDeleted, Guid psOldPublished)
        {
            var producersToDelete = new List<ServiceProducer>();
            if (serviceProducers == null) return producersToDelete;

            foreach (var producer in serviceProducers)
            {
                if (producer.ProvisionTypeId == typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString()))
                {
                    var producerOrganizations = producer.Organizations.Where(spo => spo.OrganizationId != organizationId && spo.Organization.Versions.Any(ov => ov.PublishingStatusId != psDeleted && ov.PublishingStatusId != psOldPublished)).ToList();
                    if (producerOrganizations.Any())
                    {
                        producer.Organizations = producerOrganizations;
                    }
                    else
                    {
                        producersToDelete.Add(producer);
                    }
                }

                if (producer.ProvisionTypeId == typesCache.Get<ProvisionType>(ProvisionTypeEnum.PurchaseServices.ToString()))
                {
                    var spo = producer.Organizations.FirstOrDefault();
                    if (spo == null)
                    {
                        producersToDelete.Add(producer);
                    }
                    else
                    {
                        if (spo.OrganizationId == organizationId && !spo.Organization.Versions.Any(ov => ov.PublishingStatusId != psDeleted && ov.PublishingStatusId != psOldPublished))
                        {
                            producersToDelete.Add(producer);
                        }
                    }
                }

                if (producer.ProvisionTypeId == typesCache.Get<ProvisionType>(ProvisionTypeEnum.Other.ToString()))
                {
                    var spo = producer.Organizations.FirstOrDefault();
                    if (spo != null && spo.OrganizationId == organizationId && !spo.Organization.Versions.Any(ov => ov.PublishingStatusId != psDeleted && ov.PublishingStatusId != psOldPublished))
                    {
                        producersToDelete.Add(producer);
                    }
                }
            }

            return producersToDelete;
        }

        private void ArchiveSubOrganizations(IUnitOfWorkWritable unitOfWork, Guid organizationId)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var subOrgs = organizationRep.All().Where(x => x.PublishingStatusId != psDeleted && x.PublishingStatusId != psOldPublished)
                            .Where(x => x.ParentId == organizationId).ToList();

            foreach (var subOrg in subOrgs)
            {
                subOrg.PublishingStatusId = psDeleted;
                ArchiveConnectedChannels(unitOfWork, subOrg.UnificRootId);
                ArchiveConnectedServices(unitOfWork, subOrg.UnificRootId);
                ArchiveSubOrganizations(unitOfWork, subOrg.UnificRootId);
            }

        }

        private OrganizationVersioned DeleteOrganization(IUnitOfWorkWritable unitOfWork, Guid? id)
        {
            return commonService.ChangeEntityToDeleted<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWork, id.Value);
        }

        public List<string> GetAvailableLanguagesForOwnOrganization(Guid id)
        {
            var organization = contextManager.ExecuteReader(unitOfWork =>
            {
                var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                var statusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                return unitOfWork.ApplyIncludes(organizationRep.All().Where(o => o.UnificRootId.Equals(id) && o.PublishingStatusId == statusId),
                    q => q.Include(i => i.UnificRoot).Include(i => i.LanguageAvailabilities)).FirstOrDefault();
            });

            if (organization != null && organization.LanguageAvailabilities != null)
            {
                // Eeva can use all organizations
                if (tokenProcessor.UserRole == UserRoleEnum.Eeva)
                {
                    return organization.LanguageAvailabilities.Select(l => languageCache.GetByValue(l.LanguageId)).ToList();
                }

                // For other roles than Eeva we need to check that organization is one of users own.
                var userOrganizations = userOrganizationService.GetAllUserOrganizationIds();
                if (!userOrganizations.Contains(id))
                {
                    throw new Exception("User has no rights to update or create this entity!");
                }

                return organization.LanguageAvailabilities.Select(l => languageCache.GetByValue(l.LanguageId)).ToList();
            }

            return null;
       }

        public IVmOpenApiOrganizationVersionBase SaveOrganization(IVmOpenApiOrganizationInVersionBase vm, int openApiVersion)
        {
            var saveMode = SaveMode.Normal;
            var userId = utilities.GetRelationIdForExternalSource();
            var organization = new OrganizationVersioned();

            try
            {
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    // Get the root id according to source id (if defined)
                    var rootId = vm.Id ?? GetPTVId<Organization>(vm.SourceId, userId, unitOfWork);

                    // Get right version id
                    vm.Id = VersioningManager.GetVersionId<OrganizationVersioned>(unitOfWork, rootId, null, false);

                    // Set user name which is used within language availabilities and check the publishing status (SFIPTV-190)
                    vm.UserName = unitOfWork.GetUserNameForAuditing();
                    var currentTime = DateTime.UtcNow;
                    if ((vm.ValidFrom.HasValue && vm.ValidFrom.Value > currentTime) || (vm.ValidTo.HasValue && vm.ValidTo.Value > currentTime))
                    {
                        if (vm.ValidFrom.HasValue)
                        {
                            // For timed publishing the version created needs to be set as modified
                            vm.PublishingStatus = PublishingStatus.Modified.ToString();
                        }

                        // We need to get the available languages to be able update the publishing and archiving dates for different language versions
                        var allAvailableLanguages = unitOfWork.CreateRepository<IRepository<OrganizationLanguageAvailability>>().All()
                            .Where(x => x.OrganizationVersionedId == vm.Id).Select(x => languageCache.GetByValue(x.LanguageId)).ToHashSet();
                        vm.AvailableLanguages.ForEach(l => allAvailableLanguages.Add(l));
                        vm.AvailableLanguages = allAvailableLanguages.ToList();
                    }

                    if (vm.PublishingStatus == PublishingStatus.Deleted.ToString())
                    {
                        try
                        {
                            var archiveResult = CascadeDeleteOrganization(unitOfWork, vm.Id, true);
                            if (!archiveResult.AnyConnected)
                            {
                                organization = DeleteOrganization(unitOfWork, vm.Id);
                            }
                            else
                            {
                                throw new Exception($"You cannot delete organization {rootId}. Services or service channels attached!");
                            }
                        }
                        catch (OrganizationNotDeleteInUserUseException)
                        {
                            throw new Exception($"You cannot delete organization {rootId}. Users are attached to the organization!");
                        }
                        catch (OrganizationNotDeleteInUseException)
                        {
                            throw new Exception($"You cannot delete organization {rootId}. Services or service channels attached!");
                        }
                    }
                    else
                    {
                        var parentOrgId = vm.ParentOrganizationId.ParseToGuid();
                        if (IsCyclicDependency(unitOfWork, rootId, parentOrgId))
                        {
                            throw new Exception($"You cannot use {vm.ParentOrganizationId} as a parent organization. A cycling dependency is not allowed to be created between organizations!");
                        }

                        // Entity needs to be restored?
                        if (vm.CurrentPublishingStatus == PublishingStatus.Deleted.ToString())
                        {
                            if (vm.PublishingStatus == PublishingStatus.Modified.ToString() || vm.PublishingStatus == PublishingStatus.Published.ToString())
                            {
                                // We need to restore already archived item
                                var publishingResult = commonService.RestoreArchivedEntity<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWork, vm.Id.Value);
                            }
                        }

                        // Check address related municipalities
                        vm.Addresses.ForEach(a => CheckAddress(unitOfWork, a));

                        organization = TranslationManagerToEntity.Translate<IVmOpenApiOrganizationInVersionBase, OrganizationVersioned>(vm, unitOfWork);

                        if (!vm.OrganizationType.IsNullOrEmpty() && vm.OrganizationType != OrganizationTypeEnum.Municipality.ToString()) // Municipality info needs to be removed if organization type is not municipality!
                        {
                            organization.MunicipalityId = null;
                        }

                        if (vm.CurrentPublishingStatus == PublishingStatus.Draft.ToString())
                        {
                            // We need to manually remove items from collections!
                            if (!vm.SubAreaType.IsNullOrEmpty() && vm.Areas.Count > 0)
                            {
                                if (vm.SubAreaType == AreaTypeEnum.Municipality.ToString())
                                {
                                    organization.OrganizationAreaMunicipalities = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, organization.OrganizationAreaMunicipalities,
                                        query => query.OrganizationVersionedId == organization.Id, area => area.MunicipalityId);
                                    // Remove all possible old areas
                                    dataUtils.RemoveItemCollection<OrganizationArea>(unitOfWork, s => s.OrganizationVersionedId == organization.Id);
                                }
                                else
                                {
                                    organization.OrganizationAreas = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, organization.OrganizationAreas,
                                        query => query.OrganizationVersionedId == organization.Id, area => area.AreaId);
                                    // Remove all possible old municipalities
                                    dataUtils.RemoveItemCollection<OrganizationAreaMunicipality>(unitOfWork, s => s.OrganizationVersionedId == organization.Id);
                                }
                            }
                            else if (!vm.AreaType.IsNullOrEmpty() && vm.AreaType != AreaInformationTypeEnum.AreaType.ToString())
                            {
                                // We need to remove possible old areas and municipalities
                                dataUtils.RemoveItemCollection<OrganizationArea>(unitOfWork, s => s.OrganizationVersionedId == organization.Id);
                                dataUtils.RemoveItemCollection<OrganizationAreaMunicipality>(unitOfWork, s => s.OrganizationVersionedId == organization.Id);
                            }

                            if (vm.DeleteAllPhones || (vm.PhoneNumbers != null && vm.PhoneNumbers.Count > 0))
                            {
                                // Remove the phones that were not included in vm
                                organization.OrganizationPhones = UpdateOrganizationCollectionWithRemove<OrganizationPhone, Phone>(unitOfWork, organization.Id,
                                    organization.OrganizationPhones, e => e.PhoneId);
                            }

                            if (vm.DeleteAllEmails || (vm.Emails != null && vm.Emails.Count > 0))
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

                            if (vm.DeleteAllElectronicInvoicings || (vm.ElectronicInvoicings != null && vm.ElectronicInvoicings.Count > 0))
                            {
                                // Remove all electronic invoicing addresses that were not included in vm
                                dataUtils.RemoveItemCollection<OrganizationEInvoicing>(unitOfWork, s => s.OrganizationVersionedId == organization.Id);
                            }
                        }

                        // Update the mapping between external source id and PTV id
                        if (!string.IsNullOrEmpty(vm.SourceId))
                        {
                            UpdateExternalSource<Organization>(organization.UnificRootId, vm.SourceId, userId, unitOfWork);
                        }
                    }

                    commonService.AddHistoryMetaData<OrganizationVersioned, OrganizationLanguageAvailability>(organization, setByEntity: true);
                    unitOfWork.Save(saveMode, PreSaveAction.Standard, organization);
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
            catch (Exception ex)
            {
                logger.LogError("SaveOrganization error: " + ex.Message);
                throw ex;
            }
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

        #region Lock


        public IReadOnlyList<IVmBase> Get(IUnitOfWork unitOfWork)
        {
              return CreateTree<VmTreeItem>(commonService.GetOrganizationNamesTree());
        }
        #endregion Lock

        private Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>> GetOrganizationIncludeChain()
        {
            return q =>
                q.Include(i => i.Business)
                .Include(i => i.Type)
                .Include(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.OrganizationEmails).ThenInclude(i => i.Email)
                .Include(i => i.OrganizationNames)
                .Include(i => i.OrganizationDisplayNameTypes)
                .Include(i => i.OrganizationDescriptions)
                .Include(i => i.OrganizationPhones).ThenInclude(i => i.Phone).ThenInclude(i => i.PrefixNumber)
                .Include(i => i.OrganizationWebAddress).ThenInclude(i => i.WebPage)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreet).ThenInclude(i => i.StreetNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreet).ThenInclude(i => i.StreetNumbers).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreetNumber)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.Municipality)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressOthers).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
                .Include(i => i.OrganizationEInvoicings).ThenInclude(i => i.EInvoicingAdditionalInformations)
                .Include(i => i.LanguageAvailabilities)
                .Include(i => i.OrganizationAreas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaNames)
                .Include(i => i.OrganizationAreas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.OrganizationAreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.Parent).ThenInclude(i => i.Versions)
                .Include(i => i.UnificRoot);
        }

        private void CheckSoteUserRights(string organizationType)
        {
            if (organizationType.IsNullOrWhitespace()) return;
            if (!commonService.OrganizationIsSote(organizationType)) return;
            if (tokenProcessor.UserAccessRights.HasFlag(AccessRightEnum.SOTEWrite)) return;
            throw new PtvAppException(string.Format(CoreMessages.OpenApi.UserHasNoRightsToCreateSoteOrganization, tokenProcessor.UserName));
        }

        private void HandleOrganizationSoteFocFilter(IUnitOfWorkWritable unitOfWork, OrganizationVersioned organization)
        {
            if (!commonService.OrganizationIsSote(organization.TypeId)) return;
            commonService.HandleOrganizationSoteFocFilter(unitOfWork, new List<Guid>{organization.UnificRootId});
        }
    }
}

