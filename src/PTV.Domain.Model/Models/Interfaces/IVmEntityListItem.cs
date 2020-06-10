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
using System;
using System.Collections.Generic;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models.Interfaces
{
    /// <summary>
    /// View model interface of entity list item
    /// </summary>
    public interface IVmEntityListItem : IVmBase, IVmRootBasedEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        Dictionary<string, string> Name { get; set; }
        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        SearchEntityTypeEnum EntityType { get; set; }
        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        SearchEntityTypeEnum SubEntityType { get; set; }
        /// <summary>
        /// Gets or sets the modified in ticks.
        /// </summary>
        /// <value>
        /// The modified.
        /// </value>
        long Modified { get; set; }
        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        /// <value>
        /// The modified by.
        /// </value>
        string ModifiedBy { get; set; }
        /// <summary>
        /// Gets or sets the languages availabilities.
        /// </summary>
        /// <value>
        /// The languages availabilities.
        /// </value>
        IReadOnlyList<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }
        /// <summary>
        /// Gets or sets the organization id.
        /// </summary>
        /// <value>
        /// The organization id.
        /// </value>
        Guid? OrganizationId { get; set; }
        /// <summary>
        /// Gets or sets the publishing status.
        /// </summary>
        /// <value>
        /// The publishing status.
        /// </value>
        Guid? PublishingStatusId { get; set; }
        /// <summary>
        /// Gets or sets the organizationIds.
        /// </summary>
        /// <value>
        /// The organizations ids.
        /// </value>
        List<Guid> Organizations { get; set; }
        /// <summary>
        /// Gets or sets the producerIds.
        /// </summary>
        /// <value>
        /// The producers ids.
        /// </value>
        List<Guid> Producers { get; set; }

        /// <summary>
        /// Gets or sets the expire on.
        /// </summary>
        /// <value>
        /// The expire on.
        /// The expire on.
        /// </value>
        long? ExpireOn { get; set; }

        /// <summary>
        /// Gets or sets the languages ids.
        /// </summary>
        /// <value>
        /// The languages ids.
        /// </value>
        List<Guid> MissingLanguages { get; set; }

        /// <summary>
        /// Gets or sets the original id of copied entity.
        /// </summary>
        /// <value>
        /// The original id.
        /// </value>
        Guid? OriginalId { get; set; }

        /// <summary>
        /// Gets or sets entity is copied tag.
        /// </summary>
        /// <value>
        /// The entity is copied.
        /// </value>
        bool IsCopyTagVisible { get; set; }

        /// <summary>
        /// Gets or sets the major version.
        /// </summary>
        /// <value>
        /// The  major version.
        /// </value>
        int VersionMajor { get; set; }

        /// <summary>
        /// Gets or sets the minor version.
        /// </summary>
        /// <value>
        /// The  minor version.
        /// </value>
        int VersionMinor { get; set; }

        /// <summary>
        /// Gets or sets the if entity has translation order.
        /// </summary>
        bool HasTranslationOrder { get; set; }

        /// <summary>
        /// Defines whether the entity has at least one ASTI connections.
        /// Relates only to organizations, services and channels.
        /// </summary>
        bool HasAstiConnection { get; set; }

        /// <summary>
        /// Id of the versioning record.
        /// </summary>
        Guid VersioningId { get; set; }

        /// <summary>
        /// Definition of what role did what action as the last operation.
        /// </summary>
        LastOperationType LastOperationType { get; set; }
    }
}
