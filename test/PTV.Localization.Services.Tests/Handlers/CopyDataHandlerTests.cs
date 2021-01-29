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
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using PTV.Framework;
using PTV.Localization.Services.Model;
using Xunit;

namespace PTV.Localization.Services.Handlers
{
    public class CopyDataHandlerTests
    {
        private CopyDataHandler handler;
        private const string Keys = "testkey";
        private const string DefaultsKey = "copyData";

        public CopyDataHandlerTests()
        {

            handler = new CopyDataHandler();
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void CopyDataTest(string language, Dictionary<string, Dictionary<string, string>> data,
            Dictionary<string, string> expected)
        {
            var dataMock = new Mock<ITranslationData>(MockBehavior.Strict);
            dataMock
                .Setup(x => x.GetData(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string dataKey, string key) => data.TryGet(dataKey + key));

            handler.Keys = Keys;
            handler.DefaultsKey = DefaultsKey;
            var resultData = handler.Execute(language, dataMock.Object, null);
            var languageData = resultData.GetData(language);
            if (expected == null)
            {
                languageData.Should().BeNull();
            }
            else
            {
                languageData.Should().HaveCount(expected.Count);
                languageData.Should().ContainValues(expected.Values);
                languageData.Should().ContainKeys(expected.Keys);
            }
        }

        [Fact]
        public void UpdateKeyNotFound()
        {
            var language = "fi";
            var data = new Dictionary<string, Dictionary<string, string>>
            {
                {
                    "fi",
                    new Dictionary<string, string>
                    {
                        {"k1", "some1"},
                        {"k2", "some2"},
                        {"k3", ""}

                    }
                },
                {
                    $"fi{DefaultsKey}",
                    new Dictionary<string, string>
                    {
                        {"k1", "text1"},
                        {"k2", "text2"},
                        {"k3", "text3"}

                    }
                },
                {
                    Keys,
                    new Dictionary<string, string>
                    {
                        {"k4", "k1"},
                    }
                }
            };
            var dataMock = new Mock<ITranslationData>(MockBehavior.Strict);
            dataMock
                .Setup(x => x.GetData(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string dataKey, string key) => data.TryGet(dataKey + key));

            handler.Keys = Keys;
            handler.DefaultsKey = DefaultsKey;
            Action call = () => handler.Execute(language, dataMock.Object, null);
            call.Should().Throw<KeyNotFoundException>();

        }

        public static IEnumerable<object[]> GetData()
        {
            yield return new object[]
            {
                "fi",
                new Dictionary<string, Dictionary<string, string>>
                {
                    {
                        "fi",
                        new Dictionary<string, string>
                        {
                            {"k1", "some1"},
                            {"k2", "some2"},
                            {"k3", ""}

                        }
                    },                    {
                        $"fi{DefaultsKey}",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "text2"},
                            {"k3", "text3"}

                        }
                    },
                    {
                        Keys,
                        new Dictionary<string, string>
                        {
                            {"k3", "k1"},
                            {"k2", "k1"}
                        }
                    }
                },
                new Dictionary<string, string>
                {
                    {"k1", "some1"},
                    {"k2", "some2"},
                    {"k3", "text1"}
                },
            };
            yield return new object[]
            {
                "key not exists",
                new Dictionary<string, Dictionary<string, string>>
                {
                    {
                        "key not exists",
                        new Dictionary<string, string>
                        {
                            {"k1", "some1"},
                            {"k2", "some2"},
                            {"k3", ""}

                        }
                    },                    {
                        $"key not exists{DefaultsKey}",
                        new Dictionary<string, string>
                        {
                            {"k2", "text2"},
                            {"k3", "text3"}

                        }
                    },
                    {
                        Keys,
                        new Dictionary<string, string>
                        {
                            {"k3", "k1"},
                            {"k2", "k1"}
                        }
                    }
                },
                new Dictionary<string, string>
                {
                    {"k1", "some1"},
                    {"k2", "some2"},
                    {"k3", ""}
                },
            };
            yield return new object[]
            {
                "languageMissing",
                new Dictionary<string, Dictionary<string, string>>
                {
                    {
                        $"languageMissing{DefaultsKey}",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "text2"},
                            {"k3", "text3"}

                        }
                    },
                    {
                        Keys,
                        new Dictionary<string, string>
                        {
                            {"k3", "k1"},
                            {"k2", "k1"}
                        }
                    }
                },
                null
            };
            yield return new object[]
            {
                "keys missing",
                new Dictionary<string, Dictionary<string, string>>
                {
                    {
                        "keys missing",
                        new Dictionary<string, string>
                        {
                            {"k1", "some1"},
                            {"k2", "some2"},
                            {"k3", ""}

                        }
                    },                    {
                        $"keys missing{DefaultsKey}",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "text2"},
                            {"k3", "text3"}

                        }
                    }
                },
                new Dictionary<string, string>
                {
                    {"k1", "some1"},
                    {"k2", "some2"},
                    {"k3", ""}
                },
            };
            yield return new object[]
            {
                "defaults missing",
                new Dictionary<string, Dictionary<string, string>>
                {
                    {
                        "defaults missing",
                        new Dictionary<string, string>
                        {
                            {"k1", "some1"},
                            {"k2", "some2"},
                            {"k3", ""}

                        }
                    },
                    {
                        Keys,
                        new Dictionary<string, string>
                        {
                            {"k3", "k1"},
                            {"k2", "k1"}
                        }
                    }
                },
                new Dictionary<string, string>
                {
                    {"k1", "some1"},
                    {"k2", "some2"},
                    {"k3", ""}
                },
            };
        }
    }
}
