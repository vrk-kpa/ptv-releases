using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class UserOrganizationsAndRolesModel
    {
        public List<OrganizationModel> UserOrganizations { get; set; } = new List<OrganizationModel>();
        public List<OrganizationRoleModel> OrganizationRoles { get; set; } = new List<OrganizationRoleModel>();
    }
}
