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
using Microsoft.EntityFrameworkCore.Internal;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Cloning;
using PTV.Database.DataAccess.Services;
using PTV.Framework.Formatters.Attributes;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework.Attributes;
using PTV.Framework.Exceptions.DataAccess;

namespace PTV.Database.DataAccess
{
    /// <summary>
    /// Unit of Work is scope of database operations. Writable type provides Save method which allows to make changes in database
    /// </summary>
    [RegisterService(typeof(IUnitOfWorkWritable), RegisterType.Transient)]
    internal class UnitOfWorkWritable : UnitOfWork, IUnitOfWorkWritable, IUnitOfWorkCachedSearching
    {
        private readonly Dictionary<Type, Action<object,EntityState>> saveProcessingActions = new Dictionary<Type, Action<object, EntityState>>();
        private readonly IPahaTokenAccessor pahaTokenAccessor;
        private readonly ILogger logger;
        private readonly ApplicationConfiguration applicationConfiguration;
        private readonly ILockingManager lockingManager;
        private readonly IRoleCheckerManager roleChecker;
        private readonly ITranslationCloneCache translationCloneCache;
        private readonly IEntityTrackingManager entityTrackingManager;
        private IEntityModifiedPropagationCreator modifiedPropagationCreator;

        private Lazy<Guid> publishingStatusDraft;
        private Lazy<Guid> publishingStatusPublished;
        private Lazy<Guid> publishingStatusModified;
        private Lazy<Guid> publishingStatusDeleted;
        private Lazy<Guid> publishingStatusOldPublished;
        private Lazy<Guid> publishingStatusRemoved;

        private Guid GetPublishingStatusDeleted => publishingStatusDeleted.Value;
        private Guid GetPublishingStatusOldPublished => publishingStatusOldPublished.Value;
        private Guid GetPublishingStatusPublished => publishingStatusPublished.Value;
        private Guid GetPublishingStatusDraft => publishingStatusDraft.Value;
        private Guid GetPublishingStatusModified => publishingStatusModified.Value;
        private Guid GetPublishingStatusRemoved => publishingStatusRemoved.Value;

        ITranslationCloneCache ITranslationUnitOfWork.TranslationCloneCache => translationCloneCache;

