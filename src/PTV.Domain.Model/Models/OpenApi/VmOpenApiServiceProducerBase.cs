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
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Framework.Attributes;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service producer base
    /// </summary>
    public class VmOpenApiServiceProducerBase
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonIgnore]
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or sets the owner reference identifier.
        /// </summary>
        /// <value>
        /// The owner reference identifier.
        /// </value>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }

        /// <summary>
        /// The order of service voucher.
        /// </summary>
        [Required]
        [JsonProperty(Order = 1)]
        public int OrderNumber { get; set; }

        /// <summary>
        /// Provision type, valid values SelfProduced, PurchaseServices, Other.
        /// </summary>
        [ValidEnum(typeof(ProvisionTypeEnum))]
        [Required]
        [JsonProperty(Order = 2)]
        public string ProvisionType { get; set; }

        /// <summary>
        /// Localized list of additional information.
        /// </summary>
        [JsonProperty(Order = 4)]
        [ListValueNotEmpty("Value")]
        [LocalizedListLanguageDuplicityForbidden]
        [ListPropertyMaxLength(150, "Value")]
        public virtual IList<VmOpenApiLanguageItem> AdditionalInformation { get; set; } = new List<VmOpenApiLanguageItem>();
    }
}
