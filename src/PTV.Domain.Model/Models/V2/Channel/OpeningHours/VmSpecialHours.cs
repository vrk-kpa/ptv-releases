/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models.V2.Channel.OpeningHours
{
    /// <summary>
    /// View model of special hours
    /// </summary>
    /// <seealso cref="VmOpeningHour" />
    public class VmSpecialHours : VmOpeningHour
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmSpecialHours"/> class.
        /// </summary>
        public VmSpecialHours()
        {
            OpeningPeriod = new VmDailyHourCommon();
        }

        /// <summary>
        /// Gets or sets the opening period.
        /// </summary>
        /// <value>
        /// The opening period.
        /// </value>
        [JsonIgnore]
        public VmDailyHourCommon OpeningPeriod { get; set; }

        /// <summary>
        /// Gets or sets the day from.
        /// </summary>
        /// <value>
        /// The day from.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter), true)]
        public WeekDayEnum? DayFrom { get => OpeningPeriod.DayFrom; set => OpeningPeriod.DayFrom = value; }
        /// <summary>
        /// Gets or sets the day to.
        /// </summary>
        /// <value>
        /// The day to.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter), true)]
        public WeekDayEnum? DayTo { get => OpeningPeriod.DayTo; set => OpeningPeriod.DayTo = value; }
        /// <summary>
        /// Gets or sets the time from.
        /// </summary>
        /// <value>
        /// The time from.
        /// </value>
        public long? TimeFrom { get => OpeningPeriod.TimeFrom; set => OpeningPeriod.TimeFrom = value; }
        /// <summary>
        /// Gets or sets the time to.
        /// </summary>
        /// <value>
        /// The time to.
        /// </value>
        public long? TimeTo { get => OpeningPeriod.TimeTo; set => OpeningPeriod.TimeTo = value; }
    }
}
