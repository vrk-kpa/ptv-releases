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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Extensions;
using System;
using System.Linq;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class ServicesAndChannelsByOrganizationPagingHandler : PagingHandlerBase<VmOpenApiEntityGuidPage, VmOpenApiEntityItem>
    {
        private Guid organizationId;
        private bool getSpecialTypes;
        private IPublishingStatusCache publishingStatusCache;
        private ITypesCache typesCache;
        private DateTime? date;
        private DateTime? dateBefore;

        public ServicesAndChannelsByOrganizationPagingHandler(
            Guid organizationId,
            bool getSpecialTypes,
            IPublishingStatusCache publishingStatusCache,
            ITypesCache typesCache,
            DateTime? date,
            DateTime? dateBefore,
            int pageNumber,
            int pageSize
            ) : base(pageNumber, pageSize)
        {
            this.organizationId = organizationId;
            this.getSpecialTypes = getSpecialTypes;
            this.publishingStatusCache = publishingStatusCache;
            this.typesCache = typesCache;
            this.date = date;
            this.dateBefore = dateBefore;
        }

        public override int Search(IUnitOfWork unitOfWork)
        {
            var publishedId = publishingStatusCache.Get(PublishingStatus.Published);

            var serviceQuery = unitOfWork.CreateRepository<IRepository<ServiceVersioned>>().All()
                    .Where(s => s.PublishingStatusId == publishedId && s.LanguageAvailabilities.Any(l => l.StatusId == publishedId) &&
                    (s.OrganizationId == organizationId || // Main responsible organization
                    s.OrganizationServices.Any(o => o.OrganizationId == organizationId) || // Other responsible organizations
                    s.ServiceProducers.Any(sp => sp.Organizations.Any(o => o.OrganizationId == organizationId)) // Producers
                    ));

            var channelQuery = unitOfWork.CreateRepository<IRepository<ServiceChannelVersioned>>().All()
                .Where(c => c.OrganizationId.Equals(organizationId) && c.PublishingStatusId == publishedId && c.LanguageAvailabilities.Any(l => l.StatusId == publishedId));

            if (date.HasValue)
            {
                serviceQuery = serviceQuery.Where(service => service.Modified > date.Value);
                channelQuery = channelQuery.Where(serviceChannel => serviceChannel.Modified > date.Value);
            }
            if (dateBefore.HasValue)
            {
                serviceQuery = serviceQuery.Where(service => service.Modified < dateBefore.Value);
                channelQuery = channelQuery.Where(serviceChannel => serviceChannel.Modified < dateBefore.Value);
            }

            SetPageCount(serviceQuery.Count() + channelQuery.Count());
            if (ValidPageNumber)
            {
                var services = serviceQuery.Select(i => new VmOpenApiEntityItem
                {
                    Id = i.UnificRootId,
                    Created = i.Created,
                    Modified = i.Modified,
                    Type = getSpecialTypes ? 
                        (i.TypeId.IsAssigned() ? typesCache.GetByValue<ServiceType>(i.TypeId.Value).GetOpenApiEnumValue<ServiceTypeEnum>() : null)
                        : typeof(Service).Name,
                    GeneralDescriptionId = i.StatutoryServiceGeneralDescriptionId
                });
                var channels = channelQuery.Select(i => new VmOpenApiEntityItem
                {
                    Id = i.UnificRootId,
                    Created = i.Created,
                    Modified = i.Modified,
                    Type = getSpecialTypes ? typesCache.GetByValue<ServiceChannelType>(i.TypeId) : typeof(ServiceChannel).Name
                });
                var items = services.Union(channels);

                // Get the items for one page
                ViewModel.ItemList = items.OrderBy(o => o.Created).Skip(GetSkipSize()).Take(GetTakeSize()).ToList();

                // Set the type values for services from general description if is attached to one. SFIPTV-782
                if (getSpecialTypes)
                {
                    var serviceItemsWithGd = ViewModel.ItemList.Where(i => i.GeneralDescriptionId.IsAssigned()).ToList();
                    var gdIds = serviceItemsWithGd.Select(i => i.GeneralDescriptionId.Value).ToList();
                    var gdTypes = unitOfWork.CreateRepository<IRepository<StatutoryServiceGeneralDescriptionVersioned>>().All()
                        .Where(gd => gdIds.Contains(gd.UnificRootId) && gd.PublishingStatusId == publishedId && gd.LanguageAvailabilities.Any(l => l.StatusId == publishedId))
                        .ToDictionary(i => i.UnificRootId, i => typesCache.GetByValue<ServiceType>(i.TypeId).GetOpenApiEnumValue<ServiceTypeEnum>());
                    serviceItemsWithGd.ForEach(s => s.Type = gdTypes.TryGetOrDefault(s.GeneralDescriptionId.Value, null));
                }
            }

            return TotalCount;
        }

        public override IVmOpenApiModelWithPagingBase<VmOpenApiEntityItem> GetModel()
        {
            return ViewModel;
        }
    }
}
