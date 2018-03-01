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
using PTV.Framework;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using Microsoft.Extensions.Logging;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework.Enums;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.OpenApi.V8;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api Organization related methods. Version 8
    /// </summary>
    // PTV-3870 - temporarily hide version 8 for release 1.8.
    //[Authorize(ActiveAuthenticationSchemes = "Bearer", Roles = "Eeva,Pete")]
    [Authorize(ActiveAuthenticationSchemes = "Bearer", Roles = "Eeva")]
    [Route("api/v8/Organization")]
    public class V8OrganizationController : OrganizationBaseController
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public V8OrganizationController(
            IOrganizationService organizationService,
            ICodeService codeService,
            IOptions<AppSettings> settings,
            ILogger<V8OrganizationController> logger,
            ICommonService commonService,
            IUserOrganizationService userService)
            : base(organizationService, codeService, settings, logger, commonService, userService, 8)
        {
        }

        /// <summary>
        /// Gets all the published organizations within PTV as a list of organization ids and names.
        /// Organizations created/modified after certain date can be fetched by adding date as query string parameter.
        /// Archived items can be fetched by setting parameter archived to true.
        /// </summary>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <param name="archived">Get archived items by setting archived to true.</param>
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
        //[AllowAnonymous]// PTV-3870 - temporarily hide version 8 for release 1.8.
        [HttpGet]
        [ValidateDate]
        [ProducesResponseType(typeof(VmOpenApiOrganizationGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of organization ids and names with paging.", type: typeof(VmOpenApiOrganizationGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")] // can be caused by using wrong version number
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Get([FromQuery]DateTime? date, [FromQuery]int page = 1, [FromQuery]bool archived = false)
        {
            if (page < 0)
            {
                ModelState.AddModelError("page", "The page number cannot be negative value.");
                return BadRequest(ModelState);
            }

            return Ok(OrganizationService.GetOrganizations(date, versionNumber, page, PageSize, archived));
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
        //[AllowAnonymous]// PTV-3870 - temporarily hide version 8 for release 1.8.
        [HttpGet("{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V8VmOpenApiOrganization), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about an organization.", type: typeof(V8VmOpenApiOrganization))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Get(string id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Fetches all the information related to requested organizations.
        /// </summary>
        /// <param name="guids">Comma separated list of guids. Max 100 can be added into list.</param>
        /// <returns>Detailed information about organizations.</returns>
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
        //[AllowAnonymous]// PTV-3870 - temporarily hide version 8 for release 1.8.
        [HttpGet("list")]
        [ProducesResponseType(typeof(IList<V8VmOpenApiOrganization>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about organizations.", type: typeof(IList<V8VmOpenApiOrganization>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByIdList([FromQuery]string guids)
        {
            return base.GetByIdListBase(guids);
        }

        /// <summary>
        /// Gets main organizations and two sub levels of organizations. Returns both published and archived organizations.
        /// NOTE! This is a restricted endpoint.
        /// </summary>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <returns>A list of organizations with paging.</returns>
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [AccessRightRequirement(AccessRightEnum.OpenApiRead)]
        [HttpGet("saha")]
        [ValidateDate]
        [ProducesResponseType(typeof(VmOpenApiOrganizationSahaGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of organization ids and names with paging.", type: typeof(VmOpenApiOrganizationSahaGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")] // can be caused by using wrong version number
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetSaha([FromQuery]DateTime? date, [FromQuery]int page = 1)
        {
            return Ok(OrganizationService.GetOrganizationsSaha(date, page, PageSize));
        }

        /// <summary>
        /// Fetches Saha related information of a single organization.
        /// NOTE! This is a restricted endpoint.
        /// </summary>
        /// <param name="id">Guid</param>
        /// <returns>Information about an organization.</returns>
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [AccessRightRequirement(AccessRightEnum.OpenApiRead)]
        [HttpGet("saha/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(VmOpenApiOrganizationSaha), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about an organization.", type: typeof(VmOpenApiOrganizationSaha))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetSaha(string id)
        {
            return base.GetSahaById(id);
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
        //[AllowAnonymous]// PTV-3870 - temporarily hide version 8 for release 1.8.
        [HttpGet("businesscode/{code}")]
        [ValidateRegEx("code", @"^[0-9]{7}-[0-9]{1}$")]
        [ProducesResponseType(typeof(IList<V8VmOpenApiOrganization>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about organizations.", type: typeof(IList<V8VmOpenApiOrganization>))]
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
        //[AllowAnonymous]// PTV-3870 - temporarily hide version 8 for release 1.8.
        [HttpGet("oid/{oid}")]
        [ValidateRegEx("oid", @"^[A-Za-z0-9.-]*$")]
        [ProducesResponseType(typeof(V8VmOpenApiOrganization), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about an organization.", type: typeof(V8VmOpenApiOrganization))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByOid(string oid)
        {
            return base.GetByOidBase(oid);
        }

        /// <summary>
        /// Gets a list of published organizations related to defined municipality.
        /// Services created after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="code">Municipality code</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <returns>A list of organization ids with paging.</returns>
        //[AllowAnonymous]// PTV-3870 - temporarily hide version 8 for release 1.8.
        [HttpGet("municipality/{code}")]
        [ValidateRegEx("code", @"^[0-9]{1,3}$")]
        [ValidateDate]
        [ProducesResponseType(typeof(VmOpenApiOrganizationGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of organization ids and names with paging.", type: typeof(VmOpenApiOrganizationGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByMunicipality(string code, [FromQuery]DateTime? date, [FromQuery]int page = 1)
        {
            return base.GetGuidPageByMunicipality(code, date, page);
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost]
        [ProducesResponseType(typeof(V8VmOpenApiOrganization), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created organization.", type: typeof(V8VmOpenApiOrganization))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult Post([FromBody]V8VmOpenApiOrganizationIn request)
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V8VmOpenApiOrganization), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated organization.", type: typeof(V8VmOpenApiOrganization))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Put(string id, [FromBody]V8VmOpenApiOrganizationInBase request)
        {
            return base.Put(request, id);
        }

        // PUT api/organization/sourceId/5
        /// <summary>
        /// Updates organization.
        /// </summary>
        /// <param name="sourceId">organization external id</param>
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", @"^[A-Za-z0-9-.]*$")]
        [ProducesResponseType(typeof(V8VmOpenApiOrganization), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated organization.", type: typeof(V8VmOpenApiOrganization))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult PutBySource(string sourceId, [FromBody]V8VmOpenApiOrganizationInBase request)
        {
            return base.Put(request, sourceId: sourceId);
        }

    }
}
