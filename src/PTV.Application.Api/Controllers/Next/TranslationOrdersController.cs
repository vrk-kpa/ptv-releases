using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Next.Model;

namespace PTV.Application.Api.Controllers.Next
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/next/translation-orders")]
    public class TranslationOrdersController: BaseController
    {
        private readonly ITranslationCommands translationCommands;
        private readonly ITranslationQueries translationQueries;
        private readonly IServiceQueries serviceQueries;

        public TranslationOrdersController(
            ITranslationCommands translationCommands,
            ITranslationQueries translationQueries,
            IServiceQueries serviceQueries,
            ILogger<TranslationOrdersController> logger) 
            : base(logger)
        {
            this.translationCommands = translationCommands;
            this.translationQueries = translationQueries;
            this.serviceQueries = serviceQueries;
        }
        
        [HttpGet("{id}")]
        public IActionResult GetDetails(Guid id)
        {
            var result = translationQueries.GetDetails(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("service/{serviceId}")]
        public IActionResult OrderService(Guid serviceId, [FromBody] TranslationOrderModel model)
        {
            var result = translationCommands.OrderService(serviceId, model, out var newId);
            if (result)
            {
                var service = serviceQueries.GetModel(newId);
                return Ok(service);
            }

            return BadRequest();
        }

        [HttpGet("service/{serviceId}/{language}")]
        public IActionResult GetHistory(Guid serviceId, LanguageEnum language)
        {
            var result = translationQueries.GetServiceHistory(serviceId, language);
            return Ok(result);
        }
    }
}