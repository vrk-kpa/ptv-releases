using FluentAssertions;
using PTV.Framework.Attributes;
using PTV.Framework.Tests.DummyClasses;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PTV.Framework.Tests.Attributes
{
    public class AttributeHelperTests
    {
        // TODO: Do we actually even need this AttributeHelper class as PTV.Framework.CoreExtensions contains these same methods
        // AttributeHelper is used by validator attributes

        [Fact]
        public void UsageOnNullInstance()
        {
            AttributeHelperTestingObject obj = null;

            obj.GetItemValue("SomeDemo").Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void PropertyNameIsNullOrEmptyOrWhitespace(string propertyName)
        {
            // if property name is null or empty the method should return the same object
            // currently works only when property name is empty string
            // should it work like null, "", would always return the same object

            SomeDemoObject obj = new SomeDemoObject() { Integer = 100, String = "somestring" };

            obj.GetItemValue(propertyName).Should().BeSameAs(obj);
        }

        [Theory]
        [InlineData("NotValidPropertyName")]
        public void UsageOnInvalidPropertyName(string propertyName)
        {
            AttributeHelperTestingObject obj = new AttributeHelperTestingObject();

            Action act = () => obj.GetItemValue(propertyName);
            act.ShouldThrowExactly<ArgumentException>().WithMessage($"Item doesn't contain property named: '{propertyName}'.*");
        }

        [Theory]
        [InlineData("Integer")]
        [InlineData("NullableInteger")]
        [InlineData("String")]
        [InlineData("DateTime")]
        [InlineData("SomeDemo")]
        public void UsageOnValidPropertyName(string propertyName)
        {
            AttributeHelperTestingObject obj = new AttributeHelperTestingObject() {
                DateTime = DateTime.Now,
                Integer = 100,
                String = "merkkijono",
                NullableInteger = 10,
                SomeDemo = new SomeDemoObject() { Integer = 1, String = "ptv"}
            };

            // assume we get a non null value always, datetime and integer are boxed
            obj.GetItemValue(propertyName).Should().NotBeNull();
        }
    }
}
