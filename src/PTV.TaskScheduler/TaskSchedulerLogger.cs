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
using System.IO.Compression;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Targets;
using PTV.Framework;
using PTV.Framework.Logging;
using PTV.TaskScheduler.Enums;
using PTV.TaskScheduler.Models;

namespace PTV.TaskScheduler
{
    /// <summary>
    /// TaskScheduller logger
    /// </summary>
    public static class TaskSchedulerLogger
    {

        private const string JSON_JOB_LOGGER = "jsonJobLoggerInner";
        private const string JOB_LOGGER_NAME = "IJob";

        internal static ILogger JobLogger { get; } = LogManager.GetLogger(JOB_LOGGER_NAME);

        /// <summary>
        /// Get all job logs
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<VmJsonJobLog> GetAllJobsLog()
        {
            return GetAllJobsLogInternal().OrderByDescending(log => log.Time).ThenByDescending(log => log.Level);
        }

        private static IEnumerable<VmJsonJobLog> GetAllJobsLogInternal()
        {
            var result = new List<VmJsonJobLog>();
            if (!(LogManager.Configuration.FindTargetByName(JSON_JOB_LOGGER) is FileTarget jsonLogger)) return result;

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

            return result;
        }
        
        private static IEnumerable<VmJsonJobLog> GetAllJobsArchiveLogInternal()
        {
            var result = new List<VmJsonJobLog>();
            if (!(LogManager.Configuration.FindTargetByName(JSON_JOB_LOGGER) is FileTarget jsonLogger)) return result;
            
            var fileName = jsonLogger.ArchiveFileName.Render(new LogEventInfo());

            var archiveDir = Path.GetDirectoryName(fileName);

            var allFiles = Directory.GetFiles(archiveDir);
            foreach (var archiveFile in allFiles)
            {
                if (!File.Exists(archiveFile)) return result;

                using (var file = File.OpenText(archiveFile))
                {

                    var jsonReader = new JsonTextReader(file) { SupportMultipleContent = true };
                    while (jsonReader.Read())
                    {
                        result.Add(JObject.Load(jsonReader).ToObject<VmJsonJobLog>());
                    }
                }
            }
            return result;
        }

