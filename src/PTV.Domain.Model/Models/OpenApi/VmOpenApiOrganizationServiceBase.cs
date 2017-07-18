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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of organization service - base
    /// </summary>
    public class VmOpenApiOrganizationServiceBase : IVmOpenApiOrganizationServiceBase
    {
        /// <summary>
        /// Role type, valid values Responsible or Producer.
        /// </summary>
        [Required]
        [ValidEnum(typeof(RoleTypeEnum))]
        public string RoleType { get; set; }

        /// <summary>
        /// Provision type, valid values SelfProduced, PurchaseServices, Other or VoucherServices. Required if RoleType value is Producer.
        /// </summary>
        [AllowedValues("ProvisionType", new [] { "SelfProduced", "PurchaseServices", "Other", "VoucherServices" })]
        [RequiredIf("RoleType", "Producer")]
        public virtual string ProvisionType { get; set; }

        /// <summary>
        /// Localized list of additional information.
        /// </summary>
        [ListValueNotEmpty("Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual IList<VmOpenApiLanguageItem> AdditionalInformation { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of web pages.
        /// </summary>
        [LocalizedListLanguageDuplicityForbidden]
        public virtual IList<V4VmOpenApiWebPage> WebPages { get; set; } = new List<V4VmOpenApiWebPage>();

        /// <summary>
        /// Converts to version.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>converted model</returns>
        protected TModel ConvertToVersionBase<TModel>() where TModel : IVmOpenApiOrganizationServiceBase, new()
        {
            return new TModel()
            {
                RoleType = this.RoleType,
                ProvisionType = this.ProvisionType,
                AdditionalInformation = this.AdditionalInformation,
                WebPages = this.WebPages,
            };
        }
    }
}
