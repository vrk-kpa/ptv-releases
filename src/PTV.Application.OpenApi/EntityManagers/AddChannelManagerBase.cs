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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System;

namespace PTV.Application.OpenApi.EntityManagers
{
    /// <summary>
    /// Add channel base manager
    /// </summary>
    public abstract class AddChannelManagerBase<TModelRequestChannel> : EntityManagerBase<IVmOpenApiServiceChannelIn, IVmOpenApiServiceChannel>
        where TModelRequestChannel : class, IVmOpenApiServiceChannelIn
    {
        private readonly IChannelService service;
        private TModelRequestChannel channelVm;

        /// <summary>
        /// Service channel view model
        /// </summary>
        protected TModelRequestChannel ServiceChannelVm
        {
            get
            {
                try
                {
                    return channelVm ?? (channelVm =
                               (TModelRequestChannel) Convert.ChangeType(ViewModel, typeof(TModelRequestChannel)));
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error occured in AddChannelManagerBase.ServiceChannelVm. {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Organization service
        /// </summary>
        protected IOrganizationService OrganizationService;
        /// <summary>
        /// Code service
        /// </summary>
        protected ICodeService CodeService;
        /// <summary>
        /// Service service
        /// </summary>
        protected IServiceService ServiceService;
        /// <summary>
        /// Common service
        /// </summary>
        protected ICommonService CommonService;

        /// <summary>
        /// Ctrl
        /// </summary>
        /// <param name="model">the request model</param>
        /// <param name="openApiVersion">Open api version</param>
        /// <param name="modelState">Model state</param>
        /// <param name="logger">Logger</param>
        /// <param name="channelService">Channel service</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="serviceService">Service service</param>
        /// <param name="userRole">User role</param>
        /// <param name="commonService"></param>
        protected AddChannelManagerBase(
            IVmOpenApiServiceChannelIn model,
            int openApiVersion,
            ModelStateDictionary modelState,
            ILogger logger,
            IChannelService channelService,
            IOrganizationService organizationService,
            ICodeService codeService,
            IServiceService serviceService,
            ICommonService commonService)
            : base(model, openApiVersion, modelState, logger)
        {
            service = channelService;
            OrganizationService = organizationService;
            CodeService = codeService;
            ServiceService = serviceService;
            CommonService = commonService;
        }

        /// <summary>
        /// The method for adding the service channel
        /// </summary>
        /// <returns></returns>
        protected override IVmOpenApiServiceChannel CallServiceMethod()
        {
            return service.AddServiceChannel(ServiceChannelVm, OpenApiVersion);
        }
    }
}
