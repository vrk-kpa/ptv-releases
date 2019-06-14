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
using System.Text;
using Xunit;

namespace PTV.Framework.Tests.Attributes
{
    public class EmailAddressListAttributeTests
    {
        // EmailAddressListAttribute actually expects the value to be an IList
        // propertyname is used to point object property name in the items of the list
        // model property (which should be an IList<something>) is decorated with this attribute
        // propertyname is used to reference the something items property like something.property

        [Fact]
        public void UsageWithDefaultSettingsNullValue()
        {
            // validator should try to validate null list, nothing to validate so should return true

            EmailAddressListAttribute attr = new EmailAddressListAttribute();
            attr.IsValid(null).Should().BeTrue();
        }

        [Fact]
        public void UsageWithDefaultSettingsInvalidPropertyType()
        {
            // don't set propertyname and pass empty string (this means the attribute is used on wrong kind of property)
            // this should throw exception about wrong kind of property type, only valid type is ilist

            EmailAddressListAttribute attr = new EmailAddressListAttribute();
            // isvalid is actually passed an object property
            Action act = () => attr.IsValid(string.Empty);
            act.Should().Throw<Exception>(because: "EmailAddressListAttribute is used on wrong type of property. Should be only used on properties of type IList.");
        }

        [Fact]
        public void EmptyList()
        {
            // empty list, nothing to validate

            EmailAddressListAttribute attr = new EmailAddressListAttribute();
            attr.IsValid(new ArrayList()).Should().BeTrue();
        }

        [Fact]
        public void PropertyNameNotDefinedDefault()
        {
            EmailAddressListAttribute attr = new EmailAddressListAttribute();

            // because property name is not defined the email validation should be done on the list item not to its property
            // this should fail because the passed object cannot be validated for email address string
            // if passing custom object, then the propertyname needs to be set

            Action act = () => attr.IsValid(new ArrayList() {new AttributeHelperTestingObject() });
            act.Should().Throw<Exception>(because: "EmailAddressListAttribute propertyName is not defined (default).");
        }

        [Fact]
        public void PropertyNameNotDefinedEmptyString()
        {
            // string.empty is currently the default but if the implementation is changed then we still need to test with empty.string
            EmailAddressListAttribute attr = new EmailAddressListAttribute(string.Empty);

            Action act = () => attr.IsValid(new ArrayList() { new AttributeHelperTestingObject() });
            act.Should().Throw<Exception>(because: "EmailAddressListAttribute propertyName is not defined (string.empty).");
        }

        [Fact]
        public void PropertyNameNotDefinedNull()
        {
            // explicitly set property name to null
            EmailAddressListAttribute attr = new EmailAddressListAttribute(null);

            Action act = () => attr.IsValid(new ArrayList() { new AttributeHelperTestingObject() });
            act.Should().Throw<Exception>(because: "EmailAddressListAttribute propertyName is not defined (null).");
        }

        [Fact]
        public void InvalidPropertyName()
        {
            // explicitly set property name to null
            EmailAddressListAttribute attr = new EmailAddressListAttribute("BogusPropName");

            // it is valid to assume that the list have items that are of the same type or implement the same interface
            // so all items should have the same property and if the property is not found on the object it should throw
            // or change this test to expect validation error
            // exception would be better to indicate that one or more items in the list doesn't have the expected property

            Action act = () => attr.IsValid(new ArrayList() { new AttributeHelperTestingObject() });
            act.Should().Throw<Exception>(because: "EmailAddressListAttribute invalid propertyName defined (BogusPropName).");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("some text")]
        [InlineData("@ptv.com")]
        [InlineData("a .email@ptv.com")]// PTV-4296
        public void CustomObjectListContainsInvalidEmail(string invalidEmail)
        {
            // email address is in the String named property
            EmailAddressListAttribute attr = new EmailAddressListAttribute("String");

            List<SomeDemoObject> inList = new List<SomeDemoObject>(4);
            inList.Add(new SomeDemoObject() { String = "ptv@ptv.com" });
            inList.Add(new SomeDemoObject() { String = "ptv.vrk@example.com" });
            inList.Add(new SomeDemoObject() { String = "shirley-ptv.vrk@example.com" });
            inList.Add(new SomeDemoObject() { String = invalidEmail });

            attr.IsValid(inList).Should().BeFalse();
        }

