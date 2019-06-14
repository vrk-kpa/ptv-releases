﻿/**
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
using PTV.Domain.Model.Models.OpenApi.V7;
using System.Linq;
using Xunit;
using System.Reflection;
using Newtonsoft.Json;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V5;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V7
{
    public class V7VmOpenApiOrganizationTests : ModelsTestBase
    {
        [Fact]
        public void ValidModel()
        {
            // Make sure Emails is shown as EmailAddresses
            var emails = typeof(V7VmOpenApiOrganization).GetProperties().SelectMany(p => p.GetCustomAttributes<JsonPropertyAttribute>()).FirstOrDefault(jp => jp.PropertyName == "emailAddresses");
            emails.Should().NotBeNull();

            // Make sure ParentOrganizationId is shown as ParentOrganization
            var org = typeof(V7VmOpenApiOrganization).GetProperties().SelectMany(p => p.GetCustomAttributes<JsonPropertyAttribute>()).FirstOrDefault(jp => jp.PropertyName == "parentOrganization");
            org.Should().NotBeNull();
        }

        [Fact]
        public void CheckPropertiesTest()
        {
            var model = new V7VmOpenApiOrganization
            {
                PhoneNumbers = new List<V4VmOpenApiPhone> { new V4VmOpenApiPhone() },
                Emails = new List<V4VmOpenApiEmail> { new V4VmOpenApiEmail() },
                Addresses = new List<V7VmOpenApiAddress> { new V7VmOpenApiAddress() },
                WebPages = new List<VmOpenApiWebPageWithOrderNumber> { new VmOpenApiWebPageWithOrderNumber() },
                Services = new List<V5VmOpenApiOrganizationService> { new V5VmOpenApiOrganizationService() }
            };
            CheckProperties(model, 7);
            CheckProperties(model.PhoneNumbers.First(), 4);
            CheckProperties(model.Emails.First(), 4);
            CheckProperties(model.Addresses.First(), 7);
            CheckProperties(model.WebPages.First(), 8);
            CheckOrganizationServices(model.Services.First(), 5);
        }
    }
}
