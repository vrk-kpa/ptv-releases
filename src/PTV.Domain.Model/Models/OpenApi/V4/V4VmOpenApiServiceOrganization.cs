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
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V6;

namespace PTV.Domain.Model.Models.OpenApi.V4
{
    /// <summary>
    /// OPEN API V4 - View Model of service organization
    /// </summary>
    public class V4VmOpenApiServiceOrganization : VmOpenApiServiceOrganizationInVersionBase, IV4VmOpenApiOrganizationService
    {
        /// <summary>
        /// Localized list of additional information.
        /// </summary>
        [ListPropertyMaxLength(150, "Value")]
        public override IList<VmOpenApiLanguageItem> AdditionalInformation
        {
            get {
                return base.AdditionalInformation;
            }
            set {
                base.AdditionalInformation = value;
            }
        }

        /// <summary>
        /// Converts to version4.
        /// </summary>
        /// <returns>model converted to version 4</returns>
        public V6VmOpenApiServiceOrganizationIn ConvertToVersion6()
        {
            var serviceOrganization = new V6VmOpenApiServiceOrganizationIn()
            {
                OrganizationId = this.OrganizationId,
                OwnerReferenceId = this.OwnerReferenceId,
                ServiceId = this.ServiceId,
                ProvisionType = this.ProvisionType,
                RoleType = this.RoleType,
                AdditionalInformation = this.AdditionalInformation.SetListValueLength(150),
                WebPages = this.WebPages,
            };
            return serviceOrganization;
        }
    }
}
