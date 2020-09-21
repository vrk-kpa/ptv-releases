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
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using PTV.Application.Web.ViewModels.Home;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PTV.Database.DataAccess.Interfaces.Services;
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
    public class HomeController : BaseController
    {
        private readonly ApplicationConfiguration configuration;
        private readonly IMessagesService messagesService;
        private readonly IServiceManager serviceManager;
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly MapDNSes mapDnSes;
        private readonly IResolveManager resolveManager;
        private readonly IOrganizationService organizationService;
        private readonly ILogger<HomeController> logger;
        private readonly ITokenIntrospecter tokenIntrospector;

        /// <summary>
        /// Constructor of home controller
        /// </summary>
        /// <param name="configuration">Configuration of application. <see cref="ApplicationConfiguration"/></param>
        /// <param name="logger">Logger</param>
        /// <param name="messagesService">messages service responsible for operation related to messages - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="authorizationService"></param>
        /// <param name="httpContextAccessor">http context accessor - injected by framework</param>
        /// <param name="organizationService">service working with organizations</param>
        /// <param name="resolveManager"></param>
        /// <param name="mapDnSes"></param>
        /// <param name="tokenIntrospector"></param>
        public HomeController(
            ApplicationConfiguration configuration,
            ILogger<HomeController> logger,
            IResolveManager resolveManager,
            IOrganizationService organizationService,
            IMessagesService messagesService,
            IServiceManager serviceManager,
            IOptions<MapDNSes> mapDnSes,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            ITokenIntrospecter tokenIntrospector) : base(configuration, logger)
        {
            this.configuration = configuration;
            this.messagesService = messagesService;
            this.serviceManager = serviceManager;
            this.mapDnSes = mapDnSes.Value;
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.organizationService = organizationService;
            this.resolveManager = resolveManager;
            this.tokenIntrospector = tokenIntrospector;
            this.logger = logger;
        }

        private IActionResult RedirectToPahaLogin()
        {
            // ReSharper disable twice StringLiteralTypo
            return Redirect(Configuration.GetPAHARedirectUrl() + "/fi/kirjautuminen/kirjaudu?returnUrl=" +
                            WebUtility.HtmlEncode(Configuration.GetRedirectUrl() +
                                                  httpContextAccessor.HttpContext.Request.Path.Value));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Index(null);
        }

        //[Authorize]
        [HttpGet("~/login")]
        public IActionResult Index([FromQuery] string tokenId)
        {
            Logger.LogInformation($"****Logging is on!**** token ID {tokenId}");
            if (!httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("ptv_token", out var accessToken))
            {
                accessToken = string.Empty;
            }
            else
            {
                Logger.LogInformation("Token read from cookie.");
            }

            if (!string.IsNullOrEmpty(tokenId))
            {
                var tokenGuid = tokenId.ParseToGuid();
                if (tokenGuid.HasValue)
                {
                    try
                    {
                        Logger.LogInformation($"Token Guid: {tokenGuid}");
                        accessToken = authorizationService.GetAuthorizationToken(tokenGuid.Value);
                        Logger.LogInformation("Token loaded from db.");
                    }
                    catch (Exception e)
                    {
                        Logger.LogError("Token load from db failed.", e);
                        accessToken = string.Empty;
                    }
                }
            }

            if (configuration.UsePAHAAuthentication && string.IsNullOrEmpty(accessToken))
            {
                return RedirectToPahaLogin();
            }

            httpContextAccessor.HttpContext.Response.Cookies.Append(configuration.FakeAuthorization ? "paha_token" : "ptv_token", accessToken);

            if (!ProcessPahaToken(ref accessToken, out var actionResult))
            {
                return actionResult;
            }

            var model = new UiAppSettings
            {
                IsWebpackDisabled = Configuration.IsWebpackDisabled(),
                CustomApiUrl = Configuration.GetApiUrl(),
                EnvironmentType = Configuration.GetEnvironmentType(),
                StsUrl = Configuration.GetStsUrl(),
                IsPAHALoginEnabled = configuration.UsePAHAAuthentication,
                IsFakeAuthenticationEnabled = configuration.FakeAuthorization,
                MapDNSNames = this.mapDnSes,
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

        private bool ProcessPahaToken(ref string accessToken, out IActionResult actionResult)
        {
            using (var scope = resolveManager.CreateScope())
            {
                var pahaTokenProcessor = scope.ServiceProvider.GetService<IPahaTokenProcessor>();
                pahaTokenProcessor.ProcessToken(accessToken);
                logger.LogInformation($"Token decoded:\n {pahaTokenProcessor.InternalToken}");
                if (pahaTokenProcessor.InvalidToken)
                {
                    logger.LogError($"Error occured during processing token!\n{pahaTokenProcessor.ErrorMessageFlat}");
                }

                if (pahaTokenProcessor.IsTokenPresent)
                {
                    if (!tokenIntrospector.IntrospectToken(accessToken))
                    {
                        logger.LogError("Invalid token, introspection failed!");
                        if (!string.IsNullOrEmpty(accessToken) && configuration.UsePAHAAuthentication &&
                            !configuration.FakeAuthorization)
                        {
                            {
                                actionResult = RedirectToPahaLogin();
                                return false;
                            }
                        }

                        accessToken = string.Empty;
                    }
                    else
                    {
                        organizationService.CreateNonExistingSahaOrganization(pahaTokenProcessor.SelectedOrganization);
                    }
                }
                else
                {
                    logger.LogError($"Invalid token or token not available, TOKEN {pahaTokenProcessor?.InternalToken} ORG {pahaTokenProcessor?.SelectedOrganization != null} NAME {pahaTokenProcessor?.UserName} ROLE {pahaTokenProcessor?.AccessRightGroup}");
                    if (!string.IsNullOrEmpty(accessToken) && configuration.UsePAHAAuthentication &&
                        !configuration.FakeAuthorization)
                    {
                        {
                            actionResult = RedirectToPahaLogin();
                            return false;
                        }
                    }
                }
            }

            actionResult = null;
            return true;
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
        /// Gets translations of all messages
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
