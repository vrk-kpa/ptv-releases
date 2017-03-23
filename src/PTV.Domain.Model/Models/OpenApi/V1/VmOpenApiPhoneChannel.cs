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
    /// OPEN API - View Model of phone channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiPhoneChannelVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V1.IVmOpenApiPhoneChannel" />
    public class VmOpenApiPhoneChannel : VmOpenApiPhoneChannelVersionBase, IVmOpenApiPhoneChannel
    {
        /// <summary>
        /// Type of phone channel. Values: Phone, Sms or Fax.
        /// </summary>
        [JsonProperty(Order = 10)]
        public string PhoneType { get; set; }

        /// <summary>
        /// List of service charge types. Values: Charged, Free or Other.
        /// </summary>
        [JsonProperty(Order = 11)]
        public List<string> ServiceChargeTypes { get; set; }

        /// <summary>
        /// Localized list of phone numbers.
        /// </summary>
        [JsonProperty(Order = 12)]
        public new List<VmOpenApiLanguageItem> PhoneNumbers { get; set; }

        /// <summary>
        /// Localized list of phone charge descriptions.
        /// </summary>
        [JsonProperty(Order = 14)]
        public List<VmOpenApiLanguageItem> PhoneChargeDescriptions { get; set; }

        /// <summary>
        /// List of service channel web pages.
        /// </summary>
        [JsonProperty(Order = 19)]
        public new IList<VmOpenApiWebPage> WebPages { get; set; } = new List<VmOpenApiWebPage>();

        /// <summary>
        /// List of service hours.
        /// </summary>
        [JsonProperty(Order = 25)]
        public new IReadOnlyList<VmOpenApiServiceHour> ServiceHours { get; set; } = new List<VmOpenApiServiceHour>();

        /// <summary>
        /// List of support contacts for the service channel.
        /// </summary>
        [JsonProperty(Order = 35)]
        public IList<VmOpenApiSupport> SupportContacts { get; set; } = new List<VmOpenApiSupport>();

        /// <summary>
        /// List of support email addresses for the service channel.
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

        /// <summary>
        /// Gets or sets the support phones.
        /// </summary>
        /// <value>
        /// The support phones.
        /// </value>
        [JsonIgnore]
        public override IList<V4VmOpenApiPhone> SupportPhones
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
        /// <exception cref="System.NotSupportedException">No previous version for class VmOpenApiPhoneChannel!</exception>
        public override IVmOpenApiServiceChannel PreviousVersion()
        {
            throw new NotSupportedException("No previous version for class VmOpenApiPhoneChannel!");
        }
    }
}
