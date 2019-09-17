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
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal abstract class PagingAndNameHandlerBase<TModel, TItemModel, TEntity> : PagingHandlerBase<TModel, TItemModel>, IPagingAndNameHandlerBase<TEntity, TItemModel>
        where TModel : IVmOpenApiModelWithPagingBase<TItemModel>, new()
        where TItemModel : IVmOpenApiItemBase, new()
        where TEntity : class, IEntityIdentifier, IAuditing
    {
        private List<Guid> entityIds;

        protected List<TEntity> Entities { get; set; }

        public List<Guid> EntityIds
        {
            get
            {
                if (entityIds != null)
                {
                    return entityIds;
                }
                if (Entities?.Count > 0)
                {
                    entityIds = Entities.Select(i => i.Id).ToList();
                }
                else
                {
                    entityIds = new List<Guid>();
                }
                return entityIds;
            }
            protected set
            {
                entityIds = value;
            }
        }

        public Dictionary<Guid, Dictionary<Guid, string>> Names { get; set; }

        public PagingAndNameHandlerBase(
            int pageNumber,
            int pageSize
            ) : base(pageNumber, pageSize)
        {
        }

        protected virtual IList<Expression<Func<TEntity, bool>>> GetFilters(IUnitOfWork unitOfWork)
        {
            return null;
        }

        protected abstract TItemModel GetItemData(TEntity entity);

        public override int Search(IUnitOfWork unitOfWork)
        {
            if (PageNumber <= 0) return 0;

            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var query = repository.All();

            var filters = GetFilters(unitOfWork);

            // Filter items
            if (filters?.Count > 0)
            {
                filters.ForEach(a => query = query.Where(a));
            }

            // Get entities total count.            
            SetPageCount(query.Count());
            if (ValidPageNumber)
            {
                query = query.OrderBy(o => o.Created).Skip(GetSkipSize()).Take(GetTakeSize());
                Entities = query.ToList();
            }

            return TotalCount;
        }

        public override IVmOpenApiModelWithPagingBase<TItemModel> GetModel()
        {
            if (Entities?.Count > 0)
            {
                ViewModel.ItemList = Entities.Select(i => GetItemData(i)).ToList();
            }

            return ViewModel;
        }
    }
}
