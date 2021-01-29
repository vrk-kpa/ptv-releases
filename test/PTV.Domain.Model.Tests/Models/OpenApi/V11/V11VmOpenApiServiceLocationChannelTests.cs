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

using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V11
{
    public class V11VmOpenApiServiceLocationChannelTests : ModelsTestBase
    {
        [Fact]
        public void CheckPropertiesTest()
        {
            var model = new V11VmOpenApiServiceLocationChannel
            {
                PhoneNumbers = new List<V4VmOpenApiPhoneWithType> { new V4VmOpenApiPhoneWithType() },
                SupportEmails = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem() },
                WebPages = new List<V9VmOpenApiWebPage> { new V9VmOpenApiWebPage() },
                Addresses = new List<V9VmOpenApiAddressLocation> { new V9VmOpenApiAddressLocation() },
                ServiceHours = new List<V11VmOpenApiServiceHour> { new V11VmOpenApiServiceHour { OpeningHour = new List<V8VmOpenApiDailyOpeningTime> { new V8VmOpenApiDailyOpeningTime() } } },
                Services = new List<V11VmOpenApiServiceChannelService> { new V11VmOpenApiServiceChannelService() },
                ServiceCollections = new List<VmOpenApiServiceServiceCollection> { new VmOpenApiServiceServiceCollection() },
            };
            CheckProperties(model, 11);
            CheckProperties(model.PhoneNumbers.First(), 4);
            CheckProperties(model.SupportEmails.First());
            CheckProperties(model.WebPages.First(), 9);
            CheckProperties(model.Addresses.First(), 9);
            CheckServiceHourProperties(model.ServiceHours.First(), 11);
            CheckServiceServiceChannel(model.Services.First(), 11);
            CheckServiceServiceCollection(model.ServiceCollections.First(), 11);
        }
    }
}
