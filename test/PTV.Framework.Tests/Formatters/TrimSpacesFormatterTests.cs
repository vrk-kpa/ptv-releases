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
using PTV.Framework.Formatters;
using PTV.Framework.ServiceManager;
using PTV.Framework.Tests.DummyClasses;

namespace PTV.Framework.Tests.Formatters
{
    public class TrimSpacesFormatterTests
    {
        [Fact]
        public void FormatValueShouldNotThrowNotImplemented()
        {
            TrimSpacesFormatter formatter = new TrimSpacesFormatter();

            Action act = () => formatter.Format("something");

            act.Should().NotThrow<NotImplementedException>();
        }

        [Fact]
        public void FormatValueNameShouldNotThrowNotImplemented()
        {
            TrimSpacesFormatter formatter = new TrimSpacesFormatter();

            Action act = () => formatter.Format("something", "somename");

            act.Should().NotThrow<NotImplementedException>();
        }

        [Fact]
        public void FormatValueNameResolveManagerShouldNotThrowNotImplemented()
        {
            TrimSpacesFormatter formatter = new TrimSpacesFormatter();

            Action act = () => formatter.Format("something", "somename", null);

            act.Should().NotThrow<NotImplementedException>();
        }

        [Fact]
        public void FormatValueNameResolveManagerEntityShouldNotThrowNotImplemented()
        {
            TrimSpacesFormatter formatter = new TrimSpacesFormatter();

            Action act = () => formatter.Format("something", "somename", null, null);

            act.Should().NotThrow<NotImplementedException>();
        }

        [Fact]
        public void NullStringValue()
        {
            // passing null to format should return null
            TrimSpacesFormatter formatter = new TrimSpacesFormatter();
            var formatted = formatter.Format(null);
            formatted.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        public void EmptyOrWhitespacesValueShouldReturnEmptyString(string inValue)
        {
            TrimSpacesFormatter formatter = new TrimSpacesFormatter();
            // expecting a string other than null is always returned
            var formatted = formatter.Format(inValue) as string;
            formatted.Length.Should().Be(0);
        }

        [Theory]
        [InlineData("ptv")]
        [InlineData("ptv vrk")]
        [InlineData("väestörekisterikeskus ptv vrk")]
        public void ValidValuesNothingToTrim(string inValue)
        {
            TrimSpacesFormatter formatter = new TrimSpacesFormatter();
            // expecting a string other than null is always returned
            var formatted = formatter.Format(inValue) as string;
            // not comparing the reference but the actual string
            formatted.Should().Be(inValue);
        }

        [Theory]
        [InlineData("ptv ")]
        [InlineData(" ptv vrk")]
        [InlineData(" väestörekisterikeskus ptv vrk ")]
        [InlineData("   väestörekisterikeskus ptv vrk   ")]
        public void ValuesToTrim(string inValue)
        {
            TrimSpacesFormatter formatter = new TrimSpacesFormatter();
            // expecting a string other than null is always returned
            var formatted = formatter.Format(inValue) as string;
            // all test cases have atleast one whitespace to trim
            formatted.Length.Should().BeLessOrEqualTo(inValue.Length - 1);
        }

        [Fact]
        public void MultipleSpacesReplacedWithOne()
        {
            TrimSpacesFormatter formatter = new TrimSpacesFormatter();

            // implementation replaces all occurances of space characters with one space character
            // so if we have string " something  somewhere " => "something somewhere" so the middle double space is also trimmer to one space
            string formatted = formatter.Format(" something  somewhere ") as string;
            formatted.Should().Be("something somewhere");
        }

        [Fact]
        public void IfNoSpacesThenSameReference()
        {
            // if the string to be formatted doesn't have any spaces, then the same string reference should be returned
            string invalue = "someword";

            TrimSpacesFormatter formatter = new TrimSpacesFormatter();
            var f = formatter.Format(invalue);
            f.Should().BeSameAs(invalue);
        }

        [Fact]
        public void TestWithObject()
        {
            // the formatter expects the value to be string
            TrimSpacesFormatter formatter = new TrimSpacesFormatter();
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 100,
                String = "Some text",
                Type = "Demo"
            };

            Action act = () => formatter.Format(obj);
            act.Should().ThrowExactly<PtvArgumentException>().WithMessage($"Value to be formatted is expected to be of type string. Passed values type is 'PTV.Framework.Tests.DummyClasses.SomeDemoObject'.");
        }
    }
}
