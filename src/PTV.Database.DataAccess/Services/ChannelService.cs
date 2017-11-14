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
using System.Net;
using System.Xml.Linq;
using PTV.Domain.Logic.Channels;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Logic;
using PTV.Database.DataAccess.Utils;
using PTV.Framework.Exceptions;
using Microsoft.Extensions.Options;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework.ServiceManager;
using PTV.Database.Model.Interfaces;
using System.Diagnostics;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.V2.Common;
using VmElectronicChannel = PTV.Domain.Model.Models.VmElectronicChannel;
using VmPhoneChannel = PTV.Domain.Model.Models.VmPhoneChannel;
using VmWebPageChannel = PTV.Domain.Model.Models.VmWebPageChannel;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IChannelService), RegisterType.Transient)]
    internal class ChannelService : ServiceBase, IChannelService
    {
        private IContextManager contextManager;
        private readonly IUserIdentification userIdentification;
        private ILogger logger;
        private const string invalidElectronicChannelUrl = "Electronic channel url '{0}'";
        private const string invalidWebPageChannelUrl = "Web page channel url '{0}'";
        private const string invalidElectronicChannelAttachmentUrl = "Electronic channel attachment url '{0}'";
        private const string publishedOrganizationDoesNotExist = "Channel.Save.OrganizationMissing.MessageFailed";
        private ServiceChannelLogic channelLogic;
        private ServiceUtilities utilities;
        private ICommonServiceInternal commonService;
        private VmListItemLogic listItemLogic;
        private readonly DataUtils dataUtils;
        private VmOwnerReferenceLogic ownerReferenceLogic;
        private IAddressService addressService;
        private IUrlService urlService;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private IVersioningManager versioningManager;
        private IUserOrganizationChecker userOrganizationChecker;

        public ICollection<ServiceChannelLanguageAvailability> LanguageAvailabilities
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public ChannelService(
            IContextManager contextManager,
            IUserIdentification userIdentification,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            ILogger<ChannelService> logger,
            ServiceChannelLogic channelLogic,
            ServiceUtilities utilities,
            ICommonServiceInternal commonService,
            VmListItemLogic listItemLogic,
            DataUtils dataUtils,
            VmOwnerReferenceLogic ownerReferenceLogic,
            IAddressService addressService,
            ICacheManager cacheManager,
            IPublishingStatusCache publishingStatusCache,
            IVersioningManager versioningManager,
            IUserOrganizationChecker userOrganizationChecker,
            IUrlService urlService) :
            base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.channelLogic = channelLogic;
            this.userIdentification = userIdentification;
            this.utilities = utilities;
            this.commonService = commonService;
            this.listItemLogic = listItemLogic;
            this.dataUtils = dataUtils;
            this.ownerReferenceLogic = ownerReferenceLogic;
            this.addressService = addressService;
            this.typesCache = cacheManager.TypesCache;
            this.languageCache = cacheManager.LanguageCache;
            this.versioningManager = versioningManager;
            this.userOrganizationChecker = userOrganizationChecker;
            this.urlService = urlService;
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
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("PublishingStatuses", publishingStatuses),
                    () => GetEnumEntityCollectionModel("PhoneNumberTypes", phoneNumbers),
                    () => GetEnumEntityCollectionModel("ChannelTypes", channelTypes)
                );

                result.SelectedPublishingStatuses = publishingStatuses.Where(x => x.Code != statusModifiedCode && x.Code != statusDeletedCode && x.Code != statusOldPublishedCode).Select(x => x.Id).ToList();
            });
            return result;
        }

        public IVmChannelSearchResult SearchChannelResult(VmChannelSearchParams vm)
        {
            vm.Name = vm.Name != null ? vm.Name.Trim() : vm.Name;
            IReadOnlyList<IVmChannelListItem> result = new List<VmChannelListItem>();
            bool moreAvailable = false;
            int count = 0;
            int safePageNumber = vm.PageNumber.PositiveOrZero();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var languageCode = SetTranslatorLanguage(vm);
                var formIdentifier = vm.ChannelFormIdentifier?.ToLower();
                var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
                var publishedStatusId = PublishingStatusCache.Get(PublishingStatus.Published);
                var languagesIds = vm.Languages.Select(language => languageCache.Get(language.ToString()));
                var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                var resultTemp = channelRep.All();


                #region FilteringData

                if (!string.IsNullOrEmpty(vm.ChannelType))
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

                if (vm.OrganizationId != null)
                {
                    resultTemp = resultTemp.Where(sc => sc.OrganizationId != null && sc.OrganizationId == vm.OrganizationId);
                }

                if (vm.SelectedPublishingStatuses != null)
                {
                    commonService.ExtendPublishingStatusesByEquivalents(vm.SelectedPublishingStatuses);
                    resultTemp = resultTemp.WherePublishingStatusIn(vm.SelectedPublishingStatuses);
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

                resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
//                    q.Include(i => i.ServiceChannelNames)
//                    .Include(i => i.PublishingStatus)
//                    .Include(i => i.Phones).ThenInclude(i => i.Phone)
//                    .Include(i => i.PrintableFormChannels)
//                    .Include(j => j.LanguageAvailabilities)
                    q.Include(j => j.Versioning)
//                    .Include(j => j.PrintableFormChannels).ThenInclude(j => j.FormIdentifiers)
//                    .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.Service).ThenInclude(i => i.Versions)
                    );

                #endregion FilteringData

                count = resultTemp.Count();
                moreAvailable = count.MoreResultsAvailable(safePageNumber);

                var nameType = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());

                var resultTempData = resultTemp.Select(i => new
                {
                    Id = i.Id,
                    PublishingStatusId = i.PublishingStatusId,
                    UnificRootId = i.UnificRootId,
                    TypeId = i.TypeId,
                    Name = i.ServiceChannelNames.OrderBy(x=>x.Localization.OrderNumber).FirstOrDefault(x => languagesIds.Contains(x.LocalizationId) && x.TypeId == nameTypeId).Name,
                    AllNames = i.ServiceChannelNames.Where(x => x.TypeId == nameType).Select(x => new { x.LocalizationId, x.Name }),
                    LanguageAvailabilities = i.LanguageAvailabilities.OrderBy(x => x.Language.OrderNumber),
                    OrganizationId = i.OrganizationId,
                    Versioning = i.Versioning,
                    VersionMajor = i.Versioning.VersionMajor,
                    VersionMinor = i.Versioning.VersionMinor,
                    Modified = i.Modified,
                    ModifiedBy = i.ModifiedBy,
                })
                .ApplySortingByVersions(vm.SortData, new VmSortParam() { Column = "Modified", SortDirection = SortDirectionEnum.Desc })
                    .Select(i => new
                    {
                        Id = i.Id,
                        PublishingStatusId = i.PublishingStatusId,
                        UnificRootId = i.UnificRootId,
                        TypeId = i.TypeId,
                        OrganizationId = i.OrganizationId,
                        Versioning = i.Versioning,
                        VersionMajor = i.Versioning.VersionMajor,
                        VersionMinor = i.Versioning.VersionMinor,
                        Modified = i.Modified,
                        ModifiedBy = i.ModifiedBy,
                        test = i.ModifiedBy
                    })
                .ApplyPagination(safePageNumber)
                .ToList();

                var serviceChannelIds = resultTempData.Select(i => i.Id).ToList();
                var serviceChannelNameRep = unitOfWork.CreateRepository<IServiceChannelNameRepository>();
                var serviceChannelNames = serviceChannelNameRep.All().Where(x => serviceChannelIds.Contains(x.ServiceChannelVersionedId) && languagesIds.Contains(x.LocalizationId) && x.TypeId == nameTypeId).OrderBy(i => i.Localization.OrderNumber).Select(i => new { i.ServiceChannelVersionedId, i.Name, i.LocalizationId }).ToList().GroupBy(i => i.ServiceChannelVersionedId)
                    .ToDictionary(i => i.Key, i => i.ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));
                var channelLangAvailabilitiesRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
                var channelLangAvailabilities = channelLangAvailabilitiesRep.All().Where(x => serviceChannelIds.Contains(x.ServiceChannelVersionedId)).OrderBy(x => x.Language.OrderNumber).ToList()
                    .GroupBy(i => i.ServiceChannelVersionedId).ToDictionary(i => i.Key, i => i.ToList());
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
                    Version = TranslationManagerToVm.Translate<Versioning, VmVersion>(i.Versioning),
                    Modified = i.Modified.ToEpochTime(),
                    ModifiedBy = i.ModifiedBy,
                })
                .ToList();
            });

            return new VmChannelSearchResultResult() {
                Channels = result,
                ChannelType = vm.ChannelType,
                PageNumber = ++safePageNumber,
                MoreAvailable = moreAvailable,
                Count = count
            };
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

        //Step4
        public IVmChannelServiceStep GetChannelServiceStep(IVmEntityGet vm)
        {
            var result = new VmChannelServiceStep();

            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(vm);
                var channelRootId = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>().All().Where(x => x.Id == vm.Id).Select(sv => sv.UnificRootId).FirstOrDefault();

                if (channelRootId != null)
                {
                    var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    var resultTemp = serviceRep.All().Where(x => x.UnificRoot.ServiceServiceChannels.Any(y => y.ServiceChannelId == channelRootId));

                    resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                       q.Include(i => i.ServiceNames)
                            .ThenInclude(i => i.Type)
                        .Include(i => i.Type)
                        .Include(i => i.StatutoryServiceGeneralDescription)
                            .ThenInclude(i => i.Versions)
                            .ThenInclude(i => i.Type)
                        .Include(i => i.LanguageAvailabilities)
                            .ThenInclude(i => i.Language)
                        .Include(i => i.UnificRoot)
                            .ThenInclude(i => i.ServiceServiceChannels)
                            .ThenInclude(i => i.ServiceChannel)
                            .ThenInclude(i => i.Versions)
                            .ThenInclude(i => i.LanguageAvailabilities)
                            .ThenInclude(x => x.Language)
                        );

                    resultTemp = versioningManager.ApplyPublishingStatusOrderByPriorityFallback(resultTemp);

                    var resultData = resultTemp.ToList();
                    var serviceServiceChannels = resultData.SelectMany(x => x.UnificRoot.ServiceServiceChannels.Where(y => y.ServiceChannelId == channelRootId));

                    result.Id = vm.Id;
                    result.ConnectedServices = serviceServiceChannels.Count();
                    result.Services = TranslationManagerToVm.TranslateAll<ServiceVersioned, VmServiceListItem>(resultData);
                    serviceServiceChannels.ForEach(i => i.RequestedForServiceChannel = vm.Id);
                    result.Connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, Domain.Model.Models.VmConnection>(serviceServiceChannels);
                }
            });
            return result;
        }

        public IVmChannelSearchResult RelationSearchChannels(VmChannelSearchParams vm)
        {
            vm.Name = vm.Name != null ? vm.Name.Trim() : vm.Name;
            IReadOnlyList<IVmChannelListItem> result = new List<VmChannelListItem>();
            bool moreAvailable = false;
            var count = 0;

            contextManager.ExecuteReader(unitOfWork =>
            {
                var languageCode = SetTranslatorLanguage(vm).ToString();
                var languageId = languageCache.Get(languageCode);
                var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                var resultTemp = channelRep.All();

                #region FilteringData

                if (vm.ChannelTypeId.IsAssigned())
                {
                    var channelTypeId = vm.ChannelTypeId.Value;
                    resultTemp = resultTemp.Where(sc => sc.TypeId == channelTypeId);
                }
                if (vm.OrganizationId.IsAssigned())
                {
                    var organizationId = vm.OrganizationId.Value;
                    resultTemp = resultTemp.Where(sc => sc.OrganizationId == organizationId);
                }

                if (!string.IsNullOrEmpty(vm.Name))
                {

                    var rootId = GetRootIdFromString(vm.Name);
                    if (!rootId.IsAssigned())
                    {
                        var name = vm.Name.ToLower();
                        //var languagesIds = vm.Languages.Select(language => languageCache.Get(language.ToString())).ToList();
                        resultTemp = resultTemp.Where(sc => sc.ServiceChannelNames.Any(y => y.Name.ToLower().Contains(name) && y.LocalizationId == languageId)); //languagesIds.Any(l => l == y.LocalizationId)));
                    }
                    else
                    {
                        var rootIdGuid = rootId.Value;
                        resultTemp = resultTemp.Where(channel => channel.UnificRootId == rootIdGuid);
                    }
                }
                else
                {
                    resultTemp = resultTemp.Where(sc => sc.ServiceChannelNames.Any(y => y.LocalizationId == languageId && y.Name != null));
                }




                //FilteredRootIds
                //var serviceChannelRootIds = resultTemp.Select(x => x.UnificRootId);
                //var resultTempData = channelRep.All();
                var resultData = resultTemp;// resultTempData.Where(x => serviceChannelRootIds.Any(y => x.UnificRootId == y));

                //                resultData = unitOfWork.ApplyIncludes(resultData, q =>
                //                    q.Include(i => i.ServiceChannelNames)
                ////                    .Include(i => i.LanguageAvailabilities)
                ////                    .Include(i => i.UnificRoot)
                ////                        .ThenInclude(i => i.ServiceServiceChannels)
                ////                        .ThenInclude(i => i.Service)
                ////                        .ThenInclude(i => i.Versions)
                //                    );
                resultData = versioningManager.ApplyPublishingStatusOrderByPriorityFallback(resultData);

                var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                if (utilities.UserHighestRole() != UserRoleEnum.Eeva)
                {
                    var psCommonForAll = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.CommonForAll.ToString());
                    var userOrgs = utilities.GetAllUserOrganizations(unitOfWork);
                    if (userOrgs.Count == 0)
                    {
                        resultData = Enumerable.Empty<ServiceChannelVersioned>().AsQueryable();
                    }
                    else
                    {
                        resultData = resultData.Where(i => userOrgs.Contains(i.OrganizationId) ||
                                                            (i.PublishingStatusId == psPublished &&
                                                             i.ConnectionTypeId == psCommonForAll));
                    }
                }

                #endregion FilteringData
                var excludedChannelRootIds = PrefilterServiceChannelRootIds(vm.ServiceChannelRelations);
                if (!excludedChannelRootIds.IsNullOrEmpty())
                {
                    resultData = resultData.Where(x => !excludedChannelRootIds.Contains(x.UnificRootId));
                }
                var resultFromDb = resultData.ApplyPaging(vm.PageNumber);
                var channelsIds = resultFromDb.Data.Select(i => i.Id).ToList();
                var names = unitOfWork.CreateRepository<IServiceChannelNameRepository>().All().Where(i => channelsIds.Contains(i.ServiceChannelVersionedId)).GroupBy(i => i.ServiceChannelVersionedId).ToDictionary(i => i.Key, i => i.ToList());
                resultFromDb.Data.ForEach(c =>
                {
                    c.ServiceChannelNames = names.TryGet(c.Id);
                });

                moreAvailable = resultFromDb.MoreAvailable;
                result = TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmChannelListItem>(resultFromDb.Data);
            });

            return new VmChannelSearchResultResult()
            {
                Channels = result,
                ChannelType = vm.ChannelType,
                PageNumber = ++vm.PageNumber,
                MoreAvailable = moreAvailable,
                Count = count
            };
        }

        public IVmChannelSearchResultBase ConnectingChannelSearchResult(VmServiceStep4 vm)
        {
            IReadOnlyList<IVmChannelListItem> result = new List<VmChannelListItem>();
            var maxNumberOfItems = CoreConstants.MaximumNumberOfAllItems;
            int numberOfAllItems = 0;

            contextManager.ExecuteReader(unitOfWork =>
            {
                var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                var resultTemp = channelRep.All();

                if (!string.IsNullOrEmpty(vm.ChannelName))
                {
                    var searchText = vm.ChannelName.ToLower();
                    resultTemp = resultTemp.Where(x => x.ServiceChannelNames.Any(y => y.Name.ToLower().Contains(searchText)));
                }

                if (vm.OrganizationId.IsAssigned())
                {
                    var organizationIdGuid = vm.OrganizationId.Value;
                    resultTemp = resultTemp.Where(x => x.OrganizationId == organizationIdGuid);
                }

                if (!vm.SelectedChannelTypes.IsNullOrEmpty())
                {
                    var selectedChannelTypes = vm.SelectedChannelTypes;
                    resultTemp = resultTemp.Where(x => selectedChannelTypes.Any(y => y == x.TypeId));
                }

                numberOfAllItems = resultTemp.Count();

                resultTemp = resultTemp
                    .OrderBy(i => i.ServiceChannelNames.FirstOrDefault(x => x.Type.Code == NameTypeEnum.Name.ToString()).Name)
                    .Take(maxNumberOfItems);

                resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                    q.Include(i => i.ServiceChannelNames).ThenInclude(i => i.Type))
                     .Include(i => i.Type);

                result = TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmChannelListItem>(resultTemp);
            });

            return new VmChannelSearchResultBase() { Channels = result, IsMoreThanMax = (numberOfAllItems > maxNumberOfItems), NumberOfAllItems = numberOfAllItems };
        }

        private void SetupChannelDescription(IUnitOfWork unitOfWork, IVmChannelDescription model, bool isNew)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            if (isNew)
            {
                model.OrganizationId = utilities.GetUserMainOrganization();
            }
        }

        private VmPhone SetupPhoneTypes(IUnitOfWork unitOfWork, Guid? entityId, string numberTypeCode = null)
        {
            if (entityId.IsAssigned())
            {
                var serviceChannelPhoneRep = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();
                var phones = serviceChannelPhoneRep.All().Where(x => x.ServiceChannelVersionedId == entityId).Include(x=>x.Phone);
                var phone = string.IsNullOrEmpty(numberTypeCode) ? phones.FirstOrDefault() : phones.FirstOrDefault(x => x.Phone.Type.Code == numberTypeCode);
                return phone != null ? new VmPhone { ChargeType = phone.Phone.ChargeTypeId, TypeId = phone.Phone.TypeId, Id = Guid.NewGuid() } : null;
            }
            return null;
        }

        private void UpdatePhoneTypes(IUnitOfWork unitOfWork, VmPhone model, Guid? entityId, string numberTypeCode = null)
        {
            if (entityId.IsAssigned())
            {
                var serviceChannelPhoneRep = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();
                var phones = serviceChannelPhoneRep.All().Where(x => x.ServiceChannelVersionedId == entityId && x.PhoneId == model.Id).Include(x=>x.Phone);
                if (!string.IsNullOrEmpty(numberTypeCode))
                {
                    phones = phones.Where(x => x.Phone.Type.Code == numberTypeCode).Include(x=>x.Phone);
                }
                phones.ForEach(x =>
                {
                    x.Phone.ChargeTypeId = model.ChargeType.IsAssigned() ? model.ChargeType : x.Phone.ChargeTypeId;
                    x.Phone.TypeId = model.TypeId.IsAssigned() ? model.TypeId : x.Phone.TypeId;
                });
            }
        }

        #region Electronic channel

        public IVmElectronicChannelStep1 GetElectronicChannelStep1(IVmEntityGet model)
        {
            VmElectronicChannelStep1 result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                ServiceChannelVersioned entity = null;
                result = GetModel<ServiceChannelVersioned, VmElectronicChannelStep1>(entity = GetEntity<ServiceChannelVersioned>(model.Id, unitOfWork,
                    q => q.Include(x => x.ServiceChannelNames)
                        .Include(x => x.ServiceChannelDescriptions)
                        .Include(x => x.LanguageAvailabilities)
                        .Include(x => x.PublishingStatus)
                        .Include(x => x.AreaMunicipalities)
                        .Include(x => x.Areas).ThenInclude(x => x.Area)
                        .Include(x => x.Attachments).ThenInclude(x => x.Attachment)
                        .Include(x => x.ElectronicChannels).ThenInclude(x => x.LocalizedUrls)
                        .Include(i => i.ServiceChannelServiceHours).ThenInclude(i => i.ServiceHours).ThenInclude(i => i.DailyOpeningTimes)
                        .Include(i => i.ServiceChannelServiceHours).ThenInclude(i => i.ServiceHours).ThenInclude(i => i.ServiceHourType)
                        .Include(i => i.ServiceChannelServiceHours).ThenInclude(i => i.ServiceHours).ThenInclude(i => i.AdditionalInformations)
                        .Include(x => x.Phones).ThenInclude(x => x.Phone).ThenInclude(x => x.Type)
                        .Include(x => x.Phones).ThenInclude(x => x.Phone).ThenInclude(x => x.ChargeType)
                        .Include(x => x.Phones).ThenInclude(x => x.Phone).ThenInclude(x => x.PrefixNumber).ThenInclude(x => x.Country).ThenInclude(x => x.CountryNames)
                        .Include(x => x.Phones).ThenInclude(x => x.Phone)
                        .Include(x => x.Emails).ThenInclude(x => x.Email)
                        .Include(x => x.ConnectionType)
                ), unitOfWork);
                var publishingStatuses = commonService.GetPublishingStatuses();
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("AvailableOrganizations", commonService.GetUserAvailableOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("PublishingStatuses", publishingStatuses),
                    () => GetEnumEntityCollectionModel("Municipalities", commonService.GetMunicipalities(unitOfWork)),
                    () => GetEnumEntityCollectionModel("AreaInformationTypes", commonService.GetAreaInformationTypes()),
                    () => GetEnumEntityCollectionModel("AreaTypes", commonService.GetAreaTypes()),
                    () => GetEnumEntityCollectionModel("BusinessRegions", commonService.GetAreas(unitOfWork, AreaTypeEnum.BusinessRegions)),
                    () => GetEnumEntityCollectionModel("HospitalRegions", commonService.GetAreas(unitOfWork, AreaTypeEnum.HospitalRegions)),
                    () => GetEnumEntityCollectionModel("Provinces", commonService.GetAreas(unitOfWork, AreaTypeEnum.Province)),
                    () => GetEnumEntityCollectionModel("ServiceChannelConnectionTypes", commonService.GetServiceChannelConnectionTypes()),
                    () => GetEnumEntityCollectionModel("DialCodes", commonService.GetDefaultDialCode(unitOfWork)), //DefaultPrefixNumber
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages())
               );

                UpdateConnectionTypeCheck(unitOfWork, result, entity);
                SetupChannelDescription(unitOfWork, result, !model.Id.IsAssigned());
            });
            return result;
        }

        public IVmEntityBase AddElectronicChannel(VmElectronicChannel model)
        {
            var result = new ServiceChannelVersioned();
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = AddElectronicChannel(unitOfWork, model);
            });
            return new VmEntityRootStatusBase() { Id = result.Id, UnificRootId = result.UnificRootId, PublishingStatusId = commonService.GetDraftStatusId()};
        }

        public IVmElectronicChannelStep1 SaveElectronicChannelStep1(VmElectronicChannelStep1 model)
        {
            Guid? channelId = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                Guid languageId = languageCache.Get(SetTranslatorLanguage(model).ToString());
                channelLogic.PrefilterViewModel(model);
                var electronicChannelRep = unitOfWork.CreateRepository<IElectronicChannelRepository>();
                model.ElectronicChannelId = electronicChannelRep.All().Where(i => i.ServiceChannelVersionedId == model.Id).Select(i => i.Id).FirstOrDefault();
                var serviceChannel = TranslateElectronicChannel(unitOfWork, model);

                //Removing attachments
                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                    serviceChannel.Attachments.Select(x => x.Attachment).ToList(),
                    curr => curr.ServiceChannelAttachments.Any(x => x.ServiceChannelVersionedId == serviceChannel.Id) && curr.LocalizationId == languageId);

                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                 serviceChannel.Phones.Select(p => p.Phone).ToList(),
                 curr => curr.ServiceChannelPhones.Any(x => x.ServiceChannelVersionedId == serviceChannel.Id) && curr.LocalizationId == languageId);

                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                    serviceChannel.Emails.Select(e => e.Email).ToList(),
                    curr => curr.ServiceChannelEmails.Any(x => x.ServiceChannelVersionedId == serviceChannel.Id) && curr.LocalizationId == languageId);

                model.PhoneNumbers.ForEach(phoneNumber => UpdatePhoneTypes(unitOfWork, phoneNumber, serviceChannel.Id));

                UpdateChannelAreaInformation(unitOfWork, serviceChannel);

                unitOfWork.Save(parentEntity: serviceChannel);
                channelId = serviceChannel.Id;
            });
            return GetElectronicChannelStep1(new VmChannelBasic { Id = channelId, Language = model.Language});
        }

        public IVmOpeningHours SaveOpeningHoursStep(VmOpeningHoursStep model)
        {
            Guid? channelId = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                channelLogic.PrefilterViewModel(model);
                var serviceChannel = TranslationManagerToEntity.Translate<VmOpeningHoursStep, ServiceChannelVersioned>(model, unitOfWork);
                if (serviceChannel.Id == model.Id)
                {
                    // TODO!
                    //dataUtils.UpdateCollectionWithRemove(unitOfWork, serviceChannel.ServiceChannelServiceHours,
                    //    query => query.ServiceChannelVersionedId == serviceChannel.Id);
                }
                // Remove daily opening hours
                var openingTimeRep = unitOfWork.CreateRepository<IDailyOpeningTimeRepository>();

                var serviceHours = serviceChannel.ServiceChannelServiceHours?.Count > 0 ? serviceChannel.ServiceChannelServiceHours.Select(i => i.ServiceHours).ToList() : new List<ServiceHours>();
                foreach (var serviceHour in serviceHours)
                {
                    var openingTimes = serviceHour.DailyOpeningTimes.ToList();
                    var openingTimesToRemove = openingTimeRep.All().Where(x => x.OpeningHourId == serviceHour.Id && !openingTimes.Contains(x));
                    openingTimesToRemove.ForEach(x => openingTimeRep.Remove(x));
                }

                unitOfWork.Save(parentEntity: serviceChannel);
                channelId = serviceChannel.Id;
            });
            return GetOpeningHoursStep(new VmChannelBasic { Id = channelId, Language = model.Language });
        }

        private ServiceChannelVersioned AddElectronicChannel(IUnitOfWorkWritable unitOfWork, VmElectronicChannel vm)
        {
            EnsureOrganization(vm.Step1Form.OrganizationId);
            SetTranslatorLanguage(vm);
            channelLogic.PrefilterViewModel(vm);
            vm.PublishingStatusId = commonService.GetDraftStatusId();
            var electronicChannelRep = unitOfWork.CreateRepository<IElectronicChannelRepository>();
            vm.Step1Form.ElectronicChannelId = electronicChannelRep.All().Where(i => i.ServiceChannelVersionedId == vm.Id).Select(i => i.Id).FirstOrDefault();
            var serviceChannel = TranslateElectronicChannel(unitOfWork, vm);
            unitOfWork.Save();
            return serviceChannel;
        }

        private ServiceChannelVersioned TranslateElectronicChannel<TElectronicChannelModel>(IUnitOfWorkWritable unitOfWork, TElectronicChannelModel vm) where TElectronicChannelModel : class
        {
            var serviceChannel = TranslationManagerToEntity.Translate<TElectronicChannelModel, ServiceChannelVersioned>(vm, unitOfWork);
            // the channel should be stored, even the webpages is not exist
            //CheckElectronicChannelValidity(serviceChannel);
            return serviceChannel;
        }

        private void CheckElectronicChannelValidity(ServiceChannelVersioned channelVersioned)
        {
            var channelUrl = channelVersioned.ElectronicChannels?.FirstOrDefault()?.LocalizedUrls?.FirstOrDefault();
            if (channelUrl != null)
            {
                var resultUrl = urlService.CheckUrl(new VmUrlChecker() { UrlAddress = channelUrl.Url });
                // the channel should be stored, even the webpage is not exist
                //if (resultUrl.UrlExists.HasValue && !resultUrl.UrlExists.Value)
                //{
                //    throw new ArgumentException("", string.Format(invalidElectronicChannelUrl, channelUrl.Url));
                //}
            }
            if (channelVersioned.Attachments?.Any() == true)
            {
                foreach (var attachment in channelVersioned.Attachments)
                {
                    if (!string.IsNullOrEmpty(attachment.Attachment.Url))
                    {
                        var resultUrl = urlService.CheckUrl(new VmUrlChecker() { UrlAddress = attachment.Attachment.Url });
                        // the channel should be stored, even the webpage is not exist
                        //if (resultUrl.UrlExists.HasValue && !resultUrl.UrlExists.Value)
                        //{
                        //    throw new ArgumentException("", string.Format(invalidElectronicChannelAttachmentUrl, attachment.Attachment.Url) );
                        //}
                    }
                }
            }
        }

        #endregion Electronic channel

        #region Location channel

        public IVmLocationChannelStep1 GetLocationChannelStep1(IVmEntityGet model)
        {
            var result = new VmLocationChannelStep1();
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                ServiceChannelVersioned entity = null;
                result = GetModel<ServiceChannelVersioned, VmLocationChannelStep1>(entity = GetEntity<ServiceChannelVersioned>(model.Id, unitOfWork,
                    q => q
                        .Include(x => x.ServiceChannelNames)
                        .Include(x => x.LanguageAvailabilities)
                        .Include(x => x.ServiceChannelDescriptions)
                        .Include(x => x.PublishingStatus)
                        .Include(x => x.AreaMunicipalities)
                        .Include(x => x.Areas).ThenInclude(x => x.Area)
                        .Include(x => x.Languages).ThenInclude(x => x.Language).ThenInclude(x => x.Names)
                        .Include(x => x.WebPages).ThenInclude(x => x.WebPage)
                        .Include(x => x.WebPages).ThenInclude(x => x.Type)
                        .Include(x => x.Phones).ThenInclude(x => x.Phone).ThenInclude(x => x.Type)
                        .Include(x => x.Phones).ThenInclude(x => x.Phone).ThenInclude(x => x.ChargeType)
                        .Include(x => x.Phones).ThenInclude(x => x.Phone).ThenInclude(x => x.PrefixNumber).ThenInclude(x => x.Country).ThenInclude(x => x.CountryNames)
                        .Include(x => x.Emails).ThenInclude(x => x.Email)
                        .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Character)
                        .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                        .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                        .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                        .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                        .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                        .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressAdditionalInformations)
                        .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(x => x.Coordinates)
                        .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(x => x.Country).ThenInclude(x => x.CountryNames)
                        .Include(x => x.ConnectionType)
                ), unitOfWork);

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("AvailableOrganizations", commonService.GetUserAvailableOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("PublishingStatuses", commonService.GetPublishingStatuses()),
                    () => GetEnumEntityCollectionModel("Municipalities", commonService.GetMunicipalities(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages()),
                    () => GetEnumEntityCollectionModel("WebPageTypes", commonService.GetWebPageTypes()),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("AreaInformationTypes", commonService.GetAreaInformationTypes()),
                    () => GetEnumEntityCollectionModel("AreaTypes", commonService.GetAreaTypes()),
                    () => GetEnumEntityCollectionModel("BusinessRegions", commonService.GetAreas(unitOfWork, AreaTypeEnum.BusinessRegions)),
                    () => GetEnumEntityCollectionModel("HospitalRegions", commonService.GetAreas(unitOfWork, AreaTypeEnum.HospitalRegions)),
                    () => GetEnumEntityCollectionModel("Provinces", commonService.GetAreas(unitOfWork, AreaTypeEnum.Province)),
                    () => GetEnumEntityCollectionModel("ServiceChannelConnectionTypes", commonService.GetServiceChannelConnectionTypes()),
                    () => GetEnumEntityCollectionModel("DialCodes", commonService.GetDefaultDialCode(unitOfWork)) //DefaultPrefixNumber
                );

                UpdateConnectionTypeCheck(unitOfWork, result, entity);
                SetupChannelDescription(unitOfWork, result, !model.Id.IsAssigned());

                result.PhoneNumbers = result.PhoneNumbers ?? new List<VmPhone> { SetupPhoneTypes(unitOfWork, model.Id, PhoneNumberTypeEnum.Phone.ToString())};
                result.FaxNumbers = result.FaxNumbers ?? new List<VmPhone> { SetupPhoneTypes(unitOfWork, model.Id, PhoneNumberTypeEnum.Fax.ToString()) };

            });
            return result;
        }

        public IVmOpeningHours GetOpeningHoursStep(IVmEntityGet model)
        {
            var result = new VmOpeningHoursStep();
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                result = GetModel<ServiceChannelVersioned, VmOpeningHoursStep>(GetEntity<ServiceChannelVersioned>(model.Id, unitOfWork,
                    q => q.Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours).ThenInclude(j => j.DailyOpeningTimes)
                        .Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours).ThenInclude(j => j.AdditionalInformations)), unitOfWork);

            });
            return result;
        }

        public IVmEntityBase AddLocationChannel(VmLocationChannel model)
        {
            var result = new ServiceChannelVersioned();
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = AddLocationChannel(unitOfWork, model);
                unitOfWork.Save();
            });
            var addresses = result.ServiceLocationChannels?.FirstOrDefault()?.Addresses?.Select(x => x.AddressId);
            addressService.UpdateAddress(addresses.ToList());
            return new VmEntityRootStatusBase { Id = result.Id, UnificRootId = result.UnificRootId, PublishingStatusId = commonService.GetDraftStatusId() };
        }

        private ServiceChannelVersioned AddLocationChannel(IUnitOfWorkWritable unitOfWork, VmLocationChannel vm)
        {
            EnsureOrganization(vm.Step1Form.OrganizationId);
            SetTranslatorLanguage(vm);
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            vm.PublishingStatusId = commonService.GetDraftStatusId();
            channelLogic.PrefilterViewModel(vm);
            var serviceChannel = TranslationManagerToEntity.Translate<VmLocationChannel, ServiceChannelVersioned>(vm, unitOfWork);
            serviceChannelRep.Add(serviceChannel);
            return serviceChannel;
        }

        public IVmLocationChannelStep1 SaveLocationChannelStep1(VmLocationChannelStep1 model)
        {
            Guid? channelId = null;
            ServiceChannelVersioned serviceChannel = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                Guid languageId = languageCache.Get(SetTranslatorLanguage(model).ToString());
                channelLogic.PrefilterViewModel(model);
                if (model.Id.HasValue)
                {
                    ownerReferenceLogic.SetOwnerReference(model.PostalAddresses.Concat(model.VisitingAddresses), model.Id.Value);
                }

                serviceChannel = TranslationManagerToEntity.Translate<VmLocationChannelStep1, ServiceChannelVersioned>(model, unitOfWork);

                serviceChannel.Languages = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannel.Languages,
                    query => query.ServiceChannelVersionedId == serviceChannel.Id,
                    lang => lang.LanguageId);

                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                    serviceChannel.WebPages.Select(x => x.WebPage).ToList(),
                    curr => curr.ServiceChannelWebPages.Any(x => x.ServiceChannelVersionedId == serviceChannel.Id) && curr.LocalizationId == languageId);

                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                    serviceChannel.Phones.Select(p => p.Phone).ToList(),
                    curr => curr.ServiceChannelPhones.Any(x => x.ServiceChannelVersionedId == serviceChannel.Id) && curr.LocalizationId == languageId);

                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                    serviceChannel.Emails.Select(e => e.Email).ToList(),
                    curr => curr.ServiceChannelEmails.Any(x => x.ServiceChannelVersionedId == serviceChannel.Id) && curr.LocalizationId == languageId);

                var addressRepository = unitOfWork.CreateRepository<IAddressRepository>();
                var channelAddressRepository = unitOfWork.CreateRepository<IServiceLocationChannelAddressRepository>();
                var wpIds = serviceChannel.ServiceLocationChannels?.FirstOrDefault()?.Addresses?.Select(x => x.Address.Id).ToList();

                var existingAddresses = channelAddressRepository.All().Where(address => address.ServiceLocationChannel.ServiceChannelVersionedId == serviceChannel.Id).Select(x => x.Address).ToList();
                if (wpIds != null)
                {
                    addressRepository.Remove(existingAddresses.Where(x => !wpIds.Contains(x.Id)));
                }

                UpdateChannelAreaInformation(unitOfWork, serviceChannel);

                unitOfWork.Save(parentEntity: serviceChannel);
                channelId = serviceChannel.Id;

            });
            var addresses = serviceChannel.ServiceLocationChannels?.FirstOrDefault()?.Addresses?.Select(x => x.AddressId);
            addressService.UpdateAddress(addresses.ToList());
            return GetLocationChannelStep1(new VmChannelBasic { Id = channelId, Language = model.Language });
        }
        //public IVmLocationChannelStep2 SaveLocationChannelStep2(VmLocationChannelStep2 model)
        //{
        //    Guid? channelId = null;
        //    contextManager.ExecuteWriter(unitOfWork =>
        //    {
        //        Guid languageId = languageCache.Get(SetTranslatorLanguage(model).ToString());
        //        channelLogic.PrefilterViewModel(model);
        //        var serviceChannel = TranslationManagerToEntity.Translate<VmLocationChannelStep2, ServiceChannel>(model, unitOfWork);


        //        //Removing web pages
        //        dataUtils.UpdateCollectionWithRemove(unitOfWork,
        //            serviceChannel.WebPages.Select(x => x.WebPage).ToList(),
        //            curr => curr.ServiceChannelWebPages.Any(x => x.ServiceChannelId == model.Id) && curr.LocalizationId == languageId);


        //        UpdatePhoneTypes(unitOfWork, model.PhoneNumber, model.Id, PhoneNumberTypeEnum.Phone.ToString());
        //        unitOfWork.Save(parentEntity: serviceChannel);
        //        channelId = serviceChannel.Id;

        //    });
        //    return GetLocationChannelStep2(new VmChannelBasic { EntityId = channelId, Language = model.Language });
        //}
        //public IVmLocationChannelStep3 SaveLocationChannelStep3(VmLocationChannelStep3 model)
        //{
        //    Guid? channelId = null;
        //    ServiceChannel serviceChannel = null;
        //    contextManager.ExecuteWriter(unitOfWork =>
        //    {
        //        SetTranslatorLanguage(model);
        //        channelLogic.PrefilterViewModel(model);
        //        if (model.Id.HasValue)
        //        {
        //            ownerReferenceLogic.SetOwnerReference(model.PostalAddresses.Concat(model.VisitingAddresses), model.Id.Value);
        //        }
        //        serviceChannel = TranslationManagerToEntity.Translate<VmLocationChannelStep3, ServiceChannel>(model, unitOfWork);

        //        var addressRepository = unitOfWork.CreateRepository<IAddressRepository>();
        //        var channelAddressRepository = unitOfWork.CreateRepository<IServiceLocationChannelAddressRepository>();
        //        var wpIds = serviceChannel.ServiceLocationChannels?.FirstOrDefault()?.Addresses?.Select(x => x.Address.Id).ToList();

        //        var existingAddresses = channelAddressRepository.All().Where(address => address.ServiceLocationChannel.ServiceChannelId == model.Id).Select(x => x.Address).ToList();
        //        if (wpIds != null)
        //        {
        //            addressRepository.Remove(existingAddresses.Where(x => !wpIds.Contains(x.Id)));
        //        }
        //        unitOfWork.Save(parentEntity: serviceChannel);
        //        channelId = serviceChannel.Id;

        //    });
        //    var addresses = serviceChannel.ServiceLocationChannels?.FirstOrDefault()?.Addresses?.Select(x => x.AddressId);
        //    addressService.UpdateAddress(addresses.ToList());
        //    return GetLocationChannelStep3(new VmChannelBasic { EntityId = channelId, Language = model.Language });
        //}

        #endregion Location channel

        #region PhoneChannel

        public IVmPhoneChannelStep1 GetPhoneChannelStep1(IVmEntityGet model)
        {
            var result = new VmPhoneChannelStep1();

            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                ServiceChannelVersioned entity = null;
                result = GetModel<ServiceChannelVersioned, VmPhoneChannelStep1>(entity = GetEntity<ServiceChannelVersioned>(model.Id, unitOfWork,
                    q => q.Include(j => j.Attachments).ThenInclude(j => j.Attachment)
                        .Include(j => j.PublishingStatus)
                        .Include(x => x.LanguageAvailabilities)
                        .Include(j => j.ServiceChannelDescriptions)
                        .Include(j => j.ServiceChannelNames)
                        .Include(j => j.AreaMunicipalities)
                        .Include(j => j.Areas).ThenInclude(x => x.Area)
                        .Include(j => j.Languages).ThenInclude(j => j.Language).ThenInclude(j => j.Names)
                        .Include(j => j.Organization).ThenInclude(j => j.Versions).ThenInclude(j => j.OrganizationNames).ThenInclude(k => k.Type)
                        .Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours).ThenInclude(j => j.AdditionalInformations)
                        .Include(j => j.WebPages).ThenInclude(j => j.WebPage)
                        .Include(j => j.Emails).ThenInclude(j => j.Email)
                        .Include(j => j.Phones).ThenInclude(j => j.Phone).ThenInclude(x => x.PrefixNumber).ThenInclude(x => x.Country).ThenInclude(x => x.CountryNames)
                        .Include(j => j.ConnectionType)
                ), unitOfWork);

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("AvailableOrganizations", commonService.GetUserAvailableOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages()),
                    () => GetEnumEntityCollectionModel("PhoneNumberTypes", commonService.GetPhoneTypes()),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("Municipalities", commonService.GetMunicipalities(unitOfWork)),
                    () => GetEnumEntityCollectionModel("AreaInformationTypes", commonService.GetAreaInformationTypes()),
                    () => GetEnumEntityCollectionModel("AreaTypes", commonService.GetAreaTypes()),
                    () => GetEnumEntityCollectionModel("BusinessRegions", commonService.GetAreas(unitOfWork, AreaTypeEnum.BusinessRegions)),
                    () => GetEnumEntityCollectionModel("HospitalRegions", commonService.GetAreas(unitOfWork, AreaTypeEnum.HospitalRegions)),
                    () => GetEnumEntityCollectionModel("Provinces", commonService.GetAreas(unitOfWork, AreaTypeEnum.Province)),
                    () => GetEnumEntityCollectionModel("ServiceChannelConnectionTypes", commonService.GetServiceChannelConnectionTypes()),
                    () => GetEnumEntityCollectionModel("DialCodes", commonService.GetDefaultDialCode(unitOfWork)) //DefaultPrefixNumber
                    );
                UpdateConnectionTypeCheck(unitOfWork, result, entity);
                SetupChannelDescription(unitOfWork, result, !model.Id.IsAssigned());
            });
            return result;
        }

        public IVmEntityBase AddPhoneChannel(VmPhoneChannel model)
        {
            ServiceChannelVersioned result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = AddPhoneChannel(model, unitOfWork);
                unitOfWork.Save();
            });
            return new VmEntityRootStatusBase { Id = result.Id, UnificRootId = result.UnificRootId, PublishingStatusId = commonService.GetDraftStatusId() };
        }

        private ServiceChannelVersioned AddPhoneChannel(VmPhoneChannel vm, IUnitOfWorkWritable unitOfWork)
        {
            EnsureOrganization(vm.Step1Form.OrganizationId);
            SetTranslatorLanguage(vm);
            vm.PublishingStatus = PublishingStatus.Draft;
            channelLogic.PrefilterViewModel(vm);
            return TranslationManagerToEntity.Translate<VmPhoneChannel, ServiceChannelVersioned>(vm, unitOfWork);
        }

        public IVmPhoneChannelStep1 SavePhoneChannelStep1(VmPhoneChannelStep1 model)
        {
            Guid? channelId = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                Guid languageId = languageCache.Get(SetTranslatorLanguage(model).ToString());
                var serviceChannel = TranslationManagerToEntity.Translate<VmPhoneChannelStep1, ServiceChannelVersioned>(model, unitOfWork);

                serviceChannel.Languages = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannel.Languages,
                    query => query.ServiceChannelVersionedId == serviceChannel.Id,
                    lang => lang.LanguageId);

                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                    serviceChannel.Phones.Select(p => p.Phone).ToList(),
                    curr => curr.ServiceChannelPhones.Any(x => x.ServiceChannelVersionedId == serviceChannel.Id) && curr.LocalizationId == languageId);

                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                    serviceChannel.Emails.Select(e => e.Email).ToList(),
                    curr => curr.ServiceChannelEmails.Any(x => x.ServiceChannelVersionedId == serviceChannel.Id) && curr.LocalizationId == languageId);

                UpdateChannelAreaInformation(unitOfWork, serviceChannel);

                model.PhoneNumbers.ForEach(phoneNumber => UpdatePhoneTypes(unitOfWork, phoneNumber, model.Id));
                unitOfWork.Save(parentEntity: serviceChannel);
                channelId = serviceChannel.Id;
            });
            return GetPhoneChannelStep1(new VmChannelBasic { Id = channelId, Language = model.Language });
        }

        #endregion PhoneChannel

        #region WebPage channel

        public IVmWebPageChannelStep1 GetWebPageChannelStep1(IVmEntityGet model)
        {
            var result = new VmWebPageChannelStep1();
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                ServiceChannelVersioned entity = null;
                result = GetModel<ServiceChannelVersioned, VmWebPageChannelStep1>(entity = GetEntity<ServiceChannelVersioned>(model.Id, unitOfWork,
                    q => q.Include(j => j.Attachments).ThenInclude(j => j.Attachment)
                        .Include(x => x.PublishingStatus)
                        .Include(x => x.LanguageAvailabilities)
                        .Include(j => j.ServiceChannelDescriptions)
                        .Include(j => j.ServiceChannelNames)
                        .Include(j => j.WebpageChannels).ThenInclude(j => j.LocalizedUrls)
                        .Include(j => j.Keywords).ThenInclude(j => j.Keyword)
                        .Include(j => j.Languages).ThenInclude(j => j.Language).ThenInclude(j => j.Names)
                        .Include(x => x.Emails).ThenInclude(x => x.Email)
                        .Include(x => x.Phones).ThenInclude(x => x.Phone).ThenInclude(x => x.PrefixNumber).ThenInclude(x => x.Country).ThenInclude(x => x.CountryNames)
                        .Include(j => j.OntologyTerms).ThenInclude(j => j.OntologyTerm)
                        .Include(j => j.Organization).ThenInclude(j => j.Versions).ThenInclude(j => j.OrganizationNames).ThenInclude(k => k.Type)
                        .Include(j => j.ServiceClasses).ThenInclude(j => j.ServiceClass).ThenInclude(k => k.Names)
                        .Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours).ThenInclude(j => j.AdditionalInformations)
                        .Include(j => j.WebPages).ThenInclude(j => j.WebPage)
                        .Include(j => j.WebPages).ThenInclude(j => j.Type)
                        .Include(j => j.ConnectionType)
                ), unitOfWork);

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages()),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("AvailableOrganizations", commonService.GetUserAvailableOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ServiceChannelConnectionTypes", commonService.GetServiceChannelConnectionTypes()),
                    () => GetEnumEntityCollectionModel("DialCodes", commonService.GetDefaultDialCode(unitOfWork)) //DefaultPrefixNumber

                );

                UpdateConnectionTypeCheck(unitOfWork, result, entity);
                SetupChannelDescription(unitOfWork, result, !model.Id.IsAssigned());
             });
            return result;
        }

        public IVmEntityBase AddWebPageChannel(VmWebPageChannel model)
        {
            ServiceChannelVersioned result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = AddWebPageChannel(unitOfWork, model);
                unitOfWork.Save();

            });
            return new VmEntityRootStatusBase() { Id = result.Id, UnificRootId = result.UnificRootId, PublishingStatusId = commonService.GetDraftStatusId() };
        }

        public IVmWebPageChannelStep1 SaveWebPageChannelStep1(VmWebPageChannelStep1 model)
        {
            Guid? channelId = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                Guid languageId = languageCache.Get(SetTranslatorLanguage(model).ToString());

                var webpageChannelRep = unitOfWork.CreateRepository<IWebpageChannelRepository>();
                model.WebPageChannelId = webpageChannelRep.All().Where(i => i.ServiceChannelVersionedId == model.Id).Select(i => i.Id).FirstOrDefault();

                var serviceChannel = TranslationManagerToEntity.Translate<VmWebPageChannelStep1, ServiceChannelVersioned>(model, unitOfWork);

                serviceChannel.Languages = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannel.Languages,
                query => query.ServiceChannelVersionedId == serviceChannel.Id,
                lang => lang.LanguageId);

                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                  serviceChannel.Phones.Select(p => p.Phone).ToList(),
                  curr => curr.ServiceChannelPhones.Any(x => x.ServiceChannelVersionedId == serviceChannel.Id) && curr.LocalizationId == languageId);

                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                    serviceChannel.Emails.Select(e => e.Email).ToList(),
                    curr => curr.ServiceChannelEmails.Any(x => x.ServiceChannelVersionedId == serviceChannel.Id) && curr.LocalizationId == languageId);

                model.PhoneNumbers.ForEach(phoneNumber => UpdatePhoneTypes(unitOfWork, phoneNumber, model.Id));
                unitOfWork.Save(parentEntity: serviceChannel);
                channelId = serviceChannel.Id;

            });
            return GetWebPageChannelStep1(new VmChannelBasic { Id = channelId, Language = model.Language });
        }

        private ServiceChannelVersioned AddWebPageChannel(IUnitOfWorkWritable unitOfWork, VmWebPageChannel vm)
        {
            EnsureOrganization(vm.Step1Form.OrganizationId);
            SetTranslatorLanguage(vm);
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            vm.PublishingStatusId = commonService.GetDraftStatusId();
            var serviceChannel = TranslationManagerToEntity.Translate<VmWebPageChannel, ServiceChannelVersioned>(vm, unitOfWork);
            CheckWebPageChannelValidity(serviceChannel);
            serviceChannelRep.Add(serviceChannel);
            return serviceChannel;
        }

        private void CheckWebPageChannelValidity(ServiceChannelVersioned channelVersioned)
        {
            var channelUrl = channelVersioned.WebpageChannels?.FirstOrDefault()?.LocalizedUrls.FirstOrDefault();
            if (channelUrl != null)
            {
                var resultUrl = urlService.CheckUrl(new VmUrlChecker() { UrlAddress = channelUrl.Url });

                // the channel should be stored, even the webpage is not exist
                //if (resultUrl.UrlExists.HasValue && !resultUrl.UrlExists.Value)
                //{
                //    //throw new ArgumentException("", string.Format(invalidElectronicChannelUrl, channelUrl.Url));
                //}
            }
        }
        #endregion WebPage channel

        #region Printable channel
        public IVmPrintableFormChannelStep1 GetPrintableFormChannelStep1(IVmEntityGet model)
        {
            var result = new VmPrintableFormChannelStep1();
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                ServiceChannelVersioned entity = null;
                result = GetModel<ServiceChannelVersioned, VmPrintableFormChannelStep1>(entity = GetEntity<ServiceChannelVersioned>(model.Id, unitOfWork,
                    q => q.Include(x => x.ServiceChannelNames)
                        .Include(x => x.ServiceChannelDescriptions)
                        .Include(x => x.LanguageAvailabilities)
                        .Include(x => x.Emails).ThenInclude(x => x.Email)
                        .Include(x => x.Phones).ThenInclude(x => x.Phone).ThenInclude(x => x.PrefixNumber).ThenInclude(x => x.Country).ThenInclude(x => x.CountryNames)
                        .Include(x => x.PublishingStatus)
                        .Include(x => x.AreaMunicipalities)
                        .Include(x => x.Areas).ThenInclude(x => x.Area)
                        .Include(x => x.PrintableFormChannels).ThenInclude(x => x.DeliveryAddress).ThenInclude(i => i.AddressStreets).ThenInclude(x => x.StreetNames)
                        .Include(x => x.PrintableFormChannels).ThenInclude(x => x.DeliveryAddress).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(x => x.PostOfficeBoxNames)
                        .Include(x => x.PrintableFormChannels).ThenInclude(x => x.DeliveryAddress).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(x => x.PostalCode)
                        .Include(x => x.PrintableFormChannels).ThenInclude(x => x.DeliveryAddress).ThenInclude(i => i.AddressForeigns).ThenInclude(x => x.ForeignTextNames)
                        .Include(x => x.PrintableFormChannels).ThenInclude(x => x.DeliveryAddress).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(x => x.PostOfficeBoxNames)
                        .Include(x => x.PrintableFormChannels).ThenInclude(x => x.DeliveryAddress).ThenInclude(i => i.AddressStreets).ThenInclude(x => x.PostalCode)
                        .Include(x => x.Attachments).ThenInclude(x => x.Attachment)
                        .Include(x => x.PrintableFormChannels).ThenInclude(x => x.ChannelUrls).ThenInclude(x => x.Type)
                        .Include(x => x.PrintableFormChannels).ThenInclude(x => x.DeliveryAddress).ThenInclude(x => x.AddressAdditionalInformations)
                        .Include(x => x.PrintableFormChannels).ThenInclude(x => x.DeliveryAddress).ThenInclude(x => x.Coordinates)
                        .Include(x => x.PrintableFormChannels).ThenInclude(x => x.FormIdentifiers)
                        .Include(x => x.PrintableFormChannels).ThenInclude(x => x.FormReceivers)
                        .Include(x => x.ConnectionType)
                ), unitOfWork);

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("PrintableFormUrlTypes", commonService.GetPrintableFormUrlTypes()),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("AvailableOrganizations", commonService.GetUserAvailableOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Municipalities", commonService.GetMunicipalities(unitOfWork)),
                    () => GetEnumEntityCollectionModel("AreaInformationTypes", commonService.GetAreaInformationTypes()),
                    () => GetEnumEntityCollectionModel("AreaTypes", commonService.GetAreaTypes()),
                    () => GetEnumEntityCollectionModel("BusinessRegions", commonService.GetAreas(unitOfWork, AreaTypeEnum.BusinessRegions)),
                    () => GetEnumEntityCollectionModel("HospitalRegions", commonService.GetAreas(unitOfWork, AreaTypeEnum.HospitalRegions)),
                    () => GetEnumEntityCollectionModel("Provinces", commonService.GetAreas(unitOfWork, AreaTypeEnum.Province)),
                    () => GetEnumEntityCollectionModel("ServiceChannelConnectionTypes", commonService.GetServiceChannelConnectionTypes()),
                    () => GetEnumEntityCollectionModel("DialCodes", commonService.GetDefaultDialCode(unitOfWork)), //DefaultPrefixNumber
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages())
                );

                UpdateConnectionTypeCheck(unitOfWork, result, entity);
                SetupChannelDescription(unitOfWork, result, !model.Id.IsAssigned());
            });
            return result;
        }

        public IVmEntityBase AddPrintableFormChannel(VmPrintableFormChannel model)
        {
            ServiceChannelVersioned result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = AddPrintableFormChannel(unitOfWork, model);
                unitOfWork.Save();

            });

            addressService.UpdateAddress(result?.PrintableFormChannels.Where(x => x.DeliveryAddressId.HasValue).Select(x => x.DeliveryAddressId.Value).ToList());
            return new VmEntityRootStatusBase { Id = result.Id, UnificRootId = result.UnificRootId, PublishingStatusId = commonService.GetDraftStatusId() };
        }

        private ServiceChannelVersioned AddPrintableFormChannel(IUnitOfWorkWritable unitOfWork, VmPrintableFormChannel vm)
        {
            EnsureOrganization(vm.Step1Form.OrganizationId);
            SetTranslatorLanguage(vm);
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            vm.PublishingStatusId = commonService.GetDraftStatusId();
            //channelLogic.PrefilterViewModel(vm);  TODO Change
            var serviceChannel = TranslationManagerToEntity.Translate<VmPrintableFormChannel, ServiceChannelVersioned>(vm, unitOfWork);
            //CheckWebPageChannelValidity(serviceChannel); TODO Change
            serviceChannelRep.Add(serviceChannel);
            return serviceChannel;
        }

        public IVmPrintableFormChannelStep1 SavePrintableFormChannelStep1(VmPrintableFormChannelStep1 model)
        {
            Guid? channelId = null;
            ServiceChannelVersioned serviceChannelVersioned = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                Guid languageId = languageCache.Get(SetTranslatorLanguage(model).ToString());
                serviceChannelVersioned = TranslationManagerToEntity.Translate<VmPrintableFormChannelStep1, ServiceChannelVersioned>(model, unitOfWork);

                var webPageRep = unitOfWork.CreateRepository<IPrintableFormChannelUrlRepository>();
                var wpIds = serviceChannelVersioned.PrintableFormChannels.SelectMany(x => x.ChannelUrls).Select(x => x.Id).ToList();

                var existingWebPages = webPageRep.All().Where(wp => wp.PrintableFormChannel.ServiceChannelVersionedId == serviceChannelVersioned.Id && wp.LocalizationId == languageId).ToList();
                webPageRep.Remove(existingWebPages.Where(x => !wpIds.Contains(x.Id)));

                //Removing attachments
                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                    serviceChannelVersioned.Attachments?.Select(x => x.Attachment)?.ToList(),
                    curr => curr.ServiceChannelAttachments.Any(x => x.ServiceChannelVersionedId == serviceChannelVersioned.Id) && curr.LocalizationId == languageId);

                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                 serviceChannelVersioned.Phones.Select(p => p.Phone).ToList(),
                 curr => curr.ServiceChannelPhones.Any(x => x.ServiceChannelVersionedId == serviceChannelVersioned.Id) && curr.LocalizationId == languageId);

                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                    serviceChannelVersioned.Emails.Select(e => e.Email).ToList(),
                    curr => curr.ServiceChannelEmails.Any(x => x.ServiceChannelVersionedId == serviceChannelVersioned.Id) && curr.LocalizationId == languageId);

                model.PhoneNumbers.ForEach(phoneNumber => UpdatePhoneTypes(unitOfWork, phoneNumber, serviceChannelVersioned.Id));

                UpdateChannelAreaInformation(unitOfWork, serviceChannelVersioned);

                unitOfWork.Save(parentEntity: serviceChannelVersioned);

                channelId = serviceChannelVersioned.Id;
            });

