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

namespace PTV.Domain.Model.Models.Interfaces
{
    /// <summary>
    /// View model of Service location search 
    /// </summary>
    /// <seealso cref="IVmSearchParamsBase" />
    public class VmServiceLocationChannelAddressSearch : IVmSearchParamsBase, IVmSortBase
    {
        /// <summary>
        /// Street Id
        /// </summary>
        public Guid? Street { get; set; }
        /// <summary>
        /// Street number
        /// </summary>
        public string StreetNumber { get; set; }
        /// <summary>
        /// Postal code Id
        /// </summary>
        public Guid? PostalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MaxPageCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Skip { get; set; }
        /// <summary>
        /// Sort data
        /// </summary>
        public List<VmSortParam> SortData { get; set; }
        /// <summary>
        /// Language code
        /// </summary>
        public string LanguageCode { get; set; }
    }
}
