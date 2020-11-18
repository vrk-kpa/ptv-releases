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
using FluentAssertions;
using PTV.Framework.Tests.DummyClasses;
using Xunit;

namespace PTV.Framework.Tests.CoreExtensions
{
    public class SafeCallTest
    {
        [Fact]
        public void TestSafeCallWithNullInstance()
        {
            SomeDemoObject obj = null;

            Action act = () => obj.SafeCall(sdo => {
                sdo.Integer = 100;
                sdo.String = "Hello world";
                sdo.Type = "Demo";
            });

            act.Should().NotThrow();
        }

        [Fact]
        public void TestSafeCall()
        {
            SomeDemoObject obj = new SomeDemoObject
            {
                Integer = 9,
                String = "vrk",
                Type = "demo"
            };

            obj.SafeCall(sdo =>
            {
                sdo.Integer = 100;
            });

            obj.Integer.Should().Be(100);
        }

        [Fact]
        public void TestSafeCallReturnWithNullInstance()
        {
            SomeDemoObject obj = null;
            string retval = "vrk"; // just initializing to something, we are expecting this to be null after the safecall

            // implementation returns default(T) when called instance is null, so retval should be null
            retval = obj.SafeCall(sdo =>
            {
                return sdo.ToString();
            });

            retval.Should().BeNull();
        }

        [Fact]
        public void TestSafeCallReturn()
        {
            SomeDemoObject obj = new SomeDemoObject
            {
                Integer = 9,
                String = "vrk",
                Type = "demo"
            };

            string str = obj.ToString();

            string strB = obj.SafeCall(sdo =>
            {
                return sdo.ToString();
            });

            strB.Should().Be(str);
        }

        [Fact]
        public void TestSafeCallReturnCalculation()
        {
            SomeDemoObject obj = new SomeDemoObject
            {
                Integer = 9,
                String = "vrk",
                Type = "demo"
            };

            int calcResult = obj.SafeCall(sdo =>
            {
                return sdo.Integer + 1;
            });

            calcResult.Should().Be(10);
        }
    }
}
