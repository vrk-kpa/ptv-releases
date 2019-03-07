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
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Framework;

namespace PTV.Database.DataAccess.Translators
{
    [RegisterService(typeof(ITranslatedInstanceStorage), RegisterType.Transient)]
    internal class TranslatedInstanceStorage : ITranslatedInstanceStorage
    {
        private Dictionary<Type, Dictionary<object, object>> storage = new Dictionary<Type, Dictionary<object, object>>();
        
        public TOut ProcessInstance<TIn, TOut>(TIn source, TOut target) where TOut : class
        {
            var instanceType = source.GetType();
            Dictionary<object, object> similarObjects;
            if (!storage.TryGetValue(instanceType, out similarObjects))
            {
                similarObjects = new Dictionary<object, object>();
                similarObjects.Add(source, target);
                storage[instanceType] = similarObjects;
                return target;
            }
            object oldTarget;
            if (similarObjects.TryGetValue(source, out oldTarget))
            {
                return (oldTarget as TOut) ?? target;
            }
            similarObjects.Add(source, target);
            return target;
        }
    }
}
