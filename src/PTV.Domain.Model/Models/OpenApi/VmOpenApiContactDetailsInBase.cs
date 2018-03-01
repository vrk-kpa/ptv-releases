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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework.Attributes;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service location channel - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiContactDetailsInBase" />
    public class VmOpenApiContactDetailsInBase : IVmOpenApiContactDetailsInBase
    {
        /// <summary>
        /// List of connection related email addresses.
        /// </summary>
        [JsonProperty(Order = 2)]
        [EmailAddressList("Value")]
        public virtual IList<VmOpenApiLanguageItem> Emails { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of connection related phone numbers.
        /// </summary>
        [JsonProperty(Order = 3)]
        public virtual IList<V4VmOpenApiPhone> Phones { get; set; } = new List<V4VmOpenApiPhone>();

        /// <summary>
        /// List of connection related web pages.
        /// </summary>
        [JsonProperty(Order = 4)]
        [LocalizedListPropertyDuplicityForbidden("OrderNumber")]
        public virtual IList<VmOpenApiWebPageWithOrderNumber> WebPages { get; set; } = new List<VmOpenApiWebPageWithOrderNumber>();

        /// <summary>
        /// List of service location addresses.
        /// </summary>
        [JsonProperty(Order = 5)]
        public virtual IList<V7VmOpenApiAddressIn> Addresses { get; set; } = new List<V7VmOpenApiAddressIn>();

        /// <summary>
        /// Gets or sets a value indicating whether all emails should be delted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all email should be deleted; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(Order = 6)]
        public virtual bool DeleteAllEmails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all phones should be delted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all phones should be deleted; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(Order = 7)]
        public virtual bool DeleteAllPhones { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all web pages should be delted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all web pages should be deleted; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(Order = 8)]
        public virtual bool DeleteAllWebPages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all addresses should be delted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all addresses should be deleted; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(Order = 9)]
        public virtual bool DeleteAllAddresses { get; set; }
    }
}
