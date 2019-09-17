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

namespace PTV.Domain.Model.Models.V2.TranslationOrder
{
    /// <summary>
    ///  View model for sending translation order state output
    /// </summary>
    public class VmTranslationOrderStateOutput
    {
        /// <summary>
        /// Constructor of transaltion order state
        /// </summary>
        public VmTranslationOrderStateOutput()
        {
           
        }

        /// <summary>
        /// Identifier of translation order state
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the state of tranlsation .
        /// </summary>
        /// <value>
        /// The state of translation.
        /// </value>
        public Guid TranslationStateTypeId { get; set; }

        /// <summary>
        /// Gets or sets the datetime of sent translation.
        /// </summary>
        /// <value>
        /// The datetime of sent translation.
        /// </value>
        public long SentAt { get; set; }

        /// <summary>
        /// Gets or sets the presumed datetime of translation.
        /// </summary>
        /// <value>
        /// The presumed datetime of translation.
        /// </value>
        public long? DeliverAt { get; set; }
        
        /// <summary>
        /// Gets or sets the last state.
        /// </summary>
        /// <value>
        /// The Last state of translation.
        /// </value>
        public bool Last { get; set; }

        /// <summary>
        /// Gets or sets the translation order.
        /// </summary>
        /// <value>
        /// The translation order.
        /// </value>
        public VmTranslationOrderOutput TranslationOrder { get; set; }
    }
}
