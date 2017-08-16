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
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.Security;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// Base model for service channels
    /// <seealso cref="PTV.Domain.Model.Models.VmEnumEntityStatusBase" />
    /// <seealso cref="IVmEntitySecurity" />
    /// <seealso cref="IVmChannelDescription" />
    /// </summary>
    public class VmServiceChannel : VmEnumEntityStatusBase, IVmEntitySecurity, IVmChannelDescription, IVmRootBasedEntity, IVmChannelConnectionType
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the short description.
        /// </summary>
        /// <value>
        /// The short description.
        /// </value>
        public string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>
        /// The organization identifier.
        /// </value>
        public Guid? OrganizationId { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// <see cref="IVmEntitySecurity.Security"/>
        /// </summary>
        public ISecurityOwnOrganization Security { get; set; }

        /// <summary>
        /// Root node identifier
        /// </summary>
        public Guid UnificRootId { get; set; }

        /// <summary>
        /// Gets or sets the area information type identifier.
        /// </summary>
        /// <value>
        /// The area information type identifier.
        /// </value>
        public Guid? AreaInformationTypeId { get; set; }

        /// <summary>
        /// Gets or sets the connection type ID.
        /// </summary>
        /// <value>
        /// The connection type ID.
        /// </value>
        public Guid? ConnectionTypeId { get; set; }

        /// <summary>
        /// Gets o sets channel is set as common for published version
        /// </summary>
        public bool IsPublishedCommonType { get; set; }

        /// <summary>
        /// Language availabilities informations
        /// </summary>
        public List<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }
    }

    /// <summary>
    /// Base model for service channels
    /// <seealso cref="PTV.Domain.Model.Models.VmEnumEntityStatusBase" />
    /// <seealso cref="IVmEntitySecurity" />
    /// <seealso cref="IVmChannelDescription" />
    /// </summary>
    public class VmServiceChannelPublish : VmPublishingResultModel, IVmChannelConnectionType
    {
        /// <summary>
        /// Root node identifier
        /// </summary>
        [JsonIgnore]
        public Guid UnificRootId { get; set; }
        /// <summary>
        /// Gets o sets channel is set as common for published version
        /// </summary>
        public bool IsPublishedCommonType { get; set; }
    }
}