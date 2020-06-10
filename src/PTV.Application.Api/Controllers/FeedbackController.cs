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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models.Feedback;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// Takes care of feedback from Suomi.fi.
    /// </summary>
    [Route("api/feedback")]
    public class FeedbackController : RESTBaseController
    {
        private readonly IServiceManager serviceManager;
        private readonly IFeedbackService feedbackService;

        /// <summary>
        /// Creates a new instance of the controller.
        /// </summary>
        /// <param name="feedbackService">Instance of feedback service.</param>
        /// <param name="logger">Instance of logger.</param>
        /// <param name="serviceManager">Instance of service manager.</param>
        public FeedbackController(
            IServiceManager serviceManager,
            IFeedbackService feedbackService,
            ILogger<FeedbackController> logger)
            : base(logger)
        {
            this.serviceManager = serviceManager;
            this.feedbackService = feedbackService;
        }

        /// <summary>
        /// Receives a feedback message from Suomi.fi. Adds subject, maps SAHA Id to the provided
        /// organization and then forwards the message to PAHA.
        /// </summary>
        /// <param name="feedback">Feedback data from Suomi.fi.</param>
        /// <returns></returns>
        [Route("Post")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.OpenApiWrite)]
        public IActionResult PostFeedback([FromBody] VmFeedback feedback)
        {
            try
            {
                return feedbackService.ProcessFeedback(feedback);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
