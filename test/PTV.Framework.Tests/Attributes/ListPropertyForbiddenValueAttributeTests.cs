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
    public class ListPropertyForbiddenValueAttributeTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "")]
        [InlineData("", null)]
        [InlineData("", "")]
        public void ConstructorDoesntThrow(string forbiddenValue, string propertyName)
        {
            Action act = () => new ListPropertyForbiddenValueAttribute(forbiddenValue, propertyName);
            act.Should().NotThrow();
        }

        [Fact]
        public void UsedOnWrongTypeShouldThrow()
        {
            // the attribute should be used only types that implement IList
            // so if the attribute is used on an incorrect type the developer should get feedback and not say everything is ok
            ListPropertyForbiddenValueAttribute attr = new ListPropertyForbiddenValueAttribute("vrk");

            Action act = () => attr.IsValid("NotAnIList");

            act.Should().Throw<Exception>("Because attribute is used on a wrong type (should be used only on types that implement IList).");
        }

        [Fact]
        public void ForbiddenValueInList()
        {
            ListPropertyForbiddenValueAttribute attr = new ListPropertyForbiddenValueAttribute("vrk");

            List<string> inValues = new List<string>(4)
            {
                "ptv", "question", " ", "vrk"
            };

            attr.IsValid(inValues).Should().BeFalse();
        }

        [Fact]
        public void ForbiddenValueInCustomObjectList()
        {
            ListPropertyForbiddenValueAttribute attr = new ListPropertyForbiddenValueAttribute("vrk", "String");

            List<SomeDemoObject> inValues = new List<SomeDemoObject>()
            {
                new SomeDemoObject()
                {
                    Integer = 10,
                    String = "text"
                },
                new SomeDemoObject()
                {
                    Integer = 100,
                    String = "vrk"
                }
            };

            attr.IsValid(inValues).Should().BeFalse();
        }

        [Fact]
        public void AllowOnlyStringsInList()
        {
            ListPropertyForbiddenValueAttribute attr = new ListPropertyForbiddenValueAttribute("vrk");

            List<int> inValues = new List<int>(3) { 10, 100, 151 };

            attr.IsValid(inValues).Should().BeFalse();
        }

        [Fact]
        public void NullListIsValidationSuccess()
        {
            ListPropertyForbiddenValueAttribute attr = new ListPropertyForbiddenValueAttribute("vrk");
            attr.IsValid(null).Should().BeTrue();
        }

        [Fact]
        public void EmptyListIsValidationSuccess()
        {
            ListPropertyForbiddenValueAttribute attr = new ListPropertyForbiddenValueAttribute("vrk");
            attr.IsValid(new List<string>()).Should().BeTrue();
        }

        [Fact]
        public void NullValueInListIsNotValid()
        {
            // null values are not allowed in list
            ListPropertyForbiddenValueAttribute attr = new ListPropertyForbiddenValueAttribute("vrk");

            List<string> inValues = new List<string>()
            {
                "ptv", "some long string", null
            };

            attr.IsValid(inValues).Should().BeFalse();
        }

        [Fact]
        public void NullValueInListThrowsUnknownPropertyValidationException()
        {
            ForbiddenTestObject obj = new ForbiddenTestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "ContainsNullValue";

            Action act = () => Validator.ValidateProperty(obj.ContainsNullValue, ctx);
            act.Should().ThrowExactly<ValidationException>().WithMessage("Unknown property:*");
        }

        [Fact]
        public void ForbiddenValueInListThrowsNotAllowedValueValidationException()
        {
            ForbiddenTestObject obj = new ForbiddenTestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "ContainsForbiddenValue";

            Action act = () => Validator.ValidateProperty(obj.ContainsForbiddenValue, ctx);
            act.Should().ThrowExactly<ValidationException>().WithMessage("*is not allowed value*");
        }

        [Fact]
        public void CustomObjectPropertyNotStringThrowsValidationException()
        {
            ForbiddenTestObject obj = new ForbiddenTestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "DemoObjects";

            Action act = () => Validator.ValidateProperty(obj.DemoObjects, ctx);
            act.Should().ThrowExactly<ValidationException>().WithMessage("Unknown property:*");
        }

        #region testing objects

        internal class ForbiddenTestObject
        {
            public ForbiddenTestObject()
            {
                ContainsNullValue = new List<string>() { "ptv", "some string", null };
                ContainsForbiddenValue = new List<string>() { "ptv", "some string", "test" };
                DemoObjects = new List<SomeDemoObject>()
                {
                    new SomeDemoObject()
                    {
                        Integer = 100,
                        String = "jep"
                    }
                };
            }

            [ListPropertyForbiddenValue(null)]
            public List<string> ContainsNullValue { get; private set; }

            [ListPropertyForbiddenValue("test")]
            public List<string> ContainsForbiddenValue { get; private set; }

            [ListPropertyForbiddenValue("test", "Integer")]
            public List<SomeDemoObject> DemoObjects { get; set; }
        }

        #endregion
    }
}
