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
using System.IO;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services.Security
{
    [RegisterService(typeof(IUserInfoService), RegisterType.Transient)]
    internal class UserInfoService : IUserInfoService
    {
        private const string RoleClaimIdentifier = "role";
        private const string NameClaimIdentifier = "name";
        private readonly IHttpContextAccessor ctxAccessor;

        private readonly UserRolesCache userRolesCache;
        private IUserOrganizationService userOrganizationService;
        private IContextManager contextManager;

        public UserInfoService(IHttpContextAccessor ctxAccessor, UserRolesCache userRolesCache, IUserOrganizationService userOrganizationService, IContextManager contextManager)
        {
            this.ctxAccessor = ctxAccessor;
            this.userRolesCache = userRolesCache;
            this.userOrganizationService = userOrganizationService;
            this.contextManager = contextManager;
        }

        private IEnumerable<Claim> GetClaims()
        {
            return ctxAccessor?.HttpContext?.User?.Claims;
        }

//        private UserRoleEnum GetClaimRole()
//        {
//            var role = GetClaim(RoleClaimIdentifier);
//            UserRoleEnum result;
//            if (Enum.TryParse(role.Value, out result))
//            {
//                return result;
//            }
//            return UserRoleEnum.Shirley;
//        }

        private Claim GetClaim(string claimIdentifier)
        {
            return GetClaims().FirstOrDefault(i => i.Type == claimIdentifier);
        }

        private string GetClaimName()
        {
            return GetClaim(NameClaimIdentifier)?.Value;
        }

        private VmRoleInfo GetRoleInfo(UserRoleEnum role)
        {
            return userRolesCache.GetRole(role.ToString()) ?? new VmRoleInfo();
        }
        private Dictionary<string, VmPermision> GetPermisions(VmRoleInfo roleInfo, bool readOnly)
        {
            var permisions = roleInfo?.Permisions ?? new Dictionary<string, VmPermision>();
            if (readOnly){
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

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public VmUserInfo GetUserInfo()
        {
            var userOrgs = userOrganizationService.GetOrganizationsAndRolesForLoggedUser();
            if (userOrgs.IsNullOrEmpty())
            {
                return null;
            }
            var role = userOrgs.First().Role;
            userOrgs.ForEach(c => role = role < c.Role ? role : c.Role);
            var roleInfo = GetRoleInfo(role);
            var userOrganizationId = (userOrgs.FirstOrDefault(i => i.IsMain) ?? userOrgs.First()).OrganizationId;
            return new VmUserInfo
            {
                Name = GetClaimName(),
                Role = role,
                Permisions = GetPermisions(roleInfo, role != UserRoleEnum.Eeva && !userOrganizationId.IsAssigned()),
                UserOrganization = userOrganizationId
            };
        }
//
//       private Guid? GetUserOrganization()
//       {
//           return contextManager.ExecuteReader(unitOfWork => userOrganizationService.GetUserOrganizationId(unitOfWork).FirstOrDefault());
//       }
    }

    [RegisterService(typeof(UserRolesCache), RegisterType.Singleton)]
    internal class UserRolesCache
    {
        private Dictionary<string, VmRoleInfo> roles;
        private const string configurationFile = "UserRoles.json";

        public UserRolesCache(IHostingEnvironment environment)
        {
            var file = File.ReadAllText(environment.GetFilePath(Path.Combine("..", @"PTV.Database.DataAccess", "Services", "Security"), configurationFile));
            roles = JsonConvert.DeserializeObject<Dictionary<string, VmRoleInfo>>(file);
        }

        public VmRoleInfo GetRole(string roleName)
        {
            return roles.TryGet(roleName.ToLower());
        }
    }
}
