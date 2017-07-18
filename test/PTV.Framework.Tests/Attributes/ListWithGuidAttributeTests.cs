using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;
using PTV.Framework.Attributes;
using System.ComponentModel.DataAnnotations;

namespace PTV.Framework.Tests.Attributes
{
    public class ListWithGuidAttributeTests
    {
        [Fact]
        public void EmptyListIsValid()
        {
            TestObject obj = new TestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Guids";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Guids, ctx, validationResults).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("         ")]
        public void TestValidSpecialGuidStrings(string specialGuid)
        {
            TestObject obj = new TestObject();
            obj.Guids.Add(specialGuid);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Guids";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Guids, ctx, validationResults).Should().BeTrue();
        }

        [Theory]
        [InlineData("N")]
        [InlineData("D")]
        [InlineData("B")]
        [InlineData("P")]
        [InlineData("X")]
        public void TestValidGuidStringFormats(string format)
        {
            TestObject obj = new TestObject();
            obj.Guids.Add(Guid.NewGuid().ToString(format));

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Guids";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Guids, ctx, validationResults).Should().BeTrue();
        }

        [Theory]
        [InlineData("hello world")]
        [InlineData("5555-8888-2922-1111")]
        public void TestInvalidGuidValues(string specialGuid)
        {
            TestObject obj = new TestObject();
            obj.Guids.Add(specialGuid);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Guids";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Guids, ctx, validationResults).Should().BeFalse();
        }

        internal class TestObject
        {
            public TestObject()
            {
                Guids = new List<string>();
            }

            [ListWithGuid]
            public List<string> Guids { get; private set; }
        }
    }
}
