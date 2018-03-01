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
using PTV.Database.DataAccess.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Domain.Model.Models;
using PTV.Framework.Extensions;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to all "Common" stuff
    /// </summary>
    [Route("api/auth")]
    [Controller]
    public class RESTAuthorizationController
    {
        private readonly ApplicationConfiguration configuration;
        private readonly IAuthorizationService authorizationService;

        /// <summary>
        /// Constructor of Authorization controller
        /// </summary>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        /// <param name="configuration">logger commponent to support logging - injected by framework</param>
        /// <param name="authorizationService">logger commponent to support logging - injected by framework</param>
        public RESTAuthorizationController(ILogger<RESTAuthorizationController> logger, ApplicationConfiguration configuration, IAuthorizationService authorizationService)
        {
            this.configuration = configuration;
            this.authorizationService = authorizationService;
        }

        /// <summary>
        /// Gets organization area information for service
        /// </summary>
        /// <returns>area information</returns>
        [Route("SetAuthEntryPoint")]
        [HttpPost]
        public IActionResult SetAuthEntryPoint([FromBody] VmAuthEntryPoint model)
        {
            var tokenId = authorizationService.SaveAuthorizationInfo(model);
            return new JsonResult(new { redirectUrl=$"{configuration.GetRedirectUrl()}?tokenID={tokenId}" });
        }
    }
}
