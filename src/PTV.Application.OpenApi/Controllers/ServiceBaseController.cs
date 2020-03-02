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


using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.EntityManagers;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Service base controller
    /// </summary>
    /// <seealso cref="PTV.Application.OpenApi.Controllers.EntityWithOrganizationBaseController" />
    public class ServiceBaseController : EntityWithOrganizationBaseController
    {
        private readonly IServiceService serviceService;
        private readonly IGeneralDescriptionService generalDescriptionService;
        private readonly ICodeService codeService;
        private readonly IFintoService fintoService;
        private readonly IChannelService channelService;

        private readonly int versionNumber;
        private readonly IOntologyTermDataCache ontologyTermDataCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBaseController"/> class.
        /// </summary>
        /// <param name="serviceService">The service service.</param>
        /// <param name="commonService">The common service.</param>
        /// <param name="codeService">The code service.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generalDescriptionService">The general description service.</param>
        /// <param name="fintoService">The finto service.</param>
        /// <param name="ontologyTermDataCache">ontologyTermDataCache</param>
        /// <param name="serviceAndChannelService">The service and channel service.</param>
        /// <param name="channelService">The channel service.</param>
        /// <param name="organizationService">The organization service.</param>
        /// <param name="userOrganizationService">The user organization service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="versionNumber">Open api version.</param>
        /// <param name="industrialClassCache">Industrial class cache</param>
        public ServiceBaseController(
            IServiceService serviceService,
            ICommonService commonService,
            ICodeService codeService,
            IOptions<AppSettings> settings,
            IGeneralDescriptionService generalDescriptionService,
            IFintoService fintoService,
            IOntologyTermDataCache ontologyTermDataCache,
            IChannelService channelService,
            IUserOrganizationService userOrganizationService,
            IOrganizationService organizationService,
            ILogger logger,
            int versionNumber)
            : base(organizationService, commonService, userOrganizationService, settings, logger)
        {
            this.serviceService = serviceService;
            this.generalDescriptionService = generalDescriptionService;
            this.codeService = codeService;
            this.fintoService = fintoService;
            this.channelService = channelService;
            this.versionNumber = versionNumber;
            this.ontologyTermDataCache = ontologyTermDataCache;
        }

        /// <summary>
        /// Gets the identifier and name list.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="dateBefore">The date before</param>
        /// <param name="page">The page.</param>
        /// <param name="status">Indicates if only published services should be returned or also services with draft and modified versions.</param>
        /// <returns>ids and names list</returns>
        protected IActionResult GetIdAndNameList(DateTime? date, int page, EntityStatusExtendedEnum status = EntityStatusExtendedEnum.Published, DateTime? dateBefore = null)
        {
            if (page < 0)
            {
                ModelState.AddModelError("page", "The page number cannot be negative value.");
                return BadRequest(ModelState);
            }

            return Ok(serviceService.GetServices(date, page, PageSize, status, dateBefore));
        }

        /// <summary>
        /// Get service by id base
        /// </summary>
        /// <param name="id"></param>
        /// <param name="getOnlyPublished"></param>
        /// <param name="fillWithAllGdData"></param>
        /// <returns></returns>
        protected IActionResult GetById(string id, bool getOnlyPublished = true, bool fillWithAllGdData = false)
        {
            var guid = id.ParseToGuidWithExeption();

            var srv = serviceService.GetServiceById(guid, versionNumber, getOnlyPublished ? VersionStatusEnum.Published : VersionStatusEnum.LatestActive, fillWithAllGdData);

            if (srv == null)
            {
                return NotFound(new VmError { ErrorMessage = $"Service with id '{id}' not found." });
            }

            return Ok(srv);
        }

        /// <summary>
        /// Get services by ids base
        /// </summary>
        /// <param name="guids"></param>
        /// <param name="fillWithAllGdData"></param>
        /// <returns></returns>
        protected IActionResult GetByIdListBase(string guids, bool fillWithAllGdData = false)
        {
            if (string.IsNullOrEmpty(guids))
            {
                ModelState.AddModelError("guids", "Property guids is required.");
                return new BadRequestObjectResult(ModelState);
            }
            var guidList = new List<Guid>();
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

            var service = serviceService.GetServices(guidList, versionNumber, fillWithAllGdData);

            if (service == null)
            {
                return NotFound(new VmError { ErrorMessage = "Services not found." });
            }

            return Ok(service);
        }

        /// <summary>
        /// Get services by ids base.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="code"></param>
        /// <param name="oid"></param>
        /// <param name="page"></param>
        /// <param name="fillWithAllGdData"></param>
        /// <returns></returns>
        protected IActionResult GetByOrganizationBase(string organizationId = null, string code = null, string oid = null, int page = 1, bool fillWithAllGdData = false)
        {
            if (page < 0)
            {
                ModelState.AddModelError("page", "The page number cannot be negative value.");
                return BadRequest(ModelState);
            }

            // Check that the organization exists
            var organizationResult = CheckAndGetOrganizationIds(organizationId, code, oid, true);
            if (organizationResult.Item1 != null)
            {
                return organizationResult.Item1;
            }

            if (organizationResult.Item2?.Count > 0)
            {
                return Ok(serviceService.GetServicesByOrganization(organizationResult.Item2, versionNumber, page, fillWithAllGdData));
            }

            return NotFound(new VmError { ErrorMessage = "Services not found." });
        }

        /// <summary>
        /// Get services by ids base.
        /// </summary>
        /// <param name="area"></param>
        /// <param name="code"></param>
        /// <param name="includeWholeCountry"></param>
        /// <param name="serviceWithGD"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        protected IActionResult GetByAreaBase(string area, string code, bool includeWholeCountry, bool serviceWithGD, int page = 1)
        {
            // Check if municipality with given code exists
            if (area == AreaTypeEnum.Municipality.ToString())
            {
                var municipality = codeService.GetMunicipalityByCode(code, true);
                if (municipality == null || !municipality.Id.IsAssigned())
                {
                    return NotFound(new VmError { ErrorMessage = $"Municipality with code '{code}' not found." });
                }
                return Ok(serviceService.GetServicesWithAllDataByMunicipality(municipality.Id, includeWholeCountry, versionNumber, page, serviceWithGD));
            }

            // Get services for certain area (not municipality). SFIPTV-822
            var areaId = codeService.GetAreaIdByCodeAndType(code, area);
            if (!areaId.IsAssigned())
            {
                return NotFound(new VmError { ErrorMessage = $"Area {area} with code '{code}' not found." });
            }
            // ReSharper disable once PossibleInvalidOperationException
            return Ok(serviceService.GetServicesWithAllDataByArea(areaId.Value, includeWholeCountry, versionNumber, page, serviceWithGD));
        }

        /// <summary>
        /// Get service by service channel base.
        /// </summary>
        /// <param name="serviceChannelId"></param>
        /// <param name="date"></param>
        /// <param name="page">The page.</param>
        /// <param name="dateBefore"></param>
        /// <returns></returns>
        protected IActionResult GetGuidPageByServiceChannel(string serviceChannelId, DateTime? date, int page, DateTime? dateBefore = null)
        {
            var guid = serviceChannelId.ParseToGuidWithExeption();

            if (!channelService.ChannelExists(guid))
            {
                return NotFound(new VmError { ErrorMessage = $"Service channel with id '{serviceChannelId}' not found." });
            }

            return Ok(serviceService.GetServicesByServiceChannel(guid, date, page, PageSize, dateBefore));
        }

        /// <summary>
        /// Gets a list of published services for defined service class.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="date"></param>
        /// <param name="page"></param>
        /// <param name="dateBefore"></param>
        /// <returns></returns>
        protected IActionResult GetByServiceClassBase(string uri, DateTime? date, int page = 1, DateTime? dateBefore = null)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                return BadRequest(new VmError { ErrorMessage = "Query string parameter uri is mandatory." });
            }
            var decodedUri = WebUtility.UrlDecode(uri);
            var serviceClass = fintoService.GetServiceClassByUri(decodedUri);
            if (serviceClass == null || !serviceClass.Id.IsAssigned())
            {
                return NotFound(new VmError { ErrorMessage = $"Service class with uri '{uri}' not found." });
            }

            // Include also the parent service class within search criteria if one can be found (PTV-3613).
            var serviceClassIdList = new List<Guid> { serviceClass.Id };
            if (serviceClass.ParentId.HasValue)
            {
                serviceClassIdList.Add(serviceClass.ParentId.Value);
            }
            return Ok(serviceService.GetServicesByServiceClass(serviceClassIdList, date, page, PageSize, dateBefore));
        }

        /// <summary>
        /// Gets a list of services related to defined municipality.
        /// </summary>
        /// <param name="area"></param>
        /// <param name="code"></param>
        /// <param name="includeWholeCountry"></param>
        /// <param name="date"></param>
        /// <param name="page"></param>
        /// <param name="dateBefore"></param>
        /// <returns></returns>
        protected IActionResult GetGuidPageByArea(string area, string code, bool includeWholeCountry, [FromQuery]DateTime? date, [FromQuery]int page, [FromQuery]DateTime? dateBefore = null)
        {
            // check if municipality with given code exists
            if (area == AreaTypeEnum.Municipality.ToString())
            {
                var municipality = codeService.GetMunicipalityByCode(code, true);
                if (municipality == null || !municipality.Id.IsAssigned())
                {
                    return NotFound(new VmError { ErrorMessage = $"Municipality with code '{code}' not found." });
                }
                return Ok(serviceService.GetServicesByMunicipality(municipality.Id, includeWholeCountry, date, page, PageSize, dateBefore));
            }

            // Get services for certain area (not municipality). SFIPTV-843
            var areaId = codeService.GetAreaIdByCodeAndType(code, area);
            if (!areaId.IsAssigned())
            {
                return NotFound(new VmError { ErrorMessage = $"Area {area} with code '{code}' not found." });
            }
            // ReSharper disable once PossibleInvalidOperationException
            return Ok(serviceService.GetServicesByArea(areaId.Value, includeWholeCountry, date, page, PageSize, dateBefore));
        }

        /// <summary>
        /// Gets a list of published services for defined service class.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="date"></param>
        /// <param name="page"></param>
        /// <param name="dateBefore"></param>
        /// <returns></returns>
        protected IActionResult GetByTargetGroupBase(string uri, DateTime? date, int page = 1, DateTime? dateBefore = null)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                return BadRequest(new VmError { ErrorMessage = "Query string parameter uri is mandatory." });
            }
            var decodedUri = WebUtility.UrlDecode(uri);
            var targetGroups = fintoService.GetTargetGroupByUri(decodedUri);
            if (targetGroups == null || !targetGroups.Id.IsAssigned())
            {
                return NotFound(new VmError { ErrorMessage = $"Target group with uri '{uri}' not found." });
            }
            return Ok(serviceService.GetServicesByTargetGroup(targetGroups.Id, date, page, PageSize, dateBefore));
        }

        /// <summary>
        /// Gets a list of published services for defined service class.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="date"></param>
        /// <param name="page"></param>
        /// <param name="dateBefore"></param>
        /// <returns></returns>
        protected IActionResult GetByIndustrialClassBase(string uri, DateTime? date, int page = 1, DateTime? dateBefore = null)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                return BadRequest(new VmError { ErrorMessage = "Query string parameter uri is mandatory." });
            }
            var decodedUri = WebUtility.UrlDecode(uri);
            var test = fintoService.CheckIndustrialClasses(new List<string> { decodedUri });
            var industrialClass = fintoService.GetIndustrialClassByUri(decodedUri);
            if (industrialClass == null || !industrialClass.Id.IsAssigned())
            {
                return NotFound(new VmError { ErrorMessage = $"Industrial class with uri '{uri}' not found." });
            }
            return Ok(serviceService.GetServicesByIndustrialClass(industrialClass.Id, date, page, PageSize, dateBefore));
        }

        /// <summary>
        /// Get service by type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <param name="page"></param>
        /// <param name="dateBefore"></param>
        /// <returns></returns>
        protected IActionResult GetByType(string type, DateTime? date, int page, DateTime? dateBefore = null)
        {
            var serviceType = type.GetEnumValueByOpenApiEnumValue<ServiceTypeEnum>();
            return Ok(serviceService.GetServicesByType(serviceType, date, page, PageSize, dateBefore));
        }

        /// <summary>
        /// Post service base.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="attachProposedChannels"></param>
        /// <returns></returns>
        protected IActionResult Post(IVmOpenApiServiceInVersionBase request, bool attachProposedChannels = false)
        {
            var mgr = new AddServiceManager(request, versionNumber, ModelState, Logger, attachProposedChannels, serviceService, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, CommonService, channelService, OrganizationService, UserRole());
            return mgr.PerformAction();
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
            var mgr = new SaveServiceManager(id, sourceId, request, versionNumber, ModelState, Logger, attachProposedChannels, serviceService, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, CommonService, channelService, OrganizationService, UserRole());
            return mgr.PerformAction();
        }
    }
}
