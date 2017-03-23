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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<ServiceVersioned, IVmOpenApiServiceInVersionBase>), RegisterType.Transient)]
    internal class OpenApiServiceInTranslator : OpenApiServiceBaseTranslator<IVmOpenApiServiceInVersionBase>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override IVmOpenApiServiceInVersionBase TranslateEntityToVm(ServiceVersioned entity)
        {
            return base.TranslateEntityToVm(entity);
        }
        public override ServiceVersioned TranslateVmToEntity(IVmOpenApiServiceInVersionBase vModel)
        {
            if (vModel.Id.IsAssigned())
            {
                var entityId = vModel.Id.Value;
                vModel.ServiceOrganizations.ForEach(o => { o.ServiceId = entityId.ToString(); o.OwnerReferenceId = entityId; });
                vModel.Keywords.ForEach(k => k.OwnerReferenceId = entityId);
                vModel.Legislation.ForEach(l => l.OwnerReferenceId = entityId);
            }

            var definitions = base.CreateBaseVmEntityDefinitions(vModel)
                .DisableAutoTranslation();

            // Set available languages
            var languages = GetAvailableLanguages(vModel);
            if (languages.Count > 0)
            {
                definitions.AddCollection(i => languages, o => o.LanguageAvailabilities, true);
            }

            if (!string.IsNullOrEmpty(vModel.Type))
            {
                definitions.AddSimple(i => typesCache.Get<ServiceType>(i.Type), o => o.TypeId);
            }

            // set chargeType
            var chargeType = (string.Compare(vModel.ServiceChargeType, ServiceChargeTypeEnum.Other.ToString(), StringComparison.CurrentCultureIgnoreCase) == 0)
                    ? null
                    : vModel.ServiceChargeType;

            definitions.AddSimple(i => (chargeType == null) ? (Guid?) null : typesCache.Get<ServiceChargeType>(chargeType), o => o.ServiceChargeTypeId);

            if (vModel.DeleteAllMunicipalities || vModel.Municipalities?.Count > 0)
            {
                var municipalities = new List<VmOpenApiStringItem>();
                vModel.Municipalities.ForEach(m => municipalities.Add(new VmOpenApiStringItem { Value = m, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => municipalities, o => o.ServiceMunicipalities, false);
            }

            if (vModel.ServiceClasses?.Count > 0)
            {
                var classes = new List<VmOpenApiStringItem>();
                vModel.ServiceClasses.ForEach(o => classes.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => classes, o => o.ServiceServiceClasses, false);
            }

            if (vModel.OntologyTerms?.Count > 0)
            {
                var terms = new List<VmOpenApiStringItem>();
                vModel.OntologyTerms.ForEach(o => terms.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => terms, o => o.ServiceOntologyTerms, false);
            }

            if (vModel.TargetGroups?.Count > 0)
            {
                var groups = new List<VmOpenApiStringItem>();
                vModel.TargetGroups.ForEach(o => groups.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => groups, o => o.ServiceTargetGroups, false);
            }

            if (vModel.DeleteAllLifeEvents || vModel.LifeEvents?.Count > 0)
            {
                var events = new List<VmOpenApiStringItem>();
                vModel.LifeEvents.ForEach(o => events.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => events, o => o.ServiceLifeEvents, false);
            }

            if (vModel.DeleteAllIndustrialClasses || vModel.IndustrialClasses?.Count > 0)
            {
                var classes = new List<VmOpenApiStringItem>();
                vModel.IndustrialClasses.ForEach(o => classes.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => classes, o => o.ServiceIndustrialClasses, false);
            }

            if (vModel.DeleteAllLaws || vModel.Legislation?.Count > 0)
            {
                definitions.AddCollection(i => i.Legislation, o => o.ServiceLaws, false);
            }

            if (vModel.DeleteAllKeywords || vModel.Keywords?.Count > 0)
            {
                definitions.AddCollection(i => i.Keywords, o => o.ServiceKeywords, false);
            }

            if (vModel.ServiceOrganizations?.Count > 0)
            {
                definitions.AddCollection(i => i.ServiceOrganizations, o => o.OrganizationServices, false);
            }

            if (!string.IsNullOrEmpty(vModel.StatutoryServiceGeneralDescriptionId))
            {
                Guid id;
                id = Guid.TryParse(vModel.StatutoryServiceGeneralDescriptionId, out id) ? id : Guid.Empty;
                definitions = definitions.AddSimple(i => id, o => o.StatutoryServiceGeneralDescriptionId);
            }

            return definitions.GetFinal();
        }

        private List<VmOpenApiLanguageAvailability> GetAvailableLanguages(IVmOpenApiServiceInVersionBase vModel)
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
