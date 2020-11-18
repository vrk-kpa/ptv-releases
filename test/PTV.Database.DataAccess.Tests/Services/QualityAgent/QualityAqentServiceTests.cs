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
using System.IO;
using System.Linq;
using System.Net;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Services;
using PTV.Database.Model.Views;
using PTV.Domain.Model.Models.QualityAgent;
using PTV.Domain.Model.Models.QualityAgent.Output;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.QualityAgent
{
    public class QualityAqentServiceTests
    {
//        [Fact]
//        public void DeserializeINput()
//        {
//
//            var x = File.ReadAllText(@"d:\projects\vrk\Dev\test\PTV.Database.DataAccess.Tests\Services\QualityAgent\input.json");
//
//            var result = JsonConvert.DeserializeObject<VmQualityAgentOutput>(x);
//            result.Results.Should().NotBeNullOrEmpty();
////            JsonConvert.SerializeObject(result).Should().Be(x);
//            var f = JsonConvert.SerializeObject(result, Formatting.Indented);
//            File.WriteAllText(
//                @"d:\projects\vrk\Dev\test\PTV.Database.DataAccess.Tests\Services\QualityAgent\serialized.json", f );
//        }

//        [Fact]
//        public void DeserializeINput()
//        {
//            var x = File.ReadAllText(@"d:\projects\vrk\Dev\test\PTV.Database.DataAccess.Tests\Services\QualityAgent\lingsoftResult.json");
//            var result = JsonConvert.DeserializeObject<List<VmQualityAgentOutput>>(x);
//            result.Should().NotBeEmpty();
//            var all = result.SelectMany(op => op.Results).Select( op=> new VmQualityAgentValidation(op)).ToList();
//
//            all.Where(i => i.RuleId == "5").Should().HaveCount(2);
//            all.Where(i => i.RuleId == "5" && i.FieldId != "unknown").Should().HaveCount(2);
//        }
    }


}
