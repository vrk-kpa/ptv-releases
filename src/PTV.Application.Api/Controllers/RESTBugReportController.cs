
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using PTV.Domain.Model.Models.Localization;
using PTV.Domain.Model.Models;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for bug reporting actions
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/bugreport")]
    [Controller]
    public class RESTBugReportController : RESTBaseController
    {
        private readonly IBugReportService bugReportService;
        /// <summary>
        /// Constructor of BugReport controller
        /// </summary>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        /// <param name="bugReportService">Bug report service</param>
        public RESTBugReportController(ILogger<RESTBugReportController> logger, IBugReportService bugReportService) : base(logger)
        {
            this.bugReportService = bugReportService;
        }
        /// <summary>
        /// Gets all reported bugs
        /// </summary>
        [Route("GetAllBugs")]
        [HttpGet]
        public IEnumerable<VmBugReport> GetAllBugs()
        {
            return bugReportService.GetAllBugReports();
        }
        /// <summary>
        /// Get bug report by id
        /// </summary>
        [Route("GetBugReportById")]
        [HttpGet]
        public VmBugReport GetBugReportById(Guid id)
        {
            return bugReportService.GetBugReportById(id);
        }
        /// <summary>
        /// Save a bug report
        /// </summary>
        [Route("SaveBugReport")]
        [HttpPost]
        public void SaveBugReport([FromBody] VmBugReport bugReport)
        {
            bugReportService.SaveBugReport(bugReport);
        }
    }
}
