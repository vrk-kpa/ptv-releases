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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Framework;
using System.Reflection;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators
{
    [RegisterService(typeof(ITranslationPrimitives), RegisterType.Transient)]
    internal class TranslationPrimitives : ITranslationPrimitives
    {
        private static readonly List<Type> AssumeAsValueTypeList = new List<Type>
        {
            typeof (string)
        };

        private IResolveManager resolveManager;
        public TranslationPrimitives(IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
        }

        public bool CanBeTranslatedAsPrimitive<TIn, TOut>()
        {
            bool isInputPropertyPrimitive = IsPrimitiveTypeProperty<TIn>();
            bool isOutputPropertyPrimitive = IsPrimitiveTypeProperty<TOut>();
            if (!isInputPropertyPrimitive && !isOutputPropertyPrimitive) return false;
            if (isInputPropertyPrimitive != isOutputPropertyPrimitive)
            {
                return false;
                //throw new Exception($"{CoreMessages.IncorrectTranslationDefinition}. Input type {typeof(TIn).Name} and output type {typeof(TOut).Name} ");
            }
            return typeof(TOut).IsAssignableFrom(typeof(TIn));
        }

        public bool TranslatePrimitive<TIn, TOut, TInProperty, TOutProperty>(TIn source, ref TOut target, Func<TIn, TInProperty> fromProperty,
            Expression<Func<TOut, TOutProperty>> toProperty)
        {
            return TranslatePrimitiveInternal<TIn, TOut, TInProperty, TOutProperty>(source, ref target, fromProperty, toProperty);
        }

        private bool TranslatePrimitiveInternal<TIn, TOut, TInProperty, TOutProperty>(TIn source, ref TOut target, Func<TIn, TInProperty> fromProperty,
            Expression<Func<TOut, TOutProperty>> toProperty)
        {
            if (!CanBeTranslatedAsPrimitive<TInProperty, TOutProperty>())
            {
                return ValueTranslation(source, ref target, fromProperty, toProperty);
            }
            var valueOfTargetProp = toProperty.Compile()(target);
            if ((valueOfTargetProp != null) && (valueOfTargetProp.Equals(target)))
            {
                target = (TOut)(object)fromProperty(source);
                return true;
            }
            target.SetPropertyValue(toProperty, fromProperty(source));
            return true;
        }

        public bool TranslatePrimitive<TIn, TOut, TInProperty, TOutProperty>(TIn source, ref TOut target, Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty)
        {
            if (!CanBeTranslatedAsPrimitive<TInProperty, TOutProperty>())
            {
                return ValueTranslation(source, ref target, fromProperty, toProperty);
            }
            target.SetPropertyValue(toProperty, fromProperty(source).Select(i => i).ToList());
            return true;
        }


        private bool TranslateByValueTranslator<TIn, TOut>(TIn input, out TOut output)
        {
            var translatorL2R = this.resolveManager.Resolve<IValueTranslator<TIn, TOut>>(isOptional: true);
            if (translatorL2R != null)
            {
                var result = translatorL2R.TranslateLeftToRight(input);
                output = result;
                return true;
            }
            var translatorR2L = this.resolveManager.Resolve<IValueTranslator<TOut, TIn>>(isOptional: true);
            if (translatorR2L != null)
            {
                var result = translatorR2L.TranslateRightToLeft(input);
                output = result;
                return true;
            }
            output = default(TOut);
            return false;
        }

        private bool ValueTranslation<TIn, TOut, TInProperty, TOutProperty>(TIn source, ref TOut target, Func<TIn, TInProperty> fromProperty,
           Expression<Func<TOut, TOutProperty>> toProperty)
        {
            var value = fromProperty(source);
            TOutProperty outValue;
            if (!TranslateByValueTranslator(value, out outValue)) return false;
            target.SetPropertyValue(toProperty, outValue);
            return true;
        }



        private bool IsPrimitiveTypeProperty<TestedType>()
        {
            var propertyType = typeof(TestedType);
            var propertyTypeInfo = propertyType.GetTypeInfo();
            return propertyTypeInfo.IsPrimitive || propertyTypeInfo.IsValueType || AssumeAsValueTypeList.Contains(propertyType) || (Nullable.GetUnderlyingType(propertyType) != null);
        }
    }
}
