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
using PTV.Domain.Logic;
using PTV.Domain.Model.Models.OpenApi;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Internal;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Utils;
using PTV.Domain.Logic.Services;
using PTV.Framework.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using PTV.Domain.Model.Models.OpenApi.V4;
using Remotion.Linq.Clauses;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof (IServiceService), RegisterType.Transient)]
    internal class ServiceService : ServiceBase, IServiceService
    {
        private IContextManager contextManager;

        private ILogger logger;
        private ServiceLogic logic;
        private ServiceUtilities utilities;
        private DataUtils dataUtils;
        private ICommonServiceInternal commonService;
        private VmOwnerReferenceLogic ownerReferenceLogic;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private IVersioningManager versioningManager;
        private IGeneralDescriptionService generalDescriptionService;
        private const string messageOrganizersFailed = "Service.OrganizerUpdateException.MessageFailed";
        private const string publishedOrganizationDoesNotExist = "Service.Save.OrganizationMissing.MessageFailed";
        private const string messageServicePublishFailed = "Service.Publish.MessageFailed";

        public ServiceService(
            IContextManager contextManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            ILogger<ServiceService> logger,
            ServiceLogic logic,
            ServiceUtilities utilities,
            DataUtils dataUtils,
            ICommonServiceInternal commonService,
            VmOwnerReferenceLogic ownerReferenceLogic,
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
            this.logic = logic;
            this.utilities = utilities;
            this.dataUtils = dataUtils;
            this.commonService = commonService;
            this.ownerReferenceLogic = ownerReferenceLogic;
            this.typesCache = typesCache;
            this.languageCache = languageCache;
            this.versioningManager = versioningManager;
            this.generalDescriptionService = generalDescriptionService;
        }

        public IVmGetServiceSearch GetServiceSearch()
        {
            // get enum values to local variables before using them inside LINQ queries as otherwise the provider does the queries in-memory and not in DB
            // as it cannot translate the enum values to db queries and this way we get better performance as the queries are evaluated in DB
            string statusDeletedCode = PublishingStatus.Deleted.ToString();
            string statusOldPublishedCode = PublishingStatus.OldPublished.ToString();
            string statusModifiedPublishedCode = PublishingStatus.Modified.ToString();

            var result = new VmGetServiceSearch();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceClassesRep = unitOfWork.CreateRepository<IServiceClassRepository>();
                var userOrganization = utilities.GetUserOrganization(unitOfWork);
                var digitalAuthorizationRep = unitOfWork.CreateRepository<IDigitalAuthorizationRepository>();
                var chargeTypes = commonService.GetPhoneChargeTypes();
                var digitalAuthorization = CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<DigitalAuthorization, DigitalAuthorizationName>(unitOfWork, digitalAuthorizationRep.All()), null), x => x.Name);

                result = new VmGetServiceSearch
                {
                    OrganizationId = userOrganization
                };
                var publishingStatuses = commonService.GetPublishingStatuses();
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ServiceClasses", TranslationManagerToVm.TranslateAll<ServiceClass, VmListItem>(GetIncludesForFinto<ServiceClass, ServiceClassName>(unitOfWork, serviceClassesRep.All().OrderBy(x => x.Label)))),
                    () => GetEnumEntityCollectionModel("PublishingStatuses", publishingStatuses),
                    () => GetEnumEntityCollectionModel("ServiceTypes", commonService.GetServiceTypes()),
                    () => GetEnumEntityCollectionModel("DigitalAuthorizations", digitalAuthorization),
                    () => GetEnumEntityCollectionModel("ChargeTypes", chargeTypes),
                    () => GetEnumEntityCollectionModel("ChannelTypes", commonService.GetServiceChannelTypes())
                );
                result.SelectedPublishingStatuses = publishingStatuses.Where(x => x.Code != statusModifiedPublishedCode && x.Code != statusDeletedCode && x.Code != statusOldPublishedCode).Select(x => x.Id).ToList();

            });
            return result;
        }

        public VmEntityNames GetServiceNames(VmEntityBase model)
        {
            var result = new VmEntityNames();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var service = unitOfWork.ApplyIncludes(serviceRep.All(), q =>
                    q.Include(i => i.ServiceNames)
                        .Include(i => i.LanguageAvailabilities)).Single(x => x.Id == model.Id.Value);

                result = TranslationManagerToVm.Translate<ServiceVersioned, VmEntityNames>(service);

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages())
                );
            });
            return result;
        }

        public IVmServiceSearchResult SearchServices(IVmServiceSearch vmServiceSearch)
        {
            vmServiceSearch.Name = vmServiceSearch.Name != null ? vmServiceSearch.Name.Trim() : vmServiceSearch.Name;
            IReadOnlyList<IVmServiceListItem> result = new List<IVmServiceListItem>();
            bool moreAvailable = false;
            var count = 0;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var languageCode = SetTranslatorLanguage(vmServiceSearch);
                var languagesIds = vmServiceSearch.Languages.Select(language => languageCache.Get(language.ToString())).ToList();
                var selectedLanguageId = languageCache.Get(languageCode.ToString());
                var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
                var roleTypeResponsibleId = typesCache.Get<RoleType>(RoleTypeEnum.Responsible.ToString());
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
                    resultTemp = resultTemp.Where(x => x.OrganizationServices.Any(o => o.RoleTypeId == typesCache.Get<RoleType>(RoleTypeEnum.Responsible.ToString()) && o.OrganizationId == vmServiceSearch.OrganizationId));
                }
                if (!string.IsNullOrEmpty(vmServiceSearch.Name))
                {
                    var rootId = GetRootIdFromString(vmServiceSearch.Name);
                    if (!rootId.HasValue)
                    {
                        var searchText = vmServiceSearch.Name.ToLower();
                        resultTemp =
                            resultTemp.Where(
                                x =>
                                    x.ServiceNames.Any(
                                        y =>
                                            y.Name.ToLower().Contains(searchText) &&
                                            languagesIds.Contains(y.LocalizationId)));
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
                moreAvailable = count > (vmServiceSearch.PageNumber.PositiveOrZero() == 0
                                    ? CoreConstants.MaximumNumberOfAllItems
                                    : vmServiceSearch.PageNumber.PositiveOrZero() * CoreConstants.MaximumNumberOfAllItems);

                var resultTempData = resultTemp.Select(i => new
                {
                    Id = i.Id,
                    PublishingStatusId = i.PublishingStatusId,
                    UnificRootId = i.UnificRootId,
                    Name = i.ServiceNames.OrderBy(x=>x.Localization.OrderNumber).FirstOrDefault(x => languagesIds.Contains(x.LocalizationId) && x.TypeId == nameTypeId).Name,
                    TypeId = i.TypeId,
                    TypeName = i.Type.Names.OrderBy(x => x.Localization.OrderNumber).FirstOrDefault(x => languagesIds.Contains(x.LocalizationId)).Name,
                    LanguageAvailabilities = i.LanguageAvailabilities.OrderBy(x => x.Language.OrderNumber),
                    StatutoryServiceDescriptionVersions = i.StatutoryServiceGeneralDescription.Versions,
                    Versioning = i.Versioning,
                    VersionMajor = i.Versioning.VersionMajor,
                    VersionMinor = i.Versioning.VersionMinor,
                    OrganizationServices = i.OrganizationServices.Where(x => x.RoleTypeId == roleTypeResponsibleId && x.OrganizationId != null).Select(x => x.OrganizationId.Value),
                    Modified = i.Modified,
                    ModifiedBy = i.ModifiedBy,
                    GeneralDescription = i.StatutoryServiceGeneralDescription.Versions.FirstOrDefault(x => x.PublishingStatusId == publishedStatusId).TypeId
                })
                .ApplySortingByVersions(vmServiceSearch.SortData, new VmSortParam() { Column = "Modified", SortDirection = SortDirectionEnum.Desc })
                .ApplyPagination(vmServiceSearch.PageNumber)
                .ToList();

                result = resultTempData.Select(i => new VmServiceListItem
                {
                    Id = i.Id,
                    PublishingStatusId = i.PublishingStatusId,
                    UnificRootId = i.UnificRootId,
                    Name = i.Name,
                    ServiceTypeId = i.TypeId,
                    ServiceType = typesCache.GetByValue<ServiceType>(i.TypeId ?? versioningManager.ApplyPublishingStatusFilterFallback(i.StatutoryServiceDescriptionVersions).TypeId),
                    LanguagesAvailabilities = TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(i.LanguageAvailabilities),
                    Version = TranslationManagerToVm.Translate<Versioning, VmVersion>(i.Versioning),
                    Organizations = i.OrganizationServices.ToList(),
                    Modified = i.Modified.ToEpochTime(),
                    ModifiedBy = i.ModifiedBy,
                    GeneralDescriptionTypeId = i.GeneralDescription
                })
                .ToList();                

                return result;
            });
            return new VmServiceSearchResult() {
                Services = result,
                PageNumber = ++vmServiceSearch.PageNumber,
                MoreAvailable = moreAvailable,
                Count = count
            };
        }
                
        public IVmServiceSearchResult SearchRelationService(IVmServiceSearch model)
        {
            IReadOnlyList<IVmServiceListItem> result = new List<IVmServiceListItem>();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var resultTemp = unitOfWork.ApplyIncludes(serviceRep.All().Where(x => x.Id == model.Id.Value), i => i
                    .Include(j => j.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.ServiceClasses).ThenInclude(j => j.ServiceClass)
                    .Include(j => j.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.OntologyTerms).ThenInclude(j => j.OntologyTerm), true);

                resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                    q.Include(i => i.ServiceNames).ThenInclude(i => i.Type)
                    .Include(i => i.UnificRoot)
                        .ThenInclude(i => i.ServiceServiceChannels)
                        .ThenInclude(i => i.ServiceChannel)
                        .ThenInclude(i => i.Versions)
                        .ThenInclude(i => i.ServiceChannelNames)
                        .ThenInclude(i => i.Type)
                    .Include(i => i.UnificRoot)
                        .ThenInclude(i => i.ServiceServiceChannels)
                        .ThenInclude(i => i.ServiceChannel)
                        .ThenInclude(i => i.Versions)
                        .ThenInclude(i => i.Type)
                    .Include(i => i.UnificRoot)
                        .ThenInclude(i => i.ServiceServiceChannels)
                        .ThenInclude(i => i.ServiceServiceChannelDescriptions)
                        .ThenInclude(i => i.Type)
                     .Include(i => i.LanguageAvailabilities)
                        .ThenInclude(i => i.Language)
                        );

                // showing finish texts
                SetTranslatorLanguage(model);
                result = TranslationManagerToVm.TranslateAll<ServiceVersioned, VmServiceRelationListItem>(resultTemp).Cast<IVmServiceListItem>().ToList();
            });
            return new VmServiceSearchResult() { Services = result };
        }
                
        public IVmServiceSearchResult SearchRelationServices(IVmServiceSearch model)
        {
            IReadOnlyList<IVmServiceListItem> result = new List<IVmServiceListItem>();
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);

                var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                var channel = channelRep.All().FirstOrDefault(x => x.Id == model.Id);
                if (channel!=null)
                {
                    var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    var resultTemp = unitOfWork.ApplyIncludes(serviceRep.All().Where(x => x.UnificRoot.ServiceServiceChannels.Any(y => y.ServiceChannelId == channel.UnificRootId)), i => i
                        .Include(j => j.StatutoryServiceGeneralDescription)
                            .ThenInclude(j => j.Versions)
                            .ThenInclude(j => j.ServiceClasses)
                            .ThenInclude(j => j.ServiceClass)
                        .Include(j => j.StatutoryServiceGeneralDescription)
                            .ThenInclude(j => j.Versions)
                            .ThenInclude(j => j.OntologyTerms)
                            .ThenInclude(j => j.OntologyTerm)
                        .Include(j => j.LanguageAvailabilities)
                            .ThenInclude(j => j.Language)
                        , true);

                    resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                        q.Include(i => i.ServiceNames).ThenInclude(i => i.Type)
                        .Include(i => i.UnificRoot)
                            .ThenInclude(i => i.ServiceServiceChannels)
                            .ThenInclude(i => i.ServiceChannel)
                            .ThenInclude(i => i.Versions)
                            .ThenInclude(i => i.ServiceChannelNames)
                            .ThenInclude(i => i.Type)
                        .Include(i => i.UnificRoot)
                            .ThenInclude(i => i.ServiceServiceChannels)
                            .ThenInclude(i => i.ServiceChannel)
                            .ThenInclude(i => i.Versions)
                            .ThenInclude(i => i.Type)
                        .Include(i => i.UnificRoot)
                            .ThenInclude(i => i.ServiceServiceChannels)
                            .ThenInclude(i => i.ServiceServiceChannelDescriptions)
                            .ThenInclude(i => i.Type)
                        .Include(i => i.UnificRoot)
                            .ThenInclude(i => i.ServiceServiceChannels)
                            .ThenInclude(i => i.ServiceChannel)
                            .ThenInclude(i => i.Versions)
                            .ThenInclude(i => i.LanguageAvailabilities)
                            .ThenInclude(i => i.Language)
                        );

                    resultTemp = versioningManager.ApplyPublishingStatusOrderByPriorityFallback(resultTemp);

                    result = TranslationManagerToVm.TranslateAll<ServiceVersioned, VmServiceRelationListItem>(resultTemp.ToList());
                }
            });
            return new VmServiceSearchResult() { Services = result };
        }
        
