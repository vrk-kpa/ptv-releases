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
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of finto item - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiFintoItemVersionBase" />
    public class VmOpenApiFintoItemVersionBase : IVmOpenApiFintoItemVersionBase
    {
        /// <summary>
        /// Entity Guid identifier.
        /// </summary>
        [JsonIgnore]
        public Guid Id { get; set; }
        /// <summary>
        /// List of localized entity names.
        /// </summary>
        public IList<VmOpenApiLanguageItem> Name { get; set; }
        /// <summary>
        /// Entity code.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Ontology term type.
        /// </summary>
        public string OntologyType { get; set; }
        /// <summary>
        /// Entity uri.
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        /// Entity parent identifier.
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// Entity parent uri.
        /// </summary>
        public string ParentUri { get; set; }
        /// <summary>
        /// Indicates if item is overriden by general description.
        /// </summary>
        [JsonIgnore]
        public bool Override { get; set; }
    }
}
