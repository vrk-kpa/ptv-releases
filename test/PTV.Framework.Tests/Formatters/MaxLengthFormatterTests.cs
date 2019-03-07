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
using PTV.Framework.Interfaces;
using Moq;
using PTV.Framework.ServiceManager;
using PTV.Framework.Tests.DummyClasses;

namespace PTV.Framework.Tests.Formatters
{
    public class MaxLengthFormatterTests
    {
        [Fact]
        public void FormatValueShouldNotThrowNotImplemented()
        {
            MaxLengthFormatter formatter = new MaxLengthFormatter();

            Action act = () => formatter.Format("something");

            act.Should().NotThrow<NotImplementedException>();
        }

        [Fact]
        public void FormatValueNameShouldNotThrowNotImplemented()
        {
            MaxLengthFormatter formatter = new MaxLengthFormatter();

            Action act = () => formatter.Format("something", "somename");

            act.Should().NotThrow<NotImplementedException>();
        }

        [Fact]
        public void FormatValueNameResolveManagerShouldNotThrowNotImplemented()
        {
            MaxLengthFormatter formatter = new MaxLengthFormatter();

            Action act = () => formatter.Format("something", "somename", null);

            act.Should().NotThrow<NotImplementedException>();
        }

        [Fact]
        public void FormatValueNameResolveManagerEntityShouldNotThrowNotImplemented()
        {
            MaxLengthFormatter formatter = new MaxLengthFormatter();

            Action act = () => formatter.Format("something", "somename", null, null);

            act.Should().NotThrow<NotImplementedException>();
        }

        [Fact]
        public void IResolveManagerIsNullShouldThrow()
        {
            MaxLengthFormatter formatter = new MaxLengthFormatter();

            Action act = () => formatter.Format("something", "somename", null);

            act.Should().Throw<PtvArgumentException>().WithMessage("Resolve manager cannot be null and has to be provided.", "Because resolve manager is used for resolving of other instances.");
        }

        [Fact]
        public void TestWithPlainText()
        {
            MaxLengthFormatter formatter = new MaxLengthFormatter();

            string valueIn = "some text here";

            string valueOut = formatter.Format(valueIn, "DemoName", new MockResolveManager()) as string;

            // MaxLengthFormatter returns the same string that it was passed so the string references should be the same
            ReferenceEquals(valueIn, valueOut).Should().BeTrue();
        }

        [Fact]
        public void TestWithJson()
        {
            MaxLengthFormatter formatter = new MaxLengthFormatter(500);

            string valueIn = "{\"entityMap\":{},\"blocks\":[{\"text\":\"Toimisto ottaa vastaan asumisoikeusasunnon hakemukset järjestysnumeron saaneilta hakijoilta.\",\"type\":\"unstyled\",\"key\":\"N5A1X\",\"depth\":0,\"inlineStyleRanges\":[],\"entityRanges\":[]}]}";

            string valueOut = formatter.Format(valueIn, "DemoName", new MockResolveManager()) as string;

            // MaxLengthFormatter returns the same string that it was passed so the string references should be the same
            ReferenceEquals(valueIn, valueOut).Should().BeTrue();
        }

        [Fact]
        public void TestWithTooLongJson()
        {
            MaxLengthFormatter formatter = new MaxLengthFormatter(50);

            string valueIn = "{\"entityMap\":{},\"blocks\":[{\"text\":\"Toimisto ottaa vastaan asumisoikeusasunnon hakemukset järjestysnumeron saaneilta hakijoilta.\",\"type\":\"unstyled\",\"key\":\"N5A1X\",\"depth\":0,\"inlineStyleRanges\":[],\"entityRanges\":[]}]}";

            Action act = () => formatter.Format(valueIn, "DemoName", new MockResolveManager());
            act.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void TestWithObject()
        {
            // the formatter expects the value to be string
            MaxLengthFormatter formatter = new MaxLengthFormatter();
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
