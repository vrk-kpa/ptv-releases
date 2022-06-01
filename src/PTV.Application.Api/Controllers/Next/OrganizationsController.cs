using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Next;

namespace PTV.Application.Api.Controllers.Next
{
    /// <summary>
    /// Controller for handling organization data for the new UI.
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/next/organizations")]
    public class OrganizationsController : BaseController
    {
        private readonly IOrganizationQueries queries;

        /// <summary>
        /// Creates a new instance of the "next" service controller.
        /// </summary>
        public OrganizationsController(ILogger<OrganizationsController> logger, IOrganizationQueries queries) : base(logger)
        {
            this.queries = queries;
        }

        /// <summary>
        /// Gets a list of organizations
        /// </summary>
        [HttpGet("byids")]
        public IActionResult GetByIds(List<Guid> ids)
        {
            return Ok(queries.Get(ids));
        }

        /// <summary>
        /// Gets a list of organizations
        /// </summary>
        [HttpGet("user-organizations-and-roles")]
        public IActionResult GetUserOrganizationAndRoles()
        {
            var data = queries.GetAllUserOrganizationsAndRoles();
            return Ok(data);
        }
    }
}