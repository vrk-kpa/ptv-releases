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
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Framework;
using PTV.ToolUtilities;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// Responsible for providing status of components using external resources like database and services
    /// </summary>
    [AllowAnonymous]
    public class HealthCheckApiController : Controller
    {
        private readonly HealthChecker healthChecker;
        private readonly EnvironmentHelper environmentHelper;

        /// <summary>
        /// Constructor of HealthCheckApiController
        /// </summary>
        /// <param name="healthChecker"></param>
        /// <param name="environmentHelper"></param>
        public HealthCheckApiController(HealthChecker healthChecker, EnvironmentHelper environmentHelper)
        {
            this.healthChecker = healthChecker;
            this.environmentHelper = environmentHelper;
        }

        /// <summary>
        /// Simple test that app is responding
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Ping")]
        public IActionResult Ping()
        {
            return Content(HealthCheckResult.Ok.ToString());
        }


        /// <summary>
        /// Advanced test, provides status of each core component
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Pong")]
        public IActionResult Pong()
        {
            string result = environmentHelper.GetExecutingEnvironment() == ExecutingEnvironment.Web || environmentHelper.GetExecutingEnvironment() == ExecutingEnvironment.Unknown
                ? HealthCheckResult.Ok.ToString()
                : healthChecker.CallAllCheckers();
            return Content($"{result}{Environment.NewLine}{DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss")}");
        }
    }
}
