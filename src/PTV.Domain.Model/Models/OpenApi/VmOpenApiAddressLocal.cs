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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Framework;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API V7 - View Model of local (Finnish) address.
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiAddressLocal" />
    public class VmOpenApiAddressLocal : VmOpenApiBase, IVmOpenApiAddressLocal
    {
        /// <summary>
        /// Postal code, for example 00100.
        /// </summary>
        [Required]
        [RegularExpression(ValidationConsts.PostalCode)]
        [JsonProperty(Order = 3)]
        public virtual string PostalCode { get; set; }

        /// <summary>
        /// List of localized Post offices, for example Helsinki, Helsingfors.
        /// </summary>
        [JsonProperty(Order = 4)]
        public IList<VmOpenApiLanguageItem> PostOffice { get; set; }

        /// <summary>
        /// Municipality code (e.g. 091).
        /// </summary>
        [RegularExpression(ValidationConsts.Municipality, ErrorMessage = CoreMessages.OpenApi.MustBeNumeric)]
        [JsonProperty(Order = 5)]
        public virtual VmOpenApiMunicipality Municipality { get; set; }

        /// <summary>
        /// Localized list of additional information about the address.
        /// </summary>
        [ListValueNotEmpty("Value")]
        [ListPropertyMaxLength(150, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        [JsonProperty(Order = 9)]
        public IList<VmOpenApiLanguageItem> AdditionalInformation { get; set; }
    }
}
