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
    /// Validates that the list contains an item with value matching to required property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ListRequiredIfProperty : ValidationAttribute
    {
        private string propertyName;
        private string linkedPropertyName;
        private Type linkedPropertyType;

        public ListRequiredIfProperty(string propertyName, string linkedPropertyName, Type linkedPropertyType = null) : base(CoreMessages.OpenApi.ListRequiredIfProperty)
        {
            this.propertyName = propertyName;
            this.linkedPropertyName = linkedPropertyName;
            this.linkedPropertyType = linkedPropertyType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }
            var instance = validationContext.ObjectInstance;
            var list = value as IList;
            if (list == null && value != null)
            {
                throw new InvalidOperationException("Attribute used on something that doesn't implement IList should throw an exception.");
            }
            var properyWithValueExists = false;
            var type = instance.GetType();

            var linkedProperty = linkedPropertyType == null ? type.GetProperty(linkedPropertyName) : type.GetProperty(linkedPropertyName, linkedPropertyType);
            var linkedPropertyValue = linkedProperty.GetValue(instance, null);

            if (linkedPropertyValue == null)
            {
                return null;
            }
            else
            {
                this.ErrorMessage = string.Format(CoreMessages.OpenApi.ListRequiredIfProperty, propertyName, linkedPropertyValue, linkedPropertyName, linkedPropertyValue);
                if (list == null)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            foreach (var item in list)
            {
                if (item == null)
                {
                    return new ValidationResult(ErrorMessage);
                }
                var property = item.GetType().GetProperty(propertyName);
                if (property == null)
                {
                    throw new InvalidOperationException(string.Format("The attribute is misconfigured with wrong item property name (typo in attribute: '{0}')", propertyName));
                }
                var itemValue = property.GetValue(item, null);
                if (itemValue != null && itemValue.ToString() == linkedPropertyValue.ToString())
                {
                    properyWithValueExists = true;
                }
            }
            return properyWithValueExists ? null : new ValidationResult(ErrorMessage);
        }
    }
}
