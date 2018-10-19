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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models.OpenApi.V6;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V6
{
    public class V6VmOpenApiPrintableFormChannelTests : ModelsTestBase
    {
        [Fact]
        public void CheckProperties()
        {
            var model = new V6VmOpenApiPrintableFormChannel
            {
                DeliveryAddress = new V5VmOpenApiAddressWithCoordinates(),
                ChannelUrls = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem() },
                Attachments = new List<VmOpenApiAttachmentWithType> { new VmOpenApiAttachmentWithType() },
                SupportEmails = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem() },
                SupportPhones = new List<V4VmOpenApiPhone> { new V4VmOpenApiPhone() },
                ServiceHours = new List<V4VmOpenApiServiceHour> { new V4VmOpenApiServiceHour { OpeningHour = new List<V2VmOpenApiDailyOpeningTime> { new V2VmOpenApiDailyOpeningTime() } } },
            };
            CheckProperties(model, 6);
            var addressProperties = JObject.Parse(JsonConvert.SerializeObject(model.DeliveryAddress, Formatting.Indented)).ToIgnoreCaseDictionary();
            CheckProperties(addressProperties, 5, model.DeliveryAddress.GetType().Name);
            CheckListItemProperties(model.ChannelUrls.First(), 8);
            CheckProperties(model.Attachments.First(), 8);
            CheckProperties(model.SupportEmails.First(), 8);
            CheckProperties(model.SupportPhones.First(), 4);
            CheckServiceHourProperties(model.ServiceHours.First(), 4);
        }
    }
}
