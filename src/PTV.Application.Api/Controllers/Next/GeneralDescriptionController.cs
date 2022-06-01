using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Next.Model;
using PTV.Domain.Model.Enums;

namespace PTV.Application.Api.Controllers.Next
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/next/gd")]
    public class GeneralDescriptionController : BaseController
    {
        private readonly IGeneralDescriptionQueries queries;

        public GeneralDescriptionController(ILogger<OrganizationsController> logger,
            IGeneralDescriptionQueries queries) : base(logger)
        {
            this.queries = queries;
        }

        [HttpPost("search")]
        public IActionResult Search([FromBody] GdSearchModel searchParameters)
        {
            var result = this.queries.Search(searchParameters);
            return Ok(result);
        }

        [HttpGet("versioned")]
        public IActionResult Get(Guid id, List<PublishingStatus> acceptedStatuses)
        {
            var result = this.queries.Get(id, acceptedStatuses);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("published")]
        public IActionResult Get(Guid unificRootId)
        {
            var result = this.queries.GetPublishedByUnificRootId(unificRootId);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}