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

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using PTV.Application.OpenApi.DataValidators;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using System.Linq;

namespace PTV.Application.OpenApi.EntityManagers
{
    /// <summary>
    /// Save organization manager
    /// </summary>
    public class SaveOrganizationManager : SaveEntityManagerBase<IVmOpenApiOrganizationInVersionBase, IVmOpenApiOrganizationVersionBase>
    {
        private readonly IOrganizationService service;
        private readonly ICodeService codeService;
        private readonly ICommonService commonService;

        /// <summary>
        /// Ctrl
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <param name="sourceId">External source id</param>
        /// <param name="model">the request model</param>
        /// <param name="openApiVersion">Open api version</param>
        /// <param name="modelState">Model state</param>
        /// <param name="logger">Logger</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="commonService">Common service</param>
        public SaveOrganizationManager(
            string id,
            string sourceId,
            IVmOpenApiOrganizationInVersionBase model,
            int openApiVersion,
            ModelStateDictionary modelState,
            ILogger logger,
            IOrganizationService organizationService,
            ICodeService codeService,
            ICommonService commonService)
            : base(id, sourceId, model, openApiVersion, modelState, logger)
        {
            service = organizationService;
            this.codeService = codeService;
            this.commonService = commonService;
        }

        /// <summary>
        /// Get the entity related error message.
        /// </summary>
        /// <returns></returns>
        protected override string GetErrorMessage()
        {
            if (ViewModel.Id.IsAssigned())
            {
                return $"Organization with id '{Id}' not found.";
            }

            return $"Organization with source id '{SourceId}' not found.";
        }

        /// <summary>
        /// The method to call to get the current version of entity.
        /// </summary>
        /// <returns></returns>
        protected override IVmOpenApiOrganizationVersionBase CallCurrentVersionServiceMethod()
        {
            return ViewModel.Id.IsAssigned() ? service.GetOrganizationById(ViewModel.Id.Value, 0, false) : service.GetOrganizationBySource(ViewModel.SourceId, 0, false);
        }

        /// <summary>
        /// Get the entity related validator.
        /// </summary>
        /// <returns></returns>
        protected override IBaseValidator GetValidator()
        {
            return new OrganizationValidator(ViewModel, codeService, service, CurrentVersion?.AvailableLanguages, commonService, OpenApiVersion);
        }

        /// <summary>
        /// The method for adding the organization.
        /// </summary>
        /// <returns></returns>
        protected override IVmOpenApiOrganizationVersionBase CallServiceMethod()
        {
            return service.SaveOrganization(ViewModel, OpenApiVersion);
        }
    }
}
