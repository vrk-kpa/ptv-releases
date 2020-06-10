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
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal abstract class EntitiesByStatusPagingHandler<TModel, TItemModel, TEntity, TRoot, TName, TLanguageAvailability> : VersionsPagingHandlerBase<TModel, TItemModel, TEntity, TName, TLanguageAvailability>
        where TModel : IVmOpenApiModelWithPagingBase<TItemModel>, new()
        where TItemModel : IVmOpenApiItem, new()
        where TEntity : class, IEntityIdentifier, IAuditing, IVersionedVolume<TRoot>
        where TRoot : VersionedRoot<TEntity>
        where TName : IName
        where TLanguageAvailability : ILanguageAvailabilityBase
    {
        private IPublishingStatusCache publishingStatusCache;

        protected EntityStatusExtendedEnum EntityStatus;
        protected Guid PublishedId;

        public EntitiesByStatusPagingHandler(
            EntityStatusExtendedEnum entityStatus,
            DateTime? date,
            DateTime? dateBefore,
            IPublishingStatusCache publishingStatusCache,
            ITypesCache typesCache,
            int pageNumber,
            int pageSize
            ) : base(date, dateBefore, typesCache, pageNumber, pageSize)
        {
            this.EntityStatus = entityStatus;
            this.publishingStatusCache = publishingStatusCache;
            PublishedId = publishingStatusCache.Get(PublishingStatus.Published);
        }

        public override int Search(IUnitOfWork unitOfWork)
        {
            if (EntityStatus == EntityStatusExtendedEnum.Published)
            {
                return base.Search(unitOfWork);
            }

            // To get active, archived or withdrawn items we need to fetch only one version per root
            return base.SearchDistinctItems(unitOfWork);
        }

        protected override IList<Expression<Func<TEntity, bool>>> GetFilters(IUnitOfWork unitOfWork)
        {
            var draftId = publishingStatusCache.Get(PublishingStatus.Draft);
            var modifiedId = publishingStatusCache.Get(PublishingStatus.Modified);
            var deletedId = publishingStatusCache.Get(PublishingStatus.Deleted);
            var oldPublishedId = publishingStatusCache.Get(PublishingStatus.OldPublished);

            IList<Expression<Func<TEntity, bool>>> filters = base.GetFilters(unitOfWork);

            switch (EntityStatus)
            {
                case EntityStatusExtendedEnum.Published:
                    filters.Add(e => e.PublishingStatusId == PublishedId);
                    break;
                case EntityStatusExtendedEnum.Archived:
                    filters.Add(p => (p.PublishingStatusId == deletedId || p.PublishingStatusId == oldPublishedId) &&
                    !p.UnificRoot.Versions.Any(x => x.PublishingStatusId == PublishedId));
                    break;
                case EntityStatusExtendedEnum.Withdrawn:
                    filters.Add(e => e.PublishingStatusId == modifiedId &&
                        !e.UnificRoot.Versions.Any(x => x.PublishingStatusId == PublishedId));
                    break;
                case EntityStatusExtendedEnum.Active:
                    filters.Add(e => e.PublishingStatusId == draftId || e.PublishingStatusId == PublishedId || e.PublishingStatusId == modifiedId);
                    break;
                default:
                    break;
            }

            return filters;
        }

        protected override TItemModel GetItemData(TEntity entity)
        {
            return new TItemModel
            {
                Id = entity.UnificRootId,
                Name = Names?.TryGetOrDefault(entity.Id, new Dictionary<Guid, string>())?.FirstOrDefault().Value
            };
        }

        protected override void SetReturnValues(IUnitOfWork unitOfWork)
        {
            SetNames(unitOfWork, EntityStatus == EntityStatusExtendedEnum.Published, PublishedId);
        }
    }
}
