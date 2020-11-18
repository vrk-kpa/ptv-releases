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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Jobs;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Extensions;
using PTV.TaskScheduler.Configuration;
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
    public static partial class QuartzScheduler
    {
        internal const string COUNT_OF_FAILED_EXECUTIONS = "COUNT_OF_FAILED_EXECUTIONS";
        internal const string IS_FORCED = "IS_FORCED";
        internal const string SERVICE_PROVIDER = "SERVICE_PROVIDER";

        private const string JOB_SCHEDULING_CONFIGURATION = "JOB_SCHEDULING_CONFIGURATION";
        private const string JOB_DATA_CONFIGURATION = "JOB_DATA_CONFIGURATION";
        private const string APPLICATION_CONFIGURATION = "APPLICATION_CONFIGURATION";

        private const string PTV_JOB_GROUP = "PTV_JOB_GROUP";
        private static readonly Dictionary<string, ICronTrigger> _scheduledTriggers = new Dictionary<string, ICronTrigger>();
        private static readonly Dictionary<string, string> _notStartedTriggers = new Dictionary<string, string>();

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

            _configuration = serviceProvider.GetService<IConfiguration>();

            var jobStatusService = serviceProvider.GetService<IJobStatusService>();
            var applicationConfiguration = serviceProvider.GetRequiredService<IConfigurationRoot>();
            var databaseSchema = applicationConfiguration.GetConnectionString("QuartzSchemaName");
            var ptvConfiguration = new ApplicationConfiguration(applicationConfiguration);

            var properties = new NameValueCollection
            {
                ["quartz.serializer.type"] = "json",
                ["quartz.scheduler.instanceName"] = "PtvScheduler",
                ["quartz.scheduler.instanceId"] = "ptv_quartz_instance",
                ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
                ["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.StdAdoDelegate, Quartz",
                ["quartz.jobStore.useProperties"] = "false",
                ["quartz.jobStore.dataSource"] = "default",
                ["quartz.jobStore.tablePrefix"] = $"{databaseSchema}.QRTZ_",
                ["quartz.jobStore.maxMisfiresToHandleAtATime"] = "1",
                ["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz",
                ["quartz.dataSource.default.connectionString"] = ptvConfiguration.GetAwsConnectionString(AwsDbConnectionStringEnum.QuartzConnection),
                ["quartz.dataSource.default.provider"] = "Npgsql",
                ["quartz.threadPool.threadCount"] = "5", // Allows only 5 Thread in parallel
//                ["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz",
//                ["quartz.threadPool.threadPriority"] = "2",
//                ["quartz.jobStore.misfireThreshold"] = "600000" //// Amount of milliseconds that may pass till a trigger that couldn't start in time counts as misfired (600000 ms => 10 minutes)
            };

            try
            {
                lock (serviceProvider)
                {
                    var schedulerFactory = new StdSchedulerFactory(properties);
                    if(_scheduler != null) return;
                    _scheduler = schedulerFactory.GetScheduler().Result;
                    _scheduler.Context.Put(SERVICE_PROVIDER, serviceProvider);
                    _scheduler.Context.Put(APPLICATION_CONFIGURATION, applicationConfiguration);

//                _scheduler.Clear().Wait();
//                TaskSchedulerLogger.ClearAllLogs();
                    ConfigureJobs(jobStatusService);
                    ResumeAllPausedTriggers();
                    _logger.Info("QuartzScheduler has been initialized");
                }

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
            if (_scheduler == null) return;
            lock (_scheduler)
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
        }

        /// <summary>
        /// Stop scheduler
        /// </summary>
        public static void Stop()
        {
            if (_scheduler == null) return;
            lock (_scheduler)
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
        }

        /// <summary>
        /// Get job
        /// </summary>
        /// <returns></returns>
        public static VmApiJobInfo GetJob(string jobKeyStr, IJobStatusService jobStatusService)
        {
            var result = new VmApiJobInfo();
            if (_scheduler == null) return result;
            lock (_scheduler)
            {
                var triggerKeys = _scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup()).Result;

                var triggerKey = triggerKeys.SingleOrDefault(x => (_scheduler.GetTrigger(x).Result as ICronTrigger)?.JobKey?.Name == jobKeyStr);
                if (triggerKey == null) return result;

                if (!(_scheduler.GetTrigger(triggerKey).Result is ICronTrigger trigger)) return result;

                var job = _scheduler.GetJobDetail(trigger.JobKey).Result;
                var jobSchedulingConfiguration = GetJobSchedulingConfiguration(job);
                var nextFireTimeUtc = trigger.GetNextFireTimeUtc();
                
                var summary = jobStatusService.GetJobsSummary(job.JobType.Name).ToList();
                var archiveSummary = jobStatusService.GetJobsSummary(job.JobType.Name, true).ToList();

                result.Code = job.Key.Name;
                result.CronExpression = trigger.CronExpressionString;
                result.State = _scheduler.GetTriggerState(triggerKey).Result.ToString();
                result.LastFireTimeUtc = trigger.GetPreviousFireTimeUtc();
                result.NextFireTimeUtc = nextFireTimeUtc;
                result.NextFireTimeLocal = nextFireTimeUtc?.LocalDateTime;
                result.RegularScheduling = jobSchedulingConfiguration.Scheduler;
                result.ExceptionScheduling = jobSchedulingConfiguration.SchedulerOnFail;
                result.RetriesOnFails = jobSchedulingConfiguration.RetriesOnFail;
                result.CountOfFailedExecutions = GetFailedJobCountFromLogs(summary, archiveSummary);
                result.IsRunning = IsJobRunning(job.Key.Name);
                result.Summary = summary;
                result.ArchiveSummary = archiveSummary;
                result.LastJobStatus = GetLastJobStatus(summary, archiveSummary)?.ToString();
                result.LastExecutionTime = GetLastExecutionTime(summary, archiveSummary).ToMillisecondsTime();
                return result;
            }
        }

        private static JobResultStatusEnum? GetLastJobStatus(List<VmJobSummary> actualLog, List<VmJobSummary> archiveLog)
        {
            JobResultStatusEnum? result = null;
            var lastStatus = actualLog.FirstOrDefault(x => x.Logs.Any(y => y.JobStatus.HasValue));
            if (lastStatus == null)
            {
                lastStatus = archiveLog.FirstOrDefault(x => x.Logs.Any(y => y.JobStatus.HasValue));
            }

            if (lastStatus != null)
            {
                result = lastStatus.Logs.First(x => x.JobStatus.HasValue).JobStatus;
            }

            return result;
        }

        private static TimeSpan GetLastExecutionTime(List<VmJobSummary> actualLog, List<VmJobSummary> archiveLog)
        {
            var result = new TimeSpan();

            var lastStatus = actualLog.FirstOrDefault(x => x.ExecutionTime.Ticks > 0);
            if (lastStatus == null)
            {
                lastStatus = archiveLog.FirstOrDefault(x => x.ExecutionTime.Ticks > 0);
            }

            if (lastStatus != null)
            {
                result = lastStatus.ExecutionTime;
            }

            return result;
        }

        private static int GetFailedJobCountFromLogs(List<VmJobSummary> actualLog, List<VmJobSummary> archiveLog)
        {
            var allLogsData = actualLog.Union(archiveLog).DistinctBy(x => x.OperationId).ToList();
            var result = allLogsData.SelectMany(x => x.Logs.Where(y => y.JobStatus == JobResultStatusEnum.Fail)).Count();
            return result;
        }

        public static async Task<IEnumerable<string>> GetScheduledJobNames()
        {
            var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            return jobKeys?.Select(j => j.Name);
        }

        public static IEnumerable<VmApiNotStartedJobInfo> GetNotStartedJobs()
        {
            return _notStartedTriggers.Select(t => new VmApiNotStartedJobInfo { Name = t.Key, Exception = t.Value});
        }

        /// <summary>
        /// Force job
        /// </summary>
        /// <param name="jobKeyStr"></param>
        public static void ForceJobExecution(string jobKeyStr)
        {
            if (_scheduler == null) return;
//            var trigger = jobType.GetTrigger();
//            if (_scheduler.GetTriggerState(trigger.Key).Result == TriggerState.Paused)
//            {
//                throw new Exception($"Trigger of '{jobType}' is paused.");
//            }
            lock (_scheduler)
            {
                var jobKey = GetJobKey(jobKeyStr);
                var jobDataMap = new JobDataMap {{IS_FORCED, true}};
                _scheduler.TriggerJob(jobKey, jobDataMap).Wait();
            }
        }

//        /// <summary>
//        ///
//        /// </summary>
//        /// <param name="jobType"></param>
//        /// <returns></returns>
//        public static async Task ForceJobExecutionAsync(JobTypeEnum jobType)
//        {
//            if (_scheduler == null) return;
//            var jobKey = jobType.GetJobKey();
//            var jobDataMap = new JobDataMap { { IS_FORCED, true } };
//            await _scheduler.TriggerJob(jobKey, jobDataMap).ConfigureAwait(false);
//        }

        private static void ConfigureJobs(IJobStatusService jobStatusService)
        {
            if (_scheduler == null) return;
            lock (_scheduler)
            {
                RegisterJob<PostiStreetLoaderJob>();
                RegisterJob<PostiPostalCodesJob>();
                RegisterJob<MunicipalityCodesJob, UrlJobDataConfiguration>();
                RegisterJob<ProvinceCodesJob, AreaCodeJobDataConfiguration>();
                RegisterJob<HospitalRegionCodesJob, AreaCodeJobDataConfiguration>();
                RegisterJob<LifeEventsJob, UrlJobDataConfiguration>();
                RegisterJob<IndustrialClassesJob, UrlJobDataConfiguration>();
                RegisterJob<ServiceClassesJob, UrlJobDataConfiguration>();
                RegisterJob<TargetGroupsJob, UrlJobDataConfiguration>();
                RegisterJob<OrganizationTypesJob, UrlJobDataConfiguration>();
                RegisterJob<ProvisionTypesJob, UrlJobDataConfiguration>();
                RegisterJob<OntologyTermsFintoJob, FintoDataJobDataConfiguration>();
                RegisterJob<DigitalAuthorizationCodesJob, UrlJobDataConfiguration>();
                RegisterJob<TranslationOrderSendNewJob, TranslationOrderJobDataConfiguration>();
                RegisterJob<TranslationOrderProcessingDataByStateJob, TranslationOrderJobDataConfiguration>();
                RegisterJob<TranslationOrderSendAgainJob, TranslationOrderJobDataConfiguration>();
                RegisterJob<TranslationOrderHandlingJob, TranslationOrderJobDataConfiguration>();
                RegisterJob<ArchiveEntitiesByExpirationDateJob>();
                RegisterJob<ClearNotificationsJob>();
                RegisterJob<RestoreCleanDatabaseJob>();
                
                // SFIPTV-1148 - do not run soteJob till 2019-03-20
                // RegisterJob<SoteJob, SoteJobDataConfiguration>();
                
                RegisterJob<MassJob>();
                RegisterJob<GeoServerJob, GeoServerJobConfiguration>();
                RegisterJob<StreetDataJob>();
                RegisterJob<PostalCodeCoordinatesJob, PostalCodeCoordinatesJobConfiguration>();
                RegisterJob<EmailNotifyJob, EmailNotifyJobDataConfiguration>();
                
                RegisterJob<OldArchivedAncientJob, OldArchivedAncientConfiguration>();
                RegisterJob<PermanentDeleteJob, PermanentDeleteConfiguration>();
                RegisterJob<AccessibilityRegisterJob, AccessibilityRegisterConfiguration>();
                RegisterJob<LocalizationJob>();
                RegisterJob<BrokenLinkJob, BrokenLinkConfiguration>();
                RegisterJob<HolidayJob>();
                RegisterJob<ServiceNumberJob, UrlJobDataConfiguration>();
                RegisterJob<DatabaseBackupJob, DatabaseBackupConfiguration>();
                

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

            // log started triggers
            _scheduledTriggers.ForEach(trigger =>
            {
                var summary = new VmJobSummary("JobRegister", trigger.Key);
                TaskSchedulerLogger.LogJobInfo(summary, $"Trigger '{trigger.Key}' has been scheduled with cron expression '{trigger.Value.CronExpressionString}', next fire: {trigger.Value.GetNextFireTimeUtc()}", jobStatusService);
            });

                // log not-started jobs
                _notStartedTriggers.ForEach(t =>
                {
                    var summary = new VmJobSummary("JobConfiguration", t.Key);
                    TaskSchedulerLogger.LogJobWarn(summary, $"'Trigger {t.Key}' has not been started. [{t.Value}]", jobStatusService);
                });
            }
        }

        private static void PauseTrigger(TriggerKey triggerKey)
        {
            if (_scheduler == null) return;
            lock (_scheduler)
            {
                _scheduler.PauseTrigger(triggerKey).Wait();
            }
        }

        internal static void PauseTrigger(string jobKey)
        {
            var trigger = GetTrigger(jobKey);
            PauseTrigger(trigger.Key);
        }

        internal static bool ResumeTrigger(string jobKey, bool withRegularScheduling = true)
        {
            var trigger = GetTrigger(jobKey);
            return ResumeTrigger(trigger.Key, withRegularScheduling);
        }

        private static bool ResumeTrigger(TriggerKey triggerKey, bool withRegularScheduling = true)
        {
            if (triggerKey == null) return false;
            if (_scheduler == null) return false;
            lock (_scheduler)
            {
                var trigger = _scheduler.GetTrigger(triggerKey).Result;

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
        }

        internal static bool PauseAllTriggers()
        {
            if (_scheduler == null) return false;
            lock (_scheduler)
            {
                _scheduler.PauseAll().Wait();
                return true;
            }
        }

        internal static void RestartJob(string jobKeyStr)
        {
            if (_scheduler == null) return;
            lock (_scheduler)
            {
                // get job
                if (!_scheduledTriggers.ContainsKey(jobKeyStr)) throw new Exception($"Job '{jobKeyStr}' is not scheduled.");
                var trigger = _scheduledTriggers[jobKeyStr];
                if (trigger == null) throw new Exception($"Trigger for job '{jobKeyStr}' was not found.");
                var jobKey = trigger.JobKey;
                if (jobKey == null) throw new Exception($"Job key for job '{jobKeyStr}' was not found.");
                var job = _scheduler.GetJobDetail(jobKey).Result;
                if (job == null) throw new Exception($"JobDetail for job '{jobKeyStr}' was not found.");

                var jt = job.JobType;

                // get job data type
                var jobData = job.JobDataMap[JOB_DATA_CONFIGURATION];
                var jobDataType = jobData.GetType();

                // load job configuration
                var loadConfigurationMethod = typeof(QuartzScheduler).GetMethod("LoadConfigurationForJob", BindingFlags.NonPublic | BindingFlags.Static);
                var genericMethod = loadConfigurationMethod.MakeGenericMethod(jobDataType);
                var jobConfiguration = genericMethod.Invoke(null, new object[] {jobKeyStr});
                if (jobConfiguration == null) throw new Exception($"Job configuration loading failed for job '{job.Key}'.");

                // get executing jobs
                var executingJobs = _scheduler.GetCurrentlyExecutingJobs().Result.ToList();
                if (executingJobs.Count > 0)
                {
                    var jobList = executingJobs.Where(j => j.JobDetail.Equals(job)).ToList();
                    foreach (var ej in jobList)
                    {
                        _scheduler.Interrupt(ej.FireInstanceId).Wait();
                        var t = ej.Trigger;
                        if (t != null)
                        {
                            _scheduler.UnscheduleJob(t.Key).Wait();
                        }
                    }
                }

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

                // force restart
//            _scheduler.Shutdown().Wait();
//            _scheduler.Start().Wait();
            }
        }

        internal static void RestartAllJobs(IJobStatusService jobStatusService)
        {
            if (_scheduler == null) return;
            lock (_scheduler)
            {
                _scheduler.UnscheduleAllJobs();
                _scheduler.DeleteAllJobs();
                _scheduledTriggers.Clear();

                _scheduler.Standby().Wait();
                ConfigureJobs(jobStatusService);
                ConfigureJobs(jobStatusService);
                _scheduler.Start().Wait();
                _scheduler.ResumeAll().Wait();
            }
        }

        private static void RegisterJob<TJob>() where TJob : IJob, IBaseJob, new()
        {
            RegisterJob<TJob, EmptyJobDataConfiguration>();
        }

        private static void RegisterJob<TJob, TJobData>()
            where TJobData: IJobDataConfiguration<TJobData>
            where TJob: IJob, IBaseJob, new()
        {
            if (_scheduler == null) return;
            lock (_scheduler)
            {
                if (_configuration == null) return;
                if (_logger == null) return;
                var jobType = new TJob().JobKey;
                if (_scheduledTriggers.ContainsKey(jobType)) throw new Exception($"Job '{jobType}' is already registered.");

                var jobConfiguration = LoadConfigurationForJob<TJobData>(jobType);
                if (jobConfiguration == null) return;

                var jobKey = new JobKey(jobType, PTV_JOB_GROUP);
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
                    _scheduledTriggers.Add(jobType, (ICronTrigger) trigger);
                    _logger.Info($"Trigger '{triggerKey}' for job '{jobKey}' has been scheduled.");
                }
            }
        }

        private static ICronTrigger ValidateTrigger<TJobData>(TriggerKey triggerKey, JobKey jobKey, JobConfiguration<TJobData> jobConfiguration)
            where TJobData: IJobDataConfiguration<TJobData>

        {
            if (_scheduler == null) return null;
            lock (_scheduler)
            {
                // validate trigger type
                if (!(_scheduler.GetTrigger(triggerKey).Result is ICronTrigger trigger))
                {
                    _logger.Info($"Trigger '{triggerKey}' has been unscheduled, because it was not a cron trigger.");
                    return null;
                }

                // check scheduler time zone
                if (!trigger.TimeZone.Equals(TimeZoneInfo.Utc))
                {
                    _logger.Info($"Trigger '{triggerKey}' has been unscheduled, because time zone was not UTC (was '{trigger.TimeZone.StandardName}').");
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
                IJobDetail job;
                try
                {
                    job = _scheduler.GetJobDetail(jobKey).Result;
                    if (job == null)
                    {
                        _logger.Info($"Trigger '{triggerKey}' has been unscheduled, because job '{jobKey}' was not found.");
                        return null;
                    }
                }
                catch (Exception e)
                {
                    _logger.Info($"Trigger '{triggerKey}' has been unscheduled, because job '{jobKey}' couldn't be deserilized: {e.Message}.");
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
        }

        private static bool ValidateJobConfiguration<TJobData>(ICronTrigger trigger, IJobDetail job, JobConfiguration<TJobData> jobConfiguration)
            where TJobData: IJobDataConfiguration<TJobData>
        {
            if (job == null) return false;
            if (_scheduler == null) return false;
            lock (_scheduler)
            {
                var jobDataMap = job.JobDataMap;

                if (!(jobDataMap[JOB_SCHEDULING_CONFIGURATION] is JobSchedulingConfiguration jobScheduling))
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
            var isEmptyJobConfiguration = typeof(TJobData) == typeof(EmptyJobDataConfiguration);

            if (!isEmptyJobConfiguration)
            {
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
        }

        private static JobConfiguration<TJobData> LoadConfigurationForJob<TJobData>(string jobKeyStr)
            where TJobData: IJobDataConfiguration<TJobData>
        {
            if (_scheduler == null) return null;
            lock (_scheduler)
            {
                var jobConfiguration = new JobConfiguration<TJobData>();
                var configuration = _configuration.GetSection(jobKeyStr);
                configuration.Bind(jobConfiguration);

                if (jobConfiguration.JobInfo == null)
                {
                    _notStartedTriggers.Add(jobKeyStr, $"Binding of configuration for JobInfo failed for type '{jobKeyStr}'.");
                    return null;
                    //throw new Exception($"JobInfo binding failed for type '{jobType}'.");
                }

                if (jobConfiguration.Scheduling == null)
                {
                    _notStartedTriggers.Add(jobKeyStr, $"Binding of configuration for Scheduling failed for type '{jobKeyStr}'.");
                    return null;
                    //throw new Exception($"Scheduling binding failed for type '{jobType}'.");
                }

                if (typeof(TJobData) == typeof(EmptyJobDataConfiguration))
                {
                    // do not handle JobData for EmptyJobDataConfiguration
                    return jobConfiguration;
                }

                if (jobConfiguration.JobData == null)
                {
                    _notStartedTriggers.Add(jobKeyStr, $"Binding of configuration for JobData failed for type '{jobKeyStr}'.");
                    return null;
                    //throw new Exception($"JobData binding failed for type '{jobType}'.");
                }

                if (jobConfiguration.JobData is IApplicationJobConfiguration applicationJobConfiguration && _scheduler != null)
                {
                    var applicationConfiguration = _scheduler.Context.Get(APPLICATION_CONFIGURATION) as IConfigurationRoot;
                    var awsConfig = new ApplicationConfiguration(applicationConfiguration);
                    awsConfig.GetAwsConfiguration(applicationJobConfiguration.ConfigurationName).Bind(jobConfiguration.JobData);
                }

                if (!jobConfiguration.JobData.Validate())
                {
                    _notStartedTriggers.Add(jobKeyStr, $"Validation of JobData for type '{jobKeyStr}' failed.");
                    return null;
                }

                return jobConfiguration;
            }
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
        /// Get is allowed for displaying job setting
        /// </summary>
        /// <typeparam name="TJobData"></typeparam>
        /// <param name="job"></param>
        /// <param name="jobKeyStr"></param>
        /// <returns></returns>
        private static bool GetAllowedForDisplayingJobConfiguration(string jobKeyStr)
        {
            var allowedForDisplayingConfiguration = new AllowedForDisplayingJobConfiguration();
            if (!string.IsNullOrEmpty(jobKeyStr))
            {
                _configuration?.GetSection(jobKeyStr).Bind(allowedForDisplayingConfiguration);
            }
            return allowedForDisplayingConfiguration.AllowForDisplaying;
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

        private static JobKey GetJobKey(string jobKeyStr)
        {
            if (_scheduler == null) return null;
            lock (_scheduler)
            {
                if (!_scheduledTriggers.ContainsKey(jobKeyStr)) throw new Exception($"Job '{jobKeyStr}' is not scheduled.");
                var trigger = _scheduledTriggers[jobKeyStr];
                var jobKey = trigger.JobKey;
                if (jobKey == null) throw new Exception($"No job is defined for trigger '{trigger.Key}'.");
                return jobKey;
            }
        }

        private static ICronTrigger GetTrigger(string jobKeyStr)
        {
            if (_scheduler == null) return null;
            lock (_scheduler)
            {
                if (!_scheduledTriggers.ContainsKey(jobKeyStr)) throw new Exception($"Job '{jobKeyStr}' is not scheduled.");
                return _scheduledTriggers[jobKeyStr];
            }
        }

        public static IEnumerable<string> GetRunningJobIds()
        {
            return _scheduler?.GetCurrentlyExecutingJobs().Result.Select(job => job.FireInstanceId);
        }

        public static bool IsJobRunning(string jobKeyStr)
        {
            return _scheduler?.GetCurrentlyExecutingJobs().Result.Any(job => job.JobDetail?.Key.Name == jobKeyStr) ?? false;
        }

        public static IEnumerable<VmApiExecutingJobInfo> GetExecutingJobs()
        {
            if (_scheduler == null) return null;
            lock (_scheduler)
            {
                var executingJobs = _scheduler?.GetCurrentlyExecutingJobs().Result;
                return executingJobs?.Select(job => new VmApiExecutingJobInfo
                {
                    JobType = job.JobDetail?.Key.Name,
                    StartingTimeUtc = job.FireTimeUtc,
                    StartingTimeLocal = job.FireTimeUtc.LocalDateTime,
                    ExecutionTime = job.JobRunTime
                });
            }
        }

        private static void ResumeAllPausedTriggers()
        {
            if (_scheduler == null) return;
            lock (_scheduler)
            {
                foreach (var triggerKey in _scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup()).Result.ToList())
                {
                    if (_scheduler.GetTriggerState(triggerKey).Result.IsPaused())
                    {
                        ResumeTrigger(triggerKey);
                    }
                }
            }
        }
    }

}
