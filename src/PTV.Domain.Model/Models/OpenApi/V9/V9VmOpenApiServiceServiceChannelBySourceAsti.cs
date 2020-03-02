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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework;
using PTV.Framework.Attributes;
using PTV.Framework.Attributes.ValidationAttributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V9
{
    /// <summary>
    /// OPEN API - View Model of Service service channel - with external source and external types
    /// </summary>
    public class V9VmOpenApiServiceServiceChannelBySourceAsti : IOpenApiAstiConnectionBySource<V9VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>
    {
        /// <summary>
        /// The external source id for service channel.
        /// </summary>
        [RegularExpression(ValidationConsts.ExternalSource)]
        [Required]
        public string ServiceChannelSourceId { get; set; }

        /// <summary>
        /// List of localized service channel relationship descriptions. Possible type values are: Description, ChargeTypeAdditionalInfo.
        /// </summary>
        [ListValueNotEmpty("Value")]
        [ListPropertyAllowedValues("Type", allowedValues: new[] { "Description", "ChargeTypeAdditionalInfo" })]
        [ListPropertyMaxLength(500, "Value", "Description")]
        [ListPropertyMaxLength(500, "Value", "ChargeTypeAdditionalInfo")]
        public IList<VmOpenApiLocalizedListItem> Description { get; set; } = new List<VmOpenApiLocalizedListItem>();

        /// <summary>
        /// Service charge type. Possible values are: Chargeable, FreeOfCharge or Other
        /// </summary>
        [JsonProperty(Order = 3)]
        [ValidOpenApiEnum(typeof(ServiceChargeTypeEnum))]
        public string ServiceChargeType { get; set; }

        /// <summary>
        /// The extra types related to service and service channel connection.
        /// </summary>
        [JsonProperty(Order = 5)]
        public virtual IList<VmOpenApiExtraType> ExtraTypes { get; set; } = new List<VmOpenApiExtraType>();

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
        public virtual V9VmOpenApiContactDetailsInBase ContactDetails { get; set; }

        /// <summary>
        /// Indicates if value for property ServiceChargeType should be deleted.
        /// </summary>
        [JsonProperty(Order = 10)]
        public virtual bool DeleteServiceChargeType { get; set; }

        /// <summary>
        /// Indicates if all descriptions should be deleted.
        /// </summary>
        [JsonProperty(Order = 11)]
        public virtual bool DeleteAllDescriptions { get; set; }

        /// <summary>
        /// Indicates if all extra types should be deleted.
        /// </summary>
        [JsonProperty(Order = 12)]
        public virtual bool DeleteAllExtraTypes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all service hours should be deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all service hours should be deleted; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(Order = 13)]
        public virtual bool DeleteAllServiceHours { get; set; }

        /// <summary>
        /// Indicates if connection between service and service channel is ASTI related.
        /// </summary>
        [JsonIgnore]
        public virtual bool IsASTIConnection { get; set; }

        /// <summary>
        /// Converts model into latest version.
        /// </summary>
        public V11VmOpenApiServiceServiceChannelBySourceAsti ConvertToLatestVersion()
        {
            var model =  new V11VmOpenApiServiceServiceChannelBySourceAsti
            {
                ServiceChannelSourceId = this.ServiceChannelSourceId,
                Description = this.Description,
                ServiceChargeType = this.ServiceChargeType,
                ExtraTypes = this.ExtraTypes,
                ContactDetails = this.ContactDetails,
                DeleteServiceChargeType = this.DeleteServiceChargeType,
                DeleteAllDescriptions = this.DeleteAllDescriptions,
                DeleteAllExtraTypes = this.DeleteAllExtraTypes,
                DeleteAllServiceHours = this.DeleteAllServiceHours,
                IsASTIConnection = this.IsASTIConnection
            };
            this.ServiceHours?.ForEach(h => model.ServiceHours.Add(h.ConvertToInVersionBase()));
            return model;
        }
    }
}
