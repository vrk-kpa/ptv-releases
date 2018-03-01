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
using System.Threading;

namespace PTV.Framework.Interfaces
{
    /// <summary>
    /// Resolve manager interface. ResolveManager is responsible for resolving instances from IoC container
    /// </summary>
    public interface IResolveManager
    {
        /// <summary>
        /// Resolve (get/instantiate) object of specific type from container
        /// </summary>
        /// <typeparam name="T">Specify type that should be resolved</typeparam>
        /// <returns>Resolved instance to object of requested type</returns>
        T Resolve<T>(bool isOptional = false) where T : class;

        /// <summary>
        /// Resolve (get/instantiate) object of specific type from container
        /// </summary>
        /// <typeparam name="type">Specify type that should be resolved</typeparam>
        /// <typeparam name="isOptional">set true if null should be returned if type is not registered instead of throwing exception</typeparam>
        /// <returns>Resolved instance to object of requested type</returns>
        object Resolve(Type type, bool isOptional = false);

        /// <summary>
        /// Resolve all types registered for specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> ResolveAllOfType<T>() where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <param name="action"></param>
        /// <param name="waitForExit"></param>
        Thread RunInThread(Action<IResolveManager> action, bool waitForExit = true);

        /// <summary>
        /// Run action in separated thread and wait for its exit
        /// </summary>
        /// <param name="action">Action that will be called in thread</param>
        void RunInScope(Action<IResolveManager> action);
    }

    public interface IResolveManagerTest : IResolveManager
    {
        void RegisterInstances<T>(IEnumerable<T> register);

        void RegisterInstance<T>(T register);

        void Clean();
    }
}
