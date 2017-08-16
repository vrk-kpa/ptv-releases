using System;

namespace PTV.Database.DataAccess.Interfaces
{
    public interface IUserOrganizationRoleDefinition
    {
        Guid UserId { get; set; }
        Guid OrganizationId { get; set; }
        Guid RoleId { get; set; }

        bool IsMain { get; set; }
    }
}