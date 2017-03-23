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
using System.ComponentModel.DataAnnotations;

namespace PTV.Framework.Attributes
{
    public class ValidEnumAttribute : DataTypeAttribute
    {
        private Type enumType;
        private EnumDataTypeAttribute enumValidator;

        public ValidEnumAttribute(Type enumType) : base("Enumeration")
        {
            this.enumType = enumType;
            this.enumValidator = new EnumDataTypeAttribute(enumType);
            this.ErrorMessage = CoreMessages.OpenApi.RequestMalFormatted;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!enumValidator.IsValid(value))
            {
                return new ValidationResult(string.Format(CoreMessages.OpenApi.RequestMalFormatted, GetEnumvaluesAsList()));
            }

            return ValidationResult.Success;
        }

        public override bool IsValid(object value)
        {
            return enumValidator.IsValid(value);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CoreMessages.OpenApi.RequestMalFormatted, GetEnumvaluesAsList());
        }

        private string GetEnumvaluesAsList()
        {
            return string.Join(", ", Enum.GetNames(enumType));
        }
    }
}
