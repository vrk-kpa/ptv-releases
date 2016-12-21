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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    public class VmOpenApiServiceHour : IVmOpenApiServiceHour
    {
        /// <summary>
        /// Type of service hour. Valid values are: Standard, Exception or Special.
        /// </summary>
        [ValidEnum(typeof(ServiceHoursTypeEnum))]
        public string ServiceHourType { get; set; }

        /// <summary>
        /// Type of service hour exception type. Valid values are: Open or Closed.
        /// </summary>
        [ValidEnum(typeof(ExceptionHoursStatus))]
        public string ExceptionHourType { get; set; }

        /// <summary>
        /// Date time where from this entry is valid.
        /// </summary>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Date time to this entry is valid.
        /// </summary>
        public DateTime? ValidTo { get; set; }

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
        /// Localized list of additional information.
        /// </summary>
        public IReadOnlyList<VmOpenApiLanguageItem> AdditionalInformation { get; set; } = new List<VmOpenApiLanguageItem>();

        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
    }
}
