using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Caches
{
    internal abstract class LiveDataCache<TKey, TData> : ILiveDataCache<TKey, TData>
    {
        protected int MinRefreshInterval = 0;
        
        protected DateTime UpdatedTime { get; set; } = DateTime.MinValue;

        protected bool ShouldBeRefreshed
        {
            get
            {
                DateTime now = DateTime.UtcNow;
                bool shouldBe = (now - UpdatedTime).TotalMinutes > MinRefreshInterval; 
                UpdatedTime = now;
                return shouldBe;
            }
        }

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


    public interface ILiveCacheManager
    {
        void RegisterLiveCaches();
    }

    [RegisterService(typeof(ILiveCacheManager), RegisterType.Singleton)]
    public class LiveCacheManager : ILiveCacheManager
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
            RegisterCache<IOntologyTermDataCache>();
        }
    }

}
