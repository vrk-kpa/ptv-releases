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

using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of Address in - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiAddressVersionBase" />
    public class VmOpenApiAddressInVersionBase : VmOpenApiBase, IVmOpenApiAddressInVersionBase
    {
        /// <summary>
        /// Post office box like PL 310
        /// </summary>
        [RequiredIf("StreetAddress", null)]
        public string PostOfficeBox { get; set; }

        /// <summary>
        /// Postal code, for example 00010.
        /// </summary>
        [Required]
        [RegularExpression(@"\d{5}?")]
        public string PostalCode { get; set; }

        /// <summary>
        /// List of localized Post offices, for example Helsinki, Helsingfors.
        /// </summary>
        public IList<VmOpenApiLanguageItem> PostOffice { get; set; }

        /// <summary>
        /// List of localized street addresses.
        /// </summary>
        [RequiredIf("PostOfficeBox", "")]
        [LocalizedListLanguageDuplicityForbidden]
        [ListPropertyMaxLength(100, "Value")]
        public IList<VmOpenApiLanguageItem> StreetAddress { get; set; }

        /// <summary>
        /// Street number for street address.
        /// </summary>
        [MaxLength(30)]
        public virtual string StreetNumber { get; set; }

        /// <summary>
        /// Municipality including municipality code and a localized list of municipality names.
        /// </summary>
        public VmOpenApiMunicipality Municipality { get; set; }

        /// <summary>
        /// Country code (ISO 3166-1 alpha-2), for example FI.
        /// </summary>
        [RegularExpression(@"^[A-Z]{2}$")]
        public string Country { get; set; }

        /// <summary>
        /// Localized list of additional information about the address.
        /// </summary>
        [ListValueNotEmpty("Value")]
        [ListPropertyMaxLength(150, "Value")]
        public IList<VmOpenApiLanguageItem> AdditionalInformations { get; set; } = new List<VmOpenApiLanguageItem>();

    }
}
