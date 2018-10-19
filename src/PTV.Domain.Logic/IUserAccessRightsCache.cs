using System;
using System.Collections.Generic;
using PTV.Framework;

namespace PTV.Domain.Logic
{
    public interface IUserAccessRightsCache
    {
        VmUserAccessRights Get(string userGroup);
        VmUserAccessRights Get(Guid entityId);
        void Init(Dictionary<string, VmUserAccessRights> data);
    }
}