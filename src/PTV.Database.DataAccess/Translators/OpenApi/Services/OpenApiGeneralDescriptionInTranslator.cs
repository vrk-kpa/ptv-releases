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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Collections.Generic;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescriptionVersioned, IVmOpenApiGeneralDescriptionInVersionBase>), RegisterType.Transient)]
    internal class OpenApiGeneralDescriptionInTranslator : Translator<StatutoryServiceGeneralDescriptionVersioned, IVmOpenApiGeneralDescriptionInVersionBase>
    {
        private ITypesCache typesCache;
        private ILanguageCache languageCache;

        public OpenApiGeneralDescriptionInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManage) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = cacheManage.TypesCache;
            this.languageCache = cacheManage.LanguageCache;
        }

        public override IVmOpenApiGeneralDescriptionInVersionBase TranslateEntityToVm(StatutoryServiceGeneralDescriptionVersioned entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiGeneralDescriptionInTranslator!");
        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(IVmOpenApiGeneralDescriptionInVersionBase vModel)
        {
            if (vModel.Id.IsAssigned())
            {
                // We are updating existing service
                var id = vModel.Id.Value;
                vModel.Names.ForEach(n => n.OwnerReferenceId = id);
                vModel.Descriptions.ForEach(d => d.OwnerReferenceId = id);
                vModel.Legislation.ForEach(l => l.OwnerReferenceId = id);
                vModel.Requirements.ForEach(r => r.OwnerReferenceId = id);
            }
            var definitions = CreateViewModelEntityDefinition<StatutoryServiceGeneralDescriptionVersioned>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.Id.HasValue, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.HasValue, i => o => i.Id.Value == o.Id)
                .UseVersioning<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(o => o);

            // Set available languages
            var languages = GetAvailableLanguages(vModel);
            if (languages.Count > 0)
            {
                definitions.AddCollection(i => languages, o => o.LanguageAvailabilities, true);
            }

            if (vModel.Names != null && vModel.Names.Count > 0)
            {
                definitions.AddCollection(i => i.Names, o => o.Names, true);
            }

            if (vModel.Descriptions != null && vModel.Descriptions.Count > 0)
            {
                definitions.AddCollection(i => i.Descriptions, o => o.Descriptions, true);
            }

            if (vModel.Languages != null && vModel.Languages.Count > 0)
            {
                var languageList = new List<VmOpenApiStringItem>();
                vModel.Languages.ForEach(l => languageList.Add(new VmOpenApiStringItem { Value = l, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => languageList, o => o.Languages, false);
            }

            if (vModel.Requirements?.Count > 0)
            {
                definitions.AddCollection(i => i.Requirements, o => o.StatutoryServiceRequirements, false);
            }

            if (!string.IsNullOrEmpty(vModel.Type))
            {
                definitions.AddSimple(i => typesCache.Get<ServiceType>(i.Type), o => o.TypeId);
            }

            //if (!string.IsNullOrEmpty(vModel.ServiceChargeType))
            if (vModel.DeleteServiceChargeType || !string.IsNullOrEmpty(vModel.ServiceChargeType))
            {
                var chargeType = (string.Compare(vModel.ServiceChargeType, ServiceChargeTypeEnum.Other.ToString(), StringComparison.CurrentCultureIgnoreCase) == 0)
                        ? null
                        : vModel.ServiceChargeType;
                definitions.AddSimple(i => (chargeType == null) ? (Guid?)null : typesCache.Get<ServiceChargeType>(chargeType), o => o.ChargeTypeId);
            }

            if (vModel.ServiceClasses?.Count > 0)
            {
                var classes = new List<VmOpenApiStringItem>();
                vModel.ServiceClasses.ForEach(o => classes.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => classes, o => o.ServiceClasses, false);
            }

            if (vModel.OntologyTerms?.Count > 0)
            {
                var terms = new List<VmOpenApiStringItem>();
                vModel.OntologyTerms.ForEach(o => terms.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => terms, o => o.OntologyTerms, false);
            }

            if (vModel.TargetGroups?.Count > 0)
            {
                var groups = new List<VmOpenApiStringItem>();
                vModel.TargetGroups.ForEach(o => groups.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => groups, o => o.TargetGroups, false);
            }

            if (vModel.LifeEvents?.Count > 0)
            {
                var events = new List<VmOpenApiStringItem>();
                vModel.LifeEvents.ForEach(o => events.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => events, o => o.LifeEvents, false);
            }
            else if (vModel.DeleteAllLifeEvents) // life events need to be removed
            {
                definitions.AddCollection(i => new List<VmOpenApiStringItem>(), o => o.LifeEvents, false);
            }

            if (vModel.IndustrialClasses?.Count > 0)
            {
                var classes = new List<VmOpenApiStringItem>();
                vModel.IndustrialClasses.ForEach(o => classes.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => classes, o => o.IndustrialClasses, false);
            }
            else if (vModel.DeleteAllIndustrialClasses) // industrial classes need to be removed
            {
                definitions.AddCollection(i => new List<VmOpenApiStringItem>(), o => o.IndustrialClasses, false);
            }

            if (vModel.DeleteAllLaws || vModel.Legislation?.Count > 0)
            {
                definitions.AddCollection(i => i.Legislation, o => o.StatutoryServiceLaws, false);
            }

            return definitions.GetFinal();
        }

        private List<VmOpenApiLanguageAvailability> GetAvailableLanguages(IVmOpenApiGeneralDescriptionInVersionBase vModel)
        {
            var languages = new List<VmOpenApiLanguageAvailability>();
            var currentPublishingStatus = !string.IsNullOrEmpty(vModel.CurrentPublishingStatus) ? vModel.CurrentPublishingStatus : PublishingStatus.Draft.ToString();
            vModel.AvailableLanguages.ForEach(lang =>
            {
                languages.Add(new VmOpenApiLanguageAvailability() { Language = lang, OwnerReferenceId = vModel.Id, PublishingStatus = currentPublishingStatus });
            });

            return languages;
        }
    }
}
