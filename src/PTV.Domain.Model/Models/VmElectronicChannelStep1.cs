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
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of electronic step 1
    /// </summary>
    public class VmElectronicChannelStep1 : VmServiceChannel, IVmElectronicChannelStep1
    {
        /// <summary>
        /// Id of electronic channel
        /// </summary>
        public Guid? ElectronicChannelId { get; set; }
        /// <summary>
        /// Indicates wheter channel supports online authentication
        /// </summary>
        public bool? IsOnLineAuthentication { get; set; }
        /// <summary>
        /// Indicates whether channel supports online sign
        /// </summary>
        public bool IsOnLineSign { get; set; }
        /// <summary>
        /// Count sign number
        /// </summary>
        public int? NumberOfSigns { get; set; }
        /// <summary>
        /// list of url attachments
        /// </summary>
        public List<VmChannelAttachment> UrlAttachments { get; set; }
        /// <summary>
        /// web page of the channel
        /// </summary>
        public VmWebPage WebPage { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public VmElectronicChannelStep1()
        {
            UrlAttachments = new List<VmChannelAttachment>();
            AreaBusinessRegions = new List<Guid>();
            AreaHospitalRegions = new List<Guid>();
            AreaProvince = new List<Guid>();
            AreaMunicipality = new List<Guid>();
        }

        /// <summary>
        /// phone numbers of channel
        /// </summary>
        public List<VmPhone> PhoneNumbers { get; set; }
        /// <summary>
        /// emails of the channel
        /// </summary>
        public List<VmEmailData> Emails { get; set; }
        /// <summary>
        /// language code of the channel
        /// </summary>
        public LanguageCode Language { get; set; }
        /// <summary>
        /// Gets or sets the list of HospitalRegions.
        /// </summary>
        /// <value>
        /// The list of HospitalRegions.
        /// </value>
        public List<Guid> AreaHospitalRegions { get; set; }
        /// <summary>
        /// Gets or sets the list of Municipality.
        /// </summary>
        /// <value>
        /// The list of Municipality.
        /// </value>
        public List<Guid> AreaMunicipality { get; set; }
        /// <summary>
        /// Gets or sets the list of Province.
        /// </summary>
        /// <value>
        /// The list of Province.
        /// </value>
        public List<Guid> AreaProvince { get; set; }
        /// <summary>
        /// Gets or sets the list of BusinessRegions.
        /// </summary>
        /// <value>
        /// The list of BusinessRegions.
        /// </value>
        public List<Guid> AreaBusinessRegions { get; set; }
    }
}
