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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.Model.Models.Base;
using PTV.Database.Model.ServiceDataHolders;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Framework.ServiceManager;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V2;

namespace PTV.Database.DataAccess.Services
{
    internal abstract class ServiceBase
    {
        protected IPublishingStatusCache PublishingStatusCache;
        protected ITranslationEntity TranslationManagerToVm { get; }
        protected ITranslationViewModel TranslationManagerToEntity { get; }

        private const string EntityNotFoundMessage = ".Exception.NotFound";
        private IUserOrganizationChecker userOrganizationChecker;

        protected ServiceBase(ITranslationEntity translationManagerToVm, ITranslationViewModel translationManagerToEntity, IPublishingStatusCache publishingStatusCache, IUserOrganizationChecker userOrganizationChecker)
        {
            TranslationManagerToVm = translationManagerToVm;
            TranslationManagerToEntity = translationManagerToEntity;
            PublishingStatusCache = publishingStatusCache;
            this.userOrganizationChecker = userOrganizationChecker;
        }

        internal TEntity GetEntity<TEntity>(Guid? id, IUnitOfWork unitOfWork, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeChain) where TEntity : class, IEntityIdentifier
        {
            if (id.IsAssigned())
            {
                var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
                var entity = unitOfWork.ApplyIncludes(repository.All(), includeChain, true).SingleOrDefault(x => x.Id == id);
                if (entity == null)
                {
                    throw new EntityNotFoundException($"Entity {typeof(TEntity).Name} with id {id} not found", typeof(TEntity).Name.Replace("Versioned", string.Empty) + EntityNotFoundMessage, new[] { id.ToString() });
                }
                return entity;
            }
            return null;
        }

        protected TModel GetModel<TEntity, TModel>(TEntity entity, IUnitOfWork unitOfWork) where TModel : VmEntityBase, new() where TEntity: class
        {
            if (entity != null)
            {
                var model = TranslationManagerToVm.Translate<TEntity, TModel>(entity);
                SetOrganizationSecurity(model as IVmEntitySecurity, entity, unitOfWork);
                return model;
            }
            return new TModel();
        }

        private void SetOrganizationSecurity<TEntity>(IVmEntitySecurity model, TEntity entity, IUnitOfWork unitOfWork)
        {
            if (model != null)
            {
                model.Security = userOrganizationChecker.CheckEntity(entity, unitOfWork);
            }
        }

        /// <summary>
        /// Get the filter for fetching only valid (in datetime from/to) entities from database.
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <returns>Published filter</returns>
        internal Expression<Func<TEntity, bool>> ValidityFilter<TEntity>() where TEntity : class, IValidity
        {
            var now = DateTime.UtcNow;
            return p => ((p.ValidFrom <= now && p.ValidTo >= now) || (p.ValidFrom == null && p.ValidTo == null));
        }

        /// <summary>
        /// Get the filter for fetching only published entities from database.
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <returns>Published filter</returns>
        internal Expression<Func<TEntity, bool>> PublishedFilter<TEntity>() where TEntity : class, IPublishingStatus
        {
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            return p => p.PublishingStatusId == publishedId;
            //return p => p.PublishingStatus.Code == PublishingStatus.Published.ToString();
        }

        /// <summary>
        /// Get the filter for fetching only archived entities from database. Item is archived if no published version exist. If item has only Draft version it is not archived.
        /// So we need to get items that have either Deleted or OldPublished version but no Published version.
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <returns>Published filter</returns>
        internal Expression<Func<TEntity, bool>> ArchivedFilter<TEntity, TRoot>() where TEntity : class, IPublishingStatus, IVersionedVolume<TRoot> where TRoot : VersionedRoot<TEntity>
        {
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            var deletedId = PublishingStatusCache.Get(PublishingStatus.Deleted);
            var oldPublishedId = PublishingStatusCache.Get(PublishingStatus.OldPublished);

            return p => (p.PublishingStatusId == deletedId || p.PublishingStatusId == oldPublishedId) &&
                !p.UnificRoot.Versions.Any(x => x.PublishingStatusId == publishedId);
        }

        internal List<OrganizationTreeItem> SearchOrganizationTree(IUnitOfWork unitOfWork, string searchName)
        {
            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            searchName = searchName.ToLower();
            var psPublished = PublishingStatusCache.Get(PublishingStatus.Published);
            var orgFoundIds = orgRep.All().Where(i => i.PublishingStatusId == psPublished && i.OrganizationNames.Select(j => j.Name.ToLower()).Any(j => j.Contains(searchName))).Select(i => i.UnificRootId).ToList();
            var organizations = LoadOrganizationTreeRecursiveFromBottom(orgRep.All(), orgFoundIds).Select(i => new OrganizationTreeItem() { Organization = i, UnificRootId = i.UnificRootId, Children = new List<OrganizationTreeItem>(), Parent = null}).ToList();
            organizations.ForEach(parent =>
            {
                parent.Children = organizations.Where(i => i.Organization.ParentId == parent.UnificRootId).ToList();
                parent.Children.ForEach(ch =>
                {
                    ch.Parent = parent;
                });
            });
            return organizations.Where(o => o.Parent == null).ToList();
        }


