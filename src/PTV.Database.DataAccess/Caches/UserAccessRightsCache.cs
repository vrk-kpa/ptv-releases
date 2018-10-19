using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Translators.UserAccessRightsGroup;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;
using PTV.Framework.Enums;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(IUserAccessRightsCache), RegisterType.Singleton)]
    public class UserAccessRightsCache : IUserAccessRightsCache
    {
        private Dictionary<string, VmUserAccessRights> store = new Dictionary<string, VmUserAccessRights>();

        public UserAccessRightsCache()
        {}

        public void Init(Dictionary<string, VmUserAccessRights> data)
        {
            store = data;
        }

        public VmUserAccessRights Get(string userGroup)
        {
            return store.TryGetOrDefault(userGroup.ToLower(), GetDefault());
        }

        public VmUserAccessRights Get(Guid entityId)
        {
            return store.Values.FirstOrDefault(i => i.EntityId == entityId) ?? GetDefault();
        }

        private VmUserAccessRights GetDefault()
        {
            return new VmUserAccessRights() {UserRole = UserRoleEnum.Shirley, GroupCode = "none", AccessRights = 0};
        }
    }
}
