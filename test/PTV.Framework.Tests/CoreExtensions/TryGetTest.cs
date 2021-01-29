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
using System.Collections.Generic;
using FluentAssertions;
using PTV.Framework.Tests.DummyClasses;
using Xunit;

namespace PTV.Framework.Tests.CoreExtensions
{
    public class TryGetTest
    {
        [Fact]
        public void TryGetDictionaryValueByKey()
        {
            Dictionary<string, SomeDemoObject> dict = new Dictionary<string, SomeDemoObject>();

            SomeDemoObject obj = new SomeDemoObject
            {
                Integer = 65
            };

            dict.Add("demo", obj);

            var foundObj = dict.TryGet("demo");

            foundObj.Should().NotBeNull();
            foundObj.Should().BeSameAs(obj);
        }

        [Theory]
        [InlineData("test")]
        [InlineData("")]
        public void TryGetDictionaryValueByKeyNotFound(string keyValue)
        {
            Dictionary<string, SomeDemoObject> dict = new Dictionary<string, SomeDemoObject>();

            SomeDemoObject obj = new SomeDemoObject
            {
                Integer = 65
            };

            dict.Add("demo", obj);

            var foundObj = dict.TryGet(keyValue);

            foundObj.Should().BeNull();
        }

        [Fact]
        public void TryGetDictionaryValueWithNullThrows()
        {
            Dictionary<string, SomeDemoObject> dict = new Dictionary<string, SomeDemoObject>();

            SomeDemoObject obj = new SomeDemoObject
            {
                Integer = 65
            };

            dict.Add("demo", obj);

            Action act = () => dict.TryGet(null);

            act.Should().ThrowExactly<ArgumentNullException>("Key cannot be null.");
        }

        [Fact]
        public void TryGetDictionaryValueWithNullInstanceShouldThrow()
        {
            Dictionary<string, SomeDemoObject> dict = null;

            Action act = () => dict.TryGet("somekey");

            act.Should().ThrowExactly<ArgumentNullException>("Implementation should handle the extension used on null instance.");
        }

        [Fact]
        public void TryGetIDictionaryValueByKey()
        {
            IDictionary<string, SomeDemoObject> dict = new Dictionary<string, SomeDemoObject>();

            SomeDemoObject obj = new SomeDemoObject
            {
                Integer = 65
            };

            dict.Add("demo", obj);

            var foundObj = dict.TryGet("demo");

            foundObj.Should().NotBeNull();
            foundObj.Should().BeSameAs(obj);
        }

        [Theory]
        [InlineData("test")]
        [InlineData("")]
        public void TryGetIDictionaryValueByKeyNotFound(string keyValue)
        {
            IDictionary<string, SomeDemoObject> dict = new Dictionary<string, SomeDemoObject>();

            SomeDemoObject obj = new SomeDemoObject
            {
                Integer = 65
            };

            dict.Add("demo", obj);

            var foundObj = dict.TryGet(keyValue);

            foundObj.Should().BeNull();
        }

        [Fact]
        public void TryGetIDictionaryValueWithNullThrows()
        {
            IDictionary<string, SomeDemoObject> dict = new Dictionary<string, SomeDemoObject>();

            SomeDemoObject obj = new SomeDemoObject
            {
                Integer = 65
            };

            dict.Add("demo", obj);

            Action act = () => dict.TryGet(null);

            act.Should().ThrowExactly<ArgumentNullException>("Key cannot be null.");
        }

        [Fact]
        public void TryGetIDictionaryValueWithNullInstanceShouldThrow()
        {
            IDictionary<string, SomeDemoObject> dict = null;

            Action act = () => dict.TryGet("somekey");

            act.Should().ThrowExactly<ArgumentNullException>("Implementation should handle the extension used on null instance.");
        }
    }
}
