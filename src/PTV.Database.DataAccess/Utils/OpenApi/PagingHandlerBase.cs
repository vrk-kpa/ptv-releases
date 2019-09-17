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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal abstract class PagingHandlerBase<TModel, TItemModel>
        where TModel : IVmOpenApiModelWithPagingBase<TItemModel>, new()
    {
        protected int TotalCount { get; private set; }

        protected bool ValidPageNumber { get
            {
                if (ViewModel.PageCount <= 0) return false;
                if (ViewModel.PageNumber <= 0) return false;
                if (ViewModel.PageNumber > ViewModel.PageCount) return false;
                return true;
            } set { } }

        protected TModel ViewModel { get; private set; }

        public int PageNumber { get
            {
                if (ViewModel == null) return 0;
                return ViewModel.PageNumber;
            } }

        public PagingHandlerBase(
            int pageNumber,
            int pageSize)
        {
            ViewModel = new TModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public abstract int Search(IUnitOfWork unitOfWork);

        public abstract IVmOpenApiModelWithPagingBase<TItemModel> GetModel();

        /// <summary>
        /// Sets the page count.
        /// </summary>
        /// <param name="totalCount">The total count.</param>
        protected void SetPageCount(int totalCount)
        {
            TotalCount = totalCount;
            ViewModel.PageCount = totalCount / ViewModel.PageSize + (totalCount % ViewModel.PageSize > 0 ? 1 : 0);
        }

        /// <summary>
        /// Gets the size of the skip.
        /// </summary>
        /// <returns>
        /// size
        /// </returns>
        protected int GetSkipSize()
        {
            // If we are in the first page let's not skip any items, otherwise let's skip previous page items.
            return ViewModel.PageNumber <= 1 ? 0 : (ViewModel.PageNumber - 1) * ViewModel.PageSize;
        }

        /// <summary>
        /// Gets the size of the take.
        /// </summary>
        /// <returns>
        /// size
        /// </returns>
        protected int GetTakeSize()
        {
            // If we are on the last page, let's take the last remaining items, otherwise let's take size of PageSize.
            return ViewModel.PageNumber == ViewModel.PageCount ? (TotalCount - (ViewModel.PageSize * (ViewModel.PageNumber - 1))) : ViewModel.PageSize;
        }

        #region Entity related filters

        protected Expression<Func<OrganizationVersioned, bool>> GetMunicipalityFilterForOrganization(IUnitOfWork unitOfWork, ITypesCache typesCache, Guid municipalityId, bool includeWholeCountry)
        {
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
                    return o => (o.TypeId == municipalityTypeId && o.MunicipalityId == municipalityId) || // For municipality organizations check attached municipality (PTV-3423)
                        (o.TypeId != municipalityTypeId && // For all other organization types let's check areas (PTV-3423)
                        (o.AreaInformationTypeId == wholeCountryId || o.OrganizationAreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || o.OrganizationAreas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId))));
                }
                else
                {
                    var wholeCountryExceptAlandId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
                    return o => (o.TypeId == municipalityTypeId && o.MunicipalityId == municipalityId) || // For municipality organizations check attached municipality (PTV-3423)
                        (o.TypeId != municipalityTypeId &&  // For all other organization types let's check areas (PTV-3423)
                        (o.AreaInformationTypeId == wholeCountryId || o.AreaInformationTypeId == wholeCountryExceptAlandId ||
                            o.OrganizationAreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || o.OrganizationAreas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId))));
                }
            }
            else
            {
                // Only return organizations that have the defined municipality attached
                return o => (o.TypeId == municipalityTypeId && o.MunicipalityId == municipalityId) || // For municipality organizations check attached municipality (PTV-3423)
                        o.TypeId != municipalityTypeId && // For all other organization types let's check areas (PTV-3423)
                        o.OrganizationAreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || o.OrganizationAreas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId));
            }
        }

        protected Expression<Func<ServiceVersioned, bool>> GetMunicipalityFilterForService(IUnitOfWork unitOfWork, ITypesCache typesCache, Guid municipalityId, bool includeWholeCountry)
        {
            if (includeWholeCountry) // SFIPTV-806
            {
                // Areas related to defined municipality
                var areas = unitOfWork.CreateRepository<IAreaMunicipalityRepository>().All()
                    .Where(a => a.MunicipalityId == municipalityId).Select(a => a.AreaId).ToList();

                // Get services
                var wholeCountryId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
                // is the municipality in 'Åland'? So do we need to include also AreaInformationType WholeCountryExceptAlandIslands?
                if (IsAreaInAland(unitOfWork, areas, typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString()))) // Åland
                {
                    return s => s.AreaInformationTypeId == wholeCountryId || s.AreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) ||
                    s.Areas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId));
                }
                else
                {
                    var wholeCountryExceptAlandId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
                    return s => s.AreaInformationTypeId == wholeCountryId || s.AreaInformationTypeId == wholeCountryExceptAlandId ||
                                    s.AreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || s.Areas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId));
                }
            }
            else
            {
                // Only return services that have the defined municipality attached
                return s => s.AreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || s.Areas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId));
            }
        }

        protected Expression<Func<ServiceChannelVersioned, bool>> GetMunicipalityFilterForServiceChannel(IUnitOfWork unitOfWork, ITypesCache typesCache, Guid municipalityId, bool includeWholeCountry)
        {
            if (includeWholeCountry) // SFIPTV-806
            {
                // Areas related to defined municipality
                var areas = unitOfWork.CreateRepository<IAreaMunicipalityRepository>().All()
                .Where(a => a.MunicipalityId == municipalityId).Select(a => a.AreaId).ToList();

                // Get channels
                var wholeCountryId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
                // is the municipality in 'Åland'? So do we need to include also AreaInformationType WholeCountryExceptAlandIslands?
                if (IsAreaInAland(unitOfWork, areas, typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString()))) // Åland
                {
                    return c => (c.AreaInformationTypeId == wholeCountryId) ||
                    c.AreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || c.Areas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId));
                }
                else
                {
                    var wholeCountryExceptAlandId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
                    return c => (c.AreaInformationTypeId == wholeCountryId) || c.AreaInformationTypeId == wholeCountryExceptAlandId ||
                    c.AreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || c.Areas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId));
                }
            }
            else
            {
                // Only return services that have the defined municipality attached
                return c => c.AreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || c.Areas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId));
            }
        }

        protected Expression<Func<OrganizationVersioned, bool>> GetAreaFilterForOrganization(IUnitOfWork unitOfWork, ITypesCache typesCache, Guid areaId, bool includeWholeCountry)
        {
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
                    return o => o.AreaInformationTypeId == wholeCountryId || o.OrganizationAreas.Any(a => a.AreaId == areaId);
                }
                else
                {
                    var wholeCountryExceptAlandId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
                    return o => o.AreaInformationTypeId == wholeCountryId || o.AreaInformationTypeId == wholeCountryExceptAlandId
                    || o.OrganizationAreas.Any(a => a.AreaId == areaId);
                }
            }
            else
            {
                return o => o.OrganizationAreas.Any(a => a.AreaId == areaId);
            }
        }

        protected Expression<Func<ServiceVersioned, bool>> GetAreaFilterForService(IUnitOfWork unitOfWork, ITypesCache typesCache, Guid areaId, bool includeWholeCountry)
        {
            if (includeWholeCountry)
            {
                var wholeCountryId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
                // Is the area in Åland?
                // Lets first get the municipalities for the defined area and then all the areas related to the municipalities.
                // The area is in Åland if the related municipality is in Åland province.
                var repo = unitOfWork.CreateRepository<IAreaMunicipalityRepository>();
                var municipalities = repo.All()
                    .Where(a => a.AreaId == areaId).Select(a => a.MunicipalityId).ToList();
                var allAreas = repo.All().Where(a => municipalities.Contains(a.MunicipalityId)).Select(a => a.AreaId).ToList();
                if (IsAreaInAland(unitOfWork, allAreas, typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString()))) // Åland
                {
                    return c => c.AreaInformationTypeId == wholeCountryId || c.Areas.Any(a => a.AreaId == areaId);
                }
                else
                {
                    var wholeCountryExceptAlandId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
                    return c => c.AreaInformationTypeId == wholeCountryId || c.AreaInformationTypeId == wholeCountryExceptAlandId
                                    || c.Areas.Any(a => a.AreaId == areaId);
                }
            }
            else
            {
                return c => c.Areas.Any(a => a.AreaId == areaId);
            }
        }

        protected Expression<Func<ServiceChannelVersioned, bool>> GetAreaFilterForServiceChannel(IUnitOfWork unitOfWork, ITypesCache typesCache, Guid areaId, bool includeWholeCountry)
        {
            if (includeWholeCountry)
            {
                var wholeCountryId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
                // Is the area in Åland?
                // Lets first get the municipalities for the defined area and then all the areas related to the municipalities.
                // The area is in Åland if the related municipality is in Åland province.
                var repo = unitOfWork.CreateRepository<IAreaMunicipalityRepository>();
                var municipalities = repo.All()
                    .Where(a => a.AreaId == areaId).Select(a => a.MunicipalityId).ToList();
                var allAreas = repo.All().Where(a => municipalities.Contains(a.MunicipalityId)).Select(a => a.AreaId).ToList();
                if (IsAreaInAland(unitOfWork, allAreas, typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString()))) // Åland
                {
                    return c => c.AreaInformationTypeId == wholeCountryId || c.Areas.Any(a => a.AreaId == areaId);
                }
                else
                {
                    var wholeCountryExceptAlandId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
                    return c => c.AreaInformationTypeId == wholeCountryId || c.AreaInformationTypeId == wholeCountryExceptAlandId
                                    || c.Areas.Any(a => a.AreaId == areaId);
                }
            }
            else
            {
                return c => c.Areas.Any(a => a.AreaId == areaId);
            }
        }

        private bool IsAreaInAland(IUnitOfWork unitOfWork, List<Guid> areas, Guid provinceId)
        {
            var areaCode = unitOfWork.CreateRepository<IAreaRepository>().All().Where(a => areas.Contains(a.Id) && a.AreaTypeId == provinceId).Select(a => a.Code).FirstOrDefault();
            if (areaCode != null && areaCode.Trim() == "20") // Åland
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
