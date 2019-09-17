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
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Models.OpenApi.V4;
using System.Linq;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of general description - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiGeneralDescriptionBase" />
    public class VmOpenApiGeneralDescriptionBase : IVmOpenApiGeneralDescriptionBase
    {
        private IList<string> _availableLanguages;

        /// <summary>
        /// Entity Guid identifier.
        /// </summary>
        [JsonProperty(Order = 1)]
        public virtual Guid? Id { get; set; }

        /// <summary>
        /// External system identifier. User needs to be logged in to be able to get/set value.
        /// </summary>
        [JsonIgnore]
        public virtual string SourceId { get; set; }

        /// <summary>
        /// List of localized names. Possible type values are: Name, AlternativeName (in version 7 AlternateName).
        /// </summary>
        [JsonProperty(Order = 2)]
        [ListPropertyMaxLength(100, "Value")]
        public virtual IList<VmOpenApiLocalizedListItem> Names { get; set; }

        /// <summary>
        /// List of localized descriptions. Possible type values are: Description, Summary, BackgroundDescription, UserInstruction, GeneralDescriptionTypeAdditionalInformation, ChargeTypeAdditionalInfo, DeadLine, ProcessingTime, ValidityTime.
        /// </summary>
        [JsonProperty(Order = 3)]
        public virtual IList<VmOpenApiLocalizedListItem> Descriptions { get; set; }

        /// <summary>
        /// Localized service usage requirements (description of requirement).
        /// </summary>
        [JsonProperty(Order = 10)]
        [ListPropertyMaxLength(2500, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual IList<VmOpenApiLanguageItem> Requirements { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// Service type. Possible values in version 8 are: Service, PermitOrObligation or ProfessionalQualification.
        /// In version 7: Service, PermissionAndObligation or ProfessionalQualifications.
        /// In older versions: Service or PermissionAndObligation.
        /// </summary>
        [JsonProperty(Order = 11)]
        public virtual string Type { get; set; }

        /// <summary>
        /// Service charge type. Possible values are:  Chargeable or FreeOfCharge.
        /// In version 7 and older: Charged, Free or Other
        /// </summary>
        [JsonProperty(Order = 12)]
        public virtual string ServiceChargeType { get; set; }

        /// <summary>
        /// Laws that a general description is based on.
        /// </summary>
        [JsonProperty(Order = 13)]
        public virtual IList<V4VmOpenApiLaw> Legislation { get; set; } = new List<V4VmOpenApiLaw>();

        /// <summary>
        /// General description type. Possible values in version 9 are: Municipality, BusinessSubregion, AlandIsland, PrescribedByFreedomOfChoiceAct, OtherPermissionGrantedSote.
        /// In version 8: Municipality, BusinessSubregion, AlandIsland.
        /// In older versions: Default general description is Municipality.
        /// </summary>
        [JsonProperty(Order = 14)]
        [AllowedValues(propertyName: "GeneralDescriptionType", allowedValues: new[] { "Municipality", "BusinessSubregion", "AlandIsland" })]
        public virtual string GeneralDescriptionType { get; set; }

        /// <summary>
        /// General description type id. Used internally to check the restrictions for usage.
        /// In older versions: Default general description is Municipality.
        /// </summary>
        [JsonIgnore]
        public Guid GeneralDescriptionTypeId { get; set; }

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
        public IList<string> AvailableLanguages
        {
            get
            {
                // Return available languages or calculate from Names and Descriptions
                if (this._availableLanguages == null)
                {
                    var list = new HashSet<string>();
                    list.GetAvailableLanguages(this.Names);
                    list.GetAvailableLanguages(this.Descriptions);

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

// SOTE has been disabled (SFIPTV-1177)        
//        /// <summary>
//        /// Gets if general description is SOTE type
//        /// </summary>
//        [JsonIgnore]
//        public bool IsSoteType => GeneralDescriptionType == GeneralDescriptionTypeEnum.PrescribedByFreedomOfChoiceAct.ToString() || GeneralDescriptionType == GeneralDescriptionTypeEnum.OtherPermissionGrantedSote.ToString();

        /// <summary>
        /// Gets general description base model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        protected virtual TModel GetBaseModel<TModel>() where TModel : IVmOpenApiGeneralDescriptionBase, new()
        {
            return new TModel
            {
                Id = this.Id,
                Names = this.Names,
                Descriptions = this.Descriptions,
                Requirements = this.Requirements,
                Type = this.Type,
                GeneralDescriptionType = this.GeneralDescriptionType,
                ServiceChargeType = this.ServiceChargeType,
                Legislation = this.Legislation,
                PublishingStatus = this.PublishingStatus,
            };
        }
    }
}