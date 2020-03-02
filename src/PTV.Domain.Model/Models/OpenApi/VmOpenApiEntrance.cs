/**
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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of entrance for address
    /// </summary>
    public class VmOpenApiEntrance
    {
        /// <summary>
        /// List of localized service names.
        /// </summary>
        [JsonProperty(Order = 1)]
        public virtual IList<VmOpenApiLanguageItem> Name { get; set; }

        /// <summary>
        /// Indicates if entrance is main entrance.
        /// </summary>
        [JsonProperty(Order = 2)]
        public virtual bool IsMainEntrance { get; set; }

        /// <summary>
        /// Location coordinates
        /// </summary>
        [JsonProperty(Order = 3)]
        public virtual VmOpenApiCoordinates Coordinates { get; set; }

        /// <summary>
        /// List of accessibility sentences.
        /// </summary>
        [JsonProperty(Order = 10)]
        public virtual IList<VmOpenApiAccessibilitySentence> AccessibilitySentences { get; set; }
    }
}
