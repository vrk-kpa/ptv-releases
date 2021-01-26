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
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class EntitiesWithConnectionsPagingHandler<TEntity, TRoot, TName, TLanguageAvailability> : V3GuidPagingHandler<TEntity, TRoot, TName, TLanguageAvailability>
        where TEntity : class, IEntityIdentifier, IAuditing, IVersionedVolume<TRoot>
        where TRoot : VersionedRoot<TEntity>
        where TName : IName
        where TLanguageAvailability : ILanguageAvailabilityBase
    {
        private DateTime? date;
        private DateTime? dateBefore;

        public EntitiesWithConnectionsPagingHandler(
            EntityStatusExtendedEnum entityStatus,
            DateTime? date,
            DateTime? dateBefore,
            IPublishingStatusCache publishingStatusCache,
            ITypesCache typesCache,
            int pageNumber,
            int pageSize
            ) :base(entityStatus, date, dateBefore, publishingStatusCache, typesCache, pageNumber, pageSize)
        {
            this.date = date;
            this.dateBefore = dateBefore;
        }

        protected override IList<Expression<Func<TEntity, bool>>> GetFilters(IUnitOfWork unitOfWork)
        {
            // In case we are fetching published items having updates within certain date frame we need to also take into account updates within connection data.
            if (EntityStatus == EntityStatusExtendedEnum.Published && (date.HasValue || dateBefore.HasValue))
            {
                // Let's first get a list of items having updates within connections
                List<Guid> entitiesWithConnectionUpdates = null;
                var connectionUpdatesQuery = unitOfWork.CreateRepository<ITrackingServiceServiceChannelRepository>().All();
                if (date.HasValue)
                {
                    connectionUpdatesQuery = connectionUpdatesQuery.Where(connectionChange => connectionChange.Created > date.Value);
                }
                if (dateBefore.HasValue)
                {
                    connectionUpdatesQuery = connectionUpdatesQuery.Where(connectionChange => connectionChange.Created < dateBefore.Value);
                }

                // Are we fetching for Services or Channels?
                if (typeof(TRoot) == typeof(Service))
                {
                    entitiesWithConnectionUpdates = connectionUpdatesQuery.Select(connectionChange => connectionChange.ServiceId).Distinct().ToList();
                }
                else if (typeof(TRoot) == typeof(ServiceChannel))
                {
                    entitiesWithConnectionUpdates = connectionUpdatesQuery.Select(connectionChange => connectionChange.ChannelId).Distinct().ToList();
                }

                if (entitiesWithConnectionUpdates?.Count > 0)
                {
                    // The entity needs to be published and its modified date should match the set dates or the entity should be included within entities having connection updates.
                    IList<Expression<Func<TEntity, bool>>> filters = new List<Expression<Func<TEntity, bool>>>();
                    filters.Add(entity => entity.PublishingStatusId == PublishedId);

                    if (date.HasValue && dateBefore.HasValue)
                    {
                        filters.Add(entity => (entity.Modified > date.Value && entity.Modified < dateBefore.Value) || entitiesWithConnectionUpdates.Contains(entity.UnificRootId));
                    }
                    else if (date.HasValue)
                    {
                        filters.Add(entity => entity.Modified > date.Value || entitiesWithConnectionUpdates.Contains(entity.UnificRootId));
                    }
                    else
                    {
                        filters.Add(entity => entity.Modified < dateBefore.Value || entitiesWithConnectionUpdates.Contains(entity.UnificRootId));
                    }

                    return filters;
                }

                return base.GetFilters(unitOfWork);
            }
            else
            {
                return base.GetFilters(unitOfWork);
            }
        }
    }
}
