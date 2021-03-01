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
    public class GetGuidTest
    {
        [Fact]
        public void GetGuidFromStringValues()
        {
            string valueToUse = "SomeDemoObject";
            string forType = null;

            Guid g = valueToUse.GetGuid();
            Guid gNull = valueToUse.GetGuid(forType);

            forType = string.Empty;

            Guid gEmpty = valueToUse.GetGuid(forType);

            // returned guid shouldn't be empty
            g.Should().NotBeEmpty();

            // using default value for type, null or empty string should all return the same "new" guid
            g.Should().Be(gNull);
            gNull.Should().Be(gEmpty);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetGuidUsingNullAndEmptyValues(string value)
        {
            Guid g = value.GetGuid();
            g.Should().NotBeEmpty();
        }

        [Fact]
        public void GetGuidFromStringValuesTyped()
        {
            string valueToUse = "some text";

            Guid g = valueToUse.GetGuid<SomeDemoObject>();
            Guid gnd = valueToUse.GetGuid<SomeDemoObject>();

            // returned guid shouldn't be empty
            g.Should().NotBeEmpty();

            // should be the same guid always for the same string using the same type
            g.Should().Be(gnd);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetGuidUsingNullAndEmptyValuesTyped(string value)
        {
            Guid g = value.GetGuid<SomeDemoObject>();
            g.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetGuidUsingNullAndEmptyValuesWithType(string value)
        {
            Guid g = value.GetGuid(typeof(SomeDemoObject));
            g.Should().NotBeEmpty();
        }

        [Fact]
        public void GetGuidUsingTypeShouldHandleNull()
        {
            // passing null as type should work because other overloads also work with null type (where passed as string)
            Type t = null;
            Guid g = "some string".GetGuid(t);

            g.Should().NotBeEmpty();
        }
    }
}
