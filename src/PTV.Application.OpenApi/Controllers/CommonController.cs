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
using PTV.Framework;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using PTV.Framework.Attributes;
using Microsoft.Extensions.Logging;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Enums;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api services related methods.
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer", Roles = "Eeva")] // PTV-2648: v7 is only allowed for Eeva in 1.6 release
    [Route("api/v7/Common")]
    public class CommonController : BaseController
    {
        private ICommonService commonService;

        private int pageSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonController"/> class.
        /// </summary>
        /// <param name="commonService">The common service.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        public CommonController(
            ICommonService commonService,
            IOptions<AppSettings> settings,
            ILogger<CommonController> logger) : base(settings, logger)
        {
            this.commonService = commonService;
            pageSize = Settings.PageSize > 0 ? Settings.PageSize : 1000;
        }


        /// <summary>
        /// Gets a list of published services and service channels by organization.
        /// Services/channels created/modified after certain date can be fetched by adding date as query string parameter.
        /// </summary>
        /// <param name="organizationId">Guid</param>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <returns>Detailed information about a service.</returns>
        //[AllowAnonymous] - PTV-2648: v7 is only allowed for Eeva in 1.6 release
        [HttpGet("EntitiesByOrganization/{organizationId}")]
        [ValidateId("organizationId")]
        [ValidateDate]
        [ProducesResponseType(typeof(VmOpenApiEntityGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service and channel ids with paging.", type: typeof(VmOpenApiEntityGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetServicesAndChannels(string organizationId, [FromQuery]DateTime? date, [FromQuery]int page = 1)
        {
            Guid guid = organizationId.ParseToGuidWithExeption();

            // check if the organization exists with the given id
            if (!commonService.OrganizationExists(guid, PublishingStatus.Published))
            {
                return NotFound(new VmError() { ErrorMessage = $"Organization with id '{guid}' not found." });
            }

            return Ok(commonService.GetServcesAndChannelsByOrganization(guid, date, page, pageSize));
        }
    }
}
