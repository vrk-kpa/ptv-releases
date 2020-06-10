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
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class SahaOrganizationsPagingHandler : VersionsPagingHandlerBase<VmOpenApiOrganizationSahaGuidPage, VmOpenApiOrganizationSaha, OrganizationVersioned, OrganizationName, OrganizationLanguageAvailability>
    {
        private IPublishingStatusCache publishingStatusCache;
        private ILanguageCache languageCache;
        private ITypesCache typesCache;
        private Dictionary<Guid, string> sahaIds;
        private Dictionary<Guid, List<VmOpenApiLocalizedListItem>> names;

        public SahaOrganizationsPagingHandler(
            DateTime? date,
            DateTime? dateBefore,
            IPublishingStatusCache publishingStatusCache,
            ILanguageCache languageCache,
            ITypesCache typesCache,
            int pageNumber,
            int pageSize
            ) : base(date, dateBefore, typesCache, pageNumber, pageSize)
        {
            this.publishingStatusCache = publishingStatusCache;
            this.languageCache = languageCache;
            this.typesCache = typesCache;
        }

        protected override IList<Expression<Func<OrganizationVersioned, bool>>> GetFilters(IUnitOfWork unitOfWork)
        {
            var publishedId = publishingStatusCache.Get(PublishingStatus.Published);
            var deletedId = publishingStatusCache.Get(PublishingStatus.Deleted);
            var oldPublishedId = publishingStatusCache.Get(PublishingStatus.OldPublished);

            var filters = base.GetFilters(unitOfWork);

            // published and archived versions
            filters.Add(o => (o.PublishingStatusId == publishedId && o.LanguageAvailabilities.Any(l => l.StatusId == publishedId)) ||
                    ((o.PublishingStatusId == deletedId || o.PublishingStatusId == oldPublishedId) && (!o.UnificRoot.Versions.Any(x => x.PublishingStatusId == publishedId))));
            // Get only main and two sub organization levels.
            filters.Add(o => o.ParentId == null || // main level
                    (o.Parent != null && o.Parent.Versions.Any(p => (p.PublishingStatusId == publishedId || p.PublishingStatusId == deletedId) && (p.ParentId == null ||// first level child
                    p.Parent != null && p.Parent.Versions.Any(pp => (pp.PublishingStatusId == publishedId || pp.PublishingStatusId == deletedId) && pp.ParentId == null)))));// second level child

            return filters;
        }

        public override int Search(IUnitOfWork unitOfWork)
        {
            return base.SearchDistinctItems(unitOfWork);
        }

        protected override void SetReturnValues(IUnitOfWork unitOfWork)
        {
            if (EntityIds?.Count > 0)
            {
                // Get related sahaId's for organizations
                sahaIds = unitOfWork.CreateRepository<IRepository<SahaOrganizationInformation>>().All().Where(o => UnificEntityIds.Contains(o.OrganizationId))
                    .ToDictionary(i => i.OrganizationId, i => i.SahaId.ToString());

                // Get names including alternateName
                names = unitOfWork.CreateRepository<IRepository<OrganizationName>>().All().Where(x => EntityIds.Contains(x.OrganizationVersionedId)).OrderBy(i => i.Localization.OrderNumber)
                    .Select(i => new { id = i.OrganizationVersionedId, i.Name, i.LocalizationId, i.TypeId }).ToList().GroupBy(i => i.id)
                    .ToDictionary(i => i.Key, i => i.ToList().Select(x => new VmOpenApiLocalizedListItem
                    {
                        Value = x.Name,
                        Language = languageCache.GetByValue(x.LocalizationId),
                        Type = typesCache.GetByValue<NameType>(x.TypeId).GetOpenApiEnumValue<NameTypeEnum>()
                    }).ToList());
            }
        }

        protected override VmOpenApiOrganizationSaha GetItemData(OrganizationVersioned entity)
        {
            return new VmOpenApiOrganizationSaha
            {
                Id = entity.UnificRootId,
                OrganizationNames = names?.TryGetOrDefault(entity.Id, null),
                ParentOrganizationId = entity.ParentId,
                SahaId = sahaIds.TryGetOrDefault(entity.UnificRootId, null),
                Modified = entity.Modified,
                PublishingStatus = publishingStatusCache.GetByValue(entity.PublishingStatusId)
            };
        }
    }
}
