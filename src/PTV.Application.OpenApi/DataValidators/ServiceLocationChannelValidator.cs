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
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using System;
using System.Linq;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service location channel.
    /// </summary>
    public class ServiceLocationChannelValidator : ServiceChannelValidator<VmOpenApiServiceLocationChannelInVersionBase>
    {
        private PhoneNumberListValidator<V4VmOpenApiPhone> phones;
        private PhoneNumberListValidator<V4VmOpenApiPhoneSimple> faxNumbers;
        private AddressListValidator<V7VmOpenApiAddressWithMovingIn> addresses;

        /// <summary>
        /// Ctor - service location channel validator
        /// </summary>
        /// <param name="model">service location channel model</param>
        /// <param name="commonService">Common service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="serviceService">Service service</param>
        public ServiceLocationChannelValidator(VmOpenApiServiceLocationChannelInVersionBase model, ICommonService commonService, ICodeService codeService, IServiceService serviceService) :
            base(model, "ServiceLocationChannel", commonService, codeService, serviceService)
        {
            phones = new PhoneNumberListValidator<V4VmOpenApiPhone>(model.PhoneNumbers, codeService);
            faxNumbers = new PhoneNumberListValidator<V4VmOpenApiPhoneSimple>(model.FaxNumbers, codeService, "FaxNumbers");
            addresses = new AddressListValidator<V7VmOpenApiAddressWithMovingIn>(model.Addresses, codeService);
        }

        /// <summary>
        /// Validates service location channel model
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            base.Validate(modelState);

            phones.Validate(modelState);
            faxNumbers.Validate(modelState);

            // Validate addresses
            // validation rules, see PTV-2910
            var i = 0;
            Model.Addresses.ForEach(a =>
            {
                if ((a.Type == AddressConsts.LOCATION && (a.SubType == AddressTypeEnum.Street.ToString() || a.SubType == AddressTypeEnum.PostOfficeBox.ToString())) ||
                (a.Type == AddressCharacterEnum.Postal.ToString() && (a.SubType == AddressConsts.SINGLE || a.SubType == AddressConsts.MULTIPOINT)))
                {
                    modelState.AddModelError($"Addresses[{i}].SubType", $"'{a.SubType}' is not allowed value of 'SubType' when 'Type' has value '{ a.Type}'.");
                }
                i++;
            });

            // if subType is Single only one other address (with same type) can be added - PTV-2910
            var singleAddresses = Model.Addresses.Where(a => a.Type == AddressConsts.LOCATION && a.SubType == AddressConsts.SINGLE).ToList();
            if (singleAddresses?.Count > 0)
            {
                if (Model.Addresses.Any(a => a.Type == AddressConsts.LOCATION && a.SubType != AddressConsts.SINGLE))
                {
                    modelState.AddModelError($"Addresses", $"The field is invalid. When 'SubType' has value '{ AddressConsts.SINGLE }' no other types of { AddressConsts.LOCATION } addresses can be defined.");
                }

                if (singleAddresses.Count > 2)
                {
                    modelState.AddModelError($"Addresses", $"The field is invalid. You can only add two addresses with 'SubType' as '{ AddressConsts.SINGLE }'.");
                }
            }

            // if address list includes foreign addresses no other address types are allowed - PTV-2910
            var foreignAddresses = Model.Addresses.Where(a => a.SubType == AddressConsts.ABROAD).ToList();
            foreignAddresses.GroupBy(a => a.Type).ToDictionary(a => a.Key, a => a.ToList()).ForEach(fa =>
            {
                // multible foreign addresses?
                if (fa.Value.Count > 1)
                {
                    modelState.AddModelError($"Addresses", $"The field is invalid. Only one address with 'SubType' = '{ AddressConsts.ABROAD }' and 'Type' = '{ fa.Key }' is allowed.");
                }

                // other sub types defined?
                if (Model.Addresses.Any(a => a.Type == fa.Key && a.SubType != AddressConsts.ABROAD))
                {
                    modelState.AddModelError($"Addresses", $"The field is invalid. When 'SubType' has value '{ AddressConsts.ABROAD }' no other types of { fa.Key } addresses can be defined.");
                }
                
            });
            
            // if address list includes moving address other type of visiting addresses cannot exist
            if (Model.Addresses.Any(a => a.SubType == AddressConsts.MULTIPOINT))
            {
                if (Model.Addresses.Any(a => a.Type == AddressConsts.LOCATION &&  a.SubType != AddressConsts.MULTIPOINT))
                {
                    modelState.AddModelError("Addresses", $"The field is invalid. When 'SubType' has value '{ AddressConsts.MULTIPOINT }' no other types of { AddressConsts.LOCATION } addresses can be defined.");
                }
            }
            addresses.Validate(modelState);
        }
    }
}
