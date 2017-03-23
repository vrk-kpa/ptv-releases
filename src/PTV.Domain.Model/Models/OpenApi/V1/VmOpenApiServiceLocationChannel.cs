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

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V1;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Domain.Model.Models.OpenApi.V1
{
    /// <summary>
    /// OPEN API - View Model of service location channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceLocationChannelVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V1.IVmOpenApiServiceLocationChannel" />
    public class VmOpenApiServiceLocationChannel : VmOpenApiServiceLocationChannelVersionBase, IVmOpenApiServiceLocationChannel
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
        /// List of service channel web pages.
        /// </summary>
        [JsonProperty(Order = 19)]
        public new IList<VmOpenApiWebPage> WebPages { get; set; } = new List<VmOpenApiWebPage>();

        /// <summary>
        /// List of serviceareas. Used when location service channel is restricted by service area (ServiceAreaRestricted=true). List contains municipality names.
        /// </summary>
        [JsonProperty(Order = 20)]
        public new IList<string> ServiceAreas { get; set; }

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

        /// <summary>
        /// List of phone numbers for the service channel. Includes also fax numbers.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiPhoneWithType> PhoneNumbers
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

        /// <summary>
        /// List email addresses for the service channel.
        /// </summary>
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

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 1;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        /// <exception cref="System.NotSupportedException">No previous version for class VmOpenApiServiceLocationChannel!</exception>
        public override IVmOpenApiServiceChannel PreviousVersion()
        {
            throw new NotSupportedException("No previous version for class VmOpenApiServiceLocationChannel!");
        }
        #endregion
    }
}