        private List<OrganizationVersioned> LoadOrganizationTreeRecursiveFromBottom(IQueryable<OrganizationVersioned> inputQuery, List<Guid> leafRootIds)
        {
            if (leafRootIds.IsNullOrEmpty())
            {
                return new List<OrganizationVersioned>();
            }
            var psPublished = PublishingStatusCache.Get(PublishingStatus.Published);
            var orgs = inputQuery.Where(x => x .PublishingStatusId == psPublished && leafRootIds.Contains(x.UnificRootId)).Include(i => i.OrganizationNames).ToList();
            var parentRoots = orgs.Where(i => i.ParentId != null).Select(i => i.ParentId.Value).ToList();
            orgs.AddRange(LoadOrganizationTreeRecursiveFromBottom(inputQuery, parentRoots));
            return orgs;
        }


        internal List<OrganizationTreeItem> LoadOrganizationTree(IQueryable<OrganizationVersioned> inputQuery , int levelsToLoad = 0, ICollection<Guid> ids = null)
        {
            return LoadOrganizationTreeRecursive(inputQuery, new List<Guid>(), levelsToLoad, ids);
        }
        

        protected List<OrganizationTreeItem> LoadOrganizationTreeRecursive(IQueryable<OrganizationVersioned> inputQuery, List<Guid> loaded, int levelsToLoad = 0, ICollection<Guid> ids = null)
        {
            var query = inputQuery;
            if (ids == null)
            {
                query = query.Where(x => x.ParentId == null);
            }
            else if (ids.Count > 0)
            {
                var idsNullable = ids.Cast<Guid?>().ToList();
                query = query.Where(x => idsNullable.Contains(x.ParentId));
            }
            var orgs = query.ToList();
            var result = orgs.Select(i => new OrganizationTreeItem() { Organization = i, UnificRootId = i.UnificRootId, Children = new List<OrganizationTreeItem>()}).ToList();
            var subRootIds = orgs.Select(i => i.UnificRootId).Distinct().Except(loaded).ToList();
            loaded.AddRange(subRootIds);
            if (subRootIds.Count > 0 && (levelsToLoad > 0))
            {
                var subResult = LoadOrganizationTreeRecursive(inputQuery, loaded,--levelsToLoad, subRootIds);
                result.ForEach(parent =>
                {
                    parent.Children = subResult.Where(i => i.Organization.ParentId == parent.UnificRootId).ToList();
                    parent.Children.ForEach(ch =>
                    {
                        ch.Parent = parent;
                    });
                });
            }
            return result;
        }


        internal List<T> LoadFintoTree<T>(IQueryable<T> inputQuery, int? levelsToLoad = null, ICollection<Guid> ids = null)
            where T : IHierarchy<T>, IEntityIdentifier
        {
            var result = LoadFintoTreeRecursive(inputQuery, levelsToLoad, ids);
            if (result.IsNullOrEmpty())
            {
                return new List<T>();
            }
            var map = result.ToDictionary(i => i.Id, i => i);
            result.Where(i => i.ParentId != null).ForEach(i =>
            {
                // ReSharper disable once PossibleInvalidOperationException
                i.Parent = map.TryGet(i.ParentId.Value);
            });
            result.ForEach(i =>
            {
                i.Children = result.Where(j => j.ParentId == i.Id).ToList();
            });
            return ids == null ? result.Where(i => i.ParentId == null).ToList() : result.Where(i => i.ParentId == null || (i.ParentId != null && ids.Contains(i.ParentId.Value))).ToList();
        }

        internal IQueryable<T> GetIncludesForFinto<T, TName>(IUnitOfWork unitOfWork, IQueryable<T> inputQuery)
            where T : class, IFintoItemNames<TName>, IEntityIdentifier where TName  : NameBase
        {
            return unitOfWork.ApplyIncludes(inputQuery, q => q.Include(i => i.Names).ThenInclude(i => i.Localization));
        }

        private List<T> LoadFintoTreeRecursive<T>(IQueryable<T> inputQuery, int? levelsToLoad = null, ICollection<Guid> ids = null) where T : IHierarchy<T>, IEntityIdentifier
        {
            var query = inputQuery;
            if (ids == null)
            {
                query = query.Where(x => x.ParentId == null);
            }
            else if (ids.Count > 0)
            {
                var idsNullable = ids.Cast<Guid?>().ToList();
                query = query.Where(x => idsNullable.Contains(x.ParentId));
            }
            else
            {
                return new List<T>();
            }
            var result = query.ToList();
            if (result.Count > 0 && (!levelsToLoad.HasValue || levelsToLoad > 0))
            {
                result.AddRange(LoadFintoTreeRecursive(inputQuery, --levelsToLoad, result.Select(x => x.Id).ToList()));
            }
            return result;
        }

        internal List<T> CreateList<T>(IEnumerable<IFintoItemBase> data, Func<T, string> orderBy) where T : VmListItem
        {
            var result = TranslationManagerToVm.TranslateAll<IFintoItemBase, T>(data.ToList()).OrderBy(orderBy).ToList();
            return result;
        }

        internal List<TOut> CreateTree<TTranslate, TOut>(IEnumerable<IFintoItem> data, Func<TTranslate, string> orderBy) where TTranslate : class, IVmTreeItem where TOut : IVmTreeItem
        {
            return CreateTreeInternal(data, orderBy).Cast<TOut>().ToList();
        }

