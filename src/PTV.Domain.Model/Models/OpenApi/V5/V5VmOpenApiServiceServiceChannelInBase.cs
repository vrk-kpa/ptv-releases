/**
 * The MIT License
 * Copyright(c) 2016 Population Register Centre(VRK)
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
using PTV.Domain.Model.Models.OpenApi.V7;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V5
{
    /// <summary>
    /// OPEN API V5 - View Model of service service channel IN.
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceServiceChannelInVersionBase" />
    public class V5VmOpenApiServiceServiceChannelInBase : VmOpenApiServiceServiceChannelInVersionBase
    {
        /// <summary>
        /// PTV service channel identifier.
        /// </summary>
        [Required]
        public override string ServiceChannelId { get => base.ServiceChannelId; set => base.ServiceChannelId = value; }

        /// <summary>
        /// PTV service identifier.
        /// </summary>
        [JsonIgnore]
        public override string ServiceId { get => base.ServiceId; set => base.ServiceId = value; }

        /// <summary>
        /// The extra types related to service and service channel connection.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiExtraType> ExtraTypes { get => base.ExtraTypes; set => base.ExtraTypes = value; }

        /// <summary>
        /// Converts model into version 7.
        /// </summary>
        /// <returns></returns>
        public V7VmOpenApiServiceServiceChannelInBase ConvertToVersion7()
        {
            return base.GetBase<V7VmOpenApiServiceServiceChannelInBase>();
        }
    }
}
