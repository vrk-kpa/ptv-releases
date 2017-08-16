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
using PTV.Domain.Model.Models.Interfaces.OpenApi.V5;
using PTV.Domain.Model.Models.OpenApi.V4;
using Newtonsoft.Json;
using System;

namespace PTV.Domain.Model.Models.OpenApi.V5
{
    /// <summary>
    /// OPEN API V5 - View Model of service
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V5.IV5VmOpenApiService" />
    public class V5VmOpenApiService : VmOpenApiServiceVersionBase, IV5VmOpenApiService
    {
        /// <summary>
        /// Service funding type. Possible values are: PubliclyFunded or MarketFunded.
        /// </summary>
        [JsonIgnore]
        public override string FundingType { get => base.FundingType; set => base.FundingType = value; }

        /// <summary>
        /// List of linked service channels including relationship data.
        /// </summary>
        [JsonProperty(Order = 29)]
        public new IList<V4VmOpenApiServiceServiceChannel> ServiceChannels { get; set; } = new List<V4VmOpenApiServiceServiceChannel>();

        /// <summary>
        /// List of organizations, responsible and producer organizations of the service.
        /// </summary>
        [JsonProperty(Order = 30)]
        public new IList<V4VmOpenApiServiceOrganization> Organizations { get; set; } = new List<V4VmOpenApiServiceOrganization>();

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
        /// Date when item was modified/created..
        /// </summary>
        [JsonIgnore]
        public override DateTime Modified { get => base.Modified; set => base.Modified = value; }

        /// <summary>
        /// List of service vouchers.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiServiceVoucher> ServiceVouchers { get; set; }

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
            return 5;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceVersionBase PreviousVersion()
        {
            var vm = GetVersionBaseModel<V4VmOpenApiService>();
            vm.ServiceChannels = ServiceChannels;
            vm.Organizations = Organizations;
            return vm;
        }
        #endregion
    }
}
