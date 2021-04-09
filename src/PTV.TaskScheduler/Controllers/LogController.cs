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
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTV.Database.DataAccess.Interfaces.Services;

namespace PTV.TaskScheduler.Controllers
{
    /// <summary>
    /// Log controller
    /// </summary>
    [ServiceFilter(typeof(Attributes.ExceptionFilterAttribute))]
    [AllowAnonymous]
    public class LogController : Controller
    {
        private readonly IJobStatusService jobStatusService;

        public LogController(IJobStatusService jobStatusService)
        {
            this.jobStatusService = jobStatusService;
        }
        
        /// <summary>
        /// Get all job logs
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/log")]
        public IActionResult Get()
        {
            return Ok(jobStatusService.GetAllJobsLog().ToList());
        }

        /// <summary>
        /// Get running job logs
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/log/running")]
        public IActionResult GetRunningJobs()
        {
            return Ok(QuartzScheduler.GetExecutingJobs());
        }

        /// <summary>
        /// Get jobs summary
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/log/summary")]
        public IActionResult GetJobsSummary()
        {
            return Ok(jobStatusService.GetJobsSummary());
        }

        /// <summary>
        /// Get jobs summary of type
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/log/summary/{jobType}")]
        public IActionResult GetJobsSummary(string jobType)
        {
            if (jobType == null) throw new Exception($"JobType '{ControllerContext.RouteData.Values[nameof(jobType)]}' is not defined.");
            return Ok(jobStatusService.GetJobsSummary(jobType));
        }

        /// <summary>
        /// Clear all job logs
        /// </summary>
        /// <returns></returns>
        [HttpPost("api/log/clearAll")]
        [HttpPost("api/log/clearJobLogs")]
        public IActionResult ClearJobLogs()
        {
            jobStatusService.ClearAllLogs();
            return Ok("deleted ...");
        }
    }
}
