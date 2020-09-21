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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Models.Converters;

namespace PTV.Domain.Model.Models.Jobs
{
    /// <summary>
    /// 
    /// </summary>
    public class VmJobLog
    {
        /// <summary>
        /// 
        /// </summary>
        public VmJobLog()
        {
         TimeUtc = DateTime.UtcNow;   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="jobStatus"></param>
        /// <param name="exception"></param>
        public VmJobLog(string message, JobResultStatusEnum? jobStatus = null, Exception exception = null)
            :this()
        {
            Message = message;
            JobStatus = jobStatus;
            Exception = exception == null 
                ? null 
                : new VmJobException(exception);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public Guid? Id { get; set; }
        
        /// <summary>
        /// Time
        /// </summary>
        public DateTime TimeUtc { get; set; }

        /// <summary>
        /// Level
        /// </summary>
        [JsonConverter(typeof(NLogLevelConverter))]
        public NLog.LogLevel Level { get; set; }

        /// <summary>
        /// Job result status
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public JobResultStatusEnum? JobStatus { get; set; }

        /// <summary>
        /// Log message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Exception
        /// </summary>
        public VmJobException Exception { get; set; }
    }
}
