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

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.DataValidators;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Service and channel base controller
    /// </summary>
    public class ServiceAndChannelBaseController : BaseController
    {
        private IServiceAndChannelService serviceAndChannelService;
        private IUserInfoService userService;
        private IServiceService serviceService;
        private IChannelService channelService;
        private ICommonService commonService;

        private int versionNumber;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceAndChannelService">The service and channel service.</param>
        /// <param name="userService">The user info service.</param>
        /// <param name="serviceService">The service service.</param>
        /// <param name="channelService">The channel service.</param>
        /// <param name="commonService">The common service</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="versionNumber">Open api version.</param>
        public ServiceAndChannelBaseController(
            IServiceAndChannelService serviceAndChannelService, 
            IUserInfoService userService,
            IServiceService serviceService,
            IChannelService channelService,
            ICommonService commonService,
            IOptions<AppSettings> settings, 
            ILogger logger,
            int versionNumber)
            : base(settings, logger)
        {
            this.serviceAndChannelService = serviceAndChannelService;
            this.userService = userService;
            this.serviceService = serviceService;
            this.channelService = channelService;
            this.commonService = commonService;

            this.versionNumber = versionNumber;
        }

        /// <summary>
        /// Post service and channel relationship.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult V2PostServiceAndChannelBase(List<V2VmOpenApiServiceAndChannel> request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }
            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var list = new List<VmOpenApiServiceServiceChannelInVersionBase>();
            request.ForEach(i => list.Add(i.VersionBase()));

            var userInfo = userService.GetUserInfo();
            Guid? orgId = null;
            if (userInfo != null) orgId = userInfo.UserOrganization;
            var msgList = serviceAndChannelService.SaveServicesAndChannels(list, orgId);

            return Ok(msgList);
        }

        /// <summary>
        /// Post service and channel relationship.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PostServiceAndChannelBase(List<VmOpenApiServiceServiceChannelInVersionBase> request)
        {            
            var userInfo = userService.GetUserInfo();
            Guid? orgId = null;
            if (userInfo != null) orgId = userInfo.UserOrganization;
            var msgList = serviceAndChannelService.SaveServicesAndChannels(request, orgId);

            return Ok(msgList);
        }
        /// <summary>
        /// Update service and channel relationships.
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult V5PutServiceAndChannelBase(string serviceId, V5VmOpenApiServiceAndChannelRelationInBase request)
        {
            return PutServiceAndChannelBase(serviceId, request != null ? request.ConvertToVersion7() : null);
            
        }


        /// <summary>
        /// Update service and channel relationships.
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PutServiceAndChannelBase(string serviceId, V7VmOpenApiServiceAndChannelRelationInBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            if (!string.IsNullOrEmpty(serviceId))
            {
                request.ServiceId = serviceId.ParseToGuid();

                // check that service exists
                if (!request.ServiceId.HasValue || !serviceService.ServiceExists(request.ServiceId.Value))
                {
                    return NotFound(new VmError() { ErrorMessage = $"Service with id '{serviceId}' not found." });
                }

                var currentVersion = serviceService.GetServiceById(request.ServiceId.Value, 0, false);
                if (currentVersion == null || string.IsNullOrEmpty(currentVersion.PublishingStatus))
                {
                    this.ModelState.AddModelError("Service id", $"Version for service with id '{serviceId}' not found.");
                }

                // Validate publishing status
                PublishingStatusValidator status = new PublishingStatusValidator(PublishingStatus.Published.ToString(), currentVersion.PublishingStatus);
                status.Validate(ModelState);

                // Validate the items
                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }
            }

            // Check the item values from db and validate
            request.ChannelRelations.ForEach(r =>
            {
                r.ServiceGuid = request.ServiceId.Value;
                r.ChannelGuid = r.ServiceChannelId.ParseToGuidWithExeption();
                r.ExtraTypes.ForEach(e => { e.ServiceGuid = r.ServiceGuid; e.ChannelGuid = r.ChannelGuid; });
            });

            var channelIds = request.ChannelRelations.Select(c => c.ChannelGuid).ToList();
            ServiceChannelIdListValidator channels = new ServiceChannelIdListValidator(channelIds, channelService, UserOrganizations());            
            channels.Validate(this.ModelState);

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            return Ok(serviceAndChannelService.SaveServiceConnections(request, versionNumber));
        }

        /// <summary>
        /// Update channel and service relationships.
        /// </summary>
        /// <param name="serviceChannelId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PutChannelServices(string serviceChannelId, V7VmOpenApiChannelServicesIn request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            if (!string.IsNullOrEmpty(serviceChannelId))
            {
                request.ChannelId = serviceChannelId.ParseToGuid();

                // check that channel exists
                if (!request.ChannelId.HasValue || !channelService.ChannelExists(request.ChannelId.Value))
                {
                    return NotFound(new VmError() { ErrorMessage = $"Service channel with id '{serviceChannelId}' not found." });
                }
            }

            request.ServiceRelations.ForEach(r => 
            {
                r.ChannelGuid = request.ChannelId.Value;
                r.ServiceGuid = r.ServiceId.ParseToGuidWithExeption();
                r.ExtraTypes.ForEach(e => { e.ServiceGuid = r.ServiceGuid; e.ChannelGuid = r.ChannelGuid; });
            });

            // Validate channel for visibility
            ServiceChannelIdListValidator channel = new ServiceChannelIdListValidator(new List<Guid> { request.ChannelId.Value }, channelService, UserOrganizations());
            channel.Validate(this.ModelState);

            // Validate service ids
            var serviceIds = request.ServiceRelations.Select(c => c.ServiceGuid).ToList();
            ServiceIdListValidator services = new ServiceIdListValidator(serviceIds, serviceService);
            services.Validate(this.ModelState);

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            return Ok(serviceAndChannelService.SaveServiceChannelConnections(request, versionNumber));
        }

        /// <summary>
        /// Post service and channel relationship.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PostServiceAndChannelBySourceBase(List<VmOpenApiServiceServiceChannelBySource> request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }
            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var userInfo = userService.GetUserInfo();
            Guid? orgId = null;
            if (userInfo != null) orgId = userInfo.UserOrganization;
            var msgList = serviceAndChannelService.SaveServicesAndChannelsBySource(request, orgId);

            return Ok(msgList);
        }

        /// <summary>
        /// Post service and channel relationship.
        /// </summary>
        /// <param name="serviceSourceId">External source identifier for service</param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PutServiceConnectionsBySourceBase(string serviceSourceId, V6VmOpenApiServiceAndChannelRelationBySourceInBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }
            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var userInfo = userService.GetUserInfo();
            Guid? orgId = null;
            if (userInfo != null) orgId = userInfo.UserOrganization;
            var msgList = serviceAndChannelService.SaveServiceConnectionsBySource(serviceSourceId, request, orgId, versionNumber);

            return Ok(msgList);
        }

        /// <summary>
        /// Get all user organizations
        /// </summary>
        /// <returns></returns>
        protected List<Guid> UserOrganizations()
        {
            var userInfo = userService.GetUserInfo();
            var userOrganizationList = new List<Guid>();

            if (userInfo != null && userInfo.UserOrganization.IsAssigned())
            {
                userOrganizationList = commonService.GetUserOrganizations(userInfo.UserOrganization.Value);
            }

            return userOrganizationList;
        }        
    }
}
