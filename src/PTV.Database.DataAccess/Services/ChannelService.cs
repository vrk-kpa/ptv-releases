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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Database.Model.Interfaces;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using PTV.Database.DataAccess.EntityCloners;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Database.DataAccess.Utils.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V10;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework.ServiceManager;
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IChannelService), RegisterType.Transient)]
    internal class ChannelService : ServiceBase, IChannelService
    {
        private IContextManager contextManager;
        private ILogger logger;
        private IServiceUtilities utilities;
        private ICommonServiceInternal commonService;
        private IAddressService addressService;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private ILanguageOrderCache languageOrderCache;
        private ICloningManager cloningManager;

        public ChannelService(
            IContextManager contextManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            ILogger<ChannelService> logger,
            IServiceUtilities utilities,
            ICommonServiceInternal commonService,
            IAddressService addressService,
            ICacheManager cacheManager,
            IPublishingStatusCache publishingStatusCache,
            IVersioningManager versioningManager,
            IUserOrganizationChecker userOrganizationChecker,
            ILanguageOrderCache languageOrderCache,
            ICloningManager cloningManager) :
            base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker,
                versioningManager)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.commonService = commonService;
            this.addressService = addressService;
            this.typesCache = cacheManager.TypesCache;
            this.languageCache = cacheManager.LanguageCache;
            this.languageOrderCache = languageOrderCache;
            this.cloningManager = cloningManager;
         }

//        public IVmChannelSearch GetChannelSearch()
//        {
//            string statusDeletedCode = PublishingStatus.Deleted.ToString();
//            string statusOldPublishedCode = PublishingStatus.OldPublished.ToString();
//            string statusModifiedCode = PublishingStatus.Modified.ToString();
//
//            VmChannelSearch result = new VmChannelSearch();
//            contextManager.ExecuteReader(unitOfWork =>
//            {
//                result = new VmChannelSearch()
//                {
//                    OrganizationId = utilities.GetUserMainOrganization()
//
//                };
//                var publishingStatuses = commonService.GetPublishingStatuses();
//                var phoneNumbers = commonService.GetPhoneTypes();
//                var channelTypes = commonService.GetServiceChannelTypes();
//
//                FillEnumEntities(result,
//                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames()),
//                    () => GetEnumEntityCollectionModel("PublishingStatuses", publishingStatuses),
//                    () => GetEnumEntityCollectionModel("PhoneNumberTypes", phoneNumbers),
//                    () => GetEnumEntityCollectionModel("ChannelTypes", channelTypes)
//                );
//
//                result.SelectedPublishingStatuses = publishingStatuses.Where(x => x.Code != statusModifiedCode && x.Code != statusDeletedCode && x.Code != statusOldPublishedCode).Select(x => x.Id).ToList();
//            });
//            return result;
//        }

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

