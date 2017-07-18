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
