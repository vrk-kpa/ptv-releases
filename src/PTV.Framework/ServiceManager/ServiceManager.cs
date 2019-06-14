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
using Microsoft.AspNetCore.SignalR;
using PTV.Framework.Enums;
using PTV.Framework.Extensions;
using Remotion.Linq.Clauses;

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
        private readonly IResolveManager resolveManager;
        
        public ServiceManager(ILoggerFactory loggerFactory, ApplicationConfiguration configuration, IHttpContextAccessor ctxAccessor, IResolveManager resolveManager)
        {
            this.logger = loggerFactory.CreateLogger<ServiceManager>();
            this.configuration = configuration;
            this.ctxAccessor = ctxAccessor;
            this.resolveManager = resolveManager;
        }

        /// <summary>
        /// Call action, i.e. service, and wrap the exceptions and result into generic result envelope
        /// </summary>
        /// <param name="serviceCall"></param>
        /// <param name="resultMessages"></param>
        /// <param name="clients"></param>
        /// <param name="okMessage"></param>
        /// <param name="serviceCallFinishAction"></param>
        /// <returns></returns>
        public void CallRService
        (
            Func<IResolveManager, IClientProxy, IServiceResultWrap, IMessage> serviceCall,
            Dictionary<Type, string> resultMessages,
            IHubCallerClients clients,
            IMessage okMessage = null,
            Action<IClientProxy> serviceCallFinishAction = null
        )
        {
            clients.Caller.SendAsync("ReceiveMessage", "Ok");
            
            resolveManager.RunInThread(rm =>
            {
                IServiceResultWrap result = new ServiceResultWrap();
                try
                {
                    var message = serviceCall(rm, clients.Caller, result);
                    if (message != null)
                    {
                        result.Messages.Infos.Add(message);
                    }
                    
                    if (okMessage != null)
                    {
                        result.Messages.Infos.Add(okMessage);
                    }
                    else if (resultMessages.ContainsKey(typeof(string)))
                    {
                        result.Messages.Infos.Add(new Message(resultMessages[typeof(string)], null));
                    }
                }
                catch (PtvArgumentException arEx)
                {
                    var error = resultMessages.ContainsKey(arEx.GetType()) ? resultMessages[arEx.GetType()] : arEx.Message;
                    result.Messages.Errors.Add(new Error(error, arEx.ParamName));
                    var wrappedExcps = WrapAllExceptions(arEx);
                    logger.LogError("Service call error : " + wrappedExcps);
                }
                catch (PtvAppException ptvEx)
                {
                    result.Data = ptvEx.AdditionalData;
    
                    var errorCode = string.IsNullOrEmpty(ptvEx.CodeIdentifier)
                        ? resultMessages.ContainsKey(ptvEx.GetType())
                            ? resultMessages[ptvEx.GetType()]
                            : ptvEx.Message
                        : ptvEx.CodeIdentifier;
    
                    if (ptvEx is IError error)
                    {
                        error.Code = errorCode;
                        result.Messages.Errors.Add(error);
                    }
                    else
                    {
                        result.Messages.Errors.Add(new Error(errorCode, ptvEx.AdditionalParams));
                    }
                    logger.LogDebug(WrapAllExceptions(ptvEx));
                }
                catch (ErrorException errorEx)
                {
                    result.Messages.Errors.Add(errorEx);
                    logger.LogDebug(WrapAllExceptions(errorEx));
                }
                catch (Exception ex)
                {
                    var wrappedExcps = WrapAllExceptions(ex);
                    logger.LogError("Service call error : " + wrappedExcps, ex);
                    var eType = configuration.GetEnvironmentType();
                    if ((eType & (EnvironmentTypeEnum.Dev | EnvironmentTypeEnum.Qa | EnvironmentTypeEnum.Test)) == 0)
                    {
                        ctxAccessor.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        // return result;
                    }
                    var error = new Error(
                        resultMessages.ContainsKey(ex.GetType()) ? resultMessages[ex.GetType()] : ex.Message,
                        null,
                        wrappedExcps);
                    result.Messages.Errors.Add(error);
                }
                finally
                {
                    serviceCallFinishAction?.Invoke(clients.Caller);
                    clients.Caller.SendAsync("RServiceResult", result);
                }
            },waitForExit:false);
        }
        
        /// <summary>
        /// Call action, i.e. service, and wrap the exceptions and result into generic result envelope
        /// </summary>
        /// <param name="serviceCall"></param>
        /// <param name="resultMessages"></param>
        /// <returns></returns>
        public IServiceResultWrap CallService(Func<IServiceResultWrap> serviceCall, Dictionary<Type, string> resultMessages, IMessage okMessage = null)
        {
            IServiceResultWrap result = null;
            try
            {
                try
                {
                    result = serviceCall();
                    if (okMessage != null)
                    {
                        result.Messages.Infos.Add(okMessage);
                    }
                    else if (resultMessages.ContainsKey(typeof(string)))
                    {
                        result.Messages.Infos.Add(new Message(resultMessages[typeof(string)], null));
                    }
                }
                catch (AggregateException e)
                {
                    throw e.InnerException;
                }
            }
            catch (PtvActionCancelledException)
            {
                logger.LogInformation("Action cancelled");
                result = new ServiceResultWrap();
            }
            catch (PtvArgumentException arEx)
            {
                result = new ServiceResultWrap();
                var error = resultMessages.ContainsKey(arEx.GetType()) ? resultMessages[arEx.GetType()] : arEx.Message;
                result.Messages.Errors.Add(new Error(error, arEx.ParamName));
                var wrappedExcps = WrapAllExceptions(arEx);
                logger.LogError("Service call error : " + wrappedExcps);
            }
            catch (PtvAppException ptvEx)
            {
                result = new ServiceResultWrap() {Data = ptvEx.AdditionalData};

                var errorCode = string.IsNullOrEmpty(ptvEx.CodeIdentifier)
                    ? resultMessages.ContainsKey(ptvEx.GetType())
                        ? resultMessages[ptvEx.GetType()]
                        : ptvEx.Message
                    : ptvEx.CodeIdentifier;

                if (ptvEx is IError error)
                {
                    error.Code = errorCode;
                    result.Messages.Errors.Add(error);
                }
                else
                {
                    result.Messages.Errors.Add(new Error(errorCode, ptvEx.AdditionalParams));
                }
                logger.LogDebug(WrapAllExceptions(ptvEx));
            }
            catch (ErrorException errorEx)
            {
                result = new ServiceResultWrap();
                result.Messages.Errors.Add(errorEx);
                logger.LogDebug(WrapAllExceptions(errorEx));
            }
            catch (Exception ex)
            {
                var wrappedExcps = WrapAllExceptions(ex);
                logger.LogError("Service call error : " + wrappedExcps, ex);
                var eType = configuration.GetEnvironmentType();
                if ((eType & (EnvironmentTypeEnum.Dev | EnvironmentTypeEnum.Qa | EnvironmentTypeEnum.Test)) == 0)
                {
                    ctxAccessor.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    return result;
                }
                result = new ServiceResultWrap();
                var error = new Error(
                    resultMessages.ContainsKey(ex.GetType()) ? resultMessages[ex.GetType()] : ex.Message,
                    null,
                    wrappedExcps);
                result.Messages.Errors.Add(error);
            }

            return result;
        }
        
        /// <summary>
        /// Call action, i.e. service, and wrap the exceptions and result into generic result envelope
        /// </summary>
        /// <param name="serviceCall"></param>
        /// <param name="resultMessages"></param>
        /// <param name="okMessage"></param>
        /// <returns></returns>
        public async Task<IServiceResultWrap> CallServiceAsync(Func<Task<IServiceResultWrap>> serviceCall, Dictionary<Type, string> resultMessages, IMessage okMessage = null)
        {
            IServiceResultWrap result = null;
            try
            {
                try
                {
                    result = await serviceCall();
                    if (okMessage != null)
                    {
                        result.Messages.Infos.Add(okMessage);
                    }
                    else if (resultMessages.ContainsKey(typeof(string)))
                    {
                        result.Messages.Infos.Add(new Message(resultMessages[typeof(string)], null));
                    }
                }
                catch (AggregateException e)
                {
                    throw e.InnerException;
                }
            }
            catch (PtvActionCancelledException)
            {
                logger.LogInformation("Action cancelled");
                result = new ServiceResultWrap();
            }
            catch (PtvArgumentException arEx)
            {
                result = new ServiceResultWrap();
                var error = resultMessages.ContainsKey(arEx.GetType()) ? resultMessages[arEx.GetType()] : arEx.Message;
                result.Messages.Errors.Add(new Error(error, arEx.ParamName));
                var wrappedExcps = WrapAllExceptions(arEx);
                logger.LogError("Service call error : " + wrappedExcps);
            }
            catch (PtvAppException ptvEx)
            {
                result = new ServiceResultWrap() {Data = ptvEx.AdditionalData};

                var errorCode = string.IsNullOrEmpty(ptvEx.CodeIdentifier)
                    ? resultMessages.ContainsKey(ptvEx.GetType())
                        ? resultMessages[ptvEx.GetType()]
                        : ptvEx.Message
                    : ptvEx.CodeIdentifier;

                if (ptvEx is IError error)
                {
                    error.Code = errorCode;
                    result.Messages.Errors.Add(error);
                }
                else
                {
                    result.Messages.Errors.Add(new Error(errorCode, ptvEx.AdditionalParams));
                }
                logger.LogDebug(WrapAllExceptions(ptvEx));
            }
            catch (ErrorException errorEx)
            {
                result = new ServiceResultWrap();
                result.Messages.Errors.Add(errorEx);
                logger.LogDebug(WrapAllExceptions(errorEx));
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
                var error = new Error(
                    resultMessages.ContainsKey(ex.GetType()) ? resultMessages[ex.GetType()] : ex.Message, 
                    null,
                    wrappedExcps);
                result.Messages.Errors.Add(error);
            }

            return result;
        }

        /// <summary>
        /// Call action, i.e. service, and wrap the exceptions and result into generic result envelope
        /// </summary>
        /// <param name="serviceCall"></param>
        /// <param name="resultMessages"></param>
        /// <param name="okMessage"></param>
        /// <returns></returns>
        public async Task<IServiceResultWrap> CallServiceInSync(Func<IServiceResultWrap> serviceCall, Dictionary<Type, string> resultMessages, IMessage okMessage = null)
        {
            return await CallServiceAsync(async () => await Task.Run(serviceCall), resultMessages, okMessage);
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

    public class Error : Message, IError
    {
        public Error(string code, IEnumerable<string> arguments, string stackTrace = "", string subCode = "") : base(code, arguments, subCode)
        {
            StackTrace = stackTrace;
        }

        public Error(string code, string argument) : base(code, string.IsNullOrEmpty(argument) ? null : new [] { argument })
        {
        }

        public string StackTrace { get; set; }
    }
    
    public class Message : IMessage
    {
        public Message(string code, IEnumerable<string> arguments, string subCode = "")
        {
            Code = code;
            SubCode = subCode; 
            Params = arguments?.ToList();
        }

        public string SubCode { get; set; }

        public string Code { get; set; }
        public IReadOnlyList<string> Params { get; set; }
    }
}
