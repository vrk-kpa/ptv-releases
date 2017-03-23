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

using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi.V2
{
    /// <summary>
    /// OPEN API V2 - View model interface of service
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceVersionBase" />
    public interface IV2VmOpenApiService : IVmOpenApiServiceVersionBase
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
        /// Gets or sets the service additional informations.
        /// </summary>
        /// <value>
        /// The service additional informations.
        /// </value>
        IList<VmOpenApiLocalizedListItem> ServiceAdditionalInformations { get; set; }
        /// <summary>
        /// Gets or sets the municipalities.
        /// </summary>
        /// <value>
        /// The municipalities.
        /// </value>
        new IList<string> Municipalities { get; set; }
        /// <summary>
        /// Gets or sets the service channels.
        /// </summary>
        /// <value>
        /// The service channels.
        /// </value>
        new IList<V2VmOpenApiServiceServiceChannel> ServiceChannels { get; set; }
        /// <summary>
        /// Gets or sets the organizations.
        /// </summary>
        /// <value>
        /// The organizations.
        /// </value>
        new IList<VmOpenApiServiceOrganization> Organizations { get; set; }

    }
}
