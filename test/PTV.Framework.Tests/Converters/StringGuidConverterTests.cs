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
using System.Text;
using FluentAssertions;
using Xunit;
using Newtonsoft.Json;
using PTV.Framework.Converters;

namespace PTV.Framework.Tests.Converters
{
    public class StringGuidConverterTests
    {
        [Theory]
        [InlineData("ca761232ed4211cebacd00aa0057b222")]
        [InlineData("CA761232-ED42-11CE-BACD-00AA0057B223")]
        [InlineData("{CA761232-ED42-11CE-BACD-00AA0057B223}")]
        [InlineData("(CA761232-ED42-11CE-BACD-00AA0057B223)")]
        [InlineData("{0xCA761232, 0xED42, 0x11CE, {0xBA, 0xCD, 0x00, 0xAA, 0x00, 0x57, 0xB2, 0x23}}")]
        public void ValidGuidToJsonToGuidConversions(string guidTestValue)
        {
            Guid testGuid = new Guid(guidTestValue);

            TestObject obj = new TestObject()
            {
                Identifier = testGuid,
                NullableIdentifier = testGuid,
                Text = guidTestValue
            };

            string json = JsonConvert.SerializeObject(obj);

            TestObject desObj = JsonConvert.DeserializeObject<TestObject>(json);

            obj.Identifier.Should().Be(desObj.Identifier);
            obj.NullableIdentifier.Should().Be(desObj.NullableIdentifier);
            obj.Text.Should().Be(desObj.Text);
        }

        [Fact]
        public void NullInNullableIdentifier()
        {
            TestObject obj = new TestObject()
            {
                Identifier = Guid.NewGuid(),
                NullableIdentifier = null,
                Text = string.Empty
            };

            string json = JsonConvert.SerializeObject(obj);

            TestObject desObj = JsonConvert.DeserializeObject<TestObject>(json);

            obj.Identifier.Should().Be(desObj.Identifier);
            obj.NullableIdentifier.Should().Be(desObj.NullableIdentifier);
            obj.Text.Should().Be(desObj.Text);
        }

        [Fact]
        public void CanConvertNullableGuid()
        {
            StringGuidConverter converter = new StringGuidConverter();
            converter.CanConvert(typeof(Guid?)).Should().BeTrue();
        }

        [Fact]
        public void CanConvertGuid()
        {
            StringGuidConverter converter = new StringGuidConverter();
            converter.CanConvert(typeof(Guid)).Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(decimal))]
        [InlineData(typeof(object))]
        public void CanConvertShouldReturnFalse(Type objectType)
        {
            // the converter is intended for Guids and nothing else
            // NOTE! CanConvert method is not called by json.net if using with the JsonConverterAttribute
            // like this: [JsonConverter(typeof(StringGuidConverter))]

            StringGuidConverter converter = new StringGuidConverter();
            converter.CanConvert(objectType).Should().BeFalse("Because the converter can only support Guid to string and back to Guid conversions.");
        }

        [Theory]
        [InlineData(typeof(Guid))]
        [InlineData(typeof(Guid?))]
        public void CanConvertShouldReturnTrue(Type objectType)
        {
            // the converter is intended for Guids and nothing else
            // NOTE! CanConvert method is not called ny json.net if using with the JsonConverterAttribute
            // like this: [JsonConverter(typeof(StringGuidConverter))]

            StringGuidConverter converter = new StringGuidConverter();
            converter.CanConvert(objectType).Should().BeTrue();
        }

        internal class TestObject
        {
            public string Text { get; set; }

            [JsonConverter(typeof(StringGuidConverter))]
            public Guid Identifier { get; set; }

            [JsonConverter(typeof(StringGuidConverter))]
            public Guid? NullableIdentifier { get; set; }
        }
    }
}
