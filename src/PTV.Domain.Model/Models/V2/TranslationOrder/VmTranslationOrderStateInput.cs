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
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models.V2.TranslationOrder
{
    /// <summary>
    ///  View model for translation order state input
    /// </summary>
    public class VmTranslationOrderStateInput : IVmBase
    {
        /// <summary>
        /// Constructor of translation company
        /// </summary>
        public VmTranslationOrderStateInput()
        {
           
        }

        /// <summary>
        /// Gets or sets identfier of order state.
        /// </summary>
        /// <value>
        /// The identfier of Order state.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets  translation order id.
        /// </summary>
        /// <value>
        /// The translation order id.
        /// </value>
        public Guid TranslationOrderId { get; set; }

        /// <summary>
        /// Gets or sets translation state id.
        /// </summary>
        /// <value>
        /// The translation state id.
        /// </value>
        public Guid TranslationStateId { get; set; }
        
        /// <summary>
        /// Gets or sets checked value.
        /// </summary>
        /// <value>
        /// The checked value.
        /// </value>
        public bool Checked { get; set; }


        /// <summary>
        /// Gets or sets last value.
        /// </summary>
        /// <value>
        /// The last value.
        /// </value>
        public bool Last { get; set; }
        
        
        /// <summary>
        /// Additional information related to state
        /// </summary>
        public string InfoDetails { get; set; }

    }
}
