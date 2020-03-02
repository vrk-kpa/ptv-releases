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
using System.ComponentModel.DataAnnotations;
using PTV.Framework.ServiceManager;

namespace PTV.Framework.Attributes
{
    /// <summary>
    /// Property must be empty or have at least five characters (leading and trailing white space omitted).
    /// </summary>
    public class ValueNotEmptyAttribute : ValidationAttribute
    {
        public ValueNotEmptyAttribute() : base(CoreMessages.OpenApi.ValueNotEmpty)
        {
            ErrorMessage = CoreMessages.OpenApi.ValueNotEmpty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return null;

            var valueToValidate = value as string;
            if (valueToValidate == null)
            {
                throw new PtvArgumentException($"Expected value is string! Value {value} is not valid.");
            }

            if (string.IsNullOrWhiteSpace(valueToValidate))
            {
                return null;
            }

            if (valueToValidate.Trim().Length <= 4)
            {
                // not a valid string with at least 5 characters with no leading or trailing white space
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
