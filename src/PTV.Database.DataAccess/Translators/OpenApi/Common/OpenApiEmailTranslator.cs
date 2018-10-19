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
**/

using System;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<Email, V4VmOpenApiEmail>), RegisterType.Transient)]
    internal class OpenApiEmailTranslator : Translator<Email, V4VmOpenApiEmail>
    {
        private readonly ILanguageCache languageCache;

        public OpenApiEmailTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override V4VmOpenApiEmail TranslateEntityToVm(Email entity)
        {
            return CreateEntityViewModelDefinition(entity)
                    .AddNavigation(i => i.Description, o => o.Value)
                    .AddNavigation(i => i.Value, o => o.Value)
                    .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                    .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
                    .GetFinal();
        }

        public override Email TranslateVmToEntity(V4VmOpenApiEmail vModel)
        {
            var exists = vModel.Id.IsAssigned();

            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => i.Id == o.Id, e => e.UseDataContextCreate(x => true))
                .AddNavigation(i => i.Description, o => o.Value)
                .AddNavigation(i => i.Value, o => o.Value)
                .AddSimple(i => languageCache.Get(i.Language), o => o.LocalizationId)
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
                .GetFinal();
        }
    }
}
