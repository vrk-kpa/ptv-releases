using System;
using System.Collections.Generic;
using System.Text;
using PTV.Database.Model.Models.Attributes;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.Model.Models
{
        internal class UserOrganization : EntityIdentifierBase
        {
            [NotForeignKey]
            public Guid UserId { get; set; }
    
            public Guid OrganizationId { get; set; }
    
            public Organization Organization { get; set; }
    
            [NotForeignKey]
            public Guid RoleId { get; set; }
    
            public bool IsMain { get; set; }
        }
}
