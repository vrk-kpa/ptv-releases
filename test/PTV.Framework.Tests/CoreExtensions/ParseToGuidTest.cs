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
using Xunit;

namespace PTV.Framework.Tests.CoreExtensions
{
    public class ParseToGuidTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")] // Will be Guid.Empty
        [InlineData(" ")] // Will be Guid.Empty
        [InlineData("  ")] // Will be Guid.Empty
        [InlineData("5-5-5-5")]
        [InlineData("vrk")]
        [InlineData("ca761232ed4211cebacd00aa0057b22")] // missing one character
        [InlineData("CA761232-ED4211CE-BACD-00AA0057B223")] // missing one '-' character
        [InlineData("{CA761232-ED42-11CE-BACD-00AA0057B223")] // missing ending '}' character
        [InlineData("(CA761232-ED42-11CE-BACD-00AA0057B223")] // missing ending ')' character
        [InlineData("{0xCA761232, 0xED42, 0x11CE, {0xBA, 0xCD, 0x00, 0xAA, 0x00, 0x57, 0xB2, 0x23}")] // missing ending '}' character
        public void ParseInvalidStringToGuid(string valueToParse)
        {
            Guid? g = valueToParse.ParseToGuid();
            g.HasValue.Should().BeFalse();
        }

        [Theory]
        [InlineData("ca761232ed4211cebacd00aa0057b222")]
        [InlineData("CA761232-ED42-11CE-BACD-00AA0057B223")]
        [InlineData("{CA761232-ED42-11CE-BACD-00AA0057B223}")]
        [InlineData("(CA761232-ED42-11CE-BACD-00AA0057B223)")]
        [InlineData("{0xCA761232, 0xED42, 0x11CE, {0xBA, 0xCD, 0x00, 0xAA, 0x00, 0x57, 0xB2, 0x23}}")]
        public void ParseValidStringToGuid(string valueToParse)
        {
            Guid? g = valueToParse.ParseToGuid();
            g.HasValue.Should().BeTrue();
        }
    }
}
