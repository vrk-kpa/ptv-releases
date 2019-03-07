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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models.Security
{
    /// <summary>
    ///c
    /// </summary>
    public class UserOrganizationRoleDefinition : IUserOrganizationRoleDefinition
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>
        /// The organization identifier.
        /// </value>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        /// <value>
        /// The role identifier.
        /// </value>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is main.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is main; otherwise, <c>false</c>.
        /// </value>
        public bool IsMain { get; set; }
    }

    /// <summary>
    /// Model for connection organization and user role
    /// </summary>
    public class UserOrganizationRoles : IUserOrganizationRoles
    {
        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>
        /// The organization identifier.
        /// </value>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public UserRoleEnum Role { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is main.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is main; otherwise, <c>false</c>.
        /// </value>
        public bool IsMain { get; set; }
    }

    /// <summary>
    /// Model for user organization and roles information
    /// </summary>
    public class UserOrganizationsAndRolesResult : IVmBase
    {
        /// <summary>
        /// Returns roles for each user organization
        /// </summary>
        [JsonProperty]
        public IReadOnlyList<IUserOrganizationRoles> OrganizationRoles { get; set; }
        /// <summary>
        /// Returns list of user's organizations 
        /// </summary>
        [JsonProperty]
        public IReadOnlyList<VmListItem> UserOrganizations { get; set; }
        /// <summary>
        /// Returns true if any user organization has <see cref="OrganizationTypeEnum.Region"/> 
        /// </summary>
        [JsonProperty]
        public bool IsRegionUserOrganization { get; set; }
    }

//    /// <summary>
//    /// Model for allowed values (restriction filters)
//    /// </summary>
//    public class VmAllowedValue
//    {
//        /// <summary>
//        /// Entity type value
//        /// </summary>
//        public string Value { get; set; }
//    }
}
