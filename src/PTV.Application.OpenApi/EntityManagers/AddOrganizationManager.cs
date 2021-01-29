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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using PTV.Application.OpenApi.DataValidators;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;

namespace PTV.Application.OpenApi.EntityManagers
{
    /// <summary>
    /// Add organization manager
    /// </summary>
    public class AddOrganizationManager : EntityManagerBase<IVmOpenApiOrganizationInVersionBase, IVmOpenApiOrganizationVersionBase>
    {
        private readonly IOrganizationService service;
        private readonly ICodeService codeService;
        private readonly ICommonService commonService;
        private readonly IUserOrganizationService userService;

        /// <summary>
        /// Ctrl
        /// </summary>
        /// <param name="model">Channel request model</param>
        /// <param name="openApiVersion">Open api version</param>
        /// <param name="modelState">Model state</param>
        /// <param name="logger">Logger</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="commonService">Common service</param>
        /// <param name="userService">User service</param>
        public AddOrganizationManager(
            IVmOpenApiOrganizationInVersionBase model,
            int openApiVersion,
            ModelStateDictionary modelState,
            ILogger logger,
            IOrganizationService organizationService,
            ICodeService codeService,
            ICommonService commonService,
            IUserOrganizationService userService)
            : base(model, openApiVersion, modelState, logger)
        {
            service = organizationService;
            this.codeService = codeService;
            this.commonService = commonService;
            this.userService = userService;
        }

        /// <summary>
        /// Check the request and parameters
        /// </summary>
        /// <returns></returns>
        protected override IActionResult CheckRequestAndParameters()
        {
            var result = base.CheckRequestAndParameters();
            if (result != null) return result;

            if (ViewModel.ParentOrganizationId.IsNullOrEmpty())
            {
                // Check the user role - Pete user is not allowed to create main organization
                var userRole = userService.UserHighestRole();
                if (userRole != UserRoleEnum.Eeva)
                {
                    ModelState.AddModelError("Organization", "User has no rights to create this entity!");
                    return new BadRequestObjectResult(ModelState);
                }
            }

            return null;
        }

        /// <summary>
        /// Get the entity related validator.
        /// </summary>
        /// <returns></returns>
        protected override IBaseValidator GetValidator()
        {
            return new OrganizationValidator(ViewModel, codeService, service, null, commonService, OpenApiVersion, true);
        }

        /// <summary>
        /// The method for adding the organization.
        /// </summary>
        /// <returns></returns>
        protected override IVmOpenApiOrganizationVersionBase CallServiceMethod()
        {
            return service.AddOrganization(ViewModel, OpenApiVersion);
        }
    }
}
