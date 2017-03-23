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
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V1;
using PTV.Framework;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    /// <summary>
    /// OPEN API V2 - View Model of organization
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V2.IV2VmOpenApiOrganization" />
    public class V2VmOpenApiOrganization : VmOpenApiOrganizationVersionBase, IV2VmOpenApiOrganization
    {
        /// <summary>
        /// Municipality name (like Mikkeli or Helsinki).
        /// </summary>
        [JsonProperty(Order = 3)]
        public new string Municipality { get; set; }

        /// <summary>
        /// List of organizations email addresses.
        /// </summary>
        [JsonProperty(Order = 20)]
        public new IList<VmOpenApiEmail> EmailAddresses { get; set; } = new List<VmOpenApiEmail>();

        /// <summary>
        /// List of organizations phone numbers.
        /// </summary>
        [JsonProperty(Order = 21)]
        public new IList<VmOpenApiPhone> PhoneNumbers { get; set; } = new List<VmOpenApiPhone>();

        /// <summary>
        /// List of organizations web pages.
        /// </summary>
        [JsonProperty(Order = 22)]
        public new IList<VmOpenApiWebPage> WebPages { get; set; } = new List<VmOpenApiWebPage>();

        /// <summary>
        /// List of organizations addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public new IList<V2VmOpenApiAddressWithTypeAndCoordinates> Addresses { get; set; } = new List<V2VmOpenApiAddressWithTypeAndCoordinates>();

        /// <summary>
        /// List of organizations services.
        /// </summary>
        [JsonProperty(Order = 31)]
        public new IList<VmOpenApiOrganizationService> Services { get; set; }

        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 2;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns> previous version</returns>
        public override IVmOpenApiOrganizationVersionBase PreviousVersion()
        {
            var vm = base.GetVersionBaseModel<VmOpenApiOrganization>();
            vm.Municipality = this.Municipality;
            this.PhoneNumbers?.ForEach(p => vm.PhoneNumbers.Add(p.ConvertToVersion1()));
            this.EmailAddresses?.ForEach(a => vm.EmailAddresses.Add(a.ConvertToVersion1()));
            vm.WebPages = this.WebPages;
            this.Addresses?.ForEach(a => vm.Addresses.Add(a.ConvertToVersion1()));
            vm.Services = this.Services;

            return vm;
        }
    }
}
