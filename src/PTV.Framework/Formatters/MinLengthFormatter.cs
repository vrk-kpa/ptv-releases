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
using PTV.Framework.Attributes;
using PTV.Framework.Formatters.Attributes;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Framework.Formatters
{
    [RegisterFormatter(typeof(MinLengthFormatter))]
    public class MinLengthFormatter : ValueFormatterAttribute
    {
        private readonly int minLength = 0;

        public MinLengthFormatter(int minLength = 0, int order = 1) : base(typeof(MinLengthFormatter), order)
        {
            this.minLength = minLength;
        }

        public override object Format(object value, string name)
        {
            if (value == null)
            {
                return value;
            }

            var valueToValidate = value as string;
            if (valueToValidate == null)
            {
                throw new PtvArgumentException($"Expected value is string! Value {value} of type {name} is not valid.");
            }

            if (valueToValidate.Length <= minLength)
            {
                throw new ArgumentOutOfRangeException(name, $"Expected length of '{name}' value is zero! '{name}' length zero is not valid.");
            }

            return value;
        }
    }
}
