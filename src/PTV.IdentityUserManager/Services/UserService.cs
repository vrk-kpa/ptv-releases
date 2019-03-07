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
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;
using PTV.LocalAuthentication;

namespace PTV.IdentityUserManager.Services
{
    [RegisterService(typeof(IUserService), RegisterType.Scope)]
    public class UserService : IUserService
    {
        UserManager<StsUser> userManager;
        private IUserRepository userRepository;
        private readonly RoleManager<StsRole> roleManager;
        private readonly IUserOrganizationService userOrganizationService;
        private IResolveManager resolveManager;

        public UserService(IUserRepository userRepository, UserManager<StsUser> userManager, RoleManager<StsRole> roleManager, IUserOrganizationService userOrganizationService, IResolveManager resolveManager)
        {
            this.userRepository = userRepository;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.userOrganizationService = userOrganizationService;
            this.resolveManager = resolveManager;
        }

        public Dictionary<Guid, Guid> GetUserOrganizations(Guid userId)
        {
            return new Dictionary<Guid, Guid>();
            //var roles = roleManager.Roles.ToDictionary(i => i.Id, i => i.Name);
            //var priorityMap = roles.ToDictionary(i => i.Key, i => (int) stsAndPtvRolesMap[i.Value]);
            //return userOrganizationService.GetUserOrganizations(userId, priorityMap);
        }

        public void FixWrongUserOrgMaps()
        {
//            var wrongUserMaps = userOrganizationService.GetUsersMissingRoles();
//            var usersRoles = userRepository.GetUsersAndRoles(wrongUserMaps);
//            usersRoles.ForEach(u => userOrganizationService.ReAssignMissingRoles(u.Item1, u.Item2));
//            var userEeva = userRepository.GetUserByUserName(IdentityServer4.Constants.BuiltInEeva.UserName);
//            var orgId = organizationService.GetOrganizationIdByBusinessCode("0245437-2");
//            if (orgId != null && userEeva != null)
//            {
//                var roleId = roleManager.Roles.FirstOrDefault(i => i.Name.ToLower() == "eeva");
//                if (roleId != null)
//                {
//                    userOrganizationService.MapUserToOrganization(userEeva.Id,
//                        new List<OrganizationRole>() {new OrganizationRole() {OrganizationId = orgId.Value, RoleId = roleId.Id}});
//                }
//            }
        }

        public async Task<IdentityResult> CreateNewUser(StsUser user, string password)
        {
            var result = await userManager.CreateAsync(user, password);
            if (result.Errors.Any()) return result;
            result = await userManager.AddClaimsAsync(user, new List<Claim>
            {
                new Claim(JwtClaimTypes.Name, user.Name),
                new Claim(JwtClaimTypes.Email, user.Email)
            });
            //if (result.Errors.Any()) return result;
            return result;
        }

