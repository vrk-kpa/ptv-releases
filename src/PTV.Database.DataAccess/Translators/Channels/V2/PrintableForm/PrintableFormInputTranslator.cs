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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.V2.Channel.PrintableForm;

namespace PTV.Database.DataAccess.Translators.Channels.V2.PrintableForm
{
    [RegisterService(typeof(ITranslator<PrintableFormChannel, VmPrintableFormInput>), RegisterType.Transient)]
    internal class PrintableFormInputTranslator : Translator<PrintableFormChannel, VmPrintableFormInput>
    {
        private ILanguageCache languageCache;
        public PrintableFormInputTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
        }

        public override VmPrintableFormInput TranslateEntityToVm(PrintableFormChannel entity)
        {
            throw new NotImplementedException();
        }

        public override PrintableFormChannel TranslateVmToEntity(VmPrintableFormInput vModel)
        {
            vModel.DeliveryAddress.SafeCall(i =>
            {
                i.OwnerReferenceId = vModel.Id;
            });
            var transaltionDefinition = CreateViewModelEntityDefinition<PrintableFormChannel>(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextLocalizedUpdate(i => i.Id.IsAssigned(), i => o => o.ServiceChannelVersionedId == i.Id)
                .Propagation((i, o) => i.PrintableFormChannelId = o.Id)
                .AddCollection(i => i.FormIdentifier?.Select(
                    pair => new VmPrintableFormChannelIdentifier() {
                        PrintableFormChannelId = i.PrintableFormChannelId,
                        FormIdentifier = pair.Value,
                        LocalizationId = languageCache.Get(pair.Key)
                    }),
                    o => o.FormIdentifiers, true)
                .AddCollection(i => i.FormReceiver?.Select(
                    pair => new VmPrintableFormChannelReceiver() {
                        PrintableFormChannelId = i.PrintableFormChannelId,
                        FormReceiver = pair.Value,
                        LocalizationId = languageCache.Get(pair.Key)
                    }),
                    o => o.FormReceivers, true)
                .AddCollectionWithRemove(i => i.FormFiles?.SelectMany(pair =>
                {
                    var orderNumber = 0;
                    var localizationId = languageCache.Get(pair.Key);
                    return pair.Value.Select(ff =>
                    {
                        ff.OrderNumber = orderNumber++;
                        ff.OwnerReferenceId = i.PrintableFormChannelId;
                        ff.LocalizationId = localizationId;
                        return ff;
                    });
                }), o => o.ChannelUrls, x => true)
                .AddNavigation(i => i.DeliveryAddress, o => o.DeliveryAddress);

            var entity = transaltionDefinition.GetFinal();
            return entity;
        }
    }

    [RegisterService(typeof(ITranslator<PrintableFormChannel, VmPrintableFormOutput>), RegisterType.Transient)]
    internal class PrintableFormChannelOutputTranslator : Translator<PrintableFormChannel, VmPrintableFormOutput>
    {
        private ILanguageCache languageCache;
        public PrintableFormChannelOutputTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
        }

        public override VmPrintableFormOutput TranslateEntityToVm(PrintableFormChannel entity)
        {
            var result = CreateEntityViewModelDefinition(entity)
                .AddDictionary(
                    i => i.FormIdentifiers,
                    o => o.FormIdentifier,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .AddDictionary(
                    i => i.FormReceivers,
                    o => o.FormReceiver,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .AddNavigation(i => i.DeliveryAddress ?? new Address(), o => o.DeliveryAddress)
                .AddDictionaryList(
                    input => languageCache.FilterCollection(input.ChannelUrls, RequestLanguageCode).OrderBy(x=>x.OrderNumber).ThenBy(x=>x.Created),
                    output => output.FormFiles,
                    k => languageCache.GetByValue(k.LocalizationId)
                 )
                .GetFinal();
            return result;
        }

        public override PrintableFormChannel TranslateVmToEntity(VmPrintableFormOutput vModel)
        {
            throw new NotImplementedException();
        }
    }
}

