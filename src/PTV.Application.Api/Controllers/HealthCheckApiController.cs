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
