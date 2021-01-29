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

namespace PTV.Domain.Model.Models.YPlatform
{
    /// <summary>
    /// Structure to hold Y-platform code data (service classes, target groups, life events, industrial classes,
    /// organization types and service producers)
    /// </summary>
    public class VmYCodeData
    {
        /// <summary>
        /// Y-platform unique code.
        /// </summary>
        public string CodeValue { get; set; }
        /// <summary>
        /// Y-platform URI.
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        /// Whether the code is valid or not.
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Value computed from the Status field.
        /// </summary>
        [JsonIgnore]
        public bool IsValid => Status?.ToLower() == "valid";
        /// <summary>
        /// Names in Finnish, Swedish and English.
        /// </summary>
        public Dictionary<string, string> PrefLabel { get; set; }
        /// <summary>
        /// Descriptions in Finnish, Swedish and English.
        /// </summary>
        public Dictionary<string, string> Description { get; set; }
        /// <summary>
        /// Parent entity.
        /// </summary>
        public VmYCodeData BroaderCode { get; set; }
        /// <summary>
        /// Short name of the code.
        /// </summary>
        public string ShortName { get; set; }
    }
}
