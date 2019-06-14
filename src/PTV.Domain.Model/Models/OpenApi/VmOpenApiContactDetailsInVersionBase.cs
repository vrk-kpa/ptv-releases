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
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework.Attributes;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of contact details (POST/PUT) - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiContactDetailsInVersionBase" />
    public class VmOpenApiContactDetailsInVersionBase : IVmOpenApiContactDetailsInVersionBase
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
        public virtual IList<V4VmOpenApiPhone> PhoneNumbers { get; set; } = new List<V4VmOpenApiPhone>();

        /// <summary>
        /// List of connection related fax numbers numbers.
        /// </summary>
        [JsonProperty(Order = 4)]
        public virtual IList<V4VmOpenApiPhoneSimple> FaxNumbers { get; set; } = new List<V4VmOpenApiPhoneSimple>();

        /// <summary>
        /// List of connection related web pages.
        /// </summary>
        [JsonProperty(Order = 5)]
        public virtual IList<V9VmOpenApiWebPage> WebPages { get; set; } = new List<V9VmOpenApiWebPage>();

        /// <summary>
        /// List of service location addresses.
        /// </summary>
        [JsonProperty(Order = 6)]
        public virtual IList<V7VmOpenApiAddressContactIn> Addresses { get; set; } = new List<V7VmOpenApiAddressContactIn>();

        /// <summary>
        /// Converts into latest in base model
        /// </summary>
        /// <returns></returns>
        public virtual V9VmOpenApiContactDetailsInBase ConvertToInBaseModel()
        {
            return new V9VmOpenApiContactDetailsInBase
            {
                Emails = this.Emails,
                PhoneNumbers = this.PhoneNumbers,
                FaxNumbers = this.FaxNumbers,
                WebPages = this.WebPages,
                Addresses = this.Addresses
            };
        }

        /// <summary>
        /// Converts into latest in model
        /// </summary>
        /// <returns></returns>
        public virtual V9VmOpenApiContactDetailsIn ConvertToInModel()
        {
            return new V9VmOpenApiContactDetailsIn
            {
                Emails = this.Emails,
                PhoneNumbers = this.PhoneNumbers,
                FaxNumbers = this.FaxNumbers,
                WebPages = this.WebPages,
                Addresses = this.Addresses,
            };
        }
    }
}
