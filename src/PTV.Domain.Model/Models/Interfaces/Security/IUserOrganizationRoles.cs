using System;
using PTV.Domain.Model.Enums.Security;

namespace PTV.Domain.Model.Models.Interfaces.Security
{
    /// <summary>
    /// Interface to link user role and organization
    /// </summary>
    public interface IUserOrganizationRoles
    {
        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>
        /// The organization identifier.
        /// </value>
        Guid OrganizationId { get; set; }
        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        UserRoleEnum Role { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is main.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is main; otherwise, <c>false</c>.
        /// </value>
        bool IsMain { get; set; }
    }
}