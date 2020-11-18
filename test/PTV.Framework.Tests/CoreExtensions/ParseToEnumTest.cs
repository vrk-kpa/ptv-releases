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
    public class ParseToEnumTest
    {
        [Fact]
        public void TestParseToEnum()
        {
            var parsed = "Color".Parse<DummyEnum>();
            parsed.Should().HaveFlag(DummyEnum.Color);
        }

        [Fact]
        public void TestParseToEnumNotEnumTypeThrows()
        {
            Action act = () => "Name".Parse<SomeDemoObject>();
            act.Should().ThrowExactly<ServiceManager.PtvArgumentException>("Because generic type is not enum type.");
        }

        [Fact]
        public void TestParseToEnumInvalidValueShouldThrow()
        {
            Action act = () => "PTV-VRK".Parse<DummyEnum>();
            act.Should().ThrowExactly<Exception>().WithMessage("The field is invalid. Please use one of these: *");
        }

        [Fact]
        public void TestParseToEnumNullValueShouldThrow()
        {
            string val = null;
            Action act = () => val.Parse<DummyEnum>();
            act.Should().ThrowExactly<Exception>().WithMessage("The field is invalid. Please use one of these: *");
        }
    }
}
