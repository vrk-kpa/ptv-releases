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
    public class ServiceChannelValidator<TModel> : TimedPublishingBaseValidator<TModel> where TModel : IVmOpenApiServiceChannelIn
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
        /// Available languages
        /// </summary>
        protected IList<string> AvailableLanguages;
        /// <summary>
        /// Required languages
        /// </summary>
        protected IList<string> RequiredLanguages;
        /// <summary>
        /// The latest version of service channel.
        /// </summary>
        protected IVmOpenApiServiceChannel CurrentVersion;

        /// <summary>
        /// Ctor - channel validator
        /// </summary>
        /// <param name="model"></param>
        /// <param name="propertyName">Property name</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="serviceService">Service service</param>
        /// <param name="availableLanguages">Available languages</param>
        /// <param name="requiredLanguages">Required languages</param>
        /// <param name="currentVersion">The current version of service channel</param>
        /// <param name="openApiVersion">The open api version</param>
        public ServiceChannelValidator(
            TModel model,
            string propertyName,
            IOrganizationService organizationService,
            ICodeService codeService,
            IServiceService serviceService,
            IList<string> availableLanguages,
            IList<string> requiredLanguages,
            IVmOpenApiServiceChannel currentVersion,
            int openApiVersion)
            : base(model, propertyName)
        {
            if (model == null)
            {
                throw new ArgumentNullException(PropertyName, $"{PropertyName} must be defined.");
            }

            this.codeService = codeService;
            this.organizationService = organizationService;

            hours = new ServiceHourListValidator<V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(model.ServiceHours);
            languages = new LanguageListValidator(model.Languages, codeService);
            phones = new PhoneNumberListValidator<V4VmOpenApiPhone>(model.SupportPhones, codeService); // Support phone property is not required so no need to check language items (required/available languages)
            areas = new AreaAndTypeValidator(model.Areas, model.AreaType, codeService);
            services = new ServiceIdListValidator(model.ServiceChannelServices.ToList(), serviceService, "Services", true);

            this.AvailableLanguages = availableLanguages;
            this.RequiredLanguages = requiredLanguages;
            this.CurrentVersion = currentVersion;
            this.openApiVersion = openApiVersion;
        }

        /// <summary>
        /// Checks if service channel model is valid or not.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            base.Validate(modelState);

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
        }
    }
}
