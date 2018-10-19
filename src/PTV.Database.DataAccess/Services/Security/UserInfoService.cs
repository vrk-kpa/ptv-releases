/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;
using PTV.Framework.Enums;

namespace PTV.Database.DataAccess.Services.Security
{
    [RegisterService(typeof(IUserInfoService), RegisterType.Transient)]
    internal class UserInfoService : IUserInfoService
    {
        private readonly UserRolesCache userRolesCache;
        private readonly IPahaTokenProcessor pahaTokenProcessor;

        public UserInfoService(UserRolesCache userRolesCache,IPahaTokenProcessor pahaTokenProcessor)
        {

            this.userRolesCache = userRolesCache;
            this.pahaTokenProcessor = pahaTokenProcessor;
        }

        private IPahaTokenProcessor GetToken(string token = "")
        {
            if (!string.IsNullOrEmpty(token))
            {
                pahaTokenProcessor.ProcessToken(token);
            }
            return pahaTokenProcessor.UserPresent ? pahaTokenProcessor : null;
        }

        private VmRoleInfo GetRoleInfo(UserRoleEnum role, Guid orgId)
        {
            return userRolesCache.GetRole(role.ToString(),orgId) ?? new VmRoleInfo();
        }

        private Dictionary<string, VmPermision> GetPermisions(VmRoleInfo roleInfo, bool readOnly)
        {
            var permisions = roleInfo?.Permisions ?? new Dictionary<string, VmPermision>();
            if (readOnly)
            {
                return permisions.Select(x =>
                    new KeyValuePair<string, VmPermision>(
                        x.Key,
                        new VmPermision
                        {
                            Code = x.Value.Code,
                            RulesAll = x.Value.RulesAll & PermisionEnum.Read,
                            RulesOwn = x.Value.RulesOwn & PermisionEnum.Read
                        }
                    )
                ).ToDictionary(x => x.Key, x => x.Value);
            }

            return permisions;
        }

        public IDictionary<string, VmPermision> GetPermissions()
        {
            var token = GetToken();
            return token == null ? new Dictionary<string, VmPermision>() : GetPermisions(GetRoleInfo(token.UserRole, token.ActiveOrganization), false);
        }

        public VmUserInfo GetUserInfo(string token = "")
        {
            var pahaToken = GetToken(token);

            if (pahaToken == null)
            {
                return null;
            }
            
            UserRoleEnum role = pahaToken.UserRole;
            return new VmUserInfo
            {
                Name = pahaToken.FirstName,
                Surname = pahaToken.Surname,
                Email = pahaToken.UserName,
                Role = role,
                Permisions = GetPermisions(GetRoleInfo(role,pahaToken.ActiveOrganization), !pahaToken.UserAccessRights.HasFlag(AccessRightEnum.UiAppWrite)),
                UserOrganization = pahaToken.ActiveOrganization,
                HasAccess = pahaToken.UserAccessRights.HasFlag(AccessRightEnum.UiAppRead)
            };
        }
    }

    [RegisterService(typeof(UserRolesCache), RegisterType.Singleton)]
    internal class UserRolesCache
    {
        private Dictionary<Guid, Dictionary<string, VmRoleInfo>> rolesPerOrg;

        public UserRolesCache(IContextManager contextManager) 
        {
            contextManager.ExecuteReader(unitOfwork =>
            {
                var rep = unitOfwork.CreateRepository<IAccessRightsOperationsUIRepository>();
                var allAccessSettings = rep.All().ToList();
                rolesPerOrg = allAccessSettings.GroupBy(i => i.OrganizationId).ToDictionary(i => i.Key ?? Guid.Empty, i => i.GroupBy(j => j.Role).ToDictionary(j => j.Key, j => new VmRoleInfo()
                {
                    Permisions = j.Select(h => new VmPermision() {Code = h.Permission, RulesAll = (PermisionEnum) h.RulesAll, RulesOwn = (PermisionEnum) h.RulesOwn}).ToDictionary(n => n.Code, n => n),
                    OrganizationId = j.FirstOrDefault()?.OrganizationId,
                    AllowedAllOrganizations = j.FirstOrDefault()?.AllowedAllOrganizations ?? false
                }));
            });
        }

        public VmRoleInfo GetRole(string roleName, Guid organizationId)
        {
            var globalRoles = rolesPerOrg.TryGetOrDefault(Guid.Empty, null)?.TryGet(roleName.ToLower());
            var forOrgRoles = rolesPerOrg.TryGetOrDefault(organizationId, null)?.TryGet(roleName.ToLower());
            if (forOrgRoles != null && globalRoles != null)
            {
                return new VmRoleInfo(){ AllowedAllOrganizations = forOrgRoles.AllowedAllOrganizations, Permisions = forOrgRoles.Permisions.Merge(globalRoles.Permisions)};
            }
            return globalRoles ?? forOrgRoles;
        }
    }
}
