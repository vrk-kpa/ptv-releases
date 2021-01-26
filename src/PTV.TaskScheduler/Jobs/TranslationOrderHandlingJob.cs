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
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Models.Jobs;
using PTV.Framework.Logging;
using PTV.SoapServices.Interfaces;
using PTV.TaskScheduler.Configuration;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class TranslationOrderHandlingJob : BaseJob, IJob
    {
        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager, VmJobSummary jobSummary)
        {
            var jobData = QuartzScheduler.GetJobDataConfiguration<TranslationOrderJobDataConfiguration>(context.JobDetail);
            var translationConfiguration = GetTranslationConfiguration(jobData, context);

            if (string.IsNullOrEmpty(jobData.FileUrl) ||
                string.IsNullOrEmpty(translationConfiguration.UserName) ||
                string.IsNullOrEmpty(translationConfiguration.Password))
            {
                TaskSchedulerLogger.LogJobInfo(jobSummary, "Translation order handling job is not configured.", JobStatusService);
                return null;
            }

            contextManager.ExecuteWriter(unitOfWork =>
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var loggerInfo  =new VmJobLogEntry {JobType = JobKey, UserName = context.JobDetail.Key.Name, OperationId = JobOperationId};
                    var translationOrderManager = scope.ServiceProvider.GetRequiredService<ITranslationOrderManager>();

                    TaskSchedulerLogger.LogJobInfo(jobSummary, "Starting send translation order for Repetition....", JobStatusService);
                    translationOrderManager.SendTranslationOrdersForRepetition(translationConfiguration, loggerInfo);
                    TaskSchedulerLogger.LogJobInfo(jobSummary, "Send translation order for Repetition done.", JobStatusService);

                    TaskSchedulerLogger.LogJobInfo(jobSummary, "Starting canceling translation order....", JobStatusService);
                    translationOrderManager.CancelTranslationOrders(translationConfiguration, loggerInfo);
                    TaskSchedulerLogger.LogJobInfo(jobSummary, "Canceling translation order done.", JobStatusService);
                }

            });
            return null;
        }
    }
}
