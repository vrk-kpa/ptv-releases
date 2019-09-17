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
using PTV.Framework;
using System;
using System.Collections.Generic;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for organization id. Checks also if the given organization is one of users own.
    /// </summary>
    public class OwnOrganizationValidator : BaseValidator<string>
    {
        private IOrganizationService organizationService;
        private IList<string> availableLanguages;

        /// <summary>
        /// Ctor - organization id validator.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="organizationService"></param>
        /// <param name="availableLanguages"></param>
        /// <param name="propertyName"></param>
        public OwnOrganizationValidator(string model, IOrganizationService organizationService, List<string> availableLanguages = null, string propertyName = "OrganizationId") : base(model, propertyName)
        {
            this.organizationService = organizationService;
            this.availableLanguages = availableLanguages;
        }

        /// <summary>
        /// Checks if organization id is valid or not.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (string.IsNullOrEmpty(Model))
            {
                return;
            }

            Guid? guid = Model.ParseToGuid();
            if (!guid.IsAssigned())
            {
                modelState.AddModelError(PropertyName, CoreMessages.OpenApi.RecordNotFound);
                return;
            }
            try
            {
                var organizationAvailablelanguages = organizationService.GetAvailableLanguagesForOwnOrganization(guid.Value);
                if (organizationAvailablelanguages == null)
                {
                    modelState.AddModelError(PropertyName, CoreMessages.OpenApi.RecordNotFound);
                    return;
                }
                if (availableLanguages != null)
                {
                    availableLanguages.ForEach(l =>
                    {
                        if (!organizationAvailablelanguages.Contains(l))
                        {
                            modelState.AddModelError(PropertyName, $"Organisation language version '{l}' missing! Organisation must be described in the same language as the item.");
                        }
                    });
                }
            }
            catch(Exception ex)
            {
                modelState.AddModelError(PropertyName, ex.Message);
            }
        }
    }
}
