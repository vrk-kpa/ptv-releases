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
    public class ListRegularExpressionAttributeTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("+?'-)&")]
        public void ConstructorDoesntThrowWithInvalidPattern(string invalidPattern)
        {
            // regex attribute throws in IsValid
            // but it would be better to get sooner the information about invalid pattern so it would be good if the constructor would throw
            // because the pattern cannot be changed after creation

            Action act = () => new ListRegularExpressionAttribute(invalidPattern);
            act.ShouldNotThrow();
        }

        [Fact]
        public void NullListIsValid()
        {
            ListRegularExpressionAttribute attr = new ListRegularExpressionAttribute(@"^\d+$");
            attr.IsValid(null).Should().BeTrue();
        }

        [Fact]
        public void NullListIsValidProtectedIsValid()
        {
            RegExpDemoObject obj = new RegExpDemoObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "NullList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.NullList, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void EmptyListIsValid()
        {
            ListRegularExpressionAttribute attr = new ListRegularExpressionAttribute(@"^\d+$");
            attr.IsValid(new List<string>()).Should().BeTrue();
        }

        [Fact]
        public void EmptyListIsValidProtectedIsValid()
        {
            RegExpDemoObject obj = new RegExpDemoObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "DemoStrings";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.DemoStrings, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void PublicIsValidAndProtectedIsValidShouldUseSameImplementationEmptyList()
        {
            ListRegularExpressionAttribute attr = new ListRegularExpressionAttribute(@"^\d+$");
            // actually currently what happens is that list is converted to string and it doesn't match the numeric string pattern
            bool publicIsValid = attr.IsValid(new List<string>());

            RegExpDemoObject obj = new RegExpDemoObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "DemoStrings";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.DemoStrings, ctx, validationResults).Should().Be(publicIsValid);
        }

        [Fact]
        public void PublicIsValidAndProtectedIsValidShouldUseSameImplementation()
        {
            ListRegularExpressionAttribute attr = new ListRegularExpressionAttribute(@"^\d+$");
            // actually currently what happens is that list is converted to string and it doesn't match the numeric string pattern
            bool publicIsValid = attr.IsValid(new List<string>() { "555", "1", "2017" });

            RegExpDemoObject obj = new RegExpDemoObject();
            obj.DemoStrings.Add("555");
            obj.DemoStrings.Add("1");
            obj.DemoStrings.Add("2017");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "DemoStrings";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.DemoStrings, ctx, validationResults).Should().Be(publicIsValid);
        }

        [Fact]
        public void ShouldThrowInvalidPropertyTypeUsage()
        {
            RegExpDemoObject obj = new RegExpDemoObject();
            // this needs to be different from null so that the TryValidateProperty doesn't throw ArgumentNullException
            obj.InvalidPropertyUsage = "some string";

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "InvalidPropertyUsage";

            Action act = () => Validator.ValidateProperty(obj.InvalidPropertyUsage, ctx);

            act.ShouldThrow<InvalidOperationException>("Attribute used on a property that doesn't implement IList.");
        }

        [Fact]
        public void ListContainsValidNumericStrings()
        {
            RegExpDemoObject obj = new RegExpDemoObject();
            obj.DemoStrings.Add("555");
            obj.DemoStrings.Add("1");
            obj.DemoStrings.Add("2017");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "DemoStrings";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.DemoStrings, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ListContainsValidThreeLetterStrings()
        {
            RegExpDemoObject obj = new RegExpDemoObject();

            obj.ThreeLetterItems = new List<string>()
            {
                "ana",
                "ptv",
                "vrk"
            };

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "ThreeLetterItems";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.ThreeLetterItems, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ListContainsInvalidThreeLetterStrings()
        {
            RegExpDemoObject obj = new RegExpDemoObject();

            obj.ThreeLetterItems = new List<string>()
            {
                "ana",
                "ptv",
                "vrk",
                "IS"
            };

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "ThreeLetterItems";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.ThreeLetterItems, ctx, validationResults).Should().BeFalse("Because 'IS' is not a three letter string.");
        }

        [Fact]
        public void ListContainsThreeLetterStringsAndNull()
        {
            RegExpDemoObject obj = new RegExpDemoObject();

            obj.ThreeLetterItems = new List<string>()
            {
                "ana",
                "ptv",
                "vrk",
                null
            };

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "ThreeLetterItems";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.ThreeLetterItems, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ListContainsThreeLetterStringsAndNullShouldntCauseInternalCrash()
        {
            RegExpDemoObject obj = new RegExpDemoObject();

            obj.ThreeLetterItems = new List<string>()
            {
                "ana",
                "ptv",
                "vrk",
                null
            };

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "ThreeLetterItems";

            Action act = () => Validator.ValidateProperty(obj.ThreeLetterItems, ctx);

            act.ShouldNotThrow<NullReferenceException>("Code shouldn't crash to the null value but check for nulls.");
        }

        [Fact]
        public void EmptyStringIsAlwaysValidValue()
        {
            RegExpDemoObject obj = new RegExpDemoObject();

            obj.ThreeLetterItems = new List<string>()
            {
                "ana",
                "ptv",
                "vrk",
                string.Empty
            };

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "ThreeLetterItems";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.ThreeLetterItems, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void NullPatternShouldThrow()
        {
            RegExpDemoObject obj = new RegExpDemoObject();

            obj.NullRegExPattern.Add("Something");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "NullRegExPattern";

            Action act = () => Validator.ValidateProperty(obj.NullRegExPattern, ctx);

            act.ShouldThrow<InvalidOperationException>("RegEx pattern cannot be null.");
        }

        [Fact]
        public void EmptyPatternShouldThrow()
        {
            RegExpDemoObject obj = new RegExpDemoObject();

            obj.EmptyRegExPattern.Add("Something");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "EmptyRegExPattern";

            Action act = () => Validator.ValidateProperty(obj.EmptyRegExPattern, ctx);

            act.ShouldThrow<InvalidOperationException>("RegEx pattern cannot be empty string.");
        }

        [Fact]
        public void InvalidPatternShouldThrow()
        {
            RegExpDemoObject obj = new RegExpDemoObject();

            obj.InvalidRegExPattern.Add("Something");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "InvalidRegExPattern";

            Action act = () => Validator.ValidateProperty(obj.InvalidRegExPattern, ctx);

            act.ShouldThrowExactly<ArgumentException>("Because regex pattern is invalid.");
        }

        #region test demo objects

        internal class RegExpDemoObject
        {
            public RegExpDemoObject()
            {
                DemoStrings = new List<string>();
                NullRegExPattern = new List<string>();
                EmptyRegExPattern = new List<string>();
                InvalidRegExPattern = new List<string>();
            }

            [ListRegularExpression(@"^\d+$")]
            public List<string> DemoStrings { get; private set; }

            [ListRegularExpression(@"^\w{3}$")]
            public List<string> ThreeLetterItems { get; set; }

            [ListRegularExpression(@"^\d+$")]
            public List<string> NullList { get; set; }

            [ListRegularExpression(@"^\d+$")]
            public string InvalidPropertyUsage { get; set; }

            [ListRegularExpression(null)]
            public List<string> NullRegExPattern { get; private set; }

            [ListRegularExpression("")]
            public List<string> EmptyRegExPattern { get; private set; }

            [ListRegularExpression(@"+?'-)&")]
            public List<string> InvalidRegExPattern { get; private set; }
        }

        #endregion
    }
}
