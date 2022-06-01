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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.EntityManagers
{
    /// <summary>
    /// Save channel base manager
    /// </summary>
    public abstract class SaveChannelManagerBase<TModelRequestChannel> : SaveEntityManagerBase<IVmOpenApiServiceChannelIn, IVmOpenApiServiceChannel>
        where TModelRequestChannel : class, IVmOpenApiServiceChannelIn
    {
        private readonly string channelName;
        private readonly UserRoleEnum userRole;
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
                    if (channelVm == null)
                    {
                        channelVm = (TModelRequestChannel)Convert.ChangeType(ViewModel, typeof(TModelRequestChannel));
                    }

                    return channelVm;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Error occured in SaveChannelManagerBase.ServiceChannelVm. {ex.Message}");
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
        public SaveChannelManagerBase(
            string id,
            string sourceId,
            IVmOpenApiServiceChannelIn model,
            int openApiVersion,
            ModelStateDictionary modelState,
            ILogger logger,
            string channelName,
            IChannelService channelService,
            IOrganizationService organizationService,
            ICodeService codeService,
            IServiceService serviceService,
            ICommonService commonService,
            UserRoleEnum userRole)
            : base(id, sourceId, model, openApiVersion, modelState, logger)
        {
            this.channelName = channelName;
            service = channelService;
            OrganizationService = organizationService;
            CodeService = codeService;
            ServiceService = serviceService;
            CommonService = commonService;
            this.userRole = userRole;
        }

        /// <summary>
        /// Get the entity related error message.
        /// </summary>
        /// <returns></returns>
        protected override string GetErrorMessage()
        {
            if (ViewModel.Id.IsAssigned())
            {
                return $"{channelName} channel with id '{Id}' not found.";
            }

            return $"{channelName} channel with source id '{SourceId}' not found.";
        }

        /// <summary>
        /// The method to call to get the current version of entity.
        /// </summary>
        /// <returns></returns>
        protected override IVmOpenApiServiceChannel CallCurrentVersionServiceMethod()
        {
            return ViewModel.Id.IsAssigned() ? service.GetServiceChannelByIdSimple(ViewModel.Id.Value, false) : service.GetServiceChannelBySource(ViewModel.SourceId);
        }

        /// <summary>
        /// Check the request and parameters.
        /// </summary>
        /// <returns></returns>
        protected override IActionResult CheckRequestAndParameters()
        {
            var result = base.CheckRequestAndParameters();
            if (result != null) return result;


            // Check lock (PTV-3391)
            if (CurrentVersion.Id.IsAssigned())
            {
                var entityLock = CommonService.EntityLockedBy(CurrentVersion.Id.Value);
                if (entityLock.EntityLockStatus != EntityLockEnum.Unlocked)
                {
                    return new NotFoundObjectResult(new VmError { ErrorMessage = $"{channelName} channel is locked by '{entityLock.LockedBy}'." });
                }
            }

            // Has user rights for the channel
            var isOwnOrganization = ((VmOpenApiServiceChannel)CurrentVersion).Security == null ? false : ((VmOpenApiServiceChannel)CurrentVersion).Security.IsOwnOrganization;
            if (userRole != UserRoleEnum.Eeva && !isOwnOrganization)
            {
                return new NotFoundObjectResult(new VmError { ErrorMessage = "User has no rights to update or create this entity!" });
            }

            return null;
        }

        /// <summary>
        /// The steps performed before validating the data within request view model.
        /// </summary>
        protected override void PreDataValidationSteps()
        {
            base.PreDataValidationSteps();
            ViewModel.ChannelId = CurrentVersion.ChannelId;
            // Set the organization from current version if it is not set within the request (PTV-4391)
            if (ViewModel.OrganizationId.IsNullOrEmpty() && CurrentVersion.OrganizationId != null)
            {
                ViewModel.OrganizationId = CurrentVersion.OrganizationId.ToString();
            }
        }

        /// <summary>
        /// The method for saving the service channel.
        /// </summary>
        /// <returns></returns>
        protected override IVmOpenApiServiceChannel CallServiceMethod()
        {
            return service.SaveServiceChannel(ServiceChannelVm, OpenApiVersion);
        }
    }
}