        public UnitOfWorkWritable(IPahaTokenAccessor pahaTokenAccessor, IResolveManager resolveManager, ILogger<UnitOfWorkWritable> logger, ApplicationConfiguration applicationConfiguration, ILockingManager lockingManager, IRoleCheckerManager roleChecker, ITranslationCloneCache translationCloneCache, IEntityTrackingManager entityTrackingManager, IEntityModifiedPropagationCreator modifiedPropagationCreator) : base(resolveManager)
        {
            this.pahaTokenAccessor = pahaTokenAccessor;
            this.logger = logger;
            this.applicationConfiguration = applicationConfiguration;
            this.lockingManager = lockingManager;
            this.roleChecker = roleChecker;
            this.translationCloneCache = translationCloneCache;
            this.entityTrackingManager = entityTrackingManager;
            this.modifiedPropagationCreator = modifiedPropagationCreator;

            var typesCache = resolveManager.Resolve<ITypesCache>();
            publishingStatusDraft = new Lazy<Guid>(typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString()));
            publishingStatusPublished = new Lazy<Guid>(typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString()));
            publishingStatusModified = new Lazy<Guid>(typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString()));
            publishingStatusOldPublished = new Lazy<Guid>(typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString()));
            publishingStatusDeleted = new Lazy<Guid>(typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString()));
            publishingStatusRemoved = new Lazy<Guid>(typesCache.Get<PublishingStatusType>(PublishingStatus.Removed.ToString()));
        }

        protected override void CustomConfigure()
        {
            DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
        }

        public void AddModifiedPropagationChain<TEntity, TTarget>(Action<IMainEntityChangesPropagationPath<TEntity>> propagationChain) where TEntity : class where TTarget : class, IAuditing
        {
            DbContext.ChangeTracker.DetectChanges();
            DbContext.ChangeTracker.Entries<TEntity>().Where(i => i.State == EntityState.Added || i.State == EntityState.Modified || i.State == EntityState.Deleted).ToList().ForEach(e =>
            {
                var modifiedPropagator = modifiedPropagationCreator.Create<TEntity, TTarget>(this, e.Entity);
                propagationChain(modifiedPropagator.CreateDefinition());
                modifiedPropagator.GetMarkedEntities().ForEach(entity =>
                {
                    var eEntry = DbContext.Entry(entity);
                    if (eEntry.State == EntityState.Unchanged)
                    {
                        eEntry.State = EntityState.Modified;
                    }
                    eEntry.Property("Modified").IsModified = true;
                });
            });
        }

        /// <summary>
        /// Register pre save processing action which will be called for all entities if this type once just before they are saved
        /// </summary>
        /// <param name="processingAction"></param>
        /// <typeparam name="T"></typeparam>
        public void AddPreSaveEntityProcessing<T>(Action<T,EntityState> processingAction) where T : class
        {
            saveProcessingActions.Add(typeof(T), (o, s) => processingAction(o as T,s));
        }

        /// <summary>
        /// Push changes done in context into database
        /// </summary>
        /// <param name="saveMode"></param>
        /// <param name="preSaveAction"></param>
        /// <param name="parentEntity">Logical parent entity to mark as updated when committing changes to database</param>
        /// <param name="userName">The user name for user. Used in console application (PTV.DataMapper.ConsoleApp) where there is no Httpcontext</param>
        public int Save(SaveMode saveMode = SaveMode.Normal, PreSaveAction preSaveAction = 0, object parentEntity = null, string userName = null)
        {
            IList<VmLogEntry> logEntries = new List<VmLogEntry>();
            IList<Guid> lockedIds = new List<Guid>();

            var writableContext = this.DbContext as DbContext;
            logger.LogDebug("*** Before detect changes of entities ***");
            CreateLogs(writableContext);

//            writableContext.ChangeTracker.Entries().ToList().Where(i => i.Properties.Any(m => m.IsModified)).SelectMany(e => e.Properties.Where(m => m.IsModified && (m.OriginalValue?.ToString() == m.CurrentValue?.ToString()))).ForEach(e => e.IsModified = false);
//
//
//            var allEntries = writableContext.ChangeTracker.Entries().ToList().Where(i => i.State == EntityState.Modified || i.Properties.Any(m => m.IsModified || (m.OriginalValue?.ToString() != m.CurrentValue?.ToString())));
//            allEntries.ForEach(e =>
//            {
//                var propertiesStatuses = e.Properties.Select(p => $"{p.Metadata.Name} is modified: {p.IsModified}, original: {p.OriginalValue}, new value: {p.CurrentValue}").ToList();
//                Console.WriteLine($"{e.Metadata.Name} is {e.State}");
//                propertiesStatuses.ForEach(p =>
//                {
//                    Console.WriteLine($" - {p}");
//                });
//            });


            writableContext.ChangeTracker.DetectChanges();
            logger.LogDebug("*** After detect changes of entities ***");
            CreateLogs(writableContext);
            userName = GetUserNameForAuditing(saveMode, userName);
            var dateToSave = DateTime.UtcNow;
            var operationId = Guid.NewGuid();
            if (!string.IsNullOrEmpty(userName) || saveMode == SaveMode.NonTrackedDataMigration)
            {
                if (saveMode != SaveMode.NonTrackedDataMigration)
                {
                    if (parentEntity != null && writableContext.ChangeTracker.Entries<IAuditing>().Any(x => x.State == EntityState.Added || x.State == EntityState.Modified))
                    {
                        writableContext.Entry(parentEntity).Property("Modified").IsModified = true;
                        writableContext.Entry(parentEntity).Property("ModifiedBy").IsModified = true;
                    }
                    if (saveMode != SaveMode.AllowAnonymous && !preSaveAction.HasFlag(PreSaveAction.DoNotCheckRoles))
                    {
                        CheckRoles(saveMode, writableContext);
                        CheckEntityLock(writableContext, lockedIds);
                    }
                }

                var orgRelevantEntities = roleChecker.AssignmentsToOwnOrganization();
                var userRole = pahaTokenAccessor.UserRole;
                foreach (var updatedEntityEntry in writableContext.ChangeTracker.Entries<IAuditing>().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted))
                {
                    if (updatedEntityEntry.State != EntityState.Deleted)
                    {
                        ApplyFormattingToProperties(updatedEntityEntry);
                        if (!preSaveAction.HasFlag(PreSaveAction.DoNotSetAudits))
                        {
                            SetAuditingFields(updatedEntityEntry, userName, dateToSave, saveMode, (userRole == (int)UserRoleEnum.Eeva && orgRelevantEntities.Any(i => i.Entity.Entity == updatedEntityEntry.Entity && i.IsOwnOrganization == false)));
                        }

                        SetOperationInfoFields(updatedEntityEntry, operationId, dateToSave, (UserRoleEnum)userRole);
                    }
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
                throw new LockException("", new List<string> { lockedEntity.LockedBy});
            }

            var processableStates = new List<EntityState> {EntityState.Added, EntityState.Modified};

            this.DbContext.ChangeTracker.Entries().Where(i => processableStates.Contains(i.State)).ForEach(entityEntry =>
            {
                if (this.saveProcessingActions.TryGetValue(entityEntry.Entity.GetType(), out var preProcessAction))
                {
                    preProcessAction(entityEntry.Entity, entityEntry.State);
                }
            });

            entityTrackingManager.ProcessKnownEntities(
                new TrackingContextInfo
                {
                    UserName = userName,
                    ChangeTracker = this.DbContext.ChangeTracker,
                    OperationId = operationId,
                    TimeStamp = dateToSave,
                    UnitOfWork = this,
                    EntityStates = new List<EntityState> { EntityState.Added, EntityState.Deleted }
                });
            var affectedEntities = writableContext.ChangeTracker.Entries().Count();
            // save changes
            writableContext.SaveChanges();
            // Let's add the log entries only when we are sure database changes have been successfully saved into database.
            logger.LogDBEntries(logEntries);
            return affectedEntities;
        }

        private void SetOperationInfoFields(EntityEntry<IAuditing> updatedEntityEntry, Guid operationId, DateTime dateToSave, UserRoleEnum userRole)
        {
            var additionalOperationType = GetAdditionalOperationType(updatedEntityEntry);

            updatedEntityEntry.Entity.LastOperationIdentifier = operationId;
            updatedEntityEntry.Entity.LastOperationTimeStamp = dateToSave;
            updatedEntityEntry.Entity.LastOperationType = userRole.GetLastOperationType(updatedEntityEntry.State, additionalOperationType);
            updatedEntityEntry.Property("LastOperationIdentifier").IsModified = true;
            updatedEntityEntry.Property("LastOperationTimeStamp").IsModified = true;
            updatedEntityEntry.Property("LastOperationType").IsModified = true;
        }

        private LastOperationType GetAdditionalOperationType(EntityEntry<IAuditing> updatedEntityEntry)
        {
            var additionalOperationType = LastOperationType.None;
            if (updatedEntityEntry.Entity is IPublishingStatus publishableEntity)
            {
                additionalOperationType = GetPublishingOperationType(publishableEntity.PublishingStatusId);
            }

            if (updatedEntityEntry.Entity is ILanguageAvailability languageAvailability)
            {
                additionalOperationType = GetPublishingOperationType(languageAvailability.StatusId);
            }

            return additionalOperationType;
        }

        private LastOperationType GetPublishingOperationType(Guid publishingStatusId)
        {
            if (publishingStatusId == GetPublishingStatusDraft || publishingStatusId == GetPublishingStatusModified)
            {
                return LastOperationType.Edited;
            }

            if (publishingStatusId == GetPublishingStatusPublished)
            {
                return LastOperationType.Published;
            }

            if (publishingStatusId == GetPublishingStatusDeleted ||
                publishingStatusId == GetPublishingStatusOldPublished)
            {
                return LastOperationType.Archived;
            }

            if (publishingStatusId == GetPublishingStatusRemoved)
            {
                return LastOperationType.PermanentlyDeleted;
            }

            return LastOperationType.None;
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
            userName = userName ?? pahaTokenAccessor.UserName;
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
        /// <param name="doNotSetModifiedDate"></param>
        private void SetAuditingFields(EntityEntry<IAuditing> entityEntry, string userName, DateTime savingTimeStamp, SaveMode saveMode, bool doNotSetModifiedDate = false)
        {
            if (saveMode != SaveMode.NonTrackedDataMigration || (saveMode == SaveMode.NonTrackedDataMigration && string.IsNullOrEmpty(entityEntry.Entity.ModifiedBy)))
            {
                bool isModified = true;
                if (entityEntry.State == EntityState.Modified)
                {
                    var typeOfEntity = entityEntry.Entity.GetType();
                    var modifiedProps = entityEntry.Properties.Where(i => i.IsModified).Where(i =>
                    {
                        var property = typeOfEntity.GetProperty(i.Metadata.Name);
                        var attributes = property.GetCustomAttributes().ToList();
                        return i.IsModified && !attributes.Any(j => j is IgnoreWhenModifiedAttribute) && (!attributes.Any(j => j is IgnoreWhenInitilizedAttribute && !IsDefaultValue(i.CurrentValue.GetType(), i.CurrentValue) && IsDefaultValue(i.OriginalValue.GetType(), i.OriginalValue)));
                    });
                    isModified = modifiedProps.Any() || string.IsNullOrEmpty(entityEntry.Entity.ModifiedBy);
                }
                if (isModified)
                {
                    entityEntry.Entity.SetModified(userName, savingTimeStamp);
                    if (!doNotSetModifiedDate)
                    {
                        entityEntry.Property("Modified").IsModified = true;
                    }
                    entityEntry.Property("ModifiedBy").IsModified = true;
                }
            }

            if (string.IsNullOrEmpty(entityEntry.Entity.CreatedBy))
            {
                entityEntry.Entity.SetCreated(userName, savingTimeStamp);
            }
            if (string.IsNullOrEmpty(entityEntry.Entity.ModifiedBy))
            {
                entityEntry.Entity.SetModified(userName, doNotSetModifiedDate ? (DateTime?)null : savingTimeStamp);
            }

            // Notify changes to the "Modified" values when any other data has been modified
//            if (entityEntry.State == EntityState.Modified)
//            {
//                entityEntry.Property("Modified").IsModified = true;
//                entityEntry.Property("ModifiedBy").IsModified = true;
//            }
        }
        
        private static bool IsDefaultValue(Type type, object value)
            => (value?.Equals(default) != false);

        private void ApplyFormattingToProperties(EntityEntry<IAuditing> entityEntry)
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
            return new VmLogEntry
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
            return this.DbContext.Model.FindEntityType(type).GetTableName();
        }
        private LogOperation GetOperation(IAuditing entity)
        {
            var state = this.DbContext.Entry(entity).State;
            switch (state)
            {
                case EntityState.Added: return LogOperation.Create;
                case EntityState.Modified:
                    if (entity is IPublishingStatus valitidyEntity)
                    {
                        if (valitidyEntity.PublishingStatusId == GetPublishingStatusDeleted || valitidyEntity.PublishingStatusId == GetPublishingStatusOldPublished)
                        {
                            return LogOperation.Archive;
                        }
                    }
                    return LogOperation.Update;
                case EntityState.Deleted: return LogOperation.Delete;
                    default: return LogOperation.NotDefined;
            }
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
            ((ITranslationUnitOfWork)this).DetachOrRemoveEntities(new List<T> { entity });
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
