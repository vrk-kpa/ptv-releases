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
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.V2.Channel.OpeningHours
{
    /// <summary>
    /// View model of daily hours
    /// </summary>
    public class VmNormalDailyOpeningHour
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="VmDailyHourCommon"/> is day.
        /// </summary>
        /// <value>
        ///   <c>true</c> if day; otherwise, <c>false</c>.
        /// </value>
        public bool Active { get; set; }
        /// <summary>
        /// Gets or sets the intervals.
        /// </summary>
        /// <value>
        /// The intervals.
        /// </value>
        public List<VmDailyHourCommon> Intervals { get; set; }
    }

    /// <summary>
    /// View model of daily hours
    /// </summary>
    public class VmDailyHourCommon : IVmOwnerReference
    {
        /// <summary>
        /// Gets or sets the day from.
        /// </summary>
        /// <value>
        /// The day from.
        /// </value>
        [JsonIgnore]
        public WeekDayEnum? DayFrom { get; set; }
        /// <summary>
        /// Gets or sets the day to.
        /// </summary>
        /// <value>
        /// The day from.
        /// </value>
        [JsonIgnore]
        public WeekDayEnum? DayTo { get; set; }
        /// <summary>
        /// Gets or sets the time from.
        /// </summary>
        /// <value>
        /// The time from.
        /// </value>
        [JsonProperty("from")]
        public long? TimeFrom { get; set; }
        /// <summary>
        /// Gets or sets the time to.
        /// </summary>
        /// <value>
        /// The time to.
        /// </value>
        [JsonProperty("to")]
        public long? TimeTo { get; set; }
        /// <summary>
        /// Id of owner entity
        /// </summary>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
        /// <summary>
        /// Order of owner entity
        /// </summary>
        [JsonIgnore]
        public int Order { get; set; }
    }
}