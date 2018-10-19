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
using System;
using PTV.Framework;
using System.Linq;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for organization.
    /// </summary>
    public class ServiceChannelValidator<TModel> : BaseValidator<TModel> where TModel : IVmOpenApiServiceChannelIn
    {
        private ICodeService codeService;
        private IOrganizationService organizationService;

        // Validators
        private ServiceHourListValidator<V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime> hours;
        private OwnOrganizationValidator organizationId;
        private LanguageListValidator languages;
        private PhoneNumberListValidator<V4VmOpenApiPhone> phones;
        private AreaAndTypeValidator areas;
        private ServiceIdListValidator services;

        private int openApiVersion;

        /// <summary>
        /// Required languages for lists.
        /// </summary>
        public IList<string> RequiredLanguages { get; set; }

        /// <summary>
        /// All available languages for service channel.
        /// </summary>
        public IList<string> AvailableLanguages { get; set; }

        /// <summary>
        /// The latest version of service channel
        /// </summary>
        public IVmOpenApiServiceChannel CurrentVersion { get; set; }

        /// <summary>
        /// Ctor - channel validator
        /// </summary>
        /// <param name="model"></param>
        /// <param name="propertyName">Property name</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="serviceService">Service service</param>
        /// <param name="openApiVersion">The open api version</param>
        public ServiceChannelValidator(
            TModel model,
            string propertyName,
            IOrganizationService organizationService,
            ICodeService codeService,
            IServiceService serviceService,
            int openApiVersion)
            : base(model, propertyName)
        {
            if (model == null)
            {
                throw new ArgumentNullException(PropertyName, $"{PropertyName} must be defined.");
            }

            this.codeService = codeService;
            this.organizationService = organizationService;
            RequiredLanguages = new List<string>();
            AvailableLanguages = new List<string>();

            hours = new ServiceHourListValidator<V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(model.ServiceHours);
            languages = new LanguageListValidator(model.Languages, codeService);
            phones = new PhoneNumberListValidator<V4VmOpenApiPhone>(model.SupportPhones, codeService); // Support phone property is not required so no need to check language items (required/available languages)
            areas = new AreaAndTypeValidator(model.Areas, model.AreaType, codeService);
            services = new ServiceIdListValidator(model.ServiceChannelServices.ToList(), serviceService, "Services");

            this.openApiVersion = openApiVersion;
        }

        /// <summary>
        /// Checks if service channel model is valid or not.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            // Validate names against newly added languages (required languages) 
            var names = new LocalizedListValidator(Model.ServiceChannelNamesWithType, "ServiceChannelNames", RequiredLanguages);
            names.Validate(modelState);
            // Validate descriptions against newly added languages (required languages), all the available languages does not need to be validated.
            var descriptions = new LocalizedListValidator(Model.ServiceChannelDescriptions, "ServiceChannelDescriptions", RequiredLanguages,
                new List<string>() { DescriptionTypeEnum.ShortDescription.ToString(), DescriptionTypeEnum.Description.ToString() });
            descriptions.Validate(modelState);

            hours.Validate(modelState);
            if (!Model.OrganizationId.IsNullOrEmpty())
            {
                // Check the organization available languages only if user is publishing the item (SFIPTV-191)
                // And only for version 9 and up
                if (Model.PublishingStatus == PublishingStatus.Published.ToString() && openApiVersion > 8)
                {
                    organizationId = new OwnOrganizationValidator(Model.OrganizationId, organizationService, AvailableLanguages.ToList());
                }
                else
                {
                    organizationId = new OwnOrganizationValidator(Model.OrganizationId, organizationService);
                }
                organizationId.Validate(modelState);
            }
            // Checked only if user is publishing the item. And only for version 9 and up (SFIPTV-191)
            else if (CurrentVersion != null && AvailableLanguages?.Count > 0 && Model.PublishingStatus == PublishingStatus.Published.ToString() && openApiVersion > 8)
            {
                // Get the main organization from current version - needed for checking that the related organization has all newly added languages as available languages
                var currentOrganization = CurrentVersion.OrganizationId;
                if (currentOrganization != null)
                {
                    organizationId = new OwnOrganizationValidator(currentOrganization.ToString(), organizationService, AvailableLanguages.ToList());
                    organizationId.Validate(modelState);
                }
                else
                {
                    // There needs to be a main responsible organization attached. Rise a validation error.
                    modelState.AddModelError("OrganizationId", "Required property missing!");
                }
            }
            languages.Validate(modelState);
            phones.Validate(modelState);

            // Validate publishing status
            var status = new PublishingStatusValidator(Model.PublishingStatus, CurrentVersion != null ? CurrentVersion.PublishingStatus : null);
            status.Validate(modelState);

            areas.Validate(modelState);
            services.Validate(modelState);

            // Validate summary and name - they cannot be the same (SFIPTV-39)
            // Checking done only if user is publishing the item and for version 9 and up.
            if (Model.PublishingStatus == PublishingStatus.Published.ToString() && openApiVersion > 8)
            {
                if (CurrentVersion != null)
                {
                    var channelNames = Model.ServiceChannelNamesWithType;
                    var channelDescriptions = Model.ServiceChannelDescriptions;
                    if (channelNames == null || channelNames.Count == 0)
                    {
                        channelNames = CurrentVersion.ServiceChannelNames;
                    }
                    if (channelDescriptions == null || channelDescriptions.Count == 0)
                    {
                        channelDescriptions = CurrentVersion.ServiceChannelDescriptions;
                    }
                    channelNames?.Where(n => n.Type == NameTypeEnum.Name.ToString()).ForEach(name =>
                    {
                        var shortDescription = channelDescriptions?.FirstOrDefault(d => 
                            (d.Type == DescriptionTypeEnum.ShortDescription.ToString() || d.Type == DescriptionTypeEnum.ShortDescription.GetOpenApiValue())
                            && d.Language == name.Language);
                        if (shortDescription != null && name.Value.Equals(shortDescription.Value))
                        {
                            modelState.AddModelError("ServiceChannelNames", string.Format(CoreMessages.OpenApi.SameNameAndSummaryNotAllowed, name.Language));
                        }
                    });
                }
                else
                {
                    // Let's go through all the language versioned names.
                    Model.ServiceChannelNamesWithType?.Where(n => n.Type == NameTypeEnum.Name.ToString()).ForEach(name =>
                    {
                        var shortDescription = Model.ServiceChannelDescriptions.FirstOrDefault(d => d.Type == DescriptionTypeEnum.ShortDescription.ToString() && d.Language == name.Language);
                        if (shortDescription != null && name.Value.Equals(shortDescription.Value))
                        {
                            modelState.AddModelError("ServiceChannelNames", string.Format(CoreMessages.OpenApi.SameNameAndSummaryNotAllowed, name.Language));
                        }
                    });
                }
            }

            // Check that ValidTo is greater than ValidFrom (SFIPTV-190)
            if (Model.ValidFrom.HasValue && Model.ValidTo.HasValue && Model.ValidFrom.Value.Date >= Model.ValidTo.Value.Date)
            {
                modelState.AddModelError("ValidTo", $"Archiving date must be greater than publishing date.");
            }
        }

        /// <summary>
        /// Validate WCAG level according to selected accessibility classification level. (SFIPTV-37)
        /// </summary>
        /// <param name="accessibilityClassificationLevel"></param>
        /// <param name="wcagLevel"></param>
        /// <param name="modelState"></param>
        protected void ValidatedAccessibilityClassification(
            string accessibilityClassificationLevel,
            string wcagLevel,
            ModelStateDictionary modelState)
        {
            // Allowed combinations:
            // If accessibilityClassificationLevel = FullyCompliant, wcagLevel can be AAA or AA.
            // If accessibilityClassificationLevel = PartiallyCompliant, wcagLevel can be A.
            // If accessibilityClassificationLevel = NonCompliant, wcagLevel needs to be empty or null.
            // If accessibilityClassificationLevel = Unknown, wcagLevel needs to be empty or null.
            if (!string.IsNullOrEmpty(accessibilityClassificationLevel))
            {
                if (accessibilityClassificationLevel == AccessibilityClassificationLevelTypeEnum.FullyCompliant.ToString() &&
                    (string.IsNullOrEmpty(wcagLevel) || wcagLevel == WcagLevelTypeEnum.LevelA.ToString())
                    ||
                    (accessibilityClassificationLevel == AccessibilityClassificationLevelTypeEnum.PartiallyCompliant.ToString() && wcagLevel != WcagLevelTypeEnum.LevelA.ToString())
                    ||
                    (accessibilityClassificationLevel == AccessibilityClassificationLevelTypeEnum.NonCompliant.ToString() && !string.IsNullOrEmpty(wcagLevel))
                    ||
                    (accessibilityClassificationLevel == AccessibilityClassificationLevelTypeEnum.Unknown.ToString() && !string.IsNullOrEmpty(wcagLevel)))
                {
                    modelState.AddModelError("WcagLevel", $"'{wcagLevel}' is not allowed value of 'WcagLevel' when 'AccessibilityClassificationLevel' has value '{ accessibilityClassificationLevel }'.");
                }
            }
        }
    }
}
