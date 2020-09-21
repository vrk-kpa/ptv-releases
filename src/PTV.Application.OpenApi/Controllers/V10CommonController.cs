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
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api services related methods.
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v10/Common")]
    [Route("api/v11/Common")]
    public class V10CommonController : BaseController
    {
        private readonly ICommonService commonService;
        private readonly ITranslationService translationService;
        private readonly ITasksService tasksService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonController"/> class.
        /// </summary>
        /// <param name="commonService">The common service.</param>
        /// <param name="userOrganizationService">The user organization service</param>
        /// <param name="translationService">The translation service</param>
        /// <param name="tasksService">The tasks service</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        public V10CommonController(
            ICommonService commonService,
            IUserOrganizationService userOrganizationService,
            ITranslationService translationService,
            ITasksService tasksService,
            IOptions<AppSettings> settings,
            ILogger<V10CommonController> logger) : base(userOrganizationService, settings, logger)
        {
            this.commonService = commonService;
            this.translationService = translationService;
            this.tasksService = tasksService;
        }

        /// <summary>
        /// Gets a list of published services and service channels by organization.
        /// Services/channels created/modified after certain date can be fetched by adding date as query string parameter.
        /// Services/channels created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// </summary>
        /// <param name="organizationId">Guid</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <returns>A list of services and channels related to given organization.</returns>
        [AllowAnonymous]
        [HttpGet("EntitiesByOrganization/{organizationId}")]
        [ValidateId("organizationId")]
        [ValidateDate]
        [ValidateDate("dateBefore")]
        [ProducesResponseType(typeof(VmOpenApiEntityGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service and channel ids with paging.", type: typeof(VmOpenApiEntityGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetServicesAndChannels(string organizationId, [FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1)
        {
            var guid = organizationId.ParseToGuidWithExeption();

            // check if the organization exists with the given id
            if (!commonService.OrganizationExists(guid, PublishingStatus.Published))
            {
                return NotFound(new VmError { ErrorMessage = $"Organization with id '{guid}' not found." });
            }

            return Ok(commonService.GetServicesAndChannelsByOrganization(guid, true, date, page, PageSize, dateBefore));
        }

        /// <summary>
        /// Gets information of translation processes within PTV.
        /// Translation items created/modified after certain date can be fetched by adding date as query string parameter.
        /// Translation items created/modified before certain date can be fetched by adding dateBefore as query string parameter.
        /// </summary>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <returns>A list of information about translation processes.</returns>
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [HttpGet("Translation")]
        [ValidateDate]
        [ValidateDate("dateBefore")]
        [ProducesResponseType(typeof(VmOpenApiTranslationItemsPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of translation process items with paging.", type: typeof(VmOpenApiTranslationItemsPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetTranslationStatistics([FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1)
        {
            return Ok(translationService.GetTranslationItems(date, dateBefore, page, PageSize));
        }

        /// <summary>
        /// Gets information of user's organization's expiring services.
        /// </summary>
        /// <param name="page">The page number to be fetched.</param>
        /// <param name="status">Set status to get items either in published or draft state.</param>
        /// <returns>A list of expiring services.</returns>
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [HttpGet("ExpiringServices")]
        [ValidateEnum("status", typeof(TaskEntityStatusEnum), false)]
        [ProducesResponseType(typeof(VmOpenApiTasks), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of organization's expiring services.", type: typeof(VmOpenApiTasks))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetExpiringServices([FromQuery]int page = 1, [FromQuery]string status = "Published")
        {
            var taskStatus = TasksIdsEnum.OutdatedPublishedServices;
            if (status == TaskEntityStatusEnum.Draft.ToString())
            {
                taskStatus = TasksIdsEnum.OutdatedDraftServices;
            }
            return Ok(tasksService.GetTasks(taskStatus, page, PageSize));
        }

        /// <summary>
        /// Gets information of user's organization's expiring service channels.
        /// </summary>
        /// <param name="page">The page number to be fetched.</param>
        /// <param name="status">Set status to get items either in published or draft state.</param>
        /// <returns>A list of expiring service channels.</returns>
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [HttpGet("ExpiringServiceChannels")]
        [ValidateEnum("status", typeof(TaskEntityStatusEnum), false)]
        [ProducesResponseType(typeof(VmOpenApiTasks), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of organization's expiring service channels.", type: typeof(VmOpenApiTasks))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetExpiringServiceChannels([FromQuery]int page = 1, [FromQuery]string status = "Published")
        {
            var taskStatus = TasksIdsEnum.OutdatedPublishedChannels;
            if (status == TaskEntityStatusEnum.Draft.ToString())
            {
                taskStatus = TasksIdsEnum.OutdatedDraftChannels;
            }
            return Ok(tasksService.GetTasks(taskStatus, page, PageSize));
        }

        /// <summary>
        /// Gets information of user's organization's services that have no connections to channels.
        /// </summary>
        /// <param name="page">The page number to be fetched.</param>
        /// <returns>A list of services.</returns>
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [HttpGet("ServicesWithoutChannels")]
        [ValidateEnum("status", typeof(TaskEntityStatusEnum), false)]
        [ProducesResponseType(typeof(VmOpenApiTasks), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of services.", type: typeof(VmOpenApiTasks))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetServicesWithoutChannels([FromQuery]int page = 1)
        {
            return Ok(tasksService.GetOrphanItemsTasks(TasksIdsEnum.ServicesWithoutChannels, page, PageSize));
        }

        /// <summary>
        /// Gets information of user's organization's channels that have no connections to services.
        /// </summary>
        /// <param name="page">The page number to be fetched.</param>
        /// <returns>A list of services.</returns>
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [HttpGet("ChannelsWithoutServices")]
        [ValidateEnum("status", typeof(TaskEntityStatusEnum), false)]
        [ProducesResponseType(typeof(VmOpenApiTasks), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of services.", type: typeof(VmOpenApiTasks))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetChannelsWithoutServices([FromQuery]int page = 1)
        {
            return Ok(tasksService.GetOrphanItemsTasks(TasksIdsEnum.ChannelsWithoutServices, page, PageSize));
        }
    }
}
