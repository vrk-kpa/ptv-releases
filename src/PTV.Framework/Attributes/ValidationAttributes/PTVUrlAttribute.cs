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
using System.Net;
using System.Text.RegularExpressions;

namespace PTV.Framework.Attributes.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PTVUrlAttribute : ValidationAttribute
    {
        private UrlAttribute urlAttribute;

        public PTVUrlAttribute(): base()
        {
            urlAttribute = new UrlAttribute();
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            if (!urlAttribute.IsValid(value))
            {
                return false;
            }

            // In UI react validator is used - works differently than .net UrlValidator.
            // Let's add some more validation for IN api from react validator (see under node_modules/validator) (PTV-4296)
            var urlParts = (value as string).Split('#');
            var url = urlParts[0];
            urlParts = url.Split('?');
            url = urlParts[0];
            urlParts = url.Split("://");
            url = urlParts[1];
            urlParts = url.Split('/');
            url = urlParts[0];
            urlParts = url.Split('@');
            var host = urlParts[0];

            var ipv6Regex = new Regex(@"^\[([^\]]+)\](?::([0-9]+))?$");
            if (ipv6Regex.IsMatch(host))
            {
                return true;
            }

            // Check possible port number
            urlParts = host.Split(':');
            host = urlParts[0];
            if (urlParts.Length > 1)
            {
                var port = urlParts[1];
                int portNumber;
                if (!int.TryParse(port, out portNumber))
                {
                    return false;
                }
            }

            // Check if host is ip address
            IPAddress ipAddress;
            if (host.Split('.').Length == 4 && IPAddress.TryParse(host, out ipAddress))
            {
                return true;
            }
            if (host == "localhost")
            {
                return true;
            }


            if (!host.IsValidFQDN())
            {
                return false;
            }

            return true;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!IsValid(value))
            {
                var name = string.Empty;
                if (validationContext != null)
                {
                    name = validationContext.DisplayName;
                }
                return new ValidationResult(string.Format(urlAttribute.ErrorMessage, name));
            }

            return ValidationResult.Success;
        }
    }
}
