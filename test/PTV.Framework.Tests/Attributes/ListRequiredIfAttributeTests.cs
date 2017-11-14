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
    public class ListRequiredIfAttributeTests
    {
        [Fact]
        public void StringListNotRequiredNullList()
        {
            // nulllist ok because the property value doesn't match

            ListRequiredIfTestObject obj = new ListRequiredIfTestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "StringList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.StringList, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void StringListRequiredNullList()
        {
            // list required because property value matches to value that indicates the list is needed

            ListRequiredIfTestObject obj = new ListRequiredIfTestObject();
            obj.StringListIfValue = "ptv";

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "StringList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.StringList, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void StringListNotRequiredEmptyList()
        {
            // empty list ok because the property value doesn't match to what is needed for the list to be required

            ListRequiredIfTestObject obj = new ListRequiredIfTestObject();
            obj.StringList = new List<string>();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "StringList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.StringList, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void StringListRequiredEmptyList()
        {
            // empty list is not valid when the property value matches the value given in the attribute

            ListRequiredIfTestObject obj = new ListRequiredIfTestObject();
            obj.StringListIfValue = "ptv";
            obj.StringList = new List<string>();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "StringList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.StringList, ctx, validationResults).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RequiredListCannotHaveEmptyOrNullValues(string extraValue)
        {
            ListRequiredIfTestObject obj = new ListRequiredIfTestObject();
            obj.StringListIfValue = "ptv";
            obj.StringList = new List<string>() {"some", "value", extraValue};

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "StringList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.StringList, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void NoValidationContextShouldNotCauseInternalNullReferenceException()
        {
            // the current implementation crashes if the public IsValid(object) is called
            // anyways the attribute can't really be used in this way as it dependant on another property on the object
            // but it shouldn't crash, it should check that it has the validationcontext before using it

            ListRequiredIfAttribute attr = new ListRequiredIfAttribute("StringListIfValue", "ptv");

            ListRequiredIfTestObject obj = new ListRequiredIfTestObject();
            obj.StringListIfValue = "ptv";
            obj.StringList = new List<string>() {"vrk"};

            Action act = () => attr.IsValid(obj);
            act.ShouldThrowExactly<InvalidOperationException>("validationContext cannot be null.");
        }

        // TODO: public IsValid and protected IsValid result compare test

        [Fact]
        public void ShouldThrowInvalidPropertyTypeUsage()
        {
            // Comment: should throw an exception that indicates that the property is used on a wrong property type
            // instead just saying that the value is not valid

            var obj = new ListRequiredIfTestObject();
            obj.SomeDictionary.Add("ptv", "vrk");

            var ctx = new ValidationContext(obj) {MemberName = "SomeDictionary"};

            var validationResults = new List<ValidationResult>();

            Action act = () => Validator.TryValidateProperty(obj.SomeDictionary, ctx, validationResults);

            // currently throws validation exception because dictionary doesn't implement ilist
            // the implementation should throw exception other than validation exception and indicate what is wrong
            act.ShouldThrowExactly<InvalidOperationException>("Attribute used on a property that doesn't implement IList.");
        }

        [Fact]
        public void ShouldThrowInvalidPropertyNameDefined()
        {
            ListRequiredIfTestObject obj = new ListRequiredIfTestObject();
            obj.DemoItems = new List<SomeDemoObject>();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "DemoItems";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Action act = () => Validator.TryValidateProperty(obj.DemoItems, ctx, validationResults);

            // the attribute should indicate that the desired property was not found and not just silently return success
            // case: some property has value hsl and in that case the list should contain values
            // now if there is a typo in the property name, no values are required in the list by the validation
            act.ShouldThrowExactly<InvalidOperationException>().WithMessage("Property 'TypoInPropertyName' not found.");
        }

        [Fact]
        public void PropertyNameIsNull()
        {
            var obj = new ListRequiredIfNullPropertyTestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "PropertyNameNullList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Action act = () => Validator.TryValidateProperty(obj.PropertyNameNullList, ctx, validationResults);

            // currently throw with param name: name because the implementation is not handling this but the exception is thrown from reflection
            act.ShouldThrowExactly<ArgumentNullException>().WithMessage("Value cannot be null.\\r\\nParameter name: propertyName");
        }

        [Fact]
        public void PropertyNameIsEmpty()
        {
            var obj = new ListRequiredIfEmptyPropertyTestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "PropertyNameEmptyList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Action act = () => Validator.TryValidateProperty(obj.PropertyNameEmptyList, ctx, validationResults);

            // currently doesn't throw, it would be good if the constructor already would throw for invalid values
            act.ShouldThrowExactly<ArgumentException>().WithMessage("Value cannot be empty. Parameter name: propertyName");
        }

        [Fact]
        public void PropertyNameStringListContainsNullAndEmptyString()
        {
            // the RequiredListIfPropertyValueList is required because value 'vrk' is in the PropertyValueList

            ListRequiredIfTestObject obj = new ListRequiredIfTestObject();
            obj.PropertyValueList.Add("ptv");
            obj.PropertyValueList.Add("hsl");
            obj.PropertyValueList.Add(null); // should be fine to have a null value in the list
            obj.PropertyValueList.Add(string.Empty); // should be fine to have an empty string
            obj.PropertyValueList.Add("vrk");

            obj.RequiredListIfPropertyValueList.Add("demo value");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "RequiredListIfPropertyValueList";

            List<ValidationResult> validationResults = new List<ValidationResult>();
            // should be true because we have one value in the RequiredListIfPropertyValueList -list
            Validator.TryValidateProperty(obj.RequiredListIfPropertyValueList, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ListRequiredIfPropertyValueIsNull()
        {
            ListRequiredIfTestObject obj = new ListRequiredIfTestObject();
            obj.StringListIfValue = null;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "ReqListIfPropertyIsNull";

            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateProperty(obj.ReqListIfPropertyIsNull, ctx, validationResults).Should().BeFalse();
        }

        // TODO: property value in a list test (string list)
        // test what happens when the property list contains objects
        // test where property value list contains null value
        // test where desired value is null

        #region test objects

        internal class ListRequiredIfTestObject
        {
            public ListRequiredIfTestObject()
            {
                SomeDictionary = new Dictionary<string, string>();
                RequiredListIfPropertyValueList = new List<string>();
                PropertyValueList = new List<string>();
                ReqListIfPropertyIsNull = new List<string>();
            }

            [ListRequiredIf("StringListIfValue", "ptv")]
            public List<string> StringList { get; set; }

            public string StringListIfValue { get; set; }

            [ListRequiredIf("PropertyValueList", "vrk")]
            public Dictionary<string, string> SomeDictionary { get; set; }

            [ListRequiredIf("TypoInPropertyName", "hsl")]
            public List<SomeDemoObject> DemoItems { get; set; }

            [ListRequiredIf("PropertyValueList", "vrk")]
            public List<string> RequiredListIfPropertyValueList { get; set; }

            public List<string> PropertyValueList { get; set; }

            [ListRequiredIf("StringListIfValue", null)]
            public List<string> ReqListIfPropertyIsNull { get; set; }
        }

        internal class ListRequiredIfNullPropertyTestObject
        {
            [ListRequiredIf(null, "hsl")]
            public List<string> PropertyNameNullList { get; set; }
        }

        internal class ListRequiredIfEmptyPropertyTestObject
        {
            [ListRequiredIf("", "hsl")]
            public List<string> PropertyNameEmptyList { get; set; }
        }

        #endregion
    }
}
