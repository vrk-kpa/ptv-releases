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
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Targets;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Jobs;
using PTV.Framework;
using PTV.Framework.Logging;
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
        /// Log job event
        /// </summary>
        private static void LogJobEvent(VmJobSummary summary, VmJobLog log, IJobStatusService jobStatusService)
        {
            jobStatusService.Save(summary, log, true);
            var exception = log?.Exception == null ? null : new Exception(log.Exception.Message);
            JobLogger.LogSchedulerEvent(log.Level, new VmJobLogEntry
            {
                JobStatus = log.JobStatus.ToString(),
                JobType = summary.JobType,
                ExecutionType = summary.ExecutionType.ToString(),
                OperationId = summary.OperationId
            }, log.Message, exception);
        }

        public static void LogJobWarn(VmJobSummary summary, string message, IJobStatusService jobStatusService)
        {
            var log = new VmJobLog(message);
            LogJobWarn(summary, log, jobStatusService);
        }

        public static void LogJobWarn(VmJobSummary summary, VmJobLog log, IJobStatusService jobStatusService)
        {
            log.Level = LogLevel.Warn;
            LogJobEvent(summary, log, jobStatusService);
        }

        public static void LogJobError(VmJobSummary summary, string message, IJobStatusService jobStatusService)
        {
            var log = new VmJobLog(message);
            LogJobError(summary, log, jobStatusService);
        }

        public static void LogJobError(VmJobSummary summary, VmJobLog log, IJobStatusService jobStatusService)
        {
            log.Level = LogLevel.Error;
            LogJobEvent(summary, log, jobStatusService);
        }

        public static void LogJobInfo(VmJobSummary summary, string message, IJobStatusService jobStatusService)
        {
            var log = new VmJobLog(message);
            LogJobInfo(summary, log, jobStatusService);
        }

        public static void LogJobInfo(VmJobSummary summary, VmJobLog log, IJobStatusService jobStatusService)
        {
            log.Level = LogLevel.Info;
            LogJobEvent(summary, log, jobStatusService);
            if (Debugger.IsAttached)
            {
                Console.WriteLine(log.Message);
            }
        }
    }
}
