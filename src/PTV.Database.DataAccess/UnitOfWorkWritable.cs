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
using System.Security.Cryptography;
using System.Threading.Tasks;
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

namespace PTV.Database.DataAccess
{
    /// <summary>
    /// Unit of Work is scope of database operations. Writable type provides Save method which allows to make changes in database
    /// </summary>
    [RegisterService(typeof(IUnitOfWorkWritable), RegisterType.Transient)]
    internal class UnitOfWorkWritable : UnitOfWork, IUnitOfWorkWritable
    {
        private readonly IUserIdentification userIdentification;
        private readonly ILogger logger;
        private readonly ApplicationConfiguration applicationConfiguration;

        public UnitOfWorkWritable(IResolveManager resolveManager, PtvDbContext dbContext, IUserIdentification userIdentification, ILogger<UnitOfWorkWritable> logger, ApplicationConfiguration applicationConfiguration) : base(resolveManager, dbContext)
        {
            this.userIdentification = userIdentification;
            this.logger = logger;
            this.applicationConfiguration = applicationConfiguration;
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
            var writableContext = this.DbContext as DbContext;
            writableContext.ChangeTracker.DetectChanges();
            userName = userName ?? userIdentification.UserName;
            if (string.IsNullOrEmpty(userName) && (saveMode == SaveMode.AllowAnonymous))
            {
                userName = "PTVapp";
            }
            if (!string.IsNullOrEmpty(userName))
            {
                if (applicationConfiguration.LogSavedEntities)
                {
                    var entries = writableContext.ChangeTracker.Entries().ToList();

                    var toAdd = entries.Where(x => x.State == EntityState.Added).Select(i => i.Entity.GetType().Name).ToArray();
                    var toModify = entries.Where(x => x.State == EntityState.Modified).Select(i => i.Entity.GetType().Name).ToArray();
                    var toDelete = entries.Where(x => x.State == EntityState.Deleted).Select(i => i.Entity.GetType().Name).ToArray();
                    var toDetach = entries.Where(x => x.State == EntityState.Detached).Select(i => i.Entity.GetType().Name).ToArray();

                    string infoMessage = "----- Context SAVE -----\n"
                                         + string.Format("Entities Added: {0}\n", string.Join(",", toAdd))
                                         + string.Format("Entities Modified: {0}\n", string.Join(",", toModify))
                                         + string.Format("Entities Deleted: {0}\n", string.Join(",", toDelete))
                                         + string.Format("Entities Detached: {0}\n", string.Join(",", toDetach));

                    logger.LogDebug(infoMessage);
                };

                if (parentEntity != null && writableContext.ChangeTracker.Entries<IAuditing>().Any(x => x.State == EntityState.Added || x.State == EntityState.Modified))
                {
                    writableContext.Entry(parentEntity).Property("Modified").IsModified = true;
                    writableContext.Entry(parentEntity).Property("ModifiedBy").IsModified = true;
                }

                foreach (var updatedEntityEntry in writableContext.ChangeTracker.Entries<IAuditing>().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified))
                {
                    SetAuditingFields(updatedEntityEntry, userName);
                    logEntries.Add(GetLogEntry(updatedEntityEntry.Entity));
                }
            }
            else
            {
                throw new Exception(CoreMessages.AnonymousSaveNotAllowed);
            }

            writableContext.SaveChanges();
            // Let's add the log entries only when we are sure database changes have been successfully saved into database.
            logger.LogDBEntries(logEntries);
        }

        /// <summary>
        /// Sets auditing fields of entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userName"></param>
        private void SetAuditingFields(EntityEntry<IAuditing> entityEntry, string userName)
        {
            var savingTimeStamp = DateTime.UtcNow;
            entityEntry.Entity.Modified = savingTimeStamp;
            entityEntry.Entity.ModifiedBy = userName;

            if (string.IsNullOrEmpty(entityEntry.Entity.CreatedBy))
            {
                entityEntry.Entity.Created = savingTimeStamp;
                entityEntry.Entity.CreatedBy = userName;
            }

            // Notify changes to the "Modified" values when any other data has been modified
            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property("Modified").IsModified = true;
                entityEntry.Property("ModifiedBy").IsModified = true;
            }
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
    }
}
