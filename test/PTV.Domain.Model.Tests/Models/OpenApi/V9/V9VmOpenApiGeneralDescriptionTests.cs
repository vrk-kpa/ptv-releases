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

using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V9;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V9
{
    public class V9VmOpenApiGeneralDescriptionTests : ModelsTestBase
    {
        [Fact]
        public void CheckProperties()
        {
            var model = new V9VmOpenApiGeneralDescription
            {
                Legislation = new List<V4VmOpenApiLaw> { new V4VmOpenApiLaw() },
                ServiceChannels = new List<V6VmOpenApiServiceServiceChannel> { new V6VmOpenApiServiceServiceChannel() }
            };
            CheckProperties(model, 9);
            CheckProperties(model.Legislation.First(), 4);
            CheckServiceServiceChannel(model.ServiceChannels.First(), 6);
        }
    }
}
