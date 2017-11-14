using System;

namespace PTV.Database.DataAccess.Interfaces
{
    public class OrganizationRole
    {
        public Guid OrganizationId { get; set; }
        public Guid RoleId { get; set; }
    }
}