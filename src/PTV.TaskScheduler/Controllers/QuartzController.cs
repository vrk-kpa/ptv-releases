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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework.Enums;
using PTV.TaskScheduler.Enums;
using PTV.TaskScheduler.Models;

namespace PTV.TaskScheduler.Controllers
{
    /// <summary>
    /// QuartzController
    /// </summary>
    [ServiceFilter(typeof(Attributes.ExceptionFilterAttribute))]
    //[Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/quartz")]
    [AllowAnonymous]
    //[AccessRightRequirement(AccessRightEnum.UiAppRead)]
    //[ClaimRoleRequirement(UserRoleEnum.Eeva)]
    [Controller]
    public class QuartzController : Controller
    {
        /// <summary>
        /// Job list
        /// </summary>
        /// <returns></returns>
        [HttpGet("JobList")]
        public async Task<IActionResult> JobList()
        {
            var jobs = await QuartzScheduler.GetJobs(true);
            return Json(new {Jobs = jobs.OrderBy(j => j.Code)});
        }
        
        /// <summary>
        /// Job list
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("GetJob/{jobType}")]
        public IActionResult GetJob(string jobType)
        {
            return Json(new {Jobs = new List<VmApiJobInfo>{ QuartzScheduler.GetJob(jobType)}});
        }
        
        /// <summary>
        /// Get all scheduler log files 
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLog")]
        public FileResult GetLog()
        {
            return QuartzScheduler.GetLog();
        }

        /// <summary>
        /// Force job
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("ForceJob/{jobType}")]
        public IActionResult ForceJob(string jobType)
        {
            if (jobType == null) throw new Exception($"JobType '{ControllerContext.RouteData.Values[nameof(jobType)]}' is not defined.");
            QuartzScheduler.ForceJobExecution(jobType);
            return Json(Ok("Task started ..."));
        }

        /// <summary>
        /// Resume job
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("ResumeJob/{jobType}")]
        public IActionResult ResumeJob(string jobType)
        {
            if (jobType == null) throw new Exception($"JobType '{ControllerContext.RouteData.Values[nameof(jobType)]}' is not defined.");
            QuartzScheduler.ResumeTrigger(jobType);
            return Json(Ok($"Task '{jobType}' has been resumed."));
        }
        
        /// <summary>
        /// Pause job
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("PauseJob/{jobType}")]
        public IActionResult PauseJob(string jobType)
        {
            if (jobType == null) throw new Exception($"JobType '{ControllerContext.RouteData.Values[nameof(jobType)]}' is not defined.");
            QuartzScheduler.PauseTrigger(jobType);
            return Json(Ok($"Task '{jobType}' has been paused."));
        }

        /// <summary>
        /// Restart job
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("RestartJob/{jobType}")]
        public IActionResult RestartJob(string jobType)
        {
            if (jobType == null) throw new Exception($"JobType '{ControllerContext.RouteData.Values[nameof(jobType)]}' is not defined.");
            QuartzScheduler.RestartJob(jobType);
            return Json(Ok($"Task '{jobType}' has been restarted."));
        }

        /// <summary>
        /// Restart all jobs
        /// </summary>
        /// <returns></returns>
        [HttpGet("RestartAll")]
        public async Task<IEnumerable<VmApiJobInfo>> RestartAll()
        {
            QuartzScheduler.RestartAllJobs();
            return await QuartzScheduler.GetJobs();
        }
        
        /// <summary>
        /// Shutdown scheduler
        /// </summary>
        [HttpGet("DisableScheduler")]
        public void DisableScheduler()
        {
           QuartzScheduler.Stop();
        }
        
        /// <summary>
        /// Shutdown scheduler
        /// </summary>
        [HttpGet("EnableScheduler")]
        public void EnableScheduler()
        {
            QuartzScheduler.Start();
        }
    }
}
