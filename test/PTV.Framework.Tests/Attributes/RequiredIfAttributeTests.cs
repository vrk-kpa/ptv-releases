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
    public class RequiredIfAttributeTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void InvalidStringValuesWhenValueIsRequired(string value)
        {
            TestObject obj = new TestObject();
            obj.AdditionalDescription = value;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "AdditionalDescription";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.AdditionalDescription, ctx, validationResults).Should().BeFalse();
        }

        [Theory]
        [InlineData("a")]
        [InlineData("Hello")]
        [InlineData("5")]
        public void ValidStringValueWhenValueRequired(string value)
        {
            TestObject obj = new TestObject();
            obj.AdditionalDescription = value;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "AdditionalDescription";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.AdditionalDescription, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void AdditionalDescriptionNotRequired()
        {
            TestObject obj = new TestObject();

            obj.ExtraInfo = new SomeDemoObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "AdditionalDescription";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.AdditionalDescription, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void AttributeDependencyPropertyNameMisconfigured()
        {
            TestObject obj = new TestObject();
            
            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "InvalidPropertyNameInAttribute";

            Action act = () => Validator.ValidateProperty(obj.InvalidPropertyNameInAttribute, ctx);
            // expected message could be something else and should be updated here when the attribute is modified
            act.ShouldThrowExactly<InvalidOperationException>().WithMessage("Property 'MissingPropertyName' not found.", "Because the dependant property is not found.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void DependencyStringPropertyValuesThatAreConsideredAsNull(string value)
        {
            // when dependency to a string type property: null, string.empty and whitespaces are treated the same in the validator
            // meaning [RequiredIf("RequiredIfStringDependencyProperty", null)] means that if the RequiredIfStringDependencyProperty value is null or empty or whitespaces
            // the decorated property is required, so using null in the attribute doesn't mean just null but empty and whitespace
            TestObject obj = new TestObject();
            obj.RequiredIfStringDependencyProperty = value;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "RequiredIfString";

            Action act = () => Validator.ValidateProperty(obj.RequiredIfString, ctx);

            act.ShouldThrowExactly<ValidationException>().WithMessage("The field is required when 'RequiredIfStringDependencyProperty' has value '*'.");
        }
        
        [Fact]
        public void RequiredWhenDependencyStringIsEmptyString()
        {
            TestObject obj = new TestObject();
            obj.RequiredIfEmptyStringDependencyProperty = string.Empty;            

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "RequiredIfEmptyString";

            List<ValidationResult> validationResults = new List<ValidationResult>();
            // property RequiredIfEmptyString should be required (be other than null) because the RequiredIfEmptyStringDependencyProperty
            // is explicitly set to value empty.string
            Validator.TryValidateProperty(obj.RequiredIfEmptyString, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void RequiredWhenDependencyStringIsWhitespaces()
        {
            TestObject obj = new TestObject();
            obj.RequiredIfEmptyStringDependencyProperty = "  ";

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "RequiredIfWhitespacesString";

            List<ValidationResult> validationResults = new List<ValidationResult>();
            // property RequiredIfEmptyString should be required (be other than null) because the RequiredIfEmptyStringDependencyProperty
            // is explicitly set to value "  " (2 whitespaces)
            Validator.TryValidateProperty(obj.RequiredIfWhitespacesString, ctx, validationResults).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RequiredWhenDependencyListIsNullOrEmpty(string option)
        {
            TestObject obj = new TestObject();

            // abusing [Theory], null means set list null, all other values let the list be empty
            if (option == null)
            {
                obj.ValueList = null;
            }

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "RequiredIfListIsNullOrEmpty";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.RequiredIfListIsNullOrEmpty, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void RequiredIfListContainsMatch()
        {
            TestObject obj = new TestObject();
            obj.ValueList.Add("SomeMatch");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "RequiredIfListContainsValue";

            List<ValidationResult> validationResults = new List<ValidationResult>();
            // property RequiredIfListContainsValue should have a value because list contains a match
            Validator.TryValidateProperty(obj.RequiredIfListContainsValue, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void NotRequiredBecauseThereIsNoMatchInTheList()
        {
            TestObject obj = new TestObject();
            obj.ValueList.Add("VRK");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "RequiredIfListContainsValue";

            List<ValidationResult> validationResults = new List<ValidationResult>();
            // property RequiredIfListContainsValue should have a value because list contains a match
            Validator.TryValidateProperty(obj.RequiredIfListContainsValue, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ValidatorShouldHandleNullValuesInList()
        {
            TestObject obj = new TestObject();
            obj.ValueList.Add("VRK");
            obj.ValueList.Add(null);
            obj.ValueList.Add("SomeMatch");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "RequiredIfListContainsValue";

            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateProperty(obj.RequiredIfListContainsValue, ctx, validationResults).Should().BeFalse();            
        }

        internal class TestObject
        {
            public TestObject()
            {
                ValueList = new List<string>();
            }

            [RequiredIf("ExtraInfo", null)]
            public string AdditionalDescription { get; set; }

            public SomeDemoObject ExtraInfo { get; set; }


            [RequiredIf("MissingPropertyName", null)]
            public string InvalidPropertyNameInAttribute { get; set; }

            [RequiredIf("RequiredIfStringDependencyProperty", null)]
            public string RequiredIfString { get; set; }

            public string RequiredIfStringDependencyProperty { get; set; }

            [RequiredIf("RequiredIfEmptyStringDependencyProperty", "")]
            public string RequiredIfEmptyString { get; set; }

            [RequiredIf("RequiredIfEmptyStringDependencyProperty", "  ")]
            public string RequiredIfWhitespacesString { get; set; }

            public string RequiredIfEmptyStringDependencyProperty { get; set; }


            [RequiredIf("ValueList", null)]
            public string RequiredIfListIsNullOrEmpty { get; set; }

            public List<string> ValueList { get; set; }

            [RequiredIf("ValueList", "SomeMatch")]
            public string RequiredIfListContainsValue { get; set; }
            
        }
    }
}
