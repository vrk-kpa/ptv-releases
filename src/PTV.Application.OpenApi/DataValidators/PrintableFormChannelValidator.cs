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

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for printable form channel.
    /// </summary>
    public class PrintableFormChannelValidator : ServiceChannelValidator<VmOpenApiPrintableFormChannelInVersionBase>
    {
        private readonly AddressListValidator<VmOpenApiAddressInVersionBase> addresses;
        private static readonly string ChannelType = ServiceChannelTypeEnum.PrintableForm.ToString();

        /// <summary>
        /// Ctor - printable form channel validator
        /// </summary>
        /// <param name="model">printable form channel model</param>
        /// <param name="organizationService">Common service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="serviceService">Service service</param>
        /// <param name="commonService"></param>
        /// <param name="currentVersion">Current version</param>
        /// <param name="openApiVersion">The open api version</param>
        public PrintableFormChannelValidator(
            VmOpenApiPrintableFormChannelInVersionBase model,
            IOrganizationService organizationService,
            ICodeService codeService,
            IServiceService serviceService,
            ICommonService commonService,
            IVmOpenApiServiceChannel currentVersion,
            int openApiVersion) :
            base(model, "PrintableFormChannel", ChannelType, organizationService, codeService, serviceService, commonService, currentVersion, openApiVersion)
        {
            addresses = new AddressListValidator<VmOpenApiAddressInVersionBase>(
                model.DeliveryAddresses?.Select(a => new VmOpenApiAddressInVersionBase
                {
                    SubType = a.SubType,
                    StreetAddress = a.SubType == AddressTypeEnum.Street.ToString() && a.StreetAddress != null
                        ? new VmOpenApiAddressStreetWithCoordinatesIn
                            {
                                PostalCode = a.StreetAddress.PostalCode,
                                Municipality = a.StreetAddress.Municipality

                            }
                        : null,
                    PostOfficeBoxAddress = a.SubType == AddressTypeEnum.PostOfficeBox.ToString() && a.PostOfficeBoxAddress != null
                        ? new VmOpenApiAddressPostOfficeBoxIn
                            {
                                PostalCode = a.PostOfficeBoxAddress.PostalCode,
                                Municipality = a.PostOfficeBoxAddress.Municipality

                            }
                        : null
                }).ToList(),
                codeService);
        }
        /// <summary>
        /// Validates printable form channel model.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            base.Validate(modelState);
            addresses?.Validate(modelState);

            // Validate required channelUrls property
            var urls = new LocalizedListValidator(Model.ChannelUrls, "ChannelUrls", requiredLanguages: RequiredLanguages, availableLanguages: Model.AvailableLanguages);
            urls.Validate(modelState);

            if (Model.DeleteAllChannelUrls && Model.PublishingStatus == PublishingStatus.Published.ToString() &&
                Model.ChannelUrls.IsNullOrEmpty())
            {
                modelState.AddModelError(nameof(Model.ChannelUrls), "At least one URL needs to be specified for a published channel.");
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

            if (IsLanguagesMissing(Model.ServiceChannelNamesWithType))
            {
                list.Add("ServiceChannelNames");
            }
            if (IsLanguagesMissing(Model.ServiceChannelDescriptions))
            {
                list.Add("ServiceChannelDescriptions");
            }
            if (IsLanguagesMissing(Model.ChannelUrls))
            {
                list.Add("ChannelUrls");
            }
            return list;
        }
    }
}
