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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Model;
using PTV.Framework;

namespace PTV.Database.DataAccess.Filters
{
//    [QueryFilter]
//    internal class GeneralDescriptionByOrganizationFilter : IQueryFilter<StatutoryServiceGeneralDescriptionVersioned>
//    {
//        private readonly List<Guid> restrictedGuids;
//        private readonly IRestrictionFilterCache restrictionFilterCache;
//
//        public List<RestrictedEntityInfo> GetRestrictedData<T,TProp>(Guid organizationId, Expression<Func<T, TProp>> property)
//        {
//            var entityName = typeof(T).Name;
//            var propertyName = (property != null) ? CoreExtensions.GetPropertyName(property) : null;
//            var orgFilters = restrictionFilterCache.Get(organizationId).Where(i => i.ForEntityName == entityName);
//            if (propertyName != null)
//            {
//                orgFilters = orgFilters.Where(i => i.EntityTypeName == propertyName).ToList();
//            }
//
//            return orgFilters.Cast<RestrictedEntityInfo>().ToList();
//        }
//        
//        
//        public GeneralDescriptionByOrganizationFilter(IPahaTokenProcessor tokenProcessor, IRestrictionFilterCache restrictionFilterCache)
//        {
//            this.restrictionFilterCache = restrictionFilterCache;
//            restrictedGuids = GetRestrictedData<StatutoryServiceGeneralDescriptionVersioned, Guid>(tokenProcessor.ActiveOrganization,
//                    i => i.GeneralDescriptionTypeId).Select(i => i.EntityTypeValue.ParseToGuid())
//                .Where(i => i.IsAssigned()).Cast<Guid>().ToList();
//        }
//
//        public IQueryable<T> Filter<T>(IQueryable<T> query) where T : StatutoryServiceGeneralDescriptionVersioned
//        {
//            return query.Where(i => !restrictedGuids.Contains(i.GeneralDescriptionTypeId));
//        }
//
//    }
}