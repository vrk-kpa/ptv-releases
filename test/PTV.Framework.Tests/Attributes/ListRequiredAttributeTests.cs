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
    public class ListRequiredAttributeTests
    {
        [Fact]
        public void NullIsNotValidValue()
        {
            ListRequiredAttribute attr = new ListRequiredAttribute();

            attr.IsValid(null).Should().BeFalse();
        }

        [Fact]
        public void PublicIsValidAndProtectedIsValidOnStringListShouldReturnTheSameResult()
        {
            List<string> inValues = new List<string>() { "vrk", "ptv" };

            ListRequiredAttribute attr = new ListRequiredAttribute();

            bool publicIsValid = attr.IsValid(inValues);

            TestRequiredObject obj = new TestRequiredObject();
            obj.RequiredStringList = inValues;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "RequiredStringList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.RequiredStringList, ctx, validationResults).Should().Be(publicIsValid);
        }

        [Fact]
        public void PublicIsValidAndProtectedIsValidOnObjectListShouldReturnTheSameResult()
        {
            List<SomeDemoObject> inValues = new List<SomeDemoObject>()
            {
               new SomeDemoObject()
               {
                   Integer = 100,
                   String = "a string",
                   Type = "Demo"
               },
               null,
               new SomeDemoObject()
               {
                   Integer = 101,
                   String = "some string",
                   Type = "SomeType"
               }
            };

            ListRequiredAttribute attr = new ListRequiredAttribute();
            bool publicIsValid = attr.IsValid(inValues);
            
            TestRequiredObject obj = new TestRequiredObject();
            obj.RequiredObjectList = inValues;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "RequiredObjectList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.RequiredObjectList, ctx, validationResults).Should().Be(publicIsValid);
        }

        [Fact]
        public void NullItemInObjectListShouldBeInvalidResult()
        {
            List<SomeDemoObject> inValues = new List<SomeDemoObject>()
            {
               new SomeDemoObject()
               {
                   Integer = 100,
                   String = "a string",
                   Type = "Demo"
               },
               null,
               new SomeDemoObject()
               {
                   Integer = 101,
                   String = "some string",
                   Type = "SomeType"
               }
            };

            TestRequiredObject obj = new TestRequiredObject();
            obj.RequiredObjectList = inValues;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "RequiredObjectList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.RequiredObjectList, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void RequiredStringListShouldBeFalse()
        {
            ListRequiredAttribute attr = new ListRequiredAttribute();

            TestRequiredObject obj = new TestRequiredObject();

            attr.IsValid(obj.RequiredStringList).Should().BeFalse();
        }

        [Fact]
        public void RequiredStringListShouldBeFalseUsingProtectedIsValid()
        {
            TestRequiredObject obj = new TestRequiredObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "RequiredStringList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.RequiredStringList, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void ShouldThrowInvalidPropertyTypeUsage()
        {
            TestRequiredObject obj = new TestRequiredObject();
            obj.SomeDictionary.Add("ptv", "vrk");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "SomeDictionary";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Action act = () => Validator.TryValidateProperty(obj.SomeDictionary, ctx, validationResults);

            act.ShouldThrowExactly<InvalidOperationException>();
        }

        #region test helper classes

        internal class TestRequiredObject
        {
            public TestRequiredObject()
            {
                RequiredObjectList = new List<SomeDemoObject>();
                SomeDictionary = new Dictionary<string, string>();
            }

            [ListRequired]
            public List<string> RequiredStringList { get; set; }

            [ListRequired]
            public List<SomeDemoObject> RequiredObjectList { get; set; }

            [ListRequired]
            public Dictionary<string,string> SomeDictionary { get; set; }
        }

        #endregion
    }
}
