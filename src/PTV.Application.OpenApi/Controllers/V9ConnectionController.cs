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

using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using PTV.Framework.Attributes;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Framework.Enums;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.OpenApi.V9;
using Swashbuckle.AspNetCore.Annotations;
using PTV.Domain.Model.Models.OpenApi.Extensions;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api service and service channel connection related methods.
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
    [Route("api/v9/Connection")]
    [Route("api/v10/Connection")]
    public class V9ConnectionController : ServiceAndChannelBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceAndChannelService">The service and channel service.</param>
        /// <param name="serviceService">The service service.</param>
        /// <param name="channelService">The channel service.</param>
        /// <param name="userOrganizationService">The user organization service</param>
        /// <param name="codeService"></param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        public V9ConnectionController(
            IServiceAndChannelService serviceAndChannelService,
            IServiceService serviceService,
            IChannelService channelService,
            IUserOrganizationService userOrganizationService,
            ICodeService codeService,
            IOptions<AppSettings> settings,
            ILogger<V9ConnectionController> logger)
            : base(serviceAndChannelService, serviceService, channelService, userOrganizationService, codeService, settings, logger, 9)
        {
        }

        /// <summary>
        /// Creates a connections between services and service channels with extra data.
        /// </summary>
        /// <param name="request">A list of services and service channels.</param>
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
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost]
        [ProducesResponseType(typeof(IList<string>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of messages about succesfull and unsuccesfull addings.", type: typeof(List<string>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(IList<string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PostServiceAndChannel([FromBody]List<V9VmOpenApiServiceAndChannelIn> request)
        {
            return base.PostServiceAndChannel<V9VmOpenApiServiceAndChannelIn, V9VmOpenApiContactDetailsIn, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(request);
        }

        /// <summary>
        /// Updates connections between a service and service channels with extra data. 
        /// Request includes service channels for one certain service so regular connections missing from request are removed.
        /// ASTI connections are left as they are.
        /// To delete all regular service channel connections for a service, set 'deleteAllChannelRelations' property to true.
        /// </summary>
        /// <param name="serviceId">Service identifier</param>
        /// <param name="request">A list of service channels.</param>
        /// <returns>The service with updated connections for service channels.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "Service with id '00000000-0000-0000-0000-00000000' not found!"
        /// }
        /// </code>
        /// </remarks>
        // POST api/service
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("serviceId/{serviceId}")]
        [ValidateId("serviceId")]
        [ProducesResponseType(typeof(V9VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service.", type: typeof(V9VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(IList<string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutServiceAndChannel(string serviceId, [FromBody]V9VmOpenApiServiceAndChannelRelationInBase request)
        {
            return PutServiceConnections(serviceId, request, false);
        }

        /// <summary>
        /// Creates a connections between services and service channels with extra data. External source ids are used.
        /// </summary>
        /// <param name="request">A list of services and service channels.</param>
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
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost("Source")]
        [ProducesResponseType(typeof(IList<string>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of messages about succesfull and unsuccesfull addings.", type: typeof(List<string>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(IList<string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PostServiceAndChannelBySource([FromBody]List<V9VmOpenApiServiceAndChannelRelationBySource> request)
        {
            return PostServiceAndChannelBySource<V9VmOpenApiServiceAndChannelRelationBySource, V9VmOpenApiContactDetailsIn, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(request);
        }

        /// <summary>
        /// Updates connections between a service and service channels with extra data. External source ids are used.
        /// Request includes service channels for one certain service so service channels missing from request are removed.
        /// To delete all service channel connections for a service set 'deleteAllChannelRelations' property to true.
        /// ASTI connections are not removed - data for those connections can be updated though.
        /// </summary>
        /// <param name="serviceSourceId">External source identifier for service</param>
        /// <param name="request">A list of service channels.</param>
        /// <returns>The service with updated connections for service channels.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "Service with id '00000000-0000-0000-0000-00000000' not found!"
        /// }
        /// </code>
        /// </remarks>
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("serviceSourceId/{serviceSourceId}")]
        [ValidateRegEx("serviceSourceId", ValidationConsts.ExternalSource)]
        [ProducesResponseType(typeof(V9VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service.", type: typeof(V9VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(IList<string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutServiceAndChannelBySource(string serviceSourceId, [FromBody]V9VmOpenApiServiceAndChannelRelationBySourceInBase request)
        {
            return PutServiceConnectionsBySource(serviceSourceId, request);
        }

        /// <summary>
        /// Updates connections between a service and service channels with extra data. 
        /// Request includes service channels for one certain service and missing ASTI connections are removed. Regular connections are left as they are.
        /// To delete all ASTI service channel connections for a service, set 'deleteAllChannelRelations' property to true.
        /// This is special endpoint for ASTI and users need to have special access right to be able use it.
        /// </summary>
        /// <param name="serviceId">Service identifier</param>
        /// <param name="request">A list of service channels.</param>
        /// <returns>The service with updated connections for service channels.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "Service with id '00000000-0000-0000-0000-00000000' not found!"
        /// }
        /// </code>
        /// </remarks>
        // POST api/service
        [AccessRightRequirement(AccessRightEnum.ASTIWrite)]
        [HttpPut("ASTI/serviceId/{serviceId}")]
        [ValidateId("serviceId")]
        [ProducesResponseType(typeof(V9VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service.", type: typeof(V9VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(IList<string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult ASTIPutServiceAndChannel(string serviceId, [FromBody]V9VmOpenApiServiceAndChannelRelationAstiInBase request)
        {
            return PutAstiServiceConnections(serviceId, request);
        }

        /// <summary>
        /// Updates connections between a service and service channels with extra data. External source ids are used.
        /// Request includes service channels for one certain service and missing ASTI connections are removed. Regular connections are left as they are.
        /// To delete all ASTI service channel connections for a service set 'deleteAllChannelRelations' property to true.
        /// This is special endpoint for ASTI and users need to have special access right to be able use it.
        /// </summary>
        /// <param name="serviceSourceId">External source identifier for service</param>
        /// <param name="request">A list of service channels.</param>
        /// <returns>The service with updated connections for service channels.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "Service with id '00000000-0000-0000-0000-00000000' not found!"
        /// }
        /// </code>
        /// </remarks>
        [AccessRightRequirement(AccessRightEnum.ASTIWrite)]
        [HttpPut("ASTI/serviceSourceId/{serviceSourceId}")]
        [ValidateRegEx("serviceSourceId", ValidationConsts.ExternalSource)]
        [ProducesResponseType(typeof(V9VmOpenApiService), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service.", type: typeof(V9VmOpenApiService))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(IList<string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult ASTIPutServiceAndChannelBySource(string serviceSourceId, [FromBody]V9VmOpenApiServiceAndChannelRelationBySourceAsti request)
        {
            return PutAstiServiceConnectionsBySource(serviceSourceId, request);
        }

        /// <summary>
        /// Updates connections between a service channel and services with extra data.
        /// Request includes services for one certain service channel and missing ASTI connections are removed. Regular connections are left as they are.
        /// To delete all ASTI connections for a service channel set 'deleteAllServiceRelations' property to true.
        /// This is special endpoint for ASTI and users need to have special access right to be able use it.
        /// </summary>
        /// <param name="serviceChannelId">Service channel identifier</param>
        /// <param name="request">A list of service channels.</param>
        /// <returns>The service with updated connections for service channels.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "Service with id '00000000-0000-0000-0000-00000000' not found!"
        /// }
        /// </code>
        /// </remarks>
        // POST api/service
        [AccessRightRequirement(AccessRightEnum.ASTIWrite)]
        [HttpPut("ASTI/serviceChannelId/{serviceChannelId}")]
        [ValidateId("serviceChannelId")]
        [ProducesResponseType(typeof(V9VmOpenApiServiceChannels), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated serviceChannel.", type: typeof(V9VmOpenApiServiceChannels))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(IList<string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult ASTIPutServiceAndChannelByChannel(string serviceChannelId, [FromBody]V9VmOpenApiChannelServicesIn request)
        {
            return PutChannelConnections(serviceChannelId, request);
        }
    }
}
