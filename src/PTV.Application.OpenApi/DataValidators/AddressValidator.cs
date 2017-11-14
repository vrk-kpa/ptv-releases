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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V7;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for address.
    /// </summary>
    public class AddressValidator : BaseValidator<IV7VmOpenApiAddressInVersionBase>
    {
        private ICodeService codeService;

        /// <summary>
        /// Ctor - address validator.
        /// </summary>
        /// <param name="model">Address</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="codeService">Code service</param>
        public AddressValidator(IV7VmOpenApiAddressInVersionBase model, string propertyName, ICodeService codeService) : base(model, propertyName)
        {
            this.codeService = codeService;
        }

        /// <summary>
        /// Checks if address is valid or not.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            // Visiting address cannot be post office box
            if (Model.Type == AddressCharacterEnum.Visiting.ToString() && Model.SubType == AddressTypeEnum.PostOfficeBox.ToString())
            {
                modelState.AddModelError(PropertyName, "The field is invalid. 'SubType' cannot have value 'PostOfficeBox' when 'Type' = 'Visiting'.");
                return;
            }

            if (Model.SubType == AddressTypeEnum.PostOfficeBox.ToString())
            {
                if (Model.PostOfficeBoxAddress == null) return;

                // Validate municipality code
                var municipality = new MunicipalityCodeValidator(Model.PostOfficeBoxAddress.Municipality, codeService, $"{PropertyName}.PostOfficeBoxAddress.Municipality");
                municipality.Validate(modelState);

                // Validate postal code
                var postalCode = new PostalCodeValidator(Model.PostOfficeBoxAddress.PostalCode, codeService, $"{PropertyName}.PostOfficeBoxAddress.PostalCode");
                postalCode.Validate(modelState);
            }
            else if (Model.SubType == AddressTypeEnum.Street.ToString() || Model.SubType == AddressConsts.SINGLE)
            {
                if (Model.StreetAddress == null) return;

                // Validate municipality code
                var municipality = new MunicipalityCodeValidator(Model.StreetAddress.Municipality, codeService, $"{PropertyName}.StreetAddress.PostOfficeBoxAddress.Municipality");
                municipality.Validate(modelState);

                // Validate postal code
                var postalCode = new PostalCodeValidator(Model.StreetAddress.PostalCode, codeService, $"{PropertyName}.StreetAddress.PostalCode");
                postalCode.Validate(modelState);
            }            
        }
    }
}
