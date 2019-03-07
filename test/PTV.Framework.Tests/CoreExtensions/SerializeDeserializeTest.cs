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
using FluentAssertions;
using Xunit;

namespace PTV.Framework.Tests.CoreExtensions
{
    public class SerializeDeserializeTest
    {
        [Fact]
        public void TestSerializeAndDeserialize()
        {
            SerTestObject obj = new SerTestObject()
            {
                Number = 100,
                Text = "Demo"
            };

            obj.SomeList.Add("ptv");
            obj.SomeList.Add("vrk");

            var serialized = obj.SerializeObject();

            serialized.Should().NotBeNullOrWhiteSpace();

            SerTestObject desObj = serialized.DeserializeObject<SerTestObject>();

            desObj.Should().NotBeNull();

            desObj.Number.Should().Be(obj.Number);
            desObj.Text.Should().Be(obj.Text);

            desObj.SomeList.Count.Should().Be(obj.SomeList.Count); // this is not really needed as the below line covers this too
            Enumerable.SequenceEqual(desObj.SomeList.OrderBy(k => k), obj.SomeList.OrderBy(t => t)).Should().BeTrue();
        }

        [Fact]
        public void SerializeObjectShouldHandleNullInstance()
        {
            SerTestObject obj = null;
            Action act = () => obj.SerializeObject();
            // currently throws NullReferenceException
            act.Should().ThrowExactly<ArgumentNullException>("Object to serialize is null.");
        }

        [Fact]
        public void DeserializeObjectShouldHandleNullInstance()
        {
            string des = null;

            Action act = () => des.DeserializeObject<SerTestObject>();

            act.Should().ThrowExactly<ArgumentNullException>("String to deserialize is null.");
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void DeserializeObjectShouldHandleEmptyStrings(string toDeserialize)
        {
            Action act = () => toDeserialize.DeserializeObject<SerTestObject>();

            act.Should().ThrowExactly<ArgumentException>("String to deserialize is empty or whitespaces. Would be better to throw ArgumetnException by the implementation that it is more clear to caller vs the currently thrown InvalidOperationException about bad xml.");
        }
        
        public class SerTestObject
        {
            public SerTestObject()
            {
                SomeList = new List<string>();
            }

            public int Number { get; set; }
            public string Text { get; set; }
            public List<string> SomeList { get; set; }
        }
    }
}
