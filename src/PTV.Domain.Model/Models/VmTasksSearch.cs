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
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model for getting entitiest to tasks accordions
    /// </summary>
    public class VmTasksSearch : IVmSortBase
    {
        /// <summary>
        /// Ctor of search params
        /// </summary>
        public VmTasksSearch()
        {
            SortData = new List<VmSortParam>();
            GetCount = false;
        }
        
        /// <summary>
        /// Entity type service /channel
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public TasksIdsEnum TaskType { get; set; }
        /// <summary>
        /// unific root ids to be find
        /// </summary>
        public IEnumerable<Guid> EntityIds { get; set; }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        public int PageNumber { get; set; }
        /// <summary>
        /// Size of one page
        /// </summary>
        public int MaxPageCount { get; set; }
        /// <summary>
        /// Gets or sets how many items to skip.
        /// </summary>
        /// <value>
        /// The skip amount.
        /// </value>
        public int Skip { get; set; }
        /// <summary>
        /// Sort params.
        /// </summary>
        public List<VmSortParam> SortData { get; set; }
        /// <summary>
        /// If true return count of results
        /// </summary>
        public bool GetCount { get; set; }
    }
}
