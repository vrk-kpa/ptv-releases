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
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    ///  View model of accessibility register
    /// </summary>
    public class VmAccessibilityRegisterBase : VmEntityBase
    {
        /// <summary>
        /// ServiceChannelVersionedId
        /// </summary>
        public  Guid ServiceChannelVersionedId { get; set; }

        /// <summary>
        /// URL for setting of Accessibility Register
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// ServiceChannelId
        /// </summary>
        public Guid? ServiceChannelId { get; set; }

        /// <summary>
        /// Address language
        /// </summary>
        public string AddressLanguage { get; set; }

        /// <summary>
        /// Address language
        /// </summary>
        [JsonIgnore]
        public Guid AddressLanguageId { get; set; }

        /// <summary>
        /// IsExpired
        /// </summary>
        public bool IsExpired { get; set; }

        /// <summary>
        /// Set at
        /// </summary>
        public DateTime? SetAt { get; set; }

        /// <summary>
        /// Address id
        /// </summary>
        [JsonIgnore]
        public Guid AddressId { get; set; }
        
        /// <summary>
        /// Address
        /// </summary>
        [JsonIgnore]
        public VmAddressSimple Address { get; set; }
        
        /// <summary>
        /// Contact phone
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// Contact email
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// Contact url
        /// </summary>
        public string ContactUrl { get; set; }
    }
}
