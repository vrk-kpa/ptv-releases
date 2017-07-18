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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTV.Framework.Interfaces;

namespace PTV.Framework
{
    /// <summary>
    /// Responsible for resolving instances from IoC container
    /// </summary>
    [RegisterService(typeof(IResolveManager), RegisterType.Transient)]
    public class ResolveManager : IResolveManager
    {
        private readonly ILogger<ResolveManager> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly IUserIdentification userIdentification;

        public ResolveManager(IServiceProvider serviceProvider, IUserIdentification userIdentification, ILogger<ResolveManager> logger)
        {
            this.serviceProvider = serviceProvider;
            this.userIdentification = userIdentification;
            this.logger = logger;
        }

        /// <summary>
        /// Resolve (get/instantiate) object of specific type from container
        /// </summary>
        /// <typeparam name="T">Specify type that should be resolved</typeparam>
        /// <param name="isOptional">set true if null should be returned if type is not registered instead of throwing exception</param>
        /// <returns>Resolved instance to object of requested type</returns>
        public T Resolve<T>(bool isOptional = false) where T:class
        {
            return isOptional ? serviceProvider.GetService<T>() : serviceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Resolve (get/instantiate) object of specific type from container
        /// </summary>
        /// <typeparam name="type">Specify type that should be resolved</typeparam>
		/// <param name="isOptional">set true if null should be returned if type is not registered instead of throwing exception</param>
        /// <returns>Resolved instance to object of requested type</returns>
        public object Resolve(Type type, bool isOptional = false)
        {
            return isOptional ? serviceProvider.GetService(type) : serviceProvider.GetRequiredService(type);
        }

        /// <summary>
        /// Resolve and return all registered classes of specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> ResolveAllOfType<T>() where T : class
        {
            return serviceProvider.GetServices<T>();
        }

        /// <summary>
        /// Run action in separated thread and wait for its exit
        /// </summary>
        /// <param name="action">Action that will be called in thread</param>
        public void RunInThread(Action<IResolveManager> action)
        {
            var userName = userIdentification.UserName;
            new Thread(() =>
            {
                try
                {
                    using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    {
                        var resolveManager = serviceScope.ServiceProvider.GetRequiredService<IResolveManager>();
                        var threadUserInterface = resolveManager.Resolve<IUserIdentification>() as IThreadUserInterface;
                        threadUserInterface?.SetUserName(userName);
                        action(resolveManager);
                    }
                }
                catch (Exception e)
                {
                    logger.LogError("THREAD ERROR\n"+CoreExtensions.ExtractAllInnerExceptions(e));
                }
            }) { IsBackground = false }.Start();
        }
    }
}
