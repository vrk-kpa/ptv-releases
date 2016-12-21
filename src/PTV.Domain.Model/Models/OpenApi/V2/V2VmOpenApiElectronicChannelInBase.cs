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
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    public class V2VmOpenApiElectronicChannelInBase : VmOpenApiServiceChannelIn, IV2VmOpenApiElectronicChannelInBase
    {
        /// <summary>
        /// How many signatures are required (number). Required if RequiresSignature is true.
        /// </summary>
        [JsonProperty(Order = 10)]
        [RegularExpression(@"^\d+?$")]
        [RequiredIf("RequiresSignature", true)]
        public string SignatureQuantity { get; set; }

        /// <summary>
        /// Signature required.
        /// </summary>
        [JsonProperty(Order = 11)]
        public bool RequiresSignature { get; set; }

        /// <summary>
        /// Does the service require authentication.
        /// </summary>
        [JsonProperty(Order = 12)]
        [Required]
        public virtual bool RequiresAuthentication { get; set; }

        /// <summary>
        /// List of localized urls.
        /// </summary>
        [JsonProperty(Order = 13)]
        [ListWithUrl("Value")]
        public virtual IReadOnlyList<VmOpenApiLanguageItem> Urls { get; set; }

        /// <summary>
        /// List of attachments.
        /// </summary>
        [JsonProperty(Order = 14)]
        public IReadOnlyList<VmOpenApiAttachment> Attachments { get; set; } = new List<VmOpenApiAttachment>();

        /// <summary>
        /// Set to true to delete all existing attachments. The attachments collection should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 35)]
        public virtual bool DeleteAllAttachments { get; set; }

        [JsonIgnore]
        public override IReadOnlyList<string> Languages
        {
            get
            {
                return base.Languages;
            }

            set
            {
                base.Languages = value;
            }
        }

        [JsonIgnore]
        public override IList<VmOpenApiWebPage> WebPages
        {
            get
            {
                return base.WebPages;
            }

            set
            {
                base.WebPages = value;
            }
        }
    }
}
