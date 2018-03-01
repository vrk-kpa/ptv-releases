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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;
using System;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>), RegisterType.Transient)]
    internal class OpenApiGeneralDescriptionTranslator : Translator<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>
    {
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        public OpenApiGeneralDescriptionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManage) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = cacheManage.TypesCache;
            this.languageCache = cacheManage.LanguageCache;
        }

        public override VmOpenApiGeneralDescriptionVersionBase TranslateEntityToVm(StatutoryServiceGeneralDescriptionVersioned entity)
        {
            if (entity == null)
            {
                return null;
            }

            // Property value changes (PTV-2184)
            var type = entity.TypeId.IsAssigned() ? typesCache.GetByValue<ServiceType>(entity.TypeId) : null;
            var chargeType = entity.ChargeTypeId.IsAssigned() ? typesCache.GetByValue<ServiceChargeType>(entity.ChargeTypeId.Value) : null;

            var definition = CreateEntityViewModelDefinition(entity)
                // We have to use unique root id as id!
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddSimple(i => i.Id, o => o.CurrentVersionId)
                .AddCollection(i => i.Names, o => o.Names)
                .AddCollection(i => i.Descriptions, o => o.Descriptions)
                .AddNavigation(i => string.IsNullOrEmpty(chargeType) ? null : chargeType.GetOpenApiEnumValue<ServiceChargeTypeEnum>(), o => o.ServiceChargeType)
                .AddCollection(i => i.Languages.Select(j => j.Language.Code), o => o.Languages)
                .AddCollection(i => i.StatutoryServiceRequirements, o => o.Requirements)
                .AddNavigation(i => string.IsNullOrEmpty(type) ? null : type.GetOpenApiEnumValue<ServiceTypeEnum>(), o => o.Type)
                .AddCollection(i => i.ServiceClasses.Select(j => j.ServiceClass).ToList(), o => o.ServiceClasses)
                .AddCollection(i => i.OntologyTerms.Select(j => j.OntologyTerm).ToList(), o => o.OntologyTerms)
                .AddCollection(i => i.TargetGroups.Select(j => j.TargetGroup).ToList(), o => o.TargetGroups)
                .AddCollection(i => i.LifeEvents.Select(j => j.LifeEvent).ToList(), o => o.LifeEvents)
                .AddCollection(i => i.IndustrialClasses.Select(j => j.IndustrialClass).ToList(), o => o.IndustrialClasses)
                .AddSimple(i => i.Modified, o => o.Modified)
                .AddCollection(i => i.StatutoryServiceLaws.Select(j => j.Law), o => o.Legislation)
                .AddNavigation(i => typesCache.GetByValue<PublishingStatusType>(i.PublishingStatusId), o => o.PublishingStatus)
                .AddCollection(i => i.LanguageAvailabilities.Select(l => languageCache.GetByValue(l.LanguageId)).ToList(), o => o.AvailableLanguages)
                .AddCollection(i => i.UnificRoot?.StatutoryServiceGeneralDescriptionServiceChannels, o => o.ServiceChannels);
            
            return definition.GetFinal();
        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(VmOpenApiGeneralDescriptionVersionBase vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiGeneralDescriptionTranslator!");
        }
    }
}
