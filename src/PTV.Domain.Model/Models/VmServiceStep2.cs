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
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Converters;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of service step 2
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEnumEntityBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmServiceStep2" />
    public class VmServiceStep2 : VmEnumEntityBase, IVmServiceStep2
    {
        /// <summary>
        /// Gets or sets the service classes.
        /// </summary>
        /// <value>
        /// The service classes.
        /// </value>
        [JsonConverter(typeof(ListItemListConverter))]
        public List<VmListItem> ServiceClasses { get; set; }
        /// <summary>
        /// Gets or sets the industrial classes.
        /// </summary>
        /// <value>
        /// The industrial classes.
        /// </value>
        [JsonConverter(typeof(ListItemListConverter))]
        public List<VmListItem> IndustrialClasses { get; set; }
        /// <summary>
        /// Gets or sets the ontology terms.
        /// </summary>
        /// <value>
        /// The ontology terms.
        /// </value>
        [JsonConverter(typeof(ListItemListConverter))]
        public List<VmListItem> OntologyTerms { get; set; }
        /// <summary>
        /// Gets or sets the life events.
        /// </summary>
        /// <value>
        /// The life events.
        /// </value>
        [JsonConverter(typeof(ListItemListConverter))]
        public List<VmListItem> LifeEvents { get; set; }
        /// <summary>
        /// Gets or sets the target groups.
        /// </summary>
        /// <value>
        /// The target groups.
        /// </value>
        public List<Guid> TargetGroups { get; set; }
        /// <summary>
        /// Gets or sets target groups, that will override the general description target groups.
        /// </summary>
        /// <value>
        /// The overriding target groups.
        /// </value>
        public List<Guid> OverrideTargetGroups { get; set; }
        /// <summary>
        /// Gets or sets the key words.
        /// </summary>
        /// <value>
        /// The key words.
        /// </value>
        public List<Guid> KeyWords { get; set; }
        /// <summary>
        /// Gets or sets the new key words (not exist in db).
        /// </summary>
        /// <value>
        /// The new key words.
        /// </value>
        public List<VmKeywordItem> NewKeyWords { get; set; }
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public LanguageCode Language { get; set; }
        /// <summary>
        /// Gets the ontology terms from annotation tool.
        /// </summary>
        /// <value>
        /// The annotation ontology terms.
        /// </value>
        public List<VmListItem> AnnotationOntologyTerms { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="VmServiceStep2"/> class.
        /// </summary>
        public VmServiceStep2()
        {
            TargetGroups = new List<Guid>();
            OverrideTargetGroups = new List<Guid>();
            ServiceClasses = new List<VmListItem>();
            IndustrialClasses = new List<VmListItem>();
            OntologyTerms = new List<VmListItem>();
            LifeEvents = new List<VmListItem>();
            KeyWords = new List<Guid>();
            NewKeyWords = new List<VmKeywordItem>();
            AnnotationOntologyTerms = new List<VmListItem>();
        }
    }

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
        /// The language code.
        /// </value>
        public LanguageCode Language { get; set; }
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