        public async Task<IdentityResult> SaveUserInfo(string userId, Dictionary<Guid, Guid> organizationIds, StsUser newUserInfo)
        {
            var user = await userManager.FindByIdAsync(userId);

            IdentityResult result;

            // try to change the username first because duplicate e-mails are allowed
            // if we first change the email it will succeed but the username change will fail if duplicate
            // it will lead to situation that we return failure but the code has changed the email address
            // current sts implementation sets email and username to the same value
            if (user.UserName.ToLower() != newUserInfo.UserName.ToLower())
            {
                result = await userManager.SetUserNameAsync(user, newUserInfo.UserName);
                if (result.Errors.Any()) return result;
            }

            if (user.Email.ToLower() != newUserInfo.Email.ToLower())
            {
                result = await userManager.SetEmailAsync(user, newUserInfo.Email);
                if (result.Errors.Any()) return result;
            }

            MapUserToOrganization(userId, organizationIds);

            var claims = await userManager.GetClaimsAsync(user);
            var emailClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email);
            if (emailClaim != null)
            {
                result = await userManager.ReplaceClaimAsync(user, emailClaim, new Claim(JwtClaimTypes.Email, newUserInfo.Email));
            }
            else
            {
                result = await userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Email, newUserInfo.Email));
            }
            if (result.Errors.Any()) return result;

            var nameClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email);
            if (nameClaim != null)
            {
                result = await userManager.ReplaceClaimAsync(user, nameClaim, new Claim(JwtClaimTypes.Name, newUserInfo.Name));
            }
            else
            {
                result = await userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Name, newUserInfo.Name));
            }
            var allClaims = await userManager.GetClaimsAsync(user);
            var toRemove = allClaims.GroupBy(i => i.Type).SelectMany(i => i.Skip(1)).ToList();
            if (toRemove.Any())
            {
                await userManager.RemoveClaimsAsync(user, toRemove);
            }
            return result;
        }

        public IList<StsUser> GetAllUsers()
        {
            return userRepository.GetAllUsers();
        }

        public Dictionary<string, Guid> GetRoles()
        {
            return userRepository.GetRoles();
        }

        public void MapUserToOrganization(string userId, Dictionary<Guid, Guid> organizationIds)
        {
            var user = userRepository.GetUserById(userId);
            userOrganizationService.MapUserToOrganization(user.Id, organizationIds.Select(i => new OrganizationRole() { OrganizationId = i.Key, RoleId = i.Value}).ToList());
        }

        public Dictionary<Guid, Guid> GetUserOrganizationsForUserName(string userName)
        {
            var user = userRepository.GetUserByUserName(userName);
            if (user == null) return new Dictionary<Guid, Guid>();
            return GetUserOrganizations(user.Id);
        }

        public List<Guid> GetUserAllSubOrganizationsForUserName(IUnitOfWork unitOfWork, string userName)
        {
            var user = userRepository.GetUserByUserName(userName);
            if (user == null) return new List<Guid>();
            return userOrganizationService.GetUserCompleteOrgList(unitOfWork,user.Id);
        }

        public List<Guid> GetUserAllSubOrganizationsForUserName(string userName)
        {
            return resolveManager.Resolve<IContextManager>().ExecuteReader(unitOfWork => GetUserAllSubOrganizationsForUserName(unitOfWork, userName));
        }

        public List<Guid> GetCoUsersForUserName(string userName, Guid? whereRoleId = null)
        {
            var user = userRepository.GetUserByUserName(userName);
            if (user == null) return new List<Guid>();
            return userOrganizationService.GetCoUsersOfUser(user.Id, whereRoleId);
        }
        
        public bool IsUserAssignedToOrganization(string userName)
        {
            return GetUserOrganizationsForUserName(userName).Any();
        }

        public Dictionary<string, IEnumerable<VmUserOrganizationSts>> GetConnectedUsers()
        {
            var allUsers = GetAllUsers();
            var eevaId = allUsers.FirstOrDefault(x => string.Compare(x.UserName, Constants.BuiltInEeva.UserName, StringComparison.OrdinalIgnoreCase) == 0)?.Id;
            var roles = roleManager.Roles.ToDictionary(x => x.Id, x => x.Name);
            var allUsersWithOrganization = userOrganizationService.GetUsersWithOrganizations().Where(x => x.UserId != eevaId);
            var result = allUsers.Join(allUsersWithOrganization, x => x.Id, x => x.UserId,
                (stsUser, connectedUser) => new VmUserOrganizationSts()
                {
                    UserName = stsUser.UserName,
                    UserId = connectedUser.UserId,
                    OrganizationId = connectedUser.OrganizationId,
                    OrganizationName = connectedUser.OrganizationId.IsAssigned() ? connectedUser.OrganizationName : "Not assigned",
                    RoleId = connectedUser.RoleId,
                    Role = connectedUser.RoleId.IsAssigned() ? roles[connectedUser.RoleId] : "Not assigned"
                }).ToList();

            
            var notConnectedUsers = allUsers.Where(x => x.Id != eevaId && !result.Select(y => y.UserId).Contains(x.Id));
            result.AddRange(notConnectedUsers.Select(x => new VmUserOrganizationSts()
            {
                UserName = x.UserName,
                UserId = x.Id
            }));

            return result.GroupBy(x => x.UserId).ToDictionary(x => x.Select(y => y.UserName).First(), x => x.Select(y => y));
        }
    }
}
