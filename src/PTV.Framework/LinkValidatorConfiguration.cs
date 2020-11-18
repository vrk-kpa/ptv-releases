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
using System.IO;

namespace PTV.Framework
{
    /// <summary>
    /// Configuration for validating links.
    /// </summary>
    public class LinkValidatorConfiguration
    {
        /// <summary>
        /// SEVI Url for validating links.
        /// </summary>
        public string UrlBase { get; set; }

        /// <summary>
        /// ApiKey to access the SEVI API.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Complete environment-aware link for link validation.
        /// </summary>
        public string ValidationUrl => $"{UrlBase}/sevi-link-validation-service/api/link/v1/validate?apikey={ApiKey}";

        /// <summary>
        /// How long should the request wait for a response, in seconds.
        /// </summary>
        public int Timeout { get; set; }
        
        /// <summary>
        /// How many web pages should be validated in one request.
        /// </summary>
        public int BatchSize { get; set; }
    }
}
