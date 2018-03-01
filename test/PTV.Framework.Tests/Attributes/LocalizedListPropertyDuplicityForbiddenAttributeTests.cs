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
    public class LocalizedListPropertyDuplicityForbiddenAttributeTests
    {
        [Fact]
        public void ListNullAllowed()
        {
            TestObject obj = new TestObject();
            obj.Items = null;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ListEmptyAllowed()
        {
            TestObject obj = new TestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ValidItemsInTheList()
        {
            TestObject obj = new TestObject();
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "fi",
                Type = "Demo"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "en",
                Type = "Demo"
            });

            // different casing in language and type
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "Fi",
                Type = "Demo"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "en",
                Type = "demo"
            });

            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "sv",
                Type = "Demo"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "fi",
                Type = "Test"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "en",
                Type = "Test"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "fi",
                Type = ""
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "en",
                Type = ""
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "fi",
                Type = " "
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "en",
                Type = " "
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "",
                Type = "Test"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = " ",
                Type = "Test"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "",
                Type = "Demo"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = " ",
                Type = "Demo"
            });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeTrue();
        }

        [Theory]
        [InlineData("fi", "Demo")]
        [InlineData("en", "Test")]
        public void DuplicateItemsInTheList(string language, string type)
        {
            TestObject obj = new TestObject();
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "fi",
                Type = "Demo"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "en",
                Type = "Demo"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "sv",
                Type = "Demo"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "fi",
                Type = "Test"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "en",
                Type = "Test"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "sv",
                Type = "Test"
            });

            obj.Items.Add(new CustomLanguageObject()
            {
                Language = language,
                Type = type
            });


            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void ValidationShouldHandleNullItem()
        {
            TestObject obj = new TestObject();
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "fi",
                Type = "Demo"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "en",
                Type = "Demo"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "sv",
                Type = "Demo"
            });
            obj.Items.Add(null);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            Action act = () => Validator.ValidateProperty(obj.Items, ctx);
            // edit the expected message when the validator is modified, currently throws exception where the message says Unknown property: [propertyName here]
            act.ShouldThrowExactly<ValidationException>().WithMessage("Null value not allowed in the list.", "List contains null item.");
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "")]
        [InlineData("", null)]
        [InlineData("", "")]
        [InlineData("", "  ")]
        [InlineData("  ", "")]
        public void ConstructorShouldThrowWithInvalidPropertyNames(string localizationPropertyName, string valuePropertyName)
        {
            Action act = () => new LocalizedListPropertyDuplicityForbiddenAttribute(valuePropertyName, localizationPropertyName);

            act.ShouldThrowExactly<ArgumentException>("The property names shouldn't be null, empty string or whistespaces.");
        }

        [Fact]
        public void ShouldHandlePropertyNameValueNull()
        {
            // the validator should handle item value property being null

            TestObject obj = new TestObject();
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "fi",
                Type = "Demo"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "en",
                Type = null
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "sv",
                Type = "Demo"
            });
            obj.Items.Add(null);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            Action act = () => Validator.ValidateProperty(obj.Items, ctx);
            // edit the expected message when the validator is modified, currently throws exception where the message says Unknown property: [propertyName here]
            // this message is misleading as the property is found but its value is null
            // the validator could allow null and handle it, if that is the case then this test needs to be modified accordingly
            act.ShouldThrowExactly<ValidationException>().WithMessage("Null value not allowed in the item property name: Type.", "List contains null value in the Type property.");
        }

        [Fact]
        public void ShouldHandleLocalizationPropertyNameValueNull()
        {
            // the validator should handle item value property being null

            TestObject obj = new TestObject();
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "fi",
                Type = "Demo"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = null,
                Type = "Demo"
            });
            obj.Items.Add(new CustomLanguageObject()
            {
                Language = "sv",
                Type = "Demo"
            });
            obj.Items.Add(null);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            Action act = () => Validator.ValidateProperty(obj.Items, ctx);
            // edit the expected message when the validator is modified, currently throws exception where the message says Unknown property: [propertyName here]
            // this message is misleading as the property is found but its value is null
            // the validator could allow null and handle it, if that is the case then this test needs to be modified accordingly
            act.ShouldThrowExactly<ValidationException>().WithMessage("Null value not allowed in the item property name: Language.", "List contains null value in the Language property.");
        }

        [Fact]
        public void UsedOnWrongTypeShouldThrow()
        {
            TestObject obj = new TestObject();
            obj.UsedOnStringType = "hello";

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "UsedOnStringType";

            Action act = () => Validator.ValidateProperty(obj.UsedOnStringType, ctx);
            act.ShouldThrowExactly<InvalidOperationException>().WithMessage(
                "The validation attribute is used on a property that doesn't implement IList (property name: 'UsedOnStringType').",
                "Because attribute is used on a type that doesn't implement IList.");
        }

        internal class TestObject
        {
            public TestObject()
            {
                Items = new List<CustomLanguageObject>();
                //PropertyNameNotFound = new List<CustomLanguageObject>();
                //AttributePropertyNameNull = new List<CustomLanguageObject>();
                //AttributePropertyNameEmpty = new List<CustomLanguageObject>();
                //AttributePropertyNameWhitespaces = new List<CustomLanguageObject>();
            }

            [LocalizedListPropertyDuplicityForbidden("Type")]
            public List<CustomLanguageObject> Items { get; set; }

            [LocalizedListPropertyDuplicityForbidden("Type")]
            public string UsedOnStringType { get; set; }

            //[LocalizedListPropertyDuplicityForbidden("NotFoundPropertyName")]
            //public List<CustomLanguageObject> PropertyNameNotFound { get; set; }

            //[LocalizedListPropertyDuplicityForbidden(null)]
            //public List<CustomLanguageObject> AttributePropertyNameNull { get; set; }

            //[LocalizedListPropertyDuplicityForbidden("")]
            //public List<CustomLanguageObject> AttributePropertyNameEmpty { get; set; }

            //[LocalizedListPropertyDuplicityForbidden("  ")]
            //public List<CustomLanguageObject> AttributePropertyNameWhitespaces { get; set; }
        }

        internal class CustomLanguageObject
        {
            //default property name in LocalizedListLanguageDuplicityForbidden
            public string Language { get; set; }

            public string Type { get; set; }
        }
    }
}
