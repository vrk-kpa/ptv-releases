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
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescriptionVersioned, IVmOpenApiGeneralDescriptionInVersionBase>), RegisterType.Transient)]
    internal class OpenApiGeneralDescriptionInTranslator : Translator<StatutoryServiceGeneralDescriptionVersioned, IVmOpenApiGeneralDescriptionInVersionBase>
    {
        private ITypesCache typesCache;

        public OpenApiGeneralDescriptionInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManage) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = cacheManage.TypesCache;
        }

        public override IVmOpenApiGeneralDescriptionInVersionBase TranslateEntityToVm(StatutoryServiceGeneralDescriptionVersioned entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiGeneralDescriptionInTranslator!");
        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(IVmOpenApiGeneralDescriptionInVersionBase vModel)
        {
            if (vModel.VersionId.IsAssigned())
            {
                // We are updating existing service
                var id = vModel.VersionId.Value;
                vModel.Names.ForEach(n => n.OwnerReferenceId = id);
                vModel.Descriptions.ForEach(d => d.OwnerReferenceId = id);
                vModel.Legislation.ForEach(l => l.OwnerReferenceId = id);//TODO!
                vModel.Requirements.ForEach(r => r.OwnerReferenceId = id);
            }
            else
            {
                // Set default values for POST operation
                if (vModel.GeneralDescriptionType.IsNullOrEmpty())
                {
                    vModel.GeneralDescriptionType = GeneralDescriptionTypeEnum.Municipality.ToString();
                }
            }
            var definitions = CreateViewModelEntityDefinition<StatutoryServiceGeneralDescriptionVersioned>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.VersionId.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.VersionId.IsAssigned(), i => o => i.VersionId.Value == o.Id)
                .UseVersioning<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(o => o);

            // Set available languages
            var languages = GetAvailableLanguages(vModel);
            if (languages.Count > 0)
            {
                definitions.AddCollectionWithRemove(i => languages, o => o.LanguageAvailabilities, x => true);
            }

            if (vModel.Names != null && vModel.Names.Count > 0)
            {
                definitions.AddCollectionWithRemove(i => i.Names, o => o.Names, x => true);
            }

            if (vModel.Descriptions != null && vModel.Descriptions.Count > 0)
            {
                definitions.AddCollectionWithRemove(i => i.Descriptions, o => o.Descriptions, x => true);
            }

            if (vModel.Requirements?.Count > 0)
            {
                definitions.AddCollectionWithRemove(i => i.Requirements, o => o.StatutoryServiceRequirements, x => true);
            }

            if (!string.IsNullOrEmpty(vModel.Type))
            {
                definitions.AddSimple(i => typesCache.Get<ServiceType>(i.Type.GetEnumValueByOpenApiEnumValue<ServiceTypeEnum>()), o => o.TypeId);
            }

            if (vModel.DeleteServiceChargeType || !string.IsNullOrEmpty(vModel.ServiceChargeType))
            {
                var chargeType = string.IsNullOrEmpty(vModel.ServiceChargeType) ? null : vModel.ServiceChargeType.GetEnumValueByOpenApiEnumValue<ServiceChargeTypeEnum>();
                if (string.Compare(chargeType, ServiceChargeTypeEnum.Other.ToString(), StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    chargeType = null;
                }
                definitions.AddSimple(i => (chargeType == null) ? (Guid?)null : typesCache.Get<ServiceChargeType>(chargeType), o => o.ChargeTypeId);
            }

            if (vModel.ServiceClasses?.Count > 0)
            {
                var classes = new List<VmOpenApiStringItem>();

                vModel.ServiceClasses.ForEach(o => classes.Add(new VmOpenApiStringItem
                    {Value = o, OwnerReferenceId = vModel.VersionId}));
                definitions.AddCollectionWithRemove(i => classes, o => o.ServiceClasses, x => true);
            }

            if (vModel.OntologyTerms?.Count > 0)
            {
                var terms = new List<VmOpenApiStringItem>();
                vModel.OntologyTerms.ForEach(o => terms.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.VersionId }));
                definitions.AddCollectionWithRemove(i => terms, o => o.OntologyTerms, x => true);
            }

            if (vModel.TargetGroups?.Count > 0)
            {
                var groups = new List<VmOpenApiStringItem>();
                vModel.TargetGroups.ForEach(o => groups.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.VersionId }));
                definitions.AddCollectionWithRemove(i => groups, o => o.TargetGroups, x => true);
            }

            if (vModel.LifeEvents?.Count > 0 || vModel.DeleteAllLifeEvents)
            {
                var events = new List<VmOpenApiStringItem>();
                vModel.LifeEvents?.ForEach(o => events.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.VersionId }));
                definitions.AddCollectionWithRemove(i => events, o => o.LifeEvents, x => true);
            }

            if (vModel.IndustrialClasses?.Count > 0 || vModel.DeleteAllIndustrialClasses)
            {
                var classes = new List<VmOpenApiStringItem>();
                vModel.IndustrialClasses?.ForEach(o => classes.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.VersionId }));
                definitions.AddCollectionWithRemove(i => classes, o => o.IndustrialClasses, x => true);
            }

            if (vModel.DeleteAllLaws || vModel.Legislation?.Count > 0)
            {
                definitions.AddCollectionWithRemove(i => i.Legislation, o => o.StatutoryServiceLaws, x => true);
            }

            if (!string.IsNullOrEmpty(vModel.GeneralDescriptionType))
            {
                definitions.AddSimple(i => typesCache.Get<GeneralDescriptionType>(i.GeneralDescriptionType), o => o.GeneralDescriptionTypeId);
            }

            return definitions.GetFinal();
        }

        private List<VmOpenApiLanguageAvailability> GetAvailableLanguages(IVmOpenApiGeneralDescriptionInVersionBase vModel)
        {
            var languages = new List<VmOpenApiLanguageAvailability>();

            var currentPublishingStatus = string.IsNullOrEmpty(vModel.CurrentPublishingStatus)
                ? PublishingStatus.Draft.ToString()
                : vModel.CurrentPublishingStatus == PublishingStatus.Published.ToString() && vModel.PublishingStatus == PublishingStatus.Modified.ToString() // SFIPTV-234
                    ? PublishingStatus.Modified.ToString()
                    : vModel.CurrentPublishingStatus;

            vModel.AvailableLanguages.ForEach(lang =>
            {
                languages.Add(new VmOpenApiLanguageAvailability { Language = lang, OwnerReferenceId = vModel.VersionId, PublishingStatus = currentPublishingStatus });
            });

            return languages;
        }
    }
}
