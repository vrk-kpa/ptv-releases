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
