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
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.SoapServices.Models;
using PTV.TaskScheduler.Configuration;
using PTV.TaskScheduler.Enums;
using PTV.TaskScheduler.Interfaces;
using Quartz;


namespace PTV.TaskScheduler.Jobs
{
    internal abstract class BaseJob : IBaseJob, IJob
    {
        //public abstract JobKeyEnum JobKey { get; }
        public string JobKey { get; set; }

        public BaseJob()
        {
            JobKey = this.GetType().Name;
        }
        
        
        protected string JobOperationId { get; private set; }  

        private ProxyServerSettings proxySettings;
        private JobSchedulingConfiguration jobSchedulingConfiguration;

        public Task Execute(IJobExecutionContext context)
        {
            JobOperationId = context.FireInstanceId;
            var serviceProvider = context.Scheduler.Context.Get(QuartzScheduler.SERVICE_PROVIDER) as IServiceProvider;
            jobSchedulingConfiguration = QuartzScheduler.GetJobSchedulingConfiguration(context.JobDetail);
            proxySettings = context.Scheduler.Context.Get(QuartzScheduler.PROXY_SERVER_SETTINGS) as ProxyServerSettings;
            var scopedCtxMgr = serviceProvider.GetService<IContextManager>();

            ExecuteInternal(context, serviceProvider, scopedCtxMgr);
            
            return Task.CompletedTask;
        }

        protected abstract string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager);
        
