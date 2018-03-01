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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Enums;

using Microsoft.Extensions.Logging;
using PTV.Database.Model.Models.Base;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Internal;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Utils;
using PTV.Framework.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.V2.Common;
using Remotion.Linq.Clauses;
using PTV.Domain.Model.Models.OpenApi.V7;
using System.Diagnostics;
using PTV.Domain.Model.Models.OpenApi.V8;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof (IServiceService), RegisterType.Transient)]
    internal class ServiceService : ServiceBase, IServiceService
    {
        private IContextManager contextManager;

        private ILogger logger;
        private ServiceUtilities utilities;
        private DataUtils dataUtils;
        private ICommonServiceInternal commonService;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private IVersioningManager versioningManager;
        private IGeneralDescriptionService generalDescriptionService;

        public ServiceService(
            IContextManager contextManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            ILogger<ServiceService> logger,
            ServiceUtilities utilities,
            DataUtils dataUtils,
            ICommonServiceInternal commonService,
            ITypesCache typesCache,
            ILanguageCache languageCache,
            IPublishingStatusCache publishingStatusCache,
            IVersioningManager versioningManager,
            IGeneralDescriptionService generalDescriptionService,
            IUserOrganizationChecker userOrganizationChecker)
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.dataUtils = dataUtils;
            this.commonService = commonService;
            this.typesCache = typesCache;
            this.languageCache = languageCache;
            this.versioningManager = versioningManager;
            this.generalDescriptionService = generalDescriptionService;
        }

        public IVmServiceSearchResult SearchServices(IVmServiceSearch vmServiceSearch)
        {
            vmServiceSearch.Name = vmServiceSearch.Name != null ? Regex.Replace(vmServiceSearch.Name.Trim(), @"\s+", " ") : vmServiceSearch.Name;
            IReadOnlyList<IVmServiceListItem> result = new List<IVmServiceListItem>();
            bool moreAvailable = false;
            int count = 0;
            int safePageNumber = vmServiceSearch.PageNumber.PositiveOrZero();

            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(vmServiceSearch);
                var languagesIds = vmServiceSearch.Languages.Select(language => languageCache.Get(language.ToString())).ToList();
                var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
                var publishedStatusId = PublishingStatusCache.Get(PublishingStatus.Published);

                var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var resultTemp = serviceRep.All();

                #region SearchByFilterParam

                if (vmServiceSearch.ServiceClassId.HasValue)
                {
                    var generalDescIds = generalDescriptionRep.All()
                        .Where(x => x.ServiceClasses.Any(s => s.ServiceClassId == vmServiceSearch.ServiceClassId) &&
                                    x.PublishingStatusId == publishedStatusId)
                        .Select(x => x.UnificRootId);

                    resultTemp = resultTemp.Where(
                        x => x.ServiceServiceClasses.Any(s => s.ServiceClassId == vmServiceSearch.ServiceClassId.Value) ||
                            generalDescIds.Any(d => d == x.StatutoryServiceGeneralDescriptionId)
                        );
                }
                if (vmServiceSearch.OrganizationId.HasValue)
                {
                    resultTemp = resultTemp.Where(x => x.OrganizationServices.Any(o => o.OrganizationId == vmServiceSearch.OrganizationId) || x.OrganizationId == vmServiceSearch.OrganizationId.Value);

                }
                if (!string.IsNullOrEmpty(vmServiceSearch.Name))
                {
                    var rootId = GetRootIdFromString(vmServiceSearch.Name);
                    if (!rootId.HasValue)
                    {
                        var searchText = vmServiceSearch.Name.ToLower();
                        resultTemp = resultTemp.Where(
                            x => x.ServiceNames.Any(
                                y => (y.Name.ToLower().Contains(searchText) || y.CreatedBy.ToLower().Contains(searchText) || y.ModifiedBy.ToLower().Contains(searchText))
                                     && languagesIds.Contains(y.LocalizationId)));
                    }
                    else
                    {
                        resultTemp = resultTemp
                            .Where(service =>
                                service.UnificRootId == rootId
                            );
                    }
                }
                else
                {
                    resultTemp =
                        resultTemp.Where(
                            x =>
                                x.ServiceNames.Any(
                                    y =>
                                        languagesIds.Contains(y.LocalizationId) &&
                                        !string.IsNullOrEmpty(y.Name)));
                }

                if (vmServiceSearch.OntologyTerms.IsAssigned())
                {
                    var generalDescIds = generalDescriptionRep.All()
                        .Where(x => x.PublishingStatusId == publishedStatusId &&
                                    x.OntologyTerms.Select(s => s.OntologyTermId)
                                        .Contains(vmServiceSearch.OntologyTerms.Value))
                        .Select(x => x.UnificRootId);

                    resultTemp = resultTemp.Where(
                        x => x.ServiceOntologyTerms.Any(y => y.OntologyTermId == vmServiceSearch.OntologyTerms.Value) ||
                            generalDescIds.Any(d => d == x.StatutoryServiceGeneralDescriptionId)
                        );
                }

                if (vmServiceSearch.ServiceTypeId.HasValue)
                {
                    var generalDescIds = generalDescriptionRep.All()
                                            .Where(x => x.TypeId == vmServiceSearch.ServiceTypeId &&
                                                        x.PublishingStatusId == publishedStatusId)
                                            .Select(x => x.UnificRootId);
                    resultTemp = resultTemp.Where(x => (x.TypeId == vmServiceSearch.ServiceTypeId.Value && x.StatutoryServiceGeneralDescriptionId == null) || generalDescIds.Any(d => d == x.StatutoryServiceGeneralDescriptionId));
                }

                if (vmServiceSearch.SelectedPublishingStatuses != null)
                {
                    commonService.ExtendPublishingStatusesByEquivalents(vmServiceSearch.SelectedPublishingStatuses);
                    resultTemp = resultTemp.WherePublishingStatusIn(vmServiceSearch.SelectedPublishingStatuses);
                }

                #endregion SearchByFilterParam

                //SWITCH sort params
                SwitchSortParams(vmServiceSearch.SortData, new Dictionary<string, string>()
                {
                    { "serviceTypeId", "TypeName" }
                });

                count = resultTemp.Count();

                // includes for entity properties (intendation used for easy reading ;) )
                resultTemp = resultTemp
                                .Include(sv => sv.OrganizationServices)
                                .Include(sv => sv.LanguageAvailabilities)
                                    .ThenInclude(sla => sla.Language)
                                .Include(sv => sv.Versioning)
                                .Include(sv => sv.ServiceProducers)
                                .Include(sv => sv.StatutoryServiceGeneralDescription)
                                    .ThenInclude(ssgd => ssgd.Versions);

                // Get ServiceVersioned entities and only get values to the anonymous object that are needed for sorting
                var nameType = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
                // to avoid as many in memory operations as possible
                var resultTempData = resultTemp.Select(i => new
                {
                    ServiceVersioned = i,
                    UnificRootId = i.UnificRootId, // required for sorting
                    // Name property is required for sorting
                    Name = i.ServiceNames.OrderBy(x => x.Localization.OrderNumber).FirstOrDefault(x => languagesIds.Contains(x.LocalizationId) && x.TypeId == nameTypeId).Name,
                    AllNames = i.ServiceNames.Where(x => x.TypeId == nameType).Select(x => new { x.LocalizationId, x.Name }),
                    VersionMajor = i.Versioning.VersionMajor, // required for sorting
                    VersionMinor = i.Versioning.VersionMinor, // required for sorting
                    Modified = i.Modified, // required for sorting
                    ModifiedBy = i.ModifiedBy, // required for sorting
                })
                .ApplySortingByVersions(vmServiceSearch.SortData, new VmSortParam() { Column = "Modified", SortDirection = SortDirectionEnum.Desc })
                .Select(i => new
                    {
                        ServiceVersioned = i.ServiceVersioned,
                        UnificRootId = i.UnificRootId, // required for sorting
                        VersionMajor = i.VersionMajor, // required for sorting
                        VersionMinor = i.VersionMinor, // required for sorting
                        Modified = i.Modified, // required for sorting
                        ModifiedBy = i.ModifiedBy, // required for sorting
                })
                .ApplyPagination(safePageNumber)
                .ToList();
                moreAvailable = count.MoreResultsAvailable(safePageNumber);
                var serviceIds = resultTempData.Select(i => i.ServiceVersioned.Id).ToList();

                var serviceNameRep = unitOfWork.CreateRepository<IServiceNameRepository>();

                var serviceNames = serviceNameRep.All().Where(x => serviceIds.Contains(x.ServiceVersionedId) && languagesIds.Contains(x.LocalizationId) && x.TypeId == nameTypeId).OrderBy(i => i.Localization.OrderNumber).Select(i => new { i.ServiceVersionedId, i.Name, i.LocalizationId }).ToList().GroupBy(i => i.ServiceVersionedId)
                    .ToDictionary(i => i.Key, i => i.ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));
                result = resultTempData.Select(i => {

                    Guid? typeId = i.ServiceVersioned?.TypeId;

                    if(typeId == null && i.ServiceVersioned?.StatutoryServiceGeneralDescription?.Versions?.Count > 0)
                    {
                        typeId = versioningManager.ApplyPublishingStatusFilterFallback(i.ServiceVersioned.StatutoryServiceGeneralDescription.Versions)?.TypeId;
                    }

                    return new VmServiceListItem
                    {
                        Id = i.ServiceVersioned.Id,
                        PublishingStatusId = i.ServiceVersioned.PublishingStatusId,
                        UnificRootId = i.ServiceVersioned.UnificRootId,
                        Name = serviceNames.TryGetOrDefault(i.ServiceVersioned.Id, new Dictionary<string, string>()),
                        ServiceTypeId = typeId,
                        ServiceType = typeId != null ? typesCache.GetByValue<ServiceType>(typeId.Value) : string.Empty,
                        LanguagesAvailabilities = TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(i.ServiceVersioned.LanguageAvailabilities.OrderBy(x => x.Language.OrderNumber)),
                        Version = TranslationManagerToVm.Translate<Versioning, VmVersion>(i.ServiceVersioned.Versioning),
                        Organizations = new List<Guid> { i.ServiceVersioned.OrganizationId }.Union(i.ServiceVersioned.OrganizationServices.Select(x => x.OrganizationId)).ToList(),
                        Producers = i.ServiceVersioned.ServiceProducers.Select(x => x.Id).ToList(),
                        Modified = i.ServiceVersioned.Modified.ToEpochTime(),
                        ModifiedBy = i.ServiceVersioned.ModifiedBy,
                        GeneralDescriptionTypeId = i.ServiceVersioned?.StatutoryServiceGeneralDescription?.Versions.FirstOrDefault(x => x.PublishingStatusId == publishedStatusId)?.TypeId
                    };
                })
                .ToList();

                return result;
            });
            return new VmServiceSearchResultResult() {
                Services = result,
                PageNumber = ++safePageNumber,
                MoreAvailable = moreAvailable,
                Count = count,

            };
        }

        private ServiceVersioned DeleteService(IUnitOfWorkWritable unitOfWork, Guid? serviceId)
        {
            var publishStatus = TranslationManagerToEntity.Translate<String, PublishingStatusType>(PublishingStatus.Deleted.ToString(), unitOfWork);

            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var service = serviceRep.All().Single(x => x.Id == serviceId.Value);
            service.PublishingStatus = publishStatus;

            return service;
        }

        private void SwitchSortParams(List<VmSortParam> sortParams, Dictionary<string, string> rules)
        {
            foreach (var sortParam in sortParams)
            {
                if (!string.IsNullOrWhiteSpace(sortParam.Column) && rules.ContainsKey(sortParam.Column))
                {
                    sortParam.Column = rules[sortParam.Column];
                }
            }
        }

        #region Open Api

        public IVmOpenApiGuidPageVersionBase GetServices(DateTime? date, int pageNumber, int pageSize, bool archived, bool active)
        {
            var vm = new V3VmOpenApiGuidPage(pageNumber, pageSize);

            if (pageNumber <= 0) return vm;

            List<ServiceVersioned> services = null;

            contextManager.ExecuteReader(unitOfWork =>
            {
                if (archived)
                {
                    services = GetArchivedEntities<ServiceVersioned, Service, ServiceLanguageAvailability>(vm, date, unitOfWork,
                        q => q.Include(i => i.ServiceNames));
                }
                else if (active)
                {
                    services = GetActiveEntities<ServiceVersioned, Service, ServiceLanguageAvailability>(vm, date, unitOfWork,
                        q => q.Include(i => i.ServiceNames));
                }
                else
                {
                    // Get the published services and filter out item names that are not published (PTV-3689).
                    services = FilterOutNotPublishedNames(GetPublishedEntities<ServiceVersioned, Service, ServiceLanguageAvailability>(vm, date, unitOfWork,
                        q => q.Include(i => i.ServiceNames).Include(i => i.LanguageAvailabilities)));
                }
            });

            return GetGuidPage(services, vm);
        }

        public IList<IVmOpenApiServiceVersionBase> GetServices(List<Guid> idList)
        {
            if (idList == null || idList.Count == 0)
            {
                return null;
            }

            IList<IVmOpenApiServiceVersionBase> result = new List<IVmOpenApiServiceVersionBase>();
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    IList<Expression<Func<ServiceVersioned, bool>>> filters = new List<Expression<Func<ServiceVersioned, bool>>>
                    {
                        c => idList.Contains(c.UnificRootId)
                    };

                    result = GetServicesWithDetails(unitOfWork, filters, 8);
                });
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting services. {0}", ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }

            return result;
        }

        public IVmOpenApiGuidPageVersionBase GetServicesByServiceChannel(Guid channelId, DateTime? date, int pageNumber = 1, int pageSize = 1000)
        {
            var vm = new V3VmOpenApiGuidPage(pageNumber, pageSize);

            if (pageNumber <= 0) return vm;

            List<ServiceVersioned> services = null;

            contextManager.ExecuteReader(unitOfWork =>
            {
                // Get services related to given service channel
                var serviceChannelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
                var serviceIdQuery = serviceChannelRep.All().Where(s => s.ServiceChannelId.Equals(channelId));
                var serviceIdList = serviceIdQuery.ToList().Select(s => s.ServiceId).ToList();
                var additionalFilters = new List<Expression<Func<ServiceVersioned, bool>>>() { s => serviceIdList.Contains(s.UnificRootId) };
                // Get the published services and filter out item names that are not published (PTV-3689).
                services = FilterOutNotPublishedNames(GetPublishedEntities<ServiceVersioned, Service, ServiceLanguageAvailability>
                    (vm, date, unitOfWork, q => q.Include(i => i.ServiceNames).Include(i => i.LanguageAvailabilities), additionalFilters));                
            });

            return GetGuidPage(services, vm);
        }

        /// <summary>
        /// Gets all published services that are related to given service class. Takes also into account services where attached general desription is related to given service class.
        /// </summary>
        /// <param name="serviceClassId"></param>
        /// <param name="date"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IVmOpenApiGuidPageVersionBase GetServicesByServiceClass(Guid serviceClassId, DateTime? date, int pageNumber = 1, int pageSize = 1000)
        {
            var vm = new V3VmOpenApiGuidPage(pageNumber, pageSize);

            if (pageNumber <= 0) return vm;

            List<ServiceVersioned> services = null;

            contextManager.ExecuteReader(unitOfWork =>
            {
                var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

                // Get statutory service descriptions that are related to defined service class
                var gdRep = unitOfWork.CreateRepository<IStatutoryServiceServiceClassRepository>();
                var gdQuery = gdRep.All().Where(s => s.ServiceClassId.Equals(serviceClassId) &&
                    s.StatutoryServiceGeneralDescriptionVersioned.PublishingStatusId == publishedId &&
                    s.StatutoryServiceGeneralDescriptionVersioned.LanguageAvailabilities.Any(l => l.StatusId == publishedId));

                var gdList = unitOfWork.ApplyIncludes(gdQuery, q =>
                    q.Include(i => i.StatutoryServiceGeneralDescriptionVersioned)
                    ).ToList();

                var gdIdList = gdList.Select(g => g.StatutoryServiceGeneralDescriptionVersioned.UnificRootId).Distinct().ToList();

                // Get services related to given service class.
                var additionalFilters = new List<Expression<Func<ServiceVersioned, bool>>>();
                if (gdIdList?.Count > 0)
                {
                    // Get services that are attached to one of the general desriptions or that are related to given service class.
                    additionalFilters.Add(service =>
                        (service.StatutoryServiceGeneralDescriptionId != null && gdIdList.Contains(service.StatutoryServiceGeneralDescriptionId.Value)) ||
                            service.ServiceServiceClasses.Any(c => c.ServiceClassId == serviceClassId));
                }
                else
                {
                    // Get services that are related to given service class.
                    additionalFilters.Add(service => service.ServiceServiceClasses.Any(c => c.ServiceClassId == serviceClassId));
                }

                // Get the published services and filter out item names that are not published (PTV-3689).
                services = FilterOutNotPublishedNames(GetPublishedEntities<ServiceVersioned, Service, ServiceLanguageAvailability>
                        (vm, date, unitOfWork, q => q.Include(i => i.ServiceNames).Include(i => i.LanguageAvailabilities), additionalFilters));
            });

            return GetGuidPage(services, vm);
        }

        public IVmOpenApiGuidPageVersionBase GetServicesByMunicipality(Guid municipalityId, DateTime? date, int pageNumber, int pageSize)
        {
            var vm = new V3VmOpenApiGuidPage(pageNumber, pageSize);

            if (pageNumber <= 0) return vm;

            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            List<ServiceVersioned> services = null;

            contextManager.ExecuteReader(unitOfWork =>
            {
                // Areas related to defined municipality
                var areas = unitOfWork.CreateRepository<IAreaMunicipalityRepository>().All()
                    .Where(a => a.MunicipalityId == municipalityId).Select(a => a.AreaId).ToList();

                // Get services
                var wholeCountryId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
                var additionalFilters = new List<Expression<Func<ServiceVersioned, bool>>>();
                // is the municipality in 'Åland'? So do we need to include also AreaInformationType WholeCountryExceptAlandIslands?
                if (IsAreaInAland(unitOfWork, areas, typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString()))) // Åland
                {
                    additionalFilters.Add(s => (s.AreaInformationTypeId == wholeCountryId || s.AreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) ||
                    s.Areas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId))));
                }
                else
                {
                    var wholeCountryExceptAlandId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
                    additionalFilters.Add(s => (s.AreaInformationTypeId == wholeCountryId) || s.AreaInformationTypeId == wholeCountryExceptAlandId ||
                    (s.AreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || s.Areas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId))));
                }

                // Get the published services and filter out item names that are not published (PTV-3689).
                services = FilterOutNotPublishedNames(GetPublishedEntities<ServiceVersioned, Service, ServiceLanguageAvailability>
                        (vm, date, unitOfWork, q => q.Include(i => i.ServiceNames).Include(i => i.LanguageAvailabilities), additionalFilters));
            });

            return GetGuidPage(services, vm);
        }

        public IVmOpenApiServiceVersionBase GetServiceById(Guid id, int openApiVersion, VersionStatusEnum status)
        {
            IVmOpenApiServiceVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetServiceById(unitOfWork, id, openApiVersion, status);
            });

            return result;
        }

        /// <summary>
        /// Returns the latest version of a service with minimum data included.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IVmOpenApiServiceVersionBase GetServiceByIdSimple(Guid id, bool getOnlyPublished = true)
        {
            IVmOpenApiServiceVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                try
                {
                    Guid? entityId = null;
                    if (getOnlyPublished)
                    {   // Get published version
                        entityId = versioningManager.GetVersionId<ServiceVersioned>(unitOfWork, id, PublishingStatus.Published);
                    }
                    else
                    {   // Get latest version regardless of the publishing status
                        entityId = versioningManager.GetVersionId<ServiceVersioned>(unitOfWork, id, null, false);
                    }

                    if (entityId.IsAssigned())
                    {
                        result = GetServiceWithSimpleDetails(unitOfWork, entityId.Value);
                    }
                }
                catch (Exception ex)
                {
                    var errorMsg = string.Format("Error occured while getting a service with id {0}. {1}", id, ex.Message);
                    logger.LogError(errorMsg + " " + ex.StackTrace);
                    throw new Exception(errorMsg);
                }
            });

            return result;
        }

        private List<ServiceVersioned> FilterOutNotPublishedNames(List<ServiceVersioned> services)
        {
            if (services == null) return services;

            var publishedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            services.ForEach(s =>
            {
                var publishedLanguageIds = s.LanguageAvailabilities.Where(l => l.StatusId == publishedStatusId).Select(l => l.LanguageId).ToList();
                s.ServiceNames = s.ServiceNames.Where(i => publishedLanguageIds.Contains(i.LocalizationId)).ToList();
            });
            return services;
        }

        private IVmOpenApiGuidPageVersionBase GetGuidPage(List<ServiceVersioned> services, V3VmOpenApiGuidPage vm)
        {
            if (services?.Count > 0)
            {
                vm.ItemList = TranslationManagerToVm.TranslateAll<ServiceVersioned, VmOpenApiItem>(services).ToList();
            }

            return vm;
        }

        private IVmOpenApiServiceVersionBase GetServiceById(IUnitOfWork unitOfWork, Guid id, int openApiVersion, VersionStatusEnum status)
        {
            try
            {
                // Get the right version id
                Guid? entityId = null;
                switch (status)
                {
                    case VersionStatusEnum.Published:
                        entityId = versioningManager.GetVersionId<ServiceVersioned>(unitOfWork, id, PublishingStatus.Published);
                        break;
                    case VersionStatusEnum.Latest:
                        entityId = versioningManager.GetVersionId<ServiceVersioned>(unitOfWork, id, null, false);
                        break;
                    case VersionStatusEnum.LatestActive:
                        entityId = versioningManager.GetVersionId<ServiceVersioned>(unitOfWork, id, null, true);
                        break;
                    default:
                        break;
                }
                if (entityId.IsAssigned())
                {
                    return GetServiceWithDetails(unitOfWork, entityId.Value, openApiVersion, status == VersionStatusEnum.Published ? true : false);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting a service with id {0}. {1}", id, ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }
            return null;
        }

        public IList<IVmOpenApiServiceVersionBase> GetServicesByServiceChannel(Guid id, DateTime? date, int openApiVersion)
        {
            IList<IVmOpenApiServiceVersionBase> result = new List<IVmOpenApiServiceVersionBase>();
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    // Get all the published services that are related to defined service channel
                    var serviceChannelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
                    var serviceQuery = serviceChannelRep.All().Where(s => s.ServiceChannelId.Equals(id));
                    if (date.HasValue)
                    {
                        serviceQuery = serviceQuery.Where(s => s.Modified > date);
                    }
                    var serviceList = serviceQuery.Select(c => c.ServiceId).ToList();
                    IList<Expression<Func<ServiceVersioned, bool>>> filters = new List<Expression<Func<ServiceVersioned, bool>>>
                    {
                        c => serviceList.Contains(c.UnificRootId)
                    };
                    result = GetServicesWithDetails(unitOfWork, filters, openApiVersion);
                });
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting services by service channel id {0}. {1}", id, ex.Message);
                logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
            return result;
        }

        public IVmOpenApiServiceVersionBase GetServiceBySource(string sourceId)
        {
            var userId = utilities.GetRelationIdForExternalSource();
            Guid? rootId = null;
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    rootId = GetPTVId<Service>(sourceId, userId, unitOfWork);

                });
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting services by source id {0}. {1}", sourceId, ex.Message);
                logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
            return rootId.HasValue ? GetServiceByIdSimple(rootId.Value, false) : null;
        }

        public IVmOpenApiServiceBase AddService(IVmOpenApiServiceInVersionBase vm, bool allowAnonymous, int openApiVersion, bool attachProposedChannels)
        {
            var service = new ServiceVersioned();
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = utilities.GetRelationIdForExternalSource();
            var useOtherEndPoint = false;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Check if the external source already exists. Let's not throw the excpetion here to avoid contextManager to catch the exception.
                useOtherEndPoint = ExternalSourceExists<Service>(vm.SourceId, userId, unitOfWork);
                if (!useOtherEndPoint)
                {
                    CheckVm(vm, unitOfWork, attachProposedChannels, true); // Includes checking general description data!
                    service = TranslationManagerToEntity.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(vm, unitOfWork);

                    // Add connections for defined service channels (PTV-2317)
                    if (vm.ServiceServiceChannels?.Count > 0)
                    {
                       service.UnificRoot.ServiceServiceChannels = TranslationManagerToEntity.TranslateAll<V7VmOpenApiServiceServiceChannelAstiInBase, ServiceServiceChannel>(vm.ServiceServiceChannels, unitOfWork).ToList();
                    }

                    var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    serviceRep.Add(service);

                    // Create the mapping between external source id and PTV id
                    if (!string.IsNullOrEmpty(vm.SourceId))
                    {
                        SetExternalSource(service.UnificRoot, vm.SourceId, userId, unitOfWork);
                    }

                    unitOfWork.Save(saveMode);
                }
            });

            if (useOtherEndPoint)
            {
                throw new ExternalSourceExistsException(string.Format(CoreMessages.OpenApi.ExternalSourceExists, vm.SourceId));
            }

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = commonService.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(service.Id, i => i.ServiceVersionedId == service.Id);
            }

            return GetServiceWithDetails(service.Id, openApiVersion, false);
        }

        public IVmOpenApiServiceBase SaveService(IVmOpenApiServiceInVersionBase vm, bool allowAnonymous, int openApiVersion, bool attachProposedChannels, string sourceId = null)
        {
            if (vm == null) return null;

            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = utilities.GetRelationIdForExternalSource();
            IVmOpenApiServiceBase result = new VmOpenApiServiceBase();
            ServiceVersioned service = null;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Get the root id according to source id (if defined)
                var rootId = vm.Id ?? GetPTVId<Service>(sourceId, userId, unitOfWork);

                // Get right version id
                vm.Id = versioningManager.GetVersionId<ServiceVersioned>(unitOfWork, rootId, null, false);

                if (vm.Id.IsAssigned())
                {
                    CheckVm(vm, unitOfWork, attachProposedChannels, rootId: rootId);

                    if (vm.PublishingStatus == PublishingStatus.Deleted.ToString())
                    {
                        service = DeleteService(unitOfWork, vm.Id);
                    }
                    else
                    {
                        // Entity needs to be restored?
                        if (vm.CurrentPublishingStatus == PublishingStatus.Deleted.ToString())
                        {
                            if (vm.PublishingStatus == PublishingStatus.Modified.ToString() || vm.PublishingStatus == PublishingStatus.Published.ToString())
                            {
                                // We need to restore already archived item
                                var publishingResult = commonService.RestoreArchivedEntity<ServiceVersioned>(unitOfWork, vm.Id.Value);
                            }
                        }

                        service = TranslationManagerToEntity.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(vm, unitOfWork);

                        // Add connections for defined service channels (PTV-2315)
                        if (vm.ServiceServiceChannels?.Count > 0)
                        {
                            vm.ServiceServiceChannels.ForEach(s => s.ServiceGuid = service.UnificRootId);
                            var relations = new V7VmOpenApiServiceAndChannelRelationAstiInBase { ChannelRelations = vm.ServiceServiceChannels.ToList(), ServiceId = service.UnificRootId };
                            service.UnificRoot = TranslationManagerToEntity.Translate<V7VmOpenApiServiceAndChannelRelationAstiInBase, Service>(relations, unitOfWork);
                        }

                        if (vm.CurrentPublishingStatus == PublishingStatus.Draft.ToString())
                        {
                            // We need to manually remove items from collections!
                            if (vm.Areas.Count > 0)
                            {
                                var municipalities = vm.Areas.Where(a => a.Type == AreaTypeEnum.Municipality.ToString()).ToList();
                                var otherAreas = vm.Areas.Where(a => a.Type != AreaTypeEnum.Municipality.ToString()).ToList();
                                if (municipalities.Count > 0)
                                {
                                    service.AreaMunicipalities = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.AreaMunicipalities,
                                    query => query.ServiceVersionedId == service.Id, area => area.MunicipalityId);
                                    if (otherAreas.Count == 0)
                                    {
                                        // Remove all possible old areas
                                        dataUtils.RemoveItemCollection<ServiceArea>(unitOfWork, s => s.ServiceVersionedId == service.Id);
                                    }
                                }
                                if (otherAreas.Count > 0)
                                {
                                    service.Areas = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.Areas,
                                        query => query.ServiceVersionedId == service.Id, area => area.AreaId);
                                    if (municipalities.Count == 0)
                                    {
                                        // Remove all possible old municipalities
                                        dataUtils.RemoveItemCollection<ServiceAreaMunicipality>(unitOfWork, s => s.ServiceVersionedId == service.Id);
                                    }
                                }
                            }
                            else if (!vm.AreaType.IsNullOrEmpty() && vm.AreaType != AreaInformationTypeEnum.AreaType.ToString())
                            {
                                // We need to remove possible old areas and municipalities
                                dataUtils.RemoveItemCollection<ServiceArea>(unitOfWork, s => s.ServiceVersionedId == service.Id);
                                dataUtils.RemoveItemCollection<ServiceAreaMunicipality>(unitOfWork, s => s.ServiceVersionedId == service.Id);
                            }

                            if (vm.Languages?.Count > 0)
                            {
                                service.ServiceLanguages = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceLanguages,
                                    query => query.ServiceVersionedId == service.Id, language => language.LanguageId);
                            }
                            if (vm.ServiceClasses?.Count > 0)
                            {
                                service.ServiceServiceClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceServiceClasses,
                                    query => query.ServiceVersionedId == service.Id, serviceClass => serviceClass.ServiceClass != null ? serviceClass.ServiceClass.Id : serviceClass.ServiceClassId);
                            }
                            if (vm.TargetGroups?.Count > 0)
                            {
                                service.ServiceTargetGroups = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceTargetGroups,
                                    query => query.ServiceVersionedId == service.Id, targetGroup => targetGroup.TargetGroup != null ? targetGroup.TargetGroup.Id : targetGroup.TargetGroupId);
                            }
                            if (vm.DeleteAllLifeEvents || (vm.LifeEvents?.Count > 0))
                            {
                                var updatedEvents = service.ServiceLifeEvents.Select(l => l.LifeEventId).ToList();
                                var rep = unitOfWork.CreateRepository<IServiceLifeEventRepository>();
                                var currentItems = rep.All().Where(s => s.ServiceVersionedId == service.Id).ToList();
                                var toRemove = currentItems.Where(e => !updatedEvents.Contains(e.LifeEventId));
                                toRemove.ForEach(i => rep.Remove(i));
                            }
                            if (vm.DeleteAllIndustrialClasses || (vm.IndustrialClasses?.Count > 0))
                            {
                                var updatedClasses = service.ServiceIndustrialClasses.Select(l => l.IndustrialClassId).ToList();
                                var rep = unitOfWork.CreateRepository<IServiceIndustrialClassRepository>();
                                var currentItems = rep.All().Where(s => s.ServiceVersionedId == service.Id).ToList();
                                var toRemove = currentItems.Where(e => !updatedClasses.Contains(e.IndustrialClassId));
                                toRemove.ForEach(i => rep.Remove(i));
                            }
                            if (vm.OntologyTerms?.Count > 0)
                            {
                                service.ServiceOntologyTerms = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceOntologyTerms,
                                    query => query.ServiceVersionedId == service.Id, term => term.OntologyTerm != null ? term.OntologyTerm.Id : term.OntologyTermId);
                            }
                            if (vm.DeleteAllLaws || vm.Legislation?.Count > 0)
                            {
                                // Delete all law related names and web pages that were not included in vm
                                List<Guid> updatedServiceLaws = service.ServiceLaws.Select(l => l.LawId).ToList();
                                List<Law> updatedLaws = service.ServiceLaws.Select(l => l.Law).ToList();
                                var rep = unitOfWork.CreateRepository<IServiceLawRepository>();
                                var lawRep = unitOfWork.CreateRepository<ILawRepository>();
                                var lawNameRep = unitOfWork.CreateRepository<ILawNameRepository>();
                                var webPageRep = unitOfWork.CreateRepository<IWebPageRepository>();
                                var currentServiceLaws = unitOfWork.ApplyIncludes(rep.All().Where(s => s.ServiceVersionedId == service.Id), q => q.Include(i => i.Law)).ToList();
                                currentServiceLaws.ForEach(l =>
                                {
                                    if (updatedServiceLaws.Contains(l.LawId))
                                    {
                                        // Check names and webPages lists for removed items
                                        var updatedLaw = updatedLaws.Where(s => s.Id == l.LawId).FirstOrDefault();
                                        var updatedWebPages = updatedLaw.WebPages.Select(w => w.WebPageId).ToList();

                                        var updatedNames = updatedLaw.Names.Select(n => new { n.LawId, n.LocalizationId }).ToList();
                                        var currentLaw = unitOfWork.ApplyIncludes(lawRep.All().Where(w => w.Id == l.LawId), q => q.Include(i => i.Names).Include(i => i.WebPages)).FirstOrDefault();
                                        // Delete the web pages that were not included in updated webpages
                                        currentLaw.WebPages.Where(w => !updatedWebPages.Contains(w.WebPageId)).ForEach(w => webPageRep.Remove(w.WebPage));
                                        // Delete all names that were not included in updated names

                                        currentLaw.Names.Where(n => !updatedNames.Any(un => un.LawId == n.LawId && un.LocalizationId == n.LocalizationId)).ForEach(n => lawNameRep.Remove(n));
                                    }
                                    else
                                    {
                                    // The item was removed from service laws so let's remove all webPages and names also.
                                    l.Law.WebPages.ForEach(w => webPageRep.Remove(w.WebPage));
                                        l.Law.Names.ForEach(n => lawNameRep.Remove(n));
                                        lawRep.Remove(l.Law);
                                    }
                                });
                            }

                            if (vm.DeleteAllKeywords || vm.Keywords?.Count > 0)
                            {
                                var updatedwords = service.ServiceKeywords.Select(l => l.KeywordId).ToList();
                                var rep = unitOfWork.CreateRepository<IServiceKeywordRepository>();
                                var currentItems = rep.All().Where(s => s.ServiceVersionedId == service.Id).ToList();
                                var toRemove = currentItems.Where(e => !updatedwords.Contains(e.KeywordId));
                                toRemove.ForEach(i => rep.Remove(i));
                            }
                        }

                        // Update the mapping between external source id and PTV id
                        if (!string.IsNullOrEmpty(vm.SourceId))
                        {
                            UpdateExternalSource<Service>(service.UnificRootId, vm.SourceId, userId, unitOfWork);
                        }
                    }

                    unitOfWork.Save(saveMode, service);
                }

            });

            if (service == null) return null;

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = commonService.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(service.Id, i => i.ServiceVersionedId == service.Id);
            }

            return GetServiceWithDetails(service.Id, openApiVersion, false);
        }

        public bool ServiceExists(Guid serviceId)
        {
            bool srvExists = false;

            if (Guid.Empty == serviceId)
            {
                return srvExists;
            }

            contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceRepo = unitOfWork.CreateRepository<IServiceRepository>().All();

                if (serviceRepo.FirstOrDefault(s => s.Id.Equals(serviceId)) != null)
                {
                    srvExists = true;
                }
            });

            return srvExists;
        }

        /// <summary>
        /// Returns a list of services that do not exist (within idList).
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public List<Guid> CheckServices(List<Guid> idList)
        {
            List<Guid> existingServices = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
                var query = unitOfWork.CreateRepository<IServiceVersionedRepository>().All().Where(s => idList.Contains(s.UnificRootId) && s.PublishingStatusId == publishedId);
                
                existingServices = query.Select(s => s.UnificRootId).ToList();
            });

            if (existingServices?.Count > 0) return idList.Where(i => !existingServices.Contains(i)).ToList();

            return idList;
        }


        private IVmOpenApiServiceVersionBase GetServiceWithDetails(IUnitOfWork unitOfWork, Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            //return GetServicesWithDetails(unitOfWork, new List<Guid> { versionId }, openApiVersion, getOnlyPublished).FirstOrDefault();
            //// Measure
            //var watch = new Stopwatch();var totalWatch = new Stopwatch();
            //logger.LogTrace("****************************************");
            //logger.LogTrace($"GetServiceWithDetails starts. Id: {versionId}");
            //watch.Start(); totalWatch.Start();
            //// end measure
            IVmOpenApiServiceVersionBase result = null;
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var query = serviceRep.All().Where(s => s.Id == versionId);
            if (getOnlyPublished)
            {
                query = query.Where(c => c.LanguageAvailabilities.Any(l => l.StatusId == publishedId));
            }
            var service = unitOfWork.ApplyIncludes(query, GetServiceIncludeChain()).FirstOrDefault();

            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Fetch from db: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// end measure

            if (service != null)
            {
                // Find only published organizations for services and map the data
                GetAndMapOrganizationsForServices(new List<ServiceVersioned> { service }, unitOfWork);

                ////Measure
                //watch.Stop();
                //logger.LogTrace($"*** Get organizations from db: {watch.ElapsedMilliseconds} ms.");
                //watch.Restart();
                //// End measure

                // Filter out not published language versions
                FilterOutNotPublishedLanguageVersions(service, publishedId, getOnlyPublished);

                ////Measure
                //watch.Stop();
                //logger.LogTrace($"*** Filtering: {watch.ElapsedMilliseconds} ms.");
                //watch.Restart();
                //// End measure

                // HACK: don't use translator for service channels
                // this is because the translator kills the performance when there are many items in the collection(s)
                // For example it takes roughly 0.002ms to translate an item and there are 1000 items in the collection. It takes 2 seconds to translate the collection

                result = TranslationManagerToVm.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(service);

                ////Measure
                //watch.Stop();
                //logger.LogTrace($"*** Translation: {watch.ElapsedMilliseconds} ms.");
                //watch.Restart();
                //// End measure                
            }

            if (result == null)
            {
                return null;
            }

            // Find only published service collections for a service (which have at least one published language version)
            result.ServiceCollections = GetServiceCollections(new List<Guid> { service.UnificRootId }, unitOfWork);

            // Find only published service channels for a service
            result.ServiceChannels = GetServiceChannels(new List<Guid> { service.UnificRootId }, unitOfWork);
            
            ////Measure
            //watch.Stop(); totalWatch.Stop();
            //logger.LogTrace($"*** Get connections from db: {watch.ElapsedMilliseconds} ms.");
            //logger.LogTrace($"*** Total: {totalWatch.ElapsedMilliseconds} ms.");
            //logger.LogTrace($"***************************************************.");
            //// End measure

            return GetServiceByOpenApiVersion(unitOfWork, result, openApiVersion);
        }

        private void GetAndMapOrganizationsForServices(List<ServiceVersioned> services, IUnitOfWork unitOfWork)
        {
            if (services == null || services.Count == 0) return;
            // Find only published organizations for services
            var allOrganizationIds = services.SelectMany(service => service.OrganizationServices
                .Where(i => i.OrganizationId != null)).Select(o => o.OrganizationId).Distinct().ToList();
            var serviceProducers = services.SelectMany(service => service.ServiceProducers.SelectMany(s => s.Organizations)
                .Where(s => s.OrganizationId != null)).Select(o => o.OrganizationId).Distinct().ToList();
            allOrganizationIds.AddRange(serviceProducers.Except(allOrganizationIds));

            // Let's also include main responsible organization
            var mainOrganizations = services.Select(service => service.OrganizationId).Distinct().ToList();
            allOrganizationIds.AddRange(mainOrganizations.Except(allOrganizationIds));

            var publishedOrganizations = new List<OrganizationVersioned>();
            var publishedOrganizationRootIds = new List<Guid>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            // Get published organizations and the names
            var organizationVersionedRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            publishedOrganizations = unitOfWork.ApplyIncludes(
                organizationVersionedRep.All().Where(i => allOrganizationIds.Contains(i.UnificRootId) &&
                i.PublishingStatusId == publishedId &&
                i.LanguageAvailabilities.Any(l => l.StatusId == publishedId)), // Filter out organizations with no language versions published
                q => q.Include(i => i.OrganizationNames).Include(i => i.LanguageAvailabilities)).ToList();
            publishedOrganizationRootIds = publishedOrganizations.Select(o => o.UnificRootId).ToList();

            // Map organization data for all services. Also filter out organizations that are not published.
            services.ForEach(service =>
            {
                // Filter out not published organizations
                service.OrganizationServices = service.OrganizationServices.Where(i => publishedOrganizationRootIds.Contains(i.OrganizationId)).ToList();
                // Map service organizations
                if (publishedOrganizations?.Count > 0)
                {
                    service.OrganizationServices.ForEach(os =>
                    {
                        var organizationVersioned = publishedOrganizations.Where(po => po.UnificRootId == os.OrganizationId).FirstOrDefault();
                        if (organizationVersioned != null)
                        {
                            os.Organization = new Organization { Id = os.OrganizationId };
                            os.Organization.Versions.Add(organizationVersioned);
                        }

                    });
                }
                // Map service producers
                service.ServiceProducers.Where(sp => sp.Organizations != null).ForEach(producer =>
                {
                    // Filter out not published organizations
                    producer.Organizations = producer.Organizations.Where(o => publishedOrganizationRootIds.Contains(o.OrganizationId)).ToList();
                    // Map organization
                    if (publishedOrganizations?.Count > 0)
                    {
                        producer.Organizations.ForEach(org =>
                        {
                            var organizationVersioned = publishedOrganizations.Where(po => po.UnificRootId == org.OrganizationId).FirstOrDefault();
                            if (organizationVersioned != null)
                            {
                                org.Organization = new Organization { Id = org.OrganizationId };
                                org.Organization.Versions.Add(organizationVersioned);
                            }
                        });
                    }
                });
                // Map main organization
                if (publishedOrganizations?.Count > 0)
                {

                    var mainOrganization = publishedOrganizations.Where(o => o.UnificRootId == service.OrganizationId).FirstOrDefault();
                    if (mainOrganization != null)
                    {
                        service.Organization.Versions.Add(mainOrganization);
                    }
                    else
                    {
                        service.Organization = null;
                    }
                }
                else { service.Organization = null; }
            });
        }

        private IVmOpenApiServiceVersionBase GetServiceWithDetails(Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiServiceVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetServiceWithDetails(unitOfWork, versionId, openApiVersion, getOnlyPublished);
            });

            return result;
        }

        private IList<IVmOpenApiServiceVersionBase> GetServicesWithDetailsOld(IUnitOfWork unitOfWork, List<Guid> versionIdList, int openApiVersion, bool getOnlyPublished = true)
        {
            if (versionIdList.Count == 0) return new List<IVmOpenApiServiceVersionBase>();

            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var resultTemp = unitOfWork.ApplyIncludes(serviceRep.All().Where(s => versionIdList.Contains(s.Id)), q =>
                q.Include(i => i.ServiceLanguages).ThenInclude(i => i.Language)
                .Include(i => i.ServiceNames)
                .Include(i => i.ServiceDescriptions)
                .Include(i => i.ServiceServiceClasses).ThenInclude(i => i.ServiceClass).ThenInclude(i => i.Names)
                .Include(i => i.ServiceServiceClasses).ThenInclude(i => i.ServiceClass).ThenInclude(i => i.Descriptions)
                .Include(i => i.ServiceOntologyTerms).ThenInclude(i => i.OntologyTerm).ThenInclude(i => i.Names)
                .Include(i => i.ServiceTargetGroups).ThenInclude(i => i.TargetGroup).ThenInclude(i => i.Names)
                .Include(i => i.ServiceLifeEvents).ThenInclude(i => i.LifeEvent).ThenInclude(i => i.Names)
                .Include(i => i.ServiceIndustrialClasses).ThenInclude(i => i.IndustrialClass).ThenInclude(i => i.Names)
                .Include(i => i.ServiceKeywords).ThenInclude(i => i.Keyword)
                .Include(i => i.ServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.Names)
                .Include(i => i.ServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.WebPages).ThenInclude(i => i.WebPage)
                .Include(i => i.ServiceRequirements)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Versions)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDescriptions)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDigitalAuthorizations)
                    .ThenInclude(i => i.DigitalAuthorization).ThenInclude(i => i.Names)
                .Include(i => i.OrganizationServices).ThenInclude(i => i.Organization).ThenInclude(i => i.Versions)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelExtraTypes).ThenInclude(i => i.ExtraSubType)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelExtraTypes).ThenInclude(i => i.ServiceServiceChannelExtraTypeDescriptions)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelServiceHours).ThenInclude(i => i.ServiceHours)
                    .ThenInclude(i => i.DailyOpeningTimes)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelServiceHours).ThenInclude(i => i.ServiceHours)
                    .ThenInclude(i => i.AdditionalInformations)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelEmails).ThenInclude(i => i.Email)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelPhones).ThenInclude(i => i.Phone).ThenInclude(i => i.PrefixNumber)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelWebPages).ThenInclude(i => i.WebPage)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.AddressStreets).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.AddressAdditionalInformations)
                .Include(i => i.ServiceProducers).ThenInclude(i => i.AdditionalInformations)
                .Include(i => i.ServiceProducers).ThenInclude(i => i.Organizations).ThenInclude(i => i.Organization).ThenInclude(i => i.Versions)
                .Include(i => i.ServiceWebPages).ThenInclude(i => i.WebPage)
                .Include(i => i.LanguageAvailabilities)
                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaNames)
                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceCollectionServices).ThenInclude(i => i.ServiceCollectionVersioned).ThenInclude(i => i.LanguageAvailabilities)
                .Include(i => i.Organization).ThenInclude(i => i.Versions)
                .OrderByDescending(i => i.Modified));

            // Filter out items that do not have language versions published!
            var services = getOnlyPublished ? resultTemp.Where(c => c.LanguageAvailabilities.Any(l => l.StatusId == publishedId)).ToList() : resultTemp.ToList();

            // Find only published service channels for services
            var allChannels = services.SelectMany(i => i.UnificRoot.ServiceServiceChannels).Select(i => i.ServiceChannel)
                .SelectMany(i => i.Versions).Where(i => i.PublishingStatusId == publishedId).ToList();
            var channelsIds = allChannels.Select(i => i.Id).ToList();
            var publishedServiceChannelRootIds = new List<Guid>();
            if (channelsIds.Count > 0)
            {
                var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                publishedServiceChannelRootIds = serviceChannelRep.All().Where(c => channelsIds.Contains(c.Id))
                    .Where(s => s.LanguageAvailabilities.Any(l => l.StatusId == publishedId)) // Filter out channels with no language versions published
                    .Select(c => c.UnificRootId).ToList();
            }

            // Find only published service collections for services (which have at least one published language version)
            var publishedServiceCollections = services.SelectMany(i => i.UnificRoot.ServiceCollectionServices).Select(i => i.ServiceCollectionVersioned)
                .Where(i => i.PublishingStatusId == publishedId && i.LanguageAvailabilities.Any(l => l.StatusId == publishedId)).ToList();
            var publishedServiceCollectionIds = publishedServiceCollections.Select(i => i.Id).ToList();

            // Find only published organizations for services
            var allOrganizations = services.SelectMany(i => i.OrganizationServices).Where(i => i.Organization != null).Select(i => i.Organization)
                .SelectMany(i => i.Versions).Where(i => i.PublishingStatusId == publishedId).ToList();
            var serviceProducerOrganizations = services.SelectMany(i => i.ServiceProducers).SelectMany(i => i.Organizations).Where(i => i.Organization != null).Select(i => i.Organization)
                .SelectMany(i => i.Versions).Where(i => i.PublishingStatusId == publishedId && !allOrganizations.Contains(i)).ToList();
            allOrganizations.AddRange(serviceProducerOrganizations);
            // Let's also include main responsible organization
            allOrganizations.AddRange(services.Select(i => i.Organization).SelectMany(i => i.Versions).Where(i => i.PublishingStatusId == publishedId).ToList());
            var organizationIds = allOrganizations.Select(i => i.Id).ToList();
            var publishedOrganizationRootIds = new List<Guid>();
            if (organizationIds.Count > 0)
            {
                var organizationVersionedRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                publishedOrganizationRootIds = organizationVersionedRep.All().Where(i => organizationIds.Contains(i.Id))
                    .Where(s => s.LanguageAvailabilities.Any(l => l.StatusId == publishedId)) // Filter out organizations with no language versions published
                    .Select(i => i.UnificRootId).ToList();
            }

            services.ForEach(service =>
            {
                // Filter out not published serviceChannels
                service.UnificRoot.ServiceServiceChannels = service.UnificRoot.ServiceServiceChannels.Where(c => publishedServiceChannelRootIds.Contains(c.ServiceChannelId)).ToList();

                // Filter out not published service collections
                service.UnificRoot.ServiceCollectionServices = service.UnificRoot.ServiceCollectionServices
                    .Where(c => c.ServiceCollectionVersioned.PublishingStatusId == publishedId)
                    .GroupBy(c => c.ServiceCollectionVersioned.UnificRootId)
                    .Select(c => c.FirstOrDefault())
                    .ToList();

                // Filter out not published organizations
                service.OrganizationServices = service.OrganizationServices.Where(i => publishedOrganizationRootIds.Contains(i.OrganizationId)).ToList();
                foreach (var producer in service.ServiceProducers.Where(sp => sp.Organizations != null))
                {
                    producer.Organizations = producer.Organizations.Where(o => publishedOrganizationRootIds.Contains(o.OrganizationId)).ToList();
                }

                // Filter out not published language versions
                FilterOutNotPublishedLanguageVersions(service, publishedId, getOnlyPublished);
            });

            // Fill with service channel names
            var channelNames = unitOfWork.CreateRepository<IServiceChannelNameRepository>().All().Where(i => channelsIds.Contains(i.ServiceChannelVersionedId)).ToList()
                .GroupBy(i => i.ServiceChannelVersionedId).ToDictionary(i => i.Key, i => i.ToList());
            allChannels.ForEach(c =>
            {
                c.ServiceChannelNames = channelNames.TryGet(c.Id);
            });

            // Fill with organization names
            var organizationNames = unitOfWork.CreateRepository<IOrganizationNameRepository>().All().Where(i => organizationIds.Contains(i.OrganizationVersionedId)).ToList()
                .GroupBy(i => i.OrganizationVersionedId).ToDictionary(i => i.Key, i => i.ToList());
            allOrganizations.ForEach(o =>
            {
                o.OrganizationNames = organizationNames.TryGet(o.Id);
            });

            // Fill with service collection names
            var serviceCollectionNames = unitOfWork.CreateRepository<IServiceCollectionNameRepository>().All()
                .Where(i => publishedServiceCollectionIds.Contains(i.ServiceCollectionVersionedId)).ToList()
                .GroupBy(i => i.ServiceCollectionVersionedId).ToDictionary(i => i.Key, i => i.ToList());
            publishedServiceCollections.ForEach(o =>
            {
                o.ServiceCollectionNames = serviceCollectionNames.TryGet(o.Id);
            });

            var result = TranslationManagerToVm.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(services);
            if (result == null)
            {
                throw new Exception(CoreMessages.OpenApi.RecordNotFound);
            }

            // Get the right open api view model version
            var versionList = new List<IVmOpenApiServiceVersionBase>();
            result.ForEach(service =>
            {
                versionList.Add(GetServiceByOpenApiVersion(unitOfWork, service, openApiVersion));
            });

            return versionList;
        }

        private IList<IVmOpenApiServiceVersionBase> GetServicesWithDetails(IUnitOfWork unitOfWork, IList<Expression<Func<ServiceVersioned, bool>>> filters, int openApiVersion, bool getOnlyPublished = true)
        {
            //// Measure
            //var watch = new Stopwatch();var totalWatch = new Stopwatch();
            //logger.LogTrace("****************************************");
            //logger.LogTrace($"GetServicesWithDetails - a list");
            //watch.Start(); totalWatch.Start();
            //// end measure

            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            // Get only published items -  filter out items that do not have any language versions published.
            filters.Add(e => e.PublishingStatusId == publishedId && e.LanguageAvailabilities.Any(l => l.StatusId == publishedId));

            var query = serviceRep.All();
            filters.ForEach(a => query = query.Where(a));

            var totalCount = query.Count();
            if (totalCount > 100)
            {
                throw new Exception(CoreMessages.OpenApi.TooManyItems);
            }
            if (totalCount == 0)
            {
                return null;
            }

            var services = unitOfWork.ApplyIncludes(query, GetServiceIncludeChain()).ToList();

            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Fetch from db: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// end measure

            // Find only published organizations for services and map data
            GetAndMapOrganizationsForServices(services, unitOfWork);

            services.ForEach(service =>
            {
                // Filter out not published language versions
                FilterOutNotPublishedLanguageVersions(service, publishedId, getOnlyPublished);
            });

            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Filtering: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// End measure

            var result = TranslationManagerToVm.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(services);

            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Translation: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// End measure

            if (result == null)
            {
                throw new Exception(CoreMessages.OpenApi.RecordNotFound);
            }

            // Service root ids
            List<Guid> rootIds = result.Where(c => c.Id != null).Select(c => c.Id.Value).Distinct().ToList();
            
            // Find only published service collections for services (which have at least one published language version)
            var allCollections = GetServiceCollections(rootIds, unitOfWork);
            // Find only published service channels for services
            var allConnections = GetServiceChannels(rootIds, unitOfWork);

            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Get connections from db: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// End measure

            var versionList = new List<IVmOpenApiServiceVersionBase>();
            result.ForEach(service =>
            {
                // Map service collections
                if (allCollections?.Count > 0)
                {
                    var collections = allCollections.Where(c => c.OwnerReferenceId == service.Id).ToList();
                    if (collections?.Count > 0)
                    {
                        service.ServiceCollections = collections;
                    }
                }
                // Map connections
                if (allConnections?.Count > 0)
                {
                    var connections = allConnections.Where(s => s.OwnerReferenceId == service.Id).ToList();
                    if (connections?.Count > 0)
                    {
                        service.ServiceChannels = connections;
                    }
                }
                // Get the right open api view model version.
                versionList.Add(GetServiceByOpenApiVersion(unitOfWork, service, openApiVersion));
            });

            ////Measure
            //watch.Stop(); totalWatch.Stop();
            //logger.LogTrace($"*** Channels for services mapping: {watch.ElapsedMilliseconds} ms.");
            //logger.LogTrace($"*** Total: {totalWatch.ElapsedMilliseconds} ms.");
            //logger.LogTrace($"***************************************************.");
            //// End measure

            return versionList;
        }

        private List<VmOpenApiServiceServiceCollection> GetServiceCollections(List<Guid> rootIds, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            var list = new List<VmOpenApiServiceServiceCollection>();

            // Find only published service collections for a service (which have at least one published language version)
            var serviceCollectionRep = unitOfWork.CreateRepository<IServiceCollectionServiceRepository>();
            var collectionQuery = serviceCollectionRep.All().Where(c => rootIds.Contains(c.ServiceId) && c.ServiceCollectionVersioned.PublishingStatusId == publishedId &&
                c.ServiceCollectionVersioned.LanguageAvailabilities.Any(l => l.StatusId == publishedId));
            var allCollections = unitOfWork.ApplyIncludes(collectionQuery, q =>
                q.Include(i => i.ServiceCollectionVersioned)
                .ThenInclude(i => i.ServiceCollectionNames)).ToList();
            allCollections.ForEach(collection =>
            {
                var vm = new VmOpenApiServiceServiceCollection
                {
                    Id = collection.ServiceCollectionVersioned.UnificRootId,
                    Name = TranslationManagerToVm.TranslateAll<ServiceCollectionName, VmOpenApiLanguageItem>(collection.ServiceCollectionVersioned.ServiceCollectionNames).ToList(),
                    OwnerReferenceId = collection.ServiceId
                };
                list.Add(vm);
            });

            return list;
        }

        private List<V8VmOpenApiServiceServiceChannel> GetServiceChannels(List<Guid> rootIds, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            // Find only published service channels for services
            var connectionRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var connectionQuery = connectionRep.All().Where(c => rootIds.Contains(c.ServiceId) && c.ServiceChannel.Versions.Any(v => v.PublishingStatusId == publishedId && v.LanguageAvailabilities.Any(l => l.StatusId == publishedId)));
            var allConnections = unitOfWork.ApplyIncludes(connectionQuery, GetConnectionIncludeChain()).ToList();
            if (allConnections?.Count > 0)
            {
                var serviceList = new List<V8VmOpenApiServiceServiceChannel>();

                // Fill with service channel names
                var channelRootIds = allConnections.Select(s => s.ServiceChannelId).Distinct().ToList();

                var channels = unitOfWork.ApplyIncludes(
                    unitOfWork.CreateRepository<IServiceChannelVersionedRepository>().All().Where(i => channelRootIds.Contains(i.UnificRootId) && i.PublishingStatusId == publishedId),
                    q => q.Include(i => i.UnificRoot).Include(i => i.ServiceChannelNames).Include(i => i.LanguageAvailabilities)).ToList();

                allConnections.ForEach(connection =>
                {
                    string name = null;
                    var channel = channels.Where(i => i.UnificRootId == connection.ServiceChannelId).FirstOrDefault();
                    if (channel != null)
                    {
                        var version = channel.UnificRoot.Versions.FirstOrDefault();
                        if (version != null)
                        {
                            name = GetNameWithFallback(
                                version.ServiceChannelNames,
                                version.LanguageAvailabilities.Where(i => i.StatusId == publishedId).Select(i => i.LanguageId).ToList(),
                                typesCache,
                                languageCache);
                        }
                    }

                    V8VmOpenApiServiceServiceChannel vmssc = new V8VmOpenApiServiceServiceChannel
                    {
                        OwnerReferenceId = connection.ServiceId,
                        ServiceChannel = new VmOpenApiItem { Id = connection.ServiceChannelId, Name = name }
                    };

                    // map base connection data
                    MapConnection(connection, vmssc, typesCache, languageCache);

                    // contactdetails
                    vmssc.ContactDetails = GetContactDetails<V8VmOpenApiContactDetails>(connection, typesCache, languageCache);

                    // digitalAuthorizations
                    vmssc.DigitalAuthorizations = GetDigitalAuthorizations(connection, languageCache);

                    serviceList.Add(vmssc);
                });

                return serviceList;
            }

            return null;
        }

        private Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>> GetServiceIncludeChain()
        {
            return q =>
                q.Include(i => i.ServiceLanguages).ThenInclude(i => i.Language)
                .Include(i => i.ServiceNames)
                .Include(i => i.ServiceDescriptions)
                .Include(i => i.ServiceServiceClasses).ThenInclude(i => i.ServiceClass).ThenInclude(i => i.Names)
                .Include(i => i.ServiceServiceClasses).ThenInclude(i => i.ServiceClass).ThenInclude(i => i.Descriptions)
                .Include(i => i.ServiceOntologyTerms).ThenInclude(i => i.OntologyTerm).ThenInclude(i => i.Names)
                .Include(i => i.ServiceTargetGroups).ThenInclude(i => i.TargetGroup).ThenInclude(i => i.Names)
                .Include(i => i.ServiceLifeEvents).ThenInclude(i => i.LifeEvent).ThenInclude(i => i.Names)
                .Include(i => i.ServiceIndustrialClasses).ThenInclude(i => i.IndustrialClass).ThenInclude(i => i.Names)
                .Include(i => i.ServiceKeywords).ThenInclude(i => i.Keyword)
                .Include(i => i.ServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.Names)
                .Include(i => i.ServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.WebPages).ThenInclude(i => i.WebPage)
                .Include(i => i.ServiceRequirements)
                .Include(i => i.OrganizationServices)
                .Include(i => i.ServiceProducers).ThenInclude(i => i.AdditionalInformations)
                .Include(i => i.ServiceProducers).ThenInclude(i => i.Organizations)
                .Include(i => i.ServiceWebPages).ThenInclude(i => i.WebPage)
                .Include(i => i.LanguageAvailabilities)
                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaNames)
                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.Organization)
                .Include(i => i.UnificRoot);
        }

        private void FilterOutNotPublishedLanguageVersions(ServiceVersioned service, Guid publishedId, bool getOnlyPublished)
        {
            // Filter out not published language versions
            if (getOnlyPublished)
            {
                var notPublishedLanguageVersions = service.LanguageAvailabilities.Where(l => l.StatusId != publishedId).Select(l => l.LanguageId).ToList();
                if (notPublishedLanguageVersions.Count > 0)
                {
                    service.ServiceNames = service.ServiceNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    service.ServiceDescriptions = service.ServiceDescriptions.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    service.ServiceKeywords = service.ServiceKeywords.Where(i => !notPublishedLanguageVersions.Contains(i.Keyword.LocalizationId)).ToList();
                    service.ServiceLaws.ForEach(law =>
                    {
                        law.Law.Names = law.Law.Names.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        law.Law.WebPages = law.Law.WebPages.Where(i => !notPublishedLanguageVersions.Contains(i.WebPage.LocalizationId)).ToList();
                    });
                    service.ServiceRequirements = service.ServiceRequirements.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    service.UnificRoot.ServiceServiceChannels.ForEach(channel =>
                    {
                        channel.ServiceServiceChannelDescriptions = channel.ServiceServiceChannelDescriptions.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    });
                    service.ServiceProducers.ForEach(sp =>
                    {
                        sp.AdditionalInformations = sp.AdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    });
                    service.ServiceWebPages = service.ServiceWebPages.Where(i => !notPublishedLanguageVersions.Contains(i.WebPage.LocalizationId)).ToList();
                }
            }
        }

        private IVmOpenApiServiceVersionBase GetServiceByOpenApiVersion(IUnitOfWork unitOfWork, IVmOpenApiServiceVersionBase baseVersion, int openApiVersion)
        {
            // Fill with the general description related data
            FillWithGeneralDescriptionData(baseVersion, unitOfWork);
            // Get the sourceId if user is logged in
            var userId = utilities.GetRelationIdForExternalSource(false);
            if (!string.IsNullOrEmpty(userId))
            {
                baseVersion.SourceId = GetSourceId<Service>(baseVersion.Id.Value, userId, unitOfWork);
            }
            return GetEntityByOpenApiVersion(baseVersion, openApiVersion);
        }

        private IVmOpenApiServiceVersionBase GetServiceWithSimpleDetails(IUnitOfWork unitOfWork, Guid versionId)
        {
            if (!versionId.IsAssigned()) return null;

            ServiceVersioned entity = null;
            return GetModel<ServiceVersioned, VmOpenApiServiceVersionBase>(entity = GetEntity<ServiceVersioned>(versionId, unitOfWork,
                    q => q.Include(x => x.LanguageAvailabilities)
                    .Include(i => i.ServiceTargetGroups).ThenInclude(i => i.TargetGroup)), unitOfWork);
        }

        private void CheckVm(IVmOpenApiServiceInVersionBase vm, IUnitOfWorkWritable unitOfWork, bool attachProposedChannels, bool createOperation = false, Guid? rootId = null)
        {
            CheckKeywords(vm, unitOfWork);
            CheckTargetGroups(vm.TargetGroups, unitOfWork);

            // Check general description related data.
            // In PUT method the view model may not include general description even the service has earlier been attached into a general description.
            // Therefore if the request viewmodel does not include general description id let's get the general description related to the service from db.
            var generalDescriptionID = vm.GeneralDescriptionId.ParseToGuid();
            if (!generalDescriptionID.IsAssigned() && rootId.IsAssigned())
            {
                // Let's try to get the statutory general description attached for service from db.
                var service = versioningManager.GetSpecificVersionByRoot<ServiceVersioned>(unitOfWork, rootId.Value, PublishingStatus.Published);
                if (service != null)
                {
                    generalDescriptionID = service.StatutoryServiceGeneralDescriptionId;
                }
                else
                {
                    service = versioningManager.GetSpecificVersionByRoot<ServiceVersioned>(unitOfWork, rootId.Value, PublishingStatus.Draft);
                    if (service != null)
                    {
                        generalDescriptionID = service.StatutoryServiceGeneralDescriptionId;
                    }
                }
            }
            if (generalDescriptionID.IsAssigned())
            {
                // Get the general description
                var generalDescription = generalDescriptionService.GetGeneralDescriptionVersionBase(generalDescriptionID.Value, 0);
                if (generalDescription != null)
                {
                    // If name is defined within general description and service name is empty let's copy general decription name into service - only when creating the object!
                    if (generalDescription.Names?.Count() > 0 && (vm.ServiceNames == null || vm.ServiceNames?.Count() == 0) && createOperation)
                    {
                        vm.ServiceNames = generalDescription.Names.ToList();
                    }

                    // If service type is defined within general description, service related type is ignored
                    if (!string.IsNullOrEmpty(generalDescription.Type))
                    {
                        vm.Type = null;
                    }

                    // If service charge type is defined within general description, service related service charge type is ignored
                    if (!string.IsNullOrEmpty(generalDescription.ServiceChargeType))
                    {
                        vm.ServiceChargeType = null;
                    }

                    // Check finto items so that service and general description does not include overlapping finto items
                    vm.ServiceClasses = CheckFintoItems(vm.ServiceClasses, generalDescription.ServiceClasses.ToList());
                    vm.OntologyTerms = CheckFintoItems(vm.OntologyTerms, generalDescription.OntologyTerms.ToList());
                    vm.TargetGroups = CheckFintoItems(vm.TargetGroups, generalDescription.TargetGroups.ToList());
                    vm.LifeEvents = CheckFintoItems(vm.LifeEvents, generalDescription.LifeEvents.ToList());
                    vm.IndustrialClasses = CheckFintoItems(vm.IndustrialClasses, generalDescription.IndustrialClasses.ToList());

                    // attach proposed service channels into service (PTV-2315)
                    if (attachProposedChannels && generalDescription.ServiceChannels?.Count > 0)
                    {
                        if (vm.ServiceServiceChannels == null)
                        {
                            vm.ServiceServiceChannels = new List<V7VmOpenApiServiceServiceChannelAstiInBase>();
                        }
                        generalDescription.ServiceChannels.ForEach(channel =>
                        {
                            // do we already have the item in a channel list?
                            var item = vm.ServiceServiceChannels.Where(i => i.ChannelGuid == channel.ServiceChannel.Id.Value).FirstOrDefault();
                            var daList = channel.DigitalAuthorizations?.Count > 0 ? channel.DigitalAuthorizations.Select(i => i.Id).ToList() : new List<Guid>();
                            VmOpenApiContactDetailsInBase contactDetails = null;
                            if (channel.ContactDetails != null)
                            {
                                contactDetails = new VmOpenApiContactDetailsInBase { Emails = channel.ContactDetails.Emails, Phones = channel.ContactDetails.PhoneNumbers, WebPages = channel.ContactDetails.WebPages };
                                if (channel.ContactDetails.Addresses?.Count > 0)
                                {
                                    channel.ContactDetails.Addresses.ForEach(a => { contactDetails.Addresses.Add(a.ConvertToInModel()); });
                                }
                            }
                            if (item == null)
                            {
                                vm.ServiceServiceChannels.Add(new V7VmOpenApiServiceServiceChannelAstiInBase
                                {
                                    ChannelGuid = channel.ServiceChannel.Id.Value,
                                    ServiceChargeType = channel.ServiceChargeType,
                                    Description = channel.Description,
                                    DigitalAuthorizations = daList,
                                    ServiceHours = channel.ServiceHours,
                                    ContactDetails = contactDetails
                                });
                            }
                            else
                            {
                                item.ServiceChargeType = channel.ServiceChargeType;
                                item.Description = channel.Description;
                                item.DigitalAuthorizations = daList;
                                item.ServiceHours = channel.ServiceHours;
                                item.ContactDetails = contactDetails;
                            }
                        });
                    }
                }
            }
        }

        private IList<string> CheckFintoItems<TFintoModel>(IList<string> list, List<TFintoModel> fintoItemList) where TFintoModel : IVmOpenApiFintoItemVersionBase
        {
            if (list?.Count == 0 || fintoItemList?.Count == 0)
            {
                return list;
            }

            var newList = new List<string>();
            var fintoUriList = fintoItemList?.Select(i => i.Uri).ToList();
            list.ForEach(i =>
            {
                if (!fintoUriList.Contains(i))
                {
                    newList.Add(i);
                }
            });

            return newList;
        }

        private static void CheckKeywords(IVmOpenApiServiceInVersionBase vm, IUnitOfWorkWritable unitOfWork)
        {
            var rep = unitOfWork.CreateRepository<IKeywordRepository>();
            vm.Keywords.ForEach(k =>
            {
                var keyWord = rep.All().FirstOrDefault(x => x.Name.ToLower() == k.Value.ToLower() && x.Localization.Code == k.Language);
                if (keyWord != null)
                {
                    k.Id = keyWord.Id;
                }
            });
        }

        private static void CheckTargetGroups(ICollection<string> targetGroupsUri, IUnitOfWork unitOfWork)
        {
            if (targetGroupsUri.IsNullOrEmpty()) return;

            var targetGroups = unitOfWork.CreateRepository<ITargetGroupRepository>().All().Where(tg => targetGroupsUri.Contains(tg.Uri));
            foreach (var targetGroup in targetGroups)
            {
                if (string.IsNullOrEmpty(targetGroup.ParentUri)) continue;
                if (!targetGroupsUri.Contains(targetGroup.ParentUri))
                {
                    targetGroupsUri.Add(targetGroup.ParentUri);
                }
            }
        }

        private void FillWithGeneralDescriptionData(IVmOpenApiServiceVersionBase service, IUnitOfWork unitOfWork)
        {
            // PTV-1667: Type (Tyyppi), Name (Nimi), Target Groups (Kohderyhmä), ServiceClass (Palveluluokka) and OntologyWords (Asiasanat) are filled from general description.
            // Name is always saved into db (copied from general description), so we do not need to fill it.
            if (!service.GeneralDescriptionId.IsAssigned())
            {
                return;
            }

            // Get the general description
            var generalDescription = generalDescriptionService.GetGeneralDescriptionSimple(unitOfWork, service.GeneralDescriptionId.Value);
            if (generalDescription != null)
            {
                // If service type is defined within general description, service related type is ignored
                if (!string.IsNullOrEmpty(generalDescription.Type) && service.Type.IsNullOrEmpty())
                {
                    service.Type = generalDescription.Type;
                }

                // finto items - attach items from general description into service items.
                // Target groups
                if (service.TargetGroups.Count == 0)
                {
                    service.TargetGroups = generalDescription.TargetGroups;
                }
                else
                {
                    var targetGroups = service.TargetGroups.Where(t => !t.Override).ToList(); // Get the items that are not overridden
                    var targetGroupUris = targetGroups.Select(t => t.Uri).ToList();
                    var overriddenUris = service.TargetGroups.Where(t => t.Override).Select(t => t.Uri).ToList();
                    generalDescription.TargetGroups.ForEach(target =>
                    {
                        if (!overriddenUris.Contains(target.Uri)) // If general description target group is not overridden by service target group let's attach it into service target groups.
                        {
                            if (!targetGroupUris.Contains(target.Uri)) targetGroups.Add(target);
                        }
                    });
                    service.TargetGroups = targetGroups;
                }

                service.ServiceClasses = generalDescription.ServiceClasses.Union(service.ServiceClasses, new FintoItemComparer<V7VmOpenApiFintoItemWithDescription>()).ToList();
                service.OntologyTerms = generalDescription.OntologyTerms.Union(service.OntologyTerms, new FintoItemComparer<V4VmOpenApiFintoItem>()).ToList();

                // Check service charge type.
                // If general description has charge type set, charge type for service has to be null! PTV-2347
                if (!string.IsNullOrEmpty(generalDescription.ServiceChargeType))
                {
                    service.ServiceChargeType = null;
                }
            }
        }

        public class FintoItemComparer<T> : IEqualityComparer<T> where T : VmOpenApiFintoItemVersionBase
        {
            public bool Equals(T x, T y)
            {
                return x.Uri == y.Uri;
            }

            public int GetHashCode(T obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        #endregion
    }
}