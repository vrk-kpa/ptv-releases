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
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Models.Jobs;
using PTV.Framework;
using PTV.Framework.Logging;
using PTV.Framework.ServiceManager;
using PTV.NetworkServices.Emailing;
using PTV.TaskScheduler.Configuration;
using Quartz;
using IOrganizationService = PTV.Database.DataAccess.Interfaces.Services.V2.IOrganizationService;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class EmailNotifyJob : BaseJob, IJob
    {
        private const string EmailTemplateBodyFile = "NotificationEmailTemplate.html";
        private const string EmailTemplateSentencesFile = "NotificationEmailSentences_??.def";


        private readonly string emailTemplateBody;
        private readonly Dictionary<string,string> emailTemplateSentences;

        private string ExtractLanguageFromFileName(string fileName)
        {
            return fileName.Substring(fileName.LastIndexOf('_')+1, 2).ToUpperInvariant();
        }

        public EmailNotifyJob()
        {
            emailTemplateBody = File.ReadAllText(EmailTemplateBodyFile);
            emailTemplateSentences = Directory.GetFiles(Directory.GetCurrentDirectory(), EmailTemplateSentencesFile).ToDictionary(ExtractLanguageFromFileName, File.ReadAllText);
        }

        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager, VmJobSummary jobSummary)
        {
            var jobData = QuartzScheduler.GetJobDataConfiguration<EmailNotifyJobDataConfiguration>(context.JobDetail);
//            MergeApplicationJobConfiguration(context, jobData);

            var emailingService = serviceProvider.GetService<IEmailService>();
            var loggerInfo  = new VmJobLogEntry {JobType = JobKey, UserName = context.JobDetail.Key.Name, OperationId = JobOperationId};
            if (string.IsNullOrEmpty(jobData.AuthenticationUrl) || string.IsNullOrEmpty(jobData.UrlBase) || string.IsNullOrEmpty(jobData.Username) ||
                string.IsNullOrEmpty(jobData.Password))
            {
                TaskSchedulerLogger.LogJobInfo(jobSummary, "Email notifications skipped, job not configured.", JobStatusService);
                return null;
            }
            TaskSchedulerLogger.LogJobInfo(jobSummary, "Starting sending email notifications....", JobStatusService);
            try
            {
                emailingService.GenerateNotificationAndTaskEmails(
                    new EmailGenerationSettings
                    {
                        AuthenticationEndPoint = jobData.AuthenticationUrl,
                        EmailEndPoint = jobData.UrlBase,
                        EmailTemplate = emailTemplateBody,
                        EmailSentences = emailTemplateSentences,
                        AuthenticationUsername = jobData.Username,
                        EmailSubject = jobData.EmailSubject,
                        AuthenticationPassword = jobData.Password
                    }, loggerInfo);
                TaskSchedulerLogger.LogJobInfo(jobSummary, "Email notifications done.", JobStatusService);
            }
            catch (PtvAppException e)
            {
                TaskSchedulerLogger.LogJobError(jobSummary, $"Sending of email notifications failed with error: {e.Message}", JobStatusService);
            }

            return null;
        }
    }
}
