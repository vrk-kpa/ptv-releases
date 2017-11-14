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
using PTV.Domain.Model.Models.Interfaces.OpenApi.V7;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V7
{
    /// <summary>
    /// OPEN API - View Model of Service service channel - with external source and external types
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceServiceChannelBySourceBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V7.IV7VmOpenApiServiceServiceChannelBySourceAsti" />
    public class V7VmOpenApiServiceServiceChannelBySourceAsti : VmOpenApiServiceServiceChannelBySourceBase, IV7VmOpenApiServiceServiceChannelBySourceAsti
    {
        /// <summary>
        /// The extra types related to service and service channel connection.
        /// </summary>
        [JsonProperty(Order = 5)]
        public virtual IList<VmOpenApiExtraType> ExtraTypes { get; set; } = new List<VmOpenApiExtraType>();

        /// <summary>
        /// Indicates if all extra types should be deleted.
        /// </summary>
        [JsonProperty(Order = 12)]
        public virtual bool DeleteAllExtraTypes { get; set; }

        /// <summary>
        /// Indicates if connection between service and service channel is ASTI related.
        /// </summary>
        /// <value>
        /// The channel unique identifier.
        /// </value>
        [JsonIgnore]
        public virtual bool IsASTIConnection { get; set; }
    }
}
