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
using PTV.Framework.Interfaces;
using PTV.TaskScheduler.Enums;

namespace PTV.TaskScheduler.Models
{
    /// <summary>
    /// JobInfo model for WebAPI
    /// </summary>
    public class VmApiJobInfo : IVmBase
    {

        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Cron expression
        /// </summary>
        public string CronExpression { get; set; }

        /// <summary>
        /// Job state
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Last fire time
        /// </summary>
        public DateTimeOffset? LastFireTimeUtc { get; set; }

        /// <summary>
        /// Next fire time
        /// </summary>
        public DateTimeOffset? NextFireTimeUtc { get; set; }

        /// <summary>
        /// Next fire time
        /// </summary>
        public DateTimeOffset? NextFireTimeLocal { get; set; }

        /// <summary>
        /// Regular scheduling cron expression
        /// </summary>
        public string RegularScheduling { get; set; }

        /// <summary>
        /// Exception scheduling cron expression
        /// </summary>
        public string ExceptionScheduling { get; set; }

        /// <summary>
        /// Count of re-fire attempts, when job fails
        /// </summary>
        public int RetriesOnFails { get; set; }

        /// <summary>
        /// Count of failed job executions
        /// </summary>
        public int CountOfFailedExecutions { get; set; }

        /// <summary>
        /// Is running
        /// </summary>
        public bool IsRunning { get; set; }
        /// <summary>
        /// Last log file content
        /// </summary>
        public IEnumerable<VmApiJobInfoSummary> Summary { get; set; }
        /// <summary>
        /// Archive logs files content
        /// </summary>
        public IEnumerable<VmApiJobInfoSummary> ArchiveSummary { get; set; }
        /// <summary>
        /// Last job status
        /// </summary>
        public string LastJobStatus { get; set; }
        /// <summary>
        /// ExecutionTime
        /// </summary>
        public long LastExecutionTime { get; set; }
    }
}
