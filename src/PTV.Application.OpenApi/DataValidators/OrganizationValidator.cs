/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for organization.
    /// </summary>
    public class OrganizationValidator : TimedPublishingBaseValidator<IVmOpenApiOrganizationInVersionBase>
    {
        private readonly ICodeService codeService;

        private LocalizedListValidator name;
        private LocalizedListValidator description;

        private readonly MunicipalityCodeValidator municipality;
        private readonly AddressListValidator<V9VmOpenApiAddressIn> addresses;
        private readonly OrganizationIdValidator parentOrganizationId;
        private readonly OidValidator oid;
        private readonly PhoneNumberListValidator<V4VmOpenApiPhone> phones;
        private readonly PublishingStatusValidator status;
        private AreaListValidator areas;
        private MunicipalityCodeListValidator municipalityList;
        private readonly int versionNumber;
        private readonly IList<string> currentAvailableLanguages;

        // SOTE has been disabled (SFIPTV-1177)
        //        private OrganizationIdValidator responsibleOrganizationId;

        /// <summary>
        /// Ctor - organization validator
        /// </summary>
        /// <param name="model">Organization model</param>
        /// <param name="codeService">Code service</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="commonService">Common service</param>
        /// <param name="currentAvailableLanguages">Language versions that are published currently</param>
        /// <param name="isCreateOperation">Indicates if organization is being inserted or updated.</param>
        /// <param name="versionNumber">Version number.</param>
        public OrganizationValidator(
            IVmOpenApiOrganizationInVersionBase model,
            ICodeService codeService,
            IOrganizationService organizationService,
            IList<string> currentAvailableLanguages,
            ICommonService commonService,
            int versionNumber,
            bool isCreateOperation = false)
            : base(model, currentAvailableLanguages, "Organization")
        {
            this.codeService = codeService;
            this.currentAvailableLanguages = currentAvailableLanguages;

            municipality = new MunicipalityCodeValidator(model.Municipality, codeService);
            addresses = new AddressListValidator<V9VmOpenApiAddressIn>(model.Addresses, codeService);
            parentOrganizationId = new OrganizationIdValidator(model.ParentOrganizationId, commonService, "ParentOrganizationId");
            oid = new OidValidator(model.Oid, organizationService, commonService, isCreateOperation: isCreateOperation, organizationId: model.Id, sourceId: model.SourceId);
            phones = new PhoneNumberListValidator<V4VmOpenApiPhone>(model.PhoneNumbers, codeService);
            status = new PublishingStatusValidator(model.PublishingStatus, model.CurrentPublishingStatus);

            // SOTE has been disabled (SFIPTV-1177)
            //            responsibleOrganizationId = new OrganizationIdValidator(model.ResponsibleOrganizationId, commonService, "ResponsibleOrganizationId");

            this.versionNumber = versionNumber;
        }

        /// <summary>
        /// Validate organization model.
        /// </summary>
        public override void Validate(ModelStateDictionary modelState)
        {
            base.Validate(modelState);

            OrganizationTypeEnum? organizationType = null;

            if (!Model.OrganizationType.IsNullOrEmpty())
            {
                organizationType = Model.OrganizationType.Parse<OrganizationTypeEnum>();
            }

            name = new LocalizedListValidator(Model.OrganizationNames, "OrganizationNames", RequiredLanguages, new List<string> { NameTypeEnum.Name.ToString() }, Model.AvailableLanguages);
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

/* SOTE has been disabled (SFIPTV-1177)
            if ((organizationType == OrganizationTypeEnum.SotePublic) || (organizationType == OrganizationTypeEnum.SotePrivate))
            {
                var parentOrgId = Model.ParentOrganizationId.ParseToGuid();
                if (!parentOrgId.IsAssigned())
                {
                    modelState.AddModelError("SoteOrganization", $"When OrganizationType is {Model.OrganizationType} type, parent organization must be specified.");
                }
            }
*/
            addresses.Validate(modelState);
            parentOrganizationId.Validate(modelState);
            oid.Validate(modelState);
            phones.Validate(modelState);
            status.Validate(modelState);

            // Validate organization descriptions: ShortDescription is mandatory on versions 7+
            if (versionNumber >= 7)
            {
                description = new LocalizedListValidator(Model.OrganizationDescriptions, "OrganizationDescriptions", RequiredLanguages, new List<string> { DescriptionTypeEnum.ShortDescription.GetOpenApiValue() }, Model.AvailableLanguages);
                description.Validate(modelState);
            }

            // Validate area data according to organization type
            AreaInformationTypeEnum? areaType = Model.AreaType.IsNullOrEmpty() ? (AreaInformationTypeEnum?)null: (AreaInformationTypeEnum)Model.AreaType.GetEnumByOpenApiEnumValue(typeof(AreaInformationTypeEnum));

            switch (organizationType)
            {
                case OrganizationTypeEnum.Municipality:
                case OrganizationTypeEnum.TT1:
                case OrganizationTypeEnum.TT2:
                    // No area info accepted if organization type is Municipality, TT1 or TT2 - except default value areaType => WholeCountry!
                    if (areaType.HasValue && areaType != AreaInformationTypeEnum.WholeCountry)
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
                    if (areaType != AreaInformationTypeEnum.AreaType)
                    {
                        modelState.AddModelError("AreaType", $"Value {AreaInformationTypeEnum.AreaType.GetOpenApiValue()} required when OrganizationType has value {Model.OrganizationType}.");
                    }

                    // Validate the area codes
                    ValidateAreas(modelState);

                    break;
                case null:
                    if (areaType.HasValue || !Model.SubAreaType.IsNullOrEmpty() || Model.Areas?.Count > 0)
                    {
                        modelState.AddModelError("OrganizationType", "OrganizationType field is required when AreaType, SubAreaType or Areas has values.");
                    }
                    break;
                default:
                    if (areaType != AreaInformationTypeEnum.AreaType && !Model.SubAreaType.IsNullOrEmpty())
                    {
                        modelState.AddModelError("SubAreaType", $"No SubAreaType accepted when AreaType has value {Model.AreaType}.");
                    }
                    if (areaType != AreaInformationTypeEnum.AreaType && Model.Areas?.Count > 0)
                    {
                        modelState.AddModelError("Areas", $"No Areas accepted when AreaType has value {Model.AreaType}.");
                    }
                    // For State, Organization and Company types we are validating the area codes
                    ValidateAreas(modelState);
                    break;
            }

            // validate sote organization - it is not allowed to change sote organization to other type
/* SOTE has been disabled (SFIPTV-1177)
            if (organizationType.HasValue) // SFIPTV-953
            {
                if (currentVersion != null &&
                    commonService.OrganizationIsSote(currentVersion.OrganizationType) &&
                    !commonService.OrganizationIsSote(organizationType.ToString()))
                {
                    modelState.AddModelError("OrganizationType", $"It is not allowed to change type of SOTE organization.");
                }
            }

            // Validate responsible organization - only allowed for Sote organizations
            if (!string.IsNullOrEmpty(Model.ResponsibleOrganizationId))
            {
                if (organizationType == null)
                {
                    modelState.AddModelError("OrganizationType", $"OrganizationType field is required when ResponsibleOrganizationId has value.");
                }
                else if (!commonService.OrganizationIsSote(organizationType.ToString()))
                {
                    modelState.AddModelError("ResponsibleOrganizationId", $"No ResponsibleOrganizationId accepted when OrganizationType has value {Model.OrganizationType}.");
                }
                else
                {
                    responsibleOrganizationId.Validate(modelState);
                }
            }
*/
        }

        /// <summary>
        /// Get the required property list names where languages do not exist.
        /// </summary>
        /// <returns></returns>
        protected override IList<string> GetPropertyListsWhereMissingLanguages()
        {
            if (RequiredLanguages?.Count == 0)
            {
                return null;
            }

            var list = new List<string>();

            if (IsLanguagesMissing(Model.OrganizationNames))
            {
                list.Add("OrganizationNames");
            }
            if (IsLanguagesMissing(Model.OrganizationDescriptions))
            {
                list.Add("OrganizationDescriptions");
            }
            return list;
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
