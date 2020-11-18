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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;
using PTV.Framework.Attributes;

namespace PTV.IdentityUserManager.Models
{
    /// <summary>
    /// View model for register view
    /// </summary>
    public class RegisterViewModel
    {
        // Resources for display names should be used, not fully working in NET 5

        [Required]
        [Display(Name = "Surname")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string SurName { get; set; }
        [Required]
        [Display(Name = "Given Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string GivenName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(Constants.PasswordMaxLength, ErrorMessage = Constants.PasswordUIErrorFormatMessage, MinimumLength = Constants.PasswordMinLength)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        //[AllowedValues(allowedValues: new string[] { UserRoleEnum.Eeva.ToString(), UserRoleEnum.Pete.ToString(), UserRoleEnum.Shirley.ToString() })]
        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        public IReadOnlyCollection<SelectListItem> Organizations { get; set; }
        public string Organization { get; set; }
    }
}
