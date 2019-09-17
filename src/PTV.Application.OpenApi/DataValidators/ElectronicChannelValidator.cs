﻿/**
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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Enums;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for electronic channel.
    /// </summary>
    public class ElectronicChannelValidator : ServiceChannelValidator<VmOpenApiElectronicChannelInVersionBase>
    {
        private LanguageItemListValidator urls;
        private AccessibilityClassificationListValidator accessibilityClassifications;

        /// <summary>
        /// Ctor - electronic channel validator
        /// </summary>
        /// <param name="model">Electronic channel model</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="serviceService">Service service</param>
        /// <param name="availableLanguages">Available languages</param>
        /// <param name="requiredLanguages">Required languages</param>
        /// <param name="currentVersion">Current version</param>
        /// <param name="openApiVersion">The open api version</param>
        public ElectronicChannelValidator(
            VmOpenApiElectronicChannelInVersionBase model,
            IOrganizationService organizationService,
            ICodeService codeService,
            IServiceService serviceService,
            IList<string> availableLanguages,
            IList<string> requiredLanguages,
            IVmOpenApiServiceChannel currentVersion,
            int openApiVersion)
            : base(model, "ElectronicChannel", organizationService, codeService, serviceService, availableLanguages, requiredLanguages, currentVersion, openApiVersion)
        {
            accessibilityClassifications = new AccessibilityClassificationListValidator(model.AccessibilityClassification, availableLanguages, openApiVersion);
        }

        /// <summary>
        /// Validates electronic channel model.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            base.Validate(modelState);
            var isPublishing = Model.PublishingStatus == PublishingStatus.Published.ToString();
            
            // If the model is going to be published, there need to be webpages for all languages which already exist 
            // for an eChannel or are included in the current request (AvailableLanguages). If this is just a Draft 
            // update which will not yet be published, then only the languages of the current request are checked
            // (RequiredLanguages).
            var languagesToCheck = isPublishing ? AvailableLanguages : RequiredLanguages;
            urls = new LanguageItemListValidator(Model.WebPage, "WebPage", languagesToCheck);
            urls.Validate(modelState);

            // Validate WCAG level according to selected accessibility classification level (SFIPTV-37)
            accessibilityClassifications.Validate(modelState);
        }
    }
}
