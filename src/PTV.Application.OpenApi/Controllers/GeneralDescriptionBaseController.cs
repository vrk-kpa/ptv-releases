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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Base controller for general description
    /// </summary>
    public class GeneralDescriptionBaseController : BaseController
    {
        private readonly IGeneralDescriptionService generalDescriptionService;

        private int pageSize;
        private int versionNumber;
        private ICodeService codeService;
        private IFintoService fintoService;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="generalDescriptionService"></param>
        /// <param name="codeService"></param>
        /// <param name="settings"></param>
        /// <param name="fintoService"></param>
        /// <param name="logger"></param>
        /// <param name="userOrganizationService"></param>
        /// <param name="versionNumber"></param>
        public GeneralDescriptionBaseController(
            IGeneralDescriptionService generalDescriptionService,
            ICodeService codeService,
            IOptions<AppSettings> settings,
            IFintoService fintoService,
            ILogger logger,
            IUserOrganizationService userOrganizationService,
            int versionNumber) : base(userOrganizationService, settings, logger)
        {
            this.generalDescriptionService = generalDescriptionService;
            pageSize = Settings.PageSize > 0 ? Settings.PageSize : 1000;
            this.versionNumber = versionNumber;

            this.codeService = codeService;
            this.fintoService = fintoService;
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
        /// Creates a new general description with the data provided as input.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected IActionResult Post(IVmOpenApiGeneralDescriptionInVersionBase request)
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
            ValidateParameters(request);
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var result = GeneralDescriptionService.AddGeneralDescription(request, Settings.AllowAnonymous, versionNumber);
            return Ok(result);
        }

        /// <summary>
        ///  Updates the defined general description with the data provided as input.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected IActionResult Put(string id, IVmOpenApiGeneralDescriptionInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            if (!string.IsNullOrEmpty(id))
            {
                request.Id = id.ParseToGuid();

                // check that general description exists
                if (!request.Id.HasValue || !GeneralDescriptionService.GeneralDescriptionExists(request.Id.Value))
                {
                    return NotFound(new VmError() { ErrorMessage = $"General description with id '{id}' not found." });
                }
            }
            else
            {
                return NotFound(new VmError() { ErrorMessage = $"General description with id '{id}' not found." });
            }

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            // get the base model for service
            request = request.VersionBase();

            // Check the item values from db and validate
            ValidateParameters(request, false);
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var result = GeneralDescriptionService.SaveGeneralDescription(request, versionNumber);
            return Ok(result);
        }

        /// <summary>
        /// Validates general description model.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="createOperation"></param>
        protected void ValidateParameters(IVmOpenApiGeneralDescriptionInVersionBase request, bool createOperation = true)
        {
            IList<string> newLanguages = new List<string>();
            if (createOperation)
            {
                newLanguages = request.AvailableLanguages;
            }
            else
            {
                var currentVersion = generalDescriptionService.GetGeneralDescriptionVersionBase(request.Id.Value, 0, false);
                if (currentVersion == null || string.IsNullOrEmpty(currentVersion.PublishingStatus))
                {
                    this.ModelState.AddModelError("General description id", $"Version for service with id '{request.Id.Value}' not found.");                
                }
                else
                {
                    request.CurrentPublishingStatus = currentVersion.PublishingStatus;
                    // Get the available languages from current version
                    // Check if user has added new language versions. New available languages and data need to be validated (required fields need to exist in request).
                    newLanguages = request.AvailableLanguages.Where(i => !currentVersion.AvailableLanguages.Contains(i)).ToList();
                    request.CurrentVersionId = currentVersion.CurrentVersionId;
                }
                
            }
            
            GeneralDescriptionValidator service = new GeneralDescriptionValidator(request, codeService, fintoService, newLanguages, createOperation);
            service.Validate(this.ModelState);
        }
    }
}
