﻿/**
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
using PTV.Framework.Tests.DummyClasses;

namespace PTV.Framework.Tests.Attributes
{
    public class ValidGuidAttributeTests
    {
        [Theory]
        [InlineData("5-5-5-5")]
        [InlineData("vrk")]
        [InlineData("ca761232ed4211cebacd00aa0057b22")] // missing one character
        [InlineData("CA761232-ED4211CE-BACD-00AA0057B223")] // missing one '-' character
        [InlineData("{CA761232-ED42-11CE-BACD-00AA0057B223")] // missing ending '}' character
        [InlineData("(CA761232-ED42-11CE-BACD-00AA0057B223")] // missing ending ')' character
        [InlineData("{0xCA761232, 0xED42, 0x11CE, {0xBA, 0xCD, 0x00, 0xAA, 0x00, 0x57, 0xB2, 0x23}")] // missing ending '}' character
        public void InvalidGuidValues(string guidString)
        {
            TestObject obj = new TestObject()
            {
                GuidString = guidString
            };

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "GuidString";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.GuidString, ctx, validationResults).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)] // Will be Guid.Empty
        [InlineData("")] // Will be Guid.Empty
        [InlineData(" ")] // Will be Guid.Empty
        [InlineData("  ")] // Will be Guid.Empty
        [InlineData("ca761232ed4211cebacd00aa0057b222")]
        [InlineData("CA761232-ED42-11CE-BACD-00AA0057B223")]
        [InlineData("{CA761232-ED42-11CE-BACD-00AA0057B223}")]
        [InlineData("(CA761232-ED42-11CE-BACD-00AA0057B223)")]
        [InlineData("{0xCA761232, 0xED42, 0x11CE, {0xBA, 0xCD, 0x00, 0xAA, 0x00, 0x57, 0xB2, 0x23}}")]
        public void ValidGuidValues(string guidString)
        {
            TestObject obj = new TestObject()
            {
                GuidString = guidString
            };

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "GuidString";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.GuidString, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void TestWithObject()
        {
            TestObject obj = new TestObject()
            {
                Object = new SomeDemoObject()
                {
                    Integer = 1,
                    String = "vrk",
                    Type = "Demo"
                }
            };

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Object";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Object, ctx, validationResults).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)] // Will be Guid.Empty
        [InlineData("")] // Will be Guid.Empty
        [InlineData(" ")] // Will be Guid.Empty
        [InlineData("  ")] // Will be Guid.Empty
        [InlineData("ca761232ed4211cebacd00aa0057b222")]
        [InlineData("CA761232-ED42-11CE-BACD-00AA0057B223")]
        [InlineData("{CA761232-ED42-11CE-BACD-00AA0057B223}")]
        [InlineData("(CA761232-ED42-11CE-BACD-00AA0057B223)")]
        [InlineData("{0xCA761232, 0xED42, 0x11CE, {0xBA, 0xCD, 0x00, 0xAA, 0x00, 0x57, 0xB2, 0x23}}")]
        public void TestWithGuidObject(string guidString)
        {
            TestObject obj = new TestObject()
            {
                GuidObject = new GuidObject()
                {
                    Guid = guidString
                }
            };

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "GuidObject";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.GuidObject, ctx, validationResults).Should().BeTrue();
        }

        internal class TestObject
        {
            public TestObject()
            {
                //
            }
            
            [ValidGuid]
            public string GuidString { get; set; }

            [ValidGuid]
            public SomeDemoObject Object { get; set; }

            [ValidGuid]
            public GuidObject GuidObject { get; set; }
        }

        internal class GuidObject
        {
            public string Guid { get; set; }

            public override string ToString()
            {
                return Guid;
            }

            public override int GetHashCode()
            {
                return Guid == null ? 0 : Guid.GetHashCode();
            }
        }
    }
}
