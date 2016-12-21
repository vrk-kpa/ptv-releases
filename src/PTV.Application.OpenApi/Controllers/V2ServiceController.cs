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
using PTV.Domain.Model.Models.Interfaces.V2;
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
    /// PTV Open Api services related methods.
    /// </summary>
    [Route("api/v2/Service")]
    public class V2ServiceController : ServiceBaseController
    {
        private IServiceAndChannelService serviceAndChannelService;

        public V2ServiceController(
            IServiceService serviceService,
            IOrganizationService organizationService,
            ICodeService codeService,
            IOptions<AppSettings> settings,
            IGeneralDescriptionService generalDescriptionService,
            IFintoService fintoService,
            IServiceAndChannelService serviceAndChannelService)
            : base(serviceService, organizationService, codeService, settings, generalDescriptionService, fintoService)
        {
            this.serviceAndChannelService = serviceAndChannelService;
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
        [HttpGet]
        [ValidateDate]
        [ProducesResponseType(typeof(VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "A list of service ids with paging.", typeof(VmOpenApiGuidPage))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Invalid version number.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public override IActionResult Get([FromQuery]DateTime? date, [FromQuery]int page = 1)
        {
            return base.Get(date, page);
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
        [HttpGet("{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V2VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "Detailed information about a service.", typeof(V2VmOpenApiService))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.NotFound, CoreMessages.OpenApi.NotFoundGeneralMessage + " Invalid version number defined.", typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public IActionResult Get(string id)
        {
            Guid guid = id.ParseToGuidWithExeption();

            var srv = ServiceService.V2GetService(guid);

            if (srv == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Service with id '{id}' not found." });
            }

            return Ok(srv);
        }


        /// <summary>
        /// Gets a list of published services for defined service channel.
        /// Services joined to service channel after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="serviceChannelId">Guid</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss"</param>
        /// <param name="version">The open api version.</param>
        /// <returns>Detailed information about a service.</returns>
        [HttpGet("serviceChannel/{serviceChannelId}")]
        [ValidateId("serviceChannelId")]
        [ValidateDate]
        [ProducesResponseType(typeof(IList<V2VmOpenApiService>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "Detailed information about a service.", typeof(IList<V2VmOpenApiService>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, null, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Invalid version number.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public IActionResult GetByServiceChannel(string serviceChannelId, [FromQuery]DateTime? date)
        {
            Guid guid = serviceChannelId.ParseToGuidWithExeption();

            // TODO: should we actually add a check that the service channel with the id exists and if not return 404? -- AAL

            return Ok(ServiceService.V2GetServicesByServiceChannel(guid, date));
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
        [ProducesResponseType(typeof(V2VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The created service.", typeof(V2VmOpenApiService))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Invalid version number.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult Post([FromBody]V2VmOpenApiServiceIn request)
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
            ValidateParameters(request);
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var result = ServiceService.V2AddService(request, Settings.AllowAnonymous);
            return Ok(result);
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPost("ServiceAndChannel")]
        [ProducesResponseType(typeof(IList<string>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "A list of messages about succesfull and unsuccesfull addings.", typeof(List<string>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Invalid version number.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(IList<string>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult PostServiceAndChannel([FromBody]List<V2VmOpenApiServiceAndChannel> request)
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

            var msgList = serviceAndChannelService.SaveServicesAndChannels(request);

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
        [ProducesResponseType(typeof(V2VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The updated service.", typeof(V2VmOpenApiService))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.NotFound, CoreMessages.OpenApi.NotFoundGeneralMessage + " Invalid version number.", typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult Put(string id, [FromBody]V2VmOpenApiServiceInBase request)
        {
            Guid? serviceId = id.ParseToGuid();

            // check that service exists
            if (!serviceId.HasValue || !ServiceService.ServiceExists(serviceId.Value))
            {
                return NotFound(new VmError() { ErrorMessage = $"Organization with id '{id}' not found." });
            }

            if (request != null)
            {
                request.Id = serviceId;
            }
            return SaveService(request);
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
        [ProducesResponseType(typeof(V2VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The updated service.", typeof(V2VmOpenApiService))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Invalid version number.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult PutBySource(string sourceId, [FromBody]V2VmOpenApiServiceInBase request)
        {
            return SaveService(request, sourceId);
        }

        private IActionResult SaveService(IV2VmOpenApiServiceInBase request, string sourceId = null)
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
            ValidateParameters(request);
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            return Ok(ServiceService.V2SaveService(request, Settings.AllowAnonymous, sourceId));
        }
    }
}
