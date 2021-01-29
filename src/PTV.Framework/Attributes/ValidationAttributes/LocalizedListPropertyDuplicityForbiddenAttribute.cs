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
    public class LocalizedListPropertyDuplicityForbiddenAttribute : ValidationAttribute
    {

        private readonly string propertyName;
        private readonly string localizationPropertyName;

        public LocalizedListPropertyDuplicityForbiddenAttribute(string propertyName, string localizationPropertyName = "Language")
        {
            if (string.IsNullOrWhiteSpace(propertyName) ||
               string.IsNullOrWhiteSpace(localizationPropertyName))
            {
                throw new ArgumentException("The property names shouldn't be null, empty string or whistespaces.");
            }

            this.propertyName = propertyName;
            this.localizationPropertyName = localizationPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && !(value is IList))
            {
                throw new InvalidOperationException(string.Format("The validation attribute is used on a property that doesn't implement IList (property name: '{0}').", validationContext.MemberName));
            }

            var list = value as IList;
            if (list == null)
            {
                return ValidationResult.Success;
            }

            var uniqueList = new Dictionary<string, List<object>>();

            foreach (var item in list)
            {
                if (item == null)
                {
                    return new ValidationResult(CoreMessages.OpenApi.NullNotAllowedInList);
                }
                var language = item.GetItemValue(localizationPropertyName) as string;
                if (language == null)
                {
                    return new ValidationResult(string.Format(CoreMessages.OpenApi.NullNotAllowedInProperty, localizationPropertyName));
                }

                if (!uniqueList.ContainsKey(language))
                {
                    uniqueList.Add(language, new List<object>());
                }

                var itemValue = item.GetItemValue(propertyName);
                if (itemValue == null)
                {
                    return new ValidationResult(string.Format(CoreMessages.OpenApi.NullNotAllowedInProperty, propertyName));
                }

                if (uniqueList[language].Contains(itemValue))
                {
                    return new ValidationResult(string.Format(CoreMessages.OpenApi.LocalizedDuplicitiesOfPropertyAreNotAllowed, propertyName));
                }
                uniqueList[language].Add(itemValue);
            }

            return ValidationResult.Success;
        }
    }
}
