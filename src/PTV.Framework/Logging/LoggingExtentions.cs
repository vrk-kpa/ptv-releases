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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.Framework.Logging
{
    public static class LoggingExtentions
    {
        public static void LogDBEntries(this Microsoft.Extensions.Logging.ILogger logger, IList<VmLogEntry> logEntries)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            var genericArgs = logger.GetType().GenericTypeArguments;
            var loggerName = genericArgs.Any() ? genericArgs[0].FullName : logger.GetType().FullName;
            var nLogger = LogManager.GetLogger(loggerName);
            foreach (var entry in logEntries)
            {
                var eventInfo = LogEventInfo.Create(NLog.LogLevel.Info, loggerName, string.Empty);
                eventInfo.Properties["Operation"] = entry.Operation.ToString().ToUpper();
                eventInfo.Properties["Table"] = entry.Table;
                eventInfo.Properties["RowId"] = entry.Identifier;
                eventInfo.Properties["User"] = entry.UserName;
                nLogger.Log(eventInfo);
            }
        }

        /// <summary>
        /// Extension for log request log entry model
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="entry">Log entry</param>
        public static void LogRequestEntry(this Microsoft.Extensions.Logging.ILogger logger, VmRequestLogEntry entry)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            var genericArgs = logger.GetType().GenericTypeArguments;
            var loggerName = genericArgs.Any() ? genericArgs[0].FullName : logger.GetType().FullName;
            var nLogger = LogManager.GetLogger(loggerName);
            var eventInfo = LogEventInfo.Create(NLog.LogLevel.Info, loggerName, string.Empty);
            eventInfo.Properties["Method"] = entry.Method;
            eventInfo.Properties["Url"] = string.Format("\"{0}://{1}{2}{3}\"",entry.Scheme,entry.Host, entry.Action, entry.QueryString);
            eventInfo.Properties["User"] = entry.UserName;
            nLogger.Log(eventInfo);     
        }
    }
}
