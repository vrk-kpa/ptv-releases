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
using PTV.Domain.Model.Models.Interfaces.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V3
{
    /// <summary>
    /// OPEN API V3 - View Model of law
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V3.IV3VmOpenApiLaw" />
    public class V3VmOpenApiLaw : VmOpenApiBase, IV3VmOpenApiLaw
    {
        /// <summary>
        /// List of localized law names.
        /// </summary>
        [LocalizedListLanguageDuplicityForbidden]
        public IList<VmOpenApiLanguageItem> Names { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of localized web page urls.
        /// </summary>
        [ListRequired]
        [LocalizedListLanguageDuplicityForbidden]
        public IList<V3VmOpenApiWebPage> WebPages { get; set; } = new List<V3VmOpenApiWebPage>();

        /// <summary>
        /// Gets or sets the owner reference identifier.
        /// </summary>
        /// <value>
        /// The owner reference identifier.
        /// </value>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }

        /// <summary>
        /// Converts to version2.
        /// </summary>
        /// <returns>model converted to version 2</returns>
        public V2VmOpenApiLaw ConvertToVersion2()
        {
            var law = new V2VmOpenApiLaw
            {
                Names = this.Names
            };

            law.WebPages = new List<VmOpenApiWebPage>();
            this.WebPages.ForEach(a => law.WebPages.Add(a.ConvertToVersion2()));

            return law;
        }
    }
}
