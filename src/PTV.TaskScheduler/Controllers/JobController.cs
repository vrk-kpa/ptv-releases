﻿/**
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
using PTV.Database.DataAccess.Interfaces.Services;
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
        private readonly IJobStatusService jobStatusService;

        public JobController(IJobStatusService jobStatusService)
        {
            this.jobStatusService = jobStatusService;
        }
        
        /// <summary>
        /// Job list
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/job/list")]
        public async Task<IEnumerable<VmApiJobInfo>> JobList()
        {
            var jobs = await QuartzScheduler.GetJobs(jobStatusService);
            return jobs.OrderBy(j => j.Code);
        }
        
        /// <summary>
        /// Job name list
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/job/names")]
        public async Task<IActionResult> JobNames()
        {
            var jobs = await QuartzScheduler.GetScheduledJobNames();
            return Ok(jobs);
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
        public IActionResult ForceJob(string jobType)
        {
            if (jobType == null) throw new Exception($"JobType '{ControllerContext.RouteData.Values[nameof(jobType)]}' is not defined.");
            QuartzScheduler.ForceJobExecution(jobType);
            return Ok("Task started ...");
        }

        /// <summary>
        /// Resume job
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("api/job/resume/{jobType}")]
        public IActionResult ResumeJob(string jobType)
        {
            if (jobType == null) throw new Exception($"JobType '{ControllerContext.RouteData.Values[nameof(jobType)]}' is not defined.");
            QuartzScheduler.ResumeTrigger(jobType);
            return Ok($"Task '{jobType}' has been resumed.");
        }

        /// <summary>
        /// Pause job
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("api/job/pause/{jobType}")]
        public IActionResult PauseJob(string jobType)
        {
            if (jobType == null) throw new Exception($"JobType '{ControllerContext.RouteData.Values[nameof(jobType)]}' is not defined.");
            QuartzScheduler.PauseTrigger(jobType);
            return Ok($"Task '{jobType}' has been paused.");
        }

        /// <summary>
        /// Restart job
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("api/job/restart/{jobType}")]
        public IActionResult RestartJob(string jobType)
        {
            if (jobType == null) throw new Exception($"JobType '{ControllerContext.RouteData.Values[nameof(jobType)]}' is not defined.");
            QuartzScheduler.RestartJob(jobType);
            return Ok($"Task '{jobType}' has been restarted.");
        }

        /// <summary>
        /// Restart all jobs
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/job/restartAll")]
        public async Task<IEnumerable<VmApiJobInfo>> RestartAll()
        {
            QuartzScheduler.RestartAllJobs(jobStatusService);
            return await QuartzScheduler.GetJobs(jobStatusService);
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
