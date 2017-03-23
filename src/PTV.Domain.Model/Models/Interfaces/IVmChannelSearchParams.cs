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

namespace PTV.Domain.Model.Models.Interfaces
{
    /// <summary>
    /// View model interface of channel search for searching of channel
    /// </summary>
    public interface IVmChannelSearchParams : IVmMultiLocalized, IVmSearchParamsBase
    {
        /// <summary>
        /// Gets or sets the type of the channel.
        /// </summary>
        /// <value>
        /// The type of the channel.
        /// </value>
        string ChannelType { get; set; }
        /// <summary>
        /// Gets or sets the channel type identifier.
        /// </summary>
        /// <value>
        /// The channel type identifier.
        /// </value>
        Guid? ChannelTypeId { get; set; }
        /// <summary>
        /// Gets or sets the channel identifier.
        /// </summary>
        /// <value>
        /// The channel identifier.
        /// </value>
        Guid? ChannelId { get; set; }
        /// <summary>
        /// Gets or sets the name of the channel.
        /// </summary>
        /// <value>
        /// The name of the channel.
        /// </value>
        string Name { get; set; }
        /// <summary>
        /// Gets or sets the selected publishing statuses.
        /// </summary>
        /// <value>
        /// The selected publishing statuses.
        /// </value>
        List<Guid> SelectedPublishingStatuses { get; set; }
        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>
        /// The organization identifier.
        /// </value>
        Guid? OrganizationId { get; set; }
        /// <summary>
        /// Gets or sets the selected phone number types.
        /// </summary>
        /// <value>
        /// The selected phone number types.
        /// </value>
        List<Guid> SelectedPhoneNumberTypes { get; set; }
        /// <summary>
        /// Gets or sets the service channel relations.
        /// </summary>
        /// <value>
        /// The service channel relations.
        /// </value>
        List<VmServiceChannelRelation> ServiceChannelRelations { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether only published should be searched.
        /// </summary>
        /// <value>
        ///   <c>true</c> if published should be found; otherwise, <c>false</c>.
        /// </value>
        bool PublishedSearch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether relations should be included.
        /// </summary>
        /// <value>
        ///   <c>true</c> if relations included; otherwise, <c>false</c>.
        /// </value>
        bool IncludedRelations { get; set; }
    }
}
