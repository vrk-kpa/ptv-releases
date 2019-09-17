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
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// View model with paging.
    /// </summary>
    public abstract class VmOpenApiModelWithPagingBase<TItem> : IVmOpenApiModelWithPagingBase<TItem>
        //where TItem : IVmOpenApiItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmOpenApiModelWithPagingBase{TItem}"/> class.
        /// </summary>
        public VmOpenApiModelWithPagingBase() { }

        /// <summary>
        /// Resultset page number (resultset paging). Page numbering starts from one.
        /// </summary>
        [JsonProperty(Order = 1)]
        public int PageNumber { get; set; }

        /// <summary>
        /// How many results per page are returned (resultset paging).
        /// </summary>
        [JsonProperty(Order = 2)]
        public int PageSize { get; set; }

        /// <summary>
        /// Total count of pages the resultset has (resultset paging).
        /// </summary>
        [JsonProperty(Order = 3)]
        public int PageCount { get; set; }

        /// <summary>
        /// List of entity Guids.
        /// </summary>
        [JsonProperty(Order = 4)]
        public virtual IList<TItem> ItemList { get; set; }
    }
}
