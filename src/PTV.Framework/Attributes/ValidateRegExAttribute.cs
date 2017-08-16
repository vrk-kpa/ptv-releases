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
using Microsoft.AspNetCore.Mvc.Filters;
using PTV.Framework.ServiceManager;

namespace PTV.Framework.Attributes
{
    /// <summary>
    /// Validation attribute to indicate that a given parameter is valid Finnish Y code.
    /// </summary>
    public class ValidateRegExAttribute : ValidationBaseAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateRegExAttribute"/> class.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="pattern">The pattern.</param>
        public ValidateRegExAttribute(string keyName, string pattern) : base(keyName, new RegularExpressionAttribute(pattern))
        {
            if (string.IsNullOrEmpty(keyName) || string.IsNullOrWhiteSpace(keyName))
            {
                throw new PtvArgumentException("keyName cannot be null, empty string or whitespaces.");
            }

            if (string.IsNullOrEmpty(pattern) || string.IsNullOrWhiteSpace(pattern))
            {
                throw new PtvArgumentException("pattern cannot be null, empty string or not valid.");
            }

            try
            {
                var regex = new Regex(pattern);
            }
            catch (ArgumentException)
            {
                throw new PtvArgumentException("pattern cannot be null, empty string or not valid.");
            }
        }
    }
}
