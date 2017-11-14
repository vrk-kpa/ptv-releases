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
using System.IO;
//using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Targets;
using PTV.Framework;
using PTV.TaskScheduler.Enums;
using PTV.TaskScheduler.Models;

namespace PTV.TaskScheduler
{
    /// <summary>
    /// TaskScheduller logger
    /// </summary>
    public static class TaskSchedulerLogger
    {

        private const string JSON_JOB_LOGGER = "jsonJobLogger";
        private const string JOB_LOGGER_NAME= "IJob";

        internal static ILogger JobLogger { get; } = LogManager.GetLogger(JOB_LOGGER_NAME);

        /// <summary>
        /// Get all job logs
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<VmJsonJobLog> GetAllJobsLog()
        {
            var result = new List<VmJsonJobLog>();
            var jsonLogger = LogManager.Configuration.FindTargetByName(JSON_JOB_LOGGER) as FileTarget;
            if (jsonLogger == null) return result;

            var fileName = jsonLogger.FileName.Render(new LogEventInfo());
            if (!File.Exists(fileName)) return result;

            using (var file = File.OpenText(fileName))
            {

                var jsonReader = new JsonTextReader(file) { SupportMultipleContent = true };
                while (jsonReader.Read())
                {
                    result.Add(JObject.Load(jsonReader).ToObject<VmJsonJobLog>());
                }
            }

            return result.OrderByDescending(log => log.Time).ThenByDescending(log => log.Level);
        }

        /// <summary>
        /// Clear all logs
        /// </summary>
        public static void ClearAllLogs()
        {
            LogManager.Configuration.AllTargets
                .OfType<FileTarget>()
                .Select(l => l.FileName.Render(new LogEventInfo()))
                .ForEach(l => { if (File.Exists(l)) File.Delete(l); });
        }

        /// <summary>
        /// Clear job logs
        /// </summary>
        public static void ClearJobLogs()
        {
            var jsonLogger = LogManager.Configuration.FindTargetByName(JSON_JOB_LOGGER) as FileTarget;
            if (jsonLogger == null) return;

            var fileName = jsonLogger.FileName.Render(new LogEventInfo());
            if (!File.Exists(fileName)) return;

            File.Delete(fileName);
        }

        /// <summary>
        /// Log job info
        /// </summary>
        /// <param name="jobType"></param>
        /// <param name="executionType"></param>
        /// <param name="resultStatus"></param>
        /// <param name="message"></param>
        public static void LogJobInfo(JobTypeEnum jobType, JobExecutionTypeEnum executionType, JobResultStatusEnum resultStatus, string message)
        {
//            var theEvent = new LogEventInfo(LogLevel.Info, JOB_LOGGER_NAME, message);
//            theEvent.Properties["Status"] = JobResultStatusEnum.Success.ToString();
//            theEvent.Properties["JobType"] = jobType.ToString();
//            theEvent.Properties["IsForced"] = isForced.ToString();
//            JobLogger.Log(theEvent);
            LogJobEvent(LogLevel.Info, jobType, executionType, resultStatus, message);
        }

        /// <summary>
        /// Log job error
        /// </summary>
        /// <param name="jobType"></param>
        /// <param name="executionType"></param>
        /// <param name="resultStatus"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void LogJobError(JobTypeEnum jobType, JobExecutionTypeEnum executionType, JobResultStatusEnum resultStatus, string message, Exception exception)
        {
            //            var theEvent = new LogEventInfo(LogLevel.Error, JOB_LOGGER_NAME, message);
            //            theEvent.Properties["Status"] = JobResultStatusEnum.Success.ToString();
            //            theEvent.Properties["JobType"] = jobType.ToString();
            //            theEvent.Properties["IsForced"] = isForced.ToString();
            //            JobLogger.Log(theEvent);
            LogJobEvent(LogLevel.Error, jobType, executionType, resultStatus, message, exception);
        }

        /// <summary>
        /// Log job event
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="jobType"></param>
        /// <param name="executionType"></param>
        /// <param name="resultStatus"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        private static void LogJobEvent(LogLevel logLevel, JobTypeEnum jobType, JobExecutionTypeEnum executionType, JobResultStatusEnum resultStatus, string message, Exception exception = null)
        {
            var theEvent = new LogEventInfo(logLevel, JOB_LOGGER_NAME, message);
            theEvent.Properties["JobStatus"] = resultStatus.ToString();
            theEvent.Properties["JobType"] = jobType.ToString();
            theEvent.Properties["ExecutionType"] = executionType.ToString();
            if (exception != null) theEvent.Exception = exception;
            JobLogger.Log(theEvent);
        }


    }
}
