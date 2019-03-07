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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal abstract class GuidPageHandlerBase<TModel, TItemModel>
        where TModel : IVmOpenApiGuidPageVersionBase<TItemModel>, new()
        where TItemModel : IVmOpenApiItemBase, new()
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

        public GuidPageHandlerBase(
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

        public abstract IVmOpenApiGuidPageVersionBase<TItemModel> GetModel();

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
    }
}
