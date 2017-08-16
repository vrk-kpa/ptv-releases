using System;
using PTV.Database.DataAccess.Interfaces;
using PTV.Domain.Model.Enums.Security;

namespace PTV.Database.DataAccess.Services.Security
{
    public class UserOrganizationRoleDefinition : IUserOrganizationRoleDefinition
    {
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid RoleId { get; set; }
        public bool IsMain { get; set; }
    }

    public class UserOrganizationRoles : IUserOrganizationRoles
    {
        public Guid OrganizationId { get; set; }
        public UserRoleEnum Role { get; set; }
        public bool IsMain { get; set; }
    }
}