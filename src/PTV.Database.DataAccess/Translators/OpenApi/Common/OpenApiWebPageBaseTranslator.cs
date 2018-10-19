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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    internal abstract class OpenApiWebPageBaseTranslator<TModel> : Translator<WebPage, TModel> where TModel : class, IVmOpenApiWebPageVersionBase
    {
        private ILanguageCache languageCache;

        public OpenApiWebPageBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
        }
        public override TModel TranslateEntityToVm(WebPage entity)
        {
            return CreateBaseEntityVmDefinitions(entity).GetFinal();
        }
        public override WebPage TranslateVmToEntity(TModel vModel)
        {
            return CreateBaseVmEntityDefinitions(vModel).GetFinal();
        }

        protected ITranslationDefinitions<WebPage, TModel> CreateBaseEntityVmDefinitions(WebPage entity)
        {
            return CreateEntityViewModelDefinition<TModel>(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.Name, o => o.Value)
                .AddNavigation(i => i.Url, o => o.Url)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language);
        }
        protected ITranslationDefinitions<TModel, WebPage> CreateBaseVmEntityDefinitions(TModel vModel)
        {
            var definition = CreateViewModelEntityDefinition<WebPage>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.Id, e => e.UseDataContextCreate(x => true));

            if (!string.IsNullOrEmpty(vModel.Value))
            {
                definition.AddNavigation(i => i.Value, o => o.Name);
            }
            if (!string.IsNullOrEmpty(vModel.Url))
            {
                definition.AddNavigation(i => i.Url, o => o.Url);
            }
            if (!string.IsNullOrEmpty(vModel.Language))
            {
                definition.AddNavigation(i => i.Language, o => o.Localization);
            }

            return definition;
        }
    }
}
