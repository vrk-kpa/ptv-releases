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
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceBase" />
    public class VmOpenApiServiceBase : IVmOpenApiServiceBase
    {
        private IList<string> _availableLanguages;

        /// <summary>
        /// PTV service identifier.
        /// </summary>
        [JsonProperty(Order = 1)]
        public virtual Guid? Id { get; set; }

        /// <summary>
        /// Service type. Possible values are: Service or PermissionAndObligation. In version 6 (and newer) also ProfessionalQualifications.
        /// NOTE! Current PTV database does not anymore support for types Notice, Registration or Permission - they are automatically mapped into PermissionAndObligation type when possible.
        /// POST and PUT methods accepts old types but GET method only can return Service or PermissionAndObligation types.
        /// </summary>
        [JsonProperty(Order = 3)]
        public virtual string Type { get; set; }

        /// <summary>
        /// Service funding type. Possible values are: PubliclyFunded or MarketFunded.
        /// </summary>
        [JsonProperty(Order = 4)]
        [ValidEnum(typeof(ServiceFundingTypeEnum))]
        public virtual string FundingType { get; set; }

        /// <summary>
        /// List of localized service names.
        /// </summary>
        [JsonProperty(Order = 5)]
        [ListWithEnum(typeof(NameTypeEnum), "Type")]
        [ListPropertyMaxLength(100, "Value")]
        [LocalizedListPropertyDuplicityForbidden("Type")]
        public virtual IList<VmOpenApiLocalizedListItem> ServiceNames { get; set; }

        /// <summary>
        /// Service charge type. Possible values are: Charged, Free or Other.
        /// NOTE! If service charge type has been defined within attached statutory service general description, the charge type for service is ignored.
        /// </summary>
        [JsonProperty(Order = 6)]
        [ValidEnum(typeof(ServiceChargeTypeEnum))]
        [AllowedValues("ServiceChargeType", new[] { "Charged", "Free"})]
        public virtual string ServiceChargeType { get; set; }

        /// <summary>
        /// Area type (WholeCountry, WholeCountryExceptAlandIslands, AreaType). 
        /// </summary>
        [JsonProperty(Order = 7)]
        [ValidEnum(typeof(AreaInformationTypeEnum))]
        public virtual string AreaType { get; set; }
        
        /// <summary>
        /// List of localized service descriptions.
        /// </summary>
        [JsonProperty(Order = 10)]
        [ListValueNotEmpty("Value")]
        [ListPropertyAllowedValues(propertyName: "Type", allowedValues: new[] {"Description", "ShortDescription", "ServiceUserInstruction", "ValidityTimeAdditionalInfo",
            "ProcessingTimeAdditionalInfo", "DeadLineAdditionalInfo", "ChargeTypeAdditionalInfo", "ServiceTypeAdditionalInfo"})]
        [ListPropertyMaxLength(2500, "Value", "Description")]
        [ListPropertyMaxLength(2500, "Value", "ServiceUserInstruction")]
        [ListPropertyMaxLength(150, "Value", "ShortDescription")]
        [ListPropertyMaxLength(500, "Value", "ProcessingTimeAdditionalInfo")]
        [ListPropertyMaxLength(500, "Value", "DeadLineAdditionalInfo")]
        [ListPropertyMaxLength(500, "Value", "ChargeTypeAdditionalInfo")]
        [ListPropertyMaxLength(500, "Value", "ValidityTimeAdditionalInfo")]
        [LocalizedListPropertyDuplicityForbidden("Type")]
        public virtual IList<VmOpenApiLocalizedListItem> ServiceDescriptions { get; set; }

        /// <summary>
        /// List of service languages.
        /// </summary>
        [JsonProperty(Order = 11)]
        [ListRegularExpression(@"^[a-z]{2}$")]
        public virtual IList<string> Languages { get; set; }

        /// <summary>
        /// List of laws related to the service.
        /// </summary>
        [JsonProperty(Order = 20)]
        public virtual IList<V4VmOpenApiLaw> Legislation { get; set; } = new List<V4VmOpenApiLaw>();

        /// <summary>
        /// List of localized service keywords.
        /// </summary>
        [ListPropertyMaxLength(150, "Value")]
        [JsonProperty(Order = 21)]
        public IList<VmOpenApiLanguageItem> Keywords { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// Service coverage type. Valid values are: Local or Nationwide.
        /// </summary>
        [JsonProperty(Order = 22)]
        [ValidEnum(typeof(ServiceCoverageTypeEnum))]
        public virtual string ServiceCoverageType { get; set; }

        /// <summary>
        /// Localized service usage requirements (description of requirement).
        /// </summary>
        [JsonProperty(Order = 24)]
        [ListValueNotEmpty("Value")]
        [LocalizedListLanguageDuplicityForbidden]
        [ListPropertyMaxLength(2500, "Value")]
        public virtual IList<VmOpenApiLanguageItem> Requirements { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// Publishing status. Possible values are: Draft, Published, Deleted or Modified.
        /// </summary>
        [JsonProperty(Order = 40)]
        [ValidEnum(typeof(PublishingStatus))]
        public virtual string PublishingStatus { get; set; }

        /// <summary>
        /// Gets or sets availble languages
        /// </summary>
        [JsonIgnore]
        public IList<string> AvailableLanguages {
            get
            {
                // Return available languages or calculate from ServiceNames and ServiceDescriptions
                if (this._availableLanguages == null)
                {
                    var list = new HashSet<string>();
                    list.GetAvailableLanguages(this.ServiceNames);
                    list.GetAvailableLanguages(this.ServiceDescriptions);

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
        /// Gets the base model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>base model</returns>
        protected virtual TModel GetBaseModel<TModel>() where TModel : IVmOpenApiServiceBase, new()
        {
            var model = new TModel
            {
                Id = this.Id,
                Type = this.Type,
                FundingType = this.FundingType,
                ServiceChargeType = this.ServiceChargeType,
                ServiceNames = this.ServiceNames,
                AreaType = this.AreaType,
                ServiceDescriptions = this.ServiceDescriptions,
                Legislation = this.Legislation,
                Languages = this.Languages,
                Keywords = this.Keywords,
                ServiceCoverageType = this.ServiceCoverageType,
                Requirements = this.Requirements,
                PublishingStatus = this.PublishingStatus,
                AvailableLanguages = this.AvailableLanguages,
            };

            return model;
        }
    }
}
