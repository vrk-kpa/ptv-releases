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
using Newtonsoft.Json;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Converters;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// Base view model of entities
    /// </summary>
    public class VmEntityBase : VmBase, IVmEntityBase
    {
        /// <summary>
        /// Entity identifier.
        /// </summary>
        [JsonConverter(typeof(StringGuidConverter))]
        public virtual Guid? Id { get; set; }
    }

    /// <summary>
    /// A basic entity with defined language.
    /// </summary>
    public class VmEntityLocalized : VmEntityBase
    {
        /// <summary>
        /// Language ID.
        /// </summary>
        public Guid? LocalizationId { get; set; }
    }

    /// <summary>
    /// Base view model of entities, with organization
    /// </summary>
    public class VmOrganizationEntityBase : VmEntityBase, IVmOrganizationInfo
    {
        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>
        /// The organization identifier.
        /// </value>
        public Guid? OrganizationId { get; set; }

        private List<Guid> organizationIds = new List<Guid>();
        /// <summary>
        /// Gets or sets the organization identifiers.
        /// </summary>
        /// <value>
        /// The organization identifiers.
        /// </value>
        public List<Guid> OrganizationIds {
            get
            {
                if (OrganizationId.HasValue)
                {
                    organizationIds.Add(OrganizationId.Value);
                }

                return organizationIds;
            }

            set => organizationIds = value;
        }
    }

    /// <summary>
    /// Base view model of entities with included publishing status
    /// </summary>
    public class VmEntityStatusBase : VmEntityBase, IVmEntityStatusBase
    {
        /// <summary>
        /// id of publishing status
        /// </summary>
        public Guid? PublishingStatusId { get; set; }
    }


    /// <summary>
    /// Base view model of entities with included publishing status
    /// </summary>
    public class VmEntityRootStatusBase : VmEntityStatusBase, IVmRootBasedEntity
    {
        /// <summary>
        /// Id of unific root node
        /// </summary>
        public Guid UnificRootId { get; set; }
    }


    /// <summary>
    ///  Base view model of entities with included publishing status and available languages
    /// </summary>
    public class VmEntityLanguageAvailable : VmEntityStatusBase
    {
        /// <summary>
        /// available languages
        /// </summary>
        public IDictionary<Guid, Guid> LanguagesAvailability { get; set; }
    }

    /// <summary>
    /// base view model of entity with locking functionality
    /// </summary>
    public class VmEntityLockBase : VmEntityBase, IVmEntityLockBase
    {
        /// <summary>
        /// indicates whether entity is locked
        /// </summary>
        public bool EntityLockedForMe { get; set; }
        /// <summary>
        /// Name of user the entity is locked by
        /// </summary>
        public string LockedBy { get; set; }

        /// <summary>
        /// Lock status of entity
        /// </summary>
        public EntityLockEnum EntityLockStatus { get; set; }
    }

    /// <summary>
    /// base view model of entity editable information
    /// </summary>
    public class VmEntityEditableBase : VmEntityBase, IVmRootBasedEntity
    {
        /// <summary>
        /// Last published id of entity
        /// </summary>
        public Guid? LastPublishedId { get; set; }
        /// <summary>
        /// Last modified id of entity
        /// </summary>
        public Guid? LastModifiedId { get; set; }
        /// <summary>
        /// Last modified of entity
        /// </summary>
        public DateTime? ModifiedOfLastModified { get; set; }
        /// <summary>
        /// Last modified of entity
        /// </summary>
        public DateTime? ModifiedOfLastPublished { get; set; }
        /// <summary>
        /// true if entity is editable
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// Id of root entity
        /// </summary>
        public Guid UnificRootId { get; set; }

        /// <summary>
        /// List of published languages of published entity
        /// </summary>
        public List<Guid> LastPublishedLanguages { get; set; }
    }

    /// <summary>
    /// Base view model for enum presented to UI
    /// </summary>
    public class VmEnumBase : VmBase, IEnumCollection
    {
        /// <summary>
        /// List of enumerations
        /// </summary>
        public IVmDictionaryItemsData<IEnumerable<IVmBase>> EnumCollection { get; set; }
    }

    /// <summary>
    /// Base view model for entities represented like enumerations
    /// </summary>
    public class VmEnumEntityBase : VmEntityBase, IEnumCollection
    {
        /// <summary>
        /// list of entity enumerations
        /// </summary>
        public IVmDictionaryItemsData<IEnumerable<IVmBase>> EnumCollection { get; set; }
    }

    /// <summary>
    /// Base view model for entities represented like enumerations with included entity status
    /// </summary>
    public class VmEnumEntityStatusBase : VmEntityStatusBase, IEnumCollection
    {
        /// <summary>
        /// list of entity enumerations
        /// </summary>
        public IVmDictionaryItemsData<IEnumerable<IVmBase>> EnumCollection { get; set; }
    }


    /// <summary>
    /// Base view model for entities represented like enumerations with included entity status
    /// </summary>
    public class VmEnumEntityRootStatusBase : VmEnumEntityStatusBase, IVmRootBasedEntity
    {
        /// <summary>
        /// Root node ID
        /// </summary>
        public Guid UnificRootId { get; set; }
    }
}
