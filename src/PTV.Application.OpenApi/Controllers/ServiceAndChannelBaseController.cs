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
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.OpenApi;
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
        private IServiceService serviceService;
        private IChannelService channelService;
        private IUserOrganizationService userOrganizationService;

        private int versionNumber;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceAndChannelService">The service and channel service.</param>
        /// <param name="serviceService">The service service.</param>
        /// <param name="channelService">The channel service.</param>
        /// <param name="userOrganizationService">The user organization service</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="versionNumber">Open api version.</param>
        public ServiceAndChannelBaseController(
            IServiceAndChannelService serviceAndChannelService,
            IServiceService serviceService,
            IChannelService channelService,
            IUserOrganizationService userOrganizationService,
            IOptions<AppSettings> settings, 
            ILogger logger,
            int versionNumber)
            : base(userOrganizationService, settings, logger)
        {
            this.serviceAndChannelService = serviceAndChannelService;
            this.serviceService = serviceService;
            this.channelService = channelService;
            this.userOrganizationService = userOrganizationService;

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

            var list = new List<V7VmOpenApiServiceServiceChannelAstiInBase>();
            request.ForEach(i => list.Add(i.GetLatestInVersionModel()));

            var msgList = serviceAndChannelService.SaveServicesAndChannels(list);

            return Ok(msgList);
        }

        /// <summary>
        /// Post service and channel relationship.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PostServiceAndChannelBase(List<V7VmOpenApiServiceAndChannelIn> request)
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

            var list = new List<V7VmOpenApiServiceServiceChannelAstiInBase>();
            request.ForEach(i => list.Add(i.GetLatestInVersionModel()));

            var msgList = serviceAndChannelService.SaveServicesAndChannels(list);

            return Ok(msgList);
        }

        /// <summary>
        /// Update service and channel relationships. This is for regular connections.
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PutServiceAndChannelBase(string serviceId, V5VmOpenApiServiceAndChannelRelationInBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }
            return PutServiceAndChannelByService(serviceId, request.ConvertToLatestVersion());
        }

        /// <summary>
        /// Update service and channel relationships. This is for regular connections.
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
            return PutServiceAndChannelByService(serviceId, request.ConvertToLatestVersion());
        }

        /// <summary>
        /// Update service and channel relationships. This is for ASTI connections.
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PutServiceAndChannelBase(string serviceId, V7VmOpenApiServiceAndChannelRelationAstiInBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }
            request.IsASTI = true;
            request.ChannelRelations.ForEach(r => r.IsASTIConnection = true);
            return PutServiceAndChannelByService(serviceId, request);
        }

        /// <summary>
        /// Update channel and service relationships. This is for ASTI connections.
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
            else
            {
                return NotFound(new VmError() { ErrorMessage = $"Service channel id has to be set." });
            }

            request.ServiceRelations.ForEach(r => 
            {
                r.ChannelGuid = request.ChannelId.Value;
                r.ServiceGuid = r.ServiceId.ParseToGuidWithExeption();
                r.ExtraTypes.ForEach(e => { e.ServiceGuid = r.ServiceGuid; e.ChannelGuid = r.ChannelGuid; });
                r.IsASTIConnection = true;
            });

            // Validate channel and services connections.
            // Validate channel for channel type (only service location channel can include additional connection information - PTV-2475)
            // Validate service ids.
            var channelValidator = new ServiceChannelConnectionListValidator(request.ServiceRelations, channelService, serviceService);
            channelValidator.Validate(this.ModelState);
            
            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var srv = serviceAndChannelService.SaveServiceChannelConnections(request, versionNumber);
            if (srv == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Service channel with id '{serviceChannelId}' not found." });
            }

            return Ok(srv);
        }

        /// <summary>
        /// Post service and channel relationship.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PostServiceAndChannelBySourceBase(List<VmOpenApiServiceServiceChannelBySource> request)
        {
            List<V7VmOpenApiServiceAndChannelRelationBySource> list = null;
            if (request?.Count > 0)
            {
                list = new List<V7VmOpenApiServiceAndChannelRelationBySource>();
                request.ForEach(r => list.Add(r.ConvertToLatestVersion()));
            }
            return PostServiceAndChannelBySourceBase(list);
        }

        /// <summary>
        /// Post service and channel relationship.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PostServiceAndChannelBySourceBase(List<V7VmOpenApiServiceAndChannelRelationBySource> request)
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

            return Ok(serviceAndChannelService.SaveServicesAndChannelsBySource(request));
        }

        /// <summary>
        /// Post service and channel relationship. This is for regular connections.
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
            return PutServiceConnectionsBySourceBaseCommon(serviceSourceId, request.ConvertToLatestVersion());
        }

        /// <summary>
        /// Post service and channel relationship. This is for regular connections.
        /// </summary>
        /// <param name="serviceSourceId">External source identifier for service</param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PutServiceConnectionsBySourceBase(string serviceSourceId, V7VmOpenApiServiceAndChannelRelationBySourceInBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }
            return PutServiceConnectionsBySourceBaseCommon(serviceSourceId, request.ConvertToLatestVersion());
        }


        /// <summary>
        /// Post service and channel relationship. This is for ASTI connections.
        /// </summary>
        /// <param name="serviceSourceId">External source identifier for service</param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PutServiceConnectionsBySourceBase(string serviceSourceId, V7VmOpenApiServiceAndChannelRelationBySourceAsti request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }
            request.IsASTI = true;
            request.ChannelRelations.ForEach(r => r.IsASTIConnection = true);
            return PutServiceConnectionsBySourceBaseCommon(serviceSourceId, request);
        }
                
        private IActionResult PutServiceAndChannelByService(string serviceId, V7VmOpenApiServiceAndChannelRelationAstiInBase request)
        {
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

                var currentVersion = serviceService.GetServiceByIdSimple(request.ServiceId.Value);
                if (currentVersion == null || string.IsNullOrEmpty(currentVersion.PublishingStatus))
                {
                    this.ModelState.AddModelError("Service id", $"Version for service with id '{serviceId}' not found.");
                }
                else
                {
                    // Validate publishing status
                    PublishingStatusValidator status = new PublishingStatusValidator(PublishingStatus.Published.ToString(), currentVersion.PublishingStatus);
                    status.Validate(ModelState);
                }

                // Validate the items
                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }
            }
            else
            {
                return NotFound(new VmError() { ErrorMessage = $"Service id has to be set." });
            }

            // Check the item values from db and validate
            request.ChannelRelations.ForEach(r =>
            {
                r.ServiceGuid = request.ServiceId.Value;
                r.ChannelGuid = r.ServiceChannelId.ParseToGuidWithExeption();
                r.ExtraTypes.ForEach(e => { e.ServiceGuid = r.ServiceGuid; e.ChannelGuid = r.ChannelGuid; });
            });

            // Asti users have Eeva rights - so channel visibility is not checked!
            var channels = new ServiceConnectionListValidator(request.ChannelRelations, channelService, request.IsASTI ? UserRoleEnum.Eeva : UserRole());
            channels.Validate(this.ModelState);

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var srv = serviceAndChannelService.SaveServiceConnections(request, versionNumber);
            if (srv == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Service with id '{serviceId}' not found." });
            }

            return Ok(srv);
        }

        private IActionResult PutServiceConnectionsBySourceBaseCommon(string serviceSourceId, V7VmOpenApiServiceAndChannelRelationBySourceAsti request)
        {
            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var srv = serviceAndChannelService.SaveServiceConnectionsBySource(serviceSourceId, request, versionNumber);
            if (srv == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Service with source id '{serviceSourceId}' not found." });
            }

            return Ok(srv);
        }
    }
}
