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

using System.Text.RegularExpressions;
using PTV.Framework.Attributes;
using PTV.Framework.Formatters.Attributes;
using PTV.Framework.ServiceManager;

namespace PTV.Framework.Formatters
{
    [RegisterFormatter(typeof(TrimSpacesFormatter))]
    public class TrimSpacesFormatter : ValueFormatterAttribute
    {
        public TrimSpacesFormatter(int order = 1) : base(typeof(TrimSpacesFormatter), order)
        {
        }

        public override object Format(object value)
        {
            if (value == null) return null;

            var valueToFormat = value as string;

            if (valueToFormat == null)
            {
                throw new PtvArgumentException($"Value to be formatted is expected to be of type string. Passed values type is '{value?.GetType()}'.");
            }
            // Remove all leading and trailling spaces //
            valueToFormat = valueToFormat.Trim();
            // Remove doubled whitespaces replace them with one //
            valueToFormat = Regex.Replace(valueToFormat, @"[ ]{2,}", " ");
            // Remove soft hyphens //
            valueToFormat = Regex.Replace(valueToFormat, "&shy;", "");
            return valueToFormat;
        }
    }
}
