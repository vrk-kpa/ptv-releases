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
using System.Text;
using FluentAssertions;
using Xunit;
using PTV.Framework;

namespace PTV.Framework.Tests
{
    public class LinqExtensionsTests
    {
        [Fact]
        public void ForEachEnumerableIsNullDoesntThrow()
        {
            IEnumerable<string> e = null;

            e.ForEach(s => {
                // could do something here for each string
            });
        }

        [Fact]
        public void ForEachEnumerableIsNullAndActionIsNullDoesntThrow()
        {
            IEnumerable<string> e = null;

            e.ForEach(null);
        }

        [Fact]
        public void ForEachActionIsNullDoesntThrow()
        {
            List<string> testItems = new List<string>(2);
            testItems.Add("ptv");
            testItems.Add("vrk");

            IEnumerable<string> e = testItems as IEnumerable<string>;

            e.ForEach(null);
        }

        [Fact]
        public void ForEachEnumerableIsEmptyDoesntThrow()
        {
            List<string> testItems = new List<string>();

            IEnumerable<string> e = testItems as IEnumerable<string>;

            e.ForEach(null);
        }

        [Fact]
        public void AddNullRangeListIsNullAndEnumerableIsNullDoesntThrow()
        {
            List<string> testItems = null;

            testItems.AddNullRange(null);
        }

        [Fact]
        public void AddNullRangeListIsNullDoesntThrow()
        {
            List<string> testItems = null;

            List<string> src = new List<string>(2);
            src.Add("ptv");
            src.Add("vrk");

            testItems.AddNullRange(src);
        }

        [Fact]
        public void AddNullRangeEnumerableIsNullDoesntThrow()
        {
            List<string> testItems = new List<string>();

            testItems.AddNullRange(null);
        }
    }
}
