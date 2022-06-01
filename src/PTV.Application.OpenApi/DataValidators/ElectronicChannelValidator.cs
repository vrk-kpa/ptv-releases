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
        private static readonly string ChannelType = ServiceChannelTypeEnum.EChannel.ToString();
        private LanguageItemListValidator urls;
        private AccessibilityClassificationListValidator accessibilityClassifications;

        /// <summary>
        /// Ctor - electronic channel validator
        /// </summary>
        /// <param name="model">Electronic channel model</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="serviceService">Service service</param>
        /// <param name="commonService"></param>
        /// <param name="currentVersion">Current version</param>
        /// <param name="openApiVersion">The open api version</param>
        public ElectronicChannelValidator(
            VmOpenApiElectronicChannelInVersionBase model,
            IOrganizationService organizationService,
            ICodeService codeService,
            IServiceService serviceService,
            ICommonService commonService,
            IVmOpenApiServiceChannel currentVersion,
            int openApiVersion)
            : base(model, "ElectronicChannel", ChannelType, organizationService, codeService, serviceService, commonService, currentVersion, openApiVersion)
        {}

        /// <summary>
        /// Validates electronic channel model.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            base.Validate(modelState);

            // Check the web pages - model can be empty, but if any items exist in model, all the languages has to be included (required field)
            urls = new LanguageItemListValidator(Model.WebPage, "WebPage", RequiredLanguages, Model.AvailableLanguages);
            urls.Validate(modelState);

            // Validate WCAG level according to selected accessibility classification level (SFIPTV-37)
            accessibilityClassifications = new AccessibilityClassificationListValidator(Model.AccessibilityClassification, OpenApiVersion, RequiredLanguages, Model.AvailableLanguages);
            accessibilityClassifications.Validate(modelState);
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

            if (IsLanguagesMissing(Model.ServiceChannelNamesWithType))
            {
                list.Add("ServiceChannelNames");
            }
            if (IsLanguagesMissing(Model.ServiceChannelDescriptions))
            {
                list.Add("ServiceChannelDescriptions");
            }
            if (IsLanguagesMissing(Model.WebPage))
            {
                list.Add("WebPage");
            }
            if (IsLanguagesMissing(Model.AccessibilityClassification))
            {
                list.Add("AccessibilityClassification");
            }
            return list;
        }
    }
}
