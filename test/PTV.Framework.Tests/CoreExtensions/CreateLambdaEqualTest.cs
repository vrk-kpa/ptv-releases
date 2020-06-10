/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using System.Linq;
using FluentAssertions;
using PTV.Framework.Tests.DummyClasses;
using Xunit;

namespace PTV.Framework.Tests.CoreExtensions
{
    public class CreateLambdaEqualTest
    {
        [Fact]
        public void CreateLambdaEqualPropertyNameIsNullShouldThrow()
        {
            Action act = () => Framework.CoreExtensions.CreateLambdaEqual<SomeDemoObject>(null, "test");
            act.Should().ThrowExactly<ArgumentNullException>("Property name argument is null.");
        }

        [Fact]
        public void CreateLambdaEqualInvalidPropertyNameShouldThrow()
        {
            Action act = () => Framework.CoreExtensions.CreateLambdaEqual<SomeDemoObject>("NotFoundPropertyName", "test");
            act.Should().ThrowExactly<ArgumentException>("Named property not found from the object.");
        }

        [Fact]
        public void CreateLambdaEqual()
        {
            int equalityValue = 101;
            var lambda = Framework.CoreExtensions.CreateLambdaEqual<SomeDemoObject>("Integer", equalityValue);

            lambda.Should().NotBeNull();

            List<SomeDemoObject> theList = new List<SomeDemoObject>
            {
                new SomeDemoObject
                {
                    Integer = 5
                },
                new SomeDemoObject
                {
                    Type = "Demo"
                },
                new SomeDemoObject
                {
                    Integer = equalityValue
                }
            };

            var sdo = theList.AsQueryable().FirstOrDefault(lambda);

            sdo.Should().NotBeNull();
            sdo.Integer.Should().Be(equalityValue);
        }
    }
}
