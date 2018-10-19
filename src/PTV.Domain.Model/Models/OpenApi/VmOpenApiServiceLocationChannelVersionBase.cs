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
using PTV.Domain.Model.Models.OpenApi.V9;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service location channel - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceChannel" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceLocationChannelVersionBase" />
    public class VmOpenApiServiceLocationChannelVersionBase : VmOpenApiServiceChannel, IVmOpenApiServiceLocationChannelVersionBase
    {
        /// <summary>
        /// Service channel OID. Must match the regex @"^[A-Za-z0-9.-]*$".
        /// </summary>
        [JsonProperty(Order = 3)]
        [RegularExpression(@"^[A-Za-z0-9-.]*$")]
        public virtual string Oid { get; set; }

        /// <summary>
        /// List of Display name types (Name or AlternativeName) for each language version of ServiceChannelNames.
        /// </summary>
        [JsonProperty(Order = 7)]
        public virtual IList<VmOpenApiNameTypeByLanguage> DisplayNameType { get; set; } = new List<VmOpenApiNameTypeByLanguage>();

        /// <summary>
        /// List of phone numbers for the service channel. Includes also fax numbers.
        /// </summary>
        [JsonProperty(Order = 12)]
        public virtual IList<V4VmOpenApiPhoneWithType> PhoneNumbers { get; set; } = new List<V4VmOpenApiPhoneWithType>();

        /// <summary>
        /// List email addresses for the service channel.
        /// </summary>
        [JsonProperty(PropertyName = "emails", Order = 13)]
        public override IList<VmOpenApiLanguageItem> SupportEmails { get => base.SupportEmails; set => base.SupportEmails = value; }

        /// <summary>
        /// List of service location addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public IList<V9VmOpenApiAddressLocation> Addresses { get; set; } = new List<V9VmOpenApiAddressLocation>();

        /// <summary>
        /// List of support phone numbers for the service channel.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiPhone> SupportPhones { get; set; }

        #region Methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>
        /// version number
        /// </returns>
        public override int VersionNumber()
        {
            return 0;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>
        /// model of previous version
        /// </returns>
        public override IVmOpenApiServiceChannel PreviousVersion()
        {
            var vm = GetVersionBaseModel<V9VmOpenApiServiceLocationChannel>();
            return vm;
        }

        /// <summary>
        /// Gets the base version model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>base version model</returns>
        protected TModel GetVersionBaseModel<TModel>() where TModel : IVmOpenApiServiceLocationChannelVersionBase, new()
        {
            var vm = base.GetServiceChannelModel<TModel>();
            vm.Oid = this.Oid;
            vm.DisplayNameType = this.DisplayNameType;
            vm.PhoneNumbers = this.PhoneNumbers;
            vm.SupportEmails = this.SupportEmails;
            vm.Addresses = this.Addresses;
            vm.SupportPhones = this.SupportPhones;
            return vm;
        }
        #endregion
    }
}
