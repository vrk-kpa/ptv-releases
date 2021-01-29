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
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for general description.
    /// </summary>
    public class GeneralDescriptionValidator : EntityBaseValidator<IVmOpenApiGeneralDescriptionInVersionBase>
    {
        private LocalizedListValidator names;
        private LocalizedListValidator descriptions;
        private readonly ServiceClassListValidator serviceClasses;
        private readonly OntologyTermListValidator ontologyTerms;
        private readonly TargetGroupListValidator targetGroups;
        private readonly LifeEventListValidator lifeEvents;
        private readonly IndustrialClassListValidator industrialClasses;
        private readonly PublishingStatusValidator status;

        private readonly IList<string> currentAvailableLanguages;
        private readonly List<IVmOpenApiFintoItemVersionBase> currentTargetGroups;

        /// <summary>
        /// Ctor - general description validator
        /// </summary>
        /// <param name="model">General description model</param>
        /// <param name="fintoService">Finto item service</param>
        /// <param name="ontologyTermDataCache">ontologyTermDataCache</param>
        /// <param name="currentAvailableLanguages">Language versions that are published in currently.</param>
        /// <param name="currentTargetGroups">Target groups that are attached for latest version of general description</param>
        public GeneralDescriptionValidator(
            IVmOpenApiGeneralDescriptionInVersionBase model,
            IFintoService fintoService,
            IOntologyTermDataCache ontologyTermDataCache,
            IList<string> currentAvailableLanguages,
            List<IVmOpenApiFintoItemVersionBase> currentTargetGroups = null)
            : base(model, currentAvailableLanguages, "GeneralDescription")
        {
            serviceClasses = new ServiceClassListValidator(model.ServiceClasses, fintoService);
            ontologyTerms = new OntologyTermListValidator(model.OntologyTerms, ontologyTermDataCache);
            targetGroups = new TargetGroupListValidator(model.TargetGroups, fintoService);
            lifeEvents = new LifeEventListValidator(model.LifeEvents, fintoService);
            industrialClasses = new IndustrialClassListValidator(model.IndustrialClasses, fintoService);
            status = new PublishingStatusValidator(model.PublishingStatus, model.CurrentPublishingStatus);

            this.currentAvailableLanguages = currentAvailableLanguages;
            this.currentTargetGroups = currentTargetGroups;
        }

        /// <summary>
        /// Validates general description model.
        /// </summary>
        public override void Validate(ModelStateDictionary modelState)
        {
            base.Validate(modelState);

            names = new LocalizedListValidator(Model.Names, "Names", RequiredLanguages, new List<string> { NameTypeEnum.Name.ToString() }, Model.AvailableLanguages);
            names.Validate(modelState);

            if (Model.Descriptions?.Where(d => d.Type == DescriptionTypeEnum.Description.ToString()).FirstOrDefault() == null &&
                Model.Descriptions?.Where(d => d.Type == DescriptionTypeEnum.BackgroundDescription.ToString()).FirstOrDefault() == null && RequiredLanguages?.Count > 0)
            {
                modelState.AddModelError("Descriptions", string.Format(CoreMessages.OpenApi.RequiredValueWithLanguageAndTypeNotFound,
                    $"{ DescriptionTypeEnum.Description.ToString()} or { DescriptionTypeEnum.BackgroundDescription.ToString()}", Model.AvailableLanguages.First()));

                descriptions = new LocalizedListValidator(Model.Descriptions, "Descriptions", RequiredLanguages,
                new List<string> { DescriptionTypeEnum.ShortDescription.GetOpenApiValue() }, Model.AvailableLanguages);
            }
            else
            {
                var descriptionType = Model.Descriptions?.Where(d => d.Type != DescriptionTypeEnum.ShortDescription.ToString()).Select(d => d.Type).FirstOrDefault();
                descriptions = new LocalizedListValidator(Model.Descriptions, "Descriptions", RequiredLanguages,
                new List<string> { DescriptionTypeEnum.ShortDescription.GetOpenApiValue(), descriptionType }, Model.AvailableLanguages);
            }

            descriptions.Validate(modelState);

            serviceClasses.Validate(modelState);
            ontologyTerms.Validate(modelState);
            targetGroups.Validate(modelState);
            var tgList = Model.TargetGroups?.Count > 0 ? targetGroups.TargetGroups : currentTargetGroups;
            // Check life events - can be attached only if target group 'Citizens' is attached (PTV-3184)
            if (Model.LifeEvents?.Count > 0)
            {
                if (tgList?.Count > 0)
                {
                    if (!tgList.Any(i => i.Code.StartsWith("KR1")))
                    {
                        modelState.AddModelError("LifeEvents", string.Format(CoreMessages.OpenApi.TargetGroupRequired, "Citizens (KR1)", "life events"));
                    }
                }
                else
                {
                    modelState.AddModelError("LifeEvents", string.Format(CoreMessages.OpenApi.TargetGroupRequired, "Citizens (KR1)", "life events"));
                }
                lifeEvents.Validate(modelState);
            }
            // Check industrial classes - can be attached only if target group 'Businesses and non-government organizations' is attached (PTV-3184)
            if (Model.IndustrialClasses?.Count > 0)
            {
                if (tgList?.Count > 0)
                {
                    if (!tgList.Any(i => i.Code.StartsWith("KR2")))
                    {
                        modelState.AddModelError("IndustrialClasses", string.Format(CoreMessages.OpenApi.TargetGroupRequired, "Businesses and non-government organizations (KR2)", "industrial classes"));
                    }
                }
                else
                {
                    modelState.AddModelError("IndustrialClasses", string.Format(CoreMessages.OpenApi.TargetGroupRequired, "Businesses and non-government organizations (KR2)", "industrial classes"));
                }
                industrialClasses.Validate(modelState);
            }
            status.Validate(modelState);
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

            if (IsLanguagesMissing(Model.Names))
            {
                list.Add("Names");
            }
            if (IsLanguagesMissing(Model.Descriptions))
            {
                list.Add("Descriptions");
            }
            return list;
        }
    }
}
