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
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework.Attributes;
using PTV.Framework.Extensions;
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
        private IList<VmOpenApiLocalizedListItem> _organizationNames;

        /// <summary>
        /// Organization external system identifier. User needs to be logged in to be able to get/set value.
        /// </summary>
        [JsonProperty(Order = 1)]
        [RegularExpression(ValidationConsts.ExternalSource)]
        public virtual string SourceId { get; set; }

        /// <summary>
        /// Organization type (State, Municipality, RegionalOrganization, Organization, Company, SotePublic, SotePrivate, Region).
        /// </summary>
        [JsonProperty(Order = 5)]
        public virtual string OrganizationType { get; set; }

        /// <summary>
        /// Organization business code (Y-tunnus).
        /// </summary>
        [JsonProperty(Order = 11)]
        [RegularExpression(ValidationConsts.BusinessCode)]
        public string BusinessCode { get; set; }

        /// <summary>
        /// Organization business name (name used for business code).
        /// </summary>
        [JsonProperty(Order = 12)]
        public string BusinessName { get; set; }

        /// <summary>
        /// List of organization names. Possible type values are: Name, AlternativeName (in version 7 AlternateName).
        /// </summary>
        [JsonProperty(Order = 13)]
        [LocalizedListPropertyDuplicityForbidden("Type")]
        public virtual IList<VmOpenApiLocalizedListItem> OrganizationNames
        {
            get { return _organizationNames; }
            set
            {
                if (value == null)
                {
                    _organizationNames = null;
                    return;
                }
                
                var trimmedList = new List<VmOpenApiLocalizedListItem>();
                foreach (var item in value)
                {
                    if (item?.Value?.Length > 100)
                    {
                        item.Value = item.Value.Substring(0, 100);
                    }
                    trimmedList.Add(item);
                }

                _organizationNames = trimmedList;
            }
        }

        /// <summary>
        /// List of Display name types (Name or AlternativeName) for each language version of OrganizationNames.
        /// </summary>
        [JsonProperty(Order = 14)]
        public virtual IList<VmOpenApiNameTypeByLanguage> DisplayNameType { get; set; } = new List<VmOpenApiNameTypeByLanguage>();

        /// <summary>
        /// Area type (Nationwide, NationwideExceptAlandIslands, LimitedType). 
        /// </summary>
        [JsonProperty(Order = 16)]
        public virtual string AreaType { get; set; }

        /// <summary>
        /// Localized list of organization descriptions. Possible type values are: Description, Summary (in version 7 ShortDescription).
        /// </summary>
        [JsonProperty(Order = 20)]
        [ListValueNotEmpty("Value")]
        [ListPropertyAllowedValues(propertyName: "Type", allowedValues: new[] { "Description", "ShortDescription" })]
        [ListPropertyMaxLength(2500, "Value", "Description")]
        [ListPropertyMaxLength(150, "Value", "ShortDescription")]
        [LocalizedListPropertyDuplicityForbidden("Type")]
        public virtual IList<VmOpenApiLocalizedListItem> OrganizationDescriptions { get; set; } = new List<VmOpenApiLocalizedListItem>();

        /// <summary>
        /// List of email addresses.
        /// </summary>
        [JsonProperty(Order = 21)]
        [EmailAddressList("Value")]
        public virtual IList<V4VmOpenApiEmail> Emails { get; set; } = new List<V4VmOpenApiEmail>();

        /// <summary>
        /// List of organizations phone numbers.
        /// </summary>
        [JsonProperty(Order = 22)]
        public virtual IList<V4VmOpenApiPhone> PhoneNumbers { get; set; } = new List<V4VmOpenApiPhone>();

        /// <summary>
        /// List of organizations web pages.
        /// </summary>
        [JsonProperty(Order = 23)]
        public virtual IList<V9VmOpenApiWebPage> WebPages { get; set; } = new List<V9VmOpenApiWebPage>();

        /// <summary>
        /// List of organizations electronic invoicing information.
        /// </summary>
        [JsonProperty(Order = 24)]
        public virtual IList<VmOpenApiOrganizationEInvoicing> ElectronicInvoicings { get; set; } = new List<VmOpenApiOrganizationEInvoicing>();

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
            get
            {
                // Return available languages or calculate from organizationNames

                return this._availableLanguages;
            }
            set
            {
                this._availableLanguages = value;
            }
        }

        /// <summary>
        /// The identifier for current version.
        /// </summary>
        [JsonIgnore]
        public Guid? VersionId { get; set; }

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
                SourceId = this.SourceId,
                Id = this.Id,
                Oid = this.Oid,
                OrganizationType = this.OrganizationType,
                BusinessCode = this.BusinessCode,
                BusinessName = this.BusinessName,
                OrganizationNames = this.OrganizationNames,
                DisplayNameType = this.DisplayNameType,
                AreaType = this.AreaType,
                Emails = this.Emails,
                PhoneNumbers = this.PhoneNumbers,
                WebPages = this.WebPages,
                AvailableLanguages = this.AvailableLanguages,
                ElectronicInvoicings = this.ElectronicInvoicings
            };
        }
    }
}
