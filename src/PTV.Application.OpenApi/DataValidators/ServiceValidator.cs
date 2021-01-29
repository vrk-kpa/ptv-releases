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
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service.
    /// </summary>
    public class ServiceValidator : TimedPublishingBaseValidator<IVmOpenApiServiceInVersionBase>
    {
        private readonly IGeneralDescriptionService generalDescriptionService;
        private readonly IOrganizationService organizationService;

        private LocalizedListValidator names;
        private LocalizedListValidator descriptions;
        private readonly LanguageListValidator languages;
        private readonly ServiceClassListValidator serviceClasses;
        private readonly OntologyTermListValidator ontologyTerms;
        private readonly TargetGroupListValidator targetGroups;
        private readonly LifeEventListValidator lifeEvents;
        private readonly IndustrialClassListValidator industrialClasses;
        private readonly OrganizationIdListValidator organizations;
        private readonly PublishingStatusValidator status;
        private readonly AreaAndTypeValidator areas;
        private readonly ServiceChannelIdListValidator channels;
        private readonly ServiceProducerListValidator serviceProducers;
        private readonly ServiceNamesDuplicityValidator serviceNamesDuplicity;
        private readonly FundingValidator funding;
        private OwnOrganizationValidator mainOrganization;

        private readonly bool generalDescriptionAttached;
        private readonly int openApiVersion;
        private readonly IVmOpenApiServiceVersionBase currentVersion;

        /// <summary>
        /// Ctor - service validator
        /// </summary>
        /// <param name="model">Service model</param>
        /// <param name="generalDescriptionService">General description service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="fintoService">Finto item service</param>
        /// <param name="ontologyTermDataCache">ontologyTermDataCache</param>
        /// <param name="commonService">Common service</param>
        /// <param name="channelService">Channel service</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="userRole">User role</param>
        /// <param name="openApiVersion">The open api version</param>
        /// <param name="currentVersion">The latest version of service</param>
        public ServiceValidator(
            IVmOpenApiServiceInVersionBase model,
            IGeneralDescriptionService generalDescriptionService,
            ICodeService codeService,
            IFintoService fintoService,
            IOntologyTermDataCache ontologyTermDataCache,
            ICommonService commonService,
            IChannelService channelService,
            IOrganizationService organizationService,
            UserRoleEnum userRole,
            int openApiVersion,
            IVmOpenApiServiceVersionBase currentVersion = null
            ) : base(model, currentVersion?.AvailableLanguages, "Service")
        {
            this.generalDescriptionService = generalDescriptionService;
            this.organizationService = organizationService;

            funding = new FundingValidator(model.FundingType, model.MainResponsibleOrganization.ParseToGuid() ?? Guid.Empty, codeService);
            serviceNamesDuplicity = new ServiceNamesDuplicityValidator(model.ServiceNames, commonService, model.MainResponsibleOrganization.ParseToGuid(), model.Id);
            languages = new LanguageListValidator(model.Languages, codeService);
            // Number of ontology terms/service classes is validated only for version 9 and up. (PTV-4395)
            if (openApiVersion > 8)
            {
                serviceClasses = new ServiceClassListValidator(model.ServiceClasses, fintoService, validCount: 4, checkMainClasses: true);
                ontologyTerms = new OntologyTermListValidator(model.OntologyTerms, ontologyTermDataCache, validCount: 10);
            }
            else
            {
                serviceClasses = new ServiceClassListValidator(model.ServiceClasses, fintoService);
                ontologyTerms = new OntologyTermListValidator(model.OntologyTerms, ontologyTermDataCache);
            }
            targetGroups = new TargetGroupListValidator(model.TargetGroups, fintoService);
            lifeEvents = new LifeEventListValidator(model.LifeEvents, fintoService);
            industrialClasses = new IndustrialClassListValidator(model.IndustrialClasses, fintoService);
            organizations = new OrganizationIdListValidator(model.OtherResponsibleOrganizations?.Select(o => o.ToString()).ToList(), commonService, model.MainResponsibleOrganization, "OtherResponsibleOrganizations");
            status = new PublishingStatusValidator(model.PublishingStatus, model.CurrentPublishingStatus);
            areas = new AreaAndTypeValidator(model.Areas, model.AreaType, codeService, commonService:commonService, organizationId:model.MainResponsibleOrganization.ParseToGuid());
            var channelList = model.ServiceServiceChannels?.Count > 0 ? model.ServiceServiceChannels.Select(i => i.ChannelGuid).ToList() : new List<Guid>();
            channels = new ServiceChannelIdListValidator(channelList, channelService, userRole, "ServiceChannels");

            var availableOrganizations = new List<Guid>();
            if (!model.MainResponsibleOrganization.IsNullOrEmpty()) { availableOrganizations.Add(model.MainResponsibleOrganization.ParseToGuidWithExeption()); }
            if (model.OtherResponsibleOrganizations?.Count > 0) { availableOrganizations.AddRange(model.OtherResponsibleOrganizations); }
            serviceProducers = new ServiceProducerListValidator(model.ServiceProducers, availableOrganizations, commonService);

            generalDescriptionAttached = !string.IsNullOrEmpty(model.GeneralDescriptionId);
            this.openApiVersion = openApiVersion;
            this.currentVersion = currentVersion;
        }


        /// <summary>
        /// Checks if service model is valid or not.
        /// </summary>
        public override void Validate(ModelStateDictionary modelState)
        {
            base.Validate(modelState);

            var allTgList = new List<IVmOpenApiFintoItemVersionBase>();
            if (!generalDescriptionAttached)
            {
                // validate names if general description is not set (name is taken from general description if not set).
                names = new LocalizedListValidator(Model.ServiceNames, "ServiceNames", RequiredLanguages, new List<string> { NameTypeEnum.Name.ToString() }, Model.AvailableLanguages);
                names.Validate(modelState);
                // Validate all required descriptions
                // Descriptions need to be checked also for available languages (SFIPTV-847).
                descriptions = new LocalizedListValidator(Model.ServiceDescriptions, "ServiceDescriptions", RequiredLanguages,
                    new List<string> { DescriptionTypeEnum.ShortDescription.GetOpenApiValue(), DescriptionTypeEnum.Description.GetOpenApiValue() }, Model.AvailableLanguages);

                // Check if current version has gd attached and user is modifying ontologyterms or service classes
                if (currentVersion != null && currentVersion.GeneralDescriptionId.IsAssigned() && !Model.DeleteGeneralDescriptionId && (Model.ServiceClasses?.Count > 0 || Model?.OntologyTerms?.Count > 0))
                {
                    /* SFIPTV-2048 - postpone functionality of SFIPTV-1990
                     var gd = generalDescriptionService.GetGeneralDescriptionVersionBase(currentVersion.GeneralDescriptionId.Value, 0, true, false);
                     // Set the amount of service classes and ontology terms within GD.
                     serviceClasses.GeneralDescriptionServiceClassCount = gd?.ServiceClasses != null ? gd.ServiceClasses.Count : 0;
                     ontologyTerms.GeneralDescriptionOntologyTermCount = gd?.OntologyTerms != null ? gd.OntologyTerms.Count : 0;
                    */
                    serviceClasses.GeneralDescriptionServiceClassCount = 0;
                    ontologyTerms.GeneralDescriptionOntologyTermCount = 0;
                }
            }
            else
            {
                // General description was defined.
                var gdId = Model.GeneralDescriptionId.ParseToGuid();
                if (gdId.HasValue)
                {
                    // Check that user has rights for GD.
                    var gd = generalDescriptionService.GetGeneralDescriptionVersionBase(gdId.Value, 0, true, true);
                    if (gd?.Id == null)
                    {
                        modelState.AddModelError("GeneralDescriptionId", CoreMessages.OpenApi.RecordNotFound);
                    }
                    else
                    {
                        if (gd.Descriptions?.Where(d => d.Type == DescriptionTypeEnum.Description.ToString() && !string.IsNullOrEmpty(d.Value)).FirstOrDefault() == null)
                        {
                            // Description was not defined within GD - service model need to include all required description, so let's validate both ShortDescription and Description
                            // Descriptions need to be checked also for available languages (SFIPTV-847).
                            descriptions = new LocalizedListValidator(Model.ServiceDescriptions, "ServiceDescriptions", RequiredLanguages,
                                new List<string> { DescriptionTypeEnum.ShortDescription.GetOpenApiValue(), DescriptionTypeEnum.Description.GetOpenApiValue() }, Model.AvailableLanguages);
                        }
                        else
                        {
                            // Validate only ShortDescription for Descriptions. Description is not required if it is defined within GD.
                            // Descriptions need to be checked also for available languages (SFIPTV-847).
                            descriptions = new LocalizedListValidator(Model.ServiceDescriptions, "ServiceDescriptions", RequiredLanguages,
                                new List<string> { DescriptionTypeEnum.ShortDescription.GetOpenApiValue() }, Model.AvailableLanguages);
                        }
                        allTgList.AddRange(gd.TargetGroups);
                        // Set the amount of service classes and ontology terms within GD.
                        /*
                         SFIPTV-2048 - postpone functionality of SFIPTV-1990
                         serviceClasses.GeneralDescriptionServiceClassCount = gd.ServiceClasses != null ? gd.ServiceClasses.Count : 0;
                         ontologyTerms.GeneralDescriptionOntologyTermCount = gd.OntologyTerms != null ? gd.OntologyTerms.Count : 0;
                        */
                        serviceClasses.GeneralDescriptionServiceClassCount = 0;
                        ontologyTerms.GeneralDescriptionOntologyTermCount = 0;
                    }

                }
                else
                {
                    modelState.AddModelError("GeneralDescriptionId", CoreMessages.OpenApi.RecordNotFound);
                }
            }
            serviceNamesDuplicity.Validate(modelState);
            funding.Validate(modelState);
            descriptions?.Validate(modelState);
            languages.Validate(modelState);
            serviceClasses.Validate(modelState);
            ontologyTerms.Validate(modelState);
            targetGroups.Validate(modelState);
            var tgList = new List<IVmOpenApiFintoItemVersionBase>();
            if (Model.TargetGroups?.Count > 0)
            {
                tgList = targetGroups.TargetGroups.ToList();
                allTgList.AddRange(tgList);
            }
            else if (currentVersion?.TargetGroups?.Count > 0)
            {
                currentVersion.TargetGroups.ForEach(i => tgList.Add(i));
            }

            if (allTgList.Any())
            {
                var parents = allTgList.Where(x => x.Code== "KR2").Select(x => x.Id);
                var subParents = allTgList.Where(x => x.ParentId.HasValue).Select(x => x.ParentId).Distinct();
                if (parents.Any(pId => !subParents.Contains(pId)))
                {
                    modelState.AddModelError("TargetGroups", CoreMessages.OpenApi.SubTargetGroupRequired);
                }
            }

            // Check life events - can be attached only if target group 'Citizens' is attached (PTV-3184)
            if (Model.LifeEvents?.Count > 0)
            {
                if (tgList.Count > 0)
                {
                    if (!tgList.Any(i => i.Code.StartsWith("KR1")))
                    {
                        modelState.AddModelError("LifeEvents", string.Format(CoreMessages.OpenApi.TargetGroupRequired, "Citizens (KR1)", "life events"));
                    }
                }
                else
                {
                    modelState.AddModelError("LifeEvents", string.Format(CoreMessages.OpenApi.TargetGroupRequired, "Citizens (KR1)", "life events"));
                }
                lifeEvents.Validate(modelState);
            }
            // Check industrial classes - can be attached only if target group 'Businesses and non-government organizations' is attached (PTV-3184)
            if (Model.IndustrialClasses?.Count > 0)
            {
                if (tgList.Count > 0)
                {
                    if (!tgList.Any(i => i.Code.StartsWith("KR2")))
                    {
                        modelState.AddModelError("IndustrialClasses", string.Format(CoreMessages.OpenApi.TargetGroupRequired, "Businesses and non-government organizations (KR2)", "industrial classes"));
                    }
                }
                else
                {
                    modelState.AddModelError("IndustrialClasses", string.Format(CoreMessages.OpenApi.TargetGroupRequired, "Businesses and non-government organizations (KR2)", "industrial classes"));
                }
                industrialClasses.Validate(modelState);
            }
            organizations.Validate(modelState);
            status.Validate(modelState);

            // Validate area type, service areas and municipalities
            areas.Validate(modelState);

            channels.Validate(modelState);
            serviceProducers.Validate(modelState);
            if (!Model.MainResponsibleOrganization.IsNullOrEmpty())
            {
                // Check the organization available languages only if user is publishing the item (SFIPTV-191)
                // And only for version 9 and up
                if (Model.PublishingStatus == PublishingStatus.Published.ToString() && openApiVersion > 8)
                {
                    var newAvailableLanguages = new HashSet<string>();
                    Model.AvailableLanguages.ForEach(i => newAvailableLanguages.Add(i));
                    if (currentVersion != null && currentVersion.AvailableLanguages?.Count > 0)
                    {
                        currentVersion.AvailableLanguages.ForEach(i => newAvailableLanguages.Add(i));
                    }
                    mainOrganization = new OwnOrganizationValidator(Model.MainResponsibleOrganization, organizationService, newAvailableLanguages.ToList(), "MainResponsibleOrganization");
                }
                else
                {
                    mainOrganization = new OwnOrganizationValidator(Model.MainResponsibleOrganization, organizationService, propertyName: "MainResponsibleOrganization");
                }
                mainOrganization.Validate(modelState);
            }
            // Checked only if user is publishing the item. And only for version 9 and up (SFIPTV-191)
            else if(currentVersion != null /*&& newLanguages?.Count > 0*/ && Model.PublishingStatus == PublishingStatus.Published.ToString() && openApiVersion > 8)
            {
                // Get the main organization from current version - needed for checking that the related organization has all newly added languages as available languages
                var currentMainOrganization = currentVersion.Organizations.FirstOrDefault(o => o.RoleType == CommonConsts.RESPONSIBLE);
                if (currentMainOrganization?.Organization != null)
                {
                    mainOrganization = new OwnOrganizationValidator(currentMainOrganization.Organization.Id?.ToString(), organizationService, RequiredLanguages?.ToList(), "MainResponsibleOrganization");
                    mainOrganization.Validate(modelState);
                }
                else
                {
                    // There needs to be a main responsible organization attached. Rise a validation error.
                    modelState.AddModelError("MainResponsibleOrganization", "Required property missing!");
                }
            }

            // Validate summary and name - they cannot be the same (PTV-4395)
            // Update 4.9.2018 (SFIPTV-39): Checking is only done if user is publishing the item and only for version 9 and up.
            if (Model.PublishingStatus == PublishingStatus.Published.ToString() && openApiVersion > 8)
            {
                if (currentVersion != null)
                {
                    var serviceNames = Model.ServiceNames;
                    var newDescriptions = Model.ServiceDescriptions;
                    if (serviceNames == null || serviceNames.Count == 0)
                    {
                        serviceNames = currentVersion.ServiceNames;
                    }
                    if (newDescriptions == null || newDescriptions.Count == 0)
                    {
                        newDescriptions = currentVersion.ServiceDescriptions;
                    }
                    serviceNames?.Where(n => n.Type == NameTypeEnum.Name.ToString()).ForEach(name =>
                    {
                        var shortDescription = newDescriptions?.FirstOrDefault(d =>
                            (d.Type == DescriptionTypeEnum.ShortDescription.GetOpenApiValue() || d.Type == DescriptionTypeEnum.ShortDescription.GetOpenApiValue())
                            && d.Language == name.Language);
                        if (shortDescription != null && name.Value.Equals(shortDescription.Value))
                        {
                            modelState.AddModelError("ServiceNames", string.Format(CoreMessages.OpenApi.SameNameAndSummaryNotAllowed, name.Language));
                        }
                    });
                }
                else
                {
                    // Let's go through all the language versioned names.
                    Model.ServiceNames?.Where(n => n.Type == NameTypeEnum.Name.ToString()).ForEach(name =>
                    {
                        var shortDescription = Model.ServiceDescriptions?.FirstOrDefault(d => d.Type == DescriptionTypeEnum.ShortDescription.GetOpenApiValue() && d.Language == name.Language);
                        if (shortDescription != null && name.Value.Equals(shortDescription.Value))
                        {
                            modelState.AddModelError("ServiceNames", string.Format(CoreMessages.OpenApi.SameNameAndSummaryNotAllowed, name.Language));
                        }
                    });
                }
            }
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

            if (IsLanguagesMissing(Model.ServiceNames))
            {
                list.Add("ServiceNames");
            }
            if (IsLanguagesMissing(Model.ServiceDescriptions))
            {
                list.Add("ServiceDescriptions");
            }
            return list;
        }
    }
}
