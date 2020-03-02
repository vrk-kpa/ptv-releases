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

using PTV.Framework.Interfaces;
using Microsoft.Extensions.Logging;
using Xunit;
using FluentAssertions;

namespace PTV.Framework.Tests
{
    public class TextManagerTest
    {
        private const string TEXT = "TEST TEXT";
        private const string TEXT_MARKDOWN = TEXT + "\n";
        private const string TEXT_JSON = "{\"entityMap\":{},\"blocks\":[{\"text\":\"" + TEXT + "\",\"type\":\"unstyled\",\"key\":\"VVMA5\",\"depth\":0,\"inlineStyleRanges\":[],\"entityRanges\":[]},{\"text\":\"\",\"type\":\"unstyled\",\"key\":\"LSBZU\",\"depth\":0,\"inlineStyleRanges\":[],\"entityRanges\":[]}]}";
        private const string TEXT_WRONG_JSON = "entityMap:{},blocks:{}";

        protected ITextManager TextManager { get; set; }
        protected ILoggerFactory LoggerFactory { get; private set; }

        public TextManagerTest()
        {
            LoggerFactory = new LoggerFactory();
            this.TextManager = new TextManager.TextManager();
        }

        [Theory]
        [InlineData(TEXT_MARKDOWN, TEXT_JSON)]
        public void ConvertFromJsonToMarkDown(string expected, string json)
        {
            var result = TextManager.ConvertToMarkDown(json);
            CheckResult(expected, result);
        }

        [Theory]
        [InlineData(TEXT, TEXT_JSON)]
        public void ConvertFromJsonToPureText(string expected, string json)
        {
            var result = TextManager.ConvertToPureText(json);
            CheckResult(expected, result);
        }

        [Theory]
        [InlineData("", TEXT_WRONG_JSON)]
        public void ConvertFromWrongJsonToPureText(string expected, string json)
        {
            var result = TextManager.ConvertToPureText(json);
            CheckResult(expected, result);
        }

        private void CheckResult(string expected, string target)
        {
            target.Should().NotBeNull();
            target.Should().Be(expected);
        }

        [Theory]
        [InlineData(TEXT, TEXT_MARKDOWN)]
        public void ConvertMarkDownToJson(string expected, string json)
        {
            var result = TextManager.ConvertMarkdownToJson(json);
            var isResultJson = IsJson(result);

            result.Should().NotBeNull();
            result.Should().NotBeSameAs(expected);
            result.Should().Contain(expected);
        }

        [Theory]
        [InlineData(TEXT_MARKDOWN)]
        public void ConvertMarkDownToJsonIsJson(string json)
        {
            var result = TextManager.ConvertMarkdownToJson(json);
            var isResultJson = IsJson(result);
            isResultJson.Should().BeTrue();
        }

        private bool IsJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            return (json.StartsWith("{") && json.EndsWith("}")) || (json.StartsWith("[") && json.EndsWith("]"));
        }
    }
}
