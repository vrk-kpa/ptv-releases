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
using Microsoft.AspNetCore.Http;
using PTV.Framework.Enums;
using PTV.Framework.Extensions;

namespace PTV.Framework.ServiceManager
{
    /// <summary>
    /// Handles service calls, catching exceptions and wrapping results into generic envelope
    /// </summary>
    [RegisterService(typeof(IServiceManager), RegisterType.Transient)]
    public class ServiceManager : IServiceManager
    {
        private readonly ILogger logger;
        private readonly ApplicationConfiguration configuration;
        private readonly IHttpContextAccessor ctxAccessor;
        public ServiceManager(ILoggerFactory loggerFactory, ApplicationConfiguration configuration, IHttpContextAccessor ctxAccessor)
        {
            logger = loggerFactory.CreateLogger<ServiceManager>();
            this.configuration = configuration;
            this.ctxAccessor = ctxAccessor;
        }

        /// <summary>
        /// Call action, i.e. service, and wrap the exceptions and result into generic result envelope
        /// </summary>
        /// <param name="serviceCall"></param>
        /// <param name="resultMessages"></param>
        /// <returns></returns>
        public IServiceResultWrap CallService(Func<IServiceResultWrap> serviceCall, Dictionary<Type, string> resultMessages)
        {
            IServiceResultWrap result = null;
            try
            {
                result = serviceCall();
                if (resultMessages.ContainsKey(typeof(string)))
                {
                    result.Messages.Infos.Add(resultMessages[typeof(string)]);
                }
            }
            catch (PtvArgumentException arEx)
            {
                result = new ServiceResultWrap();
                var error = resultMessages.ContainsKey(arEx.GetType()) ? resultMessages[arEx.GetType()] : arEx.Message;
                result.Messages.Errors.Add(error + ";" + arEx.ParamName);
                var wrappedExcps = WrapAllExceptions(arEx);
                result.Messages.StackTrace.Add(string.Empty);
                logger.LogError("Service call error : " + wrappedExcps);
            }
            catch (PtvAppException ptvEx)
            {
                result = new ServiceResultWrap() { Data = ptvEx.AdditionalData };

                var error = string.IsNullOrEmpty(ptvEx.CodeIdentifier) ?
                        resultMessages.ContainsKey(ptvEx.GetType()) ? resultMessages[ptvEx.GetType()] : ptvEx.Message :
                        ptvEx.CodeIdentifier;

                StringBuilder errorBuilder = new StringBuilder(error + ";");
                ptvEx.AdditionalParams.ForEach(i => errorBuilder.Append(i + ";"));
                result.Messages.Errors.Add(errorBuilder.ToString());

                result.Messages.StackTrace.Add(string.Empty);
                logger.LogDebug(WrapAllExceptions(ptvEx));
            }
            catch (Exception ex)
            {
                var wrappedExcps = WrapAllExceptions(ex);
                logger.LogError("Service call error : " + wrappedExcps);
                var eType = configuration.GetEnvironmentType();
                if ((eType & (EnvironmentTypeEnum.Dev | EnvironmentTypeEnum.Qa | EnvironmentTypeEnum.Test)) == 0)
                {
                    ctxAccessor.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    return result;
                }
                result = new ServiceResultWrap();
                var error = resultMessages.ContainsKey(ex.GetType()) ? resultMessages[ex.GetType()] : ex.Message;
                result.Messages.Errors.Add(error);
                result.Messages.StackTrace.Add(wrappedExcps);
            }
            return result;
        }
        private string WrapAllExceptions(Exception exception)
        {
            return WrapAllExceptionsMessage(exception) + Environment.NewLine + WrapAllExceptionsStackTrace(exception);
        }
        private string WrapAllExceptionsStackTrace(Exception exception)
        {
            var result = exception.GetType().Name + Environment.NewLine + exception.StackTrace;
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
