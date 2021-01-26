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
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Caches
{
    internal abstract class LiveDataCache<TKey, TData> : ILiveDataCache<TKey, TData>, ICacheAfterChangeRefresh
    {
        protected int MinRefreshInterval = 15;
        protected readonly int SearchResultLimit = 50;

        /// <summary>
        /// Last time when an attempt to refresh the cache was made.
        /// </summary>
        protected DateTime UpdatedTime { get; set; } = DateTime.MinValue;
        
        /// <summary>
        /// The highest modified date from the database.
        /// </summary>
        protected DateTime CacheBuildTime { get; set; } = DateTime.MinValue;

        protected bool ShouldBeRefreshed
        {
            get
            {
                DateTime now = DateTime.UtcNow;
                bool shouldBe = (now - UpdatedTime).TotalMinutes > MinRefreshInterval;
                if (shouldBe)
                {
                    UpdatedTime = now;
                }
                return shouldBe;
            }
        }

        protected Dictionary<TKey, TData> Data { get; set; }

        protected abstract void LoadData();

        protected abstract bool HasNewData(IUnitOfWork unitOfWork);

        public void InitRefreshCache()
        {
            lock (this)
            {
                if (ShouldBeRefreshed)
                {
                    LoadData();
                }
            }
        }

        public Dictionary<TKey, TData> GetData()
        {
           InitRefreshCache();
           return Data;
        }

        public void CheckAndRefresh()
        {
            GetData();
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
            RegisterCache<ITargetGroupDataCache>();
            RegisterCache<IServiceClassCacheInternal>();
            RegisterCache<ILifeEventCacheInternal>();
            RegisterCache<IDigitalAuthorizationCacheInternal>();
        }
    }

}
