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

namespace PTV.Domain.Model.Models
{
    /// <summary>
    ///  View model of accessibility register sentence
    /// </summary>
    public class VmJsonAccessibilityRegisterSentence
    {
        /// <summary>
        /// System id
        /// </summary>
        public Guid SystemId { get; set; }

        /// <summary>
        /// EntranceId
        /// </summary>
        public int EntranceId { get; set; }

        /// <summary>
        /// SentenceOrderText
        /// </summary>
        public string SentenceOrderText { get; set; }

        /// <summary>
        /// SentenceGroups
        /// </summary>
        public List<VmAccessibilityRegisterLanguageItem> SentenceGroups { get; set; }

        /// <summary>
        /// Sentences
        /// </summary>
        public List<VmAccessibilityRegisterLanguageItem> Sentences { get; set; }


    }
}
