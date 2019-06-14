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
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using PTV.Framework;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums.Security;
using PTV.LocalAuthentication;

namespace PTV.IdentityUserManager.Services
{
    [RegisterService(typeof(IUserRepository), RegisterType.Scope)]
    internal class UserRepository : IUserRepository
    {
        private readonly StsDbContext stsContext;
        private readonly IUserOrganizationService userOrganizationService;

        public UserRepository(StsDbContext context, IUserOrganizationService userOrganizationService)
        {
            this.stsContext = context;
            this.userOrganizationService = userOrganizationService;
        }

        public IList<StsUser> GetAllUsers()
        {
            var result = stsContext.Users.OrderBy(x => x.UserName).ToList();
            return result;
        }
        public Dictionary<string, Guid> GetRoles()
        {
            var result = stsContext.Roles.ToDictionary(x => x.Name, x => x.Id);
            return result;
        }

        public IList<Claim> GetUserClaims(Guid userId)
        {
            var stsClaims = stsContext.UserClaims.Where(i => i.UserId == userId).ToList()
                .Where(i => i.ClaimType.ToLower() != "role")
                .Select(i => new Claim(i.ClaimType, i.ClaimValue)).ToList();
            var roles = stsContext.Roles.ToDictionary(i => i.Id, i => i.Name);
            roles[Guid.Empty] = UserRoleEnum.Shirley.ToString();
            var mappedOrgs = userOrganizationService.GetUserOrganizationsWithRoles(userId);
            mappedOrgs.Where(i => !i.RoleId.IsAssigned()).ForEach(i => i.RoleId = roles.First(j => j.Value == UserRoleEnum.Shirley.ToString()).Key);
            //var orgClaimBuilder = mappedOrgs.Select(o => $"{o.OrganizationId}={roles[o.RoleId]}:{(o.IsMain ? "*" : string.Empty)}").ToList();
            //stsClaims.Add(new Claim(PtvClaims.UserOrganizations, string.Join(",", orgClaimBuilder)));
            var userHighestRole = mappedOrgs.Where(i => roles.Keys.Contains(i.RoleId)).Select(i => (UserRoleEnum)Enum.Parse(typeof(UserRoleEnum), roles[i.RoleId], true)).OrderBy(i => i).FirstOrDefault();
            stsClaims.Add(new Claim(JwtClaimTypes.Role, userHighestRole.ToString()));
            return stsClaims;
        }

        public StsUser GetUserById(string userId)
        {
            return stsContext.Users.First(user => user.Id == userId.ParseToGuid());
        }

        public StsUser GetUserByUserName(string userName)
        {
            return stsContext.Users.FirstOrDefault(user => user.UserName.ToLower() == userName.ToLower());
        }

        public List<Tuple<Guid, Guid>> GetUsersAndRoles(IList<Guid> userIds)
        {
            return stsContext.UserRoles.Where(i => userIds.Contains(i.UserId)).Select(i => new {i.UserId, i.RoleId}).ToList().Select(i => new Tuple<Guid, Guid>(i.UserId, i.RoleId)).ToList();
        }
    }
}
