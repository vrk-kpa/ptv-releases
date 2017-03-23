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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service location channel.
    /// </summary>
    public class ServiceLocationChannelValidator : ServiceChannelValidator<VmOpenApiServiceLocationChannelInVersionBase>
    {
        private MunicipalityCodeListValidator areas;
        private PhoneNumberListValidator<V4VmOpenApiPhone> phones;
        private PhoneNumberListValidator<V4VmOpenApiPhoneSimple> faxNumbers;
        private AddressListValidator addresses;

        /// <summary>
        /// Ctor - service location channel validator
        /// </summary>
        /// <param name="model">service location channel model</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="codeService">Code service</param>
        public ServiceLocationChannelValidator(VmOpenApiServiceLocationChannelInVersionBase model, IOrganizationService organizationService, ICodeService codeService) :
            base(model, "ServiceLocationChannel", organizationService, codeService)
        {
            areas = new MunicipalityCodeListValidator(model.ServiceAreas, codeService, "ServiceAreas");
            phones = new PhoneNumberListValidator<V4VmOpenApiPhone>(model.PhoneNumbers, codeService);
            faxNumbers = new PhoneNumberListValidator<V4VmOpenApiPhoneSimple>(model.FaxNumbers, codeService, "FaxNumbers");
            addresses = new AddressListValidator(model.Addresses, codeService);
        }

        /// <summary>
        /// Validates service location channel model
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            base.Validate(modelState);

            areas.Validate(modelState);
            phones.Validate(modelState);
            faxNumbers.Validate(modelState);
            addresses.Validate(modelState);
        }
    }
}
