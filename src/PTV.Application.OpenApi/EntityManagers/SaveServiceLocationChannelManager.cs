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
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Application.OpenApi.EntityManagers
{
    /// <summary>
    /// Save service location channel manager
    /// </summary>
    public class SaveServiceLocationChannelManager : SaveChannelManagerBase<VmOpenApiServiceLocationChannelInVersionBase>
    {
        private const string ChannelName = "Service location";

        /// <summary>
        /// Ctrl
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <param name="sourceId">External source id</param>
        /// <param name="model">the request model</param>
        /// <param name="openApiVersion">Open api version</param>
        /// <param name="modelState">Model state</param>
        /// <param name="logger">Logger</param>
        /// <param name="channelName">The electronic channel name</param>
        /// <param name="channelService">Channel service</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="serviceService">Service service</param>
        /// <param name="commonService"></param>
        /// <param name="userRole">User role</param>
        public SaveServiceLocationChannelManager(
            string id,
            string sourceId,
            IVmOpenApiServiceLocationChannelInVersionBase model,
            int openApiVersion,
            ModelStateDictionary modelState,
            ILogger logger,
            IChannelService channelService,
            IOrganizationService organizationService,
            ICodeService codeService,
            IServiceService serviceService,
            ICommonService commonService,
            UserRoleEnum userRole)
            : base(id, sourceId, model, openApiVersion, modelState, logger, ChannelName, channelService, organizationService, codeService, serviceService, commonService, userRole)
        {
        }

        /// <summary>
        /// Get the entity related validator.
        /// </summary>
        /// <returns></returns>
        protected override IBaseValidator GetValidator()
        {
            return new ServiceLocationChannelValidator(ServiceChannelVm, OrganizationService, CodeService, ServiceService, CommonService, CurrentVersion, OpenApiVersion);
        }

        /// <summary>
        /// The method for saving the service channel.
        /// </summary>
        /// <returns></returns>
        protected override IVmOpenApiServiceChannel CallServiceMethod()
        {
            ServiceChannelVm.SetAddressProperties();
            return base.CallServiceMethod();
        }
    }
}
