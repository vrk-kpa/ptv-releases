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
using PTV.Domain.Model.Enums;
using PTV.Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    /// <summary>
    /// OPEN API V2 - View Model of Daily opening hours
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V2.IV2VmOpenApiDailyOpeningTime" />
    public class V2VmOpenApiDailyOpeningTime : IV2VmOpenApiDailyOpeningTime
    {
        /// <summary>
        /// Starts from weekday (e.g. Monday).
        /// </summary>
        [Required]
        [ValidEnum(typeof(WeekDayEnum))]
        public string DayFrom { get; set; }

        /// <summary>
        /// Ends to weekday (e.g. Monday).
        /// </summary>
        [ValidEnum(typeof(WeekDayEnum))]
        public string DayTo { get; set; }

        /// <summary>
        /// Start time for example 10:00.
        /// </summary>
        [Required]
        public virtual string From { get; set; }

        /// <summary>
        /// End time for example 20:00.
        /// </summary>
        [Required]
        public virtual string To { get; set; }

        /// <summary>
        /// Set to true to have extra time for a day. Enables to have open times like 10:00-14:00 and also on the same day 16:00-20:00. So the latter time is extra time.
        /// </summary>
        public bool IsExtra { get; set; }

        /// <summary>
        /// Gets or sets the owner reference identifier.
        /// </summary>
        /// <value>
        /// The owner reference identifier.
        /// </value>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
    }
}
