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
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of organization - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmBaseOrganization" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiOrganizationBase" />
    public abstract class VmOpenApiOrganizationBase : VmBaseOrganization, IVmOpenApiOrganizationBase
    {
        private IList<string> _availableLanguages;

        /// <summary>
        /// Organization type (State, Municipality, RegionalOrganization, Organization, Company).
        /// </summary>
        [JsonProperty(Order = 4)]
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
        [LocalizedListPropertyDuplicityForbidden("Type")]
        public virtual IList<VmOpenApiLocalizedListItem> OrganizationNames { get; set; }

        /// <summary>
        /// List of Display name types (Name or AlternateName) for each language version of OrganizationNames.
        /// </summary>
        [JsonProperty(Order = 14)]
        public virtual IList<VmOpenApiNameTypeByLanguage> DisplayNameType { get; set; } = new List<VmOpenApiNameTypeByLanguage>();

        /// <summary>
        /// Area type (WholeCountry, WholeCountryExceptAlandIslands, AreaType). 
        /// </summary>
        [JsonProperty(Order = 16)]
        [ValidEnum(typeof(AreaInformationTypeEnum))]
        public virtual string AreaType { get; set; }

        /// <summary>
        /// Localized list of organization descriptions.
        /// </summary>
        [JsonProperty(Order = 20)]
        [ListValueNotEmpty("Value")]
        [ListPropertyAllowedValues(propertyName: "Type", allowedValues: new[] { "Description", "ShortDescription" })]
        [ListPropertyMaxLength(2500, "Value", "Description")]
        [ListPropertyMaxLength(150, "Value", "ShortDescription")]
        [LocalizedListPropertyDuplicityForbidden("Type")]
        public IList<VmOpenApiLocalizedListItem> OrganizationDescriptions { get; set; } = new List<VmOpenApiLocalizedListItem>();

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
        /// Publishing status (Draft, Published, Deleted or Modified).
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
        /// Gets or sets available languages
        /// </summary>
        [JsonIgnore]
        public IList<string> AvailableLanguages
        { 
            get {
                // Return available languages or calculate from organizationNames

                return this._availableLanguages;
            }
            set
            {
                this._availableLanguages = value;
            }
        }

        /// <summary>
        /// Gets available languages by version
        /// </summary>
        /// <param name="version">Open Api version</param>
        /// <returns>List of available languages by version</returns>
        public IList<string> GetAvailableLanguages(int version)
        {
            var list = new HashSet<string>();

            if (this._availableLanguages == null)
            {
                list.GetAvailableLanguages(this.OrganizationNames);

                if (version >= 7)
                {
                    list.GetAvailableLanguages(this.OrganizationDescriptions);
                }

                this._availableLanguages = list.ToList();
            }

            return this._availableLanguages;
        }

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
                AreaType = this.AreaType,
                PhoneNumbers = this.PhoneNumbers,
                WebPages = this.WebPages,
                PublishingStatus = this.PublishingStatus,
                AvailableLanguages = this.AvailableLanguages,
            };
        }
    }
}
