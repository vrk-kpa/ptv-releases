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
using System.Linq;
using System.Reflection;

namespace PTV.Framework.Attributes
{
    /// <summary>
    /// The item is required if a given property has desired value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        private string propertyName;
        private object desiredValue;
        private Type entityType;
        private RequiredAttribute reqAttribute;
        public RequiredIfAttribute(string propertyName, object desiredValue, Type entityType = null) : base(string.Format(CoreMessages.OpenApi.RequiredIf, propertyName, desiredValue == null ? "null" : desiredValue))
        {
            this.propertyName = propertyName;
            this.desiredValue = desiredValue;
            this.entityType = entityType;
            reqAttribute = new RequiredAttribute { AllowEmptyStrings = false};
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (PropertyHasDesiredValue(validationContext))
            {
                if (!IsValidRequired(value, validationContext))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }

        protected bool PropertyHasDesiredValue(ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var type = instance.GetType();
            var property = entityType == null ? type.GetProperty(propertyName) : type.GetProperties().Single(p =>
                p.Name == propertyName && p.PropertyType == entityType);
            if (property == null)
            {
                throw new InvalidOperationException(string.Format("Property '{0}' not found.", propertyName));
            }
            var propertyValue = property.GetValue(instance, null);
            if (propertyValue == null || (propertyValue is string && string.IsNullOrWhiteSpace(propertyValue.ToString())))
            {
                if (desiredValue == null || (desiredValue is string && string.IsNullOrWhiteSpace(desiredValue.ToString())))
                {
                    return true;
                }
                return false;
            }
            if (propertyValue is IList)
            {
                var list = propertyValue as IList;
                if (desiredValue == null)
                {
                    if (list.Count == 0)
                    {
                        return true;
                    }
                    return false;
                }
                foreach (var item in list) // Let's loop through list and check if it contains the desired value
                {
                    if (item != null && item.ToString() == desiredValue.ToString())
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (desiredValue == null && propertyValue != null)
                {
                    return false;
                }
                if (propertyValue.ToString() == desiredValue.ToString())
                {
                    return true;
                }
            }
            return false;
        }

        protected bool IsValidRequired(object value, ValidationContext validationContext)
        {
            if (!reqAttribute.IsValid(value))
            {
                return false;
            }
            return true;
        }
    }
}
