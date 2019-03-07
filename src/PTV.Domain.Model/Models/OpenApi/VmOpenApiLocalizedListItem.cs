﻿/**
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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using PTV.Framework.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using PTV.Framework.Attributes.ValidationAttributes;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of localized list item
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiListItem" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiLocalizedListItem" />
    public class VmOpenApiLocalizedListItem : VmOpenApiListItem, IVmOpenApiLocalizedListItem
    {
        /// <summary>
        /// Language code.
        /// </summary>
        [ValidLanguage]
        [Required(AllowEmptyStrings = false)]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the owner reference id2.
        /// </summary>
        /// <value>
        /// The owner reference id2.
        /// </value>
        [JsonIgnore]
        public Guid? OwnerReferenceId2 { get; set; }

        /// <summary>
        /// Gets or sets the order number.
        /// </summary>
        [JsonIgnore]
        public int OrderNumber { get; set; }

        #region Methods

        /// <summary>
        /// Set localized list item values for version 7.
        /// </summary>
        public void SetV7Values<TEnum>()
        {
            Type = string.IsNullOrEmpty(Type) ? Type : Type.GetEnumValueByOpenApiEnumValue<TEnum>();
        }

        #endregion
    }
}
