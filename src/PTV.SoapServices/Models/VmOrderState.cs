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
using PTV.SoapServices.Enums;

namespace PTV.SoapServices.Models
{
    /// <summary>
    /// View model of status for status answer from translation company.  
    /// </summary>
    public class VmOrderState
    {
        /// <summary>
        /// Order id
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// The Url of file
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// Validation result of translation
        /// </summary>
        public string ValidationResult { get; set; }

        /// <summary>
        /// Order state Type 
        /// </summary>
        public ClientOrderStateEnum State { get; set; }
        
        /// <summary>
        /// Deadline of translation
        /// </summary>
        public DateTime? Deadline { get; set; }

        /// <summary>
        /// Contact
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}
