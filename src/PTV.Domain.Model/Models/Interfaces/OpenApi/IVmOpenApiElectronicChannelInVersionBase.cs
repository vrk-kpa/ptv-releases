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
using PTV.Domain.Model.Models.OpenApi.V9;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of channel for IN api - version base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceChannelIn" />
    public interface IVmOpenApiElectronicChannelInVersionBase : IVmOpenApiServiceChannelIn
    {
        /// <summary>
        /// Gets or sets the service channel names.
        /// </summary>
        /// <value>
        /// The service channel names.
        /// </value>
        IList<VmOpenApiLanguageItem> ServiceChannelNames { get; set; }
        /// <summary>
        /// Gets or sets the signature quantity.
        /// </summary>
        /// <value>
        /// The signature quantity.
        /// </value>
        string SignatureQuantity { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether signature is required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if signature required; otherwise, <c>false</c>.
        /// </value>
        bool RequiresSignature { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether authentication required.
        /// </summary>
        /// <value>
        /// <c>true</c> if authentication required; otherwise, <c>false</c>.
        /// </value>
        bool RequiresAuthentication { get; set; }
        /// <summary>
        /// Gets or sets the urls.
        /// </summary>
        /// <value>
        /// The urls.
        /// </value>
        IList<VmOpenApiLanguageItem> WebPage { get; set; }
        /// <summary>
        /// Gets or sets the attachments.
        /// </summary>
        /// <value>
        /// The attachments.
        /// </value>
        IReadOnlyList<VmOpenApiAttachment> Attachments { get; set; }
        /// <summary>
        /// The accessibility classification level.
        /// </summary>
        string AccessibilityClassificationLevel { get; set; }
        /// <summary>
        /// The web content accessibility level.
        /// </summary>
        string WCAGLevel { get; set; }
        /// <summary>
        /// List of accessibility statement web pages. One per language.
        /// </summary>
        IList<V9VmOpenApiWebPage> AccessibilityStatementWebPage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all attachments] should be delted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all attachments should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllAttachments { get; set; }
    }
}
