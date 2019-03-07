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
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes.ValidationAttributes;
using PTV.Domain.Model.Models.OpenApi.V9;

namespace PTV.Domain.Model.Models.OpenApi.V8
{
    /// <summary>
    /// OPEN API V8 - View Model of service and channel relation
    /// </summary>
    public class V8VmOpenApiServiceAndChannelIn : IOpenApiConnectionPost<V8VmOpenApiContactDetailsIn, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>
    {
        /// <summary>
        /// PTV service identifier.
        /// </summary>
        [Required]
        [ValidGuid]
        [JsonProperty(Order = 1)]
        public virtual string ServiceId { get; set; }

        /// <summary>
        /// PTV service channel identifier.
        /// </summary>
        [Required]
        [ValidGuid]
        [JsonProperty(Order = 2)]
        public virtual string ServiceChannelId { get; set; }

        /// <summary>
        /// Service charge type. Possible values are: Chargeable, FreeOfCharge or Other.
        /// </summary>
        [JsonProperty(Order = 3)]
        [ValidOpenApiEnum(typeof(ServiceChargeTypeEnum))]
        public string ServiceChargeType { get; set; }

        /// <summary>
        /// List of localized service channel relationship descriptions. Possible type values are: Description, ChargeTypeAdditionalInfo.
        /// </summary>
        [JsonProperty(Order = 4)]
        [ListValueNotEmpty("Value")]
        [ListPropertyAllowedValues("Type", allowedValues: new[] { "Description", "ChargeTypeAdditionalInfo" })]
        [ListPropertyMaxLength(500, "Value", "Description")]
        [ListPropertyMaxLength(500, "Value", "ChargeTypeAdditionalInfo")]
        public IList<VmOpenApiLocalizedListItem> Description { get; set; } = new List<VmOpenApiLocalizedListItem>();

        /// <summary>
        /// List of connection related service hours.
        /// </summary>
        [JsonProperty(Order = 6)]
        [ListWithOpenApiEnum(typeof(ServiceHoursTypeEnum), "ServiceHourType")]
        public virtual IList<V8VmOpenApiServiceHour> ServiceHours { get; set; } = new List<V8VmOpenApiServiceHour>();

        /// <summary>
        /// List of connection related contact details.
        /// </summary>
        [JsonProperty(Order = 7)]
        public virtual V8VmOpenApiContactDetailsIn ContactDetails { get; set; }

        #region methods

        /// <summary>
        /// Gets the in base version model.
        /// </summary>
        /// <returns>in base version model</returns>
        public virtual V9VmOpenApiServiceServiceChannelAstiInBase ConvertToLatestVersion()
        {
            var model = new V9VmOpenApiServiceServiceChannelAstiInBase
            {
                ServiceId = this.ServiceId,
                ServiceChannelId = this.ServiceChannelId,
                ServiceChargeType = this.ServiceChargeType,
                Description = this.Description,
                ServiceHours = this.ServiceHours,
                ContactDetails = this.ContactDetails?.ConvertToInBaseModel(),
                IsASTIConnection = false
            };

            return model;
        }

        #endregion
    }
}
