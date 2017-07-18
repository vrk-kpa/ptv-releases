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
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Extensions;
using PTV.Framework.ServiceManager;
using PTV.Framework.Interfaces;
using PTV.Database.Model.ServiceDataHolders;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IFintoService), RegisterType.Transient)]
    internal class FintoService : ServiceBase, IFintoService
    {
        private AnnotationServiceProvider annotationProvider;
        private IContextManager contextManager;
        private const string industrialClassLevel = "5";
        private const string messageAnnotationEmpty = "Service.AnnotationException.MessageEmpty";
        private const string messageAnnotationFailed = "Service.AnnotationException.MessageFailed";

        public FintoService(
            IContextManager contextManager,
            ITranslationEntity translationEntToVm,
            ITranslationViewModel translationVmToEntity,
            IPublishingStatusCache publishingStatusCache,
            AnnotationServiceProvider annotationProvider,
            IUserOrganizationChecker userOrganizationChecker)
            : base(translationEntToVm, translationVmToEntity, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
            this.annotationProvider = annotationProvider;
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
                        searchResult = SearchFintoList<ServiceClass, ServiceClassName>(unitOfWork, vmGetFilteredTree.SearchValue);
                        break;
                    case TreeTypeEnum.IndustrialClass:
                        searchResult = SearchFintoList<IndustrialClass, IndustrialClassName>(unitOfWork, vmGetFilteredTree.SearchValue, industrialClassLevel);
                        break;
                    case TreeTypeEnum.OntologyTerm:
                        searchResult = SearchFintoList<OntologyTerm, OntologyTermName>(unitOfWork, vmGetFilteredTree.SearchValue, languages: vmGetFilteredTree.Languages);
                        break;
                    case TreeTypeEnum.LifeEvent:
                        searchResult = SearchFintoList<LifeEvent, LifeEventName>(unitOfWork, vmGetFilteredTree.SearchValue);
                        break;
                    case TreeTypeEnum.DigitalAuthorization:
                        searchResult = SearchFintoList<DigitalAuthorization, DigitalAuthorizationName>(unitOfWork, vmGetFilteredTree.SearchValue);
                        break;
                    default:
                        searchResult = new List<IFintoItem>();
                        break;
                }
            });
            var result = new VmListItemsData<VmListItem>(TranslationManagerToVm.TranslateAll<IFintoItemBase, VmListItem>(searchResult.ToList()).ToList());
            return result;
        }

        private IEnumerable<T> SearchFintoList<T, TName>(IUnitOfWork unitOfWork, string searchTerm, string code = null, List<string> languages = null) where T : class, IFintoItemBase, IFintoItemNames<TName> where TName : NameBase
        {
            searchTerm = searchTerm.ToLower();
            var owRep = unitOfWork.CreateRepository<IRepository<T>>();
            var owQuery = languages != null && languages.Any() ?
                owRep.All().Where(x => x.Names.Any(n => languages.Contains(n.Localization.Code) && n.Name.ToLower().Contains(searchTerm))) :
                owRep.All().Where(x => x.Names.Any(n => n.Name.ToLower().Contains(searchTerm)));                            
            owQuery = string.IsNullOrEmpty(code) ? owQuery : owQuery.Where(x => x.Code == code);
            owQuery = unitOfWork.ApplyIncludes(owQuery, q => q.Include(x => x.Names));
            var searchResult = owQuery.OrderBy(x => x.Label).Take(50).ToList();
            return searchResult;
        }

        private IEnumerable<T> SearchFintoTree<T, TName>(IUnitOfWork unitOfWork, IVmGetFilteredTree searchTerm) where T : class, IFintoItem, IHierarchy<T>, IEntityIdentifier, IFintoItemNames<TName> where TName : NameBase
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
                owQuery = owQuery.Where(x => x.Names.Any(n => n.Name.ToLower().Contains(lowerValue)));
            }
            owQuery = unitOfWork.ApplyIncludes(owQuery, q => q.Include(i => i.Names).ThenInclude(i => i.Localization));
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
//                owQuery = owQuery.Where(x => x.Label.ToLower().Contains(lowerValue));
                owQuery = owQuery.Where(x => x.Names.Any(n => n.Name.ToLower().Contains(lowerValue)));
            }
