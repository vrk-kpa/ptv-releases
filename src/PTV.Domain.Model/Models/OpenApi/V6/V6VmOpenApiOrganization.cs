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

using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V6;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using PTV.Domain.Model.Models.OpenApi.V4;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V6
{
    /// <summary>
    /// OPEN API V6 - View Model of organization
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V6.IV6VmOpenApiOrganization" />
    public class V6VmOpenApiOrganization : VmOpenApiOrganizationVersionBase, IV6VmOpenApiOrganization
    {
        /// <summary>
        /// Organizations parent organization identifier if exists.
        /// </summary>
        [Display(Name = "ParentOrganization")]
        [JsonProperty(Order = 3, PropertyName = "parentOrganization")]
        public override Guid? ParentOrganizationId { get => base.ParentOrganizationId; set => base.ParentOrganizationId = value; }

        /// <summary>
        /// Organization type (State, Municipality, RegionalOrganization, Organization, Company).
        /// </summary>
        [JsonProperty(Order = 5)]
        public override string OrganizationType { get => base.OrganizationType; set => base.OrganizationType = value; }

        /// <summary>
        /// Organizations root organization identifier if exists.
        /// </summary>
        [JsonIgnore]
        public override Guid? OrganizationRootId { get => base.OrganizationRootId; set => base.OrganizationRootId = value; }

        /// <summary>
        /// List of Display name types (Name or AlternateName) for each language version of OrganizationNames.
        /// </summary>
        [JsonProperty(Order = 14)]
        public override IList<VmOpenApiNameTypeByLanguage> DisplayNameType { get => base.DisplayNameType; set => base.DisplayNameType = value; }

        /// <summary>
        /// Area type (WholeCountry, WholeCountryExceptAlandIslands, AreaType). 
        /// </summary>
        [JsonProperty(Order = 16)]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// List of organizations email addresses.
        /// </summary>
        [Display(Name = "EmailAddresses")]
        [JsonProperty(Order = 21, PropertyName = "emailAddresses")]
        public override IList<V4VmOpenApiEmail> Emails { get => base.Emails; set => base.Emails = value; }

        /// <summary>
        /// List of organizations web pages.
        /// </summary>
        [JsonProperty(Order = 22)]
        public virtual new IList<VmOpenApiWebPageWithOrderNumber> WebPages { get; set; } = new List<VmOpenApiWebPageWithOrderNumber>();

        /// <summary>
        /// List of organizations addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public new IList<V5VmOpenApiAddressWithTypeAndCoordinates> Addresses { get; set; } = new List<V5VmOpenApiAddressWithTypeAndCoordinates>();

        /// <summary>
        /// Organization external system identifier.
        /// </summary>
        [JsonIgnore]
        public override string SourceId { get => base.SourceId; set => base.SourceId = value; }

        /// <summary>
        /// The sub organizations
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiItem> SubOrganizations { get => base.SubOrganizations; set => base.SubOrganizations = value; }

        /// <summary>
        /// List of organizations electronic invoicing information.
        /// </summary>
        [JsonIgnore]
        override public IList<VmOpenApiOrganizationEInvoicing> ElectronicInvoicings { get => base.ElectronicInvoicings; set => base.ElectronicInvoicings = value; }

        /// <summary>
        /// Responsible organization identifier if exists.
        /// </summary>
        [JsonIgnore]
        public override Guid? ResponsibleOrganizationId { get => base.ResponsibleOrganizationId; set => base.ResponsibleOrganizationId = value; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 6;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiOrganizationVersionBase PreviousVersion()
        {
            throw new NotSupportedException("No previous version for class V6VmOpenApiOrganization!");
        }
        #endregion
    }
}
