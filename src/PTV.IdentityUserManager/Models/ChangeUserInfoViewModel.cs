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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PTV.IdentityUserManager.Models
{
    public class ChangeUserInfoViewModel
    {
        public IReadOnlyCollection<SelectListItem> Users { get; set; }
        [Required]
        public string UserId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Full Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string FullName { get; set; }

        public Dictionary<Guid, Guid> OrganizationsAuthenticatedUser { get; set; }
        public Dictionary<Guid, Guid> OrganizationsChangedUser { get; set; }

        public Dictionary<string, Guid> RoleIds { get; set; }

        public IReadOnlyCollection<SelectListItem> OrganizationsToList { get; set; }

        public Dictionary<Guid, Guid> Organizations { get; set; }

        public bool IsEevaRole { get; set; }
        public bool IsPeteRole { get; set; }


        public ChangeUserInfoViewModel()
        {
            OrganizationsAuthenticatedUser = new Dictionary<Guid, Guid>();
            OrganizationsChangedUser = new Dictionary<Guid, Guid>();
            Organizations = new Dictionary<Guid, Guid>();
            OrganizationsToList = new List<SelectListItem>();
        }
    }
}