        internal List<TTranslate> CreateTree<TTranslate>(IEnumerable<IFintoItem> data, Func<TTranslate, string> orderBy) where TTranslate : class, IVmTreeItem
        {
            return CreateTreeInternal(data, orderBy).ToList();
        }

        private List<TTranslate> CreateTreeInternal<TTranslate>(IEnumerable<IFintoItem> data, Func<TTranslate, string> orderBy) where TTranslate : class, IVmTreeItem
        {
            return TranslationManagerToVm.TranslateAll<IFintoItem, TTranslate>(data.ToList()).OrderBy(orderBy).ToList();
        }

        internal List<VmTreeItem> CreateTree<T>(IEnumerable<OntologyTerm> data) where T : VmTreeItem
        {
            var result = data.Select( x => TranslationManagerToVm.Translate<OntologyTerm, T>(x) as VmTreeItem).OrderBy(x => x.Name).ToList();
            return result;
        }

        internal List<IVmTreeItem> CreateTree<T>(ICollection<OrganizationTreeItem> data) where T : class, IVmTreeItem
        {
            return TranslationManagerToVm.TranslateAll<OrganizationTreeItem, T>(data).OrderBy(x => x.Name).Cast<IVmTreeItem>().ToList();
        }
//
//        internal IEnumerable<Guid> GetAllHierarchyChildrenFlatten<T,Tp,Tch>(IQueryable<T> inputQuery, List<Guid> ids) where T : IHierarchy<Tp, Tch> where Tp : IVersionedRoot where Tch : IVersionedVolume
//        {
//            var idsNullable = ids.Cast<Guid?>().ToList();
//            var result = inputQuery.Where(x => x.ParentId.HasValue && idsNullable.Contains(x.ParentId)).Select(x => x.Id).ToList();
//
//            if (ids.Any())
//            {
//                result.AddRange(GetAllChildrenFlatten(inputQuery, result));
//            }
//
//            return result.Distinct();
//        }

        internal List<OrganizationVersioned> GetOrganizationsFlatten(ICollection<OrganizationTreeItem> organizationTreeItems)
        {
            var result = organizationTreeItems.Select(i => i.Organization);
            var children = organizationTreeItems.SelectMany(i => i.Children).ToList();
            return (children.Any() ? result.Union(GetOrganizationsFlatten(children)) : result).ToList();
        }

        internal IEnumerable<Guid> GetAllChildrenFlatten<T>(IQueryable<T> inputQuery, List<Guid> ids) where T : IHierarchy<T>, IEntityIdentifier
        {
            var idsNullable = ids.Cast<Guid?>().ToList();
            var result = inputQuery.Where(x => x.ParentId.HasValue && idsNullable.Contains(x.ParentId)).Select(x => x.Id).ToList();

            if (ids.Any())
            {
                result.AddRange(GetAllChildrenFlatten(inputQuery, result));
            }

            return result.Distinct();
        }

        private List<T> GetTreeFlatten<T>(IQueryable<T> inputQuery, IEnumerable<T> foundQuery) where T : IHierarchy<T>, IEntityIdentifier
        {
            var newResult = foundQuery.ToList();

            if (newResult.Count > 0)
            {
                var parentIds = newResult.Where(x => x.ParentId.HasValue && x.ParentId != x.Id).Select(x => x.ParentId).Distinct().Cast<Guid>().ToList();
                var allParents = inputQuery.Where(x => parentIds.Contains(x.Id));
                var subResult = GetTreeFlatten<T>(inputQuery, allParents);
                newResult.AddRange(subResult.Where(j => !newResult.Select(i => i.Id).Contains(j.Id)));
            }

            return newResult;
        }

        internal IEnumerable<T> SearchTree<T>(IQueryable<T> inputQuery, IEnumerable<T> foundQuery) where T : IHierarchy<T>, IEntityIdentifier
        {
            return SearchFintoFlattenTree(inputQuery, foundQuery).Where(x => !x.ParentId.HasValue);
        }

        internal IEnumerable<T> SearchFintoFlattenTree<T>(IQueryable<T> inputQuery, IEnumerable<T> foundQuery) where T : IHierarchy<T>, IEntityIdentifier
        {
            var result = GetTreeFlatten(inputQuery, foundQuery);
            if(result.IsNullOrEmpty())
            {
                return new List<T>();
            }
            var map = result.ToDictionary(i => i.Id, i => i);
            result.Where(i => i.ParentId != null).ForEach(i =>
            {
                // ReSharper disable once PossibleInvalidOperationException
                i.Parent = map.TryGet(i.ParentId.Value);
            });
            result.ForEach(i =>
            {
                i.Children = result.Where(j => j.ParentId == i.Id).ToList();
            });
            return result;
        }

        internal void FillEnumEntities(IEnumCollection model, params Func<KeyValuePair<string, IEnumerable<IVmBase>>>[] getEnumsFuncs)
        {
            model.EnumCollection = new VmDictionaryItemsData<IEnumerable<IVmBase>>(getEnumsFuncs.Select(getData => getData()).ToDictionary(x => x.Key, x => x.Value));
        }

        internal KeyValuePair<string, IEnumerable<IVmBase>> GetEnumEntityCollectionModel(string key, IEnumerable<IVmBase> collection)
        {
            return new KeyValuePair<string, IEnumerable<IVmBase>>(key, collection);
        }

