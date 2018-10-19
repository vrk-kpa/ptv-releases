﻿/**
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
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V7
{
    /// <summary>
    /// OPEN API - View Model of Service service channel - with external source
    /// </summary>
    public class V7VmOpenApiServiceAndChannelRelationBySource : IOpenApiConnectionBySourcePost<VmOpenApiContactDetailsIn, V4VmOpenApiServiceHour, V2VmOpenApiDailyOpeningTime>
    {
        /// <summary>
        /// The external source id for service.
        /// </summary>
        [RegularExpression(@"^[A-Za-z0-9-.]*$")]
        [Required]
        public string ServiceSourceId { get; set; }

        /// <summary>
        /// The external source id for service channel.
        /// </summary>
        [RegularExpression(@"^[A-Za-z0-9-.]*$")]
        [Required]
        public string ServiceChannelSourceId { get; set; }

        /// <summary>
        /// Service charge type. Possible values are: Charged, Free or Other
        /// </summary>
        [JsonProperty(Order = 3)]
        [ValidEnum(typeof(ServiceChargeTypeEnum))]
        public string ServiceChargeType { get; set; }

        /// <summary>
        /// List of localized service channel relationship descriptions.
        /// </summary>
        [ListValueNotEmpty("Value")]
        [ListPropertyAllowedValues("Type", allowedValues: new[] { "Description", "ChargeTypeAdditionalInfo" })]
        [ListPropertyMaxLength(500, "Value", "Description")]
        [ListPropertyMaxLength(500, "Value", "ChargeTypeAdditionalInfo")]
        public IList<VmOpenApiLocalizedListItem> Description { get; set; } = new List<VmOpenApiLocalizedListItem>();


        /// <summary>
        /// List of connection related service hours.
        /// </summary>
        [JsonProperty(Order = 6)]

        [ListWithEnum(typeof(ServiceHoursTypeEnum), "ServiceHourType")]
        public virtual IList<V4VmOpenApiServiceHour> ServiceHours { get; set; } = new List<V4VmOpenApiServiceHour>();

        /// <summary>
        /// List of connection related contact details.
        /// </summary>
        [JsonProperty(Order = 7)]
        public virtual VmOpenApiContactDetailsIn ContactDetails { get; set; }

        /// <summary>
        /// Convert to latest version
        /// </summary>
        /// <returns></returns>
        public V9VmOpenApiServiceAndChannelRelationBySource ConvertToLatestVersion()
        {
            var vm = new V9VmOpenApiServiceAndChannelRelationBySource
            {
                ServiceSourceId = this.ServiceSourceId,
                ServiceChannelSourceId = this.ServiceChannelSourceId,
                ServiceChargeType = this.ServiceChargeType,
                Description = this.Description,
                ContactDetails = this.ContactDetails?.ConvertToInModel(),
            };
            this.ServiceHours.ForEach(h => vm.ServiceHours.Add(h.ConvertToLatestVersion()));
            return vm;
        }
    }
}
