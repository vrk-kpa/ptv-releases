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
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of Service channel - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceServiceChannelBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceServiceChannelVersionBase" />
    public abstract class VmOpenApiServiceServiceChannelVersionBase : VmOpenApiServiceServiceChannelBase, IVmOpenApiServiceServiceChannelVersionBase
    {
        /// <summary>
        /// Service channel identifier and name.
        /// </summary>
        public virtual VmOpenApiItem ServiceChannel { get; set; } = new VmOpenApiItem();

        /// <summary>
        /// Contact details for connection.
        /// </summary>
        [JsonProperty(Order = 7)]
        public virtual VmOpenApiContactDetails ContactDetails { get; set; }

        /// <summary>
        /// List of digital authorizations related to the service.
        /// </summary>
        [JsonProperty(Order = 10)]
        public virtual IReadOnlyList<V4VmOpenApiFintoItem> DigitalAuthorizations { get; set; } = new List<V4VmOpenApiFintoItem>();

        /// <summary>
        /// Gets the version base model.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        protected TModel GetVersionBaseModel<TModel>() where TModel : IVmOpenApiServiceServiceChannelVersionBase, new()
        {
            var vm = base.GetBaseModel<TModel>();
            vm.ServiceChannel = this.ServiceChannel;
            vm.ContactDetails = this.ContactDetails;
            vm.DigitalAuthorizations = this.DigitalAuthorizations;
            return vm;
        }
    }
}
