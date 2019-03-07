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
using FluentAssertions;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Converters.QualityAgent;
using PTV.Domain.Model.Models.QualityAgent.Output;
using Xunit;

namespace PTV.Domain.Model.Tests.Converters.QualityAgent
{
    public class QualityAgentProcessedConverterTests
    {
        [Fact]
        public void CheckSimpleModel()
        {
            var input = new
            {
                Data = new
                {
                    path = new[] {"serviceDescriptions", "type = Description", "language = sv"},
                    data = "some value"
                }
            };
            string json = JsonConvert.SerializeObject(input);
            var model = JsonConvert.DeserializeObject<TestConvertModel>(json);
            model.Should().NotBeNull();
            model.Data.Should().NotBeNull();
            model.Data.Data.Should().Be(input.Data.data);
            model.Data.Path.Should().Contain(new[] {"serviceDescriptions.Description.sv"});
            model.Data.Path.Should().HaveCount(1);
        }

        [Fact]
        public void CheckCompareModel()
        {
            var input = new
            {
                Data = new
                {
                    data = new
                    {
                        Compare1 = new
                        {
                            data = "field 1",
                            path = new[] {"serviceDescriptions", "type = Summary", "language = sv"}
                        },
                        Compare2 = new
                        {
                            data = "field 2",
                            path = new[] {"serviceDescriptions", "type = Description", "language = sv"}
                        }
                    }
                }
            };
            string json = JsonConvert.SerializeObject(input);
            var model = JsonConvert.DeserializeObject<TestConvertModel>(json);
            model.Should().NotBeNull();
            model.Data.Should().NotBeNull();
            model.Data.Data.Should().Be($"{input.Data.data.Compare1.data};{input.Data.data.Compare2.data}");
            model.Data.Path.Should().Contain(new[] {"serviceDescriptions.Description.sv", "serviceDescriptions.Summary.sv"});
            model.Data.Path.Should().HaveCount(2);
        }



        [Fact]
        public void CheckCompare3fieldsModel()
        {
            var input = new
            {
                Data = new
                {
                    data = new
                    {
                        Compare2 = new
                        {
                            data = "data 1",
                            path = new[]
                            {
                                "serviceDescriptions",
                                "type = Description",
                                "language = sv",
                                "+",
                                "requirements",
                                "language = sv",
                                "+",
                                "serviceDescriptions",
                                "type = UserInstruction",
                                "language = sv"
                            }
                        }
                    }
                }
            };

            string json = JsonConvert.SerializeObject(input);
            var model = JsonConvert.DeserializeObject<TestConvertModel>(json);
            model.Should().NotBeNull();
            model.Data.Should().NotBeNull();
            model.Data.Data.Should().Be(input.Data.data.Compare2.data);
            model.Data.Path.Should().Contain(new[]
            {
                "serviceDescriptions.Description.sv",
                "requirements.sv",
                "serviceDescriptions.UserInstruction.sv"
            });

            model.Data.Path.Should().HaveCount(3);
        }

    }

internal class TestConvertModel
    {
        [JsonConverter(typeof(QualityAgentProcessedConverter))]
        public VmQualityAgentProcessed Data { get; set; }
    }
}