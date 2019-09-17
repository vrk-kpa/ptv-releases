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
using PTV.Domain.Model.Models.OpenApi.V10;
using PTV.Domain.Model.Models.OpenApi.V9;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of organization - version base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiOrganizationBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IOpenApiVersionBase&lt;IVmOpenApiOrganizationVersionBase&gt;" />
    public interface IVmOpenApiOrganizationVersionBase : IVmOpenApiOrganizationBase, IOpenApiVersionBase<IVmOpenApiOrganizationVersionBase>, IOpenApiPublishing
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
        Guid? ParentOrganizationId { get; set; }
        /// <summary>
        /// Organizations root organization identifier if exists.
        /// </summary>
        Guid? OrganizationRootId { get; set; }
        /// <summary>
        /// List of organizations addresses.
        /// </summary>
        IList<V9VmOpenApiAddress> Addresses { get; set; }
        /// <summary>
        /// List of organizations services.
        /// </summary>
        IList<V10VmOpenApiOrganizationService> Services { get; set; }
        /// <summary>
        /// Date when item was modified/created.
        /// </summary>
        DateTime Modified { get; set; }
        /// <summary>
        /// The sub organizations
        /// </summary>
        IList<VmOpenApiItem> SubOrganizations { get; set; }
        
// SOTE has been disabled (SFIPTV-1177)        
//        /// <summary>
//        /// Responsible organization identifier if exists.
//        /// </summary>
//        Guid? ResponsibleOrganizationId { get; set; }
    }
}
