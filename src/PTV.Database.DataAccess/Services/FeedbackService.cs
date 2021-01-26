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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Feedback;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.Logging;
using PTV.NetworkServices.Emailing;

namespace PTV.Database.DataAccess.Services
{
    /// <summary>
    /// Service responsible for processing feedback messages from Suomi.fi.
    /// </summary>
    [RegisterService(typeof(IFeedbackService), RegisterType.Transient)]
    public class FeedbackService : IFeedbackService
    {
        private readonly IEmailService emailService;
        private readonly IOrganizationTreeDataCache organizationCache;
        private readonly ApplicationConfiguration configuration;
        private readonly ILogger<FeedbackService> logger;

        /// <summary>
        /// Creates new instance of the FeedbackService.
        /// </summary>
        /// <param name="emailService">Instance of email service.</param>
        /// <param name="organizationCache">Instance of organization tree data cache.</param>
        /// <param name="configuration">Instance of application configuration.</param>
        /// <param name="logger">Instance of logger.</param>
        public FeedbackService(
            IEmailService emailService,
            IOrganizationTreeDataCache organizationCache,
            ApplicationConfiguration configuration,
            ILogger<FeedbackService> logger)
        {
            this.emailService = emailService;
            this.organizationCache = organizationCache;
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <summary>
        /// Processes feedback from Suomi.fi. Looks for SAHA Id of corresponding parent organization,
        /// adds subject and forwards the message to PAHA.
        /// </summary>
        /// <param name="feedback">Feedback data from Suomi.fi.</param>
        /// <returns></returns>
        /// <exception cref="Exception">SAHA Id for given organization does not exist or PAHA authentication did not work.</exception>
        public IActionResult ProcessFeedback(VmFeedback feedback)
        {
            var emailConfiguration = configuration.GetEmailConfiguration();
            var (_, sahaId) = organizationCache.GetMainOrganizationIds(feedback.OrganizationId);
            var stringSahaId = sahaId.ToString();

            // If test settings should be used and the current Saha ID is not whitelisted,
            // replace it with the default test Saha ID
            if (emailConfiguration.TestConfiguration != null 
                && !emailConfiguration.TestConfiguration.AllowedSahaIds.Contains(stringSahaId))
            {
                sahaId = Guid.Parse(emailConfiguration.TestConfiguration.DefaultSahaId);
            }

            if (sahaId == null)
            {
                var message =
                    $"Feedback service - SAHA ID not found for organization {feedback.OrganizationId}, message: {feedback.Id}, time: {feedback.TimeStamp}";
                logger.LogError(message);
                return new BadRequestObjectResult(message);
            }

            var loggerInfo  = new VmJobLogEntry {JobType = nameof(ProcessFeedback)};
            var pahaToken = emailService.GetPahaTokenAuthentication(emailConfiguration.AuthenticationUrl,
                emailConfiguration.Username, emailConfiguration.Password, loggerInfo);

            if (pahaToken.IsNullOrWhitespace())
            {
                var message = $"PAHA token is null for feedback {feedback.Id}, time: {feedback.TimeStamp}";
                logger.LogError(message);
                throw new Exception(message);
            }

            var emailBody = emailConfiguration.TemplateText;
            emailBody = emailBody.Replace("$LINK$", feedback.Url);
            emailBody = emailBody.Replace("$MESSAGE$", feedback.Message);
            emailBody = emailBody.Replace("$SHOWSENDER$",
                feedback.SenderEmail.IsNullOrWhitespace() ? "none" : "initial");
            emailBody = emailBody.Replace("$SENDEREMAIL$", feedback.SenderEmail);

            var (wasSent, pahaMessage) = emailService.SendEmailToOrganizationUsers(emailConfiguration.UrlBase, pahaToken, sahaId.Value, emailBody,
                emailConfiguration.Subject, loggerInfo);

            return wasSent ? new OkObjectResult(feedback) : throw new Exception($"PAHA failed to send the email. Details: {pahaMessage}");
        }
    }
}
