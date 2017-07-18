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
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;
using Microsoft.Extensions.Logging;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Framework.Logging;
using System.Reflection;
using PTV.Framework.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Interfaces.Cloning;
using PTV.Database.DataAccess.Services;
using PTV.Framework.Formatters.Attributes;
using PTV.Framework.ServiceManager;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Security;

namespace PTV.Database.DataAccess
{
    /// <summary>
    /// Unit of Work is scope of database operations. Writable type provides Save method which allows to make changes in database
    /// </summary>
    [RegisterService(typeof(IUnitOfWorkWritable), RegisterType.Transient)]
    internal class UnitOfWorkWritable : UnitOfWork, IUnitOfWorkWritable, IUnitOfWorkCachedSearching
    {
        protected readonly IUserIdentification userIdentification;
        protected readonly ILogger logger;
        protected readonly ApplicationConfiguration applicationConfiguration;
        protected readonly ILockingManager lockingManager;
        protected readonly IRoleChecker roleChecker;
        protected readonly ITranslationCloneCache translationCloneCache;

        ITranslationCloneCache ITranslationUnitOfWork.TranslationCloneCache => translationCloneCache;

        public UnitOfWorkWritable(IResolveManager resolveManager, PtvDbContext dbContext, IUserIdentification userIdentification, ILogger<UnitOfWorkWritable> logger, ApplicationConfiguration applicationConfiguration, ILockingManager lockingManager, IRoleChecker roleChecker, ITranslationCloneCache translationCloneCache) : base(resolveManager, dbContext)
        {
            this.userIdentification = userIdentification;
            this.logger = logger;
            this.applicationConfiguration = applicationConfiguration;
            this.lockingManager = lockingManager;
            this.roleChecker = roleChecker;
            this.translationCloneCache = translationCloneCache;
        }

        protected override void CustomConfigure()
        {
            DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
        }

        /// <summary>
        /// Push changes done in context into database
        /// </summary>
        /// <param name="saveMode"></param>
        /// <param name="parentEntity">Logical parent entity to mark as updated when committing changes to database</param>
        /// <param name="userName">The user name for user. Used in console application (PTV.DataMapper.ConsoleApp) where there is no Httpcontext</param>
        public void Save(SaveMode saveMode = SaveMode.Normal, object parentEntity = null, string userName = null)
        {
            IList<VmLogEntry> logEntries = new List<VmLogEntry>();
            IList<Guid> lockedIds = new List<Guid>();

            var writableContext = this.DbContext as DbContext;
            writableContext.ChangeTracker.DetectChanges();
            userName = GetUserNameForAuditing(saveMode, userName);
            if (!string.IsNullOrEmpty(userName) || saveMode == SaveMode.NonTrackedDataMigration)
            {
                CreateLogs(writableContext);
                if (saveMode != SaveMode.NonTrackedDataMigration)
                {
                    if (parentEntity != null && writableContext.ChangeTracker.Entries<IAuditing>().Any(x => x.State == EntityState.Added || x.State == EntityState.Modified))
                    {
                        writableContext.Entry(parentEntity).Property("Modified").IsModified = true;
                        writableContext.Entry(parentEntity).Property("ModifiedBy").IsModified = true;
                    }
                    if (saveMode != SaveMode.AllowAnonymous)
                    {
                        CheckRoles(saveMode, writableContext);
                        CheckEntityLock(writableContext, lockedIds);
                    }
                }

                var dateToSave = DateTime.UtcNow;
                var operationId = dateToSave.ToString("O") + "-" + Guid.NewGuid();
                foreach (var updatedEntityEntry in writableContext.ChangeTracker.Entries<IAuditing>().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified))
                {
                    ApplyFormattingToPropertiess(updatedEntityEntry);
                    SetAuditingFields(updatedEntityEntry, userName, dateToSave, saveMode);
                    SetOperationInfoFields(updatedEntityEntry, operationId, dateToSave);
                    logEntries.Add(GetLogEntry(updatedEntityEntry.Entity));
                }
            }
            else
            {
                throw new Exception(CoreMessages.AnonymousSaveNotAllowed);
            }

            // unlock all entries

            var lockResults = lockingManager.UnLockEntities(this, lockedIds);
            EntityLockResult lockedEntity = lockResults.Values.FirstOrDefault(i => i.LockStatus != EntityLockEnum.Unlocked);
            if (lockedEntity != null)
            {
                throw new LockException("", new List<string>(){ lockedEntity.LockedBy});
            }

            // save changes
            writableContext.SaveChanges();
            // Let's add the log entries only when we are sure database changes have been successfully saved into database.
            logger.LogDBEntries(logEntries);
        }

