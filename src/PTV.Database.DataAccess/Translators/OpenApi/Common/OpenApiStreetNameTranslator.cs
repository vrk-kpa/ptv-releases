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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Models.Interfaces.OpenApi;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<ClsAddressStreetName, VmOpenApiLanguageItem>), RegisterType.Transient)]
    [RegisterService(typeof(ITranslator<ClsAddressStreetName, IVmOpenApiLanguageItem>), RegisterType.Transient)]
    internal class OpenApiStreetNameTranslator : OpenApiNameBaseTranslator<ClsAddressStreetName>
    {
        private readonly ILanguageCache languageCache;
        public OpenApiStreetNameTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) 
            : base(resolveManager, translationPrimitives, cacheManager)
        {
            this.languageCache = cacheManager.LanguageCache;
        }

        public override VmOpenApiLanguageItem TranslateEntityToVm(ClsAddressStreetName entity)
        {
            return CreateEntityViewModelDefinition<VmOpenApiLanguageItem>(entity)
                .AddNavigation(i => i.Name, o => o.Value)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .AddSimple(i => i.ClsAddressStreetId, o => o.OwnerReferenceId)
                .GetFinal();
        }

        // This method should be called only when creating INVALID non-CLS streets from
        // incorrect user input via Open API
        public override ClsAddressStreetName TranslateVmToEntity(VmOpenApiLanguageItem vModel)
        {
            return CreateViewModelEntityDefinition<ClsAddressStreetName>(vModel)
                .AddSimple(i => languageCache.Get(i.Language), o => o.LocalizationId)
                .AddNavigation(i => i.Value?.Trim(), o => o.Name)
                .AddNavigation(i => i.Value?.Trim()?.SafeSubstring(0, 3)?.ToLower(), o => o.Name3)
                .AddSimple(i => true, o => o.NonCls)
                .GetFinal();
        }
    }
}
