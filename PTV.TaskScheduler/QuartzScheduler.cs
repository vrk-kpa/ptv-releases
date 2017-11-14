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
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using PTV.Framework;
using PTV.TaskScheduler.Configuration;
using PTV.TaskScheduler.Enums;
using PTV.TaskScheduler.Interfaces;
using Quartz;
using PTV.TaskScheduler.Jobs;
using PTV.TaskScheduler.Models;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace PTV.TaskScheduler
{
    /// <summary>
    /// Quartz scheduler
    /// </summary>
    public static class QuartzScheduler
    {
        internal const string COUNT_OF_FAILED_EXECUTIONS = "COUNT_OF_FAILED_EXECUTIONS";
        internal const string IS_FORCED = "IS_FORCED";
        internal const string SERVICE_PROVIDER = "SERVICE_PROVIDER";
        internal const string PROXY_SERVER_SETTINGS = "PROXY_SERVER_SETTINGS";

        private const string JOB_SCHEDULING_CONFIGURATION = "JOB_SCHEDULING_CONFIGURATION";
        private const string JOB_DATA_CONFIGURATION = "JOB_DATA_CONFIGURATION";

        private const string PTV_JOB_GROUP = "PTV_JOB_GROUP";
        private static readonly Dictionary<JobTypeEnum, ICronTrigger> _scheduledTriggers = new Dictionary<JobTypeEnum, ICronTrigger>();

        private static IScheduler _scheduler;
        private static IConfiguration _configuration;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private static bool _isRunning;

        /// <summary>
        /// Quartz scheduler initializer
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            if (_isRunning) throw new Exception("Scheduler is already running.");
            if (_scheduler != null) throw new Exception("Scheduler is already initialized!");
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            var jobConfigurationBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("jobsettings.json", optional: false, reloadOnChange: true);
            _configuration = jobConfigurationBuilder.Build();

            var applicationConfiguration = serviceProvider.GetRequiredService<IConfigurationRoot>();
            var properties = new NameValueCollection
            {
                ["quartz.serializer.type"] = "json",
                ["quartz.scheduler.instanceName"] = "PtvScheduler",
                ["quartz.scheduler.instanceId"] = "ptv_quartz_instance",
                ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
                ["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.StdAdoDelegate, Quartz",
                ["quartz.jobStore.useProperties"] = "false",
                ["quartz.jobStore.dataSource"] = "default",
                ["quartz.jobStore.tablePrefix"] = "QRTZ_",
                ["quartz.jobStore.maxMisfiresToHandleAtATime"] = "1",
                ["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz",
                ["quartz.dataSource.default.connectionString"] = applicationConfiguration.GetConnectionString("QuartzConnection"),
                ["quartz.dataSource.default.provider"] = "Npgsql",
                ["quartz.threadPool.threadCount"] = "5", // Allows only 5 Thread in parallel
//                ["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz",
//                ["quartz.threadPool.threadPriority"] = "2",
//                ["quartz.jobStore.misfireThreshold"] = "600000" //// Amount of milliseconds that may pass till a trigger that couldn't start in time counts as misfired (600000 ms => 10 minutes)
            };

            try
            {
                var schedulerFactory = new StdSchedulerFactory(properties);
                _scheduler = schedulerFactory.GetScheduler().Result;
                _scheduler.Context.Put(SERVICE_PROVIDER, serviceProvider);

                // proxy server settings
                var pss = new ProxyServerSettings();
                applicationConfiguration.GetSection("ProxyServerSettings").Bind(pss);
                _scheduler.Context.Put(PROXY_SERVER_SETTINGS, pss);

//                _scheduler.Clear().Wait();
//                TaskSchedulerLogger.ClearAllLogs();
                ConfigureJobs();
                _logger.Info("QuartzScheduler has been initialized");

            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }

        }
        
        /// <summary>
        /// Start scheduler
        /// </summary>
        public static void Start()
        {
            if (_scheduler == null) throw new Exception("Scheduler is not initialized.");
            if (_isRunning) throw new Exception("Scheduler is already running.");

            try
            {
                _scheduler.Start().Wait();
                _isRunning = true;
                _logger.Info("QuartzScheduler has started");

            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        /// <summary>
        /// Stop scheduler
        /// </summary>
        public static void Stop()
        {
            if (!_isRunning) return;
            if (_scheduler == null) return;

            // give running jobs 30 sec (for example) to stop gracefully
            if (_scheduler.Shutdown(true).Wait(30000))
            {
                _scheduler = null;
                _isRunning = false;
                _logger.Info("QuartzScheduler shutdown.");
            }
            else
            {
                _logger.Error("QuartzScheduler shutdown failed.");
            }
        }

        /// <summary>
        /// Get all scheduled jobs
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<VmApiJobInfo> GetJobs()
        {
            var result = new List<VmApiJobInfo>();
            if (_scheduler == null) return result;

            var triggerKeys = _scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup()).Result;
            foreach (var triggerKey in triggerKeys)
            {
                var trigger = _scheduler.GetTrigger(triggerKey).Result as ICronTrigger;
                if (trigger == null) continue;

                var job = _scheduler.GetJobDetail(trigger.JobKey).Result;
                if (job == null) continue;

                var jobSchedulingConfiguration = GetJobSchedulingConfiguration(job);
                var nextFireTimeUtc = trigger.GetNextFireTimeUtc();
                result.Add(new VmApiJobInfo
                {
                    Name = job.Key.Name,
                    CronExpression = trigger.CronExpressionString,
                    State =  _scheduler.GetTriggerState(triggerKey).Result.ToString(),
                    LastFireTimeUtc = trigger.GetPreviousFireTimeUtc(),
                    NextFireTimeUtc = nextFireTimeUtc,
                    NextFireTimeLocal = nextFireTimeUtc?.LocalDateTime,
                    RegularScheduling = jobSchedulingConfiguration.Scheduler,
                    ExceptionScheduling = jobSchedulingConfiguration.SchedulerOnFail,
                    RetriesOnFails = jobSchedulingConfiguration.RetriesOnFail,
                    CountOfFailedExecutions = job.GetCountOfExceptionFires()
                });
            }
            return result;
        }

        /// <summary>
        /// Force job
        /// </summary>
        /// <param name="jobType"></param>
        public static void ForceJobExecution(JobTypeEnum jobType)
        {
            if (_scheduler == null) return;
//            var trigger = jobType.GetTrigger();
//            if (_scheduler.GetTriggerState(trigger.Key).Result == TriggerState.Paused)
//            {
//                throw new Exception($"Trigger of '{jobType}' is paused.");
//            }

            var jobKey = jobType.GetJobKey();
            var jobDataMap = new JobDataMap{{IS_FORCED, true}};
            _scheduler.TriggerJob(jobKey, jobDataMap).Wait();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        public static async Task ForceJobExecutionAsync(JobTypeEnum jobType)
        {
            if (_scheduler == null) return;
            var jobKey = jobType.GetJobKey();
            var jobDataMap = new JobDataMap { { IS_FORCED, true } };
            await _scheduler.TriggerJob(jobKey, jobDataMap).ConfigureAwait(false);
        }

        private static void ConfigureJobs()
        {
            if (_scheduler == null) return;

            RegisterJob<PostalCodeJobDataConfiguration, PostalCodesJob>();
            RegisterJob<MunicipalityCodeJobDataConfiguration, MunicipalityCodesJob>();
            RegisterJob<AreaCodeJobDataConfiguration, ProvinceCodesJob>();
            RegisterJob<AreaCodeJobDataConfiguration, HospitalRegionCodesJob>();
            RegisterJob<AreaCodeJobDataConfiguration, BusinessRegionCodesJob>();
            RegisterJob<FintoDataJobDataConfiguration, LifeEventsFintoJob>();
            RegisterJob<FintoDataJobDataConfiguration, ServiceClassesFintoJob>();
            RegisterJob<FintoDataJobDataConfiguration, TargetGroupsFintoJob>();
            RegisterJob<FintoDataJobDataConfiguration, OntologyTermsFintoJob>();

            var scheduledTriggerKeys = _scheduledTriggers.Values.Select(t => t.Key);
            var trigersToRemove = _scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupContains(PTV_JOB_GROUP)).Result
                .Where(triggerKey => !scheduledTriggerKeys.Contains(triggerKey))
                .ToList();
            _scheduler.UnscheduleJobs(trigersToRemove).Wait();

            var scheduledJobKeys = _scheduledTriggers.Values.Select(t => t.JobKey);
            var jobsToRemove = _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupContains(PTV_JOB_GROUP)).Result
                .Where(jobKey => !scheduledJobKeys.Contains(jobKey))
                .ToList();
            _scheduler.DeleteJobs(jobsToRemove).Wait();
        }

        internal static void PauseTrigger(TriggerKey triggerKey)
        {
            _scheduler?.PauseTrigger(triggerKey).Wait();
        }

        internal static bool ResumeTrigger(JobTypeEnum jobType, bool withRegularScheduling = true)
        {
            var trigger = jobType.GetTrigger();
            if (trigger == null) return false;
            
            if (withRegularScheduling)
            {
                var job = _scheduler.GetJobDetail(trigger.JobKey).Result;
                if (job == null) return false;

                var jobSchedulingConfiguration = GetJobSchedulingConfiguration(job);
                var regularScheduling = jobSchedulingConfiguration.Scheduler;
                _scheduler.RescheduleJob(trigger.Key, regularScheduling);
            }

            _scheduler.ResumeTrigger(trigger.Key).Wait();
            return true;
        }

        internal static bool PauseAllTriggers()
        {
            if (_scheduler == null) return false;
            _scheduler.PauseAll().Wait();
            return true;
        }

        internal static void RestartJob(JobTypeEnum jobType)
        {
            if (_scheduler == null) return;

            // get job
            if (!_scheduledTriggers.ContainsKey(jobType)) throw new Exception($"Job '{jobType}' is not scheduled.");
            var trigger = _scheduledTriggers[jobType];
            if (trigger == null) throw new Exception($"Trigger for job '{jobType}' was not found.");
            var jobKey = trigger.JobKey;
            if (jobKey == null) throw new Exception($"Job key for job '{jobType}' was not found.");
            var job = _scheduler.GetJobDetail(jobKey).Result;
            if (job == null) throw new Exception($"JobDetail for job '{jobType}' was not found.");

            var jt = job.JobType;

            // get job data type
            var jobData = job.JobDataMap[JOB_DATA_CONFIGURATION];
            var jobDataType = jobData.GetType();

            // load job configuration
            var loadConfigurationMethod = typeof(QuartzScheduler).GetMethod("LoadConfigurationForJob", BindingFlags.NonPublic | BindingFlags.Static);
            var genericMethod = loadConfigurationMethod.MakeGenericMethod(jobDataType);
            var jobConfiguration = genericMethod.Invoke(null, new object[]{jobType});
            if (jobConfiguration == null) throw new Exception($"Job configuration loading failed for job '{job.Key}'.");

            // unschedule trigger and delete job
            _scheduler.UnscheduleJob(trigger.Key).Wait();
            _scheduler.DeleteJob(jobKey).Wait();
            if (_scheduledTriggers.Values.Contains(trigger))
            {
                var scheduledTrigger = _scheduledTriggers.Single(i => i.Value.Key.Equals(trigger.Key));
                _scheduledTriggers.Remove(scheduledTrigger.Key);
            }

            // register job
            var registerJobMethod = typeof(QuartzScheduler).GetMethod("RegisterJob", BindingFlags.NonPublic | BindingFlags.Static);
            genericMethod = registerJobMethod.MakeGenericMethod(jobDataType, jt);
            genericMethod.Invoke(null, null);

            // resume
            _scheduler.ResumeTrigger(trigger.Key).Wait();
            _scheduler.ResumeJob(jobKey).Wait();
        }

        internal static void RestartAllJobs()
        {
            _scheduler.UnscheduleAllJobs();
            _scheduler.DeleteAllJobs();
            _scheduledTriggers.Clear();

            _scheduler.Standby().Wait();
            ConfigureJobs();
            ConfigureJobs();
            _scheduler.Start().Wait();
            _scheduler.ResumeAll().Wait();
        }

        private static void RegisterJob<TJobData, TJob>()
            where TJobData: IJobDataConfiguration<TJobData>
            where TJob: IJob, IBaseJob, new()
        {
            var jobType = new TJob().JobType;

            if (_scheduler == null) return;
            if (_configuration == null) return;
            if (_logger == null) return;
            if (_scheduledTriggers.ContainsKey(jobType))  throw new Exception($"Job '{jobType}' is already registered.");

            var jobConfiguration = LoadConfigurationForJob<TJobData>(jobType);

            var jobKey = new JobKey($"{jobType}Job", PTV_JOB_GROUP);
            var triggerKey = new TriggerKey($"{jobType}Trigger", PTV_JOB_GROUP);

            var jobExists = _scheduler.CheckExists(jobKey).Result;
            var triggerExists = _scheduler.CheckExists(triggerKey).Result;
            var registerNewJob = !jobExists && !triggerExists;

            // delete job when trigger does not exist
            if (jobExists && !triggerExists)
            {
                _scheduler.DeleteJob(jobKey).Wait();
                registerNewJob = true;
                _logger.Info($"Job '{jobKey}' has been deleted, because job trigger was not found.");
            }

            // unschedule trigger when job does not exist
            if (!jobExists && triggerExists)
            {
                _scheduler.UnscheduleJob(triggerKey).Wait();
                registerNewJob = true;
                _logger.Info($"Trigger '{triggerKey}' has been unscheduled, because trigger's job was not found.");
            }

            if (!registerNewJob)
            {
                var trigger = ValidateTrigger(triggerKey, jobKey, jobConfiguration);
                if (trigger == null)
                {
                    _scheduler.UnscheduleJob(triggerKey).Wait();
                    _scheduler.DeleteJob(jobKey).Wait();
                    registerNewJob = true;
                }
                else
                {
                    _scheduledTriggers.Add(jobType, trigger);
                }
            }

            if (registerNewJob)
            {
                var jobDataMap = new JobDataMap
                {
                    new KeyValuePair<string, object>(JOB_SCHEDULING_CONFIGURATION, jobConfiguration.Scheduling),
                    new KeyValuePair<string, object>(JOB_DATA_CONFIGURATION, jobConfiguration.JobData),
                };

                var job = JobBuilder.Create<TJob>()
                    .WithIdentity(jobKey)
                    .WithDescription(jobConfiguration.JobInfo.Description)
                    .UsingJobData(jobDataMap)
                    .UsingJobData(COUNT_OF_FAILED_EXECUTIONS, 0)
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .ForJob(job)
                    .StartNow()
                    .WithCronSchedule(jobConfiguration.Scheduling.Scheduler, c =>
                    {
                        c.WithMisfireHandlingInstructionFireAndProceed();
                        c.InTimeZone(TimeZoneInfo.Utc);
                    })
                    .Build();

                _scheduler.ScheduleJob(job, trigger).Wait();
                _scheduledTriggers.Add(jobType, (ICronTrigger)trigger);
                _logger.Info($"Trigger '{triggerKey}' for job '{jobKey}' has been scheduled.");
            }
        }

        private static ICronTrigger ValidateTrigger<TJobData>(TriggerKey triggerKey, JobKey jobKey, JobConfiguration<TJobData> jobConfiguration)
            where TJobData: IJobDataConfiguration<TJobData>
        {
            // validate trigger type
            var trigger = _scheduler.GetTrigger(triggerKey).Result as ICronTrigger;
            if (trigger == null)
            {
                _logger.Info($"Trigger '{triggerKey}' has been unscheduled, because it was not a cron trigger.");
                return null;
            }

            // check, that trigger has a job
            if (trigger.JobKey == null)
            {
                _logger.Info($"Trigger '{triggerKey}' has been unscheduled, because it had no job defined.");
                return null;
            }

            // check that job and trigger job is the same
            if (!trigger.JobKey.Equals(jobKey))
            {
                _logger.Info($"Trigger '{triggerKey}' has been unscheduled, because its job '{trigger.JobKey} didnt match job '{jobKey}'.");
                return null;
            }

            // if trigger is blocked => reschedule job and trigger
            var status = _scheduler.GetTriggerState(triggerKey).Result;
            if (status == TriggerState.Blocked)
            {
                _scheduler.ResumeJob(trigger.JobKey).Wait();
                _logger.Info($"Trigger '{triggerKey}' has been unscheduled, because it has been blocked.");
                return null;
            }

            // validate, that job belongs to just one trigger
            var jobTriggers = _scheduler.GetTriggersOfJob(jobKey).Result.ToList();
            if (jobTriggers.Count != 1)
            {
                _logger.Info($"Job '{jobKey}' has been deleted, because count of triggers didnt match.");
                _scheduler.UnscheduleJobs(jobTriggers.Select(t => t.Key).ToList()).Wait();
                return null;
            }

            // check, that jobs trigger match
            if (!jobTriggers[0].Key.Equals(triggerKey))
            {
                _logger.Info($"Job '{jobKey}' has been deleted, because count trigger '{jobTriggers[0].Key} didnt match '{triggerKey}'.");
                return null;
            }

            // validate job
            var job = _scheduler.GetJobDetail(jobKey).Result;
            if (job == null)
            {
                _logger.Info($"Trigger '{triggerKey}' has been unscheduled, because job '{jobKey}' was not found.");
                return null;
            }

            // validate job description
            if (jobConfiguration.JobInfo.Description != job.Description)
            {
                _logger.Info($"Trigger '{triggerKey}' has been unscheduled, because job description changed.");
                return null;
            }
            
            return ValidateJobConfiguration(trigger, job, jobConfiguration) ? trigger : null;
        }

        private static bool ValidateJobConfiguration<TJobData>(ICronTrigger trigger, IJobDetail job, JobConfiguration<TJobData> jobConfiguration)
            where TJobData: IJobDataConfiguration<TJobData>
        {
            if (job == null) return false;
            var jobDataMap = job.JobDataMap;

            var jobScheduling = jobDataMap[JOB_SCHEDULING_CONFIGURATION] as JobSchedulingConfiguration;
            if (jobScheduling == null)
            {
                _logger.Info($"Job '{job.Key}' has been unscheduled, because of missing job scheduling information.");
                return false;
            }

            if (!jobScheduling.Equals(jobConfiguration.Scheduling))
            {
                _logger.Info($"Job '{job.Key}' has been unscheduled, because jobs scheduling differs from configured scheduling.");
                return false;
            }

            var jobData = jobDataMap[JOB_DATA_CONFIGURATION];
            if (jobData == null)
            {
                _logger.Info($"Job '{job.Key}' has been unscheduled, because of missing job data information.");
                return false;
            }

            if (jobData.GetType() != typeof(TJobData))
            {
                _logger.Info($"Job '{job.Key}' has been unscheduled, because jobs data type differs from configured job data type.");
                return false;
            }

            var jobDataType = (TJobData) jobData;
            if (!jobDataType.Equals(jobConfiguration.JobData))
            {
                _logger.Info($"Job '{job.Key}' has been unscheduled, because jobs data differs from configured job data.");
                return false;
            }

            // check trigger cron expression
            // reschedule, if needed
            var countOfFailedExecutions = job.GetCountOfExceptionFires();
            var isPaused = _scheduler.GetTriggerState(trigger.Key).Result == TriggerState.Paused;
            var configurationCronExpression = (countOfFailedExecutions > 0 || isPaused) ? jobConfiguration.Scheduling.SchedulerOnFail : jobConfiguration.Scheduling.Scheduler;
            if (!configurationCronExpression.Equals(trigger.CronExpressionString))
            {
                _scheduler.RescheduleJob(trigger.Key, configurationCronExpression);
            }

            return true;
        }

        private static JobConfiguration<TJobData> LoadConfigurationForJob<TJobData>(JobTypeEnum jobType)
            where TJobData: IJobDataConfiguration<TJobData>
        {
            var jobConfiguration = new JobConfiguration<TJobData>();
            var configuration = _configuration.GetSection($"{jobType}");
            configuration.Bind(jobConfiguration);
            if (jobConfiguration.JobInfo == null) throw new Exception($"JobInfo binding failed for type '{jobType}'.");
            if (jobConfiguration.Scheduling == null) throw new Exception($"Scheduling binding failed for type '{jobType}'.");
            if (jobConfiguration.JobData == null) throw new Exception($"JobData binding failed for type '{jobType}'.");

            return jobConfiguration;
        }

        /// <summary>
        /// Get job data of job
        /// </summary>
        /// <typeparam name="TJobData"></typeparam>
        /// <param name="job"></param>
        /// <returns></returns>
        public static TJobData GetJobDataConfiguration<TJobData>(IJobDetail job)
            where TJobData: IJobDataConfiguration<TJobData>
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            var jobData = job.JobDataMap[JOB_DATA_CONFIGURATION];
            if (jobData == null) throw new Exception($"Job data are not configured for job '{job.Key}'.");
            if (jobData.GetType() != typeof(TJobData)) throw new Exception($"Job data type mismatch. Expected type: '{typeof(TJobData)}', current type: '{jobData.GetType()}'.");
            return (TJobData)jobData;
        }

        /// <summary>
        /// Get scheduling configuration of job
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public static JobSchedulingConfiguration GetJobSchedulingConfiguration(IJobDetail job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            var jobSchedulingConfiguration = job.JobDataMap[JOB_SCHEDULING_CONFIGURATION] as JobSchedulingConfiguration;
            if (jobSchedulingConfiguration == null) throw new Exception($"Job scheduler configuration is not configured for job '{job.Key}'.");
            return jobSchedulingConfiguration;
        }

        private static JobKey GetJobKey(this JobTypeEnum jobType)
        {
            if (_scheduler == null) return null;
            if (!_scheduledTriggers.ContainsKey(jobType)) throw new Exception($"Job '{jobType}' is not scheduled.");
            var trigger = _scheduledTriggers[jobType];
            var jobKey = trigger.JobKey;
            if (jobKey == null) throw new Exception($"No job is defined for trigger '{trigger.Key}'.");
            return jobKey;
        }

        private static ICronTrigger GetTrigger(this JobTypeEnum jobType)
        {
            if (_scheduler == null) return null;
            if (!_scheduledTriggers.ContainsKey(jobType)) throw new Exception($"Job '{jobType}' is not scheduled.");
            return _scheduledTriggers[jobType];
        }
    }

}
