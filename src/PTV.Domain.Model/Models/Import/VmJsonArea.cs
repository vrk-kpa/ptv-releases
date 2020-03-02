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
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Import
{
    /// <summary>
    /// Model for country from JSON
    /// </summary>
    public class VmJsonArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmJsonArea"/> class.
        /// </summary>
        public VmJsonArea()
        {
            Municipalities = new List<string>();
            AreaMunicipalities = new List<VmJsonAreaMunicipality>();
        }

        /// <summary>
        /// Gets or sets the area id.
        /// </summary>
        /// <value>
        /// The area id.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        public List<VmJsonName> Names { get; set; }

        /// <summary>
        /// Gets or sets the codes of municipalities.
        /// </summary>
        /// <value>
        /// The codes of municipalities.
        /// </value>
        public List<String> Municipalities { get; set; }

        /// <summary>
        /// Gets or sets the area municipalities.
        /// </summary>
        /// <value>
        /// The area municipalities.
        /// </value>
        public List<VmJsonAreaMunicipality> AreaMunicipalities { get; set; }

        /// <summary>
        /// Gets or sets the area type id.
        /// </summary>
        /// <value>
        /// The area type.
        /// </value>
        public Guid AreaTypeId { get; set; }

        /// <summary>
        /// Gets or sets the area IsValid property
        /// </summary>
        /// <value>
        /// The IsValid property.
        /// </value>
        public bool IsValid { get; set; }
    }
}
