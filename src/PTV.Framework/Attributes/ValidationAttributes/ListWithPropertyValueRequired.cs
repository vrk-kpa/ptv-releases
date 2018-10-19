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
using System.Reflection;

namespace PTV.Framework.Attributes
{
    /// <summary>
    /// Validates that the list contains an item with given property with the given property value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ListWithPropertyValueRequired : ValidationAttribute
    {
        private string propertyName;
        private string propertyValue;
        public ListWithPropertyValueRequired(string propertyName, string propertyValue) : base(string.Format(CoreMessages.OpenApi.RequiredValueNotFound, propertyName, propertyValue))
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("Property name cannot be null, empty string or whitespace(s).");
            }
            this.propertyName = propertyName;
            this.propertyValue = propertyValue;
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
                throw new InvalidOperationException($"The validation attribute is used on a property that doesn't implement IList (property name: '{validationContext.MemberName}').");
            }

            var properyWithValueExists = false;
            foreach (var item in list)
            {
                if (item == null)
                {
                    return ReturnError(validationContext);
                }
                var property = item.GetType().GetProperty(propertyName);
                if (property == null)
                {
                    return ReturnError(validationContext);
                }
                var itemValue = property.GetValue(item, null);
                if (itemValue!=null && itemValue.ToString() == propertyValue)
                {
                    properyWithValueExists = true;
                }
            }
            return properyWithValueExists ? null : ReturnError(validationContext);
        }

        private ValidationResult ReturnError(ValidationContext validationContext)
        {
            return new ValidationResult(ErrorMessage, new List<string> { validationContext.MemberName });
        }
    }
}
