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

using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes.ValidationAttributes;
using PTV.Domain.Model.Enums;
using PTV.Framework.Attributes;

namespace PTV.Domain.Model.Models.OpenApi.V9
{
    /// <summary>
    /// OPEN API V9 - View Model of service for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceInVersionBase" />
    public class V9VmOpenApiServiceInBase : VmOpenApiServiceInVersionBase
    {
        /// <summary>
        /// Service type. Possible values are: Service, PermitOrObligation or ProfessionalQualification.
        /// NOTE! If service type has been defined within attached statutory service general description, the type for service is ignored.
        /// </summary>
        [JsonProperty(Order = 3)]
        [ValidOpenApiEnum(typeof(ServiceTypeEnum))]
        [RequiredIf("DeleteGeneralDescriptionId", true)]
        public override string Type { get => base.Type; set => base.Type = value; }

        /// <summary>
        /// List of localized service names. Possible type values are: Name, AlternativeName.
        /// </summary>
        [ListWithOpenApiEnum(typeof(NameTypeEnum), "Type")]
        public override IList<VmOpenApiLocalizedListItem> ServiceNames { get => base.ServiceNames; set => base.ServiceNames = value; }

        /// <summary>
        /// Service charge type. Possible values are: Chargeable or FreeOfCharge.
        /// NOTE! If service charge type has been defined within attached statutory service general description, the charge type for service is ignored.
        /// </summary>
        [JsonProperty(Order = 6)]
        [AllowedValues("ServiceChargeType", new[] { "Chargeable", "FreeOfCharge" })]
        public override string ServiceChargeType { get => base.ServiceChargeType; set => base.ServiceChargeType = value; }

        /// <summary>
        /// Area type (Nationwide, NationwideExceptAlandIslands, LimitedType).
        /// </summary>
        [JsonProperty(Order = 7)]
        [ValidOpenApiEnum(typeof(AreaInformationTypeEnum))]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// List of areas. List can contain different types of areas.
        /// </summary>
        [JsonProperty(Order = 8)]
        [ListRequiredIf("AreaType", "LimitedType")]
        [ListWithOpenApiEnum(typeof(AreaTypeEnum), "Type")]
        public override IList<VmOpenApiAreaIn> Areas { get => base.Areas; set => base.Areas = value; }

        /// <summary>
        /// List of localized service descriptions. Possible type values are: Description, Summary, UserInstruction, ValidityTime, ProcessingTime, DeadLine, ChargeTypeAdditionalInfo, ServiceType.
        /// </summary>
        [JsonProperty(Order = 10)]
        [ListPropertyAllowedValues(propertyName: "Type", allowedValues: new[] {"Description", "Summary", "UserInstruction", "ValidityTime",
            "ProcessingTime", "DeadLine", "ChargeTypeAdditionalInfo", "ServiceType"})]
        [ListWithPropertyValueRequired("Type", "Summary")] // SFIPTV-847
        [ListPropertyMaxLength(2500, "Value", "Description")]
        [ListPropertyMaxLength(2500, "Value", "UserInstruction")]
        [ListPropertyMaxLength(150, "Value", "Summary")]
        [ListPropertyMaxLength(500, "Value", "ProcessingTime")]
        [ListPropertyMaxLength(500, "Value", "DeadLine")]
        [ListPropertyMaxLength(500, "Value", "ChargeTypeAdditionalInfo")]
        [ListPropertyMaxLength(500, "Value", "ValidityTime")]
        [ListPropertyMaxLength(500, "Value", "ServiceType")]
        public override IList<VmOpenApiLocalizedListItem> ServiceDescriptions { get => base.ServiceDescriptions; set => base.ServiceDescriptions = value; }

        /// <summary>
        /// List of service class urls (see http://finto.fi/ptvl/fi/).
        /// NOTE! If service class has been defined within attached statutory service general description, the service class is not added for service.
        /// </summary>
        [ListRequiredIf("GeneralDescriptionId", null)] // PTV-1374, SFIPTV-1989
        [JsonProperty(Order = 15)]
        public override IList<string> ServiceClasses { get => base.ServiceClasses; set => base.ServiceClasses = value; }

        /// <summary>
        /// List of ontology term urls (see http://finto.fi/koko/fi/).
        /// NOTE! If ontology term has been defined within attached statutory service general description, the ontology term is not added for service.
        /// </summary>
        [JsonProperty(Order = 16)]
        [ListRequiredIf("GeneralDescriptionId", null)] // PTV-1374, SFIPTV-1989
        public override IList<string> OntologyTerms { get => base.OntologyTerms; set => base.OntologyTerms = value; }

        /// <summary>
        /// List of target group urls (see http://finto.fi/ptvl/fi/page/?uri=http://urn.fi/URN:NBN:fi:au:ptvl:KR).
        /// NOTE! If target group has been defined within attached statutory service general description, the target group is not added for service.
        /// </summary>
        [JsonProperty(Order = 17)]
        [ListRequiredIf("GeneralDescriptionId", null)] // PTV-1374, SFIPTV-1989
        public override IList<string> TargetGroups { get => base.TargetGroups; set => base.TargetGroups = value; }

        /// <summary>
        /// List of service producers
        /// </summary>
        [JsonProperty(Order = 31)]
        [ListWithOpenApiEnum(typeof(ProvisionTypeEnum), "ProvisionType")]
        public override IList<V9VmOpenApiServiceProducerIn> ServiceProducers { get => base.ServiceProducers; set => base.ServiceProducers = value; }

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
            return 9;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceInVersionBase VersionBase()
        {
            var vm = base.GetInVersionBaseModel<VmOpenApiServiceInVersionBase>(VersionNumber());
            return vm;
        }
        #endregion
    }
}
