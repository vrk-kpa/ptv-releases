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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTV.TaskScheduler.Enums;
using PTV.TaskScheduler.Models;

namespace PTV.TaskScheduler.Controllers
{
    /// <summary>
    /// JobController
    /// </summary>
    [ServiceFilter(typeof(Attributes.ExceptionFilterAttribute))]
    [AllowAnonymous]
    public class JobController : Controller
    {
        /// <summary>
        /// Job list
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/job/list")]
        public IEnumerable<VmApiJobInfo> JobList()
        {
            return QuartzScheduler.GetJobs().OrderBy(j => j.Name);
        }
        
        /// <summary>
        /// List of jobs which failed within initialization
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/job/notStarted")]
        public IEnumerable<VmApiNotStartedJobInfo> NotStartedJobs()
        {
            return QuartzScheduler.GetNotStartedJobs().OrderBy(j => j.Name);
        }

        /// <summary>
        /// Force job
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("api/job/force/{jobType}")]
        public IActionResult ForceJob(JobTypeEnum? jobType)
        {
            if (jobType == null) throw new Exception($"JobType '{ControllerContext.RouteData.Values[nameof(jobType)]}' is not defined.");
            QuartzScheduler.ForceJobExecution(jobType.Value);
            return Ok("Task started ...");
        }

        /// <summary>
        /// Resume job
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("api/job/resume/{jobType}")]
        public IActionResult ResumeJob(JobTypeEnum? jobType)
        {
            if (jobType == null) throw new Exception($"JobType '{ControllerContext.RouteData.Values[nameof(jobType)]}' is not defined.");
            QuartzScheduler.ResumeTrigger(jobType.Value);
            return Ok($"Task '{jobType}' has been resumed.");
        }

        /// <summary>
        /// Restart job
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("api/job/restart/{jobType}")]
        public IActionResult RestartJob(JobTypeEnum? jobType)
        {
            if (jobType == null) throw new Exception($"JobType '{ControllerContext.RouteData.Values[nameof(jobType)]}' is not defined.");
            QuartzScheduler.RestartJob(jobType.Value);
            return Ok($"Task '{jobType}' has been restarted.");
        }

        /// <summary>
        /// Restart all jobs
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/job/restartAll")]
        public IEnumerable<VmApiJobInfo> RestartAll()
        {
            QuartzScheduler.RestartAllJobs();
            return QuartzScheduler.GetJobs();
        }

        /// <summary>
        /// Pause all triggers
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/job/pauseAll")]
        public IActionResult PauseAllTriggers()
        {
            return Ok(QuartzScheduler.PauseAllTriggers() 
                ? "All triggers paused" 
                : "Triggers has not been paused!");
        }
        
        /// <summary>
        /// Shutdown scheduler
        /// </summary>
        [HttpGet("api/job/stop")]
        public void Stop()
        {
           QuartzScheduler.Stop();
        }
    }
}
