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
        /// Service type. Possible values are: Service, PermissionAndObligation or ProfessionalQualifications.
        /// NOTE! If service type has been defined within attached statutory service general description, the type for service is ignored.
        /// </summary>
        [ValidEnum(typeof(ServiceTypeEnum))]
        public override string Type { get => base.Type; set => base.Type = value; }

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
        /// List of service producers.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiServiceProducerIn> ServiceProducers { get; set; }

        /// <summary>
        /// List of organizations responsible or producing the service.
        /// </summary>
        [JsonProperty(Order = 29)]
        [ListWithPropertyValueRequired("RoleType", "Responsible")]
        [ListWithPropertyValueRequired("RoleType", "Producer")]
        public virtual new IList<V6VmOpenApiServiceOrganizationIn> ServiceOrganizations { get; set; }

        /// <summary>
        /// List of service vouchers
        /// </summary>
        [JsonProperty(Order = 30)]
        [LocalizedListPropertyDuplicityForbidden("OrderNumber")]
        public override IList<VmOpenApiServiceVoucher> ServiceVouchers { get; set; }

        /// <summary>
        /// Organization Id
        /// </summary>
        [JsonIgnore]
        public override string OrganizationId { get; set; }

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
            var vm = GetInVersionBaseModel<VmOpenApiServiceInVersionBase>();
            vm.ServiceOrganizations = new List<Guid>();
            vm.ServiceProducers = new List<VmOpenApiServiceProducerIn>();
            HandleOrganizations(vm);
            HandleServiceProviders(vm);
            return vm;
        }

        /// <summary>
        /// Translate ServiceOrganizations with role 'Responsible' to ServiceOrganizations.
        /// Map the first 'Responsible' organization into 'Main responsible' organization.
        /// </summary>
        private void HandleOrganizations(IVmOpenApiServiceInVersionBase vm)
        {
            var organizations = ServiceOrganizations.Where(so => so.RoleType == "Responsible").ToList();
            organizations.ForEach(o =>
            {
                if (string.IsNullOrEmpty(vm.OrganizationId))
                {
                    vm.OrganizationId = o.OrganizationId;
                }
                else
                {
                    vm.ServiceOrganizations.Add(Guid.Parse(o.OrganizationId));
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
            vm.ServiceProducers.Add(new VmOpenApiServiceProducerIn
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
                vm.ServiceProducers.Add(new VmOpenApiServiceProducerIn
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
