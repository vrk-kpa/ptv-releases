using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Next.Model;
using System;

namespace PTV.Application.Api.Controllers.Next
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/next/channel")]
    public class ChannelController : BaseController
    {
        private readonly ILegacyServiceChannelQueryWrapper queries;

        public ChannelController(ILogger<ChannelController> logger,
            ILegacyServiceChannelQueryWrapper queries) : base(logger)
        {
            this.queries = queries;
        }

        [HttpPost("connectable-channels")]
        public IActionResult SearchForConnectableChannels([FromBody]ConnectableChannelSearchModel searchParameters)
        {
            var result = queries.GetConnectableChannels(searchParameters);
            return Ok(result);
        }

        [HttpGet("latest-version")]
        public IActionResult GetChannelLatestVersionByUnificRootId(Guid unificRootId, bool includeConnections)
        {
            var result = queries.GetChannelLatestVersionByUnificRootId(unificRootId, includeConnections);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}