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
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using PTV.Framework.Attributes.ValidationAttributes;

namespace PTV.Domain.Model.Models.OpenApi.V9
{
    /// <summary>
    /// OPEN API - View Model of service voucher
    /// </summary>
    /// <seealso cref="IVmOpenApiServiceVoucher" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiBase" />
    public class V9VmOpenApiServiceVoucher : IVmOpenApiBase
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
        [JsonIgnore]
        public int OrderNumber { get; set; }

        /// <summary>
        /// Name of the service voucher.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Language code.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        //[ValidEnum(typeof(LanguageCode))]
        public string Language { get; set; }

        /// <summary>
        /// Web page url.
        /// </summary>
        [PTVUrl]
        [MaxLength(500)]
        public string Url { get; set; }

        /// <summary>
        /// Service voucher additional information
        /// </summary>
        [MaxLength(150)]
        public string AdditionalInformation { get; set; }

        /// <summary>
        /// Converts to previous version.
        /// </summary>
        /// <returns></returns>
        public VmOpenApiServiceVoucher ConvertToPreviousVersion()
        {
            return new VmOpenApiServiceVoucher
            {
                Id = this.Id,
                OwnerReferenceId = this.OwnerReferenceId,
                OrderNumber = this.OrderNumber,
                Value = this.Value,
                Language = this.Language,
                Url = this.Url,
                AdditionalInformation = this.AdditionalInformation
            };
        }
    }
}
