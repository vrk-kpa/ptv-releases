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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V4;
using System;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service organization - base version
    /// </summary>
    public class VmOpenApiServiceOrganizationVersionBase : VmOpenApiOrganizationServiceVersionBase, IVmOpenApiServiceOrganizationVersionBase //IV3VmOpenApiOrganizationService
    {
        /// <summary>
        /// PTV service identifier.
        /// </summary>
        [JsonIgnore]
        public override string ServiceId { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [JsonIgnore]
        public Guid? Id { get; set; }

        /// <summary>
        /// Converts to version1.
        /// </summary>
        /// <returns>model converted to version 1</returns>
        public new VmOpenApiServiceOrganization ConvertToVersion1()
        {
            var organization = new VmOpenApiServiceOrganization()
            {
                AdditionalInformation = this.AdditionalInformation,
                //Id = this.Id,
                OrganizationId = this.OrganizationId,
                OwnerReferenceId = this.OwnerReferenceId,
                ProvisionType = this.ProvisionType,
                RoleType = this.RoleType,
                ServiceId = this.ServiceId,
            };

            foreach (var webPage in this.WebPages)
            {
                organization.WebPages.Add(webPage.ConvertToVersion2());
            }

            return organization;
        }
    }
}
