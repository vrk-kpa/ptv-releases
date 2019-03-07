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
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(IIndustrialClassCacheInternal), RegisterType.Singleton)]
    internal class IndustrialClassDataCache : LiveDataCache<Guid, IndustrialClass>, IIndustrialClassCacheInternal
    {
        private const int SearchResultLimit = 50;
        
        private Dictionary<string, IndustrialClass> uris = new Dictionary<string, IndustrialClass>();
        private Dictionary<Guid, IndustrialClass> industrialClassIdsAll = new Dictionary<Guid, IndustrialClass>();
        private Dictionary<Guid, IndustrialClass> industrialClassIds = new Dictionary<Guid, IndustrialClass>();
        private List<IndustrialClassName> industrialClassNames = new List<IndustrialClassName>();
        
        private readonly IResolveManager resolveManager;

        public IndustrialClassDataCache(IResolveManager resolveManager)
        {
            MinRefreshInterval = 15;
            this.resolveManager = resolveManager;
        }
        
        protected override void LoadData()
        {
            resolveManager.RunInScope((rm) =>
            {
                var contextManager = rm.Resolve<IContextManager>();
                var cacheBuildTime = CacheBuildTime;
                contextManager.ExecuteIsolatedReader(unitOfWork =>
                {
                    var rep = unitOfWork.CreateRepository<IIndustrialClassRepository>();
                    var allTermQuery = rep.AllPure();
                    var lastChange = allTermQuery.OrderByDescending(i => i.Modified).Select(i => i.Modified).FirstOrDefault();
                    if (lastChange <= cacheBuildTime)
                    {
                        return;
                    }
                    cacheBuildTime = lastChange;
                    this.industrialClassIdsAll = allTermQuery.Where(i => i.IsValid).Include(i => i.Names).ToList().ToDictionary(i => i.Id, i => i);
                    this.industrialClassIds = industrialClassIdsAll.Where(x => !string.IsNullOrEmpty(x.Value.Code)  && x.Value.Code.Trim().Length == 5).ToDictionary(x => x.Key, x => x.Value);
                    this.CacheBuildTime = cacheBuildTime;
                    this.uris = this.industrialClassIds.Values.DistinctBy(i => i.Uri).ToDictionary(i => i.Uri, i => i);
                    this.industrialClassNames = this.industrialClassIds.Values.SelectMany(i => i.Names).OrderBy(i => i.Name).Select(i => new IndustrialClassName(){ Name = i.Name.ToLower(), IndustrialClass = i.IndustrialClass, IndustrialClassId = i.IndustrialClassId, LocalizationId = i.LocalizationId}).ToList();
                });
            });
        }

        public List<IndustrialClass> SearchByName(string name, List<Guid> languagesIds = null)
        {
            GetData();
            name = name.ToLower();
            return industrialClassNames.Where(i => i.Name.Contains(name) && (languagesIds.IsNullOrEmpty() || languagesIds.Contains(i.LocalizationId))).Select(i => i.IndustrialClass).DistinctBy(i => i.Id).Take(SearchResultLimit).ToList();
        }

        public List<IndustrialClass> GetAllForLastLevel()
        {
            GetData();
            return industrialClassIds.Values.ToList();
        }

        public IEnumerable<string> Uris
        {
            get
            {
                GetData();
                return uris.Keys;
            }
        }

        public bool UriExists(string uri)
        {
            GetData();
            return uris.ContainsKey(uri);
        }

        public IList<string> HasUris(IEnumerable<string> toCheckUris)
        {
            GetData();
            return this.uris.Keys.Intersect(toCheckUris).ToList();
        }
    }
}
