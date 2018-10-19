using System;
using System.Collections.Generic;
using System.Linq;
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
        private ITokenService tokenService;
        protected IHttpContextAccessor ContextAccessor;

        protected StsController(IHttpContextAccessor contextAccessor, ITokenService tokenService, IUserIdentification userIdentification,  IUserOrganizationService userOrganizationService)
        {
            this.ContextAccessor = contextAccessor;
            this.tokenService = tokenService;
            this.userOrganizationService = userOrganizationService;
            var loggedUser = contextAccessor.HttpContext?.User?.Identity;
            if (loggedUser != null && !string.IsNullOrEmpty(loggedUser.Name) && loggedUser.IsAuthenticated)
            {
                var localToken = tokenService.CreateToken(new TokenInputParams(loggedUser.Name, loggedUser.Name, Guid.Empty, string.Empty));
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
