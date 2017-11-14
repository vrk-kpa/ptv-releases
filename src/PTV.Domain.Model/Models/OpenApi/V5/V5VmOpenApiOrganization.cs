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

using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V5;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using Newtonsoft.Json;
using System;

namespace PTV.Domain.Model.Models.OpenApi.V5
{
    /// <summary>
    /// OPEN API V5 - View Model of organization
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V5.IV5VmOpenApiOrganization" />
    public class V5VmOpenApiOrganization : VmOpenApiOrganizationVersionBase, IV5VmOpenApiOrganization
    {
        /// <summary>
        /// List of organizations addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public new IList<V5VmOpenApiAddressWithTypeAndCoordinates> Addresses { get; set; } = new List<V5VmOpenApiAddressWithTypeAndCoordinates>();

        /// <summary>
        /// Organization external system identifier.
        /// </summary>
        [JsonIgnore]
        public override string SourceId { get => base.SourceId; set => base.SourceId = value; }

        /// <summary>
        /// Date when item was modified/created..
        /// </summary>
        [JsonIgnore]
        public override DateTime Modified { get => base.Modified; set => base.Modified = value; }

        /// <summary>
        /// The sub organizations
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiItem> SubOrganizations { get => base.SubOrganizations; set => base.SubOrganizations = value; }

        /// <summary>
        /// List of organizations electronic invoicing information.
        /// </summary>
        [JsonIgnore]
        override public IList<VmOpenApiOrganizationEInvoicing> ElectronicInvoicings { get => base.ElectronicInvoicings; set => base.ElectronicInvoicings = value; }

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
        public override IVmOpenApiOrganizationVersionBase PreviousVersion()
        {
            var vm = base.GetVersionBaseModel<V4VmOpenApiOrganization>();
            vm.DisplayNameType = this.DisplayNameType.ValueString();
            this.Addresses.ForEach(a => vm.Addresses.Add(a.ConvertToVersion4()));
            if (Services?.Count > 0)
            {
                vm.Services = new List<V4VmOpenApiOrganizationService>();
                this.Services.ForEach(s => vm.Services.Add(s.ConvertToVersion4()));
            }
            return vm;
        }
        #endregion
    }
}
