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
    public class ListRequiredToMatchDisplayNameTypesTests
    {
        [Fact]
        public void ValidationContextMissing()
        {
            ListRequiredToMatchDisplayNameTypes attr = new ListRequiredToMatchDisplayNameTypes("LinkedListPropertyForItems");

            TestObject obj = new TestObject();

            Action act = () => attr.IsValid(obj);

            // the public IsValid method really can't be used because the code requires ValidationContext

            act.ShouldThrowExactly<InvalidOperationException>("The code shouldn't crash to a NullReferenceException.");
        }

        [Fact]
        public void BothListAreNull()
        {
            TestObject obj = new TestObject();
            obj.Items = null;
            obj.LinkedListForItems = null;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            // ok that both lists are null
            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void BothListAreEmpty()
        {
            TestObject obj = new TestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            // ok that both lists are null
            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ItemsListIsNullAndLinkedListIsEmpty()
        {
            TestObject obj = new TestObject();
            obj.Items = null;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            // ok that linkeditemslist is empty and items list is null
            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void LinkedListIsNullAndItemsListIsEmpty()
        {
            TestObject obj = new TestObject();
            obj.LinkedListForItems = null;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            // ok that linkeditemslist is null and items list is empty
            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ListValuesMatch()
        {
            TestObject obj = new TestObject();

            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hello", Language = "en", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hei", Language = "fi", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hej", Language = "sv", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "world", Language = "en", Type = "test" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "maailma", Language = "fi", Type = "test" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "världen", Language = "sv", Type = "test" });

            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "en", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "fi", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "sv", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "en", Type = "test" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "fi", Type = "test" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "sv", Type = "test" });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void MatchingTypeAndLanguageObjectMissingFromItemsList()
        {
            TestObject obj = new TestObject();

            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hello", Language = "en", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hei", Language = "fi", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hej", Language = "sv", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "world", Language = "en", Type = "test" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "maailma", Language = "fi", Type = "test" });

            // this item is now missing from the list, the linked list tries to match item with language=sv and type=test
            //obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "världen", Language = "sv", Type = "test" });

            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "en", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "fi", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "sv", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "en", Type = "test" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "fi", Type = "test" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "sv", Type = "test" });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            Action act = () => Validator.ValidateProperty(obj.Items, ctx);
            act.ShouldThrowExactly<ValidationException>().WithMessage("All language/type pairs defined in LinkedListForItems must be specified. Missing: sv/test");
        }

        [Fact]
        public void LinkedListLanguageMissingFromItemsList()
        {
            TestObject obj = new TestObject();

            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hello", Language = "en", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hei", Language = "fi", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "world", Language = "en", Type = "test" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "maailma", Language = "fi", Type = "test" });

            // no items with sv language in Items list

            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "en", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "fi", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "sv", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "en", Type = "test" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "fi", Type = "test" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "sv", Type = "test" });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            // sv language missing from items list

            Action act = () => Validator.ValidateProperty(obj.Items, ctx);
            act.ShouldThrowExactly<ValidationException>().WithMessage("All language/type pairs defined in LinkedListForItems must be specified. Missing: sv/demo");
        }

        [Fact]
        public void LinkedListTypeMissingFromItemsList()
        {
            TestObject obj = new TestObject();

            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "world", Language = "en", Type = "test" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "maailma", Language = "fi", Type = "test" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "världen", Language = "sv", Type = "test" });

            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "en", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "fi", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "sv", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "en", Type = "test" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "fi", Type = "test" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "sv", Type = "test" });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            Action act = () => Validator.ValidateProperty(obj.Items, ctx);
            act.ShouldThrowExactly<ValidationException>().WithMessage("All language/type pairs defined in LinkedListForItems must be specified. Missing: en/demo");
        }

        [Fact]
        public void NullObjectInLinkedList()
        {
            // should the null be skipped in linkedlist because now a null value in the items list is required
            // now leads to the fact that the items list needs to have an iten with language == null && type == null

            TestObject obj = new TestObject();

            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hello", Language = "en", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hei", Language = "fi", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hej", Language = "sv", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "world", Language = "en", Type = "test" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "maailma", Language = "fi", Type = "test" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "världen", Language = "sv", Type = "test" });

            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "en", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "fi", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "sv", Type = "demo" });
            obj.LinkedListForItems.Add(null); // null item in linkedlist
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "en", Type = "test" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "fi", Type = "test" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "sv", Type = "test" });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            Action act = () => Validator.ValidateProperty(obj.Items, ctx);
            act.ShouldThrowExactly<InvalidOperationException>("There is a null item in linked list.");
        }

        [Fact]
        public void NullObjectInItemList()
        {
            TestObject obj = new TestObject();

            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hello", Language = "en", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hei", Language = "fi", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hej", Language = "sv", Type = "demo" });
            obj.Items.Add(null);
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "world", Language = "en", Type = "test" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "maailma", Language = "fi", Type = "test" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "världen", Language = "sv", Type = "test" });

            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "en", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "fi", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "sv", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "en", Type = "test" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "fi", Type = "test" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "sv", Type = "test" });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            // in implementation the null item is considered as null language and the linkeditemlist doesn't contain null language the validation fails

            Action act = () => Validator.ValidateProperty(obj.Items, ctx);
            act.ShouldThrowExactly<InvalidOperationException>("There is a null item in items list.");
        }

        [Fact]
        public void ItemListCannotHaveOtherLanguagesThanDefinedInLinkedList()
        {
            TestObject obj = new TestObject();

            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hello", Language = "en", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hei", Language = "fi", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hej", Language = "sv", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "heyhey", Language = "de", Type = "other" }); // de language doesn't exist in linkedlist
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "world", Language = "en", Type = "test" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "maailma", Language = "fi", Type = "test" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "världen", Language = "sv", Type = "test" });

            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "en", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "fi", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "sv", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "en", Type = "test" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "fi", Type = "test" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "sv", Type = "test" });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();
            // should be false because de language doesn't exist in linkedlist
            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void ItemListCanHaveOtherTypesThanDefinedInLinkedList()
        {
            TestObject obj = new TestObject();

            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hello", Language = "en", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hei", Language = "fi", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "hej", Language = "sv", Type = "demo" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "heyhey", Language = "fi", Type = "other" }); // must use language that exists in linked list
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "world", Language = "en", Type = "test" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "maailma", Language = "fi", Type = "test" });
            obj.Items.Add(new DemoItemWithLanguageAndType() { CustomString = "världen", Language = "sv", Type = "test" });

            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "en", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "fi", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "sv", Type = "demo" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "en", Type = "test" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "fi", Type = "test" });
            obj.LinkedListForItems.Add(new LanguageAndType() { Language = "sv", Type = "test" });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();
            // other types but with language existing in linkedlist is allowed
            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void UsedOnWrongPropertyTypeShouldThrow()
        {
            TestObject obj = new TestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "InvalidPropertTypeDict";

            Action act = () => Validator.ValidateProperty(obj.InvalidPropertTypeDict, ctx);

            act.ShouldThrowExactly<InvalidOperationException>("Attribute used on a type that doesn't implement IList.");
        }

        [Fact]
        public void ValidationAttributeTargetsWrongLinkedListType()
        {
            TestObject obj = new TestObject();
            obj.NotAList = "hello world";

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "TargetsWrongTypeProperty";

            Action act = () => Validator.ValidateProperty(obj.TargetsWrongTypeProperty, ctx);

            act.ShouldThrowExactly<InvalidOperationException>("Attribute configured to target a linked list property type that doesn't implement IList.");
        }

        internal class TestObject
        {
            public TestObject()
            {
                Items = new List<DemoItemWithLanguageAndType>();
                LinkedListForItems = new List<LanguageAndType>();

                InvalidPropertTypeDict = new Dictionary<int, LanguageAndType>();
                TargetsWrongTypeProperty = new List<DemoItemWithLanguageAndType>();
            }

            public List<LanguageAndType> LinkedListForItems { get; set; }

            // item in list should have the same value in String property as is defined in LinkedPropertyForItems value
            [ListRequiredToMatchDisplayNameTypes("LinkedListForItems")]
            public List<DemoItemWithLanguageAndType> Items { get; set; }


            // attribute apllied to wrong kind pf property type, should always be IList
            [ListRequiredToMatchDisplayNameTypes("LinkedListForItems")]
            public Dictionary<int, LanguageAndType> InvalidPropertTypeDict { get; set; }

            [ListRequiredToMatchDisplayNameTypes("NotAList")]
            public List<DemoItemWithLanguageAndType> TargetsWrongTypeProperty { get; set; }

            public string NotAList { get; set; }
        }

        internal class DemoItemWithLanguageAndType : LanguageAndType
        {
            public string CustomString { get; set; }
        }
    }
}
