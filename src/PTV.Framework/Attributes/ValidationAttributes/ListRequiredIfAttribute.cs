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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Framework.Attributes
{
    /// <summary>
    /// The list is required if a given property has desired value.
    /// </summary>
    public class ListRequiredIfAttribute : RequiredIfAttribute
    {
        private string memberName = null;

        public ListRequiredIfAttribute(string propertyName, object desiredValue) : base(propertyName, desiredValue)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("Value cannot be empty. Parameter name: propertyName");
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new InvalidOperationException("validationContext cannot be null.");
            }

            if (value != null && !(value is IList))
            {
                throw new InvalidOperationException("Attribute used on a property that doesn't implement IList.");
            }

            memberName = validationContext.MemberName;

            if (PropertyHasDesiredValue(validationContext))
            {
                if (value == null)
                {
                    return ReturnError();// new ValidationResult(ErrorMessage);
                }

                var list = (IList) value;

                if (list == null || list.Count < 1)
                {
                    return ReturnError();// new ValidationResult(ErrorMessage);
                }

                foreach(var item in list)
                {
                    if (!IsValidRequired(item, validationContext))
                    {
                        return ReturnError();//new ValidationResult(ErrorMessage);
                    }
                }
            }

            return ValidationResult.Success;
        }

        private ValidationResult ReturnError()
        {
            return new ValidationResult(ErrorMessage, new List<string> { memberName });
        }
    }
}
