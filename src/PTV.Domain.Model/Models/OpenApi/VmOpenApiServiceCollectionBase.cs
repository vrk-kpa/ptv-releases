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
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service collection - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceCollectionBase" />
    public class VmOpenApiServiceCollectionBase : IVmOpenApiServiceCollectionBase
    {
        private IList<string> _availableLanguages;

        /// <summary>
        /// PTV service identifier.
        /// </summary>
        [JsonProperty(Order = 1)]
        public virtual Guid? Id { get; set; }

        /// <summary>
        /// External system identifier for the entity. User needs to be logged in to be able to get/set value.
        /// </summary>
        [JsonProperty(Order = 1)]
        [RegularExpression(ValidationConsts.ExternalSource)]
        public virtual string SourceId { get; set; }

        /// <summary>
        /// List of localized service collection names.
        /// </summary>
        [JsonProperty(Order = 5)]
        [ListPropertyMaxLength(100, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual IList<VmOpenApiLanguageItem> ServiceCollectionNames { get; set; }

        /// <summary>
        /// List of localized service collection descriptions.
        /// </summary>
        [JsonProperty(Order = 10)]
        [ListValueNotEmpty("Value")]
        [LocalizedListPropertyDuplicityForbidden("Type")]
        public virtual IList<VmOpenApiLocalizedListItem> ServiceCollectionDescriptions { get; set; }

        /// <summary>
        /// Publishing status. Possible values are: Draft, Published, Deleted or Modified.
        /// </summary>
        [JsonProperty(Order = 40)]
        [ValidEnum(typeof(PublishingStatus))]
        [Required]
        public virtual string PublishingStatus { get; set; }

        /// <summary>
        /// Gets or sets available languages
        /// </summary>
        [JsonIgnore]
        public IList<string> AvailableLanguages {
            get
            {
                // Return available languages or calculate from ServiceNames and ServiceDescriptions
                if (this._availableLanguages == null)
                {
                    var list = new HashSet<string>();
                    list.GetAvailableLanguages(this.ServiceCollectionNames);
                    list.GetAvailableLanguages(this.ServiceCollectionDescriptions);

                    this._availableLanguages = list.ToList();
                }

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
        /// Gets the base model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>base model</returns>
        protected virtual TModel GetBaseModel<TModel>() where TModel : IVmOpenApiServiceCollectionBase, new()
        {
            var model = new TModel
            {
                Id = this.Id,
                SourceId = this.SourceId,
                ServiceCollectionNames = this.ServiceCollectionNames,
                ServiceCollectionDescriptions = this.ServiceCollectionDescriptions,
                PublishingStatus = this.PublishingStatus,
                AvailableLanguages = this.AvailableLanguages,
            };

            return model;
        }
    }
}