		/// <summary>
        /// Set the data for ExternalSource table.
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="sourceId">External source id</param>
        /// <param name="PTVId">PTV object id</param>
        /// <param name="unitOfWork"></param>
        internal void SetExternalSource<TEntity>(TEntity entity, string sourceId, string userId, IUnitOfWork unitOfWork) where TEntity : class, IEntityIdentifier
        {
            if (string.IsNullOrEmpty(sourceId) || string.IsNullOrEmpty(userId))
            {
                throw new Exception(CoreMessages.OpenApi.ExternalSourceMalFormatted);
            }

            if (ExternalSourceExists<TEntity>(sourceId, userId, unitOfWork))
            {
                throw new Exception(string.Format(CoreMessages.OpenApi.ExternalSourceForOtherExists, sourceId, typeof(TEntity).Name));
            }

            var rep = unitOfWork.CreateRepository<IExternalSourceRepository>();
            rep.Add(new ExternalSource()
            {
                Id = Guid.NewGuid(),
                SourceId = sourceId,
                PTVId = entity.Id,
                ObjectType = typeof(TEntity).Name,
                RelationId = userId
            });
        }

        internal void UpdateExternalSource<TEntity>(Guid entityId, string sourceId, string userId, IUnitOfWork unitOfWork) where TEntity : class, IEntityIdentifier
        {
            if (string.IsNullOrEmpty(sourceId))
            {
                return;
            }
            var objectType = typeof(TEntity).Name;
            var rep = unitOfWork.CreateRepository<IExternalSourceRepository>();
            var currentExternalSource = rep.All().Where(e => e.RelationId == userId && e.ObjectType == objectType && e.SourceId == sourceId).FirstOrDefault();
            if (currentExternalSource != null && currentExternalSource.PTVId != entityId)
            {
                throw new Exception(string.Format(CoreMessages.OpenApi.ExternalSourceExistsUpdate, sourceId, currentExternalSource.PTVId));
            }
            var externalSource = rep.All().Where(e => e.PTVId == entityId && e.ObjectType == objectType && e.RelationId == userId).FirstOrDefault();

            if (externalSource != null)
            {
                externalSource.SourceId = sourceId;
            }
            else
            {
                rep.Add(new ExternalSource()
                {
                    Id = Guid.NewGuid(),
                    SourceId = sourceId,
                    PTVId = entityId,
                    ObjectType = typeof(TEntity).Name,
                    RelationId = userId
                });
            }
        }

        internal bool ExternalSourceExists<TEntity>(string sourceId, string userId, IUnitOfWork unitOfWork) where TEntity : class, IEntityIdentifier
        {
            if (!string.IsNullOrEmpty(sourceId))
            {
                var rep = unitOfWork.CreateRepository<IExternalSourceRepository>();
                var objectType = typeof(TEntity).Name;
                var externalSource = rep.All().Where(e => e.SourceId == sourceId && e.RelationId.ToLower() == userId.ToLower() && e.ObjectType == objectType).FirstOrDefault();
                return externalSource != null;
            }

            return false;
        }

        internal Guid GetPTVId<TEntity>(string sourceId, string userId, IUnitOfWork unitOfWork) where TEntity : class, IEntityIdentifier
        {
            var id = Guid.Empty;
            var entityName = typeof(TEntity).Name;
            if (!string.IsNullOrEmpty(sourceId))
            {
                var rep = unitOfWork.CreateRepository<IExternalSourceRepository>();
                var externalSource = rep.All().Where(e => e.SourceId == sourceId && e.RelationId.ToLower() == userId.ToLower() && e.ObjectType == entityName).FirstOrDefault();
                id = externalSource != null ? externalSource.PTVId : Guid.Empty;
            }
            if (id == Guid.Empty)
            {
                throw new Exception(string.Format(CoreMessages.OpenApi.EntityNotFound, entityName, sourceId));
            }
            return id;
        }

        internal string GetSourceId<TEntity>(Guid ptvId, string userId, IUnitOfWork unitOfWork) where TEntity : class, IEntityIdentifier
        {
            var rep = unitOfWork.CreateRepository<IExternalSourceRepository>();
            var objectType = typeof(TEntity).Name;
            var externalSource = rep.All().Where(e => e.PTVId == ptvId && e.RelationId.ToLower() == userId.ToLower() && e.ObjectType == objectType).FirstOrDefault();
            return externalSource != null ? externalSource.SourceId : null;
        }

        protected LanguageCode SetTranslatorLanguage(IVmLocalized model = null)
        {
            var languageCode = model?.Language ?? LanguageCode.fi;
            TranslationManagerToVm.SetLanguage(languageCode);
            TranslationManagerToEntity.SetLanguage(languageCode);
            return languageCode;
        }
        protected LanguageCode SetTranslatorLanguage(IVmMultiLocalized model = null)
        {
            var languageCode = model?.Languages?.FirstOrDefault() ?? LanguageCode.fi;
            TranslationManagerToVm.SetLanguage(languageCode);
            TranslationManagerToEntity.SetLanguage(languageCode);
            return languageCode;
        }

