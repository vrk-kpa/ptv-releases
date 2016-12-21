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
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Database.DataAccess.EntityCloners;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof (IServiceService), RegisterType.Transient)]
    internal class ServiceService : ServiceBase, IServiceService
    {
        private IContextManager contextManager;

        private ILogger logger;
        private VmListItemLogic listItemLogic;
        private ServiceLogic logic;
        private ServiceUtilities utilities;
        private DataUtils dataUtils;
        private ICommonService commonService;
        private VmOwnerReferenceLogic ownerReferenceLogic;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;

        public ServiceService(
            IContextManager contextManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            ILogger<ServiceService> logger,
            VmListItemLogic listItemLogic,
            ServiceLogic logic,
            ServiceUtilities utilities,
            DataUtils dataUtils,
            ICommonService commonService,
            VmOwnerReferenceLogic ownerReferenceLogic,
            ITypesCache typesCache,
            ILanguageCache languageCache,
            IPublishingStatusCache publishingStatusCache) : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.logic = logic;
            this.listItemLogic = listItemLogic;
            this.utilities = utilities;
            this.dataUtils = dataUtils;
            this.commonService = commonService;
            this.ownerReferenceLogic = ownerReferenceLogic;
            this.typesCache = typesCache;
            this.languageCache = languageCache;
        }

        public IVmGetServiceSearch GetServiceSearch()
        {
            // get enum values to local variables before using them inside LINQ queries as otherwise the provider does the queries in-memory and not in DB
            // as it cannot translate the enum values to db queries and this way we get better performance as the queries are evaluated in DB
            string statusDeletedCode = PublishingStatus.Deleted.ToString();

            var result = new VmGetServiceSearch();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceClassesRep = unitOfWork.CreateRepository<IServiceClassRepository>();

                var userOrganization = utilities.GetUserOrganization(unitOfWork);

                result = new VmGetServiceSearch
                {
                    OrganizationId = userOrganization?.Id
                };
                var publishingStatuses = commonService.GetPublishingStatuses(unitOfWork);
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ServiceClasses", TranslationManagerToVm.TranslateAll<ServiceClass, VmListItem>(serviceClassesRep.All().OrderBy(x => x.Label))),
                    () => GetEnumEntityCollectionModel("PublishingStatuses", publishingStatuses),
                    () => GetEnumEntityCollectionModel("ServiceTypes", commonService.GetServiceTypes(unitOfWork))
                );
                result.SelectedPublishingStatuses = publishingStatuses.Where(x => x.Code != statusDeletedCode).Select(x => x.Id).ToList();

            });
            return result;
        }

        public IVmServiceSearchResult SearchServices(IVmServiceSearch vmServiceSearch)
        {
            IReadOnlyList<IVmServiceListItem> result = new List<IVmServiceListItem>();
            var count = 0;
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(vmServiceSearch);

                var serviceRep = unitOfWork.CreateRepository<IServiceRepository>();
                var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionRepository>();
                var resultTemp = unitOfWork.ApplyIncludes(serviceRep.All(), i => i
                    .Include(j => j.StatutoryServiceGeneralDescription).ThenInclude(j => j.ServiceClasses).ThenInclude(j => j.ServiceClass)
                    .Include(j => j.StatutoryServiceGeneralDescription).ThenInclude(j => j.OntologyTerms).ThenInclude(j => j.OntologyTerm)
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
                    resultTemp = resultTemp.Where(x => x.OrganizationServices.Any(o => o.OrganizationId == vmServiceSearch.OrganizationId));
                }
                if (!string.IsNullOrEmpty(vmServiceSearch.ServiceName))
                {
                    var searchText = vmServiceSearch.ServiceName.ToLower();
                    resultTemp =
                        resultTemp.Where(
                            x =>
                                x.ServiceNames.Any(
                                    y =>
                                        y.Name.ToLower().Contains(searchText) &&
                                        y.LocalizationId == languageCache.Get(vmServiceSearch.Language.ToString())));
                }
                else
                {
                    resultTemp =
                        resultTemp.Where(
                            x =>
                                x.ServiceNames.Any(
                                    y =>
                                        y.LocalizationId == languageCache.Get(vmServiceSearch.Language.ToString()) &&
                                        !string.IsNullOrEmpty(y.Name)));
                }

                if (vmServiceSearch.OntologyWord.IsAssigned())
                {
                    var generalDescIds = generalDescriptionRep.All().Where(x => x.OntologyTerms.Select(s => s.OntologyTermId).Contains(vmServiceSearch.OntologyWord.Value)).Select(x => x.Id);
                    resultTemp = resultTemp.Where(
                        x => x.ServiceOntologyTerms.Any(y => y.OntologyTermId == vmServiceSearch.OntologyWord.Value) ||
                            generalDescIds.Any(d => d == x.StatutoryServiceGeneralDescriptionId)
                        );
                }

                if (vmServiceSearch.ServiceTypeId.HasValue)
                {
                    resultTemp = resultTemp.Where(x => x.TypeId == vmServiceSearch.ServiceTypeId.Value);
                }

                if (vmServiceSearch.SelectedPublishingStatuses != null)
                {
                    resultTemp = resultTemp.WherePublishingStatusIn(vmServiceSearch.SelectedPublishingStatuses);
                }

                count = resultTemp.Select(i => true).Count();
                resultTemp = resultTemp.OrderBy(x => x.Id);

                if (vmServiceSearch.IncludedRelations)
                {
                    resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                    q.Include(i => i.ServiceNames).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.ServiceChannelNames).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDescriptions).ThenInclude(i => i.Type))
                    ;
                }
                else
                {
                    resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                    q.Include(i => i.ServiceNames).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceClasses).ThenInclude(i => i.ServiceClass)
                    .Include(i => i.ServiceOntologyTerms).ThenInclude(i => i.OntologyTerm)
                    //.Include(i => i.OrganizationServices).ThenInclude(i => i.Organization).ThenInclude(i => i.OrganizationNames).ThenInclude(i => i.Type)
                    .Include(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel))
                    ;
                }

               var resultFromDb = vmServiceSearch.PageNumber > 0
                    ? resultTemp.Skip(vmServiceSearch.PageNumber*CoreConstants.MaximumNumberOfAllItems).Take(CoreConstants.MaximumNumberOfAllItems)
                    : resultTemp.Take(CoreConstants.MaximumNumberOfAllItems);

                result = vmServiceSearch.IncludedRelations
                          ? TranslationManagerToVm.TranslateAll<Service, VmServiceRelationListItem>(resultFromDb).Cast<IVmServiceListItem>().ToList()
                          : TranslationManagerToVm.TranslateAll<Service, VmServiceListItem>(resultFromDb).Cast<IVmServiceListItem>().ToList();
            });
            return new VmServiceSearchResult() { Services = result, PageNumber = ++vmServiceSearch.PageNumber, Count = count};
        }

        public IVmServiceSearchResult SearchRelationService(IVmServiceSearch model)
        {
            IReadOnlyList<IVmServiceListItem> result = new List<IVmServiceListItem>();
            var count = 0;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceRep = unitOfWork.CreateRepository<IServiceRepository>();
                var resultTemp = unitOfWork.ApplyIncludes(serviceRep.All().Where(x => x.Id == model.Id.Value), i => i
                    .Include(j => j.StatutoryServiceGeneralDescription).ThenInclude(j => j.ServiceClasses).ThenInclude(j => j.ServiceClass)
                    .Include(j => j.StatutoryServiceGeneralDescription).ThenInclude(j => j.OntologyTerms).ThenInclude(j => j.OntologyTerm), true);

                count = resultTemp.Select(i => true).Count();

                resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                    q.Include(i => i.ServiceNames).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.ServiceChannelNames).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDescriptions).ThenInclude(i => i.Type))
                    ;

                // showing finish texts
                SetTranslatorLanguage(model);
                result = TranslationManagerToVm.TranslateAll<Service, VmServiceRelationListItem>(resultTemp).Cast<IVmServiceListItem>().ToList();
            });
            return new VmServiceSearchResult() { Services = result, PageNumber = 1, Count = count };
        }

        public IVmServiceSearchResult SearchRelationServices(IVmServiceSearch model)
        {
            IReadOnlyList<IVmServiceListItem> result = new List<IVmServiceListItem>();
            var count = 0;
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                var serviceRep = unitOfWork.CreateRepository<IServiceRepository>();
                var resultTemp = unitOfWork.ApplyIncludes(serviceRep.All().Where(x => x.ServiceServiceChannels.Any(y=>y.ServiceChannelId == model.Id.Value)), i => i
                    .Include(j => j.StatutoryServiceGeneralDescription).ThenInclude(j => j.ServiceClasses).ThenInclude(j => j.ServiceClass)
                    .Include(j => j.StatutoryServiceGeneralDescription).ThenInclude(j => j.OntologyTerms).ThenInclude(j => j.OntologyTerm), true);

                count = resultTemp.Select(i => true).Count();

                resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                    q.Include(i => i.ServiceNames).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.ServiceChannelNames).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDescriptions).ThenInclude(i => i.Type))
                    ;

                result = TranslationManagerToVm.TranslateAll<Service, VmServiceRelationListItem>(resultTemp).Cast<IVmServiceListItem>().ToList();
            });
            return new VmServiceSearchResult() { Services = result, PageNumber = 1, Count = count };
        }

        public IVmServiceStep1 GetServiceStep1(IVmGetServiceStep model)
        {
            VmServiceStep1 result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);

                var serviceTypes = commonService.GetServiceTypes(unitOfWork);
                var service = GetEntity<Service>(model.Id, unitOfWork,
                    q => q.Include(x => x.ServiceNames)
                         .Include(x => x.ServiceDescriptions)
                         .Include(x => x.ServiceLanguages).ThenInclude(x => x.Language).ThenInclude(x=>x.LanguageNames)
                         .Include(x => x.ServiceRequirements).Include(x=>x.PublishingStatus)
                         .Include(x => x.ServiceMunicipalities).ThenInclude(x => x.Municipality)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Names)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.Descriptions)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.ServiceClasses).ThenInclude(j => j.ServiceClass)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.TargetGroups)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.IndustrialClasses).ThenInclude(j => j.IndustrialClass)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.LifeEvents).ThenInclude(j => j.LifeEvent)
                         .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(j => j.OntologyTerms).ThenInclude(j => j.OntologyTerm)
                );
                result = GetModel<Service, VmServiceStep1>(service);
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ServiceTypes", serviceTypes),
                    () => GetEnumEntityCollectionModel("CoverageTypes", commonService.GetServiceCoverageTypes(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Municipalities", commonService.GetMunicipalities(unitOfWork))
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
                var service = GetEntity<Service>(model.Id, unitOfWork,
                    q => q
                         .Include(x => x.ServiceKeywords).ThenInclude(x => x.Keyword)
                         .Include(x => x.ServiceKeywords).ThenInclude(x => x.Keyword).ThenInclude(x=>x.Localization)
                         .Include(x => x.ServiceServiceClasses).ThenInclude(x => x.ServiceClass)
                         .Include(x => x.ServiceOntologyTerms).ThenInclude(x => x.OntologyTerm)
                         .Include(x => x.ServiceLifeEvents).ThenInclude(x => x.LifeEvent)
                         .Include(x => x.ServiceIndustrialClasses).ThenInclude(x => x.IndustrialClass)
                         .Include(x => x.ServiceTargetGroups)

                         );
                SetTranslatorLanguage(model);
                result = GetModel<Service, VmServiceStep2>(service);

                var serviceClassesRep = unitOfWork.CreateRepository<IServiceClassRepository>();

                var lifeEventRep = unitOfWork.CreateRepository<ILifeEventRepository>();

                var keyWordRep = unitOfWork.CreateRepository<IKeywordRepository>();

                var targetGroupRep = unitOfWork.CreateRepository<ITargetGroupRepository>();

                var industrialClassesRep = unitOfWork.CreateRepository<IIndustrialClassRepository>();

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("TargetGroups", CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(targetGroupRep.All().OrderBy(x => x.Label), 1), x => x.Code)),
                    () => GetEnumEntityCollectionModel("LifeEvents", CreateTree<VmTreeItem>(LoadFintoTree(lifeEventRep.All(), 1), x => x.Name)),
                    () => GetEnumEntityCollectionModel("KeyWords", TranslationManagerToVm.TranslateAll<Keyword, VmKeywordItem>(keyWordRep.All().Where(x=>x.Localization.Code == model.Language.ToString()).OrderBy(x => x.Name)).ToList()),
                    () => GetEnumEntityCollectionModel("ServiceClasses", CreateTree<VmTreeItem>(LoadFintoTree(serviceClassesRep.All(), 1), x => x.Name)),
                    () => GetEnumEntityCollectionModel("IndustrialClasses", TranslationManagerToVm.TranslateAll<IFintoItem, VmTreeItem>(industrialClassesRep.All().Where(x=>x.Code=="5").OrderBy(x => x.Label)).ToList())
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
                var service = GetEntity<Service>(model.Id, unitOfWork,
                    q => q
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.Organization).ThenInclude(x => x.OrganizationNames).ThenInclude(x => x.Type)
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.Organization).ThenInclude(x => x.OrganizationNames)
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.RoleType)
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.ProvisionType)
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.AdditionalInformations)
                         .Include(x => x.OrganizationServices).ThenInclude(x => x.WebPages).ThenInclude(x => x.WebPage)
                         .Include(x => x.ServiceCoverageType)
                         );
                SetTranslatorLanguage(model);
                result = GetModel<Service, VmServiceStep3>(service);

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("ProvisionTypes", commonService.GetProvisionTypes(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizations(unitOfWork))
                );

                var userOrganization = utilities.GetUserOrganization(unitOfWork);
                if (userOrganization != null && result.Organizers.Count == 0)
                {
                    result.Organizers = new List<Guid> {userOrganization.Id};
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
                result.OrganizationId = userOrganization?.Id;

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork).ToList()),
                    () => GetEnumEntityCollectionModel("ChannelTypes", commonService.GetServiceChannelTypes(unitOfWork).ToList())
                );

                if (model.Id.HasValue) {
                    var channelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
                    var resultTemp = unitOfWork.ApplyIncludes(channelRep.All().Where(x => x.ServiceChannel.PublishingStatusId != typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString()) && x.ServiceId == model.Id.Value),
                        q=>q.Include(i=>i.ServiceChannel).ThenInclude(i=>i.ServiceChannelNames).ThenInclude(i => i.Type))
                         .Include(i => i.ServiceChannel).ThenInclude(i=>i.Type).Include(i => i.Service).ToList();
                    result.Id = model.Id;
                    result.AttachedChannels = resultTemp.Select(x => TranslationManagerToVm.Translate<ServiceChannel, VmChannelListItem>(x.ServiceChannel)).ToList();
                }
            });
            return result;
        }

        public IVmEntityBase AddService(VmService model)
        {
            Service result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = AddService(unitOfWork, model);
                unitOfWork.Save();

            });
            return new VmEntityStatusBase() { Id = result.Id, PublishingStatus = commonService.GetDraftStatusId() };
        }

        private Service AddService(IUnitOfWorkWritable unitOfWork, VmService vm) {

            vm.PublishingStatus = commonService.GetDraftStatusId();

            var serviceRep = unitOfWork.CreateRepository<IServiceRepository>();

            if (vm.Step3Form != null && vm.Step3Form.Organizers.Any())
            {
                vm.Step3Form.OrganizersItems = vm.Step3Form.Organizers.Select(x => new VmTreeItem() {Id = x}).ToList();
            }

            PrefilterViewModel(vm);
            SetTranslatorLanguage(vm);
            var service = TranslationManagerToEntity.Translate<VmService, Service>(vm, unitOfWork);
            serviceRep.Add(service);
            return service;
        }

        public IVmEntityBase PublishService(Guid? serviceId)
        {
            Service result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = PublishService(unitOfWork, serviceId);
                unitOfWork.Save();

            });
            return new VmEntityStatusBase { Id = result.Id, PublishingStatus = result.PublishingStatusId };
        }

        private Service PublishService(IUnitOfWorkWritable unitOfWork, Guid? serviceId)
        {
            var publishStatus = TranslationManagerToEntity.Translate<String, PublishingStatusType>(PublishingStatus.Published.ToString(), unitOfWork);

            var serviceRep = unitOfWork.CreateRepository<IServiceRepository>();
            var service = serviceRep.All().Single(x => x.Id == serviceId.Value);
            service.PublishingStatus = publishStatus;
            return service;
        }

        public List<IVmEntityBase> PublishServices(List<Guid> serviceIds)
        {
            var result = new List<IVmEntityBase>();
            contextManager.ExecuteWriter(unitOfWork =>
            {
                foreach (var id in serviceIds)
                {
                    var service = PublishService(unitOfWork, id);
                    result.Add(new VmEntityStatusBase { Id = service.Id, PublishingStatus = service.PublishingStatus.Id });
                }
                unitOfWork.Save();
            });
            return result;
        }

        public IVmEntityBase DeleteService(Guid? serviceId)
        {
            Service result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = DeleteService(unitOfWork, serviceId);
                unitOfWork.Save();

            });
            return new VmEntityStatusBase { Id = result.Id, PublishingStatus = result.PublishingStatusId };
        }

        public IVmEntityBase GetServiceStatus(Guid? serviceId)
        {
            VmPublishingStatus result = null;
            if (serviceId.IsAssigned())
            {
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    result = GetServiceStatus(unitOfWork, serviceId);
                });
            }
            return new VmEntityStatusBase { PublishingStatus = result?.Id };
        }

        private VmPublishingStatus GetServiceStatus(IUnitOfWorkWritable unitOfWork, Guid? serviceId)
        {
            var serviceRep = unitOfWork.CreateRepository<IServiceRepository>();
            var service = serviceRep.All()
                            .Include(x=>x.PublishingStatus)
                            .Single(x => x.Id == serviceId.Value);

            return TranslationManagerToVm.Translate<PublishingStatusType, VmPublishingStatus>(service.PublishingStatus);
        }

        private Service DeleteService(IUnitOfWorkWritable unitOfWork, Guid? serviceId)
        {
            var publishStatus = TranslationManagerToEntity.Translate<String, PublishingStatusType>(PublishingStatus.Deleted.ToString(), unitOfWork);

            var serviceRep = unitOfWork.CreateRepository<IServiceRepository>();
            var service = serviceRep.All().Single(x => x.Id == serviceId.Value);
            service.PublishingStatus = publishStatus;

            return service;
        }

        public IVmServiceStep1 SaveStep1Changes(Guid serviceId, VmServiceStep1 model)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SaveStep1Changes(unitOfWork, serviceId, model);
            });
            return GetServiceStep1(new VmGetServiceStep() { Id = serviceId, Language = model.Language });
        }

        private void UpdateStep1Model(IUnitOfWorkWritable unitOfWork, VmServiceStep1 model)
        {
           // model.ServiceTypeCode = unitOfWork.CreateRepository<IServiceTypeRepository>().All().FirstOrDefault(x => x.Id == model.ServiceType)?.Code;
        }

        private void SaveStep1Changes(IUnitOfWorkWritable unitOfWork, Guid serviceId, VmServiceStep1 model)
        {
            UpdateStep1Model(unitOfWork, model);

            SetTranslatorLanguage(model);
            var service = TranslationManagerToEntity.Translate<VmService, Service>(new VmService() { Step1Form = model , Id = serviceId}, unitOfWork);

            service.ServiceLanguages = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceLanguages, query => query.ServiceId == serviceId, language => language.LanguageId);

            service.ServiceMunicipalities = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceMunicipalities,
                query => query.ServiceId == serviceId,
                serviceMunicipality => serviceMunicipality.MunicipalityId);

            unitOfWork.Save(parentEntity: service);
        }

        public IVmServiceStep2 SaveStep2Changes(Guid serviceId, VmServiceStep2 model)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SaveStep2Changes(unitOfWork, serviceId, model);
            });
            return GetServiceStep2(new VmGetServiceStep() { Id = serviceId, Language = model.Language });
        }
        private void SaveStep2Changes(IUnitOfWorkWritable unitOfWork, Guid serviceId, VmServiceStep2 model)
        {
            var vmService = new VmService() { Step2Form = model, Id = serviceId };
            PrefilterViewModel(vmService);
            SetTranslatorLanguage(model);
            var service = TranslationManagerToEntity.Translate<VmService, Service>(vmService, unitOfWork);
            service.ServiceTargetGroups = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceTargetGroups,
                query => query.ServiceId == serviceId,
                targetGroup => targetGroup.TargetGroupId);
            service.ServiceLifeEvents = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceLifeEvents,
                query => query.ServiceId == serviceId,
                targetGroup => targetGroup.LifeEventId);
            service.ServiceServiceClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceServiceClasses,
                query => query.ServiceId == serviceId,
                targetGroup => targetGroup.ServiceClassId);
            service.ServiceOntologyTerms = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceOntologyTerms,
                query => query.ServiceId == serviceId,
                targetGroup => targetGroup.OntologyTermId);
            service.ServiceKeywords = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceKeywords,
                query => query.ServiceId == serviceId && query.Keyword.Localization.Code == model.Language.ToString(),
                keyWord => keyWord.KeywordId,
                query => query.ServiceId == serviceId && query.Keyword.Localization.Code != model.Language.ToString());
            service.ServiceIndustrialClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceIndustrialClasses,
               query => query.ServiceId == serviceId,
               targetGroup => targetGroup.IndustrialClassId);
            unitOfWork.Save(parentEntity: service);
        }

        public IVmServiceStep3 SaveStep3Changes(Guid serviceId, VmServiceStep3 model)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SaveStep3Changes(unitOfWork, serviceId, model);
            });
            return GetServiceStep3(new VmGetServiceStep() { Id = serviceId, Language = model.Language });
        }

        private void SaveStep3Changes(IUnitOfWorkWritable unitOfWork, Guid serviceId, VmServiceStep3 model)
        {
            var vmService = new VmService() { Step3Form = model, Id = serviceId };
            PrefilterViewModel(vmService);

            ownerReferenceLogic.SetOwnerReference(model.ServiceProducers, serviceId);

            var organizationServiceRepository = unitOfWork.CreateRepository<IOrganizationServiceRepository>();
            var organizers = organizationServiceRepository.All().Where(x => x.ServiceId == serviceId && x.RoleType.Code == RoleTypeEnum.Responsible.ToString()).GroupBy(x => x.OrganizationId).ToDictionary(x => x.Key);
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
            var service = TranslationManagerToEntity.Translate<VmService, Service>(vmService, unitOfWork);
            ICollection<Model.Models.OrganizationService> producers = service.OrganizationServices.Where(x => typesCache.GetByValue<RoleType>(x.RoleTypeId) == RoleTypeEnum.Producer.ToString()).ToList();
            dataUtils.UpdateCollectionWithRemove(unitOfWork, producers,
                query => query.ServiceId == serviceId && typesCache.GetByValue<RoleType>(query.RoleTypeId) == RoleTypeEnum.Producer.ToString());

            unitOfWork.Save(parentEntity: service);
        }

        public IVmServiceStep4ChannelData SaveStep4Changes(Guid serviceId, List<Guid> model)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SaveStep4Changes(unitOfWork, serviceId, model);
            });
            return GetServiceStep4Channeldata(new VmGetServiceStep() { Id = serviceId });
        }
        private void SaveStep4Changes(IUnitOfWorkWritable unitOfWork, Guid serviceId, List<Guid> model)
        {
            var service = TranslationManagerToEntity.Translate<VmService, Service>(new VmService() { Id = serviceId, Step4Form = model }, unitOfWork);
            service.ServiceServiceChannels = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceServiceChannels, query => query.ServiceId == serviceId, channel => channel.ServiceChannelId);
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

        #region Open Api
        public IVmOpenApiGuidPage GetServiceIds(DateTime? date, int pageNumber = 1, int pageSize = 1000)
        {
            var pagingVm = new VmOpenApiGuidPage(pageNumber, pageSize);

            contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceRep = unitOfWork.CreateRepository<IServiceRepository>();
                // Get only services that has been published
                var query = serviceRep.All().Where(PublishedFilter<Service>()).Where(ValidityFilter<Service>());

                if (date.HasValue)
                {
                    query = query.Where(s => s.Modified > date.Value);
                }

                pagingVm.SetPageCount(query.Count());

                if (pagingVm.IsValidPageNumber())
                {
                    if (pagingVm.PageCount > 1)
                    {
                        query = query.OrderBy(o => o.Created).Skip(pagingVm.GetSkipSize()).Take(pagingVm.GetTakeSize());
                    }
                    pagingVm.GuidList = query.ToList().Select(o => o.Id).ToList();
                }
            });

            return pagingVm;
        }

        public IVmOpenApiService GetService(Guid id, bool getOnlyPublished = true)
        {
            return TranslationManagerToVm.Translate<V2VmOpenApiService, VmOpenApiService>(V2GetService(id, getOnlyPublished) as V2VmOpenApiService);
        }

        public IV2VmOpenApiService V2GetService(Guid id, bool getOnlyPublished = true)
        {
            IV2VmOpenApiService result = new V2VmOpenApiService();
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var filters = new List<Expression<Func<Service, bool>>>() { service => service.Id.Equals(id) };
                    result = GetServicesWithDetails(filters, unitOfWork, getOnlyPublished).FirstOrDefault();
                });
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting a service with id {0}. {1}", id, ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }
            return result;
        }

        public IList<VmOpenApiService> GetServicesByServiceChannel(Guid id, DateTime? date)
        {
            return TranslationManagerToVm.TranslateAll<V2VmOpenApiService, VmOpenApiService>(V2GetServicesByServiceChannel(id, date)).ToList();
        }

        public IList<V2VmOpenApiService> V2GetServicesByServiceChannel(Guid id, DateTime? date)
        {
            IList<V2VmOpenApiService> result = new List<V2VmOpenApiService>();
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    // Get all the services that are related to defined service channel
                    var serviceChannelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
                    var serviceQuery = serviceChannelRep.All().Where(s => s.ServiceChannelId.Equals(id));
                    if (date.HasValue)
                    {
                        serviceQuery = serviceQuery.Where(s => s.Modified > date);
                    }
                    var serviceList = serviceQuery.Select(c => c.ServiceId).ToList();

                    // Define filters that are used to get required services.
                    var filters = new List<Expression<Func<Service, bool>>>()
                    {
                        service => serviceList.Contains(service.Id)// Get services that are listed in serviceList
                    };
                    result = GetServicesWithDetails(filters, unitOfWork);
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

        public IVmOpenApiService GetServiceBySource(string sourceId, string userName = null)
        {
            var result = new V2VmOpenApiService();
            var userId = userName ?? utilities.GetRelationIdForExternalSource();
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var id = GetPTVId<Service>(sourceId, userId, unitOfWork);
                    var filters = new List<Expression<Func<Service, bool>>>() { service => service.Id.Equals(id) };
                    result = GetServicesWithDetails(filters, unitOfWork).FirstOrDefault();
                });
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting services by source id {0}. {1}", sourceId, ex.Message);
                logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
            return TranslationManagerToVm.Translate<V2VmOpenApiService, VmOpenApiService>(result);
        }

        public IV2VmOpenApiService AddService(IVmOpenApiServiceInBase vm, bool allowAnonymous, string userName = null)
        {
            var vm2 = TranslationManagerToEntity.Translate<IVmOpenApiServiceInBase, V2VmOpenApiServiceInBase>(vm);
            return V2AddService(vm2, allowAnonymous, version1: true);
        }

        public IV2VmOpenApiService V2AddService(IV2VmOpenApiServiceInBase vm, bool allowAnonymous, string userName = null, bool version1 = false)
        {
            var service = new Service();
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = userName ?? utilities.GetRelationIdForExternalSource();
            var useOtherEndPoint = false;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Check if the external source already exists. Let's not throw the excpetion here to avoid contextManager to catch the exception.
                useOtherEndPoint = ExternalSourceExists<Service>(vm.SourceId, userId, unitOfWork);
                if (!useOtherEndPoint)
                {
                    CheckVm(vm, unitOfWork);
                    service = TranslationManagerToEntity.Translate<IV2VmOpenApiServiceInBase, Service>(vm, unitOfWork);
                    var serviceRep = unitOfWork.CreateRepository<IServiceRepository>();
                    serviceRep.Add(service);

                    // Create the mapping between external source id and PTV id
                    if (!string.IsNullOrEmpty(vm.SourceId))
                    {
                        SetExternalSource(service, vm.SourceId, userId, unitOfWork);
                    }

                    unitOfWork.Save(saveMode, userName: userName);
                }
            });

            if (useOtherEndPoint)
            {
                throw new ExternalSourceExistsException(string.Format(CoreMessages.OpenApi.ExternalSourceExists, vm.SourceId));
            }

            if (version1)
            {
                return GetService(service.Id, false);
            }
            return V2GetService(service.Id, false);
        }

        public IList<string> AddChannelsForService(Guid serviceId, IList<Guid> channelIds, bool allowAnonymous, string userName = null)
        {
            var result = new List<string>();
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var channelCount = 0;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                var serviceRep = unitOfWork.CreateRepository<IServiceRepository>();
                var channelRep = unitOfWork.CreateRepository<IServiceChannelRepository>();
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
                            var channel = channelRep.All().Where(x => x.Id == channelId).FirstOrDefault();
                            if (channel == null)
                            {
                                result.Add(string.Format(CoreMessages.OpenApi.EntityNotFound, "Service channel", channelId));
                            }
                            else
                            {
                                service.ServiceServiceChannels.Add(new ServiceServiceChannel() { ServiceChannel = channel });
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

        public IVmOpenApiService SaveService(IVmOpenApiServiceInBase vm, bool allowAnonymous, string sourceId = null, string userName = null)
        {
            var vm2 = TranslationManagerToEntity.Translate<IVmOpenApiServiceInBase, V2VmOpenApiServiceInBase>(vm);
            return V2SaveService(vm2, allowAnonymous, sourceId, userName, true) as VmOpenApiService;
        }
        public IV2VmOpenApiService V2SaveService(IV2VmOpenApiServiceInBase vm, bool allowAnonymous, string sourceId = null, string userName = null, bool version1 = false)
        {
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = userName ?? utilities.GetRelationIdForExternalSource();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                vm.Id = vm.Id == null ? GetPTVId<Service>(sourceId, userId, unitOfWork) : vm.Id;
                CheckVm(vm, unitOfWork);
                var service = TranslationManagerToEntity.Translate<IV2VmOpenApiServiceInBase, Service>(vm, unitOfWork);
                if (vm.Languages?.Count > 0)
                {
                    service.ServiceLanguages = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceLanguages,
                        query => query.ServiceId == service.Id, language => language.LanguageId);
                }
                if (vm.ServiceClasses?.Count > 0)
                {
                    service.ServiceServiceClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceServiceClasses,
                        query => query.ServiceId == service.Id, serviceClass => serviceClass.ServiceClass != null ? serviceClass.ServiceClass.Id : serviceClass.ServiceClassId);
                }
                if (vm.TargetGroups?.Count > 0)
                {
                    service.ServiceTargetGroups = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceTargetGroups,
                        query => query.ServiceId == service.Id, targetGroup => targetGroup.TargetGroup != null ? targetGroup.TargetGroup.Id : targetGroup.TargetGroupId);
                }
                if (vm.DeleteAllLifeEvents || (vm.LifeEvents?.Count > 0))
                {
                    service.ServiceLifeEvents = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceLifeEvents,
                        query => query.ServiceId == service.Id, lifeEvent => lifeEvent.LifeEvent != null ? lifeEvent.LifeEvent.Id : lifeEvent.LifeEventId);
                }
                if (vm.DeleteAllIndustrialClasses || (vm.IndustrialClasses?.Count > 0))
                {
                    service.ServiceIndustrialClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceIndustrialClasses,
                        query => query.ServiceId == service.Id, industrialClass => industrialClass.IndustrialClass != null ? industrialClass.IndustrialClass.Id : industrialClass.IndustrialClassId);
                }
                if (vm.OntologyTerms?.Count > 0)
                {
                    service.ServiceOntologyTerms = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceOntologyTerms,
                        query => query.ServiceId == service.Id, term => term.OntologyTerm != null ? term.OntologyTerm.Id : term.OntologyTermId);
                }
                if (vm.DeleteAllKeywords || vm.Keywords?.Count > 0)
                {
                    service.ServiceKeywords = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceKeywords,
                        query => query.ServiceId == service.Id, keyWord => keyWord.KeywordId);
                }
                if (vm.DeleteAllMunicipalities || (vm.Municipalities?.Count > 0))
                {
                    service.ServiceMunicipalities = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceMunicipalities,
                        query => query.ServiceId == service.Id, municipality => municipality.Municipality != null ? municipality.Municipality.Id : municipality.MunicipalityId);
                }
                if (vm.ServiceOrganizations?.Count > 0)
                {
                    // Set the responsible organizations
                    var organizationServiceRepository = unitOfWork.CreateRepository<IOrganizationServiceRepository>();
                    var responsible = RoleTypeEnum.Responsible.ToString();
                    var newResponsibles = service.OrganizationServices.Where(o => o.RoleType.Code == responsible).ToList();
                    var responsibles = organizationServiceRepository.All().Where(x => x.ServiceId == vm.Id && x.RoleType.Code == responsible).GroupBy(x => x.Id).ToDictionary(x => x.Key);
                    responsibles.Values.SelectMany(x => x).Where(x => !newResponsibles.Select(o => o.Id).Contains(x.Id)).ForEach(x =>
                    {
                        organizationServiceRepository.Remove(x);
                    });

                    // Set the producers organizations
                    var producers = service.OrganizationServices.Where(x => x.RoleType != null && x.RoleType.Code == RoleTypeEnum.Producer.ToString()).ToList();
                    dataUtils.UpdateCollectionWithRemove(unitOfWork, producers,
                        query => query.ServiceId == service.Id && query.RoleType.Code == RoleTypeEnum.Producer.ToString());
                }

                // Update the mapping between external source id and PTV id
                if (!string.IsNullOrEmpty(vm.SourceId))
                {
                    UpdateExternalSource(service, vm.SourceId, userId, unitOfWork);
                }

                unitOfWork.Save(saveMode, service, userName);
            });

            if (version1)
            {
                return GetService(vm.Id.Value, false);
            }
            return V2GetService(vm.Id.Value, false);
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

        private IList<V2VmOpenApiService> GetServicesWithDetails(IList<Expression<Func<Service, bool>>> filters, IUnitOfWork unitOfWork, bool getOnlyPublished = true)
        {
            IList<V2VmOpenApiService> result = new List<V2VmOpenApiService>();

            var serviceRep = unitOfWork.CreateRepository<IServiceRepository>();
            var query = serviceRep.All();
            if (getOnlyPublished) { query = query.Where(PublishedFilter<Service>()).Where(ValidityFilter<Service>()); }; // Get only published services
            // Add all filters into query
            filters.ForEach(p => query = query.Where(p));
            var resultTemp = unitOfWork.ApplyIncludes(query, q =>
                q.Include(i => i.ServiceLanguages).ThenInclude(i => i.Language)
                .Include(i => i.ServiceNames)
                .Include(i => i.ServiceDescriptions)
                .Include(i => i.ServiceServiceClasses).ThenInclude(i => i.ServiceClass)//.ThenInclude(i => i.Names));
                .Include(i => i.ServiceOntologyTerms).ThenInclude(i => i.OntologyTerm)//.ThenInclude(i => i.Names)
                .Include(i => i.ServiceTargetGroups).ThenInclude(i => i.TargetGroup)//.ThenInclude(i => i.Names)
                .Include(i => i.ServiceLifeEvents).ThenInclude(i => i.LifeEvent)//.ThenInclude(i => i.Names)
                .Include(i => i.ServiceIndustrialClasses).ThenInclude(i => i.IndustrialClass)
                .Include(i => i.ServiceKeywords).ThenInclude(i => i.Keyword)
                .Include(i => i.ServiceCoverageType)
                .Include(i => i.ServiceMunicipalities).ThenInclude(i => i.Municipality)
                //.Include(i => i.ServiceElectronicNotificationChannels)  // These are not included in first release version
                //.Include(i => i.ServiceElectronicCommunicationChannels) // - maybe in some future release.
                .Include(i => i.ServiceRequirements)
                .Include(i => i.OrganizationServices).ThenInclude(i => i.AdditionalInformations)
                .Include(i => i.OrganizationServices).ThenInclude(i => i.WebPages).ThenInclude(i => i.WebPage)
                .Include(i => i.PublishingStatus))
                .ToList();

            // Attach the published service channels into fetched services
            var serviceServiceChannelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            resultTemp.ForEach(r => r.ServiceServiceChannels = unitOfWork.ApplyIncludes(serviceServiceChannelRep.All().Where(s => s.ServiceId == r.Id)
                .Where(ServiceServiceChannelPublishedFilter()), q =>
                q.Include(i => i.ServiceServiceChannelDescriptions)).ToList());

            // Attach only published organizations into a service
            // Have to use a workaround because of EF 'feature'. See e.g. https://github.com/aspnet/EntityFramework/issues/5672
            var organizationRep = unitOfWork.CreateRepository<IOrganizationRepository>();
            resultTemp.ForEach(r =>
            {
                if (r.OrganizationServices.Count > 0)
                {
                    var serviceOrganizations = r.OrganizationServices.Where(i => i.OrganizationId != null).Select(s => s.OrganizationId).ToList();
                    var publishedOrganizations = organizationRep.All().Where(s => serviceOrganizations.Contains(s.Id)).Where(PublishedFilter<Organization>()).Where(ValidityFilter<Organization>()).Select(s => s.Id).ToList();
                    r.OrganizationServices = r.OrganizationServices.Where(i => i.OrganizationId == null || i.OrganizationId != null && publishedOrganizations.Contains(i.OrganizationId.Value)).ToList();
                }
            });

            result = TranslationManagerToVm.TranslateAll<Service, V2VmOpenApiService>(resultTemp).ToList();
            if (result == null)
            {
                throw new Exception(CoreMessages.OpenApi.RecordNotFound);
            }

            return result;
        }

        private Expression<Func<ServiceServiceChannel, bool>> ServiceServiceChannelPublishedFilter()
        {
            var now = DateTime.Now;
            var published = PublishingStatus.Published.ToString();
            return o => o.ServiceChannel.PublishingStatus.Code == published &&
                ((o.ServiceChannel.ValidFrom <= now && o.ServiceChannel.ValidTo >= now) ||
                    (o.ServiceChannel.ValidFrom == null && o.ServiceChannel.ValidTo == null));
        }

        private void CheckVm(IV2VmOpenApiServiceInBase vm, IUnitOfWorkWritable unitOfWork)
        {
            //SetPublishingStatus(vm);
            CheckKeywords(vm, unitOfWork);
        }

        private void CheckKeywords(IV2VmOpenApiServiceInBase vm, IUnitOfWorkWritable unitOfWork)
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
