/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V7;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of service - version base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IOpenApiVersionBase&lt;IVmOpenApiServiceVersionBase&gt;" />
    public interface IVmOpenApiServiceVersionBase : IVmOpenApiServiceBase, IOpenApiVersionBase<IVmOpenApiServiceVersionBase>, IOpenApiEntityVersion
    {
        /// <summary>
        /// PTV identifier for linked general description.
        /// </summary>
        Guid? GeneralDescriptionId { get; set; }
        /// <summary>
        /// Service sub-type. It is used for SOTE and its taken from GeneralDescription type.
        /// </summary>
        string SubType { get; set; }
        /// <summary>
        /// List of organization areas
        /// </summary>
        IList<VmOpenApiArea> Areas { get; set; }
        /// <summary>
        /// List of service classes related to the service.
        /// </summary>
        IList<V7VmOpenApiFintoItemWithDescription> ServiceClasses { get; set; }
        /// <summary>
        /// List of ontology terms related to the service.
        /// </summary>
        IList<V4VmOpenApiOntologyTerm> OntologyTerms { get; set; }
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
        /// List of PTV identifiers of linked service channels.
        /// </summary>
        IList<V11VmOpenApiServiceServiceChannel> ServiceChannels { get; set; }
        /// <summary>
        /// List of other responsible organizations of the service.
        /// </summary>
        IList<V6VmOpenApiServiceOrganization> Organizations { get; set; }
        /// <summary>
        /// List of service collections that the service has been linked to
        /// </summary>
        IList<VmOpenApiServiceServiceCollection> ServiceCollections { get; set; }
        /// <summary>
        /// Date when item was modified/created.
        /// </summary>
        DateTime Modified { get; set; }
        /// <summary>
        /// Sote organization that is responsible for the service. Notice! At the moment always empty - the property is a placeholder for later use.
        /// </summary>
        string ResponsibleSoteOrganization { get; set; }
        /// <summary>
        /// PTV identifier for main responsible organization.
        /// </summary>
        VmOpenApiItem MainOrganization { get; set; }
        /// <summary>
        /// List of service producers
        /// </summary>
        IList<VmOpenApiServiceProducer> ServiceProducers { get; set; }
    }
}
