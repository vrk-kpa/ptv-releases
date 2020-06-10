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
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V9;
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
    [Route("api/v11/Service")]
    public class V11ServiceController : ServiceBaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="V9ServiceController"/> class.
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
        public V11ServiceController(
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
            ILogger<V11ServiceController> logger)
            : base(serviceService, commonService, codeService, settings, generalDescriptionService, fintoService, ontologyTermDataCache,
                  channelService, userOrganizationService, organizationService, logger, 11)
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
        /// <param name="showHeader"></param>
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
        [ProducesResponseType(typeof(V11VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about a service.", type: typeof(V11VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Get(string id, [FromQuery]bool showHeader = false)
        {
            return GetById(id, showHeader: showHeader);
        }

        /// <summary>
        /// Fetches all the information related to a single service. If general description is attached also general description data is returned within the service data.
        /// General description related descriptions are marked with prefix 'GD_' to separate them from service related descriptions.
        /// </summary>
        /// <param name="id">Guid</param>
        /// <param name="showHeader"></param>
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
        [HttpGet("serviceWithGD/{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V11VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about a service.", type: typeof(V11VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetWithGdData(string id, [FromQuery]bool showHeader = false)
        {
            return GetById(id, true, true, showHeader);
        }

        /// <summary>
        /// Fetches all the information related to requested services.
        /// </summary>
        /// <param name="guids">Comma separated list of guids. Max 100 can be added into list.</param>
        /// <param name="showHeader"></param>
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
        [ProducesResponseType(typeof(IList<V11VmOpenApiService>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about services.", type: typeof(IList<V11VmOpenApiService>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByIdList([FromQuery]string guids, [FromQuery]bool showHeader = false)
        {
            return GetByIdListBase(guids,false,  showHeader);
        }

        /// <summary>
        /// Fetches all the information related to requests services. If general description is attached to a service also general description data is returned within the service data.
        /// General description related descriptions are marked with prefix 'GD_' to separate them from service related descriptions.
        /// </summary>
        /// <param name="guids">Comma separated list of guids. Max 100 can be added into list.</param>
        /// <param name="showHeader"></param>
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
        [HttpGet("serviceWithGD/list")]
        [ProducesResponseType(typeof(IList<V11VmOpenApiService>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about services.", type: typeof(IList<V11VmOpenApiService>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByIdListWithGdData([FromQuery]string guids, [FromQuery]bool showHeader = false)
        {
            return GetByIdListBase(guids,true, showHeader);
        }

        /// <summary>
        /// Fetches all the information of the services related to certain organization. Either organizationId, code or oid needs to be added as a parameter.
        /// User can also set serviceWithGD parameter to true to include possible attached general description data into the service data.
        /// In this case general description related descriptions are marked with prefix 'GD_' to separate them from service related descriptions.
        /// </summary>
        /// <param name="organizationId">Organization guid.</param>
        /// <param name="code">Organization business code.</param>
        /// <param name="oid">Organization oid.</param>
        /// <param name="serviceWithGD">Indicates if general description data should be attached within the service data.</param>
        /// <param name="page">The page to be fetched.</param>
        /// <param name="showHeader"></param>
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
        [HttpGet("list/organization")]
        [ProducesResponseType(typeof(V11VmOpenApiServicesWithPaging), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about services.", type: typeof(V11VmOpenApiServicesWithPaging))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByOrganization([FromQuery]string organizationId = null, [FromQuery]string code = null, [FromQuery]string oid = null, [FromQuery]bool serviceWithGD = false, [FromQuery]int page = 1, [FromQuery]bool showHeader = false)
        {
            return GetByOrganizationBase(organizationId, code, oid, page, serviceWithGD, showHeader);
        }

        /// <summary>
        /// Fetches all the information of published services related to certain area and code.
        /// User can set serviceWithGD parameter to true to include possible attached general description data into the service data.
        /// In this case general description related descriptions are marked with prefix 'GD_' to separate them from service related descriptions.
        /// </summary>
        /// <param name="area">The area type.</param>
        /// <param name="code">The code related to area.</param>
        /// <param name="includeWholeCountry">Indicates if services marked to provide services for whole country (or whole country except Åland) should be included. </param>
        /// <param name="serviceWithGD">Indicates if general description data should be attached within the service data.</param>
        /// <param name="page">The page to be fetched.</param>
        /// <param name="showHeader"></param>
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
        [HttpGet("list/area/{area}/code/{code}")]
        [ValidateEnum("area", typeof(AreaTypeEnum))]
        [ValidateRegEx("code", ValidationConsts.AreaCode)]
        [ProducesResponseType(typeof(V11VmOpenApiServicesWithPaging), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about services.", type: typeof(V11VmOpenApiServicesWithPaging))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetAllDataByArea(string area = null, string code = null, [FromQuery]bool includeWholeCountry = false, [FromQuery]bool serviceWithGD = false, [FromQuery]int page = 1, [FromQuery]bool showHeader = false)
        {
            return GetByAreaBase(area, code, includeWholeCountry, serviceWithGD, showHeader, page);
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
        public IActionResult GetActive([FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1)
        {
            return GetIdAndNameList(date, page, EntityStatusExtendedEnum.Active, dateBefore);
        }

        /// <summary>
        /// Fetches all the information related to a single service. Also services with only draft or modified versions are returned.
        /// NOTE! This is a restricted endpoint.
        /// </summary>
        /// <param name="id">Guid</param>
        /// <param name="showHeader"></param>
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
        [ProducesResponseType(typeof(V11VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about a service.", type: typeof(V11VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetActive(string id, [FromQuery]bool showHeader = false)
        {
            return GetById(id, false, showHeader: showHeader);
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
        /// Gets a list of published services related to defined area and code.
        /// Services created/modified after certain date can be fetched by adding date as query string parameter.
        /// Services created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// </summary>
        /// <param name="area">The area type</param>
        /// <param name="code">The code related to area</param>
        /// <param name="includeWholeCountry">Indicates if services marked for whole country (or whole country except Åland) should be included. </param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <returns>A list of service ids with paging.</returns>
        [AllowAnonymous]
        [HttpGet("area/{area}/code/{code}")]
        [ValidateEnum("area", typeof(AreaTypeEnum))]
        [ValidateRegEx("code", ValidationConsts.AreaCode)]
        [ValidateDate]
        [ValidateDate("dateBefore")]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service ids with paging", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByArea(string area, string code, bool includeWholeCountry, [FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1)
        {
            return GetGuidPageByArea(area, code, includeWholeCountry, date, page, dateBefore);
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
        /// Gets a list of published services for defined industrial class.
        /// Services created/modified after certain date can be fetched by adding date as query string parameter.
        /// Services created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// </summary>
        /// <param name="uri">Industrial class uri, e.g. http://www.stat.fi/meta/luokitukset/toimiala/001-2008/46909 </param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <returns>A list of service ids with paging.</returns>
        [AllowAnonymous]
        [HttpGet("industrialClass")]
        [ValidateRegEx("uri", ValidationConsts.IndustrialClass)]
        [ValidateDate]
        [ValidateDate("dateBefore")]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service ids with paging.", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: null, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByIndustrialClass([FromQuery]string uri, [FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1)
        {
            return GetByIndustrialClassBase(uri, date, page, dateBefore);
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
        [ProducesResponseType(typeof(V11VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created service.", type: typeof(V11VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult Post([FromBody]V9VmOpenApiServiceIn request, bool attachProposedChannels)
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
        [ProducesResponseType(typeof(V11VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service.", type: typeof(V11VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult Put(string id, [FromBody]V9VmOpenApiServiceInBase request, bool attachProposedChannels)
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
        [ProducesResponseType(typeof(V11VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service.", type: typeof(V11VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutBySource(string sourceId, [FromBody]V9VmOpenApiServiceInBase request, bool attachProposedChannels)
        {
            return base.Put(request, sourceId: sourceId, attachProposedChannels: attachProposedChannels);
        }

    }
}
