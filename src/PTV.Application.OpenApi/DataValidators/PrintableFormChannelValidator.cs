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

using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V7;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for printable form channel.
    /// </summary>
    public class PrintableFormChannelValidator : ServiceChannelValidator<VmOpenApiPrintableFormChannelInVersionBase>
    {
        private AddressValidator address;

        /// <summary>
        /// Ctor - printable form channel validator
        /// </summary>
        /// <param name="model">printable form channel model</param>
        /// <param name="commonService">Common service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="serviceService">Service service</param>
        public PrintableFormChannelValidator(VmOpenApiPrintableFormChannelInVersionBase model, ICommonService commonService, ICodeService codeService, IServiceService serviceService) :
            base(model, "PrintableFormChannel", commonService, codeService, serviceService)
        {
            address = new AddressValidator(model.DeliveryAddress == null ? null : new V7VmOpenApiAddressInVersionBase
            {
                SubType = model.DeliveryAddress.SubType,
                StreetAddress = (model.DeliveryAddress.SubType == AddressTypeEnum.Street.ToString() && model.DeliveryAddress.StreetAddress != null) ? 
                    new VmOpenApiAddressStreetWithCoordinatesIn
                    {
                        PostalCode = model.DeliveryAddress.StreetAddress.PostalCode,
                        Municipality = model.DeliveryAddress.StreetAddress.Municipality
                    } : null,
                PostOfficeBoxAddress = (model.DeliveryAddress.SubType == AddressTypeEnum.PostOfficeBox.ToString() && model.DeliveryAddress.PostOfficeBoxAddress != null) ?
                    new VmOpenApiAddressPostOfficeBoxIn
                    {
                        PostalCode = model.DeliveryAddress.PostOfficeBoxAddress.PostalCode,
                        Municipality = model.DeliveryAddress.PostOfficeBoxAddress.Municipality
                    } : null
            }, "DeliveryAddress", codeService);
        }
        /// <summary>
        /// Validates printable form channel model.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            base.Validate(modelState);

            address.Validate(modelState);

            // Validate required channelUrls property
            var urls = new LocalizedListValidator(Model.ChannelUrls, "ChannelUrls", requiredLanguages: RequiredLanguages, availableLanguages: AvailableLanguages);
            urls.Validate(modelState);
        }
    }
}
