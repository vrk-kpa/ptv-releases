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
using FluentAssertions;
using PTV.Framework.Attributes;
using PTV.Framework.Attributes.ValidationAttributes;
using PTV.Framework.Tests.DummyClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PTV.Framework.Tests.Attributes
{
    public class PTVUrlAttributeTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("http://[::]")]
        [InlineData("http://[::]:80")]
        [InlineData("http://[2002::]:80")]
        [InlineData("https://example.com")]
        [InlineData("https://example.com:8080")]
        [InlineData("https://example.com@username:password")]
        [InlineData("https://example.com/*?!#.,;")]
        [InlineData("http://localhost")]
        [InlineData("http://localhost:1234")]
        [InlineData("http://255.255.255.255")]
        public void ValidUrl(string validUrl)
        {
            PTVUrlAttribute attr = new PTVUrlAttribute();
            attr.IsValid(validUrl).Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("http://[::]:")]
        [InlineData("http://[0:0:0::0]:nn")]
        [InlineData("http://example .com")]
        [InlineData("http://example*.com")]
        [InlineData("http://example.c")]
        [InlineData("https://example:.com")]
        [InlineData("http://255.255.255")]
        public void InValidUrl(string invalidUrl)
        {
            PTVUrlAttribute attr = new PTVUrlAttribute();
            attr.IsValid(invalidUrl).Should().BeFalse();
        }
    }
}
