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
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Tests.TestHelpers
{
    public class TestConversion
    {
        public Guid? GetGuid(string guid)
        {
            Guid result;
            if (Guid.TryParse(guid, out result))
            {
                return result;
            }
            return null;
        }

        public IReadOnlyCollection<string> GetValidTexts(params string[] texts)
        {
            return texts
                //.Where(x => !string.IsNullOrEmpty(x))
                .ToList();
        }

        public IReadOnlyCollection<string> GetValidTextsSkipEmpty(params string[] texts)
        {
            return texts.Where(x => !string.IsNullOrEmpty(x))
                .ToList();
        }
        public IReadOnlyCollection<string> GetValidTextsSkipEmptyKeepMandatory(string mandatoryText, params string[] texts)
        {
            var list = texts.Where(x => !string.IsNullOrEmpty(x))
                .ToList();
            list.Add(mandatoryText);
            return list;
        }
    }
}
