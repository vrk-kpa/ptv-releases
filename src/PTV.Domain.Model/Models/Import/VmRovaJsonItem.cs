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

namespace PTV.Domain.Model.Models.Import
{
    /// <summary>
    /// View model of rova finto stuff from json
    /// </summary>
    public class VmRovaJsonItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmRovaJsonItem"/> class.
        /// </summary>
        public VmRovaJsonItem()
        {
            Labels = new List<JsonLanguageLabel>();
            Definitions = new List<JsonLanguageLabel>();
            ConceptMatches = new List<string>();
            NarrowerConcepts = new List<string>();
            BroaderConcepts = new List<string>();
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>
        /// The URI.
        /// </value>
        public string Uri { get; set; }
        /// <summary>
        /// Gets or sets the labels.
        /// </summary>
        /// <value>
        /// The labels.
        /// </value>
        public List<JsonLanguageLabel> Labels { get; set; }
        /// <summary>
        /// Gets or sets the definitions.
        /// </summary>
        /// <value>
        /// The definitions.
        /// </value>
        public List<JsonLanguageLabel> Definitions { get; set; }

        /// <summary>
        /// Gets or sets the broader concepts.
        /// </summary>
        /// <value>
        /// The broader concepts.
        /// </value>
        public List<string> BroaderConcepts { get; set; }
        /// <summary>
        /// Gets or sets the narrower concepts.
        /// </summary>
        /// <value>
        /// The narrower concepts.
        /// </value>
        public List<string> NarrowerConcepts { get; set; }
        /// <summary>
        /// Gets or sets the concept matches.
        /// </summary>
        /// <value>
        /// The concept matches.
        /// </value>
        public List<string> ConceptMatches { get; set; }
    }
}
