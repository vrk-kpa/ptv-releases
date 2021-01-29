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
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models.YPlatform
{
    /// <summary>
    /// View model of municipality from json
    /// </summary>
    public class VmYCodedArea
    {
        /// <summary>
        /// Municipality 3-digit code.
        /// </summary>
        public string CodeValue { get; set; }

        /// <summary>
        /// Validity status of the record.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Determines if the record is valid.
        /// </summary>
        [JsonIgnore]
        public bool IsValid => Status.ToLower() == "valid";
        
        /// <summary>
        /// Localized names in Finnish, Swedish and English.
        /// </summary>
        public Dictionary<string, string> PrefLabel { get; set; }
        
        /// <summary>
        /// Imported area type.
        /// </summary>
        [JsonIgnore]
        public AreaTypeEnum AreaType { get; set; }
    }
}
