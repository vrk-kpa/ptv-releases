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
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PTV.Framework.Attributes.ValidationAttributes
{
    public class ValidOpenApiEnumAttribute : ValidationAttribute
    {
        private Type enumType;
        private string memberName = string.Empty;

        public ValidOpenApiEnumAttribute(Type enumType)
        {
            this.enumType = enumType;
        }

        public override bool RequiresValidationContext { get { return true; } }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext != null)
            {
                memberName = validationContext.MemberName;
            }

            if (value == null)
            {
                return ValidationResult.Success;
            }
            try
            {
                var enumValue = value.ToString().GetEnumValueByOpenApiEnumValue(enumType);
                if (enumValue == null)
                {
                    return ReturnError();
                }
            }
            catch(KeyNotFoundException)
            {
                return ReturnError();
            }
            //if (value.ToString().GetOpenApiEnumValue<enumType>() == null)

            return ValidationResult.Success;
        }

        public override bool IsValid(object value)
        {
            return ValidationResult.Success == IsValid(value, null);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CoreMessages.OpenApi.RequestMalFormatted, GetOpenApiEnumvaluesAsList());
        }

        private string GetOpenApiEnumvaluesAsList()
        {
            var openApiEnumValueList = enumType.ToList();
            if (openApiEnumValueList?.Count > 0)
            {
                return string.Join(", ", openApiEnumValueList.Select(i => i.Value).ToList());
            }

            return null;
        }

        private ValidationResult ReturnError()
        {
            return new ValidationResult(string.Format(CoreMessages.OpenApi.RequestMalFormatted, GetOpenApiEnumvaluesAsList()), new List<string> { memberName });
        }
    }
}
