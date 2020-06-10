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
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.EntityCloners;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Logging;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IOldArchiveProcessingService), RegisterType.Transient)]
    internal class OldArchiveProcessingService : IOldArchiveProcessingService
    {
        private OldArchivedAncientSettings archivedAncientSettings =new OldArchivedAncientSettings
        {
                MarkAsRemovedMoreThan = 10,
                RemoveOldContentOlderThan = 15
            };

        private PermanentDeleteSettings permanentDeleteSettings = new PermanentDeleteSettings
        {
            PhysicallyDropContentOlderThan = 12,
            MaxExecutionTimeInHours = 8
        };

        private readonly int droppingBatchSize = 50;
        private int removingMarkerBatchSize = 10000;
        private readonly IContextManager contextManager;
        private readonly ITypesCache typesCache;
        private readonly ICloningManager cloningManager;
        private readonly ILogger<OldArchiveProcessingService> jobLogger;

        public OldArchiveProcessingService(IContextManager contextManager, ITypesCache typesCache, ICloningManager cloningManager, ILogger<OldArchiveProcessingService> jobLogger)
        {
            this.contextManager = contextManager;
            this.typesCache = typesCache;
            this.cloningManager = cloningManager;
            this.jobLogger = jobLogger;
        }

        public void ProcessOldContentForMainEntities(OldArchivedAncientSettings oldArchivedAncientSettings, VmJobLogEntry loggerInfo)
        {
            jobLogger.LogSchedulerInfo(loggerInfo,$"oldArchivedAncientSettings from jobsettings MarkAsRemovedMoreThan:{oldArchivedAncientSettings.MarkAsRemovedMoreThan} and RemoveOldContentOlderThan: {oldArchivedAncientSettings.RemoveOldContentOlderThan}.");
            this.archivedAncientSettings = oldArchivedAncientSettings;
            jobLogger.LogSchedulerInfo(loggerInfo, $"archivedAncientSettings after assignment MarkAsRemovedMoreThan:{archivedAncientSettings.MarkAsRemovedMoreThan} and RemoveOldContentOlderThan: {archivedAncientSettings.RemoveOldContentOlderThan}.");

            removingMarkerBatchSize = Math.Max(removingMarkerBatchSize, oldArchivedAncientSettings.MarkAsRemovedMoreThan+1);
            ProcessOldContentToRemoveState<Service, ServiceVersioned>();
            ProcessOldContentToRemoveState<ServiceChannel, ServiceChannelVersioned>();
        }

        public void PermanentDeleteForMainEntities(PermanentDeleteSettings permanentDeleteSettings, VmJobLogEntry loggerInfo)
        {
            jobLogger.LogSchedulerInfo(loggerInfo, $"permanentDeleteSettings from jobsettings PhysicallyDropContentOlderThan:{permanentDeleteSettings.PhysicallyDropContentOlderThan} and MaxExecutionTimeInHours: {permanentDeleteSettings.MaxExecutionTimeInHours}.");
            this.permanentDeleteSettings = permanentDeleteSettings;
            jobLogger.LogSchedulerInfo(loggerInfo, $"permanentDeleteSettings after assignment PhysicallyDropContentOlderThan:{this.permanentDeleteSettings.PhysicallyDropContentOlderThan} and MaxExecutionTimeInHours: {this.permanentDeleteSettings.MaxExecutionTimeInHours}.");
            var executionTime = DateTime.UtcNow.AddHours(permanentDeleteSettings.MaxExecutionTimeInHours);
            jobLogger.LogSchedulerInfo(loggerInfo, $"PermanentDeleteForMainEntities utc time now:{DateTime.UtcNow:dd.MM.yyyy HH:mm:ss} execution time: {executionTime:dd.MM.yyyy HH:mm:ss}");
            ProcessRemovedToDrop<Service, ServiceVersioned>(executionTime, loggerInfo);
            ProcessRemovedToDrop<ServiceChannel, ServiceChannelVersioned>(executionTime, loggerInfo);
        }

        private Guid SetAsRemovedEntity(IVersionedVolume entity, Guid psRemoved)
        {
            entity.PublishingStatusId = psRemoved;
            entity.Versioning.Ignored = true;
            return entity.VersioningId.Value;
        }

        public void ProcessOldContentToRemoveState<TRoot, TVersion>() where TVersion : class,IVersionedVolume<TRoot>, IArchivable where TRoot : VersionedRoot<TVersion>
        {
            var psArchived = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var psRemoved = typesCache.Get<PublishingStatusType>(PublishingStatus.Removed.ToString());
            var referenceRemoveTime = DateTime.UtcNow.AddMonths(-archivedAncientSettings.RemoveOldContentOlderThan);

            contextManager.ExecuteWriter(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IRepository<TVersion>>();
                var dataOld = rep.All()
                    .Where(j => j.UnificRoot.Versions.Any(i =>
                        (i.PublishingStatusId == psArchived || i.PublishingStatusId == psOldPublished) && (i.Modified < referenceRemoveTime))).Include(i => i.Versioning).ToList()
                    .GroupBy(j => j.UnificRootId);
                var affectedVersionings = dataOld.SelectMany(g =>
                    g.OrderByDescending(h => h.Modified).ThenByDescending(h => h.Versioning?.VersionMajor ?? 0).ThenByDescending(h => h.Versioning?.VersionMinor ?? 0).Skip(1)
                        .Where(i => (i.PublishingStatusId == psArchived || i.PublishingStatusId == psOldPublished) && (i.Modified < referenceRemoveTime))
                        .Select(e => SetAsRemovedEntity(e, psRemoved))).ToList();
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
            bool processed = true;
            while (processed)
            {
                processed = false;
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var rep = unitOfWork.CreateRepository<IRepository<TVersion>>();
                    var dataMoreTen = rep.All()
                        .Where(i => (i.PublishingStatusId == psArchived || i.PublishingStatusId == psOldPublished) &&
                                    (i.UnificRoot.Versions.Count(j => (j.PublishingStatusId == psArchived || j.PublishingStatusId == psOldPublished)) >
                                     archivedAncientSettings.MarkAsRemovedMoreThan)).Include(i => i.Versioning).OrderBy(i => i.UnificRootId).Take(removingMarkerBatchSize).ToList().GroupBy(i => i.UnificRootId);
                    dataMoreTen.ForEach(g =>
                    {
                        g.OrderByDescending(j => j.Modified)
                            .ThenByDescending(h => h.Versioning?.VersionMajor ?? 0)
                            .ThenByDescending(h => h.Versioning?.VersionMinor ?? 0)
                            .Skip(archivedAncientSettings.MarkAsRemovedMoreThan)
                            .ForEach(e =>
                            {
                                processed = true;
                                SetAsRemovedEntity(e, psRemoved);
                            });
                    });
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                });
            }
        }

        public void ProcessRemovedToDrop<TRoot, TVersion>(DateTime executionTime, VmJobLogEntry loggerInfo) where TVersion : class, IVersionedVolume<TRoot>, IArchivable where TRoot : VersionedRoot<TVersion>
        {
            var psRemoved = typesCache.Get<PublishingStatusType>(PublishingStatus.Removed.ToString());
            jobLogger.LogSchedulerInfo(loggerInfo, $"$ProcessRemovedToDrop utc time now:{DateTime.UtcNow:dd.MM.yyyy HH:mm:ss} execution time: {executionTime:dd.MM.yyyy HH:mm:ss}");
            var toRemoveCount = GetToRemoveCount<TVersion>(psRemoved);

            while (toRemoveCount > 0 && executionTime > DateTime.UtcNow)
            {
                if (Debugger.IsAttached)
                {
                    Console.WriteLine($"{DateTime.UtcNow} - Remaining {toRemoveCount} {typeof(TRoot).Name}s to remove.");
                }

                var versioningIds = contextManager.ExecuteWriter(unitOfWork =>
                {
                    var rep = unitOfWork.CreateRepository<IRepository<TVersion>>();
                    var toDrop = rep.All()
                        .Where(i => i.PublishingStatusId == psRemoved)
                        .Take(droppingBatchSize)
                        .ToList();

                    var result = toDrop.Select(x => x.VersioningId).Distinct().ToList();

                    foreach (var entity in toDrop)
                    {
                        cloningManager.DeleteEntity(entity, unitOfWork);
                    }
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                    return result;
                });

                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var repo = unitOfWork.CreateRepository<IVersioningRepository>();
                    var versionings = repo.All()
                        .Where(x => versioningIds.Contains(x.Id))
                        .ToList();
                    var unificRootIds = versionings.Select(x => x.UnificRootId).Distinct().ToList();

                    var toRemove = repo.All()
                        .Include(x => x.Channels)
                        .Include(x => x.Services)
                        .Where(x => unificRootIds.Contains(x.UnificRootId))
                        // This is OK, because the droppingBatchSize is small enough, so only limited amount of data
                        // will be loaded to the memory.
                        .ToList()
                        .Where(x => versionings.Any(y => y.UnificRootId == x.UnificRootId
                                                         && x.VersionMajor <= y.VersionMajor
                                                         // Check only for services and channels, since only they are
                                                         // being archived.
                                                         && !x.Channels.Any()
                                                         && !x.Services.Any()))
                        .ToList();
                    repo.Remove(toRemove);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                });
                
                toRemoveCount = GetToRemoveCount<TVersion>(psRemoved);
            }
        }

        private int GetToRemoveCount<TVersion>(Guid psRemoved) where TVersion : class, IVersionedVolume, IArchivable
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var repo = unitOfWork.CreateRepository<IRepository<TVersion>>();
                return repo.All().Count(x => x.PublishingStatusId == psRemoved);
            });
        }
    }
}
