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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of printable form channel step 1
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmServiceChannel" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmPrintableFormChannelStep1" />
    public class VmPrintableFormChannelStep1 : VmServiceChannel, IVmPrintableFormChannelStep1
    {
        /// <summary>
        /// Gets or sets the printable form channel identifier.
        /// </summary>
        /// <value>
        /// The printable form channel channel identifier.
        /// </value>
        [JsonIgnore]
        public Guid? PrintableFormChannelChannelId { get; set; }        
        /// <summary>
        /// Gets or sets the form identifier.
        /// </summary>
        /// <value>
        /// The form identifier.
        /// </value>
        public Dictionary<string, string> FormIdentifier { get; set; }
        /// <summary>
        /// Gets or sets the form receiver.
        /// </summary>
        /// <value>
        /// The form receiver.
        /// </value>
        public Dictionary<string, string> FormReceiver { get; set; }

        /// <summary>
        /// Gets or sets the delivery address.
        /// </summary>
        /// <value>
        /// The delivery address.
        /// </value>
        public VmAddressSimple DeliveryAddress { get; set; }
        /// <summary>
        /// Gets or sets the URL attachments.
        /// </summary>
        /// <value>
        /// The URL attachments.
        /// </value>
        public List<VmChannelAttachment> UrlAttachments { get; set; }
        /// <summary>
        /// Gets or sets the web pages.
        /// </summary>
        /// <value>
        /// The web pages.
        /// </value>
        public List<VmWebPage> WebPages { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VmPrintableFormChannelStep1"/> class.
        /// </summary>
        public VmPrintableFormChannelStep1()
        {
            PhoneNumbers = new List<VmPhone>();
            Emails = new List<VmEmailData>();
            //FormIdentifier = string.Empty;
            //FormReceiver = string.Empty;
            UrlAttachments = new List<VmChannelAttachment>();
            WebPages = new List<VmWebPage>();
            BusinessRegions = new List<Guid>();
            HospitalRegions = new List<Guid>();
            Provinces = new List<Guid>();
            Municipalities = new List<Guid>();
        }

        /// <summary>
        /// <see cref="IVmLocalized.Language"/>
        /// </summary>
        public LanguageCode Language { get; set; }
        /// <summary>
        /// Gets or sets the list of HospitalRegions.
        /// </summary>
        /// <value>
        /// The list of HospitalRegions.
        /// </value>
        public List<Guid> HospitalRegions { get; set; }
        /// <summary>
        /// Gets or sets the list of Municipality.
        /// </summary>
        /// <value>
        /// The list of Municipality.
        /// </value>
        public List<Guid> Municipalities { get; set; }
        /// <summary>
        /// Gets or sets the list of Province.
        /// </summary>
        /// <value>
        /// The list of Province.
        /// </value>
        public List<Guid> Provinces { get; set; }
        /// <summary>
        /// Gets or sets the list of BusinessRegions.
        /// </summary>
        /// <value>
        /// The list of BusinessRegions.
        /// </value>
        public List<Guid> BusinessRegions { get; set; }
        /// <summary>
        /// Gets or sets the phone numbers.
        /// </summary>
        /// <value>
        /// The phone numbers.
        /// </value>
        public List<VmPhone> PhoneNumbers { get; set; }
        /// <summary>
        /// Gets or sets the emails.
        /// </summary>
        /// <value>
        /// The emails.
        /// </value>
        public List<VmEmailData> Emails { get; set; }
    }
}
