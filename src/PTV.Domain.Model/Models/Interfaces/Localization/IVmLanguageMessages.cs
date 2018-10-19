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

namespace PTV.Domain.Model.Models.Interfaces.Localization
{
    /// <summary>
    /// View model interface of language messages
    /// </summary>
    public interface IVmLanguageMessages
    {
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        string LanguageCode { get; set; }
        /// <summary>
        /// Gets or sets the texts.
        /// </summary>
        /// <value>
        /// The texts.
        /// </value>
        IDictionary<string, string> Texts { get; set; }
    }

    /// <summary>
    /// View model interface of translation item
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmEntityBase" />
    public interface IVmTranslationItem : IVmEntityBase
    {
        /// <summary>
        /// Default text for translation
        /// </summary>
        string DefaultText { get; set; }
        /// <summary>
        /// Gets or sets the texts.
        /// </summary>
        /// <value>
        /// The texts.
        /// </value>
        IDictionary<string, string> Texts { get; set; }
    }
}
