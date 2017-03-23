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
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service.
    /// </summary>
    public class ServiceValidator : BaseValidator<IVmOpenApiServiceInVersionBase>
    {
        private LocalizedListValidator names;
        private LocalizedListValidator descriptions;
        private GeneralDescriptionIdValidator generalDesription;
        private MunicipalityCodeListValidator municipalities;
        private LanguageListValidator languages;
        private ServiceClassListValidator serviceClasses;
        private OntologyTermListValidator ontologyTerms;
        private TargetGroupListValidator targetGroups;
        private LifeEventListValidator lifeEvents;
        private IndustrialClassListValidator industrialClasses;
        private OrganizationIdListValidator organizations;
        private PublishingStatusValidator status;
        private bool generalDescriptionAttached;

        /// <summary>
        /// Ctor - service validator
        /// </summary>
        /// <param name="model">Service model</param>
        /// <param name="generalDescriptionService">General description service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="fintoService">Finto item service</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="newLanguages">Languages that should be validated within lists</param>
        public ServiceValidator(IVmOpenApiServiceInVersionBase model, IGeneralDescriptionService generalDescriptionService, ICodeService codeService, IFintoService fintoService,
            IOrganizationService organizationService, IList<string> newLanguages) : base(model, "Service")
        {
            names = new LocalizedListValidator(model.ServiceNames, "ServiceNames", newLanguages, new List<string>() { NameTypeEnum.Name.ToString() });
            descriptions = new LocalizedListValidator(model.ServiceDescriptions, "ServiceDescriptions", newLanguages,
                new List<string>() { DescriptionTypeEnum.ShortDescription.ToString(), DescriptionTypeEnum.Description.ToString() });
            generalDesription = new GeneralDescriptionIdValidator(model.StatutoryServiceGeneralDescriptionId, generalDescriptionService);
            municipalities = new MunicipalityCodeListValidator(model.Municipalities, codeService);
            languages = new LanguageListValidator(model.Languages, codeService);
            serviceClasses = new ServiceClassListValidator(model.ServiceClasses, fintoService);
            ontologyTerms = new OntologyTermListValidator(model.OntologyTerms, fintoService);
            targetGroups = new TargetGroupListValidator(model.TargetGroups, fintoService);
            lifeEvents = new LifeEventListValidator(model.LifeEvents, fintoService);
            industrialClasses = new IndustrialClassListValidator(model.IndustrialClasses, fintoService);
            organizations = new OrganizationIdListValidator(model.ServiceOrganizations != null ? model.ServiceOrganizations.Select(o => o.OrganizationId).ToList() : null,
                organizationService, "ServiceOrganizations");
            status = new PublishingStatusValidator(model.PublishingStatus, model.CurrentPublishingStatus);
            generalDescriptionAttached = !string.IsNullOrEmpty(model.StatutoryServiceGeneralDescriptionId);
        }


        /// <summary>
        /// Checks if service model is valid or not.
        /// </summary>
        public override void Validate(ModelStateDictionary modelState)
        {
            // Only validate names if general description is not set (name is taken from general description if not set).
            if (!generalDescriptionAttached)
            {
                names.Validate(modelState);
            }
            descriptions.Validate(modelState);
            generalDesription.Validate(modelState);
            municipalities.Validate(modelState);
            languages.Validate(modelState);
            serviceClasses.Validate(modelState);
            ontologyTerms.Validate(modelState);
            targetGroups.Validate(modelState);
            lifeEvents.Validate(modelState);
            industrialClasses.Validate(modelState);
            organizations.Validate(modelState);
            status.Validate(modelState);
        }
    }
}
