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
using System.Reflection;

namespace PTV.Application.OpenApi
{
    public static class HelperExtensions
    {
        public static Guid ParseToGuidWithExeption(this string stringGuid)
        {
            Guid? guid = stringGuid.ParseToGuid();
            if (guid == null)
            {
                throw new Exception($"Cannot parse '{stringGuid}' to type of Guid.");
            }
            return guid.Value;
        }

        public static TEnum Parse<TEnum>(this string enumValue)
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum) throw new ArgumentException(CoreMessages.OpenApi.NotEnum);

            try
            {
                return (TEnum)Enum.Parse(typeof(TEnum), enumValue);
            }
            catch(Exception)
            {
                throw new Exception(string.Format(CoreMessages.OpenApi.RequestMalFormatted, typeof(TEnum).Name, GetEnumvaluesAsList<TEnum>()));
            }
        }

        private static string GetEnumvaluesAsList<TEnum>()
        {
            return string.Join(", ", Enum.GetNames(typeof(TEnum)));
        }
    }
}
