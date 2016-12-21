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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using Swashbuckle.SwaggerGen.Annotations;
using System;
using System.Collections.Generic;
using System.Net;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api service channels related methods.
    /// </summary>
    //[Route("api/v{version:range(2,2)}/[controller]")]
    [Route("api/[controller]")]
    public class ServiceChannelController : ServiceChannelBaseController
    {
        /// <summary>
        /// ServiceChannelController constructor.
        /// </summary>
        public ServiceChannelController(IChannelService channelService, IOrganizationService organizationService,
            ICodeService codeService, IOptions<AppSettings> settings, IFintoService fintoService) : base(channelService, organizationService, codeService, settings, fintoService)
        {
        }

        /// <summary>
        /// Gets all published service channels within PTV as a list of service channel ids.
        /// Service channels created/modified after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss"</param>
        /// <param name="page">The page to be fetched.</param>
        /// <param name="version">The open api version.</param>
        /// <returns>A list of service channel ids with paging.</returns>
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
        [SwaggerResponse(HttpStatusCode.OK, "A list of service channel ids with paging.", typeof(VmOpenApiGuidPage))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public override IActionResult Get([FromQuery]DateTime? date, [FromQuery]int page = 1)
        {
            return base.Get(date, page);
        }

        /// <summary>
        /// Fetches all the information related to a single service channel.
        /// </summary>
        /// <param name="id">Guid</param>
        /// <param name="version">The open api version.</param>
        /// <returns>Detailed information about a service channel.</returns>
        /// <remarks>
        /// <para>Notice! The returned object is one of the following: <i>VmOpenApiElectronicChannel</i> or <i>VmOpenApiPhoneChannel</i> or
        ///  <i>VmOpenApiPrintableFormChannel</i> or <i>VmOpenApiServiceLocationChannel</i> or <i>VmOpenApiWebPageChannel</i></para>
        ///  <para>The returned object depends on the type of the channel. For example if the channel is phone channel then VmOpenApiPhoneChannel object is returned.</para>
        ///  <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        ///  <code>
        ///  {
        ///     "id": [
        ///         "Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)."
        ///     ]
        ///  }
        ///  </code>
        /// </remarks>
        [HttpGet("~/api/[controller]/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(VmOpenApiServiceChannels), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "Detailed information about a service channel.", typeof(VmOpenApiServiceChannels))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.NotFound, CoreMessages.OpenApi.NotFoundGeneralMessage, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public IActionResult Get(string id)
        {
            // Notice! the said VmOpenApiServiceChannels return type is not correct on purpose, this has been agreed with PAHA so that they can see
            // all the possible return types

            Guid guid = id.ParseToGuidWithExeption();

            var sc = ChannelService.GetServiceChannel(guid);

            if (sc == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Service channel with id '{id}' not found." });
            }

            return Ok(sc);
        }

        /// <summary>
        /// Gets a list of certain type of published service channels.
        /// Service channels created/modified after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="type">Service channel type</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss"</param>
        /// <param name="version">The open api version.</param>
        /// <returns>Detailed information about certain type of service channels.</returns>
        /// <remarks>
        ///  <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        ///  <code>
        ///  {
        ///     "type": [
        ///         "The field is invalid. Please use one of these: 'EChannel, WebPage, PrintableForm, Phone, ServiceLocation'."
        ///     ]
        ///  }
        ///  </code>
        /// </remarks>
        [HttpGet("~/api/[controller]/type/{type}")]
        [ValidateServiceChannelType]
        [ValidateDate]
        [ProducesResponseType(typeof(VmOpenApiServiceChannels), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "Detailed information about certain type of service channels.", typeof(IList<VmOpenApiServiceChannels>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public IActionResult GetByType(string type, [FromQuery]DateTime? date)
        {
            var channelType = type.Parse<ServiceChannelTypeEnum>();
            return Ok(ChannelService.GetServiceChannelsByType(channelType, date));
        }

        /// <summary>
        /// Gets a list of published service channels for defined organization.
        /// Service channels created/modified after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="organizationId">Guid</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss"</param>
        /// <param name="version">The open api version.</param>
        /// <returns>Detailed information about service channels.</returns>
        /// <remarks>
        ///  <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        ///  <code>
        ///  {
        ///     "organizationId": [
        ///         "Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)."
        ///     ]
        ///  }
        ///  </code>
        /// </remarks>
        [HttpGet("~/api/[controller]/organization/{organizationId}")]
        [ValidateId("organizationId")]
        [ValidateDate]
        [ProducesResponseType(typeof(VmOpenApiServiceChannels), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "Detailed information about service channels.", typeof(IList<VmOpenApiServiceChannels>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.NotFound, CoreMessages.OpenApi.NotFoundGeneralMessage, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public IActionResult GetByOrganizationId(string organizationId, [FromQuery]DateTime? date)
        {
            Guid guid = organizationId.ParseToGuidWithExeption();

            // check if the organization exists with the given id
            if (!OrganizationService.OrganizationExists(guid))
            {
                return NotFound(new VmError() { ErrorMessage = $"Organization with id '{guid}' not found." });
            }

            return Ok(ChannelService.GetServiceChannelsByOrganization(guid, date));
        }

        /// <summary>
        /// Gets a list of certain type of published service channels for defined organization.
        /// Service channels created/modified after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="organizationId">Guid</param>
        /// <param name="type">Service channel type</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss"</param>
        /// <param name="version">The open api version.</param>
        /// <returns>Detailed information about service channels.</returns>
        /// <remarks>
        ///  <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        ///  <code>
        ///  {
        ///     "organizationId": [
        ///         "Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)."
        ///     ]
        ///  }
        ///  </code>
        /// </remarks>
        [HttpGet("~/api/[controller]/organization/{organizationId}/type/{type}")]
        [ValidateId("organizationId")]
        [ValidateServiceChannelType]
        [ValidateDate]
        [ProducesResponseType(typeof(VmOpenApiServiceChannels), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "Detailed information about service channels.", typeof(IList<VmOpenApiServiceChannels>))]
        [SwaggerResponse(HttpStatusCode.NotFound, CoreMessages.OpenApi.NotFoundGeneralMessage, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public IActionResult GetByOrganizationIdAndType(string organizationId, string type, [FromQuery]DateTime? date)
        {
            Guid guid = organizationId.ParseToGuidWithExeption();

            // check if the organization exists with the given id
            if (!OrganizationService.OrganizationExists(guid))
            {
                return NotFound(new VmError() { ErrorMessage = $"Organization with id '{guid}' not found." });
            }

            ServiceChannelTypeEnum channelType = type.Parse<ServiceChannelTypeEnum>();
            return Ok(ChannelService.GetServiceChannelsByOrganization(guid, date, channelType));
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPost("~/api/[controller]/EChannel")]
        [ProducesResponseType(typeof(VmOpenApiElectronicChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The created electronic channel.", typeof(VmOpenApiElectronicChannel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult PostEChannel([FromBody]VmOpenApiElectronicChannelIn request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null)
            {
                return validationResult;
            }
            return Ok(ChannelService.AddElectronicChannel(request, Settings.AllowAnonymous));
        }

        /// <summary>
        /// Updates an electronic channel with the data provided as input.
        /// </summary>
        /// <param name="id">electronic channel id</param>
        /// <param name="request">electronic channel data</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated electronic channel.</returns>
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPut("EChannel/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(VmOpenApiElectronicChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The updated electronic channel.", typeof(VmOpenApiElectronicChannel))]
        [SwaggerResponse(HttpStatusCode.NotFound, CoreMessages.OpenApi.NotFoundGeneralMessage, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult PutEChannel(string id, [FromBody]VmOpenApiElectronicChannelInBase request)
        {
            // check that the channel exists
            if (!CheckChannelExists(id))
            {
                return NotFound(new VmError() { ErrorMessage = $"Electronic channel with id '{id}' not found." });
            }

            var validationResult = ValidateRequest(request);
            if (validationResult != null)
            {
                return validationResult;
            }

            request.Id = id.ParseToGuid();
            return Ok(ChannelService.SaveElectronicChannel(request, Settings.AllowAnonymous));
        }

        /// <summary>
        /// Updates an electronic channel with the data provided as input.
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPut("EChannel/sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", @"^[A-Za-z0-9-]*$")]
        [ProducesResponseType(typeof(VmOpenApiElectronicChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The updated electronic channel", typeof(VmOpenApiElectronicChannel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult PutEChannelBySource(string sourceId, [FromBody]VmOpenApiElectronicChannelInBase request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null)
            {
                return validationResult;
            }

            request.SourceId = sourceId;
            return Ok(ChannelService.SaveElectronicChannel(request, Settings.AllowAnonymous));
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPost("~/api/[controller]/Phone")]
        [ProducesResponseType(typeof(VmOpenApiPhoneChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The created phone channel.", typeof(VmOpenApiPhoneChannel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult PostPhoneChannel([FromBody]VmOpenApiPhoneChannelIn request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null)
            {
                return validationResult;
            }
            return Ok(ChannelService.AddPhoneChannel(request, Settings.AllowAnonymous));
        }

        /// <summary>
        /// Updates phone channel with the data provided as input.
        /// </summary>
        /// <param name="id">phone channel id</param>
        /// <param name="request">phone channel data</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated phone channel.</returns>
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPut]
        [ValidateId("id")]
        [Route("Phone/{id}")]
        [ProducesResponseType(typeof(VmOpenApiPhoneChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The updated phone channel.", typeof(VmOpenApiPhoneChannel))]
        [SwaggerResponse(HttpStatusCode.NotFound, CoreMessages.OpenApi.NotFoundGeneralMessage, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult PutPhoneChannel(string id, [FromBody]VmOpenApiPhoneChannelInBase request)
        {
            // check that the channel exists
            if (!CheckChannelExists(id))
            {
                return NotFound(new VmError() { ErrorMessage = $"Phone channel with id '{id}' not found." });
            }

            var validationResult = ValidateRequest(request);
            if (validationResult != null)
            {
                return validationResult;
            }

            request.Id = id.ParseToGuid();
            return Ok(ChannelService.SavePhoneChannel(request, Settings.AllowAnonymous));
        }

        /// <summary>
        /// Updates phone channel with the data provided as input.
        /// </summary>
        /// <param name="sourceId">phone channel external id</param>
        /// <param name="request">phone channel data</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated phone channel.</returns>
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPut("Phone/sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", @"^[A-Za-z0-9-]*$")]
        [ProducesResponseType(typeof(VmOpenApiPhoneChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The updated phone channel.", typeof(VmOpenApiPhoneChannel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult PutPhoneChannelBySource(string sourceId, [FromBody]VmOpenApiPhoneChannelInBase request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null)
            {
                return validationResult;
            }

            request.SourceId = sourceId;

            return Ok(ChannelService.SavePhoneChannel(request, Settings.AllowAnonymous));
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPost("~/api/[controller]/WebPage")]
        [ProducesResponseType(typeof(VmOpenApiWebPageChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The created web page channel.", typeof(VmOpenApiWebPageChannel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult PostWebPageChannel([FromBody]VmOpenApiWebPageChannelIn request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null)
            {
                return validationResult;
            }
            return Ok(ChannelService.AddWebPageChannel(request, Settings.AllowAnonymous));
        }

        /// <summary>
        /// Updates webpage channel with the data provided as input.
        /// </summary>
        /// <param name="id">web page channel id</param>
        /// <param name="request">web page channel data</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated web page channel.</returns>
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPut("WebPage/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(VmOpenApiWebPageChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The updated web page channel.", typeof(VmOpenApiWebPageChannel))]
        [SwaggerResponse(HttpStatusCode.NotFound, CoreMessages.OpenApi.NotFoundGeneralMessage, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult PutWebPageChannel(string id, [FromBody]VmOpenApiWebPageChannelInBase request)
        {
            // check that the channel exists
            if (!CheckChannelExists(id))
            {
                return NotFound(new VmError() { ErrorMessage = $"Web page channel with id '{id}' not found." });
            }

            var validationResult = ValidateRequest(request);
            if (validationResult != null)
            {
                return validationResult;
            }

            request.Id = id.ParseToGuid();
            return Ok(ChannelService.SaveWebPageChannel(request, Settings.AllowAnonymous));
        }

        /// <summary>
        /// Updates webpage channel with the data provided as input.
        /// </summary>
        /// <param name="sourceId">web page channel external source id</param>
        /// <param name="request">web page channel data</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated web page channel.</returns>
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPut("WebPage/sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", @"^[A-Za-z0-9-]*$")]
        [ProducesResponseType(typeof(VmOpenApiWebPageChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The updated web page channel.", typeof(VmOpenApiWebPageChannel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult PutWebPageChannelBySource(string sourceId, [FromBody]VmOpenApiWebPageChannelInBase request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null)
            {
                return validationResult;
            }

            request.SourceId = sourceId;
            return Ok(ChannelService.SaveWebPageChannel(request, Settings.AllowAnonymous));
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPost("~/api/[controller]/PrintableForm")]
        [ProducesResponseType(typeof(VmOpenApiPrintableFormChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The created printable form channel.", typeof(VmOpenApiPrintableFormChannel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult PostPrintableFormChannel([FromBody]VmOpenApiPrintableFormChannelIn request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null)
            {
                return validationResult;
            }
            return Ok(ChannelService.AddPrintableFormChannel(request, Settings.AllowAnonymous));
        }

        /// <summary>
        /// Updates printable form channel with the data provided as input.
        /// </summary>
        /// <param name="id">printable form channel id</param>
        /// <param name="request">printable form channel data</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated printable form channel.</returns>
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPut("PrintableForm/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(VmOpenApiPrintableFormChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The updated printable form channel.", typeof(VmOpenApiPrintableFormChannel))]
        [SwaggerResponse(HttpStatusCode.NotFound, CoreMessages.OpenApi.NotFoundGeneralMessage, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        public IActionResult PutPrintableFormChannel(string id, [FromBody]VmOpenApiPrintableFormChannelInBase request)
        {
            // check that the channel exists
            if (!CheckChannelExists(id))
            {
                return NotFound(new VmError() { ErrorMessage = $"Web page channel with id '{id}' not found." });
            }

            var validationResult = ValidateRequest(request);
            if (validationResult != null)
            {
                return validationResult;
            }

            request.Id = id.ParseToGuid();
            return Ok(ChannelService.SavePrintableFormChannel(request, Settings.AllowAnonymous));
        }

        /// <summary>
        /// Updates printable form channel with the data provided as input.
        /// </summary>
        /// <param name="sourceId">printable form channel external source id</param>
        /// <param name="request">printable form channel data</param>
        /// <param name="version">The open api version.</param>
        /// <returns>The updated printable form channel.</returns>
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPut("PrintableForm/sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", @"^[A-Za-z0-9-]*$")]
        [ProducesResponseType(typeof(VmOpenApiPrintableFormChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The updated printable form channel.", typeof(VmOpenApiPrintableFormChannel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult PutPrintableFormChannelBySource(string sourceId, [FromBody]VmOpenApiPrintableFormChannelInBase request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null)
            {
                return validationResult;
            }

            request.SourceId = sourceId;
            return Ok(ChannelService.SavePrintableFormChannel(request, Settings.AllowAnonymous));
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPost("~/api/[controller]/ServiceLocation")]
        [ProducesResponseType(typeof(VmOpenApiServiceLocationChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The created service location channel.", typeof(VmOpenApiServiceLocationChannel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult PostServiceLocationChannel([FromBody]VmOpenApiServiceLocationChannelIn request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null)
            {
                return validationResult;
            }
            return Ok(ChannelService.AddServiceLocationChannel(request, Settings.AllowAnonymous));
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPut("ServiceLocation/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(VmOpenApiServiceLocationChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The updated service location channel.", typeof(VmOpenApiServiceLocationChannel))]
        [SwaggerResponse(HttpStatusCode.NotFound, CoreMessages.OpenApi.NotFoundGeneralMessage, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult PutServiceLocationChannel(string id, [FromBody]VmOpenApiServiceLocationChannelInBase request)
        {
            // check that the channel exists
            if (!CheckChannelExists(id))
            {
                return NotFound(new VmError() { ErrorMessage = $"Service location channel with id '{id}' not found." });
            }

            var validationResult = ValidateRequest(request);
            if (validationResult != null)
            {
                return validationResult;
            }
            request.Id = id.ParseToGuid();
            return Ok(ChannelService.SaveServiceLocationChannel(request, Settings.AllowAnonymous));
        }

        /// <summary>
        /// Updates a new service location channel with the data provided as input.
        /// </summary>
        /// <param name="sourceId">service location channel external source id</param>
        /// <param name="request">service location channel data</param>
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
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [HttpPut("ServiceLocation/sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", @"^[A-Za-z0-9-]*$")]
        [ProducesResponseType(typeof(VmOpenApiServiceLocationChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse(HttpStatusCode.OK, "The updated service location channel.", typeof(VmOpenApiServiceLocationChannel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, CoreMessages.OpenApi.BadRequestGeneralMessage, typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, CoreMessages.OpenApi.InternalServerErrorDescripton, typeof(IVmError))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        public IActionResult PutServiceLocationChannelBySource(string sourceId, [FromBody]VmOpenApiServiceLocationChannelInBase request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null)
            {
                return validationResult;
            }
            request.SourceId = sourceId;
            return Ok(ChannelService.SaveServiceLocationChannel(request, Settings.AllowAnonymous));
        }
    }
}
