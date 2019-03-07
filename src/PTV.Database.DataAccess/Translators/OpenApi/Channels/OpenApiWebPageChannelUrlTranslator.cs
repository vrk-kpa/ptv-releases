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
using PTV.Domain.Model.Models.OpenApi.V9;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<WebpageChannelUrl, V9VmOpenApiWebPage>), RegisterType.Transient)]
    internal class OpenApiWebPageChannelUrlTranslator : Translator<WebpageChannelUrl, V9VmOpenApiWebPage>
    {
        private ILanguageCache languageCache;

        public OpenApiWebPageChannelUrlTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
        }

        public override V9VmOpenApiWebPage TranslateEntityToVm(WebpageChannelUrl entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => string.IsNullOrEmpty(i.Url) ? null : i.Url, o => o.Url)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .AddSimple(i => 0, o => o.OrderNumber)
                .GetFinal();
        }

        public override WebpageChannelUrl TranslateVmToEntity(V9VmOpenApiWebPage vModel)
        {
            throw new System.NotImplementedException("No translation implemented in OpenApiWebPageChannelUrlTranslator.");
        }
    }

    [RegisterService(typeof(ITranslator<WebpageChannelUrl, VmOpenApiLanguageItem>), RegisterType.Transient)]
    internal class OpenApiWebPageChannelUrlInTranslator : OpenApiTextBaseTranslator<WebpageChannelUrl>
    {
        private ILanguageCache languageCache;

        public OpenApiWebPageChannelUrlInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives, cacheManager)
        {
            this.languageCache = cacheManager.LanguageCache;
        }

        public override VmOpenApiLanguageItem TranslateEntityToVm(WebpageChannelUrl entity)
        {
            return base.TranslateEntityToVm(entity);
        }

        public override WebpageChannelUrl TranslateVmToEntity(VmOpenApiLanguageItem vModel)
        {
            return CreateViewModelEntityDefinition<WebpageChannelUrl>(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => i.OwnerReferenceId.IsAssigned(), i => o => i.OwnerReferenceId == o.WebpageChannelId &&
                    languageCache.Get(i.Language) == o.LocalizationId, e => e.UseDataContextCreate(x => true))
                .AddNavigation(i => i.Value, o => o.Url)
                .AddNavigation(i => i.Language, o => o.Localization)
                .GetFinal();
        }
    }
}