        // Custom validator added - all the emails below are not valid any more! PTV-4296
        [Theory]
        [InlineData("ptv@.com")] // this validates ok by the framework attribute
        [InlineData("ptv.@example.com")] // this validates ok by the framework attribute
        [InlineData("ptv@example..com")] // this validates ok by the framework attribute
        [InlineData("ptv @example.com")] // this validates ok by the framework attribute
        [InlineData("ptv@ example.com")] // this validates ok by the framework attribute
        [InlineData(" ptv@example.com")] // this validates ok by the framework attribute
        [InlineData("ptv@example.com ")] // this validates ok by the framework attribute
        [InlineData("email@ptv*.com")]// PTV-4296
        [InlineData("email@ptv.c")]// PTV-4296
        [InlineData("email@")]// PTV-4296
        public void CustomObjectListContainsInValidEmails(string actuallyValidEmailAddressByFrameworkAttribute)
        {
            // Comment: see RFC http://www.faqs.org/rfcs/rfc2822.html and at the bottom the silly examples A.6.1
            // so the above inlinedata email addresses are valid and reason why those are used here
            // yes on purpose running a test per bogus valid email so if we decide to write own validator
            // we can easily move the above test data there to validate "invalid" addresses
            // 

            // email address is in the String named property
            EmailAddressListAttribute attr = new EmailAddressListAttribute("String");

            List<SomeDemoObject> inList = new List<SomeDemoObject>(5);
            inList.Add(new SomeDemoObject() { String = "ptv@ptv.com" });
            inList.Add(new SomeDemoObject() { String = "ptv.vrk@example.com" });
            inList.Add(new SomeDemoObject() { String = "shirley-ptv.vrk@example.com" });
            inList.Add(new SomeDemoObject() { String = "shirley-ptv.vrk@subdomain.example.com" });
            inList.Add(new SomeDemoObject() { String = "user1234@example.com" });
            inList.Add(new SomeDemoObject() { String = actuallyValidEmailAddressByFrameworkAttribute });

            attr.IsValid(inList).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)] // this validates ok by the framework attribute
        public void CustomObjectListContainsValidEmails(string actuallyValidEmailAddressByFrameworkAttribute)
        {
            // email address is in the String named property
            EmailAddressListAttribute attr = new EmailAddressListAttribute("String");

            List<SomeDemoObject> inList = new List<SomeDemoObject>(5);
            inList.Add(new SomeDemoObject() { String = "ptv@ptv.com" });
            inList.Add(new SomeDemoObject() { String = "ptv.vrk@example.com" });
            inList.Add(new SomeDemoObject() { String = "shirley-ptv.vrk@example.com" });
            inList.Add(new SomeDemoObject() { String = "shirley-ptv.vrk@subdomain.example.com" });
            inList.Add(new SomeDemoObject() { String = "user1234@example.com" });
            inList.Add(new SomeDemoObject() { String = actuallyValidEmailAddressByFrameworkAttribute });

            attr.IsValid(inList).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ArrayListContainsValidEmails(string propertyName)
        {
            // if propertyName is null or empty
            // theimplementation should use the item in the list (its value)

            EmailAddressListAttribute attr = new EmailAddressListAttribute(propertyName);

            ArrayList inList = new ArrayList(5);
            inList.Add("ptv@ptv.com");
            inList.Add("ptv.vrk@example.com");
            inList.Add("shirley-ptv.vrk@example.com");
            inList.Add("shirley-ptv.vrk@subdomain.example.com");
            inList.Add("user1234@example.com");

            attr.IsValid(inList).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GenericListContainsValidEmails(string propertyName)
        {
            // if propertyName is null or empty
            // theimplementation should use the item in the list (its value)

            EmailAddressListAttribute attr = new EmailAddressListAttribute(propertyName);

            List<string> inList = new List<string>(5);
            inList.Add("ptv@ptv.com");
            inList.Add("ptv.vrk@example.com");
            inList.Add("shirley-ptv.vrk@example.com");
            inList.Add("shirley-ptv.vrk@subdomain.example.com");
            inList.Add("user1234@example.com");

            attr.IsValid(inList).Should().BeTrue();
        }
    }
}
