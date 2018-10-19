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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PTV.Application.Web.ViewModels.Home;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Logic;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using PTV.LocalAuthentication;
using IAuthorizationService = PTV.Database.DataAccess.Interfaces.Services.IAuthorizationService;
using IOrganizationService = PTV.Database.DataAccess.Interfaces.Services.V2.IOrganizationService;

namespace PTV.Application.Web.Controllers
{
    /// <summary>
    /// Home controller is UI entry point, it provides view with JS files and settings
    /// </summary>
    public class 
        HomeController : BaseController
    {
        private readonly ApplicationConfiguration configuration;
        private readonly IMessagesService messagesService;
        private readonly IServiceManager serviceManager;
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly MapDNSes mapDNSes;
        private IPahaTokenProcessor pahaTokenProcessor;
        private IOrganizationService organizationService;
        private ILogger<HomeController> logger;
        private ITokenIntrospecter tokenIntrospecter;

        /// <summary>
        /// Constructor of home controller
        /// </summary>
        /// <param name="configuration">Configuration of application. <see cref="ApplicationConfiguration"/></param>
        /// <param name="logger">Logger</param>
        /// <param name="userInfoService">Service returning user information</param>
        /// <param name="messagesService">messages service responsible for operation related to messages - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="httpContextAccessor">http context accessor - injected by framework</param>
        /// <param name="organizationService">service working with organizations</param>
        public HomeController(ApplicationConfiguration configuration, ILogger<HomeController> logger, IUserInfoService userInfoService, IPahaTokenProcessor pahaTokenProcessor, IOrganizationService organizationService, IMessagesService messagesService, IServiceManager serviceManager, IOptions<MapDNSes> mapDNSes, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor, ITokenIntrospecter tokenIntrospecter) : base(configuration, logger)
        {
            this.configuration = configuration;
            this.messagesService = messagesService;
            this.serviceManager = serviceManager;
            this.mapDNSes = mapDNSes.Value;
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.organizationService = organizationService;
            this.pahaTokenProcessor = pahaTokenProcessor;
            this.tokenIntrospecter = tokenIntrospecter;
            this.logger = logger;
        }
//
//        private bool UserHasViewAccess()
//        {
//            var userAccessRightClaim = httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(i => i.Type == PtvClaims.UserAccessRights);
//            if (!string.IsNullOrEmpty(userAccessRightClaim?.Value))
//            {
//                var userAccessRights = userAccessRightClaim.Value.ToLower().Split(',');
//                return userAccessRights.Any(i => i == AccessRightEnum.UiAppRead.ToString().ToLower());
//            }
//            return false;
//        }


        private IActionResult RedirectToPahaLogin()
        {
            return Redirect(Configuration.GetPAHARedirectUrl() + "/fi/kirjautuminen/kirjaudu?returnUrl=" + WebUtility.HtmlEncode(Configuration.GetRedirectUrl() + httpContextAccessor.HttpContext.Request.Path.Value));
        }

        //[Authorize]
        public IActionResult Index([FromQuery] string tokenID)
        {
            Logger.LogInformation("Logging is on!");
            if (!httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("ptv_token", out string accessToken))
            {
                accessToken = string.Empty;
            }
            Logger.LogInformation($"Token: {accessToken}");
            if (!string.IsNullOrEmpty(tokenID))
            {
                var tokenGuid = tokenID.ParseToGuid();
                if (tokenGuid.HasValue)
                {
                    try
                    {
                        accessToken = authorizationService.GetAuthorizationToken(tokenGuid.Value);
                        Logger.LogInformation($"Token: {accessToken}");
                    }
                    catch (Exception)
                    {
                        accessToken = string.Empty;
                    }
                }
            }

            if (configuration.UsePAHAAuthentication && string.IsNullOrEmpty(accessToken))
            {
                return RedirectToPahaLogin();
            }

            httpContextAccessor.HttpContext.Response.Cookies.Append(configuration.FakeAuthorization ? "paha_token" : "ptv_token", accessToken);
            pahaTokenProcessor.ProcessToken(accessToken);
            logger.LogInformation($"Token decoded:\n {pahaTokenProcessor.InternalToken}");
            if (pahaTokenProcessor.ErrorMessages.Any())
            {
                logger.LogError($"Error occured during processing token!\n{string.Join("\n", pahaTokenProcessor.ErrorMessages)}");
            }

            if (pahaTokenProcessor.IsTokenPresent)
            {
                if (!tokenIntrospecter.IntrospectToken(accessToken))
                {
                    logger.LogError("Invalid token, introspection failed!");
                    if (!string.IsNullOrEmpty(accessToken) && configuration.UsePAHAAuthentication && !configuration.FakeAuthorization)
                    {
                        return RedirectToPahaLogin();
                    }
                    else
                    {
                        accessToken = string.Empty;
                    }
                }
                else
                {
                    organizationService.CreateNonExistingSahaOrganization(pahaTokenProcessor.SelectedOrganization);
                }
            }
            else
            {
                logger.LogError("Invalid token or token not available");
                if (!string.IsNullOrEmpty(accessToken) && configuration.UsePAHAAuthentication && !configuration.FakeAuthorization)
                {
                    return RedirectToPahaLogin();
                }
            }
            var model = new UiAppSettings
            {
                IsWebpackDisabled = Configuration.IsWebpackDisabled(),
                CustomApiUrl = Configuration.GetApiUrl(),
                EnvironmentType = Configuration.GetEnvironmentType(),
                StsUrl = Configuration.GetStsUrl(),
                IsPAHALoginEnabled = configuration.UsePAHAAuthentication,
                IsFakeAuthenticationEnabled = configuration.FakeAuthorization,
                MapDNSNames = this.mapDNSes,
                Version = Configuration.GetVersionWithBuildNumber(),
                VersionPrefix = Configuration.GetVersionPrefix(),
                ReleaseNumber = Configuration.GetReleaseVersion(),
                AccessToken = accessToken,
                LogoUrl = configuration.UsePAHAAuthentication ? Configuration.GetPAHARedirectUrl() : string.Empty,
                MenuLinks = configuration.GetMenuLinks()
        };
            ViewBag.Version = model.Version;

            return View(model);
        }

        [Route("UserMissingOrganization")]
        [HttpGet]
        [Authorize]
        public IActionResult UserMissingOrganization()
        {
            return View();
        }

        [Route("Logoff")]
        [HttpGet]
        [Authorize]
        public async Task LogOff()
        {
            await httpContextAccessor.HttpContext.SignOutAsync("Cookies");
            await httpContextAccessor.HttpContext.SignOutAsync("oidc");
        }

        public IActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// Gets translations of all messages (erros/warnings), basically server messages
        /// </summary>
        /// <returns></returns>
        [Route("api/GetMessages")]
        [HttpGet]
        public JsonResult GetMessages()
        {
            return Json(serviceManager.CallService(
                () => new ServiceResultWrap { Data = messagesService.GetMessages(), DoNotCamelize = true },
                new Dictionary<Type, string>()));
        }
    }
}
