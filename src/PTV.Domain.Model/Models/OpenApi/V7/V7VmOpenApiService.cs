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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi.V6;

namespace PTV.Domain.Model.Models.OpenApi.V7
{
    /// <summary>
    /// OPEN API V7 - View Model of service
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceVersionBase" />
    public class V7VmOpenApiService : VmOpenApiServiceVersionBase
    {
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
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceVersionBase PreviousVersion()
        {
            var vm = GetVersionBaseModel<V6VmOpenApiService>();
            ServiceChannels.ForEach(c => vm.ServiceChannels.Add(c.ConvertToVersion6()));
            vm.Organizations = new List<V6VmOpenApiServiceOrganization>();

            // handle organization services
            var responsible = "Responsible";
            if (MainOrganization != null && MainOrganization.Id.IsAssigned())
            {
                vm.Organizations.Add(new V6VmOpenApiServiceOrganization
                {
                    RoleType = responsible,
                    Organization = MainOrganization,
                    AdditionalInformation = new List<VmOpenApiLanguageItem>(),
                    WebPages = new List<V4VmOpenApiWebPage>()
                });
            }
            Organizations.ForEach(os =>
            {
                if (vm.Organizations.Where(o => o.Organization.Id == os.Id && o.RoleType == responsible) == null)
                {
                    vm.Organizations.Add(new V6VmOpenApiServiceOrganization
                    {
                        RoleType = responsible,
                        Organization = os,
                        AdditionalInformation = new List<VmOpenApiLanguageItem>(),
                        WebPages = new List<V4VmOpenApiWebPage>()
                    });
                }
            });

            // handle service producers
            ServiceProducers.ForEach(sp =>
            {
                if (sp.ProvisionType == ProvisionTypeEnum.SelfProduced.ToString())
                {
                    sp.Organizations.ForEach(spo => vm.Organizations.Add(new V6VmOpenApiServiceOrganization
                    {
                        RoleType = "Producer",
                        ProvisionType = sp.ProvisionType,
                        Organization = spo,
                        AdditionalInformation = new List<VmOpenApiLanguageItem>(),
                        WebPages = new List<V4VmOpenApiWebPage>()
                    }));
                }
                else
                {
                    vm.Organizations.Add(new V6VmOpenApiServiceOrganization
                    {
                        RoleType = "Producer",
                        ProvisionType = sp.ProvisionType,
                        Organization = sp.Organizations.FirstOrDefault(),
                        AdditionalInformation = sp.AdditionalInformation,
                        WebPages = new List<V4VmOpenApiWebPage>()
                    });
                }
            });

            return vm;
        }
        #endregion
    }
}
