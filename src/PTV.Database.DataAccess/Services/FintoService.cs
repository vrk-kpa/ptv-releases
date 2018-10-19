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
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
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
using PTV.Domain.Model;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IFintoService), RegisterType.Transient)]
    internal class FintoService : ServiceBase, IFintoService
    {
        private AnnotationServiceProvider annotationProvider;
        private IContextManager contextManager;
        private ILanguageCache languageCache;
        private const string industrialClassLevel = "5";
        private const string messageAnnotationEmpty = "Service.AnnotationException.MessageEmpty";
        private const string messageAnnotationFailed = "Service.AnnotationException.MessageFailed";
        private ICommonServiceInternal commonServiceInternal;
        private IOntologyTermDataCacheInternal ontologyTermDataCache;

        public FintoService(
            IContextManager contextManager,
            ITranslationEntity translationEntToVm,
            ITranslationViewModel translationVmToEntity,
            IPublishingStatusCache publishingStatusCache,
            AnnotationServiceProvider annotationProvider,
            IUserOrganizationChecker userOrganizationChecker,
            ILanguageCache languageCache,
			ICommonServiceInternal commonServiceInternal,
            IOntologyTermDataCache ontologyTermDataCache)
            : base(translationEntToVm, translationVmToEntity, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
            this.annotationProvider = annotationProvider;
            this.languageCache = languageCache;
            this.commonServiceInternal = commonServiceInternal;
            this.ontologyTermDataCache = ontologyTermDataCache as IOntologyTermDataCacheInternal;
        }

        public IVmListItemsData<VmListItem> Search(IVmGetFilteredTree vmGetFilteredTree)
        {
            var treeType = Enum.Parse(typeof(TreeTypeEnum), vmGetFilteredTree.TreeType);


            IEnumerable<IFintoItemBase> searchResult = null;

            switch ((TreeTypeEnum) treeType)
            {
                case TreeTypeEnum.ServiceClass:
                    searchResult = SearchFintoList<ServiceClass, ServiceClassName>(vmGetFilteredTree.SearchValue);
                    break;
                case TreeTypeEnum.IndustrialClass:
                    searchResult =
                        SearchFintoList<IndustrialClass, IndustrialClassName>(vmGetFilteredTree.SearchValue,
                            industrialClassLevel);
                    break;
                case TreeTypeEnum.OntologyTerm:
                    searchResult = ontologyTermDataCache.SearchByName(vmGetFilteredTree.SearchValue, vmGetFilteredTree.Languages?.Select(i => languageCache.Get(i)).ToList() ?? new List<Guid>());
                    break;
                case TreeTypeEnum.LifeEvent:
                    searchResult = SearchFintoList<LifeEvent, LifeEventName>(vmGetFilteredTree.SearchValue);
                    break;
                case TreeTypeEnum.DigitalAuthorization:
                    searchResult =
                        SearchFintoList<DigitalAuthorization, DigitalAuthorizationName>(vmGetFilteredTree.SearchValue);
                    break;
                default:
                    searchResult = new List<IFintoItem>();
                    break;

            }
            var result = new VmListItemsData<VmListItem>(TranslationManagerToVm.TranslateAll<IFintoItemBase, VmListItem>(searchResult.ToList()).ToList());
            return result;
        }

        private IEnumerable<T> SearchFintoList<T, TName>(string searchTerm, string code = null, List<string> languages = null) where T : class, IFintoItemBase, IFintoItemNames<TName> where TName : NameBase
        {
            var langIds = languages?.Select(i => languageCache.Get(i)).ToList() ?? new List<Guid>();
            searchTerm = searchTerm.ToLower();
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var owRep = unitOfWork.CreateRepository<IRepository<T>>();
                var owQuery = langIds.Any()
                    ? owRep.All().Where(x => x.Names.Any(n => langIds.Contains(n.LocalizationId) && n.Name.ToLower().Contains(searchTerm)))
                    : owRep.All().Where(x => x.Names.Any(n => n.Name.ToLower().Contains(searchTerm)));
                owQuery = string.IsNullOrEmpty(code) ? owQuery : owQuery.Where(x => x.Code == code);
                owQuery = unitOfWork.ApplyIncludes(owQuery.Where(x => x.IsValid), q => q.Include(x => x.Names));
                return owQuery.OrderBy(x => x.Label).Take(50).ToList();
            });
        }

        private IEnumerable<T> SearchFintoTree<T, TName>(IVmGetFilteredTree searchTerm) where T : class, IFintoItem, IHierarchy<T>, IEntityIdentifier, IFintoItemNames<TName> where TName : NameBase
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {

                var owRep = unitOfWork.CreateRepository<IRepository<T>>();
                var owQuery = owRep.All().Where(x => x.IsValid);
                if (searchTerm.Id.IsAssigned())
                {
                    owQuery = owQuery.Where(x => x.Id == searchTerm.Id || x.ParentId == searchTerm.Id);
                }
                else
                {
                    string lowerValue = searchTerm.SearchValue.ToLower();
                    owQuery = owQuery.Where(x => x.Names.Any(n => n.Name.ToLower().Contains(lowerValue)));
                }

                owQuery = unitOfWork.ApplyIncludes(owQuery,
                    q => q.Include(i => i.Names).ThenInclude(i => i.Localization));
                var searchResult = SearchTree(owRep.All().Where(x => x.IsValid), owQuery.OrderBy(x => x.Label));
                return searchResult.ToList();
            });
        }

        private List<OntologyTerm> SearchFintoTree(IVmGetFilteredTree searchTerm)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {

                var owRep = unitOfWork.CreateRepository<IRepository<OntologyTerm>>();
                var owQuery = owRep.All().Where(x => x.IsValid);
                if (searchTerm.Id.IsAssigned())
                {
                    owQuery = owQuery.Where(x => x.Id == searchTerm.Id);
                }
                else
                {
                    string lowerValue = searchTerm.SearchValue?.ToLower();
//                owQuery = owQuery.Where(x => x.Label.ToLower().Contains(lowerValue));
                    owQuery = owQuery.Where(x => x.Names.Any(n => n.Name.ToLower().Contains(lowerValue)));
                }

//            var searchResult = SearchTree(owRep.All(), owQuery);
                var searchResult = unitOfWork.ApplyIncludes(owQuery.OrderBy(x => x.Label),
                    q => q.Include(x => x.Children).ThenInclude(x => x.Child).ThenInclude(x => x.Names)
                        .Include(x => x.Parents).ThenInclude(x => x.Parent).ThenInclude(x => x.Names)
                        .Include(x => x.Names)
                );
                return searchResult.ToList();
            });
        }

        private IEnumerable<OntologyTerm> SearchAnnotationFintoTree(IVmGetFilteredTree searchTerm)
        {
            return contextManager.ExecuteReader(unitOfWork =>
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
            });
        }


        public IVmListItemsData<IVmListItem> GetFilteredTree(IVmGetFilteredTree vmGetFilteredTree)
        {
            var treeType = Enum.Parse(typeof(TreeTypeEnum), vmGetFilteredTree.TreeType);
            VmListItemsData<IVmListItem> result = null;//new VmListItemsData<IVmTreeItem>();// { FilteredTree = new List<VmTreeItem>(), TreeType = vmGetFilteredTree.TreeType };
            var lowerValue = vmGetFilteredTree.SearchValue?.ToLower();

                switch ((TreeTypeEnum)treeType)
                {
                    case TreeTypeEnum.ServiceClass:
                        var scFiltered = SearchFintoTree<ServiceClass, ServiceClassName>(vmGetFilteredTree);
                        result = new VmListItemsData<IVmListItem>(CreateTree<VmFilteredTreeItem, IVmTreeItem>(scFiltered, x => x.Name));
                        break;
                    case TreeTypeEnum.IndustrialClass:
                        result = new VmListItemsData<IVmListItem>(CreateTree<VmFilteredTreeItem, IVmTreeItem>(SearchFintoList<IndustrialClass, IndustrialClassName>(vmGetFilteredTree.SearchValue, industrialClassLevel), x => x.Name));
                        break;
                    case TreeTypeEnum.OntologyTerm:
                        var owFiltered = SearchFintoTree(vmGetFilteredTree);
                        var ontologyTerms = owFiltered.SelectMany(x => x.Parents).Any() ? owFiltered.SelectMany(x => x.Parents).Select(x => x.Parent) : owFiltered;
                        result = new VmListItemsData<IVmListItem>(CreateTree<VmExpandedVmTreeItem>(ontologyTerms));
                        break;
                    case TreeTypeEnum.AnnotationOntologyTerm:
                        var aotFiltered = SearchAnnotationFintoTree(vmGetFilteredTree).ToList();
                        var annotationOntologyTerms = aotFiltered.SelectMany(x => x.Parents).Any() ? aotFiltered.SelectMany(x => x.Parents).Select(x => x.Parent) : aotFiltered;
                        result = new VmListItemsData<IVmListItem>(CreateTree<VmExpandedVmTreeItem>(annotationOntologyTerms));
                        break;

                    case TreeTypeEnum.LifeEvent:
                        var leFiltered = SearchFintoTree<LifeEvent, LifeEventName>(vmGetFilteredTree);
                        result = new VmListItemsData<IVmListItem>(CreateTree<VmFilteredTreeItem, IVmTreeItem>(leFiltered, x => x.Name));
                        break;
                    case TreeTypeEnum.Organization:
                        result = new VmListItemsData<IVmListItem>(CreateTree<VmTreeItemWithStatus>(commonServiceInternal.GetOrganizationNamesTree(vmGetFilteredTree.SearchValue)));
                        break;
                    case TreeTypeEnum.DigitalAuthorization:
                        var daFiltered = SearchFintoTree<DigitalAuthorization, DigitalAuthorizationName>(vmGetFilteredTree);
                        result = new VmListItemsData<IVmListItem>(CreateTree<VmFilteredTreeItem, IVmTreeItem>(daFiltered, x => x.Name));
                        break;
                    default:
                        break;
                }

            return result;
        }

        public VmTreeItem GetFintoTree(IVmNode model)
        {
            var treeType = Enum.Parse(typeof(TreeTypeEnum), model.TreeType);
            var result = model;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result.TreeItem.AreChildrenLoaded = true;
                switch ((TreeTypeEnum)treeType)
                {
                    case TreeTypeEnum.ServiceClass:
                        result.TreeItem.Children = CreateTree<VmTreeItem, IVmTreeItem>(LoadFintoTree(GetIncludesForFinto<ServiceClass, ServiceClassName>(unitOfWork, unitOfWork.CreateRepository<IServiceClassRepository>().All().Include(x => x.Descriptions)), 1, new List<Guid>() { model.TreeItem.Id }), x => x.Name).ToList();
                        break;
                    case TreeTypeEnum.IndustrialClass:
                        result.TreeItem.Children = CreateTree<VmTreeItem, IVmTreeItem>(LoadFintoTree(GetIncludesForFinto<IndustrialClass, IndustrialClassName>(unitOfWork, unitOfWork.CreateRepository<IIndustrialClassRepository>().All()), 1, new List<Guid>() { model.TreeItem.Id }), x => x.Name).ToList();
                        break;
                    case TreeTypeEnum.OntologyTerm:
//                        result.TreeItem.Children = CreateTree<VmTreeItem>(LoadFintoTree(unitOfWork.CreateRepository<IOntologyTermRepository>().All(), 1, new List<Guid>() { model.TreeItem.Id }));
                        break;
                    case TreeTypeEnum.LifeEvent:
                        result.TreeItem.Children = CreateTree<VmTreeItem, IVmTreeItem>(LoadFintoTree(GetIncludesForFinto<LifeEvent, LifeEventName>(unitOfWork, unitOfWork.CreateRepository<ILifeEventRepository>().All()), 1, new List<Guid>() { model.TreeItem.Id }), x => x.Name).ToList();
                        break;
                    case TreeTypeEnum.Organization:
                        result.TreeItem.Children = CreateTree<VmTreeItemWithStatus>(commonServiceInternal.GetOrganizationNamesTree(new List<Guid>() { model.TreeItem.Id }).SelectMany(org => org.Children));
                        break;
                    case TreeTypeEnum.DigitalAuthorization:
                        result.TreeItem.Children = CreateTree<VmTreeItem, IVmTreeItem> (LoadFintoTree(GetIncludesForFinto<DigitalAuthorization, DigitalAuthorizationName>(unitOfWork, unitOfWork.CreateRepository<IDigitalAuthorizationRepository>().All()), 1, new List<Guid>() { model.TreeItem.Id }), x => x.Name).ToList();
                        break;
                    default:
                        result.TreeItem.AreChildrenLoaded = false;
                        break;
                }
            });

            return result.TreeItem;
        }

        public IVmListItemsData<IVmListItem> GetAnnotationHierarchy(IVmGetFilteredTree model)
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
            VmOpenApiFintoItemVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IRepository<OntologyTerm>>();
                // The finto items need to be valid (PTV-4317)
                var item = rep.All().Where(x => x.Uri == uri && x.IsValid == true).Select(x => new { x.Id, x.Code }).FirstOrDefault();
                if (item != null)
                {
                    result = new VmOpenApiFintoItemVersionBase { Id = item.Id, Code = item.Code };
                }
            });
            return result;
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

        private IVmOpenApiFintoItemVersionBase GetFintoItemByUri<T>(string uri) where T : FintoItemBase<T>
        {
            VmOpenApiFintoItemVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IRepository<T>>();
                // The finto items need to be valid (PTV-4317)
                var item = rep.All().Where(x => x.Uri == uri && x.IsValid == true).Select(x => new { x.Id, x.Code, x.ParentId }).FirstOrDefault();
                if (item != null)
                {
                    result = new VmOpenApiFintoItemVersionBase { Id = item.Id, Code = item.Code, ParentId = item.ParentId };
                }
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
                        var uris = annotationResult.Result?.Annotations.Select(x => x.Uri.ToLower());
                        return contextManager.ExecuteReader(unitOfWork =>
                        {
                            
                            
                            
                            var exactMatchRep = unitOfWork.CreateRepository<IOntologyTermExactMatchRepository>();
                            var exactMatches = exactMatchRep.All().Where(x => uris.Contains(x.ExactMatch.Uri.ToLower())).Select(x => x.OntologyTermId).ToList();

                            var ontoRep = unitOfWork.CreateRepository<IOntologyTermRepository>();
                            var ontos = GetIncludesForFinto<OntologyTerm, OntologyTermName>(unitOfWork, ontoRep.All().Where(i => exactMatches.Contains(i.Id)));
                            var language = languageCache.Get(serviceInfo.LanguageCode);

                            if (!language.IsAssigned())
                            {
                                language = languageCache.Get(DomainConstants.DefaultLanguage);
                            }
                            
                            return new VmAnnotations()
                            {
                                Id = serviceInfo.Id,
                                LanguageId = language,
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

        /// <summary>
        /// Returns two lists of servicec classes.
        /// The first list includes service classes that do not exist (within uriList).
        /// The second list includes service classes that are main classes (parent id is not defined)
        /// </summary>
        /// <param name="uriList"></param>
        /// <returns></returns>
        public (List<string>, List<string>) CheckServiceClasses(List<string> uriList)
        {
            List<string> existingFintoItems = null;
            List<string> mainItems = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var query = unitOfWork.CreateRepository<IRepository<ServiceClass>>().All().Where(l => uriList.Contains(l.Uri) && l.IsValid == true);

                existingFintoItems = query.Select(s => s.Uri).ToList();
                mainItems = query.Where(s => s.ParentId == null).Select(s => s.Uri).ToList();
            });

            if (existingFintoItems?.Count > 0) return (uriList.Where(i => !existingFintoItems.Contains(i)).ToList(), mainItems);

            return (uriList, mainItems);
        }

        public List<string> CheckOntologyTerms(List<string> uriList)
        {
            List<string> existingFintoItems = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                // The finto items need to be valid (PTV-4317)
                existingFintoItems = unitOfWork.CreateRepository<IRepository<OntologyTerm>>().All().Where(x => uriList.Contains(x.Uri) && x.IsValid == true).Select(x => x.Uri).ToList();                
            });

            if (existingFintoItems?.Count > 0) return uriList.Where(i => !existingFintoItems.Contains(i)).ToList();

            return uriList;
        }

        public List<string> CheckTargetGroups(List<string> uriList)
        {
            return CheckFintoItemsByUri<TargetGroup>(uriList);
        }

        public List<string> CheckLifeEvents(List<string> uriList)
        {
            return CheckFintoItemsByUri<LifeEvent>(uriList);
        }

        public List<string> CheckIndustrialClasses(List<string> uriList)
        {
            return CheckFintoItemsByUri<IndustrialClass>(uriList);
        }

        /// <summary>
        /// Returns a list of finto item uris that do not exist (within uriList).
        /// </summary>
        /// <param name="uriList"></param>
        private List<string> CheckFintoItemsByUri<T>(List<string> uriList) where T : FintoItemBase<T>
        {
            List<string> existingFintoItems = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var query = unitOfWork.CreateRepository<IRepository<T>>().All().Where(l => uriList.Contains(l.Uri) && l.IsValid == true);

                existingFintoItems = query.Select(s => s.Uri).ToList();
            });

            if (existingFintoItems?.Count > 0) return uriList.Where(i => !existingFintoItems.Contains(i)).ToList();

            return uriList;
        }
    }
}
