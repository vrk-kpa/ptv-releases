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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PTV.Application.Web.ViewModels.Home;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PTV.Application.Framework;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;

namespace PTV.Application.Web.Controllers
{
    /// <summary>
    /// Home controller is UI entry point, it provides view with JS files and settings
    /// </summary>
    public class HomeController : BaseController
    {
        private readonly ApplicationConfiguration configuration;
        private IUserInfoService userInfoService;
        private readonly IMessagesService messagesService;
        private readonly IServiceManager serviceManager;
        private readonly MapDNSes mapDNSes;

        /// <summary>
        /// Constructor of home controller
        /// </summary>
        /// <param name="configuration">Configuration of application. <see cref="ApplicationConfiguration"/></param>
        /// <param name="logger">Logger</param>
        /// <param name="userInfoService">Service returning user information</param>
        /// <param name="messagesService">messages service responsible for operation related to messages - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        public HomeController(ApplicationConfiguration configuration, ILogger<HomeController> logger, IUserInfoService userInfoService, IMessagesService messagesService, IServiceManager serviceManager, IOptions<MapDNSes> mapDNSes) : base(configuration, logger)
        {
            this.configuration = configuration;
            this.userInfoService = userInfoService;
            this.messagesService = messagesService;
            this.serviceManager = serviceManager;
            this.mapDNSes = mapDNSes.Value;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            Logger.LogInformation("Logging is on!");
            var accessToken = await HttpContext.Authentication.GetTokenAsync("access_token");
            var model = new UiAppSettings
            {
                IsWebpackDisabled = Configuration.IsWebpackDisabled(),
                CustomApiUrl = Configuration.GetApiUrl(),
                EnvironmentType = Configuration.GetEnvironmentType(),
                StsUrl = Configuration.GetStsUrl(),
                MapDNSNames = this.mapDNSes,
                Version = Configuration.GetVersionWithBuildNumber(),
                VersionPrefix = Configuration.GetVersionPrefix(),
                IdentityToken = await HttpContext.Authentication.GetTokenAsync("id_token"),
                AccessToken = accessToken,
                UserInfo = userInfoService.GetUserInfo()
            };
            ViewBag.Version = model.Version;
            UserIdentityResponse userIdentification = null;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = client.GetStringAsync(configuration.StsAddress + "/connect/userinfo").Result;
                try
                {
                    userIdentification = JsonConvert.DeserializeObject<UserIdentityResponse>(response);
                }
                catch { userIdentification = null; }
            }
            if (userIdentification == null)
            {
                await HttpContext.Authentication.SignOutAsync("Cookies");
                await HttpContext.Authentication.SignOutAsync("oidc");
                return Redirect("/");
            }
            return View(model);

        }

        [Route("Logoff")]
        [HttpGet]
        public async Task LogOff()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            await HttpContext.Authentication.SignOutAsync("oidc");
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
                () => new TranslationResultWrap { TranslatedData = messagesService.GetMessages().Translations },
                new Dictionary<Type, string>()));
        }
    }
}
