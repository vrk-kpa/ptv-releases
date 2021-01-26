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
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using PTV.Localization.Services.Model;
using Xunit;

namespace PTV.Localization.Services.Handlers
{
    public class CleanEmptyDataHandlerTests
    {
        private CleanEmptyDataHandler handler;

        public CleanEmptyDataHandlerTests()
        {
            handler = new CleanEmptyDataHandler();
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void CleanDataTest(string language, Dictionary<string, string> data, ITranslationOptions options, string[] expected)
        {
            var expectedValues = data != null ? expected.Select(x => data[x]) : null;
            var dataMock = new Mock<ITranslationData>(MockBehavior.Strict);
            dataMock.Setup(x => x.GetData(language, null)).Returns(data);
            var resultData = handler.Execute(language, dataMock.Object, options);
            resultData.Should().NotBeNull();
            if (data != null)
            {
                data.Should().HaveCount(expected.Length);
                data.Should().ContainValues(expectedValues);
                data.Should().ContainKeys(expected);
            }
        }

        public static IEnumerable<object[]> GetData()
        {
            yield return new object[]
            {
                "fi",
                new Dictionary<string, string>
                {
                    {"k1", "text1"},
                    {"k2", "k2"},
                    {"k3", ""}

                },
                new TranslationOptions(),
                new [] { "k1" }
            };
            yield return new object[]
            {
                "fi",
                new Dictionary<string, string>
                {
                    {"k1", "text1"},
                    {"k2", "k2"},
                    {"k3", ""}

                },
                new TranslationOptions
                {
                    {"translated", ""}
                },
                new[] {"k1"}
            };
            yield return new object[]
            {
                "fi",
                new Dictionary<string, string>
                {
                    {"k1", "text1"},
                    {"k2", "k2"},
                    {"k3", ""}

                },
                new TranslationOptions
                {
                    { "translated", "on" }
                },
                new [] { "k3" }
            };         
            yield return new object[]
            {
                "fi",
                null,
                new TranslationOptions(),
                null
            };
        }
    }
}
