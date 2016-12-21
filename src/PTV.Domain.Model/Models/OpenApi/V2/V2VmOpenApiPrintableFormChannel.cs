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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    public class V2VmOpenApiPrintableFormChannel : VmOpenApiServiceChannel, IV2VmOpenApiPrintableFormChannel
    {
        /// <summary>
        /// Form identifier.
        /// </summary>
        [JsonProperty(Order = 10)]
        public string FormIdentifier { get; set; }
        /// <summary>
        /// Form receiver.
        /// </summary>
        [JsonProperty(Order = 11)]
        public string FormReceiver { get; set; }
        /// <summary>
        /// Form delivery address.
        /// </summary>
        [JsonProperty(Order = 12)]
        public V2VmOpenApiAddressWithCoordinates DeliveryAddress { get; set; }
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
    }
}
