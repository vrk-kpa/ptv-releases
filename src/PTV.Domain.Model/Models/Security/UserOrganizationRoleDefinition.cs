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
        /// 
        /// </summary>
        [JsonProperty]
        public List<Guid> AllowedValues { get; set; }

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