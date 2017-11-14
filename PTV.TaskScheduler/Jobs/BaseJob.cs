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
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.TaskScheduler.Configuration;
using PTV.TaskScheduler.Enums;
using PTV.TaskScheduler.Interfaces;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    internal abstract class BaseJob : IBaseJob
    {
        public abstract JobTypeEnum JobType { get; }

        private ProxyServerSettings proxySettings;


        protected void ExecuteInternal(Action<IUnitOfWorkWritable> action, IJobExecutionContext context)
        {
            ExecuteInternal(() =>
            {
                var serviceProvider = context.Scheduler.Context.Get(QuartzScheduler.SERVICE_PROVIDER) as IServiceProvider;
                using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                    scopedCtxMgr.ExecuteWriter(action);
                }
            }, context);
        }

        protected void ExecuteInternal(Action action, IJobExecutionContext context)
        {

            if (context.Scheduler == null) throw new Exception("Scheduler of context is not set.");

            var jobSchedulingConfiguration = QuartzScheduler.GetJobSchedulingConfiguration(context.JobDetail);
            var countOfExceptionFires = CountOfExecutionFires(context);
            var isForced = IsForced(context);
            var regularFire = countOfExceptionFires==0 && !isForced;
            proxySettings = context.Scheduler.Context.Get(QuartzScheduler.PROXY_SERVER_SETTINGS) as ProxyServerSettings;

            var executionType = isForced
                ? JobExecutionTypeEnum.Forced
                : regularFire
                    ? JobExecutionTypeEnum.Regular
                    : JobExecutionTypeEnum.RunAfterFail;

            try
            {
                LogTaskStarted(executionType, countOfExceptionFires, jobSchedulingConfiguration.RetriesOnFail);
                var sw = new Stopwatch();
                sw.Start();
                action();
                sw.Stop();
                LogTaskExecuted(sw.Elapsed, executionType);

                // reschedule after re-fire, if its needed
                if (!regularFire && !isForced)
                {
                    SetRegularScheduling(context, jobSchedulingConfiguration.Scheduler);
                }

                // reschedule to "regular" scheduling when job was forced on paused trigger
                if (isForced)
                {
                    var trigger = GetTriggerForJob(context);
                    var triggerState = context.Scheduler.GetTriggerState(trigger.Key).Result;
                    if (triggerState == TriggerState.Paused && trigger.CronExpressionString == jobSchedulingConfiguration.SchedulerOnFail)
                    {
                        SetRegularScheduling(context, jobSchedulingConfiguration.Scheduler);
                    }
                }
            }
            catch (Exception e)
            {
                var retriesOnFail = jobSchedulingConfiguration.RetriesOnFail;
                context.JobDetail.JobDataMap.Put(QuartzScheduler.COUNT_OF_FAILED_EXECUTIONS, ++countOfExceptionFires);
                LogTaskFailed(executionType, $"Job '{JobType}' has failed.", e);

                if (regularFire)
                {
                    SetExceptionalScheduling(context, jobSchedulingConfiguration.SchedulerOnFail);
                }

                if (countOfExceptionFires > retriesOnFail && !isForced)
                {
                    PauseTrigger(context);
//                    ClearCountOfFails(context);
                }

                throw new JobExecutionException(e)
                {
                    RefireImmediately = false
                };

            }
        }

        private static ICronTrigger GetTriggerForJob(IJobExecutionContext context)
        {
            var jobTriggers = context.Scheduler.GetTriggersOfJob(context.JobDetail.Key).Result.Where(t => t.Key.Group == context.JobDetail.Key.Group).ToList();
            if (jobTriggers.Count() != 1) throw new Exception($"Bad count of triggers for job '{context.JobDetail.Key}'.");
            var trigger = context.Scheduler.GetTrigger(jobTriggers.Single().Key).Result as ICronTrigger;
            if (trigger == null) throw new Exception($"Trigger '{jobTriggers.Single().Key}' not found or is not a cron trigger.");
            return trigger;
        }

        private void SetRegularScheduling(IJobExecutionContext context, string cronExpression)
        {
            ClearCountOfFails(context);
            var trigger = GetTriggerForJob(context);
            if (context.Scheduler.RescheduleJob(trigger.Key, cronExpression))
            {
                TaskSchedulerLogger.JobLogger.Warn($"Job '{JobType}' rescheduled back. New cron expression: '{cronExpression}'.");
            }
        }

        private void SetExceptionalScheduling(IJobExecutionContext context, string cronExpression)
        {
            if (context.Scheduler.RescheduleJob(context.Trigger.Key, cronExpression))
            {
                TaskSchedulerLogger.JobLogger.Warn($"Job '{JobType}' rescheduled due to unexpected exception. New cron expression: '{cronExpression}'.");
            }
        }

        private void PauseTrigger(IJobExecutionContext context)
        {
            TaskSchedulerLogger.JobLogger.Warn($"Job '{JobType}' has exceeded count of exception fires. Trigger has been paused.");
            QuartzScheduler.PauseTrigger(context.Trigger.Key);
            //ClearCountOfFails(context);
        }

        private static void ClearCountOfFails(IJobExecutionContext context)
        {
             context.JobDetail.JobDataMap.Put(QuartzScheduler.COUNT_OF_FAILED_EXECUTIONS, 0);
        }

        private static bool IsForced(IJobExecutionContext context)
        {
            return context.MergedJobDataMap.GetBooleanValue(QuartzScheduler.IS_FORCED);
        }

        private static int CountOfExecutionFires(IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.GetIntValue(QuartzScheduler.COUNT_OF_FAILED_EXECUTIONS);
        }

        private void LogTaskExecuted(TimeSpan timespan, JobExecutionTypeEnum executionType)
        {
            TaskSchedulerLogger.LogJobInfo(JobType, executionType, JobResultStatusEnum.Success, $"Job '{JobType}' has been sucessfully finished in {timespan}.");
        }

        private void LogTaskStarted(JobExecutionTypeEnum executionType, int executionCnt, int retriesOnFail)
        {
            var message = executionType == JobExecutionTypeEnum.RunAfterFail
                ? $"Job '{JobType}' has been re-ran after fail. Attempt {executionCnt}/{retriesOnFail}."
                : $"Job '{JobType}' has been started.";

            TaskSchedulerLogger.LogJobInfo(JobType, executionType, JobResultStatusEnum.Started, message);
        }

        private void LogTaskFailed(JobExecutionTypeEnum executionType, string message, Exception exception)
        {
            TaskSchedulerLogger.LogJobError(JobType, executionType, JobResultStatusEnum.Fail, message, exception);
        }

        private string ProxyDownload(string url)
        {
            WebProxy proxy = null;
            if (proxySettings != null && !proxySettings.Address.IsNullOrEmpty() && !proxySettings.Port.IsNullOrEmpty())
            {
                proxy = new WebProxy(string.Format("{0}:{1}", proxySettings.Address, proxySettings.Port));
                if (!proxySettings.UserName.IsNullOrEmpty() && !proxySettings.Password.IsNullOrEmpty())
                {
                    proxy.Credentials = new NetworkCredential(proxySettings.UserName, proxySettings.Password);
                }

            }

            using (var httpClientHandler = new HttpClientHandler {Proxy = proxy})
            {
                using (var client = new HttpClient(httpClientHandler))
                {
                    var content = client.GetStringAsync(url).Result;
                    return content;
                }
            }
        }

        protected JArray Download(string url, KapaConfiguration kapaConfiguration = null)
        {
            url = ParseKapaConfiguration(url, kapaConfiguration);
            var content = ProxyDownload(url);
            var parsedContent = JObject.Parse(content);
            var resultCode = (int) parsedContent["meta"]["code"];

            if (resultCode != 200)
            {
                // something went wrong
                throw new Exception($"{JobType} job: Code service returned code: {resultCode}. Something went wrong.{Environment.NewLine}Used URL: {url} ");
            }

            return (JArray) parsedContent["results"];
        }

        protected string ParseKapaConfiguration(string url, KapaConfiguration kapaConfiguration)
        {
            return (kapaConfiguration == null)
                ? url
                : string.Format(url, kapaConfiguration.UrlBase.TrimEnd('/'), kapaConfiguration.Version, kapaConfiguration.ApiKey);
        }

        protected string ParseCode(JToken token)
        {
            return token.Value<string>("code");
        }

        protected List<VmJsonName> ParseNames(JToken token)
        {
            var names = token.Value<JObject>("names");
            if (names == null) return null;

            var result = new List<VmJsonName>();
            foreach (var name in names)
            {
                result.Add(new VmJsonName
                {
                    Language = TranslateLanguageCode(name.Key),
                    Name = name.Value.ToObject<string>()
                });
            }

            return result;
        }

        private static string TranslateLanguageCode(string code)
        {
            // NOTE: in service result is SE == SV
            switch (code)
            {
                case "se": return "sv";
                default: return code;
            }
        }

        protected static IEnumerable<VmJsonName> HandleCodeNames<TExisting>(IUnitOfWorkWritable unitOfWork, IRepository repository,
            IEnumerable<TExisting> existingCol, IList<IJsonCodeNamesItem> downloadedCol)
            where TExisting : class, INameReferences, ICode
        {
            if (existingCol == null) return Enumerable.Empty<VmJsonName>();
            if (downloadedCol == null) return Enumerable.Empty<VmJsonName>();

            var namesToAdd = new List<VmJsonName>();
            foreach (var existingItem in existingCol)
            {
                var downloadedItem = downloadedCol.SingleOrDefault(di => di.Code == existingItem.Code);
                if (downloadedItem == null) continue;

                // validate existing names
                foreach (var itemName in existingItem.Names)
                {
                    var downloadedItemName = downloadedItem.Names.SingleOrDefault(din => din.Language == itemName.Localization.Code);
                    if (downloadedItemName == null) continue;
                    if (itemName.Name == downloadedItemName.Name) continue;
                    itemName.Name = downloadedItemName.Name;
                }

                // add new names
                namesToAdd.AddRange(downloadedItem.Names
                    .Where(dan => !existingItem.Names.Select(a => a.Localization.Code).Contains(dan.Language))
                    .Select(nta => new VmJsonName { Name = nta.Name, Language = nta.Language, OwnerReferenceId = existingItem.Id}));
            }

            return namesToAdd;
        }

        protected KapaConfiguration GetKapaConfiguration(IJobExecutionContext context)
        {
            if (!(context.Scheduler.Context.Get(QuartzScheduler.SERVICE_PROVIDER) is IServiceProvider serviceProvider)) return null;

            var applicationConfiguration = serviceProvider.GetRequiredService<IConfigurationRoot>();
            if (applicationConfiguration == null) return null;

            var kc = new KapaConfiguration();
            applicationConfiguration.GetSection("KapaConfiguration").Bind(kc);
            return (kc.ApiKey == null || kc.Version == null || kc.UrlBase == null) ? null : kc;
        }

    }
}
