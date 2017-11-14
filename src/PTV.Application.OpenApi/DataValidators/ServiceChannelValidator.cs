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

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for organization.
    /// </summary>
    public class ServiceChannelValidator<TModel> : BaseValidator<TModel> where TModel : IVmOpenApiServiceChannelIn
    {
        private ICodeService codeService;

        private IList<string> requiredLanguages;
        private IList<string> availableLanguages;
        private string currentPublishingStatus;

        // Validators
        private ServiceHourListValidator<V4VmOpenApiServiceHour> hours;
        private OrganizationIdValidator organizationId;
        private LanguageListValidator languages;
        private PhoneNumberListValidator<V4VmOpenApiPhone> phones;
        private AreaAndTypeValidator areas;
        private ServiceIdListValidator services;

        /// <summary>
        /// Required languages for lists.
        /// </summary>
        public IList<string> RequiredLanguages { get { return requiredLanguages; } set { requiredLanguages = value; } }

        /// <summary>
        /// All availbae languages for service channel.
        /// </summary>
        public IList<string> AvailableLanguages { get { return availableLanguages; } set { availableLanguages = value; } }

        /// <summary>
        /// Current model publishing status
        /// </summary>
        public string CurrentPublishingStatus { get { return currentPublishingStatus; } set { currentPublishingStatus = value; } }

        /// <summary>
        /// Ctor - channel validator
        /// </summary>
        /// <param name="model"></param>
        /// <param name="propertyName">Property name</param>
        /// <param name="commonService">Common service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="serviceService">Service service</param>
        public ServiceChannelValidator(TModel model, string propertyName, ICommonService commonService, ICodeService codeService, IServiceService serviceService)
            : base(model, propertyName)
        {
            if (model == null)
            {
                throw new ArgumentNullException(PropertyName, $"{PropertyName} must be defined.");
            }

            this.codeService = codeService;
            requiredLanguages = new List<string>();
            availableLanguages = new List<string>();

            hours = new ServiceHourListValidator<V4VmOpenApiServiceHour>(model.ServiceHours);
            organizationId = new OrganizationIdValidator(model.OrganizationId, commonService);
            languages = new LanguageListValidator(model.Languages, codeService);
            phones = new PhoneNumberListValidator<V4VmOpenApiPhone>(model.SupportPhones, codeService); // Support phone property is not required so no need to check language items (required/available languages)
            areas = new AreaAndTypeValidator(model.Areas, model.AreaType, codeService);
            services = new ServiceIdListValidator(model.ServiceChannelServices.ToList(), serviceService, "Services");
        }

        /// <summary>
        /// Checks if electronic channel model is valid or not.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            // Validate names against newly added languages (required languages)
            var names = new LanguageItemListValidator(Model.ServiceChannelNames, "ServiceChannelNames", requiredLanguages);
            names.Validate(modelState);
            // Validate descriptions against newly added languages (required languages), all the available languages does not need to be validated.
            var descriptions = new LocalizedListValidator(Model.ServiceChannelDescriptions, "ServiceChannelDescriptions", requiredLanguages,
                new List<string>() { DescriptionTypeEnum.ShortDescription.ToString(), DescriptionTypeEnum.Description.ToString() });
            descriptions.Validate(modelState);

            hours.Validate(modelState);
            organizationId.Validate(modelState);
            languages.Validate(modelState);
            phones.Validate(modelState);

            // Validate publishing status
            var status = new PublishingStatusValidator(Model.PublishingStatus, currentPublishingStatus);
            status.Validate(modelState);

            areas.Validate(modelState);
            services.Validate(modelState);
        }
    }
}
