/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;
using PTV.LocalAuthentication;

namespace PTV.DataImport.Console.Services
{
    [RegisterService(typeof(IUserService), RegisterType.Scope)]
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly RoleManager<StsRole> roleManager;
        private readonly IUserOrganizationService userOrganizationService;
        private readonly IResolveManager resolveManager;
        private readonly IAuthorizationService authorizationService;
        private readonly ILogger<UserService> logger;

        public UserService(IUserRepository userRepository, RoleManager<StsRole> roleManager, IUserOrganizationService userOrganizationService, IResolveManager resolveManager, IAuthorizationService authorizationService, ILogger<UserService> logger)
        {
            this.userRepository = userRepository;
            this.roleManager = roleManager;
            this.userOrganizationService = userOrganizationService;
            this.resolveManager = resolveManager;
            this.authorizationService = authorizationService;
            this.logger = logger;
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

        public IList<VmUserSts> GetAllUsers()
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
            userOrganizationService.MapUserToOrganization(user.Id, organizationIds.Select(i => new OrganizationRole { OrganizationId = i.Key, RoleId = i.Value}).ToList());
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

        private int FlagMatches(AccessRightEnum enumA, AccessRightEnum enumB)
        {
            var matches = 0;
            var allValues = Enum.GetValues(typeof(AccessRightEnum)).Cast<AccessRightEnum>();
            foreach (var enumValue in allValues)
            {
                if (enumA.HasFlag(enumValue) && enumB.HasFlag(enumValue))
                {
                    matches++;
                }
            }
            return matches;
        }

        private void MapOldIdentityClaimsToUserGroup(VmUserOrganizationSts user)
        {
            if (user.UserAccessRight == null) user.UserAccessRight = string.Empty;
            var accessRightsSet = user.UserAccessRight.Split(',').Select(i => i.Trim().ConvertToEnum<AccessRightEnum>()).WhereNotNull().Cast<AccessRightEnum>().ToList();
            if (!accessRightsSet.Any())
            {
                accessRightsSet = Enum.GetValues(typeof(AccessRightEnum)).Cast<AccessRightEnum>().ToList();
            }
            var accessRights = accessRightsSet.Aggregate((i,j) => i|j );
            var userGroups = authorizationService.GetUserAccessGroupsFull();
            var stsRole = user.Role.ToLowerInvariant();
            var perRoleGroups = userGroups.Where(i => i.UserRole.ToString().ToLowerInvariant() == stsRole).ToList();
            var bestGroup = perRoleGroups.OrderByDescending(i => FlagMatches(i.AccessRightFlag, accessRights)).FirstOrDefault();
            if (bestGroup == null)
            {
                logger.LogWarning($"TokenService> User '{user.UserName}' has no matching user group. Assigned role: {stsRole}, access rights: {user.UserAccessRight}");
                return;
            }
            user.UserAccessRightsGroup = bestGroup.Code;
        }

        public IEnumerable<VmExportUserJson> GetConnectedUsers()
        {

            var allUsers = GetAllUsers();
            if (File.Exists(@"filterUser.csv"))
            {
                var lines = File.ReadAllLines(@"filterUser.csv");
                allUsers = allUsers.Where(x => !lines.Contains(x.UserName)).ToList();
            }

            allUsers = allUsers.Where(x => !(x.UserName.ToLower().Contains("nok-") || x.UserName.ToLower().Contains("delete") || x.UserName.ToLower().Contains("nok."))).ToList();

            var eevaId = allUsers.FirstOrDefault(x => string.Compare(x.UserName, @"EevaAdmin@ptv.com", StringComparison.OrdinalIgnoreCase) == 0)?.Id;
            var roles = roleManager.Roles.ToDictionary(x => x.Id, x => x.Name);
            var allUsersWithOrganization = userOrganizationService.GetUsersWithOrganizations().Where(x => x.UserId != eevaId);
            var result = allUsers.Join(allUsersWithOrganization, x => x.Id, x => x.UserId,
                (stsUser, connectedUser) => new VmUserOrganizationSts
                {
                    UserName = stsUser.UserName,
                    UserId = connectedUser.UserId,
                    IsParent = connectedUser.IsParent,
                    OrganizationId = connectedUser.OrganizationId,
                    OrganizationName = connectedUser.OrganizationId.IsAssigned() ? connectedUser.OrganizationName : "Not assigned",
                    Role = roles[connectedUser.RoleId],
                    BusinessCode = connectedUser.BusinessCode,
                    UserAccessRight = stsUser.UserAccessRight
                }).ToList();

            var groupedUsers = result.GroupBy(x => x.UserName).Select(x =>
            {
                return new VmExportUserJson
                {
                    UserName = x.Key,
                    Organizations = x.Where(z => z.IsParent).Select(y =>
                    {
                        MapOldIdentityClaimsToUserGroup(y);
                        return new VmExportOrganizationJson
                        {
                            OrganizationName = y.OrganizationName,
                            BusinessCode = y.BusinessCode,
                            Role = y.Role,
                            UserAccessRightsGroup = y.UserAccessRightsGroup
                        };
                    }).ToList()
                };
            });
            return groupedUsers;
            // result.ForEach(x => { MapOldIdentityClaimsToUserGroup(x); });

//            var notConnectedUsers = allUsers.Where(x => x.Id != eevaId && !result.Select(y => y.UserId).Contains(x.Id.Value));
//            result.AddRange(notConnectedUsers.Select(x => new VmUserOrganizationSts()
//            {
//                UserName = x.UserName,
//                UserId = x.Id.Value
//            }));
            //result.ForEach(x => { MapOldIdentityClaimsToUserGroup(x); });
            // return groupedUsers.ToDictionary(x => x.Select(y => y.UserName).First(), x => x.Select(y => y));
            //return result.GroupBy(x => x.UserId).ToDictionary(x => x.Select(y => y.UserName).First(), x => x.Select(y => y));
        }
    }
}
