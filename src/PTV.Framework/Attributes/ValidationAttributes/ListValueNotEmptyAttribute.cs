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

namespace PTV.Framework.Attributes
{
    /// <summary>
    /// Validates all the items in a list. Each item value must be empty or have at least five characters (leading and trailing white space omitted).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ListValueNotEmptyAttribute : ListBaseAttribute
    {
        public ListValueNotEmptyAttribute(string propertyName = "") : base(new ValueNotEmptyAttribute(), propertyName)
        {

        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("{0} - {1}", name, base.FormatErrorMessage(name));
        }
    }
}
