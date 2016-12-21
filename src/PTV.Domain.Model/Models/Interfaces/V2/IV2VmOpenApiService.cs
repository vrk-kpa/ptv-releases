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
using PTV.Domain.Model.Models.OpenApi.V2;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.V2
{
    public interface IV2VmOpenApiService : IVmOpenApiServiceBase
    {

        /// <summary>
        /// PTV identifier for linked general description.
        /// </summary>
        Guid? StatutoryServiceGeneralDescriptionId { get; set; }
        /// <summary>
        /// List of service classes related to the service.
        /// </summary>
        IReadOnlyList<VmOpenApiFintoItem> ServiceClasses { get; set; }
        /// <summary>
        /// List of ontology terms related to the service.
        /// </summary>
        IReadOnlyList<VmOpenApiFintoItem> OntologyTerms { get; set; }
        /// <summary>
        /// List of target groups related to the service.
        /// </summary>
        IReadOnlyList<VmOpenApiFintoItem> TargetGroups { get; set; }
        /// <summary>
        /// List of life events  related to the service.
        /// </summary>
        IReadOnlyList<VmOpenApiFintoItem> LifeEvents { get; set; }
        /// <summary>
        /// List of industrial classes related to the service.
        /// </summary>
        IReadOnlyList<VmOpenApiFintoItem> IndustrialClasses { get; set; }

        //bool ElectronicNotification { get; set; }  // These are not included in first release version
        //bool ElectronicCommunication { get; set; } // - maybe in some future release.
        //IReadOnlyList<IVmOpenApiLanguageItem> ElectronicNotificationChannels { get; set; }
        //IReadOnlyList<IVmOpenApiLanguageItem> ElectronicCommunicationChannels { get; set; }

        /// <summary>
        /// List of PTV identifiers of linked service channels.
        /// </summary>
        //IReadOnlyList<Guid> ServiceChannels { get; set; }
        IReadOnlyList<V2VmOpenApiServiceServiceChannel> ServiceChannels { get; set; }

        /// <summary>
        /// List of organizations, responsible and producer organizations of the service.
        /// </summary>
        IReadOnlyList<VmOpenApiServiceOrganization> Organizations { get; set; }
    }
}
