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
using PTV.Domain.Model.Models.Interfaces.OpenApi.V3;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi.V1;
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V2;

namespace PTV.Domain.Model.Models.OpenApi.V3
{
    /// <summary>
    /// OPEN API V3 - View Model of organization for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V3.IV3VmOpenApiOrganizationInBase" />
    public class V3VmOpenApiOrganizationInBase : VmOpenApiOrganizationInVersionBase, IV3VmOpenApiOrganizationInBase
    {
        /// <summary>
        /// Organization OID. - must match the regex @"^[A-Za-z0-9.-]*$"
        /// </summary>
        [JsonProperty(Order = 2)]
        [RegularExpression(@"^[A-Za-z0-9.-]*$")]
        public new string Oid { get; set; }

        /// <summary>
        /// List of organizations email addresses.
        /// </summary>
        [JsonProperty(Order = 20)]
        public new IReadOnlyList<VmOpenApiEmail> EmailAddresses { get; set; } = new List<VmOpenApiEmail>();

        /// <summary>
        /// List of phone numbers.
        /// </summary>
        [JsonProperty(Order = 21)]
        public new IList<VmOpenApiPhone> PhoneNumbers { get; set; } = new List<VmOpenApiPhone>();

        /// <summary>
        /// List of web pages.
        /// </summary>
        [JsonProperty(Order = 22)]
        public new IList<V3VmOpenApiWebPage> WebPages { get; set; } = new List<V3VmOpenApiWebPage>();

        /// <summary>
        /// List of addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public new IList<V2VmOpenApiAddressWithType> Addresses { get; set; } = new List<V2VmOpenApiAddressWithType>();

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 3;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiOrganizationInVersionBase VersionBase()
        {
            var vm = base.GetInVersionBaseModel<VmOpenApiOrganizationInVersionBase>();
            var i = 1;
            vm.Oid = this.Oid.SetStringValueLength(100);
            PhoneNumbers.ForEach(p => vm.PhoneNumbers.Add(p.ConvertToVersion4()));
            WebPages.ForEach(w =>
            {
                var webPage = w.ConvertToWebPageWithOrderNumber();
                webPage.OrderNumber = (i++).ToString();
                vm.WebPages.Add(webPage);
            });
            EmailAddresses.ForEach(e => vm.EmailAddresses.Add(e.ConvertToVersion4()));
            Addresses.ForEach(a => vm.Addresses.Add(a.ConvertToVersion4()));
            return vm;
        }
        #endregion
    }
}
