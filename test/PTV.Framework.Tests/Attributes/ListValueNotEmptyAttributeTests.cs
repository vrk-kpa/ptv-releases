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
using PTV.Framework.Tests.DummyClasses;

namespace PTV.Framework.Tests.Attributes
{
    public class ListValueNotEmptyAttributeTests
    {
        [Fact]
        public void EmptyListIsOk()
        {
            TestObject to = new TestObject();

            ValidationContext ctx = new ValidationContext(to);
            ctx.MemberName = "StringList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(to.StringList, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ValidStringsAreOk()
        {
            // allows null, empty string and whitespace string
            // string needs to be more than 5 chars long after validator calling Trim()
            TestObject to = new TestObject();
            to.StringList.Add("ptv-vrk");
            to.StringList.Add("  viisi");
            to.StringList.Add("  viisi  ");
            to.StringList.Add(null);
            to.StringList.Add(string.Empty);
            to.StringList.Add("            ");
            to.StringList.Add("viisi  ");
            to.StringList.Add("ptv 5");
            to.StringList.Add("p t v");

            ValidationContext ctx = new ValidationContext(to);
            ctx.MemberName = "StringList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(to.StringList, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ValidObjectListStringsAreOk()
        {
            // allows null, empty string and whitespace string
            // string needs to be more than 5 chars long after validator calling Trim()
            TestObject to = new TestObject();

            to.ObjectList.Add(new SomeDemoObject
            {
                String = "ptv-vrk"
            });
            to.ObjectList.Add(new SomeDemoObject
            {
                String = "  viisi"
            });
            to.ObjectList.Add(new SomeDemoObject
            {
                String = "  viisi  "
            });
            to.ObjectList.Add(new SomeDemoObject
            {
                String = null
            });
            to.ObjectList.Add(new SomeDemoObject
            {
                String = string.Empty
            });
            to.ObjectList.Add(new SomeDemoObject
            {
                String = "            "
            });
            to.ObjectList.Add(new SomeDemoObject
            {
                String = "viisi  "
            });
            to.ObjectList.Add(new SomeDemoObject
            {
                String = "ptv 5"
            });
            to.ObjectList.Add(new SomeDemoObject
            {
                String = "p t v"
            });

            ValidationContext ctx = new ValidationContext(to);
            ctx.MemberName = "ObjectList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(to.ObjectList, ctx, validationResults).Should().BeTrue();
        }

        [Theory]
        [InlineData(" four")]
        [InlineData("four ")]
        [InlineData("four")]
        [InlineData("a")]
        [InlineData("ab")]
        [InlineData("abc")]
        [InlineData("ab a")]
        public void LessThanFiveCharsInAString(string testValue)
        {
            TestObject to = new TestObject();
            to.StringList.Add(testValue);

            ValidationContext ctx = new ValidationContext(to);
            ctx.MemberName = "StringList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(to.StringList, ctx, validationResults).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        [InlineData("     ")]
        [InlineData("            ")]
        public void SpecialAllowedValues(string testValue)
        {
            TestObject to = new TestObject();
            to.StringList.Add(testValue);

            ValidationContext ctx = new ValidationContext(to);
            ctx.MemberName = "StringList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(to.StringList, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void PublicAndProtectedIsValidShouldReturnSameResultForStrings()
        {
            TestObject obj = new TestObject();
            obj.StringList.Add("more than five");
            obj.StringList.Add(null);
            obj.StringList.Add(string.Empty);
            obj.StringList.Add("        ");

            bool publicResult = new ListValueNotEmptyAttribute().IsValid(obj.StringList);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "StringList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.StringList, ctx, validationResults).Should().Be(publicResult, "Public and protected IsValid methods shouild have the same result.");
        }

        [Fact]
        public void InvalidPropertyNameShouldThrow()
        {
            TestObject obj = new TestObject();
            obj.InvalidPropertyNameList.Add(new SomeDemoObject { });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "InvalidPropertyNameList";

            Action act = () => Validator.ValidateProperty(obj.InvalidPropertyNameList, ctx);

            act.Should().ThrowExactly<ArgumentException>().WithMessage(
                "Item doesn't contain property named: 'NotFoundPropertyName'. (Parameter 'propertyName')",
                "Because SomeDemoObject doesn't have a property called: NotFoundPropertyName.");
        }

        internal class TestObject
        {
            public TestObject()
            {
                StringList = new List<string>();
                ObjectList = new List<SomeDemoObject>();
                InvalidPropertyNameList = new List<SomeDemoObject>();
            }

            [ListValueNotEmpty]
            public List<string> StringList { get; private set; }

            [ListValueNotEmpty("String")]
            public List<SomeDemoObject> ObjectList { get; private set; }

            [ListValueNotEmpty("NotFoundPropertyName")]
            public List<SomeDemoObject> InvalidPropertyNameList { get; private set; }
        }
    }
}
