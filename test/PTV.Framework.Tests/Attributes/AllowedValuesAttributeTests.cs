using FluentAssertions;
using PTV.Framework.Attributes;
using PTV.Framework.Tests.DummyClasses;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PTV.Framework.Tests.Attributes
{
    public class AllowedValuesAttributeTests
    {
        // Comment: assumption how the AllowedValuesAttribute should work
        // - property name is just a text to display
        // - null value is always allowed

        [Fact]
        public void ThrowIfAllowedValuesNotDefined()
        {
            // the constructor should throw if allowedValues is not defined
            Action act = () => new AllowedValuesAttribute();
            act.ShouldThrowExactly<ArgumentNullException>().WithMessage("Allowed values must be defined.*");
        }

        [Fact]
        public void ThrowIfAllowedValuesSetToNull()
        {
            // the constructor should throw if allowedValues is not defined
            Action act = () => new AllowedValuesAttribute(allowedValues: null);
            act.ShouldThrowExactly<ArgumentNullException>().WithMessage("Allowed values must be defined.*");
        }

        [Fact]
        public void NullAllowedEmptyAllowedList()
        {
            AllowedValuesAttribute ava = new AllowedValuesAttribute(allowedValues: new string[] { });
            ava.IsValid(null).Should().BeTrue();
        }

        [Fact]
        public void NullValueAlwaysAllowed()
        {
            AllowedValuesAttribute ava = new AllowedValuesAttribute(allowedValues: new string[] { "ptv", "vrk" });
            ava.IsValid(null).Should().BeTrue();
        }


        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ptv")]
        public void AllowedListEmptyNothingShouldBeValid(string valueToValidate)
        {
            AllowedValuesAttribute ava = new AllowedValuesAttribute(allowedValues: new string[] { });
            ava.IsValid(valueToValidate).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ptv")]
        public void AllowedValuesDefined(string valueToValidate)
        {
            AllowedValuesAttribute ava = new AllowedValuesAttribute(allowedValues: new string[] { "", " ", "ptv", "something else" });
            ava.IsValid(string.Empty).Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void AllowedValuesSetValidateEmptyOrWhitespaceStringNotAllowed(string valueToValidate)
        {
            AllowedValuesAttribute ava = new AllowedValuesAttribute(allowedValues: new string[] { "ptv", "vrk" });
            ava.IsValid(valueToValidate).Should().BeFalse();
        }

        [Theory]
        [InlineData("PTV")]
        [InlineData("VRK")]
        public void AllowedValuesSetValidateCaseSensitivity(string valueToValidate)
        {
            AllowedValuesAttribute ava = new AllowedValuesAttribute(allowedValues: new string[] { "ptv", "vrk" });
            ava.IsValid(valueToValidate).Should().BeFalse();
        }
    }
}
