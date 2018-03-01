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
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.TranslationOrder;

namespace PTV.Domain.Model.Models.V2.Channel
{
    /// <summary>
    /// Base model for service channels
    /// <seealso cref="PTV.Domain.Model.Models.VmEnumEntityStatusBase" />
    /// </summary>
    public class VmServiceChannel : VmChannelHeader
    {
        /// <summary>
        /// Gets or sets the short description.
        /// </summary>
        /// <value>
        /// The short description.
        /// </value>
        public Dictionary<string, string> ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>
        /// The organization identifier.
        /// </value>
        [JsonProperty("organization")]
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public Dictionary<string, string> Description { get; set; }

        /// <summary>
        /// Gets or sets the area information type identifier.
        /// </summary>
        /// <value>
        /// The area information type identifier.
        /// </value>
        public VmAreaInformation AreaInformation { get; set; }

        /// <summary>
        /// Gets or sets the connection type ID.
        /// </summary>
        /// <value>
        /// The connection type ID.
        /// </value>
        [JsonProperty("connectionType")]
        public Guid? ConnectionTypeId { get; set; }
        //
        //        /// <summary>
        //        /// Gets o sets channel is set as common for published version
        //        /// </summary>
        //        public bool IsPublishedCommonType { get; set; }

        /// <summary>
        /// phone number of channel
        /// </summary>
        public Dictionary<string, List<VmPhone>> PhoneNumbers { get; set; }

        /// <summary>
        /// email of the channel
        /// </summary>
        public Dictionary<string, List<VmEmailData>> Emails { get; set; }

        /// <summary>
        /// connections of the channel
        /// </summary>
        public List<VmChannelConnectionOutput> Connections { get; set; }

        /// <summary>
        /// List of connected IDs of unific root of services
        /// </summary>
        [JsonIgnore]
        public List<VmConnectionLightBasics> ConnectedServicesUnific { get; set; }

        /// <summary>
        /// Setting of translation order availability
        /// </summary>
        public Dictionary<string, VmTranslationOrderAvailability> TranslationAvailability { get; set; }
    }
}