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
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Extensions;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V8
{
    /// <summary>
    /// OPEN API - View Model of Service channel service V8
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceChannelServiceVersionBase" />
    public class V8VmOpenApiServiceChannelService : VmOpenApiServiceChannelServiceVersionBase
    {
        /// <summary>
        /// The extra types related to service and service channel connection.
        /// </summary>
        [JsonProperty(Order = 5)]
        public virtual new IList<VmOpenApiExtraType> ExtraTypes { get; set; } = new List<VmOpenApiExtraType>();

        /// <summary>
        /// Contact details for connection.
        /// </summary>
        [JsonProperty(Order = 7)]
        public virtual new V8VmOpenApiContactDetails ContactDetails { get; set; }

        #region methods

        /// <summary>
        /// Converts to version 7.
        /// </summary>
        /// <returns></returns>
        public VmOpenApiServiceChannelService ConvertToVersion7()
        {
            var vm = base.GetVersionBaseModel<VmOpenApiServiceChannelService>();
            vm.ServiceChargeType = string.IsNullOrEmpty(vm.ServiceChargeType) ? null : vm.ServiceChargeType.GetEnumValueByOpenApiEnumValue<ServiceChargeTypeEnum>();
            vm.ExtraTypes = this.ExtraTypes;
            vm.ContactDetails = this.ContactDetails != null ? this.ContactDetails.ConvertToVersion7() : null;
            this.ServiceHours.ForEach(h => vm.ServiceHours.Add(h.ConvertToVersion7()));
            return vm;
        }

        #endregion
    }
}
