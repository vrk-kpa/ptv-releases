using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PTV.Framework.Enums;

namespace PTV.Framework
{
    public class ClaimRequirementFilterArgument
    {
        public List<AccessRightEnum> AccessRights { get; }

        public ClaimRequirementFilterArgument(IEnumerable<AccessRightEnum> accessRights)
        {
            this.AccessRights = accessRights.ToList();
        }
    }

    public class AccessRightRequirementAttribute : TypeFilterAttribute
    {
        public AccessRightRequirementAttribute(params AccessRightEnum[] accessRight) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[]{ new ClaimRequirementFilterArgument(accessRight) };
        }
    }

    public class ClaimRequirementFilter : IAsyncActionFilter
    {
        readonly List<string> rightsNeeded;

        public ClaimRequirementFilter(ClaimRequirementFilterArgument accessRights)
        {
            rightsNeeded = accessRights.AccessRights.Select(i => i.ToString().ToLower()).ToList();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userAccessRightClaim = context.HttpContext?.User?.Claims?.FirstOrDefault(i => i.Type == PtvClaims.UserAccessRights);
            if (!string.IsNullOrEmpty(userAccessRightClaim?.Value))
            {
                var userAccessRights = userAccessRightClaim.Value.ToLower().Split(',');
                if (userAccessRights.Any(i => rightsNeeded.Contains(i)))
                {
                    context.HttpContext.Items[PtvClaims.HandlingPrefixAction+PtvClaims.UserAccessRights] = rightsNeeded;
                    context.HttpContext.Items[PtvClaims.HandlingPrefixUser+PtvClaims.UserAccessRights] = userAccessRights;
                    await next();
                }
            }
            context.Result = new ForbidResult();
        }
    }
}
