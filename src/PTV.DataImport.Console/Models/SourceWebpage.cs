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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.DataImport.ConsoleApp.Models
{
    public class SourceWebpage
    {
        // source data has 2 different kind of json definitions for webpage only difference is WebpageName and Name property
        // tables office_entity and organization

        public string Url { get; set; }
        /// <summary>
        /// Same as Name property. Use GetWebpageName() method to get the name.
        /// </summary>
        public string WebpageName { get; set; }
        public string WebpageType { get; set; }
        /// <summary>
        /// Same as WebpageName property. Use GetWebpageName() method to get the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tries to get the webpage name first from webpagename property and if it is null then from name property.
        /// </summary>
        /// <returns>webpage name</returns>
        public string GetWebpageName()
        {
            return string.IsNullOrEmpty(WebpageName) ? Name : WebpageName;
        }
    }
}
