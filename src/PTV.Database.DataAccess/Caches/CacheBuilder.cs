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
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Caches
{
    internal class CacheBuiltResultData<TKey, TValue>
    {
        public CacheBuiltResultData(Dictionary<TKey, TValue> data, DateTime lastTimestamp)
        {
            this.Data = data;
            this.LatestData = lastTimestamp;
        }

        public Dictionary<TKey, TValue> Data { get; }
        public DateTime LatestData { get; }
    }


    [RegisterService(typeof(ICacheBuilder), RegisterType.Transient)]
    internal class CacheBuilder : ICacheBuilder
    {
        private readonly IResolveManager resolveManager;

        private DateTime ExtractLastDateTime(IEnumerable<IAuditing> data)
        {
            return data.OrderByDescending(i => i.Modified).FirstOrDefault()?.Modified ?? DateTime.MinValue;
        }


        public List<Type> TypeBaseEntities { get; set; }

        public CacheBuilder(IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
        }

        public CacheBuiltResultData<TKey, TValue> BuildCache<TBuildType, TKey, TValue>(Func<TBuildType, TKey> keySelector, Func<TBuildType, TValue> valueSelector, CacheBuiltResultData<TKey, TValue> previousData = null) where TBuildType : class, IAuditing
        {
            var context = resolveManager.Resolve<IContextManager>();
            return context.ExecuteIsolatedReader(unitOfWork =>
            {
                var repository = unitOfWork.CreateRepository<IRepository<TBuildType>>();
                var lastDataFrom = repository.All().OrderByDescending(i => i.Modified).Select(i => i.Modified).FirstOrDefault();
                if (previousData?.LatestData == null || lastDataFrom > previousData.LatestData)
                {
                    var data = repository.All().AsNoTracking().ToList();
                    return new CacheBuiltResultData<TKey, TValue>(data.ToDictionary(keySelector, valueSelector), ExtractLastDateTime(data));
                }
                return previousData;
            });
        }

        public CacheBuiltResultData<string, TBuildType> BuildTypeCache<TBuildType, TNameType>(CacheBuiltResultData<string, TBuildType> previousData = null) where TBuildType : class, IAuditing, ITypeNamed<TNameType> where TNameType : IName
        {
            var context = resolveManager.Resolve<IContextManager>();
            return context.ExecuteIsolatedReader(unitOfWork =>
            {
                var repository = unitOfWork.CreateRepository<IRepository<TBuildType>>();
                var lastDataFrom = repository.All().OrderByDescending(i => i.Modified).Select(i => i.Modified).FirstOrDefault();
                if (previousData?.LatestData == null || lastDataFrom > previousData.LatestData)
                {
                    var data = repository.All()
                        .Include(i => i.Names)
                        .OrderBy(i => i.OrderNumber ?? int.MaxValue)
                        .AsNoTracking().ToList();
                    var cache = data.ToDictionary(i => i.Code, i => i);
                    return new CacheBuiltResultData<string, TBuildType>(cache, ExtractLastDateTime(data));
                }
                return previousData;
            });
        }

        public CacheBuiltResultData<TKey, TBuildType> BuildCache<TBuildType, TKey>(Func<TBuildType, TKey> keySelector, Func<IQueryable<TBuildType>, IQueryable<TBuildType>> includeChain, CacheBuiltResultData<TKey, TBuildType> previousData = null) where TBuildType : class, IAuditing
        {
            var context = resolveManager.Resolve<IContextManager>();
            return context.ExecuteIsolatedReader(unitOfWork =>
            {
                var repository = unitOfWork.CreateRepository<IRepository<TBuildType>>();
                var lastDataFrom = repository.All().OrderByDescending(i => i.Modified).Select(i => i.Modified).FirstOrDefault();
                if (previousData?.LatestData == null || lastDataFrom > previousData.LatestData)
                {
                    var data = unitOfWork.ApplyIncludes(repository.All(), includeChain ?? (query => query))
                        .AsNoTracking().ToList();
                    var cache = data.ToDictionary(keySelector, i => i);
                    return new CacheBuiltResultData<TKey, TBuildType>(cache, ExtractLastDateTime(data));
                }
                return previousData;
            });
        }
    }

    internal interface ICacheBuilder
    {
        CacheBuiltResultData<TKey, TValue> BuildCache<TBuildType, TKey, TValue>(Func<TBuildType, TKey> keySelector, Func<TBuildType, TValue> valueSelector, CacheBuiltResultData<TKey, TValue> previousData = null) where TBuildType : class, IAuditing;

        CacheBuiltResultData<string, TBuildType> BuildTypeCache<TBuildType, TNameType>(CacheBuiltResultData<string, TBuildType> previousData = null) where TBuildType : class, IAuditing, ITypeNamed<TNameType> where TNameType : IName;

        CacheBuiltResultData<TKey, TBuildType> BuildCache<TBuildType, TKey>(Func<TBuildType, TKey> keySelector,
            Func<IQueryable<TBuildType>, IQueryable<TBuildType>> includeChain, CacheBuiltResultData<TKey, TBuildType> previousData = null) where TBuildType : class, IAuditing;

        List<Type> TypeBaseEntities { set; }
    }
}
