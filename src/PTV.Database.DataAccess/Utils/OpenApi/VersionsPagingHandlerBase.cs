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

using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal abstract class VersionsPagingHandlerBase<TModel, TItemModel, TEntity> : FilteredByDatePagingHandlerBase<TModel, TItemModel, TEntity>
        where TModel : IVmOpenApiModelWithPagingBase<TItemModel>, new()
        where TItemModel : IVmOpenApiItemBase, new()
        where TEntity : class, IEntityIdentifier, IAuditing, IVersionedVolume
    {
        private List<Guid> unificEntityIds;

        protected List<Guid> UnificEntityIds
        {
            get
            {
                if (unificEntityIds != null)
                {
                    return unificEntityIds;
                }
                if (Entities?.Count > 0)
                {
                    unificEntityIds = Entities.Select(i => i.UnificRootId).ToList();
                }
                else
                {
                    unificEntityIds = new List<Guid>();
                }
                return unificEntityIds;
            }
            private set
            {
            }
        }

        public VersionsPagingHandlerBase(
            DateTime? date,
            DateTime? dateBefore,
            int pageNumber,
            int pageSize
            ) : base(date, dateBefore, pageNumber, pageSize)
        {
        }

        protected virtual int SearchDistinctItems(
            IUnitOfWork unitOfWork
            )
        {
            if (PageNumber <= 0) return 0;

            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var query = repository.All();

            // Filter items
            var filters = GetFilters(unitOfWork);
            if (filters?.Count > 0)
            {
                filters.ForEach(a => query = query.Where(a));
            }

            // Get entities total count. Only one per root id!
            var entityIdList = query.Select(i => new { i.Id, i.UnificRootId, i.Modified, i.Created }).ToList().GroupBy(x => x.UnificRootId).ToDictionary(i => i.Key, i => i.ToList())
                .Select(x => x.Value.OrderByDescending(c => c.Modified).FirstOrDefault()).Where(x => x != null).ToList();
            SetPageCount(entityIdList.Count);

            if (ValidPageNumber)
            {
                EntityIds = entityIdList.OrderBy(o => o.Created).Skip(GetSkipSize()).Take(GetTakeSize()).Select(e => e.Id).ToList();
                Entities = repository.All().Where(e => EntityIds.Contains(e.Id)).ToList();
            }

            return TotalCount;
        }

    }
}
