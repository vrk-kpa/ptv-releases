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
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of channel list item
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmVersioned" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmEntityType" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmChannelListItem" />
    public class VmChannelListItem : IVmChannelListItem, IVmVersioned, IVmEntityType, IVmRootBasedEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of unific root.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid UnificRootId { get; set; }
        /// <summary>
        /// Gets or sets the root identifier.
        /// </summary>
        /// <value>
        /// The root identifier.
        /// </value>
        public Guid RootId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public Dictionary<string, string> Name { get; set; }
        /// <summary>
        /// Gets or sets the type id.
        /// </summary>
        /// <value>
        /// The id of type.
        /// </value>
        public Guid TypeId { get; set; }
        /// <summary>
        /// Gets or sets the modified in ticks.
        /// </summary>
        /// <value>
        /// The modified.
        /// </value>
        public long Modified { get; set; }
        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        /// <value>
        /// The modified by.
        /// </value>
        public string ModifiedBy { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected { get; set; }
        /// <summary>
        /// Gets or sets the connected services.
        /// </summary>
        /// <value>
        /// The connected services.
        /// </value>
        public int ConnectedServices { get; set; }
        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public VmPhone PhoneNumber { get; set; }
        /// <summary>
        /// Gets or sets the form identifier.
        /// </summary>
        /// <value>
        /// The form identifier.
        /// </value>
        public string FormIdentifier { get; set; }
        /// <summary>
        /// Gets or sets the publishing status.
        /// </summary>
        /// <value>
        /// The publishing status.
        /// </value>
        public Guid? PublishingStatusId { get; set; }
        /// <summary>
        /// Gets or sets the languages availabilities.
        /// </summary>
        /// <value>
        /// The languages availabilities.
        /// </value>
        public IReadOnlyList<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public VmVersion Version { get; set; }
        /// <summary>
        /// Gets or sets the type of the main entity.
        /// </summary>
        /// <value>
        /// The type of the main entity.
        /// </value>
        public EntityTypeEnum EntityType { get; set; }
        
        /// <summary>
        /// Gets or sets the expire on.
        /// </summary>
        /// <value>
        /// The expire on.
        /// The expire on.
        /// </value>
        public long ExpireOn { get; set; }
        /// <summary>
        /// Gets or sets the type of the sub entity.
        /// </summary>
        /// <value>
        /// The type of the sub entity.
        /// </value>
        public string SubEntityType { get; set; }
        /// <summary>
        /// Gets or sets the organization id.
        /// </summary>
        /// <value>
        /// The organization id.
        /// </value>
        public Guid OrganizationId { get; set; }
    }
}
