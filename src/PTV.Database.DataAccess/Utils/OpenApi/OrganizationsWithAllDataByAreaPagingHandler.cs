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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class OrganizationsWithAllDataByAreaPagingHandler : EntitiesWithAllDataPagingHandler<VmOpenApiOrganizationsWithPaging, IVmOpenApiOrganizationVersionBase, OrganizationVersioned>
    {
        private Guid areaId;
        private bool includeWholeCountry;
        private ITypesCache typesCache;

        public OrganizationsWithAllDataByAreaPagingHandler(
            Guid areaId,
            bool includeWholeCountry,
            ITypesCache typesCache,
            IPublishingStatusCache publishingStatusCache,
            int pageNumber
            ) : base(publishingStatusCache, pageNumber)
        {
            this.areaId = areaId;
            this.includeWholeCountry = includeWholeCountry;
            this.typesCache = typesCache;
        }

        protected override IList<Expression<Func<OrganizationVersioned, bool>>> GetFilters(IUnitOfWork unitOfWork)
        {
            return new List<Expression<Func<OrganizationVersioned, bool>>>
            {
                GetAreaFilterForOrganization(unitOfWork, typesCache, areaId, includeWholeCountry)
            };
        }
    }
}
