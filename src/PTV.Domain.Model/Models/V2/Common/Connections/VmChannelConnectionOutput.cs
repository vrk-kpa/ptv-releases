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
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Domain.Model.Models.V2.Service;

namespace PTV.Domain.Model.Models.V2.Common.Connections
{
    /// <summary>
    /// 
    /// </summary>
    public class VmChannelConnectionOutput : VmConnectableService
    {
        /// <summary>
        /// constructor
        /// </summary>
        public VmChannelConnectionOutput()
        {
            BasicInformation = new VmConnectionBasicInformation();
            DigitalAuthorization = new VmConnectionDigitalAuthorizationOutput();
            AstiDetails = new VmAstiDetails();
            OpeningHours = new VmOpeningHours();
        }
        /// <summary>
        /// Gets or sets the ConnectionId.
        /// </summary>
        /// <value>
        /// The ConnectionId.
        /// </value>
        public string ConnectionId { get; set; }
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
        public VmConnectionDigitalAuthorizationOutput DigitalAuthorization { get; set; }
        /// <summary>
        /// Gets or sets the contact information
        /// </summary>
        public VmContactDetails ContactDetails { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public VmAstiDetails AstiDetails { get; set; }
        /// <summary>
        /// Gets or sets Opening hours 
        /// </summary>
        public VmOpeningHours OpeningHours { get; set; }
        /// <summary>
        /// Order of the services
        /// </summary>
        [JsonIgnore]
        public int? ServiceOrderNumber { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VmAstiDetails
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsASTIConnection { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<VmAstiTypeInfo> AstiTypeInfos { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid CategoryOrganizationId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VmAstiTypeInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid AstiTypeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> AstiDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Name { get; set; }
    }
}
