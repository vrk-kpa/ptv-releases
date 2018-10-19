﻿/**
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


using PTV.Framework.Attributes;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API V7 - In view Model of post office box address.
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiAddressLocalIn" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiAddressPostOfficeBoxIn" />
    public class VmOpenApiAddressPostOfficeBoxIn : VmOpenApiAddressLocalIn, IVmOpenApiAddressPostOfficeBoxIn
    {
        /// <summary>
        /// Post office box like PL 310
        /// </summary>
        [ListPropertyMaxLength(100, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        [ListRequired]
        [JsonProperty(Order = 1)]
        public virtual IList<VmOpenApiLanguageItem> PostOfficeBox { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// Postal code, for example 00100.
        /// </summary>
        [Required]
        [JsonProperty(Order = 3)]
        public override string PostalCode { get => base.PostalCode; set => base.PostalCode = value; }
    }
}
