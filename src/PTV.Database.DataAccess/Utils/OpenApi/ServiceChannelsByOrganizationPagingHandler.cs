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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class ServiceChannelsByOrganizationPagingHandler : EntitiesWithConnectionsPagingHandler<ServiceChannelVersioned, ServiceChannel>
    {
        private IList<Guid> organizationIds;
        private Guid? typeId;

        public ServiceChannelsByOrganizationPagingHandler(
            IList<Guid> organizationIds,
            EntityStatusExtendedEnum status,
            Guid? typeId,
            DateTime? date,
            DateTime? dateBefore,
            IPublishingStatusCache publishingStatusCache,
            int pageNumber,
            int pageSize
            ) : base(status, date, dateBefore, publishingStatusCache, pageNumber, pageSize)
        {
            this.organizationIds = organizationIds;
            this.typeId = typeId;
        }

        protected override IList<Expression<Func<ServiceChannelVersioned, bool>>> GetFilters(IUnitOfWork unitOfWork)
        {
            var filters = base.GetFilters(unitOfWork);

            filters.Add(serviceChannel => organizationIds.Contains(serviceChannel.OrganizationId));
            if (typeId.IsAssigned())
            {
                filters.Add(serviceChannel => serviceChannel.TypeId == typeId);
            }

            return filters;
        }
    }
}
