using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;
using PTV.Framework.Attributes;

namespace PTV.Framework.Tests.Attributes
{
    public class ListWithPhoneAttributeTests
    {
        [Fact]
        public void EmptyListIsValid()
        {
            TestObject obj = new TestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "PhoneNumbers";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.PhoneNumbers, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void NullIsAllowedPhoneNumber()
        {
            TestObject obj = new TestObject();
            obj.PhoneNumbers.Add(null);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "PhoneNumbers";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.PhoneNumbers, ctx, validationResults).Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        [InlineData("ptv")]
        [InlineData("vrk@ptv.fi")]
        [InlineData("555,1234,543")]
        public void InvalidPhoneNumberValues(string phoneNumber)
        {
            TestObject obj = new TestObject();
            obj.PhoneNumbers.Add(phoneNumber);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "PhoneNumbers";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.PhoneNumbers, ctx, validationResults).Should().BeFalse();
        }

        [Theory]
        [InlineData("555 1234")]
        [InlineData("5 5 5 1 2 3 4")]
        [InlineData("5551234")]
        [InlineData("09 555 2929 626")]
        [InlineData("5551234 ")]
        [InlineData(" 5551234")]
        [InlineData("555-1234-543")]
        [InlineData("555+1234+543")]
        [InlineData("+358 (0)9 555 8282 334")]
        public void ValidPhoneNumberValues(string phoneNumber)
        {
            TestObject obj = new TestObject();
            obj.PhoneNumbers.Add(phoneNumber);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "PhoneNumbers";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.PhoneNumbers, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void UsedOnInvalidTypeShouldThrow()
        {
            TestObject obj = new TestObject();
            obj.UsedOnInvalidProperty = "ptv";

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "UsedOnInvalidProperty";

            Action act = () => Validator.ValidateProperty(obj.UsedOnInvalidProperty, ctx);

            act.ShouldThrowExactly<InvalidOperationException>().WithMessage(
                "The validation attribute is used on a property that doesn't implement IList (property name: 'UsedOnInvalidProperty').",
                "Because ListWithPhone is used on a type that doesnät implement IList.");
        }

        internal class TestObject
        {
            public TestObject()
            {
                PhoneNumbers = new List<string>();
            }

            [ListWithPhone]
            public List<string> PhoneNumbers { get; private set; }

            [ListWithPhone]
            public string UsedOnInvalidProperty { get; set; }
        }
    }
}
