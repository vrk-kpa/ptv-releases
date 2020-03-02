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
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace PTV.Framework.Tests.CoreExtensions
{
    public class IsEnumerableTest
    {
        [Fact]
        public void TestGenericListIsEnumerable()
        {
            typeof(List<string>).IsEnumerable().Should().BeTrue();
        }

        [Fact]
        public void TestArrayListIsNotEnumerable()
        {
            // extension assumes generic ienumerable<T>
            typeof(ArrayList).IsEnumerable().Should().BeFalse();
        }

        [Fact]
        public void TestStringIsEnumerable()
        {
            // extension assumes generic ienumerable<T>
            typeof(string).IsEnumerable().Should().BeTrue();
        }

        [Fact]
        public void TestStringExcludedIsNotEnumerable()
        {
            // extension assumes generic ienumerable<T>
            typeof(string).IsEnumerable(new List<Type> { typeof(string) }).Should().BeFalse();
        }

        [Fact]
        public void IsEnumerableShouldHandleNullInstance()
        {
            Type t = null;
            Action act = () => t.IsEnumerable();

            // parameter name: target would actually be the correct one as the current exception is thrown by framework
            act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'target')");
        }
    }
}
