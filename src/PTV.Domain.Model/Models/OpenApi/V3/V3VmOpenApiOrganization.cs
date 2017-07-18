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
using System.ComponentModel.DataAnnotations;
using System;

namespace PTV.Domain.Model.Models.OpenApi.V3
{
    /// <summary>
    /// OPEN API V3 - View Model of organization
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V3.IV3VmOpenApiOrganization" />
    public class V3VmOpenApiOrganization : VmOpenApiOrganizationVersionBase, IV3VmOpenApiOrganization
    {
        /// <summary>
        /// Display name type (Name or AlternateName). Which name type should be used as the display name for the organization (OrganizationNames list).
        /// </summary>
        [Required]
        [JsonProperty(Order = 14)]
        public new string DisplayNameType { get; set; }

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
        public new IList<V3VmOpenApiWebPage> WebPages { get; set; } = new List<V3VmOpenApiWebPage>();

        /// <summary>
        /// List of organizations addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public new IList<V3VmOpenApiAddressWithTypeAndCoordinates> Addresses { get; set; } = new List<V3VmOpenApiAddressWithTypeAndCoordinates>();

        /// <summary>
        /// List of organizations services.
        /// </summary>
        [JsonProperty(Order = 31)]
        public new IList<V3VmOpenApiOrganizationService> Services { get; set; }

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
            return 3;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous</returns>
        public override IVmOpenApiOrganizationVersionBase PreviousVersion()
        {
            throw new NotSupportedException("No previous version for class V3VmOpenApiOrganization!");
        }

        #endregion
    }
}
