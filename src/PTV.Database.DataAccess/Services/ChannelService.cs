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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Enums;

using Microsoft.Extensions.Logging;
using PTV.Domain.Model.Models.OpenApi;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Utils;
using PTV.Framework.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Database.Model.Interfaces;
using System.Text.RegularExpressions;
using System.Diagnostics;
using PTV.Database.DataAccess.EntityCloners;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IChannelService), RegisterType.Transient)]
    internal class ChannelService : ServiceBase, IChannelService
    {
        private IContextManager contextManager;
        private ILogger logger;
        private ServiceUtilities utilities;
        private ICommonServiceInternal commonService;
        private IAddressService addressService;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private IVersioningManager versioningManager;
        private ILanguageOrderCache languageOrderCache;
        private ICloningManager cloningManager;

        public ChannelService(
            IContextManager contextManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            ILogger<ChannelService> logger,
            ServiceUtilities utilities,
            ICommonServiceInternal commonService,
            IAddressService addressService,
            ICacheManager cacheManager,
            IPublishingStatusCache publishingStatusCache,
            IVersioningManager versioningManager,
            IUserOrganizationChecker userOrganizationChecker,
            ILanguageOrderCache languageOrderCache,
            ICloningManager cloningManager) :
            base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.commonService = commonService;
            this.addressService = addressService;
            this.typesCache = cacheManager.TypesCache;
            this.languageCache = cacheManager.LanguageCache;
            this.versioningManager = versioningManager;
            this.languageOrderCache = languageOrderCache;
            this.cloningManager = cloningManager;
        }

        public IVmChannelSearch GetChannelSearch()
        {
            string statusDeletedCode = PublishingStatus.Deleted.ToString();
            string statusOldPublishedCode = PublishingStatus.OldPublished.ToString();
            string statusModifiedCode = PublishingStatus.Modified.ToString();

            VmChannelSearch result = new VmChannelSearch();
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = new VmChannelSearch()
                {
                    OrganizationId = utilities.GetUserMainOrganization()

                };
                var publishingStatuses = commonService.GetPublishingStatuses();
                var phoneNumbers = commonService.GetPhoneTypes();
                var channelTypes = commonService.GetServiceChannelTypes();

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames()),
                    () => GetEnumEntityCollectionModel("PublishingStatuses", publishingStatuses),
                    () => GetEnumEntityCollectionModel("PhoneNumberTypes", phoneNumbers),
                    () => GetEnumEntityCollectionModel("ChannelTypes", channelTypes)
                );

                result.SelectedPublishingStatuses = publishingStatuses.Where(x => x.Code != statusModifiedCode && x.Code != statusDeletedCode && x.Code != statusOldPublishedCode).Select(x => x.Id).ToList();
            });
            return result;
        }

        public IVmSearchBase SearchChannels(IUnitOfWork unitOfWork, VmChannelSearchParams vm)
        {
            var dateUtcNow = DateTime.UtcNow;
            vm.Name = vm.Name != null ? Regex.Replace(vm.Name.Trim(), @"\s+", " ") : vm.Name;
            IReadOnlyList<VmChannelListItem> result = new List<VmChannelListItem>();
            bool moreAvailable = false;
            int count = 0;
            int safePageNumber = vm.PageNumber.PositiveOrZero();
            
            var formIdentifier = vm.ChannelFormIdentifier?.ToLower();
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var languagesIds = vm.Languages?.Select(language => languageCache.Get(language.ToString())) ?? new List<Guid>();
            var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var resultTemp = channelRep.All();


            #region FilteringData

            if (vm.EntityIds != null && vm.EntityIds.Any())
            {
                resultTemp = resultTemp
                    .Where(channel =>
                        vm.EntityIds.Contains(channel.UnificRootId)
                    );
            }
            else
            {
                if(!string.IsNullOrEmpty(vm.ChannelType))
                {
                    vm.ChannelTypeId = typesCache.Get<ServiceChannelType>(vm.ChannelType);
                }

                if (vm.ChannelTypeId.HasValue)
                {
                    resultTemp = resultTemp.Where(sc => sc.TypeId == vm.ChannelTypeId);
                }

                if (!string.IsNullOrEmpty(vm.Name))
                {

                    var rootId = GetRootIdFromString(vm.Name);
                    if (!rootId.HasValue)
                    {
                        var searchText = vm.Name.ToLower();
                        resultTemp = resultTemp.Where(sc => sc.ServiceChannelNames.Any(
                            y => (y.Name.ToLower().Contains(searchText) || y.CreatedBy.ToLower().Contains(searchText) || y.ModifiedBy.ToLower().Contains(searchText))
                                 && languagesIds.Contains(y.LocalizationId)));
                    }
                    else
                    {
                        resultTemp = resultTemp
                            .Where(channel =>
                                channel.UnificRootId == rootId
                            );
                    }
                }
                else
                {
                    resultTemp =
                        resultTemp.Where(sc => sc.ServiceChannelNames.Any(y => languagesIds.Contains(y.LocalizationId) &&
                                                                               !string.IsNullOrEmpty(y.Name)));
                }

                if (vm.OrganizationIds.Any())
                {
                    resultTemp = resultTemp.Where(sc => vm.OrganizationIds.Contains(sc.OrganizationId));
                }

                if (vm.ChannelType?.ToLower() == ServiceChannelTypeEnum.Phone.ToString().ToLower())
                {
                    if (vm.SelectedPhoneNumberTypes?.Any() == true)
                    {
                        resultTemp = resultTemp.Where(sc => vm.SelectedPhoneNumberTypes.Any(m => sc.Phones.Select(i => i.Phone.TypeId).Contains(m)));
                    }
                }
                if (vm.ChannelType?.ToLower() == ServiceChannelTypeEnum.PrintableForm.ToString().ToLower())
                {
                    if (!string.IsNullOrEmpty(formIdentifier))
                    {
                        resultTemp = resultTemp.Where(sc => sc.PrintableFormChannels.Any(y => y.FormIdentifiers.Any(z => z.FormIdentifier.ToLower().Contains(formIdentifier))));
                    }
                }
            }
            
            if (vm.SelectedPublishingStatuses != null)
            {
                commonService.ExtendPublishingStatusesByEquivalents(vm.SelectedPublishingStatuses);
                resultTemp = resultTemp.WherePublishingStatusIn(vm.SelectedPublishingStatuses);
            }
            
            count = resultTemp.Select(i => true).Count();
            resultTemp = unitOfWork.ApplyIncludes(resultTemp, q => q.Include(j => j.Versioning));

            #endregion FilteringData
            
            var lifeTime = vm.Expiration;
            
            moreAvailable = count.MoreResultsAvailable(safePageNumber);

            var nameType = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());

            var resultTempData = resultTemp.Select(i => new
            {
                Id = i.Id,
                PublishingStatusId = i.PublishingStatusId,
                UnificRootId = i.UnificRootId,
                TypeId = i.TypeId,
                Name = i.ServiceChannelNames.OrderBy(x=>x.Localization.OrderNumber).FirstOrDefault(x => (languagesIds != null && languagesIds.Contains(x.LocalizationId) && x.TypeId == nameTypeId) || x.TypeId == nameTypeId).Name,
                AllNames = i.ServiceChannelNames.Where(x => x.TypeId == nameType).Select(x => new { x.LocalizationId, x.Name }),
                LanguageAvailabilities = i.LanguageAvailabilities.OrderBy(x => x.Language.OrderNumber),
                OrganizationId = i.OrganizationId,
                VersionMajor = i.Versioning.VersionMajor,
                VersionMinor = i.Versioning.VersionMinor,
                Modified = i.Modified,
                ModifiedBy = i.ModifiedBy,
                ExpireOn = dateUtcNow.Add(i.Modified - lifeTime) // required for sorting
            })
            .ApplySortingByVersions(vm.SortData, new VmSortParam() { Column = "Modified", SortDirection = SortDirectionEnum.Desc })
                .Select(i => new
                {
                    Id = i.Id,
                    PublishingStatusId = i.PublishingStatusId,
                    UnificRootId = i.UnificRootId,
                    TypeId = i.TypeId,
                    OrganizationId = i.OrganizationId,
                    VersionMajor = i.VersionMajor,
                    VersionMinor = i.VersionMinor,
                    Modified = i.Modified,
                    ModifiedBy = i.ModifiedBy,
                    test = i.ModifiedBy,
                    ExpireOn = i.ExpireOn // required for sorting
                })
            .ApplyPagination(safePageNumber)
            .ToList();

            var serviceChannelIds = resultTempData.Select(i => i.Id).ToList();
            var serviceChannelNameRep = unitOfWork.CreateRepository<IServiceChannelNameRepository>();
            var serviceChannelNames = serviceChannelNameRep.All().Where(x => serviceChannelIds.Contains(x.ServiceChannelVersionedId) && ((languagesIds != null && languagesIds.Contains(x.LocalizationId) && x.TypeId == nameTypeId) || x.TypeId == nameTypeId)).Select(i => new { i.ServiceChannelVersionedId, i.Name, i.LocalizationId }).ToList().GroupBy(i => i.ServiceChannelVersionedId)
                .ToDictionary(i => i.Key, i => i.OrderBy(j => languageOrderCache.Get(j.LocalizationId)).ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));
            var channelLangAvailabilitiesRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
            var channelLangAvailabilities = channelLangAvailabilitiesRep.All().Where(x => serviceChannelIds.Contains(x.ServiceChannelVersionedId)).ToList()
                .GroupBy(i => i.ServiceChannelVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());
            result = resultTempData.Select(i => new VmChannelListItem
            {
                Id = i.Id,
                PublishingStatusId = i.PublishingStatusId,
                Name = serviceChannelNames.TryGetOrDefault(i.Id, new Dictionary<string, string>()),
                RootId = i.UnificRootId,
                UnificRootId = i.UnificRootId,
                TypeId = i.TypeId,
                MainEntityType = EntityTypeEnum.Channel,
                SubEntityType = typesCache.GetByValue<ServiceChannelType>(i.TypeId),
                OrganizationId = i.OrganizationId,
                LanguagesAvailabilities = TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(channelLangAvailabilities.TryGetOrDefault(i.Id, new List<ServiceChannelLanguageAvailability>())),
                Version = new VmVersion() { Major = i.VersionMajor, Minor = i.VersionMinor },
                Modified = i.Modified.ToEpochTime(),
                ModifiedBy = i.ModifiedBy,
                ExpireOn = i.ExpireOn.ToEpochTime(),
            })
            .ToList();

            var returnData = new VmSearchResult<IVmChannelListItem>() {
                SearchResult = result,
                PageNumber = ++safePageNumber,
                MoreAvailable = moreAvailable,
                Count = count
            };

            return returnData;
        }
        
        public IVmSearchBase SearchChannels(VmChannelSearchParams vm)
        {
            VmSearchResult<IVmChannelListItem> returnData = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                returnData = SearchChannels(unitOfWork, vm) as VmSearchResult<IVmChannelListItem>;
                return returnData;
            });
            
            FillEnumEntities(returnData, () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizations(returnData.SearchResult.Select(org => org.OrganizationId))));
            return returnData;
        }

        public VmEntityNames GetChannelNames(VmEntityBase model)
        {
            var result = new VmEntityNames();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                var channel = unitOfWork.ApplyIncludes(serviceChannelRep.All(), q =>
                    q.Include(i => i.ServiceChannelNames)
                        .Include(i => i.LanguageAvailabilities)).Single(x => x.Id == model.Id.Value);

                result = TranslationManagerToVm.Translate<ServiceChannelVersioned, VmEntityNames>(channel);

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages())
                );
            });
            return result;
        }


        #region OpenApi

        public IVmOpenApiGuidPageVersionBase GetServiceChannels(DateTime? date, int pageNumber, int pageSize, EntityStatusExtendedEnum status = EntityStatusExtendedEnum.Published, DateTime? dateBefore = null)
        {
            var vm = new V3VmOpenApiGuidPage(pageNumber, pageSize);

            if (pageNumber <= 0) return vm;

            List<ServiceChannelVersioned> channels = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                switch (status)
                {
                    case EntityStatusExtendedEnum.Published:
                        // Get the published channels and filter out item names that are not published (PTV-3689).
                        channels = FilterOutNotPublishedNames(GetPublishedEntities<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(vm, date, dateBefore, unitOfWork,
                            q => q.Include(i => i.ServiceChannelNames).Include(i => i.LanguageAvailabilities)));
                        break;
                    case EntityStatusExtendedEnum.Archived:
                        channels = GetArchivedEntities<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(vm, date, dateBefore, unitOfWork,
                            q => q.Include(i => i.ServiceChannelNames));
                        break;
                    case EntityStatusExtendedEnum.Withdrawn:
                        channels = GetWithdrawnEntities<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(vm, date, dateBefore, unitOfWork,
                            q => q.Include(i => i.ServiceChannelNames));
                        break;
                    case EntityStatusExtendedEnum.Active:
                        channels = GetActiveEntities<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(vm, date, dateBefore, unitOfWork,
                            q => q.Include(i => i.ServiceChannelNames));
                        break;
                    default:
                        break;
                }         
            });

            return GetGuidPage(channels, vm);
        }

        public IList<IVmOpenApiServiceChannel> GetServiceChannels(List<Guid> idList, int openApiVersion)
        {
            if (idList == null || idList.Count == 0)
            {
                return null;
            }

            IList<IVmOpenApiServiceChannel> result = new List<IVmOpenApiServiceChannel>();
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    IList<Expression<Func<ServiceChannelVersioned, bool>>> filters = new List<Expression<Func<ServiceChannelVersioned, bool>>>
                    {
                        c => idList.Contains(c.UnificRootId)
                    };
                    
                    result = GetServiceChannelsWithDetails(unitOfWork, filters, openApiVersion);
                });
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting service channels. {0}", ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }

            return result;
        }

        public IVmOpenApiGuidPageVersionBase GetServiceChannelsByMunicipality(Guid municipalityId, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null)
        {
            var vm = new V3VmOpenApiGuidPage(pageNumber, pageSize);

            if (pageNumber <= 0) return vm;

            List<ServiceChannelVersioned> channels = null;

            contextManager.ExecuteReader(unitOfWork =>
            {
                // Areas related to defined municipality
                var areas = unitOfWork.CreateRepository<IAreaMunicipalityRepository>().All()
                    .Where(a => a.MunicipalityId == municipalityId).Select(a => a.AreaId).ToList();

                // Get channels
                var wholeCountryId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
                IList<Expression<Func<ServiceChannelVersioned, bool>>> filters = new List<Expression<Func<ServiceChannelVersioned, bool>>>();
                // is the municipality in 'Åland'? So do we need to include also AreaInformationType WholeCountryExceptAlandIslands?
                if (IsAreaInAland(unitOfWork, areas, typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString()))) // Åland
                {
                    filters.Add(c => (c.AreaInformationTypeId == wholeCountryId) ||
                    (c.AreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || c.Areas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId))));
                }
                else
                {
                    var wholeCountryExceptAlandId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
                    filters.Add(c => (c.AreaInformationTypeId == wholeCountryId) || c.AreaInformationTypeId == wholeCountryExceptAlandId ||
                    (c.AreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || c.Areas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId))));
                }

                // Get the published channels and filter out item names that are not published (PTV-3689).
                channels = FilterOutNotPublishedNames(GetPublishedEntities<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(vm, date, dateBefore, unitOfWork, q =>
                    q.Include(i => i.ServiceChannelNames).Include(i => i.LanguageAvailabilities), filters));
            });

            return GetGuidPage(channels, vm);
        }

        public IVmOpenApiServiceChannel GetServiceChannelById(Guid id, int openApiVersion, VersionStatusEnum status)
        {
            IVmOpenApiServiceChannel result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetServiceChannelById(unitOfWork, id, openApiVersion, status);
            });

            return result;
        }

        public IVmOpenApiServiceChannel GetServiceChannelByIdSimple(Guid id, bool getOnlyPublished = true)
        {
            IVmOpenApiServiceChannel result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                try
                {
                    Guid? entityId = null;
                    if (getOnlyPublished)
                    {   // Get published version
                        entityId = versioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, PublishingStatus.Published);
                    }
                    else
                    {   // Get latest version regardless of the publishing status
                        entityId = versioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, null, false);
                    }
                    
                    if (entityId.IsAssigned())
                    {
                        result = GetServiceChannelWithSimpleDetails(unitOfWork, entityId.Value);
                    }
                }
                catch (Exception ex)
                {
                    var errorMsg = string.Format("Error occured while getting a channel with id {0}. {1}", id, ex.Message);
                    logger.LogError(errorMsg + " " + ex.StackTrace);
                    throw new Exception(errorMsg);
                }
            });

            return result;
        }

        public PublishingStatus? GetLatestVersionPublishingStatus(Guid id)
        {
            PublishingStatus? currentPublishingStatus = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                currentPublishingStatus = versioningManager.GetLatestVersionPublishingStatus<ServiceChannelVersioned>(unitOfWork, id);
            });

            return currentPublishingStatus;
        }

        private List<ServiceChannelVersioned> FilterOutNotPublishedNames(List<ServiceChannelVersioned> channels)
        {
            if (channels == null) return channels;
            var publishedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            channels.ForEach(s =>
            {
                var publishedLanguageIds = s.LanguageAvailabilities.Where(l => l.StatusId == publishedStatusId).Select(l => l.LanguageId).ToList();
                s.ServiceChannelNames = s.ServiceChannelNames.Where(i => publishedLanguageIds.Contains(i.LocalizationId)).ToList();
            });
            return channels;
        }

        private IVmOpenApiServiceChannel GetServiceChannelById(IUnitOfWork unitOfWork, Guid id, int openApiVersion, VersionStatusEnum status)
        {
            IVmOpenApiServiceChannel result = null;
            try
            {
                Guid? entityId = null;
                switch (status)
                {
                    case VersionStatusEnum.Published:
                        entityId = versioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, PublishingStatus.Published);
                        break;
                    case VersionStatusEnum.Latest:
                        entityId = versioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, null, false);
                        break;
                    case VersionStatusEnum.LatestActive:
                        entityId = versioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, null, true);
                        break;
                    default:
                        break;
                }
                if (entityId.IsAssigned())
                {
                    result = GetServiceChannelWithDetails(unitOfWork, entityId.Value, openApiVersion, status == VersionStatusEnum.Published ? true : false);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting a channel with id {0}. {1}", id, ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }
            return result;
        }

        public IVmOpenApiServiceChannel GetServiceChannelBySource(string sourceId)
        {
            var userId = utilities.GetRelationIdForExternalSource();
            Guid? rootId = null;
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    rootId = GetPTVId<ServiceChannel>(sourceId, userId, unitOfWork);

                });
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting services by source id {0}. {1}", sourceId, ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }
            return rootId.HasValue ? GetServiceChannelByIdSimple(rootId.Value, false) : null;
        }

        public IList<IVmOpenApiServiceChannel> GetServiceChannelsWithDetailsByType(ServiceChannelTypeEnum type, DateTime? date, int openApiVersion)
        {
            IList<IVmOpenApiServiceChannel> result = new List<IVmOpenApiServiceChannel>();
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var typeId = typesCache.Get<ServiceChannelType>(type.ToString());
                    IList<Expression<Func<ServiceChannelVersioned, bool>>> filters = new List<Expression<Func<ServiceChannelVersioned, bool>>>
                    {
                        c => c.TypeId.Equals(typeId)
                    };
                    if (date.HasValue)
                    {
                        filters.Add(c => c.Modified > date.Value);
                    }
                    result = GetServiceChannelsWithDetails(unitOfWork, filters, openApiVersion);
                });
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting service channels of type {0}. {1}", type.ToString(), ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }

            return result;
        }

        public IVmOpenApiGuidPageVersionBase GetServiceChannelsByType(ServiceChannelTypeEnum type, DateTime? date, int pageNumber, int pageSize, bool getOnlyPublished = true, DateTime? dateBefore = null)
        {
            var vm = new V3VmOpenApiGuidPage(pageNumber, pageSize);

            if (pageNumber <= 0) return vm;

            var typeId = typesCache.Get<ServiceChannelType>(type.ToString());
            List<ServiceChannelVersioned> channels = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                if (!getOnlyPublished)
                {
                    channels = GetActiveEntities<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>
                    (vm, date, dateBefore, unitOfWork, q => q.Include(i => i.ServiceChannelNames),
                    new List<Expression<Func<ServiceChannelVersioned, bool>>>() { c => c.TypeId.Equals(typeId) });
                }
                else
                {
                    // Get the published channels and filter out item names that are not published (PTV-3689).
                    channels = FilterOutNotPublishedNames(GetPublishedEntities<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>
                    (vm, date, dateBefore, unitOfWork, q => q.Include(i => i.ServiceChannelNames).Include(i => i.LanguageAvailabilities),
                    new List<Expression<Func<ServiceChannelVersioned, bool>>>() { c => c.TypeId.Equals(typeId) }));
                }
            });

            return GetGuidPage(channels, vm);
        }

        public IList<IVmOpenApiServiceChannel> GetServiceChannelsByOrganization(Guid organizationId, DateTime? date, int openApiVersion, ServiceChannelTypeEnum? type = null)
        {
            IList<IVmOpenApiServiceChannel> result = new List<IVmOpenApiServiceChannel>();

            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    IList<Expression<Func<ServiceChannelVersioned, bool>>> filters = new List<Expression<Func<ServiceChannelVersioned, bool>>>
                    {
                        c => c.OrganizationId.Equals(organizationId)
                    };
                    if (date.HasValue)
                    {
                        filters.Add(c => c.Modified > date.Value);
                    }
                    if (type.HasValue)
                    {
                        var typeId = typesCache.Get<ServiceChannelType>(type.Value.ToString());
                        // Filter by service channel type
                        filters.Add(c => c.TypeId == typeId);
                    }
                    result = GetServiceChannelsWithDetails(unitOfWork, filters, openApiVersion);
                    
                });
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting service channels for organization {0}. {1}", organizationId.ToString(), ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }

            return result;
        }

        public IVmOpenApiGuidPageVersionBase GetServiceChannelsByOrganization(Guid organizationId, DateTime? date, int pageNumber, int pageSize, ServiceChannelTypeEnum? type = null, DateTime? dateBefore = null)
        {
            var vm = new V3VmOpenApiGuidPage(pageNumber, pageSize);

            if (pageNumber <= 0) return vm;

            List<ServiceChannelVersioned> channels = null;

            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var additionalFilters = new List<Expression<Func<ServiceChannelVersioned, bool>>>() { serviceChannel => serviceChannel.OrganizationId.Equals(organizationId) };
                    if (type.HasValue)
                    {
                        var typeId = typesCache.Get<ServiceChannelType>(type.Value.ToString());
                        additionalFilters.Add(serviceChannel => serviceChannel.TypeId == typeId);
                    }
                    // Get the published channels and filter out item names that are not published (PTV-3689).
                    channels = FilterOutNotPublishedNames(GetPublishedEntities<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>
                        (vm, date, dateBefore, unitOfWork, q => q.Include(i => i.ServiceChannelNames).Include(i => i.LanguageAvailabilities), additionalFilters));
                });
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting service channels for organization {0}. {1}", organizationId.ToString(), ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }

            return GetGuidPage(channels, vm);
        }
        
        public VmOpenApiConnectionChannels CheckChannels(List<Guid> idList, List<Guid> userOrganizations = null)
        {
            if (idList == null || idList?.Count == 0) { return null; }

            var result = new VmOpenApiConnectionChannels();
            var serviceLocationChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString());
            contextManager.ExecuteReader(unitOfWork =>
            {
                var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
                var query = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>().All().Where(s => idList.Contains(s.UnificRootId) && s.PublishingStatusId == publishedId);
                if (userOrganizations?.Count > 0)
                {
                    var visibleForAll = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.CommonForAll.ToString());
                    query = query.Where(s => s.ConnectionTypeId == visibleForAll || userOrganizations.Contains(s.OrganizationId));
                }
                var existingChannels = query.Select(s => new { Id = s.UnificRootId, TypeId = s.TypeId }).ToList();
                var existingChannelsIds = existingChannels.Select(i => i.Id).ToList();
                result.NotExistingChannels = idList.Where(i => !existingChannelsIds.Contains(i)).ToList();
                result.ServiceLocationChannels = existingChannels.Where(i => i.TypeId == serviceLocationChannelTypeId)?.Select(i => i.Id).ToList();
            });
            return result;
        }

        private IVmOpenApiGuidPageVersionBase GetGuidPage(IList<ServiceChannelVersioned> channels, V3VmOpenApiGuidPage vm)
        {
            if (channels?.Count > 0)
            {
                vm.ItemList = TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiItem>(channels).ToList();
            }

            return vm;
        }
        private IVmOpenApiServiceChannel GetServiceChannelWithDetails(IUnitOfWork unitOfWork, Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            //return GetServiceChannelsWithDetails(unitOfWork, new List<Guid> { versionId }, openApiVersion, getOnlyPublished).FirstOrDefault();
            //// Measure
            //var watch = new Stopwatch();
            //logger.LogTrace("****************************************");
            //logger.LogTrace($"GetServiceChannelWithDetails starts. Id: {versionId}");
            //watch.Start();
            //// end measure

            IVmOpenApiServiceChannel result = null;
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var query = serviceChannelRep.All().Where(s => s.Id == versionId);
            if (getOnlyPublished)
            {
                query = query.Where(c => c.LanguageAvailabilities.Any(l => l.StatusId == publishedId));
            }

            var channel = unitOfWork.ApplyIncludes(query, GetServiceChannelIncludeChain()).FirstOrDefault();

            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Fetch from db: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// end measure

            if (channel != null)
            {
                // Set accessibility sentences. PTV-2481 & PTV-4237
                if (channel.Addresses?.Count > 0 && channel.UnificRoot?.AccessibilityRegisters?.Count > 0)
                {
                    channel.Addresses.ForEach(a =>
                    {
                        if (a.Address.UniqueId != Guid.Empty)
                        {
                            var ar = channel.UnificRoot?.AccessibilityRegisters.FirstOrDefault(r => r.Address.UniqueId == a.Address.UniqueId);
                            if (ar != null)
                            {
                                a.Address.AccessibilityRegisterEntrances = ar.Entrances;
                            }
                        }
                    });
                }

                // Filter out not published language versions
                FilterOutNotPublishedLanguageVersions(channel, publishedId, getOnlyPublished);

                ////Measure
                //watch.Stop();
                //logger.LogTrace($"*** Filtering: {watch.ElapsedMilliseconds} ms.");
                //watch.Restart();
                //// End measure

                if (channel.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.EChannel.ToString()))
                {
                    result = TranslationManagerToVm.Translate<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(channel);
                }
                else if (channel.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.Phone.ToString()))
                {
                    result = TranslationManagerToVm.Translate<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(channel);
                }
                else if (channel.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString()))
                {
                    result = TranslationManagerToVm.Translate<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(channel);
                }
                else if (channel.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.PrintableForm.ToString()))
                {
                    result = TranslationManagerToVm.Translate<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(channel);
                }
                else if (channel.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.WebPage.ToString()))
                {
                    result = TranslationManagerToVm.Translate<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(channel);
                }

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

            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Get connections from db: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// End measure

            // Find only published services for a service channel - let's do this outside of translator
            // Manually map connection data.
            result.Services = GetChannelServices(new List<Guid> { channel.UnificRootId }, unitOfWork);
                        
            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Services for channel mapping: {watch.ElapsedMilliseconds} ms.");
            //// End measure

            return GetServiceChannelByOpenApiVersion(unitOfWork, result, openApiVersion);
        }

        private Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>> GetServiceChannelIncludeChain()
        {
            return q =>
                q.Include(i => i.ServiceChannelNames)
                .Include(i => i.DisplayNameTypes)
                .Include(i => i.ServiceChannelDescriptions)
                .Include(i => i.Type)
                .Include(i => i.WebPages).ThenInclude(i => i.WebPage)
                .Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours).ThenInclude(j => j.DailyOpeningTimes)
                .Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours).ThenInclude(j => j.AdditionalInformations)
                .Include(i => i.Attachments).ThenInclude(i => i.Attachment).ThenInclude(i => i.Type)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)  
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)                    
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)                    
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressOthers).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Receivers)
                
                .Include(i => i.ElectronicChannels).ThenInclude(i => i.LocalizedUrls)

                .Include(i => i.Languages).ThenInclude(i => i.Language)
                .Include(i => i.WebpageChannels).ThenInclude(i => i.LocalizedUrls)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.FormIdentifiers)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.ChannelUrls)
                .Include(j => j.Emails).ThenInclude(j => j.Email)
                .Include(j => j.Phones).ThenInclude(j => j.Phone).ThenInclude(i => i.PrefixNumber)
                .Include(i => i.LanguageAvailabilities)
                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaNames)
                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.UnificRoot).ThenInclude(i => i.AccessibilityRegisters).ThenInclude(i => i.Address)
                .Include(i => i.UnificRoot).ThenInclude(i => i.AccessibilityRegisters).ThenInclude(i => i.Entrances).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
                .Include(i => i.UnificRoot).ThenInclude(i => i.AccessibilityRegisters).ThenInclude(i => i.Entrances).ThenInclude(i => i.Names)
                .Include(i => i.UnificRoot).ThenInclude(i => i.AccessibilityRegisters).ThenInclude(i => i.Entrances).ThenInclude(i => i.SentenceGroups).ThenInclude(i => i.Values)
                .Include(i => i.UnificRoot).ThenInclude(i => i.AccessibilityRegisters).ThenInclude(i => i.Entrances).ThenInclude(i => i.SentenceGroups).ThenInclude(i => i.Sentences).ThenInclude(i => i.Values)
                .Include(i => i.UnificRoot).ThenInclude(i => i.SocialHealthCenters);
        }

        private Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>> GetServiceChannelBaseIncludeChain()
        {
            return q =>
                q.Include(i => i.ServiceChannelNames)
                .Include(i => i.ServiceChannelDescriptions)
                .Include(i => i.Type)
                .Include(i => i.WebPages).ThenInclude(i => i.WebPage)
                .Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours).ThenInclude(j => j.DailyOpeningTimes)
                .Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours).ThenInclude(j => j.AdditionalInformations)
                .Include(i => i.Attachments).ThenInclude(i => i.Attachment).ThenInclude(i => i.Type)
                .Include(i => i.Languages).ThenInclude(i => i.Language)
                .Include(j => j.Emails).ThenInclude(j => j.Email)
                .Include(j => j.Phones).ThenInclude(j => j.Phone).ThenInclude(i => i.PrefixNumber)
                .Include(i => i.LanguageAvailabilities)
                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaNames)
                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.UnificRoot)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)  
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)                    
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)                    
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressOthers).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Receivers);
        }

        private void FilterOutNotPublishedLanguageVersions(ServiceChannelVersioned channel, Guid publishedId, bool getOnlyPublished)
        {
            // Filter out not published language versions
            if (getOnlyPublished)
            {
                // Filter out not published language versions
                var notPublishedLanguageVersions = channel.LanguageAvailabilities.Where(i => i.StatusId != publishedId).Select(i => i.LanguageId).ToList();
                if (notPublishedLanguageVersions.Count > 0)
                {
                    channel.ServiceChannelNames = channel.ServiceChannelNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    channel.ServiceChannelDescriptions = channel.ServiceChannelDescriptions.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    channel.WebPages = channel.WebPages.Where(i => !notPublishedLanguageVersions.Contains(i.WebPage.LocalizationId)).ToList();
                    if (channel.ServiceChannelServiceHours?.Count > 0)
                    {
                        var hours = channel.ServiceChannelServiceHours.Select(i => i.ServiceHours).ToList();
                        hours.ForEach(hour =>
                        {
                            hour.AdditionalInformations = hour.AdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        });
                    }

                    channel.Attachments = channel.Attachments.Where(i => !notPublishedLanguageVersions.Contains(i.Attachment.LocalizationId)).ToList();
                    channel.Emails = channel.Emails.Where(i => !notPublishedLanguageVersions.Contains(i.Email.LocalizationId)).ToList();
                    channel.Phones = channel.Phones.Where(i => !notPublishedLanguageVersions.Contains(i.Phone.LocalizationId)).ToList();

                    // Electronic channel
                    channel.ElectronicChannels.ForEach(c =>
                    {
                        c.LocalizedUrls = c.LocalizedUrls.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    });

                    // Web page channel
                    channel.WebpageChannels.ForEach(c =>
                    {
                        c.LocalizedUrls = c.LocalizedUrls.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    });

                    // Printable form channel
                    channel.PrintableFormChannels.ForEach(c =>
                    {
                        c.FormIdentifiers = c.FormIdentifiers.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        c.ChannelUrls = c.ChannelUrls.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    });
                    
                    // channel addresses
                    // (printable channels and service location channels)
                    channel.Addresses.ForEach(address =>
                    {
                        address.Address.AddressStreets.ForEach(j =>
                        {
                            j.StreetNames = j.StreetNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        });
                        address.Address.AddressPostOfficeBoxes.ForEach(j =>
                        {
                            j.PostOfficeBoxNames = j.PostOfficeBoxNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        });
                        address.Address.AddressForeigns.ForEach(j =>
                        {
                            j.ForeignTextNames = j.ForeignTextNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        });
                        address.Address.AddressAdditionalInformations = address.Address.AddressAdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        address.Address.AccessibilityRegisterEntrances.ForEach(e =>
                        {
                            e.Names = e.Names.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                            e.SentenceGroups.ForEach(g =>
                            {
                                g.Values = g.Values.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                                g.Sentences.ForEach(s => s.Values = s.Values.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList());
                            });
                        });
                    });
                }
            }
        }

        private IVmOpenApiServiceChannel GetServiceChannelByOpenApiVersion(IUnitOfWork unitOfWork, IVmOpenApiServiceChannel baseVersion, int openApiVersion)
        {
            // Get the sourceId if user is logged in
            var userId = utilities.GetRelationIdForExternalSource(false);
            if (!string.IsNullOrEmpty(userId))
            {
                baseVersion.SourceId = GetSourceId<ServiceChannel>(baseVersion.Id.Value, userId, unitOfWork);
            }
            return GetEntityByOpenApiVersion(baseVersion, openApiVersion);
        }

        private IVmOpenApiServiceChannel GetServiceChannelWithDetails(Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiServiceChannel result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetServiceChannelWithDetails(unitOfWork, versionId, openApiVersion, getOnlyPublished);
            });
            return result;
        }

        private IList<IVmOpenApiServiceChannel> GetServiceChannelsWithDetailsOld(IUnitOfWork unitOfWork, List<Guid> versionIdList, int openApiVersion, bool getOnlyPublished = true)
        {
            if (versionIdList.Count == 0) return new List<IVmOpenApiServiceChannel>();

            //// Measure
            //var watch = new Stopwatch();
            //logger.LogTrace("****************************************");
            //logger.LogTrace("*** Old implementation ***");
            //logger.LogTrace($"GetServiceChannelWithDetails starts. Ids: { string.Join( ", ", versionIdList)}");
            //watch.Start();
            //// end measure

            var resultList = new List<IVmOpenApiServiceChannel>();
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var queryWithData = unitOfWork.ApplyIncludes(serviceChannelRep.All().Where(c => versionIdList.Contains(c.Id)), q =>
                q.Include(i => i.ServiceChannelNames)
                .Include(i => i.ServiceChannelDescriptions)
                .Include(i => i.Type)
                .Include(i => i.WebPages).ThenInclude(i => i.WebPage)
                .Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours).ThenInclude(j => j.DailyOpeningTimes)
                .Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours).ThenInclude(j => j.AdditionalInformations)
                .Include(i => i.Attachments).ThenInclude(i => i.Attachment).ThenInclude(i => i.Type)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Receivers)
                .Include(i => i.ElectronicChannels).ThenInclude(i => i.LocalizedUrls)
                .Include(i => i.Languages).ThenInclude(i => i.Language)
                .Include(i => i.WebpageChannels).ThenInclude(i => i.LocalizedUrls)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.FormIdentifiers)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.ChannelUrls)
                .Include(j => j.Emails).ThenInclude(j => j.Email)
                .Include(j => j.Phones).ThenInclude(j => j.Phone).ThenInclude(i => i.PrefixNumber)
                .Include(i => i.LanguageAvailabilities)
                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaNames)
                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.Service).ThenInclude(i => i.Versions)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDescriptions)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDigitalAuthorizations)
                    .ThenInclude(i => i.DigitalAuthorization).ThenInclude(i => i.Names)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelExtraTypes).ThenInclude(i => i.ExtraSubType)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelExtraTypes).ThenInclude(i => i.ServiceServiceChannelExtraTypeDescriptions)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelServiceHours).ThenInclude(i => i.ServiceHours).ThenInclude(i => i.DailyOpeningTimes)
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelServiceHours).ThenInclude(i => i.ServiceHours).ThenInclude(i => i.AdditionalInformations)
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
                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
            );

            // Filter out items that do not have language versions published!
            var serviceChannels = getOnlyPublished ? queryWithData.Where(c => c.LanguageAvailabilities.Any(l => l.StatusId == publishedId)).ToList() : queryWithData.ToList();

            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Fetch from db: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// end measure

            var allPublishedServices = serviceChannels.SelectMany(i => i.UnificRoot.ServiceServiceChannels).Select(i => i.Service)
                .SelectMany(i => i.Versions).Where(i => i.PublishingStatusId == publishedId).ToList();
            var publishedServiceRootIds = allPublishedServices.Select(i => i.UnificRootId).ToList();
            var publishedServiceIds = allPublishedServices.Select(i => i.Id).ToList();

            serviceChannels.ForEach(
                channel =>
                {
                    // Filter out not published services
                    channel.UnificRoot.ServiceServiceChannels = channel.UnificRoot.ServiceServiceChannels.Where(s => publishedServiceRootIds.Contains(s.ServiceId)).ToList();

                    FilterOutNotPublishedLanguageVersions(channel, publishedId, getOnlyPublished);
                }
            );

            // Fill with service names
            var serviceNames = unitOfWork.CreateRepository<IServiceNameRepository>().All().Where(i => publishedServiceIds.Contains(i.ServiceVersionedId)).ToList()
                .GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.ToList());
            allPublishedServices.ForEach(c =>
            {
                c.ServiceNames = serviceNames.TryGet(c.Id);
            });


            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Filter out not published items: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// end measure


            if (serviceChannels?.Count > 0)
            {
                var eChannelId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.EChannel.ToString());
                resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(serviceChannels.Where(s => s.TypeId == eChannelId).ToList()));
                var phoneChannelId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.Phone.ToString());
                resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(serviceChannels.Where(s => s.TypeId == phoneChannelId).ToList()));
                var serviceLocationChannelId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString());
                resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(serviceChannels.Where(s => s.TypeId == serviceLocationChannelId).ToList()));
                var transactionFormChannelId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.PrintableForm.ToString());
                resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(serviceChannels.Where(s => s.TypeId == transactionFormChannelId).ToList()));
                var webpageChannelId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.WebPage.ToString());
                resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(serviceChannels.Where(s => s.TypeId == webpageChannelId).ToList()));
            }

            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Translation: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// End measure

            // Set the right version for service channels
            var versionList = new List<IVmOpenApiServiceChannel>();
            resultList.ForEach(channel =>
            {
                versionList.Add(GetServiceChannelByOpenApiVersion(unitOfWork, channel, openApiVersion));
            });

            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Get right version: {watch.ElapsedMilliseconds} ms.");
            //// End measure

            return versionList;
        }

        private IList<IVmOpenApiServiceChannel> GetServiceChannelsWithDetails(IUnitOfWork unitOfWork, IList<Expression<Func<ServiceChannelVersioned, bool>>> filters, int openApiVersion)
        {
            //// Measure
            //var watch = new Stopwatch();
            //logger.LogTrace("****************************************");
            //logger.LogTrace("*** New implementation ***");
            //logger.LogTrace($"GetServiceChannelsWithDetails starts. Ids: { string.Join(", ", versionIdList)}");
            //watch.Start();
            //// end measure

            var resultList = new List<IVmOpenApiServiceChannel>();
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            // Get only published items - filter out items that do not have any language versions published.
            var query = serviceChannelRep.All().Where(e => e.PublishingStatusId == publishedId && e.LanguageAvailabilities.Any(l => l.StatusId == publishedId));
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

            var serviceChannels = unitOfWork.ApplyIncludes(query, GetServiceChannelBaseIncludeChain()).ToList();

            //// Measure
            //watch.Stop();
            //logger.LogTrace($"*** Fetch from db: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// end measure

            List<V9VmOpenApiServiceChannelService> allConnections = null;

            if (serviceChannels?.Count > 0)
            {
                // E channels
                var eChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.EChannel.ToString());
                var eChannels = serviceChannels.Where(s => s.TypeId == eChannelTypeId).ToList();
                if (eChannels?.Count > 0)
                {
                    var eChannelIds = eChannels.Select(i => i.Id).ToList();
                    var eChannelQuery = unitOfWork.CreateRepository<IElectronicChannelRepository>().All().Where(c => eChannelIds.Contains(c.ServiceChannelVersionedId));
                    var eChannelsWithData = unitOfWork.ApplyIncludes(eChannelQuery, q =>
                        q.Include(i => i.LocalizedUrls)).ToList();
                    eChannelsWithData.ForEach(e =>
                    {
                        var channel = eChannels.Where(i => i.Id == e.ServiceChannelVersionedId).FirstOrDefault();
                        if (channel != null)
                        {
                            channel.ElectronicChannels.Add(e);
                        }
                    });
                    eChannels.ForEach(channel => FilterOutNotPublishedLanguageVersions(channel, publishedId, true));
                    resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(eChannels));
                }
                
                // Phone channels
                var phoneChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.Phone.ToString());
                var phoneChannels = serviceChannels.Where(s => s.TypeId == phoneChannelTypeId).ToList();
                if (phoneChannels?.Count > 0)
                {
                    phoneChannels.ForEach(channel => FilterOutNotPublishedLanguageVersions(channel, publishedId, true));
                    resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(phoneChannels));
                }
                
                // Service location channels
                var serviceLocationChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString());
                var locationChannels = serviceChannels.Where(s => s.TypeId == serviceLocationChannelTypeId).ToList();
                if (locationChannels?.Count > 0)
                {
                    // Get accessibility application sentences
                    var slChannelIds = locationChannels.Select(l => l.UnificRootId).ToList();
                    var arQuery = unitOfWork.CreateRepository<IAccessibilityRegisterRepository>().All()
                        .Where(ar => slChannelIds.Contains(ar.ServiceChannelId));
                    var registers = unitOfWork.ApplyIncludes(arQuery, q =>
                        q.Include(i => i.Address)
                         .Include(i => i.Entrances).ThenInclude(i => i.Names)
                         .Include(i => i.Entrances).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
                         .Include(i => i.Entrances).ThenInclude(i => i.SentenceGroups).ThenInclude(i => i.Values)
                         .Include(i => i.Entrances).ThenInclude(i => i.SentenceGroups).ThenInclude(i => i.Sentences).ThenInclude(i => i.Values)
                        ).ToList();
                    locationChannels.ForEach(channel =>
                    {
                        // Map the entrances
                        channel.Addresses.ForEach(a =>
                        {
                            var ar = registers.FirstOrDefault(r => r.Address.UniqueId == a.Address.UniqueId);
                            if (ar != null)
                            {
                                a.Address.AccessibilityRegisterEntrances = ar.Entrances;
                            }
                        });
                        FilterOutNotPublishedLanguageVersions(channel, publishedId, true);
                    });

                    resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(locationChannels));
                }
                
                // Printable form channels
                var formChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.PrintableForm.ToString());
                var formChannels = serviceChannels.Where(s => s.TypeId == formChannelTypeId).ToList();
                if (formChannels?.Count > 0)
                {
                    var formChannelIds = formChannels.Select(i => i.Id).ToList();
                    var formChannelQuery = unitOfWork.CreateRepository<IPrintableFormChannelRepository>().All().Where(c => formChannelIds.Contains(c.ServiceChannelVersionedId));
                    var formChannelsWithData = unitOfWork.ApplyIncludes(formChannelQuery, q =>
                        q.Include(i => i.FormIdentifiers)
                         .Include(i => i.ChannelUrls)
                        ).ToList();
                    formChannelsWithData.ForEach(f =>
                    {
                        var channel = formChannels.Where(i => i.Id == f.ServiceChannelVersionedId).FirstOrDefault();
                        if (channel != null)
                        {
                            channel.PrintableFormChannels.Add(f);
                        }
                    });
                    formChannels.ForEach(channel => FilterOutNotPublishedLanguageVersions(channel, publishedId, true));
                    resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(formChannels));
                }
                
                // Web page channels
                var webpageChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.WebPage.ToString());
                var webpageChannels = serviceChannels.Where(s => s.TypeId == webpageChannelTypeId).ToList();
                if (webpageChannels?.Count > 0)
                {
                    var webpageChannelIds = webpageChannels.Select(i => i.Id).ToList();
                    var webpageChannelQuery = unitOfWork.CreateRepository<IWebpageChannelRepository>().All().Where(c => webpageChannelIds.Contains(c.ServiceChannelVersionedId));
                    var webpageChannelsWithData = unitOfWork.ApplyIncludes(webpageChannelQuery, q =>
                        q.Include(i => i.LocalizedUrls)).ToList();
                    webpageChannelsWithData.ForEach(w =>
                    {
                        var channel = webpageChannels.Where(i => i.Id == w.ServiceChannelVersionedId).FirstOrDefault();
                        if (channel != null)
                        {
                            channel.WebpageChannels.Add(w);
                        }
                    });
                    webpageChannels.ForEach(channel => FilterOutNotPublishedLanguageVersions(channel, publishedId, true));
                    resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(webpageChannels));
                }

                ////Measure
                //watch.Stop();
                //logger.LogTrace($"*** Translation: {watch.ElapsedMilliseconds} ms.");
                //watch.Restart();
                //// End measure

                // Get related services only for version 7 and upper
                if (openApiVersion > 6)
                {
                    // Find only published services for channels
                    List<Guid> rootIds = resultList.Where(c => c.Id != null).Select(c => c.Id.Value).Distinct().ToList();
                    allConnections = GetChannelServices(rootIds, unitOfWork);

                    ////Measure
                    //watch.Stop();
                    //logger.LogTrace($"*** Get connections from db: {watch.ElapsedMilliseconds} ms.");
                    //watch.Restart();
                    //// End measure                    
                }
            }

            // Map connections.
            // Set the right version for service channels
            var versionList = new List<IVmOpenApiServiceChannel>();
            resultList.ForEach(channel =>
            {
                if (allConnections?.Count > 0)
                {
                    var connections = allConnections.Where(s => s.OwnerReferenceId == channel.Id).ToList();
                    if (connections?.Count > 0)
                    {
                        channel.Services = connections;
                    }
                }
                versionList.Add(GetServiceChannelByOpenApiVersion(unitOfWork, channel, openApiVersion));
            });

            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Get right version: {watch.ElapsedMilliseconds} ms.");
            //// End measure

            return versionList;
        }

        private List<V9VmOpenApiServiceChannelService> GetChannelServices(List<Guid> rootIds, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var connectionRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var connectionQuery = connectionRep.All().Where(c => rootIds.Contains(c.ServiceChannelId) &&
                c.Service.Versions.Any(v => v.PublishingStatusId == publishedId &&
                v.LanguageAvailabilities.Any(l => l.StatusId == publishedId)));
            var connections = unitOfWork.ApplyIncludes(connectionQuery, GetConnectionIncludeChain()).ToList();

            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Get connections from db: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// End measure

            if (connections?.Count > 0)
            {
                var serviceList = new List<V9VmOpenApiServiceChannelService>();

                // Fill with service names
                var serviceRootIds = connections.Select(s => s.ServiceId).ToList();

                var services = unitOfWork.ApplyIncludes(
                    unitOfWork.CreateRepository<IServiceVersionedRepository>().All().Where(i => serviceRootIds.Contains(i.UnificRootId) && i.PublishingStatusId == publishedId),
                    q => q.Include(i => i.UnificRoot).Include(i => i.ServiceNames).Include(i => i.LanguageAvailabilities)).ToList();
                connections.ForEach(c =>
                {
                    string name = null;
                    var service = services.Where(i => i.UnificRootId == c.ServiceId).FirstOrDefault();
                    if (service != null)
                    {
                        var version = service.UnificRoot.Versions.FirstOrDefault();
                        if (version != null)
                        {
                            // Get published name for service (PTV-3689).
                            name = GetNameWithFallback(
                                version.ServiceNames,
                                version.LanguageAvailabilities.Where(i => i.StatusId == publishedId).Select(i => i.LanguageId).ToList(),
                                typesCache,
                                languageCache);
                        }
                    }
                    V9VmOpenApiServiceChannelService vm = new V9VmOpenApiServiceChannelService
                    {
                        OwnerReferenceId = c.ServiceChannelId,
                        Service = new VmOpenApiItem { Id = c.ServiceId, Name = name }
                    };

                    // map base connection data
                    MapConnection(c, vm, typesCache, languageCache);

                    // extra types
                    vm.ExtraTypes = GetExtraTypes(c, typesCache, languageCache);

                    // contactdetails
                    vm.ContactDetails = GetContactDetails<V9VmOpenApiContactDetails>(c, typesCache, languageCache);

                    // digitalAuthorizations
                    vm.DigitalAuthorizations = GetDigitalAuthorizations(c, languageCache);

                    serviceList.Add(vm);
                });

                return serviceList;
            }

            return null;
        }
        private VmOpenApiServiceChannel GetServiceChannelWithSimpleDetails(IUnitOfWork unitOfWork, Guid versionId)
        {
            if (!versionId.IsAssigned()) return null;

            ServiceChannelVersioned entity = null;
            return GetModel<ServiceChannelVersioned, VmOpenApiServiceChannel>(entity = GetEntity<ServiceChannelVersioned>(versionId, unitOfWork,
                    q => q.Include(x => x.LanguageAvailabilities)
                    .Include(i => i.ElectronicChannels)
                    .Include(i => i.Addresses)
                    .Include(i => i.WebpageChannels)
                    .Include(i => i.PrintableFormChannels)
                    .Include(i => i.ServiceChannelNames)
                    .Include(i => i.ServiceChannelDescriptions)), unitOfWork);
        }

        public IVmOpenApiServiceChannel AddServiceChannel<TVmChannelIn>(TVmChannelIn vm, bool allowAnonymous, int openApiVersion, string userName = null)
           where TVmChannelIn : class, IVmOpenApiServiceChannelIn
        {
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = userName ?? utilities.GetRelationIdForExternalSource();
            var useOtherEndPoint = false;
            var serviceChannel = new ServiceChannelVersioned();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Check if the external source already exists. Let's not throw the excpetion here to avoid contextManager to catch the exception.
                useOtherEndPoint = ExternalSourceExists<ServiceChannel>(vm.SourceId, userId, unitOfWork);
                if (!useOtherEndPoint)
                {
                    // Set user name which is used within language availabilities and check the publishing status (SFIPTV-190)
                    vm.UserName = unitOfWork.GetUserNameForAuditing();
                    if (vm.ValidFrom.HasValue && vm.ValidFrom > DateTime.UtcNow)
                    {
                        // For timed publishing the version created needs to be set as draft
                        vm.PublishingStatus = PublishingStatus.Draft.ToString();
                    }

                    // Check address related municipalities
                    if (vm is VmOpenApiPrintableFormChannelInVersionBase)
                    {
                        (vm as VmOpenApiPrintableFormChannelInVersionBase).DeliveryAddresses.ForEach(a => CheckAddress(unitOfWork, a));
                    }
                    else if (vm is VmOpenApiServiceLocationChannelInVersionBase)
                    {
                        (vm as VmOpenApiServiceLocationChannelInVersionBase).Addresses.ForEach(a => CheckAddress(unitOfWork, a));
                    }

                    serviceChannel = TranslationManagerToEntity.Translate<TVmChannelIn, ServiceChannelVersioned>(vm, unitOfWork);
                    
                    var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                    serviceChannelRep.Add(serviceChannel);

                    // Create the mapping between external source id and PTV id
                    if (!string.IsNullOrEmpty(vm.SourceId))
                    {
                        SetExternalSource(serviceChannel.UnificRoot, vm.SourceId, userId, unitOfWork);
                    }
                    commonService.AddHistoryMetaData<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannel);
                    unitOfWork.Save(saveMode, userName: userName);// We need to save the item - otherwise when adding related services we are getting error "Sequence contains no elements" (SFIPTV-529)

                    // Add related services
                    if (vm.ServiceChannelServices?.Count > 0)
                    {
                        TranslationManagerToEntity.TranslateAll<V9VmOpenApiServiceServiceChannelAstiInBase, ServiceServiceChannel>(vm.ServiceChannelServices.Select(serviceId => new V9VmOpenApiServiceServiceChannelAstiInBase { ServiceGuid = serviceId, ChannelGuid = serviceChannel.UnificRootId }), unitOfWork);
                    }
                    unitOfWork.Save(saveMode, userName: userName);
                }
            });

            if (useOtherEndPoint)
            {
                throw new ExternalSourceExistsException(string.Format(CoreMessages.OpenApi.ExternalSourceExists, vm.SourceId));
            }

            // Update the map coordinates for addresses
            if (serviceChannel.Addresses?.Count > 0)
            {
                // only for visiting addresses which are of type street
                var visitingAddressId = typesCache.Get<AddressCharacter>(AddressCharacterEnum.Visiting.ToString());
                var deliveryAddressId = typesCache.Get<AddressCharacter>(AddressCharacterEnum.Delivery.ToString());
                var streetId = typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString());
                var addresses = serviceChannel.Addresses
                    .Where(a => (a.CharacterId == visitingAddressId || a.CharacterId == deliveryAddressId) && a.Address.TypeId == streetId)
                    .Select(x => x.AddressId).ToList();
                addressService.UpdateAddress(addresses);
            }

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = commonService.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannel.Id, i => i.ServiceChannelVersionedId == serviceChannel.Id);
            }

            return GetServiceChannelWithDetails(serviceChannel.Id, openApiVersion, false);
        }

        public IVmOpenApiServiceChannel SaveServiceChannel<TVmChannelIn>(TVmChannelIn vm, bool allowAnonymous, int openApiVersion, string userName = null)
            where TVmChannelIn : class, IVmOpenApiServiceChannelIn
        {
            if (vm == null) return null;

            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = userName ?? utilities.GetRelationIdForExternalSource();
            var serviceChannel = new ServiceChannelVersioned();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Get the root id according to source id (if defined)
                if (!vm.SourceId.IsNullOrEmpty() && !vm.Id.IsAssigned())
                {
                    vm.Id = GetPTVId<ServiceChannel>(vm.SourceId, userId, unitOfWork);
                }

                // Get right version id
                vm.VersionId = versioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, vm.Id.Value, null, false);

                if (vm.VersionId.IsAssigned())
                {
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
                        var allAvailableLanguages = unitOfWork.CreateRepository<IRepository<ServiceChannelLanguageAvailability>>().All()
                            .Where(x => x.ServiceChannelVersionedId == vm.VersionId).Select(i => i.LanguageId).Select(x => languageCache.GetByValue(x)).ToHashSet();
                        vm.AvailableLanguages.ForEach(l => allAvailableLanguages.Add(l));
                        vm.AvailableLanguages = allAvailableLanguages.ToList();
                    }

                    if (vm.PublishingStatus == PublishingStatus.Deleted.ToString())
                    {
                        serviceChannel = DeleteChannel(unitOfWork, vm.VersionId);
                    }
                    else
                    {
                        // Entity needs to be restored?
                        if (vm.CurrentPublishingStatus == PublishingStatus.Deleted.ToString())
                        {
                            if (vm.PublishingStatus == PublishingStatus.Modified.ToString() || vm.PublishingStatus == PublishingStatus.Published.ToString())
                            {
                                // We need to restore already archived item
                                var publishingResult = commonService.RestoreArchivedEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, vm.VersionId.Value);
                            }
                        }

                        // Check address related municipalities
                        if (vm is VmOpenApiPrintableFormChannelInVersionBase)
                        {
                            (vm as VmOpenApiPrintableFormChannelInVersionBase).DeliveryAddresses.ForEach(a => CheckAddress(unitOfWork, a));
                        }
                        else if (vm is VmOpenApiServiceLocationChannelInVersionBase)
                        {
                            (vm as VmOpenApiServiceLocationChannelInVersionBase).Addresses.ForEach(a => CheckAddress(unitOfWork, a));
                        }

                        serviceChannel = TranslationManagerToEntity.Translate<TVmChannelIn, ServiceChannelVersioned>(vm, unitOfWork);

                        // Update the mapping between external source id and PTV id
                        if (!string.IsNullOrEmpty(vm.SourceId))
                        {
                            UpdateExternalSource<ServiceChannel>(serviceChannel.UnificRootId, vm.SourceId, userId, unitOfWork);
                        }
                    }

                    if (vm is VmOpenApiServiceLocationChannelInVersionBase)
                    {
                        if (serviceChannel.Addresses.Count > 0)
                        {
                            HandleAccessibilityRegisterAddress(unitOfWork, vm.VersionId, serviceChannel);
                        }
                        var vmServiceLocation = (vm as VmOpenApiServiceLocationChannelInVersionBase);
                        if (!vmServiceLocation.Oid.IsNullOrEmpty()) // (PTV-3841)
                        {
                            TranslationManagerToEntity.Translate<VmOpenApiStringItem, ServiceChannelSocialHealthCenter>(new VmOpenApiStringItem { Value = (vm as VmOpenApiServiceLocationChannelInVersionBase).Oid, OwnerReferenceId = vm.Id }, unitOfWork);
                        }
                        else if (vmServiceLocation.DeleteOid && vmServiceLocation.Oid.IsNullOrEmpty())
                        {
                            var rep = unitOfWork.CreateRepository<IServiceChannelSocialHealthCenterRepository>();
                            rep.BatchDelete(x => x.ServiceChannelId, vm.Id);
                        }
                    }
                    commonService.AddHistoryMetaData<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannel, setByEntity: true);
                    unitOfWork.Save(saveMode, PreSaveAction.Standard, serviceChannel, userName);
                }
            });

            if (serviceChannel == null) return null;

            // Update the map coordinates for addresses
            if (vm.PublishingStatus != PublishingStatus.Deleted.ToString())
            {
                var locationChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString());
                var printableChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.PrintableForm.ToString());

                if (serviceChannel.TypeId == locationChannelTypeId || serviceChannel.TypeId == printableChannelTypeId)
                {
                    var addresses = serviceChannel.Addresses?.Select(x => x.AddressId);
                    if (addresses != null)
                    {
                        addressService.UpdateAddress(addresses.ToList());
                    }
                }
            }

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = commonService.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannel.Id, i => i.ServiceChannelVersionedId == serviceChannel.Id);
            }

            return GetServiceChannelWithDetails(serviceChannel.Id, openApiVersion, false);
        }
        
        private void HandleAccessibilityRegisterAddress(IUnitOfWork unitOfWork, Guid? versioningId, ServiceChannelVersioned serviceChannel)
        {
            if (!serviceChannel.UnificRootId.IsAssigned()) return;
            if (!versioningId.IsAssigned()) return;
            
            var accessibilityRegister = unitOfWork.CreateRepository<IAccessibilityRegisterRepository>()
                .All()
                .SingleOrDefault(ar => ar.ServiceChannelId == serviceChannel.UnificRootId);
            if (accessibilityRegister == null) return;
                
            var arAddress = unitOfWork.CreateRepository<IAddressRepository>()
                .All()
                .SingleOrDefault(a => a.Id == accessibilityRegister.AddressId);
            if (arAddress == null) return;

            var channelAddress = unitOfWork.CreateRepository<IServiceChannelAddressRepository>()
                .All()
                .Where(a => a.Address.UniqueId == arAddress.UniqueId && a.ServiceChannelVersionedId == versioningId)
                .Select(i => i.Address)
                .FirstOrDefault();
            if (channelAddress == null) return;
            
            var clonedAddress = cloningManager.CloneEntity(channelAddress, unitOfWork);
            if (clonedAddress != null)
            {
                clonedAddress.OrderNumber = 0;
                var channelAddressRep = unitOfWork.CreateRepository<IServiceChannelAddressRepository>();
                channelAddressRep.Add(new ServiceChannelAddress
                {
                    Address = clonedAddress,
                    CharacterId = typesCache.Get<AddressCharacter>(AddressCharacterEnum.Visiting.ToString()),
                    ServiceChannelVersioned = serviceChannel
                });
            }
                        
            serviceChannel.Addresses.ForEach(a => a.Address.OrderNumber+=1);
        }
        
        #endregion

        public IVmEntityBase GetChannelLanguagesAvailabilities(Guid channelId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceLangAvailRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
                var langaugeAvailabilities = serviceLangAvailRep.All().Where(x => x.ServiceChannelVersionedId == channelId).ToList();
                return new VmEntityLanguageAvailable() { Id = channelId, LanguagesAvailability = langaugeAvailabilities.ToDictionary(i => i.LanguageId, i => i.StatusId) };
            });
        }

        private ServiceChannelVersioned DeleteChannel(IUnitOfWorkWritable unitOfWork, Guid? entityId)
        {
            return commonService.ChangeEntityToDeleted<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, entityId.Value);
        }

        /// <summary>
        /// Checks if a service channel with given identifier exists in the system.
        /// </summary>
        /// <param name="channelId">guid of the channe</param>
        /// <returns>true if a channel exists otherwise false</returns>
        public bool ChannelExists(Guid channelId)
        {
            bool chExists = false;

            if (Guid.Empty == channelId)
            {
                return chExists;
            }

            contextManager.ExecuteReader(unitOfWork =>
            {
                var chRepo = unitOfWork.CreateRepository<IServiceChannelRepository>().All();

                if (chRepo.FirstOrDefault(s => s.Id.Equals(channelId)) != null)
                {
                    chExists = true;
                }
            });

            return chExists;
        }

        public IVmEntityLockBase EntityLockedBy(Guid id)
        {
            return utilities.EntityLockedBy(id);
        }
    }
}
