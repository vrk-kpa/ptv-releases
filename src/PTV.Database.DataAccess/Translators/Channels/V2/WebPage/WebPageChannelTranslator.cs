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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using VmWebPageChannel = PTV.Domain.Model.Models.V2.Channel.VmWebPageChannel;

namespace PTV.Database.DataAccess.Translators.Channels.V2.WebPage
{
    [RegisterService(typeof(ITranslator<WebpageChannel, VmWebPageChannel>), RegisterType.Transient)]
    internal class WebPageChannelTranslator : Translator<WebpageChannel, VmWebPageChannel>
    {
        private ILanguageCache languageCache;
        public WebPageChannelTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override VmWebPageChannel TranslateEntityToVm(WebpageChannel entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddDictionary(i => i.LocalizedUrls?.Cast<IText>(), o => o.WebPage, wp => languageCache.GetByValue(wp.LocalizationId))
                .GetFinal();
        }

        public override WebpageChannel TranslateVmToEntity(VmWebPageChannel vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.ServiceChannelVersionedId)
                ;
            var entity = definition.GetFinal();
            definition
                .AddCollection(
                    i => i.WebPage?.Select(pair => new VmWebPage
                    {
                        UrlAddress = pair.Value,
                        LocalizationId = languageCache.Get(pair.Key),
                        OwnerReferenceId = i.Id.IsAssigned() ? entity.Id : (Guid?)null
                    }), o => o.LocalizedUrls, true);

            return entity;
        }

    }
}
