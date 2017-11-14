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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Logic.Channels;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Framework;
using IServiceService = PTV.Database.DataAccess.Interfaces.Services.V2.IServiceService;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework.ServiceManager;
using PTV.Domain.Model.Models;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Enums.Security;

namespace PTV.Database.DataAccess.Services.V2
{
    [Framework.RegisterService(typeof(IServiceService), RegisterType.Transient)]
    internal class ServiceService : EntityServiceBase<ServiceVersioned, Service>, IServiceService
    {
        private readonly IUserIdentification userIdentification;
        private ILogger logger;
        private const string invalidElectronicChannelUrl = "Electronic channel url '{0}'";
        private const string invalidWebPageChannelUrl = "Web page channel url '{0}'";
        private const string invalidElectronicChannelAttachmentUrl = "Electronic channel attachment url '{0}'";
        private ServiceChannelLogic channelLogic;
        private VmListItemLogic listItemLogic;
        private readonly DataUtils dataUtils;
        private VmOwnerReferenceLogic ownerReferenceLogic;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private IGeneralDescriptionService generalDescriptionService;
        private IUserOrganizationService userOrganizationService;
        private IVersioningManager versioningManager;

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

        public ServiceService(
            IContextManager contextManager,
            IUserIdentification userIdentification,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            ILogger<Services.ChannelService> logger,
            ServiceChannelLogic channelLogic,
            ServiceUtilities utilities,
            ICommonServiceInternal commonService,
            IGeneralDescriptionService generalDescriptionService,
            VmListItemLogic listItemLogic,
            DataUtils dataUtils,
            VmOwnerReferenceLogic ownerReferenceLogic,
            ICacheManager cacheManager,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IValidationManager validationManager,
            IUserOrganizationService userOrganizationService,
            IVersioningManager versioningManager
            ) :
            base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker, contextManager, utilities, commonService, validationManager)
        {
            this.logger = logger;
            this.channelLogic = channelLogic;
            this.userIdentification = userIdentification;
            this.listItemLogic = listItemLogic;
            this.dataUtils = dataUtils;
            this.ownerReferenceLogic = ownerReferenceLogic;
            this.generalDescriptionService = generalDescriptionService;
            this.typesCache = cacheManager.TypesCache;
            this.languageCache = cacheManager.LanguageCache;
            this.userOrganizationService = userOrganizationService;
            this.versioningManager = versioningManager;
        }

        public VmServiceHeader GetServiceHeader(Guid? serviceId)
        {
            return ContextManager.ExecuteReader(unitOfWork => GetServiceHeader(serviceId, unitOfWork));
        }

        public VmServiceHeader GetServiceHeader(Guid? serviceId, IUnitOfWork unitOfWork)
        {
            ServiceVersioned entity;
            var result = GetModel<ServiceVersioned, VmServiceHeader>(entity = GetEntity<ServiceVersioned>(serviceId, unitOfWork,
                q => q
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x => x.ServiceNames)
                    .Include(x => x.Versioning)
            ), unitOfWork);
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var connRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            result.NumberOfConnections = connRep.All()
                .Count(x => x.ServiceId == entity.UnificRootId &&
                            x.ServiceChannel.Versions.Any(o =>
                                o.PublishingStatusId == draftStatusId ||
                                o.PublishingStatusId == publishingStatusId
                            ));

            result.PreviousInfo = serviceId.HasValue ? Utilities.CheckIsEntityEditable<ServiceVersioned, Service>(serviceId.Value, unitOfWork) : null;

