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
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Jobs;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.TaskScheduler.Configuration;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    internal class BrokenLinkJob : BaseJob
    {
        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager, VmJobSummary jobSummary)
        {
            var jobData = QuartzScheduler.GetJobDataConfiguration<BrokenLinkConfiguration>(context.JobDetail);
            var configuration = serviceProvider.GetService<ApplicationConfiguration>();
            
            if (jobData == null || configuration == null)
            {
                TaskSchedulerLogger.LogJobWarn(jobSummary, $"Job configuration for {JobKey} is not found.", JobStatusService);
                return null;
            }

            if (DuplicatesExist(contextManager))
            {
                return
                    $"Duplicate web pages were found. Skipping this job. Please, run WebPageCleanerJob first.";
            }
            
            var brokenLinkService = serviceProvider.GetService<IBrokenLinkService>();
            
            TaskSchedulerLogger.LogJobWarn(jobSummary,
                $"Calling {configuration.GetLinkValidatorConfiguration()?.ValidationUrl}.", JobStatusService);
            
            var status = Asyncs.HandleAsyncInSync(() => PtvHttpClient.UseAsync(async client =>
            {
                return await brokenLinkService.CheckAllWebPages(client);
            }));

            return status;
        }

        private bool DuplicatesExist(IContextManager contextManager)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var webPageRepo = unitOfWork.CreateRepository<IWebPageRepository>();
                var urlGroups = webPageRepo.All().GroupBy(x => x.Url);
                return urlGroups.Select(g => new {g.Key, Count = g.Count()}).Any(x => x.Count > 1);
            });
        }
    }
}
