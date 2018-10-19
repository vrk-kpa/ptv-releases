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
using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.Domain.Model.Models.OpenApi.Extensions
{
    /// <summary>
    /// Extensions for string type
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Set the value property length of the string as defined.
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="length">The length of value string</param>
        /// <returns></returns>
        public static string SetStringValueLength(this string value, int length)
        {
            return value?.Substring(0, value.Length > length ? length : value.Length);
        }

        /// <summary>
        /// Converts a string into language item list, value as a Finnish translation.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>A language item list</returns>
        public static List<VmOpenApiLanguageItem> ConvertToLanguageList(this string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return new List<VmOpenApiLanguageItem>() { new VmOpenApiLanguageItem() { Language = DomainConstants.DefaultLanguage, Value = value } };
        }
    }
}
