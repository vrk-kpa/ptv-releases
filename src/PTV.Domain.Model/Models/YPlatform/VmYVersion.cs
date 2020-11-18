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

namespace PTV.Domain.Model.Models.YPlatform
{
    /// <summary>
    /// Information about versions of a code list from Y platform
    /// </summary>
    public class VmYVersion
    {
        /// <summary>
        /// Code list version
        /// </summary>
        public string CodeValue { get; set; }
        /// <summary>
        /// Url to the base
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Url to the codes data
        /// </summary>
        public string CodesUrl { get; set; }
        /// <summary>
        /// Url to available extensions
        /// </summary>
        public string ExtensionsUrl { get; set; }
        /// <summary>
        /// List of available extensions
        /// </summary>
        public List<VmYExtension> Extensions { get; set; }
        /// <summary>
        /// Version valid from
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Version valid to
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
