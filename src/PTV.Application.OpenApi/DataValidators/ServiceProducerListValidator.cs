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
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using System;
using System.Collections.Generic;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for organization id list.
    /// </summary>
    public class ServiceProducerListValidator : BaseValidator<IList<V9VmOpenApiServiceProducerIn>>
    {
        private ICommonService commonService;
        private IList<Guid> availableOrganizations;

        /// <summary>
        /// Ctor - organization id list validator.
        /// </summary>
        /// <param name="model">organization id list</param>
        /// <param name="availableOrganizations">list of organizations that can be used as service producers</param>
        /// <param name="commonService">Common service</param>
        /// <param name="propertyName">Property name</param>
        public ServiceProducerListValidator(IList<V9VmOpenApiServiceProducerIn> model, IList<Guid> availableOrganizations, ICommonService commonService, string propertyName = "ServiceProducers") : base(model, propertyName)
        {
            this.commonService = commonService;
            this.availableOrganizations = availableOrganizations;
        }

        /// <summary>
        /// Checks if organization id list is valid or not.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            var i = 0;
            Model.ForEach(serviceProducer =>
            {
                var selfProduced = ProvisionTypeEnum.SelfProduced.ToString();
                // check provision type SelfProduced - only defined set of organizations can be used (availableOrganizations)
                if (serviceProducer.ProvisionType == selfProduced)
                {
                    serviceProducer.Organizations.ForEach(o =>
                    {
                        if (!availableOrganizations.Contains(o))
                        {
                            modelState.AddModelError(PropertyName, $"Field invalid. When 'ProvisionType' has value { selfProduced } only main responsible and other responsible organizations can be used: '{availableOrganizations.ConvertToString()}'.");
                            return;
                        }
                    });

                    // no additional information allowed when provision type is self produced
                    if (serviceProducer.AdditionalInformation?.Count > 0)
                    {
                        modelState.AddModelError(PropertyName, $"No AdditionalInformation accepted when 'ProvisionType' has value { selfProduced }.");
                        return;
                    }
                }

                // for provision type Other or PurchaseServices user needs to define either organizations or additional information
                var other = ProvisionTypeEnum.Other.ToString(); var purchaseServices = ProvisionTypeEnum.PurchaseServices.ToString();
                if (serviceProducer.ProvisionType == other || serviceProducer.ProvisionType == purchaseServices)
                {
                    if (serviceProducer.Organizations?.Count == 0 && serviceProducer.AdditionalInformation?.Count == 0)
                    {
                        modelState.AddModelError(PropertyName, $"Either AdditionalInformation or Organizations needs to be defined when 'ProvisionType' has value {other} or { purchaseServices }.");
                        return;
                    }
                }

                if (serviceProducer.Organizations?.Count > 0)
                {
                    var list = new List<string>();
                    serviceProducer.Organizations.ForEach(p => list.Add(p.ToString()));
                    var organizations = new OrganizationIdListValidator(list, commonService, $"{PropertyName}[{ i++ }].Organizations");
                    organizations.Validate(modelState);
                }                
            });
        }
    }
}
