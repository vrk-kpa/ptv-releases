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

namespace PTV.IdentityUserManager
{
    /// <summary>
    /// Contains STS logger extension methods.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Log created user.
        /// </summary>
        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
        /// <param name="message">log entry message</param>
        /// <exception cref="System.ArgumentNullException"><i>logger</i> is null reference</exception>
        public static void LogCreatedUser(this ILogger logger, string message)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.LogInformation(AuthenticationServerLogEvents.CreateNewUser, message);
        }

        /// <summary>
        /// Log create user not allowed.
        /// </summary>
        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
        /// <param name="message">log entry message</param>
        /// <exception cref="System.ArgumentNullException"><i>logger</i> is null reference</exception>
        public static void LogCreateUserNotAllowed(this ILogger logger, string message)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.LogWarning(AuthenticationServerLogEvents.CreateNewUser, message);
        }

        /// <summary>
        /// Log failed to create user.
        /// </summary>
        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
        /// <param name="message">log entry message</param>
        /// <exception cref="System.ArgumentNullException"><i>logger</i> is null reference</exception>
        public static void LogFailedToCreateUser(this ILogger logger, string message)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.LogError(AuthenticationServerLogEvents.CreateNewUser, message);
        }

        /// <summary>
        /// Log reset password for a user.
        /// </summary>
        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
        /// <param name="message">log entry message</param>
        /// <exception cref="System.ArgumentNullException"><i>logger</i> is null reference</exception>
        public static void LogResetPasswordForUser(this ILogger logger, string message)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.LogInformation(AuthenticationServerLogEvents.ChangePasswordForUser, message);
        }

        /// <summary>
        /// Log chnage user information.
        /// </summary>
        /// <param name="logger"><see cref="Microsoft.Extensions.Logging.ILogger"/></param>
        /// <param name="message">log entry message</param>
        /// <exception cref="System.ArgumentNullException"><i>logger</i> is null reference</exception>
        public static void LogChangeUserInfo(this ILogger logger, string message)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.LogInformation(AuthenticationServerLogEvents.ChangeUserInfo, message);
        }
    }
}
