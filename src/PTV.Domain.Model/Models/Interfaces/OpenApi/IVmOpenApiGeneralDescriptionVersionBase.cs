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

using PTV.Domain.Model.Models.OpenApi.V4;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of general descriptionfor base
    /// </summary>
    public interface IVmOpenApiGeneralDescriptionVersionBase : IVmOpenApiGeneralDescriptionBase, IOpenApiVersionBase<IVmOpenApiGeneralDescriptionVersionBase>
    {
        /// <summary>
        /// Gets or sets the life events.
        /// </summary>
        /// <value>
        /// The life events.
        /// </value>
        IList<V4VmOpenApiFintoItem> LifeEvents { get; set; }

        /// <summary>
        /// Gets or sets the ontology terms.
        /// </summary>
        /// <value>
        /// The ontology terms.
        /// </value>
        IList<V4VmOpenApiFintoItem> OntologyTerms { get; set; }

        /// <summary>
        /// Gets or sets the service classes.
        /// </summary>
        /// <value>
        /// The service classes.
        /// </value>
        IList<V4VmOpenApiFintoItem> ServiceClasses { get; set; }

        /// <summary>
        /// Gets or sets the target groups.
        /// </summary>
        /// <value>
        /// The target groups.
        /// </value>
        IList<V4VmOpenApiFintoItem> TargetGroups { get; set; }

        /// <summary>
        /// Gets or sets the industrial classes.
        /// </summary>
        /// <value>
        /// The industrial classes.
        /// </value>
        IList<V4VmOpenApiFintoItem> IndustrialClasses { get; set; }

        /// <summary>
        /// Date when item was modified/created (UTC).
        /// </summary>
        DateTime Modified { get; set; }
    }
}
