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
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof (IExpirationService), RegisterType.Transient)]
    internal class ExpirationService : ServiceBase, IExpirationService
    {
        private readonly IExpirationTimeCache expirationTimeCache;
        private readonly ITypesCache typesCache;
        private readonly IPahaTokenAccessor pahaTokenAccessor;
        private readonly ICommonServiceInternal commonService;
        private readonly IPublishingStatusCache publishingStatusCache;

        public ExpirationService(
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IVersioningManager versioningManager,
            ICommonServiceInternal commonService,
            IPahaTokenAccessor pahaTokenAccessor,
            ICacheManager cacheManager,
            IExpirationTimeCache expirationTimeCache)
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker, versioningManager)
        {
            this.expirationTimeCache = expirationTimeCache;
            this.typesCache = cacheManager.TypesCache;
            this.publishingStatusCache = cacheManager.PublishingStatusCache;
            this.pahaTokenAccessor = pahaTokenAccessor;
            this.commonService = commonService;
        }

        // SFIPTV-1921
        public DateTime? GetExpirationDate<TEntity>(IUnitOfWork unitOfWork, TEntity entity, DateTime? utcNow = null, DateTime? lastChangeDate = null)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IExpirable, new()
        {
            var normalizedStatusId = NormalizePublishingStatusId<TEntity>(unitOfWork, entity.UnificRootId, entity.PublishingStatusId);
            if (normalizedStatusId == null)
                return null;

            var expirationTime = expirationTimeCache.GetExpirationTimes(entity.GetType(), normalizedStatusId.Value);
            if (expirationTime == null)
                return null;

            var modifiedDate = lastChangeDate ?? GetNonEvaModifiedDate(unitOfWork, entity);
            return CalculateExpirationDate(modifiedDate, expirationTime.Value.Modified, utcNow ?? DateTime.UtcNow);
        }

        public void SetExpirationDatesForDraft<TEntity>(IUnitOfWorkWritable unitOfWork, IEnumerable<Guid> ids, bool allowAnonymous = false) 
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IExpirable, new()
        {
            var repo = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entities = repo.All().Where(x => ids.Contains(x.Id)).ToList();

            foreach (var entity in entities)
            {
                entity.Expiration = GetExpirationDate(unitOfWork, entity, lastChangeDate: DateTime.UtcNow);
            }
            unitOfWork.Save(allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal, PreSaveAction.DoNotSetAudits);
        }

        public void SetExpirationDate<TEntity>(IUnitOfWork unitOfWork, TEntity entity, ActionTypeEnum actionType, bool isEeva)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IExpirable, new()
        {
            // Eeva publish should not affect expiration date, unless there is no previous modified date set.
            var modificationDate = (isEeva && actionType == ActionTypeEnum.SaveAndPublish && entity.Modified != DateTime.MinValue)
                ? GetNonEvaModifiedDate(unitOfWork, entity)
                : DateTime.UtcNow;
            entity.Expiration = GetExpirationDate(unitOfWork, entity, DateTime.UtcNow, modificationDate);
        }

        public void SetExpirationDateForPublishing<TEntity>(IUnitOfWorkWritable unitOfWork, IEnumerable<Guid> ids, bool isEeva, bool allowAnonymous = false)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IExpirable, new()
        {
            var repo = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entities = repo.All().Where(x => ids.Contains(x.Id)).ToList();

            foreach (var entity in entities)
            {
                // Eeva publish should not affect expiration date.
                var modificationDate = isEeva ? GetNonEvaModifiedDate(unitOfWork, entity) : DateTime.UtcNow;
                entity.Expiration = GetExpirationDate(unitOfWork, entity, lastChangeDate: modificationDate);
            }
            unitOfWork.Save(allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal, PreSaveAction.DoNotSetAudits);
        }

        public void SetExpirationDateForPublishing<TEntity>(IContextManager contextManager, IEnumerable<Guid> ids, bool isEeva, bool allowAnonymous = false)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IExpirable, new()
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SetExpirationDateForPublishing<TEntity>(unitOfWork, ids, isEeva, allowAnonymous);
            });
        }

        public void PolyfillExpirationDate<TEntity, TLanguageAvailability>(IUnitOfWorkWritable unitOfWork, TEntity entity, DateTime utcNow)
            where TEntity : class, IAuditing, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            var normalizedStatusId = NormalizePublishingStatusId<TEntity>(unitOfWork, entity.UnificRootId, entity.PublishingStatusId);
            if (normalizedStatusId == null)
                return;

            var expirationTime = expirationTimeCache.GetExpirationTimes(entity.GetType(), normalizedStatusId.Value);
            if (expirationTime == null)
                return;

            var expirationPeriod = expirationTime.Value.Expires - utcNow;

            var scheduledPublishingDate = entity.LanguageAvailabilities.Max(x => x.PublishAt);
            if (scheduledPublishingDate > utcNow)
            {
                entity.Expiration = scheduledPublishingDate + expirationPeriod;
                return;
            }

            var modifiedDate = GetNonEvaModifiedDate(unitOfWork, entity);
            entity.Expiration = modifiedDate + expirationPeriod;
        }

        // SFIPTV-1921
        public DateTime GetNonEvaModifiedDate(DateTime modified, LastOperationType lastOperationType, Guid? versioningId, IUnitOfWork unitOfWork)
        {
            // If the entity was not published by Eeva or we don't have any versioning info, return modified.
            if (!lastOperationType.HasFlag(LastOperationType.PublishedByEeva) || !versioningId.IsAssigned())
            {
                return modified;
            }

            var versioningRepo = unitOfWork.CreateRepository<IVersioningRepository>();
            var versioning = versioningRepo.All().Single(v => v.Id == versioningId);

            // Try to find previous versioning info which was not edited by Eeva
            var lastNonEvaVersion= versioningRepo.All()
                .Where(v => v.UnificRootId == versioning.UnificRootId
                            && !v.LastOperationType.HasFlag(LastOperationType.Eeva)
                            && v.Created <= versioning.Created
                            && !v.Ignored)
                .OrderByDescending(v => v.Created).FirstOrDefault();

            // Return the date of last version which was not edited by Eeva.
            // If no such version exists, return last modified date.
            return lastNonEvaVersion?.LastOperationTimeStamp ?? modified;
        }

        // SFIPTV-1921
        public DateTime? GetExpirationDate<TEntity>(IUnitOfWork unitOfWork, Guid entityId, DateTime? utcNow = null)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IExpirable, new()
        {
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entity = repository.All().SingleOrDefault(e => e.Id == entityId);
            return entity == null
                ? null
                : GetExpirationDate(unitOfWork, entity, utcNow);
        }

        // SFIPTV-1921
        public bool GetIsWarningVisible<TEntity, TLanguageAvailability>(IUnitOfWork unitOfWork, TEntity entity, DateTime? utcNow = null)
            where TEntity : class, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            utcNow = utcNow ?? DateTime.UtcNow;

            var normalizedStatusId = NormalizePublishingStatusId<TEntity>(unitOfWork, entity.UnificRootId, entity.PublishingStatusId);
            if (!normalizedStatusId.HasValue) return false;

            var firstWarnings = expirationTimeCache.GetFirstWarningTimes(entity.GetType(), normalizedStatusId.Value);
            var expirations = expirationTimeCache.GetExpirationTimes(entity.GetType(), normalizedStatusId.Value);
            if (!firstWarnings.HasValue || !expirations.HasValue) return false;

            var warningPeriod = expirations.Value.Expires - firstWarnings.Value.Expires;

            if (entity.Expiration - utcNow > warningPeriod) return false;

            return !entity.LanguageAvailabilities.Any(x => x.PublishAt > utcNow);
        }

        // SFIPTV-1921
        public Dictionary<Guid, VmExpirationOfEntity> GetExpirationInformation<TEntity, TLanguageAvailability>(IUnitOfWork unitOfWork, Guid unificRootId, Guid publishingStatusId, IList<Guid> forOrganizations)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            var dateTimeUtcNow = DateTime.UtcNow;
            var normalizedStatusId = NormalizePublishingStatusId<TEntity>(unitOfWork, unificRootId, publishingStatusId);
            if (!normalizedStatusId.HasValue) return null;

            var lifeTime = expirationTimeCache.GetExpirationTimes(typeof(TEntity), normalizedStatusId.Value);
            if (!lifeTime.HasValue) return null;

            var lastWarningTime = expirationTimeCache.GetLastWarningTimes(typeof(TEntity), normalizedStatusId.Value)?.Modified;
            
            var tasks = GetExpirationTasks<TEntity, TLanguageAvailability>(unitOfWork, normalizedStatusId.Value, forOrganizations, new List<Guid> {unificRootId}, 0);

            var result = new Dictionary<Guid, VmExpirationOfEntity>();

            if ((tasks == null || !tasks.Any()) && unificRootId.IsAssigned()) //prepare setting of expireOn
            {
                var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
                tasks = repository.All()
                    .Where(x => x.UnificRootId == unificRootId && x.PublishingStatusId == normalizedStatusId.Value)
                    .Select(x => new VmTaskEntity {UnificRootId = x.UnificRootId, Modified = x.Modified, Expiration = x.Expiration})
                    .ToList();
            }
            
            foreach (var e in tasks)
            {
                result.Add(e.UnificRootId, new VmExpirationOfEntity
                {
                    ExpireOn = e.Expiration ?? CalculateExpirationDate(e.Modified, lifeTime.Value.Modified, dateTimeUtcNow),
                    // Warning should be visible only for services which do not have publishing scheduled in the future
                    IsWarningVisible = GetNonEvaModifiedDate(e.Modified, e.LastOperationType, e.VersioningId, unitOfWork) < lastWarningTime && !e.HasScheduledLanguageAvailability
                });
            }

            return result;
        }

        public List<VmTaskEntity> GetExpirationTasks<TEntity, TLanguageAvailability>
        (IUnitOfWork unitOfWork, Guid publishingStatusId, IList<Guid> forOrganizations,
            IEnumerable<Guid> definedEntities = null, int? skip = null)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume,
            IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            if (!skip.HasValue) skip = 1;
            var utcNow = DateTime.UtcNow;
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var selectedPublishingStatuses = new List<Guid> { publishingStatusId };
            var publishedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());

            var firstWarningTime = expirationTimeCache.GetFirstWarningTimes(typeof(TEntity), publishingStatusId)?.Expires;
            var lastWarningTime = expirationTimeCache.GetLastWarningTimes(typeof(TEntity), publishingStatusId)?.Expires;
            var expirationTime = expirationTimeCache.GetExpirationTimes(typeof(TEntity), publishingStatusId)?.Expires;

            if (firstWarningTime == null || lastWarningTime == null || expirationTime == null)
                return null;

            var warningPeriod = expirationTime - firstWarningTime;
            var resultTemp = definedEntities != null
                ? repository.All().Where(x => definedEntities.Contains(x.UnificRootId) && x.Expiration != null)
                : repository.All().Where(x => x.Expiration != null);

            commonService.ExtendPublishingStatusesByEquivalents(selectedPublishingStatuses);
            resultTemp = resultTemp.WherePublishingStatusIn(selectedPublishingStatuses);
            if (publishingStatusId == draftStatusId)
            {
                resultTemp = resultTemp.Include(x => x.LanguageAvailabilities);
                resultTemp = resultTemp
                    .Where(x =>
                        !repository.All()
                            .Any(y => y.UnificRootId == x.UnificRootId && y.PublishingStatusId == publishedStatusId))
                    .Where(x => !x.LanguageAvailabilities.Any(y => y.PublishAt.HasValue && y.PublishAt > utcNow));
            }
            if (publishingStatusId == publishedStatusId)
            {
                resultTemp = resultTemp.Include(x => x.LanguageAvailabilities);
                resultTemp = resultTemp
                    .Where(x =>
                        !repository.All()
                            .Include(y => y.LanguageAvailabilities)
                            .Any(y => y.UnificRootId == x.UnificRootId && y.PublishingStatusId == modifiedStatusId && y.LanguageAvailabilities.Any(z => z.PublishAt.HasValue && z.PublishAt > utcNow)))
                    .Where(x => !x.LanguageAvailabilities.Any(y => y.PublishAt.HasValue && y.PublishAt > utcNow));
            }

            var tempEntities = resultTemp
                .Where(x => forOrganizations.Contains(x.OrganizationId) && (x.Expiration - utcNow < warningPeriod))
                .ToList();
            var resultEntities = tempEntities
                .Select(x => new VmTaskEntity
                {
                    UnificRootId = x.UnificRootId,
                    Modified = x.Modified,
                    Expiration = x.Expiration.Value,
                    HasScheduledLanguageAvailability = x.LanguageAvailabilities.Any(la => la.PublishAt > DateTime.UtcNow),
                    VersioningId = x.VersioningId,
                    LastOperationType = x.LastOperationType,
                    // GroupId = tasksConfiguration.Last().PeriodId,
                    LowerThanLast = x.Expiration < lastWarningTime
                }).ToList();

            return resultEntities
                .Concat(GetEntityAllEntitiesWithGroup<TEntity, TLanguageAvailability>(unitOfWork, publishingStatusId, forOrganizations, definedEntities, skip))
                .DistinctBy(x => x.UnificRootId)
                .ToList();
        }

        // SFIPTV-1921
        public IEnumerable<TEntity> GetEntityIdsByExpirationDate<TEntity, TLanguageAvailability>(IUnitOfWork unitOfWork, PublishingStatus publishingStatus, DateTime utcNow)
            where TEntity : class, IAuditing, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            var publishingStatusId = publishingStatusCache.Get(publishingStatus);
            var expiration = expirationTimeCache.GetExpirationTimes(typeof(TEntity), publishingStatusId) ??
                             (DateTime.MinValue, DateTime.MinValue);
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();

            var allEntities = repository.All()
                .Include(x => x.LanguageAvailabilities)
                .Include(x => x.Versioning);

            if (publishingStatus == PublishingStatus.Modified)
            {
                // return only restored expired entities
                var statusPublishedId = publishingStatusCache.Get(PublishingStatus.Published.ToString());
                expiration = expirationTimeCache.GetExpirationTimes(typeof(TEntity),
                                 publishingStatusCache.Get(PublishingStatus.Draft.ToString())) ??
                             (DateTime.MinValue, DateTime.MinValue);

                var modifiedSubresult = allEntities
                    .Where(x => x.PublishingStatusId == publishingStatusId)
                    .Where(x => x.Expiration != null && x.Expiration < utcNow)
                    .Where(x => !x.LanguageAvailabilities.Any(la => la.PublishAt > utcNow))
                    .Where(x => !repository.All().Any(y=>y.UnificRootId == x.UnificRootId && y.PublishingStatusId == statusPublishedId))
                    .ToList();

                var modifiedResult = allEntities
                    .Where(x => x.PublishingStatusId == publishingStatusId)
                    .Where(x => x.Expiration == null && (x.Modified < expiration.Modified || x.LastOperationType.HasFlag(LastOperationType.PublishedByEeva)))
                    // Entities with scheduled publishing date in the future should not be archived
                    .Where(x => !x.LanguageAvailabilities.Any(la => la.PublishAt > utcNow))
                    .Where(x => !repository.All().Any(y=>y.UnificRootId == x.UnificRootId && y.PublishingStatusId == statusPublishedId))
                    .ToList();

                modifiedResult = modifiedResult.Where(x => GetNonEvaModifiedDate(unitOfWork, x) < expiration.Modified).ToList();
                return modifiedResult.Concat(modifiedSubresult).ToList();
            }

            var subResult = allEntities
                .Where(x => x.PublishingStatusId == publishingStatusId)
                .Where(x => x.Expiration != null && x.Expiration < utcNow)
                // Entities with scheduled publishing date in the future should not be archived
                .Where(x => !x.LanguageAvailabilities.Any(la => la.PublishAt > utcNow))
                .ToList();

            var result = allEntities
                .Where(x => x.PublishingStatusId == publishingStatusId)
                .Where(x => x.Expiration == null && (x.Modified < expiration.Modified || x.LastOperationType.HasFlag(LastOperationType.PublishedByEeva)))
                // Entities with scheduled publishing date in the future should not be archived
                .Where(x => !x.LanguageAvailabilities.Any(la => la.PublishAt > utcNow))
                .ToList();

            if (publishingStatus == PublishingStatus.Published && (result.Any()) || subResult.Any())
            {
                // add also modified versions to result
                var modifiedStatusId = publishingStatusCache.Get(PublishingStatus.Modified.ToString());
                var unificRootIds = result.Select(x => x.UnificRootId);
                var modifiedEntities = allEntities
                    .Where(x => unificRootIds.Contains(x.UnificRootId) && x.PublishingStatusId == modifiedStatusId);
                result.AddRange(modifiedEntities);

                var subResultRootIds = subResult.Select(x => x.UnificRootId);
                modifiedEntities = allEntities.Where(x =>
                    subResultRootIds.Contains(x.UnificRootId) && x.PublishingStatusId == modifiedStatusId);
                subResult.AddRange(modifiedEntities);
            }

            result =  result.Where(x => GetNonEvaModifiedDate(unitOfWork, x) < expiration.Modified).ToList();
            return result.Concat(subResult).ToList();
        }

        // SFIPTV-1921
        [Obsolete]
        private List<VmTaskEntity> GetEntityAllEntitiesWithGroup<TEntity, TLanguageAvailability>
            (IUnitOfWork unitOfWork, Guid publishingStatusId, IList<Guid> forOrganizations, IEnumerable<Guid> definedEntities = null, int? skip = null)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            if (!skip.HasValue) skip = 1;

            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var publishedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());

            var firstWarningTime = expirationTimeCache.GetFirstWarningTimes(typeof(TEntity), publishingStatusId)?.Modified;
            var lastWarningTime = expirationTimeCache.GetLastWarningTimes(typeof(TEntity), publishingStatusId)?.Modified;

            if (firstWarningTime == null || lastWarningTime == null)
                return null;

            var resultTemp = definedEntities != null
                ? repository.All().Where(x => definedEntities.Contains(x.UnificRootId) && x.Expiration == null)
                : repository.All().Where(x => x.Expiration == null);

            var selectedPublishingStatuses = new List<Guid> { publishingStatusId };
            commonService.ExtendPublishingStatusesByEquivalents(selectedPublishingStatuses);
            resultTemp = resultTemp.WherePublishingStatusIn(selectedPublishingStatuses);
            if (publishingStatusId == draftStatusId)
            {
                resultTemp = resultTemp
                    .Where(x =>
                        !repository.All()
                            .Any(y => y.UnificRootId == x.UnificRootId && y.PublishingStatusId == publishedStatusId));
            }

            var tempEntities = resultTemp.Where(x => forOrganizations.Contains(x.OrganizationId)
                                                 && (x.Modified < firstWarningTime ||
                                                     x.LastOperationType.HasFlag(LastOperationType.PublishedByEeva)))
                .Select(x => new VmTaskEntity
                {
                    UnificRootId = x.UnificRootId,
                    Modified = x.Modified,
                    HasScheduledLanguageAvailability = x.LanguageAvailabilities.Any(la => la.PublishAt > DateTime.UtcNow),
                    VersioningId = x.VersioningId,
                    LastOperationType = x.LastOperationType
                }).ToList();

            var resultEntities = new List<VmTaskEntity>();
            foreach (var entity in tempEntities)
            {
                var modified = GetNonEvaModifiedDate(entity.Modified, entity.LastOperationType, entity.VersioningId,
                    unitOfWork);

                if (modified >= firstWarningTime) continue;

                entity.Modified = modified;
                // entity.GroupId = config.PeriodId;
                entity.LowerThanLast = entity.Modified < lastWarningTime;
                resultEntities.Add(entity);
            }

            return resultEntities
                .DistinctBy(x => x.UnificRootId)
                .ToList();
        }

        private Guid? NormalizePublishingStatusId<TEntity>(IUnitOfWork unitOfWork, Guid unificRootId,
            Guid publishingStatusId)
            where TEntity : class, IVersionedVolume, new()
        {
            // if it is modified version and exist published one skip validation info,
            if (PublishingStatusCache.Get(PublishingStatus.Modified) != publishingStatusId)
                return publishingStatusId;

            var publishedVersion = VersioningManager.GetLastPublishedVersion<TEntity>(unitOfWork, unificRootId);
            if (publishedVersion != null)
                return PublishingStatusCache.Get(PublishingStatus.Published);

            // if it is modified (after restore from archived) - published version should not exist and validation info
            // should be provided, get configuration for draft
            return PublishingStatusCache.Get(PublishingStatus.Draft);

        }

        // SFIPTV-1921
        /// <summary>
        ///
        /// </summary>
        /// <param name="modifiedDate"></param>
        /// <param name="modifiedForExpiration">The date in past when content was edited for the last time and
        /// will be expired. E.g.: exactly today minus one year for published services. It can be found in
        /// the task configuration table.</param>
        /// <param name="utcNow"></param>
        /// <returns></returns>
        public static DateTime CalculateExpirationDate(DateTime modifiedDate, DateTime modifiedForExpiration, DateTime utcNow)
        {
            return (utcNow + (modifiedDate.Date - modifiedForExpiration.Date)).Date;
        }

        // SFIPTV-1921
        private DateTime GetNonEvaModifiedDate<TEntity>(IUnitOfWork unitOfWork, TEntity entity)
            where TEntity : class, IAuditing, IVersionedVolume, IExpirable, new()
        {
            if (!entity.LastOperationType.HasFlag(LastOperationType.PublishedByEeva))
            {
                // If entity was not edited by Eeva, return modified date.
                return entity.Modified;
            }

            var versioningRepo = unitOfWork.CreateRepository<IVersioningRepository>();
            var versioning = entity.Versioning ?? versioningRepo.All().FirstOrDefault(v => v.Id == entity.VersioningId);

            if (versioning == null)
            {
                // If versioning is not available, return modified date.
                return entity.Modified;
            }

            // Try to find previous versioning info which was not edited by Eeva
            var lastNonEvaModifiedDate = versioningRepo.All()
                .Where(v => v.UnificRootId == versioning.UnificRootId
                            && !v.LastOperationType.HasFlag(LastOperationType.Eeva)
                            && v.Created <= versioning.Created)
                .OrderByDescending(v => v.Created).FirstOrDefault()?.LastOperationTimeStamp;

            // Return the date of last version which was not edited by Eeva.
            // If no such version exists, return last modified date.
            return lastNonEvaModifiedDate ?? entity.Modified;
        }
    }
}
