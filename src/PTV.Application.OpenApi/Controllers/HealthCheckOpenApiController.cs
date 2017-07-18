using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Framework;
using PTV.ToolUtilities;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Responsible for providing status of components using external resources like database and services
    /// </summary>
    [AllowAnonymous]
    public class HealthCheckOpenApiController : Controller
    {
        private readonly HealthChecker healthChecker;

        /// <summary>
        /// Constructor of HealthCheckApiController
        /// </summary>
        /// <param name="healthChecker"></param>
        public HealthCheckOpenApiController(HealthChecker healthChecker)
        {
            this.healthChecker = healthChecker;
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
            return Content($"{healthChecker.CallAllCheckers()}{Environment.NewLine}{DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss")}");
        }
    }
}
