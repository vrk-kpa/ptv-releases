﻿/**
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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V8;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V8
{
    public class V8VmOpenApiElectronicChannelInTests : ModelsTestBase
    {
        [Fact]
        public void CheckPropertiesTest()
        {
            var model = new V8VmOpenApiElectronicChannelIn
            {
                Attachments = new List<VmOpenApiAttachment> { new VmOpenApiAttachment() },
                SupportPhones = new List<V4VmOpenApiPhone> { new V4VmOpenApiPhone() },
                SupportEmails = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem() },
                ServiceHours = new List<V8VmOpenApiServiceHour> { new V8VmOpenApiServiceHour { OpeningHour = new List<V8VmOpenApiDailyOpeningTime> { new V8VmOpenApiDailyOpeningTime() } } },
            };
            CheckProperties(model, 8);
            CheckProperties(model.Attachments.First());
            CheckProperties(model.SupportPhones.First(), 4);
            CheckProperties(model.SupportEmails.First());
            CheckServiceHourProperties(model.ServiceHours.First(), 8);
        }
    }
}
