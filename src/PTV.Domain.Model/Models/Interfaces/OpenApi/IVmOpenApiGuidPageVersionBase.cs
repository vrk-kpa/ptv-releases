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
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of guid page -version base
    /// </summary>
    public interface IVmOpenApiGuidPageVersionBase
    {
        /// <summary>
        /// Gets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        int PageNumber { get; }
        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        int PageSize { get; }
        /// <summary>
        /// Gets the page count.
        /// </summary>
        /// <value>
        /// The page count.
        /// </value>
        int PageCount { get; }
        /// <summary>
        /// Gets or sets the unique identifier list.
        /// </summary>
        /// <value>
        /// The unique identifier list.
        /// </value>
        IList<Guid> GuidList { get; set; }
        /// <summary>
        /// Gets or sets the item list.
        /// </summary>
        /// <value>
        /// The item list.
        /// </value>
        IList<VmOpenApiItem> ItemList { get; set; }

        /// <summary>
        /// Sets the page count.
        /// </summary>
        /// <param name="totalCount">The total count.</param>
        void SetPageCount(int totalCount);
        /// <summary>
        /// Determines whether page number is valid.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if page number is valid; otherwise, <c>false</c>.
        /// </returns>
        bool IsValidPageNumber();
        /// <summary>
        /// Gets the size of the skip.
        /// </summary>
        /// <returns>size</returns>
        int GetSkipSize();
        /// <summary>
        /// Gets the size of the take.
        /// </summary>
        /// <returns>size</returns>
        int GetTakeSize();
    }
}
