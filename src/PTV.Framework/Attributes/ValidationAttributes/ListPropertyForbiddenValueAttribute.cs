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
using System.ComponentModel.DataAnnotations;

namespace PTV.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ListPropertyForbiddenValueAttribute : ValidationAttribute
    {
        private readonly string propertyName;
        private readonly string forbiddenValue;

        public ListPropertyForbiddenValueAttribute(string forbiddenValue, string propertyName = null)
        {
            this.propertyName = propertyName;
            this.forbiddenValue = forbiddenValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            var list = value as IList;
            if (list == null)
            {
                throw new Exception("Because attribute is used on a wrong type (should be used only on types that implement IList).");
            }

            foreach (var item in list)
            {
                var itemValue = item.GetItemValue(propertyName) as string;
                if (itemValue == null)
                {
                    return new ValidationResult(string.Format(CoreMessages.OpenApi.UnknownProperty, propertyName));
                }

                if (string.Compare(itemValue, forbiddenValue, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    return new ValidationResult(string.Format(CoreMessages.OpenApi.EnumValueIsNotAllowed, forbiddenValue, propertyName, null));
                }
            }

            return ValidationResult.Success;
        }
    }
}
