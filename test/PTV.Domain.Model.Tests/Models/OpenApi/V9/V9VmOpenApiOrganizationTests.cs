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

using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models.OpenApi.V9;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V9
{
    public class V9VmOpenApiOrganizationTests : ModelsTestBase
    {
        [Fact]
        public void CheckPropertiesTest()
        {
            var model = new V9VmOpenApiOrganization
            {
                PhoneNumbers = new List<V4VmOpenApiPhone> { new V4VmOpenApiPhone() },
                Emails = new List<V4VmOpenApiEmail> { new V4VmOpenApiEmail() },
                Addresses = new List<V9VmOpenApiAddress> { new V9VmOpenApiAddress() },
                WebPages = new List<V9VmOpenApiWebPage> { new V9VmOpenApiWebPage() },
                Services = new List<V5VmOpenApiOrganizationService> { new V5VmOpenApiOrganizationService() }
            };
            CheckProperties(model, 9);
            CheckProperties(model.PhoneNumbers.First(), 4);
            CheckProperties(model.Emails.First(), 4);
            CheckProperties(model.Addresses.First(), 9);
            CheckProperties(model.WebPages.First(), 9);
            CheckOrganizationServices(model.Services.First(), 5);
        }
    }
}
