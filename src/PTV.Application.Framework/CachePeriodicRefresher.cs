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
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.DependencyModel;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Framework;
using PTV.Framework.Attributes;

namespace PTV.Application.Framework
{
    public class CachePeriodicRefresher
    {
        private readonly TimeSpan cacheCheckingInterval = TimeSpan.FromMinutes(60);
        private readonly IServiceProvider serviceProvider;

        public CachePeriodicRefresher(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            StartChecker();
        }

        private void StartChecker()
        {
            new Thread(() =>
            {
                while (Thread.CurrentThread.ThreadState == ThreadState.Background)
                {
                    Thread.Sleep(cacheCheckingInterval);
                    var (refreshableIfaces, refreshableTypes) = RefreshableCaches.GetRegisteredCaches();
                    refreshableTypes.ForEach(rf =>
                    {
                        (serviceProvider.GetService(rf) as ICacheAfterChangeRefresh)?.CheckAndRefresh();
                    });
                    refreshableIfaces.ForEach(rf =>
                    {
                        rf.CheckAndRefresh();
                    });
                }
            }){IsBackground = true}.Start();
        }
    }
}