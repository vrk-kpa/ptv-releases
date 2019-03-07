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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Translators.OpenApi.Common;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<StatutoryServiceName, VmOpenApiLocalizedListItem>), RegisterType.Transient)]
    internal class OpenApiStatutoryServiceNameTranslator : OpenApiNameWithTypeBaseTranslator<StatutoryServiceName>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public OpenApiStatutoryServiceNameTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiLocalizedListItem TranslateEntityToVm(StatutoryServiceName entity)
        {
            return base.TranslateEntityToVm(entity);
        }

        public override StatutoryServiceName TranslateVmToEntity(VmOpenApiLocalizedListItem vModel)
        {
            return CreateViewModelEntityDefinition<StatutoryServiceName>(vModel)
                .UseDataContextUpdate(i => i.OwnerReferenceId.HasValue, i => o => o.Type.Code == i.Type && o.LocalizationId == languageCache.Get(i.Language) &&
                    i.OwnerReferenceId == o.StatutoryServiceGeneralDescriptionVersionedId, name => name.UseDataContextCreate(x => true))
                .AddNavigation(i => i.Value, o => o.Name)
                .AddNavigation(i => i.Language, o => o.Localization)
                .AddNavigation(i => i.Type, o => o.Type)
                .AddSimple(i => typesCache.Get<NameType>(i.Type), o => o.TypeId)
                .GetFinal();
        }
    }
}
