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

using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V10
{
    /// <summary>
    /// OPEN API - View Model of service collection item.
    /// </summary>
    public class V10VmOpenApiServiceCollectionItem : IVmOpenApiItem
    {
        /// <summary>
        /// Id of the item.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// List of localized service collection names.
        /// </summary>
        [JsonProperty(Order = 5)]
        public virtual IList<VmOpenApiLanguageItem> ServiceCollectionNames { get; set; }

        /// <summary>
        /// List of localized service collection descriptions.
        /// </summary>
        [JsonProperty(Order = 10)]
        public virtual IList<VmOpenApiLanguageItem> ServiceCollectionDescriptions { get; set; }

        /// <summary>
        /// List of service collection services.
        /// </summary>
        [JsonProperty(Order = 15)]
        public virtual IList<VmOpenApiServiceCollectionService> Services { get; set; }

        /// <summary>
        ///
        /// </summary>
        [JsonIgnore]
        public virtual string Name { get; set; }

    }
}
