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
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Framework.ServiceManager;
using PTV.NetworkServices.Emailing;
using PTV.TaskScheduler.Configuration;
using PTV.TaskScheduler.Enums;
using Quartz;
using IOrganizationService = PTV.Database.DataAccess.Interfaces.Services.V2.IOrganizationService;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class EmailNotifyJob : BaseJob, IJob
    {
        private const string EmailTemplateBodyFile = "NotificationEmailTemplate.html";
        private const string EmailTemplateSentencesFile = "NotificationEmailSentences.def";
        
        public override JobTypeEnum JobType => JobTypeEnum.EmailNotify;

        private readonly string emailTemplateBody;
        private readonly string emailTemplateSentences;

        public EmailNotifyJob()
        {
            emailTemplateBody = File.ReadAllText(EmailTemplateBodyFile);
            emailTemplateSentences = File.ReadAllText(EmailTemplateSentencesFile);
        }

        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
        {
            var jobData = QuartzScheduler.GetJobDataConfiguration<EmailNotifyJobDataConfiguration>(context.JobDetail);
            var emailingService = serviceProvider.GetService<IEmailService>();
            try
            {
                emailingService.GenerateNotificationAndTaskEmails(new EmailGenerationSettings() {AuthenticationEndPoint = jobData.AuthenticationUrl, EmailEndPoint = jobData.UrlBase, EmailTemplate = emailTemplateBody, EmailSentences = emailTemplateSentences, AuthenticationUsername = jobData.Username, AuthenticationPassword = jobData.Password});
            }
            catch (PtvAppException e)
            {
                TaskSchedulerLogger.LogJobError(context.FireInstanceId, JobType, e.Message);
            }

            return null;
        }
    }
}