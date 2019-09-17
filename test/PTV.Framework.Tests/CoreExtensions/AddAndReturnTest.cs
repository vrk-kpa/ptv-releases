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
    public class AddAndReturnTest
    {
        [Fact]
        public void AddAndReturn()
        {
            List<SomeDemoObject> theList = new List<SomeDemoObject>();

            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 8,
                String = "text",
                Type = "demo"
            };

            var returnedObj = theList.AddAndReturn(obj);

            returnedObj.Should().BeSameAs(obj);
        }

        [Fact]
        public void AddAndReturnHandlesAddNullItem()
        {
            List<SomeDemoObject> theList = new List<SomeDemoObject>();
            var addedObj = theList.AddAndReturn(null);

            addedObj.Should().BeNull();
        }

        [Fact]
        public void AddAndReturnShouldHandleNullCollection()
        {
            ICollection<SomeDemoObject> col = null;

            Action act = () => col.AddAndReturn(new SomeDemoObject());

            act.Should().ThrowExactly<ArgumentNullException>("Collection instance is null.").WithMessage("Value cannot be null.*Parameter name: collection");
        }
    }
}
