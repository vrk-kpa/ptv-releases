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
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Converters;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Framework.Converters;
using PTV.Framework.ServiceManager;

namespace PTV.Domain.Model.Models.Security
{
    /// <summary>
    /// Base model for user info, contains information about user organization
    /// </summary>
    /// <seealso cref="PTV.Framework.ServiceManager.VmBase" />
    public class VmUserInfoBase : VmBase
    {
        /// <summary>
        /// User organization id
        /// </summary>
        public Guid? UserOrganization { get; set; }
    }

    /// <summary>
    /// Contains ionformation about user (name, access rights, owner organizations)
    /// </summary>
    public class VmUserInfo : VmUserInfoBase
    {
        /// <summary>
        /// User name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User role <see cref="UserRoleEnum"/>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public UserRoleEnum Role { get; set; }

        /// <summary>
        /// User permisions
        /// </summary>
        public Dictionary<UserRoleEnum, Dictionary<string, VmPermision>> Permisions { get; set; }

        /// <summary>
        /// Indicates if your has at least rights for reading data
        /// </summary>
        public bool NoAccess { get; set; }

        /// <summary>
        /// Gets or sets the user organizations.
        /// </summary>
        /// <value>
        /// The user organizations.
        /// </value>
        public List<IUserOrganizationRoles> UserOrganizations { get; set; }
    }

    /// <summary>
    /// Configuration of roles. Loaded from json file
    /// </summary>
    public class VmRoleInfo : VmBase
    {

        /// <summary>
        /// True if user (role) can manage all organizations
        /// </summary>
        public bool AllowedAllOrganizations { get; set; }

        /// <summary>
        /// Access permisions
        /// </summary>
        public Dictionary<string, VmPermision> Permisions { get; set; }
    }

    /// <summary>
    /// Access permisions
    /// </summary>
    public class VmPermision
    {
        /// <summary>
        /// Code for domain for permision (organization, service...)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Permisions for all organizations
        /// </summary>
        [JsonConverter(typeof(PermisionEnumConverter))]
        public PermisionEnum RulesAll { get; set; }

        /// <summary>
        /// Permisions for own organizations
        /// </summary>
        [JsonConverter(typeof(PermisionEnumConverter))]
        public PermisionEnum RulesOwn { get; set; }
    }
}
