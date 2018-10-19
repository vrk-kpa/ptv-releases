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
using PTV.Domain.Model;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(IRestrictionFilterCache), RegisterType.Singleton)]
    internal class RestrictionFilterCache : IRestrictionFilterCache
    {
        private Dictionary<Guid, List<VmRestrictionFilter>> orgData = new Dictionary<Guid, List<VmRestrictionFilter>>();
        private Dictionary<string, List<VmRestrictionFilter>> entityData = new Dictionary<string, List<VmRestrictionFilter>>();
        private Dictionary<string, List<VmRestrictedType>> typeData = new Dictionary<string, List<VmRestrictedType>>();
        
        public Dictionary<Guid, List<VmRestrictionFilter>> GetAll()
        {
            return orgData;
        }
        
        public List<VmRestrictionFilter> GetByOrg(Guid orgId)
        {
            return orgData.TryGetOrDefault(orgId, new List<VmRestrictionFilter>());
        }
        
        public List<VmRestrictionFilter> GetByEntity(string entityTypeName)
        {
            return entityData.TryGetOrDefault(entityTypeName, new List<VmRestrictionFilter>());
        }

        public List<VmRestrictedType> GetByType(string typeName)
        {
            return typeData.TryGetOrDefault(typeName, new List<VmRestrictedType>());
        }

        public void Init(IReadOnlyList<VmRestrictionFilter> dataSet)
        {
            dataSet.ForEach(i => i.OrganizationFilters.ForEach(j => j.Filter = i));
            dataSet.ForEach(i =>
            {
                if (i.RestrictedType.Filters == null)
                {
                    i.RestrictedType.Filters = new List<VmRestrictionFilter>();
                }
                if (!i.RestrictedType.Filters.Contains(i))
                {
                    i.RestrictedType.Filters.Add(i);
                }
            });
            this.orgData = dataSet.SelectMany(i => i.OrganizationFilters).GroupBy(i => i.OrganizationId).ToDictionary(i => i.Key, i => i.Select(j => j.Filter).ToList());
            this.entityData = dataSet.GroupBy(i => i.EntityType).ToDictionary(i => i.Key, i => i.ToList());
            this.typeData = dataSet.Select(i => i.RestrictedType).GroupBy(i => i.TypeName).ToDictionary(i => i.Key, i => i.ToList());
        }

        public bool Initialized => this.orgData.Any();
        
        public List<Guid> GetBlockedGuids<T,TProp>(Guid organizationId, Expression<Func<T, TProp>> property)
        {
            var entityName = typeof(T).Name;
            var entityEntries = GetByEntity(entityName);
            if (!entityEntries.Any())
            {
                return new List<Guid>();
            }
            var propertyName = CoreExtensions.GetPropertyName(property);
            var orgFilters = GetByOrg(organizationId).Where(i => i.EntityType == entityName).ToList();
            return entityEntries.Where(i => orgFilters.All(j => j.Id != i.Id && j.FilterType == EVmRestrictionFilterType.Allowed) || orgFilters.Any(j => j.Id == i.Id && j.FilterType == EVmRestrictionFilterType.Denied)).Where(i => i.ColumnName == propertyName).Select(i => i.RestrictedType.Value).ToList();
        }    
    }
}