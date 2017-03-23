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
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using System;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Base controller for general description
    /// </summary>
    public class GeneralDescriptionBaseController : ValidationBaseController
    {
        private readonly IGeneralDescriptionService generalDescriptionService;

        private int pageSize;
        private int versionNumber;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="generalDescriptionService"></param>
        /// <param name="codeService"></param>
        /// <param name="settings"></param>
        /// <param name="fintoService"></param>
        /// <param name="logger"></param>
        /// <param name="versionNumber"></param>
        public GeneralDescriptionBaseController(
            IGeneralDescriptionService generalDescriptionService,
            ICodeService codeService,
            IOptions<AppSettings> settings,
            IFintoService fintoService,
            ILogger logger,
            int versionNumber) :
            base(null, codeService, settings, fintoService, logger)
        {
            this.generalDescriptionService = generalDescriptionService;
            pageSize = Settings.PageSize > 0 ? Settings.PageSize : 1000;
            this.versionNumber = versionNumber;
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
    }
}
