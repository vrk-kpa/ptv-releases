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
using FluentAssertions;
using Xunit;

namespace PTV.Framework.Tests.CoreExtensions
{
    public class ParseToIntTest
    {
        [Theory]
        [InlineData(-100000)]
        [InlineData(-15000)]
        [InlineData(-2795)]
        [InlineData(-101)]
        [InlineData(-10)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(100000)]
        [InlineData(15000)]
        [InlineData(2795)]
        [InlineData(101)]
        [InlineData(10)]
        [InlineData(1)]
        public void TestParseToInt(int number)
        {
            // functionality relies on current thread localization/number formatter
            string n = number.ToString();

            int? parsed = n.ParseToInt();

            parsed.Should().HaveValue();
            parsed.Value.Should().Be(number);
        }

        [Fact]
        public void TestParseToIntHandlesNullInstance()
        {
            string toParse = null;
            int? parsed = toParse.ParseToInt();

            parsed.Should().NotHaveValue();
        }
    }
}
