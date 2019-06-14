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
using System.Linq.Expressions;
using PTV.Domain.Model;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Security;

namespace PTV.Database.DataAccess.Interfaces
{
    public interface IRestrictionFilterManager
    {
        /// <summary>
        /// Returns all available filters for one organization identified bz GUID
        /// </summary>
        /// <param name="organizationId">GUID of organization</param>
        /// <returns></returns>
        List<VmRestrictionFilter> GetFiltersForOrganization(Guid organizationId);
        
        /// <summary>
        /// Get list of restricted data by type column and value there for some specific entity
        /// </summary>
        /// <param name="organizationId">GUID of organization</param>
        /// <typeparam name="T">Filter for entity type</typeparam>
        /// <returns></returns>
        List<RestrictedEntityInfo> GetRestrictedData<T>(Guid organizationId);
        
        /// <summary>
        /// Get list of restricted data by type column and value there for some specific entity. Type column is specified by property selector.
        /// Returns list of restricted types selected by guid of organization, entity type and type column.
        /// </summary>
        /// <param name="organizationId">GUID of organization</param>
        /// <param name="property">Type column selector</param>
        /// <typeparam name="T">Filter for entity type</typeparam>
        /// <typeparam name="TProp">Type of selected column</typeparam>
        /// <returns></returns>
        List<RestrictedEntityInfo> GetRestrictedData<T,TProp>(Guid organizationId, Expression<Func<T, TProp>> property);

        IEnumerable<T> FilterOutEntities<T>(IEnumerable<T> collection, Guid organizationId);
        IEnumerable<IVmRestrictableType> SetAccessForGuidTypes(Guid organizationId, string typeName, IEnumerable<IVmRestrictableType> allValues);
        IEnumerable<IVmRestrictableType> SetAccessForGuidTypes<TType>(Guid organizationId, IEnumerable<IVmRestrictableType> allValues);
        bool IsTypeGuidAllowed<TType>(Guid organizationId, Guid typeGuid);
    }
}