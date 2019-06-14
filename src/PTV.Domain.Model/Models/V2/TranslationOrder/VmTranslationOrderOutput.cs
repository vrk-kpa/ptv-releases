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
    ///  View model for sending translation order
    /// </summary>
    public class VmTranslationOrderOutput : VmTranslationOrderBase, IVmTranslationOrderInput
    {
        /// <summary>
        /// Gets or sets translation company name.
        /// </summary>
        /// <value>
        /// The translation company name.
        /// </value>
        public string TranslationCompanyName { get; set; }

        /// <summary>
        /// Gets or sets translation company email.
        /// </summary>
        /// <value>
        /// The translation company email.
        /// </value>
        public string TranslationCompanyEmail { get; set; }

        /// <summary>
        /// Gets or sets source language code.
        /// </summary>
        /// <value>
        /// The identfier of source language code.
        /// </value>
        public string SourceLanguageCode { get; set; }

        /// <summary>
        /// Gets or sets target language code.
        /// </summary>
        /// <value>
        /// The identfier of source language code.
        /// </value>
        public string TargetLanguageCode { get; set; }

        //Detail information
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the organization id.
        /// </summary>
        /// <value>
        /// The organization id.
        /// </value>
        public Guid? EntityOrganizationId { get; set; }

    }
}
