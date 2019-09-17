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
    public class TimeStampDataHandlerTests
    {
        private TimeStampDataHandler handler;
        private string timeStampKey = "timestamp"; 

        public TimeStampDataHandlerTests()
        {
            
            handler = new TimeStampDataHandler();
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void AddTimeStampDataTest(string language, Dictionary<string, Dictionary<string, string>> data,
            bool keyAdded, Dictionary<string, string> options)
        {
            var dataMock = new Mock<ITranslationData>(MockBehavior.Strict);
            var optionsMock = new Mock<ITranslationOptions>(MockBehavior.Strict);
            dataMock
                .Setup(x => x.GetData(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string dataKey, string key) => data.TryGet(dataKey + key));            
            optionsMock
                .Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string key, string defaultValue) => options.TryGet(key) ?? defaultValue);

            var resultData = handler.Execute(language, dataMock.Object, optionsMock.Object);
            var languageData = resultData.GetData(language);
            if (keyAdded)
            {
                languageData.Should().ContainKey(timeStampKey);
                languageData[timeStampKey].Should().NotBeEmpty();
            }
            else
            {
                languageData?.Should().NotContainKey(timeStampKey);
            }
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
                    }                  
                },
                true,
                new Dictionary<string, string>
                {
                }
            };
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
                    }                  
                },
                false,
                new Dictionary<string, string>
                {
                    {"no-time", "no-time"}
                }
            };
            yield return new object[]
            {
                "nodata",
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
                    }                  
                },
                false,
                new Dictionary<string, string>
                {
                }
            };
        }
    }
}