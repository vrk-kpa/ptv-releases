﻿/**
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
    /// View model of results from search of general descritpion
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmVersioned" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmEntityType" />
    public class VmGeneralDescriptionListItem : IVmVersioned, IVmEntityType, IVmRootBasedEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the unific root id.
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
        /// Gets or sets the alternate name.
        /// </summary>
        /// <value>
        /// The alternate name.
        /// </value>
        public Dictionary<string, string> AlternateName { get; set; }

        /// <summary>
        /// Gets or sets the short description.
        /// </summary>
        /// <value>
        /// The short description.
        /// </value>
        public Dictionary<string, string> ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public Dictionary<string, string> Description { get; set; }

        /// <summary>
        /// Gets or sets the service classes.
        /// </summary>
        /// <value>
        /// The service classes.
        /// </value>
        public List<Guid> ServiceClasses { get; set; }
        /// <summary>
        /// Gets or sets the target groups.
        /// </summary>
        /// <value>
        /// The target groups.
        /// </value>
        public List<Guid> TargetGroups { get; set; }
        /// <summary>
        /// Gets or sets the languages availabilities.
        /// </summary>
        /// <value>
        /// The languages availabilities.
        /// </value>
        public List<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public VmVersion Version { get; set; }
        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public EntityTypeEnum MainEntityType
        {
            get { return EntityTypeEnum.GeneralDescription; }
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
            get { return EntityTypeEnum.GeneralDescription.ToString(); }
            set { }
        }
    }
}
