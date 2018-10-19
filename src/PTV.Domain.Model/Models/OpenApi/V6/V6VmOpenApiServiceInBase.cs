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
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V6;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Domain.Model.Models.OpenApi.Extensions;

namespace PTV.Domain.Model.Models.OpenApi.V6
{
    /// <summary>
    /// OPEN API V6 - View Model of service for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V6.IV6VmOpenApiServiceInBase" />
    public class V6VmOpenApiServiceInBase : VmOpenApiServiceInVersionBase, IV6VmOpenApiServiceInBase
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
        public override IList<VmOpenApiLocalizedListItem> ServiceDescriptions { get => base.ServiceDescriptions; set => base.ServiceDescriptions = value; }

        /// <summary>
        /// Service sub-type. It is used for SOTE and its taken from GeneralDescription type.
        /// </summary>
        [JsonIgnore]
        public override string SubType { get => base.SubType; set => base.SubType = value; }

        /// <summary>
        /// Set to true to delete all existing municipalities (the Municipalities collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllMunicipalities { get => base.DeleteAllMunicipalities; set => base.DeleteAllMunicipalities = value; }

        /// <summary>
        /// List of other responsible organizations for the service.
        /// </summary>
        [JsonIgnore]
        public override IList<Guid> OtherResponsibleOrganizations { get => base.OtherResponsibleOrganizations; set => base.OtherResponsibleOrganizations = value; }

        /// <summary>
        /// List of service producers.
        /// </summary>
        [JsonIgnore]
        public override IList<V9VmOpenApiServiceProducerIn> ServiceProducers { get; set; }

        /// <summary>
        /// List of organizations responsible or producing the service.
        /// </summary>
        [JsonProperty(Order = 29)]
        [ListWithPropertyValueRequired("RoleType", CommonConsts.RESPONSIBLE)]
        [ListWithPropertyValueRequired("RoleType", CommonConsts.PRODUCER)]
        public virtual IList<V6VmOpenApiServiceOrganizationIn> ServiceOrganizations { get; set; }

        /// <summary>
        /// Indicates if service vouchers are used in the service.
        /// </summary>
        [JsonIgnore]
        public override bool ServiceVouchersInUse { get => base.ServiceVouchersInUse; set => base.ServiceVouchersInUse = value; }

        /// <summary>
        /// List of service vouchers.
        /// </summary>
        [JsonProperty(Order = 32)]
        [LocalizedListPropertyDuplicityForbidden("OrderNumber")]
        public virtual new IList<VmOpenApiServiceVoucher> ServiceVouchers { get; set; } = new List<VmOpenApiServiceVoucher>();

        /// <summary>
        /// Organization Id
        /// </summary>
        [JsonIgnore]
        public override string MainResponsibleOrganization { get; set; }

        /// <summary>
        /// Set to true to delete statutory service general description (StatutoryServiceGeneralDescriptionId property for this object should be empty when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteGeneralDescriptionId { get => base.DeleteGeneralDescriptionId; set => base.DeleteGeneralDescriptionId = value; }

        /// <summary>
        /// Set to true to delete all existing service vouchers (the ServiceVouchers collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllServiceVouchers { get => base.DeleteAllServiceVouchers; set => base.DeleteAllServiceVouchers = value; }

        /// <summary>
        /// Date when item should be published.
        /// </summary>
        [JsonIgnore]
        public override DateTime? ValidFrom { get => base.ValidFrom; set => base.ValidFrom = value; }

        /// <summary>
        /// Date when item should be archived.
        /// </summary>
        [JsonIgnore]
        public override DateTime? ValidTo { get => base.ValidTo; set => base.ValidTo = value; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 6;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceInVersionBase VersionBase()
        {
            var vm = GetInVersionBaseModel<VmOpenApiServiceInVersionBase>(VersionNumber());

            if (ServiceOrganizations != null)
            {
                vm.OtherResponsibleOrganizations = new List<Guid>();
                vm.ServiceProducers = new List<V9VmOpenApiServiceProducerIn>();
                HandleOrganizations(vm);
                HandleServiceProviders(vm);
            }
            this.ServiceVouchers.ForEach(v => vm.ServiceVouchers.Add(v.ConvertToInVersionBase()));
            return vm;
        }

        /// <summary>
        /// Translate ServiceOrganizations with role 'Responsible' to ServiceOrganizations.
        /// Map the first 'Responsible' organization into 'Main responsible' organization.
        /// </summary>
        private void HandleOrganizations(IVmOpenApiServiceInVersionBase vm)
        {
            var organizations = ServiceOrganizations.Where(so => so.RoleType == CommonConsts.RESPONSIBLE).ToList();
            organizations.ForEach(o =>
            {
                if (string.IsNullOrEmpty(vm.MainResponsibleOrganization))
                {
                    vm.MainResponsibleOrganization = o.OrganizationId;
                }
                else
                {
                    vm.OtherResponsibleOrganizations.Add(Guid.Parse(o.OrganizationId));
                }                
            });
            organizations.ForEach(o => ServiceOrganizations.Remove(o));
        }

        /// <summary>
        /// Translate ServiceOrganizations with role 'Producer' to ServiceProducers.
        /// </summary>
        private void HandleServiceProviders(IVmOpenApiServiceInVersionBase vm)
        {
            var serviceProducers = ServiceOrganizations.Where(so => so.RoleType == "Producer" && so.ProvisionType != "VoucherServices").ToList();
            var orderNumber = 0;

            // handle 'SelfProduced'
            var selfProduced = serviceProducers.Where(sp => sp.ProvisionType == ProvisionTypeEnum.SelfProduced.ToString()).ToList();
            vm.ServiceProducers.Add(new V9VmOpenApiServiceProducerIn
            {
//                OwnerReferenceId = ??
                OrderNumber = ++orderNumber,
                ProvisionType = ProvisionTypeEnum.SelfProduced.ToString(),
                Organizations = selfProduced.Select(sp => Guid.Parse(sp.OrganizationId)).ToList()
            });

            selfProduced.ForEach(sp => serviceProducers.Remove(sp));
            selfProduced.ForEach(sp => ServiceOrganizations.Remove(sp));

            // handle 'PurchaseServices' and 'Other'
            foreach (var serviceProducer in serviceProducers)
            {
                vm.ServiceProducers.Add(new V9VmOpenApiServiceProducerIn
                {
                    OwnerReferenceId = serviceProducer.OwnerReferenceId,
                    OrderNumber = ++orderNumber,
                    ProvisionType = serviceProducer.ProvisionType,
                    Organizations = string.IsNullOrEmpty(serviceProducer.OrganizationId) ? new List<Guid>() : new List<Guid> { Guid.Parse(serviceProducer.OrganizationId)},
                    AdditionalInformation = serviceProducer.AdditionalInformation
                });
            }
            
            serviceProducers.ForEach(sp => ServiceOrganizations.Remove(sp));
        }
        
        #endregion
    }
}
