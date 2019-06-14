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
    /// View model of organization list item
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmVersioned" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmEntityType" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmOrganizationListItem" />
    public class VmOrganizationListItem : IVmOrganizationListItem, IVmVersioned, IVmEntityType, IVmRootBasedEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unific root identifier.
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
        /// Gets or sets the main organization.
        /// </summary>
        /// <value>
        /// The main organization.
        /// </value>
        public Guid? MainOrganization { get; set; }
        /// <summary>
        /// Gets or sets the sub organizations.
        /// </summary>
        /// <value>
        /// The sub organizations.
        /// </value>
        public List<Guid> SubOrganizations { get; set; }
        /// <summary>
        /// Gets or sets the modified.
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
        /// Gets or sets the publishing status identifier.
        /// </summary>
        /// <value>
        /// The publishing status identifier.
        /// </value>
        public Guid PublishingStatusId { get; set; }
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
        public EntityTypeEnum EntityType
        {
            get { return EntityTypeEnum.Organization; }
            set { }
        }
        /// <summary>
        /// Gets or sets the type of the sub entity.
        /// </summary>
        /// <value>
        /// The type of the sub entity.
        /// </value>
        public string SubEntityType
        {
            get { return EntityTypeEnum.Organization.ToString(); }
            set { }
        }
    }
}
