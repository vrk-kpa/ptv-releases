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
using PTV.Database.DataAccess.Services;
using PTV.Database.Model.Models;
using PTV.Framework;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(IMunicipalityCache), RegisterType.Singleton)]
    internal class MunicipalityCache : IMunicipalityCache
    {
        private IContextManager contextManager;
        private ICacheBuilder cacheBuilder;
        private CacheBuiltResultData<string, Municipality> cacheDataByCode;

        public MunicipalityCache(ICacheBuilder cacheBuilder, IContextManager contextManager)
        {
            RefreshableCaches.Add(this);
            this.cacheBuilder = cacheBuilder;
            this.contextManager = contextManager;
            CheckAndRefresh();
        }
        
        public void CheckAndRefresh()
        {
            this.cacheDataByCode =
                cacheBuilder.BuildCache(x => x.Code, x => x.Include(i => i.MunicipalityNames), cacheDataByCode);
        }

        public Municipality Get(string key)
        {
            return cacheDataByCode.Data.TryGetOrDefault(key);
        }

        public string GetByValue(Municipality value)
        {
            return cacheDataByCode.Data.Values.FirstOrDefault(x => x.Id == value?.Id)?.Code;
        }

        public bool Exists(string key)
        {
            return cacheDataByCode.Data.ContainsKey(key);
        }

        public List<Municipality> GetAll()
        {
            return cacheDataByCode.Data.Values.ToList();
        }
    }
}
