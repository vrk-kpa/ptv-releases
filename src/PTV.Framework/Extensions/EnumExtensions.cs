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

using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PTV.Framework.Extensions
{
    /// <summary>
    /// Extension methods for enums.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets open api value (EnumOpenApiValueAttribute) by enum.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetOpenApiValue(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException($"value for {typeof(Enum)}");
            }

            string openApiValue = value.ToString();
            FieldInfo fieldInfo = value.GetType().GetField(openApiValue);
            EnumOpenApiValueAttribute[] attributes =
               (EnumOpenApiValueAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumOpenApiValueAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                openApiValue = attributes[0].Value;
            }

            return openApiValue;
        }

        /// <summary>
        /// Converts the enum type to a enum/open api value list.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<KeyValuePair<Enum, string>> ToList(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var list = new List<KeyValuePair<Enum, string>>();
            Array enumValues = Enum.GetValues(type);

            foreach (Enum value in enumValues)
            {
                list.Add(new KeyValuePair<Enum, string>(value, value.GetOpenApiValue()));
            }

            return list;
        }

        /// <summary>
        /// Gets open api value (EnumOpenApiValueAttribute) by enum value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The value of enum.</param>
        /// <returns></returns>
        public static string GetOpenApiEnumValue<TEnum>(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException($"value for {typeof(TEnum)}");
            }

            var openApiValue = value;
            FieldInfo fieldInfo = typeof(TEnum).GetField(openApiValue);
            EnumOpenApiValueAttribute[] attributes =
               (EnumOpenApiValueAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumOpenApiValueAttribute), false);

            if (attributes?.Length > 0)
            {
                openApiValue = attributes[0].Value;
            }

            return openApiValue;
        }

        /// <summary>
        /// Gets the enum value (as string) by EnumOpenApiValueAttribute
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="openApiValue"></param>
        /// <returns></returns>
        public static string GetEnumValueByOpenApiEnumValue<TEnum>(this string openApiValue)
        {
            return openApiValue.GetEnumValueByOpenApiEnumValue(typeof(TEnum));           
        }

        /// <summary>
        /// Gets the enum value (as string) by EnumOpenApiValueAttribute
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="openApiValue"></param>
        /// <returns></returns>
        public static string GetEnumValueByOpenApiEnumValue(this string openApiValue, Type enumType)
        {
            if (openApiValue == null)
            {
                throw new ArgumentNullException($"value for {enumType.Name}");
            }

            var list = enumType.ToList();

            var enumValue = list.FirstOrDefault(i => i.Value.Equals(openApiValue)).Key;

            if (enumValue == null)
            {
                throw new KeyNotFoundException();
            }
            
            return enumValue.ToString();
        }
    }
}
