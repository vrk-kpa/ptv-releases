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
using System.Reflection;
using PTV.Database.DataAccess.Services;
using PTV.Framework;
using PTV.Framework.Attributes;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Filters
{
    /// <summary>
    /// Manager handling filtering on entities based on implemented interfaces and available filters
    /// </summary>
    [RegisterService(typeof(IPrefilteringManager), RegisterType.Scope)]
    internal class PrefilteringManager : IPrefilteringManager
    {
        private static bool preInitialized = false;
        private IQueryFiltersList filterList = null;
        private readonly IResolveManager resolveManager;

        public PrefilteringManager(IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
            if (preInitialized)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            lock (this)
            {
                preInitialized = true;
                if (filterList == null)
                {
                    var filters = new Dictionary<Type, IQueryFilter>();
                    var queryFilters = resolveManager.ResolveAllOfType<IQueryFilter>() ?? new List<IQueryFilter>();
                    foreach (var filter in queryFilters)
                    {
                        var baseType = filter.GetType().GetInterfaces().FirstOrDefault();
                        var forEntity = baseType.GetGenericArguments().FirstOrDefault();
                        if (forEntity == null) continue;
                        filters.Add(forEntity, filter);
                    }
                    filterList = new QueryFiltersList(filters);
                }
            }
        }

        /// <summary>
        /// Apply all filters that match for requested entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<T> ApplyFilters<T>(IQueryable<T> query) where T : class
        {
            if (filterList == null) return query;
            var entityType = typeof(T);
            foreach (var oneFilter in filterList.QueryFilters)
            {
                if (oneFilter.Key.IsAssignableFrom(entityType))
                {
                    var filterType = oneFilter.Value.GetType();
                    var filterMethod = filterType.GetMethod("Filter");
                    var boundMethod = filterMethod.MakeGenericMethod(entityType);
                    query = boundMethod.Invoke(oneFilter.Value, new[] { query }) as IQueryable<T>;
                }
            }
            return query;
        }

        public IEnumerable<T> ApplyFilters<T>(IEnumerable<T> enumerable) where T : class
        {
            return ApplyFilters(enumerable.AsQueryable())?.AsEnumerable();
        }
    }

    class QueryFiltersList : IQueryFiltersList
    {
        public QueryFiltersList(Dictionary<Type, IQueryFilter> filters)
        {
            QueryFilters = filters;
        }

        public Dictionary<Type, IQueryFilter> QueryFilters { get; }
    }


    interface IQueryFiltersList
    {
       Dictionary<Type, IQueryFilter> QueryFilters { get; }
    }
}
