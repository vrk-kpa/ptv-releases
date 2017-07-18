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

using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V5;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of organization for IN api
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiOrganizationBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IOpenApiInVersionBase&lt;IVmOpenApiOrganizationInVersionBase&gt;" />
    public interface IVmOpenApiOrganizationInVersionBase : IVmOpenApiOrganizationBase, IOpenApiInVersionBase<IVmOpenApiOrganizationInVersionBase>
    {
        /// <summary>
        /// Organization OID. - must match the regex @"^[A-Za-z0-9.-]*$"
        /// </summary>
        new string Oid { get; set; }

        /// <summary>
        /// Gets or sets the municipality.
        /// </summary>
        /// <value>
        /// The municipality.
        /// </value>
        string Municipality { get; set; }
        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        /// <value>
        /// The source identifier.
        /// </value>
        string SourceId { get; set; }
        /// <summary>
        /// Gets or sets the organization descriptions.
        /// </summary>
        /// <value>
        /// The organization descriptions.
        /// </value>
        IReadOnlyList<VmOpenApiLanguageItem> OrganizationDescriptions { get; set; }
        /// <summary>
        /// Sub area type (Municipality, Province, BusinessRegions, HospitalRegions).
        /// </summary>
        string SubAreaType { get; set; }
        /// <summary>
        /// Area codes related to sub area type. For example if SubAreaType = Municipality, Areas-list need to include municipality codes like 491 or 091.
        /// </summary>
        IList<string> Areas { get; set; }
        /// <summary>
        /// Gets or sets the email addresses.
        /// </summary>
        /// <value>
        /// The email addresses.
        /// </value>
        IList<V4VmOpenApiEmail> EmailAddresses { get; set; }
        /// <summary>
        /// Gets or sets the addresses.
        /// </summary>
        /// <value>
        /// The addresses.
        /// </value>
        IList<V5VmOpenApiAddressWithTypeIn> Addresses { get; set; }
        /// <summary>
        /// Gets or sets the parent organization identifier.
        /// </summary>
        /// <value>
        /// The parent organization identifier.
        /// </value>
        string ParentOrganizationId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all emails sohould be deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all emails should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllEmails { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all phones should be delted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all phones should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllPhones { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all web pages should be deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all web pages should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllWebPages { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all addresses should be deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all addresses should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllAddresses { get; set; }
        /// <summary>
        /// Current version publishing status.
        /// </summary>
        string CurrentPublishingStatus { get; set; }
    }
}
