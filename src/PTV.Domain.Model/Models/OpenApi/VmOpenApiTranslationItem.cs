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

using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of translation item
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiTranslationItem" />
    public class VmOpenApiTranslationItem : IVmOpenApiTranslationItem
    {
        /// <summary>
        /// Id of the organization.
        /// </summary>
        public Guid? OrganizationId { get; set; }
        /// <summary>
        /// Name of the organization.
        /// </summary>
        public string OrganizationName { get; set; }
        /// <summary>
        /// Name of the organization.
        /// </summary>
        public string BusinessCode { get; set; }
        /// <summary>
        /// Order made by (email).
        /// </summary>
        public string Orderer { get; set; }
        /// <summary>
        /// Order identifier.
        /// </summary>
        public long OrderId { get; set; }
        /// <summary>
        /// Order state.
        /// </summary>
        public string OrderState { get; set; }
        /// <summary>
        /// Order date.
        /// </summary>
        public DateTime? OrderDate { get; set; }
        /// <summary>
        /// Order resolved and delivered date.
        /// </summary>
        public DateTime? OrderResolvedDate { get; set; }
        /// <summary>
        /// Source language.
        /// </summary>
        public string SourceLanguage { get; set; }
        /// <summary>
        /// Amount of chars in source text.
        /// </summary>
        public long SourceLanguageCharAmount { get; set; }
        /// <summary>
        /// Target language.
        /// </summary>
        public string TargetLanguage { get; set; }
        /// <summary>
        /// Type of the item, either Service or Channel.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Id of the item.
        /// </summary>
        public Guid? ItemId { get; set; }
        /// <summary>
        /// Name of the item.
        /// </summary>
        public string ItemName { get; set; }
    }
}
