using System;
using System.Collections.Generic;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Interfaces.Next
{
    public interface IOrganizationQueries
    {
        List<OrganizationModel> Get(List<Guid> ids);
        UserOrganizationsAndRolesModel GetAllUserOrganizationsAndRoles();
        List<OrganizationModel> Search(OrganizationSearchModel parameters);
    }
}