        // Get published items
        internal List<TEntity> GetPublishedEntities<TEntity, TRoot, TLanguageAvailability>(IVmOpenApiGuidPageVersionBase vm, DateTime? date, IUnitOfWork unitOfWork,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeChain, IList<Expression<Func<TEntity, bool>>> filters = null)
            where TEntity : class, IAuditing, IVersionedVolume<TRoot>, IMultilanguagedEntity<TLanguageAvailability>
            where TRoot : VersionedRoot<TEntity>
            where TLanguageAvailability : LanguageAvailability
        {
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            if (filters == null)
            {
                filters = new List<Expression<Func<TEntity, bool>>>();
            }

            // Get only published items -  filter out items that do not have any language versions published.
            filters.Add(e => e.PublishingStatusId == publishedId && e.LanguageAvailabilities.Any(l => l.StatusId == publishedId));
            
            // Date filter
            if (date.HasValue)
            {
                filters.Add(g => g.Modified > date.Value);
            }

            return GetEntitiesForPage<TEntity>(vm, unitOfWork, includeChain, filters);
        }


        // Get archived items
        internal List<TEntity> GetArchivedEntities<TEntity, TRoot, TLanguageAvailability>(IVmOpenApiGuidPageVersionBase vm, DateTime? date, IUnitOfWork unitOfWork,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeChain, IList<Expression<Func<TEntity, bool>>> filters = null)
            where TEntity : class, IAuditing, IVersionedVolume<TRoot>, IMultilanguagedEntity<TLanguageAvailability>
            where TRoot : VersionedRoot<TEntity>
            where TLanguageAvailability : LanguageAvailability
        {
            if (filters == null)
            {
                filters = new List<Expression<Func<TEntity, bool>>>();
            }

            // Get archived items.
            filters.Add(ArchivedFilter<TEntity, TRoot>());            

            // Date filter
            if (date.HasValue)
            {
                filters.Add(g => g.Modified > date.Value);
            }

            return GetDistinctEntitiesForPage<TEntity, TRoot>(vm, unitOfWork, includeChain, filters);
        }

        // Get draft, modified and published entities
        internal List<TEntity> GetActiveEntities<TEntity, TRoot, TLanguageAvailability>(IVmOpenApiGuidPageVersionBase vm, DateTime? date, IUnitOfWork unitOfWork,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeChain, IList<Expression<Func<TEntity, bool>>> filters = null)
            where TEntity : class, IAuditing, IVersionedVolume<TRoot>, IMultilanguagedEntity<TLanguageAvailability>
            where TRoot : VersionedRoot<TEntity>
            where TLanguageAvailability : LanguageAvailability
        {
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            var draftId = PublishingStatusCache.Get(PublishingStatus.Draft);
            var modifiedId = PublishingStatusCache.Get(PublishingStatus.Modified);

            if (filters == null)
            {
                filters = new List<Expression<Func<TEntity, bool>>>();
            }

            filters.Add(e => e.PublishingStatusId == draftId || e.PublishingStatusId == publishedId || e.PublishingStatusId == modifiedId);
            
            // Date filter
            if (date.HasValue)
            {
                filters.Add(g => g.Modified > date.Value);
            }

            return GetDistinctEntitiesForPage<TEntity, TRoot>(vm, unitOfWork, includeChain, filters);
        }

        //Get a list of entities according to filters
        internal List<TEntity> GetEntitiesForPage<TEntity>(IVmOpenApiGuidPageVersionBase vm, IUnitOfWork unitOfWork,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeChain, IList<Expression<Func<TEntity, bool>>> filters = null, Expression<Func<TEntity, string>> keySelector = null)
            where TEntity : class, IEntityIdentifier, IAuditing
        {
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var query = repository.All();

            // Additional filters
            if (filters?.Count > 0)
            {
                filters.ForEach(a => query = query.Where(a));
            }

            // Get entities total count.
            vm.SetPageCount(query.Count());
            if (vm.IsValidPageNumber())
            {
                // Get the items for one page
                if (keySelector != null)
                {
                    query = query.OrderBy(keySelector).Skip(vm.GetSkipSize()).Take(vm.GetTakeSize());
                }
                else
                {
                    query = query.OrderBy(o => o.Created).Skip(vm.GetSkipSize()).Take(vm.GetTakeSize());
                }                

                if (includeChain != null)
                {
                    // Items with data
                    return unitOfWork.ApplyIncludes(query, includeChain).ToList();
                }
                
                return query.ToList();
            }

            return null;
        }


        //Get a list of entities (one per root) according to filters
        internal List<TEntity> GetDistinctEntitiesForPage<TEntity, TRoot>(IVmOpenApiGuidPageVersionBase vm, IUnitOfWork unitOfWork,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeChain, IList<Expression<Func<TEntity, bool>>> filters = null)
            where TEntity : class, IEntityIdentifier, IAuditing, IVersionedVolume<TRoot>
            where TRoot : VersionedRoot<TEntity>
        {
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var query = repository.All();

            // Additional filters
            if (filters?.Count > 0)
            {
                filters.ForEach(a => query = query.Where(a));
            }

            // Get entities total count. Only one per root id!
            var entityIdList = query.Select(i => new { Id = i.Id, UnificRootId = i.UnificRootId, Modified = i.Modified, Created = i.Created }).GroupBy(x => x.UnificRootId).ToDictionary(i => i.Key, i => i.ToList())
                .Select(x => x.Value.OrderByDescending(c => c.Modified).FirstOrDefault()).Where(x => x != null).ToList();
            vm.SetPageCount(entityIdList.Count());

            if (vm.IsValidPageNumber())
            {
                // Get the items for one page
                var ids = entityIdList.OrderBy(o => o.Created).Skip(vm.GetSkipSize()).Take(vm.GetTakeSize()).Select(e => e.Id).ToList();
                // Get with data
                return unitOfWork.ApplyIncludes(repository.All().Where(e => ids.Contains(e.Id)), includeChain).ToList();                
            }

            return null;
        }

