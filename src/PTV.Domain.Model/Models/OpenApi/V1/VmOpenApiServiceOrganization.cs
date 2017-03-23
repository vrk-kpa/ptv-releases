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
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V4;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service organization
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceOrganizationVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiOrganizationService" />
    public class VmOpenApiServiceOrganization : VmOpenApiServiceOrganizationVersionBase, IVmOpenApiOrganizationService
    {
        /// <summary>
        /// List of web pages.
        /// </summary>
        public new IList<VmOpenApiWebPage> WebPages { get; set; } = new List<VmOpenApiWebPage>();

        /// <summary>
        /// Converts to version4.
        /// </summary>
        /// <returns>model converted to version 4</returns>
        public V4VmOpenApiServiceOrganization ConvertToVersion4()
        {
            var serviceOrganization = new V4VmOpenApiServiceOrganization()
            {
                AdditionalInformation = this.AdditionalInformation.SetListValueLength(150),
                OrganizationId = this.OrganizationId,
                OwnerReferenceId = this.OwnerReferenceId,
                ProvisionType = this.ProvisionType,
                RoleType = this.RoleType,
                ServiceId = this.ServiceId,
            };

            foreach (var webPage in this.WebPages)
            {
                serviceOrganization.WebPages.Add(webPage.ConvertToVersion4());
            }

            return serviceOrganization;
        }
    }
}
