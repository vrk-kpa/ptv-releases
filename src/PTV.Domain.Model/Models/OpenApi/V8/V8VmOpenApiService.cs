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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using Newtonsoft.Json;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework.Extensions;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models.OpenApi.V8
{
    /// <summary>
    /// OPEN API V87 - View Model of service
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceVersionBase" />
    public class V8VmOpenApiService : VmOpenApiServiceVersionBase
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
            return 8;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceVersionBase PreviousVersion()
        {
            var vm = GetVersionBaseModel<V7VmOpenApiService>();
            this.ServiceChannels.ForEach(sc => vm.ServiceChannels.Add(sc.ConvertToVersion7()));
            // Change property values into ones used in older versions (PTV-2184)
            vm.Type = string.IsNullOrEmpty(vm.Type) ? vm.Type : vm.Type.GetEnumValueByOpenApiEnumValue<ServiceTypeEnum>();
            vm.ServiceChargeType = string.IsNullOrEmpty(vm.ServiceChargeType) ? vm.ServiceChargeType : vm.ServiceChargeType.GetEnumValueByOpenApiEnumValue<ServiceChargeTypeEnum>();
            vm.ServiceNames?.ForEach(n => n.SetV7Values<NameTypeEnum>());
            vm.ServiceDescriptions?.ForEach(d => d.SetV7Values<DescriptionTypeEnum>());
            vm.AreaType = string.IsNullOrEmpty(vm.AreaType) ? vm.AreaType : vm.AreaType.GetEnumValueByOpenApiEnumValue<AreaInformationTypeEnum>();
            vm.Areas?.ForEach(a => a.SetV7Values());
            vm.Organizations?.ForEach(o => o.ProvisionType = string.IsNullOrEmpty(o.ProvisionType) ? o.ProvisionType : o.ProvisionType.GetEnumValueByOpenApiEnumValue<ProvisionTypeEnum>());
            vm.ServiceChannels?.ForEach(c => c.SetV7Values());
            return vm;
        }
        #endregion
    }
}
