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

using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V5;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of organization - version base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiOrganizationBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IOpenApiVersionBase&lt;IVmOpenApiOrganizationVersionBase&gt;" />
    public interface IVmOpenApiOrganizationVersionBase : IVmOpenApiOrganizationBase, IOpenApiVersionBase<IVmOpenApiOrganizationVersionBase>
    {
        /// <summary>
        /// Municipality including municipality code and a localized list of municipality names.
        /// </summary>
        VmOpenApiMunicipality Municipality { get; set; }
        /// <summary>
        /// List of organization areas
        /// </summary>
        IList<VmOpenApiArea> Areas { get; set; }
        /// <summary>
        /// List of organization area municipalities
        /// </summary>
        IList<VmOpenApiMunicipality> AreaMunicipalities { get; set; }
        /// <summary>
        /// Organizations parent organization identifier if exists.
        /// </summary>
        Guid? ParentOrganization { get; set; }
        /// <summary>
        /// List of organizations email addresses.
        /// </summary>
        IReadOnlyList<V4VmOpenApiEmail> EmailAddresses { get; set; }
        /// <summary>
        /// List of organizations addresses.
        /// </summary>
        IList<V5VmOpenApiAddressWithTypeAndCoordinates> Addresses { get; set; }
        /// <summary>
        /// List of organizations services.
        /// </summary>
        IList<V5VmOpenApiOrganizationService> Services { get; set; }
        /// <summary>
        /// Date when item was modified/created.
        /// </summary>
        DateTime Modified { get; set; }

        /// <summary>
        /// The sub organizations
        /// </summary>
        IList<VmOpenApiItem> SubOrganizations { get; set; }

    }
}
