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
using System.Collections.Generic;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<WebpageChannel, VmWebPageChannelStep1>), RegisterType.Transient)]
    internal class WebPageChannelMainStep1Translator : Translator<WebpageChannel, VmWebPageChannelStep1>
    {
        private ILanguageCache languageCache;

        public WebPageChannelMainStep1Translator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageCache languageCache) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = languageCache;
        }

        public override VmWebPageChannelStep1 TranslateEntityToVm(WebpageChannel entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddLocalizable(i => i.LocalizedUrls, o => o.WebPage)
                //.AddNavigation(i => languageCache.Filter(i.LocalizedUrls, LanguageCode.fi), o => o.Url)
                .GetFinal();
        }

        public override WebpageChannel TranslateVmToEntity(VmWebPageChannelStep1 vModel)
        {
            if (vModel.WebPage != null)
            {
                vModel.WebPage.OwnerReferenceId = vModel.WebPageChannelId;
            }

            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextLocalizedUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.ServiceChannelId)
                .AddLocalizable(i => i.WebPage, o => o.LocalizedUrls)
                //.AddCollection(i => new List<VmChannelAttachment>() { i.Url}, o => o.LocalizedUrls)
                .GetFinal();
        }
    }
}