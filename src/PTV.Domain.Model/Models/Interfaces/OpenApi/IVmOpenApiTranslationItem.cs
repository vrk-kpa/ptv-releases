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

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of translation item
    /// </summary>
    public interface IVmOpenApiTranslationItem : IVmOpenApiItemBase
    {
        /// <summary>
        /// Id of the organization.
        /// </summary>
        Guid? OrganizationId { get; set; }
        /// <summary>
        /// Name of the organization.
        /// </summary>
        string OrganizationName { get; set; }
        /// <summary>
        /// Name of the organization.
        /// </summary>
        string BusinessCode { get; set; }
        /// <summary>
        /// Order made by (email).
        /// </summary>
        string Orderer { get; set; }
        /// <summary>
        /// Order identifier.
        /// </summary>
        long OrderId { get; set; }
        /// <summary>
        /// Order state.
        /// </summary>
        string OrderState { get; set; }
        /// <summary>
        /// Order date.
        /// </summary>
        DateTime? OrderDate { get; set; }
        /// <summary>
        /// Order resolved and delivered date.
        /// </summary>
        DateTime? OrderResolvedDate { get; set; }
        /// <summary>
        /// Source language.
        /// </summary>
        string SourceLanguage { get; set; }
        /// <summary>
        /// Amount of chars in source text.
        /// </summary>
        long SourceLanguageCharAmount { get; set; }
        /// <summary>
        /// Target language.
        /// </summary>
        string TargetLanguage { get; set; }
        /// <summary>
        /// Type of the item, either Service or Channel.
        /// </summary>
        string Type { get; set; }
        /// <summary>
        /// Id of the item.
        /// </summary>
        Guid? ItemId { get; set; }
        /// <summary>
        /// Name of the item.
        /// </summary>
        string ItemName { get; set; }
    }
}
