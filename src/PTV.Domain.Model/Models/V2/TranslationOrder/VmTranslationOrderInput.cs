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
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Domain.Model.Models.V2.TranslationOrder
{
    /// <summary>
    ///  View model for sending translation order
    /// </summary>
    public class VmTranslationOrderInput : VmTranslationOrderBase, IVmTranslationOrderInput
    {
        /// <summary>
        /// Constructor of transaltion order
        /// </summary>
        public VmTranslationOrderInput()
        {
            RequiredLanguages = new List<Guid>();
        }

        /// <summary>
        /// Identifier of translation company.
        /// </summary>
        public Guid TranslationCompanyId { get; set; }

        /// <summary>
        /// Required languages for translation
        /// </summary>
        public List<Guid> RequiredLanguages { get; set; }

        /// <summary>
        /// Source entity name
        /// </summary>
        public string SourceEntityName { get; set; }

        /// <summary>
        /// Source organization name
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Source organization business code
        /// </summary>
        public string OrganizationBusinessCode { get; set; }

        /// <summary>
        /// Source organization identifier
        /// </summary>
        public Guid? OrganizationIdentifier { get; set; }

        /// <summary>
        /// Source language char amount
        /// </summary>
        public long SourceLanguageCharAmount { get; set; }


    }
}
