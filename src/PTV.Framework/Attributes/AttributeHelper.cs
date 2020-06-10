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
**/

using System;
using System.Text.RegularExpressions;

namespace PTV.Framework.Attributes
{
    public static class AttributeHelper
    {
        /// <summary>
        /// Convinience method mainly used by validation attributes to get list item value or custom object property value in a list validator.
        ///  If <paramref name="propertyName"/> is null or whitespace returns <paramref name="item"/>. Otherwise tries to get the named property value from item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="propertyName">null or empty string to get the item. Propertyname to get custom objects value</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> named property not found from <paramref name="item"/></exception>
        public static object GetItemValue(this object item, string propertyName)
        {
            if (item == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return item;
            }

            var property = item.GetType().GetProperty(propertyName);

            if (property == null)
            {
                throw new ArgumentException($"Item doesn't contain property named: '{propertyName}'.", nameof(propertyName));
            }

            return property.GetValue(item, null);
        }

        public static bool IsValidFQDN(this string fqdn)
        {
            var parts = fqdn.Split('.');

            if (parts.Length < 2)
            {
                return false;
            }

            var partRegEx = new Regex(@"^[a-zA-Z\u00a1-\uffff0-9-]+$");
            for (int i = 0; i < parts.Length; i++)
            {
                if (i == parts.Length - 1) // Is this the last part?
                {
                    var lastPartRegEx = new Regex(@"^([a-zA-Z\u00a1-\uffff]{2,}|xn[a-z0-9-]{2,})$");
                    if (!lastPartRegEx.Match(parts[i]).Success)
                    {
                        return false;
                    }
                }
                else if (!partRegEx.Match(parts[i]).Success)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
