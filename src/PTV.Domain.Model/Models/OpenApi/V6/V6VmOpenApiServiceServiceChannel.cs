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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V6
{
    /// <summary>
    /// OPEN API V6 - View Model of service channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceServiceChannelVersionBase" />
    public class V6VmOpenApiServiceServiceChannel : VmOpenApiServiceServiceChannelVersionBase
    {
        /// <summary>
        /// The extra types related to service and service channel connection.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiExtraType> ExtraTypes { get => base.ExtraTypes; set => base.ExtraTypes = value; }

        /// <summary>
        /// Converts to version 4.
        /// </summary>
        /// <returns>model converted to version 4</returns>
        public V4VmOpenApiServiceServiceChannel ConvertToVersion4()
        {
            var vm = base.GetVersionBaseModel<V4VmOpenApiServiceServiceChannel>();
            vm.ServiceChannelId = ServiceChannel != null && ServiceChannel.Id.IsAssigned() ? ServiceChannel.Id.Value.ToString() : string.Empty;
            return vm;
        }
    }
}
