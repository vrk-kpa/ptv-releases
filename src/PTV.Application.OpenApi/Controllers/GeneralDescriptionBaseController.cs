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
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Application.OpenApi.EntityManagers;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Base controller for general description
    /// </summary>
    public class GeneralDescriptionBaseController : BaseController
    {
        private readonly IGeneralDescriptionService generalDescriptionService;

        private int versionNumber;
        private ICodeService codeService;
        private IFintoService fintoService;
        private IOntologyTermDataCache ontologyTermDataCache;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="generalDescriptionService"></param>
        /// <param name="codeService"></param>
        /// <param name="settings"></param>
        /// <param name="fintoService"></param>
        /// <param name="ontologyTermDataCache"></param>
        /// <param name="logger"></param>
        /// <param name="userOrganizationService"></param>
        /// <param name="versionNumber"></param>
        public GeneralDescriptionBaseController(
            IGeneralDescriptionService generalDescriptionService,
            ICodeService codeService,
            IOptions<AppSettings> settings,
            IFintoService fintoService,
            IOntologyTermDataCache ontologyTermDataCache,
            ILogger logger,
            IUserOrganizationService userOrganizationService,
            int versionNumber) : base(userOrganizationService, settings, logger)
        {
            this.generalDescriptionService = generalDescriptionService;
            this.versionNumber = versionNumber;

            this.codeService = codeService;
            this.fintoService = fintoService;
            this.ontologyTermDataCache = ontologyTermDataCache;
        }

        /// <summary>
        /// General description service
        /// </summary>
        protected IGeneralDescriptionService GeneralDescriptionService { get { return generalDescriptionService; } private set { } }

        /// <summary>
        /// Get general description base.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="openApiVersion"></param>
        /// <returns></returns>
        protected IActionResult Get(string id, int openApiVersion = 0)
        {
            Guid guid = id.ParseToGuidWithExeption();
            var result = generalDescriptionService.GetGeneralDescriptionVersionBase(guid, openApiVersion != 0 ? openApiVersion : versionNumber);

            if (result == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"General description with id '{id}' not found." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Get general description by id list.
        /// </summary>
        /// <param name="guids"></param>
        /// <param name="getOnlyPublished"></param>
        /// <returns></returns>
        protected virtual IActionResult GetByIdList(string guids, bool getOnlyPublished = true)
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

            var scs = generalDescriptionService.GetGeneralDescriptions(guidList, versionNumber);

            if (scs == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"General descriptions not found." });
            }

            return Ok(scs);
        }

        /// <summary>
        /// Creates a new general description with the data provided as input.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected IActionResult Post(IVmOpenApiGeneralDescriptionInVersionBase request)
        {
            var mgr = new AddGeneralDescriptionManager(request, versionNumber, ModelState, Logger, GeneralDescriptionService, codeService, fintoService, ontologyTermDataCache);
            return mgr.PerformAction();
        }

        /// <summary>
        ///  Updates the defined general description with the data provided as input.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected IActionResult Put(string id, IVmOpenApiGeneralDescriptionInVersionBase request)
        {
            var mgr = new SaveGeneralDescriptionManager(id, request, versionNumber, ModelState, Logger, GeneralDescriptionService, codeService, fintoService, ontologyTermDataCache);
            return mgr.PerformAction();
        }
    }
}