        private void ExecuteInternal(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
        {

            if (context.Scheduler == null) throw new Exception("Scheduler of context is not set.");

            var countOfExceptionFires = CountOfExecutionFires(context);
            var isForced = IsForced(context);
            var regularFire = countOfExceptionFires==0 && !isForced;
            
            var executionType = isForced
                ? JobExecutionTypeEnum.Forced
                : regularFire
                    ? JobExecutionTypeEnum.Regular
                    : JobExecutionTypeEnum.RunAfterFail;

            try
            {
                LogTaskStarted(JobOperationId, executionType, countOfExceptionFires, jobSchedulingConfiguration.RetriesOnFail);
                var sw = new Stopwatch();
                sw.Start();
                string resultMessage = CallExecute(context, serviceProvider, contextManager);
                sw.Stop();
                LogTaskExecuted(JobOperationId, sw.Elapsed, executionType, resultMessage);

                // reschedule after re-fire, if its needed
                if (!regularFire && !isForced)
                {
                    SetRegularScheduling(JobOperationId, context, jobSchedulingConfiguration.Scheduler);
                }

                // reschedule to "regular" scheduling when job was forced on paused trigger
                if (isForced)
                {
                    var trigger = GetTriggerForJob(context);
                    var triggerState = context.Scheduler.GetTriggerState(trigger.Key).Result;
                    if (triggerState.IsPaused() && trigger.CronExpressionString == jobSchedulingConfiguration.SchedulerOnFail)
                    {
                        SetRegularScheduling(JobOperationId, context, jobSchedulingConfiguration.Scheduler);
                    }
                }
            }
            catch (Exception e)
            {
                var retriesOnFail = jobSchedulingConfiguration.RetriesOnFail;
                context.JobDetail.JobDataMap.Put(QuartzScheduler.COUNT_OF_FAILED_EXECUTIONS, ++countOfExceptionFires);
                LogTaskFailed(JobOperationId, executionType, $"Job '{JobKey}' has failed.", e);

                if (regularFire)
                {
                    SetExceptionalScheduling(JobOperationId, context, jobSchedulingConfiguration.SchedulerOnFail);
                }

                if (countOfExceptionFires > retriesOnFail && !isForced)
                {
                    // SFIPTV-1068: do not pause a task, just clear count of failed attempts and set regular scheduling back
                    SetRegularScheduling(JobOperationId, context, jobSchedulingConfiguration.Scheduler);
                    ClearCountOfFails(context);

                    //PauseTrigger(operationId, context);
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
            if (jobTriggers.Count != 1) throw new Exception($"Bad count of triggers for job '{context.JobDetail.Key}'.");
            var trigger = context.Scheduler.GetTrigger(jobTriggers.Single().Key).Result as ICronTrigger;
            if (trigger == null) throw new Exception($"Trigger '{jobTriggers.Single().Key}' not found or is not a cron trigger.");
            return trigger;
        }

        private void SetRegularScheduling(string operationId, IJobExecutionContext context, string cronExpression)
        {
            ClearCountOfFails(context);
            var trigger = GetTriggerForJob(context);
            if (context.Scheduler.RescheduleJob(trigger.Key, cronExpression))
            {
                TaskSchedulerLogger.LogJobWarn(operationId, JobKey, $"Job '{JobKey}' rescheduled back. New cron expression: '{cronExpression}'.");
            }
        }

        private void SetExceptionalScheduling(string operationId, IJobExecutionContext context, string cronExpression)
        {
            if (context.Scheduler.RescheduleJob(context.Trigger.Key, cronExpression))
            {
                TaskSchedulerLogger.LogJobWarn(operationId, JobKey, $"Job '{JobKey}' rescheduled due to unexpected exception. New cron expression: '{cronExpression}'.");
            }
        }

//        // SFIPTV-1068: do not pause a task
//        private void PauseTrigger(string operationId, IJobExecutionContext context)
//        {
//            TaskSchedulerLogger.LogJobWarn(operationId, JobKey, $"Job '{JobKey}' has exceeded count of exception fires. Trigger has been paused.");
//            QuartzScheduler.PauseTrigger(context.Trigger.Key);
//            //ClearCountOfFails(context);
//        }

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

        private void LogTaskExecuted(string operationId, TimeSpan timespan, JobExecutionTypeEnum executionType, string resultMessage)
        {
            TaskSchedulerLogger.LogJobInfo(operationId, JobKey, executionType, JobResultStatusEnum.Success, $"Job '{JobKey}' has been successfully finished in {timespan}. {resultMessage}");
        }

        private void LogTaskStarted(string operationId, JobExecutionTypeEnum executionType, int executionCnt, int retriesOnFail)
        {
            var message = executionType == JobExecutionTypeEnum.RunAfterFail
                ? $"Job '{JobKey}' has been re-ran after fail. Attempt {executionCnt}/{retriesOnFail}."
                : $"Job '{JobKey}' has been started.";

            TaskSchedulerLogger.LogJobInfo(operationId, JobKey, executionType, JobResultStatusEnum.Started, message);
        }

        private void LogTaskFailed(string operationId, JobExecutionTypeEnum executionType, string message, Exception exception)
        {
            TaskSchedulerLogger.LogJobError(operationId, JobKey, executionType, JobResultStatusEnum.Fail, message, exception);
        }

        protected string ProxyDownload(string url, UrlBaseConfiguration optionalConfiguration)
        {
            var proxy = proxySettings.OverrideBy(optionalConfiguration?.ProxyServerSettings);
            return HttpClientWithProxy.Use(proxy, client => 
			{
				if (jobSchedulingConfiguration.Timeout > 0)
                {
                    var timeout = new TimeSpan(0, 0, jobSchedulingConfiguration.Timeout);
                    client.Timeout = timeout;
                }
			    TaskSchedulerLogger.LogJobInfo(JobOperationId, JobKey, $"Calling HTTP request '{url}'. Proxy settings '{proxy.Address}:{proxy.Port}', disabled: {proxy.Disable}");
			    return client.GetStringAsync(url).GetAwaiter().GetResult();
            });
        }
        
        protected string GetJsonDataWithCertificate(string url, byte[] certificate, ICertificateConfiguration certificationConfiguration, ProxyServerSettings proxy = null)
        {
            var uri = new Uri(url);
            proxy = proxy ?? proxySettings;
                
            return HttpClientWithProxy.Use(proxySettings.OverrideBy(proxy), certificate, client =>
            {
                try
                {
                    if (!certificationConfiguration.AuthorizationHeaderValue.IsNullOrWhitespace())
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", certificationConfiguration.AuthorizationHeaderValue);
                    }
                   
                    TaskSchedulerLogger.LogJobInfo(JobOperationId, JobKey, 
                        $"Calling HTTP request '{url}'. " +
                        $"Proxy settings '{proxySettings.Address}:{proxySettings?.Port}', disabled: {proxySettings?.Disable}, " +
                        $"IgnoreServerCertificate: {certificationConfiguration.IgnoreServerCertificate}");
                    
                    using (var response = client.GetAsync(uri).GetAwaiter().GetResult())
                    {
                        if (response.IsSuccessStatusCode) return response.Content?.ReadAsStringAsync().Result;
                        
                        TaskSchedulerLogger.LogJobError(JobOperationId, JobKey, $"Downloading of '{url}' failed.\n(StatusCode: {response.StatusCode})", new Exception(response.Content?.ReadAsStringAsync().GetAwaiter().GetResult()));
                        return null;
                    }
                }
                catch (AggregateException ae)
                {
                    TaskSchedulerLogger.LogJobError(JobOperationId, JobKey, $"Downloading of '{url}' failed.\n{ae.Message}", ae);
                    ae.Handle(ex => ex is HttpRequestException);
                    return null;
                }
            }, ignoreServerCertificate: certificationConfiguration.IgnoreServerCertificate);
        }

        protected JArray Download(string url, UrlBaseConfiguration optionalConfiguration)
        {
            var content = ProxyDownload(url, optionalConfiguration);
            var parsedContent = JObject.Parse(content);
            var resultCode = (int) parsedContent["meta"]["code"];

            if (resultCode != 200)
            {
                // something went wrong
                throw new Exception($"{JobKey} job: Code service returned code: {resultCode}. Something went wrong.{Environment.NewLine}Used URL: {url} ");
            }

            return (JArray) parsedContent["results"];
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
                    Language = name.Key.ToLower() /*TranslateLanguageCode(name.Key)*/,
                    Name = name.Value.ToObject<string>()
                });
            }

