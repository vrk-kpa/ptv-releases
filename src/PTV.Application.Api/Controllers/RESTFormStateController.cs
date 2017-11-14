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

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for form state
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/formstate")]
    [Controller]
    public class RESTFormStateController : RESTBaseController
    {
        private readonly IFormStateService formStateService;
        private readonly IUserInfoService userService;
        /// <summary>
        /// Constructor of FormState controller
        /// </summary>
        /// <param name="formStateService"></param>
        /// <param name="userService"></param>
        /// <param name="logger"></param>
        public RESTFormStateController(
            IFormStateService formStateService,
            IUserInfoService userService,
            ILogger<RESTFormStateController> logger
            ) : base(logger)
        {
            this.formStateService = formStateService;
            this.userService = userService;
        }
        /// <summary>
        /// Check if saved form state exists for form
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("Exists")]
        [HttpPost]
        public VmFormState Exists([FromBody] VmFormStateSearch search)
        {
            var result = new VmFormState();
            var userName = userService.GetUserName();
            if (string.IsNullOrEmpty(userName))
            {
                result.Exists = false;
                return result;
            }
            search.UserName = userName;
            result.Id = formStateService.Exists(search);
            return result;
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
            formState.UserName = userService.GetClaimName();
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
