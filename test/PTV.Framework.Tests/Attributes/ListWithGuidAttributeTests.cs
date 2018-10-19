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
using PTV.Framework.Attributes;
using System.ComponentModel.DataAnnotations;

namespace PTV.Framework.Tests.Attributes
{
    public class ListWithGuidAttributeTests
    {
        [Fact]
        public void EmptyListIsValid()
        {
            TestObject obj = new TestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Guids";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Guids, ctx, validationResults).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("         ")]
        public void TestValidSpecialGuidStrings(string specialGuid)
        {
            TestObject obj = new TestObject();
            obj.Guids.Add(specialGuid);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Guids";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Guids, ctx, validationResults).Should().BeTrue();
        }

        [Theory]
        [InlineData("N")]
        [InlineData("D")]
        [InlineData("B")]
        [InlineData("P")]
        [InlineData("X")]
        public void TestValidGuidStringFormats(string format)
        {
            TestObject obj = new TestObject();
            obj.Guids.Add(Guid.NewGuid().ToString(format));

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Guids";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Guids, ctx, validationResults).Should().BeTrue();
        }

        [Theory]
        [InlineData("hello world")]
        [InlineData("5555-8888-2922-1111")]
        public void TestInvalidGuidValues(string specialGuid)
        {
            TestObject obj = new TestObject();
            obj.Guids.Add(specialGuid);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Guids";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Guids, ctx, validationResults).Should().BeFalse();
        }

        internal class TestObject
        {
            public TestObject()
            {
                Guids = new List<string>();
            }

            [ListWithGuid]
            public List<string> Guids { get; private set; }
        }
    }
}
