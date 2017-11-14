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
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V7;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of service for IN api - verison base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IOpenApiInVersionBase&lt;IVmOpenApiServiceInVersionBase&gt;" />
    public interface IVmOpenApiServiceInVersionBase : IVmOpenApiServiceBase, IOpenApiInVersionBase<IVmOpenApiServiceInVersionBase>
    {
        /// <summary>C:\workspace\dev\src\PTV.Domain.Model\Models\Interfaces\OpenApi\IVmOpenApiServiceChannel.cs
        /// Gets or sets the statutory service general description identifier.
        /// </summary>
        /// <value>
        /// The statutory service general description identifier.
        /// </value>
        string StatutoryServiceGeneralDescriptionId { get; set; }
        /// <summary>
        /// Area codes related to sub area type. For example if SubAreaType = Municipality, Areas-list need to include municipality codes like 491 or 091.
        /// </summary>
        IList<VmOpenApiAreaIn> Areas { get; set; }
        /// <summary>
        /// Gets or sets the service classes.
        /// </summary>
        /// <value>
        /// The service classes.
        /// </value>
        IList<string> ServiceClasses { get; set; }
        /// <summary>
        /// Gets or sets the ontology terms.
        /// </summary>
        /// <value>
        /// The ontology terms.
        /// </value>
        IList<string> OntologyTerms { get; set; }
        /// <summary>
        /// Gets or sets the target groups.
        /// </summary>
        /// <value>
        /// The target groups.
        /// </value>
        IList<string> TargetGroups { get; set; }
        /// <summary>
        /// Gets or sets the life events.
        /// </summary>
        /// <value>
        /// The life events.
        /// </value>
        IList<string> LifeEvents { get; set; }
        /// <summary>
        /// Gets or sets the industrial classes.
        /// </summary>
        /// <value>
        /// The industrial classes.
        /// </value>
        IList<string> IndustrialClasses { get; set; }
        /// <summary>
        /// Gets or sets the municipalities.
        /// </summary>
        /// <value>
        /// The municipalities.
        /// </value>
        IList<string> Municipalities { get; set; }
        /// <summary>
        /// Gets or sets the service organizations.
        /// </summary>
        /// <value>
        /// The service organizations.
        /// </value>
        IList<Guid> OtherResponsibleOrganizations { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all life events should be deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all life events should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllLifeEvents { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all industrial classes should be deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all industrial classes should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllIndustrialClasses { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all laws should be deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all laws should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllLaws { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all keywords should be deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all keywords should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllKeywords { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all municipalities should be deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all municipalities should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllMunicipalities { get; set; }
        /// <summary>
        /// Set to true to delete service charge type (ServiceChargeType property for this object should be empty when this option is used).
        /// </summary>
        bool DeleteServiceChargeType { get; set; }
        /// <summary>
        /// Set to true to delete statutory service general description (StatutoryServiceGeneralDescriptionId property for this object should be empty when this option is used).
        /// </summary>
        bool DeleteStatutoryServiceGeneralDescriptionId { get; set; }
        /// <summary>
        /// Current version publishing status.
        /// </summary>
        string CurrentPublishingStatus { get; set; }
        /// <summary>
        /// Gets or sets the service producers.
        /// </summary>
        /// <value>
        /// The service producers.
        /// </value>
        IList<VmOpenApiServiceProducerIn> ServiceProducers { get; set; }
        /// <summary>
        /// Internal property for adding service channel connections for a service.
        /// </summary>
        IList<V7VmOpenApiServiceServiceChannelAstiInBase> ServiceServiceChannels { get; set; }
        /// <summary>
        /// Gets or sets main organization identifier.
        /// </summary>
        string MainResponsibleOrganization { get; set; }
    }
}
