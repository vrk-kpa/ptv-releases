/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
    public class LocalizedListLanguageDuplicityForbiddenAttributeTests
    {
        [Fact]
        public void ListNullAllowed()
        {
            TestObject obj = new TestObject();
            obj.Languages = null;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Languages";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Languages, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ListEmptyAllowed()
        {
            TestObject obj = new TestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Languages";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Languages, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ListWithSingleLanguageItems()
        {
            TestObject obj = new TestObject();

            obj.Languages.Add(new CustomLanguageObject
            {
                Language = "fi",
                Value = "Suomi"
            });
            obj.Languages.Add(new CustomLanguageObject
            {
                Language = "sv",
                Value = "Suomi"
            });
            obj.Languages.Add(new CustomLanguageObject
            {
                Language = "en",
                Value = "Suomi"
            });
            obj.Languages.Add(new CustomLanguageObject
            {
                Language = string.Empty, // string empty is ok
                Value = "Suomi"
            });
            obj.Languages.Add(new CustomLanguageObject
            {
                Language = "  ", // whitespaces is ok
                Value = "Suomi"
            });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Languages";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Languages, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void StringListWithSingleLanguageItems()
        {
            // works also with string lists, maybe not intended so but...

            TestObject obj = new TestObject();

            obj.StringList.Add("fi");
            obj.StringList.Add("sv");
            obj.StringList.Add("en");
            obj.StringList.Add(string.Empty);
            obj.StringList.Add("  ");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "StringList";

            List<ValidationResult> validationResults = new List<ValidationResult>();
            Action act = () => Validator.TryValidateProperty(obj.StringList, ctx, validationResults);
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void ListWithDuplicateLanguageItem()
        {
            TestObject obj = new TestObject();

            obj.Languages.Add(new CustomLanguageObject
            {
                Language = "fi",
                Value = "Suomi"
            });
            obj.Languages.Add(new CustomLanguageObject
            {
                Language = "sv",
                Value = "Suomi"
            });
            obj.Languages.Add(new CustomLanguageObject
            {
                Language = "sv",
                Value = "Suomi"
            });
            obj.Languages.Add(new CustomLanguageObject
            {
                Language = "en",
                Value = "Suomi"
            });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Languages";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Languages, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void ValidationShouldHandleNullItem()
        {
            TestObject obj = new TestObject();

            obj.Languages.Add(new CustomLanguageObject
            {
                Language = "fi",
                Value = "Suomi"
            });
            obj.Languages.Add(new CustomLanguageObject
            {
                Language = "sv",
                Value = "Suomi"
            });
            obj.Languages.Add(new CustomLanguageObject
            {
                Language = "en",
                Value = "Suomi"
            });
            obj.Languages.Add(null);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Languages";

            Action act = () => Validator.ValidateProperty(obj.Languages, ctx);
            // currently message is Unknown property: Language, when the item is null
            // this confuses the calling code
            act.Should().ThrowExactly<ValidationException>().WithMessage("List item cannot be null.");
        }

        [Fact]
        public void ValidationShouldHandleNullPropertyValue()
        {
            TestObject obj = new TestObject();

            obj.Languages.Add(new CustomLanguageObject
            {
                Language = "fi",
                Value = "Suomi"
            });
            obj.Languages.Add(new CustomLanguageObject
            {
                Language = "sv",
                Value = "Suomi"
            });
            obj.Languages.Add(new CustomLanguageObject
            {
                Language = "en",
                Value = "Suomi"
            });
            obj.Languages.Add(new CustomLanguageObject
            {
                Language = null,
                Value = "Suomi"
            });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Languages";

            Action act = () => Validator.ValidateProperty(obj.Languages, ctx);
            // currently validator returns Unknown property: Language which is not correct, the property is found but the value is null
            // fix the expected message when the validator is fixed
            act.Should().ThrowExactly<ValidationException>().WithMessage("Property value cannot be null.");
        }

        [Fact]
        public void ValidationThrowsOnUnknownPropertyName()
        {
            TestObject obj = new TestObject();

            obj.PropertyNameNotFound.Add(new CustomLanguageObject
            {
                Language = "fi",
                Value = "Suomi"
            });
            obj.PropertyNameNotFound.Add(new CustomLanguageObject
            {
                Language = "sv",
                Value = "Suomi"
            });
            obj.PropertyNameNotFound.Add(new CustomLanguageObject
            {
                Language = "en",
                Value = "Suomi"
            });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "PropertyNameNotFound";

            Action act = () => Validator.ValidateProperty(obj.PropertyNameNotFound, ctx);
            act.Should().ThrowExactly<ArgumentException>().WithMessage("Item doesn't contain property named: 'NotFoundPropertyName'. (Parameter 'propertyName')");
        }

        [Fact]
        public void PropertyNameNullShouldThrow()
        {
            TestObject obj = new TestObject();

            obj.AttributePropertyNameNull.Add(new CustomLanguageObject
            {
                Language = "fi",
                Value = "Suomi"
            });
            obj.AttributePropertyNameNull.Add(new CustomLanguageObject
            {
                Language = "sv",
                Value = "Suomi"
            });
            obj.AttributePropertyNameNull.Add(new CustomLanguageObject
            {
                Language = "en",
                Value = "Suomi"
            });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "AttributePropertyNameNull";

            Action act = () => Validator.ValidateProperty(obj.AttributePropertyNameNull, ctx);
            act.Should().ThrowExactly<ValidationException>().WithMessage("Unknown property: ");
        }

        [Fact]
        public void PropertyNameEmptyShouldThrow()
        {
            TestObject obj = new TestObject();

            obj.AttributePropertyNameEmpty.Add(new CustomLanguageObject
            {
                Language = "fi",
                Value = "Suomi"
            });
            obj.AttributePropertyNameEmpty.Add(new CustomLanguageObject
            {
                Language = "sv",
                Value = "Suomi"
            });
            obj.AttributePropertyNameEmpty.Add(new CustomLanguageObject
            {
                Language = "en",
                Value = "Suomi"
            });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "AttributePropertyNameEmpty";

            Action act = () => Validator.ValidateProperty(obj.AttributePropertyNameEmpty, ctx);
            act.Should().ThrowExactly<ValidationException>().WithMessage("Unknown property: ");
        }

        [Fact]
        public void PropertyNameWhitespacesShouldThrow()
        {
            TestObject obj = new TestObject();

            obj.AttributePropertyNameWhitespaces.Add(new CustomLanguageObject
            {
                Language = "fi",
                Value = "Suomi"
            });
            obj.AttributePropertyNameWhitespaces.Add(new CustomLanguageObject
            {
                Language = "sv",
                Value = "Suomi"
            });
            obj.AttributePropertyNameWhitespaces.Add(new CustomLanguageObject
            {
                Language = "en",
                Value = "Suomi"
            });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "AttributePropertyNameWhitespaces";

            Action act = () => Validator.ValidateProperty(obj.AttributePropertyNameWhitespaces, ctx);
            act.Should().ThrowExactly<ValidationException>().WithMessage("Unknown property:   "); // notice the two whitespaces as the property name
        }

        [Fact]
        public void UsedOnWrongTypeShouldThrow()
        {
            TestObject obj = new TestObject();
            obj.UsedOnStringType = "hello";

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "UsedOnStringType";

            Action act = () => Validator.ValidateProperty(obj.UsedOnStringType, ctx);
            act.Should().ThrowExactly<InvalidOperationException>().WithMessage(
                "The validation attribute is used on a property that doesn't implement IList (property name: 'UsedOnStringType').",
                "Because attribute is used on a type that doesn't implement IList.");
        }

        internal class TestObject
        {
            public TestObject()
            {
                Languages = new List<CustomLanguageObject>();
                PropertyNameNotFound = new List<CustomLanguageObject>();
                AttributePropertyNameNull = new List<CustomLanguageObject>();
                AttributePropertyNameEmpty = new List<CustomLanguageObject>();
                AttributePropertyNameWhitespaces = new List<CustomLanguageObject>();
                StringList = new List<string>();
            }

            [LocalizedListLanguageDuplicityForbidden]
            public List<CustomLanguageObject> Languages { get; set; }

            [LocalizedListLanguageDuplicityForbidden]
            public string UsedOnStringType { get; set; }

            [LocalizedListLanguageDuplicityForbidden("NotFoundPropertyName")]
            public List<CustomLanguageObject> PropertyNameNotFound { get; set; }

            [LocalizedListLanguageDuplicityForbidden(null)]
            public List<CustomLanguageObject> AttributePropertyNameNull { get; set; }

            [LocalizedListLanguageDuplicityForbidden("")]
            public List<CustomLanguageObject> AttributePropertyNameEmpty { get; set; }

            [LocalizedListLanguageDuplicityForbidden("  ")]
            public List<CustomLanguageObject> AttributePropertyNameWhitespaces { get; set; }

            // it also works on stringlists, but it actually shouldn't be used or is not intended to be used
            [LocalizedListLanguageDuplicityForbidden]
            public List<string> StringList { get; set; }
        }

        internal class CustomLanguageObject
        {
            //default property name in LocalizedListLanguageDuplicityForbidden
            public string Language { get; set; }

            public string Value { get; set; }
        }
    }
}
