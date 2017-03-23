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
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Net;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authorization;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Models.OpenApi.V1;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api general description related methods.
    /// </summary>
    [Route("api/v2/[controller]")]
    [Route("api/[controller]")]
    public class GeneralDescriptionController : GeneralDescriptionBaseController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="generalDescriptionService">instance of IGeneralDescriptionService</param>
        /// <param name="codeService">instance of ICodeService</param>
        /// <param name="fintoService">instance of IFintoService</param>
        /// <param name="logger">instance of Ilogger</param>
        /// <param name="settings">instance of IOptions{AppSettings}</param>
        public GeneralDescriptionController(
            IGeneralDescriptionService generalDescriptionService,
            ICodeService codeService,
            IFintoService fintoService,
            ILogger<GeneralDescriptionController> logger,
            IOptions<AppSettings> settings) : base(generalDescriptionService, codeService, settings, fintoService, logger, 1)
        {
        }

        /// <summary>
        /// Gets all the statutory service general descriptions within PTV as a list of ids.
        /// Descriptions created/modified after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss"</param>
        /// <param name="page">The page to be fetched. Page numbering starts from one.</param>
        /// <param name="version">The open api version.</param>
        /// <returns>A list of general description ids with paging.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages. Below sample response.</para>
        /// <code>
        /// {
        ///     "Names": [
        ///         "Type - Required value 'Name' was not found!"
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        // GET: api/values
        [AllowAnonymous]
        [HttpGet]
        [ValidateDate]
        [ProducesResponseType(typeof(VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of general description ids with paging.", type: typeof(VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Get([FromQuery] DateTime? date, [FromQuery] int page = 1)
        {
            var pageSize = Settings.PageSize > 0 ? Settings.PageSize : 1000;
            return Ok(GeneralDescriptionService.GetGeneralDescriptions(date, page, pageSize));
        }

        /// <summary>
        /// Fetches all the information related to a single statutory service general description.
        /// </summary>
        /// <param name="id">Statutory service general description guid (id of the entity) to fetch.</param>
        /// <param name="version">The open api version.</param>
        /// <returns>Detailed information about a statutory service general description.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages. Below sample response.</para>
        /// <code>
        /// {
        ///     "Names": [
        ///         "Type - Required value 'Name' was not found!"
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        // GET api/values/5
        [AllowAnonymous]
        [HttpGet("/api/[controller]/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(VmOpenApiGeneralDescription), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about a statutory service general description.", type: typeof(VmOpenApiGeneralDescription))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Get(string id)
        {
            return base.Get(id);
        }

        /// <summary>
        /// Fetches all the information related to a single statutory service general description.
        /// </summary>
        /// <param name="id">Statutory service general description guid (id of the entity) to fetch.</param>
        /// <param name="version">The open api version.</param>
        /// <returns>Detailed information about a statutory service general description.</returns>
        // GET api/values/5
        [AllowAnonymous]
        [HttpGet("/api/v2/[controller]/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V2VmOpenApiGeneralDescription), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about a statutory service general description.", type: typeof(V2VmOpenApiGeneralDescription))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult V2Get(string id)
        {
            return base.Get(id, 2);
        }
    }
}
