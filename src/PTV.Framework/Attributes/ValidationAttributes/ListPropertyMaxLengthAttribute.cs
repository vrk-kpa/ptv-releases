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
using System.Reflection;

namespace PTV.Framework.Attributes
{
    /// <summary>
    /// Specifies the maximum length of string data for certain property in an item list.
    /// If 'typeValue' parameter is set the Type property of the list item needs to match with the given one.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ListPropertyMaxLengthAttribute : ValidationAttribute
    {
        private readonly int length;
        private readonly string propertyName;
        private readonly string typeValue;
        private readonly MaxLengthAttribute maxLengthAttribute;

        public string TypeValue => typeValue;
        public int Length => length;

        public ListPropertyMaxLengthAttribute(int length, string propertyName, string typeValue = "")
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("propertyName must not be null or empty");
            }
            maxLengthAttribute = new MaxLengthAttribute(length);
            this.length = length;
            this.propertyName = propertyName;
            this.typeValue = typeValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (length <= 0)
            {
                throw new InvalidOperationException("ListPropertyMaxLengthAttribute must have a length value that is greater than zero.");
            }

            if (value == null)
            {
                return ValidationResult.Success;
            }

            var list = value as IList;
            if (list == null)
            {
                throw new InvalidOperationException("Attribute used on a type that doesn't implement IList.");
            }

            foreach (var item in list)
            {
                if (item == null)
                {
                    continue;
                }
                // Check that we have the right item with Type property defined by typeValue
                if (!string.IsNullOrEmpty(typeValue))
                {
                    var typeProperty = item.GetType().GetProperty("Type");
                    if (typeProperty == null || typeProperty.GetValue(item, null)?.ToString() != typeValue)
                    {
                        continue; // the Type is not right so jump to the next item
                    }
                }

                // Get the property
                var property = item.GetType().GetProperty(propertyName);
                if (property == null)
                {
                    return new ValidationResult(string.Format(CoreMessages.OpenApi.UnknownProperty, propertyName));
                }

                // Get the property value
                var propertyValue = property.GetValue(item, null);
                if (propertyValue == null)
                {
                    continue;
                }
                if (!maxLengthAttribute.IsValid(propertyValue.ToString()))
                {
                    var formattedName = $"'{propertyName}'";
                    formattedName += string.IsNullOrEmpty(typeValue) ? "" : $" for '{typeValue}'";
                    return new ValidationResult($"Maximum length of property {formattedName} must be '{length}'.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
