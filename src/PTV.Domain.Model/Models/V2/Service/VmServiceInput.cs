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
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models.V2.Service
{
    /// <summary>
    /// ViewModel for basic service information
    /// </summary>
    public class VmServiceInput : VmServiceBase
    {
        /// <summary>
        /// Gets or sets the general description charge type id.
        /// </summary>
        /// <value>
        /// The general description charge type id.
        /// </value>
        [JsonIgnore]
        public Guid? GeneralDescriptionChargeTypeId { get; set; }
        /// <summary>
        /// Gets or sets the general description target group ids.
        /// </summary>
        /// <value>
        /// The list of target group ids.
        /// </value>
        [JsonIgnore]
        public List<Guid> GeneralDescriptionTargetGroups { get; set; } = new List<Guid>();
        /// <summary>
        /// Gets or sets the general description target group ids.
        /// </summary>
        /// <value>
        /// The list of service class ids.
        /// </value>
        [JsonIgnore]
        public List<Guid> GeneralDescriptionServiceClasses { get; set; } = new List<Guid>();
        /// <summary>
        /// Gets or sets the general description target group ids.
        /// </summary>
        /// <value>
        /// The list of ontology term ids.
        /// </value>
        [JsonIgnore]
        public List<Guid> GeneralDescriptionOntologyTerms { get; set; } = new List<Guid>();
        /// <summary>
        /// Gets or sets the service classes.
        /// </summary>
        /// <value>
        /// The service classes.
        /// </value>
        public List<Guid> ServiceClasses { get; set; }
        /// <summary>
        /// Gets or sets the industrial classes.
        /// </summary>
        /// <value>
        /// The industrial classes.
        /// </value>
        public List<Guid> IndustrialClasses { get; set; }
        /// <summary>
        /// Gets or sets the ontology terms.
        /// </summary>
        /// <value>
        /// The ontology terms.
        /// </value>
        public List<Guid> OntologyTerms { get; set; }
        /// <summary>
        /// Gets or sets the life events.
        /// </summary>
        /// <value>
        /// The life events.
        /// </value>
        public List<Guid> LifeEvents { get; set; }
        /// <summary>
        /// Gets or sets the new key words.
        /// </summary>
        /// <value>
        /// The new key words.
        /// </value>
        public Dictionary<string, List<string>>  NewKeywords { get; set; }
        /// <summary>
        /// Gets or sets the key words.
        /// </summary>
        /// <value>
        /// The key words.
        /// </value>
        public Dictionary<string, List<Guid>> Keywords { get; set; }

        /// <summary>
        /// Gets or sets the publish info.
        /// </summary>
        /// <value>
        /// Publish info.
        /// </value>
        //[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public bool Publish { get; set; }
    }
}
