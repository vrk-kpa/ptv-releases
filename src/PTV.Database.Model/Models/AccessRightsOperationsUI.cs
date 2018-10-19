using System;

namespace PTV.Database.Model.Models
{
    internal partial class AccessRightsOperationsUI
    {
        public Guid Id { get; set; }
        public Guid? OrganizationId { get; set; }
        public bool AllowedAllOrganizations { get; set; }
        public string Role { get; set; }
        public string Permission { get; set; }
        public long RulesAll { get; set; }
        public long RulesOwn { get; set; }

        public Organization Organization { get; set; }
    }
}