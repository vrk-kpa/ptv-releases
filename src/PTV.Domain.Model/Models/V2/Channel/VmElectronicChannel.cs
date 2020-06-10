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
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models.V2.Channel
{

    /// <summary>
    ///
    /// </summary>
    public class VmElectronicChannel : VmServiceChannel, Interfaces.V2.IAttachments, IOpeningHours, IVmChannel, IAccessibilityClassifications
    {
        /// <summary>
        /// web page of the channel
        /// </summary>
        [JsonProperty("urlAddress")]
        public Dictionary<string, string> WebPage { get; set; }

        /// <summary>
        /// list of url attachments
        /// </summary>
        public Dictionary<string, List<VmChannelAttachment>> Attachments { get; set; }

        /// <summary>
        /// Indicates wheter channel supports online authentication
        /// </summary>
        [JsonProperty("onlineAuthentication")]
        public bool? IsOnLineAuthentication { get; set; }

        /// <summary>
        /// Indicates whether channel supports online sign
        /// </summary>
        public bool IsOnlineSign { get; set; }

        /// <summary>
        /// Count sign number
        /// </summary>
        public int? SignatureCount { get; set; }

        /// <summary>
        /// Gets or sets the opening hours.
        /// </summary>
        /// <value>
        /// The opening hours.
        /// </value>
        public VmOpeningHours OpeningHours { get; set; }

        /// <summary>
        /// Gets or sets the Accessibility classifications.
        /// </summary>
        /// <value>
        /// The Accessibility classifications.
        /// </value>
        public Dictionary<string, VmAccessibilityClassification> AccessibilityClassifications { get; set; }
    }
}
