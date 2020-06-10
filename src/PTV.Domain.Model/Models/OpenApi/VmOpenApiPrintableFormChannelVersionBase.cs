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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V8;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V11;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of printable form channel - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceChannel" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiPrintableFormChannelVersionBase" />
    public class VmOpenApiPrintableFormChannelVersionBase : VmOpenApiServiceChannel, IVmOpenApiPrintableFormChannelVersionBase
    {
        /// <summary>
        /// List of localized form identifier. One per language.
        /// </summary>
        [JsonProperty(Order = 10)]
        public IList<VmOpenApiLanguageItem> FormIdentifier { get; set; }
        /// <summary>
        /// Form delivery addresses.
        /// </summary>
        [JsonProperty(Order = 12)]
        public virtual IList<V8VmOpenApiAddressDelivery> DeliveryAddresses { get; set; }
        /// <summary>
        /// List of localized channel urls.
        /// </summary>
        [JsonProperty(Order = 13)]
        public IReadOnlyList<VmOpenApiLocalizedListItem> ChannelUrls { get; set; }
        /// <summary>
        /// List of attachments.
        /// </summary>
        [JsonProperty(Order = 15)]
        public IReadOnlyList<VmOpenApiAttachmentWithType> Attachments { get; set; }

        #region Methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 0;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceChannel PreviousVersion()
        {
            return GetVersionBaseModel<V11VmOpenApiPrintableFormChannel>();
        }

        /// <summary>
        /// Gets the version base model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>base version model</returns>
        protected TModel GetVersionBaseModel<TModel>() where TModel : IVmOpenApiPrintableFormChannelVersionBase, new()
        {
            var vm = base.GetServiceChannelModel<TModel>();
            vm.FormIdentifier = this.FormIdentifier;
            vm.DeliveryAddresses = this.DeliveryAddresses;
            vm.ChannelUrls = this.ChannelUrls;
            vm.Attachments = this.Attachments;
            vm.OntologyTerms = this.OntologyTerms;
            return vm;
        }
        #endregion
    }
}
