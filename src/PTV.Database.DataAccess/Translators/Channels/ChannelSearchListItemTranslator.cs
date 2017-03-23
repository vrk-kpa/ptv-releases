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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmChannelSearchListItem>), RegisterType.Transient)]
    internal class ChannelSearchListItemTranslator : Translator<ServiceChannelVersioned, VmChannelSearchListItem>
    {
        private ILanguageOrderCache languageOrderCache;
        public ChannelSearchListItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageOrderCache languageOrderCache)
            : base(resolveManager, translationPrimitives)
        {
            this.languageOrderCache = languageOrderCache;
        }

        public override VmChannelSearchListItem TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var languageId = entity.ServiceChannelNames
                .Select(x => x.LocalizationId)
                .OrderBy(x => languageOrderCache.Get(x))
                .First();
            SetLanguage(languageId);
            var channelSearch = CreateEntityViewModelDefinition<VmChannelSearchListItem>(entity)
                .AddPartial(i => i, o => o as VmChannelListItem)
                .GetFinal();
            return channelSearch;
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmChannelSearchListItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
