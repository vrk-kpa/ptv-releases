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
using System;
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Services.Security;

namespace PTV.Application.OpenApi.EntityManagers
{
    /// <summary>
    /// Add service collection manager
    /// </summary>
    public class AddServiceCollectionManager : EntityManagerBase<IVmOpenApiServiceCollectionInVersionBase, IVmOpenApiServiceCollectionBase>
    {
        private readonly IServiceCollectionService service;
        private readonly IServiceService serviceService;
        private readonly IOrganizationService organizationService;
        private readonly IChannelService channelService;
        private readonly IUserOrganizationService userOrganizationService;

        /// <summary>
        /// Ctrl
        /// </summary>
        /// <param name="model">Service collection request model</param>
        /// <param name="openApiVersion">Open api version</param>
        /// <param name="modelState">Model state</param>
        /// <param name="logger">Logger</param>
        /// <param name="serviceCollectionService">Service collection service</param>
        /// <param name="serviceService">Service service</param>
        /// <param name="channelService">Service service</param>
        /// <param name="userOrganizationService">Service service</param>
        /// <param name="organizationService">Organization service</param>
        public AddServiceCollectionManager(
            IVmOpenApiServiceCollectionInVersionBase model,
            int openApiVersion,
            ModelStateDictionary modelState,
            ILogger logger,
            IServiceCollectionService serviceCollectionService,
            IServiceService serviceService,
            IChannelService channelService,
            IUserOrganizationService userOrganizationService,
            IOrganizationService organizationService
            ) : base(model, openApiVersion, modelState, logger)
        {
            service = serviceCollectionService;
            this.serviceService = serviceService;
            this.organizationService = organizationService;
            this.channelService = channelService;
            this.userOrganizationService = userOrganizationService;
        }

        /// <summary>
        /// Get the entity related validator.
        /// </summary>
        /// <returns></returns>
        protected override IBaseValidator GetValidator()
        {
            return new ServiceCollectionValidator(ViewModel, serviceService, channelService, ViewModel.AvailableLanguages, organizationService, userOrganizationService, OpenApiVersion);
        }

        /// <summary>
        /// The method for adding the service collection.
        /// </summary>
        /// <returns></returns>
        protected override IVmOpenApiServiceCollectionBase CallServiceMethod()
        {
            return service.AddServiceCollection(ViewModel, OpenApiVersion);
        }
    }
}
