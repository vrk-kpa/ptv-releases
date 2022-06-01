using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Next.Model;

namespace PTV.Application.Api.Controllers.Next
{
    /// <summary>
    /// Controller for handling service data for the new UI.
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/next/service")]
    public class ServiceController : BaseController
    {
        private readonly IServiceQueries serviceQueries;
        private readonly IServiceCommands serviceCommands;

        /// <summary>
        /// Creates a new instance of the "next" service controller.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceQueries"></param>
        /// <param name="serviceCommands"></param>
        public ServiceController(
            ILogger<ServiceController> logger, 
            IServiceQueries serviceQueries,
            IServiceCommands serviceCommands) : base(logger)
        {
            this.serviceQueries = serviceQueries;
            this.serviceCommands = serviceCommands;
        }

        /// <summary>
        /// Gets a service based on the provided ID. The return model corresponds to the new UI.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var result = serviceQueries.GetModel(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("save")]
        public IActionResult Save([FromBody] ServiceModel model)
        {
            var id = serviceCommands.Save(model);
            if (id == null)
            {
                return BadRequest();
            }

            var service = serviceQueries.GetModel(id.Value);
            return Ok(service);
        }

        [HttpGet("validate/{id}")]
        public IActionResult Validate(Guid id)
        {
            var result = serviceQueries.Validate(id);
            return Ok(result);
        }

        [HttpPost("publish/{id}")]
        public IActionResult Publish(Guid id, [FromBody] Dictionary<LanguageEnum, PublishingModel> model)
        {
            var resultId = serviceCommands.Publish(id, model);
            if (resultId == null)
            {
                return BadRequest();
            }
            
            var result = serviceQueries.GetModel(resultId.Value);
            return Ok(result);
        }

        [HttpPost("archive/{id}")]
        public IActionResult Archive(Guid id, [FromQuery] LanguageEnum? language)
        {
            var resultId = serviceCommands.Archive(id, language);
            if (resultId == null)
            {
                return BadRequest();
            }

            var result = serviceQueries.GetModel(resultId.Value);
            return Ok(result);
        }

        [HttpGet("withdraw/{id}")]
        public IActionResult Withdraw(Guid id, [FromQuery] LanguageEnum? language)
        {
            var resultId = serviceCommands.Withdraw(id, language);
            if (resultId == null)
            {
                return BadRequest();
            }
            
            var result = serviceQueries.GetModel(resultId.Value);
            return Ok(result);
        }

        [HttpPost("restore/{id}")]
        public IActionResult Restore(Guid id, [FromQuery] LanguageEnum? language)
        {
            var resultId = serviceCommands.Restore(id, language);
            if (resultId == null)
            {
                return BadRequest();
            }
            
            var result = serviceQueries.GetModel(resultId.Value);
            return Ok(result);
        }

        [HttpPost("remove/{id}")]
        public IActionResult Remove(Guid id)
        {
            var resultId = serviceCommands.Remove(id);
            if (resultId == null)
            {
                return BadRequest();
            }
            
            var result = serviceQueries.GetModel(resultId.Value);
            return Ok(result);
        }

        [HttpGet("edit-history/{id}/{page}")]
        public IActionResult GetEditHistory(Guid id, int page)
        {
            var result = serviceQueries.GetEditHistory(id, page);
            return Ok(result);
        }

        [HttpGet("connection-history/{id}/{page}")]
        public IActionResult GetConnectionHistory(Guid id, int page)
        {
            var result = serviceQueries.GetConnectionHistory(id, page);
            return Ok(result);
        }
    }
}