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
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(IRestrictionFilterCache), RegisterType.Singleton)]
    internal class RestrictionFilterCache : IRestrictionFilterCache
    {
        private Dictionary<string, List<VmRestrictionFilter>> entityData = new Dictionary<string, List<VmRestrictionFilter>>();
        private Dictionary<string, List<VmRestrictedType>> typeData = new Dictionary<string, List<VmRestrictedType>>();
        private List<VmRestrictionFilter> filterData = new List<VmRestrictionFilter>();

        public List<VmRestrictionFilter> GetAllFilters()
        {
            return filterData;
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

            filterData = dataSet.ToList();
            entityData = dataSet.GroupBy(i => i.EntityType).ToDictionary(i => i.Key, i => i.ToList());
            typeData = dataSet.Select(i => i.RestrictedType).GroupBy(i => i.TypeName).ToDictionary(i => i.Key, i => i.ToList());
        }

        public bool Initialized => filterData.Any();
    }
}
