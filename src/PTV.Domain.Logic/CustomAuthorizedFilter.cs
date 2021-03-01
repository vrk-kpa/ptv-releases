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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Logic
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
        private readonly List<AccessRightEnum> rightsNeeded;
        private readonly IResolveManager resolveManager;

        public ClaimRequirementFilter(ClaimRequirementFilterArgument accessRights, IResolveManager resolveManager)
        {
            rightsNeeded = accessRights.AccessRights.ToList();
            this.resolveManager = resolveManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            using (var scope = resolveManager.CreateScope())
            {
                var pahaTokenProcessor = scope.ServiceProvider.GetService<IPahaTokenProcessor>();
                if ((pahaTokenProcessor.InvalidToken) || (!pahaTokenProcessor.ActiveOrganization.IsAssigned()))
                {
                    context.Result = CoreExtensions.ReturnStatusForbidden(pahaTokenProcessor.ErrorMessageFlat);
                    return;
                }
                if (pahaTokenProcessor.UserPresent)
                {
                    var userAccessRights = pahaTokenProcessor.UserAccessRights;
                    if (rightsNeeded.Any(r => userAccessRights.HasFlag(r)))
                    {
                        await next();
                    }
                }
                context.Result = new ForbidResult();
            }
        }
    }

    // ------------------------------------------

    public class ClaimRoleRequirementArgument
    {
        public List<UserRoleEnum> Roles { get; }

        public ClaimRoleRequirementArgument(params UserRoleEnum[] roles)
        {
            this.Roles = roles.ToList();
        }
    }

    public class ClaimRoleRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRoleRequirementAttribute(params UserRoleEnum[] roles) : base(typeof(ClaimRoleRequirementFilter))
        {
            Arguments = new object[] { new ClaimRoleRequirementArgument(roles) };
        }
    }

    public class ClaimRoleRequirementFilter : IAsyncActionFilter
    {
        readonly List<UserRoleEnum> rolesNeeded;
        readonly IResolveManager resolveManager;

        public ClaimRoleRequirementFilter(ClaimRoleRequirementArgument roles, IResolveManager resolveManager)
        {
            rolesNeeded = roles.Roles;
            this.resolveManager = resolveManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            using (var scope = resolveManager.CreateScope())
            {
                var pahaTokenProcessor = scope.ServiceProvider.GetService<IPahaTokenProcessor>();
                if ((pahaTokenProcessor.InvalidToken) || (!pahaTokenProcessor.ActiveOrganization.IsAssigned()))
                {
                    context.Result = CoreExtensions.ReturnStatusForbidden(pahaTokenProcessor.ErrorMessageFlat);
                    return;
                }

                if (rolesNeeded.Contains(pahaTokenProcessor.UserRole))
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
}
