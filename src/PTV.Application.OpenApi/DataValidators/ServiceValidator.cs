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
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service.
    /// </summary>
    public class ServiceValidator : BaseValidator<IVmOpenApiServiceInVersionBase>
    {
        private IGeneralDescriptionService generalDescriptionService;

        private LocalizedListValidator names;
        private LocalizedListValidator descriptions;
        private LanguageListValidator languages;
        private ServiceClassListValidator serviceClasses;
        private OntologyTermListValidator ontologyTerms;
        private TargetGroupListValidator targetGroups;
        private LifeEventListValidator lifeEvents;
        private IndustrialClassListValidator industrialClasses;
        private OrganizationIdListValidator organizations;
        private PublishingStatusValidator status;
        private AreaAndTypeValidator areas;
        private ServiceChannelIdListValidator channels;
        private ServiceProducerListValidator serviceProducers;
        private OrganizationIdValidator mainOrganization;

        private bool generalDescriptionAttached;
        private IList<string> newLanguages;
        private List<IVmOpenApiFintoItemVersionBase> currentTargetGroups;

        /// <summary>
        /// Ctor - service validator
        /// </summary>
        /// <param name="model">Service model</param>
        /// <param name="generalDescriptionService">General description service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="fintoService">Finto item service</param>
        /// <param name="commonService">Common service</param>
        /// <param name="channelService">Channel service</param>
        /// <param name="newLanguages">Languages that should be validated within lists</param>
        /// <param name="userRole">User role</param>
        /// <param name="currentTargetGroups">Target groups that are attached for latest version of service</param>
        public ServiceValidator(
            IVmOpenApiServiceInVersionBase model,
            IGeneralDescriptionService generalDescriptionService,
            ICodeService codeService,
            IFintoService fintoService,
            ICommonService commonService,
            IChannelService channelService,
            IList<string> newLanguages,
            UserRoleEnum userRole,
            List<IVmOpenApiFintoItemVersionBase> currentTargetGroups = null
            ) : base(model, "Service")
        {
            this.generalDescriptionService = generalDescriptionService;

            if (model == null)
            {
                throw new ArgumentNullException(PropertyName, $"{PropertyName} must be defined.");
            }

            names = new LocalizedListValidator(model.ServiceNames, "ServiceNames", newLanguages, new List<string>() { NameTypeEnum.Name.ToString() });
            languages = new LanguageListValidator(model.Languages, codeService);
            serviceClasses = new ServiceClassListValidator(model.ServiceClasses, fintoService);
            ontologyTerms = new OntologyTermListValidator(model.OntologyTerms, fintoService);
            targetGroups = new TargetGroupListValidator(model.TargetGroups, fintoService);
            lifeEvents = new LifeEventListValidator(model.LifeEvents, fintoService);
            industrialClasses = new IndustrialClassListValidator(model.IndustrialClasses, fintoService);
            organizations = new OrganizationIdListValidator(model.OtherResponsibleOrganizations?.Select(o => o.ToString()).ToList(), commonService, "OtherResponsibleOrganizations");
            status = new PublishingStatusValidator(model.PublishingStatus, model.CurrentPublishingStatus);
            areas = new AreaAndTypeValidator(model.Areas, model.AreaType, codeService);
            var channelList = model.ServiceServiceChannels?.Count > 0 ? model.ServiceServiceChannels.Select(i => i.ChannelGuid).ToList() : new List<Guid>();
            channels = new ServiceChannelIdListValidator(channelList, channelService, userRole, "ServiceChannels");

            var availableOrganizations = new List<Guid>();
            if (!model.MainResponsibleOrganization.IsNullOrEmpty()) { availableOrganizations.Add(model.MainResponsibleOrganization.ParseToGuidWithExeption()); }
            if (model.OtherResponsibleOrganizations?.Count > 0) { availableOrganizations.AddRange(model.OtherResponsibleOrganizations); }
            serviceProducers = new ServiceProducerListValidator(model.ServiceProducers, availableOrganizations, commonService);

            mainOrganization = new OrganizationIdValidator(model.MainResponsibleOrganization, commonService, "MainResponsibleOrganization");

            generalDescriptionAttached = !string.IsNullOrEmpty(model.GeneralDescriptionId);
            this.newLanguages = newLanguages;
            this.currentTargetGroups = currentTargetGroups;
        }


        /// <summary>
        /// Checks if service model is valid or not.
        /// </summary>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (!generalDescriptionAttached)
            {
                // validate names if general description is not set (name is taken from general description if not set).
                names.Validate(modelState);
                // Validate all required descriptions
                descriptions = new LocalizedListValidator(Model.ServiceDescriptions, "ServiceDescriptions", newLanguages,
                    new List<string>() { DescriptionTypeEnum.ShortDescription.ToString(), DescriptionTypeEnum.Description.ToString() });
            }
            else
            {
                // General description was defined.
                var gdId = Model.GeneralDescriptionId.ParseToGuid();
                if (gdId.HasValue)
                {
                    var gd = generalDescriptionService.GetGeneralDescriptionVersionBase(gdId.Value, 0);
                    if (gd == null || !gd.Id.HasValue)
                    {
                        modelState.AddModelError("StatutoryServiceGeneralDescriptionId", CoreMessages.OpenApi.RecordNotFound);
                    }
                    else
                    {
                        if (gd.Descriptions == null || gd.Descriptions.Where(d => d.Type == DescriptionTypeEnum.Description.ToString() && d.Value != null && d.Value.Length > 0).FirstOrDefault() == null)
                        {
                            // Description was not defined within GD - service model need to include all required description, so let's validate both ShortDescription and Description
                            descriptions = new LocalizedListValidator(Model.ServiceDescriptions, "ServiceDescriptions", newLanguages,
                                new List<string>() { DescriptionTypeEnum.ShortDescription.ToString(), DescriptionTypeEnum.Description.ToString() });
                        }
                        else
                        {
                            // Validate only ShortDescription for Descriptions. Description is not required if it is defined within GD.
                            descriptions = new LocalizedListValidator(Model.ServiceDescriptions, "ServiceDescriptions", newLanguages,
                                new List<string>() { DescriptionTypeEnum.ShortDescription.ToString() });
                        }
                    }
                }
                else
                {
                    modelState.AddModelError("StatutoryServiceGeneralDescriptionId", CoreMessages.OpenApi.RecordNotFound);
                }                
            }

            if (descriptions != null) {  descriptions.Validate(modelState); }  
            languages.Validate(modelState);
            serviceClasses.Validate(modelState);
            ontologyTerms.Validate(modelState);
            targetGroups.Validate(modelState);
            var tgList = Model.TargetGroups?.Count > 0 ? targetGroups.TargetGroups : currentTargetGroups;
            // Check life events - can be attached only if target group 'Citizens' is attached (PTV-3184)
            if (Model.LifeEvents?.Count > 0)
            {
                if (tgList?.Count > 0)
                {
                    if (!tgList.Any(i => i.Code.StartsWith("KR1")))
                    {
                        modelState.AddModelError($"LifeEvents", string.Format(CoreMessages.OpenApi.TargetGroupRequired, "Citizens (KR1)", "life events"));
                    }
                }
                else
                {
                    modelState.AddModelError($"LifeEvents", string.Format(CoreMessages.OpenApi.TargetGroupRequired, "Citizens (KR1)", "life events"));
                }
                lifeEvents.Validate(modelState);
            }
            // Check industrial classes - can be attached only if target group 'Businesses and non-government organizations' is attached (PTV-3184)
            if (Model.IndustrialClasses?.Count > 0)
            {
                if (tgList?.Count > 0)
                {
                    if (!tgList.Any(i => i.Code.StartsWith("KR2")))
                    {
                        modelState.AddModelError($"IndustrialClasses", string.Format(CoreMessages.OpenApi.TargetGroupRequired, "Businesses and non-government organizations (KR2)", "industrial classes"));
                    }
                }
                else
                {
                    modelState.AddModelError($"IndustrialClasses", string.Format(CoreMessages.OpenApi.TargetGroupRequired, "Businesses and non-government organizations (KR2)", "industrial classes"));
                }
                industrialClasses.Validate(modelState);
            }
            organizations.Validate(modelState);
            status.Validate(modelState);

            // Validate area type, service areas and municipalities
            areas.Validate(modelState);

            channels.Validate(modelState);
            serviceProducers.Validate(modelState);
            mainOrganization.Validate(modelState);
        }
    }
}
