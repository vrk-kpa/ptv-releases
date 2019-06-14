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
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Database.Model.ServiceDataHolders;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(IOrganizationTreeDataCache), RegisterType.Singleton)]
    internal class OrganizationTreeDataCache : LiveDataCache<Guid, OrganizationTreeItem>, IOrganizationTreeDataCacheWithEntity
    {
        private readonly IResolveManager resolveManager;
        private Dictionary<Guid, OrganizationVersioned> flatData;
        private Dictionary<Guid, VmListItemWithStatus> flatDataVm;
        private Dictionary<Guid, OrganizationTreeItem> sahaIdData;
        private Dictionary<Guid, Guid> sahaPtvMapping;
        private readonly ITranslationEntity translationEntity;
        private readonly Dictionary<string, Guid> publishingStatuses;

        public OrganizationTreeDataCache(IResolveManager resolveManager, ITranslationEntity translationEntity, ITypesCache typesCache)
        {
            MinRefreshInterval = -1;
            this.resolveManager = resolveManager;
            this.translationEntity = translationEntity;
            publishingStatuses = typesCache.GetCacheData<PublishingStatusType>().ToDictionary(i => i.Code, i => i.Id);
            InitRefreshCache();
        }

        public Dictionary<Guid, OrganizationVersioned> GetFlatDataEntities()
        {
            GetData();
            return this.flatData;
        }

        public Dictionary<Guid, VmListItemWithStatus> GetFlatDataVm()
        {
            GetData();
            return this.flatDataVm;
        }

        public Guid? FindBySahaId(Guid sahaId)
        {
            var org = this.sahaIdData.TryGetOrDefault(sahaId, null);
            if (org != null) return org.UnificRootId;
            org = this.Data.TryGetOrDefault(sahaId, null);
            if (org != null) return org.UnificRootId;
            GetData();
            org = this.Data.TryGetOrDefault(sahaId, null) ?? this.sahaIdData.TryGetOrDefault(sahaId, null);
            return org?.UnificRootId;
        }

        public Dictionary<Guid, Guid> SahaMappings()
        {
            return sahaPtvMapping;
        }

        protected override void LoadData()
        {
            LiveCacheResult<Guid, OrganizationTreeItem> cacheResult = null;
            resolveManager.RunInScope((rm) =>
            {
                var contextManager = rm.Resolve<IContextManager>();
                var cacheBuildTime = CacheBuildTime;
                var allOrgs = contextManager.ExecuteIsolatedReader(unitOfWork =>
                {
                    var loaded = new List<Guid>();
                    var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                    var sahaRep = unitOfWork.CreateRepository<ISahaOrganizationInformationRepository>();
                    var allOrgQuery = orgRep.AllPure().Where(i => i.PublishingStatusId != publishingStatuses.TryGet(PublishingStatus.Removed.ToString()));
                    var lastChange = CoreExtensions.Max(allOrgQuery.OrderByDescending(i => i.Modified).Select(i => i.Modified).FirstOrDefault(),
                        sahaRep.AllPure().OrderByDescending(i => i.Modified).Select(i => i.Modified).FirstOrDefault());
                    if (lastChange <= cacheBuildTime)
                    {
                        return (List<OrganizationTreeItem>)null;
                    }
                    cacheBuildTime = lastChange;
                    var allOrganization = new List<OrganizationTreeItem>();
                    LoadOrganizationTreeRecursive(allOrgQuery.Where(i => i.PublishingStatusId != publishingStatuses[PublishingStatus.OldPublished.ToString()]), loaded, null, allOrganization);
                    return allOrganization;
                });
                if (allOrgs != null)
                {
                    cacheResult = new LiveCacheResult<Guid, OrganizationTreeItem>(cacheBuildTime, allOrgs.ToDictionary(i => i.UnificRootId, i => i));
                }
            });
            if (cacheResult == null) return;
            this.CacheBuildTime = cacheResult.BuildTime;
            this.Data = cacheResult.Data;
            this.sahaIdData = this.Data.Values.Where(i => i.SahaId.IsAssigned()).DistinctBy(i => i.SahaId.Value).ToDictionary(i => i.SahaId.Value, i => i);
            this.flatData = this.Data.Values.Select(i => i.Organization).ToDictionary(i => i.UnificRootId, i => i);
            this.flatDataVm = translationEntity.TranslateAll<OrganizationVersioned, VmListItemWithStatus>(this.flatData.Values).ToDictionary(i => i.Id, i => i);
            this.sahaPtvMapping = this.sahaIdData.ToDictionary(i => i.Key, i => i.Value.UnificRootId).Merge(this.sahaIdData.ToDictionary(i => i.Value.UnificRootId, i => i.Key));
        }
        
        private OrganizationVersioned GetOneOrgByStatus(IEnumerable<OrganizationVersioned> data)
        {
            var psPublished = publishingStatuses[PublishingStatus.Published.ToString()];
            var psDraft = publishingStatuses[PublishingStatus.Draft.ToString()];
            var psModified = publishingStatuses[PublishingStatus.Modified.ToString()];
            var psDeleted = publishingStatuses[PublishingStatus.Deleted.ToString()];
            OrganizationVersioned itemPublished = null;
            OrganizationVersioned itemDraft = null;
            OrganizationVersioned itemModified = null;
            OrganizationVersioned itemDeleted = null;
            foreach (var item in data.OrderBy(i => i.Modified))
            {
                if (item.PublishingStatusId == psPublished) itemPublished = item;
                if (item.PublishingStatusId == psDraft) itemDraft = item;
                if (item.PublishingStatusId == psModified) itemModified = item;
                if (item.PublishingStatusId == psDeleted) itemDeleted = item;
            }
            return itemPublished ?? itemDraft ?? itemModified ?? itemDeleted;
        }

        private List<OrganizationTreeItem> LoadOrganizationTreeRecursive(IQueryable<OrganizationVersioned> inputQuery,
            List<Guid> loaded,
            ICollection<Guid> ids,
            List<OrganizationTreeItem> allOrganization)
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
            var orgs = query.Include(i => i.OrganizationNames).Include(i => i.LanguageAvailabilities).Include(i => i.OrganizationDisplayNameTypes).Include(i => i.UnificRoot).ThenInclude(i => i.SahaOrganizationInformations).ToList().GroupBy(x => x.UnificRootId).Select(GetOneOrgByStatus).ToList();
            var result = orgs.Where(i => !loaded.Contains(i.UnificRootId)).Select(i => new OrganizationTreeItem() { Organization = i, UnificRootId = i.UnificRootId, SahaId = i.UnificRoot.SahaOrganizationInformations.FirstOrDefault()?.SahaId, Children = new List<OrganizationTreeItem>() }).ToList();
            var subRootIds = orgs.Select(i => i.UnificRootId).Distinct().Except(loaded).ToList();
            loaded.AddRange(subRootIds);
            if (subRootIds.Count > 0)
            {
                var subResult = LoadOrganizationTreeRecursive(inputQuery, loaded, subRootIds, allOrganization);
                result.ForEach(parent =>
                {
                    parent.Children = subResult.Where(i => i.Organization.ParentId == parent.UnificRootId).ToList();
                    parent.Children.ForEach(ch =>
                    {
                        ch.Parent = parent;
                    });
                });
            }
            allOrganization.AddRange(result);
            return result;
        }
    }
}