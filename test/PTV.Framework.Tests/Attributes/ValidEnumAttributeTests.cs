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
using PTV.Framework.Tests.DummyClasses;

namespace PTV.Framework.Tests.Attributes
{
    public class ValidEnumAttributeTests
    {
        // Comment: should the enumvalidator actually check that the type is given for the enum
        // pointless to have validator that doesn't actually validate anything when the enum type is not given

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("anything goes")]
        [InlineData(" ")]
        [InlineData("5")]
        public void EnumTypeNotSet(string testValue)
        {
            TestObject obj = new TestObject();
            obj.EnumTypeNotSet = testValue;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "EnumTypeNotSet";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.EnumTypeNotSet, ctx, validationResults).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Name")]
        [InlineData("AlternateName")]
        [InlineData("CarModel")]
        [InlineData("Hobby")]
        [InlineData("Color")]
        [InlineData("Browser")]
        [InlineData("Phone")]
        public void TestValidEnumValues(string testValue)
        {
            TestObject obj = new TestObject();
            obj.DummyEnumValue = testValue;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "DummyEnumValue";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.DummyEnumValue, ctx, validationResults).Should().BeTrue();
        }

        [Theory]
        [InlineData("name")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("VRK")]
        [InlineData("Alternate-Name")]
        public void TestInvalidValues(string testValue)
        {
            TestObject obj = new TestObject();
            obj.DummyEnumValue = testValue;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "DummyEnumValue";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.DummyEnumValue, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void TestWithObject()
        {
            TestObject obj = new TestObject();
            obj.Object = new SomeDemoObject()
            {
                Integer = 100,
                String = "Color",
                Type = "Hobby"
            };

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Object";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Object, ctx, validationResults).Should().BeFalse();
        }

        internal class TestObject
        {
            public TestObject()
            {
                //
            }

            [ValidEnum(null)]
            public string EnumTypeNotSet { get; set; }

            [ValidEnum(typeof(DummyEnum))]
            public string DummyEnumValue { get; set; }

            [ValidEnum(typeof(DummyEnum))]
            public SomeDemoObject Object { get; set; }
        }
    }
}
