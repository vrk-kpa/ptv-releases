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
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PTV.Database.DataAccess.Interfaces.Services;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Security;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

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
        private readonly IUserInfoService userInfoService;
        private readonly ILogger logger;
        private readonly IServiceManager serviceManager;
        private readonly IPahaTokenProcessor pahaTokenProcessor;
        private const string MessageUserLoginFailed = "Authorization.Exception.LoginFailed";

        /// <summary>
        /// Constructor of Authorization controller
        /// </summary>
        /// <param name="logger">logger component to support logging - injected by framework</param>
        /// <param name="configuration">logger component to support logging - injected by framework</param>
        /// <param name="authorizationService">logger component to support logging - injected by framework</param>
        /// <param name="userInfoService">logger component to support logging - injected by framework</param>
        /// <param name="serviceManager"></param>
        /// <param name="pahaTokenProcessor"></param>
        public RESTAuthorizationController(ILogger<RESTAuthorizationController> logger, IUserInfoService userInfoService, ApplicationConfiguration configuration, IAuthorizationService authorizationService, IServiceManager serviceManager, IPahaTokenProcessor pahaTokenProcessor)
        {
            this.configuration = configuration;
            this.authorizationService = authorizationService;
            this.serviceManager = serviceManager;
            this.userInfoService = userInfoService;
            this.logger = logger;
            this.pahaTokenProcessor = pahaTokenProcessor;
        }

        /// <summary>
        /// Gets organization area information for service
        /// </summary>
        /// <returns>area information</returns>
        [Route("SetAuthEntryPoint")]
        [HttpPost]
        public IActionResult SetAuthEntryPoint([FromBody] VmAuthEntryPoint model)
        {
            if (string.IsNullOrEmpty(model?.Token))
            {
                logger.LogError("No token received");
                return new JsonResult(new PahaAuthPointResult { error = "No token received" });
            }
            logger.LogInformation($"Token: {model.Token}");
            pahaTokenProcessor.ProcessToken(model.Token);
            if (pahaTokenProcessor.ErrorMessages.Any())
            {
                logger.LogError($"Errors from token: {string.Join(Environment.NewLine, pahaTokenProcessor.ErrorMessages)}");
                return new JsonResult(new PahaAuthPointResult { error = string.Join("/", pahaTokenProcessor.ErrorMessages) });
            }
            var tokenId = authorizationService.SaveAuthorizationInfo(model);

            return new JsonResult(new PahaAuthPointResult { redirectUrl = $"{configuration.GetRedirectUrl()}/login?tokenID={tokenId}", warning = string.Join("/", pahaTokenProcessor.WarningMessages) });
        }

        /// <summary>
        /// Gets all user access groups available in database
        /// </summary>
        /// <returns>area information</returns>
        [Route("GetUserAccessGroups")]
        [HttpGet]
        public IActionResult GetUserAccessGroups()
        {
            return new JsonResult(authorizationService.GetUserAccessGroupsList());
        }

        /// <summary>
        /// Create configuration for user
        /// </summary>
        /// <returns>area information</returns>
        [Route("CreateConfiguration")]
        [HttpPost]
        public IServiceResultWrap CreateConfiguration([FromBody] VmLoginForm model)
        {
            return serviceManager.CallService
            (
                () =>
                {
                    var token = authorizationService.GetAuthorizationToken(model);
                    var userInfo = userInfoService.GetUserInfo(token);
                    return new ServiceResultWrap {Data = new VmToken { Token = token, UserInfo = userInfo }};
                },
                new Dictionary<Type, string>
                {
                    { typeof(InvalidOperationException), MessageUserLoginFailed }
                });
        }

        private class PahaAuthPointResult
        {
            // ReSharper disable once InconsistentNaming
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string redirectUrl { get; set; } = string.Empty;
            // ReSharper disable once InconsistentNaming
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string error { get; set; } = string.Empty;
            // ReSharper disable once InconsistentNaming
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string warning { get; set; } = string.Empty;
        }
    }
}
