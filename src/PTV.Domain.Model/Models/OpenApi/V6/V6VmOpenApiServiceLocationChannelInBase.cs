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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V6;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi.V7;

namespace PTV.Domain.Model.Models.OpenApi.V6
{
    /// <summary>
    /// OPEN API V6 - View Model of service location channel for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceLocationChannelInVersionBase" />
    public class V6VmOpenApiServiceLocationChannelInBase : VmOpenApiServiceLocationChannelInVersionBase, IV6VmOpenApiServiceLocationChannelInBase
    {
        /// <summary>
        /// Is the service location channel restricted by service area.
        /// </summary>
        [JsonIgnore]
        public override bool ServiceAreaRestricted { get => base.ServiceAreaRestricted; set => base.ServiceAreaRestricted = value; }

        /// <summary>
        /// List of serviceareas. Used when location service channel is restricted by service area (ServiceAreaRestricted=true). List should contain municipality codes.
        /// </summary>
        [JsonIgnore]
        public override IList<string> ServiceAreas { get => base.ServiceAreas; set => base.ServiceAreas = value; }

        /// <summary>
        /// List of service location addresses.
        /// </summary>
        [JsonProperty(Order = 24)]
        public new virtual IList<V5VmOpenApiAddressWithTypeIn> Addresses { get; set; } = new List<V5VmOpenApiAddressWithTypeIn>();

        /// <summary>
        /// Service channel publishing status. Values: Draft, Published, Deleted or Modified.
        /// </summary>
        [AllowedValues("PublishingStatus", new[] { "Draft", "Published", "Deleted", "Modified" })]
        [ValidEnum(null)] // PTV-1792: suppress base ValidEnum validation
        public override string PublishingStatus { get => base.PublishingStatus; set => base.PublishingStatus = value; }

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
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            var vm = base.GetVersionBaseModel<VmOpenApiServiceLocationChannelInVersionBase>();
            Addresses.ForEach(a => vm.Addresses.Add(a.LatestVersion<V7VmOpenApiAddressWithMovingIn>()));
            return vm;
        }
        #endregion
    }
}
