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
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Jobs;
using PTV.Framework;
using PTV.TaskScheduler.Configuration;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class DigitalAuthorizationCodesJob : BaseJob, IJob
    {
        private UrlBaseConfiguration GetUrlBaseConfiguration(IJobExecutionContext context)
        {
            var urlConfig = GetApplicationJobConfiguration<ApplicationDigitalAuthorizationConfiguration>(context);
            return urlConfig.UrlBase.IsNullOrEmpty()
                ? null
                : urlConfig;
        }

        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager, VmJobSummary jobSummary)
        {
            var jobData = QuartzScheduler.GetJobDataConfiguration<UrlJobDataConfiguration>(context.JobDetail);

            var jobConfiguration = GetUrlBaseConfiguration(context);
            var url = ParseUrlBaseConfiguration(jobData.Url, jobConfiguration);
            var content = ProxyDownload(url, jobSummary, JobStatusService); // roughly 53.38kb

            if (!content.IsNullOrEmpty())
            {
                var service = serviceProvider.GetService<ISeedingService>();
                var status = service?.SeedDigitalAuthorizations(content);
                return status;
            }

            return null;
        }
    }
}
