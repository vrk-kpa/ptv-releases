using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.Model.Models;
using PTV.Database.Model.ServiceDataHolders;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Caches
{
    internal interface ILiveDataCache
    {
        void InitRefreshCache();
    }
    internal interface ILiveDataCache<TKey, TData> : ILiveDataCache
    {
        Dictionary<TKey, TData> GetData();
    }

    internal abstract class LiveDataCache<TKey, TData> : ILiveDataCache<TKey, TData>
    {
        protected DateTime CacheBuildTime { get; set; } = DateTime.MinValue;

        protected Dictionary<TKey, TData> Data { get; set; }

        protected abstract LiveCacheResult<TKey, TData> LoadData();
        public abstract void InitRefreshCache();

        public Dictionary<TKey, TData> GetData()
        {
            lock (this)
            {
                InitRefreshCache();
                return Data;
            }
        }
    }

    internal interface IOrganizationTreeDataCache : ILiveDataCache<Guid, OrganizationTreeItem>
    {
    }

    [RegisterService(typeof(IOrganizationTreeDataCache), RegisterType.Singleton)]
    internal class OrganizationTreeDataCache : LiveDataCache<Guid, OrganizationTreeItem>, IOrganizationTreeDataCache
    {
        private readonly IResolveManager resolveManager;

        public OrganizationTreeDataCache(IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
            InitRefreshCache();
        }

        public override void InitRefreshCache()
        {
            var cacheResult = LoadData();
            if (cacheResult == null) return;
            this.CacheBuildTime = cacheResult.BuildTime;
            this.Data = cacheResult.Data;
        }

        protected override LiveCacheResult<Guid, OrganizationTreeItem> LoadData()
        {
            LiveCacheResult<Guid, OrganizationTreeItem> cachedData = null;
            resolveManager.RunInScope((rm) =>
            {
                var contextManager = rm.Resolve<IContextManager>();
                var psCache = rm.Resolve<IPublishingStatusCache>();

                var cacheBuildTime = CacheBuildTime;
                var allOrgs = contextManager.ExecuteReader(unitOfWork =>
                {
                    var loaded = new List<Guid>();
                    var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                    var psPublished = psCache.Get(PublishingStatus.Published);
                    var publishedOrgs = orgRep.All().Where(i => i.PublishingStatusId == psPublished);
                    var lastestChange = publishedOrgs.OrderByDescending(i => i.Modified).Select(i => i.Modified).FirstOrDefault();
                    if (lastestChange <= cacheBuildTime)
                    {
                        return (List<OrganizationTreeItem>)null;
                    }
                    cacheBuildTime = lastestChange;
                    var allOrganization = new List<OrganizationTreeItem>();
                    LoadOrganizationTreeRecursive(publishedOrgs, loaded, null, allOrganization);
                    return allOrganization;
                });
                if (allOrgs != null)
                {
                    cachedData = new LiveCacheResult<Guid, OrganizationTreeItem>(cacheBuildTime, allOrgs.ToDictionary(i => i.UnificRootId, i => i));
                }
            });
            return cachedData;
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
            var orgs = query.ToList().GroupBy(x => x.UnificRootId).Select(x => x.OrderByDescending(i => i.Modified).FirstOrDefault()).ToList();
            var result = orgs.Select(i => new OrganizationTreeItem() { Organization = i, UnificRootId = i.UnificRootId, Children = new List<OrganizationTreeItem>() }).ToList();
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

    public class LiveCacheManager
    {
        private readonly IResolveManager resolveManager;

        public LiveCacheManager(IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
        }

        private void RegisterCache<T>() where T : class, ILiveDataCache
        {
            var cache = resolveManager.Resolve<T>();
            cache.InitRefreshCache();
        }


        public void RegisterLiveCaches()
        {
            RegisterCache<IOrganizationTreeDataCache>();
        }
    }

}
