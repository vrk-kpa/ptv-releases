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
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using System;

namespace PTV.Domain.Model.Models.V2.Channel.OpeningHours
{
    /// <summary>
    /// Base view model of opening hours
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEntityBase" />
    public class VmOpeningHour : VmEntityBase
    {
        /// <summary>
        /// Gets or sets the additional information.
        /// </summary>
        /// <value>
        /// The additional information.
        /// </value>
        [JsonProperty(PropertyName = "title")]
        public Dictionary<string, string> Name { get; set; }

        /// <summary>
        /// Gets or sets the valid from.
        /// </summary>
        /// <value>
        /// The valid from.
        /// </value>
        public long? DateFrom { get; set; }
        /// <summary>
        /// Gets or sets the valid to.
        /// </summary>
        /// <value>
        /// The valid to.
        /// </value>
        public long? DateTo { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is date range.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is date range; otherwise, <c>false</c>.
        /// </value>
        public bool IsPeriod { get; set; }

        /// <summary>
        /// Gets or sets the type of the service hours.
        /// </summary>
        /// <value>
        /// The type of the service hours.
        /// </value>
        [JsonIgnore]
        public ServiceHoursTypeEnum ServiceHoursType { get; set; }

        /// <summary>
        /// Gets or sets the type of the order hours.
        /// </summary>
        /// <value>
        /// The type of the service hours.
        /// </value>
        [JsonIgnore]
        public int? OrderNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Guid OwnerReferenceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Guid OwnerReferenceId2 { get; set; }
    }
}