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
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Quartz;
using Quartz.Impl.Matchers;

namespace PTV.TaskScheduler
{
    /// <summary>
    /// Quartz extensions
    /// </summary>
    public static class QuartzExtensions
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Reschedule job
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="key"></param>
        /// <param name="cronExpression"></param>
        public static bool RescheduleJob(this IScheduler scheduler, TriggerKey key, string cronExpression)
        {
            //var triggerState = scheduler.GetTriggerState(key).Result;
            if (!(scheduler.GetTrigger(key).Result is ICronTrigger trigger)) return false;
            if (trigger.CronExpressionString == cronExpression) return false;

            var triggerBuilder = trigger
                .GetTriggerBuilder()
                .StartNow()
                .WithCronSchedule(cronExpression, c => c.InTimeZone(TimeZoneInfo.Utc));

            var nextFireTime = scheduler.RescheduleJob(key, triggerBuilder.Build()).Result;
            if (!nextFireTime.HasValue) throw new Exception($"Rescheduling of trigger '{key}' failed.");
            _logger.Debug($"Trigger '{key}' rescheduling. Old cron expression: '{trigger.CronExpressionString}', current cron expresison: '{cronExpression}', nextFireTime: '{nextFireTime.Value.ToLocalTime()}'");
            return true;
        }

        /// <summary>
        /// Unschedule all jobs
        /// </summary>
        /// <param name="scheduler"></param>
        public static void UnscheduleAllJobs(this IScheduler scheduler)
        {
            var triggers = scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup()).Result.ToList();
            Task.Run(() => scheduler.UnscheduleJobs(triggers)).Wait();
        }

        /// <summary>
        /// Delete all jobs
        /// </summary>
        /// <param name="scheduler"></param>
        public static void DeleteAllJobs(this IScheduler scheduler)
        {
            var jobs = scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()).Result.ToList();
            Task.Run(() => scheduler.DeleteJobs(jobs)).Wait();
        }

        /// <summary>
        /// Get count of exception fires
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public static int GetCountOfExceptionFires(this IJobDetail job)
        {
            var jdm = job?.JobDataMap;
            return jdm?.GetIntValue(QuartzScheduler.COUNT_OF_FAILED_EXECUTIONS) ?? -1;
        }
    }
}
