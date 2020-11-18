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
using PTV.Framework.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API V7 - View Model of address with type
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiAddressStreetIn" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiAddressStreetWithCoordinatesIn" />
    public class VmOpenApiAddressStreetWithCoordinatesIn : VmOpenApiAddressStreetIn, IVmOpenApiAddressStreetWithCoordinatesIn
    {
        /// <summary>
        /// List of localized street addresses.
        /// </summary>
        [ValidStreetAddress]
        [JsonProperty(Order = 1)]
        public override IList<VmOpenApiLanguageItem> Street { get => base.Street; set => base.Street = value; }

        /// <summary>
        /// Location latitude coordinate.
        /// </summary>
        [RegularExpression(ValidationConsts.Coordinate)]
        [JsonProperty(Order = 10)]
        public virtual string Latitude { get; set; }

        /// <summary>
        /// Location longitude coordinate.
        /// </summary>
        [RegularExpression(ValidationConsts.Coordinate)]
        [JsonProperty(Order = 11)]
        public virtual string Longitude { get; set; }
    }
}
