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
using PTV.Domain.Model.Models.OpenApi.V4;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for organization.
    /// </summary>
    public class OrganizationValidator : BaseValidator<IVmOpenApiOrganizationInVersionBase>
    {
        private LocalizedListValidator name;
        private MunicipalityCodeValidator municipality;
        private AddressListValidator addresses;
        private OrganizationIdValidator organizationId;
        private OidValidator oid;
        private PhoneNumberListValidator<V4VmOpenApiPhone> phones;
        private PublishingStatusValidator status;

        /// <summary>
        /// Ctor - organization validator
        /// </summary>
        /// <param name="model">Organization model</param>
        /// <param name="codeService">Code service</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="newLanguages">Languages that should be validated within lists</param>
        /// <param name="isCreateOperation">Indicates if organization is beeing inserted or updated.</param>
        public OrganizationValidator(IVmOpenApiOrganizationInVersionBase model, ICodeService codeService, IOrganizationService organizationService, IList<string> newLanguages, bool isCreateOperation = false)
            : base(model, "Organization")
        {
            name = new LocalizedListValidator(model.OrganizationNames, "OrganizationNames", newLanguages, new List<string>() { NameTypeEnum.Name.ToString() });
            municipality = new MunicipalityCodeValidator(model.Municipality, codeService);
            addresses = new AddressListValidator(model.Addresses, codeService);
            organizationId = new OrganizationIdValidator(model.ParentOrganizationId, organizationService, "ParentOrganizationId");
            oid = new OidValidator(model.Oid, organizationService, isCreateOperation: isCreateOperation, organizationId: model.Id, sourceId: model.SourceId);
            phones = new PhoneNumberListValidator<V4VmOpenApiPhone>(model.PhoneNumbers, codeService);
            status = new PublishingStatusValidator(model.PublishingStatus, model.CurrentPublishingStatus);
        }

        /// <summary>
        /// Checks if organization model is valid or not.
        /// </summary>
        public override void Validate(ModelStateDictionary modelState)
        {
            name.Validate(modelState);
            municipality.Validate(modelState);
            addresses.Validate(modelState);
            organizationId.Validate(modelState);
            oid.Validate(modelState);
            phones.Validate(modelState);
            status.Validate(modelState);
        }
    }
}
