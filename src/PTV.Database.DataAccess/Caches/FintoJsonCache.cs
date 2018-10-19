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
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.ExternalSources;
using PTV.ExternalSources.Resources.Finto;

namespace PTV.Database.DataAccess.Caches
{
    internal interface IFintoJsonCache : IEntityCache<string, string>
    {
        string Get<T>(string key) where T : IFintoItemBase;
    }

    [RegisterService(typeof(IFintoJsonCache), RegisterType.Singleton)]
    internal class FintoJsonCache : IFintoJsonCache
    {
        private Dictionary<Type, FintoTypeJsonCache> cacheData;

        public FintoJsonCache(ResourceLoader resourceLoader)
        {
            cacheData = new Dictionary<Type, FintoTypeJsonCache>
            {
                { typeof(OntologyTerm),  new FintoTypeJsonCache(resourceLoader, FintoItemImportDefinition.Resources.OntologyAll)},
                { typeof(LifeEvent),  new FintoTypeJsonCache(resourceLoader, FintoItemImportDefinition.Resources.LifeSituations)},
                { typeof(TargetGroup),  new FintoTypeJsonCache(resourceLoader, FintoItemImportDefinition.Resources.TargetGroups)},
                { typeof(ServiceClass),  new FintoTypeJsonCache(resourceLoader, FintoItemImportDefinition.Resources.ServiceClasses)},
            };
        }

        public string Get<T>(string key) where T : IFintoItemBase
        {
            var innerCache = cacheData.TryGet(typeof(T)) ??
                throw new ArgumentOutOfRangeException($"Invalid type {typeof(T).Name}, should be used one of ({string.Join(", ", cacheData.Keys.Select(x => x.Name))}");
            return innerCache.Get(key);
        }

        public string Get(string key)
        {
            foreach (var replacedBy in cacheData.Values.Select(x => x.Get(key)))
            {
                if (!string.IsNullOrEmpty(replacedBy))
                {
                    return replacedBy;
                }
            }
            return string.Empty;
        }

        public string GetByValue(string value)
        {
            throw new System.NotImplementedException();
        }

        public bool Exists(string key)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class FintoTypeJsonCache : IEntityCache<string, string>
    {
        private Dictionary<string, VmServiceViewsJsonItem> cacheData;
        private readonly ResourceLoader resourceLoader;
        private FintoItemImportDefinition.Resources resourceType;

        public FintoTypeJsonCache(ResourceLoader resourceLoader, FintoItemImportDefinition.Resources resourceType)
        {
            this.resourceLoader = resourceLoader;
            this.resourceType = resourceType;
        }

        public Dictionary<string, VmServiceViewsJsonItem> CacheData => cacheData ?? Init();

        private Dictionary<string, VmServiceViewsJsonItem> Init()
        {
            var data = resourceLoader.GetDeserializedResource<List<VmServiceViewsJsonItem>>(
                new FintoItemImportDefinition { Resource = resourceType }
            );
            cacheData = data.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.FirstOrDefault());
            return cacheData;
        }

        public string Get(string key)
        {
            return CacheData.TryGet(key)?.ReplacedBy ?? string.Empty;
        }

        public string GetByValue(string value)
        {
            throw new NotSupportedException();
        }

        public bool Exists(string key)
        {
            return CacheData.ContainsKey(key);
        }
    }
}