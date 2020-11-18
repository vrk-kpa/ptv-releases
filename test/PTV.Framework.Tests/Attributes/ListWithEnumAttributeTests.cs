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
using PTV.Framework.Tests.DummyClasses;
using System.ComponentModel.DataAnnotations;

namespace PTV.Framework.Tests.Attributes
{
    public class ListWithEnumAttributeTests
    {
        [Fact]
        public void EmptyListIsValid()
        {
            TestObject obj = new TestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "EnumValueList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.EnumValueList, ctx, validationResults).Should().BeTrue();

        }

        [Fact]
        public void EnumValuesInStringListAreValid()
        {
            TestObject obj = new TestObject();

            obj.EnumValueList.Add(DummyEnum.Browser.ToString());
            obj.EnumValueList.Add(DummyEnum.Hobby.ToString());
            obj.EnumValueList.Add(DummyEnum.Phone.ToString());

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "EnumValueList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.EnumValueList, ctx, validationResults).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EmptyOrNullValueInEnumValuesStringListIsValid(string specialValue)
        {
            TestObject obj = new TestObject();

            obj.EnumValueList.Add(DummyEnum.Browser.ToString());
            obj.EnumValueList.Add(specialValue);
            obj.EnumValueList.Add(DummyEnum.Phone.ToString());

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "EnumValueList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.EnumValueList, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void NoValidatorIsValidUsingPublicIsValid()
        {
            TestObject obj = new TestObject();
            obj.ValueListNoValidator.Add("ptv");

            ListWithEnumAttribute attr = new ListWithEnumAttribute(null);

            attr.IsValid(obj.ValueListNoValidator).Should().BeTrue();
        }

        [Fact]
        public void NoValidatorIsValidUsingValidationContext()
        {
            TestObject obj = new TestObject();
            obj.ValueListNoValidator.Add("ptv");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "ValueListNoValidator";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.ValueListNoValidator, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ListItemTypeStringIsEnumMember()
        {
            TestObject obj = new TestObject();
            obj.EnumDemoItems.Add(new EnumDemoItem
            {
                ItemType = DummyEnum.Browser.ToString()
            });
            obj.EnumDemoItems.Add(new EnumDemoItem
            {
                ItemType = string.Empty
            });
            obj.EnumDemoItems.Add(new EnumDemoItem
            {
                ItemType = null
            });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "EnumDemoItems";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.EnumDemoItems, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ListItemTypeStringIsEnumMemberWithoutValidationContext()
        {
            TestObject obj = new TestObject();
            obj.EnumDemoItems.Add(new EnumDemoItem
            {
                ItemType = DummyEnum.Browser.ToString()
            });
            obj.EnumDemoItems.Add(new EnumDemoItem
            {
                ItemType = string.Empty
            });
            obj.EnumDemoItems.Add(new EnumDemoItem
            {
                ItemType = null
            });

            ListWithEnumAttribute attr = new ListWithEnumAttribute(typeof(DummyEnum), "ItemType");

            attr.IsValid(obj.EnumDemoItems).Should().BeTrue();
        }

        [Fact]
        public void AttributeUsedOnWrongTypeShouldThrow()
        {
            TestObject obj = new TestObject();
            obj.UsedOnInvalidPropertyType = "something";

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "UsedOnInvalidPropertyType";

            Action act = () => Validator.ValidateProperty(obj.UsedOnInvalidPropertyType, ctx);

            act.Should().ThrowExactly<InvalidOperationException>().WithMessage(
                "The validation attribute is used on a property that doesn't implement IList (property name: 'UsedOnInvalidPropertyType').",
                "Because ListWithEnumAttribute is used on a type that doesn't implement IList.");
        }

        [Fact]
        public void WrongPropertyNameDefinedShouldThrow()
        {
            TestObject obj = new TestObject();
            obj.InvalidPropertyNameItems.Add(new EnumDemoItem
            {
                ItemType = DummyEnum.Browser.ToString()
            });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "InvalidPropertyNameItems";

            Action act = () => Validator.ValidateProperty(obj.InvalidPropertyNameItems, ctx);

            act.Should().ThrowExactly<ArgumentException>().WithMessage(
                "Item doesn't contain property named: 'WrongPropertyName'. (Parameter 'propertyName')",
                "Because EnumDemoItem doesn't have property name: WrongPropertyName.");
        }

        internal class TestObject
        {
            public TestObject()
            {
                EnumValueList = new List<string>();
                ValueListNoValidator = new List<string>();
                EnumDemoItems = new List<EnumDemoItem>();
                InvalidPropertyNameItems = new List<EnumDemoItem>();
            }

            /// <summary>
            /// string list that can contain DummyEnum values
            /// </summary>
            [ListWithEnum(typeof(DummyEnum))]
            public List<string> EnumValueList { get; private set; }

            [ListWithEnum(null)]
            public List<string> ValueListNoValidator { get; private set; }

            [ListWithEnum(typeof(DummyEnum), "ItemType")]
            public List<EnumDemoItem> EnumDemoItems { get; private set; }

            [ListWithEnum(typeof(DummyEnum), "WrongPropertyName")]
            public List<EnumDemoItem> InvalidPropertyNameItems { get; private set; }

            [ListWithEnum(typeof(DummyEnum))]
            public string UsedOnInvalidPropertyType { get; set; }
        }

        internal class EnumDemoItem
        {
            public string ItemType { get; set; }
        }
    }
}
