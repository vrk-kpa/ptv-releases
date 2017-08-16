using System;
using PTV.Domain.Model.Enums.Security;

namespace PTV.Database.DataAccess.Interfaces
{
    public interface IUserOrganizationRoles
    {
        Guid OrganizationId { get; set; }
        UserRoleEnum Role { get; set; }
        bool IsMain { get; set; }
    }
}