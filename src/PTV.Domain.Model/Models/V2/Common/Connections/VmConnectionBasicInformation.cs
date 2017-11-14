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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.V2.Common
{
    /// <summary>
    /// ViewModel for connection basic information
    /// </summary>
    public class VmConnectionBasicInformation
    {        
        /// <summary>
        /// Gets or sets the descriptions.
        /// </summary>
        /// <value>
        /// The descriptions.
        /// </value>
        public Dictionary<string, string> Description { get; set; }
        /// <summary>
        /// Gets or sets the type id of the charge.
        /// </summary>
        /// <value>
        /// The type id of the charge.
        /// </value>
        public Guid? ChargeType { get; set; }
        /// <summary>
        /// Gets or sets the charge type additional informations.
        /// </summary>
        /// <value>
        /// The charge type additional informations.
        /// </value>
        public Dictionary<string, string> AdditionalInformation { get; set; }        
    }
}
