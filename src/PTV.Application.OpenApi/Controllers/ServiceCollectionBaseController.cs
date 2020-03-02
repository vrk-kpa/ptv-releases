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
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.EntityManagers;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Service collection base controller
    /// </summary>
    /// <seealso cref="PTV.Application.OpenApi.Controllers.BaseController" />
    public class ServiceCollectionBaseController : EntityWithOrganizationBaseController
    {
        private IServiceCollectionService serviceCollectionService;
        private IServiceService serviceService;
        private IUserOrganizationService userOrganizationService;
        private IOrganizationService organizationService;

        private int versionNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCollectionBaseController"/> class.
        /// </summary>
        /// <param name="serviceCollectionService">The service collection service.</param>
        /// <param name="organizationService">The organization service</param>
        /// <param name="commonService">The common service.</param>
        /// <param name="serviceService">The service service.</param>
        /// <param name="userOrganizationService">The user organization service</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="versionNumber">Open api version.</param>
        public ServiceCollectionBaseController(
            IServiceCollectionService serviceCollectionService,
            IOrganizationService organizationService,
            ICommonService commonService,
            IServiceService serviceService,
            IUserOrganizationService userOrganizationService,
            IOptions<AppSettings> settings,
            ILogger logger,
            int versionNumber)
            : base(organizationService, commonService, userOrganizationService, settings, logger)
        {
            this.serviceCollectionService = serviceCollectionService;
            this.serviceService = serviceService;
            this.userOrganizationService = userOrganizationService;
            this.versionNumber = versionNumber;
            this.organizationService = organizationService;
        }

        /// <summary>
        /// Gets the identifier and name list.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="page">The page.</param>
        /// <param name="archived">Indicates if archived items should be returned.</param>
        /// <param name="dateBefore"></param>
        /// <returns>Service collection ids and names list</returns>
        protected IActionResult GetIdAndNameList(DateTime? date, int page, bool archived = false, DateTime? dateBefore = null)
        {
            return Ok(serviceCollectionService.GetServiceCollections(date, page, PageSize, archived, dateBefore));
        }

        /// <summary>
        /// Get service collection by id base
        /// </summary>
        /// <param name="id"></param>
        /// <param name="getOnlyPublished"></param>
        /// <returns></returns>
        protected IActionResult GetById(string id, bool getOnlyPublished = true)
        {
            var guid = id.ParseToGuidWithExeption();

            var srv = serviceCollectionService.GetServiceCollectionById(guid, versionNumber, getOnlyPublished);

            if (srv == null)
            {
                return NotFound(new VmError { ErrorMessage = $"Service collection with id '{id}' not found." });
            }

            return Ok(srv);
        }

        /// <summary>
        /// Get service collection by organization id or business code base.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="code"></param>
        /// <param name="oid"></param>
        /// <param name="page"></param>
        /// <param name="fillWithAllGdData"></param>
        /// <returns></returns>
        protected IActionResult GetByOrganizationBase(string organizationId = null, string code = null, int page = 1)
        {
            if (page < 0)
            {
                ModelState.AddModelError("page", "The page number cannot be negative value.");
                return BadRequest(ModelState);
            }

            // Check that the organization exists
            var organizationResult = CheckAndGetOrganizationIds(organizationId, code, null, true);
            if (organizationResult.Item1 != null)
            {
                return organizationResult.Item1;
            }

            if (organizationResult.Item2?.Count > 0)
            {
                return Ok(serviceCollectionService.GetServiceCollectionsByOrganization(organizationResult.Item2, page));
            }

            return NotFound(new VmError { ErrorMessage = "Services not found." });
        }

        /// <summary>
        ///Post service collection base.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected IActionResult Post(IVmOpenApiServiceCollectionInVersionBase request)
        {
            var mgr = new AddServiceCollectionManager(request, versionNumber, ModelState, Logger, serviceCollectionService, serviceService, organizationService);
            return mgr.PerformAction();
        }

        /// <summary>
        /// Put service collection base.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        protected IActionResult Put(IVmOpenApiServiceCollectionInVersionBase request, string id = null, string sourceId = null)
        {
            var mgr = new SaveServiceCollectionManager(id, sourceId, request, versionNumber, ModelState, Logger, serviceCollectionService, serviceService, organizationService);
            return mgr.PerformAction();
        }

        /// <summary>
        /// Get all user organizations
        /// </summary>
        /// <returns></returns>
        protected List<Guid> UserOrganizations()
        {
            return userOrganizationService.GetAllUserOrganizationIds().ToList();
        }
    }
}
