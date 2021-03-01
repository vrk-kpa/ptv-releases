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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Base validator for connection list.
    /// </summary>
    public class ConnectionBaseValidator<TModel, TModelContact, TModelServiceHours, TModelOpeningTime>
        : BaseValidator<IList<TModel>>
        where TModel : IOpenApiConnectionBase<TModelContact, TModelServiceHours, TModelOpeningTime>
        where TModelContact : IVmOpenApiContactDetailsInVersionBase
        where TModelServiceHours : IVmOpenApiServiceHourBase<TModelOpeningTime>
        where TModelOpeningTime : IVmOpenApiDailyOpeningTime
    {
        private readonly ICodeService codeService;
        private readonly int openApiVersion;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="codeService"></param>
        /// <param name="openApiVersion"></param>
        /// <param name="propertyName"></param>
        public ConnectionBaseValidator(IList<TModel> model, ICodeService codeService,
            int openApiVersion,
            string propertyName = "ChannelRelations") : base(model, propertyName)
        {
            this.codeService = codeService;
            this.openApiVersion = openApiVersion;
        }

        /// <summary>
        /// Validates service hours and contact information.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            var i = 0;
            Model.ForEach(m =>
            {
                if (m.ContactDetails != null)
                {
                    // Validate addresses
                    if (m.ContactDetails.Addresses?.Count > 0)
                    {
                        var addressListValidator = new AddressListValidator<V7VmOpenApiAddressContactIn>(m.ContactDetails.Addresses, codeService, $"[{i}]{PropertyName}.ContactDetails.Addresses");
                        addressListValidator.Validate(modelState);
                    }
                }
                i++;
            });

            // Validate phones
            var itemsWithPhones = Model.Where(m => m.ContactDetails != null)?.Select(m => m.ContactDetails).Where(m => m.PhoneNumbers?.Count > 0).ToList();
            if (itemsWithPhones.Count > 0)
            {
                // Validate prefix numbers - only once per prefix number
                var prefixes = itemsWithPhones.SelectMany(p => p.PhoneNumbers).Where(p => p.PrefixNumber != null).Select(p => p.PrefixNumber).Distinct().ToList();
                prefixes.ForEach(prefix =>
                {
                    var prefixValidator = new PrefixNumberValidator(prefix, codeService, $"{PropertyName}.ContactDetails.Phones.PrefixNumber");
                    prefixValidator.Validate(modelState);
                });
            }

            // Validate service hours
            var channelsWithHours = Model.Where(m => m.ServiceHours?.Count > 0).ToList();
            if (channelsWithHours.Count > 0)
            {
                var hourListValidator = new ServiceHourListValidator<TModelServiceHours, TModelOpeningTime>(channelsWithHours.SelectMany(h => h.ServiceHours).ToList(), openApiVersion, $"{PropertyName}.ServiceHours");
                hourListValidator.Validate(modelState);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="hours"></param>
        /// <param name="modelState"></param>
        protected void ValidateServiceHours(IList<TModelServiceHours> hours, ModelStateDictionary modelState)
        {
            if (hours?.Count > 0)
            {
                var hourListValidator = new ServiceHourListValidator<TModelServiceHours, TModelOpeningTime>(hours, openApiVersion, $"{PropertyName}.ServiceHours");
                hourListValidator.Validate(modelState);
            }
        }

        /// <summary>
        /// Validates contact information
        /// </summary>
        /// <param name="list"></param>
        /// <param name="modelState"></param>
        protected void ValidateContactInfo<TContactInfoModel>(List<TContactInfoModel> list, ModelStateDictionary modelState)
            where TContactInfoModel : IVmOpenApiContactDetailsInVersionBase
        {
            if (list?.Count > 0)
            {

                // Validate addresses
                var itemsWithAddresses = list.Where(m => m.Addresses?.Count > 0).ToList();
                if (itemsWithAddresses.Count > 0)
                {
                    var addressListValidator = new AddressListValidator<V7VmOpenApiAddressContactIn>(itemsWithAddresses.SelectMany(a => a.Addresses).ToList(), codeService, $"{PropertyName}.ContactDetails.Addresses");
                    addressListValidator.Validate(modelState);
                }

                // Validate phones
                var itemsWithPhones = list.Where(m => m.PhoneNumbers?.Count > 0).ToList();
                if (itemsWithPhones.Count > 0)
                {
                    // Validate prefix numbers - only once per prefix number
                    var prefixes = itemsWithPhones.SelectMany(p => p.PhoneNumbers).Where(p => p.PrefixNumber != null).Select(p => p.PrefixNumber).Distinct().ToList();
                    prefixes.ForEach(prefix =>
                    {
                        var prefixValidator = new PrefixNumberValidator(prefix, codeService, $"{PropertyName}.ContactDetails.Phones.PrefixNumber");
                        prefixValidator.Validate(modelState);
                    });
                }
            }
        }
    }
}
