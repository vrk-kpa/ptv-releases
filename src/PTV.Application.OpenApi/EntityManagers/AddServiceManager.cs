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
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;

namespace PTV.Application.OpenApi.EntityManagers
{
    /// <summary>
    /// Add service manager
    /// </summary>
    public class AddServiceManager : EntityManagerBase<IVmOpenApiServiceInVersionBase, IVmOpenApiServiceBase>
    {
        private readonly bool attachProposedChannels;

        private readonly IServiceService service;
        private readonly IGeneralDescriptionService gdService;
        private readonly ICodeService codeService;
        private readonly IFintoService fintoService;
        private readonly IOntologyTermDataCache ontologyTermDataCache;
        private readonly ICommonService commonService;
        private readonly IChannelService channelService;
        private readonly IOrganizationService organizationService;

        private readonly UserRoleEnum userRole;

        /// <summary>
        /// Ctrl
        /// </summary>
        /// <param name="model">Channel request model</param>
        /// <param name="openApiVersion">Open api version</param>
        /// <param name="modelState">Model state</param>
        /// <param name="logger">Logger</param>
        /// <param name="attachProposedChannels"></param>
        /// <param name="serviceService">Service service</param>
        /// <param name="generalDescriptionService"></param>
        /// <param name="codeService">Code service</param>
        /// <param name="fintoService">Finto service</param>
        /// <param name="ontologyTermDataCache">Ontology term data cache</param>
        /// <param name="commonService">Common service</param>
        /// <param name="channelService">Channel service</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="userRole">User role</param>
        public AddServiceManager(
            IVmOpenApiServiceInVersionBase model,
            int openApiVersion,
            ModelStateDictionary modelState,
            ILogger logger,
            bool attachProposedChannels,
            IServiceService serviceService,
            IGeneralDescriptionService generalDescriptionService,
            ICodeService codeService,
            IFintoService fintoService,
            IOntologyTermDataCache ontologyTermDataCache,
            ICommonService commonService,
            IChannelService channelService,
            IOrganizationService organizationService,
            UserRoleEnum userRole
            ) : base(model, openApiVersion, modelState, logger)
        {
            this.attachProposedChannels = attachProposedChannels;
            service = serviceService;
            gdService = generalDescriptionService;
            this.codeService = codeService;
            this.fintoService = fintoService;
            this.ontologyTermDataCache = ontologyTermDataCache;
            this.commonService = commonService;
            this.channelService = channelService;
            this.organizationService = organizationService;
            this.userRole = userRole;
        }

        /// <summary>
        /// Get the entity related validator.
        /// </summary>
        /// <returns></returns>
        protected override IBaseValidator GetValidator()
        {
            return new ServiceValidator(ViewModel, gdService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, userRole, OpenApiVersion);
        }

        /// <summary>
        /// The method for adding the general description.
        /// </summary>
        /// <returns></returns>
        protected override IVmOpenApiServiceBase CallServiceMethod()
        {
            return service.AddService(ViewModel, OpenApiVersion, attachProposedChannels);
        }
    }
}
