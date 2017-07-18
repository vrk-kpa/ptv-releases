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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V5;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V5
{
    /// <summary>
    /// OPEN API V5 - View Model of service location channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceLocationChannelVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V5.IV5VmOpenApiServiceLocationChannel" />
    public class V5VmOpenApiServiceLocationChannel : VmOpenApiServiceLocationChannelVersionBase, IV5VmOpenApiServiceLocationChannel
    {
        /// <summary>
        /// Is the service location channel restricted by service area.
        /// </summary>
        [JsonIgnore]
        public override bool ServiceAreaRestricted { get => base.ServiceAreaRestricted; set => base.ServiceAreaRestricted = value; }

        /// <summary>
        /// List of serviceareas. Used when location service channel is restricted by service area (ServiceAreaRestricted=true). List contains municipality names.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiMunicipality> ServiceAreas { get => base.ServiceAreas; set => base.ServiceAreas = value; }
        
        /// <summary>
        /// Date when item was modified/created..
        /// </summary>
        [JsonIgnore]
        public override DateTime Modified { get => base.Modified; set => base.Modified = value; }

        /// <summary>
        /// Is the phone service charged for. This property is not used in the API anymore. Do not use.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 19)]
        public bool PhoneServiceCharge { get; set; }

        #region methods

        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>verison number</returns>
        public override int VersionNumber()
        {
            return 5;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceChannel PreviousVersion()
        {
            var vm = base.GetVersionBaseModel<V4VmOpenApiServiceLocationChannel>();
            vm.Addresses = new List<V4VmOpenApiAddressWithTypeAndCoordinates>();
            Addresses.ForEach(a => vm.Addresses.Add(a.ConvertToVersion4()));
            return vm;
        }

        #endregion
    }
}
