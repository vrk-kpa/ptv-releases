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
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Database.DataAccess.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    internal abstract class OpenApiServiceBaseTranslator<TVmOpenApiService> : Translator<ServiceVersioned, TVmOpenApiService> where TVmOpenApiService : class, IVmOpenApiServiceBase
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        protected OpenApiServiceBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override TVmOpenApiService TranslateEntityToVm(ServiceVersioned entity)
        {
            return CreateBaseEntityVmDefinitions(entity).GetFinal();
        }
        public override ServiceVersioned TranslateVmToEntity(TVmOpenApiService vModel)
        {
            return CreateBaseVmEntityDefinitions(vModel).GetFinal();
        }

        protected ITranslationDefinitions<ServiceVersioned, TVmOpenApiService> CreateBaseEntityVmDefinitions(ServiceVersioned entity)
        {
            return CreateEntityViewModelDefinition<TVmOpenApiService>(entity)
                .AddCollection(i => i.ServiceNames, o => o.ServiceNames)
                .AddCollection(i => i.ServiceDescriptions, o => o.ServiceDescriptions)
                .AddCollection(i => i.ServiceLanguages.Select(j => j.Language.Code).ToList(), o => o.Languages)
                .AddCollection(i => i.ServiceKeywords, o => o.Keywords)
                .AddNavigation(i => i.ServiceCoverageTypeId.HasValue ? typesCache.GetByValue<ServiceCoverageType>(i.ServiceCoverageTypeId.Value) : null, o => o.ServiceCoverageType)
                .AddCollection(i => i.ServiceRequirements, o => o.Requirements)
                .AddNavigation(i => i.PublishingStatus, o => o.PublishingStatus)
                .AddCollection(i => i.LanguageAvailabilities.Select(l => languageCache.GetByValue(l.LanguageId)).ToList(), o => o.AvailableLanguages);
        }

        protected ITranslationDefinitions<TVmOpenApiService, ServiceVersioned> CreateBaseVmEntityDefinitions(TVmOpenApiService vModel)
        {
            var exists = vModel.Id.IsAssigned();

            var definitions = CreateViewModelEntityDefinition<ServiceVersioned>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => i.Id.Value == o.Id)
                .UseVersioning<ServiceVersioned, Service>(o => o);

            if (exists)
            {
                var entityId = vModel.Id.Value;

                vModel.ServiceNames.ForEach(n => n.OwnerReferenceId = entityId);
                vModel.ServiceDescriptions.ForEach(d => d.OwnerReferenceId = entityId);
                vModel.Requirements.ForEach(r => r.OwnerReferenceId = entityId);
            }

            if (vModel.ServiceNames != null && vModel.ServiceNames.Count > 0)
            {
                definitions.AddCollection(i => i.ServiceNames, o => o.ServiceNames, true);
            }

            if (vModel.ServiceDescriptions != null && vModel.ServiceDescriptions.Count > 0)
            {
                definitions.AddCollection(i => i.ServiceDescriptions, o => o.ServiceDescriptions, true);
            }

            if (vModel.Languages != null && vModel.Languages.Count > 0)
            {
                var languages = new List<VmOpenApiStringItem>();
                vModel.Languages.ForEach(l => languages.Add(new VmOpenApiStringItem { Value = l, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => languages, o => o.ServiceLanguages, false);
            }

            if (!string.IsNullOrEmpty(vModel.ServiceCoverageType))
            {
                definitions.AddNavigation(i => i.ServiceCoverageType, o => o.ServiceCoverageType);
            }

            if (vModel.Requirements != null && vModel.Requirements.Count > 0)
            {
                definitions.AddCollection(i => i.Requirements, o => o.ServiceRequirements, false);
            }

            return definitions;
        }
    }
}
