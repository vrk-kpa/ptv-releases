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
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.ServiceManager;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of search forms
    /// </summary>
    /// <seealso cref="PTV.Framework.ServiceManager.VmBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmSearchBase" />
    public class VmSearchResult<T> : VmEnumBase, IVmSearchBase
    {
        /// <summary>
        /// Gets or sets the services.
        /// </summary>
        /// <value>
        /// The services.
        /// </value>
        [JsonProperty("data")]
        public IReadOnlyList<T> SearchResult { get; set; }

        /// <summary>
        /// Gets or sets the count of found results.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count { get; set; }
        /// <summary>
        /// Gets or sets the maximum page count.
        /// </summary>
        /// <value>
        /// The maximum page count.
        /// </value>
        public int MaxPageCount { get; set; } = CoreConstants.MaximumNumberOfAllItems;
        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        public int PageNumber { get; set; }

        /// <summary>
        /// Indication that more result is available
        /// </summary>
        public bool MoreAvailable { get; set; }

        /// <summary>
        /// Gets or sets how many items to skip.
        /// </summary>
        /// <value>
        /// The skip amount.
        /// </value>
        public int Skip { get; set; }
    }

    /// <summary>
    /// Model for parameters required for search
    /// </summary>
    public interface IVmSearchParamsBase
    {
        /// <summary>
        /// Current number of page
        /// </summary>
        int PageNumber { get; set; }

        /// <summary>
        /// Items per one page
        /// </summary>
        int MaxPageCount { get; set; }

        /// <summary>
        /// How many results should be skipped
        /// </summary>
        int Skip { get; set; }
    }
    
    /// <summary>
    /// Model for parameters required for search
    /// </summary>
    public interface IVmSearchEntities
    {
        /// <summary>
        /// unific root ids
        /// </summary>
        IEnumerable<Guid> EntityIds{ get; set; }
        
        /// <summary>
        /// version ids
        /// </summary>
        IEnumerable<Guid> EntityVersionIds{ get; set; }
    }
    
    /// <summary>
    /// Model for parameters required for search
    /// </summary>
    public class VmSearchParamsBase : IVmSearchParamsBase
    {
        /// <summary>
        /// Current number of page
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Items per one page
        /// </summary>
        public int MaxPageCount { get; set; }
        
        /// <summary>
        /// How many results should be skipped
        /// </summary>
        public int Skip { get; set; }
    }
}
