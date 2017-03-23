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
