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
/**
 * The MIT License
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Jobs;
using PTV.Framework;
using PTV.TaskScheduler.Models;
using Quartz;
using Quartz.Impl.Matchers;

namespace PTV.TaskScheduler
{
    public static partial class QuartzScheduler
    {
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        
        /// <summary>
        /// Get all allowed scheduled jobs
        /// </summary>
        /// <returns></returns>
        public static async Task<List<VmApiJobInfo>> GetJobs(IJobStatusService jobStatusService, bool showOnlyAllowed = false)
        {
            var result = new List<VmApiJobInfo>();
            if (_scheduler == null) return result;

            await semaphore.WaitAsync();
            
            try
            {
                var jobLogs = jobStatusService.GetJobsSummary().ToLookup(x => x.JobType);
                var archiveLogs = jobStatusService.GetJobsSummary(archive: true).ToLookup(x => x.JobType);
                
                var triggerKeys = await _scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
                var triggerTasks = triggerKeys.Select(key => GetJobTask(key, showOnlyAllowed, jobLogs, archiveLogs)).ToArray();
                var infoItems = await Task.WhenAll(triggerTasks);
                result = infoItems.Where(x => x != null).ToList();
            }
            finally
            {
                semaphore.Release();
            }

            return result;
        }

        private static async Task<VmApiJobInfo> GetJobTask(
            TriggerKey triggerKey, 
            bool showOnlyAllowed, 
            ILookup<string, VmJobSummary> jobLogs, 
            ILookup<string, VmJobSummary> archiveLogs)
        {
            var trigger = await _scheduler.GetTrigger(triggerKey) as ICronTrigger;
            if (trigger == null) return null;
                    
            if (showOnlyAllowed)
            {
                var isAllowedForDisplaying = GetAllowedForDisplayingJobConfiguration(trigger.JobKey.Name);
                if (!isAllowedForDisplaying) return null;
            }
                        
            var job = await _scheduler.GetJobDetail(trigger.JobKey);
            if (job == null) return null;

            var jobSchedulingConfiguration = GetJobSchedulingConfiguration(job);
            var nextFireTimeUtc = trigger.GetNextFireTimeUtc();
                    
            return new VmApiJobInfo
            {
                Code = job.Key.Name,
                CronExpression = trigger.CronExpressionString,
                State = (await _scheduler.GetTriggerState(triggerKey)).ToString(),
                LastFireTimeUtc = trigger.GetPreviousFireTimeUtc(),
                NextFireTimeUtc = nextFireTimeUtc,
                NextFireTimeLocal = nextFireTimeUtc?.LocalDateTime,
                RegularScheduling = jobSchedulingConfiguration.Scheduler,
                ExceptionScheduling = jobSchedulingConfiguration.SchedulerOnFail,
                RetriesOnFails = jobSchedulingConfiguration.RetriesOnFail,
                CountOfFailedExecutions = GetFailedJobCountFromLogs(job.Key.Name, jobLogs, archiveLogs), 
                IsRunning = await IsJobRunningAsync(job.Key.Name),
                LastJobStatus = GetLastJobStatus(job.Key.Name, jobLogs, archiveLogs).ToString(),
                LastExecutionTime = GetLastExecutionTime(job.Key.Name, jobLogs, archiveLogs).ToMillisecondsTime()
            };
        }
        
        private static int GetFailedJobCountFromLogs(
            string keyName, 
            ILookup<string, VmJobSummary> logs, 
            ILookup<string, VmJobSummary> archiveLogs)
        {
            var result = 0;
            var allLogsData = logs[keyName].Union(archiveLogs[keyName]).DistinctBy(x => x.OperationId).ToList();
            result = allLogsData.SelectMany(x => x.Logs.Where(y => y.JobStatus == JobResultStatusEnum.Fail))
                .Count();
            return result;
        }
        
        public static async Task<bool> IsJobRunningAsync(string jobKeyStr)
        {
            var runningJobs = await _scheduler.GetCurrentlyExecutingJobs();
            return runningJobs.Any(job => job.JobDetail?.Key.Name == jobKeyStr);
        }

        private static JobResultStatusEnum? GetLastJobStatus(
            string keyName, 
            ILookup<string, VmJobSummary> logs, 
            ILookup<string, VmJobSummary> archiveLogs)
        {
            JobResultStatusEnum? result = null;
            var actualLogData = logs[keyName];
            var lastStatus = actualLogData
                .OrderByDescending(x => x.StartUtc)
                .FirstOrDefault(x => x.Logs.Any(y => y.JobStatus.HasValue));
            if (lastStatus == null)
            {
                actualLogData = archiveLogs[keyName];
                lastStatus = actualLogData
                    .OrderByDescending(x => x.StartUtc)
                    .FirstOrDefault(x => x.Logs.Any(y => y.JobStatus.HasValue));
            }

            if (lastStatus != null)
            {
                result = lastStatus.Logs
                    .OrderByDescending(x => x.TimeUtc)
                    .FirstOrDefault(x => x.JobStatus.HasValue)?.JobStatus;
            }

            return result;
        }

        private static TimeSpan GetLastExecutionTime(
            string keyName, 
            ILookup<string, VmJobSummary> logs, 
            ILookup<string, VmJobSummary> archiveLogs)
        {
            var result = new TimeSpan();
            var actualLogData = logs[keyName];

            var lastStatus = actualLogData.FirstOrDefault(x => x.ExecutionTime.Ticks > 0);
            if (lastStatus == null)
            {
                actualLogData = archiveLogs[keyName];
                lastStatus = actualLogData.FirstOrDefault(x => x.ExecutionTime.Ticks > 0);
            }

            if (lastStatus != null)
            {
                result = lastStatus.ExecutionTime;
            }

            return result;
        }
    }
}
