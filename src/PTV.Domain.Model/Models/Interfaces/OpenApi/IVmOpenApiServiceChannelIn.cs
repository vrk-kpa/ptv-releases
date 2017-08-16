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

using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V5;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of service channel for IN api
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceChannelBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IOpenApiInVersionBase&lt;IVmOpenApiServiceChannelIn&gt;" />
    public interface IVmOpenApiServiceChannelIn : IVmOpenApiServiceChannelBase, IOpenApiInVersionBase<IVmOpenApiServiceChannelIn>
    {
        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        /// <value>
        /// The source identifier.
        /// </value>
        string SourceId { get; set; }
        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>
        /// The organization identifier.
        /// </value>
        string OrganizationId { get; set; }
        /// <summary>
        /// Gets or sets the service channel names.
        /// </summary>
        /// <value>
        /// The service channel names.
        /// </value>
        IList<VmOpenApiLanguageItem> ServiceChannelNames { get; set; }
        /// <summary>
        /// Area codes related to sub area type. For example if SubAreaType = Municipality, Areas-list need to include municipality codes like 491 or 091.
        /// </summary>
        IList<VmOpenApiAreaIn> Areas { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all service hours should be delted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all service hours should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllServiceHours { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all web pages deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all web pages should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllWebPages { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all support emails should be deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all support emails should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllSupportEmails { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all support phones should be deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all support phones should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllSupportPhones { get; set; }
        /// <summary>
        /// Current version publishing status.
        /// </summary>
        string CurrentPublishingStatus { get; set; }
        /// <summary>
        /// Internal property for adding service relations for a service channel.
        /// </summary>
        IList<Guid> ServiceChannelServices { get; set; }
    }
}
