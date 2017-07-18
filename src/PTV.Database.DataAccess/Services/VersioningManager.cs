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
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.EntityCloners;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services
{
    /// <summary>
    /// Manager handling versions of entity which uses IVersioned interface
    /// </summary>
    [RegisterService(typeof(IVersioningManager), RegisterType.Transient)]
    internal class VersioningManager : IVersioningManager
    {
        public readonly List<string> AllowedPublishingStatuses = new List<string>(){ PublishingStatus.Published.ToString(), PublishingStatus.Draft.ToString(), PublishingStatus.Modified.ToString() };
        public readonly List<string> AllowedPublishingStatusesGuids = new List<string>();

        //private readonly ICacheManager cacheManager;
        private readonly ICloningManager cloningManager;
        private readonly ILogger<VersioningManager> logger;
        private Dictionary<PublishingStatus, Guid> publishingStatusesDictionary;
        private readonly ICacheManager cacheManager;

        public VersioningManager(ICacheManager cacheManager, ICloningManager cloningManager, ILogger<VersioningManager> logger)
        {
            //this.cacheManager = cacheManager;
            this.cloningManager = cloningManager;
            this.logger = logger;
            this.cacheManager = cacheManager;
        }

        public bool IsInAllowedPublishingStatus(Guid publishingStatus)
        {
            var ps = cacheManager.TypesCache.GetByValue<PublishingStatusType>(publishingStatus);
            return AllowedPublishingStatuses.Contains(ps);
        }


        private Dictionary<PublishingStatus, Guid> PublishingStatuses
        {
            get
            {
                if (publishingStatusesDictionary == null)
                {
                    var publishingStatusesEnums = (PublishingStatus[]) Enum.GetValues(typeof(PublishingStatus));
                    publishingStatusesDictionary = publishingStatusesEnums.ToDictionary(i => i, i => cacheManager.TypesCache.Get<PublishingStatusType>(i.ToString()));
                }
                return publishingStatusesDictionary;
            }
        }



        public bool IsAllowedForEditing<TEntityType>(ITranslationUnitOfWork unitOfWork, TEntityType entity) where TEntityType : class, IVersionedVolume
        {
            if (entity == null) return false;
            if (entity.PublishingStatusId == PublishingStatuses[PublishingStatus.Draft] || entity.PublishingStatusId == PublishingStatuses[PublishingStatus.Modified] || !entity.UnificRootId.IsAssigned()) return true;
            if (entity.PublishingStatusId == PublishingStatuses[PublishingStatus.Published])
            {
                var latestStatus = GetLatestVersionPublishingStatus<TEntityType>(unitOfWork, entity.UnificRootId);
                var allVersions = GetAllVersions<TEntityType>(unitOfWork, entity.UnificRootId);
                if (allVersions.Any(x=>x.PublishingStatusId == PublishingStatuses[PublishingStatus.Modified]))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool IsAllowedForEditing<TEntityType>(ITranslationUnitOfWork unitOfWork, Guid entityId) where TEntityType : class, IVersionedVolume
        {
            var entity = unitOfWork.GetSet<TEntityType>().FirstOrDefault(i => i.Id == entityId);
            return IsAllowedForEditing(unitOfWork, entity);
        }

        /// <summary>
        /// Get and return all available versions of entity
        /// </summary>
        /// <typeparam name="TEntityType"></typeparam>
        /// <param name="unitOfWork">Unit Of Work must be provided for correct functionality</param>
        /// <param name="entity">Entity which versions will be retrieved</param>
        /// <returns>List of available versions of specified entity</returns>
        public List<VersionInfo> GetAllVersions<TEntityType>(ITranslationUnitOfWork unitOfWork, TEntityType entity) where TEntityType : class, IVersionedVolume
        {
            return entity?.VersioningId == null ? new List<VersionInfo>() : GetAllVersions<TEntityType>(unitOfWork, entity.UnificRootId);
        }

        /// <summary>
        /// Get and return all available versions of entity
        /// </summary>
        /// <typeparam name="TEntityType"></typeparam>
        /// <param name="unitOfWork">Unit Of Work must be provided for correct functionality</param>
        /// <param name="unificRootId">Entity unificrootId which versions will be retrieved</param>
        /// <returns>List of available versions of specified entity</returns>
        public List<VersionInfo> GetAllVersions<TEntityType>(ITranslationUnitOfWork unitOfWork, Guid unificRootId) where TEntityType : class, IVersionedVolume
        {
            var entityRep = unitOfWork.GetSet<TEntityType>();
            List<TEntityType> relatedEntitiesList = entityRep.Where(i => (i.UnificRootId == unificRootId) && (i.VersioningId != null)).Include(i => i.Versioning).ToList();
            return relatedEntitiesList.Select(i => new VersionInfo()
            {
                EntityId = i.Id,
                VersionMajor = i.Versioning.VersionMajor,
                VersionMinor = i.Versioning.VersionMinor,
                PublishingStatusId = i.PublishingStatusId,
                // ReSharper disable once PossibleInvalidOperationException
                VersionId = i.VersioningId.Value
            })
                    .OrderBy(i => i.VersionMajor)
                    .ThenBy(i => i.VersionMinor)
                    .ToList();
        }

        /// <summary>
        /// Get last publish version of entity
        /// </summary>
        /// <typeparam name="TEntityType"></typeparam>
        /// <param name="unitOfWork">Unit Of Work must be provided for correct functionality</param>
        /// <param name="unificRootId">Entity unificrootId which version will be retrieved</param>
        /// <returns>Available publish version of specified entity</returns>
        public VersionInfo GetLastPublishedVersion<TEntityType>(ITranslationUnitOfWork unitOfWork, Guid unificRootId) where TEntityType : class, IVersionedVolume
        {
            return GetAllVersions<TEntityType>(unitOfWork, unificRootId).LastOrDefault(x => x.PublishingStatusId == PublishingStatuses[PublishingStatus.Published]);
        }

        /// <summary>
        /// Get last modified version of entity
        /// </summary>
        /// <typeparam name="TEntityType"></typeparam>
        /// <param name="unitOfWork">Unit Of Work must be provided for correct functionality</param>
        /// <param name="unificRootId">Entity unificrootId which version will be retrieved</param>
        /// <returns>Available modified version of specified entity</returns>
        public VersionInfo GetLastModifiedVersion<TEntityType>(ITranslationUnitOfWork unitOfWork, Guid unificRootId) where TEntityType : class, IVersionedVolume
        {
            return GetAllVersions<TEntityType>(unitOfWork, unificRootId).LastOrDefault(x => x.PublishingStatusId == PublishingStatuses[PublishingStatus.Draft] || x.PublishingStatusId == PublishingStatuses[PublishingStatus.Modified]);
        }

        /// <summary>
        /// Acquire UnificRootId from versioned entity by its ID
        /// </summary>
        /// <typeparam name="T">Type of versioned entity</typeparam>
        /// <param name="unitOfWork">Instance of unit of work</param>
        /// <param name="versionedEntityId">ID of versioned entity</param>
        /// <returns></returns>
        public Guid? GetUnificRootId<T>(ITranslationUnitOfWork unitOfWork, Guid? versionedEntityId) where T : class, IVersionedVolume
        {
            if (versionedEntityId == null) return null;
            return unitOfWork.GetSet<T>().Where(i => i.Id == versionedEntityId).Select(i => i.UnificRootId).FirstOrDefault();
        }

        /// <summary>
        /// Ensure that root entity is properly created for versioned entity. If not, then new root is created.
        /// </summary>
        /// <typeparam name="TRootType">Type of unific root related to versioned entity</typeparam>
        /// <param name="entity">Instance of entity which should be checked</param>
        public void EnsureUnificRoot<TRootType>(IVersionedVolume<TRootType> entity) where TRootType : IVersionedRoot, new()
        {
            if (entity.UnificRootId == Guid.Empty)
            {
                entity.UnificRoot = new TRootType() { Id = Guid.NewGuid() };
                entity.UnificRootId = entity.UnificRoot.Id;
            }
        }

        /// <summary>
        /// Apply filter for publishing statuses to filter out entity by its status. Get Draft and Published only.
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be filtered</typeparam>
        /// <param name="query">Query on which the filter will be applied</param>
        /// <returns></returns>
        public IQueryable<TEntityType> ApplyPublishingStatusFilter<TEntityType>(IQueryable<TEntityType> query) where TEntityType : IVersionedVolume
        {
            var publishingStatusPublishedId = PublishingStatuses[PublishingStatus.Published];
            var publishingStatusDraftdId = PublishingStatuses[PublishingStatus.Draft];
            return query.Where(i => i.PublishingStatusId == publishingStatusPublishedId || i.PublishingStatusId == publishingStatusDraftdId);
        }

        /// <summary>
        /// Apply filter for publishinf statuses to filter out entity by its status. Get Draft and Published only.
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be filtered</typeparam>
        /// <param name="query">Query on which the filter will be applied</param>
        /// <returns></returns>
        public IEnumerable<TEntityType> ApplyPublishingStatusFilter<TEntityType>(IEnumerable<TEntityType> query) where TEntityType : IVersionedVolume
        {
            if (query == null) return null;
            var publishingStatusPublishedId = PublishingStatuses[PublishingStatus.Published];
            var publishingStatusDraftdId = PublishingStatuses[PublishingStatus.Draft];
            return query.Where(i => i.PublishingStatusId == publishingStatusPublishedId || i.PublishingStatusId == publishingStatusDraftdId);
        }

        /// <summary>
        /// Apply publishing status filter with fallback, i.e. try to get instance in Published state, if does not exist, then take Draft one, then Modified one.
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be filtered</typeparam>
        /// <param name="collection">Collection of entities on which the fallback filter will be applied</param>
        /// <returns></returns>
        public TEntityType ApplyPublishingStatusFilterFallback<TEntityType>(IEnumerable<TEntityType> collection) where TEntityType : class, IVersionedVolume
        {
            if (collection == null) return null;
            return collection.FirstOrDefault(i => i.PublishingStatusId == PublishingStatuses[PublishingStatus.Published]) ??
                   collection.FirstOrDefault(i => i.PublishingStatusId == PublishingStatuses[PublishingStatus.Draft]) ??
                   collection.FirstOrDefault(i => i.PublishingStatusId == PublishingStatuses[PublishingStatus.Modified]);
        }

        /// <summary>
        /// Apply publishing status filter with fallback, i.e. try to get instance in Published state, if does not exist, then take Draft one, then Modified one.
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be filtered</typeparam>
        /// <param name="collection">Collection of entities on which the fallback filter will be applied</param>
        /// <returns></returns>
        public TEntityType ApplyPublishingStatusFilterFallback<TEntityType>(IQueryable<TEntityType> collection) where TEntityType : class, IVersionedVolume
        {
            return collection.FirstOrDefault(i => i.PublishingStatusId == PublishingStatuses[PublishingStatus.Published]) ??
                   collection.FirstOrDefault(i => i.PublishingStatusId == PublishingStatuses[PublishingStatus.Draft]) ??
                   collection.FirstOrDefault(i => i.PublishingStatusId == PublishingStatuses[PublishingStatus.Modified]);
        }

        public TEntityType ApplyLanguageFilterFallback<TEntityType>(IEnumerable<TEntityType> collection, Guid? requestedLanguage) where TEntityType : class, IName
        {
            if (collection == null) return null;
            var result = collection.FirstOrDefault(i => i.LocalizationId == requestedLanguage) ??
                         collection.Select(i => new {entity = i, order = cacheManager.LanguageOrderCache.Get(i.LocalizationId)})
                             .OrderBy(i => i.order)
                             .Select(i => i.entity)
                             .FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Apply filter for publishing statuses to filter out entity by its status order by priority fallback. Get Published, Draft or modified version.
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be filtered</typeparam>
        /// <param name="query">Query on which the filter will be applied</param>
        /// <returns></returns>
        public IQueryable<TEntityType> ApplyPublishingStatusOrderByPriorityFallback<TEntityType>(IQueryable<TEntityType> query) where TEntityType : IVersionedVolume
        {
            var publishingStatusPublishedId = PublishingStatuses[PublishingStatus.Published];
            var publishingStatusDraftdId = PublishingStatuses[PublishingStatus.Draft];
            var publishingStatusModifiedId = PublishingStatuses[PublishingStatus.Modified];
            return query
                    .Where(i => i.PublishingStatusId == publishingStatusPublishedId
                             || i.PublishingStatusId == publishingStatusDraftdId
                             || i.PublishingStatusId == publishingStatusModifiedId)
                    .GroupBy(i => i.UnificRootId)
                    .Select(x => x.OrderBy(y => y.PublishingStatusId == publishingStatusPublishedId ? 0 : y.PublishingStatusId == publishingStatusDraftdId ? 1 : y.PublishingStatusId == publishingStatusModifiedId ? 2 : 3).FirstOrDefault());
            //.Select(x => x.OrderBy(y => y.PublishingStatus.PriorityFallback).FirstOrDefault());
        }

        /// <summary>
        /// Get versioned entity with specific publishing status.
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be filtered</typeparam>
        /// <param name="unitOfWork">Instance of unit of work</param>
        /// <param name="entity">Instance of entity which is used as input for searching specific version</param>
        /// <param name="publishingStatus">Searching criteria for publishing status</param>
        /// <returns></returns>
        public TEntityType GetSpecificVersion<TEntityType>(ITranslationUnitOfWork unitOfWork, TEntityType entity, PublishingStatus publishingStatus) where TEntityType : class, IVersionedVolume, new()
        {
            var publishingStatusId = PublishingStatuses[publishingStatus];
            if (entity.PublishingStatusId == publishingStatusId) return entity;
            return GetSpecificVersionByRoot<TEntityType>(unitOfWork, entity.UnificRootId, publishingStatus);
        }

        /// <summary>
        /// Get versioned entity with specific publishing status.
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be filtered</typeparam>
        /// <param name="unitOfWork">Instance of unit of work</param>
        /// <param name="rootId">ID of unific root which is used as input for searching its specific version</param>
        /// <param name="publishingStatus">Searching criteria for publishing status</param>
        /// <returns></returns>
        public TEntityType GetSpecificVersionByRoot<TEntityType>(ITranslationUnitOfWork unitOfWork, Guid rootId, PublishingStatus publishingStatus) where TEntityType : class, IVersionedVolume, new()
        {
            var publishingStatusId = PublishingStatuses[publishingStatus];
            var entityRep = unitOfWork.GetSet<TEntityType>();
            return entityRep.FirstOrDefault(i => i.UnificRootId == rootId && i.PublishingStatusId == publishingStatusId);
        }

        /// <summary>
        ///Get entity from collection of entities which is Published or Draft
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be filtered</typeparam>
        /// <param name="entities">Collection of entities which will filtered</param>
        /// <returns></returns>
        public TEntityType GetNotModifiedVersion<TEntityType>(IEnumerable<TEntityType> entities) where TEntityType : class, IVersionedVolume, new()
        {
            if (entities == null) return null;
            var publishingStatusPublishedId = PublishingStatuses[PublishingStatus.Published];
            var publishingStatusDraftdId = PublishingStatuses[PublishingStatus.Draft];
            var result = entities.FirstOrDefault(i => i.PublishingStatusId == publishingStatusPublishedId) ??
                         entities.FirstOrDefault(i => i.PublishingStatusId == publishingStatusDraftdId);
            return result;
        }


        private TEntityType GetRelatedEntity<TEntityType>(ITranslationUnitOfWork unitOfWork, TEntityType entity, VersionInfo currentLatest)
            where TEntityType : class, IVersionedVolume, new()
        {
            if (currentLatest.EntityId == entity.Id)
            {
                return entity;
            }
            throw new PtvAppException(CoreMessages.MainPublishedEntityLocked, CoreMessages.MainPublishedEntityLockedId);
        }

        private bool AllowedToProcessVersioned<TEntityType>(TEntityType entity) where TEntityType : class, IVersionedVolume
        {
            return (entity.PublishingStatusId == Guid.Empty) ||
                   (entity.PublishingStatusId == PublishingStatuses[PublishingStatus.Published]) ||
                   (entity.PublishingStatusId == PublishingStatuses[PublishingStatus.Modified]) ||
                   (entity.PublishingStatusId == PublishingStatuses[PublishingStatus.Draft]);
        }


        /// <summary>
        /// Create modified version of entity if it is needed. Status of entity is checked, if it is already Published, then entity is copied and marked as Modified.
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be checked and cloned if needed.</typeparam>
        /// <param name="unitOfWork">Instance of unit of work</param>
        /// <param name="entity">Entity which will be checked and cloned if needed.</param>
        /// <param name="versioningMode">Optional parameter how versionig should work, default is Standard mode, when Modified is created in case of update operation</param>
        /// <returns></returns>
        public TEntityType CreateModifiedVersion<TEntityType>(ITranslationUnitOfWork unitOfWork, TEntityType entity, VersioningMode versioningMode = VersioningMode.Standard) where TEntityType : class, IVersionedVolume, new()
        {
            if (entity == null) return null;
            if (!AllowedToProcessVersioned(entity))
            {
                throw new Exception(CoreMessages.CannotProcessThisVersion);
            }
            var allVersions = GetAllVersions(unitOfWork, entity);
            var versioningSet = unitOfWork.GetSet<Versioning>();
            var currentLatest = allVersions.LastOrDefault();
            if (currentLatest == null)
            {
                var publishingStatusDraftId = PublishingStatuses[PublishingStatus.Draft];
                if ((entity.PublishingStatusId == Guid.Empty) || (entity.PublishingStatusId == publishingStatusDraftId))
                {
                    entity.PublishingStatusId = publishingStatusDraftId;
                    currentLatest = new VersionInfo() {VersionMajor = 0, VersionMinor = 1};
                }
                else
                {
                    currentLatest = new VersionInfo() { VersionMajor = 1, VersionMinor = 0 };
                }
                return SetVersioning(entity, versioningSet, currentLatest);
            }
            if (versioningMode == VersioningMode.UseTheLatest)
            {
                return GetRelatedEntity(unitOfWork, entity, currentLatest);
            }
            if (versioningMode == VersioningMode.UseDraftOrPublishedOnly)
            {
                return GetRelatedEntity(unitOfWork, entity, allVersions.LastOrDefault(i => i.PublishingStatusId == PublishingStatuses[PublishingStatus.Published] || i.PublishingStatusId == PublishingStatuses[PublishingStatus.Draft]));
            }
            if (versioningMode == VersioningMode.UsePublishedOnly)
            {
                return GetRelatedEntity(unitOfWork, entity, allVersions.LastOrDefault(i => i.PublishingStatusId == PublishingStatuses[PublishingStatus.Published]));
            }

            var modifiedVersions = allVersions.Where(i => (i.PublishingStatusId == PublishingStatuses[PublishingStatus.Modified]) || (i.PublishingStatusId == PublishingStatuses[PublishingStatus.Draft])).ToList();

            if (modifiedVersions.Any())
            {
                var modifiedVersion = modifiedVersions.LastOrDefault(i => i.EntityId == entity.Id) ?? modifiedVersions.Last();
                currentLatest.VersionMinor++;
                var verInfo = versioningSet.First(i => i.Id == modifiedVersion.VersionId);
                verInfo.VersionMinor = currentLatest.VersionMinor;
                verInfo.VersionMajor = currentLatest.VersionMajor;
                return GetRelatedEntity(unitOfWork, entity, modifiedVersion);
            }
            var previousVersionId = entity.VersioningId;
            var cloned = cloningManager.CloneEntity(entity, unitOfWork);
            currentLatest.VersionMinor++;
            SetVersioning(cloned, versioningSet, currentLatest, previousVersionId, true);
            cloned.PublishingStatusId = PublishingStatuses[PublishingStatus.Modified];
            return cloned;
        }

        private TEntityType SetVersioning<TEntityType>(TEntityType entity, DbSet<Versioning> versioningDbSet, VersionInfo currentLatest, Guid? previousVersionId = null, bool setNewVersion = false) where TEntityType : class, IVersionedVolume
        {
            if ((!setNewVersion) && (entity.Versioning == null) && (entity.VersioningId != null))
            {
                entity.Versioning = versioningDbSet.FirstOrDefault(i => i.Id == entity.VersioningId);
            }
            if (setNewVersion || entity.Versioning == null)
            {
                entity.Versioning = AddAndReturn(versioningDbSet, new Versioning() { Id = Guid.NewGuid() });
            }
            currentLatest = currentLatest ?? new VersionInfo() {VersionMajor = 0, VersionMinor = 1};
            entity.Versioning.VersionMajor = currentLatest.VersionMajor;
            entity.Versioning.VersionMinor = currentLatest.VersionMinor;
            entity.Versioning.PreviousVersionId = previousVersionId;
            entity.VersioningId = entity.Versioning.Id;
            return entity;
        }

        private T AddAndReturn<T>(DbSet<T> dbSet, T item) where T: class
        {
            dbSet.Add(item);
            return item;
        }

        public IList<PublishingAffectedResult> ChangeToModified<TEntityType>(ITranslationUnitOfWork unitOfWork, TEntityType entity, List<PublishingStatus> onlyAllowedSourceState = null) where TEntityType : class, IVersionedVolume
        {
            var result = new List<PublishingAffectedResult>();
            if (onlyAllowedSourceState != null && !onlyAllowedSourceState.Select(i => PublishingStatuses[i]).Contains(entity.PublishingStatusId))
            {
                return result;
            }
            var entitySet = unitOfWork.GetSet<TEntityType>();
            var modifiedExists = entitySet.Any(i => i.UnificRootId == entity.UnificRootId && (i.PublishingStatusId == PublishingStatuses[PublishingStatus.Modified] || i.PublishingStatusId == PublishingStatuses[PublishingStatus.Draft]));
            if (modifiedExists)
            {
                throw new InvalidOperationException();
            }
            var targetPublishingStatus = PublishingStatuses[PublishingStatus.Modified];
            result.Add(new PublishingAffectedResult() { Id = entity.Id, PublishingStatusOld = entity.PublishingStatusId, PublishingStatusNew = targetPublishingStatus });
            entity.PublishingStatus = null;
            entity.PublishingStatusId = targetPublishingStatus;
            var versioningSet = unitOfWork.GetSet<Versioning>();
            if (entity.VersioningId != null && entity.Versioning == null)
            {
                entity.Versioning = versioningSet.FirstOrDefault(i => i.Id == entity.VersioningId);
            }
            var allVersions = GetAllVersions(unitOfWork, entity);
            var lastVersion = allVersions.LastOrDefault() ?? new VersionInfo() { EntityId = entity.Id, VersionMajor = 0, VersionMinor = 0 };
            lastVersion.VersionMinor++;
            SetVersioning(entity, versioningSet, lastVersion, entity.Versioning?.PreviousVersionId );
            return result;
        }

        /// <summary>
        /// Publish specified entity, check latest version and create new version with published state
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be promoted to published state</typeparam>
        /// <param name="unitOfWork">Unit Of Work</param>
        /// <param name="entity">Entity which will be promoted to published state</param>
        /// <param name="targetPublishingStatus">Entity which will be promoted to published state</param>
        public IList<PublishingAffectedResult> PublishVersion<TEntityType>(ITranslationUnitOfWork unitOfWork, TEntityType entity, PublishingStatus targetPublishingStatus = PublishingStatus.Published) where TEntityType : class, IVersionedVolume, new()
        {
            var entitySet = unitOfWork.GetSet<TEntityType>();
            var allVersions = GetAllVersions(unitOfWork, entity);
            var currentLatest = allVersions.LastOrDefault() ?? new VersionInfo();
            var result = new List<PublishingAffectedResult>();
            if ((currentLatest.PublishingStatusId == PublishingStatuses[targetPublishingStatus]) && (currentLatest.VersionId == entity.VersioningId))
            {
                logger.LogDebug($"Publishing not needed, already in desired target publishing state. {entity.GetType().Name}, Id: {entity.Id}");
                return result;
            }
            Guid? originVersioning = null;
            if (targetPublishingStatus == PublishingStatus.Published)
            {
                var previousPublishedVersion = allVersions.LastOrDefault(i => i.PublishingStatusId == PublishingStatuses[PublishingStatus.Published]);
                originVersioning = previousPublishedVersion?.VersionId;
                if (previousPublishedVersion != null)
                {
                    var previousEntity = entitySet.FirstOrDefault(i => i.Id == previousPublishedVersion.EntityId);
                    result.Add(new PublishingAffectedResult()
                    {
                        Id = previousEntity.Id,
                        PublishingStatusOld = previousEntity.PublishingStatusId,
                        PublishingStatusNew = PublishingStatuses[PublishingStatus.OldPublished]
                    });
                    previousEntity.PublishingStatus = null;
                    previousEntity.PublishingStatusId = PublishingStatuses[PublishingStatus.OldPublished];
                }
                currentLatest.VersionMajor++;
                currentLatest.VersionMinor = 0;
            }
            else
            {
                if (targetPublishingStatus == PublishingStatus.Modified)
                {
                    if (allVersions.Any(i => i.PublishingStatusId == PublishingStatuses[PublishingStatus.Modified]))
                    {
                        throw new PublishModifiedExistsException();
                    }
                    currentLatest.VersionMinor++;
                    var versioningSet = unitOfWork.GetSet<Versioning>();
                    originVersioning = entity.VersioningId != null ? versioningSet.Where(i => i.Id == entity.VersioningId).Select(i => i.PreviousVersionId).FirstOrDefault() : null;
                }
                else
                {
                    throw new InvalidOperationException($"It is not allowed to 'publish' entity to {targetPublishingStatus} state.");
                }
            }
            result.Add(new PublishingAffectedResult() { Id = entity.Id, PublishingStatusOld = entity.PublishingStatusId, PublishingStatusNew = PublishingStatuses[targetPublishingStatus] });
            entity.PublishingStatus = null;
            entity.PublishingStatusId = PublishingStatuses[targetPublishingStatus];
            SetVersioning(entity, unitOfWork.GetSet<Versioning>(), currentLatest, originVersioning);
            return result;
        }

        /// <summary>
        /// Change publishing status of language version of specific entity which is of IMultilanguagedEntity type
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <typeparam name="TLang">Type of langauge version relation</typeparam>
        /// <param name="unitOfWork">Unit of work instance</param>
        /// <param name="entity">Entity to change</param>
        /// <param name="publishingStatusTo">Target publishing status of language version</param>
        /// <param name="publishingStatusFrom">Input criteria for selecting the language versions which will be switched</param>
        /// <param name="languageGuids">Input criteria for selecting the language versions which will be switched</param>
        public void ChangeStatusOfLanguageVersion<T, TLang>(ITranslationUnitOfWork unitOfWork, T entity, PublishingStatus publishingStatusTo, IEnumerable<PublishingStatus> publishingStatusFrom = null, IEnumerable<Guid> languageGuids = null) where T : class, IMultilanguagedEntity<TLang>, new() where TLang : class, ILanguageAvailability
        {
            if (entity == null) return;
            unitOfWork.LoadNavigationProperty(entity, i => i.LanguageAvailabilities);
            var publishingStatusToId = PublishingStatuses[publishingStatusTo];
            var applyOn = languageGuids == null ? entity.LanguageAvailabilities : entity.LanguageAvailabilities.Where(i => languageGuids.Contains(i.LanguageId));
            publishingStatusFrom?.ForEach(status =>
            {
                var publishingStatusFromId = PublishingStatuses[status];
                applyOn = applyOn.Where(i => i.StatusId == publishingStatusFromId);
            });
            applyOn.ForEach(i => i.StatusId = publishingStatusToId);
        }

        /// <summary>
        /// Change publishing status of language version of specific entity which is of IMultilanguagedEntity type
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <typeparam name="TLang">Type of langauge version relation</typeparam>
        /// <param name="unitOfWork">Unit of work instance</param>
        /// <param name="entity">Entity to change</param>
        /// <param name="languageAvailabilities">languages to change</param>
        public void ChangeStatusOfLanguageVersion<T, TLang>(ITranslationUnitOfWork unitOfWork, T entity, IEnumerable<VmLanguageAvailabilityInfo> languageAvailabilities) where T : class, IMultilanguagedEntity<TLang>, new() where TLang : class, ILanguageAvailability
        {
            if (entity == null) return;
            if (!entity.LanguageAvailabilities.Any())
            {
                unitOfWork.LoadNavigationProperty(entity, i => i.LanguageAvailabilities);
            }
            entity.LanguageAvailabilities.ForEach(langAvail =>
            {
                var newStatusId = languageAvailabilities.FirstOrDefault(j => j.LanguageId == langAvail.LanguageId)?.StatusId;
                if (newStatusId.IsAssigned())
                {
                    langAvail.StatusId = newStatusId.Value;
                }
            });
        }

        /// <summary>
        /// Change publishing status of language version of specific entity which is of IMultilanguagedEntity type
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <typeparam name="TLang">Type of langauge version relation</typeparam>
        /// <param name="unitOfWork">Unit of work instance</param>
        /// <param name="entityId">Entity to change</param>
        /// <param name="languageAvailabilities">languages to change</param>
        public void ChangeStatusOfLanguageVersion<T, TLang>(ITranslationUnitOfWork unitOfWork, Guid entityId, IEnumerable<VmLanguageAvailabilityInfo> languageAvailabilities) where T : class, IEntityIdentifier, IMultilanguagedEntity<TLang>, new() where TLang : class, ILanguageAvailability
        {
            var entity = unitOfWork.GetSet<T>().Include(i => i.LanguageAvailabilities).FirstOrDefault(i => i.Id == entityId);
            ChangeStatusOfLanguageVersion<T, TLang>(unitOfWork, entity, languageAvailabilities);
        }

        public bool FilterByPublishingStatus<TEntityType>(TEntityType entity) where TEntityType : IVersionedVolume
        {
            var publishingStatusPublishedId = PublishingStatuses[PublishingStatus.Published];
            var publishingStatusDraftdId = PublishingStatuses[PublishingStatus.Draft];
            return entity.PublishingStatusId == publishingStatusPublishedId || entity.PublishingStatusId == publishingStatusDraftdId;
        }


        public List<LangaugesAvailabilityStatus> GetAvailableLanguages<T>(IMultilanguagedEntity<T> entity) where T : ILanguageAvailability
        {
            if (entity.LanguageAvailabilities == null)
            {
                throw new Exception($"LanguageAvailabilities property of type {entity.GetType().Name} is not loaded (Include is missing)");
            }
            return entity.LanguageAvailabilities.Select(i => new LangaugesAvailabilityStatus() {LanguageId = i.LanguageId, StatusId = i.StatusId}).ToList();
        }

        public Guid? GetVersionId<TEntityType>(ITranslationUnitOfWork unitOfWork, Guid rootId, PublishingStatus? publishingStatus = null) where TEntityType : class, IVersionedVolume
        {
            if (!rootId.IsAssigned()) return null;
            var entityRep = unitOfWork.GetSet<TEntityType>();
            if (publishingStatus == null)
            {
                // Get the latest version regardless of the publishing status
                List<TEntityType> allVersionsList = entityRep.Where(i => (i.UnificRootId == rootId) && (i.VersioningId != null)).Include(i => i.Versioning).ToList();
                return allVersionsList
                    .OrderBy(i => i.Versioning.VersionMajor)
                    .ThenBy(i => i.Versioning.VersionMinor).Select(i => i.Id).LastOrDefault();
            }

            var publishingStatusId = PublishingStatuses[publishingStatus.Value];
            var entity = entityRep.FirstOrDefault(i => i.UnificRootId == rootId && i.PublishingStatusId == publishingStatusId);

            if (entity == null) return null;

            return entity.Id;
        }

        public PublishingStatus? GetLatestVersionPublishingStatus<TEntityType>(ITranslationUnitOfWork unitOfWork, Guid rootId) where TEntityType : class, IVersionedVolume
        {
            var latestVersion = GetAllVersions<TEntityType>(unitOfWork, rootId).LastOrDefault();
            if (latestVersion != null) { return PublishingStatuses.FirstOrDefault(x => x.Value == latestVersion.PublishingStatusId).Key; }
            return null;
        }
    }

    /// <summary>
    /// Version type
    /// </summary>
    public enum EVersionType
    {
        Major,
        Minor
    }


    public class LangaugesAvailabilityStatus
    {
        public Guid LanguageId { get; set; }
        public Guid StatusId { get; set; }
    }
}
