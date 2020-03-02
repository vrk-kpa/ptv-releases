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
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service location channel.
    /// </summary>
    public class ServiceLocationChannelValidator : ServiceChannelValidator<VmOpenApiServiceLocationChannelInVersionBase>
    {
        private static readonly string ChannelType = ServiceChannelTypeEnum.ServiceLocation.ToString();
        private readonly PhoneNumberListValidator<V4VmOpenApiPhone> phones;
        private readonly PhoneNumberListValidator<V4VmOpenApiPhoneSimple> faxNumbers;
        private readonly AddressListValidator<V9VmOpenApiAddressLocationIn> addresses;
        private readonly int openApiVersion;

        /// <summary>
        /// Ctor - service location channel validator
        /// </summary>
        /// <param name="model">service location channel model</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="serviceService">Service service</param>
        /// <param name="commonService"></param>
        /// <param name="currentVersion">Current version</param>
        /// <param name="openApiVersion">Open api version</param>
        public ServiceLocationChannelValidator(VmOpenApiServiceLocationChannelInVersionBase model,
            IOrganizationService organizationService,
            ICodeService codeService,
            IServiceService serviceService,
            ICommonService commonService,
            IVmOpenApiServiceChannel currentVersion,
            int openApiVersion) :
            base(model, "ServiceLocationChannel", ChannelType, organizationService, codeService, serviceService, commonService, currentVersion, openApiVersion)
        {
            phones = new PhoneNumberListValidator<V4VmOpenApiPhone>(model.PhoneNumbers, codeService);
            faxNumbers = new PhoneNumberListValidator<V4VmOpenApiPhoneSimple>(model.FaxNumbers, codeService, "FaxNumbers");
            addresses = new AddressListValidator<V9VmOpenApiAddressLocationIn>(model.Addresses, codeService);
            this.openApiVersion = openApiVersion;
        }

        /// <summary>
        /// Validates service location channel model
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            base.Validate(modelState);

            // Validate displayNameType field against current (and new) service channel names (PTV-4340, SFIPTV-236)
            if (Model.DisplayNameType != null && CurrentVersion != null && CurrentVersion.ServiceChannelNames != null && CurrentVersion.ServiceChannelNames.Any())
            {
                var allNames = new List<VmOpenApiLocalizedListItem>();
                // Let's convert the open api enum values
                allNames.AddRange(CurrentVersion.ServiceChannelNames);
                if (Model.ServiceChannelNamesWithType?.Count > 0) { allNames.AddRange(Model.ServiceChannelNamesWithType); }
                Model.DisplayNameType.ForEach(d =>
                {
                    if (!allNames.Any(n => n.Type == d.Type && n.Language == d.Language))
                    {
                        modelState.AddModelError("DisplayNameType", $"The field is invalid. Location channel is missing name with type '{ d.Type.GetOpenApiEnumValue<NameTypeEnum>() }' and language '{ d.Language }'.");
                    }
                });
            }

            phones.Validate(modelState);
            faxNumbers.Validate(modelState);

            // Validate addresses
            // validation rules, see PTV-2910 - Update 1.6.2018: new validation rules PTV-2470, PTV-2835, PTV-2747
            var i = 0;
            Model.Addresses.ForEach(a =>
            {
                // Only following combinations are allowed:
                // Type is 'Location', SubType can be 'Single', 'Other' or 'Abroad'.
                // Type is 'Postal', SubType can be 'Street', 'PostOfficeBox' or 'Abroad'.
                if (!(a.Type == AddressConsts.LOCATION && (a.SubType == AddressConsts.SINGLE || a.SubType == AddressConsts.OTHER || a.SubType == AddressConsts.ABROAD)) &&
                    !(a.Type == AddressCharacterEnum.Postal.ToString() && (a.SubType == AddressConsts.STREET || a.SubType == AddressConsts.POSTOFFICEBOX || a.SubType == AddressConsts.ABROAD)))
                {
                    modelState.AddModelError($"Addresses[{i}].SubType", $"'{a.SubType}' is not allowed value of 'SubType' when 'Type' has value '{ a.Type}'.");
                }

                i++;
            });

            // If SubType is 'Abroad' no other visiting address types can be defined. Several addresses with SubType 'Abroad' can exist.
            var visitingAddresses = Model.Addresses.Where(a => a.Type == AddressConsts.LOCATION);
            if (visitingAddresses.Any(a => a.SubType == AddressConsts.ABROAD) && visitingAddresses.Any(a => a.SubType != AddressConsts.ABROAD))
            {
                modelState.AddModelError("Addresses", $"The field is invalid. When 'SubType' has value '{ AddressConsts.ABROAD }' no other types of { AddressConsts.LOCATION } addresses can be defined.");
            }

            // Check that all required languages can be found form addresses list
            var addressLanguages = new HashSet<string>();
            addressLanguages.GetAvailableLanguages(Model.Addresses);
            var notIncludedLanguages = RequiredLanguages?.Except(addressLanguages);
            if (notIncludedLanguages != null && notIncludedLanguages.ToList()?.Count > 0)
            {
                modelState.AddModelError("Addresses", string.Format(CoreMessages.OpenApi.RequiredLanguageValueNotFound, string.Join(", ", notIncludedLanguages)));
            }
            addresses.Validate(modelState);
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
            if (IsLanguagesMissing(Model.Addresses))
            {
                list.Add("Addresses");
            }
            return list;
        }
    }
}
