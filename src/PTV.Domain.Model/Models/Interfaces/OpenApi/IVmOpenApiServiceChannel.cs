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
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of service channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceChannelBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IOpenApiVersionBase&lt;IVmOpenApiServiceChannel&gt;" />
    public interface IVmOpenApiServiceChannel : IVmOpenApiServiceChannelBase, IOpenApiVersionBase<IVmOpenApiServiceChannel>
    {
        /// <summary>
        /// Gets or sets the type of the service channel.
        /// </summary>
        /// <value>
        /// The type of the service channel.
        /// </value>
        string ServiceChannelType { get; set; }
        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>
        /// The organization identifier.
        /// </value>
        Guid OrganizationId { get; set; }
        /// <summary>
        /// Gets or sets the service channel names.
        /// </summary>
        /// <value>
        /// The service channel names.
        /// </value>
        IReadOnlyList<VmOpenApiLocalizedListItem> ServiceChannelNames { get; set; }
        /// <summary>
        /// List of organization areas
        /// </summary>
        IList<VmOpenApiArea> Areas { get; set; }
        /// <summary>
        /// List of municipalities including municipality code and a localized list of municipality names.
        /// </summary>
        IList<VmOpenApiMunicipality> AreaMunicipalities { get; set; }
        /// <summary>
        /// List of linked services including relationship data.
        /// </summary>
        IList<VmOpenApiServiceChannelService> Services { get; set; }
        /// <summary>
        /// List of linked service channels including relationship data.
        /// </summary>
        IList<VmOpenApiExtraType> ServiceExtraTypes { get; set; }
        /// <summary>
        /// Date when item was modified/created (UTC).
        /// </summary>
        DateTime Modified { get; set; }
    }
}
