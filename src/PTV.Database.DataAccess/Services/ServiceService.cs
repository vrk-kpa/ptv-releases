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
using PTV.Database.Model.Models.Base;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models.OpenApi;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Utils;
using PTV.Domain.Logic.Services;
using PTV.Framework.Exceptions;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V1;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V1;
using PTV.Framework.Interfaces;

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
                    () => GetEnumEntityCollectionModel("ChargeTypes", chargeTypes)
                );
                result.SelectedPublishingStatuses = publishingStatuses.Where(x => x.Code != statusDeletedCode && x.Code != statusOldPublishedCode).Select(x => x.Id).ToList();

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

        private IVmServiceSearchResult FullTextSearchService(IVmServiceSearch vm)
        {
            var result = new List<IVmServiceListItem>();
            var skip = 0;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceNamesRepository = unitOfWork.CreateRepository<IServiceNameRepository>();
                var generalDescriptionRepository = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();

                var languagesIds = vm.Languages.Select(language => languageCache.Get(language.ToString())).ToList();
                var serviceNames = serviceNamesRepository.All();
                // Fulltext filter //
                if (!string.IsNullOrEmpty(vm.Name))
                {
                    var searchName = vm.Name.ToLower();
                    serviceNames = serviceNames
                        .Where(serviceName =>
                            serviceName.Name.ToLower().Contains(searchName) &&
                            languagesIds.Contains(serviceName.LocalizationId)
                        );
                }
                else
                {
                    serviceNames = serviceNames
                        .Where(serviceName =>
                            languagesIds.Contains(serviceName.LocalizationId)
                        );
                }
                serviceNames = unitOfWork.ApplyIncludes(serviceNames, q =>
                    q.Include(i => i.ServiceVersioned)
                        .ThenInclude(i => i.StatutoryServiceGeneralDescription)
                        .ThenInclude(i => i.Versions)
                        .ThenInclude(i => i.ServiceClasses)
                        .ThenInclude(i => i.ServiceClass)
                    .Include(i => i.ServiceVersioned)
                        .ThenInclude(i => i.StatutoryServiceGeneralDescription)
                        .ThenInclude(i => i.Versions)
                        .ThenInclude(i => i.OntologyTerms)
                        .ThenInclude(j => j.OntologyTerm)
                    .Include(i => i.ServiceVersioned)
                        .ThenInclude(i => i.StatutoryServiceGeneralDescription)
                        .ThenInclude(i => i.Versions)
                        .ThenInclude(i => i.Type)
                    .Include(i => i.ServiceVersioned)
                        .ThenInclude(i => i.LanguageAvailabilities)
                        .ThenInclude(i => i.Language)
                    .Include(i => i.ServiceVersioned)
                        .ThenInclude(i => i.Versioning)
                    .Include(i => i.ServiceVersioned)
                        .ThenInclude(i => i.ServiceServiceClasses)
                    .Include(i => i.ServiceVersioned)
                        .ThenInclude(i => i.ServiceOntologyTerms)
                        .ThenInclude(i => i.OntologyTerm)
                        .ThenInclude(i => i.Names)
                    .Include(i => i.ServiceVersioned)
                        .ThenInclude(i => i.ServiceServiceChannels)
                        .ThenInclude(i => i.ServiceChannel)
                        .ThenInclude(i => i.Versions)
                    .Include(i => i.ServiceVersioned)
                        .ThenInclude(i => i.OrganizationServices)
                );
                // Additional filters //
                if (vm.ServiceClassId.HasValue)
                {
                   var generalDescIds = generalDescriptionRepository.All().Where(x => x.ServiceClasses.Any(s => s.ServiceClassId == vm.ServiceClassId)).Select(x => x.Id);
                   serviceNames = serviceNames.Where(
                       x => x.ServiceVersioned.ServiceServiceClasses.Any(s => s.ServiceClassId == vm.ServiceClassId.Value) ||
                           generalDescIds.Any(d => d == x.ServiceVersioned.StatutoryServiceGeneralDescriptionId)
                       );
                }
                if (vm.OrganizationId.HasValue)
                {
                    serviceNames = serviceNames.Where(x => x.ServiceVersioned.OrganizationServices.Any(o => o.RoleTypeId == typesCache.Get<RoleType>(RoleTypeEnum.Responsible.ToString()) && o.OrganizationId == vm.OrganizationId));
                }
                if (vm.OntologyTerms.IsAssigned())
                {
                    var generalDescIds = generalDescriptionRepository.All().Where(x => x.OntologyTerms.Select(s => s.OntologyTermId).Contains(vm.OntologyTerms.Value)).Select(x => x.UnificRootId);
                    serviceNames = serviceNames.Where(
                        x => x.ServiceVersioned.ServiceOntologyTerms.Any(y => y.OntologyTermId == vm.OntologyTerms.Value) ||
                            generalDescIds.Any(d => d == x.ServiceVersioned.StatutoryServiceGeneralDescriptionId)
                        );
                }
                if (vm.ServiceTypeId.HasValue)
                {
                    var generalDescIds = generalDescriptionRepository.All().Where(x => x.TypeId == vm.ServiceTypeId).Select(x => x.Id);
                    serviceNames = serviceNames
                        .Where(x =>
                            (x.ServiceVersioned.TypeId == vm.ServiceTypeId.Value && x.ServiceVersioned.StatutoryServiceGeneralDescriptionId == null) ||
                            generalDescIds.Any(d => d == x.ServiceVersioned.StatutoryServiceGeneralDescriptionId)
                        );
                }
                if (vm.SelectedPublishingStatuses != null)
                {
                    commonService.ExtendPublishingStatusesDeletedOldPublished(vm.SelectedPublishingStatuses);
                    serviceNames = serviceNames.Where(x => vm.SelectedPublishingStatuses.Contains(x.ServiceVersioned.PublishingStatusId));
                }

                var names = serviceNames
                    .Skip(vm.Skip)
                    .Take(CoreConstants.MaximumNumberOfAllItems * vm.Languages.Count)
                    .ToList();
                var services = names
                    .Select(x => x.ServiceVersioned)
                    .GroupBy(x => x.Id)
                    .Select(x => x.First())
                    .Take(CoreConstants.MaximumNumberOfAllItems)
                    .ToList();

                services.ForEach(x => { x.ServiceNames = x.ServiceNames.Intersect(names).ToList(); });

                skip = services.Aggregate(0, (acc, service) => service.ServiceNames.Count + acc);

                result = TranslationManagerToVm
                    .TranslateAll<ServiceVersioned, VmServiceSearchListItem>(services)
                    .Cast<IVmServiceListItem>()
                    .ToList();
            });
            return new VmServiceSearchResult() {
                Services = result,
                Skip = skip
            };
        }
        private IVmServiceSearchResult FilteredSearchService(IVmServiceSearch vmServiceSearch)
        {
            IReadOnlyList<IVmServiceListItem> result = new List<IVmServiceListItem>();
            bool moreAvailable = false;
            //var count = 0;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var languageCode = SetTranslatorLanguage(vmServiceSearch);

                var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var resultTemp = unitOfWork.ApplyIncludes(serviceRep.All(), i => i
                    .Include(j => j.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.ServiceClasses).ThenInclude(j => j.ServiceClass)
                    .Include(j => j.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.OntologyTerms).ThenInclude(j => j.OntologyTerm)
                    .Include(j => j.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions)
                    .Include(j => j.LanguageAvailabilities).ThenInclude(j => j.Language)
                    .Include(j => j.Versioning)
                    .Include(j => j.OrganizationServices)
                );

                if (vmServiceSearch.ServiceClassId.HasValue)
                {
                    var generalDescIds = generalDescriptionRep.All().Where(x => x.ServiceClasses.Any(s => s.ServiceClassId == vmServiceSearch.ServiceClassId)).Select(x => x.Id);
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
                    var searchText = vmServiceSearch.Name.ToLower();
                    resultTemp =
                        resultTemp.Where(
                            x =>
                                x.ServiceNames.Any(
                                    y =>
                                        y.Name.ToLower().Contains(searchText) &&
                                        y.LocalizationId == languageCache.Get(languageCode.ToString())));
                }
                else
                {
                    resultTemp =
                        resultTemp.Where(
                            x =>
                                x.ServiceNames.Any(
                                    y =>
                                        y.LocalizationId == languageCache.Get(languageCode.ToString()) &&
                                        !string.IsNullOrEmpty(y.Name)));
                }

                if (vmServiceSearch.OntologyTerms.IsAssigned())
                {
                    var generalDescIds = generalDescriptionRep.All().Where(x => x.OntologyTerms.Select(s => s.OntologyTermId).Contains(vmServiceSearch.OntologyTerms.Value)).Select(x => x.UnificRootId);
                    resultTemp = resultTemp.Where(
                        x => x.ServiceOntologyTerms.Any(y => y.OntologyTermId == vmServiceSearch.OntologyTerms.Value) ||
                            generalDescIds.Any(d => d == x.StatutoryServiceGeneralDescriptionId)
                        );
                }

                if (vmServiceSearch.ServiceTypeId.HasValue)
                {
                    var generalDescIds = generalDescriptionRep.All().Where(x => x.TypeId == vmServiceSearch.ServiceTypeId).Select(x => x.UnificRootId);

                    resultTemp = resultTemp.Where(x => (x.TypeId == vmServiceSearch.ServiceTypeId.Value && x.StatutoryServiceGeneralDescriptionId == null) || generalDescIds.Any(d => d == x.StatutoryServiceGeneralDescriptionId));
                }

                if (vmServiceSearch.SelectedPublishingStatuses != null)
                {
                    commonService.ExtendPublishingStatusesDeletedOldPublished(vmServiceSearch.SelectedPublishingStatuses);
                    resultTemp = resultTemp.WherePublishingStatusIn(vmServiceSearch.SelectedPublishingStatuses);
                }

                resultTemp = resultTemp.OrderBy(x => x.UnificRootId);

                if (vmServiceSearch.IncludedRelations)
                {
                    //count = resultTemp.Select(i => true).Count();
                    resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                    q.Include(i => i.ServiceNames).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Versions).ThenInclude(i => i.ServiceChannelNames).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Versions).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDescriptions).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDigitalAuthorizations).ThenInclude(i => i.DigitalAuthorization)
                    );
                    var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                    var psDraft = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
                    var psModified = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
                    resultTemp = resultTemp
                                    .GroupBy(x => x.UnificRoot)
                                    .Select(collection => collection.FirstOrDefault(i => i.PublishingStatusId == psModified) ??
                   collection.FirstOrDefault(i => i.PublishingStatusId == psDraft) ??
                   collection.FirstOrDefault(i => i.PublishingStatusId == psPublished));
                }
                else
                {
                    resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                    q.Include(i => i.ServiceNames)
                    .Include(i => i.ServiceServiceClasses)
                    .Include(i => i.ServiceOntologyTerms).ThenInclude(i => i.OntologyTerm).ThenInclude(i => i.Names)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Versions))
                    ;
                }

                var resultFromDb = resultTemp.ApplyPaging(vmServiceSearch.PageNumber);
                moreAvailable = resultFromDb.MoreAvailable;
                result = vmServiceSearch.IncludedRelations
                          ? TranslationManagerToVm.TranslateAll<ServiceVersioned, VmServiceRelationListItem>(resultFromDb.Data).Cast<IVmServiceListItem>().ToList()
                          : TranslationManagerToVm.TranslateAll<ServiceVersioned, VmServiceListItem>(resultFromDb.Data).Cast<IVmServiceListItem>().ToList();
            });
            return new VmServiceSearchResult() {
                Services = result,
                PageNumber = ++vmServiceSearch.PageNumber,
                MoreAvailable = moreAvailable
            };
        }
        public IVmServiceSearchResult SearchServices(IVmServiceSearch vmServiceSearch)
        {
            vmServiceSearch.Name = vmServiceSearch.Name != null ? vmServiceSearch.Name.Trim() : vmServiceSearch.Name;
            var result = vmServiceSearch.Languages.Count > 1
                ? FullTextSearchService(vmServiceSearch)
                : FilteredSearchService(vmServiceSearch);
            return result;
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
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Versions).ThenInclude(i => i.ServiceChannelNames).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Versions).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDescriptions).ThenInclude(i => i.Type))
                    ;

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
                    var resultTemp = unitOfWork.ApplyIncludes(serviceRep.All().Where(x => x.ServiceServiceChannels.Any(y => y.ServiceChannelId == channel.UnificRootId)), i => i
                          .Include(j => j.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.ServiceClasses).ThenInclude(j => j.ServiceClass)
                          .Include(j => j.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.OntologyTerms).ThenInclude(j => j.OntologyTerm), true);

                    resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                        q.Include(i => i.ServiceNames).ThenInclude(i => i.Type)
                        .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Versions).ThenInclude(i => i.ServiceChannelNames).ThenInclude(i => i.Type)
                        .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Versions).ThenInclude(i => i.Type)
                        .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDescriptions).ThenInclude(i => i.Type))
                        ;

                    result = TranslationManagerToVm.TranslateAll<ServiceVersioned, VmServiceRelationListItem>(resultTemp).Cast<IVmServiceListItem>().ToList();
                }
            });
            return new VmServiceSearchResult() { Services = result };
        }

        public IVmServiceStep1 GetServiceStep1(IVmGetServiceStep model)
        {
            VmServiceStep1 result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);

                var serviceTypes = commonService.GetServiceTypes();
                var service = GetEntity<ServiceVersioned>(model.Id, unitOfWork,
                    q => q.Include(x => x.ServiceNames)
                         .Include(x => x.ServiceDescriptions)
                         .Include(x => x.ServiceLanguages)
                         .Include(x => x.ServiceRequirements).Include(x=>x.PublishingStatus)
                         .Include(x => x.ServiceMunicipalities).ThenInclude(x => x.Municipality)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.Names)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.Descriptions)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.ServiceClasses).ThenInclude(j => j.ServiceClass)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.TargetGroups)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.IndustrialClasses).ThenInclude(j => j.IndustrialClass)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.LifeEvents).ThenInclude(j => j.LifeEvent)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.OntologyTerms).ThenInclude(j => j.OntologyTerm)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.Type)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.ChargeType)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions).ThenInclude(j => j.StatutoryServiceLaws)
                         .Include(x => x.ServiceLaws).ThenInclude(j => j.Law).ThenInclude(k => k.Names)
                         .Include(x => x.ServiceLaws).ThenInclude(j => j.Law).ThenInclude(k => k.WebPages)
                         .Include(x => x.ServiceLaws).ThenInclude(j => j.Law).ThenInclude(k => k.WebPages).ThenInclude(l => l.WebPage)
                         .Include(x => x.ServiceTargetGroups)
                );
                result = GetModel<ServiceVersioned, VmServiceStep1>(service, unitOfWork);
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages()),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("ServiceTypes", serviceTypes),
                    () => GetEnumEntityCollectionModel("CoverageTypes", commonService.GetServiceCoverageTypes()),
                    () => GetEnumEntityCollectionModel("Municipalities", commonService.GetMunicipalities(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Laws", commonService.GetLaws(unitOfWork, result.GeneralDescription?.Laws))
                );
                if (!result.ServiceTypeId.IsAssigned())
                {
                    result.ServiceTypeId = serviceTypes.Single(x=>x.Code == ServiceTypeEnum.Service.ToString()).Id;
                }

                if (!result.ServiceCoverageTypeId.IsAssigned())
                {
                    result.ServiceCoverageTypeId = typesCache.Get<ServiceCoverageType>(ServiceCoverageTypeEnum.Nationwide.ToString());
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
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.WebPages).ThenInclude(x => x.WebPage)
                         .Include(x => x.ServiceCoverageType)
                         );
                SetTranslatorLanguage(model);
                result = GetModel<ServiceVersioned, VmServiceStep3>(service, unitOfWork);

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("ProvisionTypes", commonService.GetProvisionTypes()),
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizations(unitOfWork))
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

                if (model.Id.HasValue) {
                    var channelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
                    var resultTemp = unitOfWork.ApplyIncludes(channelRep.All().Where(x => x.ServiceChannel.Versions.Any(i => i.PublishingStatusId == typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString()) || (i.PublishingStatusId == typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString()))) && x.ServiceVersionedId == model.Id.Value),
                        q => q.Include(i => i.ServiceChannel).ThenInclude(i => i.Versions).ThenInclude(i => i.ServiceChannelNames).ThenInclude(i => i.Type)
                              .Include(i => i.ServiceChannel).ThenInclude(i => i.Versions).ThenInclude(i => i.Type)
                              .Include(i => i.ServiceChannel).ThenInclude(i => i.Versions).ThenInclude(i => i.UnificRoot)
                              .Include(i => i.ServiceChannel).ThenInclude(i => i.Versions)
                              .Include(i => i.ServiceVersioned))
                              .ToList();

                    result.Id = model.Id;
                    result.AttachedChannels = versioningManager.ApplyPublishingStatusFilter(resultTemp.SelectMany(x => x.ServiceChannel.Versions)).Select(x => TranslationManagerToVm.Translate<ServiceChannelVersioned, VmChannelListItem>(x)).ToList();
                    result.Connections = resultTemp.Select(x => TranslationManagerToVm.Translate<ServiceServiceChannel, VmConnection>(x)).ToList();

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

            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();

            if (vm.Step3Form != null && vm.Step3Form.Organizers.Any())
            {
                vm.Step3Form.OrganizersItems = vm.Step3Form.Organizers.Select(x => new VmTreeItem() {Id = x}).ToList();
            }

            PrefilterViewModel(vm);
            SetTranslatorLanguage(vm);
            var service = TranslationManagerToEntity.Translate<VmService, ServiceVersioned>(vm, unitOfWork);
            serviceRep.Add(service);
            return service;
        }

        public VmPublishingResultModel PublishService(VmPublishingModel model)
        {
            Guid serviceId = model.Id;
            var affected = commonService.PublishEntity<ServiceVersioned, ServiceLanguageAvailability>(model);
            var result = new VmPublishingResultModel()
            {
                Id = serviceId,
                PublishingStatusId = affected.AffectedEntities.First(i => i.Id == serviceId).PublishingStatusNew,
                LanguagesAvailabilities = model.LanguagesAvailabilities,
                Version = affected.Version
            };
            FillEnumEntities(result, () => GetEnumEntityCollectionModel("Services", affected.AffectedEntities.Select(i => new VmEntityStatusBase() { Id = i.Id, PublishingStatusId = i.PublishingStatusNew }).ToList<IVmBase>()));
            return result;
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

        private void UpdateStep1Model(IUnitOfWorkWritable unitOfWork, VmServiceStep1 model)
        {
            model.Laws = model.Laws.Where(i => i.UrlAddress != null && !i.UrlAddress.Trim().IsNullOrEmpty()).ToList();
            // model.ServiceTypeCode = unitOfWork.CreateRepository<IServiceTypeRepository>().All().FirstOrDefault(x => x.Id == model.ServiceType)?.Code;
        }

        private void SaveStep1Changes(IUnitOfWorkWritable unitOfWork, ref Guid serviceId, VmServiceStep1 model)
        {
            UpdateStep1Model(unitOfWork, model);

            SetTranslatorLanguage(model);
            var previousServiceId = serviceId;
            var service = TranslationManagerToEntity.Translate<VmService, ServiceVersioned>(new VmService() { Step1Form = model , Id = serviceId}, unitOfWork);
            serviceId = service.Id;

            var lawIds = model.Laws.Select(i => i.Id).ToList();
            var requestLanguageId = typesCache.Get<Language>((model?.Language ?? LanguageCode.fi).ToString());
            if (previousServiceId != serviceId)
            {
//                var lawsWebpageToDelete = unitOfWork.TranslationCloneCache.GetFromCachedSet<Law>().Where(i => lawIds.Contains(i.OriginalEntity.Id))
//                    .SelectMany(i => i.ClonedEntity.WebPages).Where(i => i.WebPage.LocalizationId == requestLanguageId).ToList();
//                var webPagesToDelete = lawsWebpageToDelete.Select(i => i.WebPage).Where(i => i.LocalizationId == requestLanguageId).ToList();
//
//                webPagesToDelete.ForEach(unitOfWork.DetachEntity);
//                lawsWebpageToDelete.ForEach(unitOfWork.DetachEntity);


                //
                //
                //                var clonedLawsIds = lawsToDelete.Select(i => i.ClonedEntity.Id);
                //                var webPagesToDelete = unitOfWork.TranslationCloneCache.GetFromCachedSet<WebPage>().Select(i => i.ClonedEntity).Where(i => clonedLawsIds.Contains(i.) i.LocalizationId == requestLanguageId).ToList();
                //                var lawWebPageToDelete = lawsToDelete.SelectMany(i => i.WebPages).Where(i => webPagesToDelete.Select(j => j.Id).Contains(i.WebPageId)).ToList();
                //
                //
                //                var webPagesToDelete = unitOfWork.TranslationCloneCache.GetFromCachedSet<WebPage>().Select(i => i.ClonedEntity).Where(i => i.LocalizationId == requestLanguageId).ToList();
                //                var lawsWebpageToDelete = unitOfWork.TranslationCloneCache.GetFromCachedSet<LawWebPage>().Select(i => i.ClonedEntity).Where(i => webPagesToDelete.Select(j => j.Id).Contains(i.WebPageId)).ToList();
                //
//                webPagesToDelete.ForEach(unitOfWork.DetachEntity);
//                lawsWebpageToDelete.ForEach(unitOfWork.DetachEntity);
            }
            else
            {

                var lawRep = unitOfWork.CreateRepository<ILawRepository>();
                var lawsToDelete = unitOfWork.ApplyIncludes(lawRep.All(), q => q.Include(i => i.WebPages).ThenInclude(j => j.WebPage)).Where(i => lawIds.Contains(i.Id)).ToList();
                var webPagesToDelete = lawsToDelete.SelectMany(i => i.WebPages.Select(j => j.WebPage)).Where(i => i.LocalizationId == requestLanguageId).ToList();
                var lawWebPageToDelete = lawsToDelete.SelectMany(i => i.WebPages).Where(i => webPagesToDelete.Select(j => j.Id).Contains(i.WebPageId)).ToList();

                var lawWebPageRep = unitOfWork.CreateRepository<ILawWebPageRepository>();
                var webPageRep = unitOfWork.CreateRepository<IWebPageRepository>();
                lawWebPageToDelete.ForEach(i => lawWebPageRep.Remove(i));
                webPagesToDelete.ForEach(i => webPageRep.Remove(i));
            }
            service.ServiceLanguages = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceLanguages, query => query.ServiceVersionedId == service.Id, language => language.LanguageId);

            service.ServiceMunicipalities = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceMunicipalities,
                query => query.ServiceVersionedId == service.Id,
                serviceMunicipality => serviceMunicipality.MunicipalityId);

            //Removing laws
