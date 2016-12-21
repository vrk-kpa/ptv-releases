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
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    public class V2VmOpenApiPrintableFormChannelInBase : VmOpenApiServiceChannelIn, IV2VmOpenApiPrintableFormChannelInBase
    {
        /// <summary>
        /// Form identifier.
        /// </summary>
        [JsonProperty(Order = 10)]
        [MaxLength(100)]
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
        public virtual V2VmOpenApiAddress DeliveryAddress { get; set; }

        /// <summary>
        /// List of localized channel urls.
        /// </summary>
        [JsonProperty(Order = 13)]
        [ListWithEnum(typeof(PrintableFormChannelUrlTypeEnum), "Type")]
        [ListWithUrl("Value")]
        public virtual IReadOnlyList<VmOpenApiLocalizedListItem> ChannelUrls { get; set; }

        /// <summary>
        /// List of attachments.
        /// </summary>
        [JsonProperty(Order = 15)]
        public IReadOnlyList<VmOpenApiAttachment> Attachments { get; set; } = new List<VmOpenApiAttachment>();

        /// <summary>
        /// Set to true to delete all existing delivery address for the service channel. The DeliveryAddress should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 16)]
        public virtual bool DeleteDeliveryAddress { get; set; }

        /// <summary>
        /// Set to true to delete all existing channel urls for the service channel. The ChannelUrls collection should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 17)]
        public virtual bool DeleteAllChannelUrls { get; set; }

        /// <summary>
        /// Set to true to delete all existing attachments for the service channel. The Attachments collection should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 19)]
        public virtual bool DeleteAllAttachments { get; set; }

        [JsonIgnore]
        public override IReadOnlyList<string> Languages
        {
            get
            {
                return base.Languages;
            }

            set
            {
                base.Languages = value;
            }
        }

        [JsonIgnore]
        public override IList<VmOpenApiWebPage> WebPages
        {
            get
            {
                return base.WebPages;
            }

            set
            {
                base.WebPages = value;
            }
        }

        [JsonIgnore]
        public override IReadOnlyList<V2VmOpenApiServiceHour> ServiceHours
        {
            get
            {
                return base.ServiceHours;
            }

            set
            {
                base.ServiceHours = value;
            }
        }

        [JsonIgnore]
        public override bool DeleteAllWebPages
        {
            get
            {
                return base.DeleteAllWebPages;
            }

            set
            {
                base.DeleteAllWebPages = value;
            }
        }

        [JsonIgnore]
        public override bool DeleteAllServiceHours
        {
            get
            {
                return base.DeleteAllServiceHours;
            }

            set
            {
                base.DeleteAllServiceHours = value;
            }
        }
    }
}