        public static FileResult GetAllLogsFiles()
        {
            byte[] compressedBytes;
            
            if (!(LogManager.Configuration.FindTargetByName(JSON_JOB_LOGGER) is FileTarget jsonLogger)) return null;
            
            var archiveFileName = jsonLogger.ArchiveFileName.Render(new LogEventInfo());

            var archiveDir = Path.GetDirectoryName(archiveFileName);

            var allFiles = Directory.GetFiles(archiveDir);
            
            var fileName = jsonLogger.FileName.Render(new LogEventInfo());
            if (File.Exists(fileName))
            {
                allFiles = allFiles.Append(fileName).ToArray();
            }


            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var logFile in allFiles)
                    {
                        var archiveEntry = archive.CreateEntry(Path.GetFileName(logFile));
                        using (var entryStream = archiveEntry.Open())
                        using (var file = File.Open(logFile, FileMode.Open))
                        {
                            file.CopyTo(entryStream);
                        }
                    }

                }
                compressedBytes = memoryStream.ToArray();
            }
            
            return new FileContentResult(compressedBytes, "application/zip");
        }

        public static IEnumerable<VmApiJobInfoSummary> GetJobsSummary(string jobType = null, bool archive = false)
        {
            var logs = archive ? 
                  GetAllJobsArchiveLogInternal()
                    .Where(log => !log.OperationId.IsNullOrEmpty() && (jobType == null || string.Equals(log.JobType, jobType, StringComparison.CurrentCultureIgnoreCase)))
                    .GroupBy(log => log.OperationId, log => log)
                : GetAllJobsLogInternal()
                .Where(log => !log.OperationId.IsNullOrEmpty() && (jobType == null || string.Equals(log.JobType, jobType, StringComparison.CurrentCultureIgnoreCase)))
                .GroupBy(log => log.OperationId, log => log);
            
            var result = new List<VmApiJobInfoSummary>();
            foreach (var operation in logs)
            {

                var jobLog = new VmApiJobInfoSummary{OperationId = operation.Key, Logs = new List<VmApiJobInfoSummaryLog>()};
                result.Add(jobLog);

                var startLog = operation.SingleOrDefault(l => l.JobStatus == JobResultStatusEnum.Started.ToString());
                if (startLog == null)
                {
                    jobLog.Message = "Start log was not found for the operationId";
                }
                else
                {
                    jobLog.JobType = startLog.JobType;
                    jobLog.StartedTimeUtc = startLog.Time;
                    jobLog.ExecutionType = startLog.ExecutionType;
                }

                foreach (var log in operation.Where(l => l.JobStatus != JobResultStatusEnum.Started.ToString())
                    .OrderByDescending(log => log.Time)
                    .ThenByDescending(log => log.JobStatus?.ToString()))
                {
                    jobLog.Logs.Add(ToVmApiJobInfoSummaryLog(log));
                }
                
                // add starting log into last position (if exists)
                if (startLog != null)
                {
                    jobLog.Logs.Add(ToVmApiJobInfoSummaryLog(startLog));
                }

                if (startLog != null && jobLog.Logs.Any() && 
                    DateTime.TryParse(jobLog.Logs.First().TimeUtc, out var lastLog) && 
                    DateTime.TryParse(jobLog.Logs.Last().TimeUtc, out var firstLog))
                {
                    jobLog.ExecutionTime = lastLog - firstLog;
                }
            }
            return result.OrderByDescending(log => log.StartedTimeUtc);
        }

        private static VmApiJobInfoSummaryLog ToVmApiJobInfoSummaryLog(VmJsonJobLog log)
        {
            return new VmApiJobInfoSummaryLog
            {
                Level = log.Level,
                JobStatus = log.JobStatus,
                TimeUtc = log.Time,
                Message = log.Message,
                Exception = log.Exception
            };
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
            if (!(LogManager.Configuration.FindTargetByName(JSON_JOB_LOGGER) is FileTarget jsonLogger)) return;

            var fileName = jsonLogger.FileName.Render(new LogEventInfo());
            if (!File.Exists(fileName)) return;

            File.Delete(fileName);
        }

        /// <summary>
        /// Log job info
        /// </summary>
        /// <param name="operationId"></param>
        /// <param name="jobKey"></param>
        /// <param name="executionType"></param>
        /// <param name="resultStatus"></param>
        /// <param name="message"></param>
        public static void LogJobInfo(string operationId, string jobKey, JobExecutionTypeEnum executionType, JobResultStatusEnum resultStatus, string message)
        {
            LogJobEvent(operationId, LogLevel.Info, jobKey, executionType, resultStatus, message);
        }

        /// <summary>
        /// Log job error
        /// </summary>
        /// <param name="operationId"></param>
        /// <param name="jobKey"></param>
        /// <param name="executionType"></param>
        /// <param name="resultStatus"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void LogJobError(string operationId, string jobKey, JobExecutionTypeEnum executionType, JobResultStatusEnum resultStatus, string message, Exception exception)
        {
            LogJobEvent(operationId, LogLevel.Error, jobKey, executionType, resultStatus, message, exception);
        }

        /// <summary>
        /// Log job event
        /// </summary>
        /// <param name="operationId"></param>
        /// <param name="logLevel"></param>
        /// <param name="jobKey"></param>
        /// <param name="executionType"></param>
        /// <param name="resultStatus"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        private static void LogJobEvent(string operationId, LogLevel logLevel, string jobKey, JobExecutionTypeEnum? executionType, JobResultStatusEnum? resultStatus, string message, Exception exception = null)
        {
            JobLogger.LogSchedulerEvent(logLevel, new VmJobLogEntry
            {
                JobStatus = resultStatus?.ToString(),
                JobType = jobKey,
                ExecutionType = executionType?.ToString(),
                OperationId = operationId
            }, message, exception);
        }

        public static void LogJobWarn(string operationId, string jobKey, string message)
        {
            LogJobEvent(operationId, LogLevel.Warn, jobKey, null, null, message);
        }
        
        public static void LogJobError(string operationId, string jobKey, string message, Exception exception = null)
        {
            LogJobEvent(operationId, LogLevel.Error, jobKey, null, null, message, exception);
        }

        public static void LogJobInfo(string operationId, string jobKey, string message)
        {
            LogJobEvent(operationId, LogLevel.Info, jobKey, null, null, message);
        }

    }
}
