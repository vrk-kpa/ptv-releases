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
using PTV.Framework;

namespace PTV.Database.DataAccess.Interfaces.Cloning
{
    [RegisterService(typeof(ITranslationCloneCache), RegisterType.Transient)]
    public class TranslationCloneCache : ITranslationCloneCache
    {
        private readonly Dictionary<Type, List<IClonedTraceIdentification>> cache = new Dictionary<Type, List<IClonedTraceIdentification>>();
        private readonly Dictionary<Type, EntityPropertiesDefinition> entityNavigationsMap;

        public TranslationCloneCache(IEntityNavigationsMap entityNavigationsMap)
        {
            this.entityNavigationsMap = entityNavigationsMap?.NavigationsMap;
        }

        public void AddToCachedSet<T>(T entityOriginal, T entityCloned) where T : class
        {
            var entityStorage = cache.TryGet(typeof(T));
            if (entityStorage == null)
            {
                entityStorage = new List<IClonedTraceIdentification>();
                cache[typeof(T)] = entityStorage;
            }
            entityStorage.Add(new ClonedTraceIdentification<T> { OriginalEntity = entityOriginal, ClonedEntity = entityCloned});
        }

        public List<IClonedTraceIdentification<T>> GetFromCachedSet<T>() where T : class
        {
            var data = (cache.TryGet(typeof(T)) ?? new List<IClonedTraceIdentification>()).Cast<IClonedTraceIdentification<T>>();
            return data.ToList();
        }

        private void GetChildrenSubCascade(Dictionary<Type, List<IClonedTraceIdentification>> perTypeCacheData, IList<object> rootEntities, ref List<object> result)
        {
            var rowType = rootEntities.FirstOrDefault()?.GetType();
            if (rowType == null) return;
            foreach (var nm in entityNavigationsMap[rowType].NavigationsMap)
            {
                var relevantEntities = perTypeCacheData.TryGet(nm.Navigation.Type)?.Select(i => i.ClonedEntity).ToList() ?? new List<object>();
                relevantEntities = rootEntities.Select(i => i.GetPropertyObjectValue(nm.Navigation.Name)).Intersect(relevantEntities).Except(result).ToList();
                if (relevantEntities.Any())
                {
                    result.AddRange(relevantEntities);
                    GetChildrenSubCascade(perTypeCacheData, relevantEntities, ref result);
                }
            }
            foreach (var entityType in entityNavigationsMap.Values.Where(i => i.NavigationsMap.Any(u => u.Navigation.Type == rowType)))
            {
                var propertyName = entityType.NavigationsMap.First(j => j.Navigation.Type == rowType).Navigation.Name;
                var relevantEntities = perTypeCacheData.TryGet(entityType.EntityType)?.Select(j => j.ClonedEntity).ToList() ?? new List<object>();
                var newSubEntities = relevantEntities.Select(i => new {Related = i.GetPropertyObjectValue(propertyName), Main = i}).Where(i => rootEntities.Any(j => j == i.Related)).Select(j => j.Main).Except(result).ToList();
                if (newSubEntities.Any())
                {
                    result.AddRange(newSubEntities);
                    GetChildrenSubCascade(perTypeCacheData, newSubEntities, ref result);
                }
            }
        }

        public IList<object> GetEntitiesCascade(IList<object> rootEntities)
        {
            if (entityNavigationsMap == null) return new List<object>();
            var allCachedData = cache.Values.SelectMany(j => j).Where(j => !j.ProcessedByTranslator).ToList();
            var perTypeCacheData = allCachedData.Select(i => new { EType = i.OriginalEntity.GetType(), Obj = i }).GroupBy(i => i.EType).ToDictionary(i => i.Key, i => i.Select(j => j.Obj).ToList());
            var result = new List<object>();
            rootEntities = rootEntities.Where(r => allCachedData.Any(i => i.ClonedEntity == r && !i.MustBeKept && !i.ProcessedByTranslator)).ToList();
            GetChildrenSubCascade(perTypeCacheData, rootEntities, ref result);
            return result;
        }

        public void MarkAsProcessedByTranslator<T>(T clonedEntity) where T : class
        {
            var cachedInfo = this.GetFromCachedSet<T>().FirstOrDefault(i => i.ClonedEntity == clonedEntity);
            cachedInfo.SafeCall(i => i.ProcessedByTranslator = true);
        }

        public List<T> MarkAllAsDeletable<T>(List<T> entities, bool ifNotProcessedOnly = false) where T : class
        {
            var cachedInfos = this.GetFromCachedSet<T>().Where(i => entities.Contains(i.ClonedEntity)).ToList();
            cachedInfos.Where(i => (!ifNotProcessedOnly) || (!i.ProcessedByTranslator)).ForEach(i => i.SafeCall(e => e.MustBeKept = false));
            return cachedInfos.Where(i => !ifNotProcessedOnly || !i.ProcessedByTranslator).Select(i => i.ClonedEntity).ToList();
        }

        public void MarkAllAsMustBeKept<T>(List<T> entities) where T : class
        {
            var cachedInfos = this.GetFromCachedSet<T>().Where(i => entities.Contains(i.ClonedEntity));
            cachedInfos.ForEach(i => i.SafeCall(e => e.MustBeKept = true));
        }

