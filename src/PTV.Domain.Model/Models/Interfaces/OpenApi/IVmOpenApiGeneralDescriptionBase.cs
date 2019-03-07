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
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of general description - base
    /// </summary>
    public interface IVmOpenApiGeneralDescriptionBase : IVmOpenApiEntityBase, IOpenApiPublishing
    {
        /// <summary>
        /// Gets or sets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        IList<VmOpenApiLocalizedListItem> Names { get; set; }

        /// <summary>
        /// Gets or sets the descriptions.
        /// </summary>
        /// <value>
        /// The descriptions.
        /// </value>
        IList<VmOpenApiLocalizedListItem> Descriptions { get; set; }

        /// <summary>
        /// Gets or sets the requirements.
        /// </summary>
        /// <value>
        /// The requirements.
        /// </value>
        IList<VmOpenApiLanguageItem> Requirements { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        string Type { get; set; }

        /// <summary>
        /// Gets or sets the type of the service charge.
        /// </summary>
        /// <value>
        /// The type of the service charge.
        /// </value>
        string ServiceChargeType { get; set; }

        /// <summary>
        /// Gets or sets the legislation.
        /// </summary>
        /// <value>
        /// The legislation.
        /// </value>
        IList<V4VmOpenApiLaw> Legislation { get; set; }

        /// <summary>
        /// Gets or sets availble languages
        /// </summary>
        IList<string> AvailableLanguages { get; set; }
        /// <summary>
        /// The identifier for current version.
        /// </summary>
        Guid? CurrentVersionId { get; set; }

        /// <summary>
        /// Gets or sets the general description type.
        /// </summary>
        /// <value>
        /// The general description type.
        /// </value>
        string GeneralDescriptionType { get; set; }

        /// <summary>
        /// Gets or sets the general description type.
        /// </summary>
        /// <value>
        /// The general description type.
        /// </value>
        Guid GeneralDescriptionTypeId { get; set; }

        /// <summary>
        /// Gets if general description is SOTE type
        /// </summary>
        bool IsSoteType { get; }
    }
}