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
using Microsoft.EntityFrameworkCore;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.OpenApi.V7;

namespace PTV.Database.DataAccess.Translators.OpenApi.ServiceCollections
{
    [RegisterService(typeof(ITranslator<ServiceCollectionVersioned, IVmOpenApiServiceCollectionInVersionBase>), RegisterType.Transient)]
    internal class OpenApiServiceCollectionInTranslator : Translator<ServiceCollectionVersioned, IVmOpenApiServiceCollectionInVersionBase>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceCollectionInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override IVmOpenApiServiceCollectionInVersionBase TranslateEntityToVm(ServiceCollectionVersioned entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceCollectionInTranslator!");
        }
        public override ServiceCollectionVersioned TranslateVmToEntity(IVmOpenApiServiceCollectionInVersionBase vModel)
        {
            var exists = vModel.VersionId.IsAssigned();
            if (exists)
            {
                var entityId = vModel.VersionId.Value;
                vModel.ServiceCollectionNames.ForEach(n => n.OwnerReferenceId = entityId);
                vModel.ServiceCollectionDescriptions.ForEach(d => d.OwnerReferenceId = entityId);
            }

            var definitions = CreateViewModelEntityDefinition<ServiceCollectionVersioned>(vModel)
                .DisableAutoTranslation()
                .DefineEntitySubTree(i => i.Include(j => j.UnificRoot).ThenInclude(j=>j.ServiceCollectionServices))
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => i.VersionId.Value == o.Id)
                .UseVersioning<ServiceCollectionVersioned, ServiceCollection>(o => o);

            // Set available languages
            var languages = GetAvailableLanguages(vModel);
            if (languages.Count > 0)
            {
                definitions.AddCollection(i => languages, o => o.LanguageAvailabilities, true);
            }

            // Main responsible organization
            if (!string.IsNullOrEmpty(vModel.MainResponsibleOrganization))
            {
                definitions = definitions.AddSimple(i => vModel.MainResponsibleOrganization.ParseToGuidWithExeption(), o => o.OrganizationId);
            }

            // Service collection names
            if (vModel.ServiceCollectionNames?.Count > 0)
            {
                definitions.AddCollection(i => i.ServiceCollectionNames, o => o.ServiceCollectionNames, false);
            }

            // Descriptions
            if (vModel.ServiceCollectionDescriptions != null && vModel.ServiceCollectionDescriptions.Count > 0)
            {
                definitions.AddCollection(i => i.ServiceCollectionDescriptions, o => o.ServiceCollectionDescriptions, false);
            }



            // Main responsible organization
            if (!string.IsNullOrEmpty(vModel.MainResponsibleOrganization))
            {
                definitions = definitions.AddSimple(i => vModel.MainResponsibleOrganization.ParseToGuidWithExeption(), o => o.OrganizationId);
            }

            return definitions.GetFinal();
        }

        private List<VmOpenApiLanguageAvailability> GetAvailableLanguages(IVmOpenApiServiceCollectionInVersionBase vModel)
        {
            var languages = new List<VmOpenApiLanguageAvailability>();
            
            var currentPublishingStatus = string.IsNullOrEmpty(vModel.CurrentPublishingStatus)
                ? PublishingStatus.Draft.ToString()
                : vModel.CurrentPublishingStatus == PublishingStatus.Published.ToString() && vModel.PublishingStatus == PublishingStatus.Modified.ToString() // SFIPTV-234
                    ? PublishingStatus.Modified.ToString()
                    : vModel.CurrentPublishingStatus;
            
            vModel.AvailableLanguages.ForEach(lang =>
            {
                languages.Add(new VmOpenApiLanguageAvailability() { Language = lang, OwnerReferenceId = vModel.VersionId, PublishingStatus = currentPublishingStatus });
            });
            
            return languages;
        }
    }
}
