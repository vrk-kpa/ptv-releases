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
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces;
using PTV.SoapServices.Interfaces;
using PTV.TaskScheduler.Configuration;
using PTV.TaskScheduler.Enums;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class TranslationOrderProcessingDataByStateJob : BaseJob, IJob
    {
        public override JobTypeEnum JobType => JobTypeEnum.TranslationOrderProcessingDataByState;

        public Task Execute(IJobExecutionContext context)
        {
            var jobData = QuartzScheduler.GetJobDataConfiguration<TranslationOrderJobDataConfiguration>(context.JobDetail);
            var serviceProvider = context.Scheduler.Context.Get(QuartzScheduler.SERVICE_PROVIDER) as IServiceProvider;
            var translationConfiguration = GetTranslationConfiguration(jobData, context);

            ExecuteInternal(unitOfWork =>
            {
                var translationOrderManager = serviceProvider.GetRequiredService<ITranslationOrderManager>();
                translationOrderManager.ProcessingTranslationOrderData(translationConfiguration);

            }, context);

            return Task.CompletedTask;
        }
    }
}