//            var searchResult = SearchTree(owRep.All(), owQuery);
            var searchResult = unitOfWork.ApplyIncludes(owQuery.OrderBy(x => x.Label),
                q => q.Include(x => x.Children).ThenInclude(x => x.Child).ThenInclude(x => x.Names)
                    .Include(x => x.Parents).ThenInclude(x => x.Parent).ThenInclude(x => x.Names)
                    .Include(x => x.Names)
                    );
            return searchResult;
        }

        private IEnumerable<OntologyTerm> SearchAnnotationFintoTree(IUnitOfWork unitOfWork, IVmGetFilteredTree searchTerm)
        {
            var owRep = unitOfWork.CreateRepository<IRepository<OntologyTerm>>();
            var owQuery = owRep.All();
            if (searchTerm.Id.IsAssigned())
            {
                owQuery = owQuery.Where(x => x.Id == searchTerm.Id);
            }

            var searchResult = unitOfWork.ApplyIncludes(owQuery.OrderBy(x => x.Label),
                q => q.Include(x => x.Parents).ThenInclude(x => x.Parent).ThenInclude(x => x.Names)
                    .Include(x => x.Names));
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
                        var scFiltered = SearchFintoTree<ServiceClass, ServiceClassName>(unitOfWork, vmGetFilteredTree);
                        result = new VmListItemsData<IVmTreeItem>(CreateTree<VmExpandedVmTreeItem>(scFiltered, x => x.Name));
                        break;
                    case TreeTypeEnum.IndustrialClass:
                        result = new VmListItemsData<IVmTreeItem>(CreateTree<VmTreeItem>(SearchFintoList<IndustrialClass, IndustrialClassName>(unitOfWork, lowerValue, industrialClassLevel), x => x.Name));
                        break;
                    case TreeTypeEnum.OntologyTerm:
                        var owFiltered = SearchFintoTree(unitOfWork, vmGetFilteredTree).ToList();
                        var ontologyTerms = owFiltered.SelectMany(x => x.Parents).Any() ? owFiltered.SelectMany(x => x.Parents).Select(x => x.Parent) : owFiltered;
                        result = new VmListItemsData<IVmTreeItem>(CreateTree<VmExpandedVmTreeItem>(ontologyTerms));
                        break;
                    case TreeTypeEnum.AnnotationOntologyTerm:
                        var aotFiltered = SearchAnnotationFintoTree(unitOfWork, vmGetFilteredTree).ToList();
                        var annotationOntologyTerms = aotFiltered.SelectMany(x => x.Parents).Any() ? aotFiltered.SelectMany(x => x.Parents).Select(x => x.Parent) : aotFiltered;
                        result = new VmListItemsData<IVmTreeItem>(CreateTree<VmExpandedVmTreeItem>(annotationOntologyTerms));
                        break;

                    case TreeTypeEnum.LifeEvent:
                        var leFiltered = SearchFintoTree<LifeEvent, LifeEventName>(unitOfWork, vmGetFilteredTree);
                        result = new VmListItemsData<IVmTreeItem>(CreateTree<VmExpandedVmTreeItem>(leFiltered, x => x.Name));
                        break;
                    case TreeTypeEnum.Organization:
                        result = new VmListItemsData<IVmTreeItem>(CreateTree<VmExpandedVmTreeItem>(SearchOrganizationTree(unitOfWork, lowerValue)));
                        break;
                    case TreeTypeEnum.DigitalAuthorization:
                        var daFiltered = SearchFintoTree<DigitalAuthorization, DigitalAuthorizationName>(unitOfWork, vmGetFilteredTree);
                        result = new VmListItemsData<IVmTreeItem>(CreateTree<VmExpandedVmTreeItem>(daFiltered, x => x.Name));
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
                        result.TreeItem.Children = CreateTree<VmTreeItem>(LoadFintoTree(GetIncludesForFinto<ServiceClass, ServiceClassName>(unitOfWork, unitOfWork.CreateRepository<IServiceClassRepository>().All()), 1, new List<Guid>() { model.TreeItem.Id }), x => x.Name);
                        break;
                    case TreeTypeEnum.IndustrialClass:
                        result.TreeItem.Children = CreateTree<VmTreeItem>(LoadFintoTree(GetIncludesForFinto<IndustrialClass, IndustrialClassName>(unitOfWork, unitOfWork.CreateRepository<IIndustrialClassRepository>().All()), 1, new List<Guid>() { model.TreeItem.Id }), x => x.Name);
                        break;
                    case TreeTypeEnum.OntologyTerm:
//                        result.TreeItem.Children = CreateTree<VmTreeItem>(LoadFintoTree(unitOfWork.CreateRepository<IOntologyTermRepository>().All(), 1, new List<Guid>() { model.TreeItem.Id }));
                        break;
                    case TreeTypeEnum.LifeEvent:
                        result.TreeItem.Children = CreateTree<VmTreeItem>(LoadFintoTree(GetIncludesForFinto<LifeEvent, LifeEventName>(unitOfWork, unitOfWork.CreateRepository<ILifeEventRepository>().All()), 1, new List<Guid>() { model.TreeItem.Id }), x => x.Name);
                        break;
                    case TreeTypeEnum.Organization:
                        var rep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                        var psPublished = PublishingStatusCache.Get(PublishingStatus.Published);
                        var organizations = unitOfWork.ApplyIncludes(rep.All().Where(x => x.PublishingStatusId == psPublished), query => query.Include(organization => organization.OrganizationNames));
                        result.TreeItem.Children = CreateTree<VmTreeItem>(LoadOrganizationTree(organizations, 1, new List<Guid>() { model.TreeItem.Id }));
                        break;
                    case TreeTypeEnum.DigitalAuthorization:
                        result.TreeItem.Children = CreateTree<VmTreeItem>(LoadFintoTree(GetIncludesForFinto<DigitalAuthorization, DigitalAuthorizationName>(unitOfWork, unitOfWork.CreateRepository<IDigitalAuthorizationRepository>().All()), 1, new List<Guid>() { model.TreeItem.Id }), x => x.Name);
                        break;
                    default:
                        result.TreeItem.AreChildrenLoaded = false;
                        break;
                }

                result.TreeItem.IsFetching = false;
            });

            return result.TreeItem;
        }

        public IVmListItemsData<IVmTreeItem> GetAnnotationHierarchy(IVmGetFilteredTree model)
        {
            model.TreeType = TreeTypeEnum.AnnotationOntologyTerm.ToString();
            model.SearchValue = string.Empty;
            return GetFilteredTree(model);
        }

        public IVmOpenApiFintoItemVersionBase GetServiceClassByUri(string uri)
        {
            return GetFintoItemByUri<ServiceClass>(uri);
        }


        public IVmOpenApiFintoItemVersionBase GetOntologyTermByUri(string uri)
        {
            return GetFintoItemByUri<OntologyTerm>(uri);
        }

        public IVmOpenApiFintoItemVersionBase GetTargetGroupByUri(string uri)
        {
            return GetFintoItemByUri<TargetGroup>(uri);
        }

        public IVmOpenApiFintoItemVersionBase GetLifeEventByUri(string uri)
        {
            return GetFintoItemByUri<LifeEvent>(uri);
        }

        public IVmOpenApiFintoItemVersionBase GetIndustrialClassByUri(string uri)
        {
            return GetFintoItemByUri<IndustrialClass>(uri);
        }

        private IVmOpenApiFintoItemVersionBase GetFintoItemByUri<T>(string uri) where T : FintoItemBase
        {
            var result = new VmOpenApiFintoItemVersionBase();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IRepository<T>>();
                result = TranslationManagerToVm.TranslateFirst<T, VmOpenApiFintoItemVersionBase>(rep.All().Where(x => x.Uri == uri));
            });
            return result;
        }

        public VmAnnotations GetAnnotations(ServiceInfo serviceInfo)
        {
            var annotationResult = annotationProvider.GetAnnotations(serviceInfo);
            if (annotationResult.Result != null)
            {
                switch (annotationResult.Result.State)
                {
                    case AnnotationStates.Ok:
                        var uris = annotationResult.Result?.Annotations.Select(x => x.Uri);
                        return contextManager.ExecuteReader(unitOfWork =>
                        {
                            LanguageCode language;
                            if (!Enum.TryParse<LanguageCode>(serviceInfo.LanguageCode, out language))
                            {
                                language = LanguageCode.fi;
                            }
                            var exactMatchRep = unitOfWork.CreateRepository<IOntologyTermExactMatchRepository>();
                            var exactMatches = exactMatchRep.All().Where(x => uris.Contains(x.ExactMatch.Uri.ToLower())).Select(x => x.OntologyTermId).ToList();

                            var ontoRep = unitOfWork.CreateRepository<IOntologyTermRepository>();
                            var ontos = GetIncludesForFinto<OntologyTerm, OntologyTermName>(unitOfWork, ontoRep.All().Where(i => exactMatches.Contains(i.Id)));

                            return new VmAnnotations()
                            {
                                Id = serviceInfo.Id,
                                Language = language,
                                AnnotationOntologyTerms = TranslationManagerToVm.TranslateAll<IFintoItemBase, VmListItem>(ontos).ToList()
                            };
                        });
                    case AnnotationStates.EmptyInputReceived:
                        throw new PtvAppException("Annotation failed", messageAnnotationEmpty);
                    default:
                        throw new PtvAppException("Annotation failed", messageAnnotationFailed);
                }


            }
            else
            {
                throw new PtvAppException("Annotation failed", messageAnnotationFailed);
            }
        }
    }
}
