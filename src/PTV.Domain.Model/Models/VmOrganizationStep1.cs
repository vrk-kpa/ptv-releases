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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Converters;
using PTV.Domain.Model.Models.Interfaces.Security;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of organization step 1
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEnumEntityStatusBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmOrganizationStep1" />
    public class VmOrganizationStep1 : VmEnumEntityStatusBase, IVmOrganizationStep1, IVmRootBasedEntity, IVmAreaInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmOrganizationStep1"/> class.
        /// </summary>
        public VmOrganizationStep1()
        {
            PhoneNumbers = new List<VmPhone>();
            Emails = new List<VmEmailData>();
            WebPages = new List<VmWebPage>();
            VisitingAddresses = new List<VmAddressSimple>();
            PostalAddresses = new List<VmAddressSimple>();
            AreaBusinessRegions = new List<Guid>();
            AreaHospitalRegions = new List<Guid>();
            AreaProvince = new List<Guid>();
            AreaMunicipality = new List<Guid>();
        }

        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>
        /// The parent identifier.
        /// </value>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the organization type identifier.
        /// </summary>
        /// <value>
        /// The organization type identifier.
        /// </value>
        public Guid? OrganizationTypeId { get; set; }

        /// <summary>
        /// Gets or sets the municipality.
        /// </summary>
        /// <value>
        /// The municipality.
        /// </value>
        [JsonConverter(typeof(ListItemConverter))]
        public VmListItem Municipality { get; set; }

        /// <summary>
        /// Gets or sets the name of the organization.
        /// </summary>
        /// <value>
        /// The name of the organization.
        /// </value>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Gets or sets the alternate name of the organization.
        /// </summary>
        /// <value>
        /// The alternate name of the organization.
        /// </value>
        public string OrganizationAlternateName { get; set; }

        /// <summary>
        /// Gets or sets the display name identifier.
        /// </summary>
        /// <value>
        /// The display name identifier.
        /// </value>
        [JsonIgnore]
        public Guid DisplayNameId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is alternate name used as display name.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is alternate name used as display name; otherwise, <c>false</c>.
        /// </value>
        public bool IsAlternateNameUsedAsDisplayName { get; set; }
        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>
        /// The organization identifier.
        /// </value>
        public string OrganizationId { get; set; }
        /// <summary>
        /// Gets or sets the are infomration type identifier.
        /// </summary>
        /// <value>
        /// The area information type identifier.
        /// </value>
        public Guid AreaInformationTypeId { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the business.
        /// </summary>
        /// <value>
        /// The business.
        /// </value>
        public VmBusiness Business { get; set; }

        /// <summary>
        /// Gets or sets the emails.
        /// </summary>
        /// <value>
        /// The emails.
        /// </value>
        public List<VmEmailData> Emails { get; set; }

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
        /// Gets or sets a value indicating whether postal address should be shown or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if postal address should be shown; otherwise, <c>false</c>.
        /// </value>
        public bool ShowPostalAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether visiting address should be shown.
        /// </summary>
        /// <value>
        /// <c>true</c> if visiting address should shown; otherwise, <c>false</c>.
        /// </value>
        public bool ShowVisitingAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether contacts should be shown.
        /// </summary>
        /// <value>
        /// <c>true</c> if contacts should be shown; otherwise, <c>false</c>.
        /// </value>
        public bool ShowContacts { get; set; }
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public LanguageCode Language { get; set; }

        /// <summary>
        /// Entity security information. <see cref="IVmEntitySecurity.Security"/>
        /// </summary>
        public ISecurityOwnOrganization Security { get; set; }

        /// <summary>
        /// Root node identifer
        /// </summary>
        public Guid UnificRootId { get; set; }

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
