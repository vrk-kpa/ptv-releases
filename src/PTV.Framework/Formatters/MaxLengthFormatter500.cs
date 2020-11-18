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
    [RegisterFormatter(typeof(MaxLengthFormatter500))]
    public class MaxLengthFormatter500 : ValueFormatterAttribute
    {
        private ITextManager textManager;
        public readonly int MaxLength = 500;

        public MaxLengthFormatter500(int maxLength = 500, int order = 1) : base(typeof(MaxLengthFormatter500), order)
        {
            this.MaxLength = maxLength;
        }

        public override object Format(object value, string name, IResolveManager resolveManager)
        {
            if (resolveManager != null)
            {
                textManager = resolveManager.Resolve<ITextManager>();
            }
            else
            {
                throw new PtvArgumentException("Resolve manager cannot be null and has to be provided.");
            }

            if (value == null)
            {
                return null;
            }

            var valueToValidate = value as string;
            if (valueToValidate == null)
            {
                throw new PtvArgumentException($"Expected value is string! Value {value} of type {name} is not valid.");
            }

            var pureText = textManager.ConvertToPureText(valueToValidate);
            if (pureText.Length > MaxLength)
            {
                throw new ArgumentOutOfRangeException(name, $"Expected length of '{name}' value is exceeded {MaxLength} characters! '{name}' length {pureText.Length} is not valid.");
            }

            return value;
        }
    }
}
