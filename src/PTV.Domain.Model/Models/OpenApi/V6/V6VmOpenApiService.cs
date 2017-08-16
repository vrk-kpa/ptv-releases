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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Enums;
using System.Linq;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V6;
using System;

namespace PTV.Domain.Model.Models.OpenApi.V6
{
    /// <summary>
    /// OPEN API V6 - View Model of service
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceVersionBase" />
    public class V6VmOpenApiService : VmOpenApiServiceVersionBase
    {        
        /// <summary>
        /// List of linked service channels including relationship data.
        /// </summary>
        [JsonProperty(Order = 29)]
        public new IList<V6VmOpenApiServiceServiceChannel> ServiceChannels { get; set; } = new List<V6VmOpenApiServiceServiceChannel>();

        /// <summary>
        /// Service coverage type. Valid values are: Local or Nationwide.
        /// </summary>
        [JsonIgnore]
        public override string ServiceCoverageType { get => base.ServiceCoverageType; set => base.ServiceCoverageType = value; }

        /// <summary>
        /// List of municipality codes and names that the service is available for. Used in conjunction with service coverage type Local.
        /// </summary>
        [JsonIgnore]
        public override IReadOnlyList<VmOpenApiMunicipality> Municipalities { get => base.Municipalities; set => base.Municipalities = value; }

        /// <summary>
        /// List of organizations, responsible and producer organizations of the service.
        /// </summary>
        [JsonProperty(Order = 30)]
        public new IList<V6VmOpenApiServiceOrganization> Organizations { get; set; } = new List<V6VmOpenApiServiceOrganization>();

        /// <summary>
        /// List of service producers is not available for version 6 and lower
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiServiceProducer> ServiceProducers { get => base.ServiceProducers; set => base.ServiceProducers = value; }

        /// <summary>
        /// Organization Id
        /// </summary>
        [JsonIgnore]
        public override VmOpenApiItem MainOrganization { get; set; }

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
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceVersionBase PreviousVersion()
        {
            var vm = GetVersionBaseModel<V5VmOpenApiService>();
            if (vm.Type == ServiceTypeEnum.ProfessionalQualifications.ToString()) 
            {
                // ProfessionalQualifications type is new service type for version 6 (PTV-1397).
                // In older versions PermissionAndObligation is used.
                vm.Type = ServiceTypeEnum.PermissionAndObligation.ToString();
            }

            // Version 6+ allow Markdown line breaks, older versions do not (PTV-1978)
            // Actually line breaks are allowed also in older versions so replace into space is removed! PTV-2346
            //vm.ServiceDescriptions.ForEach(description => description.Value = description.Value?.Replace(LineBreak, " "));
            //vm.Requirements.ForEach(requirement => requirement.Value = requirement.Value?.Replace(LineBreak, " "));
            ServiceChannels.ForEach(c => vm.ServiceChannels.Add(c.ConvertToVersion4()));
            Organizations.ForEach(o => vm.Organizations.Add(o.ConvertToVersion4()));

            // translate ServiceVouchers to OrganizationServices for versions < 6
            if (ServiceVouchers?.Count > 0)
            {
                vm.Organizations = vm.Organizations ?? new List<V4VmOpenApiServiceOrganization>();

                foreach (var serviceVoucher in ServiceVouchers)
                {
                    var organization = new V4VmOpenApiServiceOrganization
                    {
                        OwnerReferenceId = serviceVoucher.OwnerReferenceId,
                        ProvisionType = "VoucherServices",
                        RoleType = "Producer"
                    };

                    if (!string.IsNullOrEmpty(serviceVoucher.Value) || !string.IsNullOrEmpty(serviceVoucher.Url))
                    {
                        organization.WebPages = new List<V4VmOpenApiWebPage>
                        {
                            new V4VmOpenApiWebPage
                            {
                                Language = serviceVoucher.Language,
                                Url = serviceVoucher.Url,
                                Value = serviceVoucher.Value
                            }
                        };
                    }

                    if (!string.IsNullOrEmpty(serviceVoucher.AdditionalInformation))
                    {
                        organization.AdditionalInformation = new List<VmOpenApiLanguageItem>
                        {
                            new VmOpenApiLanguageItem
                            {
                                Language = serviceVoucher.Language,
                                Value = serviceVoucher.AdditionalInformation
                            }
                        };
                    }

                    vm.Organizations.Add(organization);
                }
            }

            return vm;
        }
        #endregion
    }
}
