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

using PTV.Framework.ServiceManager;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of language code
    /// </summary>
    public class VmLanguage : VmEntityBase
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this language code is default.
        /// </summary>
        /// <value>
        /// <c>true</c> if this language code is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this language code is for data.
        /// </summary>
        /// <value>
        /// <c>true</c> if this language code is for data; otherwise, <c>false</c>.
        /// </value>
        public bool IsForData { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this language code can be ordered for translation.
        /// </summary>
        /// <value>
        /// <c>true</c> if this language code can be ordered for translation; otherwise, <c>false</c>.
        /// </value>
        public bool IsForTranslationOrder { get; set; }
        /// <summary>
        /// Gets or sets the en code.
        /// </summary>
        /// <value>
        /// The en.
        /// </value>
        public string En { get; set; }
        /// <summary>
        /// Gets or sets the fi code.
        /// </summary>
        /// <value>
        /// The fi.
        /// </value>
        public string Fi { get; set; }
        /// <summary>
        /// Gets or sets the sv code.
        /// </summary>
        /// <value>
        /// The sv.
        /// </value>
        public string Sv { get; set; }
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }
        /// <summary>
        /// Gets or sets the native name.
        /// </summary>
        /// <value>
        /// The native name.
        /// </value>
        public string NativeName { get; set; }
    }
}
