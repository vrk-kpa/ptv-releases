﻿/**
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
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models.V2.Channel.OpeningHours
{
    /// <summary>
    /// View model of normal hours
    /// </summary>
    /// <seealso cref="VmOpeningHour" />
    public class VmNormalHours : VmOpeningHour
    {
        /// <summary>
        /// Gets or sets the daily hours.
        /// </summary>
        /// <value>
        /// The daily hours.
        /// </value>
        [JsonProperty(PropertyName = "dailyOpeningHours")]
        public Dictionary<string, VmNormalDailyOpeningHour> DailyHours { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="VmNormalHours"/> is nonstop.
        /// </summary>
        /// <value>
        ///   <c>true</c> if nonstop; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("isOpenNonStop")]
        public bool IsNonStop { get; set; }
    }
}