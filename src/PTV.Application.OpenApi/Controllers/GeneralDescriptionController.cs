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
using PTV.Application.OpenApi.Attributes;
using PTV.Application.OpenApi.Interfaces;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.OpenApi;
using Swashbuckle.SwaggerGen.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi.V2;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api general description related methods.
    /// </summary>
    [Route("api/v{version:range(2,2)}/[controller]")]
    [Route("api/[controller]")]
    public class GeneralDescriptionController : ValidationBaseController
    {
        private readonly IGeneralDescriptionService generalDescriptionService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="generalDescriptionService">instance of IGeneralDescriptionService</param>
        /// <param name="codeService">instance of ICodeService</param>
        /// <param name="fintoService">instance of IFintoService</param>
        /// <param name="settings">instance of IOptions{AppSettings}</param>
        public GeneralDescriptionController(
            IGeneralDescriptionService generalDescriptionService,
            ICodeService codeService,
            IFintoService fintoService,
            IOptions<AppSettings> settings) : base(null, codeService, settings, fintoService)
        {
            this.generalDescriptionService = generalDescriptionService;
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
        [HttpGet]
        [ValidateDate]
        [ProducesResponseType(typeof(VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "A list of general description ids with paging.", typeof (VmOpenApiGuidPage))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public IActionResult Get([FromQuery] DateTime? date, [FromQuery] int page = 1)
        {
            var pageSize = Settings.PageSize > 0 ? Settings.PageSize : 1000;
            return Ok(generalDescriptionService.GetGeneralDescriptions(date, page, pageSize));
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
        [HttpGet("/api/[controller]/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(VmOpenApiGeneralDescription), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "Detailed information about a statutory service general description.", typeof (VmOpenApiGeneralDescription))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof (Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.NotFound, CoreMessages.OpenApi.NotFoundGeneralMessage, typeof (IVmError))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof (IVmError))]
        public IActionResult Get(string id)
        {
            Guid guid = id.ParseToGuidWithExeption();
            var result = generalDescriptionService.GetGeneralDescription(guid);

            if (result == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"General description with id '{id}' not found." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Fetches all the information related to a single statutory service general description.
        /// </summary>
        /// <param name="id">Statutory service general description guid (id of the entity) to fetch.</param>
        /// <param name="version">The open api version.</param>
        /// <returns>Detailed information about a statutory service general description.</returns>
        // GET api/values/5
        [HttpGet("/api/v2/[controller]/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V2VmOpenApiGeneralDescription), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "Detailed information about a statutory service general description.", typeof(V2VmOpenApiGeneralDescription))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.NotFound, CoreMessages.OpenApi.NotFoundGeneralMessage, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public IActionResult V2Get(string id)
        {
            Guid guid = id.ParseToGuidWithExeption();
            var result = generalDescriptionService.V2GetGeneralDescription(guid);

            if (result == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"General description with id '{id}' not found." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Creates a new general description with the data provided as input.
        /// </summary>
        /// <param name="request">The general description data.</param>
        /// <returns>The created general description.</returns>
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
        // POST api/generalDescription
        //[Authorize(ActiveAuthenticationSchemes = "Bearer")]
        //[HttpPost("~/api/v2/[controller]")]
        //[ProducesResponseType(typeof(V2VmOpenApiGeneralDescription), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        //[SwaggerResponse(HttpStatusCode.OK, "The created general description.", typeof (V2VmOpenApiGeneralDescription))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        //public IActionResult Post([FromBody] VmOpenApiGeneralDescriptionIn request)
        //{
        //    if (request == null)
        //    {
        //        ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
        //        return new BadRequestObjectResult(ModelState);
        //    }

        //    // Validate the items
        //    if (!ModelState.IsValid)
        //    {
        //        return new BadRequestObjectResult(ModelState);
        //    }

        //    // Check the item values from db and validate
        //    ValidateParameters(request);
        //    if (!ModelState.IsValid)
        //    {
        //        return new BadRequestObjectResult(ModelState);
        //    }

        //    var result = generalDescriptionService.AddGeneralDescription(request, Settings.AllowAnonymous);
        //    return Ok(result);
        //}

        private void ValidateParameters(IVmOpenApiGeneralDescriptionIn request)
        {
            // Validate languages
            ValidateLanguageList(request.Languages);

            // Validate Finto items
            ValidateServiceClasses(request.ServiceClasses);
            ValidateOntologyTerms(request.OntologyTerms);
            ValidateTargetGroups(request.TargetGroups);
            ValidateLifeEvents(request.LifeEvents);
            ValidateIndustrialClasses(request.IndustrialClasses);
        }
    }
}
