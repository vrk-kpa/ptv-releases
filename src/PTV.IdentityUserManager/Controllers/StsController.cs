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
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.LocalAuthentication;

namespace PTV.IdentityUserManager.Controllers
{
    public abstract class StsController : Controller
    {
        private IUserOrganizationService userOrganizationService;
        protected IHttpContextAccessor ContextAccessor;

        private string ConvertRoleToGroup(string role)
        {
            if (role == UserRoleEnum.Eeva.ToString()) return "PTV_ADMINISTRATOR";
            if (role == UserRoleEnum.Pete.ToString()) return "PTV_MAIN_USER";
            return "PTV_USER";
        }


        protected StsController(IHttpContextAccessor contextAccessor, ITokenService tokenService, IUserIdentification userIdentification,  IUserOrganizationService userOrganizationService)
        {
            this.ContextAccessor = contextAccessor;
            this.userOrganizationService = userOrganizationService;
            var loggedUser = contextAccessor.HttpContext?.User?.Identity;
            var claims = contextAccessor.HttpContext?.User?.Claims;
            if (loggedUser != null && claims != null && !string.IsNullOrEmpty(loggedUser.Name) && loggedUser.IsAuthenticated)
            {
                var localToken = tokenService.CreateToken(new TokenInputParams(loggedUser.Name, ConvertRoleToGroup(claims.FirstOrDefault(j => j.Type == ClaimTypes.Role)?.Value), claims.FirstOrDefault(j => j.Type == ClaimTypes.UserData)?.Value?.ParseToGuid() ?? Guid.Empty, string.Empty));
                ((IThreadUserInterface)userIdentification).SetBearerToken(localToken.AccessToken);
            }
        }

        protected List<List<IUserOrganizationRoles>> GetCompleteUserOrgStructure()
        {
            return userOrganizationService.GetUserCompleteOrgStructure();
        }

        protected UserRoleEnum? GetRoleForOrganizations(IList<Guid> organizationsIds)
        {
            var userOrgStruct = GetCompleteUserOrgStructure().SelectMany(i => i).ToList();
            if (userOrgStruct.Any(x => x.Role == UserRoleEnum.Eeva))
            {
                return UserRoleEnum.Eeva;
            }
            var userOrgStructIds = userOrgStruct.Select(i => i.OrganizationId).ToList();
            if (organizationsIds.Any(i => !userOrgStructIds.Contains(i)))
            {
                return null;
            }
            var allRolesToOrgs = userOrgStruct.Where(i => organizationsIds.Contains(i.OrganizationId)).Select(i => i.Role).ToList();
            if (allRolesToOrgs.Contains(UserRoleEnum.Shirley)) return UserRoleEnum.Shirley;
            if (allRolesToOrgs.Contains(UserRoleEnum.Pete)) return UserRoleEnum.Pete;
            return null;
        }
    }
}
