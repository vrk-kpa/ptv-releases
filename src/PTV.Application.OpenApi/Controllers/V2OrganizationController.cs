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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.Attributes;
using PTV.Application.OpenApi.Interfaces;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using Swashbuckle.SwaggerGen.Annotations;
using System;
using System.Collections.Generic;
using System.Net;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api Organization related methods. Version 2
    /// </summary>
    [Route("api/v2/Organization")]
    public class V2OrganizationController : OrganizationBaseController
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public V2OrganizationController(IOrganizationService organizationService, ICodeService codeService, IOptions<AppSettings> settings, IFintoService fintoService)
            : base(organizationService, codeService, settings, fintoService)
        {
        }

        /// <summary>
        /// Gets all the published organizations within PTV as a list of organization ids.
        /// Organizations created/modified after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss"</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <param name="version">The open api version.</param>
        /// <returns>A list of organization ids with paging.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages. Below sample response.</para>
        /// <code>
        /// {
        ///    "date": [
        ///      "The value '-2' is not valid for Nullable`1.",
        ///      "The date parameter is invalid."
        ///    ]
        /// }
        /// </code>
        /// </remarks>
        // GET: api/organization
        [HttpGet]
        [ValidateDate]
        [ProducesResponseType(typeof(VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "A list of organization ids with paging.", typeof(VmOpenApiGuidPage))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Invalid version number.")] // can be caused by using wrong version number
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public new IActionResult Get([FromQuery]DateTime? date, [FromQuery]int page = 1)
        {
            return base.Get(date, page);
        }


        /// <summary>
        /// Fetches all the information related to a single organization.
        /// </summary>
        /// <param name="id">Guid</param>
        /// <param name="version">The open api version.</param>
        /// <returns>Detailed information about an organization.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages. Below sample response.</para>
        /// <code>
        /// {
        ///    "id": [
        ///      "Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)."
        ///    ]
        /// }
        /// </code>
        /// </remarks>
        // GET api/organization/5
        [HttpGet("{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V2VmOpenApiOrganization), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "Detailed information about an organization.", typeof(V2VmOpenApiOrganization))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.NotFound, CoreMessages.OpenApi.NotFoundGeneralMessage, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public IActionResult Get(string id)
        {
            Guid guid = id.ParseToGuidWithExeption();
            var org = OrganizationService.V2GetOrganization(guid);

            if (org == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Organization with id '{id}' not found." });
            }

            return Ok(org);
        }

        /// <summary>
        /// Fetches all the information related to organizations with defined business identity code.
        /// </summary>
        /// <param name="code">Finnish business identity code (Y-tunnus).</param>
        /// <param name="version">The open api version.</param>
        /// <returns>Detailed information about organizations.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages. Below sample response.</para>
        /// <code>
        /// {
        ///    "code": [
        ///      "The field code must match the regular expression '^[0-9]{7}-[0-9]{1}$'."
        ///    ]
        /// }
        /// </code>
        /// </remarks>
        [HttpGet("businesscode/{code}")]
        [ValidateRegEx("code", @"^[0-9]{7}-[0-9]{1}$")]
        [ProducesResponseType(typeof(IList<V2VmOpenApiOrganization>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "Detailed information about organizations.", typeof(IList<V2VmOpenApiOrganization>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.NotFound, CoreMessages.OpenApi.NotFoundGeneralMessage, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public IActionResult GetByYCode(string code)
        {
            // GET api/organization/businesscode/1234567-8

            var org = OrganizationService.V2GetOrganizationsByBusinessCode(code);

            if (org == null || org.Count == 0)
            {
                return NotFound(new VmError() { ErrorMessage = $"Organizations with code '{code}' not found." });
            }

            return Ok(org);
        }

        /// <summary>
        /// Fetches all the information related to a single organization with defined Oid.
        /// </summary>
        /// <param name="oid">Oid identifier</param>
        /// <param name="version">The open api version.</param>
        /// <returns>Detailed information about an organization.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages. Below sample response.</para>
        /// <code>
        /// {
        ///    "oid": [
        ///      "The field oid must match the regular expression '^[A-Za-z0-9.-]*$'."
        ///    ]
        /// }
        /// </code>
        /// </remarks>
        [HttpGet("oid/{oid}")]
        [ValidateRegEx("oid", @"^[A-Za-z0-9.-]*$")]
        [ProducesResponseType(typeof(V2VmOpenApiOrganization), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "Detailed information about an organization.", typeof(V2VmOpenApiOrganization))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.NotFound, CoreMessages.OpenApi.NotFoundGeneralMessage, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public IActionResult GetByOid(string oid)
        {
            // GET api/organization/Ycode/5

            var org = OrganizationService.V2GetOrganizationByOid(oid);

            if (org == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Organization with OID '{oid}' not found." });
            }

            return Ok(org);
        }


        /// <summary>
        /// Creates a new organization with the data provided as input.
        /// </summary>
        /// <param name="request">The organization data.</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The created organization.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "Addresses[1].StreetAddress": [
        ///         "The StreetAddress field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        // POST api/organization
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPost]
        [ProducesResponseType(typeof(V2VmOpenApiOrganization), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The created organization.", typeof(V2VmOpenApiOrganization))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public IActionResult Post([FromBody]V2VmOpenApiOrganizationIn request)
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

            // Check the item values from db and validate
            ValidateParameters(request, true);

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var result = OrganizationService.V2AddOrganization(request, Settings.AllowAnonymous);
            return Ok(result);
        }


        // PUT api/organization/5
        /// <summary>
        /// Updates organization.
        /// </summary>
        /// <param name="id">organization entity id</param>
        /// <param name="request">organization values</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated organization.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages. Below sample response.</para>
        /// <code>
        /// {
        ///    "propertyname": [
        ///      "The field propertyname has invalid characters."
        ///    ]
        /// }
        /// </code>
        /// </remarks>
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPut("{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V2VmOpenApiOrganization), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The updated organization.", typeof(V2VmOpenApiOrganization))]
        [SwaggerResponse(HttpStatusCode.NotFound, CoreMessages.OpenApi.NotFoundGeneralMessage, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public IActionResult Put(string id, [FromBody]V2VmOpenApiOrganizationInBase request)
        {
            // check that the resource exists that we are about to update
            if (!CheckOrganizationExists(id))
            {
                return NotFound(new VmError() { ErrorMessage = $"Organization with id '{id}' not found." });
            }

            // TODO: badrequest can be returned from validation also so it will be different model than VmError
            // change to use only the modelstate instead of VmError so the return model is same for errors as now there would be 2 different models for bad request
            if (request != null)
            {
                request.Id = id.ParseToGuid();
            }

            var validationResult = ValidateRequest(request);
            return validationResult ?? Ok(OrganizationService.V2SaveOrganization(request, Settings.AllowAnonymous));
        }

        // PUT api/organization/sourceId/5
        /// <summary>
        /// Updates organization.
        /// </summary>
        /// <param name="sourceId">organization external id</param>
        /// <param name="request">organization values</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated organization.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages. Below sample response.</para>
        /// <code>
        /// {
        ///    "propertyname": [
        ///      "The field propertyname has invalid characters."
        ///    ]
        /// }
        /// </code>
        /// </remarks>
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPut("sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", @"^[A-Za-z0-9-]*$")]
        [ProducesResponseType(typeof(V2VmOpenApiOrganization), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The updated organization.", typeof(V2VmOpenApiOrganization))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public IActionResult PutBySource(string sourceId, [FromBody]V2VmOpenApiOrganizationInBase request)
        {
            if (request != null)
            {
                request.SourceId = sourceId;
            }

            var validationResult = ValidateRequest(request);
            return validationResult ?? Ok(OrganizationService.V2SaveOrganization(request, Settings.AllowAnonymous));
        }
    }
}
