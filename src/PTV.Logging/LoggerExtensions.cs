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
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Logging
{
//    /// <summary>
//    /// Contains ILogger extension methods.
//    /// </summary>
//    public static class LoggerExtensions
//    {
//        // NOTE: extension methods use WriteXXX to not to conflict with LogXXX (like LogDebug, LogTrace, etc) extension methods from Microsoft namespace
//
//        private const string EventIdKey = "EventId";
//        private const string EventIdPropIdKey = "EventId.Id";
//        private const string EventIdPropNameKey = "EventId.Name";
//
//        /// <summary>
//        /// Gets the logger name (namespace, like PTV.Something.Controllers.SomeController) based on the generic ILogger{LoggerType} or non generic logger type name.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <returns>logger name</returns>
//        /// <exception cref="System.ArgumentNullException"><i>logger</i> is null reference</exception>
//        public static string GetLoggerName(this ILogger logger)
//        {
//            if (logger == null)
//            {
//                throw new ArgumentNullException(nameof(logger));
//            }
//
//            var genericArgs = logger.GetType().GenericTypeArguments;
//            return genericArgs.Any() ? genericArgs[0].FullName : logger.GetType().FullName;
//        }
//
//        /// <summary>
//        /// Writes successful login log entry.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <param name="message">log entry message</param>
//        public static void LogLogin(this ILogger logger, string message)
//        {
//            if (logger == null)
//            {
//                throw new ArgumentNullException(nameof(logger));
//            }
//
//            logger.LogInformation(LogEvents.Login, message);
//        }
//
//        /// <summary>
//        /// Writes failed login attempt.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <param name="message">log entry message</param>
//        public static void LogFailedLogin(this ILogger logger, string message)
//        {
//            if (logger == null)
//            {
//                throw new ArgumentNullException(nameof(logger));
//            }
//
//            // use warning as this might indicate someone trying to guess passwords or randomly test login credentials
//            logger.LogWarning(LogEvents.FailedLogin, message);
//        }
//
//        /// <summary>
//        /// Writes failed login attempt.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <param name="message">log entry message</param>
//        public static void LogLogout(this ILogger logger, string message)
//        {
//            if (logger == null)
//            {
//                throw new ArgumentNullException(nameof(logger));
//            }
//
//            logger.LogInformation(LogEvents.Logout, message);
//        }
//
//        // commented out for now, not needed unless additional information needs to be writen to the logs
//        // also the method names could be changed to LogXXX and have only one implementation so they don't conflict with existing logging extensions
//
//        /*
//
//        /// <summary>
//        /// Write trace log entry.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <param name="message">log entry message</param>
//        /// <param name="ex">optional related exception</param>
//        /// <param name="additionalInformation">optional IDictionary of additional information. Key value pairs are writen as structural data for the log entry (NLog LogEventInfo properties).</param>
//        public static void WriteTrace(this ILogger logger, string message, Exception ex = null, IDictionary<string, object> additionalInformation = null)
//        {
//            logger.WriteToLog(LogLevel.Trace, LogEvents.Default, message, ex, additionalInformation);
//        }
//
//        /// <summary>
//        /// Write trace log entry.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <param name="eventId">event id for the log entry, <see cref="Microsoft.Extensions.Logging.EventId"/></param>
//        /// <param name="message">log entry message</param>
//        /// <param name="ex">optional related exception</param>
//        /// <param name="additionalInformation">optional IDictionary of additional information. Key value pairs are writen as structural data for the log entry (NLog LogEventInfo properties).</param>
//        public static void WriteTrace(this ILogger logger, EventId eventId, string message, Exception ex = null, IDictionary<string, object> additionalInformation = null)
//        {
//            logger.WriteToLog(LogLevel.Trace, eventId, message, ex, additionalInformation);
//        }
//
//        /// <summary>
//        /// Write debug log entry.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <param name="message">log entry message</param>
//        /// <param name="ex">optional related exception</param>
//        /// <param name="additionalInformation">optional IDictionary of additional information. Key value pairs are writen as structural data for the log entry (NLog LogEventInfo properties).</param>
//        public static void WriteDebug(this ILogger logger, string message, Exception ex = null, IDictionary<string, object> additionalInformation = null)
//        {
//            logger.WriteToLog(LogLevel.Debug, LogEvents.Default, message, ex, additionalInformation);
//        }
//
//        /// <summary>
//        /// Write debug log entry.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <param name="eventId">event id for the log entry, <see cref="Microsoft.Extensions.Logging.EventId"/></param>
//        /// <param name="message">log entry message</param>
//        /// <param name="ex">optional related exception</param>
//        /// <param name="additionalInformation">optional IDictionary of additional information. Key value pairs are writen as structural data for the log entry (NLog LogEventInfo properties).</param>
//        public static void WriteDebug(this ILogger logger, EventId eventId, string message, Exception ex = null, IDictionary<string, object> additionalInformation = null)
//        {
//            logger.WriteToLog(LogLevel.Debug, eventId, message, ex, additionalInformation);
//        }
//
//        /// <summary>
//        /// Write information log entry.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <param name="message">log entry message</param>
//        /// <param name="ex">optional related exception</param>
//        /// <param name="additionalInformation">optional IDictionary of additional information. Key value pairs are writen as structural data for the log entry (NLog LogEventInfo properties).</param>
//        public static void WriteInformation(this ILogger logger, string message, Exception ex = null, IDictionary<string, object> additionalInformation = null)
//        {
//            logger.WriteToLog(LogLevel.Information, LogEvents.Default, message, ex, additionalInformation);
//        }
//
//        /// <summary>
//        /// Write information log entry.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <param name="eventId">event id for the log entry, <see cref="Microsoft.Extensions.Logging.EventId"/></param>
//        /// <param name="message">log entry message</param>
//        /// <param name="ex">optional related exception</param>
//        /// <param name="additionalInformation">optional IDictionary of additional information. Key value pairs are writen as structural data for the log entry (NLog LogEventInfo properties).</param>
//        public static void WriteInformation(this ILogger logger, EventId eventId, string message, Exception ex = null, IDictionary<string, object> additionalInformation = null)
//        {
//            logger.WriteToLog(LogLevel.Information, eventId, message, ex, additionalInformation);
//        }
//
//        /// <summary>
//        /// Write warning log entry.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <param name="message">log entry message</param>
//        /// <param name="ex">optional related exception</param>
//        /// <param name="additionalInformation">optional IDictionary of additional information. Key value pairs are writen as structural data for the log entry (NLog LogEventInfo properties).</param>
//        public static void WriteWarning(this ILogger logger, string message, Exception ex = null, IDictionary<string, object> additionalInformation = null)
//        {
//            logger.WriteToLog(LogLevel.Warning, LogEvents.Default, message, ex, additionalInformation);
//        }
//
//        /// <summary>
//        /// Write warning log entry.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <param name="eventId">event id for the log entry, <see cref="Microsoft.Extensions.Logging.EventId"/></param>
//        /// <param name="message">log entry message</param>
//        /// <param name="ex">optional related exception</param>
//        /// <param name="additionalInformation">optional IDictionary of additional information. Key value pairs are writen as structural data for the log entry (NLog LogEventInfo properties).</param>
//        public static void WriteWarning(this ILogger logger, EventId eventId, string message, Exception ex = null, IDictionary<string, object> additionalInformation = null)
//        {
//            logger.WriteToLog(LogLevel.Warning, eventId, message, ex, additionalInformation);
//        }
//
//        /// <summary>
//        /// Write error log entry.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <param name="message">log entry message</param>
//        /// <param name="ex">optional related exception</param>
//        /// <param name="additionalInformation">optional IDictionary of additional information. Key value pairs are writen as structural data for the log entry (NLog LogEventInfo properties).</param>
//        public static void WriteError(this ILogger logger, string message, Exception ex = null, IDictionary<string, object> additionalInformation = null)
//        {
//            logger.WriteToLog(LogLevel.Error, LogEvents.Default, message, ex, additionalInformation);
//        }
//
//        /// <summary>
//        /// Write error log entry.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <param name="eventId">event id for the log entry, <see cref="Microsoft.Extensions.Logging.EventId"/></param>
//        /// <param name="message">log entry message</param>
//        /// <param name="ex">optional related exception</param>
//        /// <param name="additionalInformation">optional IDictionary of additional information. Key value pairs are writen as structural data for the log entry (NLog LogEventInfo properties).</param>
//        public static void WriteError(this ILogger logger, EventId eventId, string message, Exception ex = null, IDictionary<string, object> additionalInformation = null)
//        {
//            logger.WriteToLog(LogLevel.Error, eventId, message, ex, additionalInformation);
//        }
//
//        /// <summary>
//        /// Write critical log entry.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <param name="message">log entry message</param>
//        /// <param name="ex">optional related exception</param>
//        /// <param name="additionalInformation">optional IDictionary of additional information. Key value pairs are writen as structural data for the log entry (NLog LogEventInfo properties).</param>
//        public static void WriteCritical(this ILogger logger, string message, Exception ex = null, IDictionary<string, object> additionalInformation = null)
//        {
//            logger.WriteToLog(LogLevel.Critical, LogEvents.Default, message, ex, additionalInformation);
//        }
//
//        /// <summary>
//        /// Write critical log entry.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <param name="eventId">event id for the log entry, <see cref="Microsoft.Extensions.Logging.EventId"/></param>
//        /// <param name="message">log entry message</param>
//        /// <param name="ex">optional related exception</param>
//        /// <param name="additionalInformation">optional IDictionary of additional information. Key value pairs are writen as structural data for the log entry (NLog LogEventInfo properties).</param>
//        public static void WriteCritical(this ILogger logger, EventId eventId, string message, Exception ex = null, IDictionary<string, object> additionalInformation = null)
//        {
//            logger.WriteToLog(LogLevel.Critical, eventId, message, ex, additionalInformation);
//        }
//
//        */
//
//        /// <summary>
//        /// Writes log entry using NLog logger.
//        /// </summary>
//        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
//        /// <param name="logLevel">log entry level, <see cref="Microsoft.Extensions.Logging.LogLevel"/></param>
//        /// <param name="eventId">event id for the log entry, <see cref="Microsoft.Extensions.Logging.EventId"/></param>
//        /// <param name="message">log entry message</param>
//        /// <param name="ex">optional related exception</param>
//        /// <param name="additionalInformation">optional IDictionary of additional information. Key value pairs are writen as structural data for the log entry (NLog LogEventInfo properties).</param>
//        public static void WriteToLog(this ILogger logger, LogLevel logLevel, EventId eventId, string message, Exception ex = null, IDictionary<string, object> additionalInformation = null)
//        {
//            if (logger == null)
//            {
//                throw new ArgumentNullException(nameof(logger));
//            }
//
//            // get NLog specific logger
//            var nLogger = NLog.LogManager.GetLogger(logger.GetLoggerName());
//            var nLogLevel = ConvertToNLogLevel(logLevel);
//
//            var eventInfo = NLog.LogEventInfo.Create(nLogLevel, nLogger.Name, message);
//            eventInfo.Exception = ex;
//
//            // adding the EventId the same way as NLogLogger implementation
//            eventInfo.Properties.Add(EventIdPropIdKey, eventId.Id);
//            eventInfo.Properties.Add(EventIdPropNameKey, eventId.Name);
//            eventInfo.Properties.Add(EventIdKey, eventId);
//
//            if (additionalInformation != null && additionalInformation.Count > 0)
//            {
//                // try to remove eventid keys, the remove method doesn't throw if the keys don't exist
//                additionalInformation.Remove(EventIdKey);
//                additionalInformation.Remove(EventIdPropIdKey);
//                additionalInformation.Remove(EventIdPropNameKey);
//
//                foreach (var kvp in additionalInformation)
//                {
//                    eventInfo.Properties.Add(kvp.Key, kvp.Value);
//                }
//            }
//
//            nLogger.Log(eventInfo);
//        }
//
//        /// <summary>
//        /// Convert
//        /// </summary>
//        /// <param name="logLevel"></param>
//        /// <returns></returns>
//        private static NLog.LogLevel ConvertToNLogLevel(LogLevel logLevel)
//        {
//            // from: NLog.Extensions.Logging.NLogLogger
//
//            switch (logLevel)
//            {
//                case LogLevel.Trace:
//                    return NLog.LogLevel.Trace;
//                case LogLevel.Debug:
//                    return NLog.LogLevel.Debug;
//                case LogLevel.Information:
//                    return NLog.LogLevel.Info;
//                case LogLevel.Warning:
//                    return NLog.LogLevel.Warn;
//                case LogLevel.Error:
//                    return NLog.LogLevel.Error;
//                case LogLevel.Critical:
//                    return NLog.LogLevel.Fatal;
//                case LogLevel.None:
//                    return NLog.LogLevel.Off;
//                default:
//                    return NLog.LogLevel.Debug;
//            }
//        }
//    }
}
