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
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of Accessibility Classification
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmAreaInformation" />
    public class VmAccessibilityClassification : VmEntityBase, IVmOwnerReference, IVmUrl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmAreaInformation"/> class.
        /// </summary>
        public VmAccessibilityClassification()
        {
            
        }

        /// <summary>
        /// Gets or sets the accessibility classification level type.
        /// </summary>
        /// <value>
        /// The accessibility classification level type.
        /// </value>
        [JsonProperty("AccessibilityClassificationLevelType")]
        public Guid? AccessibilityClassificationLevelTypeId { get; set; }
       
        /// <summary>
        /// Gets or sets the Wcag Level Type.
        /// </summary>
        /// <value>
        /// The Wcag Level Type.
        /// </value>
        [JsonProperty("WcagLevelType")]
        public Guid? WcagLevelTypeId { get; set; }
        
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the URL address.
        /// </summary>
        /// <value>
        /// The URL address.
        /// </value>
        public string UrlAddress { get; set; }

        /// <summary>
        /// Gets or sets the owner reference Id.
        /// </summary>
        /// <value>
        /// the owner reference Id.
        /// </value>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
        
        /// <summary>
        /// Id of language for web page
        /// </summary>
        [JsonIgnore]
        public Guid? LocalizationId { get; set; }
    }
}
