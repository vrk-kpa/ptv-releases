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

using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace PTV.Domain.Model.Models.OpenApi.V4
{
    /// <summary>
    /// OPEN API V4 - View Model of organization
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V4.IV4VmOpenApiOrganization" />
    public class V4VmOpenApiOrganization : VmOpenApiOrganizationVersionBase, IV4VmOpenApiOrganization
    {
        /// <summary>
        /// Display name type (Name or AlternateName). Which name type should be used as the display name for the organization (OrganizationNames list).
        /// </summary>
        [Required]
        [JsonProperty(Order = 14)]
        public new string DisplayNameType { get; set; }

        /// <summary>
        /// List of organizations addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public new IList<V4VmOpenApiAddressWithTypeAndCoordinates> Addresses { get; set; } = new List<V4VmOpenApiAddressWithTypeAndCoordinates>();

        /// <summary>
        /// List of organizations services.
        /// </summary>
        [JsonProperty(Order = 31)]
        public new IList<V4VmOpenApiOrganizationService> Services { get; set; }

        /// <summary>
        /// Area type. 
        /// </summary>
        [JsonIgnore]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// List of organization areas.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiArea> Areas { get => base.Areas; set => base.Areas = value; }

        /// <summary>
        /// Date when item was modified/created..
        /// </summary>
        [JsonIgnore]
        public override DateTime Modified { get => base.Modified; set => base.Modified = value; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 4;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiOrganizationVersionBase PreviousVersion()
        {
            var vm = base.GetVersionBaseModel<V3VmOpenApiOrganization>();
            vm.DisplayNameType = this.DisplayNameType;
            this.EmailAddresses.ForEach(e => vm.EmailAddresses.Add(e.ConvertToVersion3()));
            this.PhoneNumbers.ForEach(p => vm.PhoneNumbers.Add(p.ConvertToVersion3()));
            this.WebPages.ForEach(w => vm.WebPages.Add(w.ConvertToVersion3()));
            this.Addresses.ForEach(a => vm.Addresses.Add(a.ConvertToVersion3()));
            if (Services?.Count > 0)
            {
                vm.Services = new List<V3VmOpenApiOrganizationService>();
                this.Services.ForEach(s => vm.Services.Add(s.ConvertToVersion3()));
            }
            return vm;
        }

        #endregion
    }
}
