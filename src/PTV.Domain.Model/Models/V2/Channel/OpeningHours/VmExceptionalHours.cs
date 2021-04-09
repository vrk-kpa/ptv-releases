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

using Newtonsoft.Json;

namespace PTV.Domain.Model.Models.V2.Channel.OpeningHours
{
    /// <summary>
    /// View model of exceptional hours
    /// </summary>
    /// <seealso cref="VmOpeningHour" />
    public class VmExceptionalHours : VmOpeningHour
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmExceptionalHours"/> class.
        /// </summary>
        public VmExceptionalHours()
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
        /// Gets or sets the time from.
        /// </summary>
        /// <value>
        /// The time from.
        /// </value>
        public long? TimeFrom { get => OpeningPeriod?.TimeFrom; set => OpeningPeriod.TimeFrom = value; }
        /// <summary>
        /// Gets or sets the time to.
        /// </summary>
        /// <value>
        /// The time to.
        /// </value>
        public long? TimeTo { get => OpeningPeriod?.TimeTo; set => OpeningPeriod.TimeTo = value; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="VmExceptionalHours"/> is closed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if closed; otherwise, <c>false</c>.
        /// </value>
        public bool ClosedForPeriod { get; set; }
    }
}
