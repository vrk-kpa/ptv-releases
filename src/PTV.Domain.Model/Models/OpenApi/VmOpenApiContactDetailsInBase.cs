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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Attributes;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of contact details (PUT) - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiContactDetailsInBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiContactDetailsInBase" />
    public class VmOpenApiContactDetailsInBase : VmOpenApiContactDetailsInVersionBase, IVmOpenApiContactDetailsInBase
    {
        /// <summary>
        /// List of connection related phone numbers.
        /// </summary>
        [JsonProperty(Order = 3, PropertyName = "phones")]
        [ListWithEnum(typeof(ServiceChargeTypeEnum), "ServiceChargeType")]
        public override IList<V4VmOpenApiPhone> PhoneNumbers { get => base.PhoneNumbers; set => base.PhoneNumbers = value; }

        /// <summary>
        /// List of connection related fax numbers numbers.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiPhoneSimple> FaxNumbers { get => base.FaxNumbers; set => base.FaxNumbers = value; }

        /// <summary>
        /// List of connection related web pages.
        /// </summary>
        [JsonProperty(Order = 5)]
        [LocalizedListPropertyDuplicityForbidden("OrderNumber")]
        public virtual new IList<VmOpenApiWebPageWithOrderNumber> WebPages { get; set; } = new List<VmOpenApiWebPageWithOrderNumber>();

        /// <summary>
        /// Gets or sets a value indicating whether all emails should be delted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all email should be deleted; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(Order = 10)]
        public virtual bool DeleteAllEmails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all phones should be delted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all phones should be deleted; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(Order = 11)]
        public virtual bool DeleteAllPhones { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all web pages should be delted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all web pages should be deleted; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(Order = 12)]
        public virtual bool DeleteAllWebPages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all addresses should be delted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all addresses should be deleted; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(Order = 13)]
        public virtual bool DeleteAllAddresses { get; set; }

        /// <summary>
        /// Converts into latest in base model
        /// </summary>
        /// <returns></returns>
        public override V9VmOpenApiContactDetailsInBase ConvertToInBaseModel()
        {
            var model = base.ConvertToInBaseModel();
            this.WebPages.ForEach(w => model.WebPages.Add(w.ConvertToInBaseModel()));
            model.DeleteAllAddresses = this.DeleteAllAddresses;
            model.DeleteAllEmails = this.DeleteAllEmails;
            model.DeleteAllPhones = this.DeleteAllPhones;
            model.DeleteAllWebPages = this.DeleteAllWebPages;
            return model;
        }
    }
}
