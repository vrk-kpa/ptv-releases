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

using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V7;
using PTV.Framework.Attributes;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V7
{
    /// <summary>
    /// OPEN API V7 - View Model of service for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V7.IV7VmOpenApiServiceInBase" />
    public class V7VmOpenApiServiceInBase : VmOpenApiServiceInVersionBase, IV7VmOpenApiServiceInBase
    {
        /// <summary>
        /// Valid PTV statutory service general description identifier that this service will be linked to. List of valid identifiers can be retrieved from the endpoint /api/GeneralDescription
        /// </summary>
        [Display(Name = "StatutoryServiceGeneralDescriptionId")]
        [JsonProperty(Order = 2, PropertyName = "statutoryServiceGeneralDescriptionId")]
        public override string GeneralDescriptionId { get => base.GeneralDescriptionId; set => base.GeneralDescriptionId = value; }

        /// <summary>
        /// Service type. Possible values are: Service, PermissionAndObligation or ProfessionalQualifications.
        /// NOTE! If service type has been defined within attached statutory service general description, the type for service is ignored.
        /// </summary>
        [ValidEnum(typeof(ServiceTypeEnum))]
        [RequiredIf("DeleteGeneralDescriptionId", true)]
        public override string Type { get => base.Type; set => base.Type = value; }

        /// <summary>
        /// List of localized service names.
        /// </summary>
        [JsonProperty(Order = 5)]
        [ListWithEnum(typeof(NameTypeEnum), "Type")]
        public override IList<VmOpenApiLocalizedListItem> ServiceNames { get => base.ServiceNames; set => base.ServiceNames = value; }

        /// <summary>
        /// Service charge type. Possible values are: Charged or Free.
        /// NOTE! If service charge type has been defined within attached statutory service general description, the charge type for service is ignored.
        /// </summary>
        [JsonProperty(Order = 6)]
        [AllowedValues("ServiceChargeType", new[] { "Charged", "Free" })]
        public override string ServiceChargeType { get => base.ServiceChargeType; set => base.ServiceChargeType = value; }

        /// <summary>
        /// Area type (WholeCountry, WholeCountryExceptAlandIslands, AreaType). 
        /// </summary>
        [JsonProperty(Order = 7)]
        [ValidEnum(typeof(AreaInformationTypeEnum))]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// List of areas. List can contain different types of areas.
        /// </summary>
        [JsonProperty(Order = 8)]
        [ListRequiredIf("AreaType", "AreaType")]
        [ListWithEnum(typeof(AreaTypeEnum), "Type")]
        public override IList<VmOpenApiAreaIn> Areas { get => base.Areas; set => base.Areas = value; }

        /// <summary>
        /// List of localized service descriptions.
        /// </summary>
        [JsonProperty(Order = 10)]
        [ListPropertyAllowedValues(propertyName: "Type", allowedValues: new[] {"Description", "ShortDescription", "ServiceUserInstruction", "ValidityTimeAdditionalInfo",
            "ProcessingTimeAdditionalInfo", "DeadLineAdditionalInfo", "ChargeTypeAdditionalInfo", "ServiceTypeAdditionalInfo"})]
        [ListPropertyMaxLength(2500, "Value", "Description")]
        [ListPropertyMaxLength(2500, "Value", "ServiceUserInstruction")]
        [ListPropertyMaxLength(150, "Value", "ShortDescription")]
        [ListPropertyMaxLength(500, "Value", "ProcessingTimeAdditionalInfo")]
        [ListPropertyMaxLength(500, "Value", "DeadLineAdditionalInfo")]
        [ListPropertyMaxLength(500, "Value", "ChargeTypeAdditionalInfo")]
        [ListPropertyMaxLength(500, "Value", "ValidityTimeAdditionalInfo")]
        [ListPropertyMaxLength(500, "Value", "ServiceTypeAdditionalInfo")]
        public override IList<VmOpenApiLocalizedListItem> ServiceDescriptions { get => base.ServiceDescriptions; set => base.ServiceDescriptions = value; }

        /// <summary>
        /// List of service producers
        /// </summary>
        [JsonProperty(Order = 31)]
        [ListWithEnum(typeof(ProvisionTypeEnum), "ProvisionType")]
        public override IList<VmOpenApiServiceProducerIn> ServiceProducers { get => base.ServiceProducers; set => base.ServiceProducers = value; }

        /// <summary>
        /// Set to true to delete statutory service general description (StatutoryServiceGeneralDescriptionId property for this object should be empty when this option is used).
        /// </summary>
        [Display(Name = "DeleteStatutoryServiceGeneralDescriptionId")]
        [JsonProperty(Order = 51, PropertyName = "deleteStatutoryServiceGeneralDescriptionId")]
        public override bool DeleteGeneralDescriptionId { get => base.DeleteGeneralDescriptionId; set => base.DeleteGeneralDescriptionId = value; }

        /// <summary>
        /// Service coverage type. Possible values are: Local or Nationwide.
        /// </summary>
        [JsonIgnore]
        public override string ServiceCoverageType { get => base.ServiceCoverageType; set => base.ServiceCoverageType = value; }

        /// <summary>
        /// List of municipality codes that the service is available for. Used in conjunction with service coverage type Local.
        /// </summary>
        [JsonIgnore]
        public override IList<string> Municipalities { get => base.Municipalities; set => base.Municipalities = value; }

        /// <summary>
        /// Set to true to delete all existing municipalities (the Municipalities collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllMunicipalities { get => base.DeleteAllMunicipalities; set => base.DeleteAllMunicipalities = value; }

        /// <summary>
        /// Set to true to delete all existing service vouchers (the ServiceVouchers collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllServiceVouchers { get => base.DeleteAllServiceVouchers; set => base.DeleteAllServiceVouchers = value; }

        #region methods

        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 7;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceInVersionBase VersionBase()
        {
            return base.GetInVersionBaseModel<VmOpenApiServiceInVersionBase>(VersionNumber());
        }
        #endregion
    }
}
