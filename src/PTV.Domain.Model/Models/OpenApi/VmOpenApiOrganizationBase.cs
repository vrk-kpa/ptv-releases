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
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PTV.Framework.Attributes;
using System;

namespace PTV.Domain.Model.Models.OpenApi
{
    public class VmOpenApiOrganizationBase : VmBaseOrganization, IVmOpenApiOrganizationBase
    {
        /// <summary>
        /// Municipality name (like Mikkeli or Helsinki).
        /// </summary>
        [JsonProperty(Order = 3)]
        [RequiredIf("OrganizationType", "Municipality")]
        [RegularExpression(@"^[0-9]{3}$")]
        public virtual string Municipality { get; set; }

        /// <summary>
        /// Organization type (State, Municipality, RegionalOrganization, Organization, Company).
        /// </summary>
        [JsonProperty(Order = 10)]
        [ValidEnum(typeof(OrganizationTypeEnum))]
        public virtual string OrganizationType { get; set; }

        /// <summary>
        /// Organization business code (Y-tunnus).
        /// </summary>
        [JsonProperty(Order = 11)]
        [RegularExpression(@"^[0-9]{7}-[0-9]{1}$")]
        public string BusinessCode { get; set; }

        /// <summary>
        /// Organization business name (name used for business code).
        /// </summary>
        [JsonProperty(Order = 12)]
        public string BusinessName { get; set; }

        /// <summary>
        /// List of organization names.
        /// </summary>
        [JsonProperty(Order = 13)]
        [ListWithEnum(typeof(NameTypeEnum), "Type")]
        [ListPropertyMaxLength(100, "Value")]
        [ListRequiredIfProperty("Type", "DisplayNameType")]
        public virtual IReadOnlyList<VmOpenApiLocalizedListItem> OrganizationNames { get; set; }

        /// <summary>
        /// Display name type (Name or AlternateName). Which name type should be used as the display name for the organization (OrganizationNames list).
        /// </summary>
        [JsonProperty(Order = 14)]
        [ValidEnum(typeof(NameTypeEnum))]
        public virtual string DisplayNameType { get; set; }

        /// <summary>
        /// Publishing status (Draft, Published, Deleted, Modified and OldPublished).
        /// </summary>
        [JsonProperty(Order = 24)]
        [ValidEnum(typeof(PublishingStatus))]
        public virtual string PublishingStatus { get; set; }

        [JsonIgnore]
        public Guid? BusinessId { get; set; }
    }
}
