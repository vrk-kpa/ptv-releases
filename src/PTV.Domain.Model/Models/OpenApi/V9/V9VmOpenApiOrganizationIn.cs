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
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V9
{
    /// <summary>
    /// OPEN API V8 - View Model of organization for IN api
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.V9.V9VmOpenApiOrganizationInBase" />
    public class V9VmOpenApiOrganizationIn : V9VmOpenApiOrganizationInBase
    {
        /// <summary>
        /// Organization type (State, Municipality, RegionalOrganization, Organization, Company).
        /// </summary>
        [Required]
        public override string OrganizationType { get => base.OrganizationType; set => base.OrganizationType = value; }

        /// <summary>
        /// List of organization names. Possible type values are: Name, AlternativeName.
        /// </summary>
        [ListRequired]
        [ListWithPropertyValueRequired("Type", "Name")]
        public override IList<VmOpenApiLocalizedListItem> OrganizationNames { get => base.OrganizationNames; set => base.OrganizationNames = value; }

        /// <summary>
        /// Localized list of organization descriptions. Possible type values are: Description, Summary.
        /// </summary>
        [ListRequired]
        public override IList<VmOpenApiLocalizedListItem> OrganizationDescriptions { get => base.OrganizationDescriptions; set => base.OrganizationDescriptions = value; }

        /// <summary>
        /// List of Display name types (Name or AlternativeName) for each language version of OrganizationNames.
        /// </summary>
        [ListRequired]
        public override IList<VmOpenApiNameTypeByLanguage> DisplayNameType { get => base.DisplayNameType; set => base.DisplayNameType = value; }

        /// <summary>
        /// Area type (Nationwide, NationwideExceptAlandIslands, LimitedType). 
        /// </summary>
        [RequiredIf("OrganizationType", "State")]
        [RequiredIf("OrganizationType", "RegionalOrganization")]
        [RequiredIf("OrganizationType", "Organization")]
        [RequiredIf("OrganizationType", "Company")]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// Publishing status (Draft or Published).
        /// </summary>
        [AllowedValues(propertyName: "PublishingStatus", allowedValues: new[] { "Draft", "Published" })]
        [ValidEnum(null)] // PTV-1792: suppress base ValidEnum validation
        public override string PublishingStatus
        {
            get => base.PublishingStatus;
            set => base.PublishingStatus = value;
        }

        /// <summary>
        /// Set to true to delete all existing emails (the EmailAddresses collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllEmails
        {
            get
            {
                return base.DeleteAllEmails;
            }

            set
            {
                base.DeleteAllEmails = value;
            }
        }

        /// <summary>
        /// Set to true to delete all existing phone numbers (the PhoneNumbers collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllPhones
        {
            get
            {
                return base.DeleteAllPhones;
            }

            set
            {
                base.DeleteAllPhones = value;
            }
        }

        /// <summary>
        /// Set to true to delete all existing web pages (the WebPages collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllWebPages
        {
            get
            {
                return base.DeleteAllWebPages;
            }

            set
            {
                base.DeleteAllWebPages = value;
            }
        }

        /// <summary>
        /// Set to true to delete all existing addresses (the Addresses collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllAddresses
        {
            get
            {
                return base.DeleteAllAddresses;
            }

            set
            {
                base.DeleteAllAddresses = value;
            }
        }

        /// <summary>
        /// Set to true to delete all existing electronic invoicing addresses (the ElectronicInvoicings collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllElectronicInvoicings { get => base.DeleteAllElectronicInvoicings; set => base.DeleteAllElectronicInvoicings = value; }
    }
}