//        public VmEntityNames GetChannelNames(VmEntityBase model)
//        {
//            var result = new VmEntityNames();
//            contextManager.ExecuteReader(unitOfWork =>
//            {
//                var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
//                var channel = unitOfWork.ApplyIncludes(serviceChannelRep.All(), q =>
//                    q.Include(i => i.ServiceChannelNames)
//                        .Include(i => i.LanguageAvailabilities)).Single(x => x.Id == model.Id.Value);
//
//                result = TranslationManagerToVm.Translate<ServiceChannelVersioned, VmEntityNames>(channel);
//
//                FillEnumEntities(result,
//                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages())
//                );
//            });
//            return result;
//        }


        #region OpenApi

        public IVmOpenApiGuidPageVersionBase<VmOpenApiItem> GetServiceChannels(DateTime? date, int pageNumber, int pageSize, EntityStatusExtendedEnum status = EntityStatusExtendedEnum.Published, DateTime? dateBefore = null)
        {
            var handler = new V3GuidPageHandler<ServiceChannelVersioned, ServiceChannel>(status, date, dateBefore, PublishingStatusCache, pageNumber, pageSize);
            return GetPage(handler, status == EntityStatusExtendedEnum.Published);
        }

        public IVmOpenApiGuidPageVersionBase<VmOpenApiExpiringTask> GetTaskServiceChannels(int pageNumber, int pageSize, List<Guid> entityIds, DateTime lifeTime, List<Guid> publishingStatusIds)
        {
            var handler = new ExpiringTaskPageHandler<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(entityIds, publishingStatusIds, TranslationManagerToVm, lifeTime, pageNumber, pageSize);
            return GetTaskPage(handler);
        }

        public IVmOpenApiGuidPageVersionBase<VmOpenApiTask> GetTaskServiceChannels(int pageNumber, int pageSize, List<Guid> entityIds, List<Guid> publishingStatusIds)
        {
            var handler = new TaskPageHandler<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(entityIds, publishingStatusIds, TranslationManagerToVm, pageNumber, pageSize);
            return GetTaskPage(handler);
        }

        private Dictionary<Guid, List<ServiceChannelLanguageAvailability>> GetLanguageAvailabilities(IUnitOfWork unitOfWork, List<Guid> idList)
        {
            var langAvailabilitiesRep = unitOfWork.CreateRepository<IRepository<ServiceChannelLanguageAvailability>>();
            return langAvailabilitiesRep.All().Where(x => idList.Contains(x.ServiceChannelVersionedId)).ToList()
                .GroupBy(i => i.ServiceChannelVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());
        }

        private Dictionary<Guid, Dictionary<Guid, string>> GetNames(IUnitOfWork unitOfWork, List<Guid> idList, Dictionary<Guid, List<ServiceChannelLanguageAvailability>> languageAvailabilities = null)
        {
            var nameRep = unitOfWork.CreateRepository<IRepository<ServiceChannelName>>();
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var names = nameRep.All().Where(x => idList.Contains(x.ServiceChannelVersionedId) && (x.TypeId == nameTypeId)).OrderBy(i => i.Localization.OrderNumber)
                .Select(i => new { i.ServiceChannelVersionedId, i.Name, i.LocalizationId }).ToList().GroupBy(i => i.ServiceChannelVersionedId)
                .ToDictionary(i => i.Key, i => i.ToDictionary(x => x.LocalizationId, x => x.Name));

            // Do we need to filter out unpublished names? (PTV-3689)
            if (languageAvailabilities != null)
            {
                var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
                var publishedNames = new Dictionary<Guid, Dictionary<Guid, string>>();
                names.ForEach(name =>
                {
                    var publishedLanguageIds = languageAvailabilities.TryGetOrDefault(name.Key, new List<ServiceChannelLanguageAvailability>()).Where(l => l.StatusId == publishedId).Select(l => l.LanguageId).ToList();
                    publishedNames.Add(name.Key, name.Value.Where(n => publishedLanguageIds.Contains(n.Key)).ToDictionary(i => i.Key, i => i.Value));
                });
                return publishedNames;
            }

            return names;
        }

        private IVmOpenApiGuidPageVersionBase<TItemModel> GetPage<TItemModel>(IGuidPageWithNameHandlerBase<ServiceChannelVersioned, TItemModel> pageHandler, bool filterUnpublishedNames = true)
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

        private IVmOpenApiGuidPageVersionBase<TItemModel> GetTaskPage<TItemModel>(ITaskHandlerBase<ServiceChannelVersioned, ServiceChannelLanguageAvailability, TItemModel> pageHandler)
            where TItemModel : IVmOpenApiItem, new()
        {
            if (pageHandler.PageNumber <= 0) return pageHandler.GetModel();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var totalCount = pageHandler.Search(unitOfWork);
                if (totalCount > 0)
                {
                    pageHandler.LanguageAvailabilities = GetLanguageAvailabilities(unitOfWork, pageHandler.EntityIds);
                    pageHandler.Names = GetNames(unitOfWork, pageHandler.EntityIds);
                }
            });

            return pageHandler.GetModel();
        }

        public IList<IVmOpenApiServiceChannel> GetServiceChannels(List<Guid> idList, int openApiVersion)
        {
            if (idList.IsNullOrEmpty())
            {
                return null;
            }

            IList<Expression<Func<ServiceChannelVersioned, bool>>> filters = new List<Expression<Func<ServiceChannelVersioned, bool>>>
            {
                c => idList.Contains(c.UnificRootId)
            };
            
            try
            {
                return contextManager.ExecuteReader(unitOfWork => GetServiceChannelsWithDetails(unitOfWork, filters, openApiVersion));
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting service channels. {0}", ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }
        }

        public IVmOpenApiGuidPageVersionBase<VmOpenApiItem> GetServiceChannelsByMunicipality(Guid municipalityId, bool includeWholeCountry, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null)
        {
            var handler = new ServiceChannelsByMunicipalityPageHandler(municipalityId, includeWholeCountry, date, dateBefore, PublishingStatusCache, typesCache, pageNumber, pageSize);
            return GetPage(handler);
        }

        public IVmOpenApiGuidPageVersionBase<VmOpenApiItem> GetServiceChannelsByArea(Guid areaId, bool includeWholeCountry, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null)
        {
            var handler = new ServiceChannelsByAreaPageHandler(areaId, includeWholeCountry, typesCache, date, dateBefore, PublishingStatusCache, pageNumber, pageSize);
            return GetPage(handler);
        }

        public IVmOpenApiServiceChannel GetServiceChannelById(Guid id, int openApiVersion, VersionStatusEnum status)
        {
            return contextManager.ExecuteReader(unitOfWork => GetServiceChannelById(unitOfWork, id, openApiVersion, status));
        }

        public IVmOpenApiServiceChannel GetServiceChannelByIdSimple(Guid id, bool getOnlyPublished = true)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                try
                {
                    var entityId = getOnlyPublished ? VersioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, PublishingStatus.Published) 
                                                : VersioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, null, false);
                    
                    if (entityId.IsAssigned())
                    {
                        return GetServiceChannelWithSimpleDetails(unitOfWork, entityId.Value);
                    }
                }
                catch (Exception ex)
                {
                    var errorMsg = string.Format("Error occured while getting a channel with id {0}. {1}", id, ex.Message);
                    logger.LogError(errorMsg + " " + ex.StackTrace);
                    throw new Exception(errorMsg);
                }

                return null;
            });
        }

        public PublishingStatus? GetLatestVersionPublishingStatus(Guid id)
        {
            return contextManager.ExecuteReader(unitOfWork => VersioningManager.GetLatestVersionPublishingStatus<ServiceChannelVersioned>(unitOfWork, id));
        }

        private IVmOpenApiServiceChannel GetServiceChannelById(IUnitOfWork unitOfWork, Guid id, int openApiVersion, VersionStatusEnum status)
        {
            try
            {
                Guid? entityId = null;
                switch (status)
                {
                    case VersionStatusEnum.Published:
                        entityId = VersioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, PublishingStatus.Published);
                        break;
                    case VersionStatusEnum.Latest:
                        entityId = VersioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, null, false);
                        break;
                    case VersionStatusEnum.LatestActive:
                        entityId = VersioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, null, true);
                        break;
                    default:
                        break;
                }
                return (entityId.IsAssigned()) ? GetServiceChannelWithDetails(unitOfWork, entityId.Value, openApiVersion, status == VersionStatusEnum.Published) : null;
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting a channel with id {0}. {1}", id, ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }
        }

        public IVmOpenApiServiceChannel GetServiceChannelBySource(string sourceId)
        {
            var userId = utilities.GetRelationIdForExternalSource();
            try
            {
                var rootId = contextManager.ExecuteReader(unitOfWork => GetPTVId<ServiceChannel>(sourceId, userId, unitOfWork));
                return rootId.IsAssigned() ? GetServiceChannelByIdSimple(rootId, false) : null;
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting services by source id {0}. {1}", sourceId, ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }
        }

        public IList<IVmOpenApiServiceChannel> GetServiceChannelsWithDetailsByType(ServiceChannelTypeEnum type, DateTime? date, int openApiVersion)
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
            
            try
            {
                return contextManager.ExecuteReader(unitOfWork => GetServiceChannelsWithDetails(unitOfWork, filters, openApiVersion));
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting service channels of type {0}. {1}", type.ToString(), ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }
        }

        public IVmOpenApiGuidPageVersionBase<VmOpenApiItem> GetServiceChannelsByType(ServiceChannelTypeEnum type, DateTime? date, int pageNumber, int pageSize, bool getOnlyPublished = true, DateTime? dateBefore = null)
        {
            var typeId = typesCache.Get<ServiceChannelType>(type.ToString());
            
            var handler = new ServiceChannelsByTypePageHandler(typeId, getOnlyPublished ? EntityStatusExtendedEnum.Published : EntityStatusExtendedEnum.Active, date, dateBefore, PublishingStatusCache, pageNumber, pageSize);
            return GetPage(handler);
        }

        public IList<IVmOpenApiServiceChannel> GetServiceChannelsByOrganization(Guid organizationId, DateTime? date, int openApiVersion, ServiceChannelTypeEnum? type = null)
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
            try
            {
                return contextManager.ExecuteReader(unitOfWork => GetServiceChannelsWithDetails(unitOfWork, filters, openApiVersion));
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting service channels for organization {0}. {1}", organizationId.ToString(), ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }
        }

        public IVmOpenApiGuidPageVersionBase<VmOpenApiItem> GetServiceChannelsByOrganization(Guid organizationId, DateTime? date, int pageNumber, int pageSize, ServiceChannelTypeEnum? type = null, DateTime? dateBefore = null)
        {
            Guid? typeId = null;
            if (type.HasValue)
            {
                typeId = typesCache.Get<ServiceChannelType>(type.Value.ToString());
            }
            var handler = new ServiceChannelsByOrganizationPageHandler(organizationId, typeId, date, dateBefore, PublishingStatusCache, pageNumber, pageSize);
            return GetPage(handler);
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
                if (userOrganizations.HasData())
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

        private IVmOpenApiServiceChannel GetServiceChannelWithDetails(IUnitOfWork unitOfWork, Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiServiceChannel result = null;
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var query = serviceChannelRep.All().Where(s => s.Id == versionId);
            if (getOnlyPublished)
            {
                query = query.Where(c => c.LanguageAvailabilities.Any(l => l.StatusId == publishedId));
            }

            var channel = unitOfWork.ApplyIncludes(query, GetServiceChannelIncludeChain()).FirstOrDefault();

            if (channel != null)
            {
                // Set accessibility sentences. PTV-2481 & PTV-4237
                if (channel.Addresses.HasData() && channel.UnificRoot?.AccessibilityRegisters.HasData() == true)
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
            }
            if (result == null)
            {
                return null;
            }
            // Find only published services for a service channel - let's do this outside of translator
            // Manually map connection data.
            result.Services = GetChannelServices(new List<Guid> { channel.UnificRootId }, unitOfWork);
            result.OntologyTerms = GetAllRelatedOntologyTerms(channel.UnificRootId, unitOfWork);
            //if area not set by user set area default by areas of connected published services
            if (channel.AreaInformationTypeId == null && result.Services != null) 
            {
                AddInheritedAreaInformation(result.Services.Select(x => x.Service.Id).OfType<Guid>().ToList(), result, unitOfWork);
            }
           
            return GetServiceChannelByOpenApiVersion(unitOfWork, result, openApiVersion);
        }
        
        private void AddInheritedAreaInformation(List<Guid> connectedServiceRootIds, IVmOpenApiServiceChannel result, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }
            
            var connectedPublishedServiceIds = new List<Guid>();
            foreach (var connectedServiceRootId in connectedServiceRootIds)
            {
                var serviceVersionedId = VersioningManager.GetVersionId<ServiceVersioned>(unitOfWork, connectedServiceRootId, PublishingStatus.Published);
                if (serviceVersionedId.IsAssigned())
                {
                    connectedPublishedServiceIds.Add(serviceVersionedId.Value);
                }
            }

            if (connectedPublishedServiceIds.Any())
            {
                GetAllVmInheritedAreaInformation(connectedPublishedServiceIds, result, unitOfWork);
            }
        }
        
        private void GetAllVmInheritedAreaInformation(List<Guid> connectedPublishedServiceIds, IVmOpenApiServiceChannel result, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var connectedServiceWithAreas =
                serviceRep.All().Where(s => connectedPublishedServiceIds.Contains(s.Id))
                    .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaNames)
                    .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                    .Include(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                    .ToList();

            var wholeCountryId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
            var wholeCountryExceptAlandId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
            var areaTypeId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString());
            
            if (connectedServiceWithAreas.Any(x => x.AreaInformationTypeId == areaTypeId))
            {
                var areaMunicipalities = connectedServiceWithAreas.SelectMany(x => x.AreaMunicipalities).Select(y => y.Municipality).DistinctBy(z => z.Id).ToList();
                result.AreaMunicipalities = TranslationManagerToVm.TranslateAll<Municipality, VmOpenApiMunicipality>(areaMunicipalities).ToList();

                var areas = connectedServiceWithAreas.SelectMany(x => x.Areas).Select(y => y.Area).DistinctBy(z => z.Id).ToList();
                result.Areas = TranslationManagerToVm.TranslateAll<Area, VmOpenApiArea>(areas).ToList();
            }
            else if (connectedServiceWithAreas.Any(x => x.AreaInformationTypeId == wholeCountryExceptAlandId))
            {
                result.AreaType = AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString().GetOpenApiEnumValue<AreaInformationTypeEnum>();
            }
            else if (connectedServiceWithAreas.Any(x => x.AreaInformationTypeId == wholeCountryId))
            {
                result.AreaType =  AreaInformationTypeEnum.WholeCountry.ToString().GetOpenApiEnumValue<AreaInformationTypeEnum>();
            }
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
                .Include(i => i.Attachments).ThenInclude(i => i.Attachment)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreet).ThenInclude(i => i.StreetNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreet).ThenInclude(i => i.StreetNumbers).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreetNumber)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.Municipality)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
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
                .Include(i => i.UnificRoot).ThenInclude(i => i.SocialHealthCenters)
                .Include(i => i.AccessibilityClassifications).ThenInclude(i => i.AccessibilityClassification);
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
                .Include(i => i.Attachments).ThenInclude(i => i.Attachment)
                .Include(i => i.Languages).ThenInclude(i => i.Language)
                .Include(j => j.Emails).ThenInclude(j => j.Email)
                .Include(j => j.Phones).ThenInclude(j => j.Phone).ThenInclude(i => i.PrefixNumber)
                .Include(i => i.LanguageAvailabilities)
                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaNames)
                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.UnificRoot)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreet).ThenInclude(i => i.StreetNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreet).ThenInclude(i => i.StreetNumbers).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreetNumber)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.Municipality)
                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
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
                if (notPublishedLanguageVersions.HasData())
                {
                    channel.ServiceChannelNames = channel.ServiceChannelNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    channel.ServiceChannelDescriptions = channel.ServiceChannelDescriptions.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    channel.WebPages = channel.WebPages.Where(i => !notPublishedLanguageVersions.Contains(i.WebPage.LocalizationId)).ToList();
                    if (channel.ServiceChannelServiceHours.HasData())
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

//        private IList<IVmOpenApiServiceChannel> GetServiceChannelsWithDetailsOld(IUnitOfWork unitOfWork, List<Guid> versionIdList, int openApiVersion, bool getOnlyPublished = true)
//        {
//            if (versionIdList.Count == 0) return new List<IVmOpenApiServiceChannel>();
//
//            //// Measure
//            //var watch = new Stopwatch();
//            //logger.LogTrace("****************************************");
//            //logger.LogTrace("*** Old implementation ***");
//            //logger.LogTrace($"GetServiceChannelWithDetails starts. Ids: { string.Join( ", ", versionIdList)}");
//            //watch.Start();
//            //// end measure
//
//            var resultList = new List<IVmOpenApiServiceChannel>();
//            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
//            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
//
//            var queryWithData = unitOfWork.ApplyIncludes(serviceChannelRep.All().Where(c => versionIdList.Contains(c.Id)), q =>
//                q.Include(i => i.ServiceChannelNames)
//                .Include(i => i.ServiceChannelDescriptions)
//                .Include(i => i.Type)
//                .Include(i => i.WebPages).ThenInclude(i => i.WebPage)
//                .Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours).ThenInclude(j => j.DailyOpeningTimes)
//                .Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours).ThenInclude(j => j.AdditionalInformations)
//                .Include(i => i.Attachments).ThenInclude(i => i.Attachment)
//                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
//                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
//                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
//                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
//                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
//                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
//                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
//                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
//                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
//                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
//                .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Receivers)
//                .Include(i => i.ElectronicChannels).ThenInclude(i => i.LocalizedUrls)
//                .Include(i => i.Languages).ThenInclude(i => i.Language)
//                .Include(i => i.WebpageChannels).ThenInclude(i => i.LocalizedUrls)
//                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.FormIdentifiers)
//                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.ChannelUrls)
//                .Include(j => j.Emails).ThenInclude(j => j.Email)
//                .Include(j => j.Phones).ThenInclude(j => j.Phone).ThenInclude(i => i.PrefixNumber)
//                .Include(i => i.LanguageAvailabilities)
//                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaNames)
//                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
//                .Include(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.Service).ThenInclude(i => i.Versions)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDescriptions)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDigitalAuthorizations)
//                    .ThenInclude(i => i.DigitalAuthorization).ThenInclude(i => i.Names)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelExtraTypes).ThenInclude(i => i.ExtraSubType)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelExtraTypes).ThenInclude(i => i.ServiceServiceChannelExtraTypeDescriptions)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelServiceHours).ThenInclude(i => i.ServiceHours).ThenInclude(i => i.DailyOpeningTimes)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelServiceHours).ThenInclude(i => i.ServiceHours).ThenInclude(i => i.AdditionalInformations)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelEmails).ThenInclude(i => i.Email)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelPhones).ThenInclude(i => i.Phone).ThenInclude(i => i.PrefixNumber)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelWebPages).ThenInclude(i => i.WebPage)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
//                    .ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
//                    .ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
//                    .ThenInclude(i => i.AddressStreets).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
//                    .ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
//                    .ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
//                    .ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
//                    .ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
//                .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
//            );
//
//            // Filter out items that do not have language versions published!
//            var serviceChannels = getOnlyPublished ? queryWithData.Where(c => c.LanguageAvailabilities.Any(l => l.StatusId == publishedId)).ToList() : queryWithData.ToList();
//
//            //// Measure
//            //watch.Stop();
//            //logger.LogTrace($"*** Fetch from db: {watch.ElapsedMilliseconds} ms.");
//            //watch.Restart();
//            //// end measure
//
//            var allPublishedServices = serviceChannels.SelectMany(i => i.UnificRoot.ServiceServiceChannels).Select(i => i.Service)
//                .SelectMany(i => i.Versions).Where(i => i.PublishingStatusId == publishedId).ToList();
//            var publishedServiceRootIds = allPublishedServices.Select(i => i.UnificRootId).ToList();
//            var publishedServiceIds = allPublishedServices.Select(i => i.Id).ToList();
//
//            serviceChannels.ForEach(
//                channel =>
//                {
//                    // Filter out not published services
//                    channel.UnificRoot.ServiceServiceChannels = channel.UnificRoot.ServiceServiceChannels.Where(s => publishedServiceRootIds.Contains(s.ServiceId)).ToList();
//
//                    FilterOutNotPublishedLanguageVersions(channel, publishedId, getOnlyPublished);
//                }
//            );
//
//            // Fill with service names
//            var serviceNames = unitOfWork.CreateRepository<IServiceNameRepository>().All().Where(i => publishedServiceIds.Contains(i.ServiceVersionedId)).ToList()
//                .GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.ToList());
//            allPublishedServices.ForEach(c =>
//            {
//                c.ServiceNames = serviceNames.TryGet(c.Id);
//            });
//
//
//            //// Measure
//            //watch.Stop();
//            //logger.LogTrace($"*** Filter out not published items: {watch.ElapsedMilliseconds} ms.");
//            //watch.Restart();
//            //// end measure
//
//
//            if (serviceChannels?.Count > 0)
//            {
//                var eChannelId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.EChannel.ToString());
//                resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(serviceChannels.Where(s => s.TypeId == eChannelId).ToList()));
//                var phoneChannelId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.Phone.ToString());
//                resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(serviceChannels.Where(s => s.TypeId == phoneChannelId).ToList()));
//                var serviceLocationChannelId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString());
//                resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(serviceChannels.Where(s => s.TypeId == serviceLocationChannelId).ToList()));
//                var transactionFormChannelId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.PrintableForm.ToString());
//                resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(serviceChannels.Where(s => s.TypeId == transactionFormChannelId).ToList()));
//                var webpageChannelId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.WebPage.ToString());
//                resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(serviceChannels.Where(s => s.TypeId == webpageChannelId).ToList()));
//            }
//
//            ////Measure
//            //watch.Stop();
//            //logger.LogTrace($"*** Translation: {watch.ElapsedMilliseconds} ms.");
//            //watch.Restart();
//            //// End measure
//
//            // Set the right version for service channels
//            var versionList = new List<IVmOpenApiServiceChannel>();
//            resultList.ForEach(channel =>
//            {
//                versionList.Add(GetServiceChannelByOpenApiVersion(unitOfWork, channel, openApiVersion));
//            });
//
//            ////Measure
//            //watch.Stop();
//            //logger.LogTrace($"*** Get right version: {watch.ElapsedMilliseconds} ms.");
//            //// End measure
//
//            return versionList;
//        }

        private IList<IVmOpenApiServiceChannel> GetServiceChannelsWithDetails(IUnitOfWork unitOfWork, IList<Expression<Func<ServiceChannelVersioned, bool>>> filters, int openApiVersion)
        {
            var resultList = new List<IVmOpenApiServiceChannel>();
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            // Get only published items - filter out items that do not have any language versions published.
            var query = serviceChannelRep.All().Where(e => e.PublishingStatusId == publishedId && e.LanguageAvailabilities.Any(l => l.StatusId == publishedId));
            filters.ForEach(a => query = query.Where(a));
            
            var totalCount = query.Count();
            if (totalCount > 100 && openApiVersion > 8) // The amount checking should only be done for version 8+ (SFIPTV-568).
            {
                throw new Exception(CoreMessages.OpenApi.TooManyItems);
            }
            if (totalCount == 0)
            {
                return null;
            }
            var serviceChannels = unitOfWork.ApplyIncludes(query, GetServiceChannelBaseIncludeChain()).ToList();

            List<V10VmOpenApiServiceChannelService> allConnections = null;

            if (serviceChannels.HasData())
            {
                // E channels
                var eChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.EChannel.ToString());
                var eChannels = serviceChannels.Where(s => s.TypeId == eChannelTypeId).ToList();
                if (eChannels.HasData())
                {
                    var eChannelIds = eChannels.Select(i => i.Id).ToList();
                    var eChannelQuery = unitOfWork.CreateRepository<IElectronicChannelRepository>().All().Where(c => eChannelIds.Contains(c.ServiceChannelVersionedId));
                    var eChannelsWithData = unitOfWork.ApplyIncludes(eChannelQuery, q =>
                        q.Include(i => i.LocalizedUrls)).ToList();
                    eChannelsWithData.ForEach(e =>
                    {
                        var channel = eChannels.FirstOrDefault(i => i.Id == e.ServiceChannelVersionedId);
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
                if (phoneChannels.HasData())
                {
                    phoneChannels.ForEach(channel => FilterOutNotPublishedLanguageVersions(channel, publishedId, true));
                    resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(phoneChannels));
                }
                
                // Service location channels
                var serviceLocationChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString());
                var locationChannels = serviceChannels.Where(s => s.TypeId == serviceLocationChannelTypeId).ToList();
                if (locationChannels.HasData())
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
                if (formChannels.HasData())
                {
                    var formChannelIds = formChannels.Select(i => i.Id).ToList();
                    var formChannelQuery = unitOfWork.CreateRepository<IPrintableFormChannelRepository>().All().Where(c => formChannelIds.Contains(c.ServiceChannelVersionedId));
                    var formChannelsWithData = unitOfWork.ApplyIncludes(formChannelQuery, q =>
                        q.Include(i => i.FormIdentifiers)
                         .Include(i => i.ChannelUrls)
                        ).ToList();
                    formChannelsWithData.ForEach(f =>
                    {
                        var channel = formChannels.FirstOrDefault(i => i.Id == f.ServiceChannelVersionedId);
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
                if (webpageChannels.HasData())
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

                // Get related services only for version 7 and upper
                if (openApiVersion > 6)
                {
                    // Find only published services for channels
                    List<Guid> rootIds = resultList.Where(c => c.Id != null).Select(c => c.Id.Value).Distinct().ToList();
                    allConnections = GetChannelServices(rootIds, unitOfWork);
                }
            }

            // Map connections.
            // Set the right version for service channels
            var versionList = new List<IVmOpenApiServiceChannel>();
            resultList.ForEach(channel =>
            {
                if (allConnections.HasData())
                {
                    var connections = allConnections.Where(s => s.OwnerReferenceId == channel.Id).ToList();
                    if (connections.HasData())
                    {
                        channel.Services = connections;
                    }
                }
                versionList.Add(GetServiceChannelByOpenApiVersion(unitOfWork, channel, openApiVersion));
            });

            return versionList;
        }

        private IList<V4VmOpenApiFintoItem> GetAllRelatedOntologyTerms(Guid unificRootId, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var services = serviceRep.All()
                .Where(c => c.UnificRoot.ServiceServiceChannels.Any(x => x.ServiceChannelId == unificRootId))
                .Where(c => c.PublishingStatusId == publishedId)
                .Include(c => c.ServiceOntologyTerms).ThenInclude(x => x.OntologyTerm).ThenInclude(x => x.Names)
                .ToList();
            var serviceOntologyTerms = services.SelectMany(x => x.ServiceOntologyTerms.Select(y => y.OntologyTerm)).ToList();
                
            var gdUnificRootIds = services.Where(x=>x.StatutoryServiceGeneralDescriptionId.HasValue)
                .Select(x => x.StatutoryServiceGeneralDescriptionId)
                .ToList();
            if (gdUnificRootIds.Any())
            {
                var gdRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var gdOntologyTerms = gdRep.All()
                    .Where(c =>  gdUnificRootIds.Contains(c.UnificRootId))
                    .Where(c => c.PublishingStatusId == publishedId)
                    .Include(c => c.OntologyTerms).ThenInclude(x => x.OntologyTerm).ThenInclude(x => x.Names)
                    .ToList()
                    .SelectMany(x => x.OntologyTerms.Select(y => y.OntologyTerm));
                serviceOntologyTerms.AddRange(gdOntologyTerms);
            }
            return TranslationManagerToVm.TranslateAll<OntologyTerm, V4VmOpenApiFintoItem>(serviceOntologyTerms.DistinctBy(x => x.Id)).ToList();
        }

        private List<V10VmOpenApiServiceChannelService> GetChannelServices(List<Guid> rootIds, IUnitOfWork unitOfWork)
        {
            var serviceList = new List<V10VmOpenApiServiceChannelService>();

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

            if (connections.HasData())
            {
                // Fill with service names
                var serviceRootIds = connections.Select(s => s.ServiceId).ToList();

                var services = unitOfWork.ApplyIncludes(
                    unitOfWork.CreateRepository<IServiceVersionedRepository>().All().Where(i => serviceRootIds.Contains(i.UnificRootId) && i.PublishingStatusId == publishedId),
                    q => q.Include(i => i.UnificRoot).Include(i => i.ServiceNames).Include(i => i.LanguageAvailabilities)).ToList();
                connections.ForEach(c =>
                {
                    string name = null;
                    var service = services.FirstOrDefault(i => i.UnificRootId == c.ServiceId);
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
                    V10VmOpenApiServiceChannelService vm = new V10VmOpenApiServiceChannelService
                    {
                        OwnerReferenceId = c.ServiceChannelId,
                        Service = new VmOpenApiItem { Id = c.ServiceId, Name = name },
                        Modified = c.Modified
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

            return serviceList;
        }
        
        private VmOpenApiServiceChannel GetServiceChannelWithSimpleDetails(IUnitOfWork unitOfWork, Guid versionId)
        {
            if (!versionId.IsAssigned()) return null;
           
            return GetModel<ServiceChannelVersioned, VmOpenApiServiceChannel>(GetEntity<ServiceChannelVersioned>(versionId, unitOfWork,
                    q => q.Include(x => x.LanguageAvailabilities)
                    .Include(i => i.ElectronicChannels)
                    .Include(i => i.Addresses)
                    .Include(i => i.WebpageChannels)
                    .Include(i => i.PrintableFormChannels)
                    .Include(i => i.ServiceChannelNames)
                    .Include(i => i.ServiceChannelDescriptions)), unitOfWork);
        }

        public IVmOpenApiServiceChannel AddServiceChannel<TVmChannelIn>(TVmChannelIn vm, int openApiVersion, string userName = null)
           where TVmChannelIn : class, IVmOpenApiServiceChannelIn
        {
            var saveMode = SaveMode.Normal;
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
                    
                    ValidateExpirationTime(unitOfWork, serviceChannel, DateTime.UtcNow);
                    
                    var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                    serviceChannelRep.Add(serviceChannel);

                    // Create the mapping between external source id and PTV id
                    if (!string.IsNullOrEmpty(vm.SourceId))
                    {
                        SetExternalSource(serviceChannel.UnificRoot, vm.SourceId, userId, unitOfWork);
                    }
                    commonService.AddHistoryMetaData<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannel, setByEntity:true);
                    unitOfWork.Save(saveMode, userName: userName);// We need to save the item - otherwise when adding related services we are getting error "Sequence contains no elements" (SFIPTV-529)

                    // Add related services
                    if (vm.ServiceChannelServices.HasData())
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
            if (serviceChannel.Addresses.HasData())
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

        private void ValidateExpirationTime(IUnitOfWorkWritable unitOfWork, ServiceChannelVersioned channel, DateTime lastChangeDate)
        {
            var expirationTime = base.GetExpirationTime(unitOfWork, channel, lastChangeDate: lastChangeDate);
            if (channel.LanguageAvailabilities.Any(la => la.PublishAt > expirationTime))
            {
                throw new PtvAppException("Publishing date cannot be scheduled after automatic archiving date.", "Channel.ScheduleException.LateDate");
            }
        }

        public IVmOpenApiServiceChannel SaveServiceChannel<TVmChannelIn>(TVmChannelIn vm, int openApiVersion, string userName = null)
            where TVmChannelIn : class, IVmOpenApiServiceChannelIn
        {
            if (vm == null) return null;

            var saveMode = SaveMode.Normal;
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
                vm.VersionId = VersioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, vm.Id.Value, null, false);

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
                        
                        ValidateExpirationTime(unitOfWork, serviceChannel, DateTime.UtcNow);

                        // Update the mapping between external source id and PTV id
                        if (!string.IsNullOrEmpty(vm.SourceId))
                        {
                            UpdateExternalSource<ServiceChannel>(serviceChannel.UnificRootId, vm.SourceId, userId, unitOfWork);
                        }
                    }

                    if (vm is VmOpenApiServiceLocationChannelInVersionBase)
                    {
                        if (serviceChannel.Addresses.HasData())
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
                if (vm.VersionId.HasValue && !vm.IsVisibleForAll)
                {
                    // If service channel is changed to not common, remove connected services
                    // from other organizations
                    contextManager.ExecuteWriter(unitOfWork =>
                    {
                        commonService.RemoveNotCommonConnections(new List<Guid> {vm.VersionId.Value}, unitOfWork,
                            false);
                        unitOfWork.Save(saveMode, PreSaveAction.Standard, serviceChannel, userName);
                    });
                }
                
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

//        public IVmEntityBase GetChannelLanguagesAvailabilities(Guid channelId)
//        {
//            return contextManager.ExecuteReader(unitOfWork =>
//            {
//                var serviceLangAvailRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
//                var langaugeAvailabilities = serviceLangAvailRep.All().Where(x => x.ServiceChannelVersionedId == channelId).ToList();
//                return new VmEntityLanguageAvailable() { Id = channelId, LanguagesAvailability = langaugeAvailabilities.ToDictionary(i => i.LanguageId, i => i.StatusId) };
//            });
//        }

        private ServiceChannelVersioned DeleteChannel(IUnitOfWorkWritable unitOfWork, Guid? entityId)
        {
            if (entityId.HasValue)
            {
                commonService.DeleteServiceChannelConnections(unitOfWork, entityId.Value);
            }
            return commonService.ChangeEntityToDeleted<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, entityId.Value);
        }

        /// <summary>
        /// Checks if a service channel with given identifier exists in the system.
        /// </summary>
        /// <param name="channelId">guid of the channel</param>
        /// <returns>true if a channel exists otherwise false</returns>
        public bool ChannelExists(Guid channelId)
        {
            return channelId.IsAssigned() && contextManager.ExecuteReader(unitOfWork => unitOfWork.CreateRepository<IServiceChannelRepository>().All().Any(s => s.Id == channelId));
        }

        public IVmEntityLockBase EntityLockedBy(Guid id)
        {
            return utilities.EntityLockedBy(id);
        }
    }
}
