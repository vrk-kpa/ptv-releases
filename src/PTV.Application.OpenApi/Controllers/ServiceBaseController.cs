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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
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
    public class ServiceBaseController : ServiceAndChannelBaseController
    {
        private IServiceService serviceService;
        private IGeneralDescriptionService generalDescriptionService;
        private IServiceAndChannelService serviceAndChannelService;
        private ICodeService codeService;
        private IFintoService fintoService;
        private ICommonService commonService;
        private IChannelService channelService;
        private IUserOrganizationService userOrganizationService;

        private int pageSize;
        private int versionNumber;

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
        /// <param name="userOrganizationService">The user organization service.</param>
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
            IUserOrganizationService userOrganizationService,
            ILogger logger,
            int versionNumber)
            : base(serviceAndChannelService, serviceService, channelService, userOrganizationService, codeService, settings, logger, versionNumber)
        {
            this.serviceService = serviceService;
            this.generalDescriptionService = generalDescriptionService;
            this.serviceAndChannelService = serviceAndChannelService;
            this.codeService = codeService;
            this.fintoService = fintoService;
            this.commonService = commonService;
            this.channelService = channelService;
            this.userOrganizationService = userOrganizationService;
            pageSize = Settings.PageSize > 0 ? Settings.PageSize : 1000;
            this.versionNumber = versionNumber;
        }

        /// <summary>
        /// Gets the identifier and name list.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="page">The page.</param>
        /// <param name="archived">Indicates if archived items should be returned.</param>
        /// <param name="active">Indicates if only published services should be returned or also services with draft and modified versions.</param>
        /// <returns>ids and names list</returns>
        protected IActionResult GetIdAndNameList(DateTime? date, int page, bool archived = false, bool active = false)
        {
            if (page < 0)
            {
                ModelState.AddModelError("page", "The page number cannot be negative value.");
                return BadRequest(ModelState);
            }

            return Ok(serviceService.GetServices(date, page, pageSize, archived, active));
        }

        /// <summary>
        /// Get service by id base
        /// </summary>
        /// <param name="id"></param>
        /// <param name="getOnlyPublished"></param>
        /// <returns></returns>
        protected IActionResult GetById(string id, bool getOnlyPublished = true)
        {
            Guid guid = id.ParseToGuidWithExeption();

            var srv = serviceService.GetServiceById(guid, versionNumber, getOnlyPublished ? VersionStatusEnum.Published : VersionStatusEnum.LatestActive);

            if (srv == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Service with id '{id}' not found." });
            }

            return Ok(srv);
        }

        /// <summary>
        /// Get service by id base
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        protected IActionResult GetByIdListBase(string guids)
        {
            if (string.IsNullOrEmpty(guids))
            {
                ModelState.AddModelError("guids", "Property guids is required.");
                return new BadRequestObjectResult(ModelState);
            }
            List<Guid> guidList = new List<Guid>();
            var list = guids.Split(',');
            try
            {
                list.ForEach(id => guidList.Add(id.Trim().ParseToGuidWithExeption()));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("guids", ex.Message);
                return new BadRequestObjectResult(ModelState);
            }

            var srvs = serviceService.GetServices(guidList);

            if (srvs == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Services not found." });
            }

            return Ok(srvs);
        }

        /// <summary>
        /// Get service by service channel base.
        /// </summary>
        /// <param name="serviceChannelId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        protected virtual IActionResult GetByServiceChannelBase(string serviceChannelId, [FromQuery]DateTime? date)
        {
            Guid guid = serviceChannelId.ParseToGuidWithExeption();

            if (!channelService.ChannelExists(guid))
            {
                return NotFound(new VmError() { ErrorMessage = $"Service channel with id '{serviceChannelId}' not found." });
            }

            return Ok(serviceService.GetServicesByServiceChannel(guid, date, versionNumber));
        }

        /// <summary>
        /// Get service by service channel base.
        /// </summary>
        /// <param name="serviceChannelId"></param>
        /// <param name="date"></param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        protected virtual IActionResult GetGuidPageByServiceChannel(string serviceChannelId, DateTime? date, int page)
        {
            Guid guid = serviceChannelId.ParseToGuidWithExeption();

            if (!channelService.ChannelExists(guid))
            {
                return NotFound(new VmError() { ErrorMessage = $"Service channel with id '{serviceChannelId}' not found." });
            }

            return Ok(serviceService.GetServicesByServiceChannel(guid, date, page, pageSize));
        }

        /// <summary>
        /// Gets a list of published services for defined service class.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="date"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        protected IActionResult GetByServiceClassBase(string uri, DateTime? date, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                return BadRequest(new VmError() { ErrorMessage = $"Query string parameter uri is mandatory." });
            }
            string decodedUri = System.Net.WebUtility.UrlDecode(uri);
            var serviceClass = fintoService.GetServiceClassByUri(decodedUri);
            if (serviceClass == null || !serviceClass.Id.IsAssigned())
            {
                return NotFound(new VmError() { ErrorMessage = $"Service class with uri '{uri}' not found." });
            }
            return Ok(serviceService.GetServicesByServiceClass(serviceClass.Id, date, page, pageSize));
        }

        /// <summary>
        /// Gets a list of services related to defined municipality.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="date"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        protected IActionResult GetGuidPageByMunicipality(string code, [FromQuery]DateTime? date, [FromQuery]int page)
        {
            // check if municipality with given code exists
            var municipality = codeService.GetMunicipalityByCode(code, true);
            if (municipality == null || !municipality.Id.IsAssigned())
            {
                return NotFound(new VmError() { ErrorMessage = $"Municipality with code '{code}' not found." });
            }
            return Ok(serviceService.GetServicesByMunicipality(municipality.Id, date, page, pageSize));
        }

        /// <summary>
        ///Post service base.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="attachProposedChannels"></param>
        /// <returns></returns>
        protected IActionResult Post(IVmOpenApiServiceInVersionBase request, bool attachProposedChannels = false)
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
            if (request.PublishingStatus != PublishingStatus.Published.ToString())
            {
                request.PublishingStatus = PublishingStatus.Draft.ToString();
            }
            
            ServiceValidator service = new ServiceValidator(request, generalDescriptionService, codeService, fintoService, commonService, channelService, request.AvailableLanguages, UserRole());
            service.Validate(this.ModelState);

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var result = serviceService.AddService(request, Settings.AllowAnonymous, versionNumber, attachProposedChannels);
            return Ok(result);
        }

        /// <summary>
        /// Put service base.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <param name="sourceId"></param>
        /// <param name="attachProposedChannels"></param>
        /// <returns></returns>
        protected IActionResult Put(IVmOpenApiServiceInVersionBase request, string id = null, string sourceId = null, bool attachProposedChannels = false)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            if (id.IsNullOrEmpty() && sourceId.IsNullOrEmpty())
            {
                return NotFound(new VmError() { ErrorMessage = $"Service with id '{id}' not found." });
            }

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            // get the base model for service
            request = request.VersionBase();

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            IVmOpenApiServiceVersionBase currentVersion = null;

            if (!string.IsNullOrEmpty(id))
            {
                Guid? serviceId = id.ParseToGuid();

                // check that service exists
                if (!serviceId.HasValue)
                {
                    return NotFound(new VmError() { ErrorMessage = $"Service with id '{id}' not found." });
                }

                request.Id = serviceId;
                currentVersion = serviceService.GetServiceByIdSimple(request.Id.Value, false);
            }
            else if (!string.IsNullOrEmpty(sourceId))
            {
                currentVersion = serviceService.GetServiceBySource(sourceId);
            }

            // Check current version and data
            if (currentVersion == null || string.IsNullOrEmpty(currentVersion.PublishingStatus))
            {
                if (request.Id.IsAssigned())
                {
                    return NotFound(new VmError() { ErrorMessage = $"Service with id '{request.Id.Value}' not found." });
                }
                else
                {
                    return NotFound(new VmError() { ErrorMessage = $"Version for service with source id '{sourceId}' not found." });
                }
            }

            // Has user rights for the service
            var isOwnOrganization = ((VmOpenApiServiceVersionBase)currentVersion).Security == null ? false : ((VmOpenApiServiceVersionBase)currentVersion).Security.IsOwnOrganization;
            if (UserRole() != UserRoleEnum.Eeva && !isOwnOrganization)
            {
                return NotFound(new VmError() { ErrorMessage = $"User has no rights to update or create this entity!" });
            }

            request.CurrentPublishingStatus = currentVersion.PublishingStatus;
            // Get the available languages from current version
            // Check if user has added new language versions. New available languages and data need to be validated (required fields need to exist in request).
            var newLanguages = request.AvailableLanguages.Where(i => !currentVersion.AvailableLanguages.Contains(i)).ToList();

            // Check the general description data. If current version is attached into general description, service type cannot be updated for service.
            // Except if deleteStatutoryServiceGeneralDescriptionId is true (general description will be removed from the service).
            if (currentVersion.GeneralDescriptionId.IsAssigned() && !request.DeleteGeneralDescriptionId)
            {
                request.Type = null;
            }
            List<IVmOpenApiFintoItemVersionBase> list = new List<IVmOpenApiFintoItemVersionBase>();
            currentVersion.TargetGroups.ForEach(i => list.Add(i));
            ServiceValidator service = new ServiceValidator(request, generalDescriptionService, codeService, fintoService, commonService, channelService, newLanguages, UserRole(), list);
            service.Validate(this.ModelState);

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            return Ok(serviceService.SaveService(request, Settings.AllowAnonymous, versionNumber, attachProposedChannels, sourceId));
        }
    }
}
