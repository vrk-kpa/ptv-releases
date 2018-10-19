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
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.V6;

namespace PTV.Database.DataAccess.Translators.OpenApi.ServiceCollections
{
    [RegisterService(typeof(ITranslator<ServiceCollectionVersioned, VmOpenApiServiceCollectionVersionBase>), RegisterType.Transient)]
    internal class OpenApiServiceCollectionTranslator : Translator<ServiceCollectionVersioned, VmOpenApiServiceCollectionVersionBase>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        public OpenApiServiceCollectionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmOpenApiServiceCollectionVersionBase TranslateEntityToVm(ServiceCollectionVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition<VmOpenApiServiceCollectionVersionBase>(entity)
                // We have to use unique root id as id!
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddCollection(i => i.ServiceCollectionNames, o => o.ServiceCollectionNames)
                .AddCollection(i => i.ServiceCollectionDescriptions, o => o.ServiceCollectionDescriptions)
                .AddCollection(i => i.UnificRoot.ServiceCollectionServices, o => o.Services)
                .AddNavigation(i => typesCache.GetByValue<PublishingStatusType>(i.PublishingStatusId), o => o.PublishingStatus)
                .AddCollection(i => i.LanguageAvailabilities.Select(l => languageCache.GetByValue(l.LanguageId)).ToList(), o => o.AvailableLanguages)
                .AddSimple(i => i.OrganizationId, o => o.MainResponsibleOrganization)
                .AddSimple(i => i.Modified, o => o.Modified);

            return definition.GetFinal();
        }

        public override ServiceCollectionVersioned TranslateVmToEntity(VmOpenApiServiceCollectionVersionBase vModel)
        {
            throw new NotImplementedException();
        }
    }
}
