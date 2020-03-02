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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Interfaces.OpenApi;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service collection.
    /// </summary>
    public class ServiceCollectionValidator : BaseValidator<IVmOpenApiServiceCollectionInVersionBase>
    {
        private readonly LanguageItemListValidator names;
        private LanguageItemListValidator descriptions;
        private readonly PublishingStatusValidator status;
        //private readonly UserOrganizationIdValidator mainOrganization;
        private readonly ServiceIdListValidator services;
        private OwnOrganizationValidator mainOrganization;
        private readonly IList<string> newLanguages;
           // private readonly IOrganizationService organizationService;

        /// <summary>
        /// Ctor - service collection validator
        /// </summary>
        /// <param name="model">Service model</param>
        /// <param name="serviceService">Service service</param>
        /// <param name="newLanguages">Languages that should be validated within lists</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="openApiVersion">Open api version</param>
        public ServiceCollectionValidator(
            IVmOpenApiServiceCollectionInVersionBase model,
            IServiceService serviceService,
            IList<string> newLanguages,
            IOrganizationService organizationService,
            int openApiVersion
            ) : base(model, "ServiceCollection")
        {
            if (model == null)
            {
                throw new ArgumentNullException(PropertyName, $"{PropertyName} must be defined.");
            }

            names = new LanguageItemListValidator(model.ServiceCollectionNames, "ServiceCollectionNames", newLanguages);
            status = new PublishingStatusValidator(model.PublishingStatus, model.CurrentPublishingStatus);
            mainOrganization = new OwnOrganizationValidator(model.OrganizationId, organizationService, new List<string>(), openApiVersion > 10 ? "OrganizationId" : "MainResponsibleOrganization");

            // Validate service ids
            if (model.ServiceCollectionServices != null)
            {
                services = new ServiceIdListValidator(model.ServiceCollectionServices.ToList(), serviceService, "Services");
            }

            this.newLanguages = newLanguages;
        }

        /// <summary>
        /// Checks if service model is valid or not.
        /// </summary>
        public override void Validate(ModelStateDictionary modelState)
        {
            // Validate names
            names.Validate(modelState);

            // Validate all required descriptions
            descriptions = new LanguageItemListValidator(Model.ServiceCollectionDescriptions, "ServiceCollectionDescriptions", newLanguages);

            descriptions?.Validate(modelState);

            // Validate publishing status
            status.Validate(modelState);

            // Validate services
            services?.Validate(modelState);

            // Validate main organization (must be user organization)
            mainOrganization.Validate(modelState);
        }
    }
}
