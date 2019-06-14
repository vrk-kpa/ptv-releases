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
using Microsoft.EntityFrameworkCore.Internal;
using NLog.Filters;
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

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IOldArchiveProcessingService), RegisterType.Transient)]
    internal class OldArchiveProcessingService : IOldArchiveProcessingService
    {
        private OldArchivedAncientSettings archivedAncientSettings =new OldArchivedAncientSettings()
            {
                MarkAsRemovedMoreThan = 24,
                RemoveOldContentOlderThan = 50
            };

        private PermanentDeleteSettings permanentDeleteSettings = new PermanentDeleteSettings()
        {
            PhysicallyDropContentOlderThan = 12
        };
        
        private readonly int droppingBatchSize = 25;
        private int removingMarkerBatchSize = 10000;
        private readonly IContextManager contextManager;
        private readonly ITypesCache typesCache;
        private readonly ICloningManager cloningManager;

        

        public OldArchiveProcessingService(IContextManager contextManager, ITypesCache typesCache, ICloningManager cloningManager)
        {
            this.contextManager = contextManager;
            this.typesCache = typesCache;
            this.cloningManager = cloningManager;
        }

        public void ProcessOldContentForMainEntities(OldArchivedAncientSettings oldArchivedAncientSettings)
        {
            this.archivedAncientSettings = oldArchivedAncientSettings;
            removingMarkerBatchSize = Math.Max(removingMarkerBatchSize, oldArchivedAncientSettings.MarkAsRemovedMoreThan+1);
            ProcessOldContentToRemoveState<Service, ServiceVersioned>();
            ProcessOldContentToRemoveState<ServiceChannel, ServiceChannelVersioned>();
        }
        
        public void PermanentDeleteForMainEntities(PermanentDeleteSettings permanentDeleteSettings)
        {
            this.permanentDeleteSettings = permanentDeleteSettings;
            ProcessRemovedToDrop<Service, ServiceVersioned>();
            ProcessRemovedToDrop<ServiceChannel, ServiceChannelVersioned>();
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
//            contextManager.ExecuteWriter(unitOfWork =>
//            {
//                var rep = unitOfWork.CreateRepository<IRepository<TVersion>>();
//                var dataOld = rep.All().Where(i => ((i.PublishingStatusId == psArchived || i.PublishingStatusId == psOldPublished) && (i.Modified < referenceRemoveTime))).Include(i => i.Versioning).ToList()
//                    .GroupBy(j => j.UnificRootId).ToList();
//                dataOld.ForEach(g => g.OrderByDescending(h => h.Modified).ThenByDescending(h => h.Versioning?.VersionMajor ?? 0).ThenByDescending(h => h.Versioning?.VersionMinor ?? 0).Skip(1).ForEach(e => SetAsRemovedEntity(e, psRemoved)));
//                unitOfWork.Save(SaveMode.AllowAnonymous);
//            });

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
        
        public void ProcessRemovedToDrop<TRoot, TVersion>() where TVersion : class,IVersionedVolume<TRoot>, IArchivable where TRoot : VersionedRoot<TVersion>
        {
            var psRemoved = typesCache.Get<PublishingStatusType>(PublishingStatus.Removed.ToString());
            var referenceDropTime = DateTime.UtcNow.AddMonths(-permanentDeleteSettings.PhysicallyDropContentOlderThan);
            bool dropDone = true;
            while (dropDone)
            {
                dropDone = false;
                contextManager.ExecuteWriter(unitOfWork =>
                {
                   var rep = unitOfWork.CreateRepository<IRepository<TVersion>>();
                   var verRep = unitOfWork.CreateRepository<IRepository<Versioning>>();
                   var allEntityVersionings = rep.All().Select(j => j.VersioningId);
                   var toDrop = rep.All().Where(i => (i.PublishingStatusId == psRemoved) && (i.Modified < referenceDropTime) && (i.UnificRoot.Versions.Any(j => j.PublishingStatusId != psRemoved))).Take(droppingBatchSize).ToList();
                   var deletedVersions = toDrop.Select(e =>
                            {
                                cloningManager.DeleteEntity<TVersion>(e, unitOfWork);
                                dropDone = true;
                                return e.Versioning;
                            }).ToList().Where(i => i != null);
                   deletedVersions.ForEach(v =>
                   {
                       var deletable = verRep.All().Where(i => v.UnificRootId == i.UnificRootId && i.Created < v.Created && i.VersionMajor <= v.VersionMajor && !allEntityVersionings.Contains(i.Id));
                       verRep.Remove(v);
                       deletable.ForEach(verRep.Remove);
                   });
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                });
            }
            dropDone = true;
            while (dropDone)
            {
                dropDone = false;
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var rep = unitOfWork.CreateRepository<IRepository<TVersion>>();
                    var allEntityVersionings = rep.All().Select(j => j.VersioningId);
                    var verRep = unitOfWork.CreateRepository<IRepository<Versioning>>();
                    var toDrop = rep.All().Where(i => (i.PublishingStatusId == psRemoved) && (i.Modified < referenceDropTime) && (i.UnificRoot.Versions.Any(j => j.Id != i.Id))).Include(i => i.Versioning).Take(droppingBatchSize).ToList();
                    var grouped = toDrop.GroupBy(i => i.UnificRootId);
                    var deletedVersions = grouped.SelectMany(g => 
                        g.OrderByDescending(h => h.Modified)
                            .ThenByDescending(h => h.Versioning?.VersionMajor ?? 0)
                            .ThenByDescending(h => h.Versioning?.VersionMinor ?? 0)
                            .Skip(1)
                            .Select(e =>
                            {
                                cloningManager.DeleteEntity<TVersion>(e, unitOfWork);
                                dropDone = true;
                                return e.Versioning;
                            }));
                    deletedVersions.ToList().Where(i => i != null).ForEach(v =>
                    {
                        var deletable = verRep.All().Where(i => v.UnificRootId == i.UnificRootId && i.Created < v.Created && i.VersionMajor <= v.VersionMajor && !allEntityVersionings.Contains(i.Id));
                        verRep.Remove(v);
                        deletable.ForEach(verRep.Remove);
                    });
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                });
            }
        }
    }
}