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
using System.Text;
using FluentAssertions;
using Xunit;
using Newtonsoft.Json;
using PTV.Framework.Converters;
using PTV.Domain.Model.Enums;

namespace PTV.Framework.Tests.Converters
{
    public class IntEnumConverterTests
    {
        // NOTE! The IntEnumConverter is not used anywhere
        // most likely this converter is not needed
        // when deserializing the reader.Value is a 64-bit integer and it cannot be cast with int (32-bit)
        // maybe the converter class and this test should be removed?
        // currently the test will fail with both int and enum values to the invalid cast exception

        [Fact]
        public void ConvertToJsonAndBack()
        {
            TestObject obj = new TestObject
            {
                Number = 5,
                //Status = PublishingStatus.Published,
                Text = "Status is published."
            };

            string json = JsonConvert.SerializeObject(obj);

            TestObject desObj = JsonConvert.DeserializeObject<TestObject>(json);

            //obj.Status.Should().Be(desObj.Status);
            obj.Number.Should().Be(desObj.Number);
            obj.Text.Should().Be(desObj.Text);
        }

        internal class TestObject
        {
            public string Text { get; set; }

            //[JsonConverter(typeof(IntEnumConverter))]
            //public PublishingStatus Status { get; set; }

            [JsonConverter(typeof(IntEnumConverter))]
            public int Number { get; set; }
        }
    }
}
