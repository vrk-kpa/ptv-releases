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
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Logging;
using PTV.TaskScheduler.Configuration;
using PTV.TaskScheduler.Enums;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class AccessibilityRegisterJob : BaseJob
    {
        public override JobTypeEnum JobType => JobTypeEnum.AccessibilityRegister;
        
        private IAccessibilityRegisterService accessibilityRegisterService;
        
        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
        {
            var jobDataConfiguration = QuartzScheduler.GetJobDataConfiguration<AccessibilityRegisterConfiguration>(context.JobDetail);
            if (jobDataConfiguration == null) return null;
            
            // service points
            var urlOfServicePoints = string.Format(jobDataConfiguration.UrlServicePoints, jobDataConfiguration.UrlBase.TrimEnd('/'), jobDataConfiguration.SystemId);
            var servicePointsJsonData = ProxyDownload(urlOfServicePoints, jobDataConfiguration);
            if (servicePointsJsonData == null) return $"Unable download service point data from '{jobDataConfiguration}'."; 
            var servicePoints = JsonConvert.DeserializeObject<List<VmJsonAccessibilityRegisterService>>(servicePointsJsonData);
            TaskSchedulerLogger.LogJobInfo(JobOperationId, JobType, $"{servicePoints.Count} service points has been downloaded from {urlOfServicePoints}");

            // get AR service
            accessibilityRegisterService = serviceProvider.GetService<IAccessibilityRegisterService>();
            if (accessibilityRegisterService == null) throw new Exception("Could not load AccessibilityRegisterService!");
            
            // run data import
            var loggerInfo  =new VmJobLogEntry {JobType = JobType.ToString(), UserName = context.JobDetail.Key.Name, OperationId = JobOperationId};
            var accessibilityRegisterSettings = new AccessibilityRegisterSettings {SystemId = jobDataConfiguration.SystemId, BaseUrl = jobDataConfiguration.UrlBase, ServicePointEntrancesUrl = jobDataConfiguration.UrlEntrances, ServicePointSentencesUrl = jobDataConfiguration.UrlSentences};
            accessibilityRegisterService.ImportAccessibilityRegisterData(servicePoints,  accessibilityRegisterSettings, loggerInfo);
            
            return null;
        }
    }
}