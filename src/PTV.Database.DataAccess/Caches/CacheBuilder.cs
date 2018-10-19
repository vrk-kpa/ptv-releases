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
    [RegisterService(typeof(ICacheBuilder), RegisterType.Transient)]
    internal class CacheBuilder : ICacheBuilder
    {
        private readonly IResolveManager resolveManager;

        public List<Type> TypeBaseEntities { get; set; }

        public CacheBuilder(IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
        }

        public Dictionary<TKey, TValue> BuildCache<TBuildType, TKey, TValue>(Func<TBuildType, TKey> keySelector, Func<TBuildType, TValue> valueSelector) where TBuildType : class
        {
            var context = resolveManager.Resolve<IContextManager>();
            return context.ExecuteIsolatedReader(unitOfWork =>
            {
                var repository = resolveManager.Resolve<IRepository<TBuildType>>();
                var cache = repository.All().AsNoTracking().ToDictionary(keySelector, valueSelector);
                return cache;
            });
        }

        public Dictionary<string, TBuildType> BuildTypeCache<TBuildType, TNameType>() where TBuildType : class, ITypeNamed<TNameType> where TNameType : IName
        {
            var context = resolveManager.Resolve<IContextManager>();
            return context.ExecuteIsolatedReader(unitOfWork =>
            {
                var repository = resolveManager.Resolve<IRepository<TBuildType>>();
                var cache = repository.All()
                    .Include(i => i.Names)
                    .OrderBy(i => i.OrderNumber ?? int.MaxValue)
                    .AsNoTracking()
                    .ToDictionary(i => i.Code, i => i);
                return cache;
            });
        }
        
        public Dictionary<TKey, TBuildType> BuildCache<TBuildType, TKey>(Func<TBuildType, TKey> keySelector, Func<IQueryable<TBuildType>, IQueryable<TBuildType>> includeChain) where TBuildType : class
        {
            var context = resolveManager.Resolve<IContextManager>();
            return context.ExecuteIsolatedReader(unitOfWork =>
            {
                var repository = resolveManager.Resolve<IRepository<TBuildType>>();
                var cache = unitOfWork.ApplyIncludes(repository.All(), includeChain ?? (query => query))
                    .AsNoTracking()
                    .ToDictionary(keySelector, i => i);
                return cache;
            });
        }
    }

    internal interface ICacheBuilder
    {
        Dictionary<TKey, TValue> BuildCache<TBuildType, TKey, TValue>(Func<TBuildType, TKey> keySelector, Func<TBuildType, TValue> valueSelector) where TBuildType : class;

        Dictionary<string, TBuildType> BuildTypeCache<TBuildType, TNameType>() where TBuildType : class, ITypeNamed<TNameType> where TNameType : IName;

        Dictionary<TKey, TBuildType> BuildCache<TBuildType, TKey>(Func<TBuildType, TKey> keySelector,
            Func<IQueryable<TBuildType>, IQueryable<TBuildType>> includeChain) where TBuildType : class;
        
        List<Type> TypeBaseEntities { set; }
    }
}
