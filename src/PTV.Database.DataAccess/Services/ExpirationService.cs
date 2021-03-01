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
using System.Collections.Specialized;
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

        public DateTime? GetExpirationDate<TEntity>(IUnitOfWork unitOfWork, TEntity entity, DateTime? lastChangeDate = null)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IExpirable, new()
        {
            var normalizedStatusId = NormalizePublishingStatusId<TEntity>(unitOfWork, entity.UnificRootId, entity.PublishingStatusId);
            if (normalizedStatusId == null)
                return null;

            var expirationMonths = (int)expirationTimeCache.GetExpirationMonths(typeof(TEntity), normalizedStatusId.Value);
            if (expirationMonths == 0)
                return null;

            var modifiedDate = lastChangeDate ?? entity.Modified; //DateTime.UtcNow
            
            return modifiedDate.AddMonths(expirationMonths).Date;
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

        public void SetExpirationDate<TEntity>(IUnitOfWork unitOfWork, TEntity entity)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IExpirable, new()
        {
            entity.Expiration = GetExpirationDate(unitOfWork, entity, lastChangeDate: DateTime.UtcNow);
        }

        public void SetExpirationDateForPublishing<TEntity>(IUnitOfWorkWritable unitOfWork, IEnumerable<Guid> ids, bool allowAnonymous = false)
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

        public void SetExpirationDateForPublishing<TEntity>(IContextManager contextManager, IEnumerable<Guid> ids, bool allowAnonymous = false)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IExpirable, new()
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SetExpirationDateForPublishing<TEntity>(unitOfWork, ids, allowAnonymous);
            });
        }

        //Expiration column used in history migrations 
        public void PolyfillExpirationDate<TEntity, TLanguageAvailability>(IUnitOfWorkWritable unitOfWork, TEntity entity, DateTime utcNow)
            where TEntity : class, IAuditing, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            var normalizedStatusId = NormalizePublishingStatusId<TEntity>(unitOfWork, entity.UnificRootId, entity.PublishingStatusId);
            if (normalizedStatusId == null)
                return;

            var expirationMonths = (int)expirationTimeCache.GetExpirationMonths(entity.GetType(), normalizedStatusId.Value);
            if (expirationMonths == 0)
                return;

            var scheduledPublishingDate = entity.LanguageAvailabilities.Max(x => x.PublishAt);
            if (scheduledPublishingDate > utcNow)
            {
                entity.Expiration = scheduledPublishingDate.Value.AddMonths(expirationMonths);
                return;
            }

            entity.Expiration = entity.Modified.AddMonths(expirationMonths);
        }

        public DateTime? GetExpirationDate<TEntity>(IUnitOfWork unitOfWork, Guid entityId, DateTime? utcNow = null)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IExpirable, new()
        {
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entity = repository.All().SingleOrDefault(e => e.Id == entityId);
            return entity == null
                ? null
                : GetExpirationDate(unitOfWork, entity, lastChangeDate: utcNow);
        }

        public bool GetIsWarningVisible<TEntity, TLanguageAvailability>(IUnitOfWork unitOfWork, TEntity entity, DateTime? utcNow = null)
            where TEntity : class, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IExpirable, IAuditing, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            var normalizedStatusId = NormalizePublishingStatusId<TEntity>(unitOfWork, entity.UnificRootId, entity.PublishingStatusId);
            if (!normalizedStatusId.HasValue) return false;
            
            var lastWarningMonths = (int)expirationTimeCache.GetLastWarningMonths(typeof(TEntity), normalizedStatusId.Value);
            if (lastWarningMonths == 0)
                return false;
            
            var lastWarningLocalDateTime = DateTime.UtcNow.AddMonths(-lastWarningMonths);

            if (entity.Modified > lastWarningLocalDateTime) return false;
            return !entity.LanguageAvailabilities.Any(x => x.PublishAt > utcNow);
        }
        
        public List<VmTaskEntity> GetNotUpdatedTasks<TEntity, TLanguageAvailability>
        (IUnitOfWork unitOfWork, Guid publishingStatusId, IList<Guid> forOrganizations,
            IEnumerable<Guid> subEntityTypes,
            IEnumerable<Guid> definedEntities = null, int? skip = null)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume,
            IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            var expirationMonths = (int)expirationTimeCache.GetExpirationMonths(typeof(TEntity), publishingStatusId);
            if (expirationMonths == 0)
                return new List<VmTaskEntity>();
            var expirationLocalDateTime = DateTime.UtcNow.AddMonths(-expirationMonths);
            
            return GetTaskEntities<TEntity, TLanguageAvailability>(unitOfWork, publishingStatusId, forOrganizations, expirationLocalDateTime, subEntityTypes, definedEntities, skip);
        }
        
        public List<VmTaskEntity> GetNotUpdatedDraftTasks<TEntity, TLanguageAvailability>
        (IUnitOfWork unitOfWork, Guid publishingStatusId, IList<Guid> forOrganizations,
            IEnumerable<Guid> subEntityTypes,
            IEnumerable<Guid> definedEntities = null, int? skip = null)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume,
            IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            var lastWarningMonths = (int)expirationTimeCache.GetLastWarningMonths(typeof(TEntity), publishingStatusId);
            var lastWarningLocalDateTime = DateTime.UtcNow.AddMonths(-lastWarningMonths);

            return GetTaskEntities<TEntity, TLanguageAvailability>(unitOfWork, publishingStatusId, forOrganizations, lastWarningLocalDateTime, subEntityTypes, definedEntities, skip);
        }
        
        public List<VmTaskEntity> GetExpirationTasks<TEntity, TLanguageAvailability>
        (IUnitOfWork unitOfWork, Guid publishingStatusId, IList<Guid> forOrganizations,
            IEnumerable<Guid> subEntityTypes,
            IEnumerable<Guid> definedEntities = null, int? skip = null)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume,
            IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            var lastWarningMonths = (int)expirationTimeCache.GetLastWarningMonths(typeof(TEntity), publishingStatusId);
            if (lastWarningMonths == 0)
                return new List<VmTaskEntity>();
            var lastWarningLocalDateTime = DateTime.UtcNow.AddMonths(-lastWarningMonths);

            return GetTaskEntities<TEntity, TLanguageAvailability>(unitOfWork, publishingStatusId, forOrganizations, lastWarningLocalDateTime, subEntityTypes, definedEntities, skip);
        }

        private List<VmTaskEntity> GetTaskEntities<TEntity, TLanguageAvailability>
        (IUnitOfWork unitOfWork, Guid publishingStatusId, IList<Guid> forOrganizations, DateTime comparisonDate, IEnumerable<Guid> subEntityTypes,
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
            
            var expirationMonths = (int)expirationTimeCache.GetExpirationMonths(typeof(TEntity), publishingStatusId);
            if (comparisonDate == default(DateTime) || expirationMonths == 0)
            {
                return new List<VmTaskEntity>();
            }

            var resultTemp = definedEntities != null
                ? repository.All().Where(x => definedEntities.Contains(x.UnificRootId))
                : repository.All();
            
            commonService.ExtendPublishingStatusesByEquivalents(selectedPublishingStatuses);
            resultTemp = resultTemp.WherePublishingStatusIn(selectedPublishingStatuses);
            
            if (subEntityTypes != null && subEntityTypes.Any())
            {
                if (typeof(TEntity) == typeof(ServiceChannelVersioned))
                {
                    resultTemp = resultTemp.Cast<ITypeEntity>().WhereEntityTypesIn(subEntityTypes).Cast<TEntity>();
                }
            }
            
            if (publishingStatusId == draftStatusId)
            {
                resultTemp = resultTemp.Include(x => x.LanguageAvailabilities);
                resultTemp = resultTemp
                    .Where(x =>
                        !repository.All()
                            .Include(x => x.LanguageAvailabilities)
                            .Any(y => y.UnificRootId == x.UnificRootId && y.PublishingStatusId == publishedStatusId && y.LanguageAvailabilities.Any(z => z.PublishAt.HasValue && z.PublishAt > utcNow)))
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

            resultTemp = resultTemp
                .Where(x => forOrganizations.Contains(x.OrganizationId));
            
            var tempEntities = resultTemp
                .Where(x => x.Modified < comparisonDate)
                .ToList();
            
            var resultEntities = tempEntities
                .Select(x => new VmTaskEntity
                {
                    UnificRootId = x.UnificRootId,
                    Modified = x.Modified,
                    Expiration = x.Modified.AddMonths(expirationMonths),
                    HasScheduledLanguageAvailability = x.LanguageAvailabilities.Any(la => la.PublishAt > DateTime.UtcNow),
                    VersioningId = x.VersioningId,
                    LastOperationType = x.LastOperationType,
                }).ToList();

            var result = resultEntities
                .DistinctBy(x => x.UnificRootId)
                .ToList();
            
            //Without ASTI connections
            if (typeof(TEntity) == typeof(ServiceChannelVersioned))
            {
                var astiRootIds = commonService.GetAstiChannelIds(result.Select(x => x.UnificRootId).Distinct().ToList(), unitOfWork);
                if (astiRootIds.Any())
                {
                    result = result.Where(x => !astiRootIds.Contains(x.UnificRootId)).ToList();
                }
            }
            
            return result;
        }
        
        public IEnumerable<TEntity> GetEntityIdsByExpirationDate<TEntity, TLanguageAvailability>(IUnitOfWork unitOfWork, PublishingStatus publishingStatus, DateTime utcNow, List<Guid> subEntityTypeGuidsToArchive)
            where TEntity : class, IAuditing, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IExpirable, ITypeEntity, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            var DateTimeUtc = DateTime.UtcNow;
            var publishingStatusId = publishingStatusCache.Get(publishingStatus);
            var expirationMonths = (int)expirationTimeCache.GetExpirationMonths(typeof(TEntity), publishingStatusId);
            if (expirationMonths == 0)
                return new List<TEntity>();
            var expirationLocalDateTime = DateTimeUtc.AddMonths(-expirationMonths);
            
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            
            var allEntities = repository.All();
            allEntities = allEntities.Include(x => x.LanguageAvailabilities)
                                     .Include(x => x.Versioning);

            if (subEntityTypeGuidsToArchive != null && subEntityTypeGuidsToArchive.Any())
            {
                allEntities = allEntities.Where(x => subEntityTypeGuidsToArchive.Contains(x.TypeId));
            }

            if (publishingStatus == PublishingStatus.Modified)
            {
                // return only restored expired entities
                var statusPublishedId = publishingStatusCache.Get(PublishingStatus.Published.ToString());
                expirationMonths = (int)expirationTimeCache.GetExpirationMonths(typeof(TEntity), publishingStatusId);
                if (expirationMonths == 0)
                    return new List<TEntity>();
                expirationLocalDateTime = DateTimeUtc.AddMonths(-expirationMonths);

                var modifiedResult = allEntities
                    .Where(x => x.PublishingStatusId == publishingStatusId)
                    .Where(x => x.Modified < expirationLocalDateTime)
                    // Entities with scheduled publishing date in the future should not be archived
                    .Where(x => !x.LanguageAvailabilities.Any(la => la.PublishAt > utcNow))
                    .Where(x => !repository.All().Any(y=>y.UnificRootId == x.UnificRootId && y.PublishingStatusId == statusPublishedId))
                    .ToList();

                modifiedResult = modifiedResult.Where(x => x.Modified < expirationLocalDateTime).ToList();
                return modifiedResult;
            }

            var result = allEntities
                .Where(x => x.PublishingStatusId == publishingStatusId)
                .Where(x => x.Modified < expirationLocalDateTime)
                // Entities with scheduled publishing date in the future should not be archived
                .Where(x => !x.LanguageAvailabilities.Any(la => la.PublishAt > utcNow))
                .ToList();

            if (publishingStatus == PublishingStatus.Published && result.Any())
            {
                // add also modified versions to result
                var modifiedStatusId = publishingStatusCache.Get(PublishingStatus.Modified.ToString());
                var unificRootIds = result.Select(x => x.UnificRootId);
                var modifiedEntities = allEntities
                    .Where(x => unificRootIds.Contains(x.UnificRootId) && x.PublishingStatusId == modifiedStatusId);
                result.AddRange(modifiedEntities);
            }

            result =  result.Where(x => x.Modified < expirationLocalDateTime).ToList();
            return result;
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
        
        //REMOVE AFTER OPEN API CHANGES
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
    }
}
