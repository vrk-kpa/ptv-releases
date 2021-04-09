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
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Models;
using PTV.Framework;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(IServiceNumberCache), RegisterType.Singleton)]
    internal class ServiceNumberCache : ICacheAfterChangeRefresh, IServiceNumberCache
    {
        private readonly ICacheBuilder cacheBuilder;
        private CacheBuiltResultData<Guid, ServiceNumber> cacheDataById;

        public ServiceNumberCache(ICacheBuilder cacheBuilder)
        {
            RefreshableCaches.Add(this);
            this.cacheBuilder = cacheBuilder;
            CheckAndRefresh(); 
        }

        private void CreateCache()
        {
            cacheDataById = cacheBuilder.BuildCache<ServiceNumber, Guid, ServiceNumber>(i => i.Id, i => i, cacheDataById);
        }

        public void CheckAndRefresh()
        {
            CreateCache();
        }

        public ServiceNumber Get(Guid key)
        {
            return cacheDataById.Data.TryGetOrDefault(key);
        }

        public Guid GetByValue(ServiceNumber value)
        {
            return cacheDataById.Data.Values.FirstOrDefault(x => x.Number == value.Number)?.Id ?? Guid.Empty;
        }

        public bool Exists(Guid key)
        {
            return cacheDataById.Data.ContainsKey(key);
        }

        public List<ServiceNumber> GetAll()
        {
            return cacheDataById.Data.Values.ToList();
        }
    }
}
