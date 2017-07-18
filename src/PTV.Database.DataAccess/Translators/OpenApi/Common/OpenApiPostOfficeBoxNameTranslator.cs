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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;


namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<PostOfficeBoxName, VmOpenApiLanguageItem>), RegisterType.Transient)]
    internal class OpenApiPostOfficeBoxNameTranslator : Translator<PostOfficeBoxName, VmOpenApiLanguageItem>
    {
        ILanguageCache languageCache;

        public OpenApiPostOfficeBoxNameTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override VmOpenApiLanguageItem TranslateEntityToVm(PostOfficeBoxName entity)
        {
            return CreateEntityViewModelDefinition<VmOpenApiLanguageItem>(entity)
                .AddNavigation(i => i.Text, o => o.Value)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .GetFinal();
        }
        public override PostOfficeBoxName TranslateVmToEntity(VmOpenApiLanguageItem vModel)
        {
            var exists = vModel.Id.IsAssigned();

            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => i.Id == o.Id, e => e.UseDataContextCreate(x => true))
                .AddNavigation(i => i.Value, o => o.Text)
                .AddSimple(i => languageCache.Get(i.Language), o => o.LocalizationId)
                .GetFinal();
        }
    }
}
