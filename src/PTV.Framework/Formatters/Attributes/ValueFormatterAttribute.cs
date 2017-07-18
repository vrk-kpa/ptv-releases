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
using PTV.Framework.Interfaces;

namespace PTV.Framework.Formatters.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public abstract class ValueFormatterAttribute : Attribute, IValueFormatter
    {
        public int Order { get; set; }

        public string ValueFormatterName { get; }
        public Type ValueFormatterType { get; }

        protected ValueFormatterAttribute(Type type, int order = 1)
        {
            Order = order;
            ValueFormatterType = type;
            ValueFormatterName = type.AssemblyQualifiedName;
        }

        public virtual object Format(object value)
        {
            throw new NotImplementedException();
        }

        public virtual object Format(object value, string name)
        {
            return Format(value);
        }

        public virtual object Format(object value, string name, IResolveManager resolveManager)
        {
            return Format(value, name);
        }

        public virtual object Format(object value, string name, IResolveManager resolveManager, object entity)
        {
            return Format(value, name, resolveManager);
        }
    }
}
