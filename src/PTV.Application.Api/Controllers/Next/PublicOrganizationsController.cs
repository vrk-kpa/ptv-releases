using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Next.Model;

namespace PTV.Application.Api.Controllers.Next
{
    [AllowAnonymous]
    [Route("api/next/public/organizations")]
    public class PublicOrganizationsController : BaseController
    {
        private readonly IOrganizationQueries queries;

        public PublicOrganizationsController(ILogger<PublicOrganizationsController> logger, 
            IOrganizationQueries queries) : base(logger)
        {
            this.queries = queries;
        }

        [HttpGet("search")]
        public IActionResult SearchOrganizations([FromQuery]OrganizationSearchModel parameters)
        {
            var data = queries.Search(parameters);
            return Ok(data);
        }
    }
}
