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
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of organization service - base version
    /// </summary>
    public class VmOpenApiOrganizationServiceVersionBase : IVmOpenApiOrganizationServiceVersionBase
    {
        /// <summary>
        /// PTV service identifier.
        /// </summary>
        public virtual string ServiceId { get; set; }

        /// <summary>
        /// PTV organization identifier (organization related to the service). Required if role type is Responsible or if ProvisionType is SelfProduced.
        /// </summary>
        [ValidGuid]
        [RequiredIf("RoleType", "Responsible")]
        [RequiredIf("ProvisionType", "SelfProduced")]
        public string OrganizationId { get; set; }

        /// <summary>
        /// Role type, valid values Responsible or Producer.
        /// </summary>
        [Required]
        [ValidEnum(typeof(RoleTypeEnum))]
        public string RoleType { get; set; }

        /// <summary>
        /// Provision type, valid values SelfProduced, VoucherServices, PurchaseServices or Other. Required if RoleType value is Producer.
        /// </summary>
        [ValidEnum(typeof(ProvisionTypeEnum))]
        [RequiredIf("RoleType", "Producer")]
        public string ProvisionType { get; set; }

        /// <summary>
        /// Localized list of additional information.
        /// </summary>
        [ListValueNotEmpty("Value")]
        public virtual IList<VmOpenApiLanguageItem> AdditionalInformation { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of web pages.
        /// </summary>
        public IList<V4VmOpenApiWebPage> WebPages { get; set; } = new List<V4VmOpenApiWebPage>();

        /// <summary>
        /// Gets or sets the owner reference identifier.
        /// </summary>
        /// <value>
        /// The owner reference identifier.
        /// </value>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }

        /// <summary>
        /// Converts to version1.
        /// </summary>
        /// <returns>model converted to version 1</returns>
        public VmOpenApiOrganizationService ConvertToVersion1()
        {
            var organizationService = ConvertToVersion<VmOpenApiOrganizationService>();

            foreach (var webPage in this.WebPages)
            {
                organizationService.WebPages.Add(webPage.ConvertToVersion2());
            }

            return organizationService;
        }

        /// <summary>
        /// Converts to version.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>converted model</returns>
        protected TModel ConvertToVersion<TModel>() where TModel : VmOpenApiOrganizationServiceVersionBase, new()
        {
            return new TModel()
            {
                AdditionalInformation = this.AdditionalInformation,
                OrganizationId = this.OrganizationId,
                OwnerReferenceId = this.OwnerReferenceId,
                ProvisionType = this.ProvisionType,
                RoleType = this.RoleType,
                ServiceId = this.ServiceId
            };
        }
    }
}
