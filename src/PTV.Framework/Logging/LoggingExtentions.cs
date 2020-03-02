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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using ILogger = NLog.ILogger;
using LogLevel = NLog.LogLevel;

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
                var eventInfo = LogEventInfo.Create(LogLevel.Info, loggerName, string.Empty);
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
            var loggerName = GetLoggerName(logger);
            var nLogger = LogManager.GetLogger(loggerName);
            var eventInfo = LogEventInfo.Create(LogLevel.Info, loggerName, string.Empty);
            eventInfo.Properties["Method"] = entry.Method;
            eventInfo.Properties["Url"] = $"\"{entry.Scheme}://{entry.Host}{entry.Action}{entry.QueryString}\"";
            eventInfo.Properties["User"] = entry.UserName;
            nLogger.Log(eventInfo);
        }

        /// <summary>
        /// <para>
        /// Calls ConfigureNLog with environment specific NLog configuration file name. Environment specific file name
        ///  is determined from IWebHostEnvironment.EnvironmentName using template nlog.{env-name}.config (For example: nlog.Development.config).
        /// </para>
        /// <para>
        /// If environment specific file doesn't exists then default nlog.config file is used.
        /// </para>
        /// </summary>
        /// <param name="hostingEnvironment">IWebHostEnvironment</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="hostingEnvironment"/> is null reference</exception>
        /// <example>
        /// <para>
        /// For example if the IWebHostEnvironment.EnvironmentName returns: Development
        /// </para>
        /// <para>
        /// The the environment specific filename is nlog.Development.config and the code checks if the file exists. If the file exists then that filename
        ///  is passed to NLog as the configuration file otherwise nlog.config is passed as the name of the configuration file.
        /// </para>
        /// </example>
        public static void ConfigureNLogForEnvironment(this IWebHostEnvironment hostingEnvironment)
        {
            if (hostingEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostingEnvironment));
            }

            string envConfigFilename = $"nlog.{hostingEnvironment.EnvironmentName}.config";
            NLogBuilder.ConfigureNLog(File.Exists(envConfigFilename) ? envConfigFilename : "nlog.config");
        }

        #region TaskScheduler logger

        public static void LogSchedulerError(this Microsoft.Extensions.Logging.ILogger logger, VmJobLogEntry logInfo, string message, Exception exception = null)
        {
            LogSchedulerEvent(logger, LogLevel.Error, logInfo, message, exception);
        }

        public static void LogSchedulerWarn(this Microsoft.Extensions.Logging.ILogger logger, VmJobLogEntry logInfo, string message)
        {
            LogSchedulerEvent(logger, LogLevel.Warn, logInfo, message, null);
        }

        public static void LogSchedulerInfo(this Microsoft.Extensions.Logging.ILogger logger, VmJobLogEntry logInfo, string message)
        {
            LogSchedulerEvent(logger, LogLevel.Info, logInfo, message, null);
        }

        private static void LogSchedulerEvent(Microsoft.Extensions.Logging.ILogger logger, LogLevel logLevel, VmJobLogEntry logInfo, string message, Exception exception)
        {
            var loggerName = GetLoggerName(logger, LoggerNameType.ShortName);
            var nLogger = LogManager.GetLogger(loggerName) as ILogger;
            nLogger.LogSchedulerEvent(logLevel, logInfo, message, exception);
        }

        public static void LogSchedulerEvent(this ILogger logger, LogLevel logLevel, VmJobLogEntry jobLogEntry, string message, Exception exception = null)
        {
            var eventInfo = LogEventInfo.Create(logLevel, logger.Name, message);
            eventInfo.Properties["JobStatus"] = jobLogEntry?.JobStatus;
            eventInfo.Properties["JobType"] = jobLogEntry?.JobType;
            eventInfo.Properties["ExecutionType"] = jobLogEntry?.ExecutionType;
            eventInfo.Properties["OperationId"] = jobLogEntry?.OperationId;
            if (exception != null) eventInfo.Exception = exception;
            //if (exception != null) eventInfo.StackTrace = exception.StackTrace;
            logger.Log(eventInfo);
        }

        #endregion TaskScheduler logger

        public static void LogDatabaseException(this Microsoft.Extensions.Logging.ILogger logger, Exception exception, LogLevel logLevel = null)
        {
            var loggerName = GetLoggerName(logger, LoggerNameType.ShortName);
            var nLogger = LogManager.GetLogger(loggerName) as ILogger;
            var eventInfo = LogEventInfo.Create(logLevel ?? LogLevel.Warn, nLogger.Name, exception?.Message);
            eventInfo.Exception = exception;
            nLogger.Log(eventInfo);
        }

        public static void LogDatabaseRetry(this Microsoft.Extensions.Logging.ILogger logger, EventId eventId, Exception exception,
            string customMessage = null)
        {
            var loggerName = GetLoggerName(logger, LoggerNameType.ShortName);
            var nLogger = LogManager.GetLogger(loggerName) as ILogger;
            var eventInfo = LogEventInfo.Create(LogLevel.Warn, nLogger.Name, customMessage ?? exception?.Message);
            eventInfo.Properties["EventId"] = eventId.Id;
            eventInfo.Properties["EventName"] = eventId.Name;
            eventInfo.Exception = exception;
            nLogger.Log(eventInfo);
        }

        #region logger name

        private enum LoggerNameType
        {
            ShortName,
            FullName
        }

        private static string GetLoggerName(Microsoft.Extensions.Logging.ILogger logger, LoggerNameType loggerNameType = LoggerNameType.FullName)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            var genericArgs = logger.GetType().GenericTypeArguments;

            switch (loggerNameType)
            {
                case LoggerNameType.FullName: return genericArgs.Any() ? genericArgs[0].FullName : logger.GetType().FullName;
                case LoggerNameType.ShortName: return genericArgs.Any() ? genericArgs[0].Name : logger.GetType().Name;
                default: throw new InvalidEnumArgumentException(nameof(loggerNameType));
            }
        }

        #endregion logger name
    }
}