//        private IQueryable<ServiceVersioned> FilterServicedForLatestVersion(IQueryable<ServiceVersioned> serviceQuery)
//        {
//            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
//            var psDraft = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
//            var psModified = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
//            return serviceQuery.GroupBy(x => x.UnificRoot)
//                         .Select(collection => collection.FirstOrDefault(i => i.PublishingStatusId == psPublished) ??
//                            collection.FirstOrDefault(i => i.PublishingStatusId == psDraft) ??
//                            collection.FirstOrDefault(i => i.PublishingStatusId == psPublished)).Where(x => x != null);
//        }

        public IVmServiceStep1 GetServiceStep1(IVmGetServiceStep model)
        {
            VmServiceStep1 result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);

                var serviceTypes = commonService.GetServiceTypes();
                var areaInformationTypes = commonService.GetAreaInformationTypes();
                var service = GetEntity<ServiceVersioned>(model.Id, unitOfWork,
                    q => q.Include(x => x.ServiceNames)
                         .Include(x => x.ServiceDescriptions)
                         .Include(x => x.LanguageAvailabilities)
                         .Include(x => x.ServiceLanguages)
                         .Include(x => x.ServiceRequirements).Include(x=>x.PublishingStatus)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.Names)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.Descriptions)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.StatutoryServiceRequirements)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.ServiceClasses).ThenInclude(j => j.ServiceClass)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.TargetGroups)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.IndustrialClasses).ThenInclude(j => j.IndustrialClass)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.LifeEvents).ThenInclude(j => j.LifeEvent)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.OntologyTerms).ThenInclude(j => j.OntologyTerm)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.Type)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.ChargeType)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.StatutoryServiceLaws).ThenInclude(j=>j.Law)
                         .Include(x => x.ServiceLaws).ThenInclude(j => j.Law).ThenInclude(k => k.Names)
                         .Include(x => x.ServiceLaws).ThenInclude(j => j.Law).ThenInclude(k => k.WebPages)
                         .Include(x => x.ServiceLaws).ThenInclude(j => j.Law).ThenInclude(k => k.WebPages).ThenInclude(l => l.WebPage)
                         .Include(x => x.ServiceTargetGroups)
                         .Include(x => x.AreaMunicipalities)
                         .Include(x => x.Areas).ThenInclude(x => x.Area)
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.Organization).ThenInclude(x => x.Versions).ThenInclude(x => x.OrganizationNames).ThenInclude(x => x.Type)
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.Organization).ThenInclude(x => x.Versions).ThenInclude(x => x.OrganizationNames)
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.RoleType)
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.ProvisionType)
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.AdditionalInformations)
                         .Include(x => x.ServiceWebPages).ThenInclude(x => x.WebPage)
                         .Include(x => x.FundingType)
                );
                result = GetModel<ServiceVersioned, VmServiceStep1>(service, unitOfWork);
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages()),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("ServiceTypes", serviceTypes),
                    () => GetEnumEntityCollectionModel("Municipalities", commonService.GetMunicipalities(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Laws", commonService.GetLaws(unitOfWork, result.GeneralDescription?.Laws.Select(x => x.Id.Value).ToList())),
                    () => GetEnumEntityCollectionModel("AreaInformationTypes", areaInformationTypes),
                    () => GetEnumEntityCollectionModel("AreaTypes", commonService.GetAreaTypes()),
                    () => GetEnumEntityCollectionModel("BusinessRegions", commonService.GetAreas(unitOfWork, AreaTypeEnum.BusinessRegions)),
                    () => GetEnumEntityCollectionModel("HospitalRegions", commonService.GetAreas(unitOfWork, AreaTypeEnum.HospitalRegions)),
                    () => GetEnumEntityCollectionModel("Provincies", commonService.GetAreas(unitOfWork, AreaTypeEnum.Province)),
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork).ToList()),
                    () => GetEnumEntityCollectionModel("FundingTypes", commonService.GetServiceFundingTypes().ToList())
                );

                if (!model.Id.IsAssigned() && !result.ServiceTypeId.IsAssigned())
                {
                    result.ServiceTypeId = serviceTypes.Single(x=>x.Code == ServiceTypeEnum.Service.ToString()).Id;
                }

                if (!result.AreaInformationTypeId.IsAssigned())
                {
                    result.AreaInformationTypeId = areaInformationTypes.Single(x => x.Code == AreaInformationTypeEnum.WholeCountry.ToString()).Id;
                }

                var userOrganization = utilities.GetUserOrganization(unitOfWork);
                if (userOrganization.HasValue && service == null)
                {
                    var defaultAreaInformation = GetServiceAreaInformation(userOrganization.Value, unitOfWork);
                    result.Organizers = new List<Guid> {userOrganization.Value};
                    result.AreaInformationTypeId = defaultAreaInformation.AreaInformationTypeId;
                    result.AreaBusinessRegions = defaultAreaInformation.AreaBusinessRegions;
                    result.AreaHospitalRegions = defaultAreaInformation.AreaHospitalRegions;
                    result.AreaMunicipality = defaultAreaInformation.AreaMunicipality;
                    result.AreaProvince = defaultAreaInformation.AreaProvince;
                }
            });
            return result;
        }
        
        private T GetTopParentTree<T>(IQueryable<T> inputQuery, T item) where T : IHierarchy<T>, IEntityIdentifier
        {
            while (true)
            {
                if (!item.ParentId.HasValue)
                {
                    return item;
                }

                var parentItem = inputQuery.First(x => x.Id == item.ParentId);
                parentItem.Children.Add(item);
                item = parentItem;
            }
        }

        private Dictionary<Guid, T> MergeBranchToTree<T>(T branch, Dictionary<Guid, T> tree) where T : IHierarchy<T>, IEntityIdentifier
        {
            if (tree.ContainsKey(branch.Id))
            {
                var childrenDictionary = tree[branch.Id].Children.ToDictionary(x => x.Id);

                return branch.Children.Aggregate(childrenDictionary, (current, branchChild) => MergeBranchToTree<T>(branchChild, current));
            }

            tree.Add(branch.Id, branch);
            return tree;
        }

        public IVmServiceStep2 GetServiceStep2(IVmGetServiceStep model)
        {
            var result = new VmServiceStep2();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var service = GetEntity<ServiceVersioned>(model.Id, unitOfWork,
                    q => q
                         .Include(x => x.ServiceKeywords).ThenInclude(x => x.Keyword)
                         .Include(x => x.ServiceKeywords).ThenInclude(x => x.Keyword).ThenInclude(x=>x.Localization)
                         .Include(x => x.ServiceServiceClasses).ThenInclude(x => x.ServiceClass).ThenInclude(x => x.Names)
                         .Include(x => x.ServiceOntologyTerms).ThenInclude(x => x.OntologyTerm).ThenInclude(x => x.Names)
                         .Include(x => x.ServiceLifeEvents).ThenInclude(x => x.LifeEvent).ThenInclude(x => x.Names)
                         .Include(x => x.ServiceIndustrialClasses).ThenInclude(x => x.IndustrialClass).ThenInclude(x => x.Names)
                         .Include(x => x.ServiceTargetGroups)

                         );
                SetTranslatorLanguage(model);
                result = GetModel<ServiceVersioned, VmServiceStep2>(service, unitOfWork);

                var serviceClassesRep = unitOfWork.CreateRepository<IServiceClassRepository>();

                var lifeEventRep = unitOfWork.CreateRepository<ILifeEventRepository>();

                var keyWordRep = unitOfWork.CreateRepository<IKeywordRepository>();

                var targetGroupRep = unitOfWork.CreateRepository<ITargetGroupRepository>();

                var industrialClassesRep = unitOfWork.CreateRepository<IIndustrialClassRepository>();

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("TopTargetGroups", CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<TargetGroup, TargetGroupName>(unitOfWork, targetGroupRep.All().OrderBy(x => x.Label)), 1), x => x.Code)),
                    () => GetEnumEntityCollectionModel("TopLifeEvents", CreateTree<VmTreeItem>(LoadFintoTree(GetIncludesForFinto<LifeEvent, LifeEventName>(unitOfWork, lifeEventRep.All()), 1), x => x.Name)),
                    () => GetEnumEntityCollectionModel("KeyWords", TranslationManagerToVm.TranslateAll<Keyword, VmKeywordItem>(keyWordRep.All().Where(x=>x.Localization.Code == model.Language.ToString()).OrderBy(x => x.Name)).ToList()),
                    () => GetEnumEntityCollectionModel("TopServiceClasses", CreateTree<VmTreeItem>(LoadFintoTree(GetIncludesForFinto<ServiceClass, ServiceClassName>(unitOfWork, serviceClassesRep.All()), 1), x => x.Name)),
                    () => GetEnumEntityCollectionModel("IndustrialClasses", TranslationManagerToVm.TranslateAll<IFintoItem, VmTreeItem>(GetIncludesForFinto<IndustrialClass, IndustrialClassName>(unitOfWork, industrialClassesRep.All().Where(x=>x.Code=="5").OrderBy(x => x.Label))).ToList())
                );
            });

            return result;
        }

        private void FillParentPath<T>(IUnitOfWork unitOfWork, IList<Guid> ids, List<VmTreeItem> items) where T : FintoItemBase<T>, IEntityIdentifier
        {
            var rep = unitOfWork.CreateRepository<IRepository<T>>();
            var leQuery = unitOfWork.ApplyIncludes(rep.All().Where(x => ids.Contains(x.Id)), query => query.Include(lifeEvent => lifeEvent.Parent));
            var leFiltered = SearchFintoFlattenTree(rep.All(), leQuery).ToDictionary(x => x.Id);

            foreach (var lifeEvent in items)
            {
                lifeEvent.Name = GetParentPath(leFiltered, lifeEvent.Id);
            }
        }

        private string GetFintoNodePath<T>(IEnumerable<T> tree, Guid id)  where T : IFintoItemChildren
        {
            foreach (var item in tree)
            {
                if (item.Id == id)
                {
                    return item.Label;
                }
                else
                {
                    var subLabel = GetFintoNodePath(item.Children, id);
                    if (!string.IsNullOrEmpty(subLabel))
                    {
                        return item.Label + ":" + subLabel;
                    }
                }
            }
            return string.Empty;
        }

        private string GetParentPath<T>(IDictionary<Guid, T> tree, Guid id) where T : FintoItemBase<T>
        {
            T item;
            if (tree.TryGetValue(id, out item))
            {
                return GetParentPath(item);
            }

            return string.Empty;
        }

        private string GetParentPath<T>(T item) where T : FintoItemBase<T>
        {
            if (item.Parent != null)
            {
                return $"{GetParentPath(item.Parent)}:{item.Label}";
            }
            return item.Label;
        }

        public IVmServiceStep3 GetServiceStep3(IVmGetServiceStep model)
        {
            var result = new VmServiceStep3();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var service = GetEntity<ServiceVersioned>(model.Id, unitOfWork,
                    q => q
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.Organization).ThenInclude(x => x.Versions).ThenInclude(x => x.OrganizationNames).ThenInclude(x => x.Type)
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.Organization).ThenInclude(x => x.Versions).ThenInclude(x => x.OrganizationNames)
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.RoleType)
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.ProvisionType)
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.AdditionalInformations)
                         );
                SetTranslatorLanguage(model);
                result = GetModel<ServiceVersioned, VmServiceStep3>(service, unitOfWork);

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("ProvisionTypes", commonService.GetProvisionTypes())
                    //() => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizations(unitOfWork))
                );

                var userOrganization = utilities.GetUserOrganization(unitOfWork);
                if (userOrganization.HasValue && service == null)
                {
                    result.Organizers = new List<Guid> {userOrganization.Value};
                }
