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
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PTV.Domain.Model.Enums.Security;
using PTV.IdentityUserManager.Extensions;

namespace PTV.IdentityUserManager
{
    /// <summary>
    /// Constants used across the library
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Password minimum length.
        /// </summary>
        public const int PasswordMinLength = 11;
        /// <summary>
        /// Password maximum length.
        /// </summary>
        public const int PasswordMaxLength = 100;
        /// <summary>
        /// The UI english format error message for password complexity.
        /// </summary>
        public const string PasswordUIErrorFormatMessage = "The {0} must be between {2} and {1} characters long, must contain special character and number, must contain upper and lower case letters.";

        public const string PrimaryAuthenticationType = "cookies";
        public const string EmptyString = "";
        public static class ErrorMessages
        {
            public static class Cryptography
            {
                public static readonly string CertificateNotFound = "Could not find a single signing certificate matching the thumbprint.";
            }
        }

        public static class RegexPatterns
        {
            public static readonly string NumbersAndChars = @"[^\da-zA-z]";
        }

        public static class BuiltInEeva
        {
            public static readonly string UserName = @"EevaAdmin@ptv.com";
        }
    }

    public static class UserExtension
    {
        public static bool HasRole(this ClaimsPrincipal claimsPrincipal, UserRoleEnum role)
        {
            return claimsPrincipal.Claims.GetRoleClaim()?.Value == role.ToString();
        }
    }
}
