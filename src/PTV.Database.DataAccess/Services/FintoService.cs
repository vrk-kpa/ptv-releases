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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IFintoService), RegisterType.Transient)]
    public class FintoService : ServiceBase, IFintoService
    {
        private IContextManager contextManager;
        private const string industrialClassLevel = "5";

        public FintoService(IContextManager contextManager, ITranslationEntity translationEntToVm, ITranslationViewModel translationVmToEntity,
            IPublishingStatusCache publishingStatusCache) : base(translationEntToVm, translationVmToEntity, publishingStatusCache)
        {
            this.contextManager = contextManager;
        }

        public IVmListItemsData<VmListItem> Search(IVmGetFilteredTree vmGetFilteredTree)
        {
            var treeType = Enum.Parse(typeof(TreeTypeEnum), vmGetFilteredTree.TreeType);


            IEnumerable<IFintoItemBase> searchResult = null;

            contextManager.ExecuteReader(unitOfWork =>
            {
                switch ((TreeTypeEnum)treeType)
                {
                    case TreeTypeEnum.ServiceClass:
                        searchResult = SearchFintoList<ServiceClass>(unitOfWork, vmGetFilteredTree.SearchValue);
                        break;
                    case TreeTypeEnum.IndustrialClass:
                        searchResult = SearchFintoList<IndustrialClass>(unitOfWork, vmGetFilteredTree.SearchValue, industrialClassLevel);
                        break;
                    case TreeTypeEnum.OntologyTerm:
                        searchResult = SearchFintoList<OntologyTerm>(unitOfWork, vmGetFilteredTree.SearchValue);
                        break;
                    case TreeTypeEnum.LifeEvent:
                        searchResult = SearchFintoList<LifeEvent>(unitOfWork, vmGetFilteredTree.SearchValue);
                        break;
                    default:
                        searchResult = new List<IFintoItem>();
                        break;
                }
            });
            var result = new VmListItemsData<VmListItem>(TranslationManagerToVm.TranslateAll<IFintoItemBase, VmListItem>(searchResult.ToList()).ToList());
            return result;
        }

        private IEnumerable<T> SearchFintoList<T>(IUnitOfWork unitOfWork, string searchTerm, string code = null) where T : IFintoItemBase
        {
            searchTerm = searchTerm.ToLower();
            var owRep = unitOfWork.CreateRepository<IRepository<T>>();
            var owQuery = owRep.All().Where(x => x.Label.ToLower().Contains(searchTerm));
            owQuery = string.IsNullOrEmpty(code) ? owQuery : owQuery.Where(x => x.Code == code);
            var searchResult = owQuery.OrderBy(x => x.Label).Take(50).ToList();
            return searchResult;
        }

        private IEnumerable<T> SearchFintoTree<T>(IUnitOfWork unitOfWork, IVmGetFilteredTree searchTerm) where T : IFintoItem, IHierarchy<T>, IEntityIdentifier
        {
            var owRep = unitOfWork.CreateRepository<IRepository<T>>();
            var owQuery = owRep.All();
            if (searchTerm.Id.IsAssigned())
            {
                owQuery = owQuery.Where(x => x.Id == searchTerm.Id || x.ParentId == searchTerm.Id);
            }
            else
            {
                string lowerValue = searchTerm.SearchValue.ToLower();
                owQuery = owQuery.Where(x => x.Label.ToLower().Contains(lowerValue));
            }
            var searchResult = SearchTree(owRep.All(), owQuery.OrderBy(x => x.Label));
            return searchResult;
        }

        private IEnumerable<OntologyTerm> SearchFintoTree(IUnitOfWork unitOfWork, IVmGetFilteredTree searchTerm)
        {
            var owRep = unitOfWork.CreateRepository<IRepository<OntologyTerm>>();
            var owQuery = owRep.All();
            if (searchTerm.Id.IsAssigned())
            {
                owQuery = owQuery.Where(x => x.Id == searchTerm.Id);
            }
            else
            {
                string lowerValue = searchTerm.SearchValue.ToLower();
                owQuery = owQuery.Where(x => x.Label.ToLower().Contains(lowerValue));
            }
//            var searchResult = SearchTree(owRep.All(), owQuery);
            var searchResult = unitOfWork.ApplyIncludes(owQuery.OrderBy(x => x.Label),
                q => q.Include(x => x.Children).ThenInclude(x => x.Child)
                    .Include(x => x.Parents).ThenInclude(x => x.Parent));
            return searchResult;
        }

        public IVmListItemsData<IVmTreeItem> GetFilteredTree(IVmGetFilteredTree vmGetFilteredTree)
        {
            var treeType = Enum.Parse(typeof(TreeTypeEnum), vmGetFilteredTree.TreeType);
            VmListItemsData<IVmTreeItem> result = null;//new VmListItemsData<IVmTreeItem>();// { FilteredTree = new List<VmTreeItem>(), TreeType = vmGetFilteredTree.TreeType };
            var lowerValue = vmGetFilteredTree.SearchValue.ToLower();

            contextManager.ExecuteReader(unitOfWork =>
            {
                switch ((TreeTypeEnum)treeType)
                {
                    case TreeTypeEnum.ServiceClass:
                        var scFiltered = SearchFintoTree<ServiceClass>(unitOfWork, vmGetFilteredTree);
                        result = new VmListItemsData<IVmTreeItem>(CreateTree<VmExpandedVmTreeItem>(scFiltered, x => x.Name));
                        break;
                    case TreeTypeEnum.IndustrialClass:
                        result = new VmListItemsData<IVmTreeItem>(CreateTree<VmTreeItem>(SearchFintoList<IndustrialClass>(unitOfWork, lowerValue, industrialClassLevel), x => x.Name));
                        break;
                    case TreeTypeEnum.OntologyTerm:
                        var owFiltered = SearchFintoTree(unitOfWork, vmGetFilteredTree).ToList();
                        var ontologyTerms = owFiltered.SelectMany(x => x.Parents).Any() ? owFiltered.SelectMany(x => x.Parents).Select(x => x.Parent) : owFiltered;
                        result = new VmListItemsData<IVmTreeItem>(CreateTree<VmExpandedVmTreeItem>(ontologyTerms));
                        break;
                    case TreeTypeEnum.LifeEvent:
                        var leFiltered = SearchFintoTree<LifeEvent>(unitOfWork, vmGetFilteredTree);
                        result = new VmListItemsData<IVmTreeItem>(CreateTree<VmExpandedVmTreeItem>(leFiltered, x => x.Name));
                        break;
                    case TreeTypeEnum.Organization:
                        var rep = unitOfWork.CreateRepository<IOrganizationRepository>();
                        var deletedStatus = PublishingStatus.Deleted.ToString();
                        var organizations = unitOfWork
                            .ApplyIncludes(rep.All().Where(x => x.PublishingStatus.Code != deletedStatus), query =>
                            query.Include(organization => organization.OrganizationNames).ThenInclude(name => name.Type)
                            .Include(organization => organization.Children).ThenInclude(o => o.OrganizationNames).ThenInclude(o => o.Type));
                        var searchedOrgs = organizations.Where(x => x.OrganizationNames.FirstOrDefault(y => y.TypeId == x.DisplayNameTypeId).Name.ToLower().Contains(lowerValue));
                        var orgFiltered = SearchTree(organizations, searchedOrgs);
                        result = new VmListItemsData<IVmTreeItem>(CreateTree<VmExpandedVmTreeItem>(orgFiltered));
                        break;
                    default:
                        break;
                }
            });

            return result;
        }

        public VmTreeItem GetFintoTree(IVmNode model)
        {
            var treeType = Enum.Parse(typeof(TreeTypeEnum), model.TreeType);
            var result = model;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result.TreeItem.AreChildrenLoaded = true;
                result.TreeItem.IsCollapsed = false;
                switch ((TreeTypeEnum)treeType)
                {
                    case TreeTypeEnum.ServiceClass:
                        result.TreeItem.Children = CreateTree<VmTreeItem>(LoadFintoTree(unitOfWork.CreateRepository<IServiceClassRepository>().All(), 1, new List<Guid>() { model.TreeItem.Id }), x => x.Name);
                        break;
                    case TreeTypeEnum.IndustrialClass:
                        result.TreeItem.Children = CreateTree<VmTreeItem>(LoadFintoTree(unitOfWork.CreateRepository<IIndustrialClassRepository>().All(), 1, new List<Guid>() { model.TreeItem.Id }), x => x.Name);
                        break;
                    case TreeTypeEnum.OntologyTerm:
//                        result.TreeItem.Children = CreateTree<VmTreeItem>(LoadFintoTree(unitOfWork.CreateRepository<IOntologyTermRepository>().All(), 1, new List<Guid>() { model.TreeItem.Id }));
                        break;
                    case TreeTypeEnum.LifeEvent:
                        result.TreeItem.Children = CreateTree<VmTreeItem>(LoadFintoTree(unitOfWork.CreateRepository<ILifeEventRepository>().All(), 1, new List<Guid>() { model.TreeItem.Id }), x => x.Name);
                        break;
                    case TreeTypeEnum.Organization:
                        var rep = unitOfWork.CreateRepository<IOrganizationRepository>();
                        var organizations = unitOfWork
                            .ApplyIncludes(rep.All(), query =>
                            query.Include(organization => organization.OrganizationNames).ThenInclude(name => name.Type));
                        result.TreeItem.Children = CreateTree<VmTreeItem>(LoadFintoTree<Organization>(organizations, 1, new List<Guid>() { model.TreeItem.Id }));
                        break;
                    default:
                        result.TreeItem.AreChildrenLoaded = false;
                        break;
                }

                result.TreeItem.IsFetching = false;
            });

            return result.TreeItem;
        }

        public IVmOpenApiFintoItem GetServiceClassByUri(string uri)
        {
            var result = new VmOpenApiFintoItem();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IServiceClassRepository>();
                result = TranslationManagerToVm.TranslateFirst<ServiceClass, VmOpenApiFintoItem>(rep.All().Where(x => x.Uri == uri));
            });
            return result;
        }


        public IVmOpenApiFintoItem GetOntologyTermByUri(string uri)
        {
            var result = new VmOpenApiFintoItem();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IOntologyTermRepository>();
                result = TranslationManagerToVm.TranslateFirst<OntologyTerm, VmOpenApiFintoItem>(rep.All().Where(x => x.Uri == uri));
            });
            return result;
        }

        public IVmOpenApiFintoItem GetTargetGroupByUri(string uri)
        {
            var result = new VmOpenApiFintoItem();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<ITargetGroupRepository>();
                result = TranslationManagerToVm.TranslateFirst<TargetGroup, VmOpenApiFintoItem>(rep.All().Where(x => x.Uri == uri));
            });
            return result;
        }

        public IVmOpenApiFintoItem GetLifeEventpByUri(string uri)
        {
            var result = new VmOpenApiFintoItem();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<ILifeEventRepository>();
                result = TranslationManagerToVm.TranslateFirst<LifeEvent, VmOpenApiFintoItem>(rep.All().Where(x => x.Uri == uri));
            });
            return result;
        }

        public IVmOpenApiFintoItem GetIndustrialClassByUri(string uri)
        {
            var result = new VmOpenApiFintoItem();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IIndustrialClassRepository>();
                result = TranslationManagerToVm.TranslateFirst<IndustrialClass, VmOpenApiFintoItem>(rep.All().Where(x => x.Uri == uri));
            });
            return result;
        }

    }
}