        public void MarkAllAsMustBeKept<T>(Func<T, bool> callCondition) where T : class
        {
            if (entityNavigationsMap == null) return;
            cache.Values.SelectMany(j => j)
                .Select(i => new {Tracing = i, Entity = i.ClonedEntity as T})
                .Where(i => i.Entity != null && callCondition(i.Entity))
                .ForEach(i => i.Tracing.MustBeKept = true);
        }


        public IList<T> GetTouchedEntities<T>(IList<T> rootEntities, IList<object> exceptions) where T : class
        {
            if (entityNavigationsMap == null) return new List<T>();
            var allCachedData = cache.Values.SelectMany(j => j).Where(i => i.ClonedEntity != null).ToList();
            var perTypeCacheData = allCachedData.Select(i => new { EType = i.OriginalEntity.GetType(), Obj = i }).GroupBy(i => i.EType).ToDictionary(i => i.Key, i => i.Select(j =>(j.Obj)).ToList());
            var rowType = rootEntities.FirstOrDefault()?.GetType();
            if (rowType == null) return new List<T>();
            var rootCached = perTypeCacheData.TryGet(rowType);
            if (rootCached.IsNullOrEmpty()) return new List<T>();
            var exception = exceptions.Select(i => new ClonedTraceIdentificationWrap {ClonedEntity = i}).ToList<IClonedTraceIdentification>();
            return GetTouchedChildrenSubCascade(perTypeCacheData, rootCached.Where(i => rootEntities.Contains(i.ClonedEntity)).ToList(), ref exception).Select(i => i.ClonedEntity as T).Where(i => i != null).ToList();
        }

        private List<IClonedTraceIdentification> GetTouchedChildrenSubCascade(Dictionary<Type, List<IClonedTraceIdentification>> perTypeCacheData, IList<IClonedTraceIdentification> rootEntities, ref List<IClonedTraceIdentification> exceptions)
        {
            var rowType = rootEntities.FirstOrDefault()?.ClonedEntity?.GetType();
            if (rowType == null) return new List<IClonedTraceIdentification>();
            exceptions.AddRange(rootEntities);
            var localException = exceptions;
            var result = new List<IClonedTraceIdentification>();
            foreach (var nm in entityNavigationsMap[rowType].NavigationsMap)
            {
                var dataType = nm.Navigation.Type;
                var dataCached = perTypeCacheData.TryGet(dataType);
                if (dataCached.IsNullOrEmpty()) continue;
                var relatedEntities = rootEntities.Select(i => new { Parent = i, Sub = i.ClonedEntity.GetPropertyObjectValue(nm.Navigation.Name)}).Where(i => i.Sub != null).ToList();
                var relatedInfos = dataCached.Where(i => relatedEntities.Select(k => k.Sub).Contains(i.ClonedEntity) && !localException.Select(j => j.ClonedEntity).Contains(i.ClonedEntity)).ToList();
                var mustBeChecked = relatedInfos.Where(i => !i.ProcessedByTranslator && !i.MustBeKept).ToList();
                var subResults = mustBeChecked.Any() ? GetTouchedChildrenSubCascade(perTypeCacheData, mustBeChecked, ref exceptions) : new List<IClonedTraceIdentification>();

               var toAdd =
                relatedEntities.Where(i => i.Parent.MustBeKept || i.Parent.ProcessedByTranslator || relatedInfos.Where(j => j.ProcessedByTranslator || j.MustBeKept).Select(j => j.ClonedEntity).Contains(i.Sub) || subResults.Where(j => j.ProcessedByTranslator || j.MustBeKept).Select(j => j.ClonedEntity).Contains(i.Sub))
                    .Select(i => i.Parent).Except(result)
                    .ToList();
                result.AddRange(toAdd);
            }
            foreach (var entityType in entityNavigationsMap.Values.Where(i => i.NavigationsMap.Any(u => u.Navigation.Type == rowType)))
            {
                var propertyName = entityType.NavigationsMap.First(j => j.Navigation.Type == rowType).Navigation.Name;
                var dataCached = perTypeCacheData.TryGet(entityType.EntityType);
                if (dataCached.IsNullOrEmpty()) continue;
                var relatedInfos = dataCached.Select(i => new { Related = i.ClonedEntity.GetPropertyObjectValue(propertyName), Main = i }).Where(i => rootEntities.Any(j => j == i.Related)).Where(i => !localException.Select(j => j.ClonedEntity).Contains(i.Main) && !localException.Select(j => j.ClonedEntity).Contains(i.Related)).ToList();
                var mustBeChecked = relatedInfos.Where(i => !i.Main.ProcessedByTranslator && !i.Main.MustBeKept).Select(i => i.Main).ToList();
                var subResults = mustBeChecked.Any() ? GetTouchedChildrenSubCascade(perTypeCacheData, mustBeChecked, ref exceptions) : new List<IClonedTraceIdentification>();

                var toAdd = rootEntities.Where(i => relatedInfos.Where(j => j.Main.ProcessedByTranslator || j.Main.MustBeKept).Select(j => j.Related).Contains(i.ClonedEntity)
                ||  subResults.Where(j => j.ProcessedByTranslator || j.MustBeKept).Select(j => j.ClonedEntity).Contains(i.ClonedEntity)).Except(result).ToList();
                result.AddRange(toAdd);
            }
            return result.Distinct().ToList();
        }

        private class ClonedTraceIdentificationWrap : IClonedTraceIdentification
        {
            public object OriginalEntity { get; set; }
            public object ClonedEntity { get; set; }
            public bool ProcessedByTranslator { get; set; }
            public bool MustBeKept { get; set; }
        }
    }
}
