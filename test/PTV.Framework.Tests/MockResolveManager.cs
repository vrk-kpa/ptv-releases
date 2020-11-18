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
using Moq;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace PTV.Framework.Tests
{
    internal class MockResolveManager : IResolveManager
    {
        public Mock<IResolveManager> ResolveManager { get; private set; }

        public MockResolveManager()
        {
            ResolveManager = new Mock<IResolveManager>();

            // wire up ITextManager
            ResolveManager.Setup(x => x.Resolve<ITextManager>(false)).Returns(new TextManager.TextManager());
            ResolveManager.Setup(x => x.Resolve<ITextManager>(true)).Returns(new TextManager.TextManager());

            // Add new wiring as needed :D
        }

        public T Resolve<T>(bool isOptional = false) where T : class
        {
            return ResolveManager.Object.Resolve<T>(isOptional);
        }

        public object Resolve(Type type, bool isOptional = false)
        {
            return ResolveManager.Object.Resolve(type, isOptional);
        }

        public IEnumerable<T> ResolveAllOfType<T>() where T : class
        {
            return ResolveManager.Object.ResolveAllOfType<T>();
        }

        public IServiceScope CreateScope()
        {
            var mockScope = new Mock<IServiceScope>();
            mockScope.Setup(x => x.ServiceProvider).Returns(Resolve<IServiceProvider>());
            return mockScope.Object;
        }

        public Thread RunInThread(Action<IResolveManager> action, bool waitForExit = true, bool nonUserIsolated = false)
        {
            throw new NotImplementedException();
        }

        public void RunInScope(Action<IResolveManager> action)
        {
            throw new NotImplementedException();
        }
    }
}
