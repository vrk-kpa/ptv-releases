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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Service base controller
    /// </summary>
    /// <seealso cref="PTV.Application.OpenApi.Controllers.ValidationBaseController" />
    public class ServiceBaseController : ValidationBaseController
    {
        private IServiceService serviceService;
        private IGeneralDescriptionService generalDescriptionService;
        private IServiceAndChannelService serviceAndChannelService;
        private ICodeService codeService;
        private IFintoService fintoService;

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
        /// <param name="organizationService">The organization service.</param>
        /// <param name="codeService">The code service.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generalDescriptionService">The general description service.</param>
        /// <param name="fintoService">The finto service.</param>
        /// <param name="serviceAndChannelService">The service and channel service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="versionNumber">Open api version.</param>
        public ServiceBaseController(
            IServiceService serviceService,
            IOrganizationService organizationService,
            ICodeService codeService,
            IOptions<AppSettings> settings,
            IGeneralDescriptionService generalDescriptionService,
            IFintoService fintoService,
            IServiceAndChannelService serviceAndChannelService,
            ILogger logger,
            int versionNumber)
            : base(organizationService, codeService, settings, fintoService, logger)
        {
            this.serviceService = serviceService;
            this.generalDescriptionService = generalDescriptionService;
            this.serviceAndChannelService = serviceAndChannelService;
            this.codeService = codeService;
            this.fintoService = fintoService;
            pageSize = Settings.PageSize > 0 ? Settings.PageSize : 1000;
            this.versionNumber = versionNumber;
        }

        /// <summary>
        /// Gets the identifier list.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="page">The page.</param>
        /// <returns>list</returns>
        protected IActionResult GetIdList(DateTime? date, int page)
        {
           return Ok(ServiceService.GetServices(date, page, pageSize));
        }

        /// <summary>
        /// Gets the identifier and name list.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="page">The page.</param>
        /// <returns>ids and names list</returns>
        protected IActionResult GetIdAndNameList(DateTime? date, int page)
        {
            return Ok(ServiceService.V3GetServices(date, page, pageSize));
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

            var msgList = ServiceAndChannelService.SaveServicesAndChannels(request);

            return Ok(msgList);
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
        /// Validate the request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="createOperation"></param>
        /// <param name="sourceId"></param>
        protected void ValidateParameters(IVmOpenApiServiceInVersionBase request, bool createOperation = false, string sourceId = null)
        {
            request.AvailableLanguages = GetServiceAvailableLanguages(request).ToList();
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
                }
            }
            ServiceValidator service = new ServiceValidator(request, generalDescriptionService, codeService, fintoService, OrganizationService, newLanguages);
            service.Validate(this.ModelState);
        }

        private HashSet<string> GetServiceAvailableLanguages(IVmOpenApiServiceInVersionBase vModel)
        {
            var list = new HashSet<string>();
            list.GetAvailableLanguages(vModel.ServiceNames);
            list.GetAvailableLanguages(vModel.ServiceDescriptions);
            
            return list;
        }
    }
}