//            addressService.UpdateAddress(serviceChannelVersioned?.PrintableFormChannels.Where(x => x.DeliveryAddressId.HasValue).Select( x => x.DeliveryAddressId.Value).ToList());
            return GetPrintableFormChannelStep1(new VmChannelBasic { Id = channelId, Language = model.Language });
        }

        #endregion Printable channel

        private void UpdateChannelAreaInformation(IUnitOfWorkWritable unitOfWork, ServiceChannelVersioned serviceChannel)
        {
            serviceChannel.AreaMunicipalities = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannel.AreaMunicipalities,
                    query => query.ServiceChannelVersionedId == serviceChannel.Id,
                    areaMunicipality => areaMunicipality.MunicipalityId);

            serviceChannel.Areas = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannel.Areas,
                query => query.ServiceChannelVersionedId == serviceChannel.Id,
                area => area.AreaId);
        }

        #region OpenApi

        public IVmOpenApiGuidPageVersionBase GetServiceChannels(DateTime? date, int pageNumber, int pageSize, bool archived = false, bool active = false)
        {
            var vm = new V3VmOpenApiGuidPage(pageNumber, pageSize);

            if (pageNumber <= 0) return vm;

            List<ServiceChannelVersioned> channels = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                if (archived)
                {
                    channels = GetArchivedEntities<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(vm, date, unitOfWork, q => q.Include(i => i.ServiceChannelNames));
                }                
                else if(active)
                {
                    channels = GetActiveEntities<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(vm, date, unitOfWork, q => q.Include(i => i.ServiceChannelNames));
                }
                else
                {
                    channels = GetPublishedEntities<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(vm, date, unitOfWork, q => q.Include(i => i.ServiceChannelNames));
                }                
            });

            return GetGuidPage(channels, vm);
        }

        public IVmOpenApiGuidPageVersionBase GetServiceChannelsByMunicipality(Guid municipalityId, DateTime? date, int pageNumber, int pageSize)
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

                channels = GetPublishedEntities<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(vm, date, unitOfWork, q => q.Include(i => i.ServiceChannelNames), filters);
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

        public IList<IVmOpenApiServiceChannel> GetServiceChannelsByType(ServiceChannelTypeEnum type, DateTime? date, int openApiVersion)
        {
            IList<IVmOpenApiServiceChannel> result = new List<IVmOpenApiServiceChannel>();
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var typeId = typesCache.Get<ServiceChannelType>(type.ToString());
                    var channelRepo = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                    var query = channelRepo.All().Where(c => c.TypeId.Equals(typeId)).Where(PublishedFilter<ServiceChannelVersioned>()).Where(ValidityFilter<ServiceChannelVersioned>());
                    if (date.HasValue)
                    {
                        query = query.Where(c => c.Modified > date.Value);
                    }
                    var guidList = query.Select(o => o.Id).ToList();

                    result = GetServiceChannelsWithDetails(unitOfWork, guidList, openApiVersion);
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

        public IVmOpenApiGuidPageVersionBase GetServiceChannelsByType(ServiceChannelTypeEnum type, DateTime? date, int pageNumber, int pageSize, bool getOnlyPublished = true)
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
                    (vm, date, unitOfWork, q => q.Include(i => i.ServiceChannelNames), new List<Expression<Func<ServiceChannelVersioned, bool>>>() { c => c.TypeId.Equals(typeId) });
                }
                else
                {
                    channels = GetPublishedEntities<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>
                    (vm, date, unitOfWork, q => q.Include(i => i.ServiceChannelNames), new List<Expression<Func<ServiceChannelVersioned, bool>>>() { c => c.TypeId.Equals(typeId) });
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
                    var channelRepo = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                    var query = channelRepo.All().Where(serviceChannel => serviceChannel.OrganizationId.Equals(organizationId)).Where(PublishedFilter<ServiceChannelVersioned>()).Where(ValidityFilter<ServiceChannelVersioned>());
                    if (date.HasValue)
                    {
                        query = query.Where(serviceChannel => serviceChannel.Modified > date.Value);
                    }
                    if (type.HasValue)
                    {
                        var typeId = typesCache.Get<ServiceChannelType>(type.Value.ToString());
                        // Filter by service channel type
                        query = query.Where(serviceChannel => serviceChannel.TypeId == typeId);
                    }
                    var guidList = query.Select(o => o.Id).ToList();

                    result = GetServiceChannelsWithDetails(unitOfWork, guidList, openApiVersion);
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
        public IVmOpenApiGuidPageVersionBase GetServiceChannelsByOrganization(Guid organizationId, DateTime? date, int pageNumber, int pageSize, ServiceChannelTypeEnum? type = null)
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
                    channels = GetPublishedEntities<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>
                        (vm, date, unitOfWork, q => q.Include(i => i.ServiceChannelNames), additionalFilters);
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

        public PublishingStatus? GetChannelStatusByRootId(Guid id)
        {
            PublishingStatus? result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = versioningManager.GetLatestVersionPublishingStatus<ServiceChannelVersioned>(unitOfWork, id);
            });

            return result;
        }

        public PublishingStatus? GetChannelStatusBySourceId(string sourceId)
        {
            PublishingStatus? result = null;
            bool externalSourceExists = false;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var id = GetPTVId<ServiceChannel>(sourceId, utilities.GetRelationIdForExternalSource(), unitOfWork);
                if (id != Guid.Empty)
                {
                    externalSourceExists = true;
                    result = versioningManager.GetLatestVersionPublishingStatus<ServiceChannelVersioned>(unitOfWork, id);
                }
            });
            if (!externalSourceExists) { throw new Exception(string.Format(CoreMessages.OpenApi.ExternalSourceNotExists, sourceId)); }
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

            // Find only published services for a service channel - let's do this outside of translator
            // Manually map connection data.
            var connectionRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var connectionQuery = connectionRep.All().Where(c => c.ServiceChannelId == channel.UnificRootId && c.Service.Versions.Any(v => v.PublishingStatusId == publishedId && v.LanguageAvailabilities.Any(l => l.StatusId == publishedId)));
            var connections = unitOfWork.ApplyIncludes(connectionQuery, GetConnectionIncludeChain()).ToList();

            ////Measure
            //watch.Stop();
            //logger.LogTrace($"*** Get connections from db: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// End measure

            if (connections?.Count > 0)
            {
                result.Services = new List<VmOpenApiServiceChannelService>();

                // Fill with service names
                var serviceRootIds = connections.Select(s => s.ServiceId).ToList();

                var services = unitOfWork.ApplyIncludes(
                    unitOfWork.CreateRepository<IServiceVersionedRepository>().All().Where(i => serviceRootIds.Contains(i.UnificRootId) && i.PublishingStatusId == publishedId),
                    q => q.Include(i => i.UnificRoot).Include(i => i.ServiceNames)).ToList();
                connections.ForEach(c =>
                {
                    string name = null;
                    var service = services.Where(i => i.UnificRootId == c.ServiceId).FirstOrDefault();
                    if (service != null)
                    {
                        name = GetNameWithFallback(service.UnificRoot.Versions.FirstOrDefault()?.ServiceNames, typesCache, languageCache);
                    }
                    VmOpenApiServiceChannelService vm = new VmOpenApiServiceChannelService { Service = new VmOpenApiItem { Id = c.ServiceId, Name = name } };

                    // map base connection data
                    MapConnection(c, vm, typesCache, languageCache);

                    // contactdetails
                    vm.ContactDetails = GetContactDetails(c, typesCache, languageCache);

                    // digitalAuthorizations
                    vm.DigitalAuthorizations = GetDigitalAuthorizations(c);

                    result.Services.Add(vm);
                });
            }
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
                .Include(i => i.ServiceChannelDescriptions)
                .Include(i => i.Type)
                .Include(i => i.WebPages).ThenInclude(i => i.WebPage)
                .Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours).ThenInclude(j => j.DailyOpeningTimes)
                .Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours).ThenInclude(j => j.AdditionalInformations)
                .Include(i => i.Attachments).ThenInclude(i => i.Attachment).ThenInclude(i => i.Type)
                .Include(i => i.ElectronicChannels).ThenInclude(i => i.LocalizedUrls)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)

                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)

                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)

                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
                .Include(i => i.Languages).ThenInclude(i => i.Language)
                .Include(i => i.WebpageChannels).ThenInclude(i => i.LocalizedUrls)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.FormReceivers)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.FormIdentifiers)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressAdditionalInformations)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.Coordinates)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.ChannelUrls)
                .Include(j => j.Emails).ThenInclude(j => j.Email)
                .Include(j => j.Phones).ThenInclude(j => j.Phone).ThenInclude(i => i.PrefixNumber)
                .Include(i => i.LanguageAvailabilities)
                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaNames)
                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.UnificRoot);                
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

                    // Service location channel
                    channel.ServiceLocationChannels.ForEach(c =>
                    {
                        c.Addresses.ForEach(address =>
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
                        });
                    });

                    // Web page channel
                    channel.WebpageChannels.ForEach(c =>
                    {
                        c.LocalizedUrls = c.LocalizedUrls.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    });

                    // Printable form channel
                    channel.PrintableFormChannels.ForEach(c =>
                    {
                        c.FormReceivers = c.FormReceivers.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        c.FormIdentifiers = c.FormIdentifiers.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        if (c.DeliveryAddress != null)
                        {
                            c.DeliveryAddress.AddressStreets.ForEach(j =>
                            {
                                j.StreetNames = j.StreetNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                            });
                            c.DeliveryAddress.AddressPostOfficeBoxes.ForEach(j =>
                            {
                                j.PostOfficeBoxNames = j.PostOfficeBoxNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                            });
                            c.DeliveryAddress.AddressForeigns.ForEach(j =>
                            {
                                j.ForeignTextNames = j.ForeignTextNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                            });
                            c.DeliveryAddress.AddressAdditionalInformations = c.DeliveryAddress.AddressAdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        }
                        c.ChannelUrls = c.ChannelUrls.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
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

        private IList<IVmOpenApiServiceChannel> GetServiceChannelsWithDetails(IUnitOfWork unitOfWork, List<Guid> versionIdList, int openApiVersion, bool getOnlyPublished = true)
        {
            if (versionIdList.Count == 0) return new List<IVmOpenApiServiceChannel>();

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
                .Include(i => i.ElectronicChannels).ThenInclude(i => i.LocalizedUrls)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)

                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)

                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)

                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
                .Include(i => i.Languages).ThenInclude(i => i.Language)
                .Include(i => i.WebpageChannels).ThenInclude(i => i.LocalizedUrls)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.FormReceivers)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.FormIdentifiers)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressAdditionalInformations)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.Coordinates)
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

            // Set the right version for service channels
            var versionList = new List<IVmOpenApiServiceChannel>();
            resultList.ForEach(channel =>
            {
                versionList.Add(GetServiceChannelByOpenApiVersion(unitOfWork, channel, openApiVersion));
            });

            return versionList;
        }

        private VmOpenApiServiceChannel GetServiceChannelWithSimpleDetails(IUnitOfWork unitOfWork, Guid versionId)
        {
            if (!versionId.IsAssigned()) return null;

            ServiceChannelVersioned entity = null;
            return GetModel<ServiceChannelVersioned, VmOpenApiServiceChannel>(entity = GetEntity<ServiceChannelVersioned>(versionId, unitOfWork,
                    q => q.Include(x => x.LanguageAvailabilities)
                    .Include(i => i.ElectronicChannels)
                    .Include(i => i.ServiceLocationChannels)
                    .Include(i => i.WebpageChannels)
                    .Include(i => i.PrintableFormChannels)), unitOfWork);
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
                    var list = new List<VmOpenApiConnection>();
                    vm.ServiceChannelServices.ForEach(serviceId =>
                    {
                        list.Add(new VmOpenApiConnection { Id = serviceId, IsServiceId = true });
                    });

                    serviceChannel = TranslationManagerToEntity.Translate<TVmChannelIn, ServiceChannelVersioned>(vm, unitOfWork);
                    serviceChannel.UnificRoot.ServiceServiceChannels = TranslationManagerToEntity.TranslateAll<VmOpenApiConnection, ServiceServiceChannel>(list, unitOfWork).ToList();

                    var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                    serviceChannelRep.Add(serviceChannel);

                    // Create the mapping between external source id and PTV id
                    if (!string.IsNullOrEmpty(vm.SourceId))
                    {
                        SetExternalSource(serviceChannel.UnificRoot, vm.SourceId, userId, unitOfWork);
                    }

                    unitOfWork.Save(saveMode, userName: userName);
                }
            });

            if (useOtherEndPoint)
            {
                throw new ExternalSourceExistsException(string.Format(CoreMessages.OpenApi.ExternalSourceExists, vm.SourceId));
            }

            // Update the map coordinates for addresses
            if (serviceChannel?.ServiceLocationChannels?.FirstOrDefault() != null)
            {
                if (serviceChannel.ServiceLocationChannels.FirstOrDefault().Addresses ?.Count > 0)
                {
                    // only for visiting addresses which are of type street or moving
                    var visitingAddressId = typesCache.Get<AddressCharacter>(AddressCharacterEnum.Visiting.ToString());
                    var streetId = typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString());
                    var movingId = typesCache.Get<AddressType>(AddressTypeEnum.Moving.ToString());
                    var addresses = serviceChannel.ServiceLocationChannels.FirstOrDefault().Addresses
                        .Where(a => a.CharacterId == visitingAddressId && (a.Address.TypeId == streetId || a.Address.TypeId == movingId))
                        .Select(x => x.AddressId);
                    addressService.UpdateAddress(addresses.ToList());
                }

            }
            else if (serviceChannel?.PrintableFormChannels?.FirstOrDefault() != null && serviceChannel.PrintableFormChannels.FirstOrDefault().DeliveryAddressId.HasValue)
            {
                addressService.UpdateAddress(serviceChannel.PrintableFormChannels.Where(x => x.DeliveryAddressId.HasValue).Select(x => x.DeliveryAddressId.Value).ToList());
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
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = userName ?? utilities.GetRelationIdForExternalSource();
            var serviceChannel = new ServiceChannelVersioned();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Get the root id according to source id (if defined)
                var rootId = vm.Id ?? GetPTVId<ServiceChannel>(vm.SourceId, userId, unitOfWork);

                // Get right version id
                vm.Id = versioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, rootId, null, false);

                if (vm.PublishingStatus == PublishingStatus.Deleted.ToString())
                {
                    serviceChannel = DeleteChannel(unitOfWork, vm.Id);
                }
                else
                {
                    // Entity needs to be restored?
                    if (vm.CurrentPublishingStatus == PublishingStatus.Deleted.ToString())
                    {
                        if (vm.PublishingStatus == PublishingStatus.Modified.ToString() || vm.PublishingStatus == PublishingStatus.Published.ToString())
                        {
                            // We need to restore already archived item
                            var publishingResult = commonService.RestoreArchivedEntity<ServiceChannelVersioned>(unitOfWork, vm.Id.Value);
                        }
                    }

                    serviceChannel = TranslationManagerToEntity.Translate<TVmChannelIn, ServiceChannelVersioned>(vm, unitOfWork);

                    if (vm.CurrentPublishingStatus == PublishingStatus.Draft.ToString())
                    {
                        // We need to manully remove items from collections if the item is in draft state.
                        if (vm.Areas.Count > 0)
                        {
                            var municipalities = vm.Areas.Where(a => a.Type == AreaTypeEnum.Municipality.ToString()).ToList();
                            var otherAreas = vm.Areas.Where(a => a.Type != AreaTypeEnum.Municipality.ToString()).ToList();
                            if (municipalities.Count > 0)
                            {
                                serviceChannel.AreaMunicipalities = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannel.AreaMunicipalities,
                                query => query.ServiceChannelVersionedId == serviceChannel.Id, area => area.MunicipalityId);
                                if (otherAreas.Count == 0)
                                {
                                    // Remove all possible old areas
                                    dataUtils.RemoveItemCollection<ServiceChannelArea>(unitOfWork, s => s.ServiceChannelVersionedId == serviceChannel.Id);
                                }
                            }
                            if (otherAreas.Count > 0)
                            {
                                serviceChannel.Areas = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannel.Areas,
                                    query => query.ServiceChannelVersionedId == serviceChannel.Id, area => area.AreaId);
                                if (municipalities.Count == 0)
                                {
                                    // Remove all possible old municipalities
                                    dataUtils.RemoveItemCollection<ServiceChannelAreaMunicipality>(unitOfWork, s => s.ServiceChannelVersionedId == serviceChannel.Id);
                                }
                            }
                        }
                        else if (!vm.AreaType.IsNullOrEmpty() && vm.AreaType != AreaInformationTypeEnum.AreaType.ToString())
                        {
                            // We need to remove possible old areas and municipalities
                            dataUtils.RemoveItemCollection<ServiceChannelArea>(unitOfWork, s => s.ServiceChannelVersionedId == serviceChannel.Id);
                            dataUtils.RemoveItemCollection<ServiceChannelAreaMunicipality>(unitOfWork, s => s.ServiceChannelVersionedId == serviceChannel.Id);
                        }

                        if (vm.Languages != null && vm.Languages.Count > 0)
                        {
                            serviceChannel.Languages = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannel.Languages,
                                    query => query.ServiceChannelVersionedId == serviceChannel.Id, language => language.LanguageId);
                        }
                        if (vm.DeleteAllWebPages || (vm.WebPages != null && vm.WebPages.Count > 0))
                        {
                            serviceChannel.WebPages = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannel.WebPages,
                            query => query.ServiceChannelVersionedId == serviceChannel.Id, webPage => webPage.WebPage != null ? webPage.WebPage.Id : webPage.WebPageId);
                        }
                        // For published versions the translator and AddCollection method ( with cloned version ) will handle removing of items from collection.
                        if (vm.DeleteAllServiceHours || (vm.ServiceHours != null && vm.ServiceHours.Count > 0))
                        {
                            // Remove the ones that does not exist in viewmodel list
                            var updatedIds = serviceChannel.ServiceChannelServiceHours?.Count > 0 ? serviceChannel.ServiceChannelServiceHours.Select(i => i.ServiceHoursId).ToList() : new List<Guid>();
                            var repository = unitOfWork.CreateRepository<IServiceChannelServiceHoursRepository>();
                            var currentEntities = unitOfWork.ApplyIncludes(
                                repository.AllPure().Where(i => i.ServiceChannelVersionedId == serviceChannel.Id),
                                q => q.Include(i => i.ServiceHours).ThenInclude(i => i.DailyOpeningTimes)
                                .Include(i => i.ServiceHours).ThenInclude(i => i.AdditionalInformations)).ToList();

                            var toRemove = currentEntities.Where(x => !updatedIds.Contains(x.ServiceHoursId)).ToList();
                            if (toRemove?.Count > 0)
                            {
                                var timeRepo = unitOfWork.CreateRepository<IDailyOpeningTimeRepository>();
                                var infoRepo = unitOfWork.CreateRepository<IServiceHoursAdditionalInformationRepository>();
                                toRemove.ForEach(hour =>
                                {
                                    // Delete also related items from service hours
                                    hour.ServiceHours.DailyOpeningTimes.ForEach(d => timeRepo.Remove(d));
                                    hour.ServiceHours.AdditionalInformations.ForEach(a => infoRepo.Remove(a));
                                    repository.Remove(hour);
                                });
                            }
                        }
                        if (vm.DeleteAllSupportEmails || (vm.SupportEmails != null && vm.SupportEmails.Count > 0))
                        {
                            // Remove the ones that does not exist in viewmodel list
                            var repository = unitOfWork.CreateRepository<IServiceChannelEmailRepository>();
                            var currentEmails = unitOfWork.ApplyIncludes(
                                repository.All().Where(p => p.ServiceChannelVersionedId == serviceChannel.Id),
                                p => p.Include(i => i.Email).Include(l => l.Email.Localization)).ToList();

                            var existingLanguages = vm.SupportEmails.Select(e => e.Language).Distinct().ToList();
                            var emailsByLanguageToDelete = currentEmails.Where(e => !existingLanguages.Contains(e.Email.Localization.Code)).ToList();
                            emailsByLanguageToDelete.ForEach(e => repository.Remove(e));

                            // Remove from email
                            var emailRepository = unitOfWork.CreateRepository<IEmailRepository>();
                            emailsByLanguageToDelete.ForEach(e => emailRepository.Remove(e.Email));
                        }
                        if (vm.DeleteAllSupportPhones || (vm.SupportPhones != null && vm.SupportPhones.Count > 0))
                        {
                            // Remove the phone numbers that does not exist in viewmodel list
                            var repository = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();
                            var currentPhones = unitOfWork.ApplyIncludes(
                                repository.All().Where(p => p.ServiceChannelVersionedId == serviceChannel.Id),
                                p => p.Include(i => i.Phone).Include(l => l.Phone.Localization)).ToList();

                            var existingLanguages = vm.SupportPhones.Select(p => p.Language).Distinct().ToList();
                            var phonesByLanguageToDelete = currentPhones.Where(p => !existingLanguages.Contains(p.Phone.Localization.Code)).ToList();
                            phonesByLanguageToDelete.ForEach(p => repository.Remove(p));

                            // Remove from phone
                            var phoneRepository = unitOfWork.CreateRepository<IPhoneRepository>();
                            phonesByLanguageToDelete.ForEach(p => phoneRepository.Remove(p.Phone));
                        }

                        if (vm is VmOpenApiPhoneChannelInVersionBase)
                        {
                            SetCollections(vm as VmOpenApiPhoneChannelInVersionBase, serviceChannel, unitOfWork);
                        }
                        else if (vm is VmOpenApiWebPageChannelInVersionBase)
                        {
                            SetCollections(vm as VmOpenApiWebPageChannelInVersionBase, serviceChannel, unitOfWork);
                        }
                        else if (vm is VmOpenApiPrintableFormChannelInVersionBase)
                        {
                            SetCollections(vm as VmOpenApiPrintableFormChannelInVersionBase, serviceChannel, unitOfWork);
                        }
                        else if (vm is VmOpenApiElectronicChannelInVersionBase)
                        {
                            SetCollections(vm as VmOpenApiElectronicChannelInVersionBase, serviceChannel, unitOfWork);
                        }
                        else if (vm is VmOpenApiServiceLocationChannelInVersionBase)
                        {
                            SetCollections(vm as VmOpenApiServiceLocationChannelInVersionBase, serviceChannel, unitOfWork);
                        }
                    }

                    // Update the mapping between external source id and PTV id
                    if (!string.IsNullOrEmpty(vm.SourceId))
                    {
                        UpdateExternalSource<ServiceChannel>(serviceChannel.UnificRootId, vm.SourceId, userId, unitOfWork);
                    }
                }

                unitOfWork.Save(saveMode, serviceChannel, userName);
            });

            // Update the map coordinates for addresses
            if (vm.PublishingStatus != PublishingStatus.Deleted.ToString())
            {
                if (serviceChannel?.ServiceLocationChannels?.FirstOrDefault() != null)
                {
                    var addresses = serviceChannel.ServiceLocationChannels.FirstOrDefault().Addresses?.Select(x => x.AddressId);
                    addressService.UpdateAddress(addresses.ToList());
                }
                else if (serviceChannel?.PrintableFormChannels?.FirstOrDefault() != null && serviceChannel.PrintableFormChannels.FirstOrDefault().DeliveryAddressId.HasValue)
                {
                    addressService.UpdateAddress(serviceChannel.PrintableFormChannels.Where(x => x.DeliveryAddressId.HasValue).Select(x => x.DeliveryAddressId.Value).ToList());
                }
            }

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = commonService.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannel.Id, i => i.ServiceChannelVersionedId == serviceChannel.Id);
            }

            return GetServiceChannelWithDetails(serviceChannel.Id, openApiVersion, false);
        }

        private void SetCollections(VmOpenApiPhoneChannelInVersionBase vmPhoneChannel, ServiceChannelVersioned serviceChannelVersioned, IUnitOfWorkWritable unitOfWork)
        {
            if (vmPhoneChannel.PhoneNumbers != null && vmPhoneChannel.PhoneNumbers.Count > 0)
            {
                var repository = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();
                var currentPhones = unitOfWork.ApplyIncludes(
                    repository.All().Where(p => p.ServiceChannelVersionedId == serviceChannelVersioned.Id),
                    p => p.Include(i => i.Phone).Include(cht => cht.Phone.ChargeType).Include(l => l.Phone.Localization)).ToList();

                var existingLanguages = vmPhoneChannel.PhoneNumbers.Select(p => p.Language).Distinct().ToList();
                var phonesByLanguageToDelete = currentPhones.Where(p => !existingLanguages.Contains(p.Phone.Localization.Code)).ToList();
                phonesByLanguageToDelete.ForEach(p => repository.Remove(p));
            }
        }

        private void SetCollections(VmOpenApiWebPageChannelInVersionBase vmWebPageChannel, ServiceChannelVersioned serviceChannelVersioned, IUnitOfWorkWritable unitOfWork)
        {
            var webPageChannel = serviceChannelVersioned.WebpageChannels.FirstOrDefault();
            if (webPageChannel == null)
            {
                return;
            }

            if (vmWebPageChannel.DeleteAllWebPages || (vmWebPageChannel.Urls != null && vmWebPageChannel.Urls.Count > 0))
            {
                var rep = unitOfWork.CreateRepository<IWebpageChannelUrlRepository>();
                var currentUrls = unitOfWork.ApplyIncludes(rep.All().Where(u => u.WebpageChannelId == webPageChannel.Id), q =>
                    q.Include(i => i.Localization)).ToList();
                var existingLanguages = vmWebPageChannel.Urls != null ? vmWebPageChannel.Urls.Select(u => u.Language).ToList() : new List<string>();
                // Delete items that were not in vm.
                currentUrls.Where(u => !existingLanguages.Contains(u.Localization.Code)).ForEach(u => rep.Remove(u));
            }
        }

        private void SetCollections(VmOpenApiPrintableFormChannelInVersionBase vmPrintableFormChannel, ServiceChannelVersioned serviceChannelVersioned, IUnitOfWorkWritable unitOfWork)
        {
            var printableFormChannel = serviceChannelVersioned.PrintableFormChannels.FirstOrDefault();
            if (printableFormChannel == null)
            {
                return;
            }

            // Urls
            if (vmPrintableFormChannel.DeleteAllChannelUrls || (vmPrintableFormChannel.ChannelUrls != null && vmPrintableFormChannel.ChannelUrls.Count > 0))
            {
                printableFormChannel.ChannelUrls = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, printableFormChannel.ChannelUrls,
                    query => query.PrintableFormChannelId == printableFormChannel.Id, url => url.Id);
            }

            // Delivery address
            if (vmPrintableFormChannel.DeleteDeliveryAddress && vmPrintableFormChannel.DeliveryAddress == null)
            {
                var rep = unitOfWork.CreateRepository<IPrintableFormChannelRepository>();
                var channel = unitOfWork.ApplyIncludes(rep.All().Where(c => c.Id == printableFormChannel.Id), q =>
                    q.Include(i => i.DeliveryAddress)).FirstOrDefault();
                if (channel.DeliveryAddress != null)
                {
                    var addressRep = unitOfWork.CreateRepository<IAddressRepository>();
                    var address = addressRep.All().Where(i => i.Id == channel.DeliveryAddress.Id).FirstOrDefault();
                    if (address != null && channel.DeliveryAddress != null)
                    {
                        addressRep.Remove(address);
                    }
                }
            }
            if (vmPrintableFormChannel.DeleteAllAttachments || (vmPrintableFormChannel.Attachments?.Count > 0))
            {
                serviceChannelVersioned.Attachments = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannelVersioned.Attachments,
                    query => query.ServiceChannelVersionedId == serviceChannelVersioned.Id, attachment => attachment.Attachment != null ? attachment.Attachment.Id : attachment.AttachmentId);
            }

            // Form identifiers
            if (vmPrintableFormChannel.DeleteAllFormIdentifiers || (vmPrintableFormChannel.FormIdentifier?.Count > 0))
            {
                printableFormChannel.FormIdentifiers = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, printableFormChannel.FormIdentifiers,
                    query => query.PrintableFormChannelId == printableFormChannel.Id, form => form.LocalizationId);
            }

            // Form receivers
            if (vmPrintableFormChannel.DeleteAllFormReceivers || (vmPrintableFormChannel.FormReceiver?.Count > 0))
            {
                printableFormChannel.FormReceivers = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, printableFormChannel.FormReceivers,
                    query => query.PrintableFormChannelId == printableFormChannel.Id, receiver => receiver.LocalizationId);
            }

        }

        private void SetCollections(VmOpenApiElectronicChannelInVersionBase vmEChannel, ServiceChannelVersioned serviceChannelVersioned, IUnitOfWorkWritable unitOfWork)
        {
            var eChannel = serviceChannelVersioned.ElectronicChannels.FirstOrDefault();
            if (eChannel == null)
            {
                return;
            }
            if (vmEChannel.DeleteAllWebPages || (vmEChannel.Urls != null && vmEChannel.Urls.Count > 0))
            {
                var rep = unitOfWork.CreateRepository<IElectronicChannelUrlRepository>();
                var currentUrls = unitOfWork.ApplyIncludes(rep.All().Where(u => u.ElectronicChannelId == eChannel.Id), q =>
                    q.Include(i => i.Localization)).ToList();
                var existingLanguages = vmEChannel.Urls != null ? vmEChannel.Urls.Select(u => u.Language).ToList() : new List<string>();
                // Delete items that were not in vm.
                currentUrls.Where(u => !existingLanguages.Contains(u.Localization.Code)).ForEach(u => rep.Remove(u));
            }
            // We need to manully remove items from attachment collection if the item is in draft state.
            // For published versions the translator and AddCollection method ( with cloned version ) will handle removing of items from collection.
            if ((vmEChannel.DeleteAllAttachments || (vmEChannel.Attachments != null && vmEChannel.Attachments.Count > 0)) && vmEChannel.CurrentPublishingStatus == PublishingStatus.Draft.ToString())
            {
                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                    serviceChannelVersioned.Attachments.Select(x => x.Attachment).ToList(),
                    curr => curr.ServiceChannelAttachments.Any(x => x.ServiceChannelVersionedId == serviceChannelVersioned.Id));
            }
        }

        private void SetCollections(VmOpenApiServiceLocationChannelInVersionBase vmLocationChannel, ServiceChannelVersioned serviceChannelVersioned, IUnitOfWorkWritable unitOfWork)
        {
            var locationChannel = serviceChannelVersioned.ServiceLocationChannels.FirstOrDefault();
            if (locationChannel == null)
            {
                return;
            }

            if (vmLocationChannel.DeleteAllFaxNumbers || vmLocationChannel.FaxNumbers?.Count > 0)
            {
                RemoveServiceChannelPhoneNumbers(serviceChannelVersioned, unitOfWork, typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString()));
            }

            if (vmLocationChannel.DeleteAllPhoneNumbers || vmLocationChannel.PhoneNumbers?.Count > 0)
            {
                RemoveServiceChannelPhoneNumbers(serviceChannelVersioned, unitOfWork, typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString()));
            }

            if (vmLocationChannel.Addresses?.Count > 0)
            {
                locationChannel.Addresses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, locationChannel.Addresses,
                    query => query.ServiceLocationChannelId == locationChannel.Id, address => address.Address != null ? address.Address.Id : address.AddressId);
            }
        }

        private void RemoveServiceChannelPhoneNumbers(ServiceChannelVersioned serviceChannelVersioned, IUnitOfWorkWritable unitOfWork, Guid typeId)
        {
            var updatedNumbers = serviceChannelVersioned.Phones.Select(f => f.PhoneId).ToList();
            var rep = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();
            var repPhone = unitOfWork.CreateRepository<IPhoneRepository>();
            var currentNumbers = unitOfWork.ApplyIncludes(rep.All().Where(f => f.ServiceChannelVersionedId == serviceChannelVersioned.Id && f.Phone.TypeId == typeId), q =>
                q.Include(i => i.Phone)).ToList();
            // Delete items that were in db but not in updated ones
            currentNumbers.Where(f => !updatedNumbers.Contains(f.PhoneId)).ForEach(f => repPhone.Remove(f.Phone));
        }

        #endregion

        private List<Guid> PrefilterServiceChannelRootIds(List<VmServiceChannelRelation> serviceChannelRelations)
        {
            var connectedChannelsRootIds = new List<List<Guid>>();
            foreach (var service in serviceChannelRelations)
            {
                connectedChannelsRootIds.Add(service.ChannelRelations.Where(x => !x.IsNew)
                                                                 .Select(y => y.ConnectedChannel.RootId)
                                                                 .ToList());
            }
            return connectedChannelsRootIds.Any() ? connectedChannelsRootIds.Cast<IEnumerable<Guid>>().Aggregate((x, y) => x.Intersect(y)).ToList() : new List<Guid>();
        }

        public IVmEntityBase SwitchChannelLanguageAvailability(Guid channelId, Guid languageId, Guid publishingStatusId)
        {
            ServiceChannelVersioned channel = null;
            List<ServiceChannelLanguageAvailability> langaugeAvailabilities = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var channelVersionedRepository = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                channel = channelVersionedRepository.All().Include(i => i.LanguageAvailabilities).Single(x => x.Id == channelId);
                var languageAvailabilityInfo = channel.LanguageAvailabilities.FirstOrDefault(i => i.LanguageId == languageId) ??
                                               channel.LanguageAvailabilities.AddAndReturn(new ServiceChannelLanguageAvailability() { LanguageId = languageId });
                languageAvailabilityInfo.StatusId = publishingStatusId;
                langaugeAvailabilities = channel.LanguageAvailabilities.ToList();
                unitOfWork.Save();
            });
            return new VmEntityLanguageAvailable()
            {
                Id = channel.Id,
                PublishingStatusId = channel.PublishingStatusId,
                LanguagesAvailability = langaugeAvailabilities.ToDictionary(i => i.LanguageId, i => i.StatusId)
            };
        }

        public IVmEntityBase GetChannelLanguagesAvailabilities(Guid channelId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceLangAvailRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
                var langaugeAvailabilities = serviceLangAvailRep.All().Where(x => x.ServiceChannelVersionedId == channelId).ToList();
                return new VmEntityLanguageAvailable() { Id = channelId, LanguagesAvailability = langaugeAvailabilities.ToDictionary(i => i.LanguageId, i => i.StatusId) };
            });
        }

        public VmPublishingResultModel PublishChannel(VmPublishingModel model)
        {
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                Guid channelId = model.Id;
                var affected = commonService.PublishEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, model);
                var result = new VmServiceChannelPublish
                {
                    Id = channelId,
                    PublishingStatusId = affected.AffectedEntities.First(i => i.Id == channelId).PublishingStatusNew,
                    LanguagesAvailabilities = model.LanguagesAvailabilities,
                    Version = affected.Version
                };
                var channel = HandleConnectionTypeWhenPublished(unitOfWork, model.Id);
                UpdateConnectionTypeCheck(unitOfWork, result, channel);
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Services", affected.AffectedEntities.Select(i => new VmEntityStatusBase() { Id = i.Id, PublishingStatusId = i.PublishingStatusNew }).ToList<IVmBase>())
                );
                return result;
            });
        }

        private ServiceChannelVersioned HandleConnectionTypeWhenPublished(IUnitOfWorkWritable unitOfWork, Guid currentChannelVersionedId)
        {
            var psNotCommon = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.NotCommon.ToString());
            var serviceServiceChannelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var serviceChannelVersionedRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var channel = serviceChannelVersionedRep.All().SingleOrDefault(i => i.Id == currentChannelVersionedId && i.ConnectionTypeId == psNotCommon);
            if (channel == null)
            {
                return null;
            }
            serviceServiceChannelRep.Remove(GetConnectionsToRemoveForPublished(unitOfWork, channel, serviceServiceChannelRep));
            unitOfWork.Save();
            return channel;
        }

        private IList<ServiceServiceChannel> GetConnectionsToRemoveForPublished(IUnitOfWork unitOfWork, ServiceChannelVersioned channel, IServiceServiceChannelRepository serviceServiceChannelRep)
        {

            if (channel == null)
            {
                return new List<ServiceServiceChannel>();
            }

            var serviceServiceChannels = serviceServiceChannelRep.All().Where(ch => ch.ServiceChannelId == channel.UnificRootId);
            var psPublishedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var serviceIds = serviceServiceChannels.Select(s => s.ServiceId);
            var publishedServices = unitOfWork.CreateRepository<IServiceVersionedRepository>()
                .All()
                .Where(sv => sv.PublishingStatusId == psPublishedId && serviceIds.Contains(sv.UnificRootId))
                .Include(i => i.OrganizationServices)
                .ToDictionary(k => k.UnificRootId, v => v.OrganizationServices.Select(o => o.OrganizationId));

            var serviceIdsToRemove = publishedServices.Where(ps => !ps.Value.Contains(channel.OrganizationId)).Select(ps => ps.Key);
            var toRemove = serviceServiceChannels.Where(s => serviceIdsToRemove.Contains(s.ServiceId) && s.ServiceChannelId == channel.UnificRootId);
            return toRemove.ToList();
        }

        public VmPublishingResultModel WithdrawChannel(Guid channelId)
        {
            return commonService.WithdrawEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(channelId);
        }

        public VmPublishingResultModel RestoreChannel(Guid channelId)
        {
            return commonService.RestoreEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(channelId);
        }

        public IVmEntityBase DeleteChannel(Guid? entityId)
        {
            ServiceChannelVersioned result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = DeleteChannel(unitOfWork, entityId);
                unitOfWork.Save();

            });
            return new VmEntityStatusBase { Id = result.Id, PublishingStatusId = result.PublishingStatusId };
        }

        private ServiceChannelVersioned DeleteChannel(IUnitOfWorkWritable unitOfWork, Guid? entityId)
        {
            var publishStatus = TranslationManagerToEntity.Translate<String, PublishingStatusType>(PublishingStatus.Deleted.ToString(), unitOfWork);
            var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var channel = channelRep.All().Single(x => x.Id == entityId.Value);
            channel.PublishingStatus = publishStatus;
            return channel;
        }

        public IVmEntityBase GetChannelStatus(Guid? entityId)
        {
            VmPublishingStatus result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = GetChannelStatus(unitOfWork, entityId);
            });
            return new VmEntityStatusBase() { PublishingStatusId = result.Id };
        }

        private VmPublishingStatus GetChannelStatus(IUnitOfWorkWritable unitOfWork, Guid? entityId)
        {
            var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var channel = channelRep.All()
                            .Include(x => x.PublishingStatus)
                            .Single(x => x.Id == entityId.Value);

            return TranslationManagerToVm.Translate<PublishingStatusType, VmPublishingStatus>(channel.PublishingStatus);
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

        public IVmEntityBase LockChannel(Guid id)
        {
            return utilities.LockEntityVersioned<ServiceChannelVersioned, ServiceChannel>(id);
        }

        public IVmEntityBase UnLockChannel(Guid id)
        {
            return utilities.UnLockEntityVersioned<ServiceChannelVersioned, ServiceChannel>(id);
        }

        private void EnsureOrganization(Guid? id)
        {
            if (id.HasValue && !commonService.OrganizationExists(id.Value, PublishingStatus.Published))
            {
                throw new PtvAppException("Published organization does not exist!", publishedOrganizationDoesNotExist);
            }
        }

        private void UpdateConnectionTypeCheck(IUnitOfWork unitOfWork, IVmChannelConnectionType model, ServiceChannelVersioned channel)
        {
            var repository = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            bool toRemove = GetConnectionsToRemoveForPublished(unitOfWork, channel, unitOfWork.CreateRepository<IServiceServiceChannelRepository>()).Count > 0;
            if (toRemove)
            {

                var channelQuery = repository.All();
                if (model.UnificRootId.IsAssigned())
                {
                    channelQuery = channelQuery.Where(x => x.UnificRootId == model.UnificRootId);
                }
                else
                {
                    channelQuery = channelQuery.Where(x => x.UnificRoot.Versions.Any(sc => sc.Id == model.Id));
                }
                var publishedChannel = channelQuery.FirstOrDefault(x => x.PublishingStatusId == PublishingStatusCache.Get(PublishingStatus.Published));
                if (publishedChannel != null)
                {
                    model.IsPublishedCommonType = !typesCache.Compare<ServiceChannelConnectionType>(
                        publishedChannel.ConnectionTypeId, ServiceChannelConnectionTypeEnum.NotCommon.ToString());
                }
            }
        }

        public IVmEntityBase IsChannelEditable(Guid id)
        {
            return utilities.CheckIsEntityEditable<ServiceChannelVersioned, ServiceChannel>(id);
        }
    }
}
