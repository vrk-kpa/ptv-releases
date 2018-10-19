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
using Newtonsoft.Json;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;

namespace PTV.Domain.Model.Models.V2.Common.Connections
{
    /// <summary>
    /// ViewModel for connection information
    /// </summary>
    public class VmConnectionInput
    {
        /// <summary>
        /// Id of main entity for connection (GD, Service, Channel)
        /// </summary>
        public Guid MainEntityId { get; set; }
        /// <summary>
        /// Id of entity connected to main entity (Service, Channel)
        /// </summary>
        [JsonProperty("unificRootId")]
        public Guid ConnectedEntityId { get; set; }
        /// <summary>
        /// Gets or sets the connection basic information.
        /// </summary>
        /// <value>
        /// The basic information.
        /// </value>
        public VmConnectionBasicInformation BasicInformation { get; set; }
        /// <summary>
        /// Gets or sets the connection digital authorization information.
        /// </summary>
        /// <value>
        /// The digital authorization.
        /// </value>
        public VmConnectionDigitalAuthorizationInput DigitalAuthorization { get; set; }
        /// <summary>
        /// Gets or sets the contact information
        /// </summary>
        public VmContactDetails ContactDetails { get; set; }
        /// <summary>
        /// Gets or sets the opening hours
        /// </summary>
        public VmOpeningHours OpeningHours { get; set; }
        /// <summary>
        /// Order of connection
        /// </summary>
        [JsonIgnore]
        public int OrderNumber {get;set;}
    }
}
