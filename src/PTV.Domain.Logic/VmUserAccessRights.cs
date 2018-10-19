using System;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework.Enums;

namespace PTV.Domain.Logic
{
    public class VmUserAccessRights
    {
        public Guid EntityId { get; set; }
        public string GroupCode { get; set; }
        public UserRoleEnum UserRole { get; set; }
        public AccessRightEnum AccessRights { get; set; }
    }
}