//            dataUtils.UpdateCollectionWithRemove(unitOfWork,
//                service.ServiceLaws.Select(x => x.Law).ToList(),
//                curr => curr.ServiceLaws.Any(x => x.ServiceVersionedId == service.Id));

            //Update override target groups
            UpdateOverrideTargetGroups(service, unitOfWork);

            unitOfWork.Save(parentEntity: service);
        }

        /// <summary>
        /// Update override service target groups if general desctiption is changed
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
            serviceId = service.Id;
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

            ownerReferenceLogic.SetOwnerReference(model.ServiceProducers, serviceId);
            var serviceIdLocal = serviceId;
            var organizationServiceRepository = unitOfWork.CreateRepository<IOrganizationServiceRepository>();
            var organizers = organizationServiceRepository.All().Where(x => x.ServiceVersionedId == serviceIdLocal && x.RoleType.Code == RoleTypeEnum.Responsible.ToString()).GroupBy(x => x.OrganizationId).ToDictionary(x => x.Key);
            model.OrganizersItems = model.Organizers.Select(x =>
            {
                IGrouping<Guid?, Model.Models.OrganizationService> entity;
                var item = new VmTreeItem {Id = x};
                if (organizers.TryGetValue(x, out entity))
                {
                    item.OwnerReferenceId = entity.First().Id;
                }
                return item;
            }).ToList();
            organizers.Values.SelectMany(x => x).Where(x => !model.OrganizersItems.Select(o => o.OwnerReferenceId).Contains(x.Id)).ForEach(organizationServiceRepository.Remove);

            SetTranslatorLanguage(model);
            var service = TranslationManagerToEntity.Translate<VmService, ServiceVersioned>(vmService, unitOfWork);
            serviceId = service.Id;
            ICollection<Model.Models.OrganizationService> producers = service.OrganizationServices.Where(x => typesCache.GetByValue<RoleType>(x.RoleTypeId) == RoleTypeEnum.Producer.ToString()).ToList();
            dataUtils.UpdateCollectionWithRemove(unitOfWork, producers,
                query => query.ServiceVersionedId == service.Id && typesCache.GetByValue<RoleType>(query.RoleTypeId) == RoleTypeEnum.Producer.ToString());

            unitOfWork.Save(parentEntity: service);
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
            service.ServiceServiceChannels = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceServiceChannels, query => query.ServiceVersionedId == service.Id, channel => channel.ServiceChannelId);
            unitOfWork.Save(parentEntity: service);
        }

        private void PrefilterViewModel(VmService vm)
        {
            if (vm.Step2Form != null)
            {
                //vm.Step2Form.SelectedTargetGroups.AddRange(listItemLogic.GetSelected(vm.Step2Form.TargetGroups));
                //vm.Step2Form.ServiceClassesTarget = listItemLogic.GetSelected(vm.Step2Form.ServiceClassesTarget).ToList();
                //vm.Step2Form.LifeEventsTarget = listItemLogic.GetSelected(vm.Step2Form.LifeEventsTarget).ToList();
                //vm.Step2Form.OntologyTermsTarget = listItemLogic.GetSelected(vm.Step2Form.OntologyTermsTarget).ToList();
            }

                //if (vm.Step2Form.TargetGroupList.Any(x => (x.Code == "KR2") && !x.IsSelected))
                //{
                //    vm.Step2Form.IndustrialClassesTarget = new List<VmTreeItem>();
                //}

            logic.UpdateOrganizationSelectionForSelfProducer(vm, typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString()));
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
            return utilities.CheckIsEntityLocked(id);
        }

        #region Open Api
        public IVmOpenApiGuidPageVersionBase GetServices(DateTime? date, int pageNumber = 1, int pageSize = 1000)
        {
            return GetServices(new VmOpenApiGuidPage(pageNumber, pageSize), date);
        }

        public IVmOpenApiGuidPageVersionBase V3GetServices(DateTime? date, int pageNumber = 1, int pageSize = 1000)
        {
            return GetServices(new V3VmOpenApiGuidPage(pageNumber, pageSize), date);
        }

        public IVmOpenApiGuidPageVersionBase GetServices(IVmOpenApiGuidPageVersionBase vm, DateTime? date)
        {
            contextManager.ExecuteReader(unitOfWork =>
            {
                var filters = new List<Expression<Func<ServiceVersioned, bool>>>() { PublishedFilter<ServiceVersioned>(), ValidityFilter<ServiceVersioned>() };
                //// We need to filter out items that does not have any language versions published
                var publishedID = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                filters.Add(s => s.LanguageAvailabilities.Any(l => l.StatusId == publishedID));
                SetItemPage(vm, date, unitOfWork, filters, q => q.Include(i => i.ServiceNames));
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
                    var serviceList = serviceQuery.Select(c => c.ServiceVersionedId).ToList();

                    var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    var publishedServices = serviceRep.All().Where(s => serviceList.Contains(s.Id)).Where(PublishedFilter<ServiceVersioned>()).Where(ValidityFilter<ServiceVersioned>()).Select(s => s.Id).ToList();
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
                var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                var service = unitOfWork.ApplyIncludes(serviceRep.All().Where(s => s.Id == serviceId), q =>
                    q.Include(i => i.ServiceServiceChannels)).FirstOrDefault();
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
                        service.ServiceLifeEvents = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceLifeEvents,
                            query => query.ServiceVersionedId == service.Id, lifeEvent => lifeEvent.LifeEvent != null ? lifeEvent.LifeEvent.Id : lifeEvent.LifeEventId);
                    }
                    if (vm.DeleteAllIndustrialClasses || (vm.IndustrialClasses?.Count > 0))
                    {
                        service.ServiceIndustrialClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceIndustrialClasses,
                            query => query.ServiceVersionedId == service.Id, industrialClass => industrialClass.IndustrialClass != null ? industrialClass.IndustrialClass.Id : industrialClass.IndustrialClassId);
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
                        service.ServiceKeywords = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceKeywords,
                            query => query.ServiceVersionedId == service.Id, keyWord => keyWord.KeywordId);
                    }
                    if (vm.DeleteAllMunicipalities || (vm.Municipalities?.Count > 0))
                    {
                        service.ServiceMunicipalities = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceMunicipalities,
                            query => query.ServiceVersionedId == service.Id, municipality => municipality.Municipality != null ? municipality.Municipality.Id : municipality.MunicipalityId);
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
                .Include(i => i.ServiceCoverageType)
                .Include(i => i.ServiceMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.ServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.Names)
                .Include(i => i.ServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.WebPages).ThenInclude(i => i.WebPage)
                .Include(i => i.ServiceRequirements)
                .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDescriptions)
                .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDigitalAuthorizations)
                .Include(i => i.OrganizationServices).ThenInclude(i => i.AdditionalInformations)
                .Include(i => i.OrganizationServices).ThenInclude(i => i.WebPages).ThenInclude(i => i.WebPage)
                .Include(i => i.PublishingStatus)
                .Include(i => i.LanguageAvailabilities))
                .OrderByDescending(i => i.Modified);


            // Filter out items that do not have language versions published!
            var services = getOnlyPublished? resultTemp.Where(c => c.LanguageAvailabilities.Any(l => l.StatusId == publishedId)).ToList() : resultTemp.ToList();

            if (getOnlyPublished)
            {
                var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                var organizationVersionedRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                var publishedID = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                services.ForEach(service =>
                {
                    // Filter out not published serviceChannels
                    if (service.ServiceServiceChannels.Count > 0)
                    {
                        var channelRootIds = service.ServiceServiceChannels.Select(c => c.ServiceChannelId).ToList();
                        var publishedServiceChannelRootIds = serviceChannelRep.All().Where(c => channelRootIds.Contains(c.UnificRootId))
                            .Where(PublishedFilter<ServiceChannelVersioned>())
                            .Where(s => s.LanguageAvailabilities.Any(l => l.StatusId == publishedID)) // Filter out channels with no language versions publihed
                            .Select(c => c.UnificRootId).ToList();
                        service.ServiceServiceChannels = service.ServiceServiceChannels.Where(c => publishedServiceChannelRootIds.Contains(c.ServiceChannelId)).ToList();
                    }

                    // Filter out not published organizations
                    if (service.OrganizationServices.Count > 0)
                    {
                        var organizationRootIds = service.OrganizationServices.Where(i => i.OrganizationId != null).Select(s => s.OrganizationId).ToList();
                        // Get the organizations that have published version available in db
                        var publishedOrganizationRootIds = organizationVersionedRep.All().Where(i => organizationRootIds.Contains(i.UnificRootId))
                            .Where(PublishedFilter<OrganizationVersioned>())
                            .Where(s => s.LanguageAvailabilities.Any(l => l.StatusId == publishedID)) // Filter out organizations with no language versions publihed
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
                        service.ServiceServiceChannels.ForEach(channel =>
                        {
                            channel.ServiceServiceChannelDescriptions = channel.ServiceServiceChannelDescriptions.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        });
                        service.OrganizationServices.ForEach(organization =>
                        {
                            organization.AdditionalInformations = organization.AdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                            organization.WebPages = organization.WebPages.Where(i => !notPublishedLanguageVersions.Contains(i.WebPage.LocalizationId)).ToList();
                        });
                    }
                });
            }

            var result = TranslationManagerToVm.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(services).ToList();
            if (result == null)
            {
                throw new Exception(CoreMessages.OpenApi.RecordNotFound);
            }

            // Get the right open api view model version
            var versionList = new List<IVmOpenApiServiceVersionBase>();
            result.ForEach(service =>
            {
                // Check that service has type property attached. If not let's take it from general description
                if (service.Type == null && service.StatutoryServiceGeneralDescriptionId.IsAssigned())
                {
                    var gdRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                    var psPublished = PublishingStatusCache.Get(PublishingStatus.Published);
                    var gd = gdRep.All().FirstOrDefault(x => x.UnificRootId == service.StatutoryServiceGeneralDescriptionId && x.PublishingStatusId == psPublished);
                    if (gd != null)
                    {
                        service.GeneralDescriptionType = typesCache.GetByValue<ServiceType>(gd.TypeId);
                    }
                }
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
            //SetPublishingStatus(vm);
            CheckKeywords(vm, unitOfWork);

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

        private void CheckKeywords(IVmOpenApiServiceInVersionBase vm, IUnitOfWorkWritable unitOfWork)
        {
            var rep = unitOfWork.CreateRepository<IKeywordRepository>();
            vm.Keywords.ForEach(k =>
            {
                var keyWord = rep.All().Where(x => x.Name.ToLower() == k.Value.ToLower() && x.Localization.Code == k.Language).FirstOrDefault();
                if (keyWord != null)
                {
                    k.Id = keyWord.Id;
                }
            });
        }
        #endregion
    }
}
