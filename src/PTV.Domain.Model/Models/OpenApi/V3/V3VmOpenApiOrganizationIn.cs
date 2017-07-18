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

namespace PTV.Domain.Model.Models.OpenApi.V3
{
    /// <summary>
    /// OPEN API V3 - View Model of organization for IN api
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.V3.V3VmOpenApiOrganizationInBase" />
    public class V3VmOpenApiOrganizationIn : V3VmOpenApiOrganizationInBase
    {
        /// <summary>
        /// Organization type (State, Municipality, RegionalOrganization, Organization, Company).
        /// </summary>
        [Required]
        public override string OrganizationType
        {
            get
            {
                return base.OrganizationType;
            }

            set
            {
                base.OrganizationType = value;
            }
        }

        /// <summary>
        /// List of organization names.
        /// </summary>
        [ListRequired]
        [ListWithPropertyValueRequired("Type", "Name")]
        public override IList<VmOpenApiLocalizedListItem> OrganizationNames
        {
            get
            {
                return base.OrganizationNames;
            }

            set
            {
                base.OrganizationNames = value;
            }
        }

        /// <summary>
        /// Display name type (Name or AlternateName). Which name type should be used as the display name for the organization (OrganizationNames list).
        /// </summary>
        [Required]
        public override string DisplayNameType { get => base.DisplayNameType; set => base.DisplayNameType = value; }

        /// <summary>
        /// Publishing status. Possible values are: Draft, Published, Deleted or Modified.
        /// </summary>
        [Required]
        public override string PublishingStatus
        {
            get
            {
                return base.PublishingStatus;
            }

            set
            {
                base.PublishingStatus = value;
            }
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
    }
}
