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
using System;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<AccessibilityClassification, VmOpenApiAccessibilityClassification>), RegisterType.Transient)]
    internal class OpenApiAccessibilityClassificationTranslator : Translator<AccessibilityClassification, VmOpenApiAccessibilityClassification>
    {
        private readonly ITypesCache typesCache;

        public OpenApiAccessibilityClassificationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
             : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiAccessibilityClassification TranslateEntityToVm(AccessibilityClassification entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.AccessibilityClassificationLevelTypeId.IsAssigned() ? typesCache.GetByValue<AccessibilityClassificationLevelType>(i.AccessibilityClassificationLevelTypeId) : null,
                    o => o.AccessibilityClassificationLevel)
                .AddNavigation(i => i.WcagLevelTypeId.IsAssigned() ? typesCache.GetByValue<WcagLevelType>(i.WcagLevelTypeId.Value) : null,
                    o => o.WcagLevel)
                .AddNavigation(i => i.Name, o => o.AccessibilityStatementWebPageName)
                .AddNavigation(i => i.Url, o => o.AccessibilityStatementWebPage)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .GetFinal();
        }

        public override AccessibilityClassification TranslateVmToEntity(VmOpenApiAccessibilityClassification vModel)
        {
            bool exists = vModel.Id.IsAssigned();

            var definitions = CreateViewModelEntityDefinition<AccessibilityClassification>(vModel)
               .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
               .UseDataContextUpdate(i => exists, i => o => i.Id == o.Id, e => e.UseDataContextCreate(x => true))
               .AddSimple(i => typesCache.Get<AccessibilityClassificationLevelType>(i.AccessibilityClassificationLevel), o => o.AccessibilityClassificationLevelTypeId)
               .AddSimple(i => i.WcagLevel.IsNullOrEmpty() ? (Guid?)null : typesCache.Get<WcagLevelType>(i.WcagLevel), o => o.WcagLevelTypeId)
               .AddNavigation(i => i.AccessibilityStatementWebPage, o => o.Url)
               .AddNavigation(i => i.AccessibilityStatementWebPageName, o => o.Name)
               .AddSimple(i => languageCache.Get(i.Language), o => o.LocalizationId);

            return definitions.GetFinal();
        }
    }
}
