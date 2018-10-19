using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PTV.Domain.Logic;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for form state
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/formstate")]
    [Controller]
    public class RESTFormStateController : RESTBaseController
    {
        private readonly IFormStateService formStateService;
        private readonly IPahaTokenProcessor tokenProcessor;
        /// <summary>
        /// Constructor of FormState controller
        /// </summary>
        /// <param name="formStateService"></param>
        /// <param name="tokenProcessor"></param>
        /// <param name="logger"></param>
        public RESTFormStateController(
            IFormStateService formStateService,
            IPahaTokenProcessor tokenProcessor,
            ILogger<RESTFormStateController> logger
            ) : base(logger)
        {
            this.formStateService = formStateService;
            this.tokenProcessor = tokenProcessor;
        }
       
        /// <summary>
        /// Searches for form states
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("Search")]
        [HttpPost]
        public VmFormState Search([FromBody] VmFormStateSearch search)
        {
            return formStateService.Search(search);
        }
        /// <summary>
        /// Save new form state
        /// </summary>
        /// <param name="formState"></param>
        [Route("Save")]
        [HttpPost]
        public VmFormState Save([FromBody] VmFormState formState)
        {
            formState.UserName = tokenProcessor.UserName;
            return formStateService.Save(formState);
        }
        /// <summary>
        /// Delete form state
        /// </summary>
        /// <param name="formState"></param>
        [Route("Delete")]
        [HttpPost]
        public void Delete([FromBody] VmFormState formState)
        {
            formStateService.Delete(formState.Id);
        }
        /// <summary>
        /// Get from state by id
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("GetById")]
        [HttpPost]
        public VmFormState GetById([FromBody] VmFormStateSearch search)
        {
            return formStateService.GetById(search.Id);
        }
    }
}
