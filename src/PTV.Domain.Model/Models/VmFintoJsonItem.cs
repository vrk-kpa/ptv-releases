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
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of item from JSON
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IBaseFintoItem" />
    public class VmFintoJsonItem : IBaseFintoItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmFintoJsonItem"/> class.
        /// </summary>
        public VmFintoJsonItem()
        {
            Narrower = new List<VmFintoJsonItem>();
            Parents = new List<string>();
            Children = new List<string>();
            BroaderURIs = new List<string>();
            NarrowerURIs = new List<string>();
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; set; }
        /// <summary>
        /// Gets or sets the finnish text (default).
        /// </summary>
        /// <value>
        /// The finnish text (default).
        /// </value>
        public string Finnish { get; set; }
        /// <summary>
        /// Gets or sets the type of the ontology.
        /// </summary>
        /// <value>
        /// The type of the ontology.
        /// </value>
        public string OntologyType { get; set; }
        /// <summary>
        /// Gets or sets the notation.
        /// </summary>
        /// <value>
        /// The notation.
        /// </value>
        public string Notation { get; set; }

        /// <summary>
        /// Gets or sets the narrower.
        /// </summary>
        /// <value>
        /// The narrower.
        /// </value>
        public List<VmFintoJsonItem> Narrower { get; set; }
        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public List<string> Children { get; set; }
        /// <summary>
        /// Gets or sets the parents.
        /// </summary>
        /// <value>
        /// The parents.
        /// </value>
        public List<string> Parents { get; set; }

        /// <summary>
        /// Gets or sets the broader URI's.
        /// </summary>
        /// <value>
        /// The broader URI's.
        /// </value>
        public List<string> BroaderURIs { get; set; }
        /// <summary>
        /// Gets or sets the narrower URI's.
        /// </summary>
        /// <value>
        /// The narrower URI's.
        /// </value>
        public List<string> NarrowerURIs { get; set; }
    }
}
