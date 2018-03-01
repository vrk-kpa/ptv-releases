﻿/**
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PTV.TaskScheduler.Controllers
{
    /// <summary>
    /// Log controller
    /// </summary>
    [ServiceFilter(typeof(Attributes.ExceptionFilterAttribute))]
    [AllowAnonymous]
    public class LogController : Controller
    {
        /// <summary>
        /// Get all job logs
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/log")]
        public IActionResult Get()
        {
            return Ok(TaskSchedulerLogger.GetAllJobsLog());
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
            return Ok(TaskSchedulerLogger.GetJobsSummary());
        }

        /// <summary>
        /// Clear all logs
        /// </summary>
        /// <returns></returns>
        [HttpPost("api/log/clearAll")]
        public IActionResult Clear()
        {
            TaskSchedulerLogger.ClearAllLogs();
            return Ok("deleted ...");
        }

        /// <summary>
        /// Clear json job log
        /// </summary>
        /// <returns></returns>
        [HttpPost("api/log/clearJobLogs")]
        public IActionResult ClearJobLogs()
        {
            TaskSchedulerLogger.ClearJobLogs();
            return Ok("deleted ...");
        }
    }
}
