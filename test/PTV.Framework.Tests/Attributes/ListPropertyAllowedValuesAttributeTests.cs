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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Xunit;

namespace PTV.Framework.Tests.Attributes
{
    public class ListPropertyAllowedValuesAttributeTests
    {
        // Comment: propertyName for the validator can be null, empty string or whitespaces and it is interpreted as no property defined
        // meaning get list item value (and usually that means that it is expected to be a string value)
        // AllowedValuesAttribute always allows null value

        [Fact]
        public void AllowedValuesArrayIsNull()
        {
            Action act = () => new ListPropertyAllowedValuesAttribute();
            act.ShouldThrowExactly<ArgumentNullException>().WithMessage("Allowed values must be defined.*");
        }

        [Fact]
        public void AllowedValuesArrayInitialized()
        {
            Action act = () => new ListPropertyAllowedValuesAttribute(null, new string[] { });
            act.ShouldNotThrow();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void NoValuesSetNullList(string propertyName)
        {
            // nothing to validate because list is null
            ListPropertyAllowedValuesAttribute attr = new ListPropertyAllowedValuesAttribute(propertyName, new string[] { });
            attr.IsValid(null).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void NoValuesSetEmptyList(string propertyName)
        {
            // nothing to validate because list is empty
            ListPropertyAllowedValuesAttribute attr = new ListPropertyAllowedValuesAttribute(propertyName, new string[] { });
            attr.IsValid(new ArrayList()).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void NoValuesSetGenericEmptyList(string propertyName)
        {
            // nothing to validate because list is empty
            ListPropertyAllowedValuesAttribute attr = new ListPropertyAllowedValuesAttribute(propertyName, new string[] { });
            attr.IsValid(new List<string>()).Should().BeTrue();
        }

        [Theory]
        [InlineData("BogusPropertyName")]
        public void PropertyNameInvalidAllowedValuesNull(string propertyName)
        {
            ListPropertyAllowedValuesAttribute attr = new ListPropertyAllowedValuesAttribute(propertyName, new string[] { });

            List<SomeDemoObject> inList = new List<SomeDemoObject>(5);
            inList.Add(new SomeDemoObject());

            Action act = () => attr.IsValid(inList);
            act.ShouldThrowExactly<ArgumentException>().WithMessage($"Item doesn't contain property named: '{propertyName}'.*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void ArrayListNoValuesDefinedToBeAllowedNullValueIsAllowed(string propertyName)
        {
            // the underlying AllowedValuesAttribute validator allows null values
            // the property name is null or empty string (or just whitespaces) this should be true

            ListPropertyAllowedValuesAttribute attr = new ListPropertyAllowedValuesAttribute(propertyName, new string[] { });
            ArrayList inList = new ArrayList(1);
            inList.Add(null);

            attr.IsValid(inList).Should().BeTrue();
        }

        [Fact]
        public void ArrayListNoValuesDefinedToBeAllowedPropertyNameNull()
        {
            ListPropertyDemoObject obj = new ListPropertyDemoObject();
            obj.NoValuesAllowedPropertyNameNull.Add("ptv");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "NoValuesAllowedPropertyNameNull";

            Action act = () => Validator.ValidateProperty(obj.NoValuesAllowedPropertyNameNull, ctx);
            act.ShouldThrowExactly<ValidationException>().WithMessage("'ptv' is not allowed value of 'NoValuesAllowedPropertyNameNull'! Please use one of these: *");
        }

        [Fact]
        public void ArrayListNoValuesDefinedToBeAllowedPropertyNameEmpty()
        {
            ListPropertyDemoObject obj = new ListPropertyDemoObject();
            obj.NoValuesAllowedPropertyNameEmpty.Add("ptv");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "NoValuesAllowedPropertyNameEmpty";

            Action act = () => Validator.ValidateProperty(obj.NoValuesAllowedPropertyNameEmpty, ctx);
            act.ShouldThrowExactly<ValidationException>().WithMessage("'ptv' is not allowed value of 'NoValuesAllowedPropertyNameEmpty'! Please use one of these: *");
        }

        [Fact]
        public void GenericListNoValuesDefinedToBeAllowedPropertyNameNull()
        {
            ListPropertyDemoObject obj = new ListPropertyDemoObject();
            obj.GenericListNoValuesAllowedPropertyNameNull.Add("ptv");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "GenericListNoValuesAllowedPropertyNameNull";

            Action act = () => Validator.ValidateProperty(obj.GenericListNoValuesAllowedPropertyNameNull, ctx);
            act.ShouldThrowExactly<ValidationException>().WithMessage("'ptv' is not allowed value of 'GenericListNoValuesAllowedPropertyNameNull'! Please use one of these: *");
        }

        [Fact]
        public void GenericListNoValuesDefinedToBeAllowedPropertyNameEmpty()
        {
            ListPropertyDemoObject obj = new ListPropertyDemoObject();
            obj.GenericListNoValuesAllowedPropertyNameEmpty.Add("ptv");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "GenericListNoValuesAllowedPropertyNameEmpty";

            Action act = () => Validator.ValidateProperty(obj.GenericListNoValuesAllowedPropertyNameEmpty, ctx);
            act.ShouldThrowExactly<ValidationException>().WithMessage("'ptv' is not allowed value of 'GenericListNoValuesAllowedPropertyNameEmpty'! Please use one of these: *");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void GenericListNoValuesDefinedToBeAllowedNullValueIsAllowed(string propertyName)
        {
            ListPropertyAllowedValuesAttribute attr = new ListPropertyAllowedValuesAttribute(propertyName, new string[] { });
            List<string> inList = new List<string>(1);
            inList.Add(null);

            attr.IsValid(inList).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void ListContainsAllowedStringValues(string propertyName)
        {
            string[] allowedValues = new string[] { "ptv", "vrk", "some other text" };
            ListPropertyAllowedValuesAttribute attr = new ListPropertyAllowedValuesAttribute(propertyName, allowedValues);
            List<string> inList = new List<string>();
            inList.Add("some other text");
            inList.Add("ptv");
            inList.Add(null); // null should be always allowed
            inList.Add("vrk");

            attr.IsValid(inList).Should().BeTrue();
        }

        [Fact]
        public void ListContainsNotAllowedStringValues()
        {
            ListPropertyDemoObject obj = new ListPropertyDemoObject();
            obj.MyStringValues.Add("jee");
            obj.MyStringValues.Add("ptv");
            obj.MyStringValues.Add("quu");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "MyStringValues";

            Action act = () => Validator.ValidateProperty(obj.MyStringValues, ctx);
            act.ShouldThrowExactly<ValidationException>().WithMessage("'jee' is not allowed value of 'MyStringValues'! Please use one of these: 'ptv, vrk, some other text'.*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void ListContainsNotAllowedStringValuesCaseSensitive(string propertyName)
        {
            string[] allowedValues = new string[] { "ptv", "vrk", "some other text" };
            ListPropertyAllowedValuesAttribute attr = new ListPropertyAllowedValuesAttribute(propertyName, allowedValues);
            List<string> inList = new List<string>();
            inList.Add("ptv");
            inList.Add(null); // null should be always allowed
            inList.Add("Ptv");

            List<ValidationAttribute> validationAttributes = new List<ValidationAttribute>();
            validationAttributes.Add(attr);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext ctx = new ValidationContext(inList);

            // need to do validation like this because the implementation uses in this case validationcontext (property name is empty string)
            Validator.TryValidateValue(inList, ctx, validationResults, validationAttributes).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void ListContainsNotAllowedStringValuesPartialMatch(string propertyName)
        {
            string[] allowedValues = new string[] { "ptv", "vrk", "some other text" };
            ListPropertyAllowedValuesAttribute attr = new ListPropertyAllowedValuesAttribute(propertyName, allowedValues);
            List<string> inList = new List<string>();
            inList.Add("some");

            List<ValidationAttribute> validationAttributes = new List<ValidationAttribute>();
            validationAttributes.Add(attr);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext ctx = new ValidationContext(inList);

            // need to do validation like this because the implementation uses in this case validationcontext (property name is empty string)
            Validator.TryValidateValue(inList, ctx, validationResults, validationAttributes).Should().BeFalse();
        }

        #region demo classes

        internal class ListPropertyDemoObject
        {
            public ListPropertyDemoObject()
            {
                NoValuesAllowedPropertyNameNull = new ArrayList(5);
                NoValuesAllowedPropertyNameEmpty = new ArrayList(5);

                GenericListNoValuesAllowedPropertyNameEmpty = new List<string>(5);
                GenericListNoValuesAllowedPropertyNameNull = new List<string>(5);

                MyStringValues = new List<string>(5);
            }

            [ListPropertyAllowedValues(null, new string[] { })]
            public ArrayList NoValuesAllowedPropertyNameNull { get; private set; }

            [ListPropertyAllowedValues("", new string[] { })]
            public ArrayList NoValuesAllowedPropertyNameEmpty { get; private set; }

            [ListPropertyAllowedValues(null, new string[] { })]
            public List<string> GenericListNoValuesAllowedPropertyNameNull { get; private set; }

            [ListPropertyAllowedValues("", new string[] { })]
            public List<string> GenericListNoValuesAllowedPropertyNameEmpty { get; private set; }

            [ListPropertyAllowedValues("", new string[] { "ptv", "vrk", "some other text" })]
            public List<string> MyStringValues { get; private set; }
        }

        #endregion
    }
}
