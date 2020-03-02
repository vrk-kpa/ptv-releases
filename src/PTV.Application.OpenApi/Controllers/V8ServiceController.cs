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
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.Attributes;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework;
using PTV.Framework.Attributes;
using PTV.Framework.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api services related methods.
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v8/Service")]
    public class V8ServiceController : ServiceBaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="V8ServiceController"/> class.
        /// </summary>
        /// <param name="serviceService">The service service.</param>
        /// <param name="commonService">The common service.</param>
        /// <param name="codeService">The code service.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generalDescriptionService">The general description service.</param>
        /// <param name="fintoService">The finto service.</param>
        /// <param name="ontologyTermDataCache">ontologyTermDataCache</param>
        /// <param name="channelService">The channel service.</param>
        /// <param name="userOrganizationService">The user organization service.</param>
        /// <param name="organizationService">The organization service.</param>
        /// <param name="logger">The logger.</param>
        public V8ServiceController(
            IServiceService serviceService,
            ICommonService commonService,
            ICodeService codeService,
            IOptions<AppSettings> settings,
            IGeneralDescriptionService generalDescriptionService,
            IFintoService fintoService,
            IOntologyTermDataCache ontologyTermDataCache,
            IChannelService channelService,
            IUserOrganizationService userOrganizationService,
            IOrganizationService organizationService,
            ILogger<V8ServiceController> logger)
            : base(serviceService, commonService, codeService, settings, generalDescriptionService, fintoService, ontologyTermDataCache,
                  channelService, userOrganizationService, organizationService, logger, 8)
        {
        }

        /// <summary>
        /// Gets all the published services within PTV as a list of service ids and names.
        /// Services created/modified after certain date can be fetched by adding date as query string parameter.
        /// Services created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// Archived items can be fetched by setting status parameter as 'Archived' and withdrawn items can be fetched by setting status parameter as 'Withdrawn'.
        /// </summary>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <param name="status">Set status to get items either in published, archived or withdrawn state.</param>
        /// <returns>A list of service ids and names with paging.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "date": [
        ///         "The value 'rfsd' is not valid for Nullable`1.",
        ///         "The date parameter is invalid."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [AllowAnonymous]
        [HttpGet]
        [ValidateEnum("status", typeof(EntityStatusEnum), false)]
        [ValidateDate]
        [ValidateDate("dateBefore")]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service ids with paging.", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Get([FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1, [FromQuery]string status = "Published")
        {
            var entityStatus = status.ConvertToEnum<EntityStatusExtendedEnum>();
            if (!entityStatus.HasValue)
            {
                ModelState.AddModelError("status", "The status is invalid.");
                return BadRequest(ModelState);
            }

            return GetIdAndNameList(date, page, entityStatus.Value, dateBefore);
        }

        /// <summary>
        /// Fetches all the information related to a single service.
        /// </summary>
        /// <param name="id">Guid</param>
        /// <returns>Detailed information about a service.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "id": [
        ///         "Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V8VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about a service.", type: typeof(V8VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Get(string id)
        {
            return GetById(id);
        }

        /// <summary>
        /// Fetches all the information related to requests services.
        /// </summary>
        /// <param name="guids">Comma separated list of guids. Max 100 can be added into list.</param>
        /// <returns>Detailed information about services.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "id": [
        ///         "Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("list")]
        [ProducesResponseType(typeof(IList<V8VmOpenApiService>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about services.", type: typeof(IList<V8VmOpenApiService>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByIdList([FromQuery]string guids)
        {
            return GetByIdListBase(guids);
        }

        /// <summary>
        /// Gets all services within PTV as a list of service ids and names. Also services with draft and modified versions are included.
        /// Services created/modified after certain date can be fetched by adding date as query string parameter.
        /// Services created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// NOTE! This is a restricted endpoint.
        /// </summary>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <returns>A list of service ids and names with paging.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "date": [
        ///         "The value 'rfsd' is not valid for Nullable`1.",
        ///         "The date parameter is invalid."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiRead)]
        [HttpGet("active")]
        [ValidateDate]
        [ValidateDate("dateBefore")]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service ids with paging.", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult GetActive([FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1)
        {
            return GetIdAndNameList(date, page, EntityStatusExtendedEnum.Active, dateBefore);
        }

        /// <summary>
        /// Fetches all the information related to a single service. Also services with only draft or modified versions are returned.
        /// NOTE! This is a restricted endpoint.
        /// </summary>
        /// <param name="id">Guid</param>
        /// <returns>Detailed information about a service.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "id": [
        ///         "Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiRead)]
        [HttpGet("active/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V8VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about a service.", type: typeof(V8VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetActive(string id)
        {
            return GetById(id, false);
        }

        /// <summary>
        /// Gets a list of published services for defined service channel.
        /// Services joined to service channel after certain date can be fetched by adding date as query string parameter.
        /// Services joined to service channel before certain date can be fetched by adding dateBefore as query string parameter.
        /// </summary>
        /// <param name="serviceChannelId">Guid</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <returns>A list of service ids with paging.</returns>
        [AllowAnonymous]
        [HttpGet("serviceChannel/{serviceChannelId}")]
        [ValidateId("serviceChannelId")]
        [ValidateDate]
        [ValidateDate("dateBefore")]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service ids with paging.", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: null, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByServiceChannel(string serviceChannelId, [FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1)
        {
            return GetGuidPageByServiceChannel(serviceChannelId, date, page, dateBefore);
        }

        /// <summary>
        /// Gets a list of published services for defined service class.
        /// Services created/modified after certain date can be fetched by adding date as query string parameter.
        /// Services created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// </summary>
        /// <param name="uri">Service class uri, e.g. http://urn.fi/URN:NBN:fi:au:ptvl:v1065 </param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <returns>A list of service ids with paging.</returns>
        [AllowAnonymous]
        [HttpGet("serviceClass")]
        [ValidateServiceClass]
        [ValidateDate]
        [ValidateDate("dateBefore")]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service ids with paging.", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: null, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByServiceClass([FromQuery]string uri, [FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1)
        {
            return GetByServiceClassBase(uri, date, page, dateBefore);
        }

        /// <summary>
        /// Gets a list of published services related to defined municipality.
        /// Services created/modified after certain date can be fetched by adding date as query string parameter.
        /// Services created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// </summary>
        /// <param name="code">Municipality code</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <returns>A list of service ids with paging.</returns>
        [AllowAnonymous]
        [HttpGet("municipality/{code}")]
        [ValidateRegEx("code", ValidationConsts.Municipality)]
        [ValidateDate]
        [ValidateDate("dateBefore")]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service ids with paging", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByMunicipality(string code, [FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1)
        {
            return GetGuidPageByArea(AreaTypeEnum.Municipality.ToString(), code, true, date, page, dateBefore);
        }

        /// <summary>
        /// Gets a list of published services for defined target group.
        /// Services created/modified after certain date can be fetched by adding date as query string parameter.
        /// Services created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// </summary>
        /// <param name="uri">Target group uri, e.g. http://urn.fi/URN:NBN:fi:au:ptvl:v2001 </param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <returns>A list of service ids with paging.</returns>
        [AllowAnonymous]
        [HttpGet("targetGroup")]
        [ValidateRegEx("uri", ValidationConsts.TargetGroup)]
        [ValidateDate]
        [ValidateDate("dateBefore")]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service ids with paging.", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: null, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByTargetGroup([FromQuery]string uri, [FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1)
        {
            return GetByTargetGroupBase(uri, date, page, dateBefore);
        }

        /// <summary>
        /// Gets a list of published services of defined service type.
        /// Services created/modified after certain date can be fetched by adding date as query string parameter.
        /// Services created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// </summary>
        /// <param name="type">Service type</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <returns>A list of service ids with paging.</returns>
        [AllowAnonymous]
        [HttpGet("type/{type}")]
        [ValidateServiceType]
        [ValidateDate]
        [ValidateDate("dateBefore")]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service ids with paging.", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: null, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByType(string type, [FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1)
        {
            return base.GetByType(type, date, page, dateBefore);
        }

        /// <summary>
        /// Creates a new service with the data provided as input.
        /// </summary>
        /// <param name="request">The service data.</param>
        /// <param name="attachProposedChannels">Indicates if service channels attached into general description should automatically be attached into the service.</param>
        /// <returns>The created service.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceNames": [
        ///         "Type - Required value 'Name' was not found!"
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        // POST api/service
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost]
        [ProducesResponseType(typeof(V8VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created service.", type: typeof(V8VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult Post([FromBody]V8VmOpenApiServiceIn request, bool attachProposedChannels)
        {
            return base.Post(request, attachProposedChannels);
        }

        /// <summary>
        /// Updates the defined service with the data provided as input.
        /// </summary>
        /// <param name="id">Service identifier</param>
        /// <param name="request">The service data</param>
        /// <param name="attachProposedChannels">Indicates if service channels attached into general description should automatically be attached into the service.</param>
        /// <returns>The updated service.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceNames[0].Type": [
        ///         "The Type field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        // PUT api/service/5
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V8VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service.", type: typeof(V8VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult Put(string id, [FromBody]V8VmOpenApiServiceInBase request, bool attachProposedChannels)
        {
            return base.Put(request, id, attachProposedChannels: attachProposedChannels);
        }

        /// <summary>
        /// Updates the defined service with the data provided as input.
        /// </summary>
        /// <param name="sourceId">External source identifier</param>
        /// <param name="request">The service data</param>
        /// <param name="attachProposedChannels">Indicates if service channels attached into general description should automatically be attached into the service.</param>
        /// <returns>The updated service.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceNames[0].Type": [
        ///         "The Type field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", ValidationConsts.ExternalSource)]
        [ProducesResponseType(typeof(V8VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service.", type: typeof(V8VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutBySource(string sourceId, [FromBody]V8VmOpenApiServiceInBase request, bool attachProposedChannels)
        {
            return base.Put(request, sourceId: sourceId, attachProposedChannels: attachProposedChannels);
        }
    }
}
