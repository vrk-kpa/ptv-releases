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
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// View model for Guid-list paging.
    /// </summary>
    public class VmOpenApiGuidPage : IVmOpenApiGuidPage
    {
        private int totalCount;
        public VmOpenApiGuidPage(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        /// <summary>
        /// Resultset page number (resultset paging). Page numbering starts from one.
        /// </summary>
        public int PageNumber { get; private set; }
        /// <summary>
        /// How many results per page are returned (resultset paging).
        /// </summary>
        public int PageSize { get; private set; }
        /// <summary>
        /// Total count of pages the resultset has (resultset paging).
        /// </summary>
        public int PageCount { get; private set; }
        /// <summary>
        /// List of entity Guids.
        /// </summary>
        public IList<Guid> GuidList { get; set; }

        public void SetPageCount(int totalCount)
        {
            this.totalCount = totalCount;
            PageCount = totalCount / PageSize + (totalCount % PageSize > 0 ? 1 : 0);
        }
        public bool IsValidPageNumber()
        {
            if (PageNumber < 0) return false;
            if (PageNumber > PageCount) return false;
            return true;
        }
        public int GetSkipSize()
        {
            // If we are in the first page let's not skip any items, otherwise let's skip previous page items.
            return PageNumber <= 1 ? 0 : (PageNumber - 1) * PageSize;
        }

        public int GetTakeSize()
        {
            // If we are on the last page, let's take the last remaining items, otherwise let's take size of PageSize.
            return PageNumber == PageCount ? (totalCount - (PageSize * (PageNumber - 1))) : PageSize;
        }
    }
}
