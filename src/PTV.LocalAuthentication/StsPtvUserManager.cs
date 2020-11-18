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
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model;
using PTV.Framework;

namespace PTV.LocalAuthentication
{
    [RegisterService(typeof(IStsPtvUserManager), RegisterType.Transient)]
    public class StsPtvUserManager : IStsPtvUserManager
    {
        private readonly UserManager<StsUser> userManager;
        private readonly IUserOrganizationService organizationService;
        private readonly RoleManager<StsRole> roleManager;
        //private IServiceProvider serviceProvider;

        public StsPtvUserManager(UserManager<StsUser> userManager, RoleManager<StsRole> roleManager, IUserOrganizationService organizationService)
        {
            this.userManager = userManager;
            this.organizationService = organizationService;
            this.roleManager = roleManager;
            //this.serviceProvider = serviceProvider;
        }

        public Guid CreateOrUpdateUser(StsJsonUser user)
        {
                var stsUser = userManager.FindByNameAsync(user.Username).Result;
                if (stsUser == null)
                {
                    if (!userManager.CreateAsync(new StsUser
                    {
                        Email = user.Username,
                        Id = Guid.NewGuid(),
                        Name = user.Username,
                        UserName = user.Username
                    }, user.Password).Result.Succeeded)
                    {
                        return Guid.Empty;
                    }

                    stsUser = userManager.FindByNameAsync(user.Username).Result;
                    if (stsUser == null)
                    {
                        return Guid.Empty;
                    }
                }

                if (!userManager.RemovePasswordAsync(stsUser).Result.Succeeded)
                {
                    return Guid.Empty;
                }

                if (!userManager.AddPasswordAsync(stsUser, user.Password).Result.Succeeded)
                {
                    return Guid.Empty;
                }

                var accessRightClaimOld = userManager.GetClaimsAsync(stsUser).Result
                    .FirstOrDefault(i => i.Type == "user_access_rights");
                var accessRightClaimNew = new Claim("user_access_rights", string.Join(',', user.Rights));
                if (accessRightClaimOld != null)
                {
                    if (!userManager.ReplaceClaimAsync(stsUser, accessRightClaimOld, accessRightClaimNew).Result.Succeeded)
                    {
                        return Guid.Empty;
                    }
                }
                else
                {
                    if (!userManager.AddClaimAsync(stsUser, accessRightClaimNew).Result.Succeeded)
                    {
                        return Guid.Empty;
                    }
                }

                var userAssignedRoles = userManager.GetRolesAsync(stsUser).Result;
                var toRemoveRoles = userAssignedRoles.Where(i => !i.Is(user.Role)).ToList();
                if (toRemoveRoles.Any())
                {
                    if (!userManager.RemoveFromRolesAsync(stsUser, toRemoveRoles).Result.Succeeded)
                    {
                        return Guid.Empty;
                    }
                }
                return stsUser.Id;
        }


        public ImportUserResults ImportUserJsonList(IList<StsJsonUser> usersList)
        {
            var roles = roleManager.Roles.Select(i => new { Id = i.Id, Name = i.Name.ToLower()}).ToList().ToDictionary(i => i.Name, i => i.Id);
            var usersMaps = usersList.Select(i => new { Username = i.Username, RoleId = roles.TryGetOrDefault(i.Role.ToLower(), Guid.Empty), UserId = CreateOrUpdateUser(i), OrganizationId = i.Organization}).ToList();
            var notCreatedUsers = usersList.Select(i => i.Username).Except(usersMaps.Select(i => i.Username)).ToList();
            var toBeMapped = usersMaps.Where(i => i.UserId.IsAssigned() && i.RoleId.IsAssigned()).Select(j =>
                    new UserOrgRoleMappingData
                    {
                        RoleId = j.RoleId,
                        OrganizationId = j.OrganizationId,
                        UserId = j.UserId
                    })
                .ToList();
            var mappedUsers = organizationService.UpdateUserOrgRolesMapping(toBeMapped);
            return new ImportUserResults
            {
                NoSavedUsers = notCreatedUsers,
                NoSavedMappings = toBeMapped.Except(mappedUsers).Select(i => new Tuple<Guid, Guid>(i.UserId, i.OrganizationId)).ToList()
            };
        }
    }


    public class ImportUserResults
    {
        public List<string> NoSavedUsers { get; set; }
        public List<Tuple<Guid, Guid>> NoSavedMappings { get; set; }
    }
}
