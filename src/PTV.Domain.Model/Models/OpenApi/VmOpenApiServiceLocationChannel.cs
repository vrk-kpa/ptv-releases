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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V2;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    public class VmOpenApiServiceLocationChannel : V2VmOpenApiServiceLocationChannel, IVmOpenApiServiceLocationChannel
    {
        /// <summary>
        /// Service location contact e-mail address.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 12)]
        public string Email { get; set; }
        /// <summary>
        /// Service location contact phone number.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 13)]
        public string Phone { get; set; }
        /// <summary>
        /// Service location contact fax number.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 14)]
        public string Fax { get; set; }

        /// <summary>
        /// Service location latitude coordinate.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 15)]
        public string Latitude { get; set; }

        /// <summary>
        /// Service location longitude coordinate.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 16)]
        public string Longitude { get; set; }

        /// <summary>
        /// Coordinate system used. This property is not used in the API anymore. Do not use.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 17)]
        public string CoordinateSystem { get; set; }

        /// <summary>
        /// Are coordinates set manually. This property is not used in the API anymore. Do not use.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 18)]
        public bool CoordinatesSetManually { get; set; }

        /// <summary>
        /// Localized list of phone charge descriptions.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 22)]
        public IReadOnlyList<VmOpenApiLanguageItem> PhoneChargeDescriptions { get; set; }

        /// <summary>
        /// List of service location channel charge types. Values: Charged, Free or Other.
        /// </summary>
        [JsonProperty(Order = 24)]
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        public List<string> ServiceChargeTypes { get; set; }

        /// <summary>
        /// List of service hours.
        /// </summary>
        [JsonProperty(Order = 25)]
        public new IReadOnlyList<VmOpenApiServiceHour> ServiceHours { get; set; } = new List<VmOpenApiServiceHour>();

        /// <summary>
        /// List of service location addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public new IList<VmOpenApiAddressWithType> Addresses { get; set; }

        /// <summary>
        /// List of support contacts for the service channel.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 35)]
        public IList<VmOpenApiSupport> SupportContacts { get; set; } = new List<VmOpenApiSupport>();

        [JsonIgnore]
        public override IList<VmOpenApiPhoneWithType> PhoneNumbers
        {
            get
            {
                return base.PhoneNumbers;
            }

            set
            {
                base.PhoneNumbers = value;
            }
        }

        [JsonIgnore]
        public override IList<VmOpenApiLanguageItem> SupportEmails
        {
            get
            {
                return base.SupportEmails;
            }

            set
            {
                base.SupportEmails = value;
            }
        }

        [JsonIgnore]
        public override IList<VmOpenApiPhone> SupportPhones
        {
            get
            {
                return base.SupportPhones;
            }

            set
            {
                base.SupportPhones = value;
            }
        }
    }
}
