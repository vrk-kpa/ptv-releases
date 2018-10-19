﻿/**
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

namespace PTV.Domain.Model.Models.OpenApi.V6
{
    /// <summary>
    /// OPEN API - View Model of service channels
    /// </summary>
    public class V6VmOpenApiServiceChannels
    {
        /// <summary>
        /// Electronic channel.
        /// </summary>
        public V6VmOpenApiElectronicChannel ElectronicChannel { get; set; }
        /// <summary>
        /// Phone channel.
        /// </summary>
        public V6VmOpenApiPhoneChannel PhoneChannel { get; set; }
        /// <summary>
        /// Printable form channel.
        /// </summary>
        public V6VmOpenApiPrintableFormChannel PrintableFormChannel { get; set; }
        /// <summary>
        /// Location channel.
        /// </summary>
        public V6VmOpenApiServiceLocationChannel LocationChannel { get; set; }
        /// <summary>
        /// Web page channel.
        /// </summary>
        public V6VmOpenApiWebPageChannel WebPageChannel { get; set; }
    }
}