        private void SetOperationInfoFields(EntityEntry<IAuditing> updatedEntityEntry, string operationId, DateTime dateToSave)
        {
            updatedEntityEntry.Entity.LastOperationId = operationId;
            updatedEntityEntry.Property("LastOperationId").IsModified = true;
        }

        private void CreateLogs(DbContext writableContext)
        {
            if (applicationConfiguration.LogSavedEntities)
            {
                var entries = writableContext.ChangeTracker.Entries().ToList();


                foreach (var entry in entries.Where(x => (x.State == EntityState.Added) || (x.State == EntityState.Modified)))
                {
                    var properties = entry.Properties;
                    var msg = "Entity: " + entry.Metadata.Name + "\n";
                    foreach (var property in properties)
                    {
                        msg +=
                            $"- {property.Metadata.Name} Modified:{property.IsModified} Current:{property.CurrentValue} Original:{property.OriginalValue}\n";
                    }
                    msg += "\n";
                    logger.LogDebug(msg);
                }

                var toAdd = entries.Where(x => x.State == EntityState.Added).Select(i => i.Entity.GetType().Name).ToArray();
                var toModify =
                    entries.Where(x => x.State == EntityState.Modified).Select(i => i.Entity.GetType().Name).ToArray();
                var toDelete = entries.Where(x => x.State == EntityState.Deleted).Select(i => i.Entity.GetType().Name).ToArray();
                var toDetach =
                    entries.Where(x => x.State == EntityState.Detached).Select(i => i.Entity.GetType().Name).ToArray();

                string infoMessage = "----- Context SAVE -----\n"
                                     + string.Format("Entities Added: {0}\n", string.Join(",", toAdd))
                                     + string.Format("Entities Modified: {0}\n", string.Join(",", toModify))
                                     + string.Format("Entities Deleted: {0}\n", string.Join(",", toDelete))
                                     + string.Format("Entities Detached: {0}\n", string.Join(",", toDetach));

                logger.LogDebug(infoMessage);
            }
        }

        private void CheckEntityLock(DbContext writableContext, IList<Guid> lockedIds)
        {
            foreach (
                var updatedEntityEntry in
                writableContext.ChangeTracker.Entries<EntityIdentifierBase>().Where(x => x.State == EntityState.Modified))
            {
                EntityLockResult lockResult = null;
                if (updatedEntityEntry.Entity is ILockable)
                {
                    lockResult = lockingManager.IsLockedBy(updatedEntityEntry.Entity);
                }
                var versionedEntity = updatedEntityEntry.Entity as IVersionedVolume;
                if (versionedEntity != null)
                {
                    lockResult = lockingManager.IsLockedBy(versionedEntity.UnificRootId);
                }
                if (lockResult != null)
                {
                    switch (lockResult.LockStatus)
                    {
                        case EntityLockEnum.Unlocked:
                            continue;
                        case EntityLockEnum.LockedForCurrent:
                            lockedIds.Add(versionedEntity.UnificRootId);
                            break;
                        default:
                            throw new LockException("", new List<string> { lockResult.LockedBy });
                    }
                }
            }
        }

