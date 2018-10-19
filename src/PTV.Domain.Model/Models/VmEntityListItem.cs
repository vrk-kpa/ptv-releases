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
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of entity list item
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmEntityListItem" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmRootBasedEntity" />
    public class VmEntityListItem : IVmEntityListItem, IVmRootBasedEntity
    {
        /// <summary>
        /// constructor
        /// </summary>
        public VmEntityListItem()
        {
            Producers = new List<Guid>();
            Organizations = new List<Guid>();
        }
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
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public Dictionary<string, string> Name { get; set; }
        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public SearchEntityTypeEnum EntityType { get; set; }
        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public SearchEntityTypeEnum SubEntityType { get; set; }
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
        /// Gets or sets the expire on.
        /// </summary>
        /// <value>
        /// The expire on.
        /// The expire on.
        /// </value>
        public long? ExpireOn { get; set; }
        /// <summary>
        /// Gets or sets the organization id.
        /// </summary>
        /// <value>
        /// The organization id.
        /// </value>
        public Guid? OrganizationId { get; set; }
        /// <summary>
        /// Gets or sets the organizationIds.
        /// </summary>
        /// <value>
        /// The organizations ids.
        /// </value>
        public List<Guid> Organizations { get; set; }
        /// <summary>
        /// Gets or sets the producerIds.
        /// </summary>
        /// <value>
        /// The producers ids.
        /// </value>
        public List<Guid> Producers { get; set; }
        
    }
}
