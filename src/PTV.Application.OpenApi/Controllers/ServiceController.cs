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
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Application.OpenApi.Models;
using PTV.Framework;
using System.Net;
using Microsoft.Extensions.Options;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using Microsoft.AspNetCore.Authorization;
using PTV.Framework.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;
using PTV.Domain.Model.Models.OpenApi.V1;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api services related methods.
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer", Roles = "Eeva,Pete")]
    [Route("api/[controller]")]
    public class ServiceController : ServiceBaseController
    {
        /// <summary>
        /// ServiceController constructor.
        /// </summary>
        public ServiceController(
            IServiceService serviceService,
            IOrganizationService organizationService,
            ICodeService codeService,
            IOptions<AppSettings> settings,
            IGeneralDescriptionService generalDescriptionService,
            IFintoService fintoService,
            IServiceAndChannelService serviceAndChannelService,
            ILogger<ServiceController> logger)
            : base(serviceService, organizationService, codeService, settings, generalDescriptionService, fintoService, serviceAndChannelService, logger, 1)
        {
        }

        /// <summary>
        /// Gets all the published services within PTV as a list of service ids.
        /// Services created after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss"</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <param name="version">The open api version.</param>
        /// <returns>A list of service ids with paging.</returns>
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
        [ProducesResponseType(typeof(VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service ids with paging.", type: typeof(VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Get([FromQuery]DateTime? date, [FromQuery]int page = 1)
        {
            return base.GetIdList(date, page);
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
        [ProducesResponseType(typeof(VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about a service.", type: typeof(VmOpenApiService))]
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
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss"</param>
        /// <param name="version">The open api version.</param>
        /// <returns>Detailed information about a service.</returns>
        [AllowAnonymous]
        [HttpGet("serviceChannel/{serviceChannelId}")]
        [ValidateId("serviceChannelId")]
        [ValidateDate]
        [ProducesResponseType(typeof(IList<VmOpenApiService>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about a service.", type: typeof(IList<VmOpenApiService>))]
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPost]
        [ProducesResponseType(typeof(VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created service.", type: typeof(VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult Post([FromBody]VmOpenApiServiceIn request)
        {
            return base.Post(request);
        }

        /// <summary>
        /// Creates a relationships between services and service channels (each service is connected to the service channels listed).
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPost("ServiceAndChannel")]
        [ProducesResponseType(typeof(IList<string>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of messages about succesfull and unsuccesfull addings.", type: typeof(List<string>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(IList<string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PostServiceAndChannel([FromBody]VmOpenApiServiceAndChannel request)
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

            var msgList = new List<string>();
            var channelIds = new List<Guid>();
            request.ServiceChannelIds.ForEach(c => channelIds.Add(c.ParseToGuidWithExeption()));
            foreach(var serviceId in request.ServiceIds)
            {
                Guid guid = serviceId.ParseToGuidWithExeption();
                msgList.AddRange(ServiceService.AddChannelsForService(guid, channelIds, Settings.AllowAnonymous));
            }

            return Ok(msgList);
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPut("{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service.", type: typeof(VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage + " Invalid version number.", type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult Put(string id, [FromBody]VmOpenApiServiceInBase request)
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPut("sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", @"^[A-Za-z0-9-]*$")]
        [ProducesResponseType(typeof(VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service.", type: typeof(VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutBySource(string sourceId, [FromBody]VmOpenApiServiceInBase request)
        {
            return base.Put(request, sourceId: sourceId);
        }
    }
}
