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

namespace PTV.TaskScheduler.Configuration
{
    /// <inheritdoc />
    /// <summary>
    /// Job scheduling configuration
    /// </summary>
    public class JobSchedulingConfiguration : IEquatable<JobSchedulingConfiguration>
    {

        /// <summary>
        /// Cron expression for regular scheduling
        /// </summary>
        public string Scheduler { get; set; }

        /// <summary>
        /// Count of re-fires on fail
        /// </summary>
        public int RetriesOnFail { get; set; }

        /// <summary>
        /// Cron expression for exceptional scheduling
        /// </summary>
        public string SchedulerOnFail { get; set; }

        /// <summary>
        /// IEquatable implemantation
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(JobSchedulingConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Scheduler, other.Scheduler)
                   && RetriesOnFail == other.RetriesOnFail
                   && string.Equals(SchedulerOnFail, other.SchedulerOnFail);
        }
    }
}
