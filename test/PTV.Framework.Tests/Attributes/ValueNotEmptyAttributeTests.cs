using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;
using PTV.Framework.Attributes;
using System.ComponentModel.DataAnnotations;
using PTV.Framework.ServiceManager;
using PTV.Framework.Tests.DummyClasses;

namespace PTV.Framework.Tests.Attributes
{
    public class ValueNotEmptyAttributeTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("         ")]
        [InlineData(" 12345 ")]
        [InlineData("12 34")]
        [InlineData("it is")]
        [InlineData("v r k")]
        [InlineData("Nunc lacinia augue eros, et tristique odio luctus eget. Vestibulum faucibus blandit consectetur.")]
        public void TestValidStrings(string valueToTest)
        {
            TestObject obj = new TestObject()
            {
                SomeString = valueToTest
            };

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "SomeString";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.SomeString, ctx, validationResults).Should().BeTrue();
        }

        [Theory]
        [InlineData("vrk")]
        [InlineData("1")]
        [InlineData("12")]
        [InlineData("123")]
        [InlineData("1234")]
        [InlineData("1  4")]
        [InlineData("1234 ")]
        [InlineData(" 1234")]
        [InlineData(" 1234 ")]
        public void TestInvalidStrings(string valueToTest)
        {
            TestObject obj = new TestObject()
            {
                SomeString = valueToTest
            };

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "SomeString";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.SomeString, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void ShouldThrowWhenUsedOnWrongTypeProperty()
        {
            TestObject obj = new TestObject()
            {
                InvalidPropertyType = new SomeDemoObject()
            };

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "InvalidPropertyType";

            Action act = () => Validator.ValidateProperty(obj.InvalidPropertyType, ctx);

            act.ShouldThrow<PtvArgumentException>().WithMessage($"Expected value is string! Value {obj.InvalidPropertyType} is not valid.", "Because value should be type of string.");
        }

        internal class TestObject
        {
            public TestObject()
            {
                //
            }

            [ValueNotEmpty]
            public string SomeString { get; set; }

            [ValueNotEmpty]
            public SomeDemoObject InvalidPropertyType { get; set; }
        }
    }
}
