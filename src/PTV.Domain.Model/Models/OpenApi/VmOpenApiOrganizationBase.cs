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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PTV.Framework.Attributes;
using System;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of organization - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmBaseOrganization" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiOrganizationBase" />
    public abstract class VmOpenApiOrganizationBase : VmBaseOrganization, IVmOpenApiOrganizationBase
    {
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
        [LocalizedListPropertyDuplicityForbidden("Type")]
        public virtual IList<VmOpenApiLocalizedListItem> OrganizationNames { get; set; }

        /// <summary>
        /// Display name type (Name or AlternateName). Which name type should be used as the display name for the organization (OrganizationNames list).
        /// </summary>
        [JsonProperty(Order = 14)]
        [ValidEnum(typeof(NameTypeEnum))]
        public virtual string DisplayNameType { get; set; }

        /// <summary>
        /// List of organizations phone numbers.
        /// </summary>
        [JsonProperty(Order = 21)]
        public virtual IList<V4VmOpenApiPhone> PhoneNumbers { get; set; } = new List<V4VmOpenApiPhone>();

        /// <summary>
        /// List of organizations web pages.
        /// </summary>
        [JsonProperty(Order = 22)]
        [LocalizedListPropertyDuplicityForbidden("OrderNumber")]
        public IList<VmOpenApiWebPageWithOrderNumber> WebPages { get; set; } = new List<VmOpenApiWebPageWithOrderNumber>();

        /// <summary>
        /// Publishing status (Draft, Published, Deleted, Modified and OldPublished).
        /// </summary>
        [JsonProperty(Order = 24)]
        [ValidEnum(typeof(PublishingStatus))]
        public virtual string PublishingStatus { get; set; }

        /// <summary>
        /// Business code entity identifier.
        /// </summary>
        [JsonIgnore]
        public Guid? BusinessId { get; set; }

        /// <summary>
        /// Gets or sets availble languages
        /// </summary>
        [JsonIgnore]
        public IList<string> AvailableLanguages { get; set; }

        /// <summary>
        /// Gets the base model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>base model</returns>
        protected virtual TModel GetBaseModel<TModel>() where TModel : IVmOpenApiOrganizationBase, new()
        {
            return new TModel
            {
                Id = this.Id,
                Oid = this.Oid,
                OrganizationType = this.OrganizationType,
                BusinessCode = this.BusinessCode,
                BusinessName = this.BusinessName,
                OrganizationNames = this.OrganizationNames,
                DisplayNameType = this.DisplayNameType,
                PhoneNumbers = this.PhoneNumbers,
                WebPages = this.WebPages,
                PublishingStatus = this.PublishingStatus,
                AvailableLanguages = this.AvailableLanguages,
            };
        }
    }
}
