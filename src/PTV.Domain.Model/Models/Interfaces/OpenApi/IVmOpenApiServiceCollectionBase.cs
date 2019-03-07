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
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of service collection - base
    /// </summary>
    public interface IVmOpenApiServiceCollectionBase
    {
        /// <summary>
        /// PTV service identifier.
        /// </summary>
        Guid? Id { get; set; }
        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        /// <value>
        /// The source identifier.
        /// </value>
        string SourceId { get; set; }
        /// <summary>
        /// Main responsible organization Id
        /// </summary>
        Guid MainResponsibleOrganization { get; set; }
        /// <summary>
        /// List of localized service collection names.
        /// </summary>
        IList<VmOpenApiLanguageItem> ServiceCollectionNames { get; set; }
        /// <summary>
        /// List of localized service collection descriptions.
        /// </summary>
        IList<VmOpenApiLanguageItem> ServiceCollectionDescriptions { get; set; }
        /// <summary>
        /// Publishing status. Possible values are: Draft, Published, Deleted, Modified or OldPublished.
        /// </summary>
        string PublishingStatus { get; set; }

        /// <summary>
        /// Gets or sets availble languages
        /// </summary>
        IList<string> AvailableLanguages { get; set; }
    }
}
