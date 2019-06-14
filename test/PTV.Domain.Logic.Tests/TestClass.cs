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
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using Xunit;

namespace PTV.Domain.Logic.Tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class TestClass
    {

        [Theory]
        [InlineData("{ 'x': '5', 'y': '8'}", "{ 'x': '3', 'y': '4'}")]
        [InlineData("{ 'x': '3', 'y': '4'}", "{ 'x': '1', 'y': '2'}")]
        public void TestMethod(string expected, string input)
        {
            var data = JsonConvert.DeserializeObject<TestModel>(input);
            var dataExpected = JsonConvert.DeserializeObject<TestModel>(expected);
            Assert.NotNull(data);
            data.X += 2;
            data.Y *= 2;
            Assert.Equal(dataExpected.X, data.X);
            Assert.Equal(dataExpected.Y, data.Y);

        }

        [Fact]
        public void Enums()
        {
            ShowEnums(typeof(AddressCharacterEnum));
            ShowEnums(typeof(AttachmentTypeEnum));
            ShowEnums(typeof(CoverageTypeEnum));
            ShowEnums(typeof(DescriptionTypeEnum));
            ShowEnums(typeof(ExceptionHoursStatus));
            ShowEnums(typeof(NameTypeEnum));
            ShowEnums(typeof(OrganizationTypeEnum));
            ShowEnums(typeof(PhoneNumberTypeEnum));
            ShowEnums(typeof(PrintableFormChannelUrlTypeEnum));
            ShowEnums(typeof(ProvisionTypeEnum));
            ShowEnums(typeof(PublishingStatus));
            ShowEnums(typeof(ServiceHoursTypeEnum));
            ShowEnums(typeof(ServiceChannelTypeEnum));
            ShowEnums(typeof(ServiceChargeTypeEnum));
            ShowEnums(typeof(AddressTypeEnum));
            ShowEnums(typeof(WebPageTypeEnum));
        }

        private void ShowEnums(Type type)
        {
            Console.WriteLine(type.Name);
            Console.WriteLine(string.Join("\n", Enum.GetNames(type)));
            Console.WriteLine();
        }
    }

    public class TestModel
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
