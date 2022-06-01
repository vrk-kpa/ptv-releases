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
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace PTV.Framework.Attributes
{
    /// <summary>
    /// Validates all url string items in a list with the regular exression given. Verifies that the baseurl is uniform for all items.
    /// </summary>
    public class UrlListRegularExpressionAttribute : ValidationAttribute
    {
        private readonly string pattern;
        private RegularExpressionAttribute regularExpressionAttribute;

        public string Pattern => pattern;

        public UrlListRegularExpressionAttribute(string pattern)
        {
            regularExpressionAttribute = new RegularExpressionAttribute(pattern);
            this.pattern = pattern;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            var list = value as IList;

            if (list == null)
            {
                throw new InvalidOperationException("The UrlListRegularExpressionAttribute is used on a type that doesn't implement IList.");
            }

            if (list.Count == 0)
            {
                // List is empty
                return ValidationResult.Success;
            }

            var firstUri = new Uri(list[0]?.ToString());
            var baseUrl = $"{firstUri.Scheme}://{firstUri.Host}";

            foreach (var item in list)
            {
                var itemValue = item?.ToString();
                if (!regularExpressionAttribute.IsValid(itemValue))
                {
                    return new ValidationResult(FormatErrorMessage("item"));
                }
                if (!itemValue.StartsWith(baseUrl))
                {
                    return new ValidationResult("The urls do not have the same base url.");
                }
            }

            return ValidationResult.Success;
        }
    }
}