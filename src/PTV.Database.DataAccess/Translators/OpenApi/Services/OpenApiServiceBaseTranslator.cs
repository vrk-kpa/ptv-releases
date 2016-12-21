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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Database.DataAccess.Caches;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    internal abstract class OpenApiServiceBaseTranslator<TVmOpenApiService> : Translator<Service, TVmOpenApiService> where TVmOpenApiService : class, IVmOpenApiServiceBase
    {
        private readonly ITypesCache typesCache;

        protected OpenApiServiceBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override TVmOpenApiService TranslateEntityToVm(Service entity)
        {
            return CreateBaseEntityVmDefinitions(entity).GetFinal();
        }
        public override Service TranslateVmToEntity(TVmOpenApiService vModel)
        {
            return CreateBaseVmEntityDefinitions(vModel).GetFinal();
        }

        protected ITranslationDefinitions<Service, TVmOpenApiService> CreateBaseEntityVmDefinitions(Service entity)
        {
            const string ServiceAdditionalInformations = "AdditionalInfo";

            return CreateEntityViewModelDefinition<TVmOpenApiService>(entity)
                .AddCollection(i => i.ServiceNames, o => o.ServiceNames)
                .AddCollection(i => i.ServiceDescriptions.Where(d => !typesCache.GetByValue<DescriptionType>(d.TypeId).EndsWith(ServiceAdditionalInformations)), o => o.ServiceDescriptions)
                .AddCollection(i => i.ServiceLanguages.Select(j => j.Language.Code).ToList(), o => o.Languages)
                .AddCollection(i => i.ServiceKeywords, o => o.Keywords)
                .AddNavigation(i => i.ServiceCoverageTypeId.HasValue ? typesCache.GetByValue<ServiceCoverageType>(i.ServiceCoverageTypeId.Value) : null, o => o.ServiceCoverageType)
                .AddCollection(i => i.ServiceMunicipalities.Select(m => m.Municipality.Name).ToList(), o => o.Municipalities)
                .AddCollection(i => i.ServiceRequirements, o => o.Requirements)
                .AddCollection(i => i.ServiceDescriptions.Where(d => typesCache.GetByValue<DescriptionType>(d.TypeId).EndsWith(ServiceAdditionalInformations)), o => o.ServiceAdditionalInformations)
                .AddNavigation(i => i.PublishingStatus, o => o.PublishingStatus);
        }

        protected ITranslationDefinitions<TVmOpenApiService, Service> CreateBaseVmEntityDefinitions(TVmOpenApiService vModel)
        {
            if (vModel.Id.IsAssigned())
            {
                // We are updating existing service
                var id = vModel.Id.Value;
                vModel.ServiceNames.ForEach(n => n.OwnerReferenceId = id);
                vModel.ServiceDescriptions.ForEach(d => d.OwnerReferenceId = id);
                vModel.Requirements.ForEach(r => r.OwnerReferenceId = id);
                vModel.ServiceAdditionalInformations.ForEach(a => a.OwnerReferenceId = id);
            }

            var definitions = CreateViewModelEntityDefinition<Service>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.Id.HasValue, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.HasValue, i => o => i.Id.Value == o.Id);

            if (vModel.ServiceNames != null && vModel.ServiceNames.Count > 0)
            {
                definitions.AddCollection(i => i.ServiceNames, o => o.ServiceNames);
            }

            if (vModel.ServiceDescriptions != null && vModel.ServiceDescriptions.Count > 0)
            {
                definitions.AddCollection(i => (vModel.ServiceAdditionalInformations != null && vModel.ServiceAdditionalInformations.Count > 0)
                    ? i.ServiceDescriptions.Concat(vModel.ServiceAdditionalInformations)
                    : i.ServiceDescriptions, o => o.ServiceDescriptions);
            }

            if (vModel.Languages != null && vModel.Languages.Count > 0)
            {
                definitions.AddCollection(i => i.Languages, o => o.ServiceLanguages);
            }

            if (!string.IsNullOrEmpty(vModel.ServiceCoverageType))
            {
                definitions.AddNavigation(i => i.ServiceCoverageType, o => o.ServiceCoverageType);
            }

            if (vModel.Municipalities != null && vModel.Municipalities.Count > 0)
            {
                definitions.AddCollection(i => i.Municipalities, o => o.ServiceMunicipalities);
            }

            if (vModel.Requirements != null && vModel.Requirements.Count > 0)
            {
                definitions.AddCollection(i => i.Requirements, o => o.ServiceRequirements);
            }

            if (!string.IsNullOrEmpty(vModel.PublishingStatus))
            {
                definitions.AddNavigation(i => i.PublishingStatus, o => o.PublishingStatus);
            }

            return definitions;
        }
    }
}
