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
    public class ListRequiredIfPropertyTests
    {
        [Fact]
        public void ItemsContainObjectWithLinkedPropertyValue()
        {
            TestObject obj = new TestObject();
            obj.LinkedPropertyForItems = "ptv"; // set that this value should be matched in validation
            obj.Items.Add(new SomeDemoObject() { Integer = 1, String = "vrk", Type = "demo" });
            obj.Items.Add(new SomeDemoObject() { Integer = 2, String = "ptv", Type = "demo" });
            obj.Items.Add(new SomeDemoObject() { Integer = 3, String = "hsl", Type = "demo" });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void NullValueInListShouldMakeTheCollectionInvalid()
        {
            TestObject obj = new TestObject();
            obj.LinkedPropertyForItems = "ptv"; // set that this value should be matched in validation
            obj.Items.Add(new SomeDemoObject() { Integer = 1, String = "vrk", Type = "demo" });
            obj.Items.Add(null);
            obj.Items.Add(new SomeDemoObject() { Integer = 3, String = "ptv", Type = "demo" });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            Action act = () => Validator.ValidateProperty(obj.Items, ctx);

            act.Should().ThrowExactly<ValidationException>().WithMessage("The item with 'String = ptv' is required when 'LinkedPropertyForItems' has value 'ptv'.");
        }

        [Fact]
        public void NullItemPropertyValueInListShouldNotCrash()
        {
            TestObject obj = new TestObject();
            obj.LinkedPropertyForItems = "ptv"; // set that this value should be matched in validation
            obj.Items.Add(new SomeDemoObject() { Integer = 1, String = "vrk", Type = "demo" });
            obj.Items.Add(new SomeDemoObject() { Integer = 3, String = null, Type = "demo" }); // here is null, implementation code does null.ToString()
            obj.Items.Add(new SomeDemoObject() { Integer = 3, String = "ptv", Type = "demo" });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Action act = () => Validator.TryValidateProperty(obj.Items, ctx, validationResults);

            act.Should().NotThrow("The validator should handle the item property value being null.");
        }

        [Fact]
        public void NullLinkedPropertyValueMakesTheWholeCollectionValid()
        {
            TestObject obj = new TestObject();
            obj.LinkedPropertyForItems = null; // if linked property value is null, no validation is done
            obj.Items.Add(new SomeDemoObject() { Integer = 1, String = "vrk", Type = "demo" });
            obj.Items.Add(null); // normally this would make the colleciton invalid
            obj.Items.Add(new SomeDemoObject() { Integer = 3, String = "hsl", Type = "demo" });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void NullListIsNeverValid()
        {
            TestObject obj = new TestObject();
            obj.LinkedPropertyForItems = "ptv"; // set that this value should be matched in validation
            obj.Items = null;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeFalse();
//            Action act = () => Validator.TryValidateProperty(obj.Items, ctx, validationResults);
//            act.Should().ThrowExactly<InvalidOperationException>("Attribute used on something that doesn't implement IList should throw an exception.");
        }      
        
        [Fact]
        public void EmptyListIsNeverValid()
        {
            TestObject obj = new TestObject();
            obj.LinkedPropertyForItems = "ptv"; // set that this value should be matched in validation
            obj.Items = new List<SomeDemoObject>();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Items";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Items, ctx, validationResults).Should().BeFalse();
//            act.Should().ThrowExactly<InvalidOperationException>("Attribute used on something that doesn't implement IList should throw an exception.");
        }

        [Fact]
        public void InvalidPropertyTypeShouldThrow()
        {
            TestObject obj = new TestObject();
            obj.LinkedPropertyForItems = "ptv";
            obj.UsedOnInvalidPropertyType.Add("key", "value");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "UsedOnInvalidPropertyType";

            Action act = () => Validator.ValidateProperty(obj.UsedOnInvalidPropertyType, ctx);

            act.Should().ThrowExactly<InvalidOperationException>("Attribute used on something that doesn't implement IList should throw an exception.");
        }

        [Fact]
        public void ItemPropertyWithNameNotFoundShouldThrow()
        {
            // item doesn't contain the named property

            TestObject obj = new TestObject();
            obj.LinkedPropertyForItems = "ptv"; // set that this value should be matched in validation
            obj.TypoInItemPropertyNameList.Add(new SomeDemoObject() { Integer = 1, String = "vrk", Type = "demo" });
            obj.TypoInItemPropertyNameList.Add(new SomeDemoObject() { Integer = 2, String = "ptv", Type = "demo" });
            obj.TypoInItemPropertyNameList.Add(new SomeDemoObject() { Integer = 3, String = "hsl", Type = "demo" });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "TypoInItemPropertyNameList";

            Action act = () => Validator.ValidateProperty(obj.TypoInItemPropertyNameList, ctx);

            act.Should().ThrowExactly<InvalidOperationException>("The attribute is misconfigured with wrong item property name (typo in attribute: 'Strin')");
        }

        [Fact]
        public void ValidationContextMissing()
        {
            ListRequiredIfProperty attr = new ListRequiredIfProperty("String", "Nope");

            TestObject obj = new TestObject();
            obj.LinkedPropertyForItems = "ptv";
            obj.Items.Add(new SomeDemoObject() { Integer = 1, String = "vrk", Type = "demo" });
            obj.Items.Add(new SomeDemoObject() { Integer = 2, String = "ptv", Type = "demo" });
            obj.Items.Add(new SomeDemoObject() { Integer = 3, String = "hsl", Type = "demo" });

            Action act = () => attr.IsValid(obj);

            // the public IsValid method really can't be used because the code requires ValidationContext

            act.Should().ThrowExactly<ArgumentNullException>("validationContext");
        }

        internal class TestObject
        {
            public TestObject()
            {
                Items = new List<SomeDemoObject>();
                UsedOnInvalidPropertyType = new Dictionary<string, string>();
                TypoInItemPropertyNameList = new List<SomeDemoObject>();
            }

            public string LinkedPropertyForItems { get; set; }

            // item in list should have the same value in String property as is defined in LinkedPropertyForItems value
            [ListRequiredIfProperty("String", "LinkedPropertyForItems")]
            public List<SomeDemoObject> Items { get; set; }

            [ListRequiredIfProperty("String", "LinkedPropertyForItems")]
            public Dictionary<string,string> UsedOnInvalidPropertyType { get; set; }

            // typo in property name on purpose 'Strin'
            [ListRequiredIfProperty("Strin", "LinkedPropertyForItems")]
            public List<SomeDemoObject> TypoInItemPropertyNameList { get; set; }
        }
    }
}