//                listItemLogic.SelectTreeItems(result.Organizers, true, false);
            });
            return result;
        }

        //Step4
        public IVmServiceStep4ChannelData GetServiceStep4Channeldata(IVmGetServiceStep model)
        {
            var result = new VmServiceStep4ChannelData();
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                var userOrganization = utilities.GetUserOrganization(unitOfWork);
                result.OrganizationId = userOrganization;

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork).ToList()),
                    () => GetEnumEntityCollectionModel("ChannelTypes", commonService.GetServiceChannelTypes().ToList())
                );
                                
                var serviceRootId = unitOfWork.CreateRepository<IServiceVersionedRepository>().All().Where(sv => sv.Id == model.Id.Value).Select(sv => sv.UnificRootId).FirstOrDefault();
                if (serviceRootId.IsAssigned())
                {
                    var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                    var resultTemp = channelRep.All().Where(x => x.UnificRoot.ServiceServiceChannels.Any(y => y.ServiceId == serviceRootId));

                    resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                            q.Include(i => i.ServiceChannelNames)
                                .ThenInclude(i => i.Type)
                            .Include(i => i.Type)
                            .Include(i => i.UnificRoot)
                                .ThenInclude(i => i.ServiceServiceChannels)
                                .ThenInclude(i => i.ServiceChannel)
                                .ThenInclude(i => i.Versions)
                            .Include(i => i.UnificRoot)
                                .ThenInclude(i => i.ServiceServiceChannels)
                                .ThenInclude(i => i.Service)
                                .ThenInclude(i => i.Versions)
                            .Include(i => i.LanguageAvailabilities)
                                .ThenInclude(i => i.Language)
                            );

                    resultTemp = versioningManager.ApplyPublishingStatusOrderByPriorityFallback(resultTemp);

                    var resultData = resultTemp.ToList();
                    result.Id = model.Id;
                    result.AttachedChannels = TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmChannelListItem>(resultData);
                    result.Connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, VmConnection>(resultData.SelectMany(x => x.UnificRoot.ServiceServiceChannels.Where(y => y.ServiceId == serviceRootId)));
                }
                
            });
            return result;
        }

        public IVmEntityBase AddService(VmService model)
        {
            ServiceVersioned result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = AddService(unitOfWork, model);
                unitOfWork.Save();

            });
            return new VmEntityRootStatusBase() { Id = result.Id, UnificRootId = result.UnificRootId, PublishingStatusId = commonService.GetDraftStatusId() };
        }

        private ServiceVersioned AddService(IUnitOfWorkWritable unitOfWork, VmService vm) {

            vm.PublishingStatusId = commonService.GetDraftStatusId();
            EnsureOrganization(vm.Step1Form.Organizers);
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();

            if (vm.Step1Form != null && vm.Step1Form.Organizers.Any() && vm.Step1Form.OrganizersItems.IsNullOrEmpty())
            {
                vm.Step1Form.OrganizersItems = vm.Step1Form.Organizers.Select(x => new VmTreeItem() { Id = x }).ToList();
            }

            PrefilterViewModel(vm);
            SetTranslatorLanguage(vm);
            var service = TranslationManagerToEntity.Translate<VmService, ServiceVersioned>(vm, unitOfWork);
            serviceRep.Add(service);
            return service;
        }

        public VmPublishingResultModel PublishService(VmPublishingModel model)
        {
            var serviceId = model.Id;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var service = serviceRep.All().Include(x => x.OrganizationServices).First(x => x.Id == serviceId);

                if (service.OrganizationServices.All(x => x.RoleTypeId != typesCache.Get<RoleType>(RoleTypeEnum.Producer.ToString())))
                {
                    // TODO: this check should be extended once we will check all mandatory fields
                    throw new PtvAppException("Mandatory fields are not filled, please check all service steps.", messageServicePublishFailed);
                }
            });

            var affected = commonService.PublishEntity<ServiceVersioned, ServiceLanguageAvailability>(model);
            var result = new VmPublishingResultModel
            {
                Id = serviceId,
                PublishingStatusId = affected.AffectedEntities.First(i => i.Id == serviceId).PublishingStatusNew,
                LanguagesAvailabilities = model.LanguagesAvailabilities,
                Version = affected.Version
            };

            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            if (result.PublishingStatusId.HasValue && result.PublishingStatusId.Value == psPublished)
            {
                HandleChannelConnectionsWhenPublished(serviceId);
            };




            FillEnumEntities(result, () => GetEnumEntityCollectionModel("Services", affected.AffectedEntities.Select(i => new VmEntityStatusBase() { Id = i.Id, PublishingStatusId = i.PublishingStatusNew }).ToList<IVmBase>()));
            return result;
        }

        private void HandleChannelConnectionsWhenPublished(Guid currentServiceVersionedId)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                var psNotCommon = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.NotCommon.ToString());

                var serviceServiceChannelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
                var serviceVersioned = unitOfWork.CreateRepository<IServiceVersionedRepository>()
                    .All()
                    .Where(sv => sv.Id == currentServiceVersionedId)
                    .Include(i => i.OrganizationServices)
                    .SingleOrDefault();

                if (serviceVersioned == null) return;

                var serviceServiceChannels = serviceServiceChannelRep
                    .All()
                    .Where(s => s.ServiceId == serviceVersioned.UnificRootId)
                    .Include(i => i.ServiceChannel)
                    .ThenInclude(i => i.Versions);

                var channelsToRemove = new List<Guid>();
                var serviceOrganizations = serviceVersioned.OrganizationServices.Select(o => o.OrganizationId).Distinct().ToList();
                foreach (var ssch in serviceServiceChannels)
                {
                    var channel = ssch.ServiceChannel.Versions.SingleOrDefault(ch => ch.PublishingStatusId == psPublished && ch.ConnectionTypeId == psNotCommon);
                    if (channel == null) continue;

                    if (serviceOrganizations.Contains(channel.OrganizationId)) continue;
                    channelsToRemove.Add(ssch.ServiceChannelId);
                }

                var toRemove = serviceServiceChannels.Where(s =>  channelsToRemove.Contains(s.ServiceChannelId) && s.ServiceId == serviceVersioned.UnificRootId);
                serviceServiceChannelRep.Remove(toRemove);
                unitOfWork.Save();
            });
        }

        public VmPublishingResultModel WithdrawService(VmPublishingModel model)
        {
            return commonService.WithdrawEntity<ServiceVersioned>(model.Id);
        }

        public VmPublishingResultModel RestoreService(VmPublishingModel model)
        {
            return commonService.RestoreEntity<ServiceVersioned>(model.Id);
        }

        public IVmEntityBase GetServiceLanguagesAvailabilities(Guid serviceId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceLangAvailRep = unitOfWork.CreateRepository<IServiceLanguageAvailabilityRepository>();
                var langaugeAvailabilities = serviceLangAvailRep.All().Where(x => x.ServiceVersionedId == serviceId).ToList();
                return new VmEntityLanguageAvailable() { Id = serviceId, LanguagesAvailability = langaugeAvailabilities.ToDictionary(i => i.LanguageId, i => i.StatusId) };
            });
        }

