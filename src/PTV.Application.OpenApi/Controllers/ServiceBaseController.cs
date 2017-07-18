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
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Service base controller
    /// </summary>
    /// <seealso cref="PTV.Application.OpenApi.Controllers.BaseController" />
    public class ServiceBaseController : BaseController
    {
        private IServiceService serviceService;
        private IGeneralDescriptionService generalDescriptionService;
        private IServiceAndChannelService serviceAndChannelService;
        private ICodeService codeService;
        private IFintoService fintoService;
        private ICommonService commonService;
        private IChannelService channelService;
        private IUserInfoService userService;

        private int pageSize;
        private int versionNumber;

        /// <summary>
        /// Gets the ServiceService instance.
        /// </summary>
        protected IServiceService ServiceService { get { return serviceService; } private set { } }

        /// <summary>
        /// Gets the ServiceAndChannelService instance.
        /// </summary>
        protected IServiceAndChannelService ServiceAndChannelService { get { return serviceAndChannelService; } private set { } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBaseController"/> class.
        /// </summary>
        /// <param name="serviceService">The service service.</param>
        /// <param name="commonService">The common service.</param>
        /// <param name="codeService">The code service.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generalDescriptionService">The general description service.</param>
        /// <param name="fintoService">The finto service.</param>
        /// <param name="serviceAndChannelService">The service and channel service.</param>
        /// <param name="channelService">The channel service.</param>
        /// <param name="userService">The user info service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="versionNumber">Open api version.</param>
        public ServiceBaseController(
            IServiceService serviceService,
            ICommonService commonService,
            ICodeService codeService,
            IOptions<AppSettings> settings,
            IGeneralDescriptionService generalDescriptionService,
            IFintoService fintoService,
            IServiceAndChannelService serviceAndChannelService,
            IChannelService channelService,
            IUserInfoService userService,
            ILogger logger,
            int versionNumber)
            : base( settings, logger)
        {
            this.serviceService = serviceService;
            this.generalDescriptionService = generalDescriptionService;
            this.serviceAndChannelService = serviceAndChannelService;
            this.codeService = codeService;
            this.fintoService = fintoService;
            this.commonService = commonService;
            this.channelService = channelService;
            this.userService = userService;
            pageSize = Settings.PageSize > 0 ? Settings.PageSize : 1000;
            this.versionNumber = versionNumber;
        }

        /// <summary>
        /// Gets the identifier and name list.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="page">The page.</param>
        /// <param name="archived">Indicates if archived items should be returned.</param>
        /// <returns>ids and names list</returns>
        protected IActionResult GetIdAndNameList(DateTime? date, int page, bool archived = false)
        {
            return Ok(ServiceService.GetServices(date, page, pageSize, archived));
        }

        /// <summary>
        /// Get service by id base
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected IActionResult GetById(string id)
        {
            Guid guid = id.ParseToGuidWithExeption();

            var srv = ServiceService.GetServiceById(guid, versionNumber);

            if (srv == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Service with id '{id}' not found." });
            }

            return Ok(srv);
        }

        /// <summary>
        /// Get service by service channel base.
        /// </summary>
        /// <param name="serviceChannelId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public virtual IActionResult GetByServiceChannel(string serviceChannelId, [FromQuery]DateTime? date)
        {
            Guid guid = serviceChannelId.ParseToGuidWithExeption();

            // TODO: should we actually add a check that the service channel with the id exists and if not return 404? -- AAL

            return Ok(ServiceService.GetServicesByServiceChannel(guid, date, versionNumber));
        }

        /// <summary>
        /// Gets a list of published services for defined service class.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="date"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        protected IActionResult GetByServiceClass(string uri, DateTime? date, int page = 1)
        {
            string decodedUri = System.Net.WebUtility.UrlDecode(uri);
            var serviceClass = fintoService.GetServiceClassByUri(decodedUri);
            if (serviceClass == null || !serviceClass.Id.IsAssigned())
            {
                ModelState.AddModelError("uri", string.Format(CoreMessages.OpenApi.CodeNotFound, decodedUri));
                return new BadRequestObjectResult(ModelState);
            }
            return Ok(ServiceService.GetServicesByServiceClass(serviceClass.Id, date, page, pageSize));
        }

        /// <summary>
        ///Post service base.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected IActionResult Post(IVmOpenApiServiceInVersionBase request)
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

            // Get the base model for service
            request = request.VersionBase();

            // Check the item values from db and validate
            ValidateParameters(request, true);
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var result = ServiceService.AddService(request, Settings.AllowAnonymous, versionNumber);
            return Ok(result);
        }

        /// <summary>
        /// Put service base.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        protected IActionResult Put(IVmOpenApiServiceInVersionBase request, string id = null, string sourceId = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                Guid? serviceId = id.ParseToGuid();

                // check that service exists
                if (!serviceId.HasValue || !ServiceService.ServiceExists(serviceId.Value))
                {
                    return NotFound(new VmError() { ErrorMessage = $"Service with id '{id}' not found." });
                }

                if (request == null)
                {
                    ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                    return new BadRequestObjectResult(ModelState);
                }

                request.Id = serviceId;
            }

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            // get the base model for service
            request = request.VersionBase();

            // Check the item values from db and validate
            ValidateParameters(request, sourceId: sourceId);
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            return Ok(ServiceService.SaveService(request, Settings.AllowAnonymous, versionNumber, sourceId));
        }

        /// <summary>
        /// Post service and channel relationship.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PostServiceAndChannelBase(List<V2VmOpenApiServiceAndChannel> request)
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
            var msgList = ServiceAndChannelService.SaveServicesAndChannels(request, orgId);

            return Ok(msgList);
        }

        /// <summary>
        /// Update service and channel relationships.
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

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            
            if (!string.IsNullOrEmpty(serviceId))
            {
                request.ServiceId = serviceId.ParseToGuid();

                // check that service exists
                if (!request.ServiceId.HasValue || !ServiceService.ServiceExists(request.ServiceId.Value))
                {
                    return NotFound(new VmError() { ErrorMessage = $"Service with id '{serviceId}' not found." });
                }

                var currentVersion = ServiceService.GetServiceById(request.ServiceId.Value, 0, false);
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
            var userInfo = userService.GetUserInfo();
            var userOrganizationList = new List<Guid>();
            
            if (userInfo != null && userInfo.UserOrganization.IsAssigned())
            {
                userOrganizationList = commonService.GetUserOrganizations(userInfo.UserOrganization.Value);
            }
            
            var channelIds = request.ChannelRelations.Select(c => c.ServiceChannelId).ToList();
            ServiceChannelIdListValidator channels = new ServiceChannelIdListValidator(channelIds, channelService, userOrganizationList);
            channels.Validate(this.ModelState);

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            return Ok(ServiceAndChannelService.SaveServiceAndChannels(request, versionNumber));
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
            var msgList = ServiceAndChannelService.SaveServicesAndChannelsBySource(request, orgId);

            return Ok(msgList);
        }

        /// <summary>
        /// Post service and channel relationship.
        /// </summary>
        /// <param name="serviceSourceId">External source identifier for service</param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PutServiceAndChannelBySourceBase(string serviceSourceId, V6VmOpenApiServiceAndChannelRelationBySourceInBase request)
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
            var msgList = ServiceAndChannelService.SaveServicesAndChannelsBySource(serviceSourceId, request, orgId, versionNumber);

            return Ok(msgList);
        }

        /// <summary>
        /// Validate the request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="createOperation"></param>
        /// <param name="sourceId"></param>
        protected void ValidateParameters(IVmOpenApiServiceInVersionBase request, bool createOperation = false, string sourceId = null)
        {
            IList<string> newLanguages = new List<string>();

            if (createOperation)
            {
                if (request.PublishingStatus != PublishingStatus.Published.ToString())
                {
                    request.PublishingStatus = PublishingStatus.Draft.ToString();
                }
                newLanguages = request.AvailableLanguages;
            }
            else
            {
                // We are updating existing service.
                // Get the current version and data related to it
                var externalSourceId = string.IsNullOrEmpty(sourceId) ? request.SourceId : sourceId;
                var currentVersion = request.Id.IsAssigned() ? serviceService.GetServiceById(request.Id.Value, 0, false) : serviceService.GetServiceBySource(externalSourceId, 0, false);
                if (currentVersion == null || string.IsNullOrEmpty(currentVersion.PublishingStatus))
                {
                    if (request.Id.IsAssigned())
                    {
                        this.ModelState.AddModelError("Service id", $"Version for service with id '{request.Id.Value}' not found.");
                    }
                    else
                    {
                        this.ModelState.AddModelError("Service id", $"Version for service with source id '{externalSourceId}' not found.");
                    }
                }
                else
                {
                    request.CurrentPublishingStatus = currentVersion.PublishingStatus;
                    // Get the available languages from current version
                    // Check if user has added new language versions. New available languages and data need to be validated (required fields need to exist in request).
                    newLanguages = request.AvailableLanguages.Where(i => !currentVersion.AvailableLanguages.Contains(i)).ToList();

                    // Check the general description data. If current version is attached into general description, service type cannot be updated for service.
                    if (currentVersion.StatutoryServiceGeneralDescriptionId.IsAssigned())
                    {
                        request.Type = null;
                    }
                }
                
            }
            ServiceValidator service = new ServiceValidator(request, generalDescriptionService, codeService, fintoService, commonService, newLanguages);
            service.Validate(this.ModelState);
        }
    }
}
