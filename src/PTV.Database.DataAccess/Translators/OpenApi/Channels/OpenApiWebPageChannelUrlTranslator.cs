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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Translators.OpenApi.Common;
using PTV.Domain.Model.Models.OpenApi.V9;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<WebpageChannelUrl, V9VmOpenApiWebPage>), RegisterType.Transient)]
    internal class OpenApiWebPageChannelUrlTranslator : Translator<WebpageChannelUrl, V9VmOpenApiWebPage>
    {

        public OpenApiWebPageChannelUrlTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override V9VmOpenApiWebPage TranslateEntityToVm(WebpageChannelUrl entity)
        {
            var model = CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .AddSimple(i => 0, o => o.OrderNumber);
                
                model.AddNavigation(i => i.WebPage, o => o.Url);

            return model.GetFinal();
        }

        public override WebpageChannelUrl TranslateVmToEntity(V9VmOpenApiWebPage vModel)
        {
            throw new System.NotImplementedException("No translation implemented in OpenApiWebPageChannelUrlTranslator.");
        }
    }

    [RegisterService(typeof(ITranslator<WebpageChannelUrl, VmOpenApiLanguageItem>), RegisterType.Transient)]
    internal class OpenApiWebPageChannelUrlInTranslator : Translator<WebpageChannelUrl, VmOpenApiLanguageItem>
    {
        public OpenApiWebPageChannelUrlInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiLanguageItem TranslateEntityToVm(WebpageChannelUrl entity)
        {
            throw new System.NotImplementedException("No translation implemented in OpenApiWebPageChannelUrlInTranslator.");
        }

        public override WebpageChannelUrl TranslateVmToEntity(VmOpenApiLanguageItem vModel)
        {
            return CreateViewModelEntityDefinition<WebpageChannelUrl>(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => i.OwnerReferenceId.IsAssigned(), i => o => i.OwnerReferenceId == o.WebpageChannelId &&
                    languageCache.Get(i.Language) == o.LocalizationId, e => e.UseDataContextCreate(x => true))
                .AddNavigation(i => i.Value, o => o.WebPage)
                .AddSimple(i => languageCache.Get(i.Language), o => o.LocalizationId)
                .GetFinal();
        }
    }
}
