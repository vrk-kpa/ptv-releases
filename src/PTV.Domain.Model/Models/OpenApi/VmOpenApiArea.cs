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
using System;
using System.Collections;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of Area
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiArea" />
    public class VmOpenApiArea : IVmOpenApiArea
    {
        /// <summary>
        /// Type of the area (Municipality, Province, BusinessRegions, HospitalRegions).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Code of the area.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Localized list of names for the area
        /// </summary>
        public IList<VmOpenApiLanguageItem> Name { get; set; }

        /// <summary>
        /// List of municipalities including municipality code and a localized list of municipality names.
        /// </summary>
        public IList<VmOpenApiMunicipality> Municipalities { get; set; }

        /// <summary>
        /// Gets or sets the owner reference identifier.
        /// </summary>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
    }
}
