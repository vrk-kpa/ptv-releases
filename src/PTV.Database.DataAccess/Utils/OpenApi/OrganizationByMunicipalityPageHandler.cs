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
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class OrganizationByMunicipalityPageHandler : GuidPageHandler<V8VmOpenApiOrganizationGuidPage, V8VmOpenApiOrganizationItem, OrganizationVersioned, Organization>
    {
        private Guid municipalityId;
        private bool includeWholeCountry;
        private ITypesCache typesCache;

        public OrganizationByMunicipalityPageHandler(
            Guid municipalityId,
            bool includeWholeCountry,
            ITypesCache typesCache,
            DateTime? date,
            DateTime? dateBefore,
            IPublishingStatusCache publishingStatusCache,
            int pageNumber,
            int pageSize
            ) : base(EntityStatusExtendedEnum.Published, date, dateBefore, publishingStatusCache, pageNumber, pageSize)
        {
            this.municipalityId = municipalityId;
            this.includeWholeCountry = includeWholeCountry;
            this.typesCache = typesCache;
        }

        protected override IList<Expression<Func<OrganizationVersioned, bool>>> GetFilters(IUnitOfWork unitOfWork)
        {
            var filters = base.GetFilters(unitOfWork);

            // Areas related to defined municipality
            var areas = unitOfWork.CreateRepository<IAreaMunicipalityRepository>().All()
                .Where(a => a.MunicipalityId == municipalityId).Select(a => a.AreaId).ToList();

            var municipalityTypeId = typesCache.Get<OrganizationType>(OrganizationTypeEnum.Municipality.ToString());

            if (includeWholeCountry) // SFIPTV-784
            {
                var wholeCountryId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());

                // Is the municipality in 'Åland'? So do we need to include also AreaInformationType WholeCountryExceptAlandIslands?
                if (IsAreaInAland(unitOfWork, areas, typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString()))) // Åland
                {
                    filters.Add(o => (o.TypeId == municipalityTypeId && o.MunicipalityId == municipalityId) || // For municipality organizations check attached municipality (PTV-3423)
                        (o.TypeId != municipalityTypeId && // For all other organization types let's check areas (PTV-3423)
                        (o.AreaInformationTypeId == wholeCountryId || (o.OrganizationAreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || o.OrganizationAreas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId))))));
                }
                else
                {
                    var wholeCountryExceptAlandId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
                    filters.Add(o => (o.TypeId == municipalityTypeId && o.MunicipalityId == municipalityId) || // For municipality organizations check attached municipality (PTV-3423)
                        (o.TypeId != municipalityTypeId &&  // For all other organization types let's check areas (PTV-3423)
                        (o.AreaInformationTypeId == wholeCountryId || o.AreaInformationTypeId == wholeCountryExceptAlandId ||
                            (o.OrganizationAreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || o.OrganizationAreas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId))))));
                }
            }
            else
            {
                // Only return organizations that have the defined municipality attached
                filters.Add(o => (o.TypeId == municipalityTypeId && o.MunicipalityId == municipalityId) || // For municipality organizations check attached municipality (PTV-3423)
                        (o.TypeId != municipalityTypeId && // For all other organization types let's check areas (PTV-3423)
                        o.OrganizationAreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || o.OrganizationAreas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId))));
            }
            return filters;
        }

        protected override V8VmOpenApiOrganizationItem GetItemData(OrganizationVersioned entity)
        {
            return new V8VmOpenApiOrganizationItem
            {
                Id = entity.UnificRootId,
                Name = Names.TryGetOrDefault(entity.Id, new Dictionary<Guid, string>())?.FirstOrDefault().Value,
                ParentOrganizationId = entity.ParentId
            };
        }
    }
}
