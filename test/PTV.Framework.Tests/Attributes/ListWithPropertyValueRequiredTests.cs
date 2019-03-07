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
using System.ComponentModel.DataAnnotations;
using System.Text;
using Xunit;

namespace PTV.Framework.Tests.Attributes
{
    public class ListWithPropertyValueRequiredTests
    {
        [Fact]
        public void UsedOnInvalidPropertyTypeShouldThrow()
        {
            TestObject obj = new TestObject();
            obj.UsedOnInvalidProperty = "ptv";

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "UsedOnInvalidProperty";

            Action act = () => Validator.ValidateProperty(obj.UsedOnInvalidProperty, ctx);

            act.Should().ThrowExactly<InvalidOperationException>().WithMessage(
                "The validation attribute is used on a property that doesn't implement IList (property name: 'UsedOnInvalidProperty').",
                "Because attribute is used on a type that doesn't implement IList.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("       ")]
        public void ConstructorShouldThrowIfPropertyNameIsNotGiven(string specialPropertyName)
        {
            TestObject obj = new TestObject();
            obj.Items.Add(new SomeDemoObject() { Type = "test" });

            Action act = () => new ListWithPropertyValueRequired(specialPropertyName, "test");

            // should throw argumentnullexception when the propertyname is null
            act.Should().Throw<ArgumentException>("Property name cannot be null, empty string or whitespace(s).");
        }

        [Fact]
        public void EmptyListIsNotValid()
        {
            TestObject obj = new TestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void NullListIsValid()
        {
            TestObject obj = new TestObject();
            obj.Items = null;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void NullObjectInListIsNotValid()
        {
            TestObject obj = new TestObject();
            obj.Items.Add(null);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void ShouldHandleNullPropertyValue()
        {
            TestObject obj = new TestObject();
            obj.Items.Add(new SomeDemoObject() { Type = "other" });
            obj.Items.Add(new SomeDemoObject() { Type = null }); // should handle the null value
            obj.Items.Add(new SomeDemoObject() { Type = "test" });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ListContainsValidMatch()
        {
            TestObject obj = new TestObject();
            obj.Items.Add(new SomeDemoObject() { Type = "other" });
            obj.Items.Add(new SomeDemoObject() { Type = "" });
            obj.Items.Add(new SomeDemoObject() { Type = "test" });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ListDoesntContainRequiredPropertyValue()
        {
            TestObject obj = new TestObject();
            obj.Items.Add(new SomeDemoObject() { Type = "other" });
            obj.Items.Add(new SomeDemoObject() { Type = "" });
            obj.Items.Add(new SomeDemoObject() { Type = "Test" }); // notice capital T

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void ListObjectDoesntContainRequiredProperty()
        {
            TestObject obj = new TestObject();
            obj.NotFoundPropertyNameItems.Add(new SomeDemoObject() { Type = "other" });
            obj.NotFoundPropertyNameItems.Add(new SomeDemoObject() { Type = "" });
            obj.NotFoundPropertyNameItems.Add(new SomeDemoObject() { Type = "Test" }); // notice capital T

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "NotFoundPropertyNameItems";

            Action act = () => Validator.ValidateProperty(obj.NotFoundPropertyNameItems, ctx);
            //Comment: it would be more helpful if themessage actually told that the requested property was not found from the list object
            act.Should().ThrowExactly<ValidationException>().WithMessage("NotFoundPropertyName - Required value 'test' was not found!");
        }

        [Fact]
        public void PublisIsValidAndDatacontextValidationShouldResultToSame()
        {
            TestObject obj = new TestObject();
            obj.Items.Add(new SomeDemoObject() { Type = "other" });
            obj.Items.Add(new SomeDemoObject() { Type = "" });
            obj.Items.Add(new SomeDemoObject() { Type = "test" });

            ListWithPropertyValueRequired attr = new ListWithPropertyValueRequired("Type", "test");
            bool publicIsValid = attr.IsValid(obj.Items);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().Be(publicIsValid);
        }

        internal class TestObject
        {
            public TestObject()
            {
                Items = new List<SomeDemoObject>();
                NotFoundPropertyNameItems = new List<SomeDemoObject>();
            }

            [ListWithPropertyValueRequired("Type", "test")]
            public List<SomeDemoObject> Items { get; set; }

            [ListWithPropertyValueRequired("NotFoundPropertyName", "test")]
            public List<SomeDemoObject> NotFoundPropertyNameItems { get; set; }

            [ListWithPropertyValueRequired("demopropertyname", "demopropertyvalue")]
            public string UsedOnInvalidProperty { get; set; }
        }
    }
}
