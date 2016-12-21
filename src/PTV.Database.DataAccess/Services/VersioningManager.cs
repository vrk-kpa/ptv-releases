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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services
{
    /// <summary>
    /// Manager handling versions of entity which uses IVersioned interface
    /// </summary>
    [RegisterService(typeof(IVersioningManager), RegisterType.Transient)]
    internal class VersioningManager : IVersioningManager
    {
        private readonly ICacheManager cacheManager;

        public VersioningManager(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        /// <summary>
        /// Get and return all available versions of entity
        /// </summary>
        /// <typeparam name="TEntityType"></typeparam>
        /// <param name="unitOfWork">Unit Of Work must be provided for correct functionality</param>
        /// <param name="entity">Entity which versions will be retrieved</param>
        /// <returns>List of available versions of specified entity</returns>
        public List<VersionInfo> GetAllVersions<TEntityType>(IUnitOfWork unitOfWork, TEntityType entity) where TEntityType : class, IVersioned
        {
            if (entity?.VersioningId == null)
            {
                return new List<VersionInfo>();
            }
            var verRep = unitOfWork.CreateRepository<IVersioningRepository>();
            var versionsIds = new List<Guid>() {entity.VersioningId.Value};
            FindAllSubVersions(verRep, ref versionsIds);
            var versions = verRep.All().Where(i => versionsIds.Contains(i.Id)).ToDictionary(i => i.Id, i => i);
            var entityRep = unitOfWork.GetSet<TEntityType>();
            var relatedEntities = entityRep.Where(i => versionsIds.Contains(i.VersioningId.Value)).Where(i => i.VersioningId != null).ToDictionary(i => i.VersioningId.Value, i => new {i.Id, i.PublishingStatusId});
            return
                relatedEntities.Select(
                        i =>
                            new VersionInfo()
                            {
                                EntityId = i.Key,
                                VersionMajor = versions[i.Key].VersionMajor,
                                VersionMinor = versions[i.Key].VersionMinor,
                                PublishingStatus = i.Value.PublishingStatusId ?? cacheManager.PublishingStatusCache.Get(PublishingStatus.Published),
                                VersionId = i.Key
                            })
                    .OrderBy(i => i.VersionMajor)
                    .ThenBy(i => i.VersionMinor)
                    .ToList();
        }

        /// <summary>
        /// Check latest version of entity increase it
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be updated</typeparam>
        /// <param name="unitOfWork">Unit Of Work</param>
        /// <param name="entity">Entity which will be updated</param>
        /// <param name="versionType">Major or Minor version will be increased</param>
        public void CreateNewVersion<TEntityType>(IUnitOfWork unitOfWork, TEntityType entity, EVersionType versionType = EVersionType.Minor) where TEntityType : class, IVersioned
        {
            var verRep = unitOfWork.CreateRepository<IVersioningRepository>();
            var currentLatest = GetAllVersions(unitOfWork, entity).LastOrDefault();
            if (currentLatest == null)
            {
                if (entity.PublishingStatus == null && entity.PublishingStatusId == null)
                {
                    entity.PublishingStatusId = cacheManager.PublishingStatusCache.Get(PublishingStatus.Draft.ToString());
                }
                entity.Versioning = verRep.Add(new Versioning() { Id = Guid.NewGuid(), VersionMajor = 0, VersionMinor = 1});
                entity.VersioningId = entity.Versioning.Id;
                return;
            }
            if (versionType == EVersionType.Major)
            {
                currentLatest.VersionMajor++;
                currentLatest.VersionMinor = 0;
            }
            else
            if (versionType == EVersionType.Minor)
            {
                currentLatest.VersionMinor++;
            }
            entity.PublishingStatusId = cacheManager.PublishingStatusCache.Get(PublishingStatus.Modified.ToString());
            entity.PublishingStatus = null;
            entity.Versioning = verRep.Add(new Versioning() { Id = Guid.NewGuid(), VersionMajor = currentLatest.VersionMajor, VersionMinor = currentLatest.VersionMinor, PreviousVersionId = entity.VersioningId});
            entity.VersioningId = entity.Versioning.Id;
        }


        public void CreateModifiedVersion<TEntityType>(IUnitOfWork unitOfWork, TEntityType entity) where TEntityType : class, IVersioned
        {
            var entityRep = unitOfWork.GetSet<TEntityType>();
            var allVersions = GetAllVersions(unitOfWork, entity);
            var currentLatest = allVersions.LastOrDefault() ?? new VersionInfo();
            var latestModifiedVersion = allVersions.LastOrDefault(i => i.PublishingStatus == cacheManager.PublishingStatusCache.Get(PublishingStatus.Modified) || i.PublishingStatus == cacheManager.PublishingStatusCache.Get(PublishingStatus.Draft));
            if (currentLatest.VersionId == latestModifiedVersion?.VersionId)
            {
                currentLatest.VersionMinor++;
                var verRep = unitOfWork.CreateRepository<IVersioningRepository>();
                entity.Versioning = verRep.Add(new Versioning() { Id = Guid.NewGuid(), VersionMajor = currentLatest.VersionMajor, VersionMinor = currentLatest.VersionMinor, PreviousVersionId = entity.VersioningId });
                entity.VersioningId = entity.Versioning.Id;
                return;
            }
        }


        /// <summary>
        /// Publish specified entity, check latest version and create new version with published state
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be promoted to published state</typeparam>
        /// <param name="unitOfWork">Unit Of Work</param>
        /// <param name="entity">Entity which will be promoted to published state</param>
        public void PublishVersion<TEntityType>(IUnitOfWork unitOfWork, TEntityType entity) where TEntityType : class, IVersioned
        {
            var entityRep = unitOfWork.GetSet<TEntityType>();
            var allVersions = GetAllVersions(unitOfWork, entity);
            var currentLatest = allVersions.LastOrDefault() ?? new VersionInfo();
            var previousPublishedVersion = allVersions.LastOrDefault(i => i.PublishingStatus == cacheManager.PublishingStatusCache.Get(PublishingStatus.Published));
            if (previousPublishedVersion != null)
            {
                var previousEntity = entityRep.FirstOrDefault(i => i.Id == previousPublishedVersion.EntityId);
                previousEntity.PublishingStatus = null;
                previousEntity.PublishingStatusId = cacheManager.PublishingStatusCache.Get(PublishingStatus.OldPublished);
            }
            entity.PublishingStatus = null;
            entity.PublishingStatusId = cacheManager.PublishingStatusCache.Get(PublishingStatus.Published);
            currentLatest.VersionMajor++;
            currentLatest.VersionMinor = 0;
            var verRep = unitOfWork.CreateRepository<IVersioningRepository>();
            entity.Versioning = verRep.Add(new Versioning() { Id = Guid.NewGuid(), VersionMajor = currentLatest.VersionMajor, VersionMinor = currentLatest.VersionMinor, PreviousVersionId = entity.VersioningId });
            entity.VersioningId = entity.Versioning.Id;
        }

        private void FindAllSubVersions(IVersioningRepository versioningRep, ref List<Guid> versionsGuids)
        {
            List<Guid> guids = new List<Guid>(versionsGuids);
            var subversions = versioningRep.All().Where(i => guids.Contains(i.Id) || (i.PreviousVersionId != null) && guids.Contains(i.PreviousVersionId.Value)).ToList();
            guids.AddRange(subversions.Select(i => i.Id));
            guids.AddRange(subversions.Where(i => i.PreviousVersionId != null).Select(i => i.PreviousVersionId.Value));
            guids = guids.Distinct().ToList();
            if (guids.Except(versionsGuids).Any())
            {
                versionsGuids.AddRange(guids);
                FindAllSubVersions(versioningRep, ref versionsGuids);
            }
        }
    }
    /// <summary>
    /// Model holding information about versioning
    /// </summary>
    public class VersionInfo
    {
        public Guid VersionId { get; set; }
        public Guid EntityId { get; set; }
        public int VersionMajor { get; set; }
        public int VersionMinor { get; set; }

        public Guid PublishingStatus { get; set; }
    }

    /// <summary>
    /// Version type
    /// </summary>
    public enum EVersionType
    {
        Major,
        Minor
    }
}
