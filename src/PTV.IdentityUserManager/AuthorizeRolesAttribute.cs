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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;

namespace PTV.IdentityUserManager
{
    public class AuthorizeRolesAttribute : TypeFilterAttribute //: ClaimRoleRequirementAttribute // Microsoft.AspNetCore.Authorization.AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params UserRoleEnum[] roles) : base(typeof(AuthorizeRolesAttributeFilter))
        {
            Arguments = new object[] { new AuthorizeRolesAttributeArgument(roles) };
        }
    }

    public class AuthorizeRolesAttributeArgument
    {
        public List<UserRoleEnum> Roles { get; }

        public AuthorizeRolesAttributeArgument(params UserRoleEnum[] roles)
        {
            this.Roles = roles.ToList();
        }
    }


    public class AuthorizeRolesAttributeFilter : IActionFilter, IAsyncActionFilter
    {
        private readonly List<string> roles;

        public AuthorizeRolesAttributeFilter(AuthorizeRolesAttributeArgument roles)
        {
            this.roles = roles.Roles.Select(i => i.ToString()).ToList();
        }


        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var roleClaim = context.HttpContext?.User?.Claims?.FirstOrDefault(i => i.Type == JwtClaimTypes.Role)?.Value;
            if (!roles.Any(i => i == roleClaim))
            {
                context.Result = new ForbidResult();
            }
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var roleClaim = context.HttpContext?.User?.Claims?.FirstOrDefault(i => i.Type == JwtClaimTypes.Role)?.Value;
            if (!roles.Any(i => i == roleClaim))
            {
                context.Result = new ForbidResult();
                return;
            }
            await next();
        }
    }
}
