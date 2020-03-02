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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.EntityManagers;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Organization base controller
    /// </summary>
    /// <seealso cref="PTV.Application.OpenApi.Controllers.BaseController" />
    public class OrganizationBaseController : BaseController
    {
        private readonly ICodeService codeService;
        private readonly ICommonService commonService;
        private readonly IUserOrganizationService userService;

        /// <summary>
        /// The open api version number.
        /// </summary>
        protected int VersionNumber;

        /// <summary>
        /// Organization service
        /// </summary>
        protected IOrganizationService OrganizationService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationBaseController"/> class.
        /// </summary>
        /// <param name="organizationService">The organization service.</param>
        /// <param name="codeService">The code service.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="versionNumber">The version number.</param>
        /// <param name="commonService">The Common service.</param>
        /// <param name="userService">The user info service.</param>
        public OrganizationBaseController(
            IOrganizationService organizationService,
            ICodeService codeService,
            IOptions<AppSettings> settings,
            ILogger logger,
            ICommonService commonService,
            IUserOrganizationService userService,
            int versionNumber)
            : base(userService, settings, logger)
        {
            this.VersionNumber = versionNumber;
            OrganizationService = organizationService;
            this.codeService = codeService;
            this.commonService = commonService;
            this.userService = userService;
        }

        /// <summary>
        /// Get organization by id base.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected IActionResult GetById(string id)
        {
            var guid = id.ParseToGuidWithExeption();
            var org = OrganizationService.GetOrganizationById(guid, VersionNumber);

            if (org == null)
            {
                return NotFound(new VmError { ErrorMessage = $"Organization with id '{id}' not found." });
            }

            return Ok(org);
        }

        /// <summary>
        /// Get organization by id list base.
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        protected virtual IActionResult GetByIdListBase(string guids)
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

            var scs = OrganizationService.GetOrganizations(guidList, VersionNumber);

            if (scs == null)
            {
                return NotFound(new VmError { ErrorMessage = "Organizations not found." });
            }

            return Ok(scs);
        }

        /// <summary>
        /// Get organization by id base.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected IActionResult GetSahaById(string id)
        {
            var guid = id.ParseToGuidWithExeption();
            var org = OrganizationService.GetOrganizationSahaById(guid);

            if (org == null)
            {
                return NotFound(new VmError { ErrorMessage = $"Organization with id '{id}' not found." });
            }

            return Ok(org);
        }

        /// <summary>
        /// Gets the by y (business) code base.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>organization</returns>
        protected IActionResult GetByYCodeBase(string code)
        {
            // GET api/organization/businessCode/1234567-8

            var org = OrganizationService.GetOrganizationsByBusinessCode(code, VersionNumber);

            if (org == null || org.Count == 0)
            {
                return NotFound(new VmError { ErrorMessage = $"Organizations with code '{code}' not found." });
            }

            return Ok(org);
        }
        /// <summary>
        /// Gets the organization by oid base.
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <returns>organization</returns>
        protected IActionResult GetByOidBase(string oid)
        {
            var org = OrganizationService.GetOrganizationByOid(oid, VersionNumber);

            if (org == null)
            {
                return NotFound(new VmError { ErrorMessage = $"Organization with OID '{oid}' not found." });
            }

            return Ok(org);
        }

        /// <summary>
        /// Gets a list of organizations related to defined municipality.
        /// </summary>
        /// <param name="area"></param>
        /// <param name="code"></param>
        /// <param name="includeWholeCountry"></param>
        /// <param name="date"></param>
        /// <param name="dateBefore"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        protected IActionResult GetGuidPageByArea(string area, string code, bool includeWholeCountry, DateTime? date, int page, DateTime? dateBefore = null)
        {
            // check if municipality with given code exists
            if (area == AreaTypeEnum.Municipality.ToString())
            {
                var municipality = codeService.GetMunicipalityByCode(code, true);
                if (municipality == null || !municipality.Id.IsAssigned())
                {
                    return NotFound(new VmError { ErrorMessage = $"Municipality with code '{code}' not found." });
                }
                return Ok(OrganizationService.GetOrganizationsByMunicipality(municipality.Id, includeWholeCountry, date, page, PageSize, dateBefore));
            }

            // Get organizations for certain area (not municipality). SFIPTV-822
            var areaId = codeService.GetAreaIdByCodeAndType(code, area);
            if (!areaId.IsAssigned())
            {
                return NotFound(new VmError { ErrorMessage = $"Area {area} with code '{code}' not found." });
            }
            // ReSharper disable once PossibleInvalidOperationException
            return Ok(OrganizationService.GetOrganizationsByArea(areaId.Value, includeWholeCountry, date, page, PageSize, dateBefore));
        }

        /// <summary>
        /// Gets a list of organizations related to defined municipality.
        /// </summary>
        /// <param name="area"></param>
        /// <param name="code"></param>
        /// <param name="includeWholeCountry"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        protected IActionResult GetOrganizationsByArea(string area, string code, bool includeWholeCountry, int page)
        {
            // check if municipality with given code exists
            if (area == AreaTypeEnum.Municipality.ToString())
            {
                var municipality = codeService.GetMunicipalityByCode(code, true);
                if (municipality == null || !municipality.Id.IsAssigned())
                {
                    return NotFound(new VmError { ErrorMessage = $"Municipality with code '{code}' not found." });
                }
                return Ok(OrganizationService.GetOrganizationsWithAllDataByMunicipality(municipality.Id, includeWholeCountry, page));
            }

            // Get organizations for certain area (not municipality). SFIPTV-822
            var areaId = codeService.GetAreaIdByCodeAndType(code, area);
            if (!areaId.IsAssigned())
            {
                return NotFound(new VmError { ErrorMessage = $"Area {area} with code '{code}' not found." });
            }
            // ReSharper disable once PossibleInvalidOperationException
            return Ok(OrganizationService.GetOrganizationsWithAllDataByArea(areaId.Value, includeWholeCountry, page));
        }


        /// <summary>
        /// Save organization.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult Post(IVmOpenApiOrganizationInVersionBase request)
        {
            var mgr = new AddOrganizationManager(request, VersionNumber, ModelState, Logger, OrganizationService, codeService, commonService, userService);
            return mgr.PerformAction();
        }

        /// <summary>
        /// Change the organization.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="sourceId">The source identifier.</param>
        /// <returns></returns>
        protected IActionResult Put(IVmOpenApiOrganizationInVersionBase request, string id = null, string sourceId = null)
        {
            var mgr = new SaveOrganizationManager(id, sourceId, request, VersionNumber, ModelState, Logger, OrganizationService, codeService, commonService);
            return mgr.PerformAction();
        }
    }
}
