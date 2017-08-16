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
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces;
using System;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of service location channel step1
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEnumEntityStatusBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmLocationChannelStep1" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmLocationChannelStep2" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmLocationChannelStep3" />
    public class VmLocationChannelStep1 : VmServiceChannel, IVmLocationChannelStep1, IVmLocationChannelStep2, IVmLocationChannelStep3
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmLocationChannelStep1"/> class.
        /// </summary>
        public VmLocationChannelStep1()
        {           
            Languages = new List<Guid>();
            VisitingAddresses = new List<VmAddressSimple>();
            PostalAddresses = new List<VmAddressSimple>();
            WebPages = new List<VmWebPage>();
            AreaBusinessRegions = new List<Guid>();
            AreaHospitalRegions = new List<Guid>();
            AreaProvince = new List<Guid>();
            AreaMunicipality = new List<Guid>();
            Emails = new List<VmEmailData>();
            PhoneNumbers = new List<VmPhone>();
            FaxNumbers = new List<VmPhone>();
        }

        /// <summary>
        /// Id of location channel
        /// </summary>
        public Guid? LocationChannelId { get; set; }
        /// <summary>
        /// Gets or sets the language guids.
        /// </summary>
        /// <value>
        /// The languages.
        /// </value>
        public List<Guid> Languages { get; set; }
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public LanguageCode Language { get; set; }
        /// <summary>
        /// Gets or sets the emails.
        /// </summary>
        /// <value>
        /// The emails.
        /// </value>
        public List<VmEmailData> Emails { get; set; }
        /// <summary>
        /// Gets or sets the faxes.
        /// </summary>
        /// <value>
        /// The fax.
        /// </value>
        public List<VmPhone> FaxNumbers { get; set; }
        /// <summary>
        /// Gets or sets the phone numbers.
        /// </summary>
        /// <value>
        /// The phone numbers.
        /// </value>
        public List<VmPhone> PhoneNumbers { get; set; }
        /// <summary>
        /// Gets or sets the web pages.
        /// </summary>
        /// <value>
        /// The web pages.
        /// </value>
        public List<VmWebPage> WebPages { get; set; }
        /// <summary>
        /// Gets or sets the visiting addresses.
        /// </summary>
        /// <value>
        /// The visiting addresses.
        /// </value>
        public List<VmAddressSimple> VisitingAddresses { get; set; }
        /// <summary>
        /// Gets or sets the postal addresses.
        /// </summary>
        /// <value>
        /// The postal addresses.
        /// </value>
        public List<VmAddressSimple> PostalAddresses { get; set; }
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
