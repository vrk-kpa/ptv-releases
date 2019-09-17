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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(ITargetGroupDataCache), RegisterType.Singleton)]
    internal class TargetGroupDataCache : LiveDataCache<Guid, TargetGroup>, ITargetGroupDataCache
    {
        private readonly IResolveManager resolveManager;

        public TargetGroupDataCache(IResolveManager resolveManager)
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
                    var rep = unitOfWork.CreateRepository<ITargetGroupRepository>();
                    var allTargetGroupQuery = rep.AllPure();
                    var lastChange = allTargetGroupQuery.OrderByDescending(i => i.Modified).Select(i => i.Modified).FirstOrDefault();
                    if (lastChange <= cacheBuildTime)
                    {
                        return;
                    }
                    cacheBuildTime = lastChange;
                    this.CacheBuildTime = cacheBuildTime;
                    this.Data = rep.AllPure().Where(i => i.IsValid).Include(i => i.Names).ToList().ToDictionary(i => i.Id, i => i);
                });
            });
        }

        public IReadOnlyList<TargetGroup> GetByCode(IList<string> codes)
        {
            GetData();
            return this.Data.Values.Where(i => codes.Contains(i.Code)).ToList();
        }
        
        public IReadOnlyCollection<TargetGroup> All()
        {
            GetData();
            return this.Data.Values;
        }
    }
}