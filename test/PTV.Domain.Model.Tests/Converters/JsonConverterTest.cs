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
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Converters;
using Xunit;

namespace PTV.Domain.Model.Tests.Converters
{
    public class DictionaryToPropertyConverterTest
    {
        [Fact]
        public void ConvertObjectsTest()
        {
            var item = new ObjectListModel
            {
                Texts = new Dictionary<string, TestingConvertJson>
                {
                    {"x1", new TestingConvertJson { count = 1, Name = "sd", Desc="aaaa"} },
                    {"x2", new TestingConvertJson { Name = "y2", count = 5}},
                }
            };
            string json = JsonConvert.SerializeObject(item);
            json.Should().NotBeNullOrEmpty();

            var result = JsonConvert.DeserializeObject<Dictionary<string, TestingConvertJson>>(json);
            foreach (var text in item.Texts)
            {
                result.ContainsKey(text.Key).Should().BeTrue();
                var value = result[text.Key];
                value.Should().NotBeNull();
                value.Name.Should().Be(text.Value.Name);
                value.Desc.Should().Be(text.Value.Desc);
                value.count.Should().Be(text.Value.count);
            }
        }

        [Fact]
        public void ConvertStringsTest()
        {
            var item = new StringListModel
            {
                Name = "da",
                Texts = new Dictionary<string, string>
                {
                    {"x1", "aaaa" },
                    {"x2",  "y2" },
                }
            };
            string json = JsonConvert.SerializeObject(item);
            json.Should().NotBeNullOrEmpty();

            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            result.ContainsKey("Name").Should().BeTrue();
            result["Name"].Should().Be(item.Name);
            foreach (var text in item.Texts)
            {
                result.ContainsKey(text.Key).Should().BeTrue();
                var value = result[text.Key];
                value.Should().NotBeNull();
                value.Should().Be(text.Value);
            }
            var deserializedItem = JsonConvert.DeserializeObject<StringListModel>(json);
            deserializedItem.Should().NotBeNull();
            deserializedItem.Name.Should().Be(item.Name);
            deserializedItem.Texts.Should().BeNull();
        }
    }

    internal class StringListModel
    {
        public string Name { get; set; }
        [JsonConverter(typeof(DictionaryToPropertyConverter<string>))]
        public IDictionary<string, string> Texts { get; set; }


    }

    internal class ObjectListModel
    {
        [JsonConverter(typeof(DictionaryToPropertyConverter<TestingConvertJson>))]
        public IDictionary<string, TestingConvertJson> Texts { get; set; }


    }

    internal class TestingConvertJson
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public int count { get; set; }
    }
}