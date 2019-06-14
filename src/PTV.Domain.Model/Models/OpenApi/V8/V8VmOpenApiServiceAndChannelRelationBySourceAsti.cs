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
using PTV.Domain.Model.Models.OpenApi.V9;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V8
{
    /// <summary>
    /// OPEN API V8 - View Model of service and channel relation IN by source (PUT).
    /// </summary>
    public class V8VmOpenApiServiceAndChannelRelationBySourceAsti : IOpenApiAstiServiceRelationBySource<V8VmOpenApiServiceServiceChannelBySourceAsti, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>
    {
        /// <summary>
        /// Set to true to delete all existing relations between defined service and service channels (the ChannelRelations collection for this object should be empty collection when this option is used).
        /// </summary>
        public bool DeleteAllChannelRelations { get; set; }

        /// <summary>
        /// Gets or sets the channel relations.
        /// </summary>
        /// <value>
        /// The channel relations.
        /// </value>
        public List<V8VmOpenApiServiceServiceChannelBySourceAsti> ChannelRelations { get; set; } = new List<V8VmOpenApiServiceServiceChannelBySourceAsti>();
        
        /// <summary>
        /// Converts the service connection model for latest one (the one used in translatores for example).
        /// </summary>
        /// <returns></returns>
        public V9VmOpenApiServiceAndChannelRelationBySourceAsti ConvertToLatestVersion()
        {
            var model = new V9VmOpenApiServiceAndChannelRelationBySourceAsti
            {
                DeleteAllChannelRelations = this.DeleteAllChannelRelations
            };
            this.ChannelRelations.ForEach(r => model.ChannelRelations.Add(r.ConvertToLatestVersion()));
            return model;
        }
    }
}
