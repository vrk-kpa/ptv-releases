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

namespace PTV.Framework
{
    /// <summary>
    /// Contains ionformation map dns
    /// </summary>
    public class MapDNSes
    {
        private string _fi = string.Empty;

        /// <summary>
        /// Fi DNS
        /// </summary>
        public string Fi
        {
            get { return Uri.EscapeUriString(_fi); }
            set { _fi = value; }
        }

        private string _sv = string.Empty;

        /// <summary>
        /// Sv DNS
        /// </summary>
        public string Sv
        {
            get { return Uri.EscapeUriString(_sv); }
            set { _sv = value; }
        }

        private string _en = string.Empty;
        /// <summary>
        /// En DNS
        /// </summary>
        public string En
        {
            get { return Uri.EscapeUriString(_en); }
            set { _en = value; }
        }
    }

    public class RequestFilterAppSetting
    {
        public int MaxRequestsPerIp { get; set; }
        public int MaxRequestsPerUser { get; set; }
        public int MaxRequestsTotal { get; set; }
    }

}
