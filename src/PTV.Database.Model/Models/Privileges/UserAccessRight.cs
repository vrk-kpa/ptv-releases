using System;
using PTV.Database.Model.Models.Attributes;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.Model.Models.Privileges
{
    internal class UserAccessRight : EntityBase
    {
        [NotForeignKey]
        public Guid UserId { get; set; }
        public Guid AccessRightId { get; set; }
        public AccessRightType AccessRight { get; set; }
    }
}