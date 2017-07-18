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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of channel search for searching of channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmChannelSearchParams" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmLocalized" />
    public class VmChannelSearchParams : IVmChannelSearchParams, IVmMultiLocalized
    {
        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        public int PageNumber { get; set; }
        /// <summary>
        /// Gets or sets the type of the channel.
        /// </summary>
        /// <value>
        /// The type of the channel.
        /// </value>
        public string ChannelType { get; set; }
        /// <summary>
        /// Gets or sets the channel type identifier.
        /// </summary>
        /// <value>
        /// The channel type identifier.
        /// </value>
        public Guid? ChannelTypeId { get; set; }
        /// <summary>
        /// Gets or sets the name of the channel.
        /// </summary>
        /// <value>
        /// The name of the channel.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the channel form identifier.
        /// </summary>
        /// <value>
        /// The channel form identifier.
        /// </value>
        public string ChannelFormIdentifier { get; set; }
        /// <summary>
        /// Gets or sets the selected publishing statuses.
        /// </summary>
        /// <value>
        /// The selected publishing statuses.
        /// </value>
        public List<Guid> SelectedPublishingStatuses { get; set; }
        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>
        /// The organization identifier.
        /// </value>
        public Guid? OrganizationId { get; set; }
        /// <summary>
        /// Gets or sets the selected phone number types.
        /// </summary>
        /// <value>
        /// The selected phone number types.
        /// </value>
        public List<Guid> SelectedPhoneNumberTypes { get; set; }
        /// <summary>
        /// Gets or sets the service channel relations.
        /// </summary>
        /// <value>
        /// The service channel relations.
        /// </value>
        public List<VmServiceChannelRelation> ServiceChannelRelations { get; set; }
        /// <summary>
        /// Gets or sets the channel identifier.
        /// </summary>
        /// <value>
        /// The channel identifier.
        /// </value>
        public Guid? ChannelId { get; set; }
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public List<LanguageCode> Languages { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether only published should be searched.
        /// </summary>
        /// <value>
        /// <c>true</c> if published should be found; otherwise, <c>false</c>.
        /// </value>
        public bool PublishedSearch { get; set; }
        /// <summary>
        /// Gets or sets how many items to skip.
        /// </summary>
        /// <value>
        /// The skip amount.
        /// </value>
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether relations should be included.
        /// </summary>
        /// <value>
        ///   <c>true</c> if relations included; otherwise, <c>false</c>.
        /// </value>
        public bool IncludedRelations { get; set; }

        /// <summary>
        /// List if sorting params.
        /// </summary>
        public List<VmSortParam> SortData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VmChannelSearchParams"/> class.
        /// </summary>
        public VmChannelSearchParams()
        {
            ServiceChannelRelations = new List<VmServiceChannelRelation>();
            SortData = new List<VmSortParam>();
            ChannelType = string.Empty;
            Name = string.Empty;
            ChannelFormIdentifier = string.Empty;
        }
    }
}
