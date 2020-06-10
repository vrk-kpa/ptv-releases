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
    [RegisterService(typeof(IPostalCodeCache), RegisterType.Singleton)]
    internal class PostalCodeCache : ICacheAfterChangeRefresh, IPostalCodeCache
    {
        private CacheBuiltResultData<string, PostalCode> cacheDataByCode;
        private Dictionary<Guid, PostalCode> cacheDataById;
        
        private readonly ICacheBuilder cacheBuilder;

        public PostalCodeCache(ICacheBuilder cacheBuilder)
        {
            RefreshableCaches.Add(this);
            this.cacheBuilder = cacheBuilder;
            CreateCache();
        }

        private void CreateCache()
        {
            cacheDataByCode = cacheBuilder.BuildCache<PostalCode, string, PostalCode>(i => i.Code, i => i, cacheDataByCode);
            cacheDataById = cacheDataByCode.Data.Values.ToDictionary(i => i.Id, i => i);
        }

        public void CheckAndRefresh()
        {
            CreateCache();
        }

        public Guid GuidByCode(string code)
        {
            return cacheDataByCode.Data.TryGetOrDefault(code)?.Id ?? Guid.Empty;
        }
        
        public string CodeByGuid(Guid id)
        {
            return cacheDataById.TryGetOrDefault(id)?.Code ?? string.Empty;
        }
        
        public Guid? MunicipalityIdForCode(string code)
        {
            return cacheDataByCode.Data.TryGetOrDefault(code)?.MunicipalityId;
        }
        
        public Guid? MunicipalityIdForPostalId(Guid id)
        {
            return cacheDataById.TryGetOrDefault(id)?.MunicipalityId;
        }
    }
}
