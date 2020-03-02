/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Logic;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to tasks page
    /// </summary>
//    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/qualityAgent")]
    public class QualityAgentController : RESTBaseController
    {
        private readonly IServiceManager serviceManager;
        private readonly IQualityCheckService checkService;
        private readonly IHttpContextAccessor contextAccessor;

        /// <summary>
        /// Constructor of quality agent check controller
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="checkService">service responsible for tasks operation</param>
        /// <param name="logger">logger component to support logging - injected by framework</param>
        /// <param name="contextAccessor">context accessor</param>
        /// </summary>
        public QualityAgentController(ILogger<QualityAgentController> logger, IServiceManager serviceManager, IQualityCheckService checkService, IHttpContextAccessor contextAccessor) : base(logger)
        {
            this.serviceManager = serviceManager;
            this.checkService = checkService;
            this.contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Calls quality check for specific input
        /// </summary>
        /// <returns>validation result for fields</returns>
        [Route("Check")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public async Task<IServiceResultWrap> QualityCheck()
        {
            using (var x = new StreamReader(contextAccessor.HttpContext.Request.Body))
            {
                var text = await x.ReadToEndAsync();
                Console.WriteLine(text);
                return serviceManager.CallService(
                    () => new ServiceResultWrap { Data = checkService.CheckProperty(text) },
                    new Dictionary<Type, string>());
            }
        }
    }
}
