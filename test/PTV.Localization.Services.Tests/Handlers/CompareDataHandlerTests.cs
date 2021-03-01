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
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using PTV.Localization.Services.Model;
using Xunit;

namespace PTV.Localization.Services.Handlers
{
    public class CompareDataHandlerTests
    {
        private CompareDataHandler handler;

        public CompareDataHandlerTests()
        {
            handler = new CompareDataHandler();
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void CompareDataTest(string language, Dictionary<string, Dictionary<string, string>> data,
            bool isDifferent, bool compareContent)
        {
            var nextMock = new Mock<ITranslationDataHandler>(MockBehavior.Strict);
            var nextDataMock = new Mock<ITranslationData>(MockBehavior.Strict);
            nextMock.Setup(x => x.Execute(language, It.IsAny<ITranslationData>(), null)).Returns(nextDataMock.Object);
            handler.SetNext(nextMock.Object);

            var dataMock = new Mock<ITranslationData>(MockBehavior.Strict);
            dataMock
                .Setup(x => x.GetData(It.IsAny<string>(), null))
                .Returns((string dataKey, string keyEmpty) => data[dataKey]);

            handler.GetCompareDataKey1 = lang => lang + "d1";
            handler.GetCompareDataKey2 = lang => lang + "d2";
            handler.CompareContent = compareContent;
            var resultData = handler.Execute(language, dataMock.Object, null);

            nextMock.Verify
            (
                x => x.Execute(It.IsAny<string>(), It.IsAny<ITranslationData>(), It.IsAny<ITranslationOptions>()),
                () => isDifferent ? Times.Once() : Times.Never()
            );

            if (isDifferent)
            {
                resultData.Should().NotBe(dataMock.Object);
                resultData.Should().Be(nextDataMock.Object);
                handler.Next.Should().NotBeNull();
                handler.Next.Should().Be(nextMock.Object);
            }
            else
            {
                resultData.Should().Be(dataMock.Object);
                handler.Next.Should().BeNull();
            }
            resultData.Should().NotBeNull();

        }

        public static IEnumerable<object[]> GetData()
        {
            yield return new object[]
            {
                "same",
                new Dictionary<string, Dictionary<string, string>>
                {
                    {
                        "samed1",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "k2"},
                            {"k3", ""}

                        }
                    },
                    {
                        "samed2",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "k2"},
                            {"k3", ""}

                        }
                    }
                },
                false,
                true
            };
            yield return new object[]
            {
                "same",
                new Dictionary<string, Dictionary<string, string>>
                {
                    {
                        "samed1",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "k2"},
                            {"k3", ""}

                        }
                    },
                    {
                        "samed2",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "k2"},
                            {"k3", ""}

                        }
                    }
                },
                false,
                false
            };
            yield return new object[]
            {
                "count",
                new Dictionary<string, Dictionary<string, string>>
                {
                    {
                        "countd1",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "k2"}
                        }
                    },
                    {
                        "countd2",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "k2"},
                            {"k3", ""}

                        }
                    }
                },
                true,
                false
            };
            yield return new object[]
            {
                "values different",
                new Dictionary<string, Dictionary<string, string>>
                {
                    {
                        "values differentd1",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "k2"},
                            {"k3", "k3"}
                        }
                    },
                    {
                        "values differentd2",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "k2"},
                            {"k3", ""}

                        }
                    }
                },
                false,
                false
            };
            yield return new object[]
            {
                "values different",
                new Dictionary<string, Dictionary<string, string>>
                {
                    {
                        "values differentd1",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "k2"},
                            {"k3", "k3"}
                        }
                    },
                    {
                        "values differentd2",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "k2"},
                            {"k3", ""}

                        }
                    }
                },
                true,
                true
            };
            yield return new object[]
            {
                "key different",
                new Dictionary<string, Dictionary<string, string>>
                {
                    {
                        "key differentd1",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "k2"},
                            {"k4", "k3"}
                        }
                    },
                    {
                        "key differentd2",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "k2"},
                            {"k3", ""}

                        }
                    }
                },
                true,
                false
            };
            yield return new object[]
            {
                "key different",
                new Dictionary<string, Dictionary<string, string>>
                {
                    {
                        "key differentd1",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "k2"},
                            {"k4", "k3"}
                        }
                    },
                    {
                        "key differentd2",
                        new Dictionary<string, string>
                        {
                            {"k1", "text1"},
                            {"k2", "k2"},
                            {"k3", ""}

                        }
                    }
                },
                true,
                true
            };
        }
    }
}
