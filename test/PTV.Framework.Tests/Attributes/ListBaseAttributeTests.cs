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
    public class ListBaseAttributeTests
    {
        // Comment: Should the ListBaseAttribute actually be abstract class?
        // tests needs to be rewriten if the ListBaseAttribute is changed to be abstract class as we can't instantiate it anymore.

        [Fact]
        public void NullValidator()
        {
            // the validator to be used is not set, should throw an exception

            Action act = () => new ListBaseAttribute(null, "String");
            act.ShouldThrowExactly<ArgumentNullException>("Validator is null.");
        }

        [Fact]
        public void ValidatorUsedOnInvalidProperty()
        {
            ListBaseDemoObject obj = new ListBaseDemoObject();
            // we need to set this value to other than null so that the logic tries to cast the property to IList
            obj.InvalidAttributeUsage = "diipadaapa";

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "InvalidAttributeUsage";

            Action act = () => Validator.ValidateProperty(obj.InvalidAttributeUsage, ctx);
            act.ShouldThrowExactly<InvalidOperationException>("ListBaseAttribute based attribute used on a property that doesn't implement IList.");
        }

        [Theory]
        [InlineData("ptv")]
        public void ValidStringValuesInListProperty(string itemValue)
        {
            ListBaseDemoObject obj = new ListBaseDemoObject();
            obj.MyValidStrings.Add(itemValue);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "MyValidStrings";

            Action act = () => Validator.ValidateProperty(obj.MyValidStrings, ctx);
            act.ShouldNotThrow();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("vrk")]
        [InlineData("Ptv")]
        public void InvalidStringValuesInListProperty(string itemValue)
        {
            ListBaseDemoObject obj = new ListBaseDemoObject();
            obj.MyInValidStrings.Add(itemValue);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "MyInValidStrings";

            Action act = () => Validator.ValidateProperty(obj.MyInValidStrings, ctx);
            act.ShouldThrowExactly<ValidationException>();
        }

        [Fact]
        public void CustomObjectListWithNullValue()
        {
            ListBaseDemoObject obj = new ListBaseDemoObject();
            obj.CustomObjectList.Add(new CustomListObject()
            {
                DemoText = "ptv",
                LuckyNumber = 7
            });
            obj.CustomObjectList.Add(null); // add null item to the list

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "CustomObjectList";

            Action act = () => Validator.ValidateProperty(obj.CustomObjectList, ctx);
            act.ShouldThrowExactly<ValidationException>().WithMessage("List contains a null object and propertyName is defined. Cannot get property value from null object.");
        }

        [Fact]
        public void CustomObjectListInvalidPropertyName()
        {
            // see DemoItems collection decorated with a propertyname that doesn't exists in the CustomListObject object

            ListBaseDemoObject obj = new ListBaseDemoObject();
            obj.DemoItems.Add(new CustomListObject()
            {
                DemoText = "ptv",
                LuckyNumber = 7
            });

            obj.DemoItems.Add(new CustomListObject()
            {
                DemoText = "ptv",
                LuckyNumber = 17
            });


            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "DemoItems";

            // notice the * usage at the end of the message
            Action act = () => Validator.ValidateProperty(obj.DemoItems, ctx);
            act.ShouldThrowExactly<ArgumentException>().WithMessage("Item doesn't contain property named: 'InvalidPropertyName'.*");
        }

        [Theory]
        [InlineData("BogusPropertyName")]
        public void PropertyNotFoundFromListItem(string propertyName)
        {
            ListBaseAttribute attr = new ListBaseAttribute(new EmailAddressAttribute(), propertyName);
            // pass a list but the objects in the list don't have the propertyname defined
            // notice the * usage at the end of the message
            Action act = () => attr.IsValid(new ArrayList() { new AttributeHelperTestingObject() });
            act.ShouldThrowExactly<ArgumentException>().WithMessage($"Item doesn't contain property named: '{propertyName}'.*");
        }

        [Fact]
        public void ListIsNull()
        {
            ListBaseAttribute attr = new ListBaseAttribute(new EmailAddressAttribute(), "String");
            attr.IsValid(null).Should().BeTrue();
        }

        [Fact]
        public void ListIsEmpty()
        {
            ListBaseAttribute attr = new ListBaseAttribute(new EmailAddressAttribute(), "String");
            attr.IsValid(new ArrayList()).Should().BeTrue();
        }

        [Fact]
        public void GenericListIsEmpty()
        {
            ListBaseAttribute attr = new ListBaseAttribute(new EmailAddressAttribute(), "String");
            attr.IsValid(new List<string>()).Should().BeTrue();
        }

        [Fact]
        public void StringListPropertyContainsNullValue()
        {
            ListBaseAttribute attr = new ListBaseAttribute(new AllowedValuesAttribute(null, new string[] { "ptv", "vrk" }));

            List<string> strValues = new List<string>(3);
            strValues.Add("ptv");
            strValues.Add("vrk");
            strValues.Add(null);

            attr.IsValid(strValues).Should().BeTrue();
        }

        // Demo helper classes below

        internal class ListBaseTestAttribute : ListBaseAttribute
        {
            // just a demo attribute to be used in ListBaseAttributeTests

            public ListBaseTestAttribute(string propertyName = null) : base(new AllowedValuesAttribute(propertyName, new string[] { "ptv" }), propertyName)
            {
            }
        }

        internal class ListBaseDemoObject
        {
            public ListBaseDemoObject()
            {
                MyValidStrings = new List<string>(5);
                MyInValidStrings = new List<string>(5);
                CustomObjectList = new List<CustomListObject>(5);
                DemoItems = new List<CustomListObject>(5);
            }

            /// <summary>
            /// List validator attribute used on invalid type property, the type should be IList
            /// </summary>
            [ListBaseTest]
            public string InvalidAttributeUsage { get; set; }

            [ListBaseTest]
            public List<string> MyValidStrings { get; private set; }

            [ListBaseTest]
            public List<string> MyInValidStrings { get; private set; }

            [ListBaseTest("DemoText")]
            public List<CustomListObject> CustomObjectList { get; private set; }

            [ListBaseTest("InvalidPropertyName")]
            public List<CustomListObject> DemoItems { get; private set; }
        }

        internal class CustomListObject
        {
            public int LuckyNumber { get; set; }

            public string DemoText { get; set; }
        }
    }
}
