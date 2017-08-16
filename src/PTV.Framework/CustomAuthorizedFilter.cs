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

    // ------------------------------------------

    public class ClaimRoleRequirementArgument
    {
        public List<string> Roles { get; }

        public ClaimRoleRequirementArgument(params string[] roles)
        {
            this.Roles = roles.ToList();
        }
    }

    public class ClaimRoleRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRoleRequirementAttribute(params string[] roles) : base(typeof(ClaimRoleRequirementFilter))
        {
            Arguments = new object[] { new ClaimRoleRequirementArgument(roles) };
        }
    }

    public class ClaimRoleRequirementFilter : IAsyncActionFilter
    {
        readonly List<string> rolesNeeded;

        public ClaimRoleRequirementFilter(ClaimRoleRequirementArgument roles)
        {
            rolesNeeded = roles.Roles.Select(i => i.ToString().ToLower()).ToList();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var rolesUserClaim = context.HttpContext?.User?.Claims?.FirstOrDefault(i => i.Type == PtvClaims.UserOrganizations);
            if (rolesUserClaim == null)
            {
                context.Result = new ForbidResult();
                return;
            }
            var currentUserRoles = rolesUserClaim.Value.ToLower().Split(',').Select(i => i.Split('=')).Where(i => i.Length == 2).Select(i => i[1].Split(':')[0].ToLower()).ToList();
            if (rolesNeeded.Intersect(currentUserRoles).Any())
            {
                await next();
            }
            else
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
