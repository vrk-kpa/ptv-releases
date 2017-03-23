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
using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for address.
    /// </summary>
    public class AddressValidator : BaseValidator<IV4VmOpenApiAddressIn>
    {
        private ICodeService codeService;

        /// <summary>
        /// Ctor - address validator.
        /// </summary>
        /// <param name="model">Address</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="codeService">Code service</param>
        public AddressValidator(IV4VmOpenApiAddressIn model, string propertyName, ICodeService codeService) : base(model, propertyName)
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

            // Validate municipality code
            var municipality = new MunicipalityCodeValidator(Model.Municipality, codeService, $"{PropertyName}.Municipality");
            municipality.Validate(modelState);

            // Validate country code
            var country = new CountryCodeValidator(Model.Country, codeService, $"{PropertyName}.Country");
            country.Validate(modelState);

            // Validate postal code
            var postalCode = new PostalCodeValidator(Model.PostalCode, codeService, $"{PropertyName}.PostalCode");
            postalCode.Validate(modelState);
        }
    }
}
