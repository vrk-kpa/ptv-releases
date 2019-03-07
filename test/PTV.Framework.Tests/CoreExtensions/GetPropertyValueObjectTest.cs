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
    public class GetPropertyValueObjectTest
    {
        [Fact]
        public void GetPropertyValueObject()
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

            obj.GetPropertyObjectValue("Integer").Should().Be(number);
            obj.GetPropertyObjectValue("String").Should().Be(txt);
            obj.GetPropertyObjectValue("Type").Should().Be(typeText);

            IList<string> theList = obj.GetPropertyObjectValue("SomeList") as IList<string>;

            theList.Should().NotBeNullOrEmpty();
            theList.Count.Should().Be(2);
        }

        [Fact]
        public void GetPropertyValueObjectPropertyNameIsNullShouldThrow()
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
            Action act = () => obj.GetPropertyObjectValue(null);

            act.Should().ThrowExactly<ArgumentNullException>("Property name is null.");
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void GetPropertyValueObjectInvalidPropertyNameShouldThrow(string propertyName)
        {
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 8,
                String = "vrk ptv",
                Type = "Demo"
            };
            obj.SomeList.Add("ptv");
            obj.SomeList.Add("vrk");

            // extension should throw an argumenexception when a known invalid propertyname is passed
            // why? because otherwise the caller doesn't know that the property is missing from the object
            Action act = () => obj.GetPropertyObjectValue(propertyName);

            act.Should().ThrowExactly<ArgumentException>("Known invalid property name.");
        }
    }
}
