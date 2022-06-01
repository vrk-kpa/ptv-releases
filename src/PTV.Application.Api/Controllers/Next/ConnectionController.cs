using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Next.Model;
using System;

namespace PTV.Application.Api.Controllers.Next
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/next/connection")]
    public class ConnectionController : BaseController
    {
        private readonly IServiceChannelConnectionQueries queries;
        private readonly IServiceChannelConnectionCommands commands;

        public ConnectionController(ILogger<ConnectionController> logger,
            IServiceChannelConnectionQueries queries,
            IServiceChannelConnectionCommands commands) : base(logger)
        {
            this.queries = queries;
            this.commands = commands;
        }

        [HttpGet("for-service")]
        public IActionResult GetConnection(Guid serviceId, Guid serviceChannelUnificRootId)
        {
            var result = queries.GetConnectionForService(serviceId, serviceChannelUnificRootId);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpDelete("remove-from-service-side")]
        public IActionResult RemoveConnection(Guid serviceId, Guid serviceChannelUnificRootId)
        {
            queries.RemoveConnection(serviceId, serviceChannelUnificRootId);
            return Ok();
        }

        [HttpPost("connect-service-to-channels")]
        public IActionResult ConnectServiceToChannels([FromBody]ConnectToChannelsModel model)
        {
            var result = queries.ConnectServiceToChannels(model);
            return Ok(result);
        }

        [HttpPost("save-connection")]
        public IActionResult SaveConnection([FromBody]ConnectionModel model)
        {
            commands.SaveConnection(model);
            var result = queries.GetConnectionForService(model.ServiceId, model.ServiceChannelUnificRootId);
            return Ok(result);
        }
    }
}