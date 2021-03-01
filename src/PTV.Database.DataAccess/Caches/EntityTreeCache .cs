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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Framework;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Privileges;
using PTV.Domain.Model.Models;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Caches
{
    internal class EntityTreeCache<T, T2> : IEntityCache<string, Guid> where T : class, IAuditing, IFintoItemBase, IFintoItemNames<T2>, IEntityIdentifier where T2 : IName
    {
        private CacheBuiltResultData<Guid, T> cacheResult;
        private Dictionary<string, T> cacheData;
        private readonly ICacheBuilder cacheBuilder;

        private void CreateCache()
        {
            cacheResult = cacheBuilder.BuildCache<T, Guid>(entity => entity.Id, q => q.Include(i => i.Names), cacheResult);
            cacheData = cacheResult.Data.ToDictionary(kp => kp.Value.Code.ToLower(), kp => kp.Value);
        }

        public EntityTreeCache(ICacheBuilder cacheBuilder)
        {
            this.cacheBuilder = cacheBuilder;
            CreateCache();
        }

        public Guid Get(string key)
        {
            var guid = cacheData.TryGet(key.ToLower())?.Id;
            if (guid == null)
            {
                throw new ArgumentOutOfRangeException($"Guid for type '{typeof(T).Name}' with code '{key}' not found in cache!");
            }
            return guid.Value;
        }

        public string GetByValue(Guid id)
        {
            var code = cacheResult.Data.TryGet(id)?.Code;
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentOutOfRangeException($"Type '{typeof(T).Name}' for guid '{id}' not found in cache!");
            }
            return code;
        }

        public bool Exists(string key)
        {
            return this.cacheData.ContainsKey(key);
        }

        public ICollection<T> GetCompleteStuct()
        {
            return cacheData.Values;
        }

        public void CheckAndRefresh()
        {
            CreateCache();
        }
    }


    [RegisterService(typeof(IEntityTreesCache), RegisterType.Singleton)]
    internal class EntityTreesCache : IEntityTreesCache
    {
        private readonly Dictionary<Type,  IEntityCache<string, Guid>> cacheData = new Dictionary<Type,  IEntityCache<string, Guid>>();
//        private readonly Dictionary<string, Type> nameToTypeMap = new Dictionary<string, Type>();
        private readonly ICacheBuilder cacheBuilder;

        public EntityTreesCache(ICacheBuilder cacheBuilder)
        {
            this.cacheBuilder = cacheBuilder;
            Refresh();
        }

//        public ICollection<VmType> GetCacheData(string cacheName)
//        {
//            var cache = GetCache(cacheName);
//            return cache?.GetCompleteStuct();
//        }

//        public IEntityCache<string, Guid> GetCache(string cacheName)
//        {
//            var cacheType = nameToTypeMap.TryGet(cacheName);
//            return cacheType == null ? null : cacheData.TryGet(cacheType);
//        }

//        public ICollection<VmType> GetCacheData<T>() where T : IType
//        {
//            var cache = cacheData.TryGet(typeof(T));
//            return cache?.GetCompleteStuct();
//        }

        private void AddCache<T, T2>() where T : class, IAuditing, IFintoItemBase, IFintoItemNames<T2>, IEntityIdentifier where T2 : IName
        {
            cacheData.Add(typeof(T), new EntityTreeCache<T, T2>(cacheBuilder));
//            nameToTypeMap.Add(cacheServiceStringName ?? typeof(T).Name, typeof(T));
        }

        private IEntityCache<string, Guid> GetCache<T>() where T : IType
        {
            var typeData = cacheData.TryGet(typeof(T));
            if (typeData == null)
            {
                throw new Exception($"Requested type {typeof(T).Name} is not added to cache.");
            }
            return typeData;
        }

        public Guid Get<T>(string key) where T : IType
        {
            return Get<T>(key, null);
        }

        public Guid Get<T>(string key, string defaultKey) where T : IType
        {
            Guid guid = GetCache<T>().Get(key);
            if (!string.IsNullOrEmpty(defaultKey) && !guid.IsAssigned())
            {
                return GetCache<T>().Get(defaultKey);
            }
            return guid;
        }

        public string GetByValue<T>(Guid id) where T : IType
        {
            return GetCache<T>().GetByValue(id);
        }

        public void Refresh()
        {
            cacheData.Clear();
//            nameToTypeMap.Clear();
            AddCache<TargetGroup, TargetGroupName>();

        }
    }
}
