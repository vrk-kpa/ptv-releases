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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Framework;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using PTV.Application.OpenApi.Attributes;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using Microsoft.Extensions.Logging;
using PTV.Framework.Enums;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api service channels related methods.
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer", Roles = "Eeva,Pete")]
    [Route("api/v3/ServiceChannel")]
    public class V3ServiceChannelController : ServiceChannelBaseController
    {
        /// <summary>
        /// ServiceChannelController constructor.
        /// </summary>
        public V3ServiceChannelController(IChannelService channelService, IOrganizationService organizationService,
            ICodeService codeService, IServiceService serviceService, IOptions<AppSettings> settings, IFintoService fintoService, ILogger<V3ServiceChannelController> logger, ICommonService commonService) :
            base(channelService,organizationService, codeService, serviceService, settings, logger, commonService, 3)
        {
        }


        /// <summary>
        /// Gets all published service channels within PTV as a list of service channel ids and names.
        /// Service channels created/modified after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page to be fetched.</param>
        /// <param name="version">The open api version.</param>
        /// <returns>A list of service channel ids with paging.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages. Below sample response.</para>
        /// <code>
        /// {
        ///    "date": [
        ///      "The value '-5' is not valid for Nullable`1.",
        ///      "The date parameter is invalid."
        ///    ]
        /// }
        /// </code>
        /// </remarks>
        [AllowAnonymous]
        [HttpGet]
        [ValidateDate]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service channel ids and names with paging.", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Get([FromQuery]DateTime? date, [FromQuery]int page = 1)
        {
            return Ok(ChannelService.GetServiceChannels(date, page, PageSize));
        }

        /// <summary>
        /// Fetches all the information related to a single service channel.
        /// </summary>
        /// <param name="id">Guid</param>
        /// <param name="version">The open api version.</param>
        /// <returns>Detailed information about a service channel.</returns>
        /// <remarks>
        /// <para>Notice! The returned object is one of the following: <i>V3VmOpenApiElectronicChannel</i> or <i>V3VmOpenApiPhoneChannel</i> or
        ///  <i>V3VmOpenApiPrintableFormChannel</i> or <i>V3VmOpenApiServiceLocationChannel</i> or <i>V3VmOpenApiWebPageChannel</i></para>
        /// <para>The returned object depends on the type of the channel. For example if the channel is phone channel then V3VmOpenApiPhoneChannel object is returned.</para>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///    "id": [
        ///        "Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)."
        ///    ]
        /// }
        /// </code>
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V3VmOpenApiServiceChannels), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about a service channel.", type: typeof(V3VmOpenApiServiceChannels))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public override IActionResult Get(string id)
        {
            return base.Get(id);
        }


        /// <summary>
        /// Gets a list of certain type of published service channels.
        /// Service channels created/modified after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="type">Service channel type</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <returns>Detailed information about certain type of service channels.</returns>
        /// <remarks>
        /// <para>Notice! The returned object is one of the following: <i>V3VmOpenApiElectronicChannel</i> or <i>V3VmOpenApiPhoneChannel</i> or
        ///  <i>V3VmOpenApiPrintableFormChannel</i> or <i>V3VmOpenApiServiceLocationChannel</i> or <i>V3VmOpenApiWebPageChannel</i></para>
        /// <para>The returned object depends on the type parameter. For example if the request type was 'Phone' then V3VmOpenApiPhoneChannel objects are returned.</para>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///    "type": [
        ///        "The field is invalid. Please use one of these: 'EChannel, WebPage, PrintableForm, Phone, ServiceLocation'."
        ///    ]
        /// }
        /// </code>
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("type/{type}")]
        [ValidateServiceChannelType]
        [ValidateDate]
        [ProducesResponseType(typeof(IList<V3VmOpenApiServiceChannels>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about certain type of service channels.", type: typeof(IList<V3VmOpenApiServiceChannels>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public override IActionResult GetByType(string type, [FromQuery]DateTime? date)
        {
            return base.GetByType(type, date);
        }

        /// <summary>
        /// Gets a list of published service channels for defined organization.
        ///  Service channels created/modified after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="organizationId">Guid</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <returns>Detailed information about service channels.</returns>
        /// <remarks>
        /// <para>Notice! The returned objects are these types: <i>V3VmOpenApiElectronicChannel</i> or <i>V3VmOpenApiPhoneChannel</i> or
        ///  <i>V3VmOpenApiPrintableFormChannel</i> or <i>V3VmOpenApiServiceLocationChannel</i> or <i>V3VmOpenApiWebPageChannel</i></para>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///    "date": [
        ///        "The value '-2' is not valid for Nullable`1.",
        ///        "The date parameter is invalid."
        ///    ]
        /// }
        /// </code>
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("organization/{organizationId}")]
        [ValidateId("organizationId")]
        [ValidateDate]
        [ProducesResponseType(typeof(IList<V3VmOpenApiServiceChannels>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about service channels for an organization.", type: typeof(IList<V3VmOpenApiServiceChannels>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public override IActionResult GetByOrganizationId(string organizationId, [FromQuery]DateTime? date)
        {
            return base.GetByOrganizationId(organizationId, date);
        }

        /// <summary>
        /// Gets a list of certain type of published service channels for defined organization.
        /// Service channels created/modified after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="organizationId">Guid</param>
        /// <param name="type">Service channel type</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <returns>Detailed information about service channels.</returns>
        /// <remarks>
        /// <para>Notice! The returned objects are one of the following types: <i>V3VmOpenApiElectronicChannel</i> or <i>V3VmOpenApiPhoneChannel</i> or
        ///  <i>V3VmOpenApiPrintableFormChannel</i> or <i>V3VmOpenApiServiceLocationChannel</i> or <i>V3VmOpenApiWebPageChannel</i></para>
        /// <para>The returned object depends on the type parameter. For example if the request type was 'Phone' then V3VmOpenApiPhoneChannel objects are returned.</para>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///    "type": [
        ///        "The field is invalid. Please use one of these: 'EChannel, WebPage, PrintableForm, Phone, ServiceLocation'."
        ///    ]
        /// }
        /// </code>
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("organization/{organizationId}/type/{type}")]
        [ValidateId("organizationId")]
        [ValidateServiceChannelType]
        [ValidateDate]
        [ProducesResponseType(typeof(IList<V3VmOpenApiServiceChannels>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about service channels of certain type channels for an organization.", type: typeof(IList<V3VmOpenApiServiceChannels>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public override IActionResult GetByOrganizationIdAndType(string organizationId, string type, [FromQuery]DateTime? date)
        {
            return base.GetByOrganizationIdAndType(organizationId, type, date);
        }

        /// <summary>
        /// Creates a new electronic channel with the data provided as input.
        /// </summary>
        /// <param name="request">The electronic channel data.</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The created electronic channel.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceChannelNames":[
        ///         "The ServiceChannelNames field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        // POST api/ServiceChannel/EChannel
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost("EChannel")]
        [ProducesResponseType(typeof(V3VmOpenApiElectronicChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created electronic channel.", type: typeof(V3VmOpenApiElectronicChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PostEChannel([FromBody]V3VmOpenApiElectronicChannelIn request)
        {
            return base.PostEChannel(request);
        }

        /// <summary>
        /// Updates a new electronic channel with the data provided as input.
        /// </summary>
        /// <param name="id">electronic channel id</param>
        /// <param name="request">electronic channel data</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated electronic channel</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceChannelNames":[
        ///         "The ServiceChannelNames field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("EChannel/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V3VmOpenApiElectronicChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated electronic channel", type: typeof(V3VmOpenApiElectronicChannel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutEChannel(string id, [FromBody]V3VmOpenApiElectronicChannelInBase request)
        {
            return base.PutEChannel(id, request);
        }

        /// <summary>
        /// Updates a new electronic channel with the data provided as input.
        /// </summary>
        /// <param name="sourceId">electronic channel external source id</param>
        /// <param name="request">electronic channel data</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated electronic channel</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceChannelNames":[
        ///         "The ServiceChannelNames field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("EChannel/sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", @"^[A-Za-z0-9-.]*$")]
        [ProducesResponseType(typeof(V3VmOpenApiElectronicChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated electronic channel", type: typeof(V3VmOpenApiElectronicChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutEChannelBySource(string sourceId, [FromBody]V3VmOpenApiElectronicChannelInBase request)
        {
            return base.PutEChannelBySource(sourceId, request);
        }

        /// <summary>
        /// Creates a new phone channel with the data provided as input.
        /// </summary>
        /// <param name="request">The phone channel data.</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The created phone channel.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceChannelNames":[
        ///         "The ServiceChannelNames field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        // POST api/ServiceChannel/Phone
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost("Phone")]
        [ProducesResponseType(typeof(V3VmOpenApiPhoneChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created phone channel.", type: typeof(V3VmOpenApiPhoneChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PostPhoneChannel([FromBody]V3VmOpenApiPhoneChannelIn request)
        {
            return base.PostPhoneChannel(request);
        }

        /// <summary>
        /// Updates phone channel with the data provided as input.
        /// </summary>
        /// <param name="id">phone channel id</param>
        /// <param name="request">phone channel data</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated phone channel</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceChannelNames":[
        ///         "The ServiceChannelNames field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut]
        [ValidateId("id")]
        [Route("Phone/{id}")]
        [ProducesResponseType(typeof(V3VmOpenApiPhoneChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated phone channel.", type: typeof(V3VmOpenApiPhoneChannel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutPhoneChannel(string id, [FromBody]V3VmOpenApiPhoneChannelInBase request)
        {
            return base.PutPhoneChannel(id, request);
        }


        /// <summary>
        /// Updates phone channel with the data provided as input.
        /// </summary>
        /// <param name="sourceId">phone channel external id</param>
        /// <param name="request">phone channel data</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated phone channel</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceChannelNames":[
        ///         "The ServiceChannelNames field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("Phone/sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", @"^[A-Za-z0-9-.]*$")]
        [ProducesResponseType(typeof(V3VmOpenApiPhoneChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated phone channel.", type: typeof(V3VmOpenApiPhoneChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutPhoneChannelBySource(string sourceId, [FromBody]V3VmOpenApiPhoneChannelInBase request)
        {
            return base.PutPhoneChannelBySource(sourceId, request);
        }

        /// <summary>
        /// Creates a new web page channel with the data provided as input.
        /// </summary>
        /// <param name="request">The web page channel data.</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The created web page channel.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceChannelNames":[
        ///         "The ServiceChannelNames field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        // POST api/ServiceChannel/WebPage
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost("WebPage")]
        [ProducesResponseType(typeof(V3VmOpenApiWebPageChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created web page channel.", type: typeof(V3VmOpenApiWebPageChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PostWebPageChannel([FromBody]V3VmOpenApiWebPageChannelIn request)
        {
            return base.PostWebPageChannel(request);
        }

        /// <summary>
        /// Updates webpage channel with the data provided as input.
        /// </summary>
        /// <param name="id">web page channel id</param>
        /// <param name="request">web page channel data</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated web page channel</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceChannelNames":[
        ///         "The ServiceChannelNames field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("WebPage/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V3VmOpenApiWebPageChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated web page channel.", type: typeof(V3VmOpenApiWebPageChannel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutWebPageChannel(string id, [FromBody]V3VmOpenApiWebPageChannelInBase request)
        {
            return base.PutWebPageChannel(id, request);
        }

        /// <summary>
        /// Updates webpage channel with the data provided as input.
        /// </summary>
        /// <param name="sourceId">web page channel external source id</param>
        /// <param name="request">web page channel data</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated web page channel</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceChannelNames":[
        ///         "The ServiceChannelNames field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("WebPage/sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", @"^[A-Za-z0-9-.]*$")]
        [ProducesResponseType(typeof(V3VmOpenApiWebPageChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated web page channel.", type: typeof(V3VmOpenApiWebPageChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutWebPageChannelBySource(string sourceId, [FromBody]V3VmOpenApiWebPageChannelInBase request)
        {
            return base.PutWebPageChannelBySource(sourceId, request);
        }

        /// <summary>
        /// Creates a new printable form channel with the data provided as input.
        /// </summary>
        /// <param name="request">The printable form channel data.</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The created printable form channel.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceChannelNames":[
        ///         "The ServiceChannelNames field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        // POST api/ServiceChannel/PrintableForm
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost("PrintableForm")]
        [ProducesResponseType(typeof(V3VmOpenApiPrintableFormChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created printable form channel.", type: typeof(V3VmOpenApiPrintableFormChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PostPrintableFormChannel([FromBody]V3VmOpenApiPrintableFormChannelIn request)
        {
            return base.PostPrintableFormChannel(request);
        }

        /// <summary>
        /// Updates printable form channel with the data provided as input.
        /// </summary>
        /// <param name="id">printable form channel id</param>
        /// <param name="request">printable form channel data</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated printable form channel</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceChannelNames":[
        ///         "The ServiceChannelNames field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("PrintableForm/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V3VmOpenApiPrintableFormChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated printable form channel.", type: typeof(V3VmOpenApiPrintableFormChannel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult PutPrintableFormChannel(string id, [FromBody]V3VmOpenApiPrintableFormChannelInBase request)
        {
            return base.PutPrintableFormChannel(id, request);
        }

        /// <summary>
        /// Updates printable form channel with the data provided as input.
        /// </summary>
        /// <param name="sourceId">printable form channel external source id</param>
        /// <param name="request">printable form channel data</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated printable form channel</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceChannelNames":[
        ///         "The ServiceChannelNames field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("PrintableForm/sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", @"^[A-Za-z0-9-.]*$")]
        [ProducesResponseType(typeof(V3VmOpenApiPrintableFormChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated printable form channel.", type: typeof(V3VmOpenApiPrintableFormChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutPrintableFormChannelBySource(string sourceId, [FromBody]V3VmOpenApiPrintableFormChannelInBase request)
        {
            return base.PutPrintableFormChannelBySource(sourceId, request);
        }

        /// <summary>
        /// Creates a new service location channel with the data provided as input.
        /// </summary>
        /// <param name="request">The service location channel data.</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The created service location channel.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceChannelNames":[
        ///         "The ServiceChannelNames field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        // POST api/ServiceChannel/ServiceLocation
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost("ServiceLocation")]
        [ProducesResponseType(typeof(V3VmOpenApiServiceLocationChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created service location channel.", type: typeof(V3VmOpenApiServiceLocationChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PostServiceLocationChannel([FromBody]V3VmOpenApiServiceLocationChannelIn request)
        {
            return base.PostServiceLocationChannel(request);
        }

        /// <summary>
        /// Updates a new service location channel with the data provided as input.
        /// </summary>
        /// <param name="id">service location channel id</param>
        /// <param name="request">service location channel data.</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated service location channel.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceChannelNames":[
        ///         "The ServiceChannelNames field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("ServiceLocation/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V3VmOpenApiServiceLocationChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service location channel.", type: typeof(V3VmOpenApiServiceLocationChannel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutServiceLocationChannel(string id, [FromBody]V3VmOpenApiServiceLocationChannelInBase request)
        {
            return base.PutServiceLocationChannel(id, request);
        }


        /// <summary>
        /// Updates a new service location channel with the data provided as input.
        /// </summary>
        /// <param name="sourceId">service location channel external source id</param>
        /// <param name="request">service location channel data</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated service location channel</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceChannelNames":[
        ///         "The ServiceChannelNames field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [Authorize(ActiveAuthenticationSchemes = "Bearer")][AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("ServiceLocation/sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", @"^[A-Za-z0-9-.]*$")]
        [ProducesResponseType(typeof(V3VmOpenApiServiceLocationChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service location channel", type: typeof(V3VmOpenApiServiceLocationChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutServiceLocationChannelBySource(string sourceId, [FromBody]V3VmOpenApiServiceLocationChannelInBase request)
        {
            return base.PutServiceLocationChannelBySource(sourceId, request);
        }
    }
}
