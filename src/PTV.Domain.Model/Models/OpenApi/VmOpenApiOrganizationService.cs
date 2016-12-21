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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    public class VmOpenApiOrganizationService : IVmOpenApiOrganizationService
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
        public IList<VmOpenApiLanguageItem> AdditionalInformation { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of web pages.
        /// </summary>
        public IList<VmOpenApiWebPage> WebPages { get; set; } = new List<VmOpenApiWebPage>();

        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
    }
}
