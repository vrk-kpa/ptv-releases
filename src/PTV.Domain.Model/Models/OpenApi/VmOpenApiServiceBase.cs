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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceBase" />
    public class VmOpenApiServiceBase : VmEntityBase, IVmOpenApiServiceBase
    {
        private IList<string> _availableLanguages;

        /// <summary>
        /// External system identifier for the entity. User needs to be logged in to be able to get/set value.
        /// </summary>
        [JsonProperty(Order = 1)]
        [RegularExpression(@"^[A-Za-z0-9-.]*$")]
        public virtual string SourceId { get; set; }

        /// <summary>
        /// Service type. Possible values are: Service, PermitOrObligation or ProfessionalQualification. In version 7 Service, PermissionAndObligation or ProfessionalQualifications.
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
        /// List of localized service names. Possible type values are: Name, AlternativeName (in version 7 AlternateName).
        /// </summary>
        [JsonProperty(Order = 5)]
        [ListPropertyMaxLength(100, "Value")]
        [LocalizedListPropertyDuplicityForbidden("Type")]
        public virtual IList<VmOpenApiLocalizedListItem> ServiceNames { get; set; }

        /// <summary>
        /// Service charge type. Possible values are: Chargeable or FreeOfCharge.
        /// In version 7: Charged or Free.
        /// NOTE! If service charge type has been defined within attached statutory service general description, the charge type for service is ignored.
        /// </summary>
        [JsonProperty(Order = 6)]
        public virtual string ServiceChargeType { get; set; }

        /// <summary>
        /// Area type. Possible values are: Nationwide, NationwideExceptAlandIslands or LimitedType.
        /// In version 7: WholeCountry, WholeCountryExceptAlandIslands, AreaType. 
        /// </summary>
        [JsonProperty(Order = 7)]
        public virtual string AreaType { get; set; }

        /// <summary>
        /// List of localized service descriptions. Possible type values are: Description, Summary, UserInstruction, ValidityTime, ProcessingTime, DeadLine, ChargeTypeAdditionalInfo, ServiceType.
        /// In version 7: Description, ShortDescription, ServiceUserInstruction, ValidityTimeAdditionalInfo, ProcessingTimeAdditionalInfo, DeadLineAdditionalInfo, ChargeTypeAdditionalInfo, ServiceTypeAdditionalInfo.
        /// </summary>
        [JsonProperty(Order = 10)]
        [ListValueNotEmpty("Value")]
        [LocalizedListPropertyDuplicityForbidden("Type")]
        public virtual IList<VmOpenApiLocalizedListItem> ServiceDescriptions { get; set; }

        /// <summary>
        /// List of service languages.
        /// </summary>
        [JsonProperty(Order = 11)]
        [ListRegularExpression(@"^[a-z-]*$")]
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
        /// Localized service usage requirements (description of requirement).
        /// </summary>
        [JsonProperty(Order = 24)]
        [ListValueNotEmpty("Value")]
        [LocalizedListLanguageDuplicityForbidden]
        [ListPropertyMaxLength(2500, "Value")]
        public virtual IList<VmOpenApiLanguageItem> Requirements { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// Indicates if service vouchers are used in the service.
        /// </summary>
        [JsonProperty(Order = 31)]
        public virtual bool ServiceVouchersInUse { get; set; }

        /// <summary>
        /// List of service vouchers.
        /// </summary>
        [JsonProperty(Order = 32)]
        public virtual IList<V9VmOpenApiServiceVoucher> ServiceVouchers { get; set; } = new List<V9VmOpenApiServiceVoucher>();

        /// <summary>
        /// Gets or sets availble languages
        /// </summary>
        [JsonIgnore]
        public IList<string> AvailableLanguages
        {
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
                SourceId = this.SourceId,
                Type = this.Type,
                FundingType = this.FundingType,
                ServiceChargeType = this.ServiceChargeType,
                ServiceNames = this.ServiceNames,
                AreaType = this.AreaType,
                ServiceDescriptions = this.ServiceDescriptions,
                Legislation = this.Legislation,
                Languages = this.Languages,
                Keywords = this.Keywords,
                Requirements = this.Requirements,
                ServiceVouchersInUse = this.ServiceVouchersInUse,
                ServiceVouchers = this.ServiceVouchers,
                AvailableLanguages = this.AvailableLanguages,
            };

            return model;
        }
    }
}