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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class ServicesByServiceClassPagingHandler : V3GuidPagingHandler<ServiceVersioned, Service, ServiceName, ServiceLanguageAvailability>
    {
        private List<Guid> serviceClassIds;

        public ServicesByServiceClassPagingHandler(
            List<Guid> serviceClassIds,
            DateTime? date,
            DateTime? dateBefore,
            IPublishingStatusCache publishingStatusCache,
            ITypesCache typesCache,
            int pageNumber,
            int pageSize
            ) : base(EntityStatusExtendedEnum.Published, date, dateBefore, publishingStatusCache, typesCache, pageNumber, pageSize)
        {
            this.serviceClassIds = serviceClassIds;
        }

        protected override IList<Expression<Func<ServiceVersioned, bool>>> GetFilters(IUnitOfWork unitOfWork)
        {
            var filters = base.GetFilters(unitOfWork);

            // Get statutory service descriptions that are related to defined service class
            var gdRep = unitOfWork.CreateRepository<IStatutoryServiceServiceClassRepository>();
            var test = gdRep.All();
            var gdQuery = gdRep.All().Where(s => serviceClassIds.Contains(s.ServiceClassId) &&
                s.StatutoryServiceGeneralDescriptionVersioned.PublishingStatusId == PublishedId &&
                s.StatutoryServiceGeneralDescriptionVersioned.LanguageAvailabilities.Any(l => l.StatusId == PublishedId));

            var gdList = unitOfWork.ApplyIncludes(gdQuery, q =>
                q.Include(i => i.StatutoryServiceGeneralDescriptionVersioned)
                ).ToList();

            var gdIdList = gdList.Select(g => g.StatutoryServiceGeneralDescriptionVersioned.UnificRootId).Distinct().ToList();

            // Get services related to given service class.
            var additionalFilters = new List<Expression<Func<ServiceVersioned, bool>>>();
            if (gdIdList?.Count > 0)
            {
                // Get services that are attached to one of the general desriptions or that are related to given service class.
                filters.Add(service =>
                    (service.StatutoryServiceGeneralDescriptionId != null && gdIdList.Contains(service.StatutoryServiceGeneralDescriptionId.Value)) ||
                        service.ServiceServiceClasses.Any(c => serviceClassIds.Contains(c.ServiceClassId)));
            }
            else
            {
                // Get services that are related to given service class.
                filters.Add(service => service.ServiceServiceClasses.Any(c => serviceClassIds.Contains(c.ServiceClassId)));
            }

            return filters;
        }
    }
}
