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
using System.Text.RegularExpressions;

namespace PTV.Framework.Attributes.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PTVEmailAddressAttribute : ValidationAttribute
    {
        private EmailAddressAttribute emailAttribute;

        public PTVEmailAddressAttribute(): base()
        {
            emailAttribute = new EmailAddressAttribute();
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            if(!emailAttribute.IsValid(value))
            {
                return false;
            }

            // In UI react validator is used - works differently than .net EmailAddressValidator. 
            // Let's add some more validation for IN api from react validator (see under node_modules/validator) (PTV-4296)
            var parts = (value as string).Split('@');
            var userPart = parts[0];
            if (!IsValidPart(userPart, @"^[a-zA-Z\d!#\$%&'\*\+\-\/=\?\^_`{\|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+$"))
            {
                return false;
            }

            var domainPart = parts[1];
            if (!domainPart.IsValidFQDN())
            {
                return false;
            }
            return true;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {            
            if (!IsValid(value))
            {
                return new ValidationResult(emailAttribute.ErrorMessage);
            }

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(emailAttribute.ErrorMessage, name);
        }

        private bool IsValidPart(string part, string pattern)
        {
            var parts = part.Split('.');
            
            var partRegEx = new Regex(pattern);
            for (int i = 0; i < parts.Length; i++)
            {
                if (!partRegEx.Match(parts[i]).Success)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
