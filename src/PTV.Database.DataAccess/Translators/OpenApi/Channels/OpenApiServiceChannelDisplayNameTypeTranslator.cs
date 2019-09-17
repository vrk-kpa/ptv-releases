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
using System;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelDisplayNameType, VmOpenApiNameTypeByLanguage>), RegisterType.Transient)]
    internal class OpenApiServiceChannelDisplayNameTypeTranslator : Translator<ServiceChannelDisplayNameType, VmOpenApiNameTypeByLanguage>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public OpenApiServiceChannelDisplayNameTypeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiNameTypeByLanguage TranslateEntityToVm(ServiceChannelDisplayNameType entity)
        {
            return CreateEntityViewModelDefinition<VmOpenApiNameTypeByLanguage>(entity)
                // AlternateName changed into AlternativeName (PTV-2184)
                .AddNavigation(i => typesCache.GetByValue<NameType>(i.DisplayNameTypeId).GetOpenApiEnumValue<NameTypeEnum>(), o => o.Type)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .GetFinal();
        }

        public override ServiceChannelDisplayNameType TranslateVmToEntity(VmOpenApiNameTypeByLanguage vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(input => input.OwnerReferenceId.HasValue,
                    input => output => output.LocalizationId == languageCache.Get(input.Language) && input.OwnerReferenceId == output.ServiceChannelVersionedId,
                    name => name.UseDataContextCreate(x => true))
                .AddSimple(i => typesCache.Get<NameType>(i.Type), o => o.DisplayNameTypeId)
                .AddNavigation(i => i.Language, o => o.Localization)
                .GetFinal();
        }
    }
}
