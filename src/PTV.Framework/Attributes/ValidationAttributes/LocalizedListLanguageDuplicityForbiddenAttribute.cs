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
**/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Framework.Attributes
{
    public class LocalizedListLanguageDuplicityForbiddenAttribute : ValidationAttribute
    {
        private readonly string localizationPropertyName;

        public LocalizedListLanguageDuplicityForbiddenAttribute(string localizationPropertyName = "Language")
        {
            this.localizationPropertyName = localizationPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(localizationPropertyName))
            {
                throw new ValidationException(string.Format("Unknown property: {0}", localizationPropertyName));
            }
            if (value == null)
            {
                return ValidationResult.Success;
            }
            var list = value as IList;
            if (list == null)
            {
                throw new InvalidOperationException("The validation attribute is used on a property that doesn't implement IList (property name: 'UsedOnStringType').");
            }

            var uniqueLanguageList = new HashSet<string>();

            foreach (var item in list)
            {
                if (item == null)
                {
                    throw new ValidationException("List item cannot be null.");
                }
                var language = item.GetItemValue(localizationPropertyName) as string;
                if (language == null)
                {
                    throw new ValidationException("Property value cannot be null.");
                }

                if (!uniqueLanguageList.Add(language))
                {
                    return new ValidationResult(string.Format(CoreMessages.OpenApi.LocalizedDuplicitiesAreNotAllowed));
                }
            }

            return ValidationResult.Success;
        }
    }
}
