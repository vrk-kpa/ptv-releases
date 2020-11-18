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
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of email
    /// </summary>
    public class VmServiceNumber : IVmServiceNumber
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [JsonIgnore]
        public Guid? Id { get; set; }
        
        /// <summary>
        /// Service number
        /// </summary>
        public string Number { get; set; }
        
        /// <summary>
        /// Company
        /// </summary>
        [JsonIgnore]
        public string Company { get; set; }
        
        /// <summary>
        /// Is Valid
        /// </summary>
        [JsonIgnore]
        public bool IsValid { get; set; }
    }
    
    /// <summary>
    /// View model of email
    /// </summary>
    public class VmServiceNumberInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmServiceNumberInput"/> class.
        /// </summary>
        public VmServiceNumberInput()
        {
            Value = new List<VmServiceNumber>();
        }
        
        /// <summary>
        /// Gets or sets the values from service numbers.
        /// </summary>
        /// <value>
        /// The Value of service numbers.
        /// </value>
        public List<VmServiceNumber> Value { get; set; }
    }
}
