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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PTV.Database.DataAccess.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using PTV.Application.Web.ViewModels.Home;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using PTV.Application.Framework;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Application.Web.Controllers
{
    /// <summary>
    /// Home controller is UI entry point, it provides view with JS files and settings
    /// </summary>
    public class HomeController : BaseController
    {
        private readonly ApplicationConfiguration configuration;

        public HomeController(ApplicationConfiguration configuration, ILogger<HomeController> logger) : base(configuration, logger)
        {
            this.configuration = configuration;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            Logger.LogInformation("Logging is on!");
            var model = new UiAppSettings
            {
                IsWebpackDisabled = Configuration.IsWebpackDisabled(),
                UserName = User.FindFirst("Name")?.Value,
                CustomApiUrl = Configuration.GetApiUrl(),
                EnvironmentType = Configuration.GetEnvironmentType(),
                StsUrl = Configuration.GetStsUrl(),
            };
            ViewData["Version"] = Configuration.GetVersionWithBuildNumber();
            ViewData["VersionPrefix"] = Configuration.GetVersionPrefix();
            ViewBag.IdentityToken = await HttpContext.Authentication.GetTokenAsync("id_token");
            var accessToken = await HttpContext.Authentication.GetTokenAsync("access_token");
            ViewBag.AccessToken = accessToken;
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
    }
}
