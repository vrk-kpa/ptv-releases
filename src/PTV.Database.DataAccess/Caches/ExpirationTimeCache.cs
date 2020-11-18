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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Database.Model.Views;
using PTV.Framework;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(IExpirationTimeCache), RegisterType.Singleton)]
    internal class ExpirationTimeCache : IExpirationTimeCache
    {
        private readonly IContextManager contextManager;
        private DateTime lastUpdateTime;
        private Dictionary<Type, List<ExpirationTimeItem>> cache;
        private List<Type> AllowedTypes => new List<Type>
        {
            typeof(Service),
            typeof(ServiceChannel),
            typeof(Organization)
        };
        private Dictionary<Type, Type> VersionedTypesMap => new Dictionary<Type, Type>()
        {
            { typeof(ServiceVersioned), typeof(Service) },
            { typeof(ServiceChannelVersioned), typeof(ServiceChannel) },
            { typeof(OrganizationVersioned), typeof(Organization) }
        };

        public ExpirationTimeCache(IContextManager contextManager)
        {
            RefreshableCaches.Add(this);
            this.contextManager = contextManager;
            CheckAndRefresh();
        }

        public void CheckAndRefresh()
        {
            // It is sufficient to refresh the cache once per day
            var utcNow = DateTime.UtcNow;
            if (lastUpdateTime.Date == utcNow.Date)
            {
                return;
            }

            var newCache = new Dictionary<Type, List<ExpirationTimeItem>>();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var taskConfigRepo = unitOfWork.CreateRepository<IVTasksConfigurationRepository>();
                var entityGroups = taskConfigRepo.All().ToList().GroupBy(x => x.Entity);

                foreach (var entityGroup in entityGroups)
                {
                    var type = AllowedTypes.First(x => x.Name == entityGroup.Key);
                    var publishingStatusGroups = entityGroup.GroupBy(x => x.PublishingStatusId);

                    foreach (var publishingStatusGroup in publishingStatusGroups)
                    {
                        var orderedGroup = publishingStatusGroup.OrderBy(x => x.Interval).ToList();
                        var item = new ExpirationTimeItem
                        {
                            PublishingStatusId = publishingStatusGroup.Key,
                            ExpirationMonths = orderedGroup.First()?.Months,
                            LastWarningMonths = orderedGroup.Last()?.Months,
                        };

                        if (!newCache.ContainsKey(type))
                        {
                            newCache.Add(type, new List<ExpirationTimeItem>());
                        }

                        newCache[type].Add(item);
                    }
                }
            });

            cache = newCache;
            lastUpdateTime = utcNow;
        }

        private Type NormalizeType(Type entityType)
        {
            return AllowedTypes.Contains(entityType)
                ? entityType
                : VersionedTypesMap.GetValueOrDefault(entityType);
        }

        private ExpirationTimeItem GetCacheItem(Type entityType, Guid publishingStatusId)
        {
            var normalizedType = NormalizeType(entityType);
            var configs = cache.GetValueOrDefault(normalizedType);
            return configs?.FirstOrDefault(x => x.PublishingStatusId == publishingStatusId);
        }
        
        public decimal GetExpirationMonths(Type entityType, Guid publishingStatusId)
        {
            return GetCacheItem(entityType, publishingStatusId)?.ExpirationMonths ?? 0;
        }
        
        public decimal GetLastWarningMonths(Type entityType, Guid publishingStatusId)
        {
            return GetCacheItem(entityType, publishingStatusId)?.LastWarningMonths ?? 0;
        }

        private class ExpirationTimeItem
        {
            public decimal? ExpirationMonths { get; set; }
            public decimal? LastWarningMonths { get; set; }
            public Guid PublishingStatusId { get; set; }
        }
    }
}
