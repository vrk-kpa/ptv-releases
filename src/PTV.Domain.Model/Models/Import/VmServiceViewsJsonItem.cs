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
using Newtonsoft.Json;

namespace PTV.Domain.Model.Models.Import
{
    /// <summary>
    /// View model of service views finto stuff from json
    /// </summary>
    public class VmServiceViewsJsonItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmServiceViewsJsonItem"/> class.
        /// </summary>
        public VmServiceViewsJsonItem()
        {
            Label = new Dictionary<string, string>();
            Notes = new Dictionary<string, List<string>>();
            AssociatedURIs = new List<string>();
            NarrowerURIs = new List<string>();
            BroaderURIs = new List<string>();
            NarrowerURIs = new List<string>();
            ExactMatchURIs = new List<string>();
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the notation.
        /// </summary>
        /// <value>
        /// The notation.
        /// </value>
        public string Notation { get; set; }
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public Dictionary<string, string> Label { get; set; }
        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        /// <value>
        /// The notes.
        /// </value>
        public Dictionary<string, List<string>> Notes { get; set; }
        /// <summary>
        /// Gets or sets the scheme.
        /// </summary>
        /// <value>
        /// The scheme.
        /// </value>
        public string Scheme { get; set; }
        /// <summary>
        /// Gets or sets the type of the concept.
        /// </summary>
        /// <value>
        /// The type of the concept.
        /// </value>
        public string ConceptType { get; set; }
        /// <summary>
        /// Gets or sets the concept type URI.
        /// </summary>
        /// <value>
        /// The concept type URI.
        /// </value>
        public string ConceptTypeURI { get; set; }


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
        /// <summary>
        /// Gets or sets the associated URI's.
        /// </summary>
        /// <value>
        /// The associated URI's.
        /// </value>
        public List<string> AssociatedURIs { get; set; }
        /// <summary>
        /// Gets or sets the replaced by.
        /// </summary>
        /// <value>
        /// The replaced by.
        /// </value>
        public string ReplacedBy { get; set; }

        /// <summary>
        /// Gets or sets the exact match URI's.
        /// </summary>
        /// <value>
        /// The exact match URI's.
        /// </value>
        public List<string> ExactMatchURIs { get; set; }

    }

    /// <summary>
    /// Model to group items by replaced by id
    /// </summary>
    public class VmReplaceGroupServiceViewsJsonItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmReplaceGroupServiceViewsJsonItem"/> class.
        /// </summary>
        public VmReplaceGroupServiceViewsJsonItem()
        {
            Replaced = new List<VmReplaceItemServiceViewsJsonItem>();
        }

        /// <summary>
        /// Gets or sets the main.
        /// </summary>
        /// <value>
        /// The main.
        /// </value>
        public VmServiceViewsJsonItem Main { get; set; }
        /// <summary>
        /// Gets or sets the replaced.
        /// </summary>
        /// <value>
        /// The replaced.
        /// </value>
        public List<VmReplaceItemServiceViewsJsonItem> Replaced { get; set; }
    }

    /// <summary>
    /// View model to join item with replacing item
    /// </summary>
    public class VmReplaceItemServiceViewsJsonItem
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="VmReplaceItemServiceViewsJsonItem"/> class.
        /// </summary>
        public VmReplaceItemServiceViewsJsonItem()
        {
        }

        /// <summary>
        /// Gets or sets the replaced by item replacing <see cref="Item"/>.
        /// </summary>
        /// <value>
        /// The replaced by.
        /// </value>
        public VmServiceViewsJsonItem ReplacedBy { get; set; }
        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public VmServiceViewsJsonItem Item { get; set; }
    }
}