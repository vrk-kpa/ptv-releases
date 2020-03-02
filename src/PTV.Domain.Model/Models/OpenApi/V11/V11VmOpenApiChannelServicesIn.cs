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
using PTV.Domain.Model.Models.OpenApi.V9;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V11
{
    /// <summary>
    /// OPEN API V11 - View Model of service and channel relation IN (PUT).
    /// </summary>
    public class V11VmOpenApiChannelServicesIn : IOpenApiAstiChannelRelation<V11VmOpenApiServiceChannelServiceInBase, V9VmOpenApiContactDetailsInBase, V11VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>
    {
        /// <summary>
        /// Identifier for channel.
        /// </summary>
        [JsonIgnore]
        public Guid? ChannelId { get; set; }

        /// <summary>
        /// Set to true to delete all existing relations between defined service channel and services (the ServiceRelations collection for this object should be empty collection when this option is used).
        /// </summary>
        public bool DeleteAllServiceRelations { get; set; }

        /// <summary>
        /// Gets or sets the service relations.
        /// </summary>
        /// <value>
        /// The channel relations.
        /// </value>
        public List<V11VmOpenApiServiceChannelServiceInBase> ServiceRelations { get; set; } = new List<V11VmOpenApiServiceChannelServiceInBase>();

        /// <summary>
        /// Converts the channel connection model for latest one (the one used in translatores for example).
        /// </summary>
        /// <returns></returns>
        public V11VmOpenApiChannelServicesIn ConvertToLatestVersion()
        {
            return this;
        }
    }
}
