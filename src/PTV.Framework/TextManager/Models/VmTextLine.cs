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
using System.Linq;
using System.Threading.Tasks;

namespace PTV.Framework.TextManager.Models
{
    /// <summary>
    /// Line structure of draftJs component
    /// </summary>
    public class VmTextLine
    {
        /// <summary>
        /// Text of draftJs component
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Types of line of draftJs component
        /// </summary>
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public TextFormatClientTypeEnum Type { get; set; }

        /// <summary>
        /// Unique key of line draftJs component
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Depth key of line draftJs component
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// InlineStyleRanges of line draftJs component
        /// </summary>
        public HashSet<object> InlineStyleRanges { get; set; }

        /// <summary>
        /// EntityRanges of line draftJs component
        /// </summary>
        public HashSet<object> EntityRanges { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public VmTextLine()
        {
            InlineStyleRanges = new HashSet<object>();
            EntityRanges = new HashSet<object>();
        }

    }
}
