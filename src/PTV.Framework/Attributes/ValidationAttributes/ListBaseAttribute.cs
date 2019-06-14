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
    /// Base class for validating list of objects (property decorated with this attribute must implement IList). The given property is validated using the validator given in constructor.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Null values in list are not allowed.
    /// </para>
    /// </remarks>
    public class ListBaseAttribute : ValidationAttribute
    {
        // Comment: Disqussed with original creator and some assumptions are for the validator but not checked in code
        // list 'always' contains ptv objects not for example strings or ints even if the code is prepared for that kind of functionality
        // list items are of same type

        private readonly string propertyName;
        private readonly ValidationAttribute validator;

        private string strValue;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="validator">validator to be used</param>
        /// <param name="propertyName">null/empty string to validate the list item (for example List{string}) or propertyname of the list item to validate (for example list contains custom objects which certain property value needs to be validated)</param>
        /// <exception cref="System.ArgumentNullException"><i>validator</i> is null</exception>
        public ListBaseAttribute(ValidationAttribute validator, string propertyName = null)
        {
            this.propertyName = propertyName;
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator),"Validator cannot be null.");
        }

        public override string FormatErrorMessage(string name)
        {
            return validator.FormatErrorMessage(name);
        }

        /// <summary>
        /// Validates list items.
        /// </summary>
        /// <param name="value">object that implements IList</param>
        /// <param name="validationContext">validationContext</param>
        /// <returns>ValidationResult</returns>
        /// <exception cref="System.InvalidOperationException"><paramref name="value"/> doesn't implement IList</exception>
        /// <exception cref="System.ArgumentException"><paramref name="value"/> list contains an item that doesn't have the named property (defined in the constructor)</exception>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                // nothing to validate, not checking the property type where attribute is used on
                return ValidationResult.Success;
            }

            var list = value as IList;

            if (list == null)
            {
                string propName = validationContext == null ? string.Empty : validationContext.DisplayName;

                // this attribute expects the attribute to be used on properties that implement IList
                throw new InvalidOperationException($"The validation attribute is used on a property that doesn't implement IList (property name: '{propName}').");
            }

            foreach (var item in list)
            {
                // if propertyname is defined but the list item is null we cannot continue as we can't get the property value
                // and pass that to the defined validator (we could have (read: add) a property that allows null objects and skips the validator)
                if (item == null && !string.IsNullOrWhiteSpace(propertyName))
                {
                    return ReturnError("List contains a null object and propertyName is defined. Cannot get property value from null object.", validationContext);
                }

                // if list item is a custom class then the propertyname has to be defined to get the property containing the actual value
                if (string.IsNullOrWhiteSpace(propertyName) && item != null && !(item.GetType().Namespace.StartsWith("System")))
                {
                    throw new ArgumentNullException(nameof(validator), "PropertyName cannot be null when list contains custom objects.");
                }

                // will throw an argumentexception if named property is not found from the item
                var itemValue = item?.GetItemValue(propertyName);

                strValue = itemValue?.ToString();

                if (!validator.IsValid(itemValue))
                {
                    var name = string.IsNullOrEmpty(propertyName) ? (validationContext != null ? validationContext.DisplayName : "") 
                        : propertyName;
                    return ReturnError(FormatErrorMessage(name), validationContext);
                }
            }

            return ValidationResult.Success;
        }

        protected string Value { get { return strValue; } private set { } }

        private ValidationResult ReturnError(string msg, ValidationContext validationContext)
        {
            if (validationContext == null) return new ValidationResult(msg);

            return new ValidationResult(msg, new List<string> { validationContext.MemberName });
        }
    }
}
