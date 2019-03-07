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
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Framework.Attributes
{
    /// <summary>
    /// Validation attribute to indicate that a list is required.
    /// </summary>
    public class ListRequiredAttribute : ValidationAttribute
    {
        public bool AllowEmptyStrings { get; set; }

        public ListRequiredAttribute() : base()
        {
            
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var attr = new RequiredAttribute
            {
                AllowEmptyStrings = AllowEmptyStrings
            };

            if (value == null)
            {
                return ReturnError(validationContext);
            }

            var list = value as IList;
            if (list == null)
            {
                throw new InvalidOperationException(String.Format("ListRequiredAttribute is used on property:'{0}' of type:'{1}' instead of required type: 'IList'", validationContext.MemberName, value.GetType().ToString()));
            }

            if (list.Count < 1)
            {
                return ReturnError(validationContext);
            }

            foreach (var item in list)
            {
                if (!attr.IsValid(item))
                {
                    return ReturnError(validationContext, "item");
                }
            }
            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("The {0} field is required.", name);
        }

        private ValidationResult ReturnError(ValidationContext validationContext, string name = null)
        {
            if (validationContext == null)
            {
                return new ValidationResult(FormatErrorMessage(name ?? ""));
            }
            return new ValidationResult(FormatErrorMessage(name ?? validationContext.DisplayName), new List<string> { validationContext.MemberName });
        }
    }
}
