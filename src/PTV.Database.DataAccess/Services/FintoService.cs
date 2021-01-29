/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.ObjectCloners;
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
using PTV.Domain.Model;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IFintoService), RegisterType.Transient)]
    internal class FintoService : ServiceBase, IFintoService
    {
        private AnnotationServiceProvider annotationProvider;
        private ILanguageCache languageCache;
        private ITreeTools treeTools;
        private const string messageAnnotationEmpty = "Service.AnnotationException.MessageEmpty";
        private const string messageAnnotationFailed = "Service.AnnotationException.MessageFailed";
        private ICommonServiceInternal commonServiceInternal;
        private readonly IOntologyTermDataCacheInternal ontologyTermDataCache;
        private readonly IIndustrialClassCacheInternal industrialClassCache;
        private readonly IServiceClassCacheInternal serviceClassCache;
        private readonly ILifeEventCacheInternal lifeEventCache;
        private readonly IDigitalAuthorizationCacheInternal digitalAuthorizationCache;
        private readonly ITargetGroupCacheInternal targetGroupCache;

        public FintoService(
            ITranslationEntity translationEntToVm,
            ITranslationViewModel translationVmToEntity,
            IPublishingStatusCache publishingStatusCache,
            AnnotationServiceProvider annotationProvider,
            IUserOrganizationChecker userOrganizationChecker,
            ILanguageCache languageCache,
			ICommonServiceInternal commonServiceInternal,
            IOntologyTermDataCache ontologyTermDataCache,
            IVersioningManager versioningManager,
            IIndustrialClassCacheInternal industrialClassCache,
            IServiceClassCacheInternal serviceClassCache,
            ILifeEventCacheInternal lifeEventCache,
            IDigitalAuthorizationCacheInternal digitalAuthorizationCache,
            ITargetGroupCacheInternal targetGroupCache,
            ITreeTools treeTools)
            : base(translationEntToVm, translationVmToEntity, publishingStatusCache, userOrganizationChecker, versioningManager)
        {
            this.annotationProvider = annotationProvider;
            this.languageCache = languageCache;
            this.commonServiceInternal = commonServiceInternal;
            this.industrialClassCache = industrialClassCache;
            this.ontologyTermDataCache = ontologyTermDataCache as IOntologyTermDataCacheInternal;
            this.serviceClassCache = serviceClassCache;
            this.lifeEventCache = lifeEventCache;
            this.digitalAuthorizationCache = digitalAuthorizationCache;
            this.targetGroupCache = targetGroupCache;
            this.treeTools = treeTools;
        }

        public IVmListItemsData<VmListItem> Search(IVmGetFilteredTree vmGetFilteredTree)
        {
            var treeType = Enum.Parse(typeof(TreeTypeEnum), vmGetFilteredTree.TreeType);


            IEnumerable<IFintoItemBase> searchResult = null;
            var orderLanguageCode = vmGetFilteredTree.Languages?.FirstOrDefault();

            switch ((TreeTypeEnum) treeType)
            {
                case TreeTypeEnum.ServiceClass:
                    searchResult = serviceClassCache.SearchByName(vmGetFilteredTree.SearchValue, orderLanguageCode);
                    break;
                case TreeTypeEnum.IndustrialClass:
                    searchResult = industrialClassCache.SearchByName(vmGetFilteredTree.SearchValue, orderLanguageCode);
                    break;
                case TreeTypeEnum.OntologyTerm:
                    searchResult = ontologyTermDataCache.SearchByName(vmGetFilteredTree.SearchValue, orderLanguageCode, vmGetFilteredTree.Languages?.Select(i => languageCache.Get(i)).ToList() ?? new List<Guid>());
                    break;
                case TreeTypeEnum.LifeEvent:
                    searchResult = lifeEventCache.SearchByName(vmGetFilteredTree.SearchValue, orderLanguageCode);
                    break;
                case TreeTypeEnum.DigitalAuthorization:
                    searchResult = digitalAuthorizationCache.SearchByName(vmGetFilteredTree.SearchValue, orderLanguageCode);
                    break;
                default:
                    searchResult = new List<IFintoItem>();
                    break;

            }
            var result = new VmListItemsData<VmListItem>(TranslationManagerToVm.TranslateAll<IFintoItemBase, VmListItem>(searchResult.ToList()).ToList());
            return result;
        }

        
        private IEnumerable<T> FilterFintoItems<T>(IVmGetFilteredTree vmGetFilteredTree, IFintoCache<T> cache, string orderLanguageCode)
            where T : IFintoItemBase, IHierarchy<T>, IEntityIdentifier
        {
            var filteredItems = vmGetFilteredTree.Id.IsAssigned()
                ? cache.SearchById(vmGetFilteredTree.Id ?? Guid.Empty)
                : cache.SearchByName(vmGetFilteredTree.SearchValue, orderLanguageCode);
            return SearchTree(cache.GetAllValid(), filteredItems.OrderBy(x => x.Label));
        }

        private List<OntologyTerm> FixRelations(List<OntologyTerm> terms)
        {
            var exactMatchCloner = new ExactMatchCloner();
            var ontologyExactMatchCloner = new OntologyTermExactMatchCloner{ ExactMatchCloner = exactMatchCloner};
            var nameCloner = new OntologyTermNameCloner();
            var ontologyTermCloner = new OntologyTermCloner { ExactMatchCloner = ontologyExactMatchCloner, NameCloner = nameCloner };

            // Clone parents without relations
            var parents = terms.SelectMany(t => t.Parents.Select(p => p.Parent)).ToList();
            var parentClones = ontologyTermCloner.CloneCollection(parents).ToList();

            // Clone children without relations
            var children = terms.SelectMany(t => t.Children.Select(c => c.Child));
            var childrenClones = ontologyTermCloner.CloneCollection(children).ToList();

            // Clone searched terms with relations
            var relationCloner = new OntologyTermParentCloner();
            ontologyTermCloner.ParentCloner = relationCloner;
            ontologyTermCloner.ChildCloner = relationCloner;
            var termClones = ontologyTermCloner.CloneCollection(terms).ToList();

            // Attach cloned parents and children to the relations
            foreach (var termClone in termClones)
            {
                foreach (var parent in termClone.Parents)
                {
                    parent.Parent = parentClones.FirstOrDefault(p => p.Id == parent.ParentId);
                }

                foreach (var child in termClone.Children)
                {
                    child.Child = childrenClones.FirstOrDefault(c => c.Id == child.ChildId);
                }
            }

            // Also, attach cloned terms to parents, since the translator will start translating from parents
            var termIds = terms.Select(t => t.Id).ToHashSet();
            foreach (var parentClone in parentClones)
            {
                var originalParent = parents.First(p => p.Id == parentClone.Id);
                parentClone.Children = relationCloner
                    .CloneCollection(originalParent.Children.Where(c => termIds.Contains(c.ChildId))).ToHashSet();
                foreach (var parentChild in parentClone.Children)
                {
                    parentChild.Child = termClones.FirstOrDefault(t => t.Id == parentChild.ChildId);
                }
            }

            return termClones;
        }

        public IVmListItemsData<IVmListItem> GetFilteredTree(IVmGetFilteredTree vmGetFilteredTree)
        {
            var treeType = Enum.Parse(typeof(TreeTypeEnum), vmGetFilteredTree.TreeType);
            VmListItemsData<IVmListItem> result = null;//new VmListItemsData<IVmTreeItem>();// { FilteredTree = new List<VmTreeItem>(), TreeType = vmGetFilteredTree.TreeType };
            var lowerValue = vmGetFilteredTree.SearchValue?.ToLower();
            var orderLanguageCode = vmGetFilteredTree.Languages?.FirstOrDefault();

                switch ((TreeTypeEnum)treeType)
                {
                    case TreeTypeEnum.ServiceClass:
                        var scFiltered = FilterFintoItems(vmGetFilteredTree, serviceClassCache, orderLanguageCode);
                        result = new VmListItemsData<IVmListItem>(CreateTree<VmFilteredTreeItem, IVmTreeItem>(scFiltered, x => x.Name));
                        break;
                    case TreeTypeEnum.IndustrialClass:
                        result = new VmListItemsData<IVmListItem>(CreateTree<VmFilteredTreeItem, IVmTreeItem>(industrialClassCache.SearchByName(lowerValue, orderLanguageCode), x => x.Name));
                        break;
                    case TreeTypeEnum.OntologyTerm:
                        var owFiltered = vmGetFilteredTree.Id.IsAssigned()
                            ? FixRelations(ontologyTermDataCache.SearchById(vmGetFilteredTree.Id ?? Guid.Empty, false))
                            : ontologyTermDataCache.SearchByName(vmGetFilteredTree.SearchValue, orderLanguageCode);
                        var ontologyTerms = owFiltered.SelectMany(x => x.Parents).Any() ? owFiltered.SelectMany(x => x.Parents).Select(x => x.Parent) : owFiltered;
                        result = new VmListItemsData<IVmListItem>(CreateTree<VmExpandedVmTreeItem>(ontologyTerms));
                        break;
                    case TreeTypeEnum.AnnotationOntologyTerm:
                        var aotFiltered = ontologyTermDataCache.SearchById(vmGetFilteredTree.Id ?? Guid.Empty, false);
                        var annotationOntologyTerms = aotFiltered.SelectMany(x => x.Parents).Any()
                            ? aotFiltered.SelectMany(x => x.Parents).Select(x => x.Parent)
                            : aotFiltered;
                        result = new VmListItemsData<IVmListItem>(CreateTree<VmExpandedVmTreeItem>(annotationOntologyTerms));
                        break;
                    case TreeTypeEnum.LifeEvent:
                        var leFiltered = FilterFintoItems(vmGetFilteredTree, lifeEventCache ,orderLanguageCode);
                        result = new VmListItemsData<IVmListItem>(CreateTree<VmFilteredTreeItem, IVmTreeItem>(leFiltered, x => x.Name));
                        break;
                    case TreeTypeEnum.Organization:
                        result = new VmListItemsData<IVmListItem>(CreateTree<VmTreeItemWithStatus>(commonServiceInternal.GetOrganizationNamesTree(vmGetFilteredTree.SearchValue)));
                        break;
                    case TreeTypeEnum.DigitalAuthorization:
                        var daFiltered = FilterFintoItems(vmGetFilteredTree, digitalAuthorizationCache, orderLanguageCode);
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
            result.TreeItem.AreChildrenLoaded = true;
            switch (treeType)
            {
                case TreeTypeEnum.ServiceClass:
                    result.TreeItem.Children = CreateTree<VmTreeItem, IVmTreeItem>(
                            treeTools.LoadFintoTree(serviceClassCache.GetAllValid(), 1,
                                new List<Guid?> {model.TreeItem.Id}), x => x.Name).ToList();
                    break;
                case TreeTypeEnum.LifeEvent:
                    result.TreeItem.Children = CreateTree<VmTreeItem, IVmTreeItem>(
                            treeTools.LoadFintoTree(lifeEventCache.GetAllValid(), 1,
                                new List<Guid?> {model.TreeItem.Id}), x => x.Name).ToList();
                    break;
                case TreeTypeEnum.Organization:
                    result.TreeItem.Children = CreateTree<VmTreeItemWithStatus>(commonServiceInternal
                        .GetOrganizationNamesTree(new List<Guid> {model.TreeItem.Id})
                        .SelectMany(org => org.Children));
                    break;
                case TreeTypeEnum.DigitalAuthorization:
                    result.TreeItem.Children = CreateTree<VmTreeItem, IVmTreeItem>(
                        treeTools.LoadFintoTree(digitalAuthorizationCache.GetAllValid(), 1,
                            new List<Guid?> {model.TreeItem.Id}), x => x.Name).ToList();
                    break;
                case TreeTypeEnum.OntologyTerm:
                case TreeTypeEnum.IndustrialClass:
                    break;
                default:
                    result.TreeItem.AreChildrenLoaded = false;
                    break;
            }

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
            return GetFintoItemByUri<ServiceClass>(uri, serviceClassCache);
        }

        public IVmOpenApiFintoItemVersionBase GetTargetGroupByUri(string uri)
        {
            return GetFintoItemByUri<TargetGroup>(uri, targetGroupCache);
        }

        public IVmOpenApiFintoItemVersionBase GetIndustrialClassByUri(string uri)
        {
            return GetFintoItemByUri<IndustrialClass>(uri, industrialClassCache);
        }

        private IVmOpenApiFintoItemVersionBase GetFintoItemByUri<T>(string uri, IFintoCache<T> cache) where T : FintoItemBase<T>
        {
            var item = cache.GetByUri(uri);
            return item == null ? null : new VmOpenApiFintoItemVersionBase
            {
                Id = item.Id,
                Code = item.Code,
                ParentId = item.ParentId,
                ParentUri = item.ParentUri,
                Uri = uri
            };
        }

        public VmAnnotations GetAnnotations(List<Guid> ids)
        {
            var ontologyTerms = ontologyTermDataCache.GetAllValid().Where(x => ids.Contains(x.Id));
            return new VmAnnotations
            {
                AnnotationOntologyTerms = TranslationManagerToVm.TranslateAll<IFintoItemBase, VmListItem>(ontologyTerms)
                    .ToList()
            };
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
                        var ontos = ontologyTermDataCache.GetOntologyTermsForExactMatches(uris);
                        var language = languageCache.Get(serviceInfo.LanguageCode);

                        if (!language.IsAssigned())
                        {
                            language = languageCache.Get(DomainConstants.DefaultLanguage);
                        }

                        return new VmAnnotations
                        {
                            Id = serviceInfo.Id,
                            LanguageId = language,
                            AnnotationOntologyTerms = TranslationManagerToVm.TranslateAll<IFintoItemBase, VmListItem>(ontos).ToList()
                        };
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
            var existingFintoItems = serviceClassCache.HasUris(uriList);
            List<string> mainItems = serviceClassCache.HasTopLevelUris(uriList);

            if (existingFintoItems?.Count > 0) return (uriList.Where(i => !existingFintoItems.Contains(i)).ToList(), mainItems);

            return (uriList, mainItems);
        }

        public List<string> CheckLifeEvents(List<string> uriList)
        {
            return CheckFintoItemsByUri<LifeEvent>(uriList, lifeEventCache);
        }

        public List<string> CheckIndustrialClasses(List<string> uriList)
        {
            return uriList.Except(industrialClassCache.HasUris(uriList)).ToList();
        }

        /// <summary>
        /// Returns a list of finto item uris that do not exist (within uriList).
        /// </summary>
        /// <param name="uriList"></param>
        private List<string> CheckFintoItemsByUri<T>(List<string> uriList, IFintoCache<T> cache) where T : FintoItemBase<T>
        {
            var existingFintoItems = cache.HasUris(uriList);

            if (existingFintoItems?.Count > 0) return uriList.Where(i => !existingFintoItems.Contains(i)).ToList();

            return uriList;
        }
    }
}
