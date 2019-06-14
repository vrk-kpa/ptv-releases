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
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class ServicesByAreaPageHandler : V3GuidPageHandler<ServiceVersioned, Service>
    {
        private Guid areaId;
        private bool includeWholeCountry;
        private ITypesCache typesCache;

        public ServicesByAreaPageHandler(
            Guid areaId,
            bool includeWholeCountry,
            ITypesCache typesCache,
            DateTime? date,
            DateTime? dateBefore,
            IPublishingStatusCache publishingStatusCache,
            int pageNumber,
            int pageSize
            ) : base(EntityStatusExtendedEnum.Published, date, dateBefore, publishingStatusCache, pageNumber, pageSize)
        {
            this.areaId = areaId;
            this.includeWholeCountry = includeWholeCountry;
            this.typesCache = typesCache;
        }

        protected override IList<Expression<Func<ServiceVersioned, bool>>> GetFilters(IUnitOfWork unitOfWork)
        {
            var filters = base.GetFilters(unitOfWork);

            if (includeWholeCountry)
            {
                var wholeCountryId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
                // Is the area in Åland?
                // Lets first get the municipalities for the defined area and then all the areas related to the municipalities.
                var repo = unitOfWork.CreateRepository<IAreaMunicipalityRepository>();
                var municipalities = repo.All()
                    .Where(a => a.AreaId == areaId).Select(a => a.MunicipalityId).ToList();
                var allAreas = repo.All().Where(a => municipalities.Contains(a.MunicipalityId)).Select(a => a.AreaId).ToList();
                if (IsAreaInAland(unitOfWork, allAreas, typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString()))) // Åland
                {
                    filters.Add(o => o.AreaInformationTypeId == wholeCountryId || o.Areas.Any(a => a.AreaId == areaId));
                }
                else
                {
                    var wholeCountryExceptAlandId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
                    filters.Add(o => o.AreaInformationTypeId == wholeCountryId || o.AreaInformationTypeId == wholeCountryExceptAlandId
                    || o.Areas.Any(a => a.AreaId == areaId));
                }
            }
            else
            {
                filters.Add(o => o.Areas.Any(a => a.AreaId == areaId));
            }
            return filters;
        }
    }
}
