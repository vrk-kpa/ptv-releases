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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V2;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service hour
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceHourBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceHour" />
    public class VmOpenApiServiceHour : VmOpenApiServiceHourBase, IVmOpenApiServiceHour
    {
        /// <summary>
        /// Type of service hour exception type. Valid values are: Open or Closed.
        /// </summary>
        [ValidEnum(typeof(ExceptionHoursStatus))]
        public string ExceptionHourType { get; set; }

        /// <summary>
        /// Is this service hour effective on monday.
        /// </summary>
        public bool Monday { get; set; }

        /// <summary>
        /// Is this service hour effective on tuesday.
        /// </summary>
        public bool Tuesday { get; set; }

        /// <summary>
        /// Is this service hour effective on wednesday.
        /// </summary>
        public bool Wednesday { get; set; }

        /// <summary>
        /// Is this service hour effective on thursday.
        /// </summary>
        public bool Thursday { get; set; }

        /// <summary>
        /// Is this service hour effective on friday.
        /// </summary>
        public bool Friday { get; set; }

        /// <summary>
        /// Is this service hour effective on saturday.
        /// </summary>
        public bool Saturday { get; set; }

        /// <summary>
        /// Is this service hour effective on sunday.
        /// </summary>
        public bool Sunday { get; set; }

        /// <summary>
        /// Opening time in format HH:mm for example 08:00.
        /// </summary>
        public string Opens { get; set; }

        /// <summary>
        /// Closing time in format HH:mm for example 19:00
        /// </summary>
        public string Closes { get; set; }

        /// <summary>
        /// Set to true to present a time between the valid from and to times as closed.
        /// </summary>
        [JsonIgnore]
        public override bool IsClosed
        {
            get
            {
                return base.IsClosed;
            }

            set
            {
                base.IsClosed = value;
            }
        }

        /// <summary>
        /// Set to true to present that this entry is valid for now.
        /// </summary>
        [JsonIgnore]
        public override bool ValidForNow
        {
            get
            {
                return base.ValidForNow;
            }

            set
            {
                base.ValidForNow = value;
            }
        }

        /// <summary>
        /// Localized list of additional information.
        /// </summary>
        public new IList<VmOpenApiLanguageItem> AdditionalInformation { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of servicing hours (open and closes times).
        /// </summary>
        [JsonIgnore]
        public override IReadOnlyList<V2VmOpenApiDailyOpeningTime> OpeningHour
        {
            get
            {
                return base.OpeningHour;
            }

            set
            {
                base.OpeningHour = value;
            }
        }
    }
}
