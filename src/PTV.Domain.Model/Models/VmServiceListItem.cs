/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using PTV.Framework;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of service list item
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmVersioned" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmEntityType" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmServiceListItem" />
    public class VmServiceListItem : IVmServiceListItem, IVmVersioned, IVmEntityType, IVmRootBasedEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public Dictionary<string, string> Name { get; set; }
        /// <summary>
        /// Gets or sets the service classes.
        /// </summary>
        /// <value>
        /// The service classes.
        /// </value>
        public List<Guid> ServiceClasses { get; set; }
        /// <summary>
        /// Gets or sets the ontology terms.
        /// </summary>
        /// <value>
        /// The ontology terms.
        /// </value>
        public List<VmListItem> OntologyTerms { get; set; }
        /// <summary>
        /// Gets or sets the modified.
        /// </summary>
        /// <value>
        /// The modified.
        /// </value>
        public long Modified { get; set; }
        /// <summary>
        /// Gets or sets the expire on.
        /// </summary>
        /// <value>
        /// The expire on.
        /// The expire on.
        /// </value>
        public long ExpireOn { get; set; }
        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        /// <value>
        /// The modified by.
        /// </value>
        public string ModifiedBy { get; set; }
        /// <summary>
        /// Gets or sets the connected channels count.
        /// </summary>
        /// <value>
        /// The connected channels count.
        /// </value>
        public int ConnectedChannelsCount { get; set; }
        /// <summary>
        /// Gets or sets the service type identifier.
        /// </summary>
        /// <value>
        /// The service type identifier.
        /// </value>
        public Guid? ServiceType { get; set; }
        /// <summary>
        /// Gets or sets the publishing status.
        /// </summary>
        /// <value>
        /// The publishing status.
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
            get { return EntityTypeEnum.Service; }
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
            get { return EntityTypeEnum.Service.ToString(); }
            set { }
        }

        /// <summary>
        /// Gets or sets the unific root identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid UnificRootId { get; set; }

        /// <summary>
        /// Gets or sets the organizations.
        /// </summary>
        /// <value>
        /// The organizations.
        /// </value>
        public List<Guid> Organizations { get; set; }

        /// <summary>
        /// Gets or sets the general description.
        /// </summary>
        /// <value>
        /// The general description.
        /// </value>
        public Guid? GeneralDescriptionTypeId { get; set; }

        /// <summary>
        /// Gets or sets the producers.
        /// </summary>
        /// <value>
        /// The organizations.
        /// </value>
        public List<Guid> Producers { get; set; }
    }

    /// <summary>
    /// View model of language availability info
    /// </summary>
    public class VmLanguageAvailabilityInfo
    {
        /// <summary>
        /// Gets or sets the language identifier.
        /// </summary>
        /// <value>
        /// The language identifier.
        /// </value>
        public Guid LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the status identifier.
        /// </summary>
        /// <value>
        /// The status identifier.
        /// </value>
        public Guid StatusId { get; set; }
        /// <summary>
        /// Gets or sets the modified by
        /// </summary>
        /// <value>
        /// Who modified language availability
        /// </value>
        public string ModifiedBy { get; set; }
        /// <summary>
        /// Gets or sets the modified at
        /// </summary>
        /// <value>
        /// When was language availability modified
        /// </value>
        public long Modified { get; set; }

        /// <summary>
        /// Indicates whether the language can be published or not
        /// </summary>
        /// <value>
        /// wheter can be publishet or not
        /// </value>
        public bool CanBePublished { get; set; } = true;
        /// <summary>
        /// Fields that are not filled for publish
        /// </summary>
        /// <value>
        /// Fields that are not filled for publish
        /// </value>
        public List<ValidationMessage> ValidatedFields { get; set; }

        /// <summary>
        /// Field that set that language is new
        /// </summary>
        /// <value>
        /// Field that set that language is new
        /// </value>
        public bool IsNew { get; set; }

        /// <summary>
        /// Field that set that Valid from for scheduling
        /// </summary>
        /// <value>
        /// Field that set that Valid from for scheduling
        /// </value>
        public long? ValidFrom { get; set; }

        /// <summary>
        /// Field that set that Valid to for scheduling
        /// </summary>
        /// <value>
        /// Field that set that Valid to for scheduling
        /// </value>
        public long? ValidTo { get; set; }
        /// <summary>
        /// The date when the last publish failed.
        /// </summary>
        public long? LastFailedPublishDate { get; set; }
    }
}