//        public IVmListItemsData<VmEntityStatusBase> PublishServices(List<Guid> serviceIds)
//        {
//            var result = new List<PublishingResult>();
//            contextManager.ExecuteWriter(unitOfWork =>
//            {
//                foreach (var id in serviceIds)
//                {
//                    var service = PublishService(unitOfWork, id);
//                    result.AddRange(service);
//                }
//                unitOfWork.Save();
//            });
//            return new VmListItemsData<VmEntityStatusBase>(result.Select(i => new VmEntityStatusBase() { Id = i.Id, PublishingStatus = i.PublishingStatusNew }).ToList());
//        }

        public IVmEntityBase DeleteService(Guid serviceId)
        {
            ServiceVersioned result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = DeleteService(unitOfWork, serviceId);
                unitOfWork.Save();

            });
            return new VmEntityStatusBase { Id = result.Id, PublishingStatusId = result.PublishingStatusId };
        }

        public IVmEntityBase GetServiceStatus(Guid serviceId)
        {
            VmPublishingStatus result = null;
            if (serviceId.IsAssigned())
            {
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    result = GetServiceStatus(unitOfWork, serviceId);
                });
            }
            return new VmEntityStatusBase { PublishingStatusId = result?.Id };
        }

        private VmPublishingStatus GetServiceStatus(IUnitOfWorkWritable unitOfWork, Guid? serviceId)
        {
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var service = serviceRep.All()
                            .Include(x=>x.PublishingStatus)
                            .Single(x => x.Id == serviceId.Value);

            return TranslationManagerToVm.Translate<PublishingStatusType, VmPublishingStatus>(service.PublishingStatus);
        }

        private ServiceVersioned DeleteService(IUnitOfWorkWritable unitOfWork, Guid? serviceId)
        {
            var publishStatus = TranslationManagerToEntity.Translate<String, PublishingStatusType>(PublishingStatus.Deleted.ToString(), unitOfWork);

            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var service = serviceRep.All().Single(x => x.Id == serviceId.Value);
            service.PublishingStatus = publishStatus;

            return service;
        }

        public IVmServiceStep1 SaveStep1Changes(Guid serviceId, VmServiceStep1 model)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SaveStep1Changes(unitOfWork, ref serviceId, model);
            });
            return GetServiceStep1(new VmGetServiceStep() { Id = serviceId, Language = model.Language });
        }

        public bool CheckProducersChange(Guid serviceId, VmServiceStep1 model)
        {
            var result = false;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var orgServiceRep = unitOfWork.CreateRepository<IOrganizationServiceRepository>();
                var selfProducersIds = orgServiceRep.All()
                            .Where(x => x.ServiceVersionedId == serviceId &&
                                   x.RoleTypeId == typesCache.Get<RoleType>(RoleTypeEnum.Producer.ToString()) &&
                                   x.ProvisionTypeId == typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString()))
                            .Select(x => x.OrganizationId);
                selfProducersIds.ForEach(selfId => 
                {
                    if (!model.Organizers.Contains(selfId.Value))
                    {
                        result = true;
                    }
                });
            });
            return result;
        }

        private void UpdateStep1Model(IUnitOfWorkWritable unitOfWork, VmServiceStep1 model)
        {
//            model.Laws = model.Laws.Where(i => i.UrlAddress != null && !i.UrlAddress.Values.First().Trim().IsNullOrEmpty()).ToList();
            // model.ServiceTypeCode = unitOfWork.CreateRepository<IServiceTypeRepository>().All().FirstOrDefault(x => x.Id == model.ServiceType)?.Code;
        }

        private void SaveStep1Changes(IUnitOfWorkWritable unitOfWork, ref Guid serviceId, VmServiceStep1 model)
        {
            UpdateStep1Model(unitOfWork, model);

            SetTranslatorLanguage(model);
            ProcessOrganizers(unitOfWork, model, serviceId);

            var previousServiceId = serviceId;
            var service = TranslationManagerToEntity.Translate<VmService, ServiceVersioned>(new VmService() { Step1Form = model , Id = serviceId}, unitOfWork);
            serviceId = service.Id;

            var lawIds = model.Laws.Select(i => i.Id).ToList();
            var requestLanguageId = typesCache.Get<Language>((model?.Language ?? LanguageCode.fi).ToString());
            if (previousServiceId == serviceId)
            {
                var lawRep = unitOfWork.CreateRepository<ILawRepository>();
                var lawsToDelete = unitOfWork.ApplyIncludes(lawRep.All(), q => q.Include(i => i.WebPages).ThenInclude(j => j.WebPage)).Where(i => lawIds.Contains(i.Id)).ToList();
                var webPagesToDelete = lawsToDelete.SelectMany(i => i.WebPages.Select(j => j.WebPage)).Where(i => i.LocalizationId == requestLanguageId).ToList();
                var lawWebPageToDelete = lawsToDelete.SelectMany(i => i.WebPages).Where(i => webPagesToDelete.Select(j => j.Id).Contains(i.WebPageId)).ToList();

                var lawWebPageRep = unitOfWork.CreateRepository<ILawWebPageRepository>();
                var webPageRep = unitOfWork.CreateRepository<IWebPageRepository>();
                lawWebPageToDelete.ForEach(i => lawWebPageRep.Remove(i));
                webPagesToDelete.ForEach(i => webPageRep.Remove(i));

                //Removing laws
                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                    service.ServiceLaws.Select(x => x.Law).ToList(),
                    curr => curr.ServiceLaws.Any(x => x.ServiceVersionedId == service.Id));

                //Removing Organizers
                var orgServiceRep = unitOfWork.CreateRepository<IOrganizationServiceRepository>();
                var organizersIds = service.OrganizationServices.Select(x => x.OrganizationId).ToList();
                var organizerToRemove = orgServiceRep.All()
                    .Where(x => x.ServiceVersionedId == service.Id &&
                        x.RoleTypeId == typesCache.Get<RoleType>(RoleTypeEnum.Responsible.ToString()) && !organizersIds.Contains(x.OrganizationId)).ToList();
                organizerToRemove.ForEach(i => orgServiceRep.Remove(i));

                service.ServiceLanguages = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceLanguages, query => query.ServiceVersionedId == service.Id, language => language.LanguageId);

                service.AreaMunicipalities = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.AreaMunicipalities,
                    query => query.ServiceVersionedId == service.Id,
                    areaMunicipality => areaMunicipality.MunicipalityId);

                service.Areas = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.Areas,
                    query => query.ServiceVersionedId == service.Id,
                    area => area.AreaId);

                // removing of service vouchers (web pages)
                var serviceWebPageRep = unitOfWork.CreateRepository<IServiceWebPageRepository>();
                var webPageIds = service.ServiceWebPages.Select(x => x.WebPageId).ToList();
                var webPagesToRemove = serviceWebPageRep.All().Where(x => x.WebPage.Localization.Code == model.Language.ToString() && x.ServiceVersionedId == service.Id && !webPageIds.Contains(x.WebPageId));
                webPagesToRemove.ForEach(x => serviceWebPageRep.Remove(x));
            }

            //Update override target groups
            UpdateOverrideTargetGroups(service, unitOfWork);

            //Update service producers according organizers
            UpdateProducers(service);

            unitOfWork.Save(parentEntity: service);
        }

        /// <summary>
        /// Update service producers according organizers
        /// </summary>
        /// <param name="service"></param>
        /// <param name="unitOfWork"></param>
        private void UpdateOverrideTargetGroups(ServiceVersioned service, IUnitOfWorkWritable unitOfWork)
        {
            var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            var generalDescription = generalDescriptionRep.All().Include(x => x.TargetGroups).FirstOrDefault(x => x.Id == service.StatutoryServiceGeneralDescriptionId);
            var gdTargetGroups = generalDescription?.TargetGroups.Select(x => x.TargetGroupId);
            var serviceTargetGroupRep = unitOfWork.CreateRepository<IServiceTargetGroupRepository>();
            var overrirdeTaregtGroupToRemove = serviceTargetGroupRep.All().Include(x=>x.TargetGroup).ThenInclude(x=>x.Children).Where(x => x.ServiceVersionedId == service.Id && x.Override && !gdTargetGroups.Contains(x.TargetGroupId));
            //var overrideChildrenTargetGroupToRemoveIds = overrirdeTaregtGroupToRemove.SelectMany(x => x.TargetGroup.Children).Select(x=>x.Id);
            serviceTargetGroupRep.Remove(overrirdeTaregtGroupToRemove);
            //overrirdeTaregtGroupToRemove = serviceTargetGroupRep.All().Where(x => x.ServiceId == service.Id && overrideChildrenTargetGroupToRemoveIds.Contains(x.TargetGroupId));
            //serviceTargetGroupRep.Remove(overrirdeTaregtGroupToRemove);
        }

        /// <summary>
        /// Update All self producers to purchase producer if not used in organizers
        /// </summary>
        /// <param name="service">Service</param>
        private void UpdateProducers(ServiceVersioned service)
        {
            var organizationIds = service.OrganizationServices.Where(x => x.RoleTypeId == typesCache.Get<RoleType>(RoleTypeEnum.Responsible.ToString())).Select(x => x.OrganizationId);

            service.OrganizationServices = service.OrganizationServices.Where(x =>
                x.RoleTypeId == typesCache.Get<RoleType>(RoleTypeEnum.Responsible.ToString()) ||
                (x.RoleTypeId == typesCache.Get<RoleType>(RoleTypeEnum.Producer.ToString()) && x.ProvisionTypeId != typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString())) ||
                (x.RoleTypeId == typesCache.Get<RoleType>(RoleTypeEnum.Producer.ToString()) && x.ProvisionTypeId == typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString()) && organizationIds.Contains(x.OrganizationId))
                ).ToList();
            //if (!service.OrganizationServices.Any(x => x.RoleTypeId == typesCache.Get<RoleType>(RoleTypeEnum.Producer.ToString())))
            //{
            //    throw new PtvAppException("Can not remove organization used as self producer", messageOrganizersFailed);
            //}
        }

        public IVmServiceStep2 SaveStep2Changes(Guid serviceId, VmServiceStep2 model)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SaveStep2Changes(unitOfWork, ref serviceId, model);
            });
            return GetServiceStep2(new VmGetServiceStep() { Id = serviceId, Language = model.Language });
        }
        private void SaveStep2Changes(IUnitOfWorkWritable unitOfWork, ref Guid serviceId, VmServiceStep2 model)
        {
            var vmService = new VmService() { Step2Form = model, Id = serviceId };
            PrefilterViewModel(vmService);
            SetTranslatorLanguage(model);
            var service = TranslationManagerToEntity.Translate<VmService, ServiceVersioned>(vmService, unitOfWork);
            if (serviceId == service.Id)
            {
                var serviceIdLocal = service.Id;
                service.ServiceTargetGroups = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceTargetGroups,
                    query => query.ServiceVersionedId == serviceIdLocal,
                    targetGroup => targetGroup.TargetGroupId,
                    null,
                    (entity, newEntity) => entity.Override = newEntity.Override);
                service.ServiceLifeEvents = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceLifeEvents,
                    query => query.ServiceVersionedId == serviceIdLocal,
                    targetGroup => targetGroup.LifeEventId);
                service.ServiceServiceClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceServiceClasses,
                    query => query.ServiceVersionedId == serviceIdLocal,
                    targetGroup => targetGroup.ServiceClassId);
                service.ServiceOntologyTerms = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceOntologyTerms,
                    query => query.ServiceVersionedId == serviceIdLocal,
                    targetGroup => targetGroup.OntologyTermId);
                service.ServiceKeywords = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceKeywords,
                    query => query.ServiceVersionedId == serviceIdLocal && query.Keyword.Localization.Code == model.Language.ToString(),
                    keyWord => keyWord.KeywordId,
                    query => query.ServiceVersionedId == serviceIdLocal && query.Keyword.Localization.Code != model.Language.ToString());
                service.ServiceIndustrialClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceIndustrialClasses,
                    query => query.ServiceVersionedId == serviceIdLocal,
                    targetGroup => targetGroup.IndustrialClassId);
            }
            serviceId = service.Id;
            unitOfWork.Save(parentEntity: service);
        }

        public IVmServiceStep3 SaveStep3Changes(Guid serviceId, VmServiceStep3 model)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SaveStep3Changes(unitOfWork, ref serviceId, model);
            });
            return GetServiceStep3(new VmGetServiceStep() { Id = serviceId, Language = model.Language });
        }

        private void SaveStep3Changes(IUnitOfWorkWritable unitOfWork, ref Guid serviceId, VmServiceStep3 model)
        {
            var vmService = new VmService() { Step3Form = model, Id = serviceId };
            PrefilterViewModel(vmService);

            SetTranslatorLanguage(model);
            var service = TranslationManagerToEntity.Translate<VmService, ServiceVersioned>(vmService, unitOfWork);
            if (serviceId == service.Id)
            {
                //Removing Organizers
                var orgServiceRep = unitOfWork.CreateRepository<IOrganizationServiceRepository>();
                var organizersIds = service.OrganizationServices.Select(x => x.OrganizationId).ToList();
                var orgServicesIds = service.OrganizationServices.Select(x => x.Id).ToList();
                var organizerToRemove = orgServiceRep.All()
                    .Where(x => !orgServicesIds.Contains(x.Id) && x.ServiceVersionedId == service.Id &&
                                x.RoleTypeId == typesCache.Get<RoleType>(RoleTypeEnum.Producer.ToString()) &&
                                !organizersIds.Contains(x.OrganizationId)).ToList();
                organizerToRemove.ForEach(i => orgServiceRep.Remove(i));

                // service organizers has to be loaded for role checker, because responsible organization is needed for roles
                var copyOfOrganizers = service.OrganizationServices.ToList();
                unitOfWork.LoadNavigationProperty(service, x => x.OrganizationServices);
                service.OrganizationServices = service.OrganizationServices.Except(copyOfOrganizers).Union(copyOfOrganizers).ToList();
            }

            serviceId = service.Id;

            unitOfWork.Save(parentEntity: service);
        }

        private void ProcessOrganizers(IUnitOfWorkWritable unitOfWork, IVmServiceProducer model, Guid serviceId)
        {
            ownerReferenceLogic.SetOwnerReference(model.ServiceProducers, serviceId);
            var serviceIdLocal = serviceId;
            var organizationServiceRepository = unitOfWork.CreateRepository<IOrganizationServiceRepository>();
            var organizers = organizationServiceRepository.All().Where(x => x.ServiceVersionedId == serviceIdLocal && x.RoleType.Code == RoleTypeEnum.Responsible.ToString()).GroupBy(x => x.OrganizationId).ToDictionary(x => x.Key);
            model.OrganizersItems = model.Organizers.Select(x =>
            {
                IGrouping<Guid?, Model.Models.OrganizationService> entity;
                var item = new VmTreeItem { Id = x };
                if (organizers.TryGetValue(x, out entity))
                {
                    item.OwnerReferenceId = entity.First().Id;
                }
                return item;
            }).ToList();
            //var organizationIds = model.OrganizersItems.Select(x => x.Id).ToList();
            //if (model.ServiceProducers.Any(x =>
            //         x.ProvisionTypeId == typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString()) &&
            //         !organizationIds.Contains(x.SelfProduced.ProducerOrganizationId.Value)))
            //{
            //    throw new PtvAppException("Can not remove organization used as self producer", messageOrganizersFailed);
            //}
        }


        public IVmServiceStep4ChannelData SaveStep4Changes(Guid serviceId, List<Guid> model)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SaveStep4Changes(unitOfWork, ref serviceId, model);
            });
            return GetServiceStep4Channeldata(new VmGetServiceStep() { Id = serviceId });
        }
        private void SaveStep4Changes(IUnitOfWorkWritable unitOfWork, ref Guid serviceId, List<Guid> model)
        {
            var service = TranslationManagerToEntity.Translate<VmService, ServiceVersioned>(new VmService() { Id = serviceId, Step4Form = model }, unitOfWork);
            serviceId = service.Id;
            service.UnificRoot.ServiceServiceChannels = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.UnificRoot.ServiceServiceChannels, query => query.ServiceId == service.Id, channel => channel.ServiceChannelId);
            unitOfWork.Save(parentEntity: service);
        }

        private void PrefilterViewModel(VmService vm)
        {
            if (vm.Step1Form != null)
            {
                logic.UpdateOrganizationSelectionForSelfProducer(vm.Step1Form, typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString()));
            }
            if (vm.Step3Form != null)
            {
                logic.UpdateOrganizationSelectionForSelfProducer(vm.Step3Form, typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString()));
            }
        }

        public IVmEntityBase LockService(Guid id)
        {
            return utilities.LockEntityVersioned<ServiceVersioned, Service>(id);
        }

        public IVmEntityBase UnLockService(Guid id)
        {
            return utilities.UnLockEntityVersioned<ServiceVersioned, Service>(id);
        }

        public IVmEntityBase IsServiceLocked(Guid id)
        {
            return utilities.CheckIsEntityLocked<ServiceVersioned, Service>(id);
        }

        public IVmEntityBase IsServiceEditable(Guid id)
        {
            return utilities.CheckIsEntityEditable<ServiceVersioned, Service>(id);
        }

        /// <summary>
        /// Get area information for service from organization
        /// </summary>
        /// <param name="model">IVmGetAreaInformation</param>
        /// <returns>IVmAreaInformation</returns>
        public IVmAreaInformation GetServiceAreaInformation(IVmGetAreaInformation model)
        {
            var result = new VmAreaInformation();
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetServiceAreaInformation(model.OrganizationId, unitOfWork);
            });
            result.Id = model.ServiceId;
            return result;
        }

        public VmServiceSearchResult RelationSearchServices(IVmServiceSearch vmServiceSearch)
        {
            vmServiceSearch.Name = vmServiceSearch.Name != null ? vmServiceSearch.Name.Trim() : vmServiceSearch.Name;
            IReadOnlyList<IVmServiceListItem> result = new List<IVmServiceListItem>();
            bool moreAvailable = false;
            var count = 0;

            contextManager.ExecuteReader(unitOfWork =>
            {
                var languageCode = SetTranslatorLanguage(vmServiceSearch);
                //var languagesIds = vmServiceSearch.Languages.Select(language => languageCache.Get(language.ToString())).ToList();
                var publishedStatusId = PublishingStatusCache.Get(PublishingStatus.Published);
                var languageId = languageCache.Get(languageCode);
                var publishingStatusPublishedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                var publishingStatusDraftdId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
                var publishingStatusModifiedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());

                var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var resultTemp = serviceRep.All().Where(i => i.PublishingStatusId == publishingStatusPublishedId
                                                             || i.PublishingStatusId == publishingStatusDraftdId
                                                             || i.PublishingStatusId == publishingStatusModifiedId);

                #region SearchByFilterParam

                if (vmServiceSearch.ServiceClassId.IsAssigned())
                {
                    var generalDescIds = generalDescriptionRep.All()
                        .Where(x => x.ServiceClasses.Any(s => s.ServiceClassId == vmServiceSearch.ServiceClassId) &&
                                    x.PublishingStatusId == publishedStatusId)
                        .Select(x => x.Id);

                    resultTemp = resultTemp.Where(
                        x => x.ServiceServiceClasses.Any(s => s.ServiceClassId == vmServiceSearch.ServiceClassId.Value) ||
                             generalDescIds.Any(d => d == x.StatutoryServiceGeneralDescriptionId)
                    );
                }

                if (vmServiceSearch.OrganizationId.IsAssigned())
                {
                    var responsibleRoleTypeId = typesCache.Get<RoleType>(RoleTypeEnum.Responsible.ToString());
                    var orgnizationId = vmServiceSearch.OrganizationId;
                    resultTemp = resultTemp.Where(x => x.OrganizationServices.Any(o => o.RoleTypeId == responsibleRoleTypeId && o.OrganizationId == orgnizationId));
                }

                if (!string.IsNullOrEmpty(vmServiceSearch.Name))
                {
                    var rootId = GetRootIdFromString(vmServiceSearch.Name);
                    if (!rootId.HasValue)
                    {
                        var searchText = vmServiceSearch.Name.ToLower();
                        resultTemp =
                            resultTemp.Where(
                                x => x.ServiceNames.Any(y => y.Name.ToLower().Contains(searchText) &&
                                                             y.LocalizationId == languageId));
                    }
                    else
                    {
                        var rootIdGuid = rootId.Value;
                        resultTemp = resultTemp
                            .Where(service =>
                                service.UnificRootId == rootIdGuid
                            );
                    }
                }
                else
                {

                    resultTemp =
                        resultTemp.Where(
                            x => x.ServiceNames.Any(y => y.LocalizationId == languageId && y.Name != null));
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


                if (vmServiceSearch.ServiceTypeId.IsAssigned())
                {
                    var serviceTypeId = vmServiceSearch.ServiceTypeId;
                    var generalDescIds = generalDescriptionRep.All()
                        .Where(x => x.TypeId == vmServiceSearch.ServiceTypeId &&
                                    x.PublishingStatusId == publishedStatusId)
                        .Select(x => x.UnificRootId);

                    resultTemp = resultTemp.Where(x => (x.TypeId == serviceTypeId && x.StatutoryServiceGeneralDescriptionId == null) || generalDescIds.Any(d => d == x.StatutoryServiceGeneralDescriptionId));
                }

                #endregion SearchByFilterParam

                var resultFromDbIds = resultTemp.Select(i => i.UnificRootId).Distinct().ApplyPaging(vmServiceSearch.PageNumber);
                var resultData = serviceRep.All().Where(i => resultFromDbIds.Data.Contains(i.UnificRootId));

                resultData = unitOfWork.ApplyIncludes(resultData, i => i
                    .Include(j => j.UnificRoot)
                    .ThenInclude(j => j.ServiceServiceChannels)
                    .ThenInclude(j => j.ServiceChannel)
                    .ThenInclude(j => j.Versions)
                );

                var resultFromDb = resultData.ToList().GroupBy(i => i.UnificRootId).Select(x => x.OrderBy(y => y.PublishingStatusId == publishingStatusPublishedId ? 0 : y.PublishingStatusId == publishingStatusDraftdId ? 1 : y.PublishingStatusId == publishingStatusModifiedId ? 2 : 3).FirstOrDefault()).ToList(); 
                var servicesIds = resultFromDb.Select(i => i.Id).ToList();
                var serviceName = unitOfWork.CreateRepository<IServiceNameRepository>().All().Where(i => servicesIds.Contains(i.ServiceVersionedId)).ToList().GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.ToList());
                resultFromDb.ForEach(c =>
                {
                    c.ServiceNames = serviceName.TryGet(c.Id);
                });

                var allChannels = resultFromDb.SelectMany(i => i.UnificRoot.ServiceServiceChannels).Select(i => i.ServiceChannel).SelectMany(i => i.Versions).Where(i => i.PublishingStatusId == publishingStatusPublishedId
                                                                                                                                                                          || i.PublishingStatusId == publishingStatusDraftdId
                                                                                                                                                                          || i.PublishingStatusId == publishingStatusModifiedId).ToList();
                var channelsIds = allChannels.Select(i => i.Id).ToList();
                var channelNames = unitOfWork.CreateRepository<IServiceChannelNameRepository>().All().Where(i => channelsIds.Contains(i.ServiceChannelVersionedId)).ToList().GroupBy(i => i.ServiceChannelVersionedId).ToDictionary(i => i.Key, i => i.ToList());
                allChannels.ForEach(c =>
                {
                    c.ServiceChannelNames = channelNames.TryGet(c.Id);
                });

                moreAvailable = resultFromDbIds.MoreAvailable;

                result = TranslationManagerToVm.TranslateAll<ServiceVersioned, VmServiceRelationListItem>(resultFromDb);
            });

            return new VmServiceSearchResult()
            {
                Services = result,
                PageNumber = ++vmServiceSearch.PageNumber,
                MoreAvailable = moreAvailable,
                Count = count
            };
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

        private void EnsureOrganization(List<Guid> ids)
        {
            ids.ForEach(id =>
            {
                if (!commonService.OrganizationExists(id, PublishingStatus.Published))
                {
                    throw new PtvAppException("Published organization does not exist!", publishedOrganizationDoesNotExist);
                }
            });
        }

        private VmAreaInformation GetServiceAreaInformation(Guid organizationId, IUnitOfWork unitOfWork)
        {
            var result = new VmAreaInformation();

            var orgId = versioningManager.GetVersionId<OrganizationVersioned>(unitOfWork, organizationId);
            if (orgId.HasValue)
            {
                var organization = GetEntity<OrganizationVersioned>(orgId, unitOfWork,
                   q => q
                        .Include(x => x.OrganizationAreaMunicipalities)
                        .Include(x => x.OrganizationAreas).ThenInclude(x => x.Area)
                        );

                result = TranslationManagerToVm.Translate<OrganizationVersioned, VmAreaInformation>(organization);
            }
            return result;
        }

        #region Open Api

        public IVmOpenApiGuidPageVersionBase GetServices(DateTime? date, int pageNumber = 1, int pageSize = 1000, bool archived = false)
        {
            return GetServices(new V3VmOpenApiGuidPage(pageNumber, pageSize), date, archived);
        }

        public IVmOpenApiGuidPageVersionBase GetServices(IVmOpenApiGuidPageVersionBase vm, DateTime? date, bool archived = false)
        {
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetItemPage<ServiceVersioned, Service, ServiceLanguageAvailability>(vm, date, unitOfWork, q => q.Include(i => i.ServiceNames), archived);
            });

            return vm;
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
            contextManager.ExecuteReader(unitOfWork =>
            {
                var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
                var services = new List<ServiceVersioned>();

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
                var rep = unitOfWork.CreateRepository<IServiceServiceClassRepository>();
                var query = rep.All().Where(s => s.ServiceClassId.Equals(serviceClassId));
                var serviceIdList = rep.All().Where(s => s.ServiceClassId.Equals(serviceClassId)).ToList().Select(s => s.ServiceVersionedId).ToList();

                // Get published services
                if (gdIdList?.Count > 0 || serviceIdList?.Count > 0)
                {
                    // Get services that are attached to one of the general desriptions or that are related to given service class.
                    var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    var serviceQuery = serviceRep.All().Where(s => ((s.StatutoryServiceGeneralDescriptionId != null && gdIdList.Contains(s.StatutoryServiceGeneralDescriptionId.Value)) || serviceIdList.Contains(s.Id)) &&
                        s.PublishingStatusId == publishedId && s.LanguageAvailabilities.Any(l => l.StatusId == publishedId));
                    if (date.HasValue)
                    {
                        serviceQuery = serviceQuery.Where(g => g.Modified > date.Value);
                    }
                    services = unitOfWork.ApplyIncludes(serviceQuery, q =>
                        q.Include(i => i.ServiceNames)
                        ).ToList();
                }

                SetItemPage(vm, services.AsQueryable());
            });

            return vm;
        }

        public IVmOpenApiServiceVersionBase GetServiceById(Guid id, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiServiceVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetServiceById(unitOfWork, id, openApiVersion, getOnlyPublished);
            });

            return result;
        }

        private IVmOpenApiServiceVersionBase GetServiceById(IUnitOfWork unitOfWork, Guid id, int openApiVersion, bool getOnlyPublished = true)
        {
            try
            {
                // Get the right version id
                Guid? entityId = null;
                if (getOnlyPublished)
                {
                    entityId = versioningManager.GetVersionId<ServiceVersioned>(unitOfWork, id, PublishingStatus.Published);
                }
                else
                {
                    entityId = versioningManager.GetVersionId<ServiceVersioned>(unitOfWork, id);
                }
                if (entityId.HasValue)
                {
                    return GetServiceWithDetails(unitOfWork, entityId.Value, openApiVersion, getOnlyPublished);
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
                    var serviceVersionedRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    var publishedServices = serviceVersionedRep.All().Where(s => serviceList.Contains(s.UnificRootId)).Where(PublishedFilter<ServiceVersioned>()).Where(ValidityFilter<ServiceVersioned>()).Select(s => s.Id).ToList();
                    result = GetServicesWithDetails(unitOfWork, publishedServices, openApiVersion);
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

        public IVmOpenApiServiceVersionBase GetServiceBySource(string sourceId, int openApiVersion, bool getOnlyPublished = true, string userName = null)
        {
            IVmOpenApiServiceVersionBase result = new VmOpenApiServiceVersionBase();
            var userId = userName ?? utilities.GetRelationIdForExternalSource();
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var rootId = GetPTVId<Service>(sourceId, userId, unitOfWork);
                    result = GetServiceById(unitOfWork, rootId, openApiVersion, getOnlyPublished);

                });
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting services by source id {0}. {1}", sourceId, ex.Message);
                logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
            return result;
        }

        public PublishingStatus? GetServiceStatusByRootId(Guid id)
        {
            PublishingStatus? result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = versioningManager.GetLatestVersionPublishingStatus<ServiceVersioned>(unitOfWork, id);
            });

            return result;
        }

        public PublishingStatus? GeServiceStatusBySourceId(string sourceId)
        {
            if (string.IsNullOrEmpty(sourceId)) { throw new Exception(string.Format(CoreMessages.OpenApi.ExternalSourceNotExists, sourceId)); }
            PublishingStatus? result = null;
            bool externalSourceExists = false;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var id = GetPTVId<Service>(sourceId, utilities.GetRelationIdForExternalSource(), unitOfWork);
                if (id != Guid.Empty)
                {
                    externalSourceExists = true;
                    result = versioningManager.GetLatestVersionPublishingStatus<ServiceVersioned>(unitOfWork, id);
                }
            });
            if (!externalSourceExists) { throw new Exception(string.Format(CoreMessages.OpenApi.ExternalSourceNotExists, sourceId)); }
            return result;
        }

        public IVmOpenApiServiceBase AddService(IVmOpenApiServiceInVersionBase vm, bool allowAnonymous, int openApiVersion, string userName = null)
        {
            var service = new ServiceVersioned();
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = userName ?? utilities.GetRelationIdForExternalSource();
            var useOtherEndPoint = false;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Check if the external source already exists. Let's not throw the excpetion here to avoid contextManager to catch the exception.
                useOtherEndPoint = ExternalSourceExists<Service>(vm.SourceId, userId, unitOfWork);
                if (!useOtherEndPoint)
                {
                    CheckVm(vm, unitOfWork, true); // Includes checking general description data!
                    service = TranslationManagerToEntity.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(vm, unitOfWork);

                    var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    serviceRep.Add(service);

                    // Create the mapping between external source id and PTV id
                    if (!string.IsNullOrEmpty(vm.SourceId))
                    {
                        SetExternalSource(service.UnificRoot, vm.SourceId, userId, unitOfWork);
                    }

                    unitOfWork.Save(saveMode, userName: userName);
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

        public IList<string> AddChannelsForService(Guid serviceId, IList<Guid> channelIds, bool allowAnonymous, string userName = null)
        {
            var result = new List<string>();
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var channelCount = 0;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                var serviceRep = unitOfWork.CreateRepository<IServiceRepository>();
                var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                var service = serviceRep.All().FirstOrDefault(s => s.Id == serviceId);
                if (service == null)
                {
                    result.Add(string.Format(CoreMessages.OpenApi.EntityNotFound, "Service", serviceId));
                }
                else
                {
                    var existingServiceChannels = service.ServiceServiceChannels.Select(c => c.ServiceChannelId).ToList();
                    channelIds.ForEach(channelId =>
                    {
                        if (existingServiceChannels.Contains(channelId))
                        {
                            result.Add(string.Format(CoreMessages.OpenApi.RelationshipAlreadyExists, serviceId, channelId));
                        }
                        else
                        {
                            var channel = channelRep.All().Where(x => x.Id == channelId).Include(x => x.UnificRoot).FirstOrDefault();
                            if (channel == null)
                            {
                                result.Add(string.Format(CoreMessages.OpenApi.EntityNotFound, "Service channel", channelId));
                            }
                            else
                            {
                                service.ServiceServiceChannels.Add(new ServiceServiceChannel() { ServiceChannel = channel.UnificRoot });
                                channelCount++;
                            }
                        }
                    });
                    unitOfWork.Save(saveMode, userName: userName);
                    result.Add(string.Format(CoreMessages.OpenApi.ServiceChannelsAdded, channelCount, serviceId));
                }
            });

            return result;
        }

        public IVmOpenApiServiceBase SaveService(IVmOpenApiServiceInVersionBase vm, bool allowAnonymous, int openApiVersion, string sourceId = null, string userName = null)
        {
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = userName ?? utilities.GetRelationIdForExternalSource();
            IVmOpenApiServiceBase result = new VmOpenApiServiceBase();
            ServiceVersioned service = null;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Get the root id according to source id (if defined)
                var rootId = vm.Id ?? GetPTVId<Service>(sourceId, userId, unitOfWork);

                // Get right version id
                vm.Id = versioningManager.GetVersionId<ServiceVersioned>(unitOfWork, rootId);

                CheckVm(vm, unitOfWork);

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
                                    var updatedNames = updatedLaw.Names.Select(n => n.Id).ToList();
                                    var currentLaw = unitOfWork.ApplyIncludes(lawRep.All().Where(w => w.Id == l.LawId), q => q.Include(i => i.Names).Include(i => i.WebPages)).FirstOrDefault();
                                // Delete the web pages that were not included in updated webpages
                                currentLaw.WebPages.Where(w => !updatedWebPages.Contains(w.WebPageId)).ForEach(w => webPageRep.Remove(w.WebPage));
                                // Delete all names that were not included in updated names
                                currentLaw.Names.Where(n => !updatedNames.Contains(n.Id)).ForEach(n => lawNameRep.Remove(n));
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

                        if (vm.ServiceOrganizations?.Count > 0)
                        {
                            // Set the responsible organizations
                            var organizationServiceRepository = unitOfWork.CreateRepository<IOrganizationServiceRepository>();
                            var responsible = RoleTypeEnum.Responsible.ToString();
                            var newResponsibles = service.OrganizationServices.Where(o => o.RoleType.Code == responsible).ToList();
                            var responsibles = organizationServiceRepository.All().Where(x => x.ServiceVersionedId == vm.Id && x.RoleType.Code == responsible).GroupBy(x => x.Id).ToDictionary(x => x.Key);
                            responsibles.Values.SelectMany(x => x).Where(x => !newResponsibles.Select(o => o.Id).Contains(x.Id)).ForEach(x =>
                            {
                                organizationServiceRepository.Remove(x);
                            });

                            // Set the producers organizations
                            var producers = service.OrganizationServices.Where(x => x.RoleType != null && x.RoleType.Code == RoleTypeEnum.Producer.ToString()).ToList();
                            dataUtils.UpdateCollectionWithRemove(unitOfWork, producers,
                                query => query.ServiceVersionedId == service.Id && query.RoleType.Code == RoleTypeEnum.Producer.ToString());

                            // Delete items from collections that do not exist in vm
                            var infoRepository = unitOfWork.CreateRepository<IOrganizationServiceAdditionalInformationRepository>();
                            service.OrganizationServices.ForEach(r =>
                            {
                                // additinal information
                                var infos = r.AdditionalInformations.Select(i => i.Id).ToList();
                                var currentInfos = infoRepository.All().Where(i => i.OrganizationServiceId == r.Id);
                                var toRemove = currentInfos.Where(i => !infos.Contains(i.Id));
                                toRemove.ForEach(i => infoRepository.Remove(i));
                            });
                        }

                        if (vm.ServiceVouchers?.Count > 0)
                        {
                            var webPageRepository = unitOfWork.CreateRepository<IServiceWebPageRepository>();
                            var wpIds = service.ServiceWebPages.Select(x => x.WebPage.Id).ToList();
                            var webPagesToRemove = webPageRepository.All().Where(i => i.ServiceVersionedId == service.Id && !wpIds.Contains(i.WebPageId));
                            webPagesToRemove.ForEach(i => webPageRepository.Remove(i));

                        }
                    }

                    // Update the mapping between external source id and PTV id
                    if (!string.IsNullOrEmpty(vm.SourceId))
                    {
                        UpdateExternalSource<Service>(service.UnificRootId, vm.SourceId, userId, unitOfWork);
                    }
                }

                unitOfWork.Save(saveMode, service, userName);
            });

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


        private IVmOpenApiServiceVersionBase GetServiceWithDetails(IUnitOfWork unitOfWork, Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            return GetServicesWithDetails(unitOfWork, new List<Guid> { versionId }, openApiVersion, getOnlyPublished).FirstOrDefault();
        }

        private IVmOpenApiServiceVersionBase GetServiceWithDetails(Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiServiceVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetServicesWithDetails(unitOfWork, new List<Guid> { versionId }, openApiVersion, getOnlyPublished).FirstOrDefault();
            });
            return result;
        }

        private IList<IVmOpenApiServiceVersionBase> GetServicesWithDetails(IUnitOfWork unitOfWork, List<Guid> versionIdList, int openApiVersion, bool getOnlyPublished = true)
        {
            if (versionIdList.Count == 0) return new List<IVmOpenApiServiceVersionBase>();

            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var resultTemp = unitOfWork.ApplyIncludes(serviceRep.All().Where(s => versionIdList.Contains(s.Id)), q =>
                q.Include(i => i.ServiceLanguages).ThenInclude(i => i.Language)
                .Include(i => i.ServiceNames)
                .Include(i => i.ServiceDescriptions)
                .Include(i => i.ServiceServiceClasses).ThenInclude(i => i.ServiceClass).ThenInclude(i => i.Names)
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
                .Include(i => i.OrganizationServices).ThenInclude(i => i.Organization).ThenInclude(i => i.Versions)
                .Include(i => i.OrganizationServices).ThenInclude(i => i.AdditionalInformations)
                .Include(i => i.ServiceWebPages).ThenInclude(i => i.WebPage)
                .Include(i => i.PublishingStatus)
                .Include(i => i.LanguageAvailabilities)
                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaNames)
                .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .OrderByDescending(i => i.Modified));


            // Filter out items that do not have language versions published!
            var services = getOnlyPublished? resultTemp.Where(c => c.LanguageAvailabilities.Any(l => l.StatusId == publishedId)).ToList() : resultTemp.ToList();

            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var organizationVersionedRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();

            services.ForEach(service =>
            {
                // Filter out not published serviceChannels
                if (service.UnificRoot.ServiceServiceChannels.Count > 0)
                {
                    var channelRootIds = service.UnificRoot.ServiceServiceChannels.Select(c => c.ServiceChannelId).ToList();
                    var publishedServiceChannelRootIds = serviceChannelRep.All().Where(c => channelRootIds.Contains(c.UnificRootId))
                        .Where(PublishedFilter<ServiceChannelVersioned>())
                        .Where(s => s.LanguageAvailabilities.Any(l => l.StatusId == publishedId)) // Filter out channels with no language versions publihed
                        .Select(c => c.UnificRootId).ToList();
                    service.UnificRoot.ServiceServiceChannels = service.UnificRoot.ServiceServiceChannels.Where(c => publishedServiceChannelRootIds.Contains(c.ServiceChannelId)).ToList();
                }

                // Filter out not published organizations
                if (service.OrganizationServices.Count > 0)
                {
                    var organizationRootIds = service.OrganizationServices.Where(i => i.OrganizationId != null).Select(s => s.OrganizationId).ToList();
                    // Get the organizations that have published version available in db
                    var publishedOrganizationRootIds = organizationVersionedRep.All().Where(i => organizationRootIds.Contains(i.UnificRootId))
                        .Where(PublishedFilter<OrganizationVersioned>())
                        .Where(s => s.LanguageAvailabilities.Any(l => l.StatusId == publishedId)) // Filter out organizations with no language versions publihed
                        .Select(i => i.UnificRootId).ToList();
                    service.OrganizationServices = service.OrganizationServices.Where(i => i.OrganizationId == null || i.OrganizationId != null && publishedOrganizationRootIds.Contains(i.OrganizationId.Value)).ToList();
                }

                // Filter out not published language versions
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
                    service.OrganizationServices.ForEach(organization =>
                    {
                        organization.AdditionalInformations = organization.AdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    });
                    service.ServiceWebPages = service.ServiceWebPages.Where(i => !notPublishedLanguageVersions.Contains(i.WebPage.LocalizationId)).ToList();
                }
            });

            // Fill with service channel names
            var allChannels = services.SelectMany(i => i.UnificRoot.ServiceServiceChannels).Select(i => i.ServiceChannel)
                .SelectMany(i => i.Versions).Where(i => i.PublishingStatusId == publishedId).ToList();
            var channelsIds = allChannels.Select(i => i.Id).ToList();
            var channelNames = unitOfWork.CreateRepository<IServiceChannelNameRepository>().All().Where(i => channelsIds.Contains(i.ServiceChannelVersionedId)).ToList()
                .GroupBy(i => i.ServiceChannelVersionedId).ToDictionary(i => i.Key, i => i.ToList());
            allChannels.ForEach(c =>
            {
                c.ServiceChannelNames = channelNames.TryGet(c.Id);
            });

            // Fill with organization names
            var allOrganizations = services.SelectMany(i => i.OrganizationServices).Where(i => i.Organization != null).Select(i => i.Organization)
                .SelectMany(i => i.Versions).Where(i => i.PublishingStatusId == publishedId).ToList();
            var organizationIds = allOrganizations.Select(i => i.Id).ToList();
            var organizationNames = unitOfWork.CreateRepository<IOrganizationNameRepository>().All().Where(i => organizationIds.Contains(i.OrganizationVersionedId)).ToList()
                .GroupBy(i => i.OrganizationVersionedId).ToDictionary(i => i.Key, i => i.ToList());
            allOrganizations.ForEach(o =>
            {
                o.OrganizationNames = organizationNames.TryGet(o.Id);
            });
            
            var result = TranslationManagerToVm.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(services).ToList();
            if (result == null)
            {
                throw new Exception(CoreMessages.OpenApi.RecordNotFound);
            }

            // Get the right open api view model version
            var versionList = new List<IVmOpenApiServiceVersionBase>();
            result.ForEach(service =>
            {
                // Fill with the general description related data
                FillWithGeneralDescriptionData(service);
                versionList.Add(GetEntityByOpenApiVersion(service as IVmOpenApiServiceVersionBase, openApiVersion));
            });

            return versionList;
        }

        private Expression<Func<ServiceServiceChannel, bool>> ServiceServiceChannelPublishedFilter()
        {
            var now = DateTime.Now;
            var published = PublishingStatus.Published.ToString();
            return o => o.ServiceChannel.Versions.Any(i => i.PublishingStatus.Code == published &&
                    ((i.ValidFrom <= now && i.ValidTo >= now) ||
                    (i.ValidFrom == null && i.ValidTo == null)));
        }

        private void CheckVm(IVmOpenApiServiceInVersionBase vm, IUnitOfWorkWritable unitOfWork, bool createOperation = false)
        {
            CheckKeywords(vm, unitOfWork);
            CheckTargetGroups(vm.TargetGroups, unitOfWork);

            // Check general description related data.
            // In PUT method the view model may not include general description even the service has earlier been attached into a general description.
            // Therefore if the request viewmodel does not include general description id let's get the general description related to the service from db.
            var generalDescriptionID = vm.StatutoryServiceGeneralDescriptionId.ParseToGuid();
            if (!generalDescriptionID.IsAssigned() && vm.Id.IsAssigned())
            {
                // Let's try to get the statutory general description attached for service from db.
                var service = versioningManager.GetSpecificVersionByRoot<ServiceVersioned>(unitOfWork, vm.Id.Value, PublishingStatus.Published);//GetServicesWithDetails(new List<Expression<Func<ServiceVersioned, bool>>>() { s => s.Id.Equals(vm.Id.Value) }, unitOfWork, 0, false).FirstOrDefault();
                if (service != null)
                {
                    generalDescriptionID = service.StatutoryServiceGeneralDescriptionId;
                }
                else
                {
                    service = versioningManager.GetSpecificVersionByRoot<ServiceVersioned>(unitOfWork, vm.Id.Value, PublishingStatus.Draft);
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
            var fintoUriList = fintoItemList.Select(i => i.Uri).ToList();
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

        private void FillWithGeneralDescriptionData(VmOpenApiServiceVersionBase service)
        {
            // PTV-1667: Type (Tyyppi), Name (Nimi), Target Groups (Kohderyhmä), ServiceClass (Palveluluokka) and OntologyWords (Asiasanat) are filled from general description.
            // Name is always saved into db (copied from general description), so we do not need to fill it.
            if (!service.StatutoryServiceGeneralDescriptionId.IsAssigned())
            {
                return;
            }

            // Get the general description
            var generalDescription = generalDescriptionService.GetGeneralDescriptionVersionBase(service.StatutoryServiceGeneralDescriptionId.Value, 0);
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

                service.ServiceClasses = generalDescription.ServiceClasses.Union(service.ServiceClasses, new FintoItemComparer()).ToList();
                service.OntologyTerms = generalDescription.OntologyTerms.Union(service.OntologyTerms, new FintoItemComparer()).ToList();

                // Check service charge type.
                // If general description has charge type set, charge type for service has to be null! PTV-2347
                if (!string.IsNullOrEmpty(generalDescription.ServiceChargeType))
                {
                    service.ServiceChargeType = null;
                }
            }
        }

        class FintoItemComparer : IEqualityComparer<V4VmOpenApiFintoItem>
        {
            public bool Equals(V4VmOpenApiFintoItem x, V4VmOpenApiFintoItem y)
            {
                return x.Uri == y.Uri;
            }

            public int GetHashCode(V4VmOpenApiFintoItem obj)
            {
                return obj.Id.GetHashCode();
            }
        }
        #endregion
    }
}
