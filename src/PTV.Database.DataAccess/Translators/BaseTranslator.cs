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
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PTV.Database.DataAccess.Translators
{
    public static class BaseTranslator
    {
        public static readonly List<Type> NonStructPrimitiveTypes = new List<Type>
            {
                typeof(string)
            };

        private static bool StrEqual(string one, string two)
        {
            return string.Compare(one, two, StringComparison.OrdinalIgnoreCase) == 0;
        }

        private class EntityPropertyComparer : IEqualityComparer<PropertyInfo>
        {
            private bool ignoreType;

            public EntityPropertyComparer(bool ignoreType = false)
            {
                this.ignoreType = ignoreType;
            }

            public bool Equals(PropertyInfo item1, PropertyInfo item2)
            {
                if (item1 == null && item2 == null) return true;
                if ((item1 != null && item2 == null) || (item1 == null)) return false;
                return StrEqual(item1.Name, item2.Name) && (ignoreType ? true : item1.PropertyType == item2.PropertyType);
            }

            public int GetHashCode(PropertyInfo item)
            {
                return item.Name.ToLowerInvariant().GetHashCode();
            }
        }

        public static TOutput Translate<TInput, TOutput>(TInput source, TOutput target)
        {
            if ((source == null) || (target == null)) return target;
            var propertiesFrom = source.GetType().GetProperties();
            var propertiesTo = target.GetType().GetProperties();

            propertiesTo.Where(i => i.PropertyType == typeof(string)).Intersect(propertiesFrom, new EntityPropertyComparer(true))
                .Where(i => i.PropertyType.GetTypeInfo().IsPrimitive || i.PropertyType.GetTypeInfo().IsValueType || NonStructPrimitiveTypes.Contains(i.PropertyType))
                .ForEach(property =>
                            propertiesTo.First(i => StrEqual(i.Name, property.Name))
                                .SetValue(target, propertiesFrom.First(i => StrEqual(i.Name, property.Name)).GetValue(source)?.ToString()));

            propertiesTo.Intersect(propertiesFrom, new EntityPropertyComparer())
                        .Where(i => i.PropertyType.GetTypeInfo().IsPrimitive || i.PropertyType.GetTypeInfo().IsValueType || NonStructPrimitiveTypes.Contains(i.PropertyType))
                        .ForEach(property =>
                            propertiesTo.First(i => StrEqual(i.Name, property.Name))
                                .SetValue(target, propertiesFrom.First(i => StrEqual(i.Name, property.Name)).GetValue(source)));
            return target;
        }


        public static TType TranslateWithExceptions<TType>(TType source, TType target, IEnumerable<string> exceptProperties)
        {
            if ((source == null) || (target == null)) return target;
            var properties = source.GetType().GetProperties();

            properties.Where(i => i.PropertyType.GetTypeInfo().IsPrimitive || i.PropertyType.GetTypeInfo().IsValueType || NonStructPrimitiveTypes.Contains(i.PropertyType))
               .Where(i => !exceptProperties.Contains(i.Name)).ForEach(property => property.SetValue(target, GetCopy(property.GetValue(source))));
            return target;
        }

        public static object GetCopy(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj is string)
            {
                return obj.ToString();
            }
            return obj;
        }

        public static T TranslateEnum<T>(int? value, T defaultIfNull = default(T))
        {
            if (value == null)
            {
                return defaultIfNull;
            }

            Type enumType = typeof(T);

            if (Nullable.GetUnderlyingType(typeof (T)) != null)
            {
                enumType = enumType.GenericTypeArguments.First();
            }

            if (Enum.IsDefined(enumType, value.Value))
            {
                return (T)Enum.ToObject(enumType, value.Value);
            }
            throw new ArgumentOutOfRangeException("Value " + value.Value + " is not defined in enum " + enumType.Name);
        }
    }
}
