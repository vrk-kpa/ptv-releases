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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service hour - base
    /// </summary>
    /// <typeparam name="TModelOpeningTime"></typeparam>
    public class VmOpenApiServiceHourBase<TModelOpeningTime> : IVmOpenApiServiceHourBase<TModelOpeningTime>
         where TModelOpeningTime : IVmOpenApiDailyOpeningTime
    {
        /// <summary>
        /// Type of service hour. Valid values are: DaysOfTheWeek, Exceptional or OverMidnight.
        /// In version 7 and older: Standard, Exception or Special.
        /// </summary>
        [Required]
        public virtual string ServiceHourType { get; set; }
        /// <summary>
        /// Date time where from this entry is valid.
        /// </summary>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Date time to this entry is valid.
        /// </summary>
        public DateTime? ValidTo { get; set; }

        /// <summary>
        /// Set to true to present a time between the valid from and to times as closed.
        /// </summary>
        public virtual bool IsClosed { get; set; }

        /// <summary>
        /// Set to true to present that this entry is valid for now.
        /// </summary>
        public virtual bool ValidForNow { get; set; }

        /// <summary>
        /// Set to true to present a time between the valid from and to times as allways open.
        /// </summary>
        public virtual bool IsAlwaysOpen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open on reservation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if open on reservation; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsReservation { get; set; }

        /// <summary>
        /// Localized list of additional information.
        /// </summary>
        [ListPropertyMaxLength(100, "Value")]
        public virtual IList<VmOpenApiLanguageItem> AdditionalInformation { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// Gets or sets the opening hour.
        /// </summary>
        /// <value>
        /// The opening hour.
        /// </value>
        public IList<TModelOpeningTime> OpeningHour { get; set; } = new List<TModelOpeningTime>();

        /// <summary>
        /// The order of service hours.
        /// </summary>
        [JsonIgnore]
        public int? OrderNumber { get; set; }

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
