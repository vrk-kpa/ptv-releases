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
    ///  View model for sending translation order state
    /// </summary>
    public class VmTranslationCompany : IVmBase
    {
        /// <summary>
        /// Constructor of translation company
        /// </summary>
        public VmTranslationCompany()
        {
           
        }

        /// <summary>
        /// Identifier of translation company
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the email of translation company.
        /// </summary>
        /// <value>
        /// The email of translation company.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the url of translation company.
        /// </summary>
        /// <value>
        /// The url of translation company.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the name of translation company.
        /// </summary>
        /// <value>
        /// The name of translation company.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of translation company.
        /// </summary>
        /// <value>
        /// The description of translation company.
        /// </value>
        public string Description { get; set; }


    }
}
