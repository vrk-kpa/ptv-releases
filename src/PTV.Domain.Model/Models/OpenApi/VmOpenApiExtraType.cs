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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// 
    /// </summary>
    public class VmOpenApiExtraType : IVmOpenApiExtraType
    {
        /// <summary>
        /// Type of the area (Asti).
        /// </summary>
        [ValidEnum(typeof(ExtraTypeEnum))]
        public string Type { get; set; }

        /// <summary>
        /// Code of the extra type. 
        /// In Asti case the code can be DocumentReceived, GuidanceToOnlineSelfService, LostAndFound, OnlineSelfServicePoint, 
        /// OnsiteGuidance, OnsiteGuidanceByServiceAuthor or RemoteGuidance
        /// </summary>
        [ValidEnum(typeof(ExtraSubTypeEnum))]
        public string Code { get; set; }

        /// <summary>
        /// List of localized extra type descriptions.
        /// </summary>
        [ListValueNotEmpty("Value")]
        public IReadOnlyList<VmOpenApiLanguageItem> Description { get; set; } = new List<VmOpenApiLanguageItem>();

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
    }
}
