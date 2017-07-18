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
using PTV.Domain.Model.Models.OpenApi.V6;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of service - version base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IOpenApiVersionBase&lt;IVmOpenApiServiceVersionBase&gt;" />
    public interface IVmOpenApiServiceVersionBase : IVmOpenApiServiceBase, IOpenApiVersionBase<IVmOpenApiServiceVersionBase>
    {
        /// <summary>
        /// PTV identifier for linked general description.
        /// </summary>
        Guid? StatutoryServiceGeneralDescriptionId { get; set; }
        /// <summary>
        /// List of organization areas
        /// </summary>
        IList<VmOpenApiArea> Areas { get; set; }
        /// <summary>
        /// List of service classes related to the service.
        /// </summary>
        IReadOnlyList<V4VmOpenApiFintoItem> ServiceClasses { get; set; }
        /// <summary>
        /// List of ontology terms related to the service.
        /// </summary>
        IReadOnlyList<V4VmOpenApiFintoItem> OntologyTerms { get; set; }
        /// <summary>
        /// List of target groups related to the service.
        /// </summary>
        IList<V4VmOpenApiFintoItem> TargetGroups { get; set; }
        /// <summary>
        /// List of life events  related to the service.
        /// </summary>
        IReadOnlyList<V4VmOpenApiFintoItem> LifeEvents { get; set; }
        /// <summary>
        /// List of industrial classes related to the service.
        /// </summary>
        IReadOnlyList<V4VmOpenApiFintoItem> IndustrialClasses { get; set; }

        /// <summary>
        /// List of municipality codes and names that the service is available for. Used in conjunction with service coverage type Local.
        /// </summary>
        IReadOnlyList<VmOpenApiMunicipality> Municipalities { get; set; }

        /// <summary>
        /// List of PTV identifiers of linked service channels.
        /// </summary>
        IList<V6VmOpenApiServiceServiceChannel> ServiceChannels { get; set; }

        /// <summary>
        /// List of organizations, responsible and producer organizations of the service.
        /// </summary>
        IList<V6VmOpenApiServiceOrganization> Organizations { get; set; }

        /// <summary>
        /// List of service voucherses
        /// </summary>
        IList<VmOpenApiServiceVoucher> ServiceVouchers { get; set; }
            /// <summary>
        /// Date when item was modified/created..
        /// </summary>
        DateTime Modified { get; set; }
    }
}
