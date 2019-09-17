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
using FluentAssertions;
using PTV.Framework.Tests.DummyClasses;
using Xunit;

namespace PTV.Framework.Tests.CoreExtensions
{
    public class SetPropertyValueTest
    {
        [Fact]
        public void SetPropertyValueStringUsingExpression()
        {
            string valueToSet = "ptv";

            SomeDemoObject obj = new SomeDemoObject();

            obj.SetPropertyValue(x => x.String, valueToSet);

            obj.String.Should().Be(valueToSet);
        }

        [Fact]
        public void SetPropertyValueIntUsingExpression()
        {
            int valueToSet = 6;

            SomeDemoObject obj = new SomeDemoObject();

            obj.SetPropertyValue(x => x.Integer, valueToSet);

            obj.Integer.Should().Be(valueToSet);
        }

        [Fact]
        public void SetPropertyValueUsingInvalidExpressionShouldThrow()
        {
            string valueToSet = "ptv";

            SomeDemoObject obj = new SomeDemoObject();

            // invalid "memberacces" expression
            System.Linq.Expressions.Expression<Func<SomeDemoObject, string>> f = (sdo) => sdo.ToString();

            Action act = () => obj.SetPropertyValue(f, valueToSet);

            // currently if invalid expression is given the value is not set which can lead to cases that intrdocude hard to find bugs to our app
            // if implementation is changed to throw, change the expected exception type
            act.Should().Throw<Exception>("Maybe this should throw because the value is not set. Currently just silently does nothing because of invalid memberexpression.");
        }

        [Fact]
        public void SetPropertyValueNullInstanceShouldThrow()
        {
            // if instance is null and calling the extension method causes currently TargetInvocationException
            // our implementation should throw argumentnullexception or invalidoperationexception because called on null instead of instance
            SomeDemoObject obj = null;
            Action act = () => obj.SetPropertyValue("Type", "Test");
            act.Should().ThrowExactly<ArgumentException>("Calling SetPropertyValue on null instance.");
        }

        [Fact]
        public void SetPropertyValueNullPropertyNameShouldThrow()
        {
            // if property name is null the code should handle it and throw and not silently fail/do nothing
            SomeDemoObject obj = new SomeDemoObject();
            Action act = () => obj.SetPropertyValue(null, "something");
            act.Should().ThrowExactly<ArgumentNullException>("Property name is null.");
        }

        [Fact]
        public void SetPropertyValueNotFoundPropertyNameShouldThrow()
        {
            // if invalid property name is given it should throw and not just silently skip the setting of the value
            // if silently just skipping the caller doesn't know that the value is not set
            SomeDemoObject obj = new SomeDemoObject();
            Action act = () => obj.SetPropertyValue("NotFound", "something");
            act.Should().ThrowExactly<ArgumentException>("Invalid property name.");
        }

        [Fact]
        public void SetPropertyValue()
        {
            // if invalid property name is given it should throw and not just silently skip the setting of the value
            // if silently just skipping the caller doesn't know that the value is not set
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 100,
                String = "some text",
                Type = "Demo"
            };

            obj.SetPropertyValue("Integer", 65);
            obj.SetPropertyValue("Type", "Test");

            obj.Integer.Should().Be(65);
            obj.Type.Should().Be("Test");
        }
    }
}
