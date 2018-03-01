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

namespace PTV.DataImport.ConsoleApp
{
    /// <summary>
    /// Contains configuration setting keys.
    /// </summary>
    public static class ConfigKeys
    {
        /// <summary>
        /// Connection string key for PTV database.
        /// </summary>
        public const string PTVConnectionString = "Data:ptvdb:ConnectionString";
        /// <summary>
        /// Connection string key for source database.
        /// </summary>
        public const string SourceConnectionString = "Data:sourcedb:ConnectionString";

        /// <summary>
        /// Proxy server settings
        /// </summary>
        public const string ProxyServerSettings = "ProxyServerSettings";

        /// <summary>
        /// Digital authorizations settings
        /// </summary>
        public const string DigitalAuthorizationsSettings = "Data:DigitalAuthorizations";




    }
}
