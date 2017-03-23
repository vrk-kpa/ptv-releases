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
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi.V1
{
    /// <summary>
    /// OPEN API - View model interface of Service
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceVersionBase" />
    public interface IVmOpenApiService : IVmOpenApiServiceVersionBase
    {
        /// <summary>
        /// Gets or sets the service classes.
        /// </summary>
        /// <value>
        /// The service classes.
        /// </value>
        new IList<VmOpenApiFintoItem> ServiceClasses { get; set; }
        /// <summary>
        /// Gets or sets the ontology terms.
        /// </summary>
        /// <value>
        /// The ontology terms.
        /// </value>
        new IList<VmOpenApiFintoItem> OntologyTerms { get; set; }
        /// <summary>
        /// Gets or sets the target groups.
        /// </summary>
        /// <value>
        /// The target groups.
        /// </value>
        new IList<VmOpenApiFintoItem> TargetGroups { get; set; }
        /// <summary>
        /// Gets or sets the life events.
        /// </summary>
        /// <value>
        /// The life events.
        /// </value>
        new IList<VmOpenApiFintoItem> LifeEvents { get; set; }
        /// <summary>
        /// Gets or sets the industrial classes.
        /// </summary>
        /// <value>
        /// The industrial classes.
        /// </value>
        new IList<VmOpenApiFintoItem> IndustrialClasses { get; set; }

        /// <summary>
        /// List of service keywords.
        /// </summary>
        new IReadOnlyList<string> Keywords { get; set; }

        /// <summary>
        /// List of municipality names that the service is available for. Used in conjunction with service coverage type Local.
        /// </summary>
        new IList<string> Municipalities { get; set; }

        /// <summary>
        /// List of service web pages. This property is not used in the API anymore. Do not use.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        IList<VmOpenApiWebPage> WebPages { get; set; }

        /// <summary>
        /// Localized service additional information.
        /// </summary>
        IList<VmOpenApiLocalizedListItem> ServiceAdditionalInformations { get; set; }

        /// <summary>
        /// List of PTV identifiers of linked service channels.
        /// </summary>
        new IList<string> ServiceChannels { get; set; }

        /// <summary>
        /// List of organizations, responsible and producer organizations of the service.
        /// </summary>
        new IList<VmOpenApiServiceOrganization> Organizations { get; set; }
    }
}
