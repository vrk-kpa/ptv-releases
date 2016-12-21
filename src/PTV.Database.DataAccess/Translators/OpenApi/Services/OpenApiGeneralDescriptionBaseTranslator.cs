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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    internal abstract class OpenApiGeneralDescriptionBaseTranslator<TVmOpenApiGeneralDescription> : Translator<StatutoryServiceGeneralDescription, TVmOpenApiGeneralDescription> where TVmOpenApiGeneralDescription : class, IVmOpenApiGeneralDescriptionBase
    {
        private ITypesCache typesCache;

        protected OpenApiGeneralDescriptionBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManage) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = cacheManage.TypesCache;
        }

        public override TVmOpenApiGeneralDescription TranslateEntityToVm(StatutoryServiceGeneralDescription entity)
        {
            return CreateBaseEntityVmDefinitions(entity).GetFinal();
        }

        public override StatutoryServiceGeneralDescription TranslateVmToEntity(TVmOpenApiGeneralDescription vModel)
        {
            return CreateBaseVmEntityDefinitions(vModel).GetFinal();
        }

        protected ITranslationDefinitions<StatutoryServiceGeneralDescription, TVmOpenApiGeneralDescription> CreateBaseEntityVmDefinitions(StatutoryServiceGeneralDescription entity)
        {
            var definition = CreateEntityViewModelDefinition<TVmOpenApiGeneralDescription>(entity)
                .AddCollection(i => i.Names, o => o.Names)
                .AddCollection(i => i.Descriptions, o => o.Descriptions)
                .AddCollection(i => i.Languages.Select(j => j.Language.Code), o => o.Languages)
                .AddCollection(i => i.StatutoryServiceRequirements, o => o.Requirements)
                .AddNavigation(i => typesCache.GetByValue<ServiceType>(i.TypeId), o => o.Type)
                .AddCollection(i => i.StatutoryServiceLaws, o => o.Laws);

            if (entity.ChargeTypeId.IsAssigned())
            {
                definition.AddNavigation(i => typesCache.GetByValue<ServiceChargeType>(i.ChargeTypeId.Value), o => o.ServiceChargeType);
            }

            return definition;
        }

        protected ITranslationDefinitions<TVmOpenApiGeneralDescription, StatutoryServiceGeneralDescription> CreateBaseVmEntityDefinitions(TVmOpenApiGeneralDescription vModel)
        {
            if (vModel.Id.IsAssigned())
            {
                // We are updating existing service
                var id = vModel.Id.Value;
                vModel.Names.ForEach(n => n.OwnerReferenceId = id);
                vModel.Descriptions.ForEach(d => d.OwnerReferenceId = id);
            }
            var definitions = CreateViewModelEntityDefinition<StatutoryServiceGeneralDescription>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.Id.HasValue, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.HasValue, i => o => i.Id.Value == o.Id);

            if (vModel.Names != null && vModel.Names.Count > 0)
            {
                definitions.AddCollection(i => i.Names, o => o.Names);
            }

            if (vModel.Descriptions != null && vModel.Descriptions.Count > 0)
            {
                definitions.AddCollection(i => i.Descriptions, o => o.Descriptions);
            }

            if (vModel.Languages != null && vModel.Languages.Count > 0)
            {
                definitions.AddCollection(i => i.Languages, o => o.Languages);
            }

            if (vModel.Requirements?.Count > 0)
            {
                definitions.AddCollection(i => i.Requirements, o => o.StatutoryServiceRequirements);
            }

            if (!string.IsNullOrEmpty(vModel.Type))
            {
                definitions.AddSimple(i => typesCache.Get<ServiceType>(i.Type), o => o.TypeId);
            }

            if (!string.IsNullOrEmpty(vModel.ServiceChargeType))
            {
                definitions.AddSimple(i => typesCache.Get<ServiceChargeType>(i.ServiceChargeType), o => o.ChargeTypeId);
            }

            if (vModel.Laws?.Count > 0)
            {
                definitions.AddCollection(i => i.Laws, o => o.StatutoryServiceLaws);
            }

            return definitions;
        }
    }
}