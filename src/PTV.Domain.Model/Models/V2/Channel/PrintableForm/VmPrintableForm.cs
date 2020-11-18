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
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Domain.Model.Models.V2.Channel.PrintableForm
{
    /// <summary>
    ///
    /// </summary>
    public class VmPrintableForm : VmServiceChannel, Interfaces.V2.IAttachments, IVmChannel
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public VmPrintableForm()
        {
            DeliveryAddresses = new List<VmAddressSimple>();
        }

        /// <summary>
        /// Gets or sets the form identifier.
        /// </summary>
        /// <value>
        /// The form identifier.
        /// </value>
        public Dictionary<string, string> FormIdentifier { get; set; }
        /// <summary>
        /// Gets or sets the delivery address.
        /// </summary>
        /// <value>
        /// The delivery address.
        /// </value>
        public List<VmAddressSimple> DeliveryAddresses { get; set; }
        /// <summary>
        /// Gets or sets the URL attachments.
        /// </summary>
        /// <value>
        /// The URL attachments.
        /// </value>
        public Dictionary<string, List<VmChannelAttachment>> Attachments { get; set; }
        /// <summary>
        /// Gets or sets the form files.
        /// </summary>
        /// <value>
        /// The web form files.
        /// </value>
        public Dictionary<string, List<VmWebPage>> FormFiles { get; set; }
        /// <summary>
        ///
        /// </summary>
        [JsonIgnore]
        public Guid PrintableFormChannelId { get; set; }
    }
}
