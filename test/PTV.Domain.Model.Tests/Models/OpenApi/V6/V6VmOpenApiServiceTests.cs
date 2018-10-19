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
using System.Linq;
using Xunit;
using System.Reflection;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.OpenApi.V6;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi;
using Newtonsoft.Json.Linq;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V6
{
    public class V6VmOpenApiServiceTests : ModelsTestBase
    {
        [Fact]
        public void ValidModel()
        {
            // Make sure GeneralDescriptionId is shown as statutoryServiceGeneralDescriptionId
            var property = typeof(V6VmOpenApiService).GetProperties().SelectMany(p => p.GetCustomAttributes<JsonPropertyAttribute>()).FirstOrDefault(jp => jp.PropertyName == "statutoryServiceGeneralDescriptionId");
            property.Should().NotBeNull();
        }

        [Fact]
        public void CheckProperties()
        {
            var model = new V6VmOpenApiService
            {
                ServiceChannels = new List<V6VmOpenApiServiceServiceChannel> { new V6VmOpenApiServiceServiceChannel() },
                Organizations = new List<V6VmOpenApiServiceOrganization> { new V6VmOpenApiServiceOrganization() },
                ServiceVouchers = new List<VmOpenApiServiceVoucher> { new VmOpenApiServiceVoucher() }
            };
            CheckProperties(model, 6);
            CheckServiceServiceChannel(model.ServiceChannels.First(), 6);
            var organizationProperties = JObject.Parse(JsonConvert.SerializeObject(model.Organizations.First(), Formatting.Indented)).ToIgnoreCaseDictionary();
            CheckProperties(organizationProperties, 6, "V6VmOpenApiServiceOrganization");
            CheckProperties(model.ServiceVouchers.First(), 8);
        }
    }
}
