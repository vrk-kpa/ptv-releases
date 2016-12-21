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
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTV.Framework.ServiceManager
{
    /// <summary>
    /// Handles service calls, catching exceptions and wrapping results into generic envelope
    /// </summary>
    [RegisterService(typeof(IServiceManager), RegisterType.Transient)]
    public class ServiceManager : IServiceManager
    {
        private readonly ILogger logger;
        public ServiceManager(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<ServiceManager>();
        }

        /// <summary>
        /// Call action, i.e. service, and wrap the exceptions and result into generic result envelope
        /// </summary>
        /// <param name="serviceCall"></param>
        /// <param name="resultMessages"></param>
        /// <returns></returns>
        public IServiceResultWrap CallService(Func<IServiceResultWrap> serviceCall, Dictionary<Type, string> resultMessages)
        {
            IServiceResultWrap result;
            try
            {
                result = serviceCall();
                if (resultMessages.ContainsKey(typeof(string)))
                {
                    result.Messages.Infos.Add(resultMessages[typeof(string)]);
                }
            }
            catch (PtvAppException ptvEx)
            {
                result = new ServiceResultWrap() { Data = ptvEx.AdditionalData };
                if (string.IsNullOrEmpty(ptvEx.CodeIdentifier))
                {
                    result.Messages.Errors.Add(ptvEx.Message);
                }
                else
                {
                    StringBuilder errorBuilder = new StringBuilder(ptvEx.CodeIdentifier + ";");
                    ptvEx.AdditionalParams.ForEach(i => errorBuilder.Append(i + ";"));
                    result.Messages.Errors.Add(errorBuilder.ToString());
                }
                result.Messages.StackTrace.Add(string.Empty);
                logger.LogDebug(WrapAllExceptions(ptvEx));
            }
            catch (ArgumentException arEx)
            {
                result = new ServiceResultWrap();
                var error = resultMessages.ContainsKey(arEx.GetType()) ? resultMessages[arEx.GetType()] : arEx.Message;
                result.Messages.Errors.Add(error+";"+arEx.ParamName);
                var wrappedExcps = WrapAllExceptions(arEx);
                result.Messages.StackTrace.Add(wrappedExcps);
                logger.LogError("Service call error : " + wrappedExcps);
            }
            catch (Exception ex)
            {
                result = new ServiceResultWrap();
                var error = resultMessages.ContainsKey(ex.GetType()) ? resultMessages[ex.GetType()] : ex.Message;
                result.Messages.Errors.Add(error);
                var wrappedExcps = WrapAllExceptions(ex);
                result.Messages.StackTrace.Add(wrappedExcps);
                logger.LogError("Service call error : " + wrappedExcps);
            }
            return result;
        }
        private string WrapAllExceptions(Exception exception)
        {
            return WrapAllExceptionsMessage(exception) + Environment.NewLine + WrapAllExceptionsStackTrace(exception);
        }
        private string WrapAllExceptionsStackTrace(Exception exception)
        {
            var result = exception.StackTrace;
            if (exception.InnerException != null)
            {
                result = WrapAllExceptionsStackTrace(exception.InnerException) +
                    Environment.NewLine +
                    " --- End of inner exception stack trace --- " +
                    Environment.NewLine +
                    result;
            }
            return result;
        }
        private string WrapAllExceptionsMessage(Exception exception)
        {
            var result = exception.Message;
            if (exception.InnerException != null)
            {
                result = result + " --> " + WrapAllExceptionsMessage(exception.InnerException);
            }
            return result;
        }
    }
}
