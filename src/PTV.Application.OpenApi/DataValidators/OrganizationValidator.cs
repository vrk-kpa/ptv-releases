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
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for organization.
    /// </summary>
    public class OrganizationValidator : BaseValidator<IVmOpenApiOrganizationInVersionBase>
    {
        private ICodeService codeService;

        private LocalizedListValidator name;
        private MunicipalityCodeValidator municipality;
        private AddressListValidator<V9VmOpenApiAddressIn> addresses;
        private OrganizationIdValidator parentOrganizationId;
        private OidValidator oid;
        private PhoneNumberListValidator<V4VmOpenApiPhone> phones;
        private PublishingStatusValidator status;
        private AreaListValidator areas;
        private MunicipalityCodeListValidator municipalityList;
        private LocalizedListValidator description;
        private int versionNumber;
        private OrganizationIdValidator responsibleOrganizationId;

        /// <summary>
        /// Ctor - organization validator
        /// </summary>
        /// <param name="model">Organization model</param>
        /// <param name="codeService">Code service</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="commonService">Common service</param>
        /// <param name="newLanguages">Languages that should be validated within lists</param>
        /// <param name="availableLanguages">The languages that are available in main model and therefore need to be validated.</param>
        /// <param name="isCreateOperation">Indicates if organization is beeing inserted or updated.</param>
        /// <param name="versionNumber">Version number.</param>
        public OrganizationValidator(IVmOpenApiOrganizationInVersionBase model, ICodeService codeService, IOrganizationService organizationService, IList<string> newLanguages, IList<string> availableLanguages, ICommonService commonService, int versionNumber, bool isCreateOperation = false)
            : base(model, "Organization")
        {
            this.codeService = codeService;

            if (model == null)
            {
                throw new ArgumentNullException(PropertyName, $"{PropertyName} must be defined.");
            }

            name = new LocalizedListValidator(model.OrganizationNames, "OrganizationNames", newLanguages, new List<string>() { NameTypeEnum.Name.ToString() });
            municipality = new MunicipalityCodeValidator(model.Municipality, codeService);
            addresses = new AddressListValidator<V9VmOpenApiAddressIn>(model.Addresses, codeService);
            parentOrganizationId = new OrganizationIdValidator(model.ParentOrganizationId, commonService, "ParentOrganizationId");
            oid = new OidValidator(model.Oid, organizationService, isCreateOperation: isCreateOperation, organizationId: model.Id, sourceId: model.SourceId);
            phones = new PhoneNumberListValidator<V4VmOpenApiPhone>(model.PhoneNumbers, codeService);
            status = new PublishingStatusValidator(model.PublishingStatus, model.CurrentPublishingStatus);
            description = new LocalizedListValidator(model.OrganizationDescriptions, "OrganizationDescriptions", newLanguages, new List<string>() { DescriptionTypeEnum.ShortDescription.ToString() }, availableLanguages);
            responsibleOrganizationId = new OrganizationIdValidator(model.ResponsibleOrganizationId, commonService, "ResponsibleOrganizationId");
            this.versionNumber = versionNumber;
        }

        /// <summary>
        /// Validate organization model.
        /// </summary>
        public override void Validate(ModelStateDictionary modelState)
        {
            OrganizationTypeEnum? organizationType = null;

            if (!Model.OrganizationType.IsNullOrEmpty())
            {
                organizationType = Model.OrganizationType.Parse<OrganizationTypeEnum>();
            }

            name.Validate(modelState);

            // Validate municipality
            if (organizationType != OrganizationTypeEnum.Municipality && !Model.Municipality.IsNullOrEmpty())
            {
                modelState.AddModelError("Municipality", $"No Municipality accepted when OrganizationType has value {Model.OrganizationType}.");
            }
            else
            {
                municipality.Validate(modelState);
            }

            addresses.Validate(modelState);
            parentOrganizationId.Validate(modelState);
            oid.Validate(modelState);
            phones.Validate(modelState);
            status.Validate(modelState);

            // Validate organization descriptions: ShortDescription on is mandatory on versions 7+
            if (versionNumber >= 7)
            {
                description.Validate(modelState);
            }

            // Validate area data according to organization type
            switch (organizationType)
            {
                case OrganizationTypeEnum.Municipality:
                case OrganizationTypeEnum.TT1:
                case OrganizationTypeEnum.TT2:
                    // No area info accepted if organization type is Municipality, TT1 or TT2 - except default value areaType => WholeCountry!
                    if (!Model.AreaType.IsNullOrEmpty() && Model.AreaType != AreaInformationTypeEnum.WholeCountry.ToString())
                    {
                        modelState.AddModelError("AreaType", $"Value '{ Model.AreaType}' not accepted when OrganizationType has value {Model.OrganizationType}.");
                    }
                    if (!Model.SubAreaType.IsNullOrEmpty())
                    {
                        modelState.AddModelError("SubAreaType", $"No SubAreaType accepted when OrganizationType has value {Model.OrganizationType}.");
                    }
                    if (Model.Areas?.Count > 0)
                    {
                        modelState.AddModelError("Areas", $"No Areas accepted when OrganizationType has value {Model.OrganizationType}.");
                    }
                    break;
                case OrganizationTypeEnum.RegionalOrganization:
                    // Only AreaType is accepted if organization type is RegionalOrganization.
                    if (Model.AreaType != AreaInformationTypeEnum.AreaType.ToString())
                    {
                        modelState.AddModelError("AreaType", $"Value {AreaInformationTypeEnum.AreaType.ToString()} required when OrganizationType has value {Model.OrganizationType}.");
                    }

                    // Validate the area codes
                    ValidateAreas(modelState);

                    break;
                case null:
                    if (!Model.AreaType.IsNullOrEmpty() || !Model.SubAreaType.IsNullOrEmpty() || Model.Areas?.Count > 0)
                    {
                        modelState.AddModelError("OrganizationType", $"OrganizationType field is required when AreaType, SubAreaType or Areas has values.");
                    }
                    break;
                default:
                    if (Model.AreaType != AreaInformationTypeEnum.AreaType.ToString() && !Model.SubAreaType.IsNullOrEmpty())
                    {
                        modelState.AddModelError("SubAreaType", $"No SubAreaType accepted when AreaType has value {Model.AreaType}.");
                    }
                    if (Model.AreaType != AreaInformationTypeEnum.AreaType.ToString() && Model.Areas?.Count > 0)
                    {
                        modelState.AddModelError("Areas", $"No Areas accepted when AreaType has value {Model.AreaType}.");
                    }
                    // For State, Organization and Company types we are validating the area codes
                    ValidateAreas(modelState);
                    break;
            }

            // Validate responsible organization - only allowed for Sote organizations
            if (!string.IsNullOrEmpty(Model.ResponsibleOrganizationId))
            {
                if (organizationType == null)
                {
                    modelState.AddModelError("OrganizationType", $"OrganizationType field is required when ResponsibleOrganizationId has value.");
                }
                else if (organizationType != OrganizationTypeEnum.SotePrivate && organizationType != OrganizationTypeEnum.SotePublic)
                {
                    modelState.AddModelError("ResponsibleOrganizationId", $"No ResponsibleOrganizationId accepted when OrganizationType has value {Model.OrganizationType}.");
                }
                else
                {
                    responsibleOrganizationId.Validate(modelState);
                }
            }

            // Check that ValidTo is greater than ValidFrom (SFIPTV-190)
            if (Model.ValidFrom.HasValue && Model.ValidTo.HasValue && Model.ValidFrom.Value.Date >= Model.ValidTo.Value.Date)
            {
                modelState.AddModelError("ValidTo", $"Archiving date must be greater than publishing date.");
            }
        }

        private void ValidateAreas(ModelStateDictionary modelState)
        {
            if (Model.SubAreaType == AreaTypeEnum.Municipality.ToString())
            {
                municipalityList = new MunicipalityCodeListValidator(Model.Areas, codeService, "Areas");
                municipalityList.Validate(modelState);
            }
            else
            {
                areas = new AreaListValidator(Model.Areas, Model.SubAreaType, codeService);
                areas.Validate(modelState);
            }
        }
    }
}
