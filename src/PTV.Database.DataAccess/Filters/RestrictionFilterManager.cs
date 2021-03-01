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
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Filters
{
    [RegisterService(typeof(IRestrictionFilterManager), RegisterType.Scope)]
    public class RestrictionFilterManager : IRestrictionFilterManager
    {
        private readonly IResolveManager resolveManager;
        private readonly IRestrictionFilterCache restrictionFilterCache;
        private ITranslationEntity translationManager;

        public RestrictionFilterManager(IResolveManager resolveManager, IRestrictionFilterCache restrictionFilterCache, ITranslationEntity translationManager)
        {
            this.resolveManager = resolveManager;
            this.translationManager = translationManager;
            this.restrictionFilterCache = restrictionFilterCache;

            if (!this.restrictionFilterCache.Initialized)
            {
                BuildCache();
            }
        }

        private void BuildCache()
        {
            var contextManager = resolveManager.Resolve<IContextManager>();
            var data = contextManager.ExecuteIsolatedReader(unitOfWork =>
            {
                var filterRep = unitOfWork.CreateRepository<IRestrictionFilterRepository>();
                return translationManager.TranslateAll<RestrictionFilter, VmRestrictionFilter>(filterRep.All().Include(i => i.OrganizationFilters).Include(i => i.RestrictedType));
            });
            restrictionFilterCache.Init(data);
        }

        public bool IsTypeGuidAllowed<TType>(Guid organizationId, Guid typeGuid)
        {
            var guidToCheck = new List<IVmRestrictableType> {new VmEnumType {Id = typeGuid, Access = EVmRestrictionFilterType.Allowed}};
            SetAccessForGuidTypes<TType>(organizationId, guidToCheck);
            return guidToCheck.FirstOrDefault(i => i.Id == typeGuid)?.Access == EVmRestrictionFilterType.Allowed;
        }

        public IEnumerable<IVmRestrictableType> SetAccessForGuidTypes<TType>(Guid organizationId, IEnumerable<IVmRestrictableType> allValues)
        {
            return SetAccessForGuidTypes(organizationId, typeof(TType).Name, allValues);
        }

        public IEnumerable<IVmRestrictableType> SetAccessForGuidTypes(Guid organizationId, string typeName, IEnumerable<IVmRestrictableType> allValues)
        {
            var orgFilters = GetFiltersByOrganization(organizationId);
            var typeFilters = restrictionFilterCache.GetByType(typeName).Concat(restrictionFilterCache.GetByType(typeName + "Id"));
            allValues.ForEach(i => i.Access = EVmRestrictionFilterType.Allowed);
            var readOnlyFilters = ByType(orgFilters, EVmRestrictionFilterType.ReadOnly, false);
            if (readOnlyFilters.Any())
            {
                var readOnlyValues = typeFilters.Where(i => readOnlyFilters.Any(j => j.RestrictedType.TypeName == i.TypeName)).Select(i => i.Value).Distinct().ToList();
                allValues.Where(i => readOnlyValues.Contains(i.Id)).ForEach(i => i.Access = EVmRestrictionFilterType.ReadOnly);
            }
            readOnlyFilters = ByType(orgFilters, EVmRestrictionFilterType.ReadOnly, true);
            if (readOnlyFilters.Any())
            {
                var exceptThisReadOnly = typeFilters.Where(i => readOnlyFilters.Any(j => j.RestrictedType.TypeName == i.TypeName)).Select(i => i.Value).Distinct().ToList();
                allValues.Where(i => !exceptThisReadOnly.Contains(i.Id) && i.Access == EVmRestrictionFilterType.Allowed).ForEach(i => i.Access = EVmRestrictionFilterType.ReadOnly);
            }

            var orgFiltersOnlyForStricted = ByType(orgFilters, EVmRestrictionFilterType.Allowed, true);
            if (orgFiltersOnlyForStricted.Any())
            {
                var allowedValues = typeFilters.Where(i => orgFiltersOnlyForStricted.Any(j => j.RestrictedType == i)).Select(i => i.Value).Distinct().ToList();
                allValues.Where(i => !allowedValues.Contains(i.Id) && i.Access == EVmRestrictionFilterType.Allowed).ForEach(i => i.Access = EVmRestrictionFilterType.Denied);
            }
            var orgFiltersOnlyFor = ByType(orgFilters, EVmRestrictionFilterType.Allowed, false);
            if (orgFiltersOnlyFor.Any())
            {
                var blockedValues = typeFilters.Where(i => orgFiltersOnlyFor.All(j => j.RestrictedType.Id != i.Id && j.RestrictedType.TypeName == i.TypeName)).Select(i => i.Value).Distinct().ToList();
                allValues.Where(i => blockedValues.Contains(i.Id) && i.Access == EVmRestrictionFilterType.Allowed).ForEach(i => i.Access = EVmRestrictionFilterType.Denied);
            }
            var orgFiltersNotFor = ByType(orgFilters, EVmRestrictionFilterType.Denied);
            var blocked = typeFilters.Where(i => orgFiltersNotFor.Any(j => j.RestrictedType.TypeName == i.TypeName) || orgFilters.All(j => j.RestrictedType.TypeName != i.TypeName)).Select(i => i.Value).Distinct().ToList();
            allValues.Where(i => blocked.Contains(i.Id)).ForEach(i => i.Access = EVmRestrictionFilterType.Denied);
            return allValues;
        }

        private IEnumerable<VmRestrictionFilter> ByType(IEnumerable<VmRestrictionFilter> filters, EVmRestrictionFilterType filterType, bool blockOthers = false)
        {
            return filters.Where(i => i.FilterType == filterType && i.BlockOtherTypes == blockOthers);
        }


        private List<VmRestrictionFilter> FiltersMatching<T>(List<VmRestrictionFilter> filters, T entity)
        {
            return filters.Where(f =>
            {
                var propValue = entity.GetPropertyValue<T,Guid>(f.ColumnName);
                return (propValue == f.RestrictedType.Value);
            }).ToList();
        }


        private List<VmRestrictionFilter> GetFiltersByOrganization(Guid organizationId)
        {
            var contextManager = resolveManager.Resolve<IContextManager>();
            return contextManager.ExecuteIsolatedReader(unitOfWork =>
            {
                var filterRep = unitOfWork.CreateRepository<IOrganizationFilterRepository>();
                var filters = filterRep.All().Where(f => f.OrganizationId == organizationId).Select(f => f.FilterId);
                return restrictionFilterCache.GetAllFilters().Where(f => filters.Contains(f.Id)).ToList();
            });
        }

    }
}
