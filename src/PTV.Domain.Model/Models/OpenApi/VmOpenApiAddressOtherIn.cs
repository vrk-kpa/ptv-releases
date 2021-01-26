﻿/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of other address.
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiAddressLocal" />
    public class VmOpenApiAddressOtherIn : VmOpenApiAddressLocalIn
    {
        /// <summary>
        /// Location latitude coordinate.
        /// </summary>
        [JsonProperty(Order = 1)]
        [Required]
        [RegularExpression(ValidationConsts.Coordinate)]
        public virtual string Latitude { get; set; }

        /// <summary>
        /// Location longitude coordinate.
        /// </summary>
        [JsonProperty(Order = 2)]
        [Required]
        [RegularExpression(ValidationConsts.Coordinate)]
        public virtual string Longitude { get; set; }

        /// <summary>
        /// Localized list of additional information about the address.
        /// </summary>
        [ListRequired]
        public override IList<VmOpenApiLanguageItem> AdditionalInformation { get => base.AdditionalInformation; set => base.AdditionalInformation = value; }

        /// <summary>
        /// Municipality code (e.g. 091).
        /// </summary>
        [JsonIgnore]
        public override string Municipality { get => base.Municipality; set => base.Municipality = value; }
    }
}
