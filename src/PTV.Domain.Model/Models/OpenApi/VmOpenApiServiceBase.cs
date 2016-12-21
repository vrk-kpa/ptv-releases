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
using PTV.Framework;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    public class VmOpenApiServiceBase : IVmOpenApiServiceBase
    {
        /// <summary>
        /// PTV service identifier.
        /// </summary>
        [JsonProperty(Order = 1)]
        public virtual Guid? Id { get; set; }

        /// <summary>
        /// Service type. Possible values are: Service, Notice, Registration or Permission.
        /// </summary>
        [JsonProperty(Order = 2)]
        [ValidEnum(typeof(ServiceTypeEnum))]
        public virtual string Type { get; set; }

        /// <summary>
        /// Service charge type. Possible values are: Charged, Free or Other
        /// </summary>
        [JsonProperty(Order = 3)]
        [ValidEnum(typeof(ServiceChargeTypeEnum))]
        public virtual string ServiceChargeType { get; set; }

        /// <summary>
        /// List of localized service names.
        /// </summary>
        [JsonProperty(Order = 5)]
        [ListWithEnum(typeof(NameTypeEnum), "Type")]
        [ListPropertyMaxLength(100, "Value")]
        public virtual IReadOnlyList<VmOpenApiLocalizedListItem> ServiceNames { get; set; }

        /// <summary>
        /// List of localized service descriptions.
        /// </summary>
        [JsonProperty(Order = 6)]
        [ListWithEnum(typeof(DescriptionTypeEnum), "Type")]
        [ListPropertyMaxLength(150, "Value", "ShortDescription")]
        [ListPropertyMaxLength(4000, "Value", "Description")]
        [ListPropertyMaxLength(4000, "Value", "ServiceUserInstruction")]
        public virtual IReadOnlyList<VmOpenApiLocalizedListItem> ServiceDescriptions { get; set; }

        /// <summary>
        /// List of service languages.
        /// </summary>
        [JsonProperty(Order = 7)]
        [ListRegularExpression(@"^[a-z]{2}$")]
        public virtual IReadOnlyList<string> Languages { get; set; }

        /// <summary>
        /// List of localized service keywords.
        /// </summary>
        [JsonProperty(Order = 20)]
        public IList<VmOpenApiLanguageItem> Keywords { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// Service coverage type. Valid values are: Local or Nationwide.
        /// </summary>
        [JsonProperty(Order = 21)]
        [ValidEnum(typeof(ServiceCoverageTypeEnum))]
        public virtual string ServiceCoverageType { get; set; }

        /// <summary>
        /// List of municipality codes that the service is available for. Used in conjunction with service coverage type Local.
        /// </summary>
        [JsonProperty(Order = 22)]
        [ListRequiredIf("ServiceCoverageType", "Local")]
        [ListRegularExpression(@"^[0-9]{3}$", ErrorMessage = CoreMessages.OpenApi.MustBeNumeric)]
        public IReadOnlyList<string> Municipalities { get; set; } = new List<string>();
        
        /// <summary>
        /// Localized service usage requirements (description of requirement).
        /// </summary>
        [JsonProperty(Order = 24)]
        [ListPropertyMaxLength(4000, "Value")]
        public IReadOnlyList<VmOpenApiLanguageItem> Requirements { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// Localized service additional information.
        /// </summary>
        [JsonProperty(Order = 26)]
        [LocalizedListPropertyDuplicityForbidden("Type")]
        [ListWithEnum(typeof(DescriptionTypeEnum), "Type")]
        [ListPropertyAllowedValues(propertyName: "Type",allowedValues: new[] {"ValidityTimeAdditionalInfo", "ProcessingTimeAdditionalInfo",
            "DeadLineAdditionalInfo", "ChargeTypeAdditionalInfo", "ServiceTypeAdditionalInfo" ,"TasksAdditionalInfo"})]
        public virtual IReadOnlyList<VmOpenApiLocalizedListItem> ServiceAdditionalInformations { get; set; } = new List<VmOpenApiLocalizedListItem>();

        /// <summary>
        /// Publishing status. Possible values are: Draft, Published, Deleted, Modified or OldPublished.
        /// </summary>
        [JsonProperty(Order = 40)]
        [ValidEnum(typeof(PublishingStatus))]
        public virtual string PublishingStatus { get; set; }
    }
}
