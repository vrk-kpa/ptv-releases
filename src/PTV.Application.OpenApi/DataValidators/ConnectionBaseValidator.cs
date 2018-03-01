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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V7;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Base validator for connection list.
    /// </summary>
    public class ConnectionBaseValidator<TModel> : BaseValidator<IList<TModel>> where TModel : IVmOpenApiServiceServiceChannelInVersionBase
    {
        private ICodeService codeService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="codeService"></param>
        /// <param name="propertyName"></param>
        public ConnectionBaseValidator(IList<TModel> model, ICodeService codeService,
            string propertyName = "ChannelRelations") : base(model, propertyName)
        {
            this.codeService = codeService;
        }

        /// <summary>
        /// Validates service hours and contact information.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            // Validate service hours
            var channelsWithHours = Model.Where(m => m.ServiceHours?.Count > 0).ToList();
            if (channelsWithHours.Count > 0)
            {
                var hourListValidator = new ServiceHourListValidator<V4VmOpenApiServiceHour>(channelsWithHours.SelectMany(h => h.ServiceHours).ToList(), $"{PropertyName}.ServiceHours");
                hourListValidator.Validate(modelState);
            }
            // Validate contact information
            var channelsWithContactInfo = Model.Where(m => m.ContactDetails != null).ToList();
            if (channelsWithContactInfo.Count > 0)
            {
                // Validate addresses
                var channelsWithAddresses = channelsWithContactInfo.Where(m => m.ContactDetails.Addresses?.Count > 0).ToList();
                if (channelsWithAddresses.Count > 0)
                {
                    var addressListValidator = new AddressListValidator<V7VmOpenApiAddressIn>(channelsWithAddresses.SelectMany(a => a.ContactDetails.Addresses).ToList(), codeService, $"{PropertyName}.ContactDetails.Addresses");
                    addressListValidator.Validate(modelState);
                }

                // Validate phones
                var channelsWithPhones = channelsWithContactInfo.Where(m => m.ContactDetails.Phones?.Count > 0).ToList();
                if (channelsWithPhones.Count > 0)
                {
                    // Validate prefix numbers - only once per prefix number
                    var prefixes = channelsWithPhones.SelectMany(p => p.ContactDetails.Phones).Where(p => p.PrefixNumber != null).Select(p => p.PrefixNumber).Distinct().ToList();
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
