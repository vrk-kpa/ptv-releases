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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of Service channel - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceServiceChannelBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceChannelServiceVersionBase" />
    public class VmOpenApiServiceChannelServiceVersionBase : VmOpenApiServiceServiceChannelBase, IVmOpenApiServiceChannelServiceVersionBase
    {
        /// <summary>
        /// Service channel identifier and name.
        /// </summary>
        public virtual VmOpenApiItem Service { get; set; } = new VmOpenApiItem();

        // PTV-4184
        /// <summary>
        /// The extra types related to service and service channel connection.
        /// </summary>
        [JsonProperty(Order = 5)]
        public virtual IList<V9VmOpenApiExtraType> ExtraTypes { get; set; } = new List<V9VmOpenApiExtraType>();

        /// <summary>
        /// Contact details for connection.
        /// </summary>
        [JsonProperty(Order = 7)]
        public virtual V9VmOpenApiContactDetails ContactDetails { get; set; }

        /// <summary>
        /// List of digital authorizations related to the service.
        /// </summary>
        [JsonProperty(Order = 10)]
        public virtual IReadOnlyList<V4VmOpenApiFintoItem> DigitalAuthorizations { get; set; } = new List<V4VmOpenApiFintoItem>();

        /// <summary>
        /// Date when connection was modified/created (UTC).
        /// </summary>
        [JsonProperty(Order = 11)]
        public virtual DateTime Modified { get; set; }

        #region methods

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public TModel GetVersionBaseModel<TModel>() where TModel : IVmOpenApiServiceChannelServiceVersionBase, new()
        {
            var model = base.GetBaseModel<TModel>();
            model.Service = this.Service;
            model.ExtraTypes = this.ExtraTypes;
            model.ContactDetails = this.ContactDetails;
            model.DigitalAuthorizations = this.DigitalAuthorizations;
            model.Modified = this.Modified;
            return model;
        }

        #endregion
    }
}
