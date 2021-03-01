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

namespace PTV.Domain.Model.Models.OpenApi.V9
{
    /// <summary>
    /// OPEN API - View Model of entrance for address
    /// </summary>
    public class V9VmOpenApiEntrance : VmOpenApiEntrance
    {
        /// <summary>
        /// Contact info
        /// </summary>
        [JsonProperty(Order = 4)]
        public VmOpenApiAccessibilityContactInfo ContactInfo { get; set; }

        /// <summary>
        /// Convert to base
        /// </summary>
        /// <returns></returns>
        public VmOpenApiEntrance ConvertToBaseVersion()
        {
            return new VmOpenApiEntrance
            {
                Name = Name,
                IsMainEntrance = IsMainEntrance,
                Coordinates = Coordinates,
                AccessibilitySentences = AccessibilitySentences
            };
        }
    }
}
