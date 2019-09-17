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
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Framework.Attributes;
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Collections.Generic;
using PTV.Framework;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API V7 - View Model of address with type
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiAddressLocalIn" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiAddressStreetIn" />
    public class VmOpenApiAddressStreetIn : VmOpenApiAddressLocalIn, IVmOpenApiAddressStreetIn
    {
        /// <summary>
        /// List of localized street addresses.
        /// </summary>
        [LocalizedListLanguageDuplicityForbidden]
        [ListPropertyMaxLength(100, "Value")]
        [ListRequired]
        [JsonProperty(Order = 1)]
        public virtual IList<VmOpenApiLanguageItem> Street { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// Street number for street address.
        /// </summary>
        [MaxLength(30)]
        [JsonProperty(Order = 2)]
        public virtual string StreetNumber { get; set; }
        
        /// <summary>
        /// Postal code, for example 00100.
        /// </summary>
        [Required]
        [JsonProperty(Order = 3)]
        public override string PostalCode { get => base.PostalCode; set => base.PostalCode = value; }
        
        /// <summary>
        /// Temporarily stored Id of assigned Street during translations
        /// </summary>
        [JsonIgnore]
        public Guid ReferencedStreetId { get; set; }
    }
}
