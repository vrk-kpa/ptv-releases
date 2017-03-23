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

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V1;

namespace PTV.Domain.Model.Models.OpenApi.V1
{
    /// <summary>
    /// OPEN API - View Model of organization
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V1.IVmOpenApiOrganization" />
    public class VmOpenApiOrganization : VmOpenApiOrganizationVersionBase, IVmOpenApiOrganization
    {
        /// <summary>
        /// Municipality name (like Mikkeli or Helsinki).
        /// </summary>
        public new string Municipality { get; set; }

        /// <summary>
        /// This property is not used in the API anymore.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 4)]
        public bool StreetAddressAsPostalAddress { get; set; }

        /// <summary>
        /// List of organizations email addresses.
        /// </summary>
        [JsonProperty(Order = 20)]
        public new IList<VmOpenApiOrganizationEmail> EmailAddresses { get; set; } = new List<VmOpenApiOrganizationEmail>();

        /// <summary>
        /// List of organizations phone numbers.
        /// </summary>
        [JsonProperty(Order = 21)]
        public new IList<VmOpenApiOrganizationPhone> PhoneNumbers { get; set; } = new List<VmOpenApiOrganizationPhone>();

        /// <summary>
        /// List of organizations web pages.
        /// </summary>
        [JsonProperty(Order = 22)]
        public new IList<VmOpenApiWebPage> WebPages { get; set; } = new List<VmOpenApiWebPage>();

        /// <summary>
        /// List of organizations addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public new IList<VmOpenApiAddressWithType> Addresses { get; set; } = new List<VmOpenApiAddressWithType>();

        /// <summary>
        /// List of organizations services.
        /// </summary>
        [JsonProperty(Order = 31)]
        public new IList<VmOpenApiOrganizationService> Services { get; set; }


        /// <summary>
        /// Get the Version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 1;
        }

        /// <summary>
        /// Gets the Previous version.
        /// </summary>
        /// <returns>previous version</returns>
        /// <exception cref="System.NotSupportedException">No previous version for VmOpenApiOrganization!</exception>
        public override IVmOpenApiOrganizationVersionBase PreviousVersion()
        {
            throw new NotSupportedException("No previous version for VmOpenApiOrganization!");
        }
    }
}
