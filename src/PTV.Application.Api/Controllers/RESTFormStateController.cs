/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using Microsoft.AspNetCore.Authorization;

using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            formStateService.Delete(formState.EntityId, tokenProcessor.UserName);
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
