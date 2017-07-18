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
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service hour - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceHourBase" />
    public class VmOpenApiServiceHourBase : IVmOpenApiServiceHourBase
    {
        /// <summary>
        /// Type of service hour. Valid values are: Standard, Exception or Special.
        /// </summary>
        [ValidEnum(typeof(ServiceHoursTypeEnum))]
        [Required]
        public string ServiceHourType { get; set; }
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
        /// Localized list of additional information.
        /// </summary>
        [ListPropertyMaxLength(100, "Value")]
        public virtual IList<VmOpenApiLanguageItem> AdditionalInformation { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of servicing hours (open and closes times).
        /// </summary>
        public virtual IReadOnlyList<V2VmOpenApiDailyOpeningTime> OpeningHour { get; set; } = new List<V2VmOpenApiDailyOpeningTime>();

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

        /// <summary>
        /// Converts to the base.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>model convereted to the base model</returns>
        public TModel ConvertBase<TModel>() where TModel : IVmOpenApiServiceHourBase, new()
        {
            return new TModel()
            {
                ServiceHourType = this.ServiceHourType,
                ValidFrom = this.ValidFrom,
                ValidTo = this.ValidTo,
                IsClosed = this.IsClosed,
                ValidForNow = this.ValidForNow,
                AdditionalInformation = this.AdditionalInformation,
                OpeningHour = this.OpeningHour
            };
        }
    }
}
