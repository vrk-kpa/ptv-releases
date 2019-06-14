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


using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.DataValidators;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PTV.Application.OpenApi.EntityManagers;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Service channel base controller
    /// </summary>
    /// <seealso cref="PTV.Application.OpenApi.Controllers.BaseController" />
    public class ServiceChannelBaseController : BaseController
    {
        private IChannelService channelService;
        private IOrganizationService organizationService;
        private ICodeService codeService;
        private IServiceService serviceService;
        private ICommonService commonService;
        private int versionNumber;

        /// <summary>
        /// Gets the ChannelService instance.
        /// </summary>
        protected IChannelService ChannelService { get { return channelService; } private set { } }

        /// <summary>
        /// ServiceChannelController constructor.
        /// </summary>
        public ServiceChannelBaseController(
            IChannelService channelService,
            IOrganizationService organizationService,
            ICodeService codeService,
            IServiceService serviceService,
            IOptions<AppSettings> settings,
            ILogger logger,
            ICommonService commonService,
            int versionNumber,
            IUserOrganizationService userOrganizationService)
            : base(userOrganizationService, settings, logger)
        {
            this.channelService = channelService;
            this.organizationService = organizationService;
            this.codeService = codeService;
            this.serviceService = serviceService;
            this.versionNumber = versionNumber;
            this.commonService = commonService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dateBefore"></param>
        /// <param name="organizationId"></param>
        /// <param name="code"></param>
        /// <param name="oid"></param>
        /// <param name="page"></param>
        /// <param name="status"></param>
        protected IActionResult GetInternal([FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, string organizationId = null, string code = null, string oid = null, [FromQuery]int page = 1, [FromQuery]string status = "Published")
        {
            if (page < 0)
            {
                ModelState.AddModelError("page", "The page number cannot be negative value.");
                return BadRequest(ModelState);
            }

            var entityStatus = status.ConvertToEnum<EntityStatusExtendedEnum>();
            if (!entityStatus.HasValue)
            {
                ModelState.AddModelError("status", "The status is invalid.");
                return BadRequest(ModelState);
            }

            // Only one of the organizatio parameters can be set at a time.
            if ((!organizationId.IsNullOrEmpty() && !code.IsNullOrEmpty()) || (!organizationId.IsNullOrEmpty() && !oid.IsNullOrEmpty()) || (!code.IsNullOrEmpty() && !oid.IsNullOrEmpty()))
            {
                var parameterName = organizationId == null ? "code" : "organizationId";
                ModelState.AddModelError(parameterName, "Only one of the organization related parameters 'organizationId', 'code' or 'oid' can be set at a time.");
                return BadRequest(ModelState);
            }

            // Check that the set organization exists
            List<Guid> organizationGuids = new List<Guid>();
            if (!organizationId.IsNullOrEmpty())
            {
                var guid = organizationId.ParseToGuidWithExeption();

                // check if the organization exists with the given id
                if (!commonService.OrganizationExists(guid, PublishingStatus.Published))
                {
                    return NotFound(new VmError() { ErrorMessage = $"Organization with id '{guid}' not found." });
                }

                organizationGuids.Add(guid);
            }
            else if (!code.IsNullOrEmpty())
            {
                var guids = organizationService.GetOrganizationIdsByBusinessCode(code);
                if (guids?.Count > 0)
                {
                    organizationGuids.AddRange(guids);
                }
                else
                {
                    return NotFound(new VmError() { ErrorMessage = $"Organization with business code '{code}' not found." });
                }
            }
            else if (!oid.IsNullOrEmpty())
            {
                var guid = organizationService.GetOrganizationIdByOid(oid, true);
                if (guid == null || guid == Guid.Empty)
                {
                    return NotFound(new VmError() { ErrorMessage = $"Organization with oid '{oid}' not found." });
                }
                organizationGuids.Add(guid);
            }

            return Ok(ChannelService.GetServiceChannels(date, page, PageSize, entityStatus.Value, dateBefore, organizationGuids));
        }

        /// <summary>
        /// Get service channel base.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="getOnlyPublished"></param>
        /// <returns></returns>
        protected virtual IActionResult GetById(string id, bool getOnlyPublished = true)
        {
            Guid guid = id.ParseToGuidWithExeption();

            var sc = ChannelService.GetServiceChannelById(guid, versionNumber, getOnlyPublished ? VersionStatusEnum.Published : VersionStatusEnum.LatestActive);

            if (sc == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Service channel with id '{id}' not found." });
            }

            return Ok(sc);
        }

        /// <summary>
        /// Get service channel base.
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        protected virtual IActionResult GetByIdList(string guids)
        {
            if (string.IsNullOrEmpty(guids))
            {
                ModelState.AddModelError("guids", "Property guids is required.");
                return new BadRequestObjectResult(ModelState);
            }
            try
            {
                var guidList = guids.Split(',').Select(id => id.Trim().ParseToGuidWithExeption()).ToList();
                var scs = ChannelService.GetServiceChannels(guidList, versionNumber);

                if (scs == null)
                {
                    return NotFound(new VmError() { ErrorMessage = $"Service channels not found." });
                }

                return Ok(scs);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("guids", ex.Message);
                return new BadRequestObjectResult(ModelState);
            }
        }

        /// <summary>
        /// Get service channel by type base.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <param name="page"></param>
        /// <param name="getOnlyPublished"></param>
        /// <param name="dateBefore"></param>
        /// <returns></returns>
        protected virtual IActionResult GetGuidPageByType(string type, DateTime? date, int page, bool getOnlyPublished = true, DateTime? dateBefore = null)
        {
            var channelType = type.Parse<ServiceChannelTypeEnum>();
            return Ok(ChannelService.GetServiceChannelsByType(channelType, date, page, PageSize, getOnlyPublished, dateBefore));
        }

        /// <summary>
        /// Get service channel by organization and type base.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <param name="page"></param>
        /// <param name="dateBefore"></param>
        /// <returns></returns>
        protected virtual IActionResult GetGuidPageByOrganizationIdAndType(string organizationId, string type, DateTime? date, int page, DateTime? dateBefore = null)
        {
            Guid guid = organizationId.ParseToGuidWithExeption();

            // check if the organization exists with the given id
            if (!commonService.OrganizationExists(guid, PublishingStatus.Published))
            {
                return NotFound(new VmError() { ErrorMessage = $"Organization with id '{guid}' not found." });
            }

            ServiceChannelTypeEnum? channelType = null;
            if (!string.IsNullOrEmpty(type)) { channelType = type.Parse<ServiceChannelTypeEnum>(); }
            return Ok(ChannelService.GetServiceChannelsByOrganization(guid, date, page, PageSize, channelType, dateBefore));
        }

        /// <summary>
        /// Gets a list of service channels related to defined municipality.
        /// </summary>
        /// <param name="area"></param>
        /// <param name="code"></param>
        /// <param name="includeWholeCountry"></param>
        /// <param name="date"></param>
        /// <param name="page"></param>
        /// <param name="dateBefore"></param>
        /// <returns></returns>
        protected IActionResult GetGuidPageByArea(string area, string code, bool includeWholeCountry, DateTime? date, int page,DateTime? dateBefore = null)
        {
            // check if municipality with given code exists
            if (area == AreaTypeEnum.Municipality.ToString())
            {
                var municipality = codeService.GetMunicipalityByCode(code, true);
                if (municipality == null || !municipality.Id.IsAssigned())
                {
                    return NotFound(new VmError() { ErrorMessage = $"Municipality with code '{code}' not found." });
                }
                return Ok(ChannelService.GetServiceChannelsByMunicipality(municipality.Id, includeWholeCountry, date, page, PageSize, dateBefore));
            }
            else
            {
                // Get services for certain area (not municipality). SFIPTV-843
                var areaId = codeService.GetAreaIdByCodeAndType(code, area);
                if (!areaId.IsAssigned())
                {
                    return NotFound(new VmError() { ErrorMessage = $"Area {area} with code '{code}' not found." });
                }
                return Ok(ChannelService.GetServiceChannelsByArea(areaId.Value, includeWholeCountry, date, page, PageSize, dateBefore));
            }
        }

        /// <summary>
        /// Post electronic channel base.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected IActionResult PostEChannel(IVmOpenApiElectronicChannelInVersionBase request)
        {
            var mgr = new AddEChannelManager(request, versionNumber, ModelState, Logger, channelService, organizationService, codeService, serviceService);
            return mgr.PerformAction();
        }

        /// <summary>
        /// Puts the e channel.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutEChannel(string id, IVmOpenApiElectronicChannelInVersionBase request)
        {
            var mgr = new SaveEChannelManager(id, null, request, versionNumber, ModelState, Logger, channelService, organizationService, codeService, serviceService, UserRole());
            return mgr.PerformAction();
        }

        /// <summary>
        /// Puts the e channel by source.
        /// </summary>
        /// <param name="sourceId">The source identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutEChannelBySource(string sourceId, IVmOpenApiElectronicChannelInVersionBase request)
        {
            var mgr = new SaveEChannelManager(null, sourceId, request, versionNumber, ModelState, Logger, channelService, organizationService, codeService, serviceService, UserRole());
            return mgr.PerformAction();
        }

        /// <summary>
        /// Posts the phone channel base.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PostPhoneChannel(IVmOpenApiPhoneChannelInVersionBase request)
        {
            var mgr = new AddPhoneChannelManager(request, versionNumber, ModelState, Logger, channelService, organizationService, codeService, serviceService);
            return mgr.PerformAction();
        }

        /// <summary>
        /// Puts the phone channel base.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutPhoneChannel(string id, IVmOpenApiPhoneChannelInVersionBase request)
        {
            var mgr = new SavePhoneChannelManager(id, null, request, versionNumber, ModelState, Logger, channelService, organizationService, codeService, serviceService, UserRole());
            return mgr.PerformAction();
        }

        /// <summary>
        /// Puts the phone channel by source.
        /// </summary>
        /// <param name="sourceId">The source identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutPhoneChannelBySource(string sourceId, IVmOpenApiPhoneChannelInVersionBase request)
        {
            var mgr = new SavePhoneChannelManager(null, sourceId, request, versionNumber, ModelState, Logger, channelService, organizationService, codeService, serviceService, UserRole());
            return mgr.PerformAction();
        }

        /// <summary>
        /// Posts the web page channel.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PostWebPageChannel(IVmOpenApiWebPageChannelInVersionBase request)
        {
            var mgr = new AddWebPageChannelManager(request, versionNumber, ModelState, Logger, channelService, organizationService, codeService, serviceService);
            return mgr.PerformAction();
        }

        /// <summary>
        /// Puts the web page channel.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutWebPageChannel(string id, IVmOpenApiWebPageChannelInVersionBase request)
        {
            var mgr = new SaveWebPageChannelManager(id, null, request, versionNumber, ModelState, Logger, channelService, organizationService, codeService, serviceService, UserRole());
            return mgr.PerformAction();
        }

        /// <summary>
        /// Puts the web page channel by source.
        /// </summary>
        /// <param name="sourceId">The source identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutWebPageChannelBySource(string sourceId, IVmOpenApiWebPageChannelInVersionBase request)
        {
            var mgr = new SaveWebPageChannelManager(null, sourceId, request, versionNumber, ModelState, Logger, channelService, organizationService, codeService, serviceService, UserRole());
            return mgr.PerformAction();
        }

        /// <summary>
        /// Posts the printable form channel.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PostPrintableFormChannel(IVmOpenApiPrintableFormChannelInVersionBase request)
        {
            var mgr = new AddPrintableFormChannelManager(request, versionNumber, ModelState, Logger, channelService, organizationService, codeService, serviceService);
            return mgr.PerformAction();
        }

        /// <summary>
        /// Puts the printable form channel.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutPrintableFormChannel(string id, IVmOpenApiPrintableFormChannelInVersionBase request)
        {
            var mgr = new SavePrintableFormChannelManager(id, null, request, versionNumber, ModelState, Logger, channelService, organizationService, codeService, serviceService, UserRole());
            return mgr.PerformAction();
        }

        /// <summary>
        /// Puts the printable form channel by source.
        /// </summary>
        /// <param name="sourceId">The source identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutPrintableFormChannelBySource(string sourceId, IVmOpenApiPrintableFormChannelInVersionBase request)
        {
            var mgr = new SavePrintableFormChannelManager(null, sourceId, request, versionNumber, ModelState, Logger, channelService, organizationService, codeService, serviceService, UserRole());
            return mgr.PerformAction();
        }

        /// <summary>
        /// Posts the service location channel.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PostServiceLocationChannel(IVmOpenApiServiceLocationChannelInVersionBase request)
        {
            var mgr = new AddServiceLocationChannelManager(request, versionNumber, ModelState, Logger, channelService, organizationService, codeService, serviceService);
            return mgr.PerformAction();
        }

        /// <summary>
        /// Puts the service location channel.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutServiceLocationChannel(string id, IVmOpenApiServiceLocationChannelInVersionBase request)
        {
            var mgr = new SaveServiceLocationChannelManager(id, null, request, versionNumber, ModelState, Logger, channelService, organizationService, codeService, serviceService, UserRole());
            return mgr.PerformAction();
        }

        /// <summary>
        /// Puts the service location channel by source.
        /// </summary>
        /// <param name="sourceId">The source identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutServiceLocationChannelBySource(string sourceId, IVmOpenApiServiceLocationChannelInVersionBase request)
        {
            var mgr = new SaveServiceLocationChannelManager(null, sourceId, request, versionNumber, ModelState, Logger, channelService, organizationService, codeService, serviceService, UserRole());
            return mgr.PerformAction();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hours"></param>
        protected void ValidateServiceHours(IList<V4VmOpenApiServiceHour> hours)
        {
            if (hours == null || hours.Count == 0) return;

            // Need to validate service hours for older version related isExtra property. PTV-3875
            var hoursValidator = new ServiceHoursIsExtraValidator(hours);
            hoursValidator.Validate(ModelState);                
        }
    }
}
