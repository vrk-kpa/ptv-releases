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

using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class NewGdsPagingHandler : EntitiesByStatusPagingHandler<V3VmOpenApiGuidPage, VmOpenApiItem, StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, StatutoryServiceName, GeneralDescriptionLanguageAvailability>
    {
        public NewGdsPagingHandler(
            IPublishingStatusCache publishingStatusCache,
            ITypesCache typesCache,
            int pageNumber,
            int pageSize
            ) : base(EntityStatusExtendedEnum.Published, null, null, publishingStatusCache, typesCache, pageNumber, pageSize) // Get only published items. SFIPTV-206
        { }

        protected override IList<Expression<Func<StatutoryServiceGeneralDescriptionVersioned, bool>>> GetFilters(IUnitOfWork unitOfWork)
        {
            var filters = base.GetFilters(unitOfWork);

            var gdList = unitOfWork.CreateRepository<IRepository<TrackingGeneralDescriptionVersioned>>().All()
                .Where(x => x.OperationType == EntityState.Added.ToString())
                .Where(x => x.Created >= DateTime.UtcNow.AddMonths(-1))
                .Select(x => x.GenerealDescriptionId).ToList();

            filters.Add(gd => gdList.Contains(gd.UnificRootId)); // GeneralDescriptionId within TrackingGeneralDescriptionVersioned table is the unific root id!
            return filters;
        }
    }
}