        /// <summary>
        /// Get the requested open api version from the model.
        /// The base model has version number 0.
        /// Example: Let's assume that we are supporting versions 1 - 4. User has requested to get version 2. First we have the base model (version number 0).
        /// We'll get the previous version from base, which in this case would be version 4. Version 4 is not yet the requested one so we get the previous one (version 3).
        /// Still the version 3 is not the requested one so we get the previous one which would be version 2. This is the requested one so let's return that view model.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="vm"></param>
        /// <param name="openApiVersion"></param>
        /// <returns></returns>
        internal TModel GetEntityByOpenApiVersion<TModel>(TModel vm, int openApiVersion) where TModel : IOpenApiVersionBase<TModel>
        {
            var version = vm.VersionNumber();

            if (openApiVersion == 0 && version == 0) // Base version is requested
            {
                return vm;
            }

            if (version != 0 && version <= openApiVersion)
            {
                return vm;
            }

            return GetEntityByOpenApiVersion(vm.PreviousVersion(), openApiVersion);
        }

        /// <summary>
        /// Return Guid from string name
        /// </summary>
        /// <param name="name">Name of service, channel, GD</param>
        /// <returns></returns>
        internal Guid? GetRootIdFromString(string name)
        {
            Guid result;
            if (Guid.TryParse(name.Trim(), out result))
            {
                return result;
            }
            return null;
        }

        internal List<Guid> GetOrganizationRootIdsFlatten(ICollection<OrganizationTreeItem> organizationTreeItems)
        {
            var result = organizationTreeItems.Select(i => i.Organization.UnificRootId);
            var children = organizationTreeItems.SelectMany(i => i.Children).ToList();
            return (children.Any() ? result.Union(GetOrganizationRootIdsFlatten(children)) : result).ToList();
        }

        internal List<Guid> GetOrganizationRootIdsFlatten(IUnitOfWork unitOfWork, Guid organizationRootId)
        {
            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var organizations = GetOrganizationRootIdsFlatten(LoadOrganizationTree(orgRep.All(), int.MaxValue, new List<Guid>() { organizationRootId }));
            organizations.Add(organizationRootId);
            return organizations;
        }

        internal bool IsAreaInAland(IUnitOfWork unitOfWork, List<Guid> areas, Guid provinceId)
        {
            var areaCode = unitOfWork.CreateRepository<IAreaRepository>().All().Where(a => areas.Contains(a.Id) && a.AreaTypeId == provinceId).Select(a => a.Code).FirstOrDefault();
            if (areaCode != null && areaCode.Trim() == "20") // Åland
            {
                return true;
            }

            return false;
        }