        private void CheckRoles(SaveMode saveMode, DbContext writableContext)
        {
            if (saveMode == SaveMode.Normal)
            {
                var roleBasedEntity =
                    writableContext.ChangeTracker.Entries<IRoleBased>()
                        .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified)
                        .ToList();
                foreach (var updatedEntityEntry in roleBasedEntity)
                {
                    roleChecker.CheckEntity(updatedEntityEntry.Entity, updatedEntityEntry, this);
                }
            }
        }

        public void MarkPropertyModified(object entity, string property, bool modified)
        {
            this.DbContext.Entry(entity).Property(property).IsModified = modified;
        }

        public string GetUserNameForAuditing(SaveMode saveMode, string userName)
        {
            userName = userName ?? userIdentification.UserName;
            if (string.IsNullOrEmpty(userName) && (saveMode == SaveMode.AllowAnonymous || saveMode == SaveMode.NonTrackedDataMigration))
            {
                userName = "PTVapp";
            }
            return userName;
        }

        /// <summary>
        /// Sets auditing fields of entity
        /// </summary>
        /// <param name="entityEntry"></param>
        /// <param name="userName"></param>
        /// <param name="savingTimeStamp"></param>
        /// <param name="saveMode"></param>
        private void SetAuditingFields(EntityEntry<IAuditing> entityEntry, string userName, DateTime savingTimeStamp, SaveMode saveMode)
        {
            if (saveMode != SaveMode.NonTrackedDataMigration || (saveMode == SaveMode.NonTrackedDataMigration && string.IsNullOrEmpty(entityEntry.Entity.ModifiedBy)))
            {
                entityEntry.Entity.SetModified(userName, savingTimeStamp);
            }

            if (string.IsNullOrEmpty(entityEntry.Entity.CreatedBy))
            {
                entityEntry.Entity.SetCreated(userName, savingTimeStamp);
            }

            // Notify changes to the "Modified" values when any other data has been modified
            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property("Modified").IsModified = true;
                entityEntry.Property("ModifiedBy").IsModified = true;
            }
        }

        private void ApplyFormattingToPropertiess(EntityEntry<IAuditing> entityEntry)
        {
            var propertiesToFormat = entityEntry.Entity.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(ValueFormatterAttribute), true));
            propertiesToFormat.ForEach(prop => prop.GetCustomAttributes<ValueFormatterAttribute>(true).OrderBy(attr => attr.Order).ForEach(attr =>
            {
                var formatter = resolveManager.Resolve(attr.ValueFormatterType) as IValueFormatter;

                if (formatter != null)
                {
                    prop.SetValue(entityEntry.Entity, formatter.Format(prop.GetValue(entityEntry.Entity), entityEntry.Entity.GetType().Name, resolveManager, entityEntry.Entity));
                }
            }));
        }

        private VmLogEntry GetLogEntry(IAuditing entity)
        {
            return new VmLogEntry()
            {
                Identifier = GetPrimaryKey(entity),
                Table = GetTableName(entity.GetType()),
                UserName = entity.ModifiedBy,
                Operation = GetOperation(entity)
            };
        }

        private string GetPrimaryKey(IAuditing entity)
        {
            if (entity is EntityIdentifierBase)
            {
                return (entity as EntityIdentifierBase).Id.ToString();
            }

            // Get comma separated list of primarykeys and their values
            var primarykeys = this.DbContext.Model.FindEntityType(entity.GetType()).FindPrimaryKey().Properties;
            var list = new List<string>();
            primarykeys.ForEach(p => list.Add(string.Format("{0}: {1}", p.Name, entity.GetType().GetProperty(p.Name).GetValue(entity, null))));
            return string.Join(", ", list);
        }
        private string GetTableName(Type type)
        {
            return this.DbContext.Model.FindEntityType(type).Npgsql().TableName;
        }
        private LogOperation GetOperation(IAuditing entity)
        {
            var state = this.DbContext.Entry(entity).State;
            if (state == EntityState.Added)
            {
                return LogOperation.Create;
            }
            if (state == EntityState.Modified)
            {
                // Check if entity was marked as deleted
                var valitidyEntity = entity as IPublishingStatus;
                if (valitidyEntity != null)
                {
                    if (valitidyEntity.PublishingStatus?.Code == PublishingStatus.Deleted.ToString())
                    {
                        return LogOperation.Delete;
                    }
                }
                return LogOperation.Update;
            }
            return LogOperation.NotDefined;
        }

        T IUnitOfWorkCachedSearching.Find<T>(Guid entityId, Func<IQueryable<T>, IQueryable<T>> includeChain)
        {
            Func<IQueryable<T>, IQueryable<T>> defaultPassThrought = (i) => i;
            Func < IQueryable<T>, IQueryable < T >> applyIncludes = includeChain ?? defaultPassThrought;
            return DbContext.ChangeTracker.Entries<T>().FirstOrDefault(i => i.Entity.Id == entityId)?.Entity ?? applyIncludes(DbContext.Set<T>()).FirstOrDefault(i => i.Id == entityId);
        }

        void ITranslationUnitOfWork.DetachEntity<T>(T entity)
        {
            if (entity == null) return;
            var entityEntry = DbContext.Entry(entity);
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.State = EntityState.Detached;
            }
        }

        void ITranslationUnitOfWork.DetachOrRemoveEntity<T>(T entity)
        {
            ((ITranslationUnitOfWork)this).DetachOrRemoveEntities(new List<T>() { entity });
        }

        void ITranslationUnitOfWork.DetachOrRemoveEntities<T>(List<T> entities)
        {
            if (entities.IsNullOrEmpty()) return;
            entities.ForEach(entity =>
            {
                var entityEntry = DbContext.Entry(entity);
                switch (entityEntry.State)
                {
                    case EntityState.Added:
                        entityEntry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Unchanged:
                        entityEntry.State = EntityState.Deleted;
                        break;
                }
            });
        }
    }
}
