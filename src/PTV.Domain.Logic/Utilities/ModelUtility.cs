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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PTV.Framework;
using PTV.Domain.Model.Models.V2.TranslationOrder.Json;

namespace PTV.Domain.Logic.Channels
{
    [RegisterService(typeof(ModelUtility), RegisterType.Singleton)]
    public class ModelUtility
    {
        public void SetMaxLengthToTranslationText<T>(T model)
        {
            if (model == null) return;

            var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.PropertyType == typeof(VmJsonTranslationText));
            foreach (PropertyInfo prop in props)
            {
                var attr = prop?.GetCustomAttributes(true).FirstOrDefault(x => x.GetType().Name == nameof(MaxLengthAttribute));
                if (attr != null)
                {
                    var maxLength = (attr as MaxLengthAttribute)?.Length;
                    (prop.GetValue(model) as VmJsonTranslationText).SafeCall(x => x.MaxLength = maxLength);
                }
            }
        }
    }
}