            return result;
        }

        public VmKeywordSearchOutput KeywordSearch(VmKeywordSearch model)
        {
            return ContextManager.ExecuteReader(unitOfWork => KeywordSearch(model, unitOfWork));
        }

        public VmKeywordSearchOutput KeywordSearch(VmKeywordSearch model, IUnitOfWork unitOfWork)
        {
            var localizationId = languageCache.Get(model.LocalizationCode);
            var keywordRep = unitOfWork.CreateRepository<IKeywordRepository>();
            var name = model.Name.ToLower();
            var keywords = keywordRep.All().Where(i => i.Name.ToLower().Contains(name) && i.LocalizationId == localizationId).OrderBy(i => i.Name).Take(CoreConstants.MaximumNumberOfAllItems + 1).ToList();
            return new VmKeywordSearchOutput() { Keywords = TranslationManagerToVm.TranslateAll<Keyword, VmKeywordItem>(keywords.Take(CoreConstants.MaximumNumberOfAllItems)), MoreAvailable = keywords.Count > CoreConstants.MaximumNumberOfAllItems};
        }

        public VmServiceOutput GetService(VmServiceBasic model)
        {
            VmServiceOutput result = null;
            ContextManager.ExecuteReader(unitOfWork =>
            {
                result = GetService(model, unitOfWork);
            });

            return result;
        }

        private void AddConnectionsInfo(VmServiceOutput result, ICollection<ServiceServiceChannel> connections, IUnitOfWork unitOfWork)
        {
            var serviceUnificId = result.UnificRootId;
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var psDraft = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var psModified = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var connectedChannelsUnificIds = result.ConnectedChannels.Select(i => i.ChannelId).ToList();
            var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var connectedChannels = channelRep.All().Where(i =>
                    (connectedChannelsUnificIds.Contains(i.UnificRootId)) &&
                    (i.PublishingStatusId == psPublished || i.PublishingStatusId == psDraft || i.PublishingStatusId == psModified))
                .Include(i => i.ServiceChannelNames).Include(i => i.LanguageAvailabilities)
                .ToList()
                // could be done on db when it will be implemented in EF
                .GroupBy(i => i.UnificRootId)
                .Select(i => i.FirstOrDefault(x => x.PublishingStatusId == psPublished) ?? i.FirstOrDefault())
                .Select(i =>
                {
                    var modifiedData = result.ConnectedChannels.FirstOrDefault(m => m.ChannelId == i.UnificRootId);
                    var originalConnection =
                        connections.FirstOrDefault(x => x.ServiceId == serviceUnificId &&
                                                        x.ServiceChannelId == i.UnificRootId);
                    return new ServiceServiceChannel()
                    {
                        ServiceChannelId = i.UnificRootId,
                        ServiceId = serviceUnificId,
                        ServiceServiceChannelDescriptions = originalConnection?.ServiceServiceChannelDescriptions ?? new List<ServiceServiceChannelDescription>(),
                        ServiceServiceChannelDigitalAuthorizations = originalConnection?.ServiceServiceChannelDigitalAuthorizations ?? new List<ServiceServiceChannelDigitalAuthorization>(),
                        IsASTIConnection = originalConnection?.IsASTIConnection ?? false,
                        ServiceServiceChannelExtraTypes = originalConnection?.ServiceServiceChannelExtraTypes ?? new List<ServiceServiceChannelExtraType>(),
                        ChargeTypeId = originalConnection?.ChargeTypeId,
                        ServiceChannel = new ServiceChannel() {Id = i.UnificRootId, Versions = new List<ServiceChannelVersioned>() {i}},
                        Modified = modifiedData?.Modified ?? DateTime.MinValue,
                        ModifiedBy = modifiedData?.ModifiedBy ?? string.Empty
                    };
                }).ToList();

            result.Connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, VmConnectionOutput>(connectedChannels).InclusiveToList();
            result.NumberOfConnections = result.Connections.Count;
        }


        private VmServiceOutput GetService(VmServiceBasic model, IUnitOfWork unitOfWork)
        {
            VmServiceOutput result = null;
            SetTranslatorLanguage(model);
            ServiceVersioned entity = null;
            result = GetModel<ServiceVersioned, VmServiceOutput>(entity = GetEntity<ServiceVersioned>(model.Id, unitOfWork,
                q => q.Include(x => x.ServiceNames)
                    .Include(x => x.ServiceDescriptions)
                    .Include(x => x.OrganizationServices)
                    .Include(x => x.ServiceRequirements)
                    .Include(x => x.AreaMunicipalities)
                    .Include(x => x.Areas).ThenInclude(x => x.Area)
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x => x.Versioning)
                    .Include(x => x.ServiceLanguages)
                    .Include(x => x.ServiceKeywords).ThenInclude(x => x.Keyword)
                    .Include(x => x.ServiceServiceClasses)//.ThenInclude(x => x.ServiceClass).ThenInclude(x => x.Names)
                    .Include(x => x.ServiceOntologyTerms).ThenInclude(x => x.OntologyTerm).ThenInclude(x => x.Names) // to cache
                    .Include(x => x.ServiceLifeEvents).ThenInclude(x => x.LifeEvent).ThenInclude(x => x.Names) // to cache
                    .Include(x => x.ServiceIndustrialClasses)//.ThenInclude(x => x.IndustrialClass).ThenInclude(x => x.Names)
                    .Include(x => x.ServiceTargetGroups)
                    .Include(x => x.ServiceWebPages).ThenInclude(x => x.WebPage)
                    .Include(x => x.ServiceProducers).ThenInclude(x => x.Organizations)
                    .Include(x => x.ServiceProducers).ThenInclude(x => x.AdditionalInformations)
                    .Include(x => x.ServiceLaws).ThenInclude(j => j.Law).ThenInclude(k => k.Names)
                    .Include(x => x.ServiceLaws).ThenInclude(j => j.Law).ThenInclude(k => k.WebPages)
                    .Include(x => x.ServiceLaws).ThenInclude(j => j.Law).ThenInclude(k => k.WebPages).ThenInclude(l => l.WebPage)
                    .Include(x => x.UnificRoot).ThenInclude(j => j.ServiceServiceChannels).ThenInclude(j => j.ServiceServiceChannelDescriptions)
                    .Include(x => x.UnificRoot).ThenInclude(j => j.ServiceServiceChannels).ThenInclude(j => j.ServiceServiceChannelDigitalAuthorizations).ThenInclude(j => j.DigitalAuthorization)
                    .Include(x => x.UnificRoot).ThenInclude(j => j.ServiceServiceChannels).ThenInclude(j => j.ServiceServiceChannelExtraTypes).ThenInclude(j => j.ServiceServiceChannelExtraTypeDescriptions)
            ), unitOfWork);

            AddConnectionsInfo(result, entity.UnificRoot.ServiceServiceChannels, unitOfWork);
            result.PreviousInfo = result.Id.HasValue ? Utilities.CheckIsEntityEditable<ServiceVersioned, Service>(result.Id.Value, unitOfWork) : null;

            if (result.GeneralDescriptionId != null)
            {
                result.GeneralDescriptionOutput =
                    generalDescriptionService.GetGeneralDescription(
                        new VmGeneralDescriptionGet()
                        {
                            Id = result.GeneralDescriptionId,
                            Language = model.Language,
                            OnlyPublished = true
                        }, unitOfWork);

                result.GeneralDescriptionOutput.Id = result.GeneralDescriptionOutput.UnificRootId;
            }
            return result;
        }

        public VmServiceOutput SaveService(VmServiceInput model)
        {
            return ExecuteSave
            (
                unitOfWork => SaveService(unitOfWork, model),
                (unitOfWork, entity) => GetService(new VmServiceBasic() { Id = entity.Id }, unitOfWork)
            );
        }

        private ServiceVersioned SaveService(IUnitOfWorkWritable unitOfWork, VmServiceInput model)
        {
            if (model.GeneralDescriptionId.HasValue)
            {
                var gdRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var gd = unitOfWork.ApplyIncludes(gdRep.All()
                        .Where(x => x.UnificRootId == model.GeneralDescriptionId)
                        .OrderBy(x => x.PublishingStatus.PriorityFallback), i => i.Include(x => x.PublishingStatus))
                    .FirstOrDefault();
                model.GeneralDescriptionServiceTypeId = gd?.TypeId;
                model.GeneralDescriptionChargeTypeId = gd?.ChargeTypeId;
            }
            return TranslationManagerToEntity.Translate<VmServiceInput, ServiceVersioned>(model, unitOfWork);
        }

        public VmEntityHeaderBase PublishService(IVmPublishingModel model)
        {
            return model.Id.IsAssigned() ? ContextManager.ExecuteWriter(unitOfWork => PublishService(unitOfWork, model)) : null;
        }

        private VmEntityHeaderBase PublishService(IUnitOfWorkWritable unitOfWork, IVmPublishingModel model)
        {
            Guid? serviceId = model.Id;

            //Validate mandatory values
            var validationMessages = ValidationManager.CheckEntity<ServiceVersioned>(serviceId.Value, unitOfWork, model);
            if (validationMessages.Any())
            {
                throw new PtvValidationException(validationMessages, null);
            }

            //Publishing
            var affected = CommonService.PublishEntity<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, model);

            return GetServiceHeader(affected.Id, unitOfWork);
        }

        public VmServiceOutput SaveAndValidateService(VmServiceInput model)
        {
            var result = ExecuteSaveAndValidate
            (
                model.Id,
                unitOfWork => SaveService(unitOfWork, model),
                (unitOfWork, entity) => GetService(new VmServiceBasic() {Id = entity.Id}, unitOfWork)
            );

            return result;
        }

        public VmServiceHeader DeleteService(Guid serviceId)
        {
            VmServiceHeader result = null;
            ContextManager.ExecuteWriter(unitOfWork =>
            {
                var deletedService = DeleteService(unitOfWork, serviceId);
                unitOfWork.Save();
                result = GetServiceHeader(deletedService.Id, unitOfWork);

            });
            UnLockService(result.Id.Value);
            return result;
        }

        private ServiceVersioned DeleteService(IUnitOfWorkWritable unitOfWork, Guid? serviceId)
        {
            return CommonService.ChangeEntityToDeleted<ServiceVersioned>(unitOfWork, serviceId.Value);
        }

        public IVmEntityBase LockService(Guid id, bool isLockDisAllowedForArchived = false)
        {
            return Utilities.LockEntityVersioned<ServiceVersioned, Service>(id, isLockDisAllowedForArchived);
        }

        public IVmEntityBase UnLockService(Guid id)
        {
            return Utilities.UnLockEntityVersioned<ServiceVersioned, Service>(id);
        }

        public VmServiceHeader WithdrawService(Guid serviceId)
        {
            var result = CommonService.WithdrawEntity<ServiceVersioned, ServiceLanguageAvailability>(serviceId);
            UnLockService(result.Id.Value);
            return GetServiceHeader(result.Id);
        }

        public VmServiceHeader RestoreService(Guid serviceId)
        {
            var result = CommonService.RestoreEntity<ServiceVersioned, ServiceLanguageAvailability>(serviceId);
            UnLockService(result.Id.Value);
            return GetServiceHeader(result.Id);
        }

        public VmServiceHeader ArchiveLanguage(VmEntityBasic model)
        {
            var entity = CommonService.ArchiveLanguage<ServiceVersioned, ServiceLanguageAvailability>(model);
            UnLockService(entity.Id);
            return GetServiceHeader(entity.Id);
        }

        public VmServiceHeader RestoreLanguage(VmEntityBasic model)
        {
            var entity = CommonService.RestoreLanguage<ServiceVersioned, ServiceLanguageAvailability>(model);
            UnLockService(entity.Id);
            return GetServiceHeader(entity.Id);
        }

        public VmServiceHeader WithdrawLanguage(VmEntityBasic model)
        {
            var entity = CommonService.WithdrawLanguage<ServiceVersioned, ServiceLanguageAvailability>(model);
            UnLockService(entity.Id);
            return GetServiceHeader(entity.Id);
        }

        public VmServiceHeader GetValidatedEntity(VmEntityBasic model)
        {
            return ExecuteValidate
            (
                () => Utilities.LockEntityVersioned<ServiceVersioned, Service>(model.Id.Value, true),
                (unitOfWork) => GetServiceHeader(model.Id, unitOfWork)
            );
        }

        public VmConnectableServiceSearchResult GetConnectableService(VmConnectableServiceSearch search)
        {
            search.Name = search.Name != null
                ? search.Name.Trim()
                : search.Name;

            return ContextManager.ExecuteReader(unitOfWork =>
            {
                var languageCode = SetTranslatorLanguage(search);
                var selectedLanguageId = languageCache.Get(languageCode.ToString());
                var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
                var publishedStatusId = PublishingStatusCache.Get(PublishingStatus.Published);
                var draftStatusId = PublishingStatusCache.Get(PublishingStatus.Draft);
                var notCommonId = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.NotCommon.ToString());
                var languagesIds = new List<Guid>() { selectedLanguageId };

                var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var resultTemp = serviceRep.All();

                #region SearchByFilterParam

                if (search.Type == DomainEnum.Channels)
                {
                    var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                    var channel = channelRep.All().FirstOrDefault(x => x.Id == search.Id);
                    if (channel?.ConnectionTypeId == notCommonId)
                    {
                        resultTemp = resultTemp
                        .Where(x => (x.OrganizationId == channel.OrganizationId));
                    }

                    var languageAvaliabilitiesRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
                    languagesIds = languageAvaliabilitiesRep.All()
                    .Where(x => x.ServiceChannelVersionedId == search.Id)
                    .Select(x => x.LanguageId).ToList();
                }

                if (search.OrganizationId.HasValue)
                {
                    resultTemp = resultTemp
                        .Where(
                            x => x.OrganizationServices
                                .Any(o => o.OrganizationId == search.OrganizationId) || x.OrganizationId == search.OrganizationId.Value
                        );

                }
                if (!string.IsNullOrEmpty(search.Name))
                {
                    var rootId = GetRootIdFromString(search.Name);
                    if (!rootId.HasValue)
                    {
                        var searchText = search.Name.ToLower();
                        resultTemp = resultTemp
                            .Where(
                                x => x.ServiceNames.Any(y => y.Name.ToLower().Contains(searchText))
                            );
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
                                    y => !string.IsNullOrEmpty(y.Name)));
                }

                if (search.ServiceTypeId.HasValue)
                {
                    var generalDescIds = generalDescriptionRep.All()
                                            .Where(x => x.TypeId == search.ServiceTypeId &&
                                                        x.PublishingStatusId == publishedStatusId)
                                            .Select(x => x.UnificRootId);
                    resultTemp = resultTemp
                        .Where(x => (x.TypeId == search.ServiceTypeId.Value && x.StatutoryServiceGeneralDescriptionId == null) || generalDescIds.Any(d => d == x.StatutoryServiceGeneralDescriptionId));
                }

                //commonService.ExtendPublishingStatusesByEquivalents(search.SelectedPublishingStatuses);
                resultTemp = resultTemp.WherePublishingStatusIn(new List<Guid>() {
                    publishedStatusId,
                    draftStatusId
                });

                resultTemp = resultTemp.Where(x => x.LanguageAvailabilities.Select(y => y.LanguageId).Any(l => languagesIds.Contains(l)));

                #endregion SearchByFilterParam


                resultTemp = resultTemp
                    //                                .Include(sv => sv.OrganizationServices)
                    //                                .Include(sv => sv.ServiceNames)
                    .Include(sv => sv.StatutoryServiceGeneralDescription)
                    .ThenInclude(ssgd => ssgd.Versions)
                    //                                .Include(sv => sv.Type)
                    .Include(sv => sv.LanguageAvailabilities)
                    .ThenInclude(sla => sla.Language);



                var rowCount = resultTemp.Count();
                var pageNumber = search.PageNumber.PositiveOrZero();
                var resultTempData = resultTemp.Select(i => new
                    {
                        Id = i.Id,
                        ServiceVersioned = i,
                        UnificRootId = i.UnificRootId,
                        //                    Name = i.ServiceNames
                        //                        .OrderBy(x => x.Localization.OrderNumber)
                        //                        .FirstOrDefault(x => selectedLanguageId == x.LocalizationId && x.TypeId == nameTypeId).Name,
                        //                    AllNames = i.ServiceNames
                        //                        .Where(x => x.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()))
                        //                        .Select(x => new { x.LocalizationId, x.Name }),
                        Modified = i.Modified, // required for sorting
                        ModifiedBy = i.ModifiedBy // required for sorting
                    })
                    .OrderByDescending(i => i.Modified)
                    .ApplyPaging(pageNumber);

                var serviceIds = resultTempData.Data.Select(i => i.Id).ToList();

                var serviceNameRep = unitOfWork.CreateRepository<IServiceNameRepository>();

                var serviceNames = serviceNameRep.All().Where(x => serviceIds.Contains(x.ServiceVersionedId) && x.TypeId == nameTypeId).OrderBy(i => i.Localization.OrderNumber).Select(i => new { i.ServiceVersionedId, i.Name, i.LocalizationId }).ToList().GroupBy(i => i.ServiceVersionedId)
                    .ToDictionary(i => i.Key, i => i.ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));

                var result = resultTempData.Data.Select(i => {
                    Guid? typeId = null;

                    if (i.ServiceVersioned?.TypeId != null)
                    {
                        typeId = i.ServiceVersioned.TypeId.Value;
                    }
                    else if (i.ServiceVersioned.StatutoryServiceGeneralDescription?.Versions?.Count > 0)
                    {
                        var ssgdv = versioningManager.ApplyPublishingStatusFilterFallback(i.ServiceVersioned.StatutoryServiceGeneralDescription.Versions);
                        typeId = ssgdv?.TypeId;
                    }

                    return new VmConnectableService
                    {
                        Id = i.Id,
                        UnificRootId = i.UnificRootId,
                        Name = serviceNames.TryGetOrDefault(i.Id, new Dictionary<string, string>()),
                        ServiceTypeId = typeId,
                        ServiceType = typeId == null ? string.Empty : typesCache.GetByValue<ServiceType>(typeId.Value),
                        LanguagesAvailabilities = TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(i.ServiceVersioned.LanguageAvailabilities),
                        OrganizationId = i.ServiceVersioned.OrganizationId,
                        Modified = i.ServiceVersioned.Modified.ToEpochTime(),
                        ModifiedBy = i.ServiceVersioned.ModifiedBy
                    };
                })
                .ToList();
                return new VmConnectableServiceSearchResult()
                {
                    Data = result,
                    MoreAvailable = resultTempData.MoreAvailable,
                    Count = rowCount,
                    PageNumber = pageNumber
                };
            });
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

        public VmConnectionsOutput SaveRelations(VmConnectionsInput model)
        {
            ContextManager.ExecuteWriter(unitOfWork =>
            {
                SaveRelations(unitOfWork, model);
                unitOfWork.Save();
            });
            return GetRelations(model);
        }

        private void SaveRelations(IUnitOfWorkWritable unitOfWork, VmConnectionsInput model)
        {
            var unificRootId = versioningManager.GetUnificRootId<ServiceVersioned>(unitOfWork, model.Id);
            if (unificRootId.HasValue)
            {
                model.UnificRootId = unificRootId.Value;
                TranslationManagerToEntity.Translate<VmConnectionsInput, Service>(model, unitOfWork);
            }
        }

        private VmConnectionsOutput GetRelations(VmConnectionsInput model)
        {
            return ContextManager.ExecuteReader(unitOfWork =>
            {
                return GetRelations(unitOfWork, model);
            });
        }

        private VmConnectionsOutput GetRelations(IUnitOfWork unitOfWork, VmConnectionsInput model)
        {
            var serviceRep = unitOfWork.CreateRepository<IServiceRepository>();
            var unificRootId = versioningManager.GetUnificRootId<ServiceVersioned>(unitOfWork, model.Id);
            if (unificRootId.HasValue)
            {
                var service = serviceRep.All()
                                .Include(j => j.ServiceServiceChannels).ThenInclude(j => j.ServiceChannel).ThenInclude(j => j.Versions).ThenInclude(j => j.ServiceChannelNames)
                                .Include(j => j.ServiceServiceChannels).ThenInclude(j => j.ServiceChannel).ThenInclude(j => j.Versions).ThenInclude(j => j.LanguageAvailabilities)
                                .Include(j => j.ServiceServiceChannels).ThenInclude(j => j.ServiceServiceChannelDescriptions)
                                .Include(j => j.ServiceServiceChannels).ThenInclude(j => j.ServiceServiceChannelDigitalAuthorizations).ThenInclude(j => j.DigitalAuthorization)
                                .Single(x => x.Id == unificRootId.Value);
                var result = TranslationManagerToVm.Translate<Service, VmConnectionsOutput>(service);
                result.Id = model.Id;
                return result;
            }
            return null;
        }
    }
}
