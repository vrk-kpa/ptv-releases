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
using System;
using System.Collections.Generic;
using FluentAssertions;
using PTV.Framework.Tests.DummyClasses;
using Xunit;

namespace PTV.Framework.Tests.CoreExtensions
{
    public class GetPropertyValueTest
    {
        [Fact]
        public void GetPropertyValueTyped()
        {
            int number = 7;
            string txt = "vrk ptv";
            string typeText = "Test";

            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = number,
                String = txt,
                Type = typeText
            };
            obj.SomeList.Add("ptv");
            obj.SomeList.Add("vrk");

            obj.GetPropertyValue<SomeDemoObject, int>("Integer").Should().Be(number);
            obj.GetPropertyValue<SomeDemoObject, string>("String").Should().Be(txt);
            obj.GetPropertyValue<SomeDemoObject, string>("Type").Should().Be(typeText);

            IList<string> theList = obj.GetPropertyValue<SomeDemoObject, IList<string>>("SomeList");

            theList.Should().NotBeNullOrEmpty();
            theList.Count.Should().Be(2);
        }

        [Fact]
        public void GetPropertyValueTypedPropertyNameIsNullShouldThrow()
        {
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 8,
                String = "vrk ptv",
                Type = "Demo"
            };
            obj.SomeList.Add("ptv");
            obj.SomeList.Add("vrk");

            // extension should throw an ArgumentNullException
            Action act = () => obj.GetPropertyValue<SomeDemoObject, int>(null);

            act.Should().ThrowExactly<ArgumentNullException>("Property name is null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void GetPropertyValueTypedInvalidPropertyNameShouldThrow(string propertyName)
        {
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 8,
                String = "vrk ptv",
                Type = "Demo"
            };
            obj.SomeList.Add("ptv");
            obj.SomeList.Add("vrk");

            // extension should throw an argumenexception when a known invalid propertyname is passed or a property is not found
            // why? because otherwise the caller doesn't know that the property is missing from the object
            Action act = () => obj.GetPropertyValue<SomeDemoObject, int>(propertyName);

            act.Should().ThrowExactly<ArgumentException>("Known invalid property name.");
        }

        [Fact]
        public void GetPropertyValueTypedPropertyMissingShouldThrow()
        {
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 8,
                String = "vrk ptv",
                Type = "Demo"
            };
            obj.SomeList.Add("ptv");
            obj.SomeList.Add("vrk");

            // extension should throw an argumenexception when a property is not found
            // why? because otherwise the caller doesn't know that the property is missing from the object
            Action act = () => obj.GetPropertyValue<SomeDemoObject, int>("NotFoundPropertyName");

            act.Should().ThrowExactly<ArgumentException>("Property not found from object.");
        }

        [Fact]
        public void GetPropertyValueObjectPropertyMissingShouldThrow()
        {
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 8,
                String = "vrk ptv",
                Type = "Demo"
            };
            obj.SomeList.Add("ptv");
            obj.SomeList.Add("vrk");

            // extension should throw an argumenexception when a property is not found
            // why? because otherwise the caller doesn't know that the property is missing from the object
            Action act = () => obj.GetPropertyValue<SomeDemoObject, int>("NotFoundPropertyName");

            act.Should().ThrowExactly<ArgumentException>("Property not found from object.");
        }
    }
}
