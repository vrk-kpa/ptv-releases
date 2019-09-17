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
    public class MinLengthFormatterTests
    {
        [Fact]
        public void FormatValueShouldNotThrowNotImplemented()
        {
            MinLengthFormatter formatter = new MinLengthFormatter(50);

            Action act = () => formatter.Format("something");

            act.Should().NotThrow<NotImplementedException>();
        }

        [Fact]
        public void FormatValueNameShouldNotThrowNotImplemented()
        {
            MinLengthFormatter formatter = new MinLengthFormatter(50);

            Action act = () => formatter.Format("something", "somename");

            act.Should().NotThrow<NotImplementedException>();
        }

        [Fact]
        public void FormatValueNameResolveManagerShouldNotThrowNotImplemented()
        {
            MinLengthFormatter formatter = new MinLengthFormatter(50);

            Action act = () => formatter.Format("something", "somename", null);

            act.Should().NotThrow<NotImplementedException>();
        }

        [Fact]
        public void FormatValueNameResolveManagerEntityShouldNotThrowNotImplemented()
        {
            MinLengthFormatter formatter = new MinLengthFormatter(50);

            Action act = () => formatter.Format("something", "somename", null, null);

            act.Should().NotThrow<NotImplementedException>();
        }

        [Fact]
        public void NullStringValue()
        {
            MinLengthFormatter formatter = new MinLengthFormatter(50);
            var formatted = formatter.Format(null);
            formatted.Should().BeNull();
        }

        [Fact]
        public void TestWithObject()
        {
            // the formatter expects the value to be string
            MinLengthFormatter formatter = new MinLengthFormatter(10);
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 100,
                String = "Some text",
                Type = "Demo"
            };

            Action act = () => formatter.Format(obj, "Demo", new MockResolveManager());
            act.Should().ThrowExactly<PtvArgumentException>($"Expected value is string! Value {obj.ToString()} of type Demo is not valid.");
        }
    }
}
