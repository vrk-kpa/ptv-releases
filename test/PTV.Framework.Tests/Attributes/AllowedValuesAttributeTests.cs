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
