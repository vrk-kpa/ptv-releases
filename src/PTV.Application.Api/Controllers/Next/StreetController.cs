using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Next.Address;
using PTV.Domain.Logic;
using PTV.Framework.Enums;
using PTV.Next.Model;

namespace PTV.Application.Api.Controllers.Next
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/next/address")]
    public class StreetController : BaseController
    {
        private readonly IAddressQuery addressQuery;

        public StreetController(ILogger<StreetController> logger,
            IAddressQuery addressQuery)
            : base(logger)
        {
            this.addressQuery = addressQuery;
        }

        [HttpPost("search-streets")]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IActionResult SearchStreets([FromBody] StreetSearchModel model)
        {
            var result = addressQuery.Search(model);
            return Ok(result);
        }

        [HttpGet("search-street-names")]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IActionResult SearchStreetNames([FromQuery] StreetNameSearchModel model)
        {
            var result = addressQuery.SearchStreetName(model);
            return Ok(result);
        }

        [HttpGet("validate-address")]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public StreetModel ValidateAddress([FromQuery] StreetValidateModel model)
        {
            var result = addressQuery.ValidateStreetAddress(model);
            return result;
        }
    }
}