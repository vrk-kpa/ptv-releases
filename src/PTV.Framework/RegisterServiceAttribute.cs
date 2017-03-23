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
using System.Threading.Tasks;

namespace PTV.Framework
{
    /// <summary>
    /// Attribute for marking classes. This class (type) will be registered into IoC container.
    /// Specify type (interface) as witch the decorated class will be registered. Specify type of registration - how the it will be instantiated when requested
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RegisterServiceAttribute : Attribute
    {
        public RegisterType RegistrationType { get; private set; }
        public Type RegisterAs { get; private set; }

        public RegisterServiceAttribute(Type registerAs, RegisterType type)
        {
            RegistrationType = type;
            RegisterAs = registerAs;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class QueryFilterAttribute : Attribute
    {
        //public Type ForType { get; private set; }

        public QueryFilterAttribute()
        {
           // ForType = forType;
        }
    }

    /// <summary>
    /// Type of registration, Transient : instantiated every time; Scope : instantiated per request; Singleton : one instance only per app
    /// </summary>
    public enum RegisterType
    {
        Transient,
        Scope,
        Singleton
    }
}
