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
using System.Linq;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Jobs;
using PTV.Framework;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(IJobExecutionCache), RegisterType.Singleton)]
    internal class JobExecutionCache: IEntityCache<string, Guid>, IJobExecutionCache
    {
        private CacheBuiltResultData<string, Guid> cacheData;
        private readonly ICacheBuilder cacheBuilder;

        public JobExecutionCache(ICacheBuilder cacheBuilder)
        {
            RefreshableCaches.Add(this);
            this.cacheBuilder = cacheBuilder;
            CreateCache();
        }

        private void CreateCache()
        {
            cacheData = cacheBuilder.BuildCache<JobExecutionType, string, Guid>(i => i.Code, i => i.Id, cacheData);
        }

        public void CheckAndRefresh()
        {
            CreateCache();
        }

        public Guid Get(string key)
        {
            return cacheData.Data.TryGet(key);
        }

        public Guid Get(JobExecutionTypeEnum jobStatus)
        {
            return Get(jobStatus.ToString());
        }

        public JobExecutionTypeEnum? GetEnumByValue(Guid value)
        {
            if (Enum.TryParse<JobExecutionTypeEnum>(GetByValue(value), out var result))
                return result;
            return null;
        }

        public string GetByValue(Guid value)
        {
            return cacheData.Data.FirstOrDefault(i => i.Value == value).Key;
        }

        public bool Exists(string key)
        {
            return cacheData.Data.ContainsKey(key);
        }
    }
}
