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
    ///  View model for sending translation order state
    /// </summary>
    public class VmTranslationOrderBase
    {
        /// <summary>
        /// Constructor of transaltion order state
        /// </summary>
        public VmTranslationOrderBase()
        {
           
        }

        /// <summary>
        /// Identifier of translation
        /// </summary>
        public Guid Id { get; set; }


        /// <summary>
        /// Identifier of translation
        /// </summary>
        public Guid? PreviousTranslationOrderId { get; set; }

        /// <summary>
        /// Identifier of contentId
        /// </summary>
        public Guid? ContentId { get; set; }
        
        /// <summary>
        /// Identifier of entityId that should be translate
        /// </summary>
        public Guid EntityId { get; set; }

        /// <summary>
        /// Unific root id
        /// </summary>
        public Guid EntityRootId { get; set; }

        /// <summary>
        /// Gets or sets source language.
        /// </summary>
        /// <value>
        /// The identfier of source language.
        /// </value>
        public Guid SourceLanguage { get; set; }

        /// <summary>
        /// Gets or sets target language.
        /// </summary>
        /// <value>
        /// The identfier of target language.
        /// </value>
        public Guid TargetLanguage { get; set; }

        /// <summary>
        /// Gets or sets order identifier.
        /// </summary>
        /// <value>
        /// The order identifier.
        /// </value>
        public long OrderIdentifier { get; set; }

        /// <summary>
        /// Gets or sets sender name.
        /// </summary>
        /// <value>
        /// The identfier of sender name.
        /// </value>
        public string SenderName { get; set; }

        /// <summary>
        /// Gets or sets the sender email.
        /// </summary>
        /// <value>
        /// The sender email.
        /// </value>
        public string SenderEmail { get; set; }

        /// <summary>
        /// Gets or sets the addiitonal information.
        /// </summary>
        /// <value>
        /// The additional information.
        /// </value>
        public string AdditionalInformation { get; set; }

    }
}
