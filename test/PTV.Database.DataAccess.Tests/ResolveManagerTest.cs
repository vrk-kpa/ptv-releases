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
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace PTV.Database.DataAccess.Tests
{
    internal class ResolveManagerTest : IResolveManagerTest
    {
        private Dictionary<Type, List<object>> store = new Dictionary<Type, List<object>>();

        public bool RegisterInstance(TestRegisterServiceInfo register)
        {
            if (register != null)
            {
                var list = store.TryGet(register.RegisterAs) ?? new List<object>();
                store[register.RegisterAs] = list;
                list.Add(register.Instance);
                return true;
            }
            return false;
        }

        public void RegisterInstances<T>(IEnumerable<T> register)
        {
            foreach (var instance in register)
            {
                RegisterInstance(instance);
            }
        }

        public void RegisterInstance<T>(T register)
        {
            bool registered = RegisterInstance(register as TestRegisterServiceInfo);
            if (!registered)
            {
                var attributes =
                    register.GetType().GetTypeInfo().GetCustomAttributes<RegisterServiceAttribute>().ToList();
                foreach (var attr in attributes)
                {
                    RegisterInstance(new TestRegisterServiceInfo() {RegisterAs = attr.RegisterAs, Instance = register});
                }
            }
        }

        public T Resolve<T>(bool isOptional = false) where T : class
        {
            if (!store.ContainsKey(typeof(T)))
            {
                if (isOptional) return null;
                var type = typeof(T);
                string generic = type.GenericTypeArguments?.Any() == true ? type.GenericTypeArguments?.Select(x => x.Name).Aggregate((t1, t2) => $"{t1}, {t2}") : string.Empty;
                throw new Exception($"{type.Name}<{generic}> is missing. Registered keys are {string.Join("; ", store.Keys.Select(x => x.Name))}");
            }
            return (T) store[typeof (T)].FirstOrDefault();
        }

        public object Resolve(Type type, bool isOptional = false)
        {
            if (!store.ContainsKey(type))
            {
                string generic = type.GenericTypeArguments?.Any() == true ? type.GenericTypeArguments?.Select(x => x.Name).Aggregate((t1, t2) => $"{t1}, {t2}") : string.Empty;
                throw new Exception($"{type.Name}<{generic}> is missing. Registered keys are {string.Join("; ", store.Keys.Select(x => x.Name))}");
            }
            return store[type].FirstOrDefault();
        }

        public void Clean()
        {
            store = new Dictionary<Type, List<object>>();
        }

        public IEnumerable<T> ResolveAllOfType<T>() where T : class
        {
            var list = store.TryGet(typeof(T));
            return list.Cast<T>();
        }

        public Thread RunInThread(Action<IResolveManager> action, bool waitForExit = true)
        {
            throw new NotImplementedException();
        }

        public void RunInScope(Action<IResolveManager> action)
        {
            throw new NotImplementedException();
        }
    }

    internal class TestRegisterServiceInfo
    {
        public Type RegisterAs { get; set; }
        public object Instance { get; set; }
    }
}
