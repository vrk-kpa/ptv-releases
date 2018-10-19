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

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Converters;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of annotations
    /// </summary>
    /// <seealso cref="PTV.Framework.Interfaces.IVmBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmLocalized" />
    public class VmAnnotations : IVmBase, IVmLocalized
    {
        /// <summary>
        /// Gets or sets the ontology terms from annotation tool.
        /// </summary>
        /// <value>
        /// The annotation ontology terms.
        /// </value>
        [JsonConverter(typeof(ListItemListConverter))]
        public List<VmListItem> AnnotationOntologyTerms { get; set; }
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language id.
        /// </value>
        public Guid LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="VmAnnotations"/> class.
        /// </summary>
        public VmAnnotations()
        {
            AnnotationOntologyTerms = new List<VmListItem>();
        }
    }
}