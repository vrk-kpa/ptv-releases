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
    public class ParseToDoubleTest
    {
        [Theory]
        [InlineData(-100000.01D)]
        [InlineData(-15000.02D)]
        [InlineData(-2795.00D)]
        [InlineData(-101.00D)]
        [InlineData(-10.99D)]
        [InlineData(-1.0D)]
        [InlineData(0.00D)]
        [InlineData(100000.06D)]
        [InlineData(15000.75D)]
        [InlineData(2795.60D)]
        [InlineData(101.52D)]
        [InlineData(10.1D)]
        [InlineData(1.00000D)]
        public void TestParseToDouble(double number)
        {
            // functionality relies on current thread localization/number formatter
            string n = number.ToString();

            double? parsed = n.ParseToDouble();

            parsed.Should().HaveValue();
            parsed.Value.Should().Be(number);
        }

        [Fact]
        public void TestParseToDoubleHandlesNullInstance()
        {
            string toParse = null;
            double? parsed = toParse.ParseToDouble();

            parsed.Should().NotHaveValue();
        }
    }
}
