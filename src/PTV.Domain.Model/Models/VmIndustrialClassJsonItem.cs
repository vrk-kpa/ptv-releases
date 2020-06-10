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
using PTV.Domain.Model.Models.Import;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of Industrial class from JSON
    /// </summary>
    public class VmIndustrialClassJsonItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmIndustrialClassJsonItem"/> class.
        /// </summary>
        public VmIndustrialClassJsonItem()
        {
            Children = new List<VmIndustrialClassJsonItem>();
        }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the uri.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Uri { get; set; }
        /// <summary>
        /// Gets or sets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        public List<JsonLanguageLabel> Names { get; set; }
        /// <summary>
        /// Gets or sets the hierarchy level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public int Level { get; set; }
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public string Parent { get; set; }
        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public List<VmIndustrialClassJsonItem> Children { get; set; }

        /// <summary>
        /// Gets <see cref="Uri"/> with <see cref="Code"/>.
        /// </summary>
        [JsonIgnore]
        public string UriCode => Uri + Code;
        /// <summary>
        /// Gets <see cref="Uri"/> with <see cref="Code"/>.
        /// </summary>
        [JsonIgnore]
        public string ParentUri => Uri + Parent;
    }
}
