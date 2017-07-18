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
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Framework;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using Microsoft.Extensions.Logging;
using PTV.Framework.Enums;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api Organization related methods. Version 3
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer", Roles = "Eeva,Pete")]
    [Route("api/v3/Organization")]
    public class V3OrganizationController : OrganizationBaseController
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public V3OrganizationController(IOrganizationService organizationService, ICodeService codeService, IOptions<AppSettings> settings, IFintoService fintoService, ILogger<V3OrganizationController> logger, ICommonService commonService)
            : base(organizationService, codeService, settings, fintoService, logger, commonService, 3)
        {
        }

        /// <summary>
        /// Gets all the published organizations within PTV as a list of organization ids and names.
        /// Organizations created/modified after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page number to be fetched.</param>
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
        [AllowAnonymous]
        [HttpGet]
        [ValidateDate]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of organization ids and names with paging.", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")] // can be caused by using wrong version number
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Get([FromQuery]DateTime? date, [FromQuery]int page = 1)
        {
            return Ok(OrganizationService.GetOrganizations(date, page, PageSize));
        }


        /// <summary>
        /// Fetches all the information related to a single organization.
        /// </summary>
        /// <param name="id">Guid</param>
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
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V3VmOpenApiOrganization), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about an organization.", type: typeof(V3VmOpenApiOrganization))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Get(string id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Fetches all the information related to organizations with defined business identity code.
        /// </summary>
        /// <param name="code">Finnish business identity code (Y-tunnus).</param>
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
        [AllowAnonymous]
        [HttpGet("businesscode/{code}")]
        [ValidateRegEx("code", @"^[0-9]{7}-[0-9]{1}$")]
        [ProducesResponseType(typeof(IList<V3VmOpenApiOrganization>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about organizations.", type: typeof(IList<V3VmOpenApiOrganization>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByYCode(string code)
        {
            return base.GetByYCodeBase(code);
        }

        /// <summary>
        /// Fetches all the information related to a single organization with defined Oid.
        /// </summary>
        /// <param name="oid">Oid identifier</param>
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
        [AllowAnonymous]
        [HttpGet("oid/{oid}")]
        [ValidateRegEx("oid", @"^[A-Za-z0-9.-]*$")]
        [ProducesResponseType(typeof(V3VmOpenApiOrganization), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about an organization.", type: typeof(V3VmOpenApiOrganization))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByOid(string oid)
        {
            return base.GetByOidBase(oid);
        }


        /// <summary>
        /// Creates a new organization with the data provided as input.
        /// </summary>
        /// <param name="request">The organization data.</param>
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost]
        [ProducesResponseType(typeof(V3VmOpenApiOrganization), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created organization.", type: typeof(V3VmOpenApiOrganization))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult Post([FromBody]V3VmOpenApiOrganizationIn request)
        {
            return base.Post(request);
        }


        // PUT api/organization/5
        /// <summary>
        /// Updates organization.
        /// </summary>
        /// <param name="id">organization entity id</param>
        /// <param name="request">organization values</param>
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V3VmOpenApiOrganization), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated organization.", type: typeof(V3VmOpenApiOrganization))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Put(string id, [FromBody]V3VmOpenApiOrganizationInBase request)
        {
            return base.Put(request, id);
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", @"^[A-Za-z0-9-.]*$")]
        [ProducesResponseType(typeof(V3VmOpenApiOrganization), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated organization.", type: typeof(V3VmOpenApiOrganization))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult PutBySource(string sourceId, [FromBody]V3VmOpenApiOrganizationInBase request)
        {
            return base.Put(request, sourceId: sourceId);
        }
    }
}
