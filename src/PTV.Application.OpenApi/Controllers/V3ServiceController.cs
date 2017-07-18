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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Framework;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Attributes;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Framework.Enums;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api services related methods.
    /// </summary>
    //[ApiExplorerSettings(GroupName = @"Service")]
    [Authorize(ActiveAuthenticationSchemes = "Bearer", Roles = "Eeva,Pete")]
    [Route("api/v3/Service")]
    public class V3ServiceController : ServiceBaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="V3ServiceController"/> class.
        /// </summary>
        /// <param name="serviceService">The service service.</param>
        /// <param name="commonService">The common service.</param>
        /// <param name="codeService">The code service.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generalDescriptionService">The general description service.</param>
        /// <param name="fintoService">The finto service.</param>
        /// <param name="serviceAndChannelService">The service and channel service.</param>
        /// <param name="channelService">The channel service.</param>
        /// <param name="userService">The user info service.</param>
        /// <param name="logger">The logger</param>
        public V3ServiceController(
            IServiceService serviceService,
            ICommonService commonService,
            ICodeService codeService,
            IOptions<AppSettings> settings,
            IGeneralDescriptionService generalDescriptionService,
            IFintoService fintoService,
            IServiceAndChannelService serviceAndChannelService,
            IChannelService channelService,
            IUserInfoService userService,
            ILogger<V3ServiceController> logger)
            : base(serviceService, commonService, codeService, settings, generalDescriptionService, fintoService, serviceAndChannelService, channelService, userService, logger, 3)
        {
        }


        /// <summary>
        /// Gets all the published services within PTV as a list of service ids and names.
        /// Services created after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
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
        [AllowAnonymous]
        [HttpGet]
        [ValidateDate]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service ids with paging.", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Get([FromQuery]DateTime? date, [FromQuery]int page = 1)
        {
            return base.GetIdAndNameList(date, page);
        }

        /// <summary>
        /// Fetches all the information related to a single service.
        /// </summary>
        /// <param name="id">Guid</param>
        /// <param name="version">The open api version.</param>
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
        [ProducesResponseType(typeof(V3VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about a service.", type: typeof(V3VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage + " Invalid version number defined.", type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Get(string id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Gets a list of published services for defined service channel.
        /// Services joined to service channel after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="serviceChannelId">Guid</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <returns>Detailed information about a service.</returns>
        [AllowAnonymous]
        [HttpGet("serviceChannel/{serviceChannelId}")]
        [ValidateId("serviceChannelId")]
        [ValidateDate]
        [ProducesResponseType(typeof(IList<V3VmOpenApiService>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about a service.", type: typeof(IList<V3VmOpenApiService>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: null, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public override IActionResult GetByServiceChannel(string serviceChannelId, [FromQuery]DateTime? date)
        {
            return base.GetByServiceChannel(serviceChannelId, date);
        }


        /// <summary>
        /// Creates a new service with the data provided as input.
        /// </summary>
        /// <param name="request">The service data.</param>
        /// <param name="version">The open api version.</param>
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost]
        [ProducesResponseType(typeof(V3VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created service.", type: typeof(V3VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult Post([FromBody]V3VmOpenApiServiceIn request)
        {
            return base.Post(request);
        }

        /// <summary>
        /// Creates a relationships between services and service channels with extra data.
        /// </summary>
        /// <param name="request">A list of services and service channels.</param>
        /// <param name="version">The open api version.</param>
        /// <returns>A list of messages about succesfull and unsuccesfull addings.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "Service with id '00000000-0000-0000-0000-00000000' not found!"
        /// }
        /// </code>
        /// </remarks>
        // POST api/service
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost("ServiceAndChannel")]
        [ProducesResponseType(typeof(IList<string>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of messages about succesfull and unsuccesfull addings.", type: typeof(List<string>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(IList<string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PostServiceAndChannel([FromBody]List<V2VmOpenApiServiceAndChannel> request)
        {
            return base.PostServiceAndChannelBase(request);
        }


        /// <summary>
        /// Updates the defined service with the data provided as input.
        /// </summary>
        /// <param name="id">Service identifier</param>
        /// <param name="request">The service data</param>
        /// <param name="version">The open api version.</param>
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V3VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service.", type: typeof(V3VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage + " Invalid version number.", type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult Put(string id, [FromBody]V3VmOpenApiServiceInBase request)
        {
            return base.Put(request, id);
        }

        /// <summary>
        /// Updates the defined service with the data provided as input.
        /// </summary>
        /// <param name="sourceId">External source identifier</param>
        /// <param name="request">The service data</param>
        /// <param name="version">The open api version.</param>
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", @"^[A-Za-z0-9-.]*$")]
        [ProducesResponseType(typeof(V3VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service.", type: typeof(V3VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutBySource(string sourceId, [FromBody]V3VmOpenApiServiceInBase request)
        {
            return base.Put(request, sourceId: sourceId);
        }
    }
}
