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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V10;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Attributes;
using PTV.Framework.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api service channels related methods.
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v10/ServiceChannel")]
    public class V10ServiceChannelController : ServiceChannelBaseController
    {
        /// <summary>
        /// ServiceChannelController constructor.
        /// </summary>
        public V10ServiceChannelController(
            IChannelService channelService,
            IOrganizationService organizationService,
            ICodeService codeService,
            IServiceService serviceService,
            IOptions<AppSettings> settings,
            ILogger<V10ServiceChannelController> logger,
            ICommonService commonService,
            IUserOrganizationService userOrganizationService)
            : base(channelService, organizationService, codeService, serviceService, settings, logger, commonService, 10, userOrganizationService)
        {
        }

        /// <summary>
        /// Gets all published service channels within PTV as a list of service channel ids and names.
        /// Service channels created/modified after certain date can be fetched by adding date as query string parameter
        /// Service channels created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// Archived items can be fetched by setting status parameter as 'Archived' and withdrawn items can be fetched by setting status parameter as 'Withdrawn'.
        /// </summary>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC)</param>
        /// <param name="organizationId">You can restrict the result set by setting organization guid.</param>
        /// <param name="code">You can restrict the result set by setting organization business code.</param>
        /// <param name="oid">You can restrict the result set by setting organization oid.</param>
        /// <param name="isVisibleForAll">When set to true only service channels marked as isVisibleForAll are returned.</param>
        /// <param name="page">The page to be fetched.</param>
        /// <param name="status">Set status to get items either in published, archived or withdrawn state.</param>
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
        [ValidateEnum("status", typeof(EntityStatusEnum), false)]
        [ValidateDate]
        [ValidateDate("dateBefore")]
        [ValidateId("organizationId", false)]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service channel ids and names with paging.", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Get([FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]string organizationId = null, [FromQuery]string code = null, [FromQuery]string oid = null, [FromQuery]bool isVisibleForAll = false, [FromQuery]int page = 1, [FromQuery]string status = "Published")
        {
            return GetInternal(date, dateBefore, organizationId, code, oid, page, status, isVisibleForAll);
        }

        /// <summary>
        /// Fetches all the information related to a single service channel.
        /// </summary>
        /// <param name="id">Guid</param>
        /// <returns>Detailed information about a service channel.</returns>
        /// <remarks>
        /// <para>Notice! The returned object is one of the following: <i>V10VmOpenApiElectronicChannel</i> or <i>V10VmOpenApiPhoneChannel</i> or
        ///  <i>V10VmOpenApiPrintableFormChannel</i> or <i>V10VmOpenApiServiceLocationChannel</i> or <i>V10VmOpenApiWebPageChannel</i></para>
        /// <para>The returned object depends on the type of the channel. For example if the channel is phone channel then V10VmOpenApiPhoneChannel object is returned.</para>
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
        [ProducesResponseType(typeof(V10VmOpenApiServiceChannels), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about a service channel.", type: typeof(V10VmOpenApiServiceChannels))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Get(string id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Fetches all the information related to requested service channels.
        /// </summary>
        /// <param name="guids">Comma separated list of guids. Max 100 can be added into list.</param>
        /// <returns>Detailed information about service channels.</returns>
        /// <remarks>
        /// <para>Notice! The returned object is one of the following: <i>V10VmOpenApiElectronicChannel</i> or <i>V10VmOpenApiPhoneChannel</i> or
        ///  <i>V10VmOpenApiPrintableFormChannel</i> or <i>V10VmOpenApiServiceLocationChannel</i> or <i>V10VmOpenApiWebPageChannel</i></para>
        /// <para>The returned item type within itemList depends on the type of the channel. For example if the channel is phone channel then V10VmOpenApiPhoneChannel object is returned.</para>
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
        [HttpGet("list")]
        [ProducesResponseType(typeof(IList<V10VmOpenApiServiceChannels>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about service channels.", type: typeof(IList<V10VmOpenApiServiceChannels>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByIdList([FromQuery]string guids)
        {
            return GetByIdListBase(guids);
        }

        /// <summary>
        /// Fetches all the information of service channels related to certain organization. Either organizationId, code or oid needs to be added as a parameter.
        /// </summary>
        /// <param name="organizationId">Organization guid.</param>
        /// <param name="code">Organization business code.</param>
        /// <param name="oid">Organization oid.</param>
        /// <param name="page">The page to be fetched.</param>
        /// <returns>Detailed information about service channels.</returns>
        /// <remarks>
        /// <para>Notice! The returned itemList object includes items which can be one of the following: <i>V10VmOpenApiElectronicChannel</i> or <i>V10VmOpenApiPhoneChannel</i> or
        ///  <i>V10VmOpenApiPrintableFormChannel</i> or <i>V10VmOpenApiServiceLocationChannel</i> or <i>V10VmOpenApiWebPageChannel</i></para>
        /// <para>The returned item type within itemList depends on the type of the channel. For example if the channel is phone channel then V10VmOpenApiPhoneChannel object is returned.</para>
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
        [HttpGet("list/organization")]
        [ProducesResponseType(typeof(V10VmOpenApiServiceChannelsWithPaging), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about service channels with paging.", type: typeof(V10VmOpenApiServiceChannelsWithPaging))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByOrganization([FromQuery]string organizationId = null, [FromQuery]string code = null, [FromQuery]string oid = null, [FromQuery]int page = 1)
        {
            return base.GetByOrganizationBase(organizationId, code, oid, page);
        }

        /// <summary>
        /// Gets a list of service channels related to defined area and code.
        /// Service channels created/modified after certain date can be fetched by adding date as query string parameter.
        /// Service channels created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// </summary>
        /// <param name="area">The area type</param>
        /// <param name="code">The area code</param>
        /// <param name="includeWholeCountry">Indicates if service channels marked for whole country (or whole country except Åland) should be included. </param>
        /// <param name="page">The page to be fetched.</param>
        /// <returns>A list of service channel ids with paging.</returns>
        /// <remarks>
        /// <para>Notice! The returned itemList object includes items which can be one of the following: <i>V10VmOpenApiElectronicChannel</i> or <i>V10VmOpenApiPhoneChannel</i> or
        ///  <i>V10VmOpenApiPrintableFormChannel</i> or <i>V10VmOpenApiServiceLocationChannel</i> or <i>V10VmOpenApiWebPageChannel</i></para>
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
        [HttpGet("list/area/{area}/code/{code}")]
        [ValidateEnum("area", typeof(AreaTypeEnum))]
        [ValidateRegEx("code", ValidationConsts.AreaCode)]
        [ProducesResponseType(typeof(V10VmOpenApiServiceChannelsWithPaging), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about service channels with paging.", type: typeof(V10VmOpenApiServiceChannelsWithPaging))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetAllDataByArea(string area, string code, [FromQuery]bool includeWholeCountry, [FromQuery]int page = 1)
        {
            return GetAllDataByAreaBase(area, code, includeWholeCountry, page);
        }


        /// <summary>
        /// Gets all service channels within PTV as a list of service channel ids and names. Also service channels with draft and modified versions are included.
        /// Service channels created/modified after certain date can be fetched by adding date as query string parameter.
        /// Service channels created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// Certain type of service channels can be fetched by setting query string parameter type.
        /// NOTE! This is a restricted endpoint.
        /// </summary>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="type">Service channel type</param>
        /// <param name="page">The page to be fetched.</param>
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
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiRead)]
        [HttpGet("active")]
        [ValidateDate]
        [ValidateDate("dateBefore")]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service channel ids and names with paging.", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetActive([FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]string type = null, [FromQuery]int page = 1)
        {
            if (string.IsNullOrEmpty(type))
            {
                return Ok(ChannelService.GetServiceChannels(date, page, PageSize, EntityStatusExtendedEnum.Active, dateBefore));
            }

            // Validate type if it is set
            var validator = new ValidEnumAttribute(typeof(ServiceChannelTypeEnum));
            if (!validator.IsValid(type))
            {
                ModelState.AddModelError("type", validator.FormatErrorMessage(null));
                return new BadRequestObjectResult(ModelState);
            }
            return base.GetGuidPageByType(type, date, page, false, dateBefore);
        }

        /// <summary>
        /// Fetches all the information related to a single service channel. Also service channels with only draft or modified versions are returned.
        /// NOTE! This is a restricted endpoint.
        /// </summary>
        /// <param name="id">Guid</param>
        /// <returns>Detailed information about a service channel.</returns>
        /// <remarks>
        /// <para>Notice! The returned object is one of the following: <i>V10VmOpenApiElectronicChannel</i> or <i>V10VmOpenApiPhoneChannel</i> or
        ///  <i>V10VmOpenApiPrintableFormChannel</i> or <i>V10VmOpenApiServiceLocationChannel</i> or <i>V10mOpenApiWebPageChannel</i></para>
        /// <para>The returned object depends on the type of the channel. For example if the channel is phone channel then V10VmOpenApiPhoneChannel object is returned.</para>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///    "id": [
        ///        "Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)."
        ///    ]
        /// }
        /// </code>
        /// </remarks>
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiRead)]
        [HttpGet("active/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V10VmOpenApiServiceChannels), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about a service channel.", type: typeof(V10VmOpenApiServiceChannels))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetActive(string id)
        {
            return base.GetById(id, false);
        }

        /// <summary>
        /// Gets a list of certain type of published service channels.
        /// Service channels created/modified after certain date can be fetched by adding date as query string parameter.
        /// Service channels created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// </summary>
        /// <param name="type">Service channel type</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page to be fetched.</param>
        /// <returns>A list of service channel ids with paging.</returns>
        /// <remarks>
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
        [ValidateDate("dateBefore")]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service channel ids with paging", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByType(string type, [FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1)
        {
            return base.GetGuidPageByType(type, date, page, dateBefore: dateBefore);
        }

        /// <summary>
        /// Gets a list of published service channels for defined organization.
        /// Service channels created/modified after certain date can be fetched by adding date as query string parameter.
        /// Service channels created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// </summary>
        /// <param name="organizationId">Guid</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page to be fetched.</param>
        /// <returns>A list of service channel ids with paging.</returns>
        /// <remarks>
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
        [ValidateDate("dateBefore")]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service channel ids with paging", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByOrganizationId(string organizationId, [FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1)
        {
            return base.GetGuidPageByOrganizationIdAndType(organizationId, null, date, page, dateBefore);
        }

        /// <summary>
        /// Gets a list of certain type of published service channels for defined organization.
        /// Service channels created/modified after certain date can be fetched by adding date as query string parameter.
        /// Service channels created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// </summary>
        /// <param name="organizationId">Guid</param>
        /// <param name="type">Service channel type</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page to be fetched.</param>
        /// <returns>A list of service channel ids with paging.</returns>
        /// <remarks>
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
        [ValidateDate("dateBefore")]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service channel ids with paging", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByOrganizationIdAndType(string organizationId, string type, [FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1)
        {
            return base.GetGuidPageByOrganizationIdAndType(organizationId, type, date, page, dateBefore);
        }

        /// <summary>
        /// Gets a list of service channels related to defined area and code.
        /// Service channels created/modified after certain date can be fetched by adding date as query string parameter.
        /// Service channels created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// </summary>
        /// <param name="area">The area type</param>
        /// <param name="code">The area code</param>
        /// <param name="includeWholeCountry">Indicates if service channels marked for whole country (or whole country except Åland) should be included. </param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page to be fetched.</param>
        /// <returns>A list of service channel ids with paging.</returns>
        /// <remarks>
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
        [HttpGet("area/{area}/code/{code}")]
        [ValidateEnum("area", typeof(AreaTypeEnum))]
        [ValidateRegEx("code", ValidationConsts.AreaCode)]
        [ValidateDate]
        [ValidateDate("dateBefore")]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service channel ids with paging", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByArea(string area, string code, bool includeWholeCountry, [FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1)
        {
            return GetGuidPageByArea(area, code, includeWholeCountry, date, page, dateBefore);
        }

        /// <summary>
        /// Creates a new electronic channel with the data provided as input.
        /// </summary>
        /// <param name="request">The electronic channel data.</param>
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
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost("EChannel")]
        [ProducesResponseType(typeof(V10VmOpenApiElectronicChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created electronic channel.", type: typeof(V10VmOpenApiElectronicChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PostEChannel([FromBody]V10VmOpenApiElectronicChannelIn request)
        {
            return base.PostEChannel(request);
        }

        /// <summary>
        /// Updates electronic channel with the data provided as input.
        /// </summary>
        /// <param name="id">electronic channel id</param>
        /// <param name="request">electronic channel data</param>
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
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("EChannel/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V10VmOpenApiElectronicChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated electronic channel", type: typeof(V10VmOpenApiElectronicChannel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutEChannel(string id, [FromBody]V10VmOpenApiElectronicChannelInBase request)
        {
            return base.PutEChannel(id, request);
        }

        /// <summary>
        /// Updates electronic channel with the data provided as input.
        /// </summary>
        /// <param name="sourceId">electronic channel external source id</param>
        /// <param name="request">electronic channel data</param>
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
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("EChannel/sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", ValidationConsts.ExternalSource)]
        [ProducesResponseType(typeof(V10VmOpenApiElectronicChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated electronic channel", type: typeof(V10VmOpenApiElectronicChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutEChannelBySource(string sourceId, [FromBody]V10VmOpenApiElectronicChannelInBase request)
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
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost("Phone")]
        [ProducesResponseType(typeof(V10VmOpenApiPhoneChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created phone channel.", type: typeof(V10VmOpenApiPhoneChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PostPhoneChannel([FromBody]V9VmOpenApiPhoneChannelIn request)
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
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut]
        [ValidateId("id")]
        [Route("Phone/{id}")]
        [ProducesResponseType(typeof(V10VmOpenApiPhoneChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated phone channel.", type: typeof(V10VmOpenApiPhoneChannel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutPhoneChannel(string id, [FromBody]V9VmOpenApiPhoneChannelInBase request)
        {
            return base.PutPhoneChannel(id, request);
        }


        /// <summary>
        /// Updates phone channel with the data provided as input.
        /// </summary>
        /// <param name="sourceId">phone channel external id</param>
        /// <param name="request">phone channel data</param>
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
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("Phone/sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", ValidationConsts.ExternalSource)]
        [ProducesResponseType(typeof(V10VmOpenApiPhoneChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated phone channel.", type: typeof(V10VmOpenApiPhoneChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutPhoneChannelBySource(string sourceId, [FromBody]V9VmOpenApiPhoneChannelInBase request)
        {
            return base.PutPhoneChannelBySource(sourceId, request);
        }

        /// <summary>
        /// Creates a new web page channel with the data provided as input.
        /// </summary>
        /// <param name="request">The web page channel data.</param>
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
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost("WebPage")]
        [ProducesResponseType(typeof(V10VmOpenApiWebPageChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created web page channel.", type: typeof(V10VmOpenApiWebPageChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PostWebPageChannel([FromBody]V10VmOpenApiWebPageChannelIn request)
        {
            return base.PostWebPageChannel(request);
        }

        /// <summary>
        /// Updates webpage channel with the data provided as input.
        /// </summary>
        /// <param name="id">web page channel id</param>
        /// <param name="request">web page channel data</param>
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
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("WebPage/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V10VmOpenApiWebPageChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated web page channel.", type: typeof(V10VmOpenApiWebPageChannel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutWebPageChannel(string id, [FromBody]V10VmOpenApiWebPageChannelInBase request)
        {
            return base.PutWebPageChannel(id, request);
        }

        /// <summary>
        /// Updates webpage channel with the data provided as input.
        /// </summary>
        /// <param name="sourceId">web page channel external source id</param>
        /// <param name="request">web page channel data</param>
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
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("WebPage/sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", ValidationConsts.ExternalSource)]
        [ProducesResponseType(typeof(V10VmOpenApiWebPageChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated web page channel.", type: typeof(V10VmOpenApiWebPageChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutWebPageChannelBySource(string sourceId, [FromBody]V10VmOpenApiWebPageChannelInBase request)
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
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost("PrintableForm")]
        [ProducesResponseType(typeof(V10VmOpenApiPrintableFormChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created printable form channel.", type: typeof(V10VmOpenApiPrintableFormChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PostPrintableFormChannel([FromBody]V10VmOpenApiPrintableFormChannelIn request)
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
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("PrintableForm/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V10VmOpenApiPrintableFormChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated printable form channel.", type: typeof(V10VmOpenApiPrintableFormChannel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutPrintableFormChannel(string id, [FromBody]V10VmOpenApiPrintableFormChannelInBase request)
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
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("PrintableForm/sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", ValidationConsts.ExternalSource)]
        [ProducesResponseType(typeof(V10VmOpenApiPrintableFormChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated printable form channel.", type: typeof(V10VmOpenApiPrintableFormChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutPrintableFormChannelBySource(string sourceId, [FromBody]V10VmOpenApiPrintableFormChannelInBase request)
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
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost("ServiceLocation")]
        [ProducesResponseType(typeof(V10VmOpenApiServiceLocationChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created service location channel.", type: typeof(V10VmOpenApiServiceLocationChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PostServiceLocationChannel([FromBody]V9VmOpenApiServiceLocationChannelIn request)
        {
            return base.PostServiceLocationChannel(request);
        }

        /// <summary>
        /// Updates service location channel with the data provided as input.
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
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("ServiceLocation/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V10VmOpenApiServiceLocationChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service location channel.", type: typeof(V10VmOpenApiServiceLocationChannel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutServiceLocationChannel(string id, [FromBody]V9VmOpenApiServiceLocationChannelInBase request)
        {
            return base.PutServiceLocationChannel(id, request);
        }

        /// <summary>
        /// Updates service location channel with the data provided as input.
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
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("ServiceLocation/sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", ValidationConsts.ExternalSource)]
        [ProducesResponseType(typeof(V10VmOpenApiServiceLocationChannel), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service location channel", type: typeof(V10VmOpenApiServiceLocationChannel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutServiceLocationChannelBySource(string sourceId, [FromBody]V9VmOpenApiServiceLocationChannelInBase request)
        {
            return base.PutServiceLocationChannelBySource(sourceId, request);
        }
    }
}
