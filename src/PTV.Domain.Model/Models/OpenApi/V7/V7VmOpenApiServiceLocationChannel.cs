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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Framework;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi.V7
{
    /// <summary>
    /// OPEN API V7 - View Model of service location channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceLocationChannelVersionBase" />
    public class V7VmOpenApiServiceLocationChannel : VmOpenApiServiceLocationChannelVersionBase
    {
        /// <summary>
        /// List of Display name types (Name or AlternativeName) for each language version of ServiceChannelNames.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiNameTypeByLanguage> DisplayNameType { get => base.DisplayNameType; set => base.DisplayNameType = value; }

        /// <summary>
        /// List of service channel web pages.
        /// </summary>
        [JsonProperty(Order = 19)]
        public virtual new IList<VmOpenApiWebPageWithOrderNumber> WebPages { get; set; } = new List<VmOpenApiWebPageWithOrderNumber>();

        /// <summary>
        /// List of service location addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public virtual new IList<V7VmOpenApiAddressWithMoving> Addresses { get; set; } = new List<V7VmOpenApiAddressWithMoving>();

        /// <summary>
        /// List of service channel service hours.
        /// </summary>
        [JsonProperty(Order = 25)]
        public virtual new IList<V4VmOpenApiServiceHour> ServiceHours { get; set; } = new List<V4VmOpenApiServiceHour>();

        /// <summary>
        /// List of linked services including relationship data.
        /// </summary>
        [JsonProperty(Order = 29)]
        public virtual new IList<VmOpenApiServiceChannelService> Services { get; set; } = new List<VmOpenApiServiceChannelService>();

        /// <summary>
        /// Service channel OID. Must match the regex @"^[A-Za-z0-9.-]*$".
        /// </summary>
        [JsonIgnore]
        public override string Oid { get => base.Oid; set => base.Oid = value; }

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
        public override IVmOpenApiServiceChannel PreviousVersion()
        {
            var vm = base.GetVersionBaseModel<V6VmOpenApiServiceLocationChannel>();
            vm.WebPages = this.WebPages;
            vm.Addresses = new List<V5VmOpenApiAddressWithTypeAndCoordinates>();
            // Rename address properties back from new values since in older versions this new type of address does not exist. See ConvertToVersion5(). PTV-2910
            Addresses.ForEach(a => vm.Addresses.Add(a.ConvertToVersion5()));
            vm.ServiceHours = this.ServiceHours;
            return vm;
        }

        #endregion
    }
}
