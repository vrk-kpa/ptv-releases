using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V10;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using PTV.Framework.Attributes;
using PTV.Framework.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api service collections related methods.
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v9/ServiceCollection")]
    [Route("api/v10/ServiceCollection")]
    public class V7ServiceCollectionController : ServiceCollectionBaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="V7ServiceCollectionController"/> class.
        /// </summary>
        /// <param name="serviceCollectionService">The service collection service.</param>
        /// <param name="organizationService">The organization service.</param>
        /// <param name="commonService">The common service.</param>
        /// <param name="serviceService">The service service.</param>
        /// <param name="userOrganizationService">The user organization service.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="channelService">The settings.</param>
        /// <param name="logger">The logger.</param>
        public V7ServiceCollectionController(
            IServiceCollectionService serviceCollectionService,
            IOrganizationService organizationService,
            ICommonService commonService,
            IServiceService serviceService,
            IUserOrganizationService userOrganizationService,
            IOptions<AppSettings> settings,
            IChannelService channelService,
            ILogger<V7ServiceCollectionController> logger)
            : base(serviceCollectionService, organizationService, commonService, serviceService, userOrganizationService, settings, channelService, logger, 7)
        {
        }

        /// <summary>
        /// Gets all the published service collections within PTV as a list of service collection ids and names.
        /// Service collections created after certain date can be fetched by adding date as query string parameter.
        /// Service collections created before certain date can be fetched by adding dateBefore as query string parameter.
        /// Archived items can be fetched by setting parameter archived to true.
        /// </summary>
        /// <param name="date">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC).</param>
        /// <param name="dateBefore">Supports only format "yyyy-MM-ddTHH:mm:ss" (UTC)</param>
        /// <param name="page">The page number to be fetched.</param>
        /// <param name="archived">Get archived items by setting archived to true.</param>
        /// <returns>A list of service collection ids and names with paging.</returns>
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
        [HttpGet("/api/v9/ServiceCollection")]
        [HttpGet("/api/v10/ServiceCollection")]
        [ValidateDate]
        [ProducesResponseType(typeof(V3VmOpenApiGuidPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of service collection ids with paging.", type: typeof(V3VmOpenApiGuidPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetV8([FromQuery]DateTime? date, [FromQuery]DateTime? dateBefore, [FromQuery]int page = 1, [FromQuery]bool archived = false)
        {
            return GetIdAndNameList(date, page, archived, dateBefore);
        }

        /// <summary>
        /// Fetches all the information related to a single service collection.
        /// </summary>
        /// <param name="id">Guid</param>
        /// <returns>Detailed information about a service collection.</returns>
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
        [ProducesResponseType(typeof(V7VmOpenApiServiceCollection), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about a service collection.", type: typeof(V7VmOpenApiServiceCollection))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult Get(string id)
        {
            return GetById(id);
        }

        /// <summary>
        /// Gets the published service collections within PTV related to certain organization. Either organizationId or code needs to be added as a parameter.
        /// </summary>
        /// <param name="organizationId">Organization guid.</param>
        /// <param name="code">Organization business code.</param>
        /// <param name="page">The page to be fetched.</param>
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
        [HttpGet("/api/v10/ServiceCollection/organization")]
        [ProducesResponseType(typeof(V10VmOpenApiServiceCollectionsWithPaging), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "Detailed information about services.", type: typeof(V10VmOpenApiServiceCollectionsWithPaging))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetByOrganization([FromQuery]string organizationId = null, [FromQuery]string code = null, [FromQuery]int page = 1)
        {
            return GetByOrganizationBase(organizationId, code, page);
        }

        /// <summary>
        /// Creates a new service collection with the data provided as input.
        /// </summary>
        /// <param name="request">The service collection data.</param>
        /// <returns>List of service collections related to given organization.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceCollectionNames": [
        ///         "Type - Required value 'Name' was not found!"
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        // POST api/serviceCollection
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPost]
        [ProducesResponseType(typeof(V7VmOpenApiServiceCollection), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The created service collection.", type: typeof(V7VmOpenApiServiceCollection))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "Invalid version number.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult Post([FromBody]V7VmOpenApiServiceCollectionIn request)
        {
            return base.Post(request);
        }

        /// <summary>
        /// Updates the defined service collection with the data provided as input.
        /// </summary>
        /// <param name="id">Service collection identifier</param>
        /// <param name="request">The service collection data</param>
        /// <returns>The updated service collection.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceCollectionNames[0].Type": [
        ///         "The Type field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        // PUT api/serviceCollection/5
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("{id}")]
        [ValidateId("id")]
        [ProducesResponseType(typeof(V7VmOpenApiServiceCollection), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service collection.", type: typeof(V7VmOpenApiServiceCollection))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult Put(string id, [FromBody]V7VmOpenApiServiceCollectionInBase request)
        {
            return base.Put(request, id);
        }

        /// <summary>
        /// Updates the defined service collection with the data provided as input.
        /// </summary>
        /// <param name="sourceId">External source identifier</param>
        /// <param name="request">The service collection data</param>
        /// <returns>The updated service collection.</returns>
        /// <remarks>
        /// <para>HTTP status code 400 response model is a dictionary where key is property name and value is a list of error messages.</para>
        /// <code>
        /// {
        ///     "ServiceCollectionNames[0].Type": [
        ///         "The Type field is required."
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [ClaimRoleRequirement(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        [HttpPut("sourceId/{sourceId}")]
        [ValidateRegEx("sourceId", ValidationConsts.ExternalSource)]
        [ProducesResponseType(typeof(V7VmOpenApiServiceCollection), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "The updated service collection.", type: typeof(V7VmOpenApiServiceCollection))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: CoreMessages.OpenApi.UnauthorizedGeneralMessage)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, description: CoreMessages.OpenApi.ForbiddenGeneralMessage)]
        public IActionResult PutBySource(string sourceId, [FromBody]V7VmOpenApiServiceCollectionInBase request)
        {
            return base.Put(request, sourceId: sourceId);
        }
    }
}