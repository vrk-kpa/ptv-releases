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
using PTV.Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API base models - View Model of service and service channel connection IN (POST/PUT).
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceServiceChannelVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceServiceChannelInVersionBase" />
    public class VmOpenApiServiceServiceChannelInVersionBase : VmOpenApiServiceServiceChannelBase, IVmOpenApiServiceServiceChannelInVersionBase
    {
        /// <summary>
        /// PTV service identifier.
        /// </summary>
        [ValidGuid]
        [JsonProperty(Order = 1)]
        public virtual string ServiceId { get; set; }

        /// <summary>
        /// PTV service channel identifier.
        /// </summary>
        [ValidGuid]
        [JsonProperty(Order = 2)]
        public virtual string ServiceChannelId { get; set; }

        /// <summary>
        /// Gets or sets the service unique identifier.
        /// </summary>
        /// <value>
        /// The service unique identifier.
        /// </value>
        [JsonIgnore]
        public Guid ServiceGuid { get; set; }

        /// <summary>
        /// Gets or sets the channel unique identifier.
        /// </summary>
        /// <value>
        /// The channel unique identifier.
        /// </value>
        [JsonIgnore]
        public Guid ChannelGuid { get; set; }

        #region methods

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>
        /// view model of base
        /// </returns>
        /// <exception cref="System.NotImplementedException">VmOpenApiGeneralDescriptionInVersionBase does not have next version available!</exception>
        public virtual VmOpenApiServiceServiceChannelInVersionBase VersionBase()
        {
            throw new NotImplementedException("VmOpenApiServiceServiceChannelInVersionBase does not have version base available!");
        }

        /// <summary>
        /// Gets the in base version model.
        /// </summary>
        /// <returns>in base version model</returns>
        protected VmOpenApiServiceServiceChannelInVersionBase GetInVersionBaseModel()
        {
            return GetBase<VmOpenApiServiceServiceChannelInVersionBase>();
        }

        /// <summary>
        /// Gets the base model.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        protected TModel GetBase<TModel>() where TModel : IVmOpenApiServiceServiceChannelInVersionBase, new()
        {
            var vm = base.GetBaseModel<TModel>();
            vm.ServiceId = this.ServiceId;
            vm.ServiceChannelId = this.ServiceChannelId;
            vm.ServiceGuid = this.ServiceGuid;
            vm.ChannelGuid = this.ChannelGuid;
            return vm;
        }
        #endregion
    }
}