        internal Func<IQueryable<ServiceServiceChannel>, IQueryable<ServiceServiceChannel>> GetConnectionIncludeChain()
        {
            return q =>
                q.Include(i => i.ServiceServiceChannelDescriptions)
                .Include(i => i.ServiceServiceChannelDigitalAuthorizations).ThenInclude(i => i.DigitalAuthorization).ThenInclude(i => i.Names)
                .Include(i => i.ServiceServiceChannelExtraTypes).ThenInclude(i => i.ExtraSubType)
                .Include(i => i.ServiceServiceChannelExtraTypes).ThenInclude(i => i.ServiceServiceChannelExtraTypeDescriptions)
                .Include(i => i.ServiceServiceChannelServiceHours).ThenInclude(i => i.ServiceHours).ThenInclude(i => i.DailyOpeningTimes)
                .Include(i => i.ServiceServiceChannelServiceHours).ThenInclude(i => i.ServiceHours).ThenInclude(i => i.AdditionalInformations)
                .Include(i => i.ServiceServiceChannelEmails).ThenInclude(i => i.Email)
                .Include(i => i.ServiceServiceChannelPhones).ThenInclude(i => i.Phone).ThenInclude(i => i.PrefixNumber)
                .Include(i => i.ServiceServiceChannelWebPages).ThenInclude(i => i.WebPage)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.AddressStreets).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations);
        }
                
        internal void MapConnection(ServiceServiceChannel ssc, IVmOpenApiServiceServiceChannelBase vmssc, ITypesCache typesCache, ILanguageCache languageCache)
        {
            if (ssc == null || vmssc == null) return;

            vmssc.ServiceChargeType = ssc.ChargeTypeId.HasValue ? typesCache.GetByValue<ServiceChargeType>(ssc.ChargeTypeId.Value) : null;

            // manually map items

            // connection description
            if (ssc.ServiceServiceChannelDescriptions != null && ssc.ServiceServiceChannelDescriptions.Count > 0)
            {
                ssc.ServiceServiceChannelDescriptions.ForEach(srcDesc =>
                {
                    VmOpenApiLocalizedListItem desc = new VmOpenApiLocalizedListItem()
                    {
                        Language = languageCache.GetByValue(srcDesc.LocalizationId),
                        Type = typesCache.GetByValue<DescriptionType>(srcDesc.TypeId),
                        Value = string.IsNullOrEmpty(srcDesc.Description) ? null : srcDesc.Description
                    };

                    vmssc.Description.Add(desc);
                });
            }

            // extratypes
            if (ssc.ServiceServiceChannelExtraTypes != null && ssc.ServiceServiceChannelExtraTypes.Count > 0)
            {
                ssc.ServiceServiceChannelExtraTypes.ForEach(srcExtras =>
                {
                    VmOpenApiExtraType extra = new VmOpenApiExtraType()
                    {
                        ChannelGuid = srcExtras.ServiceChannelId,
                        Code = typesCache.GetByValue<ExtraSubType>(srcExtras.ExtraSubTypeId),
                        ServiceGuid = srcExtras.ServiceId,
                        Type = typesCache.GetByValue<ExtraType>(srcExtras.ExtraSubType.ExtraTypeId)
                    };

                    if (srcExtras.ServiceServiceChannelExtraTypeDescriptions != null && srcExtras.ServiceServiceChannelExtraTypeDescriptions.Count > 0)
                    {
                        List<VmOpenApiLanguageItem> descriptionList = new List<VmOpenApiLanguageItem>(srcExtras.ServiceServiceChannelExtraTypeDescriptions.Count);

                        srcExtras.ServiceServiceChannelExtraTypeDescriptions.ForEach(srcDesc =>
                        {
                            VmOpenApiLanguageItem desc = new VmOpenApiLanguageItem()
                            {
                                Language = languageCache.GetByValue(srcDesc.LocalizationId),
                                Value = srcDesc.Description
                            };

                            descriptionList.Add(desc);
                        });

                        // set the description list
                        extra.Description = descriptionList;
                    }

                    vmssc.ExtraTypes.Add(extra);
                });
            }
                       
            // servicehours
            if (ssc.ServiceServiceChannelServiceHours != null && ssc.ServiceServiceChannelServiceHours.Count > 0)
            {
                ssc.ServiceServiceChannelServiceHours.ForEach(srcHour =>
                {
                    //// use the translator
                    //var translated = TranslationManagerToVm.Translate<ServiceServiceChannelServiceHours, V4VmOpenApiServiceHour>(srcHour);

                    //if (translated != null)
                    //{
                    //    vmssc.ServiceHours.Add(translated);
                    //}

                    // manual mapping
                    if (srcHour != null && srcHour.ServiceHours != null)
                    {
                        var sh = srcHour.ServiceHours;
                        var type = typesCache.GetByValue<ServiceHourType>(sh.ServiceHourTypeId);
                        IList<VmOpenApiLanguageItem> aiList = new List<VmOpenApiLanguageItem>();
                        if (sh.AdditionalInformations?.Count > 0)
                        {
                            sh.AdditionalInformations.ForEach(info =>
                            {
                                aiList.Add(new VmOpenApiLanguageItem { Value = string.IsNullOrEmpty(info.Text) ? null : info.Text, Language = languageCache.GetByValue(info.LocalizationId) });
                            });
                        }
                        IList<V2VmOpenApiDailyOpeningTime> times = new List<V2VmOpenApiDailyOpeningTime>();
                        if (sh.DailyOpeningTimes?.Count > 0)
                        {
                            sh.DailyOpeningTimes.ForEach(da =>
                            {
                                times.Add(new V2VmOpenApiDailyOpeningTime
                                {
                                    DayFrom = ((WeekDayEnum)da.DayFrom).ToString(),
                                    DayTo = da.DayTo.HasValue ? ((WeekDayEnum)da.DayTo).ToString() : string.Empty,
                                    From = da.From.ToString(),
                                    To = da.To.ConvertToString()
                                });
                            });
                        }
                        var vm = new V4VmOpenApiServiceHour
                        {
                            ServiceHourType = type,
                            ValidFrom = sh.OpeningHoursFrom.HasValue ? DateTime.SpecifyKind(sh.OpeningHoursFrom.Value, DateTimeKind.Utc).ToLocalTime() : sh.OpeningHoursFrom,
                            ValidTo = sh.OpeningHoursTo.HasValue ? DateTime.SpecifyKind(sh.OpeningHoursTo.Value, DateTimeKind.Utc).ToLocalTime() : sh.OpeningHoursTo,
                            IsClosed = sh.IsClosed,
                            AdditionalInformation = aiList,
                            OpeningHour = times,
                            OrderNumber = sh.OrderNumber
                        };
                        vm.CheckServiceHour();
                        vmssc.ServiceHours.Add(vm);
                    }
                    // end manual mapping
                });
            }
        }

        internal List<V4VmOpenApiFintoItem> GetDigitalAuthorizations(ServiceServiceChannel ssc)
        {
            if (ssc.ServiceServiceChannelDigitalAuthorizations?.Count > 0)
            {
                List<V4VmOpenApiFintoItem> authorizations = new List<V4VmOpenApiFintoItem>(ssc.ServiceServiceChannelDigitalAuthorizations.Count);

                ssc.ServiceServiceChannelDigitalAuthorizations.ForEach(srcAuthorization =>
                {
                    //// With translator
                    //var vmAuthorization = TranslationManagerToVm.Translate<DigitalAuthorization, V4VmOpenApiFintoItem>(srcAuthorization.DigitalAuthorization);

                    //if (vmAuthorization != null)
                    //{
                    //    authorizations.Add(vmAuthorization);
                    //}

                    // Manually map
                    if (srcAuthorization != null && srcAuthorization.DigitalAuthorization != null)
                    {
                        var da = srcAuthorization.DigitalAuthorization;
                        var fintoItem = new V4VmOpenApiFintoItem
                        {
                            Code = da.Code,
                            OntologyType = da.OntologyType,
                            Uri = da.Uri,
                            ParentUri = da.ParentUri,
                            ParentId = da.ParentId
                        };
                        authorizations.Add(fintoItem);
                    }
                });

                return authorizations;
            }

            return new List<V4VmOpenApiFintoItem>();
        }

        internal VmOpenApiContactDetails GetContactDetails(ServiceServiceChannel connection, ITypesCache typesCache, ILanguageCache languageCache)
        {
            //// Translator
            //return TranslationManagerToVm.Translate<ServiceServiceChannel, VmOpenApiContactDetails>(connection);

            // Manually map
            if (connection == null) return null;

            var vm = new VmOpenApiContactDetails();
            if (connection.ServiceServiceChannelAddresses?.Count > 0)
            {
                connection.ServiceServiceChannelAddresses.ForEach(a =>
                {
                    // TODO - map manually!
                    vm.Addresses.Add(TranslationManagerToVm.Translate<ServiceServiceChannelAddress, V7VmOpenApiAddress>(a));
                });
            }

            if (connection.ServiceServiceChannelEmails?.Count > 0)
            {
                connection.ServiceServiceChannelEmails.ForEach(e =>
                {
                    if (e.Email != null)
                    {
                        vm.Emails.Add(new VmOpenApiLanguageItem
                        {
                            Value = string.IsNullOrEmpty(e.Email.Value) ? null : e.Email.Value,
                            Language = languageCache.GetByValue(e.Email.LocalizationId)
                        });
                    }
                });
            }

            if (connection.ServiceServiceChannelPhones?.Count > 0)
            {
                connection.ServiceServiceChannelPhones.ForEach(p =>
                {
                    if (p.Phone != null)
                    {
                        var phone = p.Phone;
                        vm.Phones.Add(new V4VmOpenApiPhone
                        {
                            Number = phone.Number,
                            PrefixNumber = phone.PrefixNumber?.Code,
                            AdditionalInformation = phone.AdditionalInformation,
                            ChargeDescription = phone.ChargeDescription,
                            ServiceChargeType = typesCache.GetByValue<ServiceChargeType>(phone.ChargeTypeId),
                            Language = languageCache.GetByValue(phone.LocalizationId),
                            IsFinnishServiceNumber = string.IsNullOrEmpty(phone.PrefixNumber?.Code) ? true : false,
                            OrderNumber = phone.OrderNumber
                        });
                    }
                });
            }

            if (connection.ServiceServiceChannelWebPages?.Count > 0)
            {
                connection.ServiceServiceChannelWebPages.ForEach(w =>
                {
                    if (w.WebPage != null)
                    {
                        var wp = w.WebPage;
                        vm.WebPages.Add(new VmOpenApiWebPageWithOrderNumber
                        {
                            Id = wp.Id,
                            Value = wp.Name,
                            Url = wp.Url,
                            Language = languageCache.GetByValue(wp.LocalizationId),
                            OrderNumber = wp.OrderNumber.HasValue ? wp.OrderNumber.Value.ToString() : null,
                        });
                    }
                });
            }

            if (vm.Addresses?.Count > 0 || vm.Emails?.Count > 0 || vm.Phones?.Count > 0 || vm.WebPages?.Count > 0)
            {
                return vm;
            }

            return null;
        }

        /// <summary>
        /// Tries to get name in the following order: FI, SV and then EN
        /// </summary>
        /// <param name="names">List of names</param>
        /// <returns>service name or null</returns>
        internal string GetNameWithFallback<TName>(ICollection<TName> names, ITypesCache typesCache, ILanguageCache languageCache)
            where TName : IName
        {
            if (names == null || names.Count == 0)
            {
                return null;
            }

            Guid nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());

            // first try to get finnish name
            string sname = GetName(names, languageCache.Get(LanguageCode.fi.ToString()), nameTypeId);

            // did we find FI name
            if (!string.IsNullOrWhiteSpace(sname))
            {
                return sname;
            }

            sname = GetName(names, languageCache.Get(LanguageCode.sv.ToString()), nameTypeId);

            // did we find SV name
            if (!string.IsNullOrWhiteSpace(sname))
            {
                return sname;
            }

            return GetName(names, languageCache.Get(LanguageCode.en.ToString()), nameTypeId);
        }

        /// <summary>
        /// Get name with defined language.
        /// </summary>
        /// <param name="names">List of names</param>
        /// <param name="languageId">what language to get</param>
        /// <param name="nameTypeId">what type of name to get</param>
        /// <returns>name or null</returns>
        private static string GetName<TName>(ICollection<TName> names, Guid languageId, Guid nameTypeId)
            where TName : IName
        {
            if (names == null || names.Count == 0)
            {
                return null;
            }

            return names.Where(sn => sn.LocalizationId == languageId && sn.TypeId == nameTypeId).FirstOrDefault()?.Name;
        }
    }
}