            return result;
        }

        /*
        Fixed in kapa - not needed anymore
        private static string TranslateLanguageCode(string code)
        {
            // NOTE: in service result is SE == SV
            switch (code)
            {
                case "se": return "sv";
                default: return code;
            }
        }
        */

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

        protected static T GetApplicationJobConfiguration<T>(IJobExecutionContext context) where T : class, IApplicationJobConfiguration, new()
        {
            var jobConfig = new T();

            if (!(context.Scheduler.Context.Get(QuartzScheduler.SERVICE_PROVIDER) is IServiceProvider serviceProvider)) return null;

            var applicationConfiguration = serviceProvider.GetRequiredService<IConfigurationRoot>();
            if (applicationConfiguration == null) return null;

            applicationConfiguration.GetSection(jobConfig.ConfigurationName).Bind(jobConfig);

            return jobConfig;
        }

        protected static void MergeApplicationJobConfiguration<T>(IJobExecutionContext context, T jobConfiguration) where T : class, IApplicationJobConfiguration, new()
        {
            if (jobConfiguration == null) return;
            if (!(context.Scheduler.Context.Get(QuartzScheduler.SERVICE_PROVIDER) is IServiceProvider serviceProvider)) return;
            var applicationConfiguration = serviceProvider.GetRequiredService<IConfigurationRoot>();
            applicationConfiguration?.GetSection(jobConfiguration.ConfigurationName).Bind(jobConfiguration);
        }

        protected ApplicationKapaConfiguration GetKapaConfiguration<T>(IJobExecutionContext context) where  T : ApplicationKapaConfiguration, new ()
        {
            var kc = GetApplicationJobConfiguration<T>(context);
            return kc.ApiKey == null || kc.Version == null || kc.UrlBase == null
                ? null
                : kc;
        }
        
        protected string ParseKapaConfiguration(string url, ApplicationKapaConfiguration kapaConfiguration)
        {
            return kapaConfiguration == null
                ? url
                : string.Format(url, kapaConfiguration.UrlBase.TrimEnd('/'), kapaConfiguration.Version, kapaConfiguration.ApiKey);
        }

        protected static string ParseUrlBaseConfiguration(string url, UrlBaseConfiguration urlConfig)
        {
            return ParseUrlBaseConfiguration(url, urlConfig, null);
        }

        protected static string ParseUrlBaseConfiguration(string url, UrlBaseConfiguration urlConfig, params string[] additionalParams)
        {
            if (urlConfig == null) return url;
            var args = additionalParams == null
                ? new object[] {urlConfig.UrlBase.TrimEnd('/')}
                : new object[] {urlConfig.UrlBase.TrimEnd('/')}.Concat(additionalParams);
            return string.Format(url, args.ToArray());
        }

        protected TranslationConfiguration GetTranslationConfiguration(TranslationOrderJobDataConfiguration jobDataConfig, IJobExecutionContext context)
        {
            var translationOrderConfig = GetTranslationOrderConfiguration(context);
            if (string.IsNullOrWhiteSpace(translationOrderConfig.UrlBase))
            {
                var applicationConfiguration = (context.Scheduler.Context.Get(QuartzScheduler.SERVICE_PROVIDER) as IServiceProvider).GetRequiredService<ApplicationConfiguration>();
                translationOrderConfig.UrlBase = new Uri(new Uri(applicationConfiguration.BindingUrl.Replace("0.0.0.0","127.0.0.1")), "relaying/LingSoftRelay").AbsoluteUri;
                Console.WriteLine($"***TR-ORDER INFO BASEJOB*** UrlBase: {translationOrderConfig.UrlBase}");
            }
            return new TranslationConfiguration()
            {
                ServiceUrl = ParseUrlFromTranslationOrderConfiguration(jobDataConfig.Url, translationOrderConfig),
                FileUrl = ParseFileUrlBaseFromTranslationOrderConfiguration(jobDataConfig.FileUrl, translationOrderConfig),
                UserName = translationOrderConfig?.UserName,
                Password = translationOrderConfig?.Password,
                MaxRepetitionErrorStatesNumber = translationOrderConfig?.MaxRepetitionErrorStatesNumber ?? 0,
                ProxySettings = context.Scheduler.Context.Get(QuartzScheduler.PROXY_SERVER_SETTINGS) as ProxyServerSettings 
            };
        }

        protected TranslationOrderConfiguration GetTranslationOrderConfiguration(IJobExecutionContext context)
        {
            var kc = GetApplicationJobConfiguration<TranslationOrderConfiguration>(context);
            return kc.FileUrlBase == null || kc.UserName == null || kc.Password == null || kc.UrlBase == null
                ? null
                : kc;
        }

        protected string ParseUrlFromTranslationOrderConfiguration(string url, TranslationOrderConfiguration configuration)
        {
            return configuration == null
                ? url
                : string.Format(url, configuration.UrlBase.TrimEnd());
        }

        protected string ParseFileUrlBaseFromTranslationOrderConfiguration(string url, TranslationOrderConfiguration configuration)
        {
            return configuration == null
                ? url
                : string.Format(url, configuration.FileUrlBase.TrimEnd('/'));
        }

    }
}
