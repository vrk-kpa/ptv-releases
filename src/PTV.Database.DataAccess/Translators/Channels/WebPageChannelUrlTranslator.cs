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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<WebpageChannelUrl, VmChannelAttachment>), RegisterType.Transient)]
    internal class WebPageChannelUrlTranslator : Translator<WebpageChannelUrl, VmChannelAttachment>
    {
        public WebPageChannelUrlTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmChannelAttachment TranslateEntityToVm(WebpageChannelUrl entity)
        {
            return CreateEntityViewModelDefinition<VmChannelAttachment>(entity)
                .AddNavigation(i => i.WebPage, o => o.UrlAddress)
                .AddSimple(i => i.WebpageChannelId, o => o.OwnerReferenceId)
                .GetFinal();
        }

        public override WebpageChannelUrl TranslateVmToEntity(VmChannelAttachment vModel)
        {
            var transaltionDefinition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.OwnerReferenceId.IsAssigned())
                .UseDataContextLocalizedUpdate(i => i.OwnerReferenceId.IsAssigned(), i => o => (i.OwnerReferenceId == o.WebpageChannelId), def => def.UseDataContextCreate(i => true))
                .AddNavigation(i => vModel.UrlAddress, o => o.WebPage)
                .AddRequestLanguage(output => output);

            var entity = transaltionDefinition.GetFinal();
            return entity;
        }

    }

    [RegisterService(typeof(ITranslator<WebpageChannelUrl, VmWebPage>), RegisterType.Transient)]
    internal class WebPageChannelWebPageUrlTranslator : Translator<WebpageChannelUrl, VmWebPage>
    {
        public WebPageChannelWebPageUrlTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmWebPage TranslateEntityToVm(WebpageChannelUrl entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.WebPage, o => o.UrlAddress)
                .AddSimple(i => i.WebpageChannelId, o => o.Id)
                .GetFinal();
        }

        public override WebpageChannelUrl TranslateVmToEntity(VmWebPage vModel)
        {
            if (vModel.LocalizationId.IsAssigned())
            {
                SetLanguage(vModel.LocalizationId.Value);
            }
            var transaltionDefinition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned())
                .UseDataContextLocalizedUpdate(i => i.Id.IsAssigned(),i => o => (i.Id == o.WebpageChannelId), def => def.UseDataContextCreate(i => true))
                .AddNavigation(i => vModel.UrlAddress, o => o.WebPage)
                // HACK: There is a rare case when a content contains multiple references
                // to the same web page and the user wants to change just one URL, that
                // the later loads of the old web page will overwrite the new one.
                // Thus we need to change the FK ID, which will not be overwritten and
                // will take precedence during saving to database.
                .Propagation((i, o) => o.WebPageId = o.WebPage.Id)
                .AddRequestLanguage(output => output);

            var entity = transaltionDefinition.GetFinal();
            return entity;
        }

    }